using System;
using System.Linq;

using CMS.Core;
using CMS.Helpers;
using CMS.OnlineMarketing;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;
using CMS.WebAnalytics;
using CMS.ExtendedControls.ActionsConfig;

[Security(Resource = ModuleName.CONTACTMANAGEMENT, Permission = "ReadActivities")]
[UIElement(ModuleName.ONLINEMARKETING, "Activities")]
public partial class CMSModules_ContactManagement_Pages_Tools_Activities_Activity_List : CMSContactManagementActivitiesPage
{
    #region "Variables"

    private int currSiteId;

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        string currSiteName = null;

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
        ucDisabledModule.SettingsKeys = "CMSEnableOnlineMarketing;CMSCMActivitiesEnabled";
        ucDisabledModule.ParentPanel = pnlDis;

        pnlDis.Visible = !globalObjectsSelected && !allSitesSelected && !ActivitySettingsHelper.ActivitiesEnabledAndModuleLoaded(currSiteName);

        // Initialize list and filter controls
        listElem.SiteID = currSiteId;

        // Show site name column if activities of all sites are displayed
        listElem.ShowSiteNameColumn = allSitesSelected || globalObjectsSelected;
        listElem.ShowIPAddressColumn = ActivitySettingsHelper.IPLoggingEnabled(currSiteName);
        listElem.OrderBy = "ActivityCreated DESC";

        if (QueryHelper.GetInteger("saved", 0) == 1)
        {
            ShowChangesSaved();
        }

        // Set header actions (add button)
        string url = ResolveUrl("New.aspx?siteId=" + currSiteId);
        if (IsSiteManager)
        {
            url = URLHelper.AddParameterToUrl(url, "isSiteManager", "1");
        }
        hdrActions.AddAction(new HeaderAction()
        {
            Text = GetString("om.activity.newcustom"),
            RedirectUrl = url
        });
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Disable manual creation of activity if no custom activity type is available
        var activityType = ActivityTypeInfoProvider.GetActivityTypes()
                                                   .WhereEquals("ActivityTypeIsCustom", 1)
                                                   .WhereEquals("ActivityTypeEnabled", 1)
                                                   .WhereEquals("ActivityTypeManualCreationAllowed", 1)
                                                   .TopN(1)
                                                   .Column("ActivityTypeID")
                                                   .FirstOrDefault();

        bool aCustomActivityExists = (activityType != null);

        // Disable actions for unauthorized users
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource(ModuleName.CONTACTMANAGEMENT, "ManageActivities"))
        {
            hdrActions.Enabled = false;
        }
        // Allow new button only if custom activity exists
        else if (!aCustomActivityExists)
        {
            lblWarnNew.ResourceString = "om.activities.nocustomactivity";
            hdrActions.Enabled = false;
            lblWarnNew.Visible = true;
        }
        // Allow new button only for particular sites
        else if (currSiteId <= 0)
        {
            lblWarnNew.ResourceString = "om.choosesite";
            hdrActions.Enabled = false;
            lblWarnNew.Visible = true;
        }
    }

    #endregion
}