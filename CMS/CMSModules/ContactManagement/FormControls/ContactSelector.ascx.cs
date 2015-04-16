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
using CMS.Membership;

public partial class CMSModules_ContactManagement_FormControls_ContactSelector : FormEngineUserControl
{
    #region "Variables"

    private int mSiteId = -1;
    private CurrentUserInfo currentUser;

    #endregion


    #region "Events"

    /// <summary>
    /// OnSelectionChanged event.
    /// </summary>
    public event EventHandler OnSelectionChanged
    {
        add
        {
            uniSelector.OnSelectionChanged += value;
        }
        remove
        {
            uniSelector.OnSelectionChanged -= value;
        }
    }

    #endregion


    #region "Public properties"

    /// <summary>
    /// Allows to display contacts only for specified site id. Use 0 for global objects. Default value is current site id.
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
    /// Gets or sets a value indicating whether a postback to the server automatically
    /// occurs when the user changes the list selection.
    /// </summary>
    public bool AutoPostBack
    {
        get
        {
            return uniSelector.DropDownSingleSelect.AutoPostBack;
        }
        set
        {
            uniSelector.DropDownSingleSelect.AutoPostBack = value;
        }
    }


    /// <summary>
    /// Gets selected ContactID.
    /// </summary>
    public int ContactID
    {
        get
        {
            EnsureChildControls();
            return ValidationHelper.GetInteger(Value, 0);
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


    /// <summary>
    /// Gets or sets if uniselector is enabled.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return uniSelector.Enabled;
        }
        set
        {
            base.Enabled = value;
            uniSelector.Enabled = value;
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
        currentUser = MembershipContext.AuthenticatedUser;

        if (string.IsNullOrEmpty(uniSelector.AdditionalSearchColumns))
        {
            uniSelector.FilterControl = "~/CMSModules/ContactManagement/FormControls/SearchContactFullName.ascx";
            uniSelector.UseDefaultNameFilter = false;
        }

        bool authorizedSiteContacts = false;
        bool authorizedGlobalContacts = ContactHelper.AuthorizedReadContact(UniSelector.US_GLOBAL_RECORD, false);

        if (SiteID > 0)
        {
            authorizedSiteContacts = ContactHelper.AuthorizedReadContact(SiteID, false);
        }
        else
        {
            authorizedSiteContacts = ContactHelper.AuthorizedReadContact(SiteContext.CurrentSiteID, false);
        }

        // Filter site objects
        if (SiteID > 0)
        {
            if (authorizedSiteContacts)
            {
                where = "(ContactSiteID = " + SiteID + " AND ContactMergedWithContactID IS NULL)";
            }
            else
            {
                where = "(1=0)";
            }
        }
        // Filter only global objects
        else if ((SiteID == UniSelector.US_GLOBAL_RECORD) || (SiteID == 0))
        {
            if (authorizedGlobalContacts)
            {
                where = "(ContactSiteID IS NULL AND ContactGlobalContactID IS NULL)";
            }
            else
            {
                where = "(1=0)";
            }
        }
        // Display current site and global contacts
        else if (SiteID == UniSelector.US_GLOBAL_AND_SITE_RECORD)
        {
            if (authorizedSiteContacts && authorizedGlobalContacts)
            {
                where = "(ContactSiteID IS NULL AND ContactGlobalContactID IS NULL) OR (ContactSiteID = " + SiteContext.CurrentSiteID + " AND ContactMergedWithContactID IS NULL)";
                uniSelector.AddGlobalObjectSuffix = true;
            }
            else if (authorizedGlobalContacts)
            {
                where = "(ContactSiteID IS NULL AND ContactMergedWithContactID IS NULL)";
            }
            else if (authorizedSiteContacts)
            {
                where = "(ContactSiteID = " + SiteContext.CurrentSiteID + " AND ContactMergedWithContactID IS NULL)";
            }
            else
            {
                where = "(1=0)";
            }
        }
        // Display all objects
        else if ((SiteID == UniSelector.US_ALL_RECORDS) && currentUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin))
        {
            where = "((ContactSiteID IS NULL AND ContactGlobalContactID IS NULL) OR (ContactSiteID > 0 AND ContactMergedWithContactID IS NULL))";
            uniSelector.AddGlobalObjectSuffix = true;
        }
        // Not enough permissions
        else
        {
            where = "(1=0)";
        }

        where = SqlHelper.AddWhereCondition(where, WhereCondition);

        uniSelector.WhereCondition = SqlHelper.AddWhereCondition(uniSelector.WhereCondition, where);
        uniSelector.Reload(true);
    }


    /// <summary>
    /// Gets where condition.
    /// </summary>
    public override string GetWhereCondition()
    {
        if (ValidationHelper.GetInteger(uniSelector.Value, UniSelector.US_NONE_RECORD) == UniSelector.US_NONE_RECORD)
        {
            return FieldInfo.Name + " IS NULL";
        }
        else
        {
            return FieldInfo.Name + " = " + uniSelector.Value;
        }
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


    /// <summary>
    /// Overrides base SetValue method. Enables to set WhereCondition and SiteID properties.
    /// </summary>
    /// <param name="propertyName">Property name</param>
    /// <param name="value">Value</param>
    public override bool SetValue(string propertyName, object value)
    {
        string property = propertyName.ToLowerCSafe();

        switch (property)
        {
            case "wherecondition":
                // Set where condition
                WhereCondition = ValidationHelper.GetString(value, string.Empty);
                break;
            case "siteid":
                // Set site ID
                SiteID = ValidationHelper.GetInteger(value, -1);
                break;

            default:
                base.SetValue(propertyName, value);
                break;
        }

        return true;
    }

    #endregion
}