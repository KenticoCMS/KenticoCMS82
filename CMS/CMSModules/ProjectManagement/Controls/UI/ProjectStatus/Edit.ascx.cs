using System;

using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.ProjectManagement;
using CMS.UIControls;

public partial class CMSModules_ProjectManagement_Controls_UI_Projectstatus_Edit : CMSAdminEditControl
{
    #region "Variables"

    private ProjectStatusInfo mProjectstatusObj = null;
    private int mProjectstatusId = 0;

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
    /// Projectstatus data.
    /// </summary>
    public ProjectStatusInfo ProjectstatusObj
    {
        get
        {
            if (mProjectstatusObj == null)
            {
                mProjectstatusObj = ProjectStatusInfoProvider.GetProjectStatusInfo(StatusID);
            }

            return mProjectstatusObj;
        }
        set
        {
            mProjectstatusObj = value;
            if (value != null)
            {
                mProjectstatusId = value.StatusID;
            }
            else
            {
                mProjectstatusId = 0;
            }
        }
    }


    /// <summary>
    /// Projectstatus ID.
    /// </summary>
    public int StatusID
    {
        get
        {
            return mProjectstatusId;
        }
        set
        {
            mProjectstatusId = value;
            mProjectstatusObj = null;
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
        if (StatusID > 0)
        {
            EditedObject = ProjectstatusObj;
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
        rfvStatusDisplayName.ErrorMessage = GetString("general.requiresdisplayname");
        rfvStatusName.ErrorMessage = GetString("general.requirescodename");

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
        if (ProjectstatusObj != null)
        {
            txtStatusName.Text = ProjectstatusObj.StatusName;
            txtStatusDisplayName.Text = ProjectstatusObj.StatusDisplayName;
            colorPicker.SelectedColor = ProjectstatusObj.StatusColor;
            txtStatusIcon.Text = ProjectstatusObj.StatusIcon;
            chkStatusIsFinished.Checked = ProjectstatusObj.StatusIsFinished;
            chkStatusIsNotStarted.Checked = ProjectstatusObj.StatusIsNotStarted;
            chkStatusEnabled.Checked = ProjectstatusObj.StatusEnabled;
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
            if (ProjectstatusObj == null)
            {
                ProjectstatusObj = new ProjectStatusInfo();
                ProjectstatusObj.StatusOrder = ProjectStatusInfoProvider.GetStatusCount(false) + 1;
            }

            // Initialize object
            ProjectstatusObj.StatusName = txtStatusName.Text.Trim();
            ProjectstatusObj.StatusDisplayName = txtStatusDisplayName.Text.Trim();
            ProjectstatusObj.StatusColor = colorPicker.SelectedColor;
            ProjectstatusObj.StatusIcon = txtStatusIcon.Text.Trim();
            ProjectstatusObj.StatusIsFinished = chkStatusIsFinished.Checked;
            ProjectstatusObj.StatusIsNotStarted = chkStatusIsNotStarted.Checked;
            ProjectstatusObj.StatusEnabled = chkStatusEnabled.Checked;

            // Save object data to database
            ProjectStatusInfoProvider.SetProjectStatusInfo(ProjectstatusObj);

            ItemID = ProjectstatusObj.StatusID;
            RaiseOnSaved();

            // Show confirmation
            ShowChangesSaved();
        }
    }


    /// <summary>
    /// Validates the form. If validation succeeds returns true, otherwise returns false.
    /// </summary>
    private bool Validate()
    {
        string codename = txtStatusName.Text.Trim();

        // Validate required fields
        string errorMessage = new Validator()
            .NotEmpty(txtStatusDisplayName.Text.Trim(), rfvStatusDisplayName.ErrorMessage)
            .NotEmpty(codename, rfvStatusName.ErrorMessage)
            .IsCodeName(codename, GetString("general.invalidcodename")).Result;

        // Check the uniqueness of the codename
        ProjectStatusInfo psi = ProjectStatusInfoProvider.GetProjectStatusInfo(codename);
        if ((psi != null) && (psi.StatusID != StatusID))
        {
            errorMessage = GetString("general.codenameexists");
        }

        // Give error if status is both: started and finished
        if (chkStatusIsFinished.Checked && chkStatusIsNotStarted.Checked)
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