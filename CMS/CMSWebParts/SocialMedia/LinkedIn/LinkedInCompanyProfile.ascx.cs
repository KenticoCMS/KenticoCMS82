using System;
using System.Linq;

using CMS.Helpers;
using CMS.Base;
using CMS.MembershipProvider;
using CMS.SiteProvider;
using CMS.SocialMedia;


public partial class CMSWebParts_SocialMedia_LinkedIn_LinkedInCompanyProfile : SocialMediaAbstractWebPart
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
    /// Display mode.
    /// </summary>
    public string DisplayMode
    {
        get
        {
            return ValidationHelper.GetString(GetValue("DisplayMode"), string.Empty);
        }
        set
        {
            SetValue("DisplayMode", value);
        }
    }


    /// <summary>
    /// Behavior.
    /// </summary>
    public string Behavior
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Behavior"), string.Empty);
        }
        set
        {
            SetValue("Behavior", value);
        }
    }


    /// <summary>
    /// Company name.
    /// </summary>
    public string CompanyName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("DataText"), string.Empty);
        }
        set
        {
            SetValue("DataText", value);
        }
    }


    /// <summary>
    /// Show connections.
    /// </summary>
    public bool ShowConnections
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowConnections"), false);
        }
        set
        {
            SetValue("ShowConnections", value);
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
            string dataFormat = null, dataText = null;
            string apiKey = LinkedInHelper.GetLinkedInApiKey(SiteContext.CurrentSiteName);
            
            // Check optional parameters
            if (DisplayMode.EqualsCSafe("inline"))
            {
                dataFormat = DisplayMode;
            }
            else
            {
                dataFormat = Behavior;
            }

            if (DisplayMode.EqualsCSafe("iconname"))
            {
                dataText = " data-text=\"" + CompanyName + "\"";
            }
            else
            {
                dataText = String.Empty;
            }

            // Build plugin code
            string output = "<div style=\"overflow: hidden;\"><script src=\"{1}\" type=\"text/javascript\">api_key: {6}</script><script type=\"IN/CompanyProfile\" data-id=\"{2}\" data-width=\"{0}\" data-format=\"{3}\" data-related=\"{4}\"{5}></script></div>";
            ltlPluginCode.Text = String.Format(output, Width.ToString(), src, CompanyID, dataFormat, ShowConnections.ToString().ToLowerCSafe(), dataText, apiKey);
        }
    }

    #endregion
}