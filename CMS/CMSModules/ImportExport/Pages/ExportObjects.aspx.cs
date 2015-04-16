using System;

using CMS.Core;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.UIControls;

[UIElementAttribute(ModuleName.CMS, "Export")]
public partial class CMSModules_ImportExport_Pages_ExportObjects : CMSImportExportPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        int siteID = QueryHelper.GetInteger("siteID", 0);
        if (siteID > 0)
        {
            wzdExport.SiteId = siteID;
        }

        // Initialize PageTitle
        ptExportSiteSettings.Breadcrumbs.AddBreadcrumb(new BreadcrumbItem()
        {
            Text = GetString("general.sites"),
            RedirectUrl = UIContextHelper.GetElementUrl(ModuleName.CMS, "Sites", false),
            Target = "cmsdesktop"
        });
        ptExportSiteSettings.Breadcrumbs.AddBreadcrumb(new BreadcrumbItem()
        {
            Text = GetString("ExportSettings.ExportSiteSetings")
        });

        ptExportSiteSettings.TitleText = GetString("ExportSettings.ExportSiteSetings");
    }
}