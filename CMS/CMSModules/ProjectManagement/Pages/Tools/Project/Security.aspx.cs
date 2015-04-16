using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Helpers;
using CMS.ProjectManagement;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_ProjectManagement_Pages_Tools_Project_Security : CMSProjectManagementProjectsPage
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

        security.ProjectID = projectId;
    }

    #endregion
}