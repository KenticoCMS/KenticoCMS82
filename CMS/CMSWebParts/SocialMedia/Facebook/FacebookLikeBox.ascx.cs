using System;
using System.Text;

using CMS.Helpers;
using CMS.Localization;
using CMS.MembershipProvider;
using CMS.SiteProvider;
using CMS.SocialMedia;


public partial class CMSWebParts_SocialMedia_Facebook_FacebookLikeBox : SocialMediaAbstractWebPart
{
    #region "Constants"

    private const int heightDefault = 63;
    private const int heightStream = 392;
    private const int heightStreamFaces = 555;
    private const int heightFaces = 255;

    #endregion


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
            ltlLikeBox.Visible = !value;
        }
    }


    /// <summary>
    /// Facebook page URL.
    /// </summary>
    public string FBPageUrl
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FBPageUrl"), "");
        }
        set
        {
            SetValue("FBPageUrl", value);
        }
    }


    /// <summary>
    /// Width of the web part in pixels.
    /// </summary>
    public int Width
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("Width"), 292);
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
            return ValidationHelper.GetString(GetValue("ColorScheme"), "");
        }
        set
        {
            SetValue("ColorScheme", value);
        }
    }


    /// <summary>
    /// Indicates whether to show profile photos or not.
    /// </summary>
    public bool ShowFaces
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowFaces"), true);
        }
        set
        {
            SetValue("ShowFaces", value);
        }
    }


    /// <summary>
    /// Indicates whether to display a stream of the latest posts.
    /// </summary>
    public bool ShowStream
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowStream"), true);
        }
        set
        {
            SetValue("ShowStream", value);
        }
    }


    /// <summary>
    /// Indicates whether to show Facebook header or not.
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
    /// Specifies whether or not to show a border around the plugin. Set to false to style the iframe with your custom CSS.
    /// </summary>
    public bool ShowBorder
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowBorder"), true);
        }
        set
        {
            SetValue("ShowBorder", value);
        }
    }


    /// <summary>
    /// For Places, specifies whether the stream contains posts from the Place's wall or just checkins from friends.
    /// </summary>
    public bool ForceWall
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ForceWall"), false);
        }
        set
        {
            SetValue("ForceWall", value);
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

            // Default height if nothing additional is shown
            int height = heightDefault;

            // If faces and stream are shown
            if (ShowFaces && ShowStream)
            {
                height = heightStreamFaces;
            }
            // If only stream is shown
            else if (ShowStream)
            {
                height = heightStream;
            }
            // If only faces are shown
            else if (ShowFaces)
            {
                height = heightFaces;
            }

            // If stream or faces are shown and header is too
            if (ShowHeader && (ShowFaces || ShowStream))
            {
                height = height + 35;
            }

            if (UseHTML5)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<div class=\"fb-like-box\" data-href=\"", HTMLHelper.EncodeForHtmlAttribute(FBPageUrl),
                          "\" data-width=\"", Width, "\" data-height=\"", height, "\" data-header=\"",
                          ShowHeader, "\" data-stream=\"", ShowStream, "\" data-show-faces=\"",
                          ShowFaces, "\" data-colorscheme=\"", ColorScheme, "\" data-force-wall=\"",
                          ForceWall, "\" data-show-border=\"", ShowBorder, "\"");
                sb.Append("></div>");
                string fbApiKey = FacebookConnectHelper.GetFacebookApiKey(SiteContext.CurrentSiteName);
                if (String.IsNullOrEmpty(fbApiKey))
                {
                    ShowError(lblErrorMessage, "socialnetworking.facebook.apikeynotset");
                }
                // Register Facebook javascript SDK
                ScriptHelper.RegisterFacebookJavascriptSDK(Page, LocalizationContext.PreferredCultureCode, fbApiKey);
                ltlLikeBox.Text = sb.ToString();
            }
            else
            {
                // Iframe code
                string src = "http://www.facebook.com/plugins/likebox.php";

                string query = URLHelper.AddUrlParameter(null, "href", FBPageUrl);
                query = URLHelper.AddUrlParameter(query, "header", ShowHeader.ToString());
                query = URLHelper.AddUrlParameter(query, "width", Width.ToString());
                query = URLHelper.AddUrlParameter(query, "show_faces", ShowFaces.ToString());
                query = URLHelper.AddUrlParameter(query, "stream", ShowStream.ToString());
                query = URLHelper.AddUrlParameter(query, "colorscheme", ColorScheme);
                query = URLHelper.AddUrlParameter(query, "height", height.ToString());
                query = URLHelper.AddUrlParameter(query, "force_wall", ForceWall.ToString());
                query = URLHelper.AddUrlParameter(query, "show_border", ShowBorder.ToString());

                src = HTMLHelper.EncodeForHtmlAttribute(URLHelper.AppendQuery(src, query));

                ltlLikeBox.Text = "<iframe src=\"" + src + "\"";
                ltlLikeBox.Text += " scrolling=\"no\" frameborder=\"0\" style=\"border:none; overflow:hidden; width:" + Width + "px; height:" + height + "px;\"></iframe>";
            }
        }
    }

    #endregion
}