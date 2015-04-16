using System;
using System.Linq;

using CMS;
using CMS.Base;
using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.ProjectManagement;


[assembly: RegisterCustomClass("ProjectTaskStatusFormExtender", typeof(ProjectTaskStatusFormExtender))]

/// <summary>
/// Extends form for creating/editing ProjectTaskStatus.
/// </summary>
public class ProjectTaskStatusFormExtender : ControlExtender<UIForm>
{

    #region "Properties"

    /// <summary>
    /// Gets the project.
    /// </summary>
    private ProjectTaskStatusInfo ProjectTaskStatus
    {
        get
        {
            return Control.EditedObject as ProjectTaskStatusInfo;
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
        if (ProjectTaskStatus == null)
        {
            return;
        }

        string errorMessage = null;

        // Give error if status is both: started and finished
        if (ProjectTaskStatus.TaskStatusIsFinished && ProjectTaskStatus.TaskStatusIsNotStarted)
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
