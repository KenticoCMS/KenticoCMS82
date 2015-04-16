using System;

using CMS.Helpers;
using CMS.Membership;
using CMS.ProjectManagement;
using CMS.UIControls;

[UIElement("CMS.ProjectManagement", "MyProjects_1")]
public partial class CMSModules_ProjectManagement_MyProjectsAndTasks_MyProjects_ProjectEdit : CMSContentManagementPage
{
    #region "Variables"

    private int projectId;

    #endregion


    #region "Methods"

    protected override void OnInit(EventArgs e)
    {
        // Get project id from query string
        projectId = QueryHelper.GetInteger("projectid", 0);

        // Check whether user can see required project
        if (!ProjectInfoProvider.IsAuthorizedPerProject(projectId, ProjectManagementPermissionType.READ, MembershipContext.AuthenticatedUser))
        {
            RedirectToAccessDenied("CMS.ProjectManagement", "Manage");
        }

        editElem.ProjectID = QueryHelper.GetInteger("projectid", 0);

        base.OnInit(e);
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Create breadcrumbs
        CreateBreadcrumbs();
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Keep title object
        PageTitle title = PageTitle;

        // Title
        // Handle check permissions
        editElem.OnCheckPermissions += editElem_OnCheckPermissions;
    }


    /// <summary>
    /// Creates breadcrumbs
    /// </summary>
    private void CreateBreadcrumbs()
    {
        // Set breadcrumb to existing task
        string name = String.Empty;
        if (editElem.ProjectObj != null)
        {
            name = editElem.ProjectObj.ProjectDisplayName;
        }

        // Initialize breadcrumbs
        PageBreadcrumbs.Items.Add(new BreadcrumbItem
        {
            Text = GetString("pm.project.list"),
            RedirectUrl = ResolveUrl("~/CMSModules/ProjectManagement/MyProjectsAndTasks/MyProjects_MyProjects.aspx"),
        });
        PageBreadcrumbs.Items.Add(new BreadcrumbItem
        {
            Text = HTMLHelper.HTMLEncode(name),
        });
    }


    private void editElem_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        // Keep current user info
        var currentUser = MembershipContext.AuthenticatedUser;

        // Check whether user has manage permission or is project owner to edit project
        if (!currentUser.IsAuthorizedPerResource("CMS.ProjectManagement", "Manage") && (editElem.ProjectObj != null) && (editElem.ProjectObj.ProjectOwner != currentUser.UserID))
        {
            sender.StopProcessing = true;
            RedirectToAccessDenied("CMS.ProjectManagement", "Manage");
        }
    }

    #endregion
}