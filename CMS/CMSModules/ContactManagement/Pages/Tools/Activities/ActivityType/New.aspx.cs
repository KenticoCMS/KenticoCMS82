using System;

using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.OnlineMarketing;
using CMS.Base;
using CMS.UIControls;

// Breadcrumbs
[Breadcrumbs()]
[Breadcrumb(0, "om.activitytype.list", "~/CMSModules/ContactManagement/Pages/Tools/Activities/ActivityType/List.aspx", null)]
[Breadcrumb(1, "om.activitytype.new")]
// Help
[Help("activitytype_new", "helptopic")]
[Security(Resource = ModuleName.CONTACTMANAGEMENT, Permission = "ReadActivities")]
public partial class CMSModules_ContactManagement_Pages_Tools_Activities_ActivityType_New : CMSContactManagementActivitiesPage
{
    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        PageBreadcrumbs.Items[0].RedirectUrl = AddSiteQuery(PageBreadcrumbs.Items[0].RedirectUrl, QueryHelper.GetInteger("siteid", 0));
    }

    #endregion
}