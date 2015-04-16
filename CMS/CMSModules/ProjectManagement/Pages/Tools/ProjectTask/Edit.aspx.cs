using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.Helpers;
using CMS.ProjectManagement;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_ProjectManagement_Pages_Tools_ProjectTask_Edit : CMSProjectManagementTasksPage
{
    #region "Variables"

    private int projectId = 0;
    private int assigneeId = 0;
    private int ownerId = 0;

    #endregion


    #region "Methods"

    protected override void OnInit(EventArgs e)
    {
        // Get the IDs from query string
        projectId = QueryHelper.GetInteger("projectid", 0);

        ProjectInfo pi = ProjectInfoProvider.GetProjectInfo(projectId);
        if (pi != null)
        {
            if (pi.ProjectSiteID != SiteContext.CurrentSiteID)
            {
                RedirectToInformation(GetString("general.notassigned"));
            }
        }

        assigneeId = QueryHelper.GetInteger("assigneeid", 0);
        ownerId = QueryHelper.GetInteger("ownerid", 0);

        editElem.ProjectID = projectId;
        editElem.ItemID = QueryHelper.GetInteger("projecttaskid", 0);
        editElem.ProjectTaskAssigneeID = assigneeId;
        editElem.ProjectTaskOwnerID = ownerId;
        editElem.OnSaved += new EventHandler(editElem_OnSaved);

        base.OnInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        PageTitle title = PageTitle;

        // Prepare the header
        string name = "";
        if (editElem.ProjectTaskObj != null)
        {
            name = editElem.ProjectTaskObj.ProjectTaskDisplayName;
        }
        else
        {
            name = (projectId > 0) ? GetString("pm.projecttask.new") : GetString("pm.projecttask.newpersonal");
        }

        // Initialize breadcrumbs
        title.Breadcrumbs.Items.Add(new BreadcrumbItem()
        {
            Text = (projectId > 0) ? GetString("pm.projecttask.list") : GetString("pm.projecttask"),
            RedirectUrl = ResolveUrl("~/CMSModules/ProjectManagement/Pages/Tools/ProjectTask/List.aspx?projectid=" + projectId),
        });
        title.Breadcrumbs.Items.Add(new BreadcrumbItem()
        {
            Text = HTMLHelper.HTMLEncode(name),
        });

        if ((projectId > 0)
            && (editElem.ProjectTaskObj != null))
        {
            ProjectTaskInfo task = editElem.ProjectTaskObj;
            HeaderAction action = new HeaderAction();
            action.Text = GetString("pm.projecttask.new");
            action.RedirectUrl = ResolveUrl(String.Format("Edit.aspx?projectid={0:D}&assigneeid={1:D}&ownerid={2:D}", projectId, task.ProjectTaskAssignedToUserID, task.ProjectTaskOwnerID));
            CurrentMaster.HeaderActions.AddAction(action);
        }
    }


    protected void editElem_OnSaved(object sender, EventArgs e)
    {
        if (editElem.ItemID > 0)
        {
            URLHelper.Redirect("~/CMSModules/ProjectManagement/Pages/Tools/ProjectTask/Edit.aspx?projecttaskid=" + editElem.ItemID + "&projectid=" + projectId + "&saved=1");
        }
    }

    #endregion
}