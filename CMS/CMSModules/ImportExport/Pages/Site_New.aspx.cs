using System;

using CMS.Core;
using CMS.PortalEngine;
using CMS.UIControls;

[UIElementAttribute(ModuleName.CMS, "NewSite")]
public partial class CMSModules_ImportExport_Pages_Site_New : CMSImportExportPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Set breadcrumbs
        SetBreadcrumb(0, GetString("general.sites"), UIContextHelper.GetElementUrl(ModuleName.CMS, "sites", false), null, null);
        SetBreadcrumb(1, GetString("Site_Edit.NewSite"), string.Empty, null, null);

        // Set page title
        PageTitle.TitleText = GetString("Site_Edit.NewSite");
        PageTitle.HeadingLevel = 3;
        PageTitle.ShowCloseButton = false;
        PageTitle.ShowFullScreenButton = false;
    }
}