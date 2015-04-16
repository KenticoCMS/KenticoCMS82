using System;
using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.OnlineMarketing;
using CMS.Base;
using CMS.UIControls;

// Help
[Help("activity_new", "helptopic")]
[Security(Resource = ModuleName.CONTACTMANAGEMENT, Permission = "ReadActivities")]
public partial class CMSModules_ContactManagement_Pages_Tools_Activities_Activity_New : CMSContactManagementActivitiesPage
{
    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        int siteId = QueryHelper.GetInteger("siteid", 0);
        int contactId = QueryHelper.GetInteger("contactid", 0);

        // Init breadcrumbs
        var listBreadCrumb = new BreadcrumbItem { 
            Text = GetString("om.activity.list"), 
            RedirectUrl = "~/CMSModules/ContactManagement/Pages/Tools/Activities/Activity/List.aspx" 
        };
        var newItemBreadCrumb = new BreadcrumbItem { 
            Text = GetString("om.activity.newcustom") 
        };

        if (contactId > 0)
        {
            // New custom activity page was opened from pages of edited contact
            listBreadCrumb.RedirectUrl = "~/CMSModules/ContactManagement/Pages/Tools/Contact/Tab_Activities.aspx?contactId=" + contactId;
        }

        listBreadCrumb.RedirectUrl = AddSiteQuery(listBreadCrumb.RedirectUrl, siteId);
        PageBreadcrumbs.AddBreadcrumb(listBreadCrumb);
        PageBreadcrumbs.AddBreadcrumb(newItemBreadCrumb);
    }

    #endregion
}