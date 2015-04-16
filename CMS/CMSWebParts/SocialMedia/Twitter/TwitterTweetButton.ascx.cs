using System;
using System.Linq;
using System.Text;

using CMS.Helpers;
using CMS.Base;
using CMS.DocumentEngine;
using CMS.SocialMedia;


public partial class CMSWebParts_SocialMedia_Twitter_TwitterTweetButton : SocialMediaAbstractWebPart
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
            ltlTweetButtonCode.Visible = !value;
        }
    }



    /// <summary>
    /// Type of the button.
    /// </summary>
    public string Type
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Type"), string.Empty);
        }
        set
        {
            SetValue("Type", value);
        }
    }


    /// <summary>
    /// Url to share - for share type.
    /// </summary>
    public string UrlToShare
    {
        get
        {
            return ValidationHelper.GetString(GetValue("UrlToShare"), string.Empty);
        }
        set
        {
            SetValue("UrlToShare", value);
        }
    }


    /// <summary>
    /// Default tweet text.
    /// </summary>
    public string TweetText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("TweetText"), string.Empty);
        }
        set
        {
            SetValue("TweetText", value);
        }
    }


    /// <summary>
    /// The language for the button.
    /// </summary>
    public string Language
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Language"), string.Empty);
        }
        set
        {
            SetValue("Language", value);
        }
    }


    /// <summary>
    /// Count URL - for share type.
    /// </summary>
    public string CountUrl
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CountUrl"), string.Empty);
        }
        set
        {
            SetValue("CountUrl", value);
        }
    }



    /// <summary>
    /// Screen name of the user to attribute the Tweet to.
    /// </summary>
    public string Via
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Via"), string.Empty);
        }
        set
        {
            SetValue("Via", value);
        }
    }


    /// <summary>
    /// Related accounts.
    /// </summary>
    public string RelatedAccounts
    {
        get
        {
            return ValidationHelper.GetString(GetValue("RelatedAccounts"), string.Empty);
        }
        set
        {
            SetValue("RelatedAccounts", value);
        }
    }


    /// <summary>
    /// Whether to show count - for share type.
    /// </summary>
    public bool ShowCount
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowCount"), false);
        }
        set
        {
            SetValue("ShowCount", value);
        }
    }


    /// <summary>
    /// User to mention - for mention type.
    /// </summary>
    public string UserToMention
    {
        get
        {
            return ValidationHelper.GetString(GetValue("UserToMention"), string.Empty);
        }
        set
        {
            SetValue("UserToMention", value);
        }
    }


    /// <summary>
    /// Button hashtag - for hashtag type.
    /// </summary>
    public string ButtonHashtag
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ButtonHashtag"), string.Empty);
        }
        set
        {
            SetValue("ButtonHashtag", value);
        }
    }


    /// <summary>
    /// Hashtags.
    /// </summary>
    public string Hashtags
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Hashtags"), string.Empty);
        }
        set
        {
            SetValue("Hashtags", value);
        }
    }


    /// <summary>
    /// Size for the button.
    /// </summary>
    public string Size
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Size"), string.Empty);
        }
        set
        {
            SetValue("Size", value);
        }
    }


    /// <summary>
    /// Width of the web part in pixels.
    /// </summary>
    public int Width
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("Width"), 110);
        }
        set
        {
            SetValue("Width", value);
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
    protected override void SetupControl()
    {
        if (StopProcessing)
        {
            // Do not process
            return;
        }
        
        if (string.IsNullOrEmpty(Language))
        {
            Language = DocumentContext.CurrentDocumentCulture.CultureCode;
        }

        if (UseHTML5)
        {
            RenderPluginHTML5();
        }
        else
        {
            RenderPlugin();
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


    #region "Private methods"
    
    /// <summary>
    /// Renders HTML5 version of Twitter plugin on the page.
    /// </summary>
    private void RenderPluginHTML5()
    {
        string serviceUrl = null;

        string url = UrlToShare;
        url = URLHelper.GetAbsoluteUrl(String.IsNullOrEmpty(url) ? RequestContext.CurrentURL : ResolveUrl(url));
        url = HTMLHelper.EncodeForHtmlAttribute(url);

        StringBuilder sb = new StringBuilder();
        switch (Type.ToLowerCSafe())
        {
            case "share":
                serviceUrl = "https://twitter.com/share";
                sb.Append("<a href=\"", serviceUrl, "\" class=\"twitter-share-button\" data-count=\"", ShowCount ? "horizontal" : "none", "\"");
                if (!string.IsNullOrEmpty(CountUrl))
                {
                    sb.Append(" data-counturl=\"", HTMLHelper.EncodeForHtmlAttribute(CountUrl), "\"");
                }

                sb.Append(" data-url=\"", url, "\"");
                break;

            // If type is hashtag
            case "hashtag":
                serviceUrl = "https://twitter.com/intent/tweet";
                serviceUrl = URLHelper.AddUrlParameter(serviceUrl, "button_hashtag", ButtonHashtag);
                serviceUrl = HTMLHelper.EncodeForHtmlAttribute(serviceUrl);
                sb.Append("<a href=\"", serviceUrl, "\" class=\"twitter-hashtag-button\"");
                break;

            // If type is mention
            case "mention":
                serviceUrl = "https://twitter.com/intent/tweet";
                serviceUrl = URLHelper.AddUrlParameter(serviceUrl, "screen_name", URLHelper.URLEncode(UserToMention));
                serviceUrl = HTMLHelper.EncodeForHtmlAttribute(serviceUrl);
                sb.Append("<a href=\"", serviceUrl, "\" class=\"twitter-mention-button\"");
                break;
        }

        if (!string.IsNullOrEmpty(TweetText))
        {
            sb.Append(" data-text=\"", HTMLHelper.EncodeForHtmlAttribute(TweetText), "\"");
        }

        sb.Append(" data-lang=\"", HTMLHelper.EncodeForHtmlAttribute(Language), "\" data-width=\"", Width, "px\" data-size=\"", (Size.EqualsCSafe("m") ? "medium" : "large"), "\"");

        if (!string.IsNullOrEmpty(Via))
        {
            sb.Append(" data-via=\"", HTMLHelper.EncodeForHtmlAttribute(Via), "\"");
        }

        if (!string.IsNullOrEmpty(RelatedAccounts))
        {
            sb.Append(" data-related=\"", HTMLHelper.EncodeForHtmlAttribute(RelatedAccounts), "\"");
        }

        if (!string.IsNullOrEmpty(Hashtags))
        {
            sb.Append(" data-hashtags=\"", HTMLHelper.EncodeForHtmlAttribute(Hashtags), "\"");
        }
        sb.Append("></a>");

        // Register Twitter widget API
        ltlTweetButtonCode.Text = sb.ToString();
        ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "TwitterWidgetsAPI", "<script>!function(d,s,id){var js,fjs=d.getElementsByTagName(s)[0];if(!d.getElementById(id)){js=d.createElement(s);js.id=id;js.src=\"http://platform.twitter.com/widgets.js\";fjs.parentNode.insertBefore(js,fjs);}}(document,\"script\",\"twitter-wjs\");</script>");
    }


    /// <summary>
    /// Renders Twitter plugin on the page.
    /// </summary>
    private void RenderPlugin()
    {
        // Build plugin code
        string query = null;
        string src = "http://platform.twitter.com/widgets/tweet_button.html";
        int height = (Size.EqualsCSafe("m")) ? 20 : 28;

        // Check optional parameters
        if (!string.IsNullOrEmpty(TweetText))
        {
            query = URLHelper.AddUrlParameter(query, "text", URLHelper.URLEncode(TweetText));
        }

        if (!string.IsNullOrEmpty(Via))
        {
            query = URLHelper.AddUrlParameter(query, "via", URLHelper.URLEncode(Via));
        }

        if (!string.IsNullOrEmpty(RelatedAccounts))
        {
            query = URLHelper.AddUrlParameter(query, "related", URLHelper.URLEncode(RelatedAccounts));
        }

        if (!string.IsNullOrEmpty(Hashtags))
        {
            query = URLHelper.AddUrlParameter(query, "hashtags", URLHelper.URLEncode(Hashtags));
        }

        query = URLHelper.AddUrlParameter(query, "lang", Language);
        query = URLHelper.AddUrlParameter(query, "size", Size);

        // If type is share
        if (Type.EqualsCSafe("share"))
        {
            query = URLHelper.AddUrlParameter(query, "count", ShowCount ? "horizontal" : "none");

            string url = UrlToShare;
            url = URLHelper.GetAbsoluteUrl(string.IsNullOrEmpty(url) ? RequestContext.CurrentURL : ResolveUrl(url));
            query = URLHelper.AddUrlParameter(query, "url", url);

            if (!string.IsNullOrEmpty(CountUrl))
            {
                query = URLHelper.AddUrlParameter(query, "counturl", URLHelper.URLEncode(CountUrl));
            }
        }

        // If type is hashtag
        if (Type.EqualsCSafe("hashtag"))
        {
            query = URLHelper.AddUrlParameter(query, "type", Type);
            query = URLHelper.AddUrlParameter(query, "button_hashtag", URLHelper.URLEncode(ButtonHashtag));
        }

        // If type is mention
        if (Type.EqualsCSafe("mention"))
        {
            query = URLHelper.AddUrlParameter(query, "type", Type);
            query = URLHelper.AddUrlParameter(query, "screen_name", URLHelper.URLEncode(UserToMention));
        }

        src = HTMLHelper.EncodeForHtmlAttribute(URLHelper.AppendQuery(src, query));

        // Output HTML code
        string output = "<iframe src=\"{0}\" scrolling=\"no\" frameborder=\"0\" allowtransparency=\"true\" style=\"width: {1}px; height: {2}px;\"></iframe>";
        ltlTweetButtonCode.Text = String.Format(output, src, Width, height);
    }

    #endregion
}