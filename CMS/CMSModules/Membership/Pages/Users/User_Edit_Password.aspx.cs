using System;
using System.Web.UI.WebControls;

using CMS.EmailEngine;
using CMS.EventLog;
using CMS.Helpers;
using CMS.Base;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.MacroEngine;
using CMS.DataEngine;

public partial class CMSModules_Membership_Pages_Users_User_Edit_Password : CMSUsersPage
{
    #region "Constants"

    private const string GENERATEPASSWORD = "generatepassword";
    private const string SENDPASSWORD = "sendpassword";

    #endregion


    #region "Private fields"

    private int mUserID;
    private UserInfo mUserInfo;

    #endregion


    #region "Private properties"

    /// <summary>
    /// Current user ID.
    /// </summary>
    private int UserID
    {
        get
        {
            if (mUserID == 0)
            {
                mUserID = QueryHelper.GetInteger("userid", 0);
            }

            return mUserID;
        }
    }


    /// <summary>
    /// Info object of currently edited user.
    /// </summary>
    private UserInfo UserInfo
    {
        get
        {
            return mUserInfo ?? (mUserInfo = UserInfoProvider.GetUserInfo(UserID));
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        UserInfo ui = UserInfo;

        EditedObject = ui;

        CheckUserAvaibleOnSite(ui);

        // Check that only global administrator can edit global administrator's accounts
        if (!CheckGlobalAdminEdit(ui))
        {
            plcTable.Visible = false;
            ShowError(GetString("Administration-User_List.ErrorGlobalAdmin"));
            return;
        }

        if (ui != null)
        {
            if (!ui.CheckPermissions(PermissionsEnum.Modify, SiteContext.CurrentSiteName, MembershipContext.AuthenticatedUser))
            {
                RedirectToAccessDenied(ui.TypeInfo.ModuleName, CMSAdminControl.PERMISSION_MODIFY);
            }

            passStrength.PlaceholderText = "mem.general.changepassword";

            // Hide warning panel after user extended validity of his own password
            if (ui.UserID == MembershipContext.AuthenticatedUser.UserID)
            {
                btnSetPassword.OnClientClick += "window.top.HideWarning()";
            }
        }

        HeaderActions.AddAction(new HeaderAction
        {
            Text = GetString("Administration-User_Edit_Password.gennew"),
            CommandName = GENERATEPASSWORD,
            OnClientClick = GetGeneratePasswordScript()
        });

        if (DisplaySendPassword())
        {
            HeaderActions.AddAction(new HeaderAction
            {
                Text = GetString("Administration-User_Edit_Password.sendpswd"),
                CommandName = SENDPASSWORD
            });
        }

        HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;
    }


    #region "Event handlers"

    private void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case GENERATEPASSWORD:
                GenerateNewPassword();
                break;

            case SENDPASSWORD:
                SendPassword();
                break;
        }
    }


    /// <summary>
    /// Generates new password and sends it to the user.
    /// </summary>
    private void GenerateNewPassword()
    {
        string pswd = UserInfoProvider.GenerateNewPassword(SiteContext.CurrentSiteName);
        string userName = UserInfoProvider.GetUserNameById(UserID);
        UserInfoProvider.SetPassword(userName, pswd);

        ShowChangesSaved();

        // Process e-mail sending
        SendEmail(GetString("Administration-User_Edit_Password.NewGen"), pswd, "changed", true);
    }


    /// <summary>
    /// Sends the actual password of the current user.
    /// </summary>
    private void SendPassword()
    {
        if (UserInfo != null)
        {
            string pswd = UserInfo.GetValue("UserPassword").ToString();

            // Process e-mail sending
            SendEmail(GetString("Administration-User_Edit_Password.Resend"), pswd, "RESEND", false);
        }
    }


    /// <summary>
    /// Sets password of current user.
    /// </summary>
    protected void btnSetPassword_Click(object sender, EventArgs e)
    {
        if (UserInfo == null)
        {
            return;
        }

        if (txtConfirmPassword.Text != passStrength.Text)
        {
            ShowError(GetString("Administration-User_Edit_Password.PasswordsDoNotMatch"));
            return;
        }

        if (!passStrength.IsValid())
        {
            ShowError(AuthenticationHelper.GetPolicyViolationMessage(SiteContext.CurrentSiteName));
            return;
        }

        // Password has been changed
        string password = passStrength.Text;
        UserInfoProvider.SetPassword(UserInfo, password);

        ShowChangesSaved();

        if (chkSendEmail.Checked)
        {
            // Process e-mail sending
            SendEmail(GetString("Administration-User_Edit_Password.Changed"), password, "CHANGED", false);
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Sends e-mail with password if required.
    /// </summary>
    /// <param name="subject">Subject of the e-mail sent</param>
    /// <param name="pswd">Password to send</param>
    /// <param name="emailType">Type of the e-mail specifying the template used (NEW, CHANGED, RESEND)</param>
    /// <param name="showPassword">Indicates whether password is shown in message.</param>
    private void SendEmail(string subject, string pswd, string emailType, bool showPassword)
    {
        // Check whether the 'From' element was specified
        string emailFrom = SettingsKeyInfoProvider.GetValue(SiteContext.CurrentSiteName + ".CMSSendPasswordEmailsFrom");
        bool fromMissing = string.IsNullOrEmpty(emailFrom);

        if ((!string.IsNullOrEmpty(emailType)) && (UserInfo != null) && (!fromMissing))
        {
            string emailTo = UserInfo.Email;
            if (!String.IsNullOrEmpty(emailTo))
            {
                EmailMessage em = new EmailMessage();

                em.From = emailFrom;
                em.Recipients = emailTo;
                em.Subject = subject;
                em.EmailFormat = EmailFormatEnum.Default;

                string templateName = null;

                // Get e-mail template name
                switch (emailType.ToLowerCSafe())
                {
                    case "new":
                        templateName = "Membership.NewPassword";
                        break;

                    case "changed":
                        templateName = "Membership.ChangedPassword";
                        break;

                    case "resend":
                        templateName = "Membership.ResendPassword";
                        break;
                }

                // Get template info object
                if (templateName != null)
                {
                    try
                    {
                        // Get e-mail template
                        EmailTemplateInfo template = EmailTemplateProvider.GetEmailTemplate(templateName, null);
                        if (template != null)
                        {
                            em.Body = template.TemplateText;

                            // Macros
                            string[,] macros = new string[2, 2];
                            macros[0, 0] = "UserName";
                            macros[0, 1] = UserInfo.UserName;
                            macros[1, 0] = "Password";
                            macros[1, 1] = pswd;

                            // Create macro resolver
                            MacroResolver resolver = MacroContext.CurrentResolver;
                            resolver.SetNamedSourceData(macros);

                            // Add template attachments
                            EmailHelper.ResolveMetaFileImages(em, template.TemplateID, EmailTemplateInfo.OBJECT_TYPE, ObjectAttachmentsCategories.TEMPLATE);

                            // Send message immediately (+ resolve macros)
                            EmailSender.SendEmailWithTemplateText(SiteContext.CurrentSiteName, em, template, resolver, true);

                            // Inform on success
                            ShowConfirmation(GetString("Administration-User_Edit_Password.PasswordsSent") + " " + HTMLHelper.HTMLEncode(emailTo));

                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log the error to the event log
                        EventLogProvider.LogException("Password retrieval", "USERPASSWORD", ex);
                        ShowError("Failed to send the password: " + ex.Message);
                    }
                }
            }
            else
            {
                // Inform on error
                if (showPassword)
                {
                    ShowConfirmation(String.Format(GetString("Administration-User_Edit_Password.passshow"), pswd), true);
                }
                else
                {
                    ShowConfirmation(GetString("Administration-User_Edit_Password.PassChangedNotSent"));
                }

                return;
            }
        }

        // Inform on error
        string errorMessage = GetString("Administration-User_Edit_Password.PasswordsNotSent");

        if (fromMissing)
        {
            errorMessage += " " + GetString("Administration-User_Edit_Password.FromMissing");
        }

        ShowError(errorMessage);
    }


    /// <summary>
    /// Indicates whether the 'Send password' button should be enabled or not.
    /// </summary>
    private bool DisplaySendPassword()
    {
        UserInfo ui = UserInfo;

        // Password is stored in plain text, allow sending
        return (ui != null) && String.IsNullOrEmpty(ui.UserPasswordFormat) && !String.IsNullOrEmpty(ui.Email);
    }


    /// <summary>
    /// Decides whether enable generate new password e-mail. 
    /// </summary>
    private string GetGeneratePasswordScript()
    {
        string clientClick = null;

        UserInfo ui = UserInfo;
        if (ui != null)
        {
            if (string.IsNullOrEmpty(ui.Email))
            {
                clientClick = "var flag = confirm('" + GetString("user.showpasswarning") + "');" + ((ui.UserID == MembershipContext.AuthenticatedUser.UserID) ? "if(flag) {window.top.HideWarning();}" : "") + "return flag;";
            }
            // Set hide action if user extend validity of his own account
            else if (ui.UserID == MembershipContext.AuthenticatedUser.UserID)
            {
                clientClick += "window.top.HideWarning()";
            }
        }

        return clientClick;
    }

    #endregion
}