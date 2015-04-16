using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Membership;
using CMS.ProjectManagement;
using CMS.Base;
using CMS.UIControls;
using CMS.ExtendedControls;
using CMS.DataEngine;

[Action(0, "pm.projecttask.newpersonal", "~/CMSModules/ProjectManagement/MyProjectsAndTasks/MyProjects_TaskEdit.aspx?mytasks=1")]
[UIElement("CMS.ProjectManagement", "MyTasks")]
public partial class CMSModules_ProjectManagement_MyProjectsAndTasks_MyProjects_TasksAssignedToMe : CMSContentManagementPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Set not specified order by default
        ucTaskList.OrderByType = ProjectTaskOrderByEnum.NotSpecified;

        // Use user order
        ucTaskList.OrderByType = ProjectTaskOrderByEnum.UserOrder;

        // Default order by
        ucTaskList.Grid.OrderBy = "TaskPriorityOrder ASC,ProjectTaskDeadline DESC";

        // Grid name
        ucTaskList.Grid.GridName = "~/CMSModules/ProjectManagement/Controls/UI/ProjectTask/ListMyTasks.xml";

        // Edit page
        ucTaskList.EditPageURL = ResolveUrl("~/CMSModules/ProjectManagement/MyProjectsAndTasks/MyProjects_TaskEdit.aspx");

        // Handle where condition
        ucTaskList.BuildCondition += new CMSModules_ProjectManagement_Controls_UI_ProjectTask_List.BuildConditionEvent(ucTaskList_BuildCondition);

        // Set my tasks
        ucTaskList.EditPageParameters = "&mytasks=1";
    }


    /// <summary>
    /// Adds  specific conditions to the list where condition.
    /// </summary>
    private string ucTaskList_BuildCondition(object sender, string whereCondition)
    {
        var currentUser = MembershipContext.AuthenticatedUser;

        // Display onlyt task assigned to me
        whereCondition = SqlHelper.AddWhereCondition(whereCondition, "ProjectTaskAssignedToUserID = " + currentUser.UserID);

        // Add security condition - display only tasks which are assigned or owned by the current user or which are a part of a project where the current user is authorised to Read from
        whereCondition = SqlHelper.AddWhereCondition(whereCondition, ProjectTaskInfoProvider.CombineSecurityWhereCondition(whereCondition, currentUser, null));

        return whereCondition;
    }
}