using System;

using CMS.Core;
using CMS.Helpers;
using CMS.OnlineMarketing;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;
using CMS.WebAnalytics;

[Action(0, "om.activitytype.new", "New.aspx")]
[Security(Resource = ModuleName.CONTACTMANAGEMENT, Permission = "ReadActivities")]
[Security(Resource = ModuleName.ONLINEMARKETING, UIElements = "ActivityTypesList;Activities")]
public partial class CMSModules_ContactManagement_Pages_Tools_Activities_ActivityType_List : CMSContactManagementActivitiesPage
{
    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        string currSiteName = null;
        int currSiteId = 0;

        // Get current site ID/name
        if (ContactHelper.IsSiteManager)
        {
            currSiteId = SiteID;
            currSiteName = SiteInfoProvider.GetSiteName(currSiteId);
        }
        else
        {
            currSiteName = SiteContext.CurrentSiteName;
            currSiteId = SiteContext.CurrentSiteID;
        }

        bool globalObjectsSelected = (currSiteId == UniSelector.US_GLOBAL_RECORD);
        bool allSitesSelected = (currSiteId == UniSelector.US_ALL_RECORDS);

        // Show warning if activity logging is disabled (do not show anything if global objects or all sites is selected)
        ucDisabledModule.SettingsKeys = "CMSEnableOnlineMarketing";
        ucDisabledModule.ParentPanel = pnlDis;

        pnlDis.Visible = !globalObjectsSelected && !allSitesSelected && !ActivitySettingsHelper.ActivitiesEnabledAndModuleLoaded(currSiteName);

        if (CurrentMaster.HeaderActions.ActionsList.Count > 0)
        {
            CurrentMaster.HeaderActions.ActionsList[0].RedirectUrl += RequestContext.CurrentQueryString;
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        CurrentMaster.HeaderActions.Enabled = MembershipContext.AuthenticatedUser.IsAuthorizedPerResource(ModuleName.CONTACTMANAGEMENT, "ManageActivities");
    }

    #endregion
}