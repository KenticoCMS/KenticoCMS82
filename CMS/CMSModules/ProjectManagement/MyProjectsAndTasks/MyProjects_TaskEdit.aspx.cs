using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Helpers;
using CMS.Membership;
using CMS.UIControls;

[UIElement("CMS.ProjectManagement", "MyProjects")]
public partial class CMSModules_ProjectManagement_MyProjectsAndTasks_MyProjects_TaskEdit : CMSContentManagementPage
{
    #region "Variables"

    private bool taskAssignedToMe = false;

    #endregion


    #region "Methods"

    protected override void OnInit(EventArgs e)
    {
        taskAssignedToMe = QueryHelper.GetBoolean("mytasks", false);
        editElem.ItemID = QueryHelper.GetInteger("projecttaskid", 0);

        var currentUser = MembershipContext.AuthenticatedUser;

        // Check whether task can be displayed (depends on display mode)
        if (editElem.ProjectTaskObj != null)
        {
            if (taskAssignedToMe)
            {
                if (editElem.ProjectTaskObj.ProjectTaskAssignedToUserID != currentUser.UserID)
                {
                    editElem.ItemID = 0;
                    editElem.ProjectTaskObj = null;
                }
            }
            else
            {
                if (editElem.ProjectTaskObj.ProjectTaskOwnerID != currentUser.UserID)
                {
                    editElem.ItemID = 0;
                    editElem.ProjectTaskObj = null;
                }
            }
        }
        editElem.OnSaved += new EventHandler(editElem_OnSaved);
        base.OnInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Keep title object
        PageTitle title = PageTitle;

        // Title
        string name = GetString("pm.projecttask.newpersonal");
        // Set breadcrumb to exisiting task
        if (editElem.ProjectTaskObj != null)
        {
            name = editElem.ProjectTaskObj.ProjectTaskDisplayName;
        }

        // Initialize breadcrumbs
        PageBreadcrumbs.Items.Add(new BreadcrumbItem()
        {
            Text = GetString("pm.projecttask"),
            RedirectUrl = taskAssignedToMe ? ResolveUrl("~/CMSModules/ProjectManagement/MyProjectsAndTasks/MyProjects_TasksAssignedToMe.aspx") : ResolveUrl("~/CMSModules/ProjectManagement/MyProjectsAndTasks/MyProjects_TasksOwnedByMe.aspx"),
        });
        PageBreadcrumbs.Items.Add(new BreadcrumbItem()
        {
            Text = HTMLHelper.HTMLEncode(name),
        });
    }


    /// <summary>
    /// OnSaved event handler.
    /// </summary>
    private void editElem_OnSaved(object sender, EventArgs e)
    {
        if (editElem.ItemID > 0)
        {
            URLHelper.Redirect("~/CMSModules/ProjectManagement/MyProjectsAndTasks/MyProjects_TaskEdit.aspx?projecttaskid=" + editElem.ItemID + "&saved=1&mytasks=" + taskAssignedToMe.ToString());
        }
    }

    #endregion
}