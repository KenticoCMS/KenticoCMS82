using System;
using System.Linq;

using CMS.Helpers;
using CMS.MembershipProvider;
using CMS.SiteProvider;
using CMS.SocialMedia;


public partial class CMSWebParts_SocialMedia_LinkedIn_LinkedInCompanyInsider : SocialMediaAbstractWebPart
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
    /// Company ID.
    /// </summary>
    public string CompanyID
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CompanyID"), string.Empty);
        }
        set
        {
            SetValue("CompanyID", value);
        }
    }


    /// <summary>
    /// Defines whether to show user's network.
    /// </summary>
    public bool ShowNetwork
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowNetwork"), false);
        }
        set
        {
            SetValue("ShowNetwork", value);
        }
    }


    /// <summary>
    /// Defines whether to show new hires.
    /// </summary>
    public bool ShowNewHires
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowNewHires"), false);
        }
        set
        {
            SetValue("ShowNewHires", value);
        }
    }


    /// <summary>
    /// Defines whether to show promotions and changes.
    /// </summary>
    public bool ShowPromotions
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowPromotions"), false);
        }
        set
        {
            SetValue("ShowPromotions", value);
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
            // Initialize variables
            string src = "http://platform.linkedin.com/in.js";
            string dataModules = String.Empty;
            string apiKey = LinkedInHelper.GetLinkedInApiKey(SiteContext.CurrentSiteName);
            
            //Check optional parameters
            if (ShowNetwork)
            {
                dataModules += ",innetwork";
            }

            if (ShowNewHires)
            {
                dataModules += ",newhires";
            }

            if (ShowPromotions)
            {
                dataModules += ",jobchanges";
            }

            // Build plugin code
            string output = "<div style=\"overflow: hidden;\"><script src=\"{0}\" type=\"text/javascript\">api_key: {3}</script><script type=\"IN/CompanyInsider\" data-id=\"{1}\" data-modules=\"{2}\"></script></div>";
            ltlPluginCode.Text = String.Format(output, src, CompanyID, dataModules.TrimStart(','), apiKey);
        }
    }

    #endregion
}