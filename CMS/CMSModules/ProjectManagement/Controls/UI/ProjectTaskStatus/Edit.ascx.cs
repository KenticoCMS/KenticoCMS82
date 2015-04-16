using System;

using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.ProjectManagement;
using CMS.UIControls;

public partial class CMSModules_ProjectManagement_Controls_UI_Projecttaskstatus_Edit : CMSAdminEditControl
{
    #region "Variables"

    private ProjectTaskStatusInfo mProjecttaskstatusObj = null;
    private int mProjecttaskstatusId = 0;

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
        }
    }


    /// <summary>
    /// Projecttaskstatus data.
    /// </summary>
    public ProjectTaskStatusInfo ProjecttaskstatusObj
    {
        get
        {
            if (mProjecttaskstatusObj == null)
            {
                mProjecttaskstatusObj = ProjectTaskStatusInfoProvider.GetProjectTaskStatusInfo(TaskStatusID);
            }

            return mProjecttaskstatusObj;
        }
        set
        {
            mProjecttaskstatusObj = value;
            if (value != null)
            {
                mProjecttaskstatusId = value.TaskStatusID;
            }
            else
            {
                mProjecttaskstatusId = 0;
            }
        }
    }


    /// <summary>
    /// Projecttaskstatus ID.
    /// </summary>
    public int TaskStatusID
    {
        get
        {
            return mProjecttaskstatusId;
        }
        set
        {
            mProjecttaskstatusId = value;
            mProjecttaskstatusObj = null;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        CheckPermissions("CMS.ProjectManagement", ProjectManagementPermissionType.MANAGE_CONFIGURATION);

        if (StopProcessing)
        {
            return;
        }

        SetupControls();

        // Set edited object
        if (mProjecttaskstatusId > 0)
        {
            EditedObject = ProjecttaskstatusObj;
        }

        // Load the form data
        if (!URLHelper.IsPostback())
        {
            LoadData();
        }
    }


    protected void btnOk_Click(object sender, EventArgs e)
    {
        // Validate and save the data
        Process();
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

        // Validators
        rfvTaskStatusDisplayName.ErrorMessage = GetString("general.requiresdisplayname");
        rfvTaskStatusName.ErrorMessage = GetString("general.requirescodename");

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
        // Load the form from the info object
        if (ProjecttaskstatusObj != null)
        {
            txtTaskStatusName.Text = ProjecttaskstatusObj.TaskStatusName;
            txtTaskStatusDisplayName.Text = ProjecttaskstatusObj.TaskStatusDisplayName;
            colorPicker.SelectedColor = ProjecttaskstatusObj.TaskStatusColor;
            txtTaskStatusIcon.Text = ProjecttaskstatusObj.TaskStatusIcon;
            chkTaskStatusIsFinished.Checked = ProjecttaskstatusObj.TaskStatusIsFinished;
            chkTaskStatusIsNotStarted.Checked = ProjecttaskstatusObj.TaskStatusIsNotStarted;
            chkTaskStatusEnabled.Checked = ProjecttaskstatusObj.TaskStatusEnabled;
        }
    }


    /// <summary>
    // Processes the form - saves the data.
    /// </summary>
    private void Process()
    {
        // Validate the form
        if (Validate())
        {
            // Ensure the info object
            if (ProjecttaskstatusObj == null)
            {
                ProjecttaskstatusObj = new ProjectTaskStatusInfo();
                ProjecttaskstatusObj.TaskStatusOrder = ProjectTaskStatusInfoProvider.GetStatusCount(false) + 1;
            }

            // Initialize object
            ProjecttaskstatusObj.TaskStatusName = txtTaskStatusName.Text.Trim();
            ProjecttaskstatusObj.TaskStatusDisplayName = txtTaskStatusDisplayName.Text.Trim();
            ProjecttaskstatusObj.TaskStatusColor = colorPicker.SelectedColor;
            ProjecttaskstatusObj.TaskStatusIcon = txtTaskStatusIcon.Text.Trim();
            ProjecttaskstatusObj.TaskStatusIsFinished = chkTaskStatusIsFinished.Checked;
            ProjecttaskstatusObj.TaskStatusIsNotStarted = chkTaskStatusIsNotStarted.Checked;
            ProjecttaskstatusObj.TaskStatusEnabled = chkTaskStatusEnabled.Checked;

            // Save object data to database
            ProjectTaskStatusInfoProvider.SetProjectTaskStatusInfo(ProjecttaskstatusObj);

            ItemID = ProjecttaskstatusObj.TaskStatusID;
            RaiseOnSaved();

            // Set the info message
            ShowChangesSaved();
        }
    }


    /// <summary>
    /// Validates the form. If validation succeeds returns true, otherwise returns false.
    /// </summary>
    private bool Validate()
    {
        string codename = txtTaskStatusName.Text.Trim();

        // Validate required fields
        string errorMessage = new Validator()
            .NotEmpty(txtTaskStatusDisplayName.Text.Trim(), rfvTaskStatusDisplayName.ErrorMessage)
            .NotEmpty(codename, rfvTaskStatusName.ErrorMessage)
            .IsCodeName(codename, GetString("general.invalidcodename")).Result;

        // Check the uniqueness of the codename
        ProjectTaskStatusInfo ptsi = ProjectTaskStatusInfoProvider.GetProjectTaskStatusInfo(codename);
        if ((ptsi != null) && (ptsi.TaskStatusID != TaskStatusID))
        {
            errorMessage = GetString("general.codenameexists");
        }

        // Give error if status is both: started and finished
        if (chkTaskStatusIsFinished.Checked && chkTaskStatusIsNotStarted.Checked)
        {
            errorMessage = GetString("pm.projectstatus.startandfinish");
        }

        // Set the error message
        if (!String.IsNullOrEmpty(errorMessage))
        {
            ShowError(errorMessage);
            return false;
        }

        return true;
    }

    #endregion
}