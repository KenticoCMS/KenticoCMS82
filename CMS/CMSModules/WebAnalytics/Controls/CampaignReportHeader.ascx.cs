using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.DataEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.Base;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.WebAnalytics;


public partial class CMSModules_WebAnalytics_Controls_CampaignReportHeader : CMSAdminControl
{
    private bool mShowVisitsInGoalSelector = true;

    #region "Properties"

    /// <summary>
    /// Selected campaign code name
    /// </summary>
    public object SelectedCampaign
    {
        get
        {
            return ucSelectCampaign.Value;
        }
        set
        {
            ucSelectCampaign.Value = value;
        }
    }


    /// <summary>
    /// Selected conversion code name
    /// </summary>
    public object SelectedConversion
    {
        get
        {
            return usSelectConversion.Value;
        }
        set
        {
            usSelectConversion.Value = value;
        }
    }


    /// <summary>
    /// If true, allow all is enabled
    /// </summary>
    public bool CampaignAllowAll
    {
        get
        {
            return ucSelectCampaign.AllowAll;
        }
        set
        {
            ucSelectCampaign.AllowAll = value;
        }
    }


    /// <summary>
    /// Indicates whether show conversion selector
    /// </summary>
    public bool ShowConversionSelector
    {
        get
        {
            return pnlConversion.Visible;
        }
        set
        {
            pnlConversion.Visible = value;
        }
    }


    /// <summary>
    /// Indicates whether show campaign selector
    /// </summary>
    public bool ShowCampaignSelector
    {
        get
        {
            return pnlCampaign.Visible;
        }
        set
        {
            pnlCampaign.Visible = value;
        }
    }


    /// <summary>
    /// Returns WA codename by given goal
    /// </summary>
    public string CodeNameByGoal
    {
        get
        {
            return drpGoals.SelectedValue == "view" ? "campaign" : "campconversion;%";
        }
    }


    /// <summary>
    /// Gets or sets goal
    /// </summary>
    public string SelectedGoal
    {
        get
        {
            return drpGoals.SelectedValue;
        }
        set
        {
            drpGoals.SelectedValue = value;
        }
    }


    /// <summary>
    /// Gets or sets SiteID
    /// </summary>
    public int SelectedSiteID
    {
        get
        {
            // All sites - use '0' for query
            if (usSite.SiteID == UniSelector.US_ALL_RECORDS)
            {
                return 0;
            }

            return usSite.SiteID;
        }
        set
        {
            usSite.SiteID = value;
        }
    }


    /// <summary>
    /// Indicates whether show goal selector
    /// </summary>
    public bool ShowGoalSelector
    {
        get
        {
            return pnlGoal.Visible;
        }
        set
        {
            pnlGoal.Visible = value;
        }
    }


    /// <summary>
    /// Indicates whether show site selector
    /// </summary>
    public bool ShowSiteSelector
    {
        get
        {
            return pnlSite.Visible;
        }
        set
        {
            pnlSite.Visible = value;
        }
    }


    /// <summary>
    /// Value for all record item
    /// </summary>
    public string AllRecordValue
    {
        get
        {
            return ucSelectCampaign.AllRecordValue;
        }
    }


    /// <summary>
    /// If true, goal selector contains visits
    /// </summary>
    public bool ShowVisitsInGoalSelector
    {
        get
        {
            return mShowVisitsInGoalSelector;
        }
        set
        {
            mShowVisitsInGoalSelector = value;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        usSelectConversion.SelectionMode = SelectionModeEnum.SingleDropDownList;
        usSite.UserId = MembershipContext.AuthenticatedUser.UserID;
        usSite.AllowAll = MembershipContext.AuthenticatedUser.IsGlobalAdministrator;

        string campaignName = ValidationHelper.GetString(ucSelectCampaign.Value, String.Empty);
        if (campaignName != ucSelectCampaign.AllRecordValue)
        {
            CampaignInfo ci = CampaignInfoProvider.GetCampaignInfo(campaignName, SiteContext.CurrentSiteName);
            if ((ci != null) && (!ci.CampaignUseAllConversions))
            {
                usSelectConversion.WhereCondition = "ConversionID  IN (SELECT ConversionID FROM Analytics_ConversionCampaign WHERE CampaignID =" + ci.CampaignID + ")";
            }
        }

        if (!URLHelper.IsPostback())
        {
            if (ShowVisitsInGoalSelector)
            {
                drpGoals.Items.Add(new ListItem(GetString("analytics.view"), "view"));
            }

            drpGoals.Items.Add(new ListItem(GetString("conversion.count"), "count"));
            drpGoals.Items.Add(new ListItem(GetString("om.trackconversionvalue"), "value"));
            usSite.SiteID = SiteContext.CurrentSiteID;
        }

        // Filter conversions for selected site  (current is site selector not visible)
        int siteID = SiteContext.CurrentSiteID;
        if (pnlSite.Visible)
        {
            siteID = usSite.SiteID;
        }

        // Filter conversions only if not all sites selected
        if (!pnlSite.Visible || (siteID.ToString() != usSite.AllRecordValue))
        {
            usSelectConversion.WhereCondition = SqlHelper.AddWhereCondition(usSelectConversion.WhereCondition, "ConversionSiteID =" + siteID);
        }

        usSelectConversion.ReloadData(true);
        drpGoals.AutoPostBack = true;
    }


    protected override void OnPreRender(EventArgs e)
    {
        if (ShowGoalSelector)
        {
            usSelectConversion.Enabled = drpGoals.SelectedValue != "view";
        }

        base.OnPreRender(e);
    }

    #endregion
}