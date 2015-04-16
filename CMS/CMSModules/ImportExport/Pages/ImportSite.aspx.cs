using System;

using CMS.Core;
using CMS.PortalEngine;
using CMS.UIControls;

[UIElementAttribute(ModuleName.CMS, "ImportSiteOrObjects")]
public partial class CMSModules_ImportExport_Pages_ImportSite : CMSImportExportPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Initializes PageTitle
        titleElem.Breadcrumbs.AddBreadcrumb(new BreadcrumbItem()
        {
            Text = GetString("general.sites"),
            RedirectUrl = UIContextHelper.GetElementUrl(ModuleName.CMS, "Sites", false)
        });

        titleElem.Breadcrumbs.AddBreadcrumb(new BreadcrumbItem()
        {
            Text = GetString("ImportSite.ImportSite")
        });

        titleElem.TitleText = GetString("ImportSite.Title");
    }
}