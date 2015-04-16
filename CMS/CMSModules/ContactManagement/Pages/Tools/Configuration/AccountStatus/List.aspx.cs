using System;
using CMS.Core;
using CMS.DataEngine;
using CMS.ExtendedControls.ActionsConfig;
using CMS.Helpers;
using CMS.OnlineMarketing;
using CMS.Base;
using CMS.SiteProvider;
using CMS.UIControls;

[Title("om.accountstatus.list")]
public partial class CMSModules_ContactManagement_Pages_Tools_Configuration_AccountStatus_List : CMSContactManagementAccountStatusPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // In CMS Desk "siteID is available in control
        if (!IsSiteManager)
        {
            // Init site filter when user is authorized for global and site account statuses
            if (AuthorizedForGlobalConfiguration && AuthorizedForSiteConfiguration)
            {
                CurrentMaster.DisplaySiteSelectorPanel = true;
                if (!RequestHelper.IsPostBack())
                {
                    SiteOrGlobalSelector.SiteID = QueryHelper.GetInteger("siteId", SiteContext.CurrentSiteID);
                }
                SiteID = SiteOrGlobalSelector.SiteID;
            }
            // User is authorized for site account statuses
            else if (AuthorizedForSiteConfiguration)
            {
                SiteID = SiteContext.CurrentSiteID;
            }
            // User is authorized only for global account statuses
            else if (AuthorizedForGlobalConfiguration)
            {
                SiteID = UniSelector.US_GLOBAL_RECORD;
            }
            // User is not authorized
            else
            {
                RedirectToAccessDenied(ModuleName.CONTACTMANAGEMENT, "ReadConfiguration");
            }
        }

        // Set header actions (add button)
        hdrActions.AddAction(new HeaderAction
        {
            Text = GetString("om.accountstatus.new"),
            RedirectUrl = AddSiteQuery("Edit.aspx", null)
        });

        // Filter site data
        Grid.WhereCondition = SqlHelper.AddWhereCondition(Grid.WhereCondition, GetSiteFilter("AccountStatusSiteID"));
        Grid.OnBeforeDataReload += () => { Grid.NamedColumns["sitename"].Visible = ((SiteID < 0) && (SiteID != UniSelector.US_GLOBAL_RECORD)); };
        Grid.EditActionUrl = AddSiteQuery(Grid.EditActionUrl, null);
        Grid.ZeroRowsText = GetString("om.accountstatus.notfound");
        Grid.OnAction += Grid_OnAction;
    }


    /// <summary>
    /// UniGrid action handler.
    /// </summary>
    private void Grid_OnAction(string actionName, object actionArgument)
    {
        if (actionName == "delete")
        {
            AccountStatusInfo asi = AccountStatusInfoProvider.GetAccountStatusInfo(ValidationHelper.GetInteger(actionArgument, 0));
            if (asi != null)
            {
                //  Check modify permission for given object
                if (ConfigurationHelper.AuthorizedModifyConfiguration(asi.AccountStatusSiteID, true))
                {
                    AccountStatusInfoProvider.DeleteAccountStatusInfo(asi);
                }
            }
        }
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        // Check permissions to create new record
        if ((SiteID > 0) && !ModifySiteConfiguration)
        {
            hdrActions.Enabled = false;
        }
        else if ((SiteID <= 0) && !ModifyGlobalConfiguration)
        {
            hdrActions.Enabled = false;
        }
        // Allow new button only for particular sites or (global) site
        else if ((SiteID < 0) && (SiteID != UniSelector.US_GLOBAL_RECORD))
        {
            hdrActions.Enabled = false;
            lblWarnNew.Visible = true;
        }
    }
}