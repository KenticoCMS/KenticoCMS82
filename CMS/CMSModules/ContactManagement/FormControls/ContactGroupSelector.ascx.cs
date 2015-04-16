using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CMS.DataEngine;
using CMS.FormControls;
using CMS.Helpers;
using CMS.Membership;
using CMS.OnlineMarketing;
using CMS.Base;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_ContactManagement_FormControls_ContactGroupSelector : FormEngineUserControl
{
    #region "Variables"

    private int mSiteId = -1;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Allows to display accounts only for specified site id. Default value is current site id.
    /// </summary>
    public int SiteID
    {
        get
        {
            if (mSiteId == -1)
            {
                mSiteId = SiteContext.CurrentSiteID;
            }

            return mSiteId;
        }
        set
        {
            mSiteId = value;
        }
    }


    /// <summary>
    /// Indicates what objects can be displayed. -5 stands for global and site objects. -4 stands for global objects. -1 stands for all objects.
    /// </summary>
    public int ObjectsRange
    {
        get;
        set;
    }


    /// <summary>
    /// Gets selected contact group ID.
    /// </summary>
    public int ContactGroupID
    {
        get
        {
            return ValidationHelper.GetInteger(Value, 0);
        }
    }


    /// <summary>
    /// Returns Uniselector.
    /// </summary>
    public UniSelector UniSelector
    {
        get
        {
            EnsureChildControls();
            return uniSelector;
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
    /// SQL WHERE condition of uniselector.
    /// </summary>
    public string WhereCondition
    {
        get;
        set;
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
    /// Reloads the data in the selector.
    /// </summary>
    public void ReloadData()
    {
        bool authorizedSite = ContactGroupHelper.AuthorizedReadContactGroup(SiteID, false);
        bool authorizedGlobal = ContactGroupHelper.AuthorizedReadContactGroup(UniSelector.US_GLOBAL_RECORD, false);

        // Site objects
        if (ObjectsRange == 0)
        {
            if (authorizedSite)
            {
                uniSelector.WhereCondition = SqlHelper.AddWhereCondition(WhereCondition, "(ContactGroupSiteID = " + SiteID + ")");
            }
            else
            {
                uniSelector.WhereCondition = "(1=0)";
            }
        }
        // Global objects
        else if (ObjectsRange == UniSelector.US_GLOBAL_RECORD)
        {
            if (authorizedGlobal)
            {
                uniSelector.WhereCondition = SqlHelper.AddWhereCondition(WhereCondition, "(ContactGroupSiteID IS NULL)");
            }
            else
            {
                uniSelector.WhereCondition = "(1=0)";
            }
        }
        // Global or site objects
        else if (ObjectsRange == UniSelector.US_GLOBAL_AND_SITE_RECORD)
        {
            if (authorizedSite && authorizedGlobal)
            {
                uniSelector.WhereCondition = "(ContactGroupSiteID IS NULL OR ContactGroupSiteID = " + SiteID + ")";
                uniSelector.AddGlobalObjectSuffix = true;
            }
            else if (authorizedGlobal)
            {
                uniSelector.WhereCondition = "ContactGroupSiteID IS NULL";
            }
            else if (authorizedSite)
            {
                uniSelector.WhereCondition = "ContactGroupSiteID = " + SiteID;
            }
            else
            {
                uniSelector.WhereCondition = "(1=0)";
            }
        }
        // Display all objects
        else if ((ObjectsRange == UniSelector.US_ALL_RECORDS) && MembershipContext.AuthenticatedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin))
        {
            uniSelector.WhereCondition = "(ContactGroupSiteID IS NULL OR ContactGroupSiteID > 0)";
            uniSelector.AddGlobalObjectSuffix = true;
        }
        // Not enough permissions
        else
        {
            uniSelector.WhereCondition = "(1=0)";
        }

        // Initialize selector
        uniSelector.IsLiveSite = false;
        uniSelector.Reload(true);
    }


    /// <summary>
    /// Overrides base GetValue method and enables to return UniSelector control with 'uniselector' property name.
    /// </summary>
    /// <param name="propertyName">Property name</param>
    public override object GetValue(string propertyName)
    {
        if (propertyName.EqualsCSafe("uniselector", true))
        {
            // Return uniselector control
            return UniSelector;
        }

        // Return other values
        return base.GetValue(propertyName);
    }

    #endregion
}