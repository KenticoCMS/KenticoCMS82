using System;
using System.Text;
using System.Web.UI.WebControls;
using System.Web;
using System.Web.UI;

using CMS.Helpers;
using CMS.MembershipProvider;
using CMS.Modules;
using CMS.PortalControls;
using CMS.PortalEngine;
using CMS.Base;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.ExtendedControls;
using CMS.DataEngine;
using CMS.DocumentEngine;

public partial class CMSWebParts_Membership_Registration_WindowsLiveID : CMSAbstractWebPart
{
    #region "Constants"

    // Settings key name containing URL to logon page 
    private static readonly string LOGON_PAGE_SETTINGS_KEY = SettingsKeyInfoProvider.GetValue(SiteContext.CurrentSiteName + ".CMSSecuredAreasLogonPage");

    private const string AUTHORIZATION_URL = "https://login.live.com/oauth20_authorize.srf";
    private const string LIVE_CONNECT_API_URL = "https://js.live.net/v5.0/wl.js";

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets the value that indicates whether to show sign out link.
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
    /// Gets or sets the value that buttons will be used instead of links.
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
    /// Gets or sets sign in button image URL.
    /// </summary>
    public string SignInImageURL
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("SignInImageURL"), GetImageUrl("Others/LiveID/signin.gif"));
        }
        set
        {
            SetValue("SignInImageURL", value);
        }
    }


    /// <summary>
    /// Gets or sets sign out button image URL.
    /// </summary>
    public string SignOutImageURL
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("SignOutImageURL"), GetImageUrl("Others/LiveID/signout.gif"));
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
            return ValidationHelper.GetString(GetValue("TrackConversionName"), SettingsKeyInfoProvider.GetValue(SiteContext.CurrentSiteName + ".CMSLiveIDConversionName"));
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
            if (SettingsKeyInfoProvider.GetBoolValue(SiteContext.CurrentSiteName + ".CMSEnableWindowsLiveID"))
            {
                string siteName = SiteContext.CurrentSiteName;
                if (!string.IsNullOrEmpty(siteName))
                {
                    // Get LiveID settings
                    string appId = SettingsKeyInfoProvider.GetValue(siteName + ".CMSApplicationID");
                    string secret = SettingsKeyInfoProvider.GetValue(siteName + ".CMSApplicationSecret");

                    if (!WindowsLiveLogin.UseServerSideAuthorization)
                    {
                        // Add windows live ID script
                        ScriptHelper.RegisterClientScriptInclude(Page, typeof(string), "WLScript", LIVE_CONNECT_API_URL);

                        // Add login functions 
                        String loginLiveIDClientScript = @"

                            function signUserIn() {
                                var scopesArr = ['wl.signin'];
                                WL.login({ scope: scopesArr });
                            }
                    
                            function refreshLiveID(param)
                            {
                                " + ControlsHelper.GetPostBackEventReference(btnHidden, "#").Replace("'#'", "param") + @" 
                            }                                       
                        ";

                        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ClientInitLiveIDScript", ScriptHelper.GetScript(loginLiveIDClientScript));
                    }

                    // Check valid Windows LiveID parameters
                    if ((appId == string.Empty) || (secret == string.Empty))
                    {
                        lblError.Visible = true;
                        lblError.Text = GetString("liveid.incorrectsettings");
                        return;
                    }

                    WindowsLiveLogin wll = new WindowsLiveLogin(appId, secret);

                    // If user is already authenticated 
                    if (AuthenticationHelper.IsAuthenticated())
                    {
                        // If signout should be visible and user has LiveID registered
                        
                        if (ShowSignOut && !String.IsNullOrEmpty(MembershipContext.AuthenticatedUser.UserSettings.WindowsLiveID))
                        {
                            // Get data from auth cookie 
                            string[] userData = AuthenticationHelper.GetUserDataFromAuthCookie();

                            // Check if user has truly logged in by LiveID
                            if ((userData != null) && (Array.IndexOf(userData, "liveidlogin") >= 0))
                            {
                                // Redirect to Windows Live and back to "home" page
                                string defaultAliasPath = SettingsKeyInfoProvider.GetValue(siteName + ".CMSDefaultAliasPath");
                                string url = DocumentURLProvider.GetUrl(defaultAliasPath);
                                string navUrl = wll.GetLogoutUrl(URLHelper.GetAbsoluteUrl(url));

                                // If text is set use text/button link
                                if (!string.IsNullOrEmpty(SignOutText))
                                {
                                    // Button link
                                    if (ShowAsButton)
                                    {
                                        btnSignOut.CommandArgument = navUrl;
                                        btnSignOut.Text = SignOutText;
                                        btnSignOut.Visible = true;
                                    }
                                    // Text link
                                    else
                                    {
                                        btnSignOutLink.CommandArgument = navUrl;
                                        btnSignOutLink.Text = SignOutText;
                                        btnSignOutLink.Visible = true;
                                    }
                                }
                                // Image link
                                else
                                {
                                    btnSignOutImage.CommandArgument = navUrl;
                                    btnSignOutImage.ImageUrl = ResolveUrl(SignOutImageURL);
                                    btnSignOutImage.Visible = true;
                                    btnSignOut.Text = GetString("webparts_membership_signoutbutton.signout");
                                }
                            }
                        }
                        else
                        {
                            Visible = false;
                        }
                    }
                    // Sign In
                    else
                    {

                        // Create return URL
                        string returnUrl = QueryHelper.GetText("returnurl", "");
                        returnUrl = (returnUrl == String.Empty) ? RequestContext.CurrentURL : returnUrl;

                        // Create parameters for LiveID request URL
                        String[] parameters = new String[3];
                        parameters[0] = returnUrl;
                        parameters[1] = TrackConversionName;
                        parameters[2] = ConversionValue.ToString();
                        SessionHelper.SetValue("LiveIDInformtion", parameters);

                        returnUrl = wll.GetLoginUrl();

                        // Get App ID
                        appId = SettingsKeyInfoProvider.GetValue(siteName + ".CMSApplicationID");

                        // Create full LiveID request URL                        
                        string navUrl = AUTHORIZATION_URL + "?client_id=" + appId + "&redirect=true&scope=wl.signin&response_type=code&redirect_uri=" + HttpUtility.UrlEncode(returnUrl);

                        // If text is set use text/button link
                        if (!string.IsNullOrEmpty(SignInText))
                        {
                            // Button link
                            if (ShowAsButton)
                            {
                                AssignButtonControl(navUrl, returnUrl, appId);
                                btnSignIn.Text = SignInText;
                            }
                            // Text link
                            else
                            {
                                AssignHyperlinkControl(navUrl, returnUrl, appId);
                                lnkSignIn.Text = SignInText;
                            }
                        }
                        // Image link
                        else
                        {
                            AssignHyperlinkControl(navUrl, returnUrl, appId);
                            lnkSignIn.ImageUrl = ResolveUrl(SignInImageURL);
                            lnkSignIn.Text = GetString("webparts_membership_signoutbutton.signin");
                        }
                    }
                }
            }
            else
            {
                // Error label is displayed in Design mode when Windows Live ID is disabled
                if (PortalContext.IsDesignMode(PortalContext.ViewMode))
                {
                    StringBuilder parameter = new StringBuilder();
                    parameter.Append(UIElementInfoProvider.GetApplicationNavigationString("cms", "Settings") + " -> ");
                    parameter.Append(GetString("settingscategory.cmsmembership") + " -> ");
                    parameter.Append(GetString("settingscategory.cmsmembershipauthentication") + " -> ");
                    parameter.Append(GetString("settingscategory.cmswindowsliveid"));
                    if (MembershipContext.AuthenticatedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin))
                    {
                        // Make it link for Admin
                        parameter.Insert(0, "<a href=\"" + URLHelper.GetAbsoluteUrl(UIContextHelper.GetApplicationUrl("cms", "settings")) + "\" target=\"_top\">");
                        parameter.Append("</a>");
                    }

                    lblError.Text = String.Format(GetString("mem.liveid.disabled"), parameter);
                    lblError.Visible = true;
                }
                else
                {
                    Visible = false;
                }
            }
        }
    }


    /// <summary>
    /// Assign attributes (url) to hyperlink sign in control
    /// </summary>
    /// <param name="navUrl">Navigation url (login.liveid....)</param>
    /// <param name="retUrl">Return url (usually CMSPages/LiveIDLogin.aspx)</param>
    /// <param name="appID">Application ID</param>
    private void AssignHyperlinkControl(String navUrl, String retUrl, String appID)
    {
        if (!WindowsLiveLogin.UseServerSideAuthorization)
        {
            AddOnClickAttribute(lnkSignIn, appID, retUrl);
        }
        else
        {
            lnkSignIn.NavigateUrl = navUrl;
        }
        lnkSignIn.Visible = true;
    }


    /// <summary>
    /// Assign attributes (url) to button sign in control
    /// </summary>
    /// <param name="navUrl">Navigation url (login.liveid....)</param>
    /// <param name="retUrl">Return url (usually CMSPages/LiveIDLogin.aspx)</param>
    /// <param name="appID">Application ID</param>
    private void AssignButtonControl(String navUrl, String retUrl, String appID)
    {
        if (!WindowsLiveLogin.UseServerSideAuthorization)
        {
            AddOnClickAttribute(btnSignIn, appID, retUrl);
        }
        else
        {
            btnSignIn.CommandArgument = navUrl;
        }
        btnSignIn.Visible = true;
    }


    /// <summary>
    /// Adds on click attribute to control (used for client login)
    /// </summary>
    /// <param name="ctrl">Web control</param>
    /// <param name="appID">Application ID</param>
    /// <param name="retUrl">Return url (usually CMSPages/LiveIDLogin.aspx)</param>
    private void AddOnClickAttribute(WebControl ctrl, String appID, string retUrl)
    {
        ctrl.Attributes["onClick"] = @"
                                            WL.init({
                                                client_id: '" + appID + @"',
                                                redirect_uri: '" + retUrl + @"',
                                                response_type: ""token""
                                            });
                                            signUserIn();";
    }


    /// <summary>
    /// SignOut handler.
    /// </summary>
    protected void btnSignOut_Click(object sender, CommandEventArgs e)
    {
        if (StopProcessing)
        {
            // Do not process
        }
        else
        {
            if (AuthenticationHelper.IsAuthenticated())
            {
                // Sign out from CMS
                AuthenticationHelper.SignOut();
                Response.Cache.SetNoStore();
            }

            // Redirect to LiveID logout
            URLHelper.Redirect(e.CommandArgument.ToString());
        }
    }


    /// <summary>
    /// SignIn handler.
    /// </summary>
    protected void btnSignIn_Click(object sender, CommandEventArgs e)
    {
        if (StopProcessing)
        {
            // Do not process
        }
        else
        {
            // Redirect to sign in to Windows Live
            URLHelper.Redirect(e.CommandArgument.ToString());
        }
    }


    /// <summary>
    /// Manages some actions, after user authorized and page postbacked
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Args</param>
    protected void btnHidden_Click(object sender, EventArgs e)
    {
        string arg = Request[Page.postEventArgumentID];
        switch (arg.ToLowerCSafe())
        {
            case "redirecttoadditionalpage":

                // Get additional page
                string additionalInfoPage = SettingsKeyInfoProvider.GetValue(SiteContext.CurrentSiteName + ".CMSLiveIDRequiredUserDataPage");

                if (!String.IsNullOrEmpty(additionalInfoPage))
                {
                    // Redirect to additional info page
                    URLHelper.Redirect(URLHelper.ResolveUrl(additionalInfoPage));
                }

                break;

            case "clearcookieandredirect":
                WindowsLiveLogin.ClearCookieAndRedirect(LOGON_PAGE_SETTINGS_KEY);
                break;
        }
    }


    /// <summary>
    /// Reloads the control data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
    }

    #endregion
}