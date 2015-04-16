using System;

using CMS.Core;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.DataEngine;

[UIElement(ModuleName.ONLINEMARKETING, "MyContacts")]
public partial class CMSModules_ContactManagement_Pages_Tools_PendingContacts_MyPendingContacts_PendingContacts : CMSContentManagementPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        int siteID = QueryHelper.GetInteger("siteID", SiteContext.CurrentSiteID);

        bool authorizedForSiteContacts = CurrentUser.IsAuthorizedPerResource(ModuleName.CONTACTMANAGEMENT, "ReadContacts");
        bool authorizedForGlobalContacts = CurrentUser.IsAuthorizedPerResource(ModuleName.CONTACTMANAGEMENT, "ReadGlobalContacts") && SettingsKeyInfoProvider.GetBoolValue(SiteContext.CurrentSiteName + ".CMSCMGlobalContacts");

        // Check UIProfile
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerUIElement("cms.onlinemarketing", "MyContacts", SiteContext.CurrentSiteName))
        {
            RedirectToUIElementAccessDenied("cms.onlinemarketing", "MyContacts");
        }

        // Init site filter when user is authorized for global and site contacts
        if (authorizedForGlobalContacts && authorizedForSiteContacts)
        {
            CurrentMaster.DisplaySiteSelectorPanel = true;

            // Set site selector
            if (!URLHelper.IsPostback())
            {
                siteOrGlobalSelector.SiteID = siteID;
            }

            listContacts.SiteID = siteOrGlobalSelector.SiteID;

        }
        else if (authorizedForSiteContacts)
        {
            // User is authorized only for site contacts so set current site id
            listContacts.SiteID = siteID;
        }
        else if (authorizedForGlobalContacts)
        {
            // User can read only global contacts
            listContacts.SiteID = UniSelector.US_GLOBAL_RECORD;
        }
        else
        {
            // User has no permissions
            RedirectToAccessDenied(ModuleName.CONTACTMANAGEMENT, "ReadContacts|ReadGlobalContacts");
        }

        // Add Refresh button
        PageTitle.HeaderActions.AddAction(new HeaderAction()
        {
            Text = GetString("general.Refresh"),
            RedirectUrl = "PendingContacts.aspx?siteid=" + listContacts.SiteID
        });
    }
}
