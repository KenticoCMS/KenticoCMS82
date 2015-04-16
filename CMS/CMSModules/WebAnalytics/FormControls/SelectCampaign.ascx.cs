using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.FormControls;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;
using CMS.WebAnalytics;
using CMS.LicenseProvider;
using CMS.Base;
using CMS.DataEngine;

public partial class CMSModules_WebAnalytics_FormControls_SelectCampaign : FormEngineUserControl
{
    #region "Variables"

    private bool mPostbackOnChange = false;
    private int mSiteID = int.MinValue;
    private bool mCreateOnUnknownName = true;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the value of campaign.
    /// </summary>
    public override object Value
    {
        get
        {
            return usCampaign.Value;
        }
        set
        {
            usCampaign.Value = value;
        }
    }


    /// <summary>
    /// Return uniselector control.
    /// </summary>
    public UniSelector UniSelector
    {
        get
        {
            return usCampaign;
        }
    }


    /// <summary>
    /// Gets or sets site ID.
    /// </summary>
    public int SiteID
    {
        get
        {
            if (mSiteID == int.MinValue)
            {
                mSiteID = GetValue<int>("siteid", SiteContext.CurrentSiteID);
            }
            return mSiteID;
        }
        set
        {
            mSiteID = value;
        }
    }


    /// <summary>
    /// If true, full postback is raised when item changed.
    /// </summary>
    public bool PostbackOnChange
    {
        get
        {
            return mPostbackOnChange;
        }
        set
        {
            mPostbackOnChange = value;
        }
    }


    /// <summary>
    /// Selection mode of control (dropdown,multiselect...).
    /// </summary>
    public SelectionModeEnum SelectionMode
    {
        get
        {
            return usCampaign.SelectionMode;
        }
        set
        {
            usCampaign.SelectionMode = value;
        }
    }


    /// <summary>
    /// If true, allow all is enabled.
    /// </summary>
    public bool AllowAll
    {
        get
        {
            return usCampaign.AllowAll;
        }
        set
        {
            usCampaign.AllowAll = value;
        }
    }


    /// <summary>
    /// Gets or sets AllowEmpty value of uniselector.
    /// </summary>
    public bool AllowEmpty
    {
        get
        {
            return usCampaign.AllowEmpty;
        }
        set
        {
            usCampaign.AllowEmpty = value;
        }
    }


    /// <summary>
    /// Value for all record item.
    /// </summary>
    public string AllRecordValue
    {
        get
        {
            return usCampaign.AllRecordValue;
        }
    }


    /// <summary>
    /// Gets or sets value for (none) record.
    /// </summary>
    public string NoneRecordValue
    {
        set
        {
            usCampaign.NoneRecordValue = value;
        }
        get
        {
            return usCampaign.NoneRecordValue;
        }
    }


    /// <summary>
    /// Indicates whether selector should try to create a new row if unknown selected or not.
    /// </summary>
    public bool CreateOnUnknownName
    {
        get
        {
            return GetValue<bool>("createonunknownname", mCreateOnUnknownName);
        }
        set
        {
            mCreateOnUnknownName = value;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        usCampaign.IsLiveSite = IsLiveSite;
        usCampaign.AllowEditTextBox = true;
        usCampaign.TextBoxSelect.MaxLength = 100;

        if ((SelectionMode == SelectionModeEnum.SingleTextBox) && MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.WebAnalytics", "managecampaigns"))
        {
            usCampaign.EditDialogWindowWidth = 800;
            usCampaign.EditDialogWindowHeight = 660;

            string url = "~/CMSModules/WebAnalytics/Pages/Tools/Campaign/Tab_General.aspx?campaignName=##ITEMID##&modaldialog=true";
            usCampaign.EditItemPageUrl = url;

            url = "~/CMSModules/WebAnalytics/Pages/Tools/Campaign/Tab_General.aspx?modaldialog=true";
            usCampaign.NewItemPageUrl = url;
        }

        if (PostbackOnChange)
        {
            usCampaign.DropDownSingleSelect.AutoPostBack = true;
            ScriptManager scr = ScriptManager.GetCurrent(Page);
            scr.RegisterPostBackControl(usCampaign);
        }

        usCampaign.WhereCondition = "CampaignSiteID = " + SiteID;
        if (!RequestHelper.IsPostBack())
        {
            ReloadData();
        }
    }


    /// <summary>
    /// Reloads uniselector.
    /// </summary>
    public void ReloadData()
    {
        usCampaign.WhereCondition = "CampaignSiteID = " + SiteID;
        usCampaign.Reload(true);
    }


    /// <summary>
    /// Test if campaign is valid.
    /// </summary>    
    public override bool IsValid()
    {
        String value = ValidationHelper.GetString(usCampaign.Value, String.Empty).Trim();
        if (value != String.Empty)
        {
            String domain = RequestContext.CurrentDomain;
            if (DataHelper.GetNotEmpty(domain, "") != "")
            {
                string parsedDomain = LicenseKeyInfoProvider.ParseDomainName(domain);
                if (!LicenseKeyInfoProvider.IsFeatureAvailable(parsedDomain, FeatureEnum.CampaignAndConversions))
                {
                    ValidationError = GetString("campaignselector.nolicence");
                    return false;
                }
            }

            if (!ValidationHelper.IsCodeName(value))
            {
                ValidationError = GetString("campaign.validcodename");
                return false;
            }

            // If campaign object not found
            CampaignInfo ci = CampaignInfoProvider.GetCampaignInfo(value, SiteInfoProvider.GetSiteName(SiteID));

            // Handle info not found
            if (ci == null)
            {
                if (!CreateOnUnknownName)
                {
                    // Selector is not set to create new campaigns so error out
                    ValidationError = GetString("campaign.validcodename");
                    return false;
                }

                // Or check permissions ..
                if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.WebAnalytics", "ManageCampaigns"))
                {
                    ValidationError = GetString("campaign.notallowedcreate");
                    return false;
                }

                // .. and try to create a new one.
                ci = new CampaignInfo();
                ci.CampaignName = value;
                ci.CampaignDisplayName = value;
                ci.CampaignEnabled = true;
                ci.CampaignSiteID = SiteID;

                CampaignInfoProvider.SetCampaignInfo(ci);
            }
        }

        return true;
    }


    /// <summary>
    /// Overrides base SetValue method and sets specific form control parameter.
    /// </summary>
    /// <param name="propertyName">Property name</param>
    /// <param name="value">Value</param>
    public override bool SetValue(string propertyName, object value)
    {
        if (string.IsNullOrEmpty(propertyName))
        {
            return false;
        }

        switch (propertyName.ToLowerCSafe())
        {
            case "allowempty":
                AllowEmpty = ValidationHelper.GetBoolean(value, false);
                break;
            case "nonerecordvalue":
                NoneRecordValue = ValidationHelper.GetString(value, string.Empty);
                break;
        }

        return base.SetValue(propertyName, value);
    }
}