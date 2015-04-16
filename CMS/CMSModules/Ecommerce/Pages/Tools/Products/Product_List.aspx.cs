using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Core;
using CMS.DocumentEngine;
using CMS.Ecommerce;
using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.Membership;
using CMS.Base;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.PortalEngine;
using CMS.DataEngine;

using TreeNode = CMS.DocumentEngine.TreeNode;

[UIElement(ModuleName.ECOMMERCE, "Products")]
public partial class CMSModules_Ecommerce_Pages_Tools_Products_Product_List : CMSProductsPage
{
    #region "Variables and constants"

    private bool? mShowSections;
    private bool showProductsInTree;
    private bool forceReloadData;

    private const string NO_DATA_CELL_VALUE = "<div style=\"text-align:center;\">-</div>";

    #endregion


    #region "Properties"

    /// <summary>
    /// Indicates whether product sections are to be included in listing.
    /// </summary>
    private bool ShowSections
    {
        get
        {
            return mShowSections ?? (mShowSections = QueryHelper.GetBoolean("showSections", false)).Value;
        }
    }


    /// <summary>
    /// Decides if document listing is used on page.
    /// </summary>
    private bool DocumentListingDisplayed
    {
        get
        {
            return (NodeID > 0);
        }
    }

    #endregion


    #region "Page events"

    protected override void OnPreInit(EventArgs e)
    {
        // Do not redirect when document does not exist
        DocumentManager.RedirectForNonExistingDocument = false;

        base.OnPreInit(e);
    }


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        showProductsInTree = ECommerceSettings.DisplayProductsInSectionsTree(CurrentSiteName);

        if (DocumentListingDisplayed)
        {
            // Init document list            
            docList.NodeID = NodeID;
            docList.Grid.GridName = "~/CMSModules/Ecommerce/Pages/Tools/Products/Product_List_Documents.xml";
            docList.AdditionalColumns = "SKUID, DocumentSKUName, NodeParentID, NodeID, NodeSKUID, SKUName, SKUNumber, SKUPrice, SKUAvailableItems, SKUEnabled, SKUSiteID, SKUPublicStatusID, SKUInternalStatusID, SKUReorderAt, SKUTrackInventory";
            docList.WhereCodition = GetDocumentWhereCondition().ToString(true);
            docList.OrderBy = ShowSections ? "CASE WHEN NodeSKUID IS NULL THEN 0 ELSE 1 END, DocumentName" : "DocumentName";
            docList.OnExternalAdditionalDataBound += gridData_OnExternalDataBound;
            docList.OnDocumentFlagsCreating += docList_OnDocumentFlagsCreating;
            docList.Grid.OnAction += gridData_OnAction;
            docList.Grid.RememberStateByParam = "";
            docList.SelectLanguageJSFunction = "EditProductInCulture";
            docList.DeleteReturnUrl = "~/CMSModules/Content/CMSDesk/Delete.aspx?multiple=true";
            docList.PublishReturnUrl = "~/CMSModules/Content/CMSDesk/PublishArchive.aspx?multiple=true";
            docList.ArchiveReturnUrl = "~/CMSModules/Content/CMSDesk/PublishArchive.aspx?multiple=true";
            docList.TranslateReturnUrl = "~/CMSModules/Translations/Pages/TranslateDocuments.aspx";

            if (!string.IsNullOrEmpty(ProductsStartingPath))
            {
                docList.CopyMoveLinkStartingPath = ProductsStartingPath;
            }

            string languageSelectionScript = String.Format(
@"
function EditProductInCulture(nodeId, culture, translated, url) {{
    parent.RefreshTree(nodeId, nodeId); 
    window.location.href = '{0}&nodeid=' + nodeId + '&culture=' + culture;
    parent.ChangeLanguage(culture);
}}
",
                ProductUIHelper.GetProductEditUrl()
            );

            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "EditProductInCulture", ScriptHelper.GetScript(languageSelectionScript));

            plcSKUListing.Visible = false;

            // Stop processing SKU table
            gridData.StopProcessing = true;

            EditedObject = docList.Node;

            // Set title
            string title = docList.Node.IsRoot() ? GetString("com.sku.productslist") : docList.Node.GetDocumentName();
            SetTitle(HTMLHelper.HTMLEncode(ResHelper.LocalizeString(title)));
        }
        else
        {
            // Init Unigrid
            gridData.OnAction += gridData_OnAction;
            gridData.OnExternalDataBound += gridData_OnExternalDataBound;

            // Stop processing product document listing
            docList.StopProcessing = true;
            plcDocumentListing.Visible = false;

            // Set title according display tree setting
            if (DisplayTreeInProducts)
            {
                SetTitle(GetString("com.sku.unassignedlist"));
            }
            else
            {
                SetTitle(GetString("com.sku.productslist"));
            }
        }

        // Show warning when exchange rate from global main currency is missing
        if (AllowGlobalObjects && ExchangeTableInfoProvider.IsExchangeRateFromGlobalMainCurrencyMissing(SiteContext.CurrentSiteID))
        {
            ShowWarning(GetString("com.NeedExchangeRateFromGlobal"));
        }
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // Register listing scripts
        ScriptHelper.RegisterStartupScript(this, typeof(String), "ListingContentAppHash", "cmsListingContentApp = '" + UIContextHelper.GetApplicationHash("cms.content", "content") + "';", true);
        ScriptHelper.RegisterScriptFile(this, @"~/CMSModules/Content/CMSDesk/View/Listing.js");

        // Register tooltip scripts
        RegisterTooltipScript();

        if (DocumentListingDisplayed)
        {
            // No data message for document mode
            docList.Grid.ZeroRowsText = GetString("general.nodatafound");

            // Do not hide product filter
            docList.Grid.FilterLimit = 0;

            if (ShowSections)
            {
                CreateCloseListingLink();
            }
        }
        else
        {
            // No data message for SKU mode
            gridData.ZeroRowsText = GetString("general.nodatafound");
            gridData.WhereCondition = GetWhereCondition().ToString(true);
        }

        InitializeMasterPage();
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Hide "Is global" flag when global products are not allowed
        if (DocumentListingDisplayed)
        {
            docList.Grid.NamedColumns["SKUSiteID"].Visible = AllowGlobalObjects;

            if (ShowSections)
            {
                // Generalize column header when listing products mixed with sections
                docList.Grid.NamedColumns["documentname"].HeaderText = GetString("general.name");
            }
        }
        else
        {
            gridData.NamedColumns["SKUSiteID"].Visible = AllowGlobalObjects;
        }
    }


    protected override void OnPreRenderComplete(EventArgs e)
    {
        // Reload grid if data were updated using inline edit
        if (forceReloadData)
        {
            docList.Grid.GetReloadScript();

            ScriptHelper.RegisterClientScriptBlock(Page, GetType(), "RefreshGrid", docList.Grid.GetReloadScript(), true);

            forceReloadData = false;
        }

        base.OnPreRenderComplete(e);
    }

    #endregion


    #region "UniGrid event handlers"

    private object docList_OnDocumentFlagsCreating(object sender, string sourceName, object parameter)
    {
        DataRowView row = parameter as DataRowView;

        // Mark irrelevant cells in full listing mode
        if (DocumentListingDisplayed && ShowSections && (row != null) && (row["NodeSKUID"] == DBNull.Value))
        {
            return NO_DATA_CELL_VALUE;
        }

        return null;
    }


    private object gridData_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        DataRowView row = parameter as DataRowView;

        if (DocumentListingDisplayed)
        {
            switch (sourceName.ToLowerCSafe())
            {
                case "skunumber":
                case "skuavailableitems":
                case "publicstatusid":
                case "allowforsale":
                case "skusiteid":
                case "typename":
                case "skuprice":

                    if (ShowSections && (row["NodeSKUID"] == DBNull.Value))
                    {
                        return NO_DATA_CELL_VALUE;
                    }

                    break;

                case "edititem":
                    row = ((GridViewRow)parameter).DataItem as DataRowView;

                    if ((row != null) && ((row["NodeSKUID"] == DBNull.Value) || showProductsInTree))
                    {
                        CMSGridActionButton btn = sender as CMSGridActionButton;
                        if (btn != null)
                        {
                            int currentNodeId = ValidationHelper.GetInteger(row["NodeID"], 0);
                            int nodeParentId = ValidationHelper.GetInteger(row["NodeParentID"], 0);

                            if (row["NodeSKUID"] == DBNull.Value)
                            {
                                btn.IconCssClass = "icon-eye";
                                btn.IconStyle = GridIconStyle.Allow;
                                btn.ToolTip = GetString("com.sku.viewproducts");
                            }

                            // Go to the selected document
                            btn.OnClientClick = "EditItem(" + currentNodeId + ", " + nodeParentId + "); return false;";
                        }
                    }

                    break;
            }
        }

        switch (sourceName.ToLowerCSafe())
        {
            case "skunumber":
                string skuNumber = ValidationHelper.GetString(row["SKUNumber"], null);
                return HTMLHelper.HTMLEncode(ResHelper.LocalizeString(skuNumber) ?? "");

            case "skuavailableitems":
                var sku = new SKUInfo(row.Row);
                int availableItems = sku.SKUAvailableItems;

                // Inventory tracked by variants
                if (sku.SKUTrackInventory == TrackInventoryTypeEnum.ByVariants)
                {
                    return GetString("com.inventory.trackedbyvariants");
                }

                // Inventory tracking disabled
                if (sku.SKUTrackInventory == TrackInventoryTypeEnum.Disabled)
                {
                    return GetString("com.inventory.nottracked");
                }

                // Ensure correct values for unigrid export
                if (sender == null)
                {
                    return availableItems;
                }

                // Tracking by products
                InlineEditingTextBox inlineAvailableItems = new InlineEditingTextBox();
                inlineAvailableItems.Text = availableItems.ToString();
                inlineAvailableItems.DelayedReload = DocumentListingDisplayed;
                inlineAvailableItems.EnableEncode = false;

                inlineAvailableItems.Formatting += (s, e) =>
                {
                    var reorderAt = sku.SKUReorderAt;

                    // Emphasize the number when product needs to be reordered
                    if (availableItems <= reorderAt)
                    {
                        // Format message informing about insufficient stock level
                        string reorderMsg = string.Format(GetString("com.sku.reorderatTooltip"), reorderAt);
                        string message = string.Format("<span class=\"alert-status-error\" onclick=\"UnTip()\" onmouseout=\"UnTip()\" onmouseover=\"Tip('{1}')\">{0}</span>", availableItems, reorderMsg);
                        inlineAvailableItems.FormattedText = message;
                    }
                };

                // Unigrid with delayed reload in combination with inline edit control requires additional postback to sort data. 
                // Update data only if external data bound is called for the first time.
                if (!forceReloadData)
                {
                    inlineAvailableItems.Update += (s, e) =>
                    {
                        var newNumberOfItems = ValidationHelper.GetInteger(inlineAvailableItems.Text, availableItems);

                        if (ValidationHelper.IsInteger(inlineAvailableItems.Text) && (-1000000000 <= newNumberOfItems) && (newNumberOfItems <= 1000000000))
                        {
                            CheckModifyPermission(sku);

                            // Ensures that grid will display inserted value
                            sku.SKUAvailableItems = newNumberOfItems;

                            // Document list is used to display products -> document has to be updated to ensure correct sku mapping
                            if (DocumentListingDisplayed)
                            {
                                int documentId = ValidationHelper.GetInteger(row.Row["DocumentID"], 0);

                                // Create an instance of the Tree provider and select edited document with coupled data
                                TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);
                                TreeNode document = tree.SelectSingleDocument(documentId);

                                if (document == null)
                                {
                                    return;
                                }

                                document.SetValue("SKUAvailableItems", newNumberOfItems);
                                document.Update();

                                forceReloadData = true;
                            }
                            // Stand-alone product -> only product has to be updated
                            else
                            {
                                sku.MakeComplete(true);
                                sku.Update();

                                gridData.ReloadData();
                            }
                        }
                        else
                        {
                            inlineAvailableItems.ErrorText = GetString("com.productedit.availableitemsinvalid");
                        }
                    };
                }

                return inlineAvailableItems;

            case "skuprice":

                SKUInfo product = new SKUInfo(row.Row);

                // Ensure correct values for unigrid export
                if (sender == null)
                {
                    return product.SKUPrice;
                }

                InlineEditingTextBox inlineSkuPrice = new InlineEditingTextBox();
                inlineSkuPrice.Text = product.SKUPrice.ToString();
                inlineSkuPrice.DelayedReload = DocumentListingDisplayed;

                inlineSkuPrice.Formatting += (s, e) =>
                {
                    // Format price
                    inlineSkuPrice.FormattedText = CurrencyInfoProvider.GetFormattedPrice(product.SKUPrice, product.SKUSiteID);
                };

                // Unigrid with delayed reload in combination with inline edit control requires additional postback to sort data. 
                // Update data only if external data bound is called for the first time.
                if (!forceReloadData)
                {
                    inlineSkuPrice.Update += (s, e) =>
                    {
                        CheckModifyPermission(product);

                        // Price must be a double number
                        double price = ValidationHelper.GetDouble(inlineSkuPrice.Text, -1);

                        if (ValidationHelper.IsDouble(inlineSkuPrice.Text) && (price >= 0))
                        {
                            // Ensures that grid will display inserted price
                            product.SKUPrice = price;

                            // Document list is used to display products -> document has to be updated to ensure correct sku mapping
                            if (DocumentListingDisplayed)
                            {
                                int documentId = ValidationHelper.GetInteger(row.Row["DocumentID"], 0);

                                // Create an instance of the Tree provider and select edited document with coupled data
                                TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);
                                TreeNode document = tree.SelectSingleDocument(documentId);

                                if (document != null)
                                {
                                    document.SetValue("SKUPrice", price);
                                    document.Update();

                                    forceReloadData = true;
                                }
                            }
                            // Stand-alone product -> only product has to be updated
                            else
                            {
                                product.MakeComplete(true);
                                product.Update();

                                gridData.ReloadData();
                            }
                        }
                        else
                        {
                            inlineSkuPrice.ErrorText = GetString("com.productedit.priceinvalid");
                        }
                    };
                }

                return inlineSkuPrice;

            case "publicstatusid":
                int id = ValidationHelper.GetInteger(row["SKUPublicStatusID"], 0);
                PublicStatusInfo publicStatus = PublicStatusInfoProvider.GetPublicStatusInfo(id);
                if (publicStatus != null)
                {
                    // Localize and encode
                    return HTMLHelper.HTMLEncode(ResHelper.LocalizeString(publicStatus.PublicStatusDisplayName));
                }

                return string.Empty;

            case "allowforsale":
                // Get "on sale" flag
                return UniGridFunctions.ColoredSpanYesNo(ValidationHelper.GetBoolean(row["SKUEnabled"], false));

            case "typename":
                string docTypeName = ValidationHelper.GetString(row["ClassDisplayName"], null);

                // Localize class display name
                if (!string.IsNullOrEmpty(docTypeName))
                {
                    return HTMLHelper.HTMLEncode(ResHelper.LocalizeString(docTypeName));
                }

                return string.Empty;
        }

        return parameter;
    }


    private void gridData_OnAction(string actionName, object actionArgument)
    {
        int argument = ValidationHelper.GetInteger(actionArgument, 0);
        actionName = actionName.ToLowerCSafe();

        switch (actionName)
        {
            case "edit":
                {
                    string url = ProductUIHelper.GetProductEditUrl();
                    string query = DocumentListingDisplayed ? "?sectionId=" + NodeID + "&nodeId=" + argument + "&culture=" + CultureCode : "?productid=" + argument;

                    url = URLHelper.AppendQuery(url, query);

                    URLHelper.Redirect(url);
                }
                break;

            case "delete":
                if (DocumentListingDisplayed)
                {
                    URLHelper.Redirect("Product_Section.aspx?action=delete&nodeId=" + argument);
                }
                else
                {
                    SKUInfo skuObj = SKUInfoProvider.GetSKUInfo(argument);

                    // Check module permissions
                    CheckModifyPermission(skuObj);

                    // Check dependencies
                    if (SKUInfoProvider.CheckDependencies(argument))
                    {
                        // Show error message
                        ShowError(ECommerceHelper.GetDependencyMessage(skuObj));

                        return;
                    }

                    SKUInfoProvider.DeleteSKUInfo(skuObj);
                }

                break;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Adds the script to the output request window.
    /// </summary>
    /// <param name="script">Script to add</param>
    public override void AddScript(string script)
    {
        ScriptHelper.RegisterStartupScript(this, typeof(string), script.GetHashCode().ToString(), ScriptHelper.GetScript(script));
    }


    /// <summary>
    /// Creates link for closing full listing mode.
    /// </summary>
    private void CreateCloseListingLink()
    {
        string closeLink = "<a href=\"#\"><span class=\"ListingClose\" style=\"cursor: pointer;\" " +
                                       "onclick=\"parent.SetMode('edit'); return false;\">" + GetString("general.close") +
                                       "</span></a>";

        // Check if node has name
        if (!string.IsNullOrEmpty(docList.Node.DocumentName))
        {
            string docNamePath = "<span class=\"ListingPath\">" + HTMLHelper.HTMLEncode(ResHelper.LocalizeString(docList.Node.DocumentName)) + "</span>";
            lblListingInfo.Text = String.Format(GetString("com.productsection.listinginfo"), docNamePath, closeLink);
        }
        else
        {
            // Use link for root
            lblListingInfo.Text = String.Format(GetString("com.productsection.rootlistinginfo"), closeLink);
        }
    }


    /// <summary>
    /// Initializes the master page elements.
    /// </summary>
    private void InitializeMasterPage()
    {
        string url = "~/CMSModules/Ecommerce/Pages/Tools/Products/Product_New.aspx";
        url = URLHelper.AddParameterToUrl(url, "siteId", SiteContext.CurrentSiteID.ToString());
        url = URLHelper.AddParameterToUrl(url, "parentNodeId", NodeID.ToString());

        // Add "New product" action when tree is not visible (it has own 'New' action)
        if ((NodeID <= 0) && !DisplayTreeInProducts)
        {
            HeaderActions hdrActions = CurrentMaster.HeaderActions;

            hdrActions.ActionsList.Add(new HeaderAction
            {
                Text = GetString("com.sku.newsku"),
                RedirectUrl = url
            });
        }
    }


    /// <summary>
    /// Creates where condition for SKUs listing.
    /// </summary>
    private WhereCondition GetWhereCondition()
    {
        // Display ONLY products - not product options
        var where = new WhereCondition().WhereNull("SKUOptionCategoryID");

        // Select only products without documents
        if ((NodeID <= 0) && DisplayTreeInProducts)
        {
            where.WhereNotIn("SKUID", SKUInfoProvider.GetSKUs().Column("SKUID").From("View_CMS_Tree_Joined").WhereNotNull("NodeSKUID").And().WhereEquals("NodeSiteID", SiteContext.CurrentSiteID));
        }

        // Ordinary user can see only product from departments he can access
        var cui = MembershipContext.AuthenticatedUser;
        if (!cui.IsGlobalAdministrator && !cui.IsAuthorizedPerResource("CMS.Ecommerce", "AccessAllDepartments"))
        {
            where.Where(w => w.WhereNull("SKUDepartmentID").Or().WhereIn("SKUDepartmentID", new IDQuery<UserDepartmentInfo>("DepartmentID").WhereEquals("UserID", cui.UserID))); 
        }

        // Reflect "Allow global products" setting
        var siteWhere = InitSiteWhereCondition("SKUSiteID");
        
        return where.Where(siteWhere);
    }


    /// <summary>
    /// Creates default where condition for product documents listing.
    /// </summary>
    private WhereCondition GetDocumentWhereCondition()
    {
        var where = new WhereCondition().WhereNotNull("NodeSKUID");
        if (ShowSections)
        {
            where.Or().WhereIn("NodeClassID", new IDQuery<DataClassInfo>("ClassID").WhereTrue("ClassIsProductSection"));
        }

        // Prepare site condition
        var siteWhere = InitSiteWhereCondition("SKUSiteID");
        if (ShowSections)
        {
            siteWhere.Or().Where(w => w.WhereNull("SKUSiteID").And().WhereNull("SKUID"));
        }

        // Combine where conditions
        where.Where(siteWhere);
        if (docList.Node != null)
        {
            where.WhereStartsWith("NodeAliasPath", docList.Node.NodeAliasPath.TrimEnd('/') + "/");
        }

        return where;
    }


    /// <summary>
    /// Checks if user is authorized to modify product.
    /// </summary>
    private void CheckModifyPermission(SKUInfo skuObj)
    {
        // Check module permissions
        if (!ECommerceContext.IsUserAuthorizedToModifySKU(skuObj))
        {
            RedirectToAccessDenied("CMS.Ecommerce", skuObj.IsGlobal ? "EcommerceGlobalModify" : "EcommerceModify OR ModifyProducts");
        }
    }

    #endregion
}