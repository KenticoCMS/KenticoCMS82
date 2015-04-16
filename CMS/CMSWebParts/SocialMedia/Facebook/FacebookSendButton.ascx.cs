using System;
using System.Text;
using System.Web.UI;

using CMS.Helpers;
using CMS.MembershipProvider;
using CMS.DocumentEngine;
using CMS.SiteProvider;
using CMS.Localization;
using CMS.SocialMedia;


public partial class CMSWebParts_SocialMedia_Facebook_FacebookSendButton : SocialMediaAbstractWebPart
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
            ltlSendButtonCode.Visible = !value;
        }
    }


    /// <summary>
    /// Url to send.
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
    /// Font of the text in iframe.
    /// </summary>
    public string Font
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Font"), "arial");
        }
        set
        {
            SetValue("Font", value);
        }
    }


    /// <summary>
    /// Color scheme of the button and text.
    /// </summary>
    public string ColorScheme
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ColorScheme"), "light");
        }
        set
        {
            SetValue("ColorScheme", value);
        }
    }


    /// <summary>
    /// Indicates whether to generate meta data tags or not.
    /// </summary>
    public bool GenerateMetaData
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("GenerateMetaData"), false);
        }
        set
        {
            SetValue("GenerateMetaData", value);
        }
    }


    /// <summary>
    /// Meta data title.
    /// </summary>
    public string MetaTitle
    {
        get
        {
            return ValidationHelper.GetString(GetValue("MetaTitle"), "");
        }
        set
        {
            SetValue("MetaTitle", value);
        }
    }


    /// <summary>
    /// Meta data type.
    /// </summary>
    public string MetaType
    {
        get
        {
            return ValidationHelper.GetString(GetValue("MetaType"), "");
        }
        set
        {
            SetValue("MetaType", value);
        }
    }


    /// <summary>
    /// Meta data URL.
    /// </summary>
    public string MetaUrl
    {
        get
        {
            return ValidationHelper.GetString(GetValue("MetaUrl"), "");
        }
        set
        {
            SetValue("MetaUrl", value);
        }
    }


    /// <summary>
    /// Meta data site name.
    /// </summary>
    public string MetaSiteName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("MetaSiteName"), "");
        }
        set
        {
            SetValue("MetaSiteName", value);
        }
    }


    /// <summary>
    /// Meta data image URL.
    /// </summary>
    public string MetaImage
    {
        get
        {
            return ValidationHelper.GetString(GetValue("MetaImage"), "");
        }
        set
        {
            SetValue("MetaImage", value);
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


    #region "Page events"

    /// <summary>
    /// Sets up the control.
    /// </summary>
    protected override void SetupControl()
    {
        if (StopProcessing)
        {
            // Do nothing
        }
        else
        {
            // Get page's URL
            string pageUrl = String.Empty;
            if (string.IsNullOrEmpty(Url))
            {
                if (DocumentContext.CurrentDocument != null)
                {
                    TreeNode node = DocumentContext.CurrentDocument;
                    pageUrl = DocumentURLProvider.GetUrl(node.NodeAliasPath, node.DocumentUrlPath, SiteContext.CurrentSiteName);
                }
                else
                {
                    pageUrl = RequestContext.CurrentURL;
                }
            }
            else
            {
                pageUrl = ResolveUrl(Url);
            }
            pageUrl = URLHelper.GetAbsoluteUrl(HTMLHelper.HTMLEncode(pageUrl));

            string fbApiKey = FacebookConnectHelper.GetFacebookApiKey(SiteContext.CurrentSiteName);
            if (String.IsNullOrEmpty(fbApiKey))
            {
                ShowError(lblErrorMessage, "socialnetworking.facebook.apikeynotset");
            }
            // Register Facebook javascript SDK
            ScriptHelper.RegisterFacebookJavascriptSDK(Page, LocalizationContext.PreferredCultureCode, fbApiKey);

            // Get FB code
            StringBuilder sb = new StringBuilder();
            if (UseHTML5)
            {
                sb.Append("<div class=\"fb-send\" data-href=\"", pageUrl, "\" data-colorscheme=\"", ColorScheme, "\"");
                if (!string.IsNullOrEmpty(Font))
                {
                    sb.Append(" data-font=\"", Font, "\"");
                }
                sb.Append("></div>");
            }
            else
            {
                sb.Append("<fb:send href=\"", pageUrl, "\" font=\"", Font, "\" colorscheme=\"", ColorScheme, "\"></fb:send>");
            }
            ltlSendButtonCode.Text = sb.ToString();
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (StopProcessing)
        {
            // Do nothing
        }
        else
        {
            // Generate meta tags
            if (GenerateMetaData)
            {
                StringBuilder sb = new StringBuilder();

                if (MetaTitle != "")
                {
                    sb.AppendLine("<meta property=\"og:title\" content=\"" + HTMLHelper.HTMLEncode(MetaTitle) + "\" />");
                }
                if (MetaType != "")
                {
                    sb.AppendLine("<meta property=\"og:type\" content=\"" + HTMLHelper.HTMLEncode(MetaType) + "\" />");
                }
                if (MetaSiteName != "")
                {
                    sb.AppendLine("<meta property=\"og:site_name\" content=\"" + HTMLHelper.HTMLEncode(MetaSiteName) + "\" />");
                }
                if (MetaImage != "")
                {
                    sb.AppendLine("<meta property=\"og:image\" content=\"" + URLHelper.GetAbsoluteUrl(ResolveUrl(HTMLHelper.HTMLEncode(MetaImage))) + "\" />");
                }
                if (MetaUrl != "")
                {
                    sb.AppendLine("<meta property=\"og:url\" content=\"" + URLHelper.GetAbsoluteUrl(ResolveUrl(HTMLHelper.HTMLEncode(MetaUrl))) + "\" />");
                }

                Page.Header.Controls.Add(new LiteralControl(sb.ToString()));
            }
        }
    }

    #endregion
}