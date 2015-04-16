using System;
using System.Data;
using System.Web;
using System.Web.UI;

using CMS.Community;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;

public partial class CMSModules_Groups_Tools_Roles_Role_Edit_Users : CMSGroupRolesPage
{
    private int roleId = 0;
    private int groupId = 0;


    protected void Page_Load(object sender, EventArgs e)
    {
        roleUsersElem.IsLiveSite = false;

        roleId = QueryHelper.GetInteger("roleid", 0);
        groupId = QueryHelper.GetInteger("groupid", 0);

        // Check whether group exists
        GroupInfo gi = GroupInfoProvider.GetGroupInfo(groupId);
        if (gi != null)
        {
            //Check whether selected role is in selected group
            RoleInfo ri = RoleInfoProvider.GetRoleInfo(roleId);
            if ((ri != null) && (ri.RoleGroupID == gi.GroupID))
            {
                roleUsersElem.RoleID = roleId;
                roleUsersElem.GroupID = groupId;
                roleUsersElem.OnCheckPermissions += roleUsersElem_OnCheckPermissions;
            }
        }
        else
        {
            roleUsersElem.Visible = false;
        }
    }


    protected void roleUsersElem_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        // Check permissions
        CheckPermissions(groupId, CMSAdminControl.PERMISSION_MANAGE);
    }
}