using System;

using CMS.Core;
using CMS.ExtendedControls.ActionsConfig;
using CMS.Helpers;
using CMS.OnlineMarketing;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;

[Title("om.contactgroup.list")]
[UIElement(ModuleName.ONLINEMARKETING, "ContactGroups")]
public partial class CMSModules_ContactManagement_Pages_Tools_ContactGroup_List : CMSContactManagementContactGroupsPage
{
    #region "Variables"

    private int siteId = SiteContext.CurrentSiteID;

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        SiteOrGlobalSelector.TargetObjectType = ContactGroupInfo.OBJECT_TYPE;

        // In CMS Desk "siteID is available in control
        if (!IsSiteManager)
        {
            // Init site filter when user is authorized for global and site objects
            if (AuthorizedForGlobalContactGroups && AuthorizedForSiteContactGroups)
            {
                CurrentMaster.DisplaySiteSelectorPanel = true;
                if (!RequestHelper.IsPostBack())
                {
                    SiteOrGlobalSelector.SiteID = SiteID;
                }
                siteId = SiteOrGlobalSelector.SiteID;
                if (siteId == UniSelector.US_GLOBAL_AND_SITE_RECORD)
                {
                    listElem.WhereCondition = SiteOrGlobalSelector.GetWhereCondition();
                }
            }
            // User is authorized for site accounts
            else if (AuthorizedForSiteContactGroups)
            {
                // Use default value = current site ID
            }
            // User is authorized only for global accounts
            else if (AuthorizedForGlobalContactGroups)
            {
                siteId = UniSelector.US_GLOBAL_RECORD;
            }
            // User is not authorized
            else
            {
                RedirectToAccessDenied(ModuleName.CONTACTMANAGEMENT, "ReadContactGroups");
            }
        }
        // In Site Manager "siteID" is in query string
        else
        {
            siteId = SiteID;

            // Hide title
            PageTitle.TitleText = string.Empty;
        }

        // Set header actions (add button)
        listElem.SiteID = siteId;

        string url = ResolveUrl(string.Format("{0}&displaytitle=false&siteid={1}", UIContextHelper.GetElementUrl(ModuleName.CONTACTMANAGEMENT, "NewContactGroup"), siteId));
        if (IsSiteManager)
        {
            url = URLHelper.AddParameterToUrl(url, "isSiteManager", "1");
        }
        hdrActions.AddAction(new HeaderAction
            {
            Text = GetString("om.contactgroup.new"),
            RedirectUrl = url
        });

        // Register script for UniMenu button selection
        AddMenuButtonSelectScript(this, "ContactGroups", null, "menu");
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Disable actions for unauthorized users
        if (!ContactGroupHelper.AuthorizedModifyContactGroup(listElem.SiteID, false))
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