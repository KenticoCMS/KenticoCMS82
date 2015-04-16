using System;
using System.Linq;

using CMS.Core;
using CMS.Membership;
using CMS.OnlineMarketing;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.ExtendedControls.ActionsConfig;


public partial class CMSModules_ContactManagement_Pages_Tools_PendingContacts_List : CMSAutomationPage
{
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Check UIProfile
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerUIElement("CMS.OnlineMarketing", new string[] { "ContactsFrameset", "PendingContacts" }, SiteContext.CurrentSiteName))
        {
            RedirectToUIElementAccessDenied("CMS.OnlineMarketing", "ContactsFrameset;PendingContacts");
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsSiteManager)
        {
            // Init site filter when user is authorized for global and site contacts
            if (AuthorizedForGlobalContacts && AuthorizedForSiteContacts)
            {
                CurrentMaster.DisplaySiteSelectorPanel = true;

                // Set site selector
                if (!URLHelper.IsPostback())
                {
                    siteOrGlobalSelector.SiteID = QueryHelper.GetInteger("siteid", SiteContext.CurrentSiteID);
                }

                listContacts.SiteID = siteOrGlobalSelector.SiteID;
            }
            else if (AuthorizedForSiteContacts)
            {
                // User is authorized only for site contacts so set current site id
                listContacts.SiteID = SiteContext.CurrentSiteID;
            }
            else if (AuthorizedForGlobalContacts)
            {
                // User can read only global contacts
                listContacts.SiteID = UniSelector.US_GLOBAL_RECORD;
            }
            else
            {
                // User has no permissions
                RedirectToAccessDenied(ModuleName.CONTACTMANAGEMENT, "ReadContacts");
            }
        }
        else
        {
            siteOrGlobalSelector.Visible = false;
            listContacts.SiteID = SiteID;
        }

        // Add Refresh action button
        AddHeaderAction(new HeaderAction()
        {
            Text = GetString("general.Refresh"),
            RedirectUrl = "List.aspx?siteid=" + listContacts.SiteID + (IsSiteManager ? "&issitemanager=1" : String.Empty)
        });
    }
}
