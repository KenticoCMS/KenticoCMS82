using System;

using CMS.Core;
using CMS.Ecommerce;
using CMS.Helpers;
using CMS.Base;
using CMS.SiteProvider;
using CMS.DocumentEngine;
using CMS.DataEngine;
using CMS.PortalEngine;
using CMS.UIControls;

[UIElement(ModuleName.ECOMMERCE, "Products")]
public partial class CMSModules_Ecommerce_Pages_Tools_Products_Product_Section : CMSProductsPage
{
    #region "Private & protected variables"

    protected string viewpage = "~/CMSPages/blank.htm";
    private DataClassInfo classInfo;
    private int parentNodeID;

    #endregion


    #region "Private properties"

    private TreeNode TreeNode
    {
        get;
        set;
    }

    #endregion


    #region "Page events"

    protected override void OnPreInit(EventArgs e)
    {
        parentNodeID = QueryHelper.GetInteger("parentNodeId", 0);

        if (parentNodeID > 0)
        {
            CheckExploreTreePermission();
        }

        // Do not redirect when document does not exist
        DocumentManager.RedirectForNonExistingDocument = false;
        
        base.OnPreInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Register script files
        ScriptHelper.RegisterScriptFile(this, @"~/CMSModules/Content/CMSDesk/EditTabs.js");

        bool checkCulture = false;
        bool splitViewSupported = false;
        string action = QueryHelper.GetString("action", "edit").ToLowerCSafe();

        switch (action)
        {
            // New dialog / new page form
            case "new":
                int classId = QueryHelper.GetInteger("classid", 0);
                if (classId <= 0)
                {
                    // Get by class name if specified
                    string className = QueryHelper.GetString("classname", string.Empty);
                    if (className != string.Empty)
                    {
                        classInfo = DataClassInfoProvider.GetDataClassInfo(className);
                        if (classInfo != null)
                        {
                            classId = classInfo.ClassID;
                        }
                    }
                }

                if (classId > 0)
                {
                    viewpage = ResolveUrl("~/CMSModules/Content/CMSDesk/Edit/Edit.aspx");

                    // Check if document type is allowed under parent node
                    if (parentNodeID > 0)
                    {
                        // Get the node
                        TreeNode = Tree.SelectSingleNode(parentNodeID, TreeProvider.ALL_CULTURES);
                        if (TreeNode != null)
                        {
                            if (!DocumentHelper.IsDocumentTypeAllowed(TreeNode, classId))
                            {
                                viewpage = "~/CMSModules/Content/CMSDesk/NotAllowed.aspx?action=child";
                            }
                        }
                    }

                    // Use product page when product type is selected
                    classInfo = classInfo ?? DataClassInfoProvider.GetDataClassInfo(classId);
                    if ((classInfo != null) && (classInfo.ClassIsProduct))
                    {
                        viewpage = ResolveUrl("~/CMSModules/Ecommerce/Pages/Tools/Products/Product_New.aspx");
                    }
                }
                else
                {
                    if (parentNodeID > 0)
                    {
                        viewpage = "~/CMSModules/Ecommerce/Pages/Tools/Products/New_ProductOrSection.aspx";
                    }
                    else
                    {
                        viewpage = "~/CMSModules/Ecommerce/Pages/Tools/Products/Product_New.aspx?parentNodeId=0";
                    }
                }
                break;

            case "delete":
                // Delete dialog
                viewpage = "~/CMSModules/Content/CMSDesk/Delete.aspx";
                break;

            default:
                // Edit mode
                viewpage = "~/CMSModules/Content/CMSDesk/Edit/edit.aspx?mode=editform";
                splitViewSupported = true;

                // Ensure class info
                if ((classInfo == null) && (Node != null))
                {
                    classInfo = DataClassInfoProvider.GetDataClassInfo(Node.NodeClassName);
                }

                // Check explicit editing page url
                if ((classInfo != null) && !string.IsNullOrEmpty(classInfo.ClassEditingPageURL))
                {
                    viewpage = URLHelper.AppendQuery(ResolveUrl(classInfo.ClassEditingPageURL), RequestContext.CurrentQueryString);
                }

                checkCulture = true;
                break;
        }

        // If culture version should be checked, check
        if (checkCulture)
        {
            // Check (and ensure) the proper content culture
            if (!CheckPreferredCulture())
            {
                RefreshParentWindow();
            }

            // Check split mode 
            bool isSplitMode = UIContext.DisplaySplitMode;
            bool combineWithDefaultCulture = !isSplitMode && SiteInfoProvider.CombineWithDefaultCulture(SiteContext.CurrentSiteName);

            var nodeId = QueryHelper.GetInteger("nodeid", 0);
            TreeNode = Tree.SelectSingleNode(nodeId, CultureCode, combineWithDefaultCulture);
            if (TreeNode == null)
            {
                // Document does not exist -> redirect to new culture version creation dialog
                viewpage = ProductUIHelper.GetNewCultureVersionPageUrl();
            }
        }

        // Apply the additional transformations to the view page URL
        viewpage = URLHelper.AppendQuery(viewpage, RequestContext.CurrentQueryString);
        viewpage = URLHelper.RemoveParameterFromUrl(viewpage, "mode");
        viewpage = URLHelper.AddParameterToUrl(viewpage, "mode", "productssection");
        viewpage = ResolveUrl(viewpage);
        viewpage = URLHelper.AddParameterToUrl(viewpage, "hash", QueryHelper.GetHash(viewpage));

        // Split mode enabled
        if (splitViewSupported && UIContext.DisplaySplitMode && (TreeNode != null) && (action == "edit" || action == "preview" || (TreeNode.IsPublished && action == "livesite")))
        {
            viewpage = DocumentUIHelper.GetSplitViewUrl(viewpage);
        }

        URLHelper.Redirect(viewpage);
    }


    #endregion
}
