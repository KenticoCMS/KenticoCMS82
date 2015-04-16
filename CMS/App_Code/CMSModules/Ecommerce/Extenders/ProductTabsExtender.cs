using System;
using System.Linq;

using CMS;
using CMS.Base;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Ecommerce;
using CMS.Helpers;
using CMS.Membership;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;


[assembly: RegisterCustomClass("ProductTabsExtender", typeof(ProductTabsExtender))]

/// <summary>
/// Extender for SKU tab of product document detail
/// </summary>
public class ProductTabsExtender : UITabsExtender
{
    #region "Variables"

    private int optionCategoryId;
    private int siteId;
    private int mProductId;

    private SKUInfo sku;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets current document.
    /// </summary>
    public TreeNode Node
    {
        get;
        set;
    }


    /// <summary>
    /// Gets node ID of current document.
    /// </summary>
    public int NodeID
    {
        get
        {
            if (Node != null)
            {
                return Node.NodeID;
            }

            return 0;
        }
    }


    /// <summary>
    /// ID of the product taken from query parameter 'productId' or from node specified by query parameter 'nodeId'.
    /// </summary>
    public int ProductID
    {
        get
        {
            if (mProductId <= 0)
            {
                mProductId = QueryHelper.GetInteger("productId", 0);
            }

            if ((mProductId <= 0) && (Node != null))
            {
                mProductId = Node.NodeSKUID;
            }

            return mProductId;
        }
    }


    /// <summary>
    /// Indicates global objects are allowed on current site besides site-specific ones.
    /// </summary>
    public bool AllowGlobalObjects
    {
        get
        {
            return SettingsKeyInfoProvider.GetBoolValue(SiteContext.CurrentSiteName + "." + ECommerceSettings.ALLOW_GLOBAL_PRODUCTS);
        }
    }


    /// <summary>
    /// Indicates if products are displayed like documents, i.e. with tree.
    /// </summary>
    public bool ProductListInTree
    {
        get
        {
            return (ECommerceSettings.ProductsTree(SiteContext.CurrentSiteID) == ProductsTreeModeEnum.Sections);
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Initializes the extender
    /// </summary>
    public override void OnInit()
    {
        base.OnInit();
        var breadcrumbName = "";
        bool generateBreadcrumbs = true;


        // Ensure selection of general tab in case EditForm is requested
        var tabName = QueryHelper.GetString("tabName", "");
        if (tabName.EqualsCSafe("EditForm", true))
        {
            Control.SelectedTabName = "Products.General";
        }

        var page = (CMSPage)Control.Page;

        if (ProductID <= 0)
        {
            // Setup the document manager
            var manager = page.DocumentManager;
            manager.RedirectForNonExistingDocument = false;
            manager.Tree.CombineWithDefaultCulture = false;

            var node = manager.Node;
            if (node == null)
            {
                // Redirect to create new culture version
                URLHelper.Redirect(ProductUIHelper.GetNewCultureVersionPageUrl());
            }
            else
            {
                Node = node;
                breadcrumbName = Node.Site.Generalized.ObjectDisplayName;
            }
        }

        // Get product name and option category ID
        if (ProductID > 0)
        {
            sku = SKUInfoProvider.GetSKUInfo(ProductID);
            if (sku != null)
            {
                breadcrumbName = ResHelper.LocalizeString(sku.SKUName);
                optionCategoryId = sku.SKUOptionCategoryID;
                siteId = sku.SKUSiteID;

                // Check if edited object belongs to configured site
                if ((siteId != SiteContext.CurrentSiteID) && ((siteId != 0) || !AllowGlobalObjects))
                {
                    page.EditedObject = null;
                }

                var dialogMode = QueryHelper.GetBoolean("dialog", false);

                int hideBreadcrumbs = QueryHelper.GetInteger("hidebreadcrumbs", 0);

                // Show breadcrumbs if not dialog mode or dialog mode is in product option UI
                if ((hideBreadcrumbs != 0) || dialogMode)
                {
                    generateBreadcrumbs = false;
                }
            }
        }

        if (generateBreadcrumbs)
        {
            ProductUIHelper.EnsureProductBreadcrumbs(page.PageBreadcrumbs, breadcrumbName, (ProductID <= 0), ProductListInTree);
        }

        Control.Page.Load += Page_Load;

        Control.ElementName = (optionCategoryId > 0) ? "ProductOptions.Options" : "Products.Properties";
    }


    /// <summary>
    /// Initialization of tabs.
    /// </summary>
    public override void OnInitTabs()
    {
        Control.OnTabCreated += OnTabCreated;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Register wopener script
        ScriptHelper.RegisterWOpenerScript(Control.Page);
    }


    protected void OnTabCreated(object sender, TabCreatedEventArgs e)
    {
        if (e.Tab == null)
        {
            return;
        }

        var tab = e.Tab;
        var element = e.UIElement;

        bool splitViewSupported = false;

        string lowerElementName = element.ElementName.ToLowerCSafe();

        switch (lowerElementName)
        {
            case "products.attachments":
            case "products.metadata":
            case "products.categories":
            case "products.workflow":
            case "products.versions":
                splitViewSupported = true;
                break;

            case "products.general":
                splitViewSupported = true;
                break;
        }

        switch (lowerElementName)
        {
            case "products.attachments":
            case "products.metadata":
            case "products.categories":
            case "products.workflow":
            case "products.versions":
            case "products.relatedproducts":
                // Check if editing product with its document
                if (NodeID <= 0)
                {
                    e.Tab = null;
                    return;
                }
                break;

            case "products.documents":
                if ((NodeID <= 0) && (ECommerceSettings.ProductsTree(SiteContext.CurrentSiteName) == ProductsTreeModeEnum.Sections))
                {
                    if (!MembershipContext.AuthenticatedUser.IsGlobalAdministrator || (sku == null) || !sku.IsGlobal)
                    {
                        e.Tab = null;
                        return;
                    }
                }
                break;

            case "products.preview":
                {
                    // Check if editing product with its document
                    if (NodeID <= 0)
                    {
                        e.Tab = null;
                        return;
                    }

                    var settings = new UIPageURLSettings
                    {
                        Mode = "preview",
                        NodeID = Node.NodeID,
                        Culture = Node.DocumentCulture,
                        Node = Node,
                        AllowViewValidate = false
                    };

                    tab.RedirectUrl = ProductUIHelper.GetProductPageUrl(settings);

                    tab.RedirectUrl = URLHelper.AddParameterToUrl(tab.RedirectUrl, "nodeid", NodeID.ToString());
                    tab.RedirectUrl = URLHelper.AddParameterToUrl(tab.RedirectUrl, "showdevicesselection", "0");
                }
                break;

            case "products.advanced":
                {
                    tab.Expand = (NodeID <= 0);

                    // Append product/node params to url
                    var url = tab.RedirectUrl;
                    url = URLHelper.AddParameterToUrl(url, "productid", ProductID.ToString());
                    if (Node != null)
                    {
                        url = URLHelper.AddParameterToUrl(url, "nodeid", Node.NodeID.ToString());
                        url = URLHelper.AddParameterToUrl(url, "culture", Node.DocumentCulture);
                    }

                    tab.RedirectUrl = url;
                }
                break;

            case "products.options":
                tab.RedirectUrl = URLHelper.AddParameterToUrl(tab.RedirectUrl, "productId", ProductID.ToString());
                break;
        }

        // Add SiteId parameter to each tab
        if (!string.IsNullOrEmpty(tab.RedirectUrl))
        {
            tab.RedirectUrl = URLHelper.AddParameterToUrl(tab.RedirectUrl, "siteId", siteId.ToString());
        }

        // Ensure split view mode
        if ((NodeID > 0) && splitViewSupported && UIContext.DisplaySplitMode)
        {
            tab.RedirectUrl = DocumentUIHelper.GetSplitViewUrl(tab.RedirectUrl);
        }

        // Make URL absolute
        tab.RedirectUrl = URLHelper.GetAbsoluteUrl(tab.RedirectUrl);
    }

    #endregion
}