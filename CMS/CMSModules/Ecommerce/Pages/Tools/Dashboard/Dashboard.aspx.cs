using System;

using CMS.Core;
using CMS.SiteProvider;
using CMS.UIControls;

[UIElement(ModuleName.ECOMMERCE, "EcommerceDashboard")]
public partial class CMSModules_Ecommerce_Pages_Tools_Dashboard_Dashboard : DashboardPage
{
    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        ucDashboard.ResourceName = "CMS.Ecommerce";
        ucDashboard.ElementName = "EcommerceDashboard";
        ucDashboard.PortalPageInstance = this;
        ucDashboard.TagsLiteral = ltlTags;
        ucDashboard.DashboardSiteName = SiteContext.CurrentSiteName;

        ucDashboard.SetupDashboard();
    }


    protected void Page_Load(object sender, EventArgs e)
    {

    }
}