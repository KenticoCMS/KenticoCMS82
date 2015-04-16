using System;

using CMS;
using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.ProjectManagement;


[assembly: RegisterCustomClass("ProjectFormExtender", typeof(ProjectFormExtender))]

/// <summary>
/// Extends form for creating/editing Project.
/// </summary>
public class ProjectFormExtender : ControlExtender<UIForm>
{

    #region "Properties"

    /// <summary>
    /// Gets the project.
    /// </summary>
    private ProjectInfo Project
    {
        get
        {
            return Control.EditedObject as ProjectInfo;
        }
    }

    #endregion


    #region "Public methods"

    public override void OnInit()
    {
        Control.OnAfterDataLoad += Control_OnAfterDataLoad;
        Control.OnUploadFile += (s, e) => { TouchProjectInfo(); };
        Control.OnDeleteFile += (s, e) => { TouchProjectInfo(); };
    }

    #endregion


    #region "Private methods"

    void Control_OnAfterDataLoad(object sender, EventArgs e)
    {
        if (Project == null)
        {
            return;
        }

        if (Project.ProjectID != 0)
        {
            // Load progress
            int progress = ProjectInfoProvider.GetProjectProgress(Project.ProjectID);
            Control.FieldControls["ProjectProgress"].Text = ProjectTaskInfoProvider.GenerateProgressHtml(progress, true);
            
        }
        else
        {
            Control.FieldsToHide.Add("ProjectProgress");
            Control.FieldsToHide.Add("ProjectAttachments");
        }
    }


    /// <summary>
    /// Updates curent Project info after uploader events (upload and delete).
    /// </summary>
    private void TouchProjectInfo()
    {
        ProjectInfoProvider.SetProjectInfo(Project);
    }

    #endregion

}
