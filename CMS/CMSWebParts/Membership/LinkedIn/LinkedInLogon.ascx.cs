using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Security;
using System.Web.UI.WebControls;
using System.Data;
using System.Web;

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
using CMS.WebAnalytics;
using CMS.DataEngine;

public partial class CMSWebParts_Membership_LinkedIn_LinkedInLogon : CMSAbstractWebPart
{
    #region "Variables"

    private LinkedInHelper linkedInHelper = null;

    #endregion


    #region "Constants"

    protected const string FILES_LOCATION = "~/CMSWebparts/Membership/LinkedIn/LinkedInLogon_files/";
    protected const string SESSION_NAME_USERDATA = "LinkedInUserData";

    #endregion


    #region "Properties"

    /// <summary>
    /// Indicates if birth date is required in registration process.
    /// </summary>
    public bool RequireBirthDate
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("BirthDate"), true);
        }
        set
        {
            SetValue("BirthDate", value);
        }
    }


    /// <summary>
    /// Indicates if first name is required in registration process.
    /// </summary>
    public bool RequireFirstName
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("FirstName"), true);
        }
        set
        {
            SetValue("FirstName", value);
        }
    }


    /// <summary>
    /// Indicates if last name is required in registration process.
    /// </summary>
    public bool RequireLastName
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("LastName"), true);
        }
        set
        {
            SetValue("LastName", value);
        }
    }


    /// <summary>
    /// Gets or sets sign in button image URL.
    /// </summary>
    public string SignInImageURL
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("SignInImageURL"), GetImageUrl(FILES_LOCATION + "signin.png"));
        }
        set
        {
            SetValue("SignInImageURL", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether to show sign out button.
    /// </summary>
    public bool ShowSignOut
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowSignOut"), false);
        }
        set
        {
            SetValue("ShowSignOut", value);
        }
    }


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
    /// Gets or sets sign out text.
    /// </summary>
    public string SignOutText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SignOutText"), "");
        }
        set
        {
            SetValue("SignOutText", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that buttons will be used instead of link buttons.
    /// </summary>
    public bool ShowAsButton
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowAsButton"), false);
        }
        set
        {
            SetValue("ShowAsButton", value);
        }
    }


    /// <summary>
    /// Gets or sets sign out button image URL.
    /// </summary>
    public string SignOutImageURL
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("SignOutImageURL"), GetImageUrl(FILES_LOCATION + "signout.png"));
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


    #region "Page events"

    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (!StopProcessing)
        {
            if (SystemContext.IsFullTrustLevel)
            {
                if (!LicenseHelper.CheckFeature(RequestContext.CurrentDomain, FeatureEnum.LinkedIn))
                {
                    Visible = DisplayMessage(String.Format(GetString("licenselimitation.featurenotavailable"), FeatureEnum.LinkedIn));
                    return;
                }

                // Check if LinkedIn module is enabled
                if (!LinkedInHelper.LinkedInIsAvailable(SiteContext.CurrentSiteName))
                {
                    Visible = DisplayMessage();
                    return;
                }

                DisplayButtons();
                linkedInHelper = new LinkedInHelper();
                CheckStatus();
            }
            // Error label is displayed in Design mode when LinkedIn library is not loaded
            else
            {
                lblError.ResourceString = "socialnetworking.fulltrustrequired";
                lblError.Visible = true;
            }
        }
        else
        {
            Visible = false;
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Checks status of current user.
    /// </summary>
    protected void CheckStatus()
    {
        // Get current site name
        string siteName = SiteContext.CurrentSiteName;
        string error = null;

        // Check return URL
        string returnUrl = QueryHelper.GetString("returnurl", null);
        returnUrl = HttpUtility.UrlDecode(returnUrl);

        // Get current URL
        string currentUrl = RequestContext.CurrentURL;
        currentUrl = URLHelper.RemoveParameterFromUrl(currentUrl, "oauth_token");
        currentUrl = URLHelper.RemoveParameterFromUrl(currentUrl, "oauth_verifier");

        // Get LinkedIn response status
        switch (linkedInHelper.CheckStatus(RequireFirstName, RequireLastName, RequireBirthDate, null))
        {
            // User is authenticated
            case CMSOpenIDHelper.RESPONSE_AUTHENTICATED:
                // LinkedIn profile Id not found  = save new user
                if (UserInfoProvider.GetUserInfoByLinkedInID(linkedInHelper.MemberId) == null)
                {
                    string additionalInfoPage = SettingsKeyInfoProvider.GetValue(siteName + ".CMSRequiredLinkedInPage").Trim();

                    // No page set, user can be created
                    if (String.IsNullOrEmpty(additionalInfoPage))
                    {
                        // Register new user
                        UserInfo ui = AuthenticationHelper.AuthenticateLinkedInUser(linkedInHelper.MemberId, linkedInHelper.FirstName, linkedInHelper.LastName, siteName, true, true, ref error);

                        // If user was successfully created
                        if (ui != null)
                        {
                            if (linkedInHelper.BirthDate != DateTimeHelper.ZERO_TIME)
                            {
                                ui.UserSettings.UserDateOfBirth = linkedInHelper.BirthDate;
                            }

                            UserInfoProvider.SetUserInfo(ui);

                            // If user is enabled
                            if (ui.Enabled)
                            {
                                // Create authentication cookie
                                AuthenticationHelper.SetAuthCookieWithUserData(ui.UserName, true, Session.Timeout, new string[] { "linkedinlogin" });

                                Activity activityLogin = new ActivityUserLogin(ModuleCommands.OnlineMarketingGetUserLoginContactID(ui), ui, DocumentContext.CurrentDocument, AnalyticsContext.ActivityEnvironmentVariables);
                                activityLogin.Log();
                            }

                            // Notify administrator
                            if (NotifyAdministrator && !String.IsNullOrEmpty(FromAddress) && !String.IsNullOrEmpty(ToAddress))
                            {
                                AuthenticationHelper.NotifyAdministrator(ui, FromAddress, ToAddress);
                            }

                            // Send registration e-mails
                            // E-mail confirmation is not required as user already provided confirmation by successful login using OpenID
                            AuthenticationHelper.SendRegistrationEmails(ui, null, null, false, false);

                            // Log user registration into the web analytics and track conversion if set
                            AnalyticsHelper.TrackUserRegistration(siteName, ui, TrackConversionName, ConversionValue);

                            Activity activity = new ActivityRegistration(ui, DocumentContext.CurrentDocument, AnalyticsContext.ActivityEnvironmentVariables);
                            if (activity.Data != null)
                            {
                                activity.Data.ContactID = ModuleCommands.OnlineMarketingGetUserLoginContactID(ui);
                                activity.Log();
                            }
                        }

                        // Redirect when authentication was successful
                        if (String.IsNullOrEmpty(error))
                        {
                            if (!String.IsNullOrEmpty(returnUrl))
                            {
                                URLHelper.Redirect(URLHelper.GetAbsoluteUrl(returnUrl));
                            }
                            else
                            {
                                URLHelper.Redirect(currentUrl);
                            }
                        }
                        // Display error otherwise
                        else
                        {
                            lblError.Text = error;
                            lblError.Visible = true;
                        }
                    }
                    // Additional information page is set
                    else
                    {
                        // Store user object in session for additional use
                        string response = (linkedInHelper.LinkedInResponse != null) ? linkedInHelper.LinkedInResponse.OuterXml : null;
                        SessionHelper.SetValue(SESSION_NAME_USERDATA, response);

                        // Redirect to additional info page
                        string targetURL = URLHelper.GetAbsoluteUrl(additionalInfoPage);

                        if (!String.IsNullOrEmpty(returnUrl))
                        {
                            // Add return URL to parameter
                            targetURL = URLHelper.AddParameterToUrl(targetURL, "returnurl", HttpUtility.UrlEncode(returnUrl));
                        }
                        URLHelper.Redirect(targetURL);
                    }
                }
                // LinkedIn profile id is in DB
                else
                {
                    // Login existing user
                    UserInfo ui = AuthenticationHelper.AuthenticateLinkedInUser(linkedInHelper.MemberId, linkedInHelper.FirstName, linkedInHelper.LastName, siteName, false, true, ref error);

                    if ((ui != null) && (ui.Enabled))
                    {
                        // Create authentication cookie
                        AuthenticationHelper.SetAuthCookieWithUserData(ui.UserName, true, Session.Timeout, new string[] { "linkedinlogin" });

                        int contactID = ModuleCommands.OnlineMarketingGetUserLoginContactID(ui);
                        Activity activityLogin = new ActivityUserLogin(contactID, ui, DocumentContext.CurrentDocument, AnalyticsContext.ActivityEnvironmentVariables);
                        activityLogin.Log();

                        // Redirect user
                        if (!String.IsNullOrEmpty(returnUrl))
                        {
                            URLHelper.Redirect(URLHelper.GetAbsoluteUrl(returnUrl));
                        }
                        else
                        {
                            URLHelper.Redirect(currentUrl);
                        }
                    }
                    // Display error which occurred during authentication process
                    else if (!String.IsNullOrEmpty(error))
                    {
                        lblError.Text = error;
                        lblError.Visible = true;
                    }
                    // Otherwise is user disabled
                    else
                    {
                        lblError.Text = GetString("membership.userdisabled");
                        lblError.Visible = true;
                    }
                }
                break;

            // No authentication, do nothing
            case LinkedInHelper.RESPONSE_NOTAUTHENTICATED:
                break;
        }
    }


    /// <summary>
    /// Displays buttons depending on web part settings.
    /// </summary>
    protected void DisplayButtons()
    {
        // If user is already authenticated 
        if (AuthenticationHelper.IsAuthenticated())
        {
            if (ShowSignOut)
            {
                // If text is set use text/button link
                if (!string.IsNullOrEmpty(SignOutText))
                {
                    // Button link
                    if (ShowAsButton)
                    {
                        btnSignOut.Text = SignOutText;
                        btnSignOut.Visible = true;
                    }
                    // Text link
                    else
                    {
                        btnSignOutLink.Text = SignOutText;
                        btnSignOutLink.Visible = true;
                    }
                }
                // Image link
                else
                {
                    btnSignOutImage.ImageUrl = ResolveUrl(SignOutImageURL);
                    btnSignOutImage.Visible = true;
                    btnSignOutImage.ToolTip = GetString("webparts_membership_signoutbutton.signout");
                    btnSignOut.Text = GetString("webparts_membership_signoutbutton.signout");
                }
            }
        }
        else
        {
            // If text is set use text/button link
            if (!string.IsNullOrEmpty(SignInText))
            {
                // Button link
                if (ShowAsButton)
                {
                    btnSignIn.Text = SignInText;
                    btnSignIn.Visible = true;
                }
                // Text link
                else
                {
                    btnSignInLink.Text = SignInText;
                    btnSignInLink.Visible = true;
                }
            }
            // Image link
            else
            {
                btnSignInImage.ImageUrl = ResolveUrl(SignInImageURL);
                btnSignInImage.Visible = true;
                btnSignInImage.ToolTip = GetString("webparts_membership_signoutbutton.signin");
                btnSignIn.Text = GetString("webparts_membership_signoutbutton.signin");
            }
        }
    }


    /// <summary>
    /// Sign in button event.
    /// </summary>
    protected void btnSignIn_Click(object sender, EventArgs e)
    {
        var requestParameters = new Dictionary<string, string>
        {
            { "scope", RequireBirthDate ? LinkedInHelper.FULL_PROFILE_PERMISSION_LEVEL : LinkedInHelper.BASIC_PROFILE_PERMISSION_LEVEL}
        };
        linkedInHelper.SendRequest(requestParameters);
    }


    /// <summary>
    /// Sign out button event.
    /// </summary>
    protected void btnSignOut_Click(object sender, EventArgs e)
    {
        if (AuthenticationHelper.IsAuthenticated())
        {
            // Sign out from CMS
            AuthenticationHelper.SignOut();

            Response.Cache.SetNoStore();

            // Clear used session
            SessionHelper.Remove(SESSION_NAME_USERDATA);

            // Redirect to return URL
            string returnUrl = QueryHelper.GetString("returnurl", RequestContext.CurrentURL);
            URLHelper.Redirect(URLHelper.GetAbsoluteUrl(HttpUtility.UrlDecode(returnUrl)));
        }
    }


    /// <summary>
    /// Displays warning message in "Design mode".
    /// </summary>
    /// <param name="message">Message that will be displayed. Default misconfiration message is used when no parameter is given.</param>
    private bool DisplayMessage(string message = null)
    {
        // Error label is displayed in Design mode when LinkedIn is disabled
        if (PortalContext.IsDesignMode(PortalContext.ViewMode))
        {
            if (String.IsNullOrEmpty(message))
            {
                // Default message informing about misconfiguration is dispalyed.
                StringBuilder parameter = new StringBuilder();
                parameter.Append(UIElementInfoProvider.GetApplicationNavigationString("cms", "Settings") + " -> ");
                parameter.Append(GetString("settingscategory.socialmedia") + " -> ");
                parameter.Append(GetString("settingscategory.cmslinkedin"));
                if (MembershipContext.AuthenticatedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin))
                {
                    // Make it link for Admin
                    parameter.Insert(0, "<a href=\"" + URLHelper.GetAbsoluteUrl(UIContextHelper.GetApplicationUrl("cms", "settings")) + "\" target=\"_top\">");
                    parameter.Append("</a>");
                }

                message = String.Format(GetString("mem.linkedin.disabled"), parameter.ToString());
            }
            lblError.Text = message;
            lblError.Visible = true;
        }

        return lblError.Visible;
    }

    #endregion
}