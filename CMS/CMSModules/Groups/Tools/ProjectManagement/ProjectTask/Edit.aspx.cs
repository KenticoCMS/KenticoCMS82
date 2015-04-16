using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Helpers;
using CMS.UIControls;

public partial class CMSModules_Groups_Tools_ProjectManagement_ProjectTask_Edit : CMSGroupProjectManagementPage
{
    #region "Variables"

    private int projectId = 0;
    private int groupId = 0;

    #endregion


    #region "Methods"

    protected override void OnInit(EventArgs e)
    {
        // Get the ID from query string
        projectId = QueryHelper.GetInteger("projectid", 0);
        groupId = QueryHelper.GetInteger("groupid", 0);

        editElem.ProjectID = projectId;
        editElem.ItemID = QueryHelper.GetInteger("projecttaskid", 0);
        editElem.OnCheckPermissionsExtended += new CMSAdminControl.CheckPermissionsExtendedEventHandler(editElem_OnCheckPermissionsExtended);
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
            name = GetString("pm.projecttask.new");
        }

        // Initialize breadcrumbs
        title.Breadcrumbs.Items.Add(new BreadcrumbItem()
        {
            Text = GetString("pm.projecttask.list"),
            RedirectUrl = ResolveUrl("~/CMSModules/Groups/Tools/ProjectManagement/ProjectTask/List.aspx?projectid=" + projectId + "&groupid=" + groupId),
        });
        title.Breadcrumbs.Items.Add(new BreadcrumbItem()
        {
            Text = HTMLHelper.HTMLEncode(name),
        });
    }


    /// <summary>
    /// Check permissions handler.
    /// </summary>
    /// <param name="permissionType">Type of a permission to check</param>
    /// <param name="modulePermissionType">Name of the module permission</param>
    /// <param name="sender">Sender</param>
    protected void editElem_OnCheckPermissionsExtended(string permissionType, string modulePermissionType, CMSAdminControl sender)
    {
        // Check permissions
        CheckPermissions(groupId, modulePermissionType);
    }


    /// <summary>
    /// Handles the OnSaved event of the editElem control.
    /// </summary>
    /// <param name="sender">The source of the event</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data</param>
    protected void editElem_OnSaved(object sender, EventArgs e)
    {
        if (editElem.ItemID > 0)
        {
            URLHelper.Redirect("~/CMSModules/Groups/Tools/ProjectManagement/ProjectTask/Edit.aspx?projecttaskid=" + editElem.ItemID + "&projectid=" + projectId + "&groupid=" + groupId + "&saved=1");
        }
    }

    #endregion
}