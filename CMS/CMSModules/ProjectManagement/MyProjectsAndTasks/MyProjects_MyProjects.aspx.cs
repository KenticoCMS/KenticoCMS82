using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Membership;
using CMS.ProjectManagement;
using CMS.SiteProvider;
using CMS.UIControls;

[UIElement("CMS.ProjectManagement", "MyProjects_1")]
public partial class CMSModules_ProjectManagement_MyProjectsAndTasks_MyProjects_MyProjects : CMSContentManagementPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        ucProjectList.Grid.PageSize = "25";
        ucProjectList.Grid.FilterLimit = 25;

        ucProjectList.Grid.GridName = "~/CMSModules/ProjectManagement/Controls/UI/Project/MyDeskProjects.xml";
        ucProjectList.EditPageURL = ResolveUrl("~/CMSModules/ProjectManagement/MyProjectsAndTasks/MyProjects_ProjectEdit.aspx");
        ucProjectList.BuildCondition += new CMSModules_ProjectManagement_Controls_UI_Project_List.BuildConditionEvent(ucProjectList_BuildCondition);
    }


    /// <summary>
    /// Builds where condition.
    /// </summary>
    private string ucProjectList_BuildCondition(object sender, string whereCondition)
    {
        // Security condition
        return ProjectInfoProvider.CombineSecurityWhereCondition(whereCondition, MembershipContext.AuthenticatedUser, SiteContext.CurrentSiteName);
    }
}