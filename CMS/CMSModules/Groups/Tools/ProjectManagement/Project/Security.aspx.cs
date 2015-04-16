using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Helpers;
using CMS.UIControls;

public partial class CMSModules_Groups_Tools_ProjectManagement_Project_Security : CMSGroupProjectManagementPage
{
    #region "Variables"

    private int groupId = 0;

    #endregion


    #region "Methods" 

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        int projectId = QueryHelper.GetInteger("projectid", 0);
        groupId = QueryHelper.GetInteger("groupid", 0);

        security.ProjectID = projectId;
        security.OnCheckPermissions += new CMSAdminControl.CheckPermissionsEventHandler(security_OnCheckPermissions);
    }


    /// <summary>
    /// Check permissions handler.
    /// </summary>
    /// <param name="permissionType">Type of a permission to check</param>
    /// <param name="sender">Sender</param>
    protected void security_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        // Check permissions
        CheckPermissions(groupId, permissionType);
    }

    #endregion
}