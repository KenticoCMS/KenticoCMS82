using System;
using System.Text;

using CMS.Helpers;
using CMS.Localization;
using CMS.MembershipProvider;
using CMS.SiteProvider;
using CMS.SocialMedia;


public partial class CMSWebParts_SocialMedia_Facebook_FacebookFacepile : SocialMediaAbstractWebPart
{
    #region "Constants"

    private const int DEFAULT_WIDTH = 200;

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
            ltlFacePile.Visible = !value;
        }
    }


    /// <summary>
    /// Facebook page URL
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
    /// Width of the web part in pixels
    /// </summary>
    public int Width
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("Width"), DEFAULT_WIDTH);
        }
        set
        {
            SetValue("Width", value);
        }
    }


    /// <summary>
    /// Size of the web part
    /// </summary>
    public string Size
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Size"), "");
        }
        set
        {
            SetValue("Size", value);
        }
    }


    /// <summary>
    /// Maximum number of rows with faces
    /// </summary>
    public int RowsNumber
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("RowsNumber"), 1);
        }
        set
        {
            SetValue("RowsNumber", value);
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
    /// Initializes the control properties
    /// </summary>
    protected override void SetupControl()
    {
        if (StopProcessing)
        {
            // Do not process
        }
        else
        {
            if (UseHTML5)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<div class=\"fb-facepile\" data-href=\"", HTMLHelper.EncodeForHtmlAttribute(FBPageUrl),
                          "\" data-size=\"", Size, "\" data-max-rows=\"", RowsNumber, "\" data-width=\"",
                          Width, "\" data-colorscheme=\"", ColorScheme, "\"></div>");

                string fbApiKey = FacebookConnectHelper.GetFacebookApiKey(SiteContext.CurrentSiteName);
                if (String.IsNullOrEmpty(fbApiKey))
                {
                    ShowError(lblErrorMessage, "socialnetworking.facebook.apikeynotset");
                }
                // Register Facebook javascript SDK
                ScriptHelper.RegisterFacebookJavascriptSDK(Page, LocalizationContext.PreferredCultureCode, fbApiKey);
                ltlFacePile.Text = sb.ToString();
            }
            else
            {

                // Iframe code
                string query = null;
                string src = "http://www.facebook.com/plugins/facepile.php";

                query = URLHelper.AddUrlParameter(query, "href", FBPageUrl);
                query = URLHelper.AddUrlParameter(query, "size", Size);
                query = URLHelper.AddUrlParameter(query, "width", Width.ToString());
                query = URLHelper.AddUrlParameter(query, "max_rows", RowsNumber.ToString());
                query = URLHelper.AddUrlParameter(query, "colorscheme", ColorScheme);

                src = HTMLHelper.EncodeForHtmlAttribute(URLHelper.AppendQuery(src, query));

                ltlFacePile.Text = "<iframe src=\"" + src + "\"";
                ltlFacePile.Text += " scrolling=\"no\" frameborder=\"0\" style=\"border:none; overflow:hidden; width:" + Width + "px;\"></iframe>";
            }
        }
    }

    #endregion
}