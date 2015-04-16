using System;
using System.Threading;
using System.Web.UI.WebControls;

using CMS.Helpers;
using CMS.ProjectManagement;
using CMS.Base;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.DocumentEngine;
using CMS.UIControls;
using CMS.ExtendedControls;

using TreeNode = CMS.DocumentEngine.TreeNode;
using CMS.DataEngine;

public partial class CMSModules_ProjectManagement_Controls_UI_ProjectTask_Edit : CMSAdminEditControl
{
    #region "Variables"

    private ProjectTaskInfo mProjectTaskObj = null;
    private int mProjectID = 0;
    private int mProjectTaskAssigneeID = 0;
    private int mProjectTaskOwnerID = 0;
    private bool mShowOKButton = true;
    private bool mDisplayTaskUrl = false;

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
    /// Indicates if control is used on live site
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
            txtProjectTaskDisplayName.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether task url should be displayed.
    /// </summary>
    public bool DisplayTaskURL
    {
        get
        {
            return mDisplayTaskUrl;
        }
        set
        {
            mDisplayTaskUrl = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether validators should be disabled
    /// If true only server side validation will be working
    /// </summary>
    public bool DisableOnSiteValidators
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisableOnSiteValidators"), false);
        }
        set
        {
            SetValue("DisableOnSiteValidators", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether OK button should be displayed.
    /// </summary>
    public bool ShowOKButton
    {
        get
        {
            return mShowOKButton;
        }
        set
        {
            mShowOKButton = value;
        }
    }


    /// <summary>
    /// Gets or sets the project task info object.
    /// </summary>
    public ProjectTaskInfo ProjectTaskObj
    {
        get
        {
            if (mProjectTaskObj == null)
            {
                mProjectTaskObj = ProjectTaskInfoProvider.GetProjectTaskInfo(ItemID);
            }
            return mProjectTaskObj;
        }
        set
        {
            mProjectTaskObj = value;
        }
    }


    /// <summary>
    /// ID of task's project.
    /// </summary>
    public int ProjectGroupID
    {
        get
        {
            // First check if task has project
            if (ProjectTaskObj != null)
            {
                ProjectInfo pi = ProjectInfoProvider.GetProjectInfo(ProjectTaskObj.ProjectTaskProjectID);
                if (pi != null)
                {
                    return pi.ProjectGroupID;
                }
            }
            return ModuleCommands.CommunityGetCurrentGroupID();
        }
    }


    /// <summary>
    /// Task's project ID.
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
    /// Task assignee ID.
    /// </summary>
    public int ProjectTaskAssigneeID
    {
        set
        {
            mProjectTaskAssigneeID = value;
        }
    }


    /// <summary>
    /// Task owner ID.
    /// </summary>
    public int ProjectTaskOwnerID
    {
        set
        {
            mProjectTaskOwnerID = value;
        }
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        // DelayedReload
        selectorAssignee.UniSelector.Value = "-1";
        selectorOwner.UniSelector.Value = "-1";

        base.OnInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        SetupControls();

        // Set edited task
        EditedObject = ProjectTaskObj ?? new ProjectTaskInfo();

        // Check whether the project still exists
        if ((ProjectID > 0) && (ProjectInfoProvider.GetProjectInfo(ProjectID) == null))
        {
            RedirectToInformation("editedobject.notexists");
        }

        // Initialize HTML Editor
        htmlTaskDescription.AutoDetectLanguage = false;
        htmlTaskDescription.DefaultLanguage = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
        htmlTaskDescription.EditorAreaCSS = "";
        htmlTaskDescription.ToolbarSet = "ProjectManagement";

        // Check stop processing
        if (StopProcessing)
        {
            return;
        }

        ReloadData();

        btnOk.Visible = ShowOKButton;
        dtpProjectTaskDeadline.IsLiveSite = IsLiveSite;

        // Set associated controls for form controls due to validity
        lblProjectTaskOwnerID.AssociatedControlClientID = selectorOwner.ValueElementID;
        lblProjectTaskAssignedToUserID.AssociatedControlClientID = selectorAssignee.ValueElementID;
    }


    public override void ReloadData(bool forceReload)
    {
        // Load the form data
        if (forceReload || !URLHelper.IsPostback())
        {
            LoadData();
        }

        // Set current group id to the user selectors
        selectorAssignee.GroupID = ProjectGroupID;
        selectorOwner.GroupID = ProjectGroupID;

        base.ReloadData(forceReload);
    }


    protected override void OnPreRender(EventArgs e)
    {


        base.OnPreRender(e);

        // Load data for Task URL field
        if ((DisplayTaskURL) && (ItemID != 0))
        {
            string url = CreateTaskURL();
            if (!String.IsNullOrEmpty(url))
            {
                plcTaskUrl.Visible = true;
                txtTaskUrl.Text = url;
            }
            else
            {
                plcTaskUrl.Visible = false;
            }
        }
        else
        {
            plcTaskUrl.Visible = false;
        }
    }


    /// <summary>
    /// CreateURL of current task from current document.
    /// </summary>
    private string CreateTaskURL()
    {
        String url = String.Empty;
        if (ProjectTaskObj != null)
        {
            if (ProjectTaskObj.ProjectTaskProjectID != 0)
            {
                // Find project node url
                ProjectInfo pi = ProjectInfoProvider.GetProjectInfo(ProjectTaskObj.ProjectTaskProjectID);
                if (pi != null)
                {
                    TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);
                    TreeNode treeNode = tree.SelectSingleNode(pi.ProjectNodeID);
                    if (treeNode != null)
                    {
                        SiteInfo si = SiteInfoProvider.GetSiteInfo(pi.ProjectSiteID);
                        if (si != null)
                        {
                            // Create absolute URL of project's node
                            url = URLHelper.GetAbsoluteUrl(DocumentURLProvider.GetUrl(treeNode.NodeAliasPath, String.Empty, si.SiteName));

                            // Add projectID as parameter
                            url = URLHelper.UpdateParameterInUrl(url, "projectid", ProjectTaskObj.ProjectTaskProjectID.ToString());

                            // Add taskID as parameter
                            url = URLHelper.UpdateParameterInUrl(url, "taskid", ProjectTaskObj.ProjectTaskID.ToString());
                        }
                    }
                }
            }
            else
            {
                // If Ad hoc task create url to current node
                url = URLHelper.GetAbsoluteUrl(RequestContext.CurrentURL);

                // Remove query
                url = URLHelper.RemoveQuery(url);

                // Add task ID
                url = URLHelper.UpdateParameterInUrl(url, "taskid", ProjectTaskObj.ProjectTaskID.ToString());
            }
        }
        return url;
    }


    protected void btnOk_Click(object sender, EventArgs e)
    {
        // Validate and save the data
        Save();
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Initializes form controls.
    /// </summary>
    private void SetupControls()
    {
        // Button
        // btnOk.Text = GetString("general.ok");
        htmlTaskDescription.IsLiveSite = IsLiveSite;

        // Set labels/tooltips
        lblProjectTaskHours.Text = "(" + GetString("pm.projecttask.hours") + ")";
        lblProjectTaskProgressSymbol.Text = "%";

        lblProjectTaskDisplayName.ToolTip = GetString("pm.projecttask.tooltip.displayname");
        lblProjectTaskAssignedToUserID.ToolTip = GetString("pm.projecttask.tooltip.assignee");
        lblProjectTaskProgress.ToolTip = GetString("pm.projecttask.tooltip.progress");
        lblProjectTaskEstimate.ToolTip = GetString("pm.projecttask.tooltip.estimate");
        lblProjectTaskOwnerID.ToolTip = GetString("pm.projecttask.tooltip.owner");
        lblProjectTaskDeadline.ToolTip = GetString("pm.projecttask.tooltip.deadline");
        lblProjectTaskStatusID.ToolTip = GetString("pm.projecttask.tooltip.status");
        lblProjectTaskPriorityID.ToolTip = GetString("pm.projecttask.tooltip.priority");
        lblProjectTaskIsPrivate.ToolTip = GetString("pm.projecttask.tooltip.private");
        lblProjectTaskDescription.ToolTip = GetString("pm.projecttask.tooltip.description");
        lblTaskUrl.ToolTip = GetString("pm.projecttask.tooltip.taskUrl");

        // Disable validators if it is required
        if (DisableOnSiteValidators)
        {
            rfvProjectTaskDisplayName.Enabled = false;
            rvHours.Enabled = false;
            regexProgress.Enabled = false;
        }

        // Validators
        rfvProjectTaskDisplayName.ErrorMessage = GetString("general.requiresdisplayname");
        regexProgress.ErrorMessage = GetString("pm.projecttask.intnumber");
        rvHours.ErrorMessage = GetString("pm.projecttask.positivenumber");

        // user filter settings
        selectorAssignee.IsLiveSite = IsLiveSite;
        selectorAssignee.ShowSiteFilter = false;
        selectorAssignee.ApplyValueRestrictions = false;

        selectorOwner.IsLiveSite = IsLiveSite;
        selectorOwner.ShowSiteFilter = false;
        selectorOwner.ApplyValueRestrictions = false;

        if (ProjectTaskObj != null)
        {
            plcAttachments.Visible = true;
            lstAttachments.ObjectType = ProjectTaskInfo.OBJECT_TYPE;
            lstAttachments.ObjectID = ProjectTaskObj.ProjectTaskID;
            lstAttachments.Category = ObjectAttachmentsCategories.ATTACHMENT;
            lstAttachments.OnAfterUpload += (s, e) => TouchProjectTaskInfo();
            lstAttachments.OnAfterDelete += (s, e) => TouchProjectTaskInfo();
            lstAttachments.ReloadData(true);
        }
        else
        {
            plcAttachments.Visible = false;
        }

        if (IsLiveSite)
        {
            selectorAssignee.HideDisabledUsers = true;
            selectorAssignee.HideNonApprovedUsers = true;
            selectorAssignee.HideHiddenUsers = true;

            selectorOwner.HideDisabledUsers = true;
            selectorOwner.HideNonApprovedUsers = true;
            selectorOwner.HideHiddenUsers = true;
        }

        // Display 'Changes were saved' message if required
        if (QueryHelper.GetBoolean("saved", false) && !URLHelper.IsPostback())
        {
            ShowChangesSaved();
        }
    }


    /// <summary>
    /// Loads the data into the form.
    /// </summary>
    private void LoadData()
    {
        // If dealayed reload or not postback with not delayed reload
        if (!URLHelper.IsPostback() || (drpTaskStatus.Items.Count == 0))
        {
            drpTaskStatus.DataSource = ProjectTaskStatusInfoProvider.GetProjectTaskStatuses(true);
            drpTaskStatus.DataValueField = "TaskStatusID";
            drpTaskStatus.DataTextField = "TaskStatusDisplayName";
            drpTaskStatus.DataBind();

            drpTaskPriority.DataSource = ProjectTaskPriorityInfoProvider.GetProjectTaskPriorities(true);
            drpTaskPriority.DataValueField = "TaskPriorityID";
            drpTaskPriority.DataTextField = "TaskPriorityDisplayName";
            drpTaskPriority.DataBind();
        }

        // Clear selector selection
        selectorOwner.UniSelector.Value = String.Empty;
        selectorAssignee.UniSelector.Value = String.Empty;

        // Load the form from the info object
        if (ProjectTaskObj != null)
        {
            txtProjectTaskDisplayName.Text = ProjectTaskObj.ProjectTaskDisplayName;
            if (ProjectTaskObj.ProjectTaskAssignedToUserID > 0)
            {
                selectorAssignee.UniSelector.Value = ProjectTaskObj.ProjectTaskAssignedToUserID;
                selectorAssignee.ReloadData();
            }
            txtProjectTaskProgress.Text = ProjectTaskObj.ProjectTaskProgress.ToString();
            txtProjectTaskHours.Text = ProjectTaskObj.ProjectTaskHours.ToString();
            if (ProjectTaskObj.ProjectTaskOwnerID > 0)
            {
                selectorOwner.UniSelector.Value = ProjectTaskObj.ProjectTaskOwnerID;
                selectorOwner.ReloadData();
            }
            dtpProjectTaskDeadline.SelectedDateTime = ProjectTaskObj.ProjectTaskDeadline;
            chkProjectTaskIsPrivate.Checked = ProjectTaskObj.ProjectTaskIsPrivate;
            htmlTaskDescription.ResolvedValue = ProjectTaskObj.ProjectTaskDescription;

            SetStatusDrp(ProjectTaskObj.ProjectTaskStatusID);
            SetPriorityDrp(ProjectTaskObj.ProjectTaskPriorityID);
        }
        else
        {
            // Set only available user
            var cui = MembershipContext.AuthenticatedUser;
            if (!IsLiveSite || !cui.UserIsDisabledManually)
            {
                // Load default data
                selectorOwner.UniSelector.Value = (mProjectTaskOwnerID > 0) ? mProjectTaskOwnerID : MembershipContext.AuthenticatedUser.UserID;
                selectorAssignee.UniSelector.Value = (mProjectTaskAssigneeID > 0) ? mProjectTaskAssigneeID : MembershipContext.AuthenticatedUser.UserID;
                selectorAssignee.ReloadData();
                selectorOwner.ReloadData();
            }

            LoadDefaultPriority();
        }
    }


    /// <summary>
    /// Saves the data.
    /// </summary>
    public bool Save()
    {
        // Validate the form
        if (Validate())
        {
            // New task
            if (ProjectTaskObj == null)
            {
                // New task - check permission of project task (ad-hoc task are allowed to create)
                if (!CheckPermissions("CMS.ProjectManagement", ProjectManagementPermissionType.CREATE, PERMISSION_MANAGE))
                {
                    return false;
                }

                ProjectTaskInfo pi = new ProjectTaskInfo();
                pi.ProjectTaskProjectOrder = 0;
                pi.ProjectTaskUserOrder = 0;
                pi.ProjectTaskProjectID = ProjectID;
                pi.ProjectTaskCreatedByID = MembershipContext.AuthenticatedUser.UserID;
                mProjectTaskObj = pi;
            }
            else
            {
                ItemID = ProjectTaskObj.ProjectTaskID;
                if (!CheckPermissions("CMS.ProjectManagement", ProjectManagementPermissionType.MODIFY, PERMISSION_MANAGE))
                {
                    return false;
                }
            }

            // Initialize object
            ProjectTaskObj.ProjectTaskDisplayName = txtProjectTaskDisplayName.Text.Trim();
            int assignedToUserId = ValidationHelper.GetInteger(selectorAssignee.UniSelector.Value, 0);
            if (assignedToUserId != ProjectTaskObj.ProjectTaskAssignedToUserID)
            {
                // If the task is reassigned - reset user order
                ProjectTaskObj.ProjectTaskUserOrder = ProjectTaskInfoProvider.GetTaskMaxOrder(ProjectTaskOrderByEnum.UserOrder, assignedToUserId) + 1;
                ProjectTaskObj.ProjectTaskAssignedToUserID = assignedToUserId;
            }

            ProjectTaskObj.ProjectTaskProgress = ValidationHelper.GetInteger(txtProjectTaskProgress.Text, 0);
            ProjectTaskObj.ProjectTaskHours = ValidationHelper.GetDouble(txtProjectTaskHours.Text, 0);
            ProjectTaskObj.ProjectTaskOwnerID = ValidationHelper.GetInteger(selectorOwner.UniSelector.Value, 0);
            ProjectTaskObj.ProjectTaskDeadline = dtpProjectTaskDeadline.SelectedDateTime;
            if ((ProjectTaskObj.ProjectTaskDeadline != DateTimeHelper.ZERO_TIME)
                && (ProjectTaskObj.ProjectTaskDeadline > DateTime.Now))
            {
                ProjectTaskObj.ProjectTaskNotificationSent = false;
            }
            ProjectTaskObj.ProjectTaskStatusID = ValidationHelper.GetInteger(drpTaskStatus.SelectedValue, 0);
            ProjectTaskObj.ProjectTaskPriorityID = ValidationHelper.GetInteger(drpTaskPriority.SelectedValue, 0);
            ProjectTaskObj.ProjectTaskIsPrivate = chkProjectTaskIsPrivate.Checked;
            ProjectTaskObj.ProjectTaskDescription = htmlTaskDescription.ResolvedValue;


            // Use try/catch due to license check
            try
            {
                // Save object data to database
                ProjectTaskInfoProvider.SetProjectTaskInfo(ProjectTaskObj);

                ItemID = ProjectTaskObj.ProjectTaskID;
                RaiseOnSaved();

                ShowChangesSaved();
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }
        return false;
    }


    /// <summary>
    /// Validates the form. If validation succeeds returns true, otherwise returns false.
    /// </summary>
    private bool Validate()
    {
        // Validate required fields
        string errorMessage = new Validator()
            .NotEmpty(txtProjectTaskDisplayName.Text.Trim(), rfvProjectTaskDisplayName.ErrorMessage).Result;

        if (!dtpProjectTaskDeadline.IsValidRange())
        {
            errorMessage = GetString("general.errorinvaliddatetimerange");
        }

        // Check if progress is out of range
        if (txtProjectTaskProgress.Text.Length > 0)
        {
            regexProgress.Validate();
            if ((!regexProgress.IsValid)
                || (!ValidationHelper.IsInteger(txtProjectTaskProgress.Text)))
            {
                errorMessage = GetString("pm.projecttask.progressrange");
            }
            else
            {
                int progress = ValidationHelper.GetInteger(txtProjectTaskProgress.Text, 0);
                if ((progress < 0) || (progress > 100) || (txtProjectTaskProgress.Text.Length > 3))
                {
                    errorMessage = GetString("pm.projecttask.progressrange");
                }
            }
        }

        // Check if hours is out of range
        if (txtProjectTaskHours.Text.Length > 0)
        {
            rvHours.Validate();
            if ((!rvHours.IsValid)
                || (!ValidationHelper.IsDouble(txtProjectTaskHours.Text)))
            {
                errorMessage = GetString("pm.projecttask.estimate") + " " + rvHours.ErrorMessage.ToLowerCSafe();
            }
            else
            {
                double hours = ValidationHelper.GetDouble(txtProjectTaskHours.Text, 0);
                if (hours < 0)
                {
                    errorMessage = GetString("pm.projecttask.positivenumber");
                }
            }
        }

        // Check if there is at least one status defined
        if (ValidationHelper.GetInteger(drpTaskStatus.SelectedValue, 0) == 0)
        {
            errorMessage = GetString("pm.projecttaskstatus.warningnorecord");
        }

        // Check if there is at least one status defined
        if (ValidationHelper.GetInteger(drpTaskStatus.SelectedValue, 0) == 0)
        {
            errorMessage = GetString("pm.projecttaskpriority.warningnorecord");
        }


        // Set the error message
        if (!String.IsNullOrEmpty(errorMessage))
        {
            ShowError(errorMessage);
            return false;
        }

        return true;
    }


    /// <summary>
    /// Updates curent project task info after uploader events (upload and delete).
    /// </summary>
    private void TouchProjectTaskInfo()
    {
        if (ProjectTaskObj != null)
        {
            ProjectTaskInfoProvider.SetProjectTaskInfo(ProjectTaskObj);
        }
    }


    /// <summary>
    /// Selects status in the drop down list.
    /// </summary>
    /// <param name="value">The selected value</param>
    private void SetStatusDrp(int value)
    {
        if (drpTaskStatus.Items.FindByValue(value.ToString()) == null)
        {
            // Status not found (is disabled) - add manually
            ProjectTaskStatusInfo status = ProjectTaskStatusInfoProvider.GetProjectTaskStatusInfo(value);
            drpTaskStatus.Items.Add(new ListItem(status.TaskStatusDisplayName, status.TaskStatusID.ToString()));
        }

        drpTaskStatus.SelectedValue = value.ToString();
    }


    /// <summary>
    /// Selects priority in the drop down list.
    /// </summary>
    /// <param name="value">The selected value</param>
    private void SetPriorityDrp(int value)
    {
        if (drpTaskPriority.Items.FindByValue(value.ToString()) == null)
        {
            // Priority not found (is disabled) - add manually
            ProjectTaskPriorityInfo priority = ProjectTaskPriorityInfoProvider.GetProjectTaskPriorityInfo(value);
            drpTaskPriority.Items.Add(new ListItem(priority.TaskPriorityDisplayName, priority.TaskPriorityID.ToString()));
        }

        drpTaskPriority.SelectedValue = value.ToString();
    }


    /// <summary>
    /// Loads and set default priority.
    /// </summary>
    private void LoadDefaultPriority()
    {
        ProjectTaskPriorityInfo defaultPriority = ProjectTaskPriorityInfoProvider.GetDefaultPriority();
        if (defaultPriority != null)
        {
            try
            {
                drpTaskPriority.SelectedValue = defaultPriority.TaskPriorityID.ToString();
            }
            catch
            {
            }
        }
    }


    /// <summary>
    /// Clears current form.
    /// </summary>
    public override void ClearForm()
    {
        mProjectTaskObj = null;
        txtProjectTaskDisplayName.Text = String.Empty;
        txtProjectTaskHours.Text = String.Empty;
        txtProjectTaskProgress.Text = String.Empty;
        chkProjectTaskIsPrivate.Checked = false;
        dtpProjectTaskDeadline.SelectedDateTime = DateTimeHelper.ZERO_TIME;
        selectorOwner.UniSelector.Value = String.Empty;
        selectorAssignee.UniSelector.Value = String.Empty;
        htmlTaskDescription.Value = String.Empty;
        drpTaskStatus.SelectedIndex = 0;
        LoadDefaultPriority();
        base.ClearForm();
    }


    /// <summary>
    /// Sets the error text.
    /// </summary>
    /// <param name="errorText">Error message</param>
    public void SetError(string errorText)
    {
        // Check whether error message is defined
        if (!String.IsNullOrEmpty(errorText))
        {
            ShowError(errorText);
        }
    }

    #endregion
}