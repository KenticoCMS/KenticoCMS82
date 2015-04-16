using System;
using System.Linq;

using CMS.Helpers;
using CMS.MembershipProvider;
using CMS.SiteProvider;
using CMS.SocialMedia;


public partial class CMSWebParts_SocialMedia_LinkedIn_LinkedInShareButton : SocialMediaAbstractWebPart
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
            ltlButtonCode.Visible = !value;
        }
    }



    /// <summary>
    /// Url to share.
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
    /// Count box position.
    /// </summary>
    public string CountBox
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CountBox"), string.Empty);
        }
        set
        {
            SetValue("CountBox", value);
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
            // Build plugin code
            string src = "http://platform.linkedin.com/in.js";
            string apiKey = LinkedInHelper.GetLinkedInApiKey(SiteContext.CurrentSiteName);

            if (string.IsNullOrEmpty(UrlToShare))
            {
                UrlToShare = RequestContext.CurrentURL;
            }
            UrlToShare = URLHelper.GetAbsoluteUrl(ResolveUrl(HTMLHelper.HTMLEncode(UrlToShare)));

            string output = "<div style=\"overflow: hidden;\"><script src=\"{0}\" type=\"text/javascript\">api_key: {3}</script><script type=\"IN/Share\" data-url=\"{1}\" data-counter=\"{2}\"></script></div>";
            ltlButtonCode.Text = String.Format(output, src, UrlToShare, CountBox, apiKey);
        }
    }

    #endregion
}