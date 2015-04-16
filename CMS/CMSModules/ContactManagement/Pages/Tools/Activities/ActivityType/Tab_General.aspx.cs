using System;

using CMS.Core;
using CMS.OnlineMarketing;
using CMS.UIControls;
using CMS.WebAnalytics;

// Edited object
[EditedObject(ActivityTypeInfo.OBJECT_TYPE, "typeId")]

// Help
[Help("activitytype_edit", "helptopic")]
[Security(Resource = ModuleName.CONTACTMANAGEMENT, Permission = "ReadActivities")]
[Breadcrumb(0, "om.activitytype.list", "~/CMSModules/ContactManagement/Pages/Tools/Activities/ActivityType/List.aspx", null)]
[Breadcrumb(1, Text = "{%EditedObject.DisplayName%}")]
public partial class CMSModules_ContactManagement_Pages_Tools_Activities_ActivityType_Tab_General : CMSContactManagementActivitiesPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
    }
}