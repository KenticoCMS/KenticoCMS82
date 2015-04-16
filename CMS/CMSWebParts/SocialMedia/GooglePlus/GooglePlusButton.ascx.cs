using System;
using System.Text;
using System.Web.UI;

using CMS.Base;
using CMS.Helpers;
using CMS.DocumentEngine;
using CMS.SiteProvider;
using CMS.SocialMedia;

using org.pdfclown.objects;


public partial class CMSWebParts_SocialMedia_GooglePlus_GooglePlusButton : SocialMediaAbstractWebPart
{
    #region "Private fiels"

    private bool mHide;
    private string mLanguage;
    private string mSize;
    private string mAnnotation;
    private string mAlign;
    private string mUrl;
    private string mExpandTo;
    private int mWidth = 0;

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
    /// Target URL.
    /// </summary>
    public string Url
    {
        get
        {
            return mUrl ?? (mUrl = ValidationHelper.GetString(GetValue("Url"), string.Empty));
        }
        set
        {
            SetValue("Url", value);
            mUrl = value;
        }
    }


    /// <summary>
    /// Width of the web part in pixels.
    /// </summary>
    public int Width
    {
        get
        {
            return mWidth != 0 ? mWidth : (mWidth = ValidationHelper.GetInteger(GetValue("Width"), -1));
        }
        set
        {
            SetValue("Width", value);
            mWidth = value;
        }
    }


    /// <summary>
    /// Size.
    /// </summary>
    public string Size
    {
        get
        {
            return mSize ?? (mSize = ValidationHelper.GetString(GetValue("Size"), string.Empty));
        }
        set
        {
            SetValue("Size", value);
            mSize = value;
        }
    }


    /// <summary>
    /// Annotation.
    /// </summary>
    public string Annotation
    {
        get
        {
            return mAnnotation ?? (mAnnotation = ValidationHelper.GetString(GetValue("Annotation"), string.Empty));
        }
        set
        {
            SetValue("Annotation", value);
            mAnnotation = value;
        }
    }


    /// <summary>
    /// Align.
    /// </summary>
    public string Align
    {
        get
        {
            return mAlign ?? (mAlign = ValidationHelper.GetString(GetValue("Align"), string.Empty));
        }
        set
        {
            SetValue("Align", value);
            mAlign = value;
        }
    }


    /// <summary>
    /// Expand to.
    /// </summary>
    public string ExpandTo
    {
        get
        {
            return mExpandTo ?? (mExpandTo = ValidationHelper.GetString(GetValue("ExpandTo"), string.Empty).ToLowerCSafe());
        }
        set
        {
            SetValue("ExpandTo", value);
            mExpandTo = value;
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
        if (StopProcessing)
        {
            return;
        }
        StringBuilder output = new StringBuilder();

        string script = String.Format("<script src=\"https://apis.google.com/js/platform.js\" async defer>{{lang: '{0}'}}</script>",Language);

        string dataHref = String.IsNullOrWhiteSpace(Url) ? "" : String.Format("data-href=\"{0}\" ", HTMLHelper.EncodeForHtmlAttribute(Url));
        string dataSize = String.IsNullOrWhiteSpace(Size) ? "" : String.Format("data-size=\"{0}\" ", HTMLHelper.EncodeForHtmlAttribute(Size));
        string dataAnnotations = String.IsNullOrWhiteSpace(Annotation) ? "" : String.Format("data-annotation=\"{0}\" ", HTMLHelper.EncodeForHtmlAttribute(Annotation));
        string dataWidth = ((Annotation != "inline") || (Width >= 0)) ? "" : String.Format("data-width=\"{0}\" ", Width);
        string dataAlign = String.IsNullOrWhiteSpace(Align) ? "" : String.Format("data-align=\"{0}\" ", HTMLHelper.EncodeForHtmlAttribute(Align));
        string expandTo = String.IsNullOrWhiteSpace(ExpandTo) ? "" : String.Format("data-expandTo=\"{0}\" ", HTMLHelper.EncodeForHtmlAttribute(ExpandTo));

        output.Append(script);
        output.Append("<div class=\"g-plusone\" ");
        output.Append(dataHref);
        output.Append(dataSize);
        output.Append(dataAnnotations);
        output.Append(dataWidth);
        output.Append(dataAlign);
        output.Append(expandTo);
        output.Append("></div>");

        ltlPluginCode.Text = output.ToString();
    }

    #endregion
}



