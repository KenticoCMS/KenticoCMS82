using System;

using CMS.Ecommerce;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_Ecommerce_Pages_Tools_Products_Product_Edit_Tax : CMSProductsPage
{
    #region "Constants"

    /// <summary>
    /// Short link to help page.
    /// </summary>
    private const string HELP_TOPIC_LINK = "tax_classes";

    #endregion


    #region "Variables"

    private SKUInfo sku;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets the value of the 'categoryId' URL parameter.
    /// </summary>
    public int OptionCategoryID
    {
        get
        {
            return QueryHelper.GetInteger("categoryId", 0);
        }
    }


    /// <summary>
    /// Gets parent product ID, if option is edited from product options page
    /// </summary>
    public int ParentProductID
    {
        get
        {
            return QueryHelper.GetInteger("parentProductId", 0);
        }
    }

    #endregion


    #region "Page events"

    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);
        // Check if product is product option
        if (QueryHelper.GetInteger("categoryid", 0) > 0)
        {
            IsProductOption = true;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Setup help
        object options = new
        {
            helpName = "lnkProductEditHelp",
            helpUrl = DocumentationHelper.GetDocumentationTopicUrl(HELP_TOPIC_LINK)
        };
        ScriptHelper.RegisterModule(this, "CMS/DialogContextHelpChange", options);

        // Check UI personalization for product / product option separately
        if (OptionCategoryID > 0)
        {
            // Check elements in product options categories subtree
            CheckUIElementAccessHierarchical("CMS.Ecommerce", "ProductOptions.Options.TaxClasses");
        }
        else
        {
            CheckUIElementAccessHierarchical("CMS.Ecommerce", "Products.TaxClasses");
        }

        if (ProductID > 0)
        {
            sku = SKUInfoProvider.GetSKUInfo(ProductID);
            EditedObject = sku;

            if (sku != null)
            {
                // Check products site id
                CheckEditedObjectSiteID(sku.SKUSiteID);

                taxForm.ProductID = ProductID;
                taxForm.UniSelector.OnSelectionChanged += UniSelector_OnSelectionChanged;

                if (sku.IsProductOption)
                {
                    var categoryInfo = OptionCategoryInfoProvider.GetOptionCategoryInfo(sku.SKUOptionCategoryID);
                    if (categoryInfo != null)
                    {
                        CreateBreadcrumbs(sku);

                        if (categoryInfo.CategoryType != OptionCategoryTypeEnum.Products)
                        {
                            ShowError(GetString("com.taxesNotSupportedForOptionType"));
                            taxForm.Visible = false;
                        }
                    }
                }
            }
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Creates breadcrumbs.
    /// </summary>
    /// <param name="skuObj">Product SKU</param>
    private void CreateBreadcrumbs(SKUInfo skuObj)
    {
        string productListText = GetString("Prodect_Edit_Header.ProductOptionsLink");
        string productListUrl = "~/CMSModules/Ecommerce/Pages/Tools/ProductOptions/OptionCategory_Edit_Options.aspx";
        productListUrl = URLHelper.AddParameterToUrl(productListUrl, "categoryId", OptionCategoryID.ToString());
        productListUrl = URLHelper.AddParameterToUrl(productListUrl, "siteId", SiteID.ToString());
        productListUrl = URLHelper.AddParameterToUrl(productListUrl, "productId", ParentProductID.ToString());
        productListUrl = URLHelper.AddParameterToUrl(productListUrl, "dialog", QueryHelper.GetString("dialog", "0"));

        // Set breadcrumb
        PageBreadcrumbs.AddBreadcrumb(new BreadcrumbItem
        {
            Text = productListText,
            Target = "_parent",
            RedirectUrl = ResolveUrl(productListUrl)
        });

        PageBreadcrumbs.AddBreadcrumb(new BreadcrumbItem
        {
            Text = FormatBreadcrumbObjectName(skuObj.SKUName, SiteID)
        });
    }


    protected void UniSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        if (CheckProductModifyAndRedirect(sku))
        {
            // Save changes
            taxForm.SaveItems();
        }
    }

    #endregion
}