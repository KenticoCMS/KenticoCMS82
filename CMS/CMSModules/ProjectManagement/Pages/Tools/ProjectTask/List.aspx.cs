using System;

using CMS.Helpers;
using CMS.ProjectManagement;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.ExtendedControls.ActionsConfig;

public partial class CMSModules_ProjectManagement_Pages_Tools_ProjectTask_List : CMSProjectManagementTasksPage
{
    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        int projectId = QueryHelper.GetInteger("projectid", 0);

        ProjectInfo pi = ProjectInfoProvider.GetProjectInfo(projectId);
        if (pi != null)
        {
            if (pi.ProjectSiteID != SiteContext.CurrentSiteID)
            {
                RedirectToInformation(GetString("general.notassigned"));
            }
        }

        if (projectId > 0)
        {
            // Display project tasks
            listElem.ProjectID = projectId;
            listElem.OrderByType = ProjectTaskOrderByEnum.ProjectOrder;
            listElem.OrderBy = "TaskPriorityOrder ASC, ProjectTaskDeadline DESC";
        }
        else
        {
            // Display all task (project + ad-hoc tasks)
            listElem.Grid.GridName = "~/CMSModules/ProjectManagement/Controls/UI/ProjectTask/ListAll.xml";
            listElem.OrderBy = "ProjectTaskDisplayName";
            listElem.SiteName = SiteContext.CurrentSiteName;
        }

        HeaderAction action = new HeaderAction();
        action.Text = (projectId > 0) ? GetString("pm.projecttask.new") : GetString("pm.projecttask.newpersonal");
        action.RedirectUrl = ResolveUrl("Edit.aspx" + ((projectId > 0) ? ("?projectid=" + projectId) : ""));
        CurrentMaster.HeaderActions.AddAction(action);               
    }

    #endregion
}