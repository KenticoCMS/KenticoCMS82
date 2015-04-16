using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.DataEngine;
using CMS.FormControls;
using CMS.Helpers;
using CMS.OnlineMarketing;
using CMS.Base;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.ExtendedControls;
using CMS.Membership;

public partial class CMSModules_ContactManagement_FormControls_AccountSelector : FormEngineUserControl
{
    #region "Variables"

    private int mSiteId = -1;
    private CurrentUserInfo currentUser;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Allows to display accounts only for specified site id. Use 0 for global objects. Default value is current site id.
    /// </summary>
    public int SiteID
    {
        get
        {
            return mSiteId;
        }
        set
        {
            mSiteId = value;
        }
    }


    /// <summary>
    /// Gets selected Account ID.
    /// </summary>
    public int AccountID
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


    /// <summary>
    /// Gets or sets if control is placed in sitemanager.
    /// </summary>
    public bool IsSiteManager
    {
        get
        {
            return uniSelector.IsSiteManager;
        }
        set
        {
            uniSelector.IsSiteManager = value;
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
    /// Reloads the data in the selector.
    /// </summary>
    public void ReloadData()
    {
        string where = null;
        bool authorizedSiteAccounts = false;
        bool authorizedGlobalAccounts = AccountHelper.AuthorizedReadAccount(UniSelector.US_GLOBAL_RECORD, false);
        currentUser = MembershipContext.AuthenticatedUser;

        if (SiteID > 0)
        {
            authorizedSiteAccounts = AccountHelper.AuthorizedReadAccount(SiteID, false);
        }
        else
        {
            authorizedSiteAccounts = AccountHelper.AuthorizedReadAccount(SiteContext.CurrentSiteID, false);
        }

        // Filter site objects
        if (SiteID > 0)
        {
            if (authorizedSiteAccounts)
            {
                where = "(AccountSiteID = " + SiteID + " AND AccountMergedWithAccountID IS NULL)";
            }
            else
            {
                where = "(1=0)";
            }
        }
        // Filter only global objects
        else if ((SiteID == UniSelector.US_GLOBAL_RECORD) || (SiteID == 0))
        {
            if (authorizedGlobalAccounts)
            {
                where = "(AccountSiteID IS NULL AND AccountGlobalAccountID IS NULL)";
            }
            else
            {
                where = "(1=0)";
            }
        }
        // Display current site and global contacts
        else if (SiteID == UniSelector.US_GLOBAL_AND_SITE_RECORD)
        {
            if (authorizedSiteAccounts && authorizedGlobalAccounts)
            {
                where = "(AccountSiteID IS NULL AND AccountGlobalAccountID IS NULL) OR (AccountSiteID = " + SiteContext.CurrentSiteID + " AND AccountMergedWithAccountID IS NULL)";
                uniSelector.AddGlobalObjectSuffix = true;
            }
            else if (authorizedGlobalAccounts)
            {
                where = "(AccountSiteID IS NULL AND AccountMergedWithAccountID IS NULL)";
            }
            else if (authorizedSiteAccounts)
            {
                where = "(AccountSiteID = " + SiteContext.CurrentSiteID + " AND AccountMergedWithAccountID IS NULL)";
            }
            else
            {
                where = "(1=0)";
            }
        }
        // Display all objects
        else if ((SiteID == UniSelector.US_ALL_RECORDS) && currentUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin))
        {
            where = "((AccountSiteID IS NULL AND AccountGlobalAccountID IS NULL) OR (AccountSiteID > 0 AND AccountMergedWithAccountID IS NULL))";
            uniSelector.AddGlobalObjectSuffix = true;
        }
        // Not enough permissions
        else
        {
            where = "(1=0)";
        }

        uniSelector.WhereCondition = SqlHelper.AddWhereCondition(WhereCondition, where);
        uniSelector.Reload(true);
    }

    #endregion
}