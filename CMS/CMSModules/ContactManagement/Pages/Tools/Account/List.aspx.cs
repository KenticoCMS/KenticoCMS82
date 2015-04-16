using System;

using CMS.Core;
using CMS.Helpers;
using CMS.OnlineMarketing;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.ExtendedControls.ActionsConfig;

[UIElement(ModuleName.ONLINEMARKETING, "Accounts")]
public partial class CMSModules_ContactManagement_Pages_Tools_Account_List : CMSContactManagementAccountsPage
{
    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        SiteOrGlobalSelector.TargetObjectType = AccountInfo.OBJECT_TYPE;

        // In CMS Desk site ID is available in control
        if (!IsSiteManager)
        {
            // Init site filter when user is authorized for global and site accounts
            if (AuthorizedForGlobalAccounts && AuthorizedForSiteAccounts)
            {
                CurrentMaster.DisplaySiteSelectorPanel = true;
                if (!RequestHelper.IsPostBack())
                {
                    SiteOrGlobalSelector.SiteID = QueryHelper.GetInteger("siteId", SiteContext.CurrentSiteID);
                }
                listElem.SiteID = SiteOrGlobalSelector.SiteID;
                listElem.WhereCondition = SiteOrGlobalSelector.GetWhereCondition();
            }
            // User is authorized for site accounts
            else if (AuthorizedForSiteAccounts)
            {
                // Use default value = current site ID
                listElem.SiteID = SiteContext.CurrentSiteID;
                listElem.WhereCondition = "AccountSiteID = " + listElem.SiteID;
            }
            // User is authorized only for global accounts
            else if (AuthorizedForGlobalAccounts)
            {
                listElem.SiteID = UniSelector.US_GLOBAL_RECORD;
                listElem.WhereCondition = "AccountSiteID IS NULL";
            }
            // User is not authorized
            else
            {
                RedirectToAccessDenied(ModuleName.CONTACTMANAGEMENT, "ReadAccounts");
            }
        }
        // In Site Manager site ID is in query string
        else
        {
            listElem.SiteID = SiteID;
            if (SiteID == UniSelector.US_GLOBAL_RECORD)
            {
                listElem.WhereCondition = "AccountSiteID IS NULL";
            }
            else if (SiteID > 0)
            {
                listElem.WhereCondition = "AccountSiteID = " + SiteID;
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
                Text = GetString("om.account.new"),
                RedirectUrl = url
            });
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Disable actions for unauthorized users
        if (!AccountHelper.AuthorizedModifyAccount(listElem.SiteID, false))
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