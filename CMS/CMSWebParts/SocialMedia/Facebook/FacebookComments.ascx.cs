using System;

using CMS.Helpers;
using CMS.Localization;
using CMS.MembershipProvider;
using CMS.DocumentEngine;
using CMS.SiteProvider;
using CMS.SocialMedia;


public partial class CMSWebParts_SocialMedia_Facebook_FacebookComments : SocialMediaAbstractWebPart
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
            ltlComments.Visible = !value;
        }
    }


    /// <summary>
    /// Url to comment on.
    /// </summary>
    public string Url
    {
        get
        {
            return ValidationHelper.GetString(GetValue("URL"), string.Empty);
        }
        set
        {
            SetValue("URL", value);
        }
    }


    /// <summary>
    /// <para>
    /// Preferred protocol to be forced in <see cref="Url"/>. Allows you to have a single thread of comments
    /// on pages which can be accessed using both HTTP and HTTPS.
    /// </para>
    /// <para>
    /// Valid values are 'http', 'https' or 'none'.
    /// Any other value (including blank) is treated as 'none'.
    /// </para>
    /// </summary>
    public string PreferredProtocol
    {
        get
        {
            return ValidationHelper.GetString(GetValue("PreferredProtocol"), string.Empty);
        }
        set
        {
            SetValue("PreferredProtocol", value);
        }
    }


    /// <summary>
    /// Number of posts.
    /// </summary>
    public int Posts
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("Posts"), 0);
        }
        set
        {
            SetValue("Posts", value);
        }
    }


    /// <summary>
    /// Width of the web part in pixels.
    /// </summary>
    public int Width
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("Width"), 500);
        }
        set
        {
            SetValue("Width", value);
        }
    }


    /// <summary>
    /// Color scheme of the web part.
    /// </summary>
    public string ColorScheme
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ColorScheme"), string.Empty);
        }
        set
        {
            SetValue("ColorScheme", value);
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
            string pageUrl = String.Empty;
            if (string.IsNullOrEmpty(Url))
            {
                // If parameter URL is empty, set URL of current document
                if (DocumentContext.CurrentDocument != null)
                {
                    TreeNode node = DocumentContext.CurrentDocument;
                    pageUrl = DocumentURLProvider.GetUrl(node.NodeAliasPath, node.DocumentUrlPath, SiteContext.CurrentSiteName);
                }
                else
                {
                    // Query string will be removed
                    pageUrl = URLHelper.RemoveQuery(RequestContext.CurrentURL);
                }
            }
            else
            {
                pageUrl = ResolveUrl(Url);
            }
            pageUrl = URLHelper.GetAbsoluteUrl(HTMLHelper.HTMLEncode(pageUrl));
            pageUrl = HandlePreferredProtocol(pageUrl);

            string fbApiKey = FacebookConnectHelper.GetFacebookApiKey(SiteContext.CurrentSiteName);
            if (String.IsNullOrEmpty(fbApiKey))
            {
                ShowError(lblErrorMessage, "socialnetworking.facebook.apikeynotset");
            }
            // Register Facebook javascript SDK
            ScriptHelper.RegisterFacebookJavascriptSDK(Page, LocalizationContext.PreferredCultureCode, fbApiKey);

            if (UseHTML5)
            {
                ltlComments.Text = "<div class=\"fb-comments\" data-href=\"" + URLHelper.GetAbsoluteUrl(pageUrl) + "\" data-num-posts=\"" + Posts + "\" data-width=\"" + Width + "\"" + (!string.IsNullOrEmpty(ColorScheme) ? " data-colorscheme=\"" + ColorScheme + "\"" : "") + "></div>";
            }
            else
            {
                ltlComments.Text = "<fb:comments href=\"" + URLHelper.GetAbsoluteUrl(pageUrl) + "\" num_posts=\"" + Posts + "\" width=\"" + Width + "\"" + (!string.IsNullOrEmpty(ColorScheme) ? " colorscheme=\"" + ColorScheme + "\"" : "") + "></fb:comments>";
            }
        }
    }


    /// <summary>
    /// Based on the <see cref="PreferredProtocol"/> decides, whether to force certain protocol in the absoluteUrl.
    /// </summary>
    /// <param name="absoluteUrl">Absolute URL to be handled.</param>
    /// <returns>Absolute URL with preferred protocol, if preference is set.</returns>
    private string HandlePreferredProtocol(string absoluteUrl)
    {
        string protocolPreferenceLower = PreferredProtocol.ToLower();

        if (protocolPreferenceLower.Equals("http") && absoluteUrl.StartsWith("https:"))
        {
            return "http" + absoluteUrl.Substring(5);
        }
        if (protocolPreferenceLower.Equals("https") && absoluteUrl.StartsWith("http:"))
        {
            return "https" + absoluteUrl.Substring(4);
        }

        return absoluteUrl;
    }

    #endregion
}