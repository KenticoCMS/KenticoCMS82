using System;

using CMS.Core;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;

[UIElement(ModuleName.ONLINEMARKETING, "OMDashBoard")]
public partial class CMSModules_OnlineMarketing_Pages_Dashboard : DashboardPage
{
    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        ucDashboard.ResourceName = "CMS.OnlineMarketing";
        ucDashboard.ElementName = "OMDashBoard";
        ucDashboard.PortalPageInstance = this as PortalPage;
        ucDashboard.TagsLiteral = ltlTags;
        ucDashboard.DashboardSiteName = SiteContext.CurrentSiteName;

        ucDashboard.SetupDashboard();
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        var cu = MembershipContext.AuthenticatedUser;

        // Check permissions
        if ((cu == null) || !cu.IsAuthorizedPerResource("CMS.OnlineMarketing", "Read"))
        {
            CMSPage.RedirectToAccessDenied("CMS.OnlineMarketing", "Read");
        }

        // Check UIProfile
        if (!cu.IsAuthorizedPerUIElement("CMS.OnlineMarketing", "OMDashBoard"))
        {
            CMSPage.RedirectToUIElementAccessDenied("CMS.OnlineMarketing", "OMDashBoard");
        }

        // Register script for unimenu button selection
        CMSDeskPage.AddMenuButtonSelectScript(this, "OMDashBoard", null, "menu");
    }
}