using System;
using System.Web;

using CMS.Base;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.EmailEngine;
using CMS.EventLog;
using CMS.FormControls;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.Localization;
using CMS.MacroEngine;
using CMS.Membership;
using CMS.PortalControls;
using CMS.PortalEngine;
using CMS.Protection;
using CMS.SiteProvider;
using CMS.WebAnalytics;

public partial class CMSWebParts_Membership_Registration_CustomRegistrationForm : CMSAbstractWebPart
{
    #region "Layout properties"

    /// <summary>
    /// Full alternative form name ('classname.formname') for usersettingsinfo.
    /// Default value is cms.user.RegistrationForm
    /// </summary>
    public string AlternativeForm
    {
        get
        {
            return ValidationHelper.GetString(GetValue("AlternativeForm"), "cms.user.RegistrationForm");
        }
        set
        {
            SetValue("AlternativeForm", value);
            formUser.AlternativeFormFullName = value;
        }
    }

    #endregion


    #region "Text properties"

    /// <summary>
    /// Gets or sets submit button text.
    /// </summary>
    public string ButtonText
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("ButtonText"), ResHelper.LocalizeString("{$Webparts_Membership_RegistrationForm.Button$}"));
        }

        set
        {
            SetValue("ButtonText", value);
            btnRegister.Text = value;
        }
    }


    /// <summary>
    /// Gets or sets registration approval page URL.
    /// </summary>
    public string ApprovalPage
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("ApprovalPage"), String.Empty);
        }
        set
        {
            SetValue("ApprovalPage", value);
        }
    }

    #endregion


    #region "Registration properties"

    /// <summary>
    /// Gets or sets the value that indicates whether email to user should be sent.
    /// </summary>
    public bool SendWelcomeEmail
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("SendWelcomeEmail"), true);
        }
        set
        {
            SetValue("SendWelcomeEmail", value);
        }
    }


    /// <summary>
    /// Determines whether the captcha image should be displayed.
    /// </summary>
    public bool DisplayCaptcha
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayCaptcha"), false);
        }

        set
        {
            SetValue("DisplayCaptcha", value);
        }
    }


    /// <summary>
    /// Gets or sets the message which is displayed after registration failed.
    /// </summary>
    public string RegistrationErrorMessage
    {
        get
        {
            return ValidationHelper.GetString(GetValue("RegistrationErrorMessage"), String.Empty);
        }
        set
        {
            SetValue("RegistrationErrorMessage", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether user is enabled after registration.
    /// </summary>
    public bool EnableUserAfterRegistration
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("EnableUserAfterRegistration"), true);
        }
        set
        {
            SetValue("EnableUserAfterRegistration", value);
        }
    }


    /// <summary>
    /// Gets or sets the sender email (from).
    /// </summary>
    public string FromAddress
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("FromAddress"), SettingsKeyInfoProvider.GetValue(SiteContext.CurrentSiteName + ".CMSNoreplyEmailAddress"));
        }
        set
        {
            SetValue("FromAddress", value);
        }
    }


    /// <summary>
    /// Gets or sets the recipient email (to).
    /// </summary>
    public string ToAddress
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("ToAddress"), SettingsKeyInfoProvider.GetValue(SiteContext.CurrentSiteName + ".CMSAdminEmailAddress"));
        }
        set
        {
            SetValue("ToAddress", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether after successful registration is 
    /// notification email sent to the administrator 
    /// </summary>
    public bool NotifyAdministrator
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("NotifyAdministrator"), false);
        }
        set
        {
            SetValue("NotifyAdministrator", value);
        }
    }


    /// <summary>
    /// Gets or sets the roles where is user assigned after successful registration.
    /// </summary>
    public string AssignRoles
    {
        get
        {
            return ValidationHelper.GetString(GetValue("AssignToRoles"), String.Empty);
        }
        set
        {
            SetValue("AssignToRoles", value);
        }
    }


    /// <summary>
    /// Gets or sets the sites where is user assigned after successful registration.
    /// </summary>
    public string AssignToSites
    {
        get
        {
            return ValidationHelper.GetString(GetValue("AssignToSites"), String.Empty);
        }
        set
        {
            SetValue("AssignToSites", value);
        }
    }


    /// <summary>
    /// Gets or sets the message which is displayed after successful registration.
    /// </summary>
    public string DisplayMessage
    {
        get
        {
            return ValidationHelper.GetString(GetValue("DisplayMessage"), String.Empty);
        }
        set
        {
            SetValue("DisplayMessage", value);
        }
    }


    /// <summary>
    /// Gets or set the url where is user redirected after successful registration.
    /// </summary>
    public string RedirectToURL
    {
        get
        {
            return ValidationHelper.GetString(GetValue("RedirectToURL"), String.Empty);
        }
        set
        {
            SetValue("RedirectToURL", value);
        }
    }


    /// <summary>
    /// Gets or sets the default starting alias path for newly registered user.
    /// </summary>
    public string StartingAliasPath
    {
        get
        {
            return ValidationHelper.GetString(GetValue("StartingAliasPath"), String.Empty);
        }
        set
        {
            SetValue("StartingAliasPath", value);
        }
    }

    #endregion


    #region "Conversion properties"

    /// <summary>
    /// Gets or sets the conversion track name used after successful registration.
    /// </summary>
    public string TrackConversionName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("TrackConversionName"), String.Empty);
        }
        set
        {
            if ((value != null) && (value.Length > 400))
            {
                value = value.Substring(0, 400);
            }
            SetValue("TrackConversionName", value);
        }
    }


    /// <summary>
    /// Gets or sets the conversion value used after successful registration.
    /// </summary>
    public double ConversionValue
    {
        get
        {
            return ValidationHelper.GetDoubleSystem(GetValue("ConversionValue"), 0);
        }
        set
        {
            SetValue("ConversionValue", value);
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Reloads data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
    }


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            // Do nothing
        }
        else
        {
            // Set default visibility
            pnlRegForm.Visible = true;
            lblInfo.Visible = false;

            // WAI validation
            lblCaptcha.AssociatedControlClientID = captchaElem.InputClientID;

            // Get alternative form info
            AlternativeFormInfo afi = AlternativeFormInfoProvider.GetAlternativeFormInfo(AlternativeForm);
            if (afi != null)
            {
                formUser.AlternativeFormFullName = AlternativeForm;
                formUser.Info = new UserInfo();
                formUser.Info.ClearData();
                formUser.ClearAfterSave = false;
                formUser.Visible = true;
                formUser.ValidationErrorMessage = RegistrationErrorMessage;
                formUser.IsLiveSite = true;
                // Reload form if not in PortalEngine environment and if post back
                if ((StandAlone) && (RequestHelper.IsPostBack()))
                {
                    formUser.ReloadData();
                }

                captchaElem.Visible = DisplayCaptcha;
                lblCaptcha.Visible = DisplayCaptcha;
                plcCaptcha.Visible = DisplayCaptcha;

                btnRegister.Text = ButtonText;
                btnRegister.Click += btnRegister_Click;

                lblInfo.CssClass = "EditingFormInfoLabel";
                lblError.CssClass = "EditingFormErrorLabel";

                if (formUser != null)
                {
                    // Set the live site context
                    formUser.ControlContext.ContextName = CMS.ExtendedControls.ControlContext.LIVE_SITE;
                }
            }
            else
            {
                lblError.Text = String.Format(GetString("altform.formdoesntexists"), AlternativeForm);
                lblError.Visible = true;
                pnlRegForm.Visible = false;
            }
        }
    }


    /// <summary>
    /// Page pre-render event handler.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        // Hide default form submit button
        if (formUser != null)
        {
            formUser.SubmitButton.Visible = false;
        }
    }



    /// <summary>
    /// OK click handler (Proceed registration).
    /// </summary>
    private void btnRegister_Click(object sender, EventArgs e)
    {
        string currentSiteName = SiteContext.CurrentSiteName;
        string[] siteList = { currentSiteName };

        // If AssignToSites field set
        if (!String.IsNullOrEmpty(AssignToSites))
        {
            siteList = AssignToSites.Split(';');
        }

        if ((PageManager.ViewMode == ViewModeEnum.Design) || (HideOnCurrentPage) || (!IsVisible))
        {
            // Do not process
        }
        else
        {
            // Ban IP addresses which are blocked for registration
            if (!BannedIPInfoProvider.IsAllowed(currentSiteName, BanControlEnum.Registration))
            {
                lblError.Visible = true;
                lblError.Text = GetString("banip.ipisbannedregistration");
                return;
            }

            // Check if captcha is required and verify captcha text
            if (DisplayCaptcha && !captchaElem.IsValid())
            {
                // Display error message if captcha text is not valid
                lblError.Visible = true;
                lblError.Text = GetString("Webparts_Membership_RegistrationForm.captchaError");
                return;
            }

            string userName = String.Empty;
            string nickName = String.Empty;
            string firstName = String.Empty;
            string lastName = String.Empty;
            string emailValue = String.Empty;

            // Check duplicate user
            // 1. Find appropriate control and get its value (i.e. user name)
            // 2. Try to find user info
            FormEngineUserControl txtUserName = formUser.FieldControls["UserName"];
            if (txtUserName != null)
            {
                userName = ValidationHelper.GetString(txtUserName.Value, String.Empty);
            }

            FormEngineUserControl txtEmail = formUser.FieldControls["Email"];
            if (txtEmail != null)
            {
                emailValue = ValidationHelper.GetString(txtEmail.Value, String.Empty);
            }

            // If user name and e-mail aren't filled stop processing and display error.
            if (string.IsNullOrEmpty(userName))
            {
                userName = emailValue;
                if (String.IsNullOrEmpty(emailValue))
                {
                    formUser.StopProcessing = true;
                    lblError.Visible = true;
                    lblError.Text = GetString("customregistrationform.usernameandemail");
                    return;
                }
                else
                {
                    formUser.Data.SetValue("UserName", userName);
                }
            }

            FormEngineUserControl txtNickName = formUser.FieldControls["UserNickName"];
            if (txtNickName != null)
            {
                nickName = ValidationHelper.GetString(txtNickName.Value, String.Empty);
            }

            FormEngineUserControl txtFirstName = formUser.FieldControls["FirstName"];
            if (txtFirstName != null)
            {
                firstName = ValidationHelper.GetString(txtFirstName.Value, String.Empty);
            }

            FormEngineUserControl txtLastName = formUser.FieldControls["LastName"];
            if (txtLastName != null)
            {
                lastName = ValidationHelper.GetString(txtLastName.Value, String.Empty);
            }

            // Test if "global" or "site" user exists. 
            SiteInfo si = SiteContext.CurrentSite;
            UserInfo siteui = UserInfoProvider.GetUserInfo(UserInfoProvider.EnsureSitePrefixUserName(userName, si));
            if ((UserInfoProvider.GetUserInfo(userName) != null) || (siteui != null))
            {
                lblError.Visible = true;
                lblError.Text = GetString("Webparts_Membership_RegistrationForm.UserAlreadyExists").Replace("%%name%%", HTMLHelper.HTMLEncode(Functions.GetFormattedUserName(userName, true)));
                return;
            }

            // Check for reserved user names like administrator, sysadmin, ...
            if (UserInfoProvider.NameIsReserved(currentSiteName, userName))
            {
                lblError.Visible = true;
                lblError.Text = GetString("Webparts_Membership_RegistrationForm.UserNameReserved").Replace("%%name%%", HTMLHelper.HTMLEncode(Functions.GetFormattedUserName(userName, true)));
                return;
            }

            if (UserInfoProvider.NameIsReserved(currentSiteName, nickName))
            {
                lblError.Visible = true;
                lblError.Text = GetString("Webparts_Membership_RegistrationForm.UserNameReserved").Replace("%%name%%", HTMLHelper.HTMLEncode(nickName));
                return;
            }

            // Check limitations for site members
            if (!UserInfoProvider.LicenseVersionCheck(RequestContext.CurrentDomain, FeatureEnum.SiteMembers, ObjectActionEnum.Insert, false))
            {
                lblError.Visible = true;
                lblError.Text = GetString("License.MaxItemsReachedSiteMember");
                return;
            }

            // Check whether email is unique if it is required
            if (!UserInfoProvider.IsEmailUnique(emailValue, siteList, 0))
            {
                lblError.Visible = true;
                lblError.Text = GetString("UserInfo.EmailAlreadyExist");
                return;
            }

            // Validate and save form with new user data
            if (!formUser.Save())
            {
                // Return if saving failed
                return;
            }

            // Get user info from form
            UserInfo ui = (UserInfo)formUser.Info;

            // Add user prefix if settings is on
            // Ensure site prefixes
            if (UserInfoProvider.UserNameSitePrefixEnabled(currentSiteName))
            {
                ui.UserName = UserInfoProvider.EnsureSitePrefixUserName(userName, si);
            }

            ui.Enabled = EnableUserAfterRegistration;
            ui.UserURLReferrer = MembershipContext.AuthenticatedUser.URLReferrer;
            ui.UserCampaign = AnalyticsHelper.Campaign;

            ui.SetPrivilegeLevel(UserPrivilegeLevelEnum.None);

            // Fill optionally full user name
            if (String.IsNullOrEmpty(ui.FullName))
            {
                ui.FullName = UserInfoProvider.GetFullName(ui.FirstName, ui.MiddleName, ui.LastName);
            }

            // Ensure nick name
            if (ui.UserNickName.Trim() == String.Empty)
            {
                ui.UserNickName = Functions.GetFormattedUserName(ui.UserName, true);
            }

            ui.UserSettings.UserRegistrationInfo.IPAddress = RequestContext.UserHostAddress;
            ui.UserSettings.UserRegistrationInfo.Agent = HttpContext.Current.Request.UserAgent;
            ui.UserSettings.UserLogActivities = true;
            ui.UserSettings.UserShowIntroductionTile = true;

            // Check whether confirmation is required
            bool requiresConfirmation = SettingsKeyInfoProvider.GetBoolValue(currentSiteName + ".CMSRegistrationEmailConfirmation");
            bool requiresAdminApprove = SettingsKeyInfoProvider.GetBoolValue(currentSiteName + ".CMSRegistrationAdministratorApproval");
            if (!requiresConfirmation)
            {
                // If confirmation is not required check whether administration approval is reqiures
                if (requiresAdminApprove)
                {
                    ui.Enabled = false;
                    ui.UserSettings.UserWaitingForApproval = true;
                }
            }
            else
            {
                // EnableUserAfterRegistration is overrided by requiresConfirmation - user needs to be confirmed before enable
                ui.Enabled = false;
            }

            // Set user's starting alias path
            if (!String.IsNullOrEmpty(StartingAliasPath))
            {
                ui.UserStartingAliasPath = MacroResolver.ResolveCurrentPath(StartingAliasPath);
            }

            // Get user password and save it in apropriate format after form save
            string password = ValidationHelper.GetString(ui.GetValue("UserPassword"), String.Empty);
            UserInfoProvider.SetPassword(ui, password);


            // Prepare macro data source for email resolver
            UserInfo userForMail = ui.Clone();
            userForMail.SetValue("UserPassword", string.Empty);

            object[] data = new object[1];
            data[0] = userForMail;

            // Prepare resolver for notification and welcome emails
            MacroResolver resolver = MacroContext.CurrentResolver;
            resolver.SetAnonymousSourceData(data);

            #region "Welcome Emails (confirmation, waiting for approval)"

            bool error = false;
            EmailTemplateInfo template = null;

            // Prepare macro replacements
            string[,] replacements = new string[6, 2];
            replacements[0, 0] = "confirmaddress";
            replacements[0, 1] = AuthenticationHelper.GetRegistrationApprovalUrl(ApprovalPage, ui.UserGUID, currentSiteName, NotifyAdministrator);
            replacements[1, 0] = "username";
            replacements[1, 1] = userName;
            replacements[2, 0] = "password";
            replacements[2, 1] = password;
            replacements[3, 0] = "Email";
            replacements[3, 1] = emailValue;
            replacements[4, 0] = "FirstName";
            replacements[4, 1] = firstName;
            replacements[5, 0] = "LastName";
            replacements[5, 1] = lastName;

            // Set resolver
            resolver.SetNamedSourceData(replacements);

            // Email message
            EmailMessage emailMessage = new EmailMessage();
            emailMessage.EmailFormat = EmailFormatEnum.Default;
            emailMessage.Recipients = ui.Email;

            // Send welcome message with username and password, with confirmation link, user must confirm registration
            if (requiresConfirmation)
            {
                template = EmailTemplateProvider.GetEmailTemplate("RegistrationConfirmation", currentSiteName);
                emailMessage.Subject = GetString("RegistrationForm.RegistrationConfirmationEmailSubject");
            }
            // Send welcome message with username and password, with information that user must be approved by administrator
            else if (SendWelcomeEmail)
            {
                if (requiresAdminApprove)
                {
                    template = EmailTemplateProvider.GetEmailTemplate("Membership.RegistrationWaitingForApproval", currentSiteName);
                    emailMessage.Subject = GetString("RegistrationForm.RegistrationWaitingForApprovalSubject");
                }
                // Send welcome message with username and password, user can logon directly
                else
                {
                    template = EmailTemplateProvider.GetEmailTemplate("Membership.Registration", currentSiteName);
                    emailMessage.Subject = GetString("RegistrationForm.RegistrationSubject");
                }
            }

            if (template != null)
            {
                emailMessage.From = EmailHelper.GetSender(template, SettingsKeyInfoProvider.GetValue(currentSiteName + ".CMSNoreplyEmailAddress"));
                // Enable macro encoding for body
                resolver.Settings.EncodeResolvedValues = true;
                emailMessage.Body = resolver.ResolveMacros(template.TemplateText);
                // Disable macro encoding for plaintext body and subject
                resolver.Settings.EncodeResolvedValues = false;
                emailMessage.PlainTextBody = resolver.ResolveMacros(template.TemplatePlainText);
                emailMessage.Subject = resolver.ResolveMacros(EmailHelper.GetSubject(template, emailMessage.Subject));

                emailMessage.CcRecipients = template.TemplateCc;
                emailMessage.BccRecipients = template.TemplateBcc;

                try
                {
                    EmailHelper.ResolveMetaFileImages(emailMessage, template.TemplateID, EmailTemplateInfo.OBJECT_TYPE, ObjectAttachmentsCategories.TEMPLATE);
                    // Send the e-mail immediately
                    EmailSender.SendEmail(currentSiteName, emailMessage, true);
                }
                catch (Exception ex)
                {
                    EventLogProvider.LogException("E", "RegistrationForm - SendEmail", ex);
                    error = true;
                }
            }

            // If there was some error, user must be deleted
            if (error)
            {
                lblError.Visible = true;
                lblError.Text = GetString("RegistrationForm.UserWasNotCreated");

                // Email was not send, user can't be approved - delete it
                UserInfoProvider.DeleteUser(ui);
                return;
            }

            #endregion


            #region "Administrator notification email"

            // Notify administrator if enabled and email confirmation is not required
            if (!requiresConfirmation && NotifyAdministrator && (FromAddress != String.Empty) && (ToAddress != String.Empty))
            {
                EmailTemplateInfo mEmailTemplate = null;

                if (requiresAdminApprove)
                {
                    mEmailTemplate = EmailTemplateProvider.GetEmailTemplate("Registration.Approve", currentSiteName);
                }
                else
                {
                    mEmailTemplate = EmailTemplateProvider.GetEmailTemplate("Registration.New", currentSiteName);
                }

                if (mEmailTemplate == null)
                {
                    EventLogProvider.LogEvent(EventType.ERROR, "RegistrationForm", "GetEmailTemplate", eventUrl: RequestContext.RawURL);
                }
                else
                {
                    // E-mail template ok
                    replacements = new string[4, 2];
                    replacements[0, 0] = "firstname";
                    replacements[0, 1] = ui.FirstName;
                    replacements[1, 0] = "lastname";
                    replacements[1, 1] = ui.LastName;
                    replacements[2, 0] = "email";
                    replacements[2, 1] = ui.Email;
                    replacements[3, 0] = "username";
                    replacements[3, 1] = userName;

                    // Set resolver
                    resolver.SetNamedSourceData(replacements);
                    // Enable macro encoding for body
                    resolver.Settings.EncodeResolvedValues = true;

                    EmailMessage message = new EmailMessage();
                    message.EmailFormat = EmailFormatEnum.Default;
                    message.From = EmailHelper.GetSender(mEmailTemplate, FromAddress);
                    message.Recipients = ToAddress;
                    message.Body = resolver.ResolveMacros(mEmailTemplate.TemplateText);
                    // Disable macro encoding for plaintext body and subject
                    resolver.Settings.EncodeResolvedValues = false;
                    message.Subject = resolver.ResolveMacros(EmailHelper.GetSubject(mEmailTemplate, GetString("RegistrationForm.EmailSubject")));
                    message.PlainTextBody = resolver.ResolveMacros(mEmailTemplate.TemplatePlainText);

                    message.CcRecipients = mEmailTemplate.TemplateCc;
                    message.BccRecipients = mEmailTemplate.TemplateBcc;

                    try
                    {
                        // Attach template meta-files to e-mail
                        EmailHelper.ResolveMetaFileImages(message, mEmailTemplate.TemplateID, EmailTemplateInfo.OBJECT_TYPE, ObjectAttachmentsCategories.TEMPLATE);
                        EmailSender.SendEmail(currentSiteName, message);
                    }
                    catch
                    {
                        EventLogProvider.LogEvent(EventType.ERROR, "Membership", "RegistrationEmail");
                    }
                }
            }

            #endregion


            #region "Web analytics"

            // Track successful registration conversion
            if (TrackConversionName != String.Empty)
            {
                if (AnalyticsHelper.AnalyticsEnabled(currentSiteName) && AnalyticsHelper.TrackConversionsEnabled(currentSiteName) && !AnalyticsHelper.IsIPExcluded(currentSiteName, RequestContext.UserHostAddress))
                {
                    HitLogProvider.LogConversions(currentSiteName, LocalizationContext.PreferredCultureCode, TrackConversionName, 0, ConversionValue);
                }
            }

            // Log registered user if confirmation is not required
            if (!requiresConfirmation)
            {
                AnalyticsHelper.LogRegisteredUser(currentSiteName, ui);
            }

            #endregion


            #region "On-line marketing - activity"

            // Log registered user if confirmation is not required
            if (!requiresConfirmation)
            {
                Activity activity = new ActivityRegistration(ui, DocumentContext.CurrentDocument, AnalyticsContext.ActivityEnvironmentVariables);
                if (activity.Data != null)
                {
                    activity.Data.ContactID = ModuleCommands.OnlineMarketingGetUserLoginContactID(ui);
                    activity.Log();
                }

                // Log login activity
                if (ui.Enabled)
                {
                    // Log activity
                    int contactID = ModuleCommands.OnlineMarketingGetUserLoginContactID(ui);
                    Activity activityLogin = new ActivityUserLogin(contactID, ui, DocumentContext.CurrentDocument, AnalyticsContext.ActivityEnvironmentVariables);
                    activityLogin.Log();
                }
            }

            #endregion


            #region "Site and roles addition and authentication"

            string[] roleList = AssignRoles.Split(';');

            foreach (string siteName in siteList)
            {
                // Add new user to the current site
                UserInfoProvider.AddUserToSite(ui.UserName, siteName);
                foreach (string roleName in roleList)
                {
                    if (!String.IsNullOrEmpty(roleName))
                    {
                        String sn = roleName.StartsWithCSafe(".") ? String.Empty : siteName;

                        // Add user to desired roles
                        if (RoleInfoProvider.RoleExists(roleName, sn))
                        {
                            UserInfoProvider.AddUserToRole(ui.UserName, roleName, sn);
                        }
                    }
                }
            }

            if (DisplayMessage.Trim() != String.Empty)
            {
                pnlRegForm.Visible = false;
                lblInfo.Visible = true;
                lblInfo.Text = DisplayMessage;
            }
            else
            {
                if (ui.Enabled)
                {
                    AuthenticationHelper.AuthenticateUser(ui.UserName, true);
                }

                string returnUrl = QueryHelper.GetString("ReturnURL", String.Empty);
                if (!String.IsNullOrEmpty(returnUrl) && (returnUrl.StartsWithCSafe("~") || returnUrl.StartsWithCSafe("/") || QueryHelper.ValidateHash("hash")))
                {
                    URLHelper.Redirect(HttpUtility.UrlDecode(returnUrl));
                }
                else if (RedirectToURL != String.Empty)
                {
                    URLHelper.Redirect(RedirectToURL);
                }
            }

            #endregion


            lblError.Visible = false;
        }
    }

    #endregion
}