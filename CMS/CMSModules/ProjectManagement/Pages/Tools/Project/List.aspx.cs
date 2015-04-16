using System;

using CMS.Helpers;
using CMS.ProjectManagement;
using CMS.SiteProvider;
using CMS.UIControls;

[Action(0, "pm.project.new", "Edit.aspx")]
public partial class CMSModules_ProjectManagement_Pages_Tools_Project_List : CMSProjectManagementProjectsPage
{
    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        int projectID = QueryHelper.GetInteger("projectid", 0);
        ProjectInfo pi = ProjectInfoProvider.GetProjectInfo(projectID);
        if (pi != null)
        {
            if (pi.ProjectSiteID != SiteContext.CurrentSiteID)
            {
                RedirectToInformation(GetString("general.notassigned"));
            }
        }
    }

    #endregion
}