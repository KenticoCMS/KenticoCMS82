using System;
using System.Data;

using CMS.Ecommerce;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_Ecommerce_Controls_UI_ProductTaxes : CMSAdminControl
{
    #region "Variables"

    protected int mProductId = 0;
    protected SKUInfo mProduct = null;
    protected string currentValues = string.Empty;

    #endregion


    #region "Properties"

    /// <summary>
    /// Product ID.
    /// </summary>
    public int ProductID
    {
        get
        {
            return mProductId;
        }
        set
        {
            mProductId = value;
            mProduct = null;
        }
    }


    /// <summary>
    /// Product info object.
    /// </summary>
    public SKUInfo Product
    {
        get
        {
            return mProduct ?? (mProduct = SKUInfoProvider.GetSKUInfo(mProductId));
        }
        set
        {
            mProduct = value;

            mProductId = 0;
            if (value != null)
            {
                mProductId = value.SKUID;
            }
        }
    }


    /// <summary>
    /// Form enabled.
    /// </summary>
    public bool Enabled
    {
        get
        {
            return uniSelector.Enabled;
        }
        set
        {
            uniSelector.Enabled = value;
        }
    }


    /// <summary>
    /// Uniselector control used for taxes selection.
    /// </summary>
    public UniSelector UniSelector
    {
        get
        {
            return uniSelector;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        EditedObject = Product;

        if (ProductID > 0)
        {
            DataSet ds = TaxClassInfoProvider.GetSKUTaxClasses(ProductID);
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                currentValues = TextHelper.Join(";", DataHelper.GetStringValues(ds.Tables[0], "TaxClassID"));
            }

            uniSelector.WhereCondition = GetWhereCondition();

            if (!RequestHelper.IsPostBack())
            {
                uniSelector.Value = currentValues;
            }

            headTitle.Text = GetString("product_edit_tax.taxtitle");
        }

        // Get category of product
        OptionCategoryInfo category = OptionCategoryInfoProvider.GetOptionCategoryInfo(Product.SKUOptionCategoryID);

        // Ensure correct info label for Attribute and Text product option
        if (category != null)
        {
            if (category.CategoryType != OptionCategoryTypeEnum.Products)
            {
                headTitle.Text = GetString("product_edit_tax.taxtitleforoption");
            }
        }
    }


    /// <summary>
    /// Saves selected items. No permission checks are performed.
    /// </summary>
    public void SaveItems()
    {
        // Remove old items
        string newValues = ValidationHelper.GetString(uniSelector.Value, null);
        string items = DataHelper.GetNewItemsInList(newValues, currentValues);
        if (!String.IsNullOrEmpty(items))
        {
            string[] newItems = items.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            // Add all new items to user
            foreach (string item in newItems)
            {
                int taxClassId = ValidationHelper.GetInteger(item, 0);
                SKUTaxClassInfoProvider.RemoveTaxClassFromSKU(taxClassId, ProductID);
            }
        }

        // Add new items
        items = DataHelper.GetNewItemsInList(currentValues, newValues);
        if (!String.IsNullOrEmpty(items))
        {
            string[] newItems = items.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            // Add all new items to user
            foreach (string item in newItems)
            {
                int taxClassId = ValidationHelper.GetInteger(item, 0);
                SKUTaxClassInfoProvider.AddTaxClassToSKU(taxClassId, ProductID);
            }
        }

        // Clear content changed flag
        DocumentManager.ClearContentChanged();

        // Show message
        ShowChangesSaved();
    }


    /// <summary>
    /// Returns Where condition according to global object settings.
    /// </summary>
    protected string GetWhereCondition()
    {
        string where = "";

        if (Product != null)
        {
            // Offer global tax classes for global products or when using global tax classes
            if (Product.IsGlobal || ECommerceSettings.UseGlobalTaxClasses(SiteContext.CurrentSiteName))
            {
                where = "TaxClassSiteID IS NULL";
            }
            else
            {
                where = "TaxClassSiteID = " + Product.SKUSiteID;
            }
        }

        // Include selected values
        if (!string.IsNullOrEmpty(currentValues))
        {
            where += " OR TaxClassID IN (" + currentValues.Replace(';', ',') + ")";
        }

        return where;
    }
}