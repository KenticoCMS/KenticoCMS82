using System;

using CMS.UIControls;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.FormControls;
using CMS.SiteProvider;

using CMS.ExtendedControls;

public partial class CMSModules_ContactManagement_FormControls_ContactStatusSelector : FormEngineUserControl
{
    #region "Variables"

    private int mSiteID = int.MinValue;

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
            uniselector.Enabled = value;
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
            if (uniselector.Value.ToString() == UniSelector.US_NONE_RECORD.ToString())
            {
                return null;
            }
            else
            {
                return uniselector.Value;
            }
        }
        set
        {
            EnsureChildControls();
            uniselector.Value = ValidationHelper.GetString(value, UniSelector.US_NONE_RECORD.ToString());
        }
    }


    /// <summary>
    /// Additional CSS class for drop down list control.
    /// </summary>
    public String AdditionalDropDownCSSClass
    {
        get
        {
            return uniselector.AdditionalDropDownCSSClass;
        }
        set
        {
            uniselector.AdditionalDropDownCSSClass = value;
        }
    }


    /// <summary>
    /// Gets or sets SiteID of contact statuses.
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
    /// Returns Uniselector.
    /// </summary>
    public UniSelector UniSelector
    {
        get
        {
            if (uniselector == null)
            {
                pnlUpdate.LoadContainer();
            }
            return uniselector;
        }
    }


    /// <summary>
    /// CMSDropDownList used in Uniselector.
    /// </summary>
    public CMSDropDownList DropDownList
    {
        get
        {
            if (uniselector == null)
            {
                pnlUpdate.LoadContainer();
            }
            return uniselector.DropDownSingleSelect;
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
            return ValidationHelper.GetBoolean(GetValue("AllowAllItem"), true);
        }
        set
        {
            SetValue("AllowAllItem", value);
        }
    }


    /// <summary>
    /// Gets or sets if all contact statuses regardless of site should be displayed.
    /// </summary>
    public bool DisplayAll
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets if only global and site statuses should be displayed.
    /// </summary>
    public bool DisplaySiteOrGlobal
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplaySiteOrGlobal"), false);
        }
        set
        {
            SetValue("DisplaySiteOrGlobal", value);
        }
    }


    /// <summary>
    /// Gets selected ContactStatusID.
    /// </summary>
    public int ContactStatusID
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


    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            uniselector.StopProcessing = true;
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

        var siteName = SiteID > 0 ? SiteInfoProvider.GetSiteName(SiteID) : SiteContext.CurrentSiteName;
        var allowGlobal = SettingsKeyInfoProvider.GetBoolValue(siteName + ".cmscmglobalconfiguration");

        uniselector.AllowAll = AllowAllItem;

        if (DisplayAll || DisplaySiteOrGlobal)
        {
            // Display all site and global statuses
            if (DisplayAll && allowGlobal)
            {
                // No WHERE condition required
            }
            // Display current site and global statuses
            else if (DisplaySiteOrGlobal && allowGlobal && (SiteID > 0))
            {
                where = SqlHelper.AddWhereCondition(where, "ContactStatusSiteID IS NULL OR ContactStatusSiteID = " + SiteID);
            }
            // Current site
            else if (SiteID > 0)
            {
                where = SqlHelper.AddWhereCondition(where, "ContactStatusSiteID = " + SiteID);
            }
            // Display global statuses
            else if (allowGlobal)
            {
                where = SqlHelper.AddWhereCondition(where, "ContactStatusSiteID IS NULL ");
            }
            // Don't display anything
            if (String.IsNullOrEmpty(where) && !DisplayAll)
            {
                where = "(1=0)";
            }
        }
        // Display either global or current site statuses
        else
        {
            // Current site
            if (SiteID > 0)
            {
                where = SqlHelper.AddWhereCondition(where, "ContactStatusSiteID = " + SiteID);
            }
            // Display global statuses
            else if (((SiteID == UniSelector.US_GLOBAL_RECORD) || (SiteID == UniSelector.US_NONE_RECORD)) && allowGlobal)
            {
                where = SqlHelper.AddWhereCondition(where, "ContactStatusSiteID IS NULL ");
            }
            // Don't display anything
            if (String.IsNullOrEmpty(where))
            {
                where = "(1=0)";
            }
        }

        // Do not add condition to empty condition which allows everything
        if (!String.IsNullOrEmpty(where))
        {
            string status = ValidationHelper.GetString(Value, "");
            if (!String.IsNullOrEmpty(status))
            {
                where = SqlHelper.AddWhereCondition(where, String.Format("{0} = {1}", SqlHelper.EscapeQuotes(uniselector.ReturnColumnName), SqlHelper.EscapeQuotes(status)), "OR");
            }
        }

        uniselector.WhereCondition = where;
        uniselector.Reload(true);
    }


    /// <summary>
    /// Gets where condition.
    /// </summary>
    public override string GetWhereCondition()
    {
        if (ValidationHelper.GetInteger(uniselector.Value, UniSelector.US_NONE_RECORD) == UniSelector.US_NONE_RECORD)
        {
            return FieldInfo.Name + " IS NULL";
        }
        else
        {
            return FieldInfo.Name + " = " + uniselector.Value;
        }
    }

    #endregion
}