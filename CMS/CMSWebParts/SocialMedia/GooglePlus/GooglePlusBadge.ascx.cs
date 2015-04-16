using System;
using System.Web.UI;

using CMS.Helpers;
using CMS.DocumentEngine;
using CMS.SocialMedia;
using System.Text;


public partial class CMSWebParts_SocialMedia_GooglePlus_GooglePlusBadge : SocialMediaAbstractWebPart
{
    #region "Private fiels"

    private bool mHide;
    private string mStyle;
    private string mLink;
    private int mWidth;
    private string mLanguage;
    private string mTheme;
    private bool? mTitlePhoto;
    private bool? mDescription;
    private string mLinkType;

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
    /// Second part of the Google+ page link.
    /// </summary>
    public string PageLink
    {
        get
        {
            return mLink ?? (mLink = ValidationHelper.GetString(GetValue("Link"), string.Empty));
        }
        set
        {
            SetValue("Link", value);
            mLink = value;
        }
    }


    /// <summary>
    /// Provides access to the LinkType property of the WebPart
    /// </summary>
    public string LinkType
    {
        get
        {
            return mLinkType ?? (mLinkType = ValidationHelper.GetString(GetValue("LinkType"), string.Empty));
        }
        set
        {
            SetValue("LinkLinkType", value);
            mLinkType = value;
        }
    }


    /// <summary>
    /// Options:
    /// badge;Standard badge
    /// badgelandscape;Landscape badge
    /// 16;Small icon
    /// 32;Medium icon
    /// 64;Large icon
    /// </summary>
    public string Style
    {
        get
        {
            return mStyle ?? (mStyle = ValidationHelper.GetString(GetValue("Style"), string.Empty));
        }
        set
        {
            SetValue("Style", value);
            mStyle = value;
        }
    }


    /// <summary>
    /// Theme property of the Web part
    /// </summary>
    public string Theme
    {
        get
        {
            return mTheme ?? (mTheme = ValidationHelper.GetString(GetValue("Theme"), string.Empty));
        }
        set
        {
            SetValue("Theme", value);
            mTheme = value;
        }
    }


    /// <summary>
    /// TitlePhoto property of the Web part
    /// </summary>
    public bool TitlePhoto
    {
        get
        {
            return mTitlePhoto.HasValue ? mTitlePhoto.Value : (mTitlePhoto = ValidationHelper.GetBoolean(GetValue("TitlePhoto"), true)).Value;
        }
        set
        {
            SetValue("TitlePhoto", value);
            mTitlePhoto = value;
        }
    }


    /// <summary>
    /// Description property of the Web part
    /// </summary>
    public bool Description
    {
        get
        {
            return mDescription.HasValue ? mDescription.Value : (mDescription = ValidationHelper.GetBoolean(GetValue("Description"), true)).Value;
        }
        set
        {
            SetValue("Description", value);
            mDescription = value;
        }
    }


    /// <summary>
    /// Width of the web part in pixels.
    /// </summary>
    public int Width
    {
        get
        {
            return mWidth < 0 ? mWidth : (mWidth = ValidationHelper.GetInteger(GetValue("Width"), 300));
        }
        set
        {
            SetValue("Width", value);
            mWidth = value;
        }
    }


    /// <summary>
    /// Language.
    /// </summary>
    public string Language
    {
        get
        {
            if (String.IsNullOrEmpty(mLanguage))
            {
                mLanguage = ValidationHelper.GetString(GetValue("Language"), string.Empty);
                if (String.IsNullOrEmpty(mLanguage))
                {
                    mLanguage = DocumentContext.CurrentDocumentCulture.CultureCode;
                    SetValue("Language", mLanguage);
                }
            }
            return mLanguage;
        }
        set
        {
            SetValue("Language", value);
            mLanguage = value;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Initializes the control properties
    /// </summary>
    protected override void SetupControl()
    {
        if (this.StopProcessing)
        {
            // Do not process
        }
        else
        {
            StringBuilder output = new StringBuilder();

            switch (Style)
            {
                case "badge":
                case "badgelandscape":
                    string script = String.Format("<script src=\"https://apis.google.com/js/platform.js\" async defer>{{lang: '{0}'}}</script>", Language);
                    var badge = "<div class=\"g-{5}\" data-width=\"{0}\" data-href=\"{1}\" data-showcoverphoto=\"{2}\" data-showtagline=\"{3}\" data-theme=\"{4}\" data-layout=\"{6}\" data-rel=\"publisher\"></div>";
                    output.Append(script);
                    output.Append(String.Format(badge, Width, HTMLHelper.EncodeForHtmlAttribute(PageLink), TitlePhoto ? "true" : "false", Description ? "true" : "false", Theme, LinkType, Style.EndsWith("landscape")?"landscape":"portrait"));
                    break;
                default:
                    int iconSize = ValidationHelper.GetInteger(Style, 32);
                    var icon = "<a href=\"{0}\" rel=\"publisher\" target=\"_top\" class=\"google-plus-badge-icon\"><img src=\"//ssl.gstatic.com/images/icons/gplus-{1}.png\" alt=\"\" class=\"google-plus-icon{1}\"/></a>";
                    output.Append(String.Format(icon, HTMLHelper.EncodeForHtmlAttribute(PageLink), iconSize));
                    break;
            }

            ltlPluginCode.Text = output.ToString();
        }
    }

    #endregion
}



