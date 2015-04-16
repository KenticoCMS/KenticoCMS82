using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Helpers;
using CMS.ProjectManagement;
using CMS.UIControls;
using CMS.ExtendedControls;

public partial class CMSModules_ProjectManagement_Controls_UI_Projecttaskpriority_Edit : CMSAdminEditControl
{
    #region "Variables"

    private ProjectTaskPriorityInfo mProjecttaskpriorityObj = null;
    private int mProjecttaskpriorityId = 0;

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
    /// Projecttaskpriority data.
    /// </summary>
    public ProjectTaskPriorityInfo ProjecttaskpriorityObj
    {
        get
        {
            if (mProjecttaskpriorityObj == null)
            {
                mProjecttaskpriorityObj = ProjectTaskPriorityInfoProvider.GetProjectTaskPriorityInfo(TaskPriorityID);
            }

            return mProjecttaskpriorityObj;
        }
        set
        {
            mProjecttaskpriorityObj = value;
            if (value != null)
            {
                mProjecttaskpriorityId = value.TaskPriorityID;
            }
            else
            {
                mProjecttaskpriorityId = 0;
            }
        }
    }


    /// <summary>
    /// Projecttaskpriority ID.
    /// </summary>
    public int TaskPriorityID
    {
        get
        {
            return mProjecttaskpriorityId;
        }
        set
        {
            mProjecttaskpriorityId = value;
            mProjecttaskpriorityObj = null;
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
        if (mProjecttaskpriorityId > 0)
        {
            EditedObject = ProjecttaskpriorityObj;
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
        rfvTaskPriorityName.ErrorMessage = GetString("general.requirescodename");
        rfvTaskPriorityDisplayName.ErrorMessage = GetString("general.requiresdisplayname");

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
        if (ProjecttaskpriorityObj != null)
        {
            txtTaskPriorityName.Text = ProjecttaskpriorityObj.TaskPriorityName;
            txtTaskPriorityDisplayName.Text = ProjecttaskpriorityObj.TaskPriorityDisplayName;
            chkTaskPriorityEnabled.Checked = ProjecttaskpriorityObj.TaskPriorityEnabled;
            chkTaskPriorityDefault.Checked = ProjecttaskpriorityObj.TaskPriorityDefault;
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
            if (ProjecttaskpriorityObj == null)
            {
                ProjecttaskpriorityObj = new ProjectTaskPriorityInfo();
                ProjecttaskpriorityObj.TaskPriorityOrder = ProjectTaskPriorityInfoProvider.GetPriorityCount(false) + 1;
            }

            // Initialize object
            ProjecttaskpriorityObj.TaskPriorityName = txtTaskPriorityName.Text.Trim();
            ProjecttaskpriorityObj.TaskPriorityDisplayName = txtTaskPriorityDisplayName.Text.Trim();
            ProjecttaskpriorityObj.TaskPriorityEnabled = chkTaskPriorityEnabled.Checked;
            ProjecttaskpriorityObj.TaskPriorityDefault = chkTaskPriorityDefault.Checked;

            // Save object data to database
            ProjectTaskPriorityInfoProvider.SetProjectTaskPriorityInfo(ProjecttaskpriorityObj);

            ItemID = ProjecttaskpriorityObj.TaskPriorityID;
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
        string codename = txtTaskPriorityName.Text.Trim();

        // Validate required fields
        string errorMessage = new Validator()
            .NotEmpty(txtTaskPriorityDisplayName.Text.Trim(), rfvTaskPriorityDisplayName.ErrorMessage)
            .NotEmpty(codename, rfvTaskPriorityName.ErrorMessage)
            .IsCodeName(codename, GetString("general.invalidcodename")).Result;

        // Check the uniqueness of the codename
        ProjectTaskPriorityInfo ptpi = ProjectTaskPriorityInfoProvider.GetProjectTaskPriorityInfo(codename);
        if ((ptpi != null) && (ptpi.TaskPriorityID != TaskPriorityID))
        {
            errorMessage = GetString("general.codenameexists");
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