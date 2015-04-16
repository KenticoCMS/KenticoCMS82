using System;

using CMS.Core;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;

[UIElement(ModuleName.CONTENT, "MyDeskDashBoardItem")]
public partial class CMSModules_MyDesk_Dashboard : DashboardPage
{
    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        ucDashboard.ResourceName = "CMS.Content";
        ucDashboard.ElementName = "MyDeskDashBoardItem";
        ucDashboard.PortalPageInstance = this as PortalPage;
        ucDashboard.TagsLiteral = ltlTags;
        ucDashboard.DashboardSiteName = SiteContext.CurrentSiteName;

        ucDashboard.SetupDashboard();
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if ((MembershipContext.AuthenticatedUser == null) || !MembershipContext.AuthenticatedUser.IsAuthorizedPerUIElement("CMS.Content", "MyDeskDashBoardItem"))
        {
            CMSPage.RedirectToUIElementAccessDenied("CMS.Content", "MyDeskDashBoardItem");
        }
    }
}