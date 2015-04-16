using System;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

using CMS.DocumentEngine;
using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.MembershipProvider;
using CMS.PortalControls;
using CMS.PortalEngine;
using CMS.Base;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.WebAnalytics;
using CMS.DataEngine;

public partial class CMSWebParts_Membership_Logon_LogonForm : CMSAbstractWebPart, ICallbackEventHandler
{
    #region "Variables"

    private string mDefaultTargetUrl = "";

    #endregion


    #region "Private properties"

    /// <summary>
    /// Gets error displayed by login control
    /// </summary>
    private string DisplayedError
    {
        get
        {
            var failureLit = Login1.FindControl("FailureText") as LocalizedLabel;
            if (failureLit != null)
            {
                return failureLit.Text;
            }

            return null;
        }
    }

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets the value that indicates whether retrieving of forgotten password is enabled.
    /// </summary>
    public bool AllowPasswordRetrieval
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowPasswordRetrieval"), true);
        }
        set
        {
            SetValue("AllowPasswordRetrieval", value);
            lnkPasswdRetrieval.Visible = value;
        }
    }


    /// <summary>
    /// Gets or sets the sender e-mail (from).
    /// </summary>
    public string SendEmailFrom
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("SendEmailFrom"), SettingsKeyInfoProvider.GetValue(SiteContext.CurrentSiteName + ".CMSSendPasswordEmailsFrom"));
        }
        set
        {
            SetValue("SendEmailFrom", value);
        }
    }


    /// <summary>
    /// Gets or sets the default target url (redirection when the user is logged in).
    /// </summary>
    public string DefaultTargetUrl
    {
        get
        {
            return ValidationHelper.GetString(GetValue("DefaultTargetUrl"), mDefaultTargetUrl);
        }
        set
        {
            SetValue("DefaultTargetUrl", value);
            mDefaultTargetUrl = value;
        }
    }


    /// <summary>
    /// Gets or sets the SkinID of the logon form.
    /// </summary>
    public override string SkinID
    {
        get
        {
            return base.SkinID;
        }
        set
        {
            base.SkinID = value;
            SetSkinID(value);
        }
    }


    /// <summary>
    /// Gets or sets the logon failure text.
    /// </summary>
    public string FailureText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FailureText"), "");
        }
        set
        {
            if (value.Trim() != "")
            {
                SetValue("FailureText", value);
            }
        }
    }


    /// <summary>
    /// Gets or sets reset password url - this url is sent to user in e-mail when he wants to reset his password.
    /// </summary>
    public string ResetPasswordURL
    {
        get
        {
            string url = ValidationHelper.GetString(GetValue("ResetPasswordURL"), string.Empty);
            return DataHelper.GetNotEmpty(URLHelper.GetAbsoluteUrl(url), AuthenticationHelper.GetResetPasswordUrl(SiteContext.CurrentSiteName));
        }
        set
        {
            SetValue("ResetPasswordURL", value);
        }
    }

    #endregion


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
            rqValue.Visible = false;
            // Do not process
        }
        else
        {
            // Set strings
            lnkPasswdRetrieval.Text = GetString("LogonForm.lnkPasswordRetrieval");
            lblPasswdRetrieval.Text = GetString("LogonForm.lblPasswordRetrieval");
            btnPasswdRetrieval.Text = GetString("LogonForm.btnPasswordRetrieval");
            rqValue.ErrorMessage = GetString("LogonForm.rqValue");
            rqValue.ValidationGroup = ClientID + "_PasswordRetrieval";

            // Set logon strings
            LocalizedLabel lblItem = (LocalizedLabel)Login1.FindControl("lblUserName");
            if (lblItem != null)
            {
                lblItem.Text = "{$LogonForm.UserName$}";
            }

            lblItem = (LocalizedLabel)Login1.FindControl("lblPassword");
            if (lblItem != null)
            {
                lblItem.Text = "{$LogonForm.Password$}";
            }

            CMSCheckBox chkItem = (CMSCheckBox)Login1.FindControl("chkRememberMe");
            if ((MFAuthenticationHelper.IsMultiFactorAutEnabled) && (chkItem != null))
            {
                chkItem.Visible = false;
            }

            if ((chkItem != null) && (!MFAuthenticationHelper.IsMultiFactorAutEnabled))
            {
                chkItem.Text = "{$LogonForm.RememberMe$}";
            }
            LocalizedButton btnItem = (LocalizedButton)Login1.FindControl("LoginButton");

            if (btnItem != null)
            {
                btnItem.Text = "{$LogonForm.LogOnButton$}";
                btnItem.ValidationGroup = ClientID + "_Logon";
            }

            RequiredFieldValidator rfv = (RequiredFieldValidator)Login1.FindControl("rfvUserNameRequired");
            if (rfv != null)
            {
                rfv.ToolTip = GetString("LogonForm.NameRequired");
                rfv.Text = rfv.ErrorMessage = GetString("LogonForm.EnterName");
                rfv.ValidationGroup = ClientID + "_Logon";
            }

            CMSTextBox txtUserName = (CMSTextBox)Login1.FindControl("UserName");
            if (txtUserName != null)
            {
                txtUserName.EnableAutoComplete = SecurityHelper.IsAutoCompleteEnabledForLogin(SiteContext.CurrentSiteName);
            }

            lnkPasswdRetrieval.Visible = pnlUpdatePasswordRetrieval.Visible = pnlUpdatePasswordRetrievalLink.Visible = AllowPasswordRetrieval;
            btnPasswdRetrieval.ValidationGroup = ClientID + "_PasswordRetrieval";

            if (!RequestHelper.IsPostBack())
            {
                Login1.UserName = QueryHelper.GetString("username", string.Empty);
                // Set SkinID properties
                if (!StandAlone && (PageCycle < PageCycleEnum.Initialized) && (ValidationHelper.GetString(Page.StyleSheetTheme, "") == ""))
                {
                    SetSkinID(SkinID);
                }
            }

            // Register script to update logon error message
            LocalizedLabel failureLit = Login1.FindControl("FailureText") as LocalizedLabel;
            if (failureLit != null)
            {
                StringBuilder sbScript = new StringBuilder();
                sbScript.Append(@"
function UpdateLabel_", ClientID, @"(content, context) {
    var lbl = document.getElementById(context);
    if(lbl)
    {       
        lbl.innerHTML = content;
        lbl.className = ""InfoLabel"";
    }
}");
                ScriptHelper.RegisterClientScriptBlock(this, GetType(), "InvalidLogonAttempts_" + ClientID, sbScript.ToString(), true);
            }
        }
    }


    void Login1_LoginError(object sender, EventArgs e)
    {
        bool showError = true;

        // Ban IP addresses which are blocked for login
        if (MembershipContext.UserIsBanned)
        {
            DisplayError(GetString("banip.ipisbannedlogin"));
        }
        else if (AuthenticationHelper.DisplayAccountLockInformation(SiteContext.CurrentSiteName) && MembershipContext.UserAccountLockedDueToInvalidLogonAttempts)
        {
            DisplayAccountLockedError(GetString("invalidlogonattempts.unlockaccount.accountlocked"));
        }
        else if (AuthenticationHelper.DisplayAccountLockInformation(SiteContext.CurrentSiteName) && MembershipContext.UserAccountLockedDueToPasswordExpiration)
        {
            DisplayAccountLockedError(GetString("passwordexpiration.accountlocked"));
        }
        else if (MembershipContext.UserIsPartiallyAuthenticated && !MembershipContext.UserAuthenticationFailedDueToInvalidPasscode)
        {
            if (MembershipContext.MFAuthenticationTokenNotInitialized && MFAuthenticationHelper.DisplayTokenID)
            {
                var lblTokenInfo = Login1.FindControl("lblTokenInfo") as LocalizedLabel;
                var lblTokenID = Login1.FindControl("lblTokenID") as LocalizedLabel;
                var plcTokenInfo = Login1.FindControl("plcTokenInfo");

                if (lblTokenInfo != null)
                {
                    lblTokenInfo.Text = string.Format("{0} {1}", GetString("mfauthentication.isRequired"), GetString("mfauthentication.token.get"));
                    lblTokenInfo.Visible = true;
                }

                if (lblTokenID != null)
                {
                    lblTokenID.Text = MFAuthenticationHelper.GetTokenIDForUser(Login1.UserName);
                }

                if (plcTokenInfo != null)
                {
                    plcTokenInfo.Visible = true;
                }
            }

            if (string.IsNullOrEmpty(DisplayedError))
            {
                HideError();
            }

            showError = false;
        }
        else if (!MembershipContext.UserIsPartiallyAuthenticated)
        {
            // Show login and password screen
            var plcPasscodeBox = Login1.FindControl("plcPasscodeBox");
            var plcLoginInputs = Login1.FindControl("plcLoginInputs");
            var plcTokenInfo = Login1.FindControl("plcTokenInfo");
            if (plcLoginInputs != null)
            {
                plcLoginInputs.Visible = true;
            }
            if (plcPasscodeBox != null)
            {
                plcPasscodeBox.Visible = false;
            }
            if (plcTokenInfo != null)
            {
                plcTokenInfo.Visible = false;
            }
        }

        if (showError && string.IsNullOrEmpty(DisplayedError))
        {
            DisplayError(DataHelper.GetNotEmpty(FailureText, GetString("Login_FailureText")));
        }
    }


    /// <summary>
    /// OnLoad override (show hide password retrieval).
    /// </summary>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        Login1.LoggedIn += Login1_LoggedIn;
        Login1.LoggingIn += Login1_LoggingIn;
        Login1.LoginError += Login1_LoginError;
        Login1.Authenticate += Login1_Authenticate;

        btnPasswdRetrieval.Click += btnPasswdRetrieval_Click;
    }


    /// <summary>
    /// Displays locked account error message.
    /// </summary>
    /// <param name="specificMessage">Specific part of the message.</param>
    private void DisplayAccountLockedError(string specificMessage)
    {
        var failureLabel = Login1.FindControl("FailureText") as LocalizedLabel;
        if (failureLabel != null)
        {
            string link = "<a href=\"#\" onclick=\"" + Page.ClientScript.GetCallbackEventReference(this, "null", "UpdateLabel_" + ClientID, "'" + failureLabel.ClientID + "'") + ";\">" + GetString("general.clickhere") + "</a>";
            DisplayError(string.Format(specificMessage + " " + GetString("invalidlogonattempts.unlockaccount.accountlockedlink"), link));
        }
    }


    /// <summary>
    /// Sets SkinId to all controls in logon form.
    /// </summary>
    private void SetSkinID(string skinId)
    {
        if (skinId != "")
        {
            Login1.SkinID = skinId;

            LocalizedLabel lbl = (LocalizedLabel)Login1.FindControl("lblUserName");
            if (lbl != null)
            {
                lbl.SkinID = skinId;
            }
            lbl = (LocalizedLabel)Login1.FindControl("lblPassword");
            if (lbl != null)
            {
                lbl.SkinID = skinId;
            }

            TextBox txt = (TextBox)Login1.FindControl("UserName");
            if (txt != null)
            {
                txt.SkinID = skinId;
            }
            txt = (TextBox)Login1.FindControl("Password");
            if (txt != null)
            {
                txt.SkinID = skinId;
            }

            CMSCheckBox chk = (CMSCheckBox)Login1.FindControl("chkRememberMe");
            if (chk != null)
            {
                chk.SkinID = skinId;
            }

            LocalizedButton btn = (LocalizedButton)Login1.FindControl("LoginButton");
            if (btn != null)
            {
                btn.SkinID = skinId;
            }
        }
    }


    /// <summary>
    /// Applies given stylesheet skin.
    /// </summary>
    public override void ApplyStyleSheetSkin(Page page)
    {
        SetSkinID(SkinID);

        base.ApplyStyleSheetSkin(page);
    }


    /// <summary>
    /// Displays error.
    /// </summary>
    /// <param name="msg">Message.</param>
    private void DisplayError(string msg)
    {
        var failureLit = Login1.FindControl("FailureText") as LocalizedLabel;

        if (failureLit != null)
        {
            failureLit.Text = msg;
            failureLit.Visible = !string.IsNullOrEmpty(msg);
        }
    }


    /// <summary>
    /// Hides displayed error.
    /// </summary>
    private void HideError()
    {
        DisplayError(string.Empty);
    }


    /// <summary>
    /// Retrieve the user password.
    /// </summary>
    private void btnPasswdRetrieval_Click(object sender, EventArgs e)
    {
        string value = txtPasswordRetrieval.Text.Trim();

        if (value != String.Empty)
        {
            // Prepare return URL
            string returnUrl = RequestContext.CurrentURL;
            string userName = Login1.UserName;
            if (!string.IsNullOrEmpty(userName))
            {
                returnUrl = URLHelper.AddParameterToUrl(returnUrl, "username", userName);
            }

            bool success;
            lblResult.Text = AuthenticationHelper.ForgottenEmailRequest(value, SiteContext.CurrentSiteName, "LOGONFORM", SendEmailFrom, ContextResolver, ResetPasswordURL, out success, returnUrl);
            lblResult.Visible = true;

            pnlPasswdRetrieval.Visible = true;
        }
    }


    /// <summary>
    /// Logged in handler.
    /// </summary>
    private void Login1_LoggedIn(object sender, EventArgs e)
    {
        // Set view mode to live site after login to prevent bar with "Close preview mode"
        PortalContext.ViewMode = ViewModeEnum.LiveSite;

        // Ensure response cookie
        CookieHelper.EnsureResponseCookie(FormsAuthentication.FormsCookieName);

        // Set cookie expiration
        if (Login1.RememberMeSet)
        {
            CookieHelper.ChangeCookieExpiration(FormsAuthentication.FormsCookieName, DateTime.Now.AddYears(1), false);
        }
        else
        {
            // Extend the expiration of the authentication cookie if required
            if (!AuthenticationHelper.UseSessionCookies && (HttpContext.Current != null) && (HttpContext.Current.Session != null))
            {
                CookieHelper.ChangeCookieExpiration(FormsAuthentication.FormsCookieName, DateTime.Now.AddMinutes(Session.Timeout), false);
            }
        }

        // Current username
        string userName = Login1.UserName;

        // Get user name (test site prefix too)
        UserInfo ui = UserInfoProvider.GetUserInfoForSitePrefix(userName, SiteContext.CurrentSite);

        // Check whether safe user name is required and if so get safe username
        if (RequestHelper.IsMixedAuthentication() && UserInfoProvider.UseSafeUserName)
        {
            // Get info on the authenticated user            
            if (ui == null)
            {
                // User stored with safe name
                userName = ValidationHelper.GetSafeUserName(Login1.UserName, SiteContext.CurrentSiteName);

                // Find user by safe name
                ui = UserInfoProvider.GetUserInfoForSitePrefix(userName, SiteContext.CurrentSite);
                if (ui != null)
                {
                    // Authenticate user by site or global safe username
                    AuthenticationHelper.AuthenticateUser(ui.UserName, Login1.RememberMeSet);
                }
            }
        }

        if (ui != null)
        {
            // If user name is site prefixed, authenticate user manually 
            if (UserInfoProvider.IsSitePrefixedUser(ui.UserName))
            {
                AuthenticationHelper.AuthenticateUser(ui.UserName, Login1.RememberMeSet);
            }

            // Log activity
            int contactID = ModuleCommands.OnlineMarketingGetUserLoginContactID(ui);
            Activity activityLogin = new ActivityUserLogin(contactID, ui, DocumentContext.CurrentDocument, AnalyticsContext.ActivityEnvironmentVariables);
            activityLogin.Log();
        }

        // Redirect user to the return url, or if is not defined redirect to the default target url
        string url = QueryHelper.GetString("ReturnURL", string.Empty);
        if (!string.IsNullOrEmpty(url))
        {
            if (url.StartsWithCSafe("~") || url.StartsWithCSafe("/") || QueryHelper.ValidateHash("hash"))
            {
                URLHelper.Redirect(ResolveUrl(QueryHelper.GetString("ReturnURL", string.Empty)));
            }
            else
            {
                URLHelper.Redirect(ResolveUrl("~/CMSMessages/Error.aspx?title=" + ResHelper.GetString("general.badhashtitle") + "&text=" + ResHelper.GetString("general.badhashtext")));
            }
        }
        else
        {
            if (DefaultTargetUrl != "")
            {
                URLHelper.Redirect(ResolveUrl(DefaultTargetUrl));
            }
            else
            {
                URLHelper.Redirect(RequestContext.CurrentURL);
            }
        }
    }


    /// <summary>
    /// Handling login authenticate event.
    /// </summary>
    /// <param name="sender">Sender.</param>
    /// <param name="e">Authenticate event arguments.</param>
    private void Login1_Authenticate(object sender, AuthenticateEventArgs e)
    {
        if (MFAuthenticationHelper.IsMultiFactorRequiredForUser(Login1.UserName))
        {
            var plcPasscodeBox = Login1.FindControl("plcPasscodeBox");
            var plcLoginInputs = Login1.FindControl("plcLoginInputs");
            var txtPasscode = Login1.FindControl("txtPasscode") as CMSTextBox;

            if (txtPasscode == null)
            {
                return;
            }
            if (plcPasscodeBox == null)
            {
                return;
            }
            if (plcLoginInputs == null)
            {
                return;
            }

            // Handle passcode
            string passcode = txtPasscode.Text;
            txtPasscode.Text = string.Empty;

            var provider = new CMSMembershipProvider();

            // Validate username and password
            if (plcLoginInputs.Visible)
            {
                if (provider.MFValidateCredentials(Login1.UserName, Login1.Password))
                {
                    // Show passcode screen
                    plcLoginInputs.Visible = false;
                    plcPasscodeBox.Visible = true;
                }
            }
            // Validate passcode
            else
            {
                if (provider.MFValidatePasscode(Login1.UserName, passcode))
                {
                    e.Authenticated = true;
                }
            }
        }
        else
        {
            e.Authenticated = Membership.Provider.ValidateUser(Login1.UserName, Login1.Password);
        }
    }


    /// <summary>
    /// Logging in handler.
    /// </summary>
    private void Login1_LoggingIn(object sender, LoginCancelEventArgs e)
    {
        if (((CMSCheckBox)Login1.FindControl("chkRememberMe")).Checked)
        {
            Login1.RememberMeSet = true;
        }
        else
        {
            Login1.RememberMeSet = false;
        }
    }


    /// <summary>
    /// Forgotten password retrieval toggle link click event.
    /// </summary>
    protected void lnkPasswdRetrieval_Click(object sender, EventArgs e)
    {
        pnlPasswdRetrieval.Visible = !pnlPasswdRetrieval.Visible;
    }


    ///<summary>
    /// Overrides the generation of the SPAN tag with custom tag.
    ///</summary>
    protected HtmlTextWriterTag TagKey
    {
        get
        {
            if (SiteContext.CurrentSite != null)
            {
                if (SettingsKeyInfoProvider.GetValue(SiteContext.CurrentSiteName + ".CMSControlElement").ToLowerCSafe().Trim() == "div")
                {
                    return HtmlTextWriterTag.Div;
                }
                else
                {
                    return HtmlTextWriterTag.Span;
                }
            }
            return HtmlTextWriterTag.Span;
        }
    }


    #region "ICallbackEventHandler Members"

    public string GetCallbackResult()
    {
        string result = "";
        UserInfo ui = UserInfoProvider.GetUserInfo(Login1.UserName);
        if (ui != null)
        {
            string siteName = SiteContext.CurrentSiteName;

            // Prepare return URL
            string returnUrl = RequestContext.CurrentURL;
            if (!string.IsNullOrEmpty(Login1.UserName))
            {
                returnUrl = URLHelper.AddParameterToUrl(returnUrl, "username", Login1.UserName);
            }

            switch (UserAccountLockCode.ToEnum(ui.UserAccountLockReason))
            {
                case UserAccountLockEnum.MaximumInvalidLogonAttemptsReached:
                    result = AuthenticationHelper.SendUnlockAccountRequest(ui, siteName, "USERLOGON", SettingsKeyInfoProvider.GetValue(siteName + ".CMSSendPasswordEmailsFrom"), ContextResolver, returnUrl);
                    break;

                case UserAccountLockEnum.PasswordExpired:
                    bool outParam = true;
                    result = AuthenticationHelper.SendPasswordRequest(ui, siteName, "USERLOGON", SettingsKeyInfoProvider.GetValue(siteName + ".CMSSendPasswordEmailsFrom"), "Membership.PasswordExpired", ContextResolver, AuthenticationHelper.GetResetPasswordUrl(siteName), out outParam, returnUrl);
                    break;
            }
        }

        return result;
    }


    public void RaiseCallbackEvent(string eventArgument)
    {
    }

    #endregion
}