using System;

using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;
using CMS.Modules;

[UIElement("CMS.Notifications", "MyProfile.Notifications")]
public partial class CMSModules_Notifications_MyDesk_MyProfile_MyProfile_Notifications : CMSContentManagementPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        userNotificationsElem.UserId = MembershipContext.AuthenticatedUser.UserID;
        userNotificationsElem.SiteID = SiteContext.CurrentSiteID;
    }
}