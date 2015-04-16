using System;
using System.Linq;

using CMS.Core;
using CMS.Helpers;
using CMS.OnlineMarketing;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.WebAnalytics;

// Edited object
[EditedObject(ContactInfo.OBJECT_TYPE, "contactId")]
[Security(Resource = ModuleName.ONLINEMARKETING, UIElements = "EditContact;ContactActivities")]
public partial class CMSModules_ContactManagement_Pages_Tools_Contact_Tab_Activities : CMSContactManagementContactsPage
{
    /// <summary>
    /// PreInit event handler
    /// </summary>
    protected override void OnPreInit(EventArgs e)
    {
        RequiresDialog = false;
        base.OnPreInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (EditedObject != null)
        {
            ContactInfo ci = (ContactInfo)EditedObject;

            // Check permission
            CheckReadPermission(ci.ContactSiteID);

            bool isGlobal = (ci.ContactSiteID == 0);
            bool isMerged = (ci.ContactMergedWithContactID > 0);

            // Show warning if activity logging is disabled
            string siteName = SiteInfoProvider.GetSiteName(ci.ContactSiteID);

            ucDisabledModule.SettingsKeys = "CMSEnableOnlineMarketing;CMSCMActivitiesEnabled";
            ucDisabledModule.ParentPanel = pnlDis;

            pnlDis.Visible = !isGlobal && !ActivitySettingsHelper.ActivitiesEnabledAndModuleLoaded(siteName);

            // Show IP addresses if enabled
            listElem.ShowIPAddressColumn = ActivitySettingsHelper.IPLoggingEnabled(siteName);
            listElem.ShowSiteNameColumn = IsSiteManager && isGlobal;

            // Restrict WHERE condition for activities of current site (if not in site manager)
            if (!IsSiteManager)
            {
                listElem.SiteID = SiteContext.CurrentSiteID;
            }
            else
            {
                // Show all records in Site Manager
                listElem.SiteID = UniSelector.US_ALL_RECORDS;
            }

            listElem.ContactID = ci.ContactID;
            listElem.IsMergedContact = isMerged;
            listElem.IsGlobalContact = isGlobal;
            listElem.ShowContactNameColumn = isGlobal;
            listElem.ShowSiteNameColumn = IsSiteManager && isGlobal;
            listElem.ShowRemoveButton = !isMerged;
            listElem.OrderBy = "ActivityCreated DESC";

            // Init header action for new custom activities only if contact is not global, a custom activity type exists and user is authorized to manage activities
            if (!isGlobal && ActivitySettingsHelper.ActivitiesEnabledAndModuleLoaded(siteName) && MembershipContext.AuthenticatedUser.IsAuthorizedPerResource(ModuleName.CONTACTMANAGEMENT, "ManageActivities"))
            {
                // Disable manual creation of activity if no custom activity type is available
                var activityType = ActivityTypeInfoProvider.GetActivityTypes()
                                                   .WhereEquals("ActivityTypeIsCustom", 1)
                                                   .WhereEquals("ActivityTypeEnabled", 1)
                                                   .WhereEquals("ActivityTypeManualCreationAllowed", 1)
                                                   .TopN(1)
                                                   .Column("ActivityTypeID")
                                                   .FirstOrDefault();

                if (activityType != null)
                {
                    // Prepare target URL
                    string url = ResolveUrl(string.Format("~/CMSModules/ContactManagement/Pages/Tools/Activities/Activity/New.aspx?contactId={0}", ci.ContactID));
                    url = AddSiteQuery(url, ci.ContactSiteID);

                    // Init header action
                    HeaderAction action = new HeaderAction()
                    {
                        Text = GetString("om.activity.newcustom"),
                        RedirectUrl = url
                    };
                    CurrentMaster.HeaderActions.ActionsList.Add(action);
                }
            }

            if (!RequestHelper.IsPostBack() && QueryHelper.GetBoolean("saved", false))
            {
                // Display 'Save' message after new custom activity was created
                ShowChangesSaved();
            }
        }
    }
}