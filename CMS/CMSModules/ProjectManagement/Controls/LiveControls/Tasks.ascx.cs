using System;
using System.Web.UI.WebControls;

using CMS.Helpers;
using CMS.ProjectManagement;
using CMS.Base;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.ExtendedControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.DataEngine;
using CMS.Membership;

public partial class CMSModules_ProjectManagement_Controls_LiveControls_Tasks : CMSAdminItemsControl
{
    #region "Variables"

    private string mProjectNames = String.Empty;
    private TasksDisplayTypeEnum mTasksDisplayType = TasksDisplayTypeEnum.TasksAssignedToMe;
    private bool mShowOverdueTasks = true;
    private bool mShowOnTimeTasks = true;
    private bool mShowPrivateTasks = true;
    private bool mShowFinishedTasks = true;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the header actions.
    /// </summary>
    public override HeaderActions HeaderActions
    {
        get
        {
            return actionsElem;
        }
    }


    /// <summary>
    /// Gets or sets the site name.
    /// </summary>
    public string SiteName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SiteName"), String.Empty);
        }
        set
        {
            SetValue("SiteName", value);
            EnsureChildControls();
            ucTaskList.SiteName = SiteName;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether paging should be used.
    /// </summary>
    public bool EnablePaging
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("EnablePaging"), false);
        }
        set
        {
            SetValue("EnablePaging", value);
        }
    }


    /// <summary>
    /// Gets or sets the number of items per page.
    /// </summary>
    public int PageSize
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("PageSize"), 10);
        }
        set
        {
            SetValue("PageSize", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether task actions should be enabled.
    /// </summary>
    public bool AllowTaskActions
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowTaskActions"), false);
        }
        set
        {
            SetValue("AllowTaskActions", value);
        }
    }


    /// <summary>
    /// Gets or sets the project names which should be used for task filtering
    /// Project names are split by semicolon
    /// </summary>
    public string ProjectNames
    {
        get
        {
            return mProjectNames;
        }
        set
        {
            mProjectNames = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether current control is displayed on live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return ucTaskList.IsLiveSite;
        }
        set
        {
            EnsureChildControls();
            ucTaskList.IsLiveSite = value;
            ucTaskEdit.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Gets or sets the list where condition.
    /// </summary>
    public string WhereCondition
    {
        get
        {
            return ucTaskList.Grid.WhereCondition;
        }
        set
        {
            EnsureChildControls();
            ucTaskList.Grid.WhereCondition = value;
        }
    }


    /// <summary>
    /// Gets or sets the current display type.
    /// </summary>
    public TasksDisplayTypeEnum TasksDisplayType
    {
        get
        {
            return mTasksDisplayType;
        }
        set
        {
            EnsureChildControls();
            mTasksDisplayType = value;
        }
    }


    /// <summary>
    /// Show on time tasks.
    /// </summary>
    public bool ShowOnTimeTasks
    {
        get
        {
            return mShowOnTimeTasks;
        }
        set
        {
            mShowOnTimeTasks = value;
        }
    }


    /// <summary>
    /// Show private tasks.
    /// </summary>
    public bool ShowPrivateTasks
    {
        get
        {
            return mShowPrivateTasks;
        }
        set
        {
            mShowPrivateTasks = value;
        }
    }


    /// <summary>
    /// Show finished tasks.
    /// </summary>
    public bool ShowFinishedTasks
    {
        get
        {
            return mShowFinishedTasks;
        }
        set
        {
            mShowFinishedTasks = value;
        }
    }


    /// <summary>
    /// Show overdue tasks.
    /// </summary>
    public bool ShowOverdueTasks
    {
        get
        {
            return mShowOverdueTasks;
        }
        set
        {
            mShowOverdueTasks = value;
        }
    }


    /// <summary>
    /// Display type of status.
    /// </summary>
    public StatusDisplayTypeEnum StatusDisplayType
    {
        get
        {
            return ucTaskList.StatusDisplayType;
        }
        set
        {
            EnsureChildControls();
            ucTaskList.StatusDisplayType = value;
        }
    }

    #endregion


    #region "Action methods"

    /// <summary>
    /// Edit task event handler.
    /// </summary>
    private void ucTaskList_OnAction(object sender, CommandEventArgs e)
    {
        // Switch by command name
        switch (e.CommandName.ToString())
        {
            // Edit
            case "edit":
                // Clear edit form
                ucTaskEdit.ClearForm();
                // Set task id from command argument
                int taskID = ValidationHelper.GetInteger(e.CommandArgument, 0);
                // Set task id 
                ucTaskEdit.ItemID = taskID;
                // Reload task edit form data
                ucTaskEdit.ReloadData(true);
                // Render dialog
                ucPopupDialog.Visible = true;
                // Show modal dialog
                ucPopupDialog.Show();
                // Reload dialog update panel
                pnlUpdate.Update();
                break;
        }
    }


    /// <summary>
    /// New task event handler.
    /// </summary>
    private void actionsElem_ActionPerformed(object sender, CommandEventArgs e)
    {
        // Clear selected task ID
        ucTaskEdit.ItemID = 0;
        // Clear form data
        ucTaskEdit.ClearForm();
        // Reload task edit data
        ucTaskEdit.ReloadData(true);
        // Set popup title 
        titleElem.TitleText = GetString("pm.projecttask.new");
        // Render dialog
        ucPopupDialog.Visible = true;
        // Shoe modal dialog
        ucPopupDialog.Show();
        // Reload dialog update panel
        pnlUpdate.Update();
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Keep current user object
        var currentUser = MembershipContext.AuthenticatedUser;
        // Title element settings
        titleElem.TitleText = GetString("pm.projecttask.edit");

        ControlsHelper.RegisterPostbackControl(btnOK);

        #region "Header actions"

        if (AuthenticationHelper.IsAuthenticated())
        {
            HeaderAction action = new HeaderAction();
            action.Text = GetString("pm.projecttask.newpersonal");
            action.CommandName = "new_task";
            actionsElem.AddAction(action);

            HeaderActions.ActionPerformed += actionsElem_ActionPerformed;
            HeaderActions.ReloadData();
        }

        #endregion


        // Switch by display type and set correct list grid name
        switch (TasksDisplayType)
        {
            // Project tasks
            case TasksDisplayTypeEnum.ProjectTasks:
                ucTaskList.OrderByType = ProjectTaskOrderByEnum.NotSpecified;
                ucTaskList.Grid.OrderBy = "TaskPriorityOrder ASC,ProjectTaskDeadline DESC";
                ucTaskList.Grid.GridName = "~/CMSModules/ProjectManagement/Controls/LiveControls/ProjectTasks.xml";
                pnlListActions.Visible = false;
                break;

            // Tasks owned by me
            case TasksDisplayTypeEnum.TasksOwnedByMe:
                ucTaskList.OrderByType = ProjectTaskOrderByEnum.NotSpecified;
                ucTaskList.Grid.OrderBy = "TaskPriorityOrder ASC,ProjectTaskDeadline DESC";
                ucTaskList.Grid.GridName = "~/CMSModules/ProjectManagement/Controls/LiveControls/TasksOwnedByMe.xml";
                break;

            // Tasks assigned to me
            case TasksDisplayTypeEnum.TasksAssignedToMe:
                // Set not specified order by default
                ucTaskList.OrderByType = ProjectTaskOrderByEnum.NotSpecified;
                // If sitename is not defined => display task from all sites => use user order
                if (String.IsNullOrEmpty(SiteName))
                {
                    ucTaskList.OrderByType = ProjectTaskOrderByEnum.UserOrder;
                }
                ucTaskList.Grid.OrderBy = "TaskPriorityOrder ASC,ProjectTaskDeadline DESC";
                ucTaskList.Grid.GridName = "~/CMSModules/ProjectManagement/Controls/LiveControls/TasksAssignedToMe.xml";
                break;
        }

        #region "Force edit by TaskID in querystring"

        // Check whether is not postback
        if (!RequestHelper.IsPostBack())
        {
            // Try get value from request storage which indicates whether force dialog is displayed
            bool isDisplayed = ValidationHelper.GetBoolean(RequestStockHelper.GetItem("cmspmforceitemdisplayed", true), false);

            // Try get task id from querystring
            int forceTaskId = QueryHelper.GetInteger("taskid", 0);
            if ((forceTaskId > 0) && (!isDisplayed))
            {
                ProjectTaskInfo pti = ProjectTaskInfoProvider.GetProjectTaskInfo(forceTaskId);

                // Check whether task is defined 
                if (pti != null)
                {
                    ProjectInfo pi = ProjectInfoProvider.GetProjectInfo(pti.ProjectTaskProjectID);

                    // and if is assigned to some project, this project is assigned to current site
                    if ((pi == null) || (pi.ProjectSiteID == SiteContext.CurrentSiteID))
                    {
                        bool taskIdValid = false;

                        // Switch by display type
                        switch (TasksDisplayType)
                        {
                                // Tasks created by me
                            case TasksDisplayTypeEnum.TasksOwnedByMe:
                                if (pti.ProjectTaskOwnerID == currentUser.UserID)
                                {
                                    taskIdValid = true;
                                }
                                break;

                                // Tasks assigned to me
                            case TasksDisplayTypeEnum.TasksAssignedToMe:
                                if (pti.ProjectTaskAssignedToUserID == currentUser.UserID)
                                {
                                    taskIdValid = true;
                                }
                                break;

                                // Project task
                            case TasksDisplayTypeEnum.ProjectTasks:
                                if (!String.IsNullOrEmpty(ProjectNames) && (pi != null))
                                {
                                    string projectNames = ";" + ProjectNames.ToLowerCSafe() + ";";
                                    if (projectNames.Contains(";" + pi.ProjectName.ToLowerCSafe() + ";"))
                                    {
                                        // Check whether user can see private task
                                        if (!pti.ProjectTaskIsPrivate
                                            || ((pti.ProjectTaskOwnerID == currentUser.UserID) || (pti.ProjectTaskAssignedToUserID == currentUser.UserID))
                                            || ((pi.ProjectGroupID > 0) && currentUser.IsGroupAdministrator(pi.ProjectGroupID))
                                            || ((pi.ProjectGroupID == 0) && (currentUser.IsAuthorizedPerResource("CMS.ProjectManagement", PERMISSION_MANAGE))))
                                        {
                                            taskIdValid = true;
                                        }
                                    }
                                }
                                break;
                        }

                        bool displayValid = true;

                        // Check whether do not display finished tasks is required
                        if (!ShowFinishedTasks)
                        {
                            ProjectTaskStatusInfo ptsi = ProjectTaskStatusInfoProvider.GetProjectTaskStatusInfo(pti.ProjectTaskStatusID);
                            if ((ptsi != null) && (ptsi.TaskStatusIsFinished))
                            {
                                displayValid = false;
                            }
                        }

                        // Check whether private task should be edited
                        if (!ShowPrivateTasks)
                        {
                            if (pti.ProjectTaskProjectID == 0)
                            {
                                displayValid = false;
                            }
                        }

                        // Check whether ontime task should be edited
                        if (!ShowOnTimeTasks)
                        {
                            if ((pti.ProjectTaskDeadline != DateTimeHelper.ZERO_TIME) && (pti.ProjectTaskDeadline < DateTime.Now))
                            {
                                displayValid = false;
                            }
                        }

                        // Check whether overdue task should be edited
                        if (!ShowOverdueTasks)
                        {
                            if ((pti.ProjectTaskDeadline != DateTimeHelper.ZERO_TIME) && (pti.ProjectTaskDeadline > DateTime.Now))
                            {
                                displayValid = false;
                            }
                        }

                        // Check whether user is allowed to see project
                        if ((pi != null) && (ProjectInfoProvider.IsAuthorizedPerProject(pi.ProjectID, ProjectManagementPermissionType.READ, MembershipContext.AuthenticatedUser)))
                        {
                            displayValid = false;
                        }

                        // If task is valid and user has permissions to see this task display edit task dialog
                        if (displayValid && taskIdValid && ProjectTaskInfoProvider.IsAuthorizedPerTask(forceTaskId, ProjectManagementPermissionType.READ, MembershipContext.AuthenticatedUser, SiteContext.CurrentSiteID))
                        {
                            ucTaskEdit.ItemID = forceTaskId;
                            ucTaskEdit.ReloadData();
                            // Render dialog
                            ucPopupDialog.Visible = true;
                            ucPopupDialog.Show();
                            // Set "force dialog displayed" flag
                            RequestStockHelper.Add("cmspmforceitemdisplayed", true, true);
                        }
                    }
                }
            }
        }

        #endregion


        #region "Event handlers registration"

        // Register list action handler
        ucTaskList.OnAction += ucTaskList_OnAction;

        #endregion


        #region "Pager settings"

        // Paging
        if (!EnablePaging)
        {
            ucTaskList.Grid.PageSize = "##ALL##";
            ucTaskList.Grid.Pager.DefaultPageSize = -1;
        }
        else
        {
            ucTaskList.Grid.Pager.DefaultPageSize = PageSize;
            ucTaskList.Grid.PageSize = PageSize.ToString();
            ucTaskList.Grid.FilterLimit = PageSize;
        }

        #endregion


        // Use postbacks on list actions
        ucTaskList.UsePostbackOnEdit = true;
        // Check delete permission
        ucTaskList.OnCheckPermissionsExtended += ucTaskList_OnCheckPermissionsExtended;
        // Don't register JS edit script
        ucTaskList.RegisterEditScript = false;

        // Hide default ok button on edit 
        ucTaskEdit.ShowOKButton = false;
        // Disable on site validators
        ucTaskEdit.DisableOnSiteValidators = true;
        // Check modify permission
        ucTaskEdit.OnCheckPermissionsExtended += ucTaskEdit_OnCheckPermissionsExtended;
        // Build condition event
        ucTaskList.BuildCondition += ucTaskList_BuildCondition;
    }


    /// <summary>
    /// Reloads data.
    /// </summary>
    public override void ReloadData(bool forceReload)
    {
        // Reload list control
        ucTaskList.ReloadData();
        pnlUpdateList.Update();

        // Call base method
        base.ReloadData(forceReload);
    }


    /// <summary>
    /// Button OK click event handler.
    /// </summary>
    protected void btnOK_onClick(object sender, EventArgs ea)
    {
        // Save data
        if (ucTaskEdit.Save())
        {
            // Do not render popup dialog
            ucPopupDialog.Visible = false;
            // Hide dialog
            ucPopupDialog.Hide();
            // Clear edit item id
            ucTaskEdit.ItemID = 0;
            // Reload data
            ucTaskList.ReloadData();

            // Update dialog panel
            pnlUpdateList.Update();
        }
        // If save was unsuccessful keep dialog displayed
        else
        {
            // If new task dialog is displayed set appropriate title
            if (ucTaskEdit.ItemID == 0)
            {
                titleElem.TitleText = GetString("pm.projecttask.edit");
            }
            // Render dialog
            ucPopupDialog.Visible = true;
            // Show dialog
            ucPopupDialog.Show();
            // Update dialog panel
            pnlUpdateList.Update();
        }
    }


    /// <summary>
    /// Build list where condition.
    /// </summary>
    private string ucTaskList_BuildCondition(object sender, string whereCondition)
    {
        // Keep current user
        var currentUser = MembershipContext.AuthenticatedUser;

        // Switch by display type
        switch (TasksDisplayType)
        {
            // Tasks owned by me
            case TasksDisplayTypeEnum.TasksOwnedByMe:
                whereCondition = SqlHelper.AddWhereCondition(whereCondition, "ProjectTaskOwnerID = " + currentUser.UserID);
                break;

            // Tasks assigned to me
            case TasksDisplayTypeEnum.TasksAssignedToMe:
                whereCondition = SqlHelper.AddWhereCondition(whereCondition, "ProjectTaskAssignedToUserID = " + currentUser.UserID);
                break;

            // Project tasks
            case TasksDisplayTypeEnum.ProjectTasks:
                // Check whether project names are defined
                if (!String.IsNullOrEmpty(ProjectNames))
                {
                    string condition = SqlHelper.GetSafeQueryString(ProjectNames, false);
                    condition = "N'" + condition.Replace(";", "',N'") + "'";
                    // Add condition for specified projects
                    condition = "ProjectTaskProjectID IN (SELECT ProjectID FROM PM_Project WHERE ProjectName IN (" + condition + "))";

                    // Add condition for private task, only if current user isn't project management admin
                    if (!currentUser.IsAuthorizedPerResource("CMS.ProjectManagement", PERMISSION_MANAGE))
                    {
                        condition = SqlHelper.AddWhereCondition(condition, "(ProjectTaskIsPrivate = 0 OR ProjectTaskIsPrivate IS NULL) OR (ProjectTaskOwnerID = " + currentUser.UserID + " OR ProjectTaskAssignedToUserID = " + currentUser.UserID + " OR ProjectOwner = " + currentUser.UserID + ")");
                    }

                    // Complete where condition
                    whereCondition = SqlHelper.AddWhereCondition(whereCondition, condition);
                }
                // If project names aren't defined do nothing
                else
                {
                    whereCondition = "(1=2)";
                }
                break;
        }

        // Do not display finished tasks
        if (!ShowFinishedTasks)
        {
            whereCondition = SqlHelper.AddWhereCondition(whereCondition, "TaskStatusIsFinished = 0");
        }

        // Do not display on time tasks
        if (!ShowOnTimeTasks)
        {
            whereCondition = SqlHelper.AddWhereCondition(whereCondition, "((ProjectTaskDeadline < @Now) OR (ProjectTaskDeadline IS NULL))");
        }

        // Do not display overdue tasks
        if (!ShowOverdueTasks)
        {
            whereCondition = SqlHelper.AddWhereCondition(whereCondition, "((ProjectTaskDeadline > @Now) OR (ProjectTaskDeadline IS NULL))");
        }

        // Do not display private tasks
        if (!ShowPrivateTasks)
        {
            whereCondition = SqlHelper.AddWhereCondition(whereCondition, "ProjectTaskIsPrivate = 0");
        }

        // Task assigned to me, Task owned by me webparts
        if ((!ShowOnTimeTasks) || (!ShowOverdueTasks))
        {
            var parameters = new QueryDataParameters();
            parameters.Add("@Now", DateTime.Now);

            ucTaskList.Grid.QueryParameters = parameters;
        }

        // Add security condition - display only tasks which are assigned or owned by the current user or which are a part of a project where the current user is authorised to Read from
        whereCondition = SqlHelper.AddWhereCondition(whereCondition, ProjectTaskInfoProvider.CombineSecurityWhereCondition(whereCondition, currentUser, SiteName));

        return whereCondition;
    }


    /// <summary>
    /// Creates child controls and ensures updatepanel load container.
    /// </summary>
    protected override void CreateChildControls()
    {
        if ((ucTaskEdit == null) || (ucTaskList == null))
        {
            pnlUpdate.LoadContainer();
            pnlUpdateList.LoadContainer();
        }

        base.CreateChildControls();
    }


    /// <summary>
    /// OnPreRender - Reloads data.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        ReloadData();


        #region "Check hide actions"

        // Check whether actions should be hidden
        if (!AllowTaskActions)
        {
            // Hide new task link
            pnlListActions.Visible = false;

            // Get current gridview 
            GridView gv = ucTaskList.Grid.GridView;
            // Check whether grid contains data and if so hide action column
            if (gv.Rows.Count > 0)
            {
                gv.Columns[0].Visible = false;
            }
        }

        #endregion


        base.OnPreRender(e);
    }

    #endregion


    #region "Security methods"

    /// <summary>
    /// Checks whether current user can modify task.
    /// </summary>
    /// <param name="permissionType">Permission type</param>
    /// <param name="modulePermissionType">Module permission type</param>
    /// <param name="sender">Sender object</param>
    private void ucTaskEdit_OnCheckPermissionsExtended(string permissionType, string modulePermissionType, CMSAdminControl sender)
    {
        // Get task info for currently deleted task
        ProjectTaskInfo pti = ProjectTaskInfoProvider.GetProjectTaskInfo(ucTaskEdit.ItemID);
        // Check permission only for existing tasks and tasks assigned to some project
        if ((pti != null) && (pti.ProjectTaskProjectID > 0))
        {
            // Keep current user
            var cui = MembershipContext.AuthenticatedUser;

            // Check access to project permission for modify action
            if ((CMSString.Compare(permissionType, ProjectManagementPermissionType.MODIFY, true) == 0) && ProjectInfoProvider.IsAuthorizedPerProject(pti.ProjectTaskProjectID, ProjectManagementPermissionType.READ, cui))
            {
                // If user is owner or assignee => allow tasks edit
                if ((pti.ProjectTaskOwnerID == cui.UserID) || (pti.ProjectTaskAssignedToUserID == cui.UserID))
                {
                    return;
                }
            }

            // Check whether user is allowed to modify task
            if (!ProjectInfoProvider.IsAuthorizedPerProject(pti.ProjectTaskProjectID, permissionType, cui))
            {
                // Set error message to the dialog
                ucTaskEdit.SetError(GetString("pm.project.permission"));
                // Stop edit control processing
                sender.StopProcessing = true;

                // Render dialog
                ucPopupDialog.Visible = true;
                // Show dialog
                ucPopupDialog.Show();
                // Update dialog panel
                pnlUpdateList.Update();
            }
        }
    }


    /// <summary>
    /// Checks whether current user can delete task.
    /// </summary>
    /// <param name="permissionType">Permission type</param>
    /// <param name="modulePermissionType">Module permission type</param>
    /// <param name="sender">Sender object</param>
    private void ucTaskList_OnCheckPermissionsExtended(string permissionType, string modulePermissionType, CMSAdminControl sender)
    {
        int itemID = ucTaskEdit.ItemID;
        
        // If edit task ItemID is 0, try get from list
        if (itemID == 0)
        {
            itemID = ucTaskList.SelectedItemID;
        }

        // Get task info for currently deleted task
        ProjectTaskInfo pti = ProjectTaskInfoProvider.GetProjectTaskInfo(itemID);

        // Check permission only for existing tasks and tasks assigned to some project
        if ((pti != null) && (pti.ProjectTaskProjectID > 0))
        {
            // Check whether user is allowed to modify or delete task
            if (!ProjectInfoProvider.IsAuthorizedPerProject(pti.ProjectTaskProjectID, permissionType, MembershipContext.AuthenticatedUser))
            {
                lblError.Visible = true;
                lblError.Text = GetString("pm.project.permission");
                sender.StopProcessing = true;
            }
        }
    }

    #endregion
}