using System;

using CMS.Base;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.ExtendedControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.Helpers;
using CMS.Membership;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;

using TreeNode = CMS.DocumentEngine.TreeNode;

[Title("Content.ListingTitle")]
public partial class CMSModules_Content_CMSDesk_View_Listing : CMSContentPage
{
    #region "Page events"

    protected override void OnPreInit(EventArgs e)
    {
        // Disable permissions check
        CheckDocPermissions = false;

        base.OnPreInit(e);
    }


    protected override void OnInit(EventArgs e)
    {
        // Do not check changes
        DocumentManager.RegisterSaveChangesScript = false;

        base.OnInit(e);

        var grid = docList.Grid;

        grid.GridName = "Listing.xml";

        // Ensure dialog parameters
        string dialogParameter = String.Empty;
        if (RequiresDialog)
        {
            dialogParameter = "&dialog=1";
            grid.GridName = "ListingDialog.xml";
            grid.Pager.DefaultPageSize = 10;
        }

        docList.NodeID = NodeID;
        docList.SelectItemJSFunction = "SelectItem";

        docList.DeleteReturnUrl = "~/CMSModules/Content/CMSDesk/Delete.aspx?multiple=true" + dialogParameter;
        docList.PublishReturnUrl = "~/CMSModules/Content/CMSDesk/PublishArchive.aspx?multiple=true" + dialogParameter;
        docList.ArchiveReturnUrl = "~/CMSModules/Content/CMSDesk/PublishArchive.aspx?multiple=true" + dialogParameter;
        docList.TranslateReturnUrl = "~/CMSModules/Translations/Pages/TranslateDocuments.aspx?currentastargetdefault=1" + dialogParameter;

        docList.RequiresDialog = RequiresDialog;

        SelectClass.SiteID = SiteContext.CurrentSiteID;
        SelectClass.DropDownSingleSelect.AutoPostBack = true;

        var filterForm = grid.FilterForm;

        filterForm.EnableViewState = true;

        if (ControlsHelper.CausedPostBack(true, SelectClass))
        {
            // Reset filter form's view state on class change
            filterForm.EnableViewState = false;

            grid.Reset();
        }
    }


    /// <summary>
    /// Page load.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        var currentUserInfo = MembershipContext.AuthenticatedUser;

        if (!RequiresDialog)
        {
            ScriptHelper.RegisterStartupScript(this, typeof(String), "ListingContentAppHash", "cmsListingContentApp = '" + UIContextHelper.GetApplicationHash("cms.content", "content") + "';", true);
            ScriptHelper.RegisterScriptFile(this, "~/CMSModules/Content/CMSDesk/View/Listing.js");
        }

        // Set selected document type
        docList.ClassID = ValidationHelper.GetInteger(SelectClass.Value, UniSelector.US_ALL_RECORDS);

        if (docList.NodeID > 0)
        {
            TreeNode node = docList.Node;
            // Set edited document
            EditedDocument = node;

            if (node != null)
            {
                if (currentUserInfo.IsAuthorizedPerDocument(node, NodePermissionsEnum.ExploreTree) != AuthorizationResultEnum.Allowed)
                {
                    RedirectToAccessDenied("CMS.Content", "exploretree");
                }

                if (RequiresDialog)
                {
                    ScriptHelper.RegisterScriptFile(this, @"~/CMSModules/Content/CMSDesk/View/ListingDialog.js");

                    // Set JavaScript for new button
                    CurrentMaster.HeaderActions.AddAction(new HeaderAction
                    {
                        Text = GetString("content.newtitle"),
                        OnClientClick = "AddItem(" + node.NodeID + ");"
                    });

                    // Ensure breadcrumbs
                    EnsureBreadcrumbs(node);
                }
                else
                {
                    // Ensure breadcrumbs for UI
                    EnsureDocumentBreadcrumbs(PageBreadcrumbs, node);
                    
                    // Setup the link to the parent document
                    if ((node.NodeClassName.ToLowerCSafe() != "cms.root") && (currentUserInfo.UserStartingAliasPath.ToLowerCSafe() != node.NodeAliasPath.ToLowerCSafe()))
                    {
                        CurrentMaster.HeaderActions.AddAction(new HeaderAction
                        {
                            Text = GetString("Listing.ParentDirectory"),
                            OnClientClick = "SelectItem(" + node.NodeParentID + ");"
                        });
                    }
                }

                // Define target window for modal dialogs (used for mobile Android browser which cannot open more than one modal dialog window at a time).
                // If set: The target window will be used for opening the new dialog in the following way: targetWindow.location.href = '...new dialog url...';
                // If not set: New modal dialog window will be opened 
                string actionTargetWindow = "var targetWindow = " + (DeviceContext.CurrentDevice.IsMobile ? "this" : "null");
                ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "listingScript", actionTargetWindow, true);
            }
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Ensures dialog breadcrumbs
    /// </summary>
    /// <param name="node">Current node</param>
    private void EnsureBreadcrumbs(TreeNode node)
    {
        if (node != null)
        {
            // Loop thru all levels and generate breadcrumbs
            int parentNodeId = 0;

            PageTitle.HideBreadcrumbs = false;

            for (int i = node.NodeLevel; i >= 0; i--)
            {
                if (node == null)
                {
                    // Document is not translated in the current culture -> get parent node from the default culture
                    TreeProvider treeProvider = new TreeProvider();
                    TreeNode parentNode = DocumentHelper.GetDocument(parentNodeId, CultureCode, true, treeProvider);
                    node = parentNode;
                }

                if (node != null)
                {
                    PageBreadcrumbs.Items.Add(new BreadcrumbItem
                    {
                        Text = node.GetDocumentName(),
                        Index = i,
                        RedirectUrl = "#",
                        OnClientClick = "SelectItem(" + node.NodeID + "); return false;"
                    });

                    parentNodeId = node.NodeParentID;
                    node = node.Parent;
                }
            }

            // Add additional css class for correct design
            CurrentMaster.PanelHeader.CssClass += " SimpleHeader";
        }
    }

    #endregion
}
