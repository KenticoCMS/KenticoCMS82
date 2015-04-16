using System;
using System.Linq;
using System.Text;

using CMS.Helpers;
using CMS.Base;
using CMS.SocialMedia;


public partial class CMSWebParts_SocialMedia_Twitter_TwitterFeed : SocialMediaAbstractWebPart
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
            ltlPluginCode.Visible = !value;
        }
    }



    /// <summary>
    /// Username.
    /// </summary>
    public string Username
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Username"), string.Empty);
        }
        set
        {
            SetValue("Username", value);
        }
    }



    /// <summary>
    /// Web Intent Related Users: As per the Tweet and follow buttons, you may provide a comma-separated
    /// list of user screen names as suggested followers to a user after they reply, Retweet, or 
    /// favorite a Tweet in the timeline.
    /// </summary>
    public string RelatedUsers
    {
        get
        {
            return ValidationHelper.GetString(GetValue("RelatedUsers"), string.Empty);
        }
        set
        {
            SetValue("RelatedUsers", value);
        }
    }


    /// <summary>
    /// Twitter widget Id.
    /// </summary>
    public string WidgetID
    {
        get
        {
            return ValidationHelper.GetString(GetValue("WidgetID"), string.Empty);
        }
        set
        {
            SetValue("WidgetID", value);
        }
    }


    /// <summary>
    /// Width of the web part in pixels.
    /// </summary>
    public int Width
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("Width"), -1);
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
            return ValidationHelper.GetInteger(GetValue("Height"), -1);
        }
        set
        {
            SetValue("Height", value);
        }
    }


    /// <summary>
    /// Number of tweets to display.
    /// </summary>
    public int NumberOfTweets
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("NumberOfTweets"), -1);
        }
        set
        {
            SetValue("NumberOfTweets", value);
        }
    }


    /// <summary>
    /// Whether to show scrollbar.
    /// </summary>
    public bool Scrollbar
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("Scrollbar"), false);
        }
        set
        {
            SetValue("Scrollbar", value);
        }
    }


    /// <summary>
    /// Link color.
    /// When not set, the default is used (set in widget's configuration on Twitter).
    /// </summary>
    public string LinkColor
    {
        get
        {
            return ValidationHelper.GetString(GetValue("LinkColor"), String.Empty);
        }
        set
        {
            SetValue("LinkColor", value);
        }
    }


    /// <summary>
    /// Indicates whether to show background
    /// </summary>
    public bool Transparent
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("Transparent"), false);
        }
        set
        {
            SetValue("Transparent", value);
        }
    }


    /// <summary>
    /// Indicates whether borders should be displayed
    /// </summary>
    public bool DisplayBorders
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayBorders"), false);
        }
        set
        {
            SetValue("DisplayBorders", value);
        }
    }


    /// <summary>
    /// Indicates whether footer should be displayed
    /// </summary>
    public bool DisplayFooter
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayFooter"), false);
        }
        set
        {
            SetValue("DisplayFooter", value);
        }
    }


    /// <summary>
    /// Indicates whether replies should be displayed
    /// </summary>
    public bool ShowReplies
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowReplies"), false);
        }
        set
        {
            SetValue("ShowReplies", value);
        }
    }


    /// <summary>
    /// Indicates whether header should be displayed
    /// </summary>
    public bool DisplayHeader
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayHeader"), false);
        }
        set
        {
            SetValue("DisplayHeader", value);
        }
    }


    /// <summary>
    /// Indicates whether light or dark theme should be used.
    /// When not set, the default is used (set in widget's configuration on Twitter).
    /// </summary>
    public string Theme
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Theme"), String.Empty);
        }
        set
        {
            SetValue("Theme", value);
        }
    }


    /// <summary>
    /// Border color
    /// </summary>
    public string BorderColor
    {
        get
        {
            return ValidationHelper.GetString(GetValue("BorderColor"), String.Empty);
        }
        set
        {
            SetValue("BorderColor", value);
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
        }
        else
        {
            StringBuilder sb = new StringBuilder();

            //Configure widget
            sb.Append("<a class=\"twitter-timeline\"  href=\"https://twitter.com/", HTMLHelper.EncodeForHtmlAttribute(URLHelper.URLEncode(Username)), "\"  data-widget-id=\"", HTMLHelper.EncodeForHtmlAttribute(WidgetID));

            //Chrome setup
            StringBuilder chromeString = new StringBuilder();
            chromeString.Append(Transparent ? "transparent " : "");
            chromeString.Append(!DisplayBorders ? "noborders " : "");
            chromeString.Append(!DisplayHeader ? "noheader " : "");
            chromeString.Append(!DisplayFooter ? "nofooter " : "");
            chromeString.Append(!Scrollbar ? "noscrollbar " : "");

            if (!String.IsNullOrEmpty(chromeString.ToString()))
            {
                sb.Append("\" data-chrome=\"", chromeString.ToString().Trim());
            }

            if (Width > 0)
            {
                sb.Append("\" width=\"", Width);
            }

            if (Height > 0)
            {
                sb.Append("\" height=\"", Height);
            }

            if (NumberOfTweets > 0)
            {
                sb.Append("\" data-tweet-limit=\"", NumberOfTweets);
            }

            sb.Append("\" data-show-replies=\"", ShowReplies ? "true" : "false");
            sb.Append("\" data-screen-name=\"", HTMLHelper.EncodeForHtmlAttribute(Username));

            if (!String.IsNullOrEmpty(Theme))
            {
                sb.Append("\" data-theme=\"", Theme.ToLowerCSafe());
            }

            if (!String.IsNullOrEmpty(LinkColor))
            {
                sb.Append("\" data-link-color=\"", LinkColor.ToLowerCSafe());
            }

            if (!String.IsNullOrWhiteSpace(RelatedUsers))
            {
                sb.Append("\" data-related=\"", HTMLHelper.EncodeForHtmlAttribute(RelatedUsers));
            }

            if (!String.IsNullOrEmpty(BorderColor))
            {
                sb.Append("\" data-border-color=\"", BorderColor.ToLowerCSafe());
            }

            sb.Append("\">Tweets by @", HTMLHelper.HTMLEncode(Username), "</a>\n");

            //widget generating script
            sb.Append("<script type=\"text/javascript\">window.twttr=(function(d,s,id){var t,js,fjs=d.getElementsByTagName(s)[0];if(d.getElementById(id))return;js=d.createElement(s);js.id=id;js.src=\"https://platform.twitter.com/widgets.js\";fjs.parentNode.insertBefore(js,fjs);return window.twttr||(t={_e:[],ready:function(f){t._e.push(f)}});}(document,\"script\",\"twitter-wjs\"));</script>");

            // Set plugin code
            ltlPluginCode.Text = sb.ToString();
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