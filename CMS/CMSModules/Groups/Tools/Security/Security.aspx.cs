using System;

using CMS.Helpers;
using CMS.UIControls;

public partial class CMSModules_Groups_Tools_Security_Security : CMSGroupPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        groupSecurity.GroupID = QueryHelper.GetInteger("groupid", 0);
        groupSecurity.OnCheckPermissions += groupSecurity_OnCheckPermissions;
    }


    private void groupSecurity_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        CheckPermissions(groupSecurity.GroupID, CMSAdminControl.PERMISSION_MANAGE);
    }
}