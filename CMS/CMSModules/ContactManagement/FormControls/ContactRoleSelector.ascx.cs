using System;

using CMS.ExtendedControls;
using CMS.OnlineMarketing;
using CMS.SiteProvider;
using CMS.FormControls;
using CMS.Membership;
using CMS.UIControls;
using CMS.DataEngine;
using CMS.Helpers;

public partial class CMSModules_ContactManagement_FormControls_ContactRoleSelector : FormEngineUserControl
{
    #region "Variables"

    private int? mSiteID = null;
    private bool? mIsSiteManager;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return base.Enabled;
        }
        set
        {
            EnsureChildControls();
            base.Enabled = value;
            uniSelector.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            EnsureChildControls();
            return uniSelector.Value;
        }
        set
        {
            EnsureChildControls();
            uniSelector.Value = ValidationHelper.GetString(value, "");
        }
    }


    /// <summary>
    /// Gets or sets SiteID of account statuses.
    /// </summary>
    public int SiteID
    {
        get
        {
            if (mSiteID != null)
            {
                return (int)mSiteID;
            }
            else
            {
                return ValidationHelper.GetInteger(GetValue("SiteID"), 0);
            }
        }
        set
        {
            mSiteID = value;
        }
    }


    /// <summary>
    /// Returns Uniselector.
    /// </summary>
    public UniSelector UniSelector
    {
        get
        {
            if (uniSelector == null)
            {
                pnlUpdate.LoadContainer();
            }
            return uniSelector;
        }
    }


    /// <summary>
    /// CMSDropDownList used in Uniselector.
    /// </summary>
    public CMSDropDownList DropDownList
    {
        get
        {
            if (uniSelector == null)
            {
                pnlUpdate.LoadContainer();
            }
            return uniSelector.DropDownSingleSelect;
        }
    }


    /// <summary>
    /// Specifies, whether the selector allows selection of all items. If the dialog allows selection of all items, 
    /// it displays the (all) field in the DDL variant and All button in the Textbox variant (default false). 
    /// When property is selected then Uniselector doesn't load any data from DB, it just returns -1 value 
    /// and external code must handle data loading.
    /// </summary>
    public bool AllowAllItem
    {
        get
        {
            return uniSelector.AllowAll;
        }
        set
        {
            uniSelector.AllowAll = value;
        }
    }


    /// <summary>
    /// Gets selected ContactRoleID.
    /// </summary>
    public int ContactRoleID
    {
        get
        {
            return ValidationHelper.GetInteger(Value, 0);
        }
    }


    /// <summary>
    /// SQL WHERE condition of uniselector.
    /// </summary>
    public string WhereCondition
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets if current context is in sitemanager.
    /// </summary>
    public bool IsSiteManager
    {
        get
        {
            return mIsSiteManager ?? QueryHelper.GetBoolean("issitemanager", false);
        }
        set
        {
            mIsSiteManager = value;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            uniSelector.StopProcessing = true;
        }
        else
        {
            ReloadData();
        }
    }


    /// <summary>
    /// Reloads control.
    /// </summary>
    public void ReloadData()
    {
        string where = WhereCondition;
        var user = MembershipContext.AuthenticatedUser;

        bool allowSite = false;

        // Show all global configuration to authorized users ..
        bool allowGlobal = ConfigurationHelper.AuthorizedReadConfiguration(UniSelector.US_GLOBAL_RECORD, false);

        // .. but just in SiteManager - fake it in CMSDesk so that even Global Admin sees user configuration
        // as Site Admins (depending on settings).
        allowGlobal &= (IsSiteManager || SettingsKeyInfoProvider.GetBoolValue(SiteInfoProvider.GetSiteName(SiteID) + ".cmscmglobalconfiguration"));

        if (SiteID > 0)
        {
            allowSite = ConfigurationHelper.AuthorizedReadConfiguration(SiteID, false);
        }
        else
        {
            allowSite = ConfigurationHelper.AuthorizedReadConfiguration(SiteContext.CurrentSiteID, false);
        }

        // Current site roles and lgobal roles
        if ((SiteID > 0) && allowSite && allowGlobal)
        {
            where = SqlHelper.AddWhereCondition(where, "ContactRoleSiteID IS NULL OR ContactRoleSiteID = " + (SiteID == 0 ? SiteContext.CurrentSiteID : SiteID));
        }
        // Current site roles only
        else if (allowSite && (SiteID > 0))
        {
            where = SqlHelper.AddWhereCondition(where, "ContactRoleSiteID = " + SiteID);
        }
        // Global roles only
        else if (allowGlobal && (SiteID == 0))
        {
            where = SqlHelper.AddWhereCondition(where, "ContactRoleSiteID IS NULL ");
        }
        // Don't display anything
        if (String.IsNullOrEmpty(where))
        {
            where = "(1=0)";
        }
        uniSelector.WhereCondition = where;
        uniSelector.Reload(true);
    }

    #endregion
}