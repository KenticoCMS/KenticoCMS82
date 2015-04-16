using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

using CMS;
using CMS.Base;
using CMS.DocumentEngine;
using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.PortalControls;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;

using TreeNode = CMS.DocumentEngine.TreeNode;

[assembly: RegisterCustomClass("ContentEditTabsControlExtender", typeof(ContentEditTabsControlExtender))]

/// <summary>
/// Content edit tabs control extender
/// </summary>
public class ContentEditTabsControlExtender : UITabsExtender
{
    #region "Variables"

    private Panel pnlContent;
    private CMSCheckBox chkContent;

    private int designTabIndex = -1;

    private bool showProductTab;
    private bool showMasterPage;
    private bool showDesign;
    
    private bool isWireframe;
    private bool hasWireframe;

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

    #endregion


    #region "Methods"

    /// <summary>
    /// OnInit event handler
    /// </summary>
    public override void OnInit()
    {
        base.OnInit();

        // Check page security
        string mode = QueryHelper.GetString("mode", null);
        CMSContentPage.CheckSecurity(mode);

        Control.Page.Load += OnLoad;
    }


    /// <summary>
    /// Initialization of tabs.
    /// </summary>
    public override void OnInitTabs()
    {
        // Add show content checkbox
        AppendDesignContentCheckBox();

        Control.OnTabsLoaded += HandleContentCheckBox;
        Control.OnTabCreated += OnTabCreated;
    }


    /// <summary>
    /// Fires when the page loads
    /// </summary>
    private void OnLoad(object sender, EventArgs eventArgs)
    {
        var page = (CMSPage)Control.Page;

        var manager = page.DocumentManager;

        manager.RedirectForNonExistingDocument = false;
        manager.Tree.CombineWithDefaultCulture = false;

        var node = manager.Node;

        if (node != null)
        {
            Node = node;

            ScriptHelper.RegisterScriptFile(Control.Page, "~/CMSModules/Content/CMSDesk/EditTabs.js");

            // Document from different site
            if (node.NodeSiteID != SiteContext.CurrentSiteID)
            {
                URLHelper.Redirect(DocumentUIHelper.GetPageNotAvailable(string.Empty, false, node.DocumentName));
            }
           
            showProductTab = node.HasSKU;

            // Initialize required variables
            isWireframe = node.IsWireframe();
            hasWireframe = isWireframe || (node.NodeWireframeTemplateID > 0);

            try
            {
                var pi = PageInfoProvider.GetPageInfo(node.NodeSiteName, node.NodeAliasPath, node.DocumentCulture, node.DocumentUrlPath, false);
                if ((pi != null) && (pi.DesignPageTemplateInfo != null))
                {
                    var pti = pi.DesignPageTemplateInfo;

                    showMasterPage = pti.IsPortal && ((node.NodeAliasPath == "/") || pti.ShowAsMasterTemplate);

                    showDesign = ((pti.PageTemplateType == PageTemplateTypeEnum.Portal) || (pti.PageTemplateType == PageTemplateTypeEnum.AspxPortal));
                }
            }
            catch
            {
                // Page info not found - probably tried to display document from different site
            }

            if (node.NodeClassName.EqualsCSafe("CMS.File", true))
            {
                showDesign = false;
                showMasterPage = false;
            }

            DocumentUIHelper.EnsureDocumentBreadcrumbs(page.PageBreadcrumbs, node, null, null);
        }
        else if (!PortalContext.ViewMode.IsDesign(true))
        {
            // Document does not exist -> redirect to new culture version creation dialog
            RedirectToNewCultureVersionPage();
        }
    }


    /// <summary>
    /// Redirects to new document language version page.
    /// </summary>
    protected virtual void RedirectToNewCultureVersionPage()
    {
        URLHelper.Redirect(DocumentUIHelper.GetNewCultureVersionPageUrl());
    }


    /// <summary>
    /// Fires when a tab is created
    /// </summary>
    private void OnTabCreated(object sender, TabCreatedEventArgs e)
    {
        if (e.Tab == null)
        {
            return;
        }

        string mode = null;

        var element = e.UIElement;

        string elem = element.ElementName.ToLowerCSafe();

        // Hide other tabs for wireframe
        if (isWireframe && ((elem != "wireframe") && (elem != "editform")))
        {
            e.Tab = null;
            return;
        }

        switch (elem)
        {
            case "page":
                {
                    mode = "edit";
                }
                break;

            case "design":
                {
                    if (!showDesign)
                    {
                        e.Tab = null;
                        return;
                    }

                    mode = "design";
                }
                break;

            case "wireframe":
                {
                    if (!hasWireframe)
                    {
                        e.Tab = null;
                        return;
                    }

                    mode = "wireframe";
                }
                break;

            case "editform":
                // Keep edit form
                {
                    mode = "editform";
                }
                break;

            case "product":
                {
                    if (!showProductTab)
                    {
                        e.Tab = null;
                        return;
                    }

                    mode = "product";
                }
                break;

            case "masterpage":
                {
                    if (!showMasterPage)
                    {
                        e.Tab = null;
                        return;
                    }

                    mode = "masterpage";
                }
                break;

            case "properties":
                {
                    // Document properties
                    mode = "properties";
                }
                break;

            case "analytics":
                {
                    if (isWireframe)
                    {
                        e.Tab = null;
                        return;
                    }

                    mode = "analytics";
                }
                break;
        }

        if (Node != null)
        {
            var tab = e.Tab;

            var settings = new UIPageURLSettings
                {
                    Mode = mode,
                    Node = Node,
                    NodeID = Node.NodeID,
                    Culture = Node.DocumentCulture,
                    PreferredURL = tab.RedirectUrl
                };

            // Ensure correct URL
            tab.RedirectUrl = DocumentUIHelper.GetDocumentPageUrl(settings);
        }
    }


    /// <summary>
    /// Registers the page scripts
    /// </summary>
    private void HandleContentCheckBox(List<UITabItem> items)
    {
        var script = String.Format(
@"var IsCMSDesk = true;
function ShowContent(show) {{
    document.getElementById('{0}').style.display = show ? 'block' : 'none';
}}", pnlContent.ClientID);

        ScriptHelper.RegisterClientScriptBlock(Control.Page, typeof(string), "UserInterfaceEditTabsControlExtender", ScriptHelper.GetScript(script));

        UpdateTabs(items);
    }


    /// <summary>
    /// Append display content checkbox to tabs header
    /// </summary>
    private void AppendDesignContentCheckBox()
    {
        pnlContent = new Panel();
        pnlContent.ID = "pc";
        pnlContent.CssClass = "design-showcontent";

        chkContent = new CMSCheckBox();
        chkContent.Text = Control.GetString("EditTabs.DisplayContent");
        chkContent.ID = "chk";
        chkContent.AutoPostBack = true;
        chkContent.EnableViewState = false;
        chkContent.Checked = PortalHelper.DisplayContentInDesignMode;
        chkContent.CheckedChanged += ContentCheckBoxCheckedChanged;

        pnlContent.Controls.Add(chkContent);

        Control.Parent.Controls.Add(pnlContent);
    }


    /// <summary>
    /// Modifies and loads the tabs so that clicking on the tab will show/hide the content checkbox
    /// </summary>
    private void UpdateTabs(IEnumerable<UITabItem> items)
    {
        int index = 0;

        foreach (var tab in items)
        {
            var isDesign = (tab.TabName == "Design");
            var script = String.Format("ShowContent({0})", isDesign.ToString().ToLower());

            if (isDesign)
            {
                designTabIndex = index;
            }

            tab.OnClientClick = ScriptHelper.AddScript(tab.OnClientClick, script);

            index++;

            // Apply the extra script also to sub-items
            if (tab.HasSubItems)
            {
                UpdateTabs(tab.SubItems);
            }
        }
    }


    /// <summary>
    /// Fires when the show content checkbox changes
    /// </summary>
    private void ContentCheckBoxCheckedChanged(object sender, EventArgs e)
    {
        if (designTabIndex >= 0)
        {
            Control.SelectedTab = designTabIndex;
        }

        PortalHelper.DisplayContentInDesignMode = chkContent.Checked;
    }

    #endregion
}