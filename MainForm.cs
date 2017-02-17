using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Net;
using ININ.PureCloudApi.Api;
using ININ.PureCloudApi.Model;


namespace PureCloudAgent
{
    public partial class MainForm : Form
    {
        // *** FILL IN  YOUR INFO HERE ***
        private const string UserName = "";                             // YOUR DUMMY USERNAME
        private const string Password = "";                             // YOUR DUMMY USER PASSWORD
        private const string ClientId = "";                             // YOUR OAUTH CLIENT ID
        private const string OrgShortName = "";                         // YOUR ORG SHORT NAME (PureCloud > Admin > Organization Settings)
        private const string DefaultRecipientEmail = "";                // DEFAULT EMAIL ADDRESS TO RECEIVE UNASSIGNED VMs/FAXES
        private const string EmailFromAddress = "";                     // FROM ADDRESS FOR VM/FAX EMAILS
        private const string EmailServer = "";                          // YOUR EMAIL SERVER
        private const int EmailPort = 587;                              // EMAIL SERVER PORT
        private const string EmailServerUsername = "";                  // EMAIL SERVER USERNAME
        private const string EmailServerPassword = "";                  // EMAIL SERVER PASSWORD
        // *******************************

        private readonly string nl = System.Environment.NewLine;
        private int vmFaxCountdown;
        private string user;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Log("Starting");
            
            String[] arguments = Environment.GetCommandLineArgs();
            user = (arguments.Length >= 2) ? arguments[1] : UserName;
            this.Text += " - " + user;            

            vmFaxCountdown = Properties.Settings.Default.vmFaxInterval;
            vmFaxCountdownLabel.Text = vmFaxCountdown.ToString();

            Authenticate();            
        }

        private void Authenticate()
        {
            Log("Authenticating");

            this.WindowState = FormWindowState.Normal;

            //Redirect the browser to the login window.
            webBrowser1.Url = new Uri("https://login.mypurecloud.com/oauth/authorize?" +
                                            "response_type=token" +
                                            "&client_id=" + ClientId +
                                            "&redirect_uri=http://localhost:8085/oauth2/callback");
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            var url = e.Url.ToString();

            if (url.Contains("https://login.mypurecloud.com/?rid") &&
                url.Contains("/authenticate-adv/org/" + OrgShortName))
            {
                loginWaitTimer.Start();
            }
        }

        private void loginWaitTimer_Tick(object sender, EventArgs e)
        {
            loginWaitTimer.Stop();

            AutoLogin();
        }

        private void AutoLogin()
        {
            string javaScript = @"
            function submitLogin()
            {
                document.getElementById('email').value = '" + user + @"';
                document.getElementById('password').value = '" + Password + @"';
                document.getElementById('password').focus();
                document.getElementById('password').blur();
                document.getElementsByTagName('button')[0].click();
            }
                ";

            HtmlDocument doc = webBrowser1.Document;
            HtmlElement head = doc.GetElementsByTagName("head")[0];
            HtmlElement s = doc.CreateElement("script");
            s.SetAttribute("text", javaScript);
            head.AppendChild(s);
            webBrowser1.Document.InvokeScript("submitLogin");
        }

        private void OnWebBrowserNavigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            //When the web browser control navigates, check if the url is the callback and contains the access_token
            var url = e.Url.ToString();
            if (url.Contains("access_token"))
            {
                var queryString = url.Substring(url.LastIndexOf("#") + 1);
                var props = queryString.Split('&');
                string accessToken = props[0].Split('=')[1];

                ININ.PureCloudApi.Client.Configuration.Default.AccessToken = accessToken;
                Log("Successfully authenticated");                
            }
        }

        private void CheckVoicemailAndFax()
        {
            vmFaxCountdown = Properties.Settings.Default.vmFaxInterval;

            try
            {
                ProcessVoicemail();

                ProcessFax();                
            }
            catch (ININ.PureCloudApi.Client.ApiException ex)
            {
                if (ex.ErrorCode == 401)
                {
                    Log("Need to authenticate first. Try again in a few seconds.");
                    Authenticate();
                }
                else
                {
                    Log("Error: " + ex.Message);
                }                
            }            
        }

        private void ProcessVoicemail()
        {
            // Check Voicemail
            var vmApi = new VoicemailApi();
            var convApi = new ConversationsApi();
            var vms = vmApi.GetMessages();
            VoicemailMediaInfo vmMedia;
            foreach (VoicemailMessage vm in vms.Entities.Where(x => !(x.Read ?? false)))        // Unread VMs only
            {
                DateTime vmDate = vm.CreatedDate != null ? ((DateTime)vm.CreatedDate).ToLocalTime() : DateTime.Now;

                Log("VM from " + vm.CallerAddress + " at " + vmDate);

                // Check for Group attribute so we know where to send it (default to IT if nothing found)                    
                string group = "unknown";
                string groupEmail = DefaultRecipientEmail;
                var call = convApi.GetCallsCallId(vm.Conversation.Id);

                CallMediaParticipant participant = null;
                try
                {
                    participant = call.Participants.First(x => x.Attributes != null && x.Attributes.ContainsKey("Group"));
                }
                catch { }
                
                if (participant != null)
                {
                    group = participant.Attributes["Group"];

                    // Try to look up the email for this group in Settings. If it's not found, just move on with the default
                    try
                    {
                        groupEmail = Properties.Settings.Default[group].ToString();
                    }
                    catch { }
                }
                else
                {
                    participant = call.Participants[0];
                }

                // Download the WAV file
                string fileName;
                vmMedia = vmApi.GetMessagesMessageIdMedia(vm.Id, "WAV");
                using (var client = new WebClient())
                {
                    fileName = Path.GetTempPath() +
                        "PureCloud_VM_" + group + "_" +
                        vmDate.ToString("yyyyMMdd-HHmmss") + ".wav";

                    client.DownloadFile(vmMedia.MediaFileUri, fileName);
                }

                // Email to the proper group
                string callerName = (vm.CallerName ?? participant.Name ?? vm.CallerAddress);
                string subject = "Voicemail from " + callerName;
                string body = ComposeVoicemailEmail(vmDate, group, vm.CallerAddress, callerName);
                SendEmail(groupEmail, EmailFromAddress, body, subject, fileName);
                Log("    Sent to " + group + ": " + groupEmail);

                // Mark the VM as read so it won't get sent again next time
                vm.Read = true;
                vmApi.PutMessagesMessageId(vm.Id, vm);
            }
        }

        private void ProcessFax()
        {
            // Check Fax
            var faxApi = new FaxApi();
            var faxes = faxApi.GetDocuments();
            foreach (FaxDocument fax in faxes.Entities.Where(x => !(x.Read ?? false)))     
            {
                DateTime faxDate = fax.DateCreated != null ? ((DateTime)fax.DateCreated).ToLocalTime() : DateTime.Now;
                string fromNumber = fax.CallerAddress.Replace("tel:+", "");
                string toNumber = fax.ReceiverAddress.Replace("tel:+", "");

                Log("FAX from " + fax.CallerAddress + " at " + faxDate);

                // Try to look up the email for this fax # in Settings. If it's not found, just move on with the default
                string groupEmail = DefaultRecipientEmail;                
                try
                {
                    groupEmail = Properties.Settings.Default["n"+toNumber].ToString();
                }
                catch { }

                // Download the fax file
                string fileName;
                DownloadResponse faxFile = faxApi.GetDocumentsDocumentIdContent(fax.Id);
                using (var client = new WebClient())
                {
                    // Replace any characters not allowed in a file name
                    string fromNumberSafeString = fromNumber
                        .Replace(@"\", "")
                        .Replace(@"/", "")
                        .Replace(@":", "")
                        .Replace(@"*", "")
                        .Replace(@"?", "")
                        .Replace(@"""", "")
                        .Replace(@"<", "")
                        .Replace(@">", "")
                        .Replace(@"|", "")
                        .Replace(@" ", "");

                    fileName = Path.GetTempPath() +
                        "PureCloud_FAX_" + fromNumberSafeString + "_" +
                        faxDate.ToString("yyyyMMdd-HHmmss") + ".pdf";

                    client.DownloadFile(faxFile.ContentLocationUri, fileName);
                }

                // Email to the proper group
                string subject = "Fax from " + fromNumber;
                string body = ComposeFaxEmail(faxDate, fromNumber, toNumber);
                SendEmail(groupEmail, EmailFromAddress, body, subject, fileName);
                Log("    Sent to " + toNumber + ": " + groupEmail);

                // Mark the fax as read so it won't get sent again next time
                fax.Read = true;
                faxApi.PutDocumentsDocumentId(fax.Id, fax);
            }
        }

        private void Logout()
        {
            // Log Out
            webBrowser1.Url = new Uri("https://login.mypurecloud.com/logout");
        }

        private void oneSecondTimer_Tick(object sender, EventArgs e)
        {
            vmFaxCountdown--;
            vmFaxCountdownLabel.Text = vmFaxCountdown.ToString();

            if (vmFaxCountdown <= 0)
            {                
                CheckVoicemailAndFax();
            }
        }

        private void vmFaxButton_Click(object sender, EventArgs e)
        {
            CheckVoicemailAndFax();
        }

        private void Log(string message)
        {
            textBox1.AppendText(message + nl);
            textBox1.Select(textBox1.TextLength - 1, 0);
        }

        private void SendEmail(string strTo, string strFrom, string strMessage, string strSubject, string attachmentPath, bool htmlFormat = true)
        {
            System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
            System.Net.Mail.MailAddress mto = new System.Net.Mail.MailAddress(strTo);
            System.Net.Mail.MailAddress mfrom = new System.Net.Mail.MailAddress(strFrom);
            mail.Subject = strSubject;
            mail.From = mfrom;
            mail.To.Add(mto);
            mail.Body = strMessage;
            mail.IsBodyHtml = htmlFormat;
            mail.Attachments.Add(new System.Net.Mail.Attachment(attachmentPath));

            System.Net.Mail.SmtpClient mailClient = new System.Net.Mail.SmtpClient(EmailServer, EmailPort);
            mailClient.Timeout = 1000000;
            mailClient.EnableSsl = true;
            mailClient.UseDefaultCredentials = false;
            mailClient.Credentials = new System.Net.NetworkCredential(EmailServerUsername, EmailServerPassword);
            mailClient.Send(mail);
        }

        private string ComposeVoicemailEmail(DateTime vmDate, string group, string callerNumber, string callerName)
        {
            string body = @"
            <span style='font-weight:bold'>Date/Time:</span> " + vmDate.ToString("MM/dd/yy HH:mm") + @"<br />
            <span style='font-weight:bold'>Group:</span> " + group + @"<br />
            <span style='font-weight:bold'>Caller Name:</span> " + callerName + @"<br />            
            <span style='font-weight:bold'>Caller #:</span> " + callerNumber + @"<br />            
            ";

            return body;
        }

        private string ComposeFaxEmail(DateTime faxDate, string fromNumber, string toNumber)
        {
            string body = @"
            <span style='font-weight:bold'>Date/Time:</span> " + faxDate.ToString("MM/dd/yy HH:mm") + @"<br />            
            <span style='font-weight:bold'>From:</span> " + fromNumber + @"<br />        
            <br />
            <span style='font-size:0.8em'><span style='font-weight:bold'>Sent to:</span> " + toNumber + @"</span>
            ";

            return body;
        }

        private void logoutButton_Click(object sender, EventArgs e)
        {
            Logout();
        }

        private void authenticateButton_Click(object sender, EventArgs e)
        {
            Authenticate();
        }
    }
}