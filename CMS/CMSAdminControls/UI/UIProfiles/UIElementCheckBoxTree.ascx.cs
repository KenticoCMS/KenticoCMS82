using System;
using System.Data;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.Modules;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSAdminControls_UI_UIProfiles_UIElementCheckBoxTree : CMSUserControl, ICallbackEventHandler
{
    #region "Variables"

    protected string mGroupPrefix = null;
    protected int mModuleID = 0;
    protected int mRoleID = 0;
    protected int mSiteID = 0;
    private bool mEnabled = true;
    private string mSiteName;
    private string mCallbackRef;
    private UIElementInfo root;

    #endregion


    #region "Constants"

    private const string SELECT_DESELECT_LINKS = @"&nbsp;(<span class='link' onclick=""SelectAllSubelements($cmsj(this), {0}, {1}); {2}; return false;"">{3}</span>,&nbsp;<span class='link' onclick=""DeselectAllSubelements($cmsj(this), {0}, {1}); {2};return false;"" >{4}</span>)";

    #endregion


    #region "Properties"

    /// <summary>
    /// Indicates if all nodes should be expanded.
    /// </summary>
    public bool ExpandAll
    {
        get
        {
            return treeElem.ExpandAll;
        }
        set
        {
            treeElem.ExpandAll = value;
        }
    }


    /// <summary>
    /// Indicates if all nodes should be collapsed.
    /// </summary>
    public bool CollapseAll
    {
        get
        {
            return treeElem.CollapseAll;
        }
        set
        {
            treeElem.CollapseAll = value;
        }
    }


    /// <summary>
    /// Gets or sets the prefix of the element name which should not have the checkbox.
    /// </summary>
    public string GroupPreffix
    {
        get
        {
            return mGroupPrefix;
        }
        set
        {
            mGroupPrefix = value;
        }
    }


    /// <summary>
    /// ID of the module.
    /// </summary>
    public int ModuleID
    {
        get
        {
            return mModuleID;
        }
        set
        {
            mModuleID = value;
        }
    }


    /// <summary>
    /// ID of the role.
    /// </summary>
    public int RoleID
    {
        get
        {
            return mRoleID;
        }
        set
        {
            mRoleID = value;
        }
    }


    /// <summary>
    /// ID of the site.
    /// </summary>
    public int SiteID
    {
        get
        {
            return mSiteID;
        }
        set
        {
            mSiteID = value;
        }
    }


    /// <summary>
    /// If true, only current module is shown
    /// </summary>
    public bool SingleModule
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if control is used on live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            base.IsLiveSite = value;
            treeElem.IsLiveSite = false;
        }
    }


    /// <summary>
    /// Indicates if checkboxes and select/deselect all should be enabled.
    /// </summary>
    public bool Enabled
    {
        get
        {
            return mEnabled;
        }
        set
        {
            mEnabled = value;
        }
    }


    /// <summary>
    /// Name of the site.
    /// </summary>
    private string SiteName
    {
        get
        {
            if (mSiteName == null)
            {
                SiteInfo si = SiteInfoProvider.GetSiteInfo(SiteID);
                if (si != null)
                {
                    mSiteName = si.SiteName;
                }
            }
            return mSiteName;
        }
    }


    /// <summary>
    /// Callback reference for selecting items.
    /// </summary>
    private string CallbackRef
    {
        get
        {
            if (String.IsNullOrEmpty(mCallbackRef))
            {
                mCallbackRef = Page.ClientScript.GetCallbackEventReference(this, "hdnValue.value", "callbackHandler", "callbackHandler");
            }

            return mCallbackRef;
        }
    }

    #endregion


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        treeElem.OnNodeCreated += treeElem_OnNodeCreated;
    }


    /// <summary>
    /// Page load event.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        // Register scripts
        ScriptHelper.RegisterJQuery(Page);
        ScriptHelper.RegisterStartupScript(Page, typeof(string), "UITreeCallbackHandler", ScriptHelper.GetScript(
            "var hdnValue = document.getElementById('" + hdnValue.ClientID + "'); function callbackHandler(content, context) {}"));

        // Use images according to culture
        treeElem.LineImagesFolder = GetImageUrl(CultureHelper.IsUICultureRTL() ? "RTL/Design/Controls/Tree" : "Design/Controls/Tree");

        string noChkSelectString = GetString("uiprofile.selectallconfirmation");
        string noChkDeselectString = GetString("uiprofile.deselectallconfirmation");
        string selectString = GetString("uiprofile.selectallcurrentconfirmation");
        string deselectString = GetString("uiprofile.deselectallcurrentconfirmation");

        // Register scripts only if enabled
        if (Enabled)
        {
            string script = string.Format(@"
function SelectAllSubelements(elem, id, hasChkBox) {{
    if ((hasChkBox ? confirm('{0}') : confirm('{1}'))) {{
        hdnValue.value = 's;' + id + ';' + (hasChkBox? 1 : 0);
        var tab = elem.parents('table');
        tab.find('input:enabled[type=checkbox]').attr('checked', 'checked');
        var node = tab.next();
        if ((node.length > 0)&&(node[0].nodeName.toLowerCase() == 'div')) {{
            node.find('input:enabled[type=checkbox]').attr('checked', 'checked');
        }}
    }}
}}
function DeselectAllSubelements(elem, id, hasChkBox) {{
    if ((hasChkBox ? confirm('{2}') : confirm('{3}'))) {{
        hdnValue.value = 'd;' + id + ';' + (hasChkBox? 1 : 0);
        var tab = elem.parents('table');
        tab.find('input:enabled[type=checkbox]').removeAttr('checked');
        var node = tab.next();
        if ((node.length > 0)&&(node[0].nodeName.toLowerCase() == 'div')) {{
            node.find('input:enabled[type=checkbox]').removeAttr('checked');
        }}
    }}
}}", selectString, noChkSelectString, deselectString, noChkDeselectString);
            ScriptHelper.RegisterClientScriptBlock(Page, typeof(Page), "UITreeSelectScripts", ScriptHelper.GetScript(script));
        }
    }


    protected TreeNode treeElem_OnNodeCreated(DataRow itemData, TreeNode defaultNode)
    {
        // Get data
        if (itemData != null)
        {
            int id = ValidationHelper.GetInteger(itemData["ElementID"], 0);
            int childCount = ValidationHelper.GetInteger(itemData["ElementChildCount"], 0);
            bool selected = ValidationHelper.GetBoolean(itemData["ElementSelected"], false);
            string displayName = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(ValidationHelper.GetString(itemData["ElementDisplayName"], string.Empty)));
            string elementName = ValidationHelper.GetString(itemData["ElementName"], string.Empty).ToLowerCSafe();
            int parentID = ValidationHelper.GetInteger(itemData["ElementParentID"], 0);
            int resourceID = ValidationHelper.GetInteger(itemData["ElementResourceID"], 0);
            string nodePath = ValidationHelper.GetString(itemData["ElementIDPath"], string.Empty);
            string itemClass = "ContentTreeItem";
            string onClickDeclaration = string.Format(" var chkElem_{0} = document.getElementById('chk_{0}'); ", id);
            string onClickCommon = string.Format("  hdnValue.value = {0} + ';' + chkElem_{0}.checked; {1};", id, CallbackRef);
            string onClickSpan = string.Format(" chkElem_{0}.checked = !chkElem_{0}.checked; ", id);

            // Expand for root
            if (parentID == 0)
            {
                defaultNode.Expanded = true;
            }

            string[] paths = treeElem.ExpandPath.ToLowerCSafe().Split(';');
            if (!nodePath.EndsWith("/"))
            {
                nodePath += "/";
            }

            bool chkEnabled = Enabled;
            if (!SingleModule && (ModuleID > 0))
            {
                bool isEndItem = false;
                bool isChild = false;
                bool isParent = false;

                // Check expanded paths
                for (int i = 0; i < paths.Length; i++)
                {
                    String path = paths[i];
                    if (path != String.Empty)
                    {
                        // Add slash - select only children
                        if (!path.EndsWith("/"))
                        {
                            path += "/";
                        }

                        if (!isChild)
                        {
                            isChild = nodePath.StartsWith(path);
                        }

                        // Module node is same node as specified in paths collection
                        isEndItem = (path == nodePath);

                        // Test for parent - expand
                        if ((path.StartsWithCSafe(nodePath)))
                        {
                            defaultNode.Expanded = true;
                            isParent = true;
                            break;
                        }
                    }
                }

                // Display for non selected module items
                if (resourceID != ModuleID)
                {
                    // Parent special css
                    if (isParent)
                    {
                        itemClass += " highlighted disabled";
                    }
                    else
                    {
                        // Disable non parent
                        chkEnabled = false;
                        itemClass += " disabled";
                    }
                }
                else if (isEndItem)
                {
                    // Special class for end module item
                    itemClass += " highlighted";
                }
            }

            // Get button links
            string links = null;

            string nodeText;
            if (!String.IsNullOrEmpty(GroupPreffix) && elementName.ToLowerCSafe().StartsWithCSafe(GroupPreffix.ToLowerCSafe()))
            {
                if (childCount > 0 && chkEnabled)
                {
                    links = string.Format(SELECT_DESELECT_LINKS, id, "false", CallbackRef, GetString("uiprofile.selectall"), GetString("uiprofile.deselectall"));
                }
                nodeText = string.Format("<span class='{0}'>{1}</span>{2}", itemClass, displayName, links);
            }
            else
            {
                string warning = string.Empty;

                if (SiteName != null)
                {
                    if (!ResourceSiteInfoProvider.IsResourceOnSite(UIContextHelper.GetResourceName(resourceID), SiteName))
                    {
                        warning = UIHelper.GetAccessibleIconTag("icon-exclamation-triangle", String.Format(GetString("uiprofile.warningmodule"), "cms." + elementName), additionalClass: "color-orange-80");
                    }
                }

                if (childCount > 0 && chkEnabled)
                {
                    links = string.Format(SELECT_DESELECT_LINKS, id, "true", CallbackRef, GetString("uiprofile.selectall"), GetString("uiprofile.deselectall"));
                }

                nodeText = string.Format(@"<span class='checkbox tree-checkbox'><input type='checkbox' id='chk_{0}' name='chk_{0}'{1}{2} onclick=""{3}"" /><label for='chk_{0}'>&nbsp;</label><span class='{4}' onclick=""{5} return false;""><span class='Name'>{6}</span></span>{7}</span>{8}",
                                         id,
                                         chkEnabled ? string.Empty : " disabled='disabled'",
                                         selected ? " checked='checked'" : string.Empty,
                                         chkEnabled ? onClickDeclaration + onClickCommon : "return false;",
                                         itemClass,
                                         chkEnabled ? onClickDeclaration + onClickSpan + onClickCommon : "return false;",
                                         displayName,
                                         warning,
                                         links);
            }

            defaultNode.ToolTip = string.Empty;
            defaultNode.Text = nodeText;
        }

        return defaultNode;
    }


    /// <summary>
    /// Reloads the tree data.
    /// </summary>
    public void ReloadData()
    {
        // Prepare the parameters
        QueryDataParameters parameters = new QueryDataParameters();
        parameters.Add("@RoleID", RoleID);

        // Create and set UIElements provider
        UniTreeProvider elementProvider = new UniTreeProvider();
        elementProvider.QueryName = "cms.uielement.selecttree";
        elementProvider.DisplayNameColumn = "ElementDisplayName";
        elementProvider.IDColumn = "ElementID";
        elementProvider.LevelColumn = "ElementLevel";
        elementProvider.OrderColumn = "ElementOrder";
        elementProvider.ParentIDColumn = "ElementParentID";
        elementProvider.PathColumn = "ElementIDPath";
        elementProvider.ValueColumn = "ElementID";
        elementProvider.ChildCountColumn = "ElementChildCount";
        elementProvider.ImageColumn = "ElementIconPath";
        elementProvider.Parameters = parameters;
        elementProvider.IconClassColumn = "ElementIconClass";

        treeElem.ExpandTooltip = GetString("general.expand");
        treeElem.CollapseTooltip = GetString("general.collapse");
        treeElem.UsePostBack = false;
        treeElem.EnableRootAction = false;
        treeElem.ProviderObject = elementProvider;
        if (SingleModule)
        {
            ResourceInfo ri = ResourceInfoProvider.GetResourceInfo(ModuleID);
            if (ri != null)
            {
                root = UIElementInfoProvider.GetModuleTopUIElement(ModuleID);
                if (root != null)
                {
                    treeElem.ExpandPath = root.ElementIDPath;
                    string links = null;
                    if (Enabled)
                    {
                        links = string.Format(SELECT_DESELECT_LINKS, root.ElementID, "false", CallbackRef, GetString("uiprofile.selectall"), GetString("uiprofile.deselectall"));
                    }
                    string rootText = HTMLHelper.HTMLEncode(ri.ResourceDisplayName) + links;
                    treeElem.SetRoot(rootText, root.ElementID.ToString(), ResolveUrl(root.ElementIconPath));
                    elementProvider.RootLevelOffset = root.ElementLevel;
                }

                elementProvider.WhereCondition = "ElementResourceID=" + ModuleID;
            }
        }
        else
        {
            if (ModuleID > 0)
            {
                String where = String.Format(@"ElementResourceID = {0} AND (ElementParentID IS NULL OR ElementParentID NOT IN (SELECT ElementID FROM CMS_UIElement WHERE ElementResourceID={0})) 
                                                AND (NOT EXISTS (SELECT  ElementIDPath FROM CMS_UIElement AS u WHERE CMS_UIElement.ElementIDPath LIKE u.ElementIDPath + '%' AND ElementResourceID = {0}
                                                AND u.ElementIDPath != CMS_UIElement.ElementIDPath))", ModuleID);
                DataSet ds = UIElementInfoProvider.GetUIElements(where, "ElementLevel ASC");
                String expandedPath = String.Empty;
                if (!DataHelper.DataSourceIsEmpty(ds))
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        String path = ValidationHelper.GetString(dr["ElementIDPath"], String.Empty);
                        if (path != String.Empty)
                        {
                            expandedPath += path + ";";
                        }
                    }
                }

                treeElem.ExpandPath = expandedPath;
            }

        }

        treeElem.ReloadData();
    }


    /// <summary>
    /// Recursivelly select or deselect all child elements.
    /// </summary>
    /// <param name="select">Determines the type of action</param>
    /// <param name="parentId">ID of the parent UIElement</param>
    /// <param name="excludeRoot">Indicates whether to exclude root element from selection/deselection</param>
    private void SelectDeselectAll(bool select, int parentId, bool excludeRoot)
    {
        // Check manage permission
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.UIPersonalization", CMSAdminControl.PERMISSION_MODIFY))
        {
            RedirectToAccessDenied("CMS.UIPersonalization", CMSAdminControl.PERMISSION_MODIFY);
        }

        // Get the children and select them
        string where = (ModuleID > 0) ?
            String.Format(@"(ElementResourceID = {0} OR EXISTS (SELECT ElementID FROM CMS_UIElement AS x WHERE x.ElementIDPath like CMS_UIElement.ElementIDPath+ '%' AND x.ElementResourceID = {0})) AND
                            ElementIDPath LIKE (SELECT TOP 1 ElementIDPath FROM CMS_UIElement WHERE ElementID = {1}) + '%' ", ModuleID, parentId) :
                           "ElementIDPath LIKE (SELECT TOP 1 ElementIDPath FROM CMS_UIElement WHERE ElementID = " + parentId + ") + '%' ";
        if (excludeRoot)
        {
            where += " AND NOT ElementID = " + parentId;
        }
        if (!String.IsNullOrEmpty(GroupPreffix))
        {
            where += " AND ElementName NOT LIKE '" + SqlHelper.EscapeLikeText(SqlHelper.EscapeQuotes(GroupPreffix)) + "%'";
        }

        using (CMSActionContext context = new CMSActionContext())
        {
            // Many updates caused deadlocks with CMS_Role table, disable touch parent of the role
            context.TouchParent = false;

            DataSet ds = UIElementInfoProvider.GetUIElements(where, null, 0, "ElementID");
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    int id = ValidationHelper.GetInteger(dr["ElementID"], 0);
                    if (select)
                    {
                        RoleUIElementInfoProvider.AddRoleUIElementInfo(RoleID, id);
                    }
                    else
                    {
                        RoleUIElementInfoProvider.DeleteRoleUIElementInfo(RoleID, id);
                    }
                }
            }

            // Explicitly touch the role only once
            var role = RoleInfoProvider.GetRoleInfo(RoleID);
            if (role != null)
            {
                role.Update();
            }
        }
    }


    #region "Callback handling"

    public string GetCallbackResult()
    {
        return string.Empty;
    }


    public void RaiseCallbackEvent(string eventArgument)
    {
        // Check manage permission
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.UIPersonalization", CMSAdminControl.PERMISSION_MODIFY))
        {
            return;
        }

        string[] test = eventArgument.Split(';');
        if ((test.Length == 2) || (test.Length == 3))
        {
            if (test.Length == 3)
            {
                bool excludeRoot = !ValidationHelper.GetBoolean(test[2], false);
                if (test[0] == "s")
                {
                    int id = ValidationHelper.GetInteger(test[1], 0);
                    SelectDeselectAll(true, id, excludeRoot);
                }
                else if (test[0] == "d")
                {
                    // Deselect all action
                    int id = ValidationHelper.GetInteger(test[1], 0);
                    SelectDeselectAll(false, id, excludeRoot);
                }
            }
            else if (test.Length == 2)
            {
                // Basic checkbox click
                int id = ValidationHelper.GetInteger(test[0], 0);
                bool chk = ValidationHelper.GetBoolean(test[1], false);

                if (chk)
                {
                    RoleUIElementInfoProvider.AddRoleUIElementInfo(RoleID, id);
                }
                else
                {
                    RoleUIElementInfoProvider.DeleteRoleUIElementInfo(RoleID, id);
                }
            }

            // Invalidate all users
            UserInfo.TYPEINFO.InvalidateAllObjects();
        }
    }

    #endregion
}