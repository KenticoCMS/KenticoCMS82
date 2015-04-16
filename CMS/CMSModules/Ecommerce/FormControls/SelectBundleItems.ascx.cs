using System;
using System.Data;

using CMS.Ecommerce;
using CMS.FormControls;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.DataEngine;

/// <summary>
/// Obsolete: Use Multiple object binding control instead
/// </summary>
public partial class CMSModules_Ecommerce_FormControls_SelectBundleItems : FormEngineUserControl
{
    #region "Backing fields"

    private int mSKUID = 0;
    private int mSiteID = -1;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the bundle ID.
    /// </summary>
    public int SKUID
    {
        get
        {
            if (mSKUID > 0)
            {
                return mSKUID;
            }

            // Try to get value from SKU form data
            if ((Form != null) && Form.Data.ContainsColumn("SKUID"))
            {
                return ValidationHelper.GetInteger(Form.Data.GetValue("SKUID"), 0);
            }

            return 0;
        }
        set
        {
            mSKUID = value;
        }
    }


    /// <summary>
    /// Gets or sets the site ID.
    /// </summary>
    public int SiteID
    {
        get
        {
            if (mSiteID >= 0)
            {
                return mSiteID;
            }

            // Try to get value from SKU form data
            if ((Form != null) && Form.Data.ContainsColumn("SKUSiteID"))
            {
                int siteId = ValidationHelper.GetInteger(Form.Data.GetValue("SKUSiteID"), 0);
                if (siteId >= 0)
                {
                    return siteId;
                }
            }

            return SiteContext.CurrentSiteID;
        }
        set
        {
            mSiteID = value;
        }
    }


    /// <summary>
    /// Gets bundle items count value.
    /// </summary>
    public override object Value
    {
        get
        {
            EnsureChildControls();
            int count = 0;
            if (!string.IsNullOrEmpty(Items))
            {
                count = Items.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Length;
            }
            return count;
        }
        set
        {
        }
    }


    /// <summary>
    /// Gets the selected bundle item IDs separated by semicolon.
    /// </summary>
    public string Items
    {
        get
        {
            return ValidationHelper.GetString(itemsUniSelector.Value, null);
        }
    }

    #endregion


    #region "Lifecycle"

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (!RequestHelper.IsPostBack() && (SKUID > 0))
        {
            itemsUniSelector.Value = GetSavedItemIds();
        }

        itemsUniSelector.StopProcessing = StopProcessing;
        itemsUniSelector.WhereCondition = GetItemsWhereCondition();

        if (Form != null)
        {
            Form.OnUploadFile += (sender, args) => SaveSelectionChanges();
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Ensure correct current pager page and correct pager data
        itemsUniSelector.Reload(true);
    }

    #endregion


    #region "Initialization"

    private string GetItemsWhereCondition()
    {
        // Exclude product options
        string where = "SKUOptionCategoryID IS NULL";

        // Exclude bundle products
        where = SqlHelper.AddWhereCondition(where, String.Format("SKUProductType <> '{0}'", SKUProductTypeEnum.Bundle.ToStringRepresentation()));

        // Exclude donation products
        where = SqlHelper.AddWhereCondition(where, String.Format("SKUProductType <> '{0}'", SKUProductTypeEnum.Donation.ToStringRepresentation()));

        // Exclude edited product itself
        where = SqlHelper.AddWhereCondition(where, "SKUID <> " + SKUID);

        // Exclude variant parents
        where = SqlHelper.AddWhereCondition(where, "SKUID NOT IN (SELECT SKUParentSKUID FROM COM_SKU WHERE SKUParentSKUID IS NOT NULL)");

        // If bundle is global
        if (SiteID == 0)
        {
            // Include global products
            where = SqlHelper.AddWhereCondition(where, "SKUSiteID IS NULL");
        }
        else
        {
            // If global products are allowed on this site
            if (ECommerceSettings.AllowGlobalProducts(SiteID))
            {
                // Include global and site products
                where = SqlHelper.AddWhereCondition(where, String.Format("(SKUSiteID IS NULL) OR (SKUSiteID = {0})", SiteID));
            }
            else
            {
                // Include site products
                where = SqlHelper.AddWhereCondition(where, "SKUSiteID = " + SiteID);
            }
        }

        // Include only enabled products
        where = SqlHelper.AddWhereCondition(where, "SKUEnabled = 1");

        // Include currently selected items
        if (!string.IsNullOrEmpty(Items))
        {
            // Perform validation of items IDs separated by ';'
            string[] idsAsString = Items.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            int[] idsAsInt = ValidationHelper.GetIntegers(idsAsString, 0);

            // Separates IDs with ',' for use in SQL query
            string idsQuery = TextHelper.Join(",", idsAsInt);

            where = SqlHelper.AddWhereCondition(where, String.Format("SKUID IN ({0})", idsQuery), "OR");
        }

        return where;
    }

    #endregion


    #region "Save"

    /// <summary>
    /// Saves the currently selected bundle items to the database.
    /// </summary>
    public void SaveSelectionChanges()
    {
        var savedItems = GetSavedItemIds();

        var addedItems = DiffItemIds(savedItems, Items);
        foreach (var id in addedItems)
        {
            BundleInfoProvider.AddSKUToBundle(SKUID, id);
        }

        var removedItems = DiffItemIds(Items, savedItems);
        foreach (var id in removedItems)
        {
            BundleInfoProvider.RemoveSKUFromBundle(SKUID, id);
        }
    }

    #endregion


    #region "Helper methods"

    private string GetSavedItemIds()
    {
        DataSet items = BundleInfoProvider.GetBundles().WhereEquals("BundleID", SKUID).OrderBy("SKUID");
        if (!DataHelper.DataSourceIsEmpty(items))
        {
            return TextHelper.Join(";", DataHelper.GetStringValues(items.Tables[0], "SKUID"));
        }
        return null;
    }


    private int[] DiffItemIds(string ids1, string ids2)
    {
        string difference = DataHelper.GetNewItemsInList(ids1, ids2);
        return GetItemIdsArray(difference);
    }


    private int[] GetItemIdsArray(string ids)
    {
        if (string.IsNullOrEmpty(ids))
        {
            return new int[0];
        }
        return ValidationHelper.GetIntegers(ids.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries), 0);
    }

    #endregion
}
