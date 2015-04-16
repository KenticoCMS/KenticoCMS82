using System;
using System.Web.UI.WebControls;

using CMS.Helpers;
using CMS.Base;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.ExtendedControls;
using CMS.Modules;

public partial class CMSModules_UIPersonalization_Controls_UIPersonalization : CMSUserControl
{
    #region "Variables"

    private int mCurrentSiteID;
    private bool globalRoles;

    #endregion


    #region "Properties

    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
    }


    /// <summary>
    /// Gets or sets the ID of the module (if set, module selector is hidden).
    /// </summary>
    public int ResourceID
    {
        get;
        set;
    }


    /// <summary>
    /// If true, only current module is selected
    /// </summary>
    public bool SingleModule
    {
        get
        {
            return treeElem.SingleModule;
        }
        set
        {
            treeElem.SingleModule = value;
        }
    }


    /// <summary>
    /// Gets or sets the ID of the site (if set, site selector is hidden).
    /// </summary>
    public int SiteID
    {
        get;
        set;
    }


    /// <summary>
    /// If false hide site selector in all cases.
    /// </summary>
    public bool HideSiteSelector
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the ID of the role (if set, role selector is hidden).
    /// </summary>
    public int RoleID
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
            // Set is livesite
            selectSite.IsLiveSite = value;
            selectRole.IsLiveSite = value;
            treeElem.IsLiveSite = value;
            selectModule.IsLiveSite = value;
            plcMess.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Sets css class for tree.
    /// </summary>
    public string CssClass
    {
        set
        {
            pnlTree.CssClass = value;
        }
        get
        {
            return pnlTree.CssClass;
        }
    }

    #endregion


    #region "Private properties"

    /// <summary>
    /// Gets ID of the currently processed site.
    /// </summary>
    private int CurrentSiteID
    {
        get
        {
            if (mCurrentSiteID == 0)
            {
                mCurrentSiteID = GetSiteID();
                if ((mCurrentSiteID == 0) && (selectSite != null))
                {
                    try
                    {
                        selectSite.AllowEmpty = false;
                        selectSite.Reload(true);
                        selectSite.DataBind();
                    }
                    catch
                    {
                    }

                    mCurrentSiteID = ValidationHelper.GetInteger(selectSite.DropDownSingleSelect.SelectedValue, 0);
                }
            }
            return mCurrentSiteID;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        // Initialize header actions
        actionsElem.ActionsList.Add(new HeaderAction
        {
            Text = GetString("uiprofile.expandall"),
            Tooltip = GetString("uiprofile.expandall"),
            CommandName = "expandall"
        });

        actionsElem.ActionsList.Add(new HeaderAction
        {
            Text = GetString("uiprofile.collapseall"),
            Tooltip = GetString("uiprofile.collapseall"),
            CommandName = "collapseall"
        });

        actionsElem.ActionPerformed += actionsElem_ActionPerformed;

        // Hide checkboxes with "group." prefix for WYSIWYG Editor
        treeElem.GroupPreffix = "group.";

        // Initialize selectors
        if (ResourceID > 0)
        {
            plcModule.Visible = false;
            selectModule.StopProcessing = true;
        }
        else
        {
            selectModule.UniSelector.OnSelectionChanged += selectModule_OnSelectionChanged;
            selectModule.DropDownSingleSelect.AutoPostBack = true;
            lblModule.AssociatedControlClientID = selectModule.DropDownSingleSelect.ClientID;
            if (!URLHelper.IsPostback())
            {
                // Module preselection from query string
                string selectedModule = QueryHelper.GetString("module", null);
                if (!String.IsNullOrEmpty(selectedModule))
                {
                    ResourceInfo ri = ResourceInfoProvider.GetResourceInfo(selectedModule);
                    if (ri != null)
                    {
                        selectModule.Value = ri.ResourceId;
                    }
                }
            }
        }

        selectRole.CurrentSelector.SelectionMode = SelectionModeEnum.SingleDropDownList;
        selectRole.DropDownSingleSelect.AutoPostBack = true;
        selectRole.CurrentSelector.OnSelectionChanged += selectRole_OnSelectionChanged;
        lblRole.AssociatedControlClientID = selectRole.DropDownSingleSelect.ClientID;

        if (HideSiteSelector)
        {
            plcSite.Visible = false;
        }
        else
        {
            selectSite.AllowGlobal = true;
            selectSite.DropDownSingleSelect.AutoPostBack = true;
            selectSite.UniSelector.OnSelectionChanged += selectSite_OnSelectionChanged;
            selectSite.IsLiveSite = IsLiveSite;
            lblSite.AssociatedControlClientID = selectSite.DropDownSingleSelect.ClientID;
        }

        if (!URLHelper.IsPostback())
        {
            // Site selector in direct UI personalization
            if (SiteID <= 0)
            {
                selectSite.SiteID = ValidationHelper.GetInteger(selectSite.GlobalRecordValue, 0);
                globalRoles = true;
            }
            else
            {
                selectSite.SiteID = CurrentSiteID;
            }

            ReloadRoles();
            if (ResourceID <= 0)
            {
                ReloadModules();
            }
        }

        globalRoles = (ValidationHelper.GetString(selectSite.Value, "") == selectSite.GlobalRecordValue);

        if (RoleID > 0)
        {
            plcRole.Visible = false;
        }
        else
        {
            selectRole.SiteID = CurrentSiteID;
        }

        if (RoleID > 0 && ((SiteID > 0) || HideSiteSelector) && ResourceID > 0)
        {
            pnlActions.Visible = false;
        }

        selectModule.SiteID = CurrentSiteID;

        // Check manage permission
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.UIPersonalization", CMSAdminControl.PERMISSION_MODIFY))
        {
            treeElem.Enabled = false;
            lblInfo.Text = String.Format(GetString("general.accessdeniedonpermissionname"), CMSAdminControl.PERMISSION_MODIFY);
            lblInfo.Visible = true;
        }

        ReloadTree();
        ucDisabledModule.InfoText = GetString("uiprofile.disabled");
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (SiteID > 0)
        {
            SiteInfo si = SiteInfoProvider.GetSiteInfo(SiteID);
            if (si != null)
            {
                ucDisabledModule.SiteName = si.SiteName;
            }
        }
        else
        {
            ucDisabledModule.SiteName = selectSite.SiteName;
        }
        if (!selectRole.CurrentSelector.HasData)
        {
            pnlTree.Visible = false;
            pnlAdditionalControls.Visible = false;
            ShowWarning(GetString("uiprofile.norole"));
        }
        else
        {
            if (plcModule.Visible && !selectModule.UniSelector.HasData)
            {
                pnlTree.Visible = false;
                pnlAdditionalControls.Visible = false;
                ShowWarning(GetString("uiprofile.nomodule"));
            }
            else
            {
                pnlTree.Visible = true;
            }
        }
    }


    #region "Event handlers"

    protected void actionsElem_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName.ToLowerCSafe())
        {
            case "expandall":
                treeElem.CollapseAll = false;
                treeElem.ExpandAll = true;
                break;

            case "collapseall":
                treeElem.CollapseAll = true;
                treeElem.ExpandAll = false;
                break;
        }
        ReloadTree();
    }


    protected void selectSite_OnSelectionChanged(object sender, EventArgs e)
    {
        ReloadRoles();
        ReloadModules();
        ReloadTree();
    }


    protected void selectRole_OnSelectionChanged(object sender, EventArgs e)
    {
        ReloadModules();
        ReloadTree();
    }


    protected void selectModule_OnSelectionChanged(object sender, EventArgs e)
    {
        ReloadTree();
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Reloads the tree.
    /// </summary>
    private void ReloadRoles()
    {
        selectRole.GlobalRoles = globalRoles;
        selectRole.SiteRoles = !globalRoles;

        selectRole.SiteID = CurrentSiteID;
        selectRole.Reload(true);
    }


    /// <summary>
    /// Reloads the modules.
    /// </summary>
    private void ReloadModules()
    {
        selectModule.DisplayOnlyForGivenSite = globalRoles ? false : true;
        selectModule.SiteID = globalRoles ? 0 : CurrentSiteID;        
        selectModule.ReloadData(true);
    }


    /// <summary>
    /// Reloads the tree.
    /// </summary>
    private void ReloadTree()
    {
        treeElem.SiteID = globalRoles ? 0 : CurrentSiteID;

        // Use gievn RoleID if explicitly given
        treeElem.RoleID = RoleID > 0 ? RoleID : ValidationHelper.GetInteger(selectRole.Value, 0);
        if (ResourceID > 0)
        {
            treeElem.ModuleID = ResourceID;
        }
        else
        {
            treeElem.ModuleID = ValidationHelper.GetInteger(selectModule.Value, 0);
        }
        if (treeElem.RoleID > 0)
        {
            treeElem.ReloadData();
        }
    }


    /// <summary>
    /// Gets the site ID from the selector.
    /// </summary>
    private int GetSiteID()
    {
        if (SiteID > 0)
        {
            return SiteID;
        }
        else
        {
            return (URLHelper.IsPostback() ? selectSite.SiteID : SiteContext.CurrentSiteID);
        }
    }

    #endregion
}