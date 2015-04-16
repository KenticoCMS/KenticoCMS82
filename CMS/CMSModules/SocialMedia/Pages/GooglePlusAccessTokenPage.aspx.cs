using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CMS.Base;
using CMS.Helpers;
using CMS.SocialMedia;
using CMS.UIControls;

public partial class CMSModules_SocialMedia_Pages_GooglePlusAccessTokenPage : CMSModalPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        PageTitle.TitleText = ResHelper.GetString("socialnetworking.googleplus.accesstoken");
        PageTitle.ShowFullScreenButton = false;
        PageTitle.ShowCloseButton = false;

        string clientID = Request.QueryString["token_client_id"];
        string clientSecret = Request.QueryString["token_client_secret"];
        string error = Request.QueryString["error"];
        string state = Request.QueryString["state"];

        // Check Social networking DLL and settings
        if (!SystemContext.IsFullTrustLevel)
        {
            lblStatus.Text = ResHelper.GetString("socialnetworking.fulltrustrequired");
        }
        else if (String.IsNullOrEmpty(state) && (String.IsNullOrEmpty(clientID) || String.IsNullOrEmpty(clientSecret)))
        {
            lblStatus.Text = ResHelper.GetString("socialnetworking.googleplus.apisettingsmissing");
        }
        else
        {
            // If access denied
            if (error.EqualsCSafe("access_denied"))
            {
                // Close the window
                StringBuilder script = new StringBuilder("if(wopener.setAccessTokenToTextBox){CloseDialog();}");

                ScriptHelper.RegisterStartupScript(Page, typeof(string), "TokenScript", ScriptHelper.GetScript(script.ToString()));
            }
            else
            {
                // If this is OAuth callback -> get clientID and clientSecret from state
                if (state != null)
                {
                    foreach (string s in state.Split(new string[1] { "&" }, StringSplitOptions.None))
                    {
                        if (s.StartsWithCSafe("token_client_id="))
                        {
                            clientID = s.Substring(16);
                        }

                        if (s.StartsWithCSafe("token_client_secret="))
                        {
                            clientSecret = s.Substring(20);
                        }
                    }
                }

                try
                {
                    // Authenticate and retrieve tokens
                    Dictionary<string, string> tokens = GooglePlusProvider.Authorize(clientID, clientSecret);
                    if (tokens.Count > 0)
                    {
                        string accessToken = tokens["AccessToken"];
                        string refreshToken = tokens["RefreshToken"];

                        if (!String.IsNullOrEmpty(accessToken))
                        {
                            // Extract txtToken values from state
                            string txtToken = null;
                            string txtTokenSecret = null;
                            string[] stateParams = state.Split(new string[1]
                            {
                                "&"
                            }, StringSplitOptions.None);
                            foreach (string s in stateParams)
                            {
                                if (s.StartsWithCSafe("txtToken="))
                                {
                                    txtToken = s.Substring(9);
                                }

                                if (s.StartsWithCSafe("txtTokenSecret="))
                                {
                                    txtTokenSecret = s.Substring(15);
                                }
                            }

                            // Return access token values and close the window
                            StringBuilder script = new StringBuilder("if(wopener.setAccessTokenToTextBox){wopener.setAccessTokenToTextBox('");
                            script.Append(txtToken);
                            script.Append("', '");
                            script.Append(accessToken);
                            script.Append("', '");
                            script.Append(txtTokenSecret);
                            script.Append("', '");
                            script.Append(refreshToken);
                            script.Append("');");
                            script.Append("CloseDialog();}");
                            ScriptHelper.RegisterStartupScript(Page, typeof (string), "TokenScript", ScriptHelper.GetScript(script.ToString()));
                        }
                    }
                    else
                    {
                        // There was an error somewhere
                        lblStatus.Text = ResHelper.GetString("socialnetworking.authorizationerror");
                    }
                }
                catch (Exception ex)
                {
                    LogAndShowError("SocialMedia", "GooglePlusProvider", ex);
                }
            }
        }
    }
}