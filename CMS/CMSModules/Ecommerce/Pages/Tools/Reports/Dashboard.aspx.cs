using System;

using CMS.Core;
using CMS.SiteProvider;
using CMS.UIControls;

[UIElement(ModuleName.ECOMMERCE, "ReportsDashboard")]
public partial class CMSModules_Ecommerce_Pages_Tools_Reports_Dashboard : DashboardPage
{
    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        ucDashboard.ResourceName = "CMS.Ecommerce";
        ucDashboard.ElementName = "ReportsDashboard";
        ucDashboard.PortalPageInstance = this;
        ucDashboard.TagsLiteral = ltlTags;
        ucDashboard.DashboardSiteName = SiteContext.CurrentSiteName;

        ucDashboard.SetupDashboard();
    }


    protected void Page_Load(object sender, EventArgs e)
    {

    }
}