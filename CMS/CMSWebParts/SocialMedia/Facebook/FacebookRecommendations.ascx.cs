using System;
using System.Text;

using CMS.Helpers;
using CMS.Localization;
using CMS.MembershipProvider;
using CMS.SiteProvider;
using CMS.SocialMedia;


public partial class CMSWebParts_SocialMedia_Facebook_FacebookRecommendations : SocialMediaAbstractWebPart
{
    #region "Private fiels"

    private bool mHide;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Indicates whether to hide content of the WebPart
    /// </summary>
    public override bool HideContent
    {
        get
        {
            return mHide;
        }
        set
        {
            mHide = value;
            ltlRecommendations.Visible = !value;
        }
    }


    /// <summary>
    /// The domain to show recommandations for.
    /// </summary>
    public string Domain
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Domain"), "");
        }
        set
        {
            SetValue("Domain", value);
        }
    }


    /// <summary>
    /// Reference parameter.
    /// </summary>
    public string RefParameter
    {
        get
        {
            return ValidationHelper.GetString(GetValue("RefParameter"), "");
        }
        set
        {
            SetValue("RefParameter", value);
        }
    }


    /// <summary>
    /// Width of the web part in pixels.
    /// </summary>
    public int Width
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("Width"), 300);
        }
        set
        {
            SetValue("Width", value);
        }
    }


    /// <summary>
    /// Height of the web part in pixels.
    /// </summary>
    public int Height
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("Height"), 300);
        }
        set
        {
            SetValue("Height", value);
        }
    }


    /// <summary>
    /// Indicates whether to show facebook header or not.
    /// </summary>
    public bool ShowHeader
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowHeader"), true);
        }
        set
        {
            SetValue("ShowHeader", value);
        }
    }


    /// <summary>
    /// Color scheme of the web part.
    /// </summary>
    public string ColorScheme
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ColorScheme"), "");
        }
        set
        {
            SetValue("ColorScheme", value);
        }
    }


    /// <summary>
    /// Font of the web part.
    /// </summary>
    public string Font
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Font"), "");
        }
        set
        {
            SetValue("Font", value);
        }
    }


    /// <summary>
    /// Indicates if HTML 5 output should be generated.
    /// </summary>
    public bool UseHTML5
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("UseHTML5"), false);
        }
        set
        {
            SetValue("UseHTML5", value);
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected override void SetupControl()
    {
        if (StopProcessing)
        {
            // Do not process
        }
        else
        {
            if (string.IsNullOrEmpty(Domain))
            {
                Domain = SiteContext.CurrentSite.DomainName;
            }

            if (UseHTML5)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<div class=\"fb-recommendations\" data-width=\"", Width, "\" data-height=\"", Height,"\" data-site=\"",
                           Domain, "\" data-header=\"", ShowHeader, "\" data-colorscheme=\"", ColorScheme, "\"");

                if (!string.IsNullOrEmpty(Font))
                {
                    sb.Append(" data-font=\"", Font, "\"");
                }
                if (!string.IsNullOrEmpty(RefParameter))
                {
                    sb.Append(" data-ref=\"", RefParameter, "\"");
                }
                sb.Append("></div>");

                string fbApiKey = FacebookConnectHelper.GetFacebookApiKey(SiteContext.CurrentSiteName);
                if (String.IsNullOrEmpty(fbApiKey))
                {
                    ShowError(lblErrorMessage, "socialnetworking.facebook.apikeynotset");
                }
                // Register Facebook javascript SDK
                ScriptHelper.RegisterFacebookJavascriptSDK(Page, LocalizationContext.PreferredCultureCode, fbApiKey);
                ltlRecommendations.Text = sb.ToString();
            }
            else
            {
                // Iframe code
                string query = null;
                string src = "http://www.facebook.com/plugins/recommendations.php";

                if (!string.IsNullOrEmpty(Font))
                {
                    query = URLHelper.AddUrlParameter(query, "font", Font);
                }

                if (!string.IsNullOrEmpty(RefParameter))
                {
                    query = URLHelper.AddUrlParameter(query, "ref", RefParameter);
                }

                query = URLHelper.AddUrlParameter(query, "site", Domain);
                query = URLHelper.AddUrlParameter(query, "header", ShowHeader.ToString());
                query = URLHelper.AddUrlParameter(query, "width", Width.ToString());
                query = URLHelper.AddUrlParameter(query, "colorscheme", ColorScheme);
                query = URLHelper.AddUrlParameter(query, "height", Height.ToString());

                src = HTMLHelper.EncodeForHtmlAttribute(URLHelper.AppendQuery(src, query));

                ltlRecommendations.Text = "<iframe src=\"" + src + "\"";
                ltlRecommendations.Text += " scrolling=\"no\" frameborder=\"0\" style=\"border:none; overflow:hidden; width:" + Width + "px; height:" + Height + "px;\"></iframe>";
            }
        }
    }

    #endregion
}