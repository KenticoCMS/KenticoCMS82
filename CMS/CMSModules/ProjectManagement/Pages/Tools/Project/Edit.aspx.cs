using System;

using CMS.Helpers;
using CMS.ProjectManagement;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_ProjectManagement_Pages_Tools_Project_Edit : CMSProjectManagementProjectsPage
{
    #region "Variables"

    private int projectId;

    #endregion


    #region "Methods"

    protected override void OnInit(EventArgs e)
    {
        // Get the ID from query string
        projectId = QueryHelper.GetInteger("projectid", 0);

        ProjectInfo pi = ProjectInfoProvider.GetProjectInfo(projectId);
        if (pi != null)
        {
            if (pi.ProjectSiteID != SiteContext.CurrentSiteID)
            {
                RedirectToInformation(GetString("general.notassigned"));
            }
        }

        editElem.ProjectID = projectId;
        editElem.OnSaved += editElem_OnSaved;

        base.OnInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        PageTitle title = PageTitle;

        // Prepare the header
        string name = "";
        if (editElem.ProjectObj == null)
        {
            name = GetString("pm.project.new");
            // Initialize breadcrumbs
            title.Breadcrumbs.Items.Add(new BreadcrumbItem()
            {
                Text = GetString("pm.project.list"),
                RedirectUrl = ResolveUrl("~/CMSModules/ProjectManagement/Pages/Tools/Project/List.aspx?projectid=" + projectId),
            });
            title.Breadcrumbs.Items.Add(new BreadcrumbItem()
            {
                Text = HTMLHelper.HTMLEncode(name),
            });
        }
    }


    protected void editElem_OnSaved(object sender, EventArgs e)
    {
        if (projectId == 0)
        {
            // New item
            URLHelper.Redirect("~/CMSModules/ProjectManagement/Pages/Tools/Project/Frameset.aspx?projectid=" + editElem.ItemID);
        }
    }

    #endregion
}