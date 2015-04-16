using System;
using System.Reflection;

using CMS;
using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.Helpers;
using CMS.ProjectManagement;

[assembly: RegisterCustomClass("GroupProjectFormExtender", typeof(GroupProjectFormExtender))]

/// <summary>
/// Extends form for creating/editing Project under group.
/// </summary>
public class GroupProjectFormExtender : ControlExtender<UIForm>
{
    #region "Properties"

    /// <summary>
    /// Gets or sets the project.
    /// </summary>
    private IProjectInfo Project
    {
        get
        {
            return Control.EditedObject as IProjectInfo;
        }
    }

    #endregion


    #region "Public methods"

    public override void OnInit()
    {
        Control.OnAfterDataLoad += Control_OnAfterDataLoad;

        Control.OnUploadFile += TouchProjectInfo;
        Control.OnDeleteFile += TouchProjectInfo;
    }

    #endregion


    #region "Private methods"

    void Control_OnAfterDataLoad(object sender, EventArgs e)
    {
        if (Project == null)
        {
            return;
        }

        int groupId = QueryHelper.GetInteger("parentobjectid", 0);

        // Set group ID to user selector for project owner
        var ownerControl = Control.FieldControls["ProjectOwner"];
        Type ownerControlType = ownerControl.GetType();
        PropertyInfo groupIdProperty = ownerControlType.GetProperty("GroupID");
        if (groupIdProperty != null)
        {
            groupIdProperty.SetValue(ownerControl, groupId, null);
        }

        if (Project.ProjectID > 0)
        {
            // Load progress
            int progress = ProjectInfoProvider.GetProjectProgress(Project.ProjectID);
            Control.FieldControls["ProjectProgress"].Text = ProjectTaskInfoProvider.GenerateProgressHtml(progress, true);

        }
        else
        {
            Project.ProjectGroupID = groupId;

            Control.FieldsToHide.Add("ProjectProgress");
            Control.FieldsToHide.Add("ProjectAttachments");
        }
    }


    /// <summary>
    /// Updates curent Project info after uploader events (upload and delete).
    /// </summary>
    private void TouchProjectInfo(object sender, EventArgs e)
    {
        Project.Update();
    }

    #endregion
}
