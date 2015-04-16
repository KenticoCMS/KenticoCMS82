using System;

using CMS.Core;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;


[UIElement(ModuleName.WEBANALYTICS, "Dashboard")]
public partial class CMSModules_WebAnalytics_Tools_Dashboard : DashboardPage
{
    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        ucDashboard.ResourceName = "CMS.WebAnalytics";
        ucDashboard.ElementName = "Dashboard";
        ucDashboard.PortalPageInstance = this as PortalPage;
        ucDashboard.TagsLiteral = ltlTags;
        ucDashboard.DashboardSiteName = SiteContext.CurrentSiteName;

        ucDashboard.SetupDashboard();
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Keep current user
        var cu = MembershipContext.AuthenticatedUser;

        // Check permissions
        if ((MembershipContext.AuthenticatedUser == null) || !cu.IsAuthorizedPerResource("CMS.WebAnalytics", "Read"))
        {
            CMSPage.RedirectToAccessDenied("CMS.WebAnalytics", "Read");
        }

        // Check ui elements
        if (!cu.IsAuthorizedPerUIElement("CMS.WebAnalytics", "Dashboard"))
        {
            CMSPage.RedirectToUIElementAccessDenied("CMS.WebAnalytics", "Dashboard");
        }
    }
}