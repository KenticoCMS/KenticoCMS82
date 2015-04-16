using System;
using System.Web;
using System.Web.UI.WebControls;

using CMS.Helpers;
using CMS.Membership;
using CMS.ProjectManagement;
using CMS.DocumentEngine;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.ExtendedControls;
using CMS.DataEngine;

using TreeNode = CMS.DocumentEngine.TreeNode;

public partial class CMSModules_ProjectManagement_Controls_UI_Project_Edit : CMSAdminEditControl
{
    #region "Variables"

    private ProjectInfo mProjectObj;
    private int mProjectId;
    private Guid mCodenameGuid = Guid.Empty;

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
            txtProjectDisplayName.IsLiveSite = value;
            txtProjectDescription.IsLiveSite = value;
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
    /// Gets the project info object.
    /// </summary>
    public ProjectInfo ProjectObj
    {
        get
        {
            if (mProjectObj == null)
            {
                mProjectObj = ProjectInfoProvider.GetProjectInfo(ProjectID);
            }

            return mProjectObj;
        }
    }


    /// <summary>
    /// Gets the guid which should be used for codename in simple mode.
    /// </summary>
    protected Guid CodenameGUID
    {
        get
        {
            if (mCodenameGuid == Guid.Empty)
            {
                mCodenameGuid = Guid.NewGuid();
            }
            return mCodenameGuid;
        }
    }


    /// <summary>
    /// If false don't display ok button.
    /// </summary>
    public bool ShowOKButton
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowOKButton"), true);
        }
        set
        {
            SetValue("ShowOKButton", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether page selector should be displayed.
    /// </summary>
    public bool ShowPageSelector
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowPageSelector"), true);
        }
        set
        {
            SetValue("ShowPageSelector", value);
        }
    }


    /// <summary>
    /// ID of group where project belongs to.
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
            userSelector.GroupID = CommunityGroupID;
        }
    }


    /// <summary>
    /// ID of document where project belongs to.
    /// </summary>
    public int ProjectNodeID
    {
        get;
        set;
    }


    /// <summary>
    /// Project ID.
    /// </summary>
    public int ProjectID
    {
        get
        {
            return mProjectId;
        }
        set
        {
            mProjectId = value;
            mProjectObj = null;
        }
    }


    /// <summary>
    /// Indicates delayed reload not from page_load.
    /// </summary>
    public bool DelayedReload
    {
        get;
        set;
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        userSelector.UniSelector.Value = "-1";
        pageSelector.Value = "-1";
        base.OnInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            return;
        }

        drpProjectStatus.DataBound += drpProjectStatus_DataBound;
        dtpProjectDeadline.IsLiveSite = IsLiveSite;
        dtpProjectStartDate.IsLiveSite = IsLiveSite;

        SetupControls();

        // Set edited object
        if (ProjectID > 0)
        {
            EditedObject = ProjectObj;
        }

        // Load the form data
        if ((!URLHelper.IsPostback()) && (!DelayedReload))
        {
            LoadData();
        }

        btnOk.Visible = ShowOKButton;

        // Set associated controls for form controls due to validity
        lblProjectOwner.AssociatedControlClientID = userSelector.ValueElementID;
        lblProjectPage.AssociatedControlClientID = pageSelector.ValueElementID;
    }


    protected void btnOk_Click(object sender, EventArgs e)
    {
        // Validate and save the data
        Save();
    }

    #endregion


    #region "Public Methods"

    /// <summary>
    /// Saves control with actual data.
    /// </summary>
    public bool Save()
    {
        if (!CheckPermissions("CMS.ProjectManagement", PERMISSION_MANAGE))
        {
            return false;
        }

        // Validate the form
        if (Validate())
        {
            // Indicates whether project is new
            bool isNew = false;

            int progress = 0;

            // Ensure the info object
            if (ProjectObj == null)
            {
                // New project

                ProjectInfo pi = new ProjectInfo();
                // First initialization of the Access property - allow authenticated users
                pi.ProjectAccess = 1222;
                pi.ProjectCreatedByID = MembershipContext.AuthenticatedUser.UserID;

                pi.ProjectOwner = 0;

                if (CommunityGroupID != 0)
                {
                    pi.ProjectGroupID = CommunityGroupID;
                    // Set default access to the group
                    pi.ProjectAccess = 3333;
                }

                mProjectObj = pi;
                isNew = true;
            }
            else
            {
                // Existing project

                // Reset ProjectOrder if checkbox was unchecked
                if ((ProjectObj.ProjectAllowOrdering)
                    && (!chkProjectAllowOrdering.Checked))
                {
                    ProjectInfoProvider.ResetProjectOrder(ProjectObj.ProjectID);
                }

                // Clear the hash tables if the codename has been changed
                if ((ProjectObj.ProjectGroupID > 0)
                    && ProjectObj.ProjectName != txtProjectName.Text)
                {
                    ProjectInfoProvider.Clear(true);
                }

                progress = ProjectInfoProvider.GetProjectProgress(ProjectObj.ProjectID);
            }

            ltrProjectProgress.Text = ProjectTaskInfoProvider.GenerateProgressHtml(progress, true);

            // Initialize object
            ProjectObj.ProjectSiteID = SiteContext.CurrentSiteID;

            if (DisplayMode == ControlDisplayModeEnum.Simple)
            {
                if (isNew)
                {
                    ProjectObj.ProjectName = ValidationHelper.GetCodeName(txtProjectDisplayName.Text, 50) + ((CommunityGroupID > 0) ? "_group_" : "_general_") + CodenameGUID;
                }
            }
            else
            {
                ProjectObj.ProjectName = txtProjectName.Text.Trim();
            }
            ProjectObj.ProjectDisplayName = txtProjectDisplayName.Text.Trim();
            ProjectObj.ProjectDescription = txtProjectDescription.Text.Trim();
            ProjectObj.ProjectStartDate = dtpProjectStartDate.SelectedDateTime;
            ProjectObj.ProjectDeadline = dtpProjectDeadline.SelectedDateTime;
            ProjectObj.ProjectOwner = ValidationHelper.GetInteger(userSelector.UniSelector.Value, 0);
            ProjectObj.ProjectStatusID = ValidationHelper.GetInteger(drpProjectStatus.SelectedValue, 0);
            ProjectObj.ProjectAllowOrdering = chkProjectAllowOrdering.Checked;

            // Set ProjectNodeID
            if (!ShowPageSelector)
            {
                // Set current node id for new project
                if (isNew && (DocumentContext.CurrentDocument != null))
                {
                    ProjectObj.ProjectNodeID = DocumentContext.CurrentDocument.NodeID;
                }
            }
            else
            {
                TreeProvider treeProvider = new TreeProvider();
                TreeNode node = treeProvider.SelectSingleNode(ValidationHelper.GetGuid(pageSelector.Value, Guid.Empty), TreeProvider.ALL_CULTURES, SiteContext.CurrentSiteName);
                ProjectObj.ProjectNodeID = node != null ? node.NodeID : 0;
            }

            // Use try/catch due to license check
            try
            {
                // Save object data to database
                ProjectInfoProvider.SetProjectInfo(ProjectObj);
                ProjectID = ProjectObj.ProjectID;

                ItemID = ProjectObj.ProjectID;
                RaiseOnSaved();

                // Set the info message
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


    #region "Private methods"

    /// <summary>
    /// Initializes form controls.
    /// </summary>
    private void SetupControls()
    {
        // Button
        btnOk.Text = GetString("general.ok");

        // Set tooltips
        lblProjectDisplayName.ToolTip = GetString("pm.project.tooltip.displayname");
        lblProjectName.ToolTip = GetString("pm.project.tooltip.codename");
        lblProjectDescription.ToolTip = GetString("pm.project.tooltip.description");
        lblProjectStartDate.ToolTip = GetString("pm.project.tooltip.startdate");
        lblProjectDeadline.ToolTip = GetString("pm.project.tooltip.deadline");
        lblProjectProgress.ToolTip = GetString("pm.project.tooltip.progress");
        lblProjectOwner.ToolTip = GetString("pm.project.tooltip.owner");
        lblProjectStatusID.ToolTip = GetString("pm.project.tooltip.status");
        lblProjectPage.ToolTip = GetString("pm.project.tooltip.page");
        lblProjectAllowOrdering.ToolTip = GetString("pm.project.tooltip.allowOrdering");

        // Disable validators if it is required
        if (DisableOnSiteValidators)
        {
            rfvProjectName.Enabled = false;
            rfvProjectDisplayName.Enabled = false;
        }

        // Validator texts
        rfvProjectName.ErrorMessage = GetString("general.requirescodename");
        rfvProjectDisplayName.ErrorMessage = GetString("general.requiresdisplayname");

        pageSelector.IsLiveSite = IsLiveSite;
        pageSelector.EnableSiteSelection = false;

        // Page selector - show only documents of the current group
        if (CommunityGroupID > 0)
        {
            GeneralizedInfo infoObj = ModuleCommands.CommunityGetGroupInfo(CommunityGroupID);
            if (infoObj != null)
            {
                Guid groupNodeGUID = ValidationHelper.GetGuid(infoObj.GetValue("GroupNodeGUID"), Guid.Empty);

                if (groupNodeGUID != Guid.Empty)
                {
                    TreeProvider treeProvider = new TreeProvider();
                    TreeNode node = treeProvider.SelectSingleNode(groupNodeGUID, TreeProvider.ALL_CULTURES, SiteContext.CurrentSiteName);
                    if (node != null)
                    {
                        pageSelector.ContentStartingPath = HttpUtility.UrlEncode(node.NodeAliasPath);
                    }
                }
                else
                {
                    pageSelector.Enabled = false;
                }
            }
        }

        userSelector.IsLiveSite = IsLiveSite;
        userSelector.SiteID = SiteContext.CurrentSiteID;
        userSelector.ShowSiteFilter = false;
        userSelector.GroupID = CommunityGroupID;
        userSelector.ApplyValueRestrictions = false;
        
        lstAttachments.ObjectType = ProjectInfo.OBJECT_TYPE;
        lstAttachments.Category = ObjectAttachmentsCategories.ATTACHMENT;
        lstAttachments.OnAfterUpload += (s, e ) => TouchProjectInfo();
        lstAttachments.OnAfterDelete += (s, e) => TouchProjectInfo();

        // Hide hidden & disabled user on live site
        if (IsLiveSite)
        {
            userSelector.HideHiddenUsers = true;
            userSelector.HideDisabledUsers = true;
            userSelector.HideNonApprovedUsers = true;
        }

        // Hide page selector on live site
        if (!ShowPageSelector)
        {
            plcProjectPage.Visible = false;
        }

        // Hide codename textbox for simple display mode
        if (DisplayMode == ControlDisplayModeEnum.Simple)
        {
            plcCodeName.Visible = false;
        }

        // Display 'Changes were saved' message if required
        if (QueryHelper.GetBoolean("saved", false) && !URLHelper.IsPostback())
        {
            ShowChangesSaved();
        }
    }


    /// <summary>
    /// Updates current project info after upload events (upload and delete).
    /// </summary>
    private void TouchProjectInfo()
    {
        if (ProjectObj != null)
        {
            ProjectInfoProvider.SetProjectInfo(ProjectObj);
        }
    }


    public override void ReloadData()
    {
        LoadData();
        base.ReloadData();
    }


    /// <summary>
    /// Loads the data into the form.
    /// </summary>
    public void LoadData()
    {
        // Check if the projects belongs to the current site
        if ((ProjectObj != null) && (ProjectObj.ProjectSiteID != SiteContext.CurrentSiteID))
        {
            return;
        }

        // If delayed reload or not post back with not delayed reload
        if (((!URLHelper.IsPostback()) && (!DelayedReload)) || (DelayedReload))
        {
            LoadDropDown();
        }

        // Load the form from the info object
        if (ProjectObj != null)
        {
            txtProjectName.Text = ProjectObj.ProjectName;
            txtProjectDisplayName.Text = ProjectObj.ProjectDisplayName;
            txtProjectDescription.Text = ProjectObj.ProjectDescription;
            dtpProjectStartDate.SelectedDateTime = ProjectObj.ProjectStartDate;
            dtpProjectDeadline.SelectedDateTime = ProjectObj.ProjectDeadline;

            int progress = ProjectInfoProvider.GetProjectProgress(ProjectObj.ProjectID);
            ltrProjectProgress.Text = ProjectTaskInfoProvider.GenerateProgressHtml(progress, true);

            if (ProjectObj.ProjectOwner != 0)
            {
                userSelector.UniSelector.Value = ProjectObj.ProjectOwner;
                userSelector.ReloadData();
            }
            else
            {
                userSelector.UniSelector.Value = String.Empty;
            }

            chkProjectAllowOrdering.Checked = ProjectObj.ProjectAllowOrdering;

            SetStatusDrp(ProjectObj.ProjectStatusID);

            if (ProjectObj.ProjectNodeID != 0)
            {
                SetProjectPage(ProjectObj.ProjectNodeID);
            }

            plcAttachments.Visible = true;
            lstAttachments.ObjectID = ProjectObj.ProjectID;
            lstAttachments.ReloadData(true);
        }
        else
        {
            userSelector.UniSelector.Value = "";
            var cui = MembershipContext.AuthenticatedUser;
            if (!IsLiveSite || !cui.UserIsDisabledManually)
            {
                // Load default data
                userSelector.UniSelector.Value = MembershipContext.AuthenticatedUser.UserID;
            }

            pageSelector.Value = String.Empty;
            if (ProjectNodeID != 0)
            {
                SetProjectPage(ProjectNodeID);
            }

            ltrProjectProgress.Text = ProjectTaskInfoProvider.GenerateProgressHtml(0, true);

            // Hide progress bar for a new project
            plcProgress.Visible = false;

            // Hide atachments for a new project
            plcAttachments.Visible = false;
        }
    }


    /// <summary>
    /// Loads the data to the status dropdown field.
    /// </summary>
    private void LoadDropDown()
    {
        drpProjectStatus.DataSource = ProjectStatusInfoProvider.GetProjectStatuses(true);
        drpProjectStatus.DataValueField = "StatusID";
        drpProjectStatus.DataTextField = "StatusDisplayName";
        drpProjectStatus.DataBind();
    }


    void drpProjectStatus_DataBound(object sender, EventArgs e)
    {
        foreach (ListItem li in drpProjectStatus.Items)
        {
            li.Text = ResHelper.LocalizeString(li.Text);
        }
    }


    /// <summary>
    /// Validates the form. If validation succeeds returns true, otherwise returns false.
    /// </summary>
    private bool Validate()
    {
        string codename = txtProjectName.Text.Trim();
        if (DisplayMode == ControlDisplayModeEnum.Simple)
        {
            codename = ValidationHelper.GetCodeName(txtProjectDisplayName.Text, 50) + ((CommunityGroupID > 0) ? "_group_" : "_general_") + CodenameGUID;
        }

        // Validate required fields
        string errorMessage = new Validator()
            .NotEmpty(txtProjectDisplayName.Text.Trim(), rfvProjectDisplayName.ErrorMessage)
            .NotEmpty(codename, rfvProjectName.ErrorMessage)
            .IsCodeName(codename, GetString("general.invalidcodename")).Result;


        if (!dtpProjectDeadline.IsValidRange() || !dtpProjectStartDate.IsValidRange())
        {
            errorMessage = GetString("general.errorinvaliddatetimerange");
        }

        // Check the uniqueness of the codename
        ProjectInfo pi = ProjectInfoProvider.GetProjectInfo(codename, SiteContext.CurrentSiteID, CommunityGroupID);
        if ((pi != null) && (pi.ProjectID != ProjectID))
        {
            errorMessage = GetString("general.codenameexists");
        }

        // Check if there is at least one status defined
        if (ValidationHelper.GetInteger(drpProjectStatus.SelectedValue, 0) == 0)
        {
            errorMessage = GetString("pm.projectstatus.warningnorecord");
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
    /// Selects status the in drop down list.
    /// </summary>
    /// <param name="value">The selected value</param>
    private void SetStatusDrp(int value)
    {
        if (drpProjectStatus.Items.FindByValue(value.ToString()) == null)
        {
            // Status not found (is disabled) - add manually
            ProjectStatusInfo status = ProjectStatusInfoProvider.GetProjectStatusInfo(value);
            if (status != null)
            {
                drpProjectStatus.Items.Add(new ListItem(status.StatusDisplayName, status.StatusID.ToString()));
            }
        }

        drpProjectStatus.SelectedValue = value.ToString();
    }


    /// <summary>
    /// Sets the project page.
    /// </summary>
    /// <param name="nodeID">The node ID</param>
    private void SetProjectPage(int nodeID)
    {
        TreeProvider treeProvider = new TreeProvider();
        TreeNode node = treeProvider.SelectSingleNode(nodeID);
        if (node != null)
        {
            pageSelector.Value = node.NodeGUID.ToString();
        }
    }


    /// <summary>
    /// Clears form.
    /// </summary>
    public override void ClearForm()
    {
        txtProjectDescription.Text = String.Empty;
        txtProjectDisplayName.Text = String.Empty;
        txtProjectName.Text = String.Empty;
        chkProjectAllowOrdering.Checked = true;
        dtpProjectStartDate.SelectedDateTime = DateTimeHelper.ZERO_TIME;
        dtpProjectDeadline.SelectedDateTime = DateTimeHelper.ZERO_TIME;
        userSelector.UniSelector.Value = "";
        drpProjectStatus.SelectedIndex = 0;
        pageSelector.Clear();
        base.ClearForm();
    }

    #endregion
}