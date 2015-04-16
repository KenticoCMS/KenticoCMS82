using System;
using System.Net.Mail;

using CMS.EmailEngine;
using CMS.EventLog;
using CMS.Helpers;
using CMS.IO;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_System_System_Email : GlobalAdminPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Initialize required field validators
        rfvServer.ErrorMessage = GetString("System_Email.ErrorServer");
        revFrom.ErrorMessage = revTo.ErrorMessage = GetString("System_Email.ErrorEmail");
        rfvFrom.ErrorMessage = rfvTo.ErrorMessage = GetString("System_Email.EmptyEmail");
        revFrom.ValidationExpression = revTo.ValidationExpression = ValidationHelper.EmailRegExp.ToString();

        if (!RequestHelper.IsPostBack())
        {
            // Fill SMTP fields with the default server data
            if (SiteContext.CurrentSite != null)
            {
                string siteName = SiteContext.CurrentSiteName;
                txtServer.Text = EmailHelper.Settings.ServerName(siteName);
                txtUserName.Text = EmailHelper.Settings.ServerUserName(siteName);
                txtPassword.Text = EmailHelper.Settings.ServerPassword(siteName);
                txtPassword.Attributes.Add("value", txtPassword.Text);
                chkSSL.Checked = EmailHelper.Settings.ServerUseSSL(siteName);
            }
        }
    }


    /// <summary>
    /// Handles the Click event of the btnSend control.
    /// </summary>
    /// <param name="sender">The source of the event</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data</param>
    protected void btnSend_Click(object sender, EventArgs e)
    {
        txtFrom.Text = txtFrom.Text.Trim();
        txtTo.Text = txtTo.Text.Trim();
        txtServer.Text = txtServer.Text.Trim();

        string result = new Validator()
            .NotEmpty(txtServer.Text, GetString("System_Email.ErrorServer"))
            .NotEmpty(txtFrom.Text, GetString("System_Email.EmptyEmail"))
            .NotEmpty(txtTo.Text, GetString("System_Email.EmptyEmail"))
            .Result;

        if (!string.IsNullOrEmpty(result))
        {
            ShowError(result);
            return;
        }

        // Validate e-mail addresses
        if (!(ValidationHelper.IsEmail(txtFrom.Text) && ValidationHelper.IsEmail(txtTo.Text)))
        {
            ShowError(GetString("System_Email.ErrorEmail"));
            return;
        }

        // Send the testing e-mail
        try
        {
            SendEmail();
            ShowInformation(GetString("System_Email.EmailSent"));
        }
        catch (Exception ex)
        {
            string message = EventLogProvider.GetExceptionLogMessage(ex);
            ShowError(ex.Message, message, null);
        }
    }


    /// <summary>
    /// Sends a test e-mail message.
    /// </summary>
    protected void SendEmail()
    {
        // Initialize message
        EmailMessage email = new EmailMessage
                                 {
                                     From = txtFrom.Text,
                                     Recipients = txtTo.Text,
                                     Subject = TextHelper.LimitLength(txtSubject.Text.Trim(), 450),
                                     EmailFormat = EmailFormatEnum.Html,
                                     Body = txtText.Text
                                 };

        // Attach file if something was uploaded
        if ((FileUploader.PostedFile != null) && (FileUploader.PostedFile.InputStream != null))
        {
            email.Attachments.Add(new Attachment(FileUploader.PostedFile.InputStream,
                                                 Path.GetFileName(FileUploader.PostedFile.FileName)));
        }

        // Initialize SMTP server object
        SMTPServerInfo smtpServer = new SMTPServerInfo
                                        {
                                            ServerName = txtServer.Text,
                                            ServerUserName = txtUserName.Text,
                                            ServerPassword = txtPassword.Text,
                                            ServerUseSSL = chkSSL.Checked
                                        };

        string siteName = SiteContext.CurrentSiteName ?? string.Empty;

        EmailSender.SendTestEmail(siteName, email, smtpServer);
    }
}