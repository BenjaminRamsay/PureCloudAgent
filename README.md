# PureCloudAgent

**Disclaimer:** This is a very quick-and-dirty little winforms app. Modify and improve it for your purposes as you see fit.

### Initial PureCloud Setup Requirements
To use this code, you'll need to pre-configure some things in PureCloud:
* Create a dummy catchall user and enable its voicemail. This is the user voicemail you will send calls to in your Architect flows.
* In your Architect flows, anywhere that you are sending a call to voicemail first use "Set Participant Data" to assign an Attribute called "Group" to the call. The Value of the attribute should be the name of the group which you will use to identify it in the PureCloudAgent settings later.
* If you need to handle group faxes, make sure to assign the fax # DID to your dummy user.
* In Admin > Integrations > OAuth add an an OAuth client application called PureCloudAgent
  * Token Duration = 86400
  * Grant Type = Token Implicit Grant (Browser)
  * Authorized redirect URI = http://localhost:8085/oauth2/callback _(this is an arbitrary, fake URI. feel free to change it here and in code if desired)_

### PureCloudAgent Setup
Once your dummy user and call flows are set up correctly, you'll need to modify a few things in the PureCloudAgent project for your organization:
* In Project > Properties > Settings add Application level settings that pair your voicemail group names and fax numbers with the group email address you want them sent to.
  * Voicemail group names should be the same name you added as an Attribute in your Architect call flow.
  * Fax numbers should be prefixed with **n** and should contain no spaces, dashes, etc. (for example: n13175555555)
  * I left in a few example entries which you can modify and add to.
* At the top of MainForm.cs, fill in the constants with the appropriate values for your organization.
* Depending on your email system, you may need to modify the SendEmail() function to work with your server. Server name, port, and credentials can be specified using the constants at the top of MainForm.cs

Once those things are done you should be able to build and run. I leave mine running on a server all the time. It checks for new voicemails/faxes every 60 seconds (configurable in Settings), and sends them to the email addresses you specified in Settings.
