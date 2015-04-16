using System;
using System.Text;

using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.LicenseProvider;
using CMS.MembershipProvider;
using CMS.Modules;
using CMS.PortalControls;
using CMS.PortalEngine;
using CMS.Base;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.SocialMedia.Facebook;
using CMS.UIControls;
using CMS.WebAnalytics;
using CMS.Protection;
using CMS.DataEngine;

public partial class CMSWebParts_Membership_FacebookConnect_FacebookConnectLogon : CMSAbstractWebPart
{

    #region "Nested enum"

    /// <summary>
    /// Indicates whether the sign out option is displayed and how.
    /// </summary>
    public enum ShowSignOutEnum
    {
        NotSet = -1,
        DoNotShow = 0,
        Image = 1,
        Link = 2,
        Button = 3
    }

    #endregion


    #region "Constants"

    protected const string CONFIRMATION_URLPARAMETER = "confirmed";

    #endregion


    #region "Private variables"

    private string[] mScopeNames;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets sign in text.
    /// </summary>
    public string SignInText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SignInText"), "");
        }
        set
        {
            SetValue("SignInText", value);
        }
    }

    /// <summary>
    /// Specifies whether to show faces underneath the Login button.
    /// </summary>
    public bool ShowFaces
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowFaces"), false);
        }
        set
        {
            SetValue("ShowFaces", value);
        }
    }


    /// <summary>
    /// Returns "show-faces" HTML attribute for FB button according to current setting.
    /// </summary>
    public string ShowFacesAttr
    {
        get
        {
            return String.Format(" show-faces=\"{0}\"", ShowFaces.ToString().ToLowerCSafe());
        }
    }


    /// <summary>
    /// The width of the plugin in pixels. Default width: 200px.
    /// </summary>
    public int ButtonWidth
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("ButtonWidth"), 200);
        }
        set
        {
            SetValue("ButtonWidth", value);
        }
    }


    /// <summary>
    /// Returns "width" HTML attribute for FB button according to current setting.
    /// </summary>
    public string WidthAttr
    {
        get
        {
            return String.Format(" width=\"{0}\"", (ButtonWidth <= 0 ? 200 : ButtonWidth));
        }
    }


    /// <summary>
    /// The maximum number of rows of profile pictures to display. Default value: 1. 
    /// </summary>
    public int MaxRows
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("MaxRows"), 1);
        }
        set
        {
            SetValue("MaxRows", value);
        }
    }


    /// <summary>
    /// Returns "max-rows" HTML attribute for FB button according to current setting.
    /// </summary>
    public string MaxRowsAttr
    {
        get
        {
            return String.Format(" max-rows=\"{0}\"", (MaxRows < 0 ? 0 : MaxRows));
        }
    }


    /// <summary>
    /// Returns "scope" HTML attribute for FB button according to current setting.
    /// </summary>
    public string ScopeAttr
    {
        get
        {
            if (mScopeNames == null)
            {
                var model = FacebookMappingHelper.GetUserProfileModel();
                var mapping = FacebookMappingHelper.GetUserProfileMapping();
                mScopeNames = mapping.GetFacebookPermissionScopeNames(model);
            }

            return mScopeNames.Length == 0 ? null : String.Format(" scope=\"{0}\"", mScopeNames.Join(","));
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether to show sign out link and how.
    /// </summary>
    public ShowSignOutEnum ShowSignOutAs
    {
        get
        {
            return (ShowSignOutEnum)ValidationHelper.GetInteger(GetValue("ShowSignOutAs"), -1);
        }
        set
        {
            SetValue("ShowSignOutAs", (int)value);
        }
    }


    /// <summary>
    /// Gets or sets sign out text.
    /// </summary>
    public string SignOutText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SignOutText"), String.Empty);
        }
        set
        {
            SetValue("SignOutText", value);
        }
    }


    /// <summary>
    /// Gets or sets sign out button image URL.
    /// </summary>
    public string SignOutImageURL
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("SignOutImageURL"), null);
        }
        set
        {
            SetValue("SignOutImageURL", value);
        }
    }


    /// <summary>
    /// Gets or sets the conversion track name used after successful registration.
    /// </summary>
    public string TrackConversionName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("TrackConversionName"), "");
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


    /// <summary>
    /// Gets or sets the value that indicates whether after successful registration is 
    /// notification email sent to the administrator.
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
    /// Gets or sets the value indicating whether a XML name-space is included in the HTML tag.
    /// This is necessary for XFBML to work in earlier versions of Internet Explorer (IE9).
    /// However, it causes invalid HTML5 markup.
    /// </summary>
    public bool IncludeNamespace
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("IncludeNamespace"), true);
        }
        set
        {
            SetValue("IncludeNamespace", value);
        }
    }


    /// <summary>
    /// Gets the sender email (from).
    /// </summary>
    private string FromAddress
    {
        get
        {
            return SettingsKeyInfoProvider.GetValue(SiteContext.CurrentSiteName + ".CMSNoreplyEmailAddress");
        }
    }


    /// <summary>
    /// Gets the recipient email (to).
    /// </summary>
    private string ToAddress
    {
        get
        {
            return SettingsKeyInfoProvider.GetValue(SiteContext.CurrentSiteName + ".CMSAdminEmailAddress");
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
    /// Initializes the control.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            Visible = false;

            return;
        }

        if (!LicenseHelper.CheckFeature(RequestContext.CurrentDomain, FeatureEnum.FaceBookConnect))
        {
            Visible = DisplayMessage(String.Format(GetString("licenselimitation.featurenotavailable"), FeatureEnum.FaceBookConnect));

            return;
        }

        // Check Facebook Connect settings
        string currentSiteName = SiteContext.CurrentSiteName;
        if (String.IsNullOrEmpty(currentSiteName) || !SettingsKeyInfoProvider.GetBoolValue(currentSiteName + ".CMSEnableFacebookConnect") || !FacebookConnectHelper.FacebookIsAvailable(currentSiteName))
        {
            // Show warning message in "Design mode"
            Visible = DisplayMessage();

            return;
        }

        // If user is already authenticated 
        if (AuthenticationHelper.IsAuthenticated())
        {
            SetupControlForAuthenticatedUser();
        }
        else
        {
            SetupControlForPublicUser();
        }
    }


    /// <summary>
    /// Setups control for authenticated users.
    /// </summary>
    private void SetupControlForAuthenticatedUser()
    {
        EnsureBackwardCompatibility();

        // If signout should not be visible or user has not FacebookID registered
        if ((ShowSignOutAs == ShowSignOutEnum.DoNotShow) || String.IsNullOrEmpty(MembershipContext.AuthenticatedUser.UserSettings.UserFacebookID))
        {
            Visible = false;

            return;
        }

        RegisterFacebookScript();

        string logoutScript = AuthenticationHelper.GetSignOutOnClickScript(Page);

        // Hide Facebook Connect button
        plcFBButton.Visible = false;
        if (String.IsNullOrEmpty(SignOutText))
        {
            SignOutText = ResHelper.GetString("webparts_membership_signoutbutton.signout");
        }

        switch (ShowSignOutAs)
        {
            // Show as image
            case ShowSignOutEnum.Image:
                string signOutImageUrl = SignOutImageURL;

                // Use default image if none is specified
                if (String.IsNullOrEmpty(signOutImageUrl))
                {
                    signOutImageUrl = GetImageUrl("Others/FacebookConnect/signout.gif");
                }
                imgSignOut.ImageUrl = ResolveUrl(signOutImageUrl);
                imgSignOut.Visible = true;
                imgSignOut.AlternateText = GetString("webparts_membership_signoutbutton.signout");
                lnkSignOutImageBtn.Visible = true;
                lnkSignOutImageBtn.Attributes.Add("onclick", logoutScript);
                lnkSignOutImageBtn.Attributes.Add("style", "cursor:pointer;");

                break;

            // Show as link
            case ShowSignOutEnum.Link:
                lnkSignOutLink.Text = SignOutText;
                lnkSignOutLink.Visible = true;
                lnkSignOutLink.Attributes.Add("onclick", logoutScript);
                lnkSignOutLink.Attributes.Add("style", "cursor:pointer;");

                break;

            // Show as button
            case ShowSignOutEnum.Button:
                btnSignOut.OnClientClick = logoutScript;
                btnSignOut.Text = SignOutText;
                btnSignOut.Visible = true;

                break;
        }
    }


    /// <summary>
    /// Registers Facebook javascript SDK to the page.
    /// </summary>
    private void RegisterFacebookScript()
    {
        if (Page is ContentPage)
        {
            // Adding XML namespace
            if (IncludeNamespace)
            {
                ((ContentPage)Page).XmlNamespace = FacebookConnectHelper.GetFacebookXmlNamespace();
            }
        }

        // Init FB connect
        string initscr = FacebookConnectHelper.GetFacebookInitScriptForSite(SiteContext.CurrentSiteName) + " " + FacebookConnectHelper.GetFacebookLoginHandlerScript();
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "FBConnectLogon", initscr);
    }


    /// <summary>
    /// Signs in user or registers a new user.
    /// </summary>
    private void SetupControlForPublicUser()
    {
        RegisterFacebookScript();

        // Validate FB access token against FB server
        string facebookUserId = null;
        bool facebookCookiesValid = false;

        string confirmToken = QueryHelper.GetString(CONFIRMATION_URLPARAMETER, null);
        if (!String.IsNullOrEmpty(confirmToken))
        {
            facebookCookiesValid = FacebookConnectHelper.ValidateFBAccessToken(confirmToken, out facebookUserId);
        }

        // If user has signed in
        if (!String.IsNullOrEmpty(facebookUserId) && facebookCookiesValid)
        {
            UserInfo ui = UserInfoProvider.GetUserInfoByFacebookConnectID(facebookUserId);

            // Claimed Facebook ID is in DB
            if (ui != null)
            {
                SignInUser(ui, facebookUserId, confirmToken);
            }
            // Claimed Facebook ID not found  = save new user
            else
            {
                RegisterNewUser(facebookUserId, confirmToken);
            }
        }
    }


    /// <summary>
    /// Signs in given user.
    /// </summary>
    /// <param name="ui">User that will be signed in.</param>
    /// <param name="facebookUserId">The user's Facebook ID</param>
    /// <param name="facebookAccessToken">The user's access token retrieved from Facebook</param>
    private void SignInUser(UserInfo ui, string facebookUserId, string facebookAccessToken)
    {
        // Login existing user
        if (ui.Enabled)
        {
            MapFacebookUserProfile(FacebookUserProfileMappingTriggerEnum.Login, ui, facebookUserId, facebookAccessToken);

            // Ban IP addresses which are blocked for login
            BannedIPInfoProvider.CheckIPandRedirect(SiteContext.CurrentSiteName, BanControlEnum.Login);

            // Create autentification cookie
            AuthenticationHelper.SetAuthCookieWithUserData(ui.UserName, true, Session.Timeout, new string[] { "facebooklogon" });
            UserInfoProvider.SetPreferredCultures(ui);

            int contactId = ModuleCommands.OnlineMarketingGetUserLoginContactID(ui);
            Activity activity = new ActivityUserLogin(contactId, ui, DocumentContext.CurrentDocument, AnalyticsContext.ActivityEnvironmentVariables);
            activity.Log();

            // Redirect user
            string returnUrl = QueryHelper.GetString("returnurl", null);
            if (String.IsNullOrEmpty(returnUrl))
            {
                returnUrl = URLHelper.RemoveParameterFromUrl(RequestContext.CurrentURL, CONFIRMATION_URLPARAMETER);
            }
            URLHelper.Redirect(returnUrl);
        }
        else
        {
            // User is disabled
            lblError.Text = GetString("membership.userdisabled");
            lblError.Visible = true;
        }
    }


    /// <summary>
    /// Registers new user.
    /// </summary>
    /// <param name="facebookUserId">The user's Facebook ID</param>
    /// <param name="facebookAccessToken">The user's access token retrieved from Facebook</param>
    private void RegisterNewUser(string facebookUserId, string facebookAccessToken)
    {
        // Check whether additional user info page is set
        string currentSiteName = SiteContext.CurrentSiteName;

        // Register new user
        string error = null;
        UserInfo ui = AuthenticationHelper.AuthenticateFacebookConnectUser(facebookUserId, currentSiteName, false, true, ref error);

        // If user was found or successfuly created
        if (ui != null)
        {
            // Send registration e-mails
            // E-mail confirmation is not required as user already provided confirmation by successful login using Facebook connect
            AuthenticationHelper.SendRegistrationEmails(ui, null, null, false, false);

            // Notify administrator
            if (NotifyAdministrator && !String.IsNullOrEmpty(FromAddress) && !String.IsNullOrEmpty(ToAddress))
            {
                AuthenticationHelper.NotifyAdministrator(ui, FromAddress, ToAddress);
            }

            // Log user registration into the web analytics and track conversion if set
            AnalyticsHelper.TrackUserRegistration(currentSiteName, ui, TrackConversionName, ConversionValue);

            Activity activity = new ActivityRegistration(ui, DocumentContext.CurrentDocument, AnalyticsContext.ActivityEnvironmentVariables);
            if (activity.Data != null)
            {
                activity.Data.ContactID = ModuleCommands.OnlineMarketingGetUserLoginContactID(ui);
                activity.Log();
            }

            MapFacebookUserProfile(FacebookUserProfileMappingTriggerEnum.Registration, ui, facebookUserId, facebookAccessToken);

            // Signs in created user and redirects her to the return URL
            SignInUser(ui, facebookUserId, facebookAccessToken);
        }

        lblError.Text = error;
        lblError.Visible = true;
    }


    /// <summary>
    /// Displays warning message in "Design mode".
    /// </summary>
    /// <param name="message">Message that will be displayed. Default misconfiration message is used when no parameter is given.</param>
    private bool DisplayMessage(string message = null)
    {
        // Error label is displayed in Design mode when Facebook Connect is disabled
        if (PortalContext.IsDesignMode(PortalContext.ViewMode))
        {
            if (String.IsNullOrEmpty(message))
            {
                // Default message informing about misconfiguration is dispalyed.
                StringBuilder parameter = new StringBuilder();
                parameter.Append(UIElementInfoProvider.GetApplicationNavigationString("cms", "Settings") + " -> ");
                parameter.Append(GetString("settingscategory.socialmedia") + " -> ");
                parameter.Append(GetString("settingscategory.cmsfacebookconnect"));
                if (MembershipContext.AuthenticatedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin))
                {
                    // Make it link for Admin
                    parameter.Insert(0, "<a href=\"" + URLHelper.GetAbsoluteUrl(UIContextHelper.GetApplicationUrl("cms", "settings")) + "\" target=\"_top\">");
                    parameter.Append("</a>");
                }

                message = String.Format(GetString("mem.facebook.disabled"), parameter.ToString());
            }
            lblError.Text = message;
            lblError.Visible = true;
            plcFBButton.Visible = false;
            imgSignOut.Visible = false;
            lnkSignOutImageBtn.Visible = false;
            lnkSignOutLink.Visible = false;
        }

        return lblError.Visible;
    }


    /// <summary>
    /// Maps the values of Facebook user profile attributes to the values of CMS user fields depending on the current settings.
    /// </summary>
    /// <param name="mappingTrigger">Specifies when the mapping of Facebook user profile occurs.</param>
    /// <param name="user">The CMS user.</param>
    /// <param name="userProfileId">The Facebook user profile identifier.</param>
    /// <param name="facebookAccessToken">The Facebook access token.</param>
    /// <remarks>
    /// The CMS user name is updated only if it was generated by the CMS.
    /// </remarks>
    private void MapFacebookUserProfile(FacebookUserProfileMappingTriggerEnum mappingTrigger, UserInfo user, string userProfileId, string facebookAccessToken)
    {
        IFacebookUserProvider facebookUserProvider = new FacebookUserProvider();
        var facebookUser = facebookUserProvider.GetFacebookUser(userProfileId, facebookAccessToken);
        bool userChanged = false;

        // Change user's full name if it is in default format
        if (String.IsNullOrEmpty(user.FullName) || (user.FullName == UserInfoProvider.FACEBOOKID_FULLNAME_PREFIX + userProfileId))
        {
            user.FullName = facebookUser.FacebookName;
            userChanged = true;
        }

        // Map Facebook user on appropriate action
        if (FacebookMappingHelper.GetUserProfileMappingTrigger(SiteContext.CurrentSiteName) == mappingTrigger)
        {
            FacebookMappingHelper.MapUserProfile(facebookUser, user);
            userChanged = true;
        }

        if (userChanged)
        {
            user.Update();
        }
    }


    /// <summary>
    /// Ensures that this new version of the web part will behave as in the past 
    /// if a user updates to this version from the old version without setting new properties.
    /// </summary>
    private void EnsureBackwardCompatibility()
    {
        if (ShowSignOutAs != ShowSignOutEnum.NotSet)
        {
            // Property "Show sign out as" is set, there is no need for compatibility checks.
            return;
        }

        // Check if show sign out may be visible
        bool showSignOut = ValidationHelper.GetBoolean(GetValue("ShowSignOut"), false);
        if (!showSignOut)
        {
            ShowSignOutAs = ShowSignOutEnum.DoNotShow;
            return;
        }

        // Check if sign out may be displayed as image
        if (String.IsNullOrEmpty(SignOutText))
        {
            ShowSignOutAs = ShowSignOutEnum.Image;
            return;
        }

        // Sign out may be displayed as link or button
        bool showAsButton = ValidationHelper.GetBoolean(GetValue("ShowAsButton"), false);
        ShowSignOutAs = showAsButton ? ShowSignOutEnum.Button : ShowSignOutEnum.Link;
    }

    #endregion
}