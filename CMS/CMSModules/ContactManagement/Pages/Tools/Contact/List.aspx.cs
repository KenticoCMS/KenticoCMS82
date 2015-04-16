using System;

using CMS.Core;
using CMS.ExtendedControls.ActionsConfig;
using CMS.Helpers;
using CMS.OnlineMarketing;
using CMS.SiteProvider;
using CMS.UIControls;

[UIElement(ModuleName.ONLINEMARKETING, "Contacts")]
public partial class CMSModules_ContactManagement_Pages_Tools_Contact_List : CMSContactManagementContactsPage
{
    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        SiteOrGlobalSelector.TargetObjectType = ContactInfo.OBJECT_TYPE;

        // In CMS Desk site ID is in control
        if (!IsSiteManager)
        {
            // Init site filter when user is authorized for global and site contacts
            if (AuthorizedForGlobalContacts && AuthorizedForSiteContacts)
            {
                CurrentMaster.DisplaySiteSelectorPanel = true;
                if (!RequestHelper.IsPostBack())
                {
                    SiteOrGlobalSelector.SiteID = QueryHelper.GetInteger("siteId", SiteContext.CurrentSiteID);
                }
                listElem.SiteID = SiteOrGlobalSelector.SiteID;
                listElem.WhereCondition = SiteOrGlobalSelector.GetWhereCondition();
            }
            // Authorized for site contacts only
            else if (AuthorizedForSiteContacts)
            {
                listElem.SiteID = SiteContext.CurrentSiteID;
                listElem.WhereCondition = "ContactSiteID = " + listElem.SiteID;
            }
            // Authorized for global contacts only
            else if (AuthorizedForGlobalContacts)
            {
                listElem.SiteID = UniSelector.US_GLOBAL_RECORD;
                listElem.WhereCondition = "ContactSiteID IS NULL";
            }
            // User is not authorized
            else
            {
                RedirectToAccessDenied(ModuleName.CONTACTMANAGEMENT, "ReadContacts");
            }
        }
        // In Site Manager "siteID" is in query string
        else
        {
            listElem.SiteID = SiteID;
            if (SiteID == UniSelector.US_GLOBAL_RECORD)
            {
                listElem.WhereCondition = "ContactSiteID IS NULL";
            }
            else if (SiteID > 0)
            {
                listElem.WhereCondition = "ContactSiteID = " + SiteID;
            }
        }

        // Set header actions (add button)
        string url = ResolveUrl("New.aspx?siteId=" + listElem.SiteID);
        if (IsSiteManager)
        {
            url = URLHelper.AddParameterToUrl(url, "isSiteManager", "1");
        }
        hdrActions.AddAction(new HeaderAction
            {
                Text = GetString("om.contact.new"),
                RedirectUrl = url
            });
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Disable actions for unauthorized users
        if (!ContactHelper.AuthorizedModifyContact(listElem.SiteID, false))
        {
            hdrActions.Enabled = false;
        }
        // Allow new button only for particular sites or (global) site
        else if ((listElem.SiteID < 0) && (listElem.SiteID != UniSelector.US_GLOBAL_RECORD))
        {
            hdrActions.Enabled = false;
            lblWarnNew.Visible = true;
        }
    }

    #endregion
}