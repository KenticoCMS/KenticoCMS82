using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Helpers;
using CMS.Membership;
using CMS.ProjectManagement;
using CMS.Base;
using CMS.UIControls;
using CMS.ExtendedControls;
using CMS.ExtendedControls.ActionsConfig;

public partial class CMSModules_ProjectManagement_Controls_LiveControls_GroupProjects : CMSAdminControl
{
    #region "Variables"

    private bool displayControlPerformed = false;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the community group id.
    /// </summary>
    public int CommunityGroupID
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("CommunityGroupID"), 0);
        }
        set
        {
            SetValue("CommunityGroupID", value);
        }
    }


    /// <summary>
    /// Gets or sets the display mode of the control.
    /// </summary>
    public override ControlDisplayModeEnum DisplayMode
    {
        get
        {
            return base.DisplayMode;
        }
        set
        {
            base.DisplayMode = value;
            ucProjectNew.DisplayMode = value;
            ucProjectEdit.DisplayMode = value;
            ucProjectList.DisplayMode = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether control is used on live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("IsLiveSite"), base.IsLiveSite);
        }
        set
        {
            base.IsLiveSite = value;
            SetValue("IsLiveSite", value);
        }
    }

    #endregion


    #region "Page methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        #region "New project link"

        HeaderAction action = new HeaderAction();
        action.Text = GetString("pm.project.new");
        action.CommandName = "new_project";
        actionsElem.AddAction(action);

        actionsElem.ActionPerformed += actionsElem_ActionPerformed;

        #endregion

        // Breadcrumbs back handlers
        lnkBackHidden.Click += lnkBackHidden_Click;

        // Breadcrumb strings
        InitializeBreadcrumbs();

        // List settings
        ucProjectList.UsePostbackOnEdit = true;
        ucProjectList.OnAction += ucProjectList_OnAction;
        ucProjectList.CommunityGroupID = CommunityGroupID;

        // New item settings
        ucProjectNew.CommunityGroupID = CommunityGroupID;
        ucProjectNew.OnSaved += ucProjectNew_OnSaved;

        ucProjectNew.OnCheckPermissions += controls_OnCheckPermissions;
        ucProjectList.OnCheckPermissions += controls_OnCheckPermissions;
        ucProjectList.OnDelete += ucProjectList_OnDelete;
    }


    /// <summary>
    /// OnPreRender - ensures default displaying.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        // Display list by default
        if ((!displayControlPerformed) && (!RequestHelper.IsPostBack()))
        {
            DisplayControl("list");
        }

        // Ensure breadcrumbs
        if (String.IsNullOrEmpty(ucBreadcrumbs.Items[1].Text))
        {
            int projectID = ValidationHelper.GetInteger(ViewState["SelectedProject"], 0);
            if (projectID > 0)
            {
                ProjectInfo pi = ProjectInfoProvider.GetProjectInfo(projectID);
                ucBreadcrumbs.Items[1].Text = (pi != null) ? pi.ProjectDisplayName : "";
            }
        }

        base.OnPreRender(e);
    }

    #endregion


    #region "Event handler methods"

    /// <summary>
    /// New task created event handler => Display edit page
    /// </summary>
    protected void ucProjectNew_OnSaved(object sender, EventArgs e)
    {
        // Set breadcrumb to currently created project
        SetBreadcrumbs(ucProjectNew.ItemID);
        // Set current project id
        ucProjectEdit.ProjectID = ucProjectNew.ItemID;
        // Display edit controls
        DisplayControl("edit");
    }


    /// <summary>
    /// List action event handler.
    /// </summary>
    protected void ucProjectList_OnAction(object sender, CommandEventArgs e)
    {
        // Switch by command name
        switch (e.CommandName.ToString())
        {
            case "edit":
                // Get project id from command argument
                int projectID = ValidationHelper.GetInteger(e.CommandArgument, 0);
                // Set breadcrumbs to edited project
                SetBreadcrumbs(projectID);
                // Set project id
                ucProjectEdit.ProjectID = projectID;
                // Display edit controls
                DisplayControl("edit");
                ViewState["SelectedProject"] = projectID;
                break;
        }
    }


    /// <summary>
    /// Breadcrumbs back link click handler.
    /// </summary>
    protected void lnkBackHidden_Click(object sender, EventArgs e)
    {
        // Clear edit form
        ucProjectEdit.ClearForm();
        // Display project list
        DisplayControl("list");
    }


    /// <summary>
    /// New project link click handler.
    /// </summary>
    protected void actionsElem_ActionPerformed(object sender, CommandEventArgs e)
    {
        // Switch by command name
        switch (e.CommandName.ToLowerCSafe())
        {
                // New project
            case "new_project":
                // Set breadcrumbs to new project
                SetBreadcrumbs(0);
                // Display new project controls
                DisplayControl("new");
                break;
        }
    }


    /// <summary>
    /// Check permission handler.
    /// </summary>
    protected void controls_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        // If user is group admin => allow all actions
        if (!MembershipContext.AuthenticatedUser.IsGroupAdministrator(CommunityGroupID))
        {
            sender.StopProcessing = true;
        }
    }


    /// <summary>
    /// Handles the OnDelete event of the ucProjectList control.
    /// </summary>
    protected void ucProjectList_OnDelete(object sender, EventArgs e)
    {
        // Clear the ProjectID of the Edit control otherwise it could redirect you to the "Edited object no longer exists" page
        ucProjectEdit.ProjectID = 0;
        ViewState["SelectedProject"] = null;
    }

    #endregion


    #region "General Methods"

    /// <summary>
    /// Display control.
    /// </summary>
    /// <param name="control">Type of displayed control</param>
    public void DisplayControl(string control)
    {
        // Set displaying flag
        displayControlPerformed = true;

        // Hide all controls
        plcEdit.Visible = false;
        plcNew.Visible = false;
        pnlBody.Visible = false;

        // Switch by control type
        switch (control.ToLowerCSafe())
        {
                // List
            case "list":
                pnlBody.Visible = true;
                plcBreadcrumbs.Visible = false;
                ucProjectList.ReloadData();
                break;

                // Edit
            case "edit":
                plcEdit.Visible = true;
                plcBreadcrumbs.Visible = true;
                ucProjectEdit.ReloadData();
                break;

                // New
            case "new":
                plcNew.Visible = true;
                plcBreadcrumbs.Visible = true;
                ucProjectNew.ReloadData();
                break;
        }
    }


    /// <summary>
    /// Sets breadcrumbs for project.
    /// </summary>
    /// <param name="projectID">ID of project</param>
    private void SetBreadcrumbs(int projectID)
    {
        // If project id is defined display actual project name
        if (projectID != 0)
        {
            // Load project info
            ProjectInfo pi = ProjectInfoProvider.GetProjectInfo(projectID);
            // Check whether project info is defined
            if (pi != null)
            {
                ucBreadcrumbs.Items[1].Text = HTMLHelper.HTMLEncode(pi.ProjectDisplayName);
            }
        }
        // Display breadcrumbs for new project
        else
        {
            ucBreadcrumbs.Items[1].Text = GetString("pm.project.new");
        }
    }


    /// <summary>
    /// Initialize breadcrumb items.
    /// </summary>
    private void InitializeBreadcrumbs()
    {
        ucBreadcrumbs.Items.Clear();

        ucBreadcrumbs.AddBreadcrumb(new BreadcrumbItem {
            Text = GetString("pm.project.list"),
            OnClientClick = ControlsHelper.GetPostBackEventReference(lnkBackHidden) + "; return false;"
        });

        ucBreadcrumbs.AddBreadcrumb(new BreadcrumbItem());
    }


    /// <summary>
    /// Sets the property value of the control, setting the value affects only local property value.
    /// </summary>
    /// <param name="propertyName">Property name to set</param>
    /// <param name="value">New property value</param>
    public override bool SetValue(string propertyName, object value)
    {
        // Community group id
        if (CMSString.Compare(propertyName, "CommunityGroupID", true) == 0)
        {
            int groupId = ValidationHelper.GetInteger(value, 0);
            ucProjectList.CommunityGroupID = groupId;
            ucProjectNew.CommunityGroupID = groupId;
            ucProjectEdit.CommunityGroupID = groupId;
        }
        // Is livesite
        else if (CMSString.Compare(propertyName, "IsLiveSite", true) == 0)
        {
            bool isLiveSite = ValidationHelper.GetBoolean(value, base.IsLiveSite);
            ucProjectEdit.IsLiveSite = isLiveSite;
            ucProjectList.IsLiveSite = isLiveSite;
            ucProjectNew.IsLiveSite = isLiveSite;
        }

        // Call base method
        return base.SetValue(propertyName, value);
    }

    #endregion
}