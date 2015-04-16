using System;

using CMS.Core;
using CMS.Helpers;
using CMS.Membership;
using CMS.UIControls;

[UIElement(ModuleName.CMS, "Dashboard")]
public partial class CMSModules_Admin_AdministrationDashboard : DashboardPage
{
    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        // Get current user info
        var currentUser = MembershipContext.AuthenticatedUser;

        // Check whether user is global admin
        if (currentUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin))
        {
            ucDashboard.PortalPageInstance = this as PortalPage;
            ucDashboard.TagsLiteral = ltlTags;

            ucDashboard.SetupDashboard();
        }
        // For non-global admin redirect to access denied
        else
        {
            URLHelper.Redirect(UIHelper.GetAccessDeniedUrl("accessdeniedtopage.globaladminrequired"));
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
    }
}