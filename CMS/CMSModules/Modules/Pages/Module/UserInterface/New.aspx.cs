using System;

using CMS.Core;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.UIControls;

[UIElement(ModuleName.CMS, "Modules.UserInterface.New")]
public partial class CMSModules_Modules_Pages_Module_UserInterface_New : GlobalAdminPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        PageTitle.TitleText = GetString("resource.ui.title");
        int parentId = QueryHelper.GetInteger("parentId", 0);
        int moduleId = QueryHelper.GetInteger("moduleid", 0);

        if (parentId > 0)
        {
            editElem.ParentID = parentId;
            editElem.ResourceID = moduleId;

            // Init breadcrumbs
            PageBreadcrumbs.AddBreadcrumb(new BreadcrumbItem()
            {
                Text = GetString("resource.ui.element"),
                RedirectUrl = URLHelper.AppendQuery(UIContextHelper.GetElementUrl(ModuleName.CMS, "Modules.UserInterface.Edit", false, parentId), "elementId=" + parentId + "&moduleid=" + moduleId + "&objectParentId=" + moduleId)
            });

            PageBreadcrumbs.AddBreadcrumb(new BreadcrumbItem()
            {
                Text = GetString("resource.ui.newelement"),
            });
        }
        else
        {
            editElem.Visible = false;
            lblInfo.Visible = true;
            lblInfo.Text = GetString("resource.ui.rootelement");
        }
    }
}