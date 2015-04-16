using System;
using System.Collections;
using System.Globalization;
using System.Web;
using System.Net;
using System.Text;

using CMS.Globalization;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.SocialMarketing;
using CMS.Helpers;
using CMS.EventLog;
using CMS.DataEngine;
using CMS.Core;

public partial class CMSModules_SocialMarketing_Pages_FacebookPageAccessTokenDialog : CMSModalPage
{ 
    protected void Page_Load(object sender, EventArgs e)
    {
        CheckPermissions(ModuleName.SOCIALMARKETING, PermissionsEnum.Read.ToString());
        PageTitle.TitleText = GetString("sm.facebook.accounts.accesstoken");    
        PageTitle.ShowFullScreenButton = false;
        PageTitle.ShowCloseButton = false;

        // Load data from session
        var sessionKey = QueryHelper.GetString("dataKey", "");
        if (String.IsNullOrEmpty(sessionKey))
        {
            ShowError(GetString("dialogs.badhashtext"));
            return;
        }

        var parameters = WindowHelper.GetItem(sessionKey) as Hashtable;
        if (parameters == null)
        {
            ShowError(GetString("dialogs.badhashtext"));
            return;
        }

        string redirectUrl = String.Format("{0}?dataKey={1}&redirected=1", URLHelper.GetAbsoluteUrl("~/CMSModules/SocialMarketing/Pages/FacebookPageAccessTokenDialog.aspx"), sessionKey);
        redirectUrl = HttpContext.Current.Server.UrlEncode(redirectUrl);
        
        string code = QueryHelper.GetString("code", String.Empty);
        bool redirected = QueryHelper.GetBoolean("redirected", false);
        if (!redirected)
        {
            // First time on this page - need to ask Facebook for code - will get redirected back as soon as user confirms permissions
            Response.Redirect(String.Format(FacebookHelper.OAUTH_AUTHORIZE_URL, parameters["AppId"], redirectUrl), true);
        }
        else if (!String.IsNullOrEmpty(code))
        {
            // Second request on this site, already with code parameter
            GetPageAccessToken(parameters, code, redirectUrl); 
        }
        else
        {
            // It was not possible to get code
            ShowError(GetString("sm.facebook.account.msg.pageaccesstokenfail"));
        }
    }


    /// <summary>
    /// Gets page access token and stores it in the parent window.
    /// </summary>
    /// <param name="parameters">Parameters retrieved form parent window.</param>
    /// <param name="code">Code retrieved from Facebook.</param>
    /// <param name="redirectUrl">Redirect url that was used for retrieving the code from Facebook.</param>
    private void GetPageAccessToken(Hashtable parameters, string code, string redirectUrl)
    {
        string appId = (string) parameters["AppId"];
        string appSecret = (string) parameters["AppSecret"];
        string pageId = (string) parameters["PageId"];
        string tokenControlId = (string) parameters["TokenCntId"];
        string tokenExpirationControlId = (string) parameters["TokenExpirationCntId"];
        string infoLabelId = (string) parameters["InfoLblId"];
        string tokenPageIdControlId = (string) parameters["TokenPageIdCntId"];
        string tokenAppIdControlId = (string) parameters["TokenAppIdCntId"];
        string tokenAppInfoId = (string) parameters["TokenAppInfoId"];
        
        WebClient wc = new WebClient();
        string request = String.Format(FacebookHelper.OAUTH_ACCESS_TOKEN_URL, appId, redirectUrl, appSecret, code);

        try
        {
            string accessToken = wc.DownloadString(request);
            DateTime expiration = new DateTime();
            bool expirationSet = false;

            // Get values from response
            string[] pairs = accessToken.Split('&');

            foreach (var pair in pairs)
            {
                string[] keyValue = pair.Split('=');

                if (keyValue[0].CompareTo("access_token") == 0)
                {
                    accessToken = keyValue[1];
                }
                else if (keyValue[0].CompareTo("expires") == 0)
                {
                    expiration = DateTime.UtcNow + TimeSpan.FromSeconds(Convert.ToInt32(keyValue[1]));
                    expirationSet = true;
                }
            }

            // Get page access token - posting will be done under page identity
            string pageAccessToken = FacebookHelper.GetPageAccessToken(accessToken, pageId);

            // Check if page access token was retrieved correctly
            if (pageAccessToken == null)
            {
                ShowError(GetString("sm.facebook.account.msg.pageaccesstokenfail"));
            }
            else
            {
                string formattedExpiration = String.Empty;
                if (expirationSet)
                {
                    formattedExpiration = TimeZoneHelper.ConvertToUserTimeZone(expiration.ToLocalTime(), true, MembershipContext.AuthenticatedUser, SiteContext.CurrentSite);
                }

                // Set retrieved access token to the opener window
                string script = String.Format(@"
if(wopener.fbSetAccessToken) {{
    wopener.fbSetAccessToken(
        {{
            tokenControlId: '{0}',
            expirationControlId: '{1}', 
            infoLabelId: '{2}', 
            pageIdControlId: '{3}',
            appIdControlId: '{4}',
            accessToken: '{5}',
            tokenPageId: '{6}', 
            tokenAppId: '{7}', 
            tokenExpiration: '{8}', 
            tokenExpirationString: '{9}'
        }}
    ); 
    CloseDialog();
}}",
                    tokenControlId,
                    tokenExpirationControlId,
                    infoLabelId,
                    tokenPageIdControlId,
                    tokenAppIdControlId,
                    pageAccessToken,
                    pageId,
                    tokenAppInfoId,
                    expirationSet ? expiration.ToString("g", CultureInfo.InvariantCulture) : String.Empty,
                    formattedExpiration
                    );
                ScriptHelper.RegisterStartupScript(Page, typeof(string), "TokenScript", script, true);
            }
        }
        catch (WebException ex)
        {
            // Log exception
            EventLogProvider.LogException("FacebookPageAccessToken", "Retrieval", ex, SiteContext.CurrentSiteID);

            // Show exception message
            ShowError(GetString("sm.facebook.account.msg.connectionfail") + ex.Message);
        }
    }
}
