using System;
using System.Web.UI.WebControls;
using System.Data;

using CMS.DataEngine;
using CMS.Globalization;
using CMS.Helpers;
using CMS.Membership;
using CMS.ProjectManagement;
using CMS.Base;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.Controls.Configuration;
using CMS.ExtendedControls.ActionsConfig;

public partial class CMSModules_ProjectManagement_Controls_LiveControls_ProjectListEdit : CMSAdminControl
{
    #region "Variables"

    private bool mHideAfterSave = true;

    #endregion


    #region "Properties"

    /// <summary>
    /// ID of group to which the project belongs
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
            ucProjectEdit.CommunityGroupID = value;
        }
    }


    /// <summary>
    /// Gets or sets the permission for creating new project.
    /// </summary>
    public string ProjectAccess
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ProjectAccess"), "nobody");
        }
        set
        {
            SetValue("ProjectAccess", value);
        }
    }


    /// <summary>
    /// Gets or sets the role names separated by semicolon which are authorized to create new project.
    /// </summary>
    public string AuthorizedRoles
    {
        get
        {
            return ValidationHelper.GetString(GetValue("AuthorizedRoles"), string.Empty);
        }
        set
        {
            SetValue("AuthorizedRoles", value);
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
            EnsureChildControls();
            ucProjectEdit.DisplayMode = value;
            ucTaskEdit.DisplayMode = value;
            ucTaskList.DisplayMode = value;
            ucSecurity.DisplayMode = value;
        }
    }


    /// <summary>
    /// Gets or sets the current project id.
    /// </summary>
    public int ProjectID
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("ProjectID"), 0);
        }
        set
        {
            SetValue("ProjectID", value);
            EnsureChildControls();
            ucTaskList.ProjectID = value;
            ucTaskEdit.ProjectID = value;
            ucSecurity.ProjectID = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether control is displayed on live site.
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
            EnsureChildControls();
            ucTaskList.IsLiveSite = value;
            ucTaskEdit.IsLiveSite = value;
            ucSecurity.IsLiveSite = value;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Initializes the control
    /// </summary>
    protected void Page_Init(object sender, EventArgs e)
    {
        // Set order column
        ucTaskList.OrderByType = ProjectTaskOrderByEnum.ProjectOrder;

        btnCancel.Click += btnCancel_Click;
        tabControlElem.UsePostback = true;
        tabControlElem.OnTabClicked += new EventHandler(tabControlElem_OnTabClicked);

        // Listen on security save
        ucSecurity.OnSaved += ucSecurity_OnSaved;

        // Popup dialogs title images and texts
        titleElem.TitleText = GetString("pm.project.edit");
        titleTaskElem.TitleText = GetString("pm.projecttask.edit");

        // Task list settings
        ucTaskList.UsePostbackOnEdit = true;
        ucTaskList.OnAction += ucTaskList_OnAction;
        ucTaskList.OnCheckPermissionsExtended += ucTaskList_OnCheckPermissionsExtended;

        // Task new/edit settings
        ucTaskEdit.DisableOnSiteValidators = true;
        ucTaskEdit.OnSaved += ucTaskEdit_OnSaved;
        ucTaskEdit.OnCheckPermissionsExtended += ucTaskEdit_OnCheckPermissionsExtended;

        // Project edit settings
        ucProjectEdit.DisableOnSiteValidators = true;
        ucProjectEdit.OnSaved += ucProjectEdit_OnSaved;
        ucProjectEdit.OnCheckPermissions += ucProjectEdit_OnCheckPermissions;
        ucProjectEdit.OnCheckPermissionsExtended += ucProjectEdit_OnCheckPermissionsExtended;

        // Handle page changed event
        ucSecurity.OnCheckPermissions += ucSecurity_OnCheckPermissions;

        // Build condition handler
        ucTaskList.BuildCondition += ucTaskList_BuildCondition;
    }


    /// <summary>
    /// OnLoad - base initialization.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        // Set current project ID to the sub-controls
        ucTaskList.ProjectID = ProjectID;
        ucTaskEdit.ProjectID = ProjectID;
        ucSecurity.ProjectID = ProjectID;



        #region "New project task link"

        // Check whether user is allowed to create new tasks
        if (IsAuthorizedPerCreateTask())
        {
            HeaderAction action = new HeaderAction();
            action.Text = GetString("pm.projecttask.new");
            action.CommandName = "new_task";
            actionsElem.AddAction(action);
            actionsElem.ActionPerformed += actionsElem_ActionPerformed;
        }
        // If user is not authorized to create new task, hide new task link
        else
        {
            pnlListActions.Visible = false;
        }

        #endregion


        #region "Tab settings"

        tabControlElem.AddTab(new TabItem
        {
            Text = GetString("general.general"),
        });
        tabControlElem.AddTab(new TabItem
        {
            Text = GetString("general.security"),
        });

        #endregion

        // Show edit task if it is required
        EnsureForceTask();

        // Check whether edit project button should be displayed
        if (!IsAuthorizedPerProjectEdit())
        {
            // Hide button if user can't edit the project
            pnlUpdateProjectEdit.Visible = false;
        }

        // Set community id
        ProjectInfo pi = ProjectInfoProvider.GetProjectInfo(ProjectID);
        if (pi != null)
        {
            ucTaskList.CommunityGroupID = pi.ProjectGroupID;
        }
    }


    /// <summary>
    /// Hide dialog when Cancel is clicked
    /// </summary>
    /// <param name="sender">Ignored</param>
    /// <param name="e">Ignored</param>
    private void btnCancel_Click(object sender, EventArgs e)
    {
        ucPopupDialogProject.Hide();
        pnlUpdateModalProject.Update();
    }


    /// <summary>
    /// Makes sure the project edit dialog is shown after security settings is saved and the update panel refreshed.
    /// </summary>
    void ucSecurity_OnSaved(object sender, EventArgs e)
    {
        // Do not hide modal dialog
        mHideAfterSave = false;
        // Set current project ID
        ucProjectEdit.ProjectID = ProjectID;
        // Show popup dialog for possibility of error on project edit form
        ucPopupDialogProject.Show();
    }


    /// <summary>
    /// Ensures displaying of task detail with dependence on querystring settings.
    /// </summary>
    protected void EnsureForceTask()
    {
        // Check whether isn't postback
        if (!RequestHelper.IsPostBack())
        {
            // Try get task id from query string
            int taskId = QueryHelper.GetInteger("taskid", 0);
            if (taskId > 0)
            {
                // Try get task info and check whether is assigned to current project
                ProjectTaskInfo pti = ProjectTaskInfoProvider.GetProjectTaskInfo(taskId);
                if ((pti != null) && (pti.ProjectTaskProjectID == ProjectID))
                {
                    // Check whether current user can see required private task
                    if (pti.ProjectTaskIsPrivate)
                    {
                        // Keep current user
                        var cui = MembershipContext.AuthenticatedUser;
                        if (!IsAuthorizedPerProjectEdit() && (pti.ProjectTaskOwnerID != cui.UserID) && (pti.ProjectTaskAssignedToUserID != cui.UserID))
                        {
                            return;
                        }
                    }

                    EditTask(taskId);
                }
            }
        }
    }


    /// <summary>
    /// Reloads project info area for current project.
    /// </summary>
    private void ReloadInfoArea()
    {
        // Reload info only if project is defined
        if (ProjectID > 0)
        {
            // Select data for current project
            DataSet ds = ProjectInfoProvider.GetProjectJoinedViewData(ProjectID);

            // Check whether data exists
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                // Set datasource
                repElem.DataSource = ds;
                // Bind repater control
                repElem.DataBind();
            }
        }
    }


    /// <summary>
    /// OnPreRender - reloads info area.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        // Reload info area
        ReloadInfoArea();
        // Update info panel
        pnlUpdateProjectInfo.Update();
        // Update Security panel
        pnlUpdateProjectSecurity.Update();

        base.OnPreRender(e);
    }


    /// <summary>
    /// Creates child controls and ensures update panel load container.
    /// </summary>
    protected override void CreateChildControls()
    {
        // If project form is not defined, load update panel container
        if (ucTaskEdit == null)
        {
            pnlUpdate.LoadContainer();
            pnlUpdateModalProject.LoadContainer();
            pnlUpdateProjectEdit.LoadContainer();
            pnlUpdateModalTask.LoadContainer();
            pnlUpdateProjectInfo.LoadContainer();
            pnlUpdateProjectSecurity.LoadContainer();
        }

        base.CreateChildControls();
    }


    /// <summary>
    /// Sets the edit popup dialog for specified task.
    /// </summary>
    /// <param name="taskId">Task ID</param>
    protected void EditTask(int taskId)
    {
        // Set task id to the task edit control
        ucTaskEdit.ItemID = taskId;
        // Set current project ID
        ucTaskEdit.ProjectID = ProjectID;
        // Reload task form data
        ucTaskEdit.ReloadData(true);
        // Display dialog with HTML editor
        ucPopupDialogTask.Visible = true;
        // Show modal dialog
        ucPopupDialogTask.Show();
        // Update dialog update panel
        pnlUpdateModalTask.Update();
    }


    /// <summary>
    /// Add timezone settings to given time. Return empty string if no time set.
    /// </summary>
    /// <param name="time">Time to convert</param>
    protected string GetConvertedTime(object time)
    {
        DateTime dt = ValidationHelper.GetDateTime(time, DateTimeHelper.ZERO_TIME);
        if (dt == DateTimeHelper.ZERO_TIME)
        {
            return String.Empty;
        }
        return Convert.ToString(TimeZoneMethods.ConvertDateTime(dt, this));
    }


    /// <summary>
    /// Returns formatted name.
    /// </summary>
    /// <param name="fullName">Full name</param>
    /// <param name="userName">User name</param>
    protected string GetFormattedName(string fullName, string userName)
    {
        return HTMLHelper.HTMLEncode(Functions.GetFormattedUserName(userName, fullName, IsLiveSite));
    }

    #endregion


    #region "Handler methods"

    /// <summary>
    /// Tab control clicked handler.
    /// </summary>
    private void tabControlElem_OnTabClicked(object sender, EventArgs e)
    {
        // Switch by selected tab
        switch (tabControlElem.SelectedTab)
        {
            // General
            case 0:
                // Show general form
                ucProjectEdit.Visible = true;
                // Hide security form
                pnlSecurity.Visible = false;
                break;
            // Security
            case 1:
                // Hide  general form
                ucProjectEdit.Visible = false;
                // Show security form
                pnlSecurity.Visible = true;
                break;
        }

        // Set current project ID 
        ucSecurity.ProjectID = ProjectID;
        ucProjectEdit.ProjectID = ProjectID;

        // Reload project form data
        ucProjectEdit.ReloadData();
        // Show modal dialog
        ucPopupDialogProject.Show();
    }


    /// <summary>
    /// Task edit event handler.
    /// </summary>
    private void ucTaskList_OnAction(object sender, CommandEventArgs e)
    {
        // Switch by command name
        switch (e.CommandName.ToString())
        {
            // Edit action
            case "edit":
                // Get task id from command argument
                int taskID = ValidationHelper.GetInteger(e.CommandArgument, 0);
                EditTask(taskID);
                break;
        }
    }


    /// <summary>
    /// New task event handler.
    /// </summary>
    private void actionsElem_ActionPerformed(object sender, CommandEventArgs e)
    {
        // Set new task title
        titleTaskElem.TitleText = GetString("pm.projecttask.new");

        // Clear task form
        ucTaskEdit.ClearForm();
        // Set current project ID
        ucTaskEdit.ProjectID = ProjectID;
        // Clear task ID, o = new
        ucTaskEdit.ItemID = 0;
        // Reload task data
        ucTaskEdit.ReloadData(true);

        // Display control with HTML editor
        ucPopupDialogTask.Visible = true;
        // Show popup dialog
        ucPopupDialogTask.Show();
        // Update popup dialog update panel
        pnlUpdateModalTask.Update();
    }


    /// <summary>
    /// Edit project click event handler.
    /// </summary>
    protected void btnEdit_Click(object sender, EventArgs e)
    {
        // Clear the settings
        ucSecurity.Clear();

        // Clear tab selection
        tabControlElem.SelectedTab = 0;

        // Display edit form
        ucProjectEdit.Visible = true;
        // Set current project ID
        ucProjectEdit.ProjectID = ProjectID;
        // Load project data
        ucProjectEdit.ReloadData();

        // Hide security form
        pnlSecurity.Visible = false;
        // Set current project ID
        ucSecurity.ProjectID = ProjectID;
        // Load security data
        ucSecurity.ReloadData();

        // Show popup dialog
        ucPopupDialogProject.Show();
        // Update popup update panel
        pnlUpdateModalProject.Update();
    }


    /// <summary>
    /// Edit project click handler.
    /// </summary>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        // If project info is selected  => save
        if (tabControlElem.SelectedTab == 0)
        {
            // Set current project ID
            ucProjectEdit.ProjectID = ProjectID;
            // Show popup dialog for possibility of error on project edit form
            ucPopupDialogProject.Show();
            // Call save method on project edit form
            ucProjectEdit.Save();
        }
        // If security is selected => hide
        else
        {
            ucPopupDialogProject.Hide();
        }
    }


    /// <summary>
    /// Apply button click.
    /// </summary>
    protected void btnApply_Click(object sender, EventArgs e)
    {
        // Do not hide modal dialog
        mHideAfterSave = false;
        // Set current project ID
        ucProjectEdit.ProjectID = ProjectID;
        // Show popup dialog for possibility of error on project edit form
        ucPopupDialogProject.Show();
        // Call save method on project edit form
        ucProjectEdit.Save();
    }


    /// <summary>
    /// Project edit - save event.
    /// </summary>
    private void ucProjectEdit_OnSaved(object sender, EventArgs e)
    {
        // Check whether hiding is required
        if (mHideAfterSave)
        {
            // Hide popup dialog after successful save
            ucPopupDialogProject.Hide();
        }

        // Update task list (hide/show sorting arrows)
        pnlUpdate.Update();
        pnlUpdateModalProject.Update();
    }


    /// <summary>
    /// Task edit ok button clicked event.
    /// </summary>
    protected void btnOkTask_Click(object sender, EventArgs e)
    {
        // Re-set modal title if current task edit is used for creating new task
        if (ucTaskEdit.ItemID == 0)
        {
            titleTaskElem.TitleText = GetString("pm.projecttask.new");
        }

        // Set current project ID
        ucTaskEdit.ProjectID = ProjectID;
        // Display dialog with HTML editor
        ucPopupDialogTask.Visible = true;
        // Show popup dialog for possibility of error on task edit form
        ucPopupDialogTask.Show();
        // Call save method on task edit form
        ucTaskEdit.Save();
        // Update modal dialog update panel
        pnlUpdateModalTask.Update();
    }


    /// <summary>
    /// Task edit save event.
    /// </summary>
    private void ucTaskEdit_OnSaved(object sender, EventArgs e)
    {
        // Hide popup after successful save
        ucPopupDialogTask.Hide();
        // Reload list data
        ucTaskList.ReloadData();
        // Refresh list update panel
        pnlUpdate.Update();
        // Hide dialog with HTML editor
        ucPopupDialogTask.Visible = false;
    }

    #endregion


    #region "Security methods"

    /// <summary>
    /// Generates security condition.
    /// </summary>
    private string ucTaskList_BuildCondition(object sender, string whereCondition)
    {
        if (!IsAuthorizedPerProjectEdit())
        {
            var cui = MembershipContext.AuthenticatedUser;
            whereCondition = SqlHelper.AddWhereCondition(whereCondition, SqlHelper.AddWhereCondition(whereCondition, "(ProjectTaskIsPrivate = 0) OR (ProjectTaskAssignedToUserID =" + cui.UserID + " OR ProjectTaskOwnerID=" + cui.UserID + ")"));
        }

        return whereCondition;
    }


    /// <summary>
    /// Checks whether user is authorized per project access.
    /// </summary>
    public bool IsAuthorizedPerProjectAccess()
    {
        // Keep current user
        var cui = MembershipContext.AuthenticatedUser;

        // Switch by create project option
        switch (ProjectAccess.ToLowerCSafe())
        {
            // All users
            case "all":
                return true;

            // Authenticated users
            case "authenticated":
                if (!cui.IsPublic())
                {
                    return true;
                }
                break;

            // Group members
            case "groupmember":
                if (CommunityGroupID > 0)
                {
                    return cui.IsGroupMember(CommunityGroupID);
                }
                break;

            // Authorized roles
            case "authorized":
                // Check whether roles are defined
                if (!String.IsNullOrEmpty(AuthorizedRoles))
                {
                    // Check whether user is valid group member if current project is assigned to some group
                    if (CommunityGroupID > 0)
                    {
                        if (!cui.IsGroupMember(CommunityGroupID))
                        {
                            return false;
                        }
                    }

                    // Keep site name
                    string siteName = SiteContext.CurrentSiteName;
                    // Split roles by semicolon
                    string[] roles = AuthorizedRoles.Split(';');

                    // Loop through all roles and check if user is assigned at least to one role
                    foreach (string role in roles)
                    {
                        // If user is in role, break current cycle and return true
                        if (cui.IsInRole(role, siteName))
                        {
                            return true;
                        }
                    }
                }
                break;

            // Nobody
            case "nobody":
                return ProjectInfoProvider.IsAuthorizedPerProject(ProjectID, ProjectManagementPermissionType.MODIFY, cui);
            default:
                return false;
        }

        return false;
    }


    /// <summary>
    /// Checks whether current user can edit the project.
    /// </summary>
    protected bool IsAuthorizedPerProjectEdit()
    {
        // Keep current user info object
        var cui = MembershipContext.AuthenticatedUser;

        // Global admin is allowed for all actions
        if (cui.IsGlobalAdministrator)
        {
            return true;
        }

        // Get project info object
        ProjectInfo pi = ProjectInfoProvider.GetProjectInfo(ProjectID);

        bool result = false;

        // Check whether project info is available, if not, user can't edit project
        if (pi != null)
        {
            // Project owner can edit the project
            if (pi.ProjectOwner == cui.UserID)
            {
                result = true;
            }
            // Community admin and group admin can edit project on group pages
            else if (pi.ProjectGroupID > 0)
            {
                result = cui.IsGroupAdministrator(pi.ProjectGroupID);
            }
            // Project management admin can edit projects on regular pages
            else
            {
                result = cui.IsAuthorizedPerResource("CMS.ProjectManagement", PERMISSION_MANAGE);
            }
        }

        // Check project access
        if (!result)
        {
            return IsAuthorizedPerProjectAccess();
        }

        return result;
    }


    /// <summary>
    /// Checks whether user can create new task.
    /// </summary>
    protected bool IsAuthorizedPerCreateTask()
    {
        // Keep current user info object
        var cui = MembershipContext.AuthenticatedUser;

        // If user can edit project => can create task
        if (IsAuthorizedPerProjectEdit())
        {
            return true;
        }

        // Check create permission
        return ProjectInfoProvider.IsAuthorizedPerProject(ProjectID, ProjectManagementPermissionType.CREATE, cui);
    }


    /// <summary>
    /// Checks modify permission on project edit.
    /// </summary>
    /// <param name="permissionType">Permission type</param>
    /// <param name="modulePermissionType">Module permission type</param>
    /// <param name="sender">Sender object</param>
    void ucProjectEdit_OnCheckPermissionsExtended(string permissionType, string modulePermissionType, CMSAdminControl sender)
    {
        if (ucProjectEdit.ItemID > 0)
        {
            if (ProjectInfoProvider.IsAuthorizedPerProject(ucProjectEdit.ItemID, permissionType, MembershipContext.AuthenticatedUser))
            {
                // Set current project ID
                ucProjectEdit.ProjectID = ProjectID;
                // Display dialog with HTML editor
                ucPopupDialogProject.Visible = true;
                // Show popup dialog for possibility of error on task edit form
                ucPopupDialogProject.Show();
                // Update modal dialog update panel
                pnlUpdateModalProject.Update();
            }
        }
    }


    /// <summary>
    /// Checks modify permission on task edit.
    /// </summary>
    /// <param name="permissionType">Permission type</param>
    /// <param name="modulePermissionType">Module permission type</param>
    /// <param name="sender">Sender object</param>
    private void ucTaskEdit_OnCheckPermissionsExtended(string permissionType, string modulePermissionType, CMSAdminControl sender)
    {
        // Indicates whether user is owner or assignee
        bool isInvolved = false;
        // Check whether task is in edit mode
        if (ucTaskEdit.ItemID > 0)
        {
            // Get task info
            ProjectTaskInfo pti = ProjectTaskInfoProvider.GetProjectTaskInfo(ucTaskEdit.ItemID);
            // Check whether task exists
            if (pti != null)
            {
                // Keep current user
                var cui = MembershipContext.AuthenticatedUser;
                // If user is assignee or owner set flag
                if ((pti.ProjectTaskAssignedToUserID == cui.UserID) || (pti.ProjectTaskOwnerID == cui.UserID))
                {
                    isInvolved = true;
                }
            }
        }


        // Check whether user is allowed to modify task
        if (!isInvolved && !ProjectInfoProvider.IsAuthorizedPerProject(ProjectID, permissionType, MembershipContext.AuthenticatedUser) && !IsAuthorizedPerProjectAccess())
        {
            // Set error message to the dialog
            ucTaskEdit.SetError(GetString("pm.project.permission"));
            // Stop edit control processing
            sender.StopProcessing = true;
            // Display dialog with HTML editor
            ucPopupDialogTask.Visible = true;
            // Set current project ID
            ucTaskEdit.ProjectID = ProjectID;
            // Show popup dialog for possibility of error on task edit form
            ucPopupDialogTask.Show();
            // Update modal dialog update panel
            pnlUpdateModalTask.Update();
        }
    }


    /// <summary>
    /// Checks delete and modify permissions (delete and up/down actions)
    /// </summary>
    /// <param name="permissionType">Permission type</param>
    /// <param name="modulePermissionType">Module permission type</param>
    /// <param name="sender">Sender object</param>
    private void ucTaskList_OnCheckPermissionsExtended(string permissionType, string modulePermissionType, CMSAdminControl sender)
    {
        // Check whether user is allowed to modify or delete task
        if (!ProjectInfoProvider.IsAuthorizedPerProject(ProjectID, permissionType, MembershipContext.AuthenticatedUser) && !IsAuthorizedPerProjectAccess())
        {
            lblError.Visible = true;
            lblError.Text = GetString("pm.project.permission");
            sender.StopProcessing = true;
        }
    }


    /// <summary>
    /// Check edit project permission for security change.
    /// </summary>
    private void ucSecurity_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        if (!IsAuthorizedPerProjectEdit())
        {
            // Set error message to the dialog
            ucTaskEdit.SetError(GetString("pm.project.permission"));
            // Stop edit control processing
            sender.StopProcessing = true;

            // Set current project ID
            ucTaskEdit.ProjectID = ProjectID;
            // Display dialog with HTML editor
            ucPopupDialogTask.Visible = true;
            // Show popup dialog for possibility of error on task edit form
            ucPopupDialogTask.Show();
            // Update modal dialog update panel
            pnlUpdateModalTask.Update();
        }
    }


    /// <summary>
    /// Check edit project permission for project change.
    /// </summary>
    private void ucProjectEdit_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        if (!IsAuthorizedPerProjectEdit())
        {
            // Set error message to the dialog
            ucTaskEdit.SetError(GetString("pm.project.permission"));
            // Stop edit control processing
            sender.StopProcessing = true;

            // Set current project ID
            ucTaskEdit.ProjectID = ProjectID;
            // Display dialog with HTML editor
            ucPopupDialogTask.Visible = true;
            // Show popup dialog for possibility of error on task edit form
            ucPopupDialogTask.Show();
            // Update modal dialog update panel
            pnlUpdateModalTask.Update();
        }
    }

    #endregion
}