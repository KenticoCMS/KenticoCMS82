using System;
using System.Data;
using System.Text;

using CMS.DataEngine;
using CMS.Ecommerce;
using CMS.Helpers;
using CMS.Base;
using CMS.SiteProvider;
using CMS.Membership;


public partial class CMSModules_Ecommerce_Pages_Tools_ProductOptions_OptionCategory_Edit_Products : CMSProductOptionCategoriesPage
{
    #region "Variables"

    protected int categoryId = 0;
    protected OptionCategoryInfo categoryObj = null;
    protected int editedSiteId = 0;
    protected bool offerGlobalProducts = false;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets uniSelector data.
    /// </summary>
    private string UniSelectorData
    {
        get
        {
            return ValidationHelper.GetString(ViewState["UniSelectorData"], null);
        }
        set
        {
            ViewState["UniSelectorData"] = value;
        }
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        uniSelector.OnSelectionChanged += uniSelector_OnSelectionChanged;
        uniSelector.OnAdditionalDataBound += uniSelector_OnAdditionalDataBound;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Get parent product from url
        int parentProductId = QueryHelper.GetInteger("productId", 0);

        // Check UI permissions
        if (parentProductId <= 0)
        {
            // UIElement from option category list
            CheckUIElementAccessHierarchical("CMS.Ecommerce", "ProductOptions.Products");
        }
        else
        {
            // UIElement from product edit
            CheckUIElementAccessHierarchical("CMS.Ecommerce", "Products.ProductOptions.Products");
        }

        categoryId = QueryHelper.GetInteger("categoryId", 0);
        categoryObj = OptionCategoryInfoProvider.GetOptionCategoryInfo(categoryId);

        EditedObject = categoryObj;
        if (categoryObj != null)
        {
            editedSiteId = categoryObj.CategorySiteID;

            // Check edited objects site id
            CheckEditedObjectSiteID(editedSiteId);

            // Offer global products when allowed
            if (editedSiteId != 0)
            {
                offerGlobalProducts = ECommerceSettings.AllowGlobalProducts(editedSiteId);
            }
            // Configuring global products
            else
            {
                offerGlobalProducts = ECommerceSettings.AllowGlobalProducts(CurrentSiteName);
            }

            PreloadUniSelector(false);
            uniSelector.WhereCondition = GetWhereCondition();

            // If option category is disabled, hide ADD button
            if (categoryObj.CategoryEnabled == false)
            {
                uniSelector.ButtonAddItems.Visible = false;
            }
        }
    }

    #endregion


    #region "Event handlers"

    protected object uniSelector_OnAdditionalDataBound(object sender, string sourceName, object parameter, object value)
    {
        DataRowView row = parameter as DataRowView;

        switch (sourceName.ToLowerCSafe())
        {
            case "skuprice":
                double skuValue = ValidationHelper.GetDouble(row["SKUPrice"], 0);
                int siteId = ValidationHelper.GetInteger(row["SKUSiteID"], 0);

                // Format price
                return CurrencyInfoProvider.GetFormattedPrice(skuValue, siteId);
        }

        return value;
    }


    protected void uniSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        var cu = MembershipContext.AuthenticatedUser;

        // Check permissions
        if ((cu == null) || (!cu.IsAuthorizedPerResource("CMS.Ecommerce", "EcommerceModify") && !cu.IsAuthorizedPerResource("CMS.Ecommerce", "ModifyProducts")))
        {
            RedirectToAccessDenied("CMS.Ecommerce", "EcommerceModify OR ModifyProducts");
        }
        else
        {
            string newValues = ValidationHelper.GetString(uniSelector.Value, null);
            string addItems = DataHelper.GetNewItemsInList(UniSelectorData, newValues);
            string removeItems = DataHelper.GetNewItemsInList(newValues, UniSelectorData);

            // Add SKU to Option Category
            if (!String.IsNullOrEmpty(addItems))
            {
                string[] newItems = addItems.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string item in newItems)
                {
                    SKUOptionCategoryInfoProvider.AddOptionCategoryToSKU(categoryId, ValidationHelper.GetInteger(item, 0));
                }

                // Show message
                ShowChangesSaved();
            }

            // Remove SKU from Option Category
            if (!String.IsNullOrEmpty(removeItems))
            {
                string[] productIds = removeItems.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                bool displayWarning = false;
                StringBuilder ids = new StringBuilder();

                // Check if category is not used in variants of product
                foreach (string skuID in productIds)
                {
                    int productId = ValidationHelper.GetInteger(skuID, 0);

                    // Inform user that removing of product is not possible
                    if (VariantHelper.AreCategoriesUsedInVariants(productId, new int[] { categoryId }))
                    {
                        displayWarning = true;
                        ids.Append(";" + skuID);
                    }
                    else
                    {
                        ProductHelper.RemoveOptionCategory(productId, categoryId);
                    }
                }

                if (displayWarning)
                {
                    // Display items which cannot be removed in the selector
                    uniSelector.Value += ids.ToString();
                    ShowWarning(GetString("com.optioncategory.removeproduct"));
                }
                else
                {
                    ShowChangesSaved();
                }
            }
        }

        PreloadUniSelector(true);
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Gets where condition for uniSelector.
    /// </summary>
    protected string GetWhereCondition()
    {
        return SqlHelper.AddWhereCondition("SKUEnabled = 1", GetPartialWhereCondition());
    }


    /// <summary>
    /// Preloads uniSelector and selects actual data.
    /// </summary>
    /// <param name="forceReload">Force reload</param>
    protected void PreloadUniSelector(bool forceReload)
    {
        if (!RequestHelper.IsPostBack())
        {
            // Get assigned products
            string where = "SKUID IN (SELECT SKUID FROM COM_SKUOptionCategory WHERE CategoryID = " + categoryId + ")";

            where = SqlHelper.AddWhereCondition(where, GetPartialWhereCondition());

            // Preload data to uniSelector and save them  
            DataSet ds = SKUInfoProvider.GetSKUs().Column("SKUID").Where(where);
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                uniSelector.Value = UniSelectorData = TextHelper.Join(";", DataHelper.GetStringValues((ds).Tables[0], "SKUID"));
            }
        }

        if (forceReload)
        {
            UniSelectorData = ValidationHelper.GetString(uniSelector.Value, string.Empty);
            uniSelector.Reload(true);
        }
    }


    /// <summary>
    /// Return partial where condition selecting only products, not product options,
    /// reflecting only product from departments that user can access,
    /// site/global settings and user permissions.
    /// </summary>
    private string GetPartialWhereCondition()
    {
        // Select only products, not product options
        string where = "SKUOptionCategoryID IS NULL";

        // Ordinary user can see only product from departments he can access
        var user = MembershipContext.AuthenticatedUser;
        where = SqlHelper.AddWhereCondition(where, GetDepartmentWhereCondition(user));

        // Add products based on settings and user permissions
        return SqlHelper.AddWhereCondition(where, GetSiteWhereCondition(user));
    }


    /// <summary>
    /// Return partial where condition reflecting only product from departments that user can access.
    /// </summary>
    /// <param name="currentUser">Current user.</param>
    private string GetDepartmentWhereCondition(CurrentUserInfo currentUser)
    {
        if (!currentUser.IsAuthorizedPerResource("CMS.Ecommerce", "AccessAllDepartments"))
        {
            return "(SKUDepartmentID IS NULL) OR SKUDepartmentID IN (SELECT DepartmentID FROM COM_UserDepartment WHERE UserID = " + currentUser.UserID + ")";
        }

        return string.Empty;
    }


    /// <summary>
    /// Returns partial where condition based site/global settings and user permissions.
    /// </summary>
    /// <param name="currentUser">Current user.</param>
    private string GetSiteWhereCondition(CurrentUserInfo currentUser)
    {
        string where = string.Empty;

        // Add site specific product records if site doesn't offer global products or user doesn't have Modify global permission
        if (!offerGlobalProducts || categoryObj.CategorySiteID > 0 || (!currentUser.IsGlobalAdministrator && !currentUser.IsAuthorizedPerResource("CMS.Ecommerce", "EcommerceGlobalModify")))
        {
            where = SqlHelper.AddWhereCondition(where, "SKUSiteID = " + SiteContext.CurrentSiteID);
        }
        // Add site specific and global product records
        else
        {
            where = SqlHelper.AddWhereCondition(where, "SKUSiteID = " + SiteContext.CurrentSiteID + " OR SKUSiteID IS NULL");
        }

        return where;
    }

    #endregion
}