using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.ExtendedControls;
using CMS.Globalization;
using CMS.Helpers;
using CMS.ProjectManagement;
using CMS.Base;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;
using CMS.DataEngine;

public partial class CMSModules_ProjectManagement_Controls_UI_ProjectTask_List : CMSAdminListControl, IPostBackEventHandler
{
    #region "Variables"

    private int mProjectID;
    private bool mUsePostbackOnEdit;
    private ProjectTaskOrderByEnum mOrderByType = ProjectTaskOrderByEnum.NotSpecified;
    private int mGroupID;
    private StatusDisplayTypeEnum mStatusDisplayType = StatusDisplayTypeEnum.Icon;
    private string mOrderBy = String.Empty;
    private bool showArrows;
    private string rowColor;
    private bool mRegisterEditScript = true;
    private const int MAX_TITLE_LENGTH = 50;

    /// <summary>
    /// Build condition event.
    /// </summary>
    public delegate string BuildConditionEvent(object sender, string whereCondition);

    /// <summary>
    /// Build condition event is fired when where condition for list control is built.
    /// </summary>
    public event BuildConditionEvent BuildCondition;

    #endregion


    #region "Properties"

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
    /// Gets or sets the edit page URL.
    /// </summary>
    public string EditPageURL
    {
        get
        {
            return ValidationHelper.GetString(GetValue("EditPageURL"), "Edit.aspx");
        }
        set
        {
            SetValue("EditPageURL", value);
        }
    }


    /// <summary>
    /// Gets or sets the additional querystring parameters in edit page URL.
    /// </summary>
    public string EditPageParameters
    {
        get
        {
            return ValidationHelper.GetString(GetValue("EditPageParameters"), String.Empty);
        }
        set
        {
            SetValue("EditPageParameters", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether community group should be used for filtering
    /// This option is used for My projects web part
    /// </summary>
    public bool IgnoreCommunityGroup
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("IgnoreCommunityGroup"), false);
        }
        set
        {
            SetValue("IgnoreCommunityGroup", value);
        }
    }


    /// <summary>
    /// If true control registers edit script.
    /// </summary>
    public bool RegisterEditScript
    {
        get
        {
            return mRegisterEditScript;
        }
        set
        {
            mRegisterEditScript = value;
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
        }
    }


    /// <summary>
    /// Inner grid.
    /// </summary>
    public UniGrid Grid
    {
        get
        {
            return gridElem;
        }
    }


    /// <summary>
    /// Status display type.
    /// </summary>
    public StatusDisplayTypeEnum StatusDisplayType
    {
        get
        {
            return mStatusDisplayType;
        }
        set
        {
            mStatusDisplayType = value;
        }
    }


    /// <summary>
    /// Group ID of task.
    /// </summary>
    public int CommunityGroupID
    {
        get
        {
            return mGroupID;
        }
        set
        {
            mGroupID = value;
        }
    }


    /// <summary>
    /// ID of object which is being reminded.
    /// </summary>
    public int ReminderTaskID
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["ReminderTaskID"], 0);
        }
        set
        {
            ViewState["ReminderTaskID"] = value;
        }
    }


    /// <summary>
    /// Indicates whether redirect or postback on edit.
    /// </summary>
    public bool UsePostbackOnEdit
    {
        get
        {
            return mUsePostbackOnEdit;
        }
        set
        {
            mUsePostbackOnEdit = value;
        }
    }


    /// <summary>
    /// Indicates if the control should perform the operations.
    /// </summary>
    public override bool StopProcessing
    {
        get
        {
            return base.StopProcessing;
        }
        set
        {
            base.StopProcessing = value;
            gridElem.StopProcessing = value;
        }
    }


    /// <summary>
    /// Indicates if the control is used on the live site.
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
            plcMess.IsLiveSite = value;
            gridElem.IsLiveSite = value;
        }
    }


    /// <summary>
    /// ProjectID.
    /// </summary>
    public int ProjectID
    {
        get
        {
            return mProjectID;
        }
        set
        {
            mProjectID = value;
        }
    }


    /// <summary>
    /// Gets or sets the order by.
    /// Possible options: "ProjectTaskProjectOrder", "ProjectTaskUserOrder"
    /// </summary>
    public ProjectTaskOrderByEnum OrderByType
    {
        get
        {
            return mOrderByType;
        }
        set
        {
            mOrderByType = value;
        }
    }


    /// <summary>
    /// Order by added to OrderByType.
    /// </summary>
    public string OrderBy
    {
        get
        {
            return mOrderBy;
        }
        set
        {
            mOrderBy = value;
        }
    }

    #endregion


    #region "Page methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        BuildConditions();

        reminderPopupDialog.Hide();

        // Grid initialization                
        gridElem.OnAction += gridElem_OnAction;
        gridElem.OnExternalDataBound += gridElem_OnExternalDataBound;
        gridElem.GridView.RowDataBound += GridView_RowDataBound;
        gridElem.GridView.RowCreated += GridView_RowCreated;
        gridElem.ZeroRowsText = GetString("pm.projectstask.notasksfound");
        gridElem.DelayedReload = true;

        QueryDataParameters parameters = new QueryDataParameters();
        parameters.Add("@Now", DateTime.Now);
        gridElem.QueryParameters = parameters;

        switch (OrderByType)
        {
            case ProjectTaskOrderByEnum.ProjectOrder:
                gridElem.OrderBy = "ProjectTaskProjectOrder";
                break;

            case ProjectTaskOrderByEnum.UserOrder:
                gridElem.OrderBy = "ProjectTaskUserOrder";
                break;
        }

        if (!String.IsNullOrEmpty(OrderBy))
        {
            gridElem.OrderBy += (!String.IsNullOrEmpty(gridElem.OrderBy)) ? ", " : "";
            gridElem.OrderBy += OrderBy;
        }
    }


    /// <summary>
    /// Reloads data
    /// </summary>
    public override void ReloadData()
    {
        BuildConditions();
        gridElem.ReloadData();
        base.ReloadData();
    }


    protected override void OnPreRender(EventArgs e)
    {
        ReloadData();

        base.OnPreRender(e);

        // Show or hide messages
        lblReminderError.Visible = !string.IsNullOrEmpty(lblReminderError.Text);

        if (RegisterEditScript)
        {
            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "EditRedirectTaskScript", ScriptHelper.GetScript(@" 
                function EditProjectTask(id) {
                    var usePostback = '" + UsePostbackOnEdit + @"';
                    var projectId = parseInt('" + ProjectID + @"');
                    var groupId = parseInt('" + CommunityGroupID + @"');
                    if (usePostback == 'False') {
                        window.location.replace('" + EditPageURL + @"?projectTaskId=' + id + '&projectid=' + projectId + '&groupid=' + groupId + '" + EditPageParameters + @"' );
                        return false;
                    }
                    return true;
                }    
                "));
        }
    }


    /// <summary>
    /// Raises build condition event.
    /// </summary>
    public string RaiseBuildCondition()
    {
        if (BuildCondition != null)
        {
            return BuildCondition(this, gridElem.WhereCondition);
        }

        return gridElem.WhereCondition;
    }

    #endregion


    #region "Grid events"

    /// <summary>
    /// Handles the RowCreated event of the GridView control.
    /// </summary>
    /// <param name="sender">The source of the event</param>
    /// <param name="e">The <see cref="System.Web.UI.WebControls.GridViewRowEventArgs"/> instance containing the event data</param>
    private void GridView_RowCreated(object sender, GridViewRowEventArgs e)
    {
        rowColor = null;
    }


    /// <summary>
    /// Handles the RowDataBound event of the GridView control.
    /// </summary>
    /// <param name="sender">The source of the event</param>
    /// <param name="e">The <see cref="System.Web.UI.WebControls.GridViewRowEventArgs"/> instance containing the event data</param>
    private void GridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (!String.IsNullOrEmpty(rowColor))
            {
                e.Row.Attributes.Add("style", "background-color: " + rowColor);
            }
        }
    }


    /// <summary>
    /// Handles UniGrid's OnExternalDataBound event.
    /// </summary>
    /// <param name="sender">Sender object (image button if it is an external databoud for action button)</param>
    /// <param name="sourceName">External source name of the column specified in the grid XML</param>
    /// <param name="parameter">Source parameter (original value of the field)</param>
    protected object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        DataRowView row = null;

        switch (sourceName.ToLowerCSafe())
        {
            case "up":
            case "down":
                CMSGridActionButton button = ((CMSGridActionButton)sender);
                button.Visible = showArrows;
                break;

            case "taskprogress":
                row = (DataRowView)parameter;
                int progress = ValidationHelper.GetInteger(row["ProjectTaskProgress"], 0);
                return ProjectTaskInfoProvider.GenerateProgressHtml(progress, true);

            case "projecttaskdisplayname":
            case "projectdisplayname":
                string displayname = parameter.ToString();
                if (displayname.Length > MAX_TITLE_LENGTH)
                {
                    return "<span title=\"" + HTMLHelper.HTMLEncode(displayname) + "\">" + HTMLHelper.HTMLEncode(TextHelper.LimitLength(displayname, MAX_TITLE_LENGTH)) + "</span>";
                }
                else
                {
                    return HTMLHelper.HTMLEncode(displayname);
                }

            case "projecttaskdeadline":
                DateTime dt = ValidationHelper.GetDateTime(parameter, DateTimeHelper.ZERO_TIME);
                if (dt == DateTimeHelper.ZERO_TIME)
                {
                    return String.Empty;
                }
                return TimeZoneMethods.ConvertDateTime(dt, this);

            case "taskstatus":
                row = parameter as DataRowView;
                if (row != null)
                {
                    string statusName = ValidationHelper.GetString(row["TaskStatusDisplayName"], String.Empty);
                    statusName = HTMLHelper.HTMLEncode(statusName);
                    string iconPath = HTMLHelper.HTMLEncode(ValidationHelper.GetString(row["TaskStatusIcon"], String.Empty));
                    // Get row color
                    rowColor = ValidationHelper.GetString(row["TaskStatusColor"], "");

                    switch (StatusDisplayType)
                    {
                        // Text
                        case StatusDisplayTypeEnum.Text:
                            return statusName;

                        // Icon
                        case StatusDisplayTypeEnum.Icon:
                            if (!String.IsNullOrEmpty(iconPath))
                            {
                                return "<div style=\"text-align:center;\"><img src=\"" + HTMLHelper.HTMLEncode(GetImageUrl(iconPath)) + "\" alt=\"" + statusName + "\" title=\"" + statusName + "\" class=\"StatusImage\" style=\"max-width:50px; max-height: 50px;\"  /></div>";
                            }
                            return statusName;

                        // Icon and text
                        case StatusDisplayTypeEnum.IconAndText:
                            if (!String.IsNullOrEmpty(iconPath))
                            {
                                return "<img src=\"" + GetImageUrl(iconPath) + "\" title=\"" + statusName + "\" class=\"StatusImage\" style=\"max-width:50px; max-height: 50px;\"  />&nbsp;" + statusName;
                            }
                            return statusName;
                    }
                }

                return row.ToString();

            case "assigneeformattedfullname":
                row = (DataRowView)parameter;
                string assigneeUserName = ValidationHelper.GetString(row["AssigneeUserName"], String.Empty);
                string assigneeFullName = ValidationHelper.GetString(row["AssigneeFullName"], String.Empty);

                return HTMLHelper.HTMLEncode(Functions.GetFormattedUserName(assigneeUserName, assigneeFullName, IsLiveSite));

            case "ownerformattedfullname":
                row = (DataRowView)parameter;
                string ownerUserName = ValidationHelper.GetString(row["OwnerUserName"], String.Empty);
                string ownerFullName = ValidationHelper.GetString(row["OwnerFullName"], String.Empty);

                return HTMLHelper.HTMLEncode(Functions.GetFormattedUserName(ownerUserName, ownerFullName, IsLiveSite));

            // Display name with link - used in webparts
            case "linkeddisplayname":
                row = (DataRowView)parameter;
                string displayName = ValidationHelper.GetString(row["ProjectTaskDisplayName"], String.Empty);
                int projectTaskID = ValidationHelper.GetInteger(row["ProjectTaskID"], 0);
                string url = SettingsKeyInfoProvider.GetValue(SiteContext.CurrentSiteName + ".CMSTaskDetailPage");

                return String.Format("<a href=\"{0}\" title=\"{1}\">{2}</a>", url + "?ProjectTaskID=" + projectTaskID, HTMLHelper.HTMLEncode(displayName), HTMLHelper.HTMLEncode(TextHelper.LimitLength(displayName, MAX_TITLE_LENGTH)));

            // Display name with link to ajax window
            case "editlinkdisplayname":
                row = (DataRowView)parameter;
                string editDisplayName = ValidationHelper.GetString(row["ProjectTaskDisplayName"], String.Empty);
                int editProjectTaskID = ValidationHelper.GetInteger(row["ProjectTaskID"], 0);
                return String.Format("<a href=\"javascript:" + ControlsHelper.GetPostBackEventReference(this, editProjectTaskID.ToString()) + "\" title=\"{0}\">{1}</a>", HTMLHelper.HTMLEncode(editDisplayName), HTMLHelper.HTMLEncode(TextHelper.LimitLength(editDisplayName, MAX_TITLE_LENGTH)));
        }

        return parameter;
    }


    /// <summary>
    /// Handles UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of the action which should be performed</param>
    /// <param name="actionArgument">ID of the item the action should be performed with</param>
    protected void gridElem_OnAction(string actionName, object actionArgument)
    {
        showArrows = GetSortingArrowsVisibility();

        int projectTaskId = ValidationHelper.GetInteger(actionArgument, 0);
        RaiseOnAction(actionName, actionArgument);
        if (projectTaskId > 0)
        {
            switch (actionName.ToLowerCSafe())
            {
                case "edit":
                    SelectedItemID = projectTaskId;
                    RaiseOnEdit();
                    break;

                case "delete":
                    // Delete the object
                    SelectedItemID = projectTaskId;
                    if (CheckPermissions("CMS.ProjectManagement", ProjectManagementPermissionType.DELETE, PERMISSION_MANAGE))
                    {
                        // Use try/catch due to license check
                        try
                        {
                            ProjectTaskInfoProvider.DeleteProjectTaskInfo(projectTaskId);
                            RaiseOnDelete();
                        }
                        catch (Exception ex)
                        {
                            ShowError(ex.Message);
                        }
                    }

                    // Reload data
                    gridElem.ReloadData();
                    break;

                case "up":
                    // Do not check permissions for user order
                    if ((OrderByType != ProjectTaskOrderByEnum.ProjectOrder) || (CheckPermissions("CMS.ProjectManagement", ProjectManagementPermissionType.MODIFY, PERMISSION_MANAGE)
                                                                                 && showArrows))
                    {
                        // Use try/catch due to license check
                        try
                        {
                            gridElem.SortDirect = string.Empty;
                            gridElem.ReloadData();
                            ProjectTaskInfoProvider.MoveTaskUp(projectTaskId, OrderByType);
                        }
                        catch (Exception ex)
                        {
                            ShowError(ex.Message);
                        }
                    }
                    break;

                case "down":
                    // Do not check permissions for user order
                    if ((OrderByType != ProjectTaskOrderByEnum.ProjectOrder) || (CheckPermissions("CMS.ProjectManagement", ProjectManagementPermissionType.MODIFY, PERMISSION_MANAGE)
                                                                                 && showArrows))
                    {
                        // Use try/catch due to license check
                        try
                        {
                            gridElem.SortDirect = string.Empty;
                            gridElem.ReloadData();
                            ProjectTaskInfoProvider.MoveTaskDown(projectTaskId, OrderByType);
                        }
                        catch (Exception ex)
                        {
                            ShowError(ex.Message);
                        }
                    }
                    break;

                case "reminder":
                    ReminderTaskID = projectTaskId;
                    txtReminderText.Text = string.Empty;
                    ShowReminderPopup();
                    break;
            }
        }
    }


    /// <summary>
    /// Reminder button OK clicked.
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="e">Event args</param>
    protected void btnReminderOK_onClick(object sender, EventArgs e)
    {
        string errorMessage = "";
        if (txtReminderText.Text.Length > 0)
        {
            ProjectTaskInfo taskInfo = ProjectTaskInfoProvider.GetProjectTaskInfo(ReminderTaskID);
            if (taskInfo != null)
            {
                if (taskInfo.ProjectTaskAssignedToUserID > 0)
                {
                    ProjectTaskInfoProvider.SendNotificationEmail(ProjectTaskEmailNotificationTypeEnum.TaskReminder, taskInfo, SiteContext.CurrentSiteName, txtReminderText.Text);
                    ShowConfirmation(GetString("pm.projecttask.remindersent"));
                }
                else
                {
                    errorMessage = GetString("pm.projecttask.remindernoassignee");
                }
            }
        }
        else
        {
            errorMessage = GetString("pm.projecttask.remindermessageerror");
        }

        if (String.IsNullOrEmpty(errorMessage))
        {
            CloseAjaxWindow();
        }
        else
        {
            ShowReminderPopup();
            lblReminderError.Text = errorMessage;
        }
    }

    #endregion


    #region "IPostBackEventHandler Members"

    public void RaisePostBackEvent(string eventArgument)
    {
        int id = ValidationHelper.GetInteger(eventArgument, 0);
        RaiseOnAction("edit", id);
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Close ajax window.
    /// </summary>
    private void CloseAjaxWindow()
    {
        reminderPopupDialog.Hide();
    }


    /// <summary>
    /// Show the reminder popup.
    /// </summary>
    private void ShowReminderPopup()
    {
        reminderPopupDialog.Show();
        titleElem.TitleText = GetString("pm.projecttask.reminder");
        titleElem.ShowCloseButton = false;
        titleElem.ShowFullScreenButton = false;
    }


    /// <summary>
    /// Build where condition for query.
    /// </summary>
    private void BuildConditions()
    {
        showArrows = GetSortingArrowsVisibility();

        // Check whether groups should be checked
        if (!IgnoreCommunityGroup)
        {
            // Display only tasks related to specific group or do not display any group task
            if (CommunityGroupID > 0)
            {
                gridElem.WhereCondition = SqlHelper.AddWhereCondition(gridElem.WhereCondition, "ProjectGroupID = " + CommunityGroupID);
            }
            else
            {
                gridElem.WhereCondition = SqlHelper.AddWhereCondition(gridElem.WhereCondition, "ProjectGroupID IS NULL");
            }
        }


        // Show tasks of the current site for project list
        if (ProjectID > 0)
        {
            gridElem.WhereCondition = SqlHelper.AddWhereCondition(gridElem.WhereCondition, "(ProjectSiteID IS NULL) OR (ProjectSiteID = " + SiteContext.CurrentSiteID) + ")";
        }
        // Show tasks with dependence on sitename setting for task list
        else
        {
            // Check whether sitename is required
            if (!String.IsNullOrEmpty(SiteName))
            {
                SiteInfo si = SiteInfoProvider.GetSiteInfo(SiteName);
                // Display tasks only for specific site
                if (si != null)
                {
                    gridElem.WhereCondition = SqlHelper.AddWhereCondition(gridElem.WhereCondition, "(ProjectSiteID IS NULL) OR  (ProjectSiteID = " + si.SiteID + ")");
                }
                // If sitename wasn't found do not display any records
                else
                {
                    gridElem.WhereCondition = SqlHelper.AddWhereCondition(gridElem.WhereCondition, "1=2");
                }
            }
        }

        // Check if the user has "Read" permissions to the module, if yes then show all tasks, if no then show only task which are NOT private
        var cui = MembershipContext.AuthenticatedUser;
        if (ProjectID > 0)
        {
            // Show only tasks related to project
            gridElem.WhereCondition = SqlHelper.AddWhereCondition(gridElem.WhereCondition, "ProjectTaskProjectID =" + ProjectID);
        }

        // Raise build condition
        gridElem.WhereCondition = RaiseBuildCondition();
    }


    /// <summary>
    /// Gets the sorting arrows visibility according to the settings of the project or other circumstances.
    /// </summary>
    private bool GetSortingArrowsVisibility()
    {
        bool arrowsVisible = false;

        // Hide/show sorting arrows  with dependence on project ordering
        if (ProjectID > 0)
        {
            if (ProjectInfoProvider.IsAuthorizedPerProject(ProjectID, ProjectManagementPermissionType.MODIFY, MembershipContext.AuthenticatedUser)
                || MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.ProjectManagement", PERMISSION_MANAGE))
            {
                arrowsVisible = true;
                if (OrderByType == ProjectTaskOrderByEnum.ProjectOrder)
                {
                    ProjectInfo projectObj = ProjectInfoProvider.GetProjectInfo(ProjectID);
                    if (projectObj != null)
                    {
                        arrowsVisible = projectObj.ProjectAllowOrdering;
                    }
                }
            }
        }
        // Tasks list only
        else
        {
            // Show sorting only for all site tasks
            if (String.IsNullOrEmpty(SiteName))
            {
                arrowsVisible = true;
            }
        }

        return arrowsVisible;
    }

    #endregion
}