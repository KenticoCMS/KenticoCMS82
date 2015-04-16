using System;
using System.Linq;

using CMS;
using CMS.Base;
using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.ProjectManagement;

[assembly: RegisterCustomClass("ProjectStatusFormExtender", typeof(ProjectStatusFormExtender))]

/// <summary>
/// Extends form for creating/editing ProjectStatus.
/// </summary>
public class ProjectStatusFormExtender : ControlExtender<UIForm>
{

    #region "Properties"

    /// <summary>
    /// Gets the project.
    /// </summary>
    private ProjectStatusInfo ProjectStatus
    {
        get
        {
            return Control.EditedObject as ProjectStatusInfo;
        }
    }

    #endregion


    #region "Public methods"

    public override void OnInit()
    {
        Control.OnBeforeSave += Control_OnBeforeSave;
    }

    #endregion


    #region "Private methods"


    private void Control_OnBeforeSave(object sender, EventArgs e)
    {
        if (ProjectStatus == null)
        {
            return;
        }

        string errorMessage = null;

        // Give error if status is both: started and finished
        if (ProjectStatus.StatusIsFinished && ProjectStatus.StatusIsNotStarted)
        {
            Control.StopProcessing = true;
            errorMessage = Control.GetString("pm.projectstatus.startandfinish");
        }

        // Set the error message
        if (!String.IsNullOrEmpty(errorMessage))
        {
            Control.ShowError(errorMessage);
        }
    }

    #endregion

}
