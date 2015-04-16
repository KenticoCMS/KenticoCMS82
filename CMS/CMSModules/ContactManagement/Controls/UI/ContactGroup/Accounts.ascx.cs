using System;
using System.Text;
using System.Web.UI.WebControls;
using System.Linq;

using CMS.Base;
using CMS.Core;
using CMS.DataEngine;
using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.OnlineMarketing;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_ContactManagement_Controls_UI_ContactGroup_Accounts : CMSAdminListControl
{
    #region "Variables"

    private ContactGroupInfo cgi;
    private int siteID = -1;
    private bool readSiteAccounts;
    private bool readGlobalAccounts;
    private bool modifySiteCG;
    private bool modifyGlobalCG;
    private bool modifyCombined;


    /// <summary>
    /// Available actions in mass action selector.
    /// </summary>
    protected enum Action
    {
        SelectAction = 0,
        Remove = 1
    }


    /// <summary>
    /// Selected objects in mass action selector.
    /// </summary>
    protected enum What
    {
        Selected = 0,
        All = 1
    }

    #endregion


    #region "Properties"

    /// <summary>
    /// Messages placeholder.
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
    }


    /// <summary>
    /// Inner grid.
    /// </summary>
    public UniGrid Grid
    {
        get
        {
            return gridElem;
        }
    }


    /// <summary>
    /// Indicates if the control should perform the operations.
    /// </summary>
    public override bool StopProcessing
    {
        get
        {
            return base.StopProcessing;
        }
        set
        {
            base.StopProcessing = value;
            gridElem.StopProcessing = value;
            accountSelector.StopProcessing = value;
        }
    }


    /// <summary>
    /// Indicates if  filter is used on live site or in UI.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            base.IsLiveSite = value;
            accountSelector.IsLiveSite = value;
            gridElem.IsLiveSite = value;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Init(object sender, EventArgs e)
    {
        // Get edited object (contact group)
        if (UIContext.EditedObject != null)
        {
            cgi = (ContactGroupInfo)UIContext.EditedObject;
            siteID = cgi.ContactGroupSiteID;

            // Check permissions
            readSiteAccounts = AccountHelper.AuthorizedReadAccount(SiteContext.CurrentSiteID, false);
            modifySiteCG = ContactGroupHelper.AuthorizedModifyContactGroup(SiteContext.CurrentSiteID, false);
            if (siteID <= 0)
            {
                readGlobalAccounts = AccountHelper.AuthorizedReadAccount(UniSelector.US_GLOBAL_RECORD, false);
                modifyGlobalCG = ContactGroupHelper.AuthorizedModifyContactGroup(UniSelector.US_GLOBAL_RECORD, false);
            }

            // Setup unigrid
            gridElem.WhereCondition = GetWhereCondition();
            gridElem.OnAction += gridElem_OnAction;
            gridElem.ZeroRowsText = GetString("om.account.noaccountsfound");
            gridElem.OnBeforeDataReload += gridElem_OnBeforeDataReload;
            gridElem.OnExternalDataBound += gridElem_OnExternalDataBound;

            modifyCombined = ((siteID > 0) && modifySiteCG) || ((siteID <= 0) && (modifyGlobalCG));

            // Initialize dropdown lists
            if (!RequestHelper.IsPostBack())
            {
                // Display mass actions
                if (modifyCombined)
                {
                    drpAction.Items.Add(new ListItem(GetString("general." + Action.SelectAction), Convert.ToInt32(Action.SelectAction).ToString()));
                    drpAction.Items.Add(new ListItem(GetString("general.remove"), Convert.ToInt32(Action.Remove).ToString()));
                    drpWhat.Items.Add(new ListItem(GetString("om.account." + What.Selected), Convert.ToInt32(What.Selected).ToString()));
                    drpWhat.Items.Add(new ListItem(GetString("om.account." + What.All), Convert.ToInt32(What.All).ToString()));
                }
            }
            else
            {
                if (ControlsHelper.CausedPostBack(btnOk))
                {
                    // Set delayed reload for unigrid if mass action is performed
                    gridElem.DelayedReload = true;
                }
            }

            // Initialize contact selector
            accountSelector.UniSelector.OnItemsSelected += UniSelector_OnItemsSelected;
            accountSelector.UniSelector.SelectionMode = SelectionModeEnum.MultipleButton;
            accountSelector.UniSelector.DialogButton.ResourceString = "om.account.addaccount";

        }
        else
        {
            StopProcessing = true;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Register JS scripts
        RegisterScripts();
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Hide footer if grid is empty
        pnlFooter.Visible = !gridElem.IsEmpty && (drpAction.Items.Count > 0);
    }

    #endregion


    #region "Events"

    /// <summary>
    /// UniGrid external databound.
    /// </summary>
    private object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        CMSGridActionButton btn = null;
        switch (sourceName.ToLowerCSafe())
        {
            case "edit":
                btn = ((CMSGridActionButton)sender);
                // Ensure accountID parameter value;
                var objectID = ValidationHelper.GetInteger(btn.CommandArgument, 0);
                // Account detail URL
                string accountURL = UIContextHelper.GetElementDialogUrl(ModuleName.ONLINEMARKETING, "EditAccount", objectID, "isSiteManager=" + ContactHelper.IsSiteManager);
                // Add modal dialog script to onClick action
                btn.OnClientClick = ScriptHelper.GetModalDialogScript(accountURL, "AccountDetail");
                break;

            // Display delete button
            case "remove":
                btn = (CMSGridActionButton)sender;

                // Display delete button only for users with appropriate permission
                btn.Enabled = siteID > 0 ? modifySiteCG : modifyGlobalCG;
                break;
        }
        return null;
    }


    /// <summary>
    /// OnBeforeDataReload event handler.
    /// </summary>
    private void gridElem_OnBeforeDataReload()
    {
        gridElem.NamedColumns["SiteName"].Visible = !(siteID > 0) && (siteID != UniSelector.US_GLOBAL_RECORD) && readSiteAccounts;
    }


    /// <summary>
    /// Unigrid button clicked.
    /// </summary>
    protected void gridElem_OnAction(string actionName, object actionArgument)
    {
        // Perform 'remove' action
        if (actionName == "remove")
        {
            // Delete the object
            int accountId = ValidationHelper.GetInteger(actionArgument, 0);
            AccountInfo account = AccountInfoProvider.GetAccountInfo(accountId);
            if (account != null)
            {
                CheckModifyPermissions();

                // Get the relationship object
                ContactGroupMemberInfo mi = ContactGroupMemberInfoProvider.GetContactGroupMemberInfoByData(cgi.ContactGroupID, accountId, ContactGroupMemberTypeEnum.Account);
                if (mi != null)
                {
                    ContactGroupMemberInfoProvider.DeleteContactGroupMemberInfo(mi);
                }
            }
        }
    }


    /// <summary>
    /// Items changed event handler.
    /// </summary>
    protected void UniSelector_OnItemsSelected(object sender, EventArgs e)
    {
        CheckModifyPermissions();

        // Get new items from selector
        string newValues = ValidationHelper.GetString(accountSelector.Value, null);
        string[] newItems = newValues.Split(new[]
        {
            ';'
        }, StringSplitOptions.RemoveEmptyEntries);

        // Get all selected items
        foreach (string item in newItems)
        {
            // Check if relation already exists
            int itemID = ValidationHelper.GetInteger(item, 0);
            if (ContactGroupMemberInfoProvider.GetContactGroupMemberInfoByData(cgi.ContactGroupID, itemID, ContactGroupMemberTypeEnum.Account) == null)
            {
                ContactGroupMemberInfoProvider.SetContactGroupMemberInfo(cgi.ContactGroupID, itemID, ContactGroupMemberTypeEnum.Account, MemberAddedHowEnum.Manual);
            }
        }

        gridElem.ReloadData();
        pnlUpdate.Update();
        accountSelector.Value = null;
    }


    /// <summary>
    /// Checks modify permission for contact group.
    /// </summary>
    private void CheckModifyPermissions()
    {
        // Check modify permission
        if ((siteID > 0) && !(CheckPermissions("cms.contactmanagement", "ModifyContactGroups")))
        {
            CMSPage.RedirectToAccessDenied(ModuleName.CONTACTMANAGEMENT, "ModifyContactGroups");
        }

        if ((siteID == 0) && !(CheckPermissions("cms.contactmanagement", "ModifyGlobalContactGroups")))
        {
            CMSPage.RedirectToAccessDenied(ModuleName.CONTACTMANAGEMENT, "ModifyGlobalContactGroups");
        }
    }


    /// <summary>
    /// Mass action 'ok' button clicked.
    /// </summary>
    protected void btnOk_Click(object sender, EventArgs e)
    {
        CheckModifyPermissions();

        Action action = (Action)ValidationHelper.GetInteger(drpAction.SelectedItem.Value, 0);
        What what = (What)ValidationHelper.GetInteger(drpWhat.SelectedItem.Value, 0);

        var where = new WhereCondition()
            .WhereEquals("ContactGroupMemberContactGroupID", cgi.ContactGroupID)
            // Set constraint for account relations only
            .WhereEquals("ContactGroupMemberType", 1);

        switch (what)
        {
            // All items
            case What.All:
                var accountIds = AccountInfoProvider.GetAccounts()
                   .Where(gridElem.WhereCondition)
                   .Where(gridElem.WhereClause)
                   .AsIDQuery();

                where.WhereIn("ContactGroupMemberRelatedID", accountIds);
                break;
            // Selected items
            case What.Selected:
                // Convert array to integer values to make sure no sql injection is possible (via string values)
                where.WhereIn("ContactGroupMemberRelatedID", gridElem.SelectedItems);
                break;
            default:
                return;
        }

        switch (action)
        {
            // Action 'Remove'
            case Action.Remove:
                // Delete the relations between contact group and accounts
                ContactGroupMemberInfoProvider.DeleteContactGroupMembers(where.ToString(true), cgi.ContactGroupID, true, true);
                // Show result message
                if (what == What.Selected)
                {
                    ShowConfirmation(GetString("om.account.massaction.removed"));
                }
                else
                {
                    ShowConfirmation(GetString("om.account.massaction.removedall"));
                }
                break;
            default:
                return;
        }

        // Reload unigrid
        gridElem.ResetSelection();
        gridElem.ReloadData();
        pnlUpdate.Update();
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Returns WHERE condition
    /// </summary>
    private string GetWhereCondition()
    {
        string where = "(ContactGroupMemberContactGroupID = " + cgi.ContactGroupID + ")";
        where = SqlHelper.AddWhereCondition(where, "((AccountSiteID IS NULL AND AccountGlobalAccountID IS NULL) OR (AccountSiteID > 0 AND AccountMergedWithAccountID IS NULL))");

        // Filter site objects
        if (siteID > 0)
        {
            if (readSiteAccounts)
            {
                where = SqlHelper.AddWhereCondition(where, "(AccountSiteID = " + siteID + ")");
                accountSelector.SiteID = siteID;
            }
            else
            {
                CMSPage.RedirectToAccessDenied(ModuleName.CONTACTMANAGEMENT, "ReadAccounts");
            }
        }
        // Current group is global object
        else if (siteID == 0)
        {
            // In CMS Desk display current site and global objects
            if (!ContactHelper.IsSiteManager)
            {
                if (readSiteAccounts && readGlobalAccounts)
                {
                    where = SqlHelper.AddWhereCondition(where, "(AccountSiteID IS NULL) OR (AccountSiteID = " + SiteContext.CurrentSiteID + ")");
                    accountSelector.SiteID = UniSelector.US_GLOBAL_AND_SITE_RECORD;
                }
                else if (readGlobalAccounts)
                {
                    where = SqlHelper.AddWhereCondition(where, "(AccountSiteID IS NULL)");
                    accountSelector.SiteID = UniSelector.US_GLOBAL_RECORD;
                }
                else if (readSiteAccounts)
                {
                    where = SqlHelper.AddWhereCondition(where, "AccountSiteID = " + SiteContext.CurrentSiteID);
                    accountSelector.SiteID = SiteContext.CurrentSiteID;
                }
                else
                {
                    CMSPage.RedirectToAccessDenied(ModuleName.CONTACTMANAGEMENT, "ReadGlobalAccounts|ReadAccounts");
                }
            }
            // In Site manager display for global contact group all site and global contacts
            else
            {
                // No WHERE condition required = displaying all data

                // Set contact selector only
                accountSelector.SiteID = UniSelector.US_ALL_RECORDS;
            }
        }
        return where;
    }


    /// <summary>
    /// Registers JS.
    /// </summary>
    private void RegisterScripts()
    {
        ScriptHelper.RegisterDialogScript(Page);
        StringBuilder script = new StringBuilder();

        // Register script to open dialogs for account editing
        script.Append(@"
function Refresh()
{
    __doPostBack('", pnlUpdate.ClientID, @"', '');
}
function PerformAction(selectionFunction, actionId, actionLabel, whatId) 
{
    var confirmation = null;
    var label = document.getElementById(actionLabel);
    var action = document.getElementById(actionId).value;
    var whatDrp = document.getElementById(whatId).value;
    label.innerHTML = '';
    if (action == '", (int)Action.SelectAction, @"') 
    {
        label.innerHTML = ", ScriptHelper.GetLocalizedString("MassAction.SelectSomeAction"), @"
    }
    else if (eval(selectionFunction) && (whatDrp == '", (int)What.Selected, @"')) 
    {
        label.innerHTML = ", ScriptHelper.GetLocalizedString("om.account.massaction.select"), @";
    }
    else 
    {
        switch(action) 
        {
            case '", (int)Action.Remove, @"':
                if (whatDrp == ", (int)What.Selected, @")
                {
                    confirmation = ", ScriptHelper.GetString(GetString("General.ConfirmRemove")), @";
                }
                else
                {
                    confirmation = ", ScriptHelper.GetString(GetString("General.ConfirmRemoveAll")), @";
                }                
            break;
            default:
                confirmation = null;
                break;
        }
        if (confirmation != null) 
        {
            return confirm(confirmation)
        }
    }
    return false;
}");

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "Actions", ScriptHelper.GetScript(script.ToString()));

        // Add action to button
        btnOk.OnClientClick = "return PerformAction('" + gridElem.GetCheckSelectionScript() + "','" + drpAction.ClientID + "','" + lblInfo.ClientID + "','" + drpWhat.ClientID + "');";
    }

    #endregion
}