namespace PureCloudAgent
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.loginWaitTimer = new System.Windows.Forms.Timer(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.vmFaxButton = new System.Windows.Forms.Button();
            this.vmFaxCountdownLabel = new System.Windows.Forms.Label();
            this.oneSecondTimer = new System.Windows.Forms.Timer(this.components);
            this.logoutButton = new System.Windows.Forms.Button();
            this.authenticateButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(12, 37);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(372, 296);
            this.textBox1.TabIndex = 0;
            // 
            // webBrowser1
            // 
            this.webBrowser1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.webBrowser1.Location = new System.Drawing.Point(12, 372);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.Size = new System.Drawing.Size(371, 20);
            this.webBrowser1.TabIndex = 1;
            this.webBrowser1.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowser1_DocumentCompleted);
            this.webBrowser1.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.OnWebBrowserNavigating);
            // 
            // loginWaitTimer
            // 
            this.loginWaitTimer.Interval = 5000;
            this.loginWaitTimer.Tick += new System.EventHandler(this.loginWaitTimer_Tick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 353);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(514, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "This embedded web browser is used for authentication to PureCloud. Performed auto" +
    "matically via javascript.";
            // 
            // vmFaxButton
            // 
            this.vmFaxButton.Location = new System.Drawing.Point(12, 8);
            this.vmFaxButton.Name = "vmFaxButton";
            this.vmFaxButton.Size = new System.Drawing.Size(139, 23);
            this.vmFaxButton.TabIndex = 3;
            this.vmFaxButton.Text = "Check Voicemail and Fax";
            this.vmFaxButton.UseVisualStyleBackColor = true;
            this.vmFaxButton.Click += new System.EventHandler(this.vmFaxButton_Click);
            // 
            // vmFaxCountdownLabel
            // 
            this.vmFaxCountdownLabel.AutoSize = true;
            this.vmFaxCountdownLabel.Location = new System.Drawing.Point(153, 13);
            this.vmFaxCountdownLabel.Name = "vmFaxCountdownLabel";
            this.vmFaxCountdownLabel.Size = new System.Drawing.Size(72, 13);
            this.vmFaxCountdownLabel.TabIndex = 4;
            this.vmFaxCountdownLabel.Text = "<countdown>";
            // 
            // oneSecondTimer
            // 
            this.oneSecondTimer.Enabled = true;
            this.oneSecondTimer.Interval = 1000;
            this.oneSecondTimer.Tick += new System.EventHandler(this.oneSecondTimer_Tick);
            // 
            // logoutButton
            // 
            this.logoutButton.Location = new System.Drawing.Point(332, 8);
            this.logoutButton.Name = "logoutButton";
            this.logoutButton.Size = new System.Drawing.Size(52, 23);
            this.logoutButton.TabIndex = 5;
            this.logoutButton.Text = "Logout";
            this.logoutButton.UseVisualStyleBackColor = true;
            this.logoutButton.Click += new System.EventHandler(this.logoutButton_Click);
            // 
            // authenticateButton
            // 
            this.authenticateButton.Location = new System.Drawing.Point(245, 8);
            this.authenticateButton.Name = "authenticateButton";
            this.authenticateButton.Size = new System.Drawing.Size(81, 23);
            this.authenticateButton.TabIndex = 6;
            this.authenticateButton.Text = "Authenticate";
            this.authenticateButton.UseVisualStyleBackColor = true;
            this.authenticateButton.Click += new System.EventHandler(this.authenticateButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(395, 342);
            this.Controls.Add(this.authenticateButton);
            this.Controls.Add(this.logoutButton);
            this.Controls.Add(this.vmFaxCountdownLabel);
            this.Controls.Add(this.vmFaxButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.webBrowser1);
            this.Controls.Add(this.textBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "PureCloud Agent";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.WebBrowser webBrowser1;
        private System.Windows.Forms.Timer loginWaitTimer;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button vmFaxButton;
        private System.Windows.Forms.Label vmFaxCountdownLabel;
        private System.Windows.Forms.Timer oneSecondTimer;
        private System.Windows.Forms.Button logoutButton;
        private System.Windows.Forms.Button authenticateButton;
    }
}

