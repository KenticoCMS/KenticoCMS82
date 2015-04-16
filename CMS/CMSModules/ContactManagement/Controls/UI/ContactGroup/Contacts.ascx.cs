using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Automation;
using CMS.Base;
using CMS.Core;
using CMS.DataEngine;
using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.Membership;
using CMS.OnlineMarketing;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.WorkflowEngine;

public partial class CMSModules_ContactManagement_Controls_UI_ContactGroup_Contacts : CMSAdminListControl, ICallbackEventHandler
{
    #region "Variables & constants"

    private Hashtable mParameters;
    private ContactGroupInfo cgi;
    private int contactGroupSiteID = -1;
    private bool readSiteContacts;
    private bool readGlobalContacts;
    private bool modifySiteCG;
    private bool modifyGlobalCG;
    private bool modifyCombined;


    /// <summary>
    /// Available actions in mass action selector.
    /// </summary>
    protected enum Action
    {
        SelectAction = 0,
        Remove = 1,
        ChangeStatus = 2,
        StartNewProcess = 3
    }


    /// <summary>
    /// Selected objects in mass action selector.
    /// </summary>
    protected enum What
    {
        Selected = 0,
        All = 1
    }


    /// <summary>
    /// URL of modal dialog for contact status selection.
    /// </summary>
    protected const string CONTACT_STATUS_DIALOG = "~/CMSModules/ContactManagement/FormControls/ContactStatusDialog.aspx";


    /// <summary>
    /// URL of selection dialog.
    /// </summary>
    public const string SELECTION_DIALOG = "~/CMSAdminControls/UI/UniSelector/SelectionDialog.aspx";

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
            contactSelector.StopProcessing = value;
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
            contactSelector.IsLiveSite = value;
            gridElem.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Gets or sets the callback argument.
    /// </summary>
    private string CallbackArgument
    {
        get;
        set;
    }


    /// <summary>
    /// Dialog control identifier.
    /// </summary>
    private string Identifier
    {
        get
        {
            string identifier = hdnIdentifier.Value;
            if (string.IsNullOrEmpty(identifier))
            {
                identifier = Guid.NewGuid().ToString();
                hdnIdentifier.Value = identifier;
            }

            return identifier;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Get edited object (contact group)
        if (UIContext.EditedObject != null)
        {
            cgi = (ContactGroupInfo)UIContext.EditedObject;
            contactGroupSiteID = cgi.ContactGroupSiteID;

            // Check permissions
            readSiteContacts = ContactHelper.AuthorizedReadContact(SiteContext.CurrentSiteID, false);
            modifySiteCG = ContactGroupHelper.AuthorizedModifyContactGroup(SiteContext.CurrentSiteID, false);
            if (contactGroupSiteID <= 0)
            {
                readGlobalContacts = ContactHelper.AuthorizedReadContact(UniSelector.US_GLOBAL_RECORD, false);
                modifyGlobalCG = ContactGroupHelper.AuthorizedModifyContactGroup(UniSelector.US_GLOBAL_RECORD, false);
            }

            // Setup unigrid
            string where = "(ContactGroupMemberContactGroupID = " + cgi.ContactGroupID + ")";
            gridElem.WhereCondition = SqlHelper.AddWhereCondition(where, GetWhereCondition());
            gridElem.OnAction += gridElem_OnAction;
            gridElem.ZeroRowsText = GetString("om.contact.nocontacts");
            gridElem.OnBeforeDataReload += gridElem_OnBeforeDataReload;
            gridElem.OnExternalDataBound += gridElem_OnExternalDataBound;

            modifyCombined = ((contactGroupSiteID > 0) && modifySiteCG) || ((contactGroupSiteID <= 0) && modifyGlobalCG);

            if (!string.IsNullOrEmpty(cgi.ContactGroupDynamicCondition))
            {
                // Set specific confirmation to remove grid action
                var removeAction = (CMS.UIControls.UniGridConfig.Action)gridElem.GridActions.Actions[1];
                removeAction.Confirmation = "$om.contactgroupmember.confirmremove$";
            }

            // Initialize dropdown lists
            if (!RequestHelper.IsPostBack())
            {
                drpAction.Items.Add(new ListItem(GetString("general." + Action.SelectAction), Convert.ToInt32(Action.SelectAction).ToString()));
                drpWhat.Items.Add(new ListItem(GetString("om.contact." + What.Selected), Convert.ToInt32(What.Selected).ToString()));
                drpWhat.Items.Add(new ListItem(GetString("om.contact." + What.All), Convert.ToInt32(What.All).ToString()));

                // Display mass actions
                if (modifyCombined)
                {
                    drpAction.Items.Add(new ListItem(GetString("general.remove"), Convert.ToInt32(Action.Remove).ToString()));
                }

                if (ContactHelper.AuthorizedModifyContact(contactGroupSiteID, false))
                {
                    drpAction.Items.Add(new ListItem(GetString("om.account." + Action.ChangeStatus), Convert.ToInt32(Action.ChangeStatus).ToString()));
                }

                if (MembershipContext.AuthenticatedUser.IsAuthorizedPerResource(ModuleName.ONLINEMARKETING, "StartProcess"))
                {
                    drpAction.Items.Add(new ListItem(GetString("ma.automationprocess.select"), Convert.ToInt32(Action.StartNewProcess).ToString()));
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
            contactSelector.UniSelector.OnItemsSelected += UniSelector_OnItemsSelected;
            contactSelector.UniSelector.SelectionMode = SelectionModeEnum.MultipleButton;

            // Register JS scripts
            RegisterScripts();
        }
        else
        {
            StopProcessing = true;
        }
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        // Hide mass actions if there are no contacts shown or no mass action is allowed (first action is always there, so comparison with 1 has to be made)
        pnlFooter.Visible = !gridElem.IsEmpty && (drpAction.Items.Count > 1);
    }

    #endregion


    #region "Events"

    /// <summary>
    /// UniGrid external databound.
    /// </summary>
    private object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        CMSGridActionButton btn;

        switch (sourceName.ToLowerCSafe())
        {
            // Display delete button
            case "remove":
                btn = sender as CMSGridActionButton;
                if (btn != null)
                {
                    btn.Enabled = contactGroupSiteID > 0 ? modifySiteCG : modifyGlobalCG;
                }
                break;

            case "edit":
                btn = (CMSGridActionButton)sender;
                // Ensure accountID parameter value;
                var objectID = ValidationHelper.GetInteger(btn.CommandArgument, 0);
                // Contact detail URL
                string contactURL = UIContextHelper.GetElementDialogUrl(ModuleName.ONLINEMARKETING, "EditContact", objectID, "isSiteManager=" + ContactHelper.IsSiteManager);
                // Add modal dialog script to onClick action
                btn.OnClientClick = ScriptHelper.GetModalDialogScript(contactURL, "ContactDetail");
                break;
        }

        return null;
    }


    /// <summary>
    /// OnBeforeDataReload event handler.
    /// </summary>
    private void gridElem_OnBeforeDataReload()
    {
        gridElem.NamedColumns["SiteName"].Visible = !(contactGroupSiteID > 0) && (contactGroupSiteID != UniSelector.US_GLOBAL_RECORD) && readSiteContacts;
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
            int contactId = ValidationHelper.GetInteger(actionArgument, 0);
            ContactInfo contact = ContactInfoProvider.GetContactInfo(contactId);
            if (contact != null)
            {
                CheckModifyPermissions();

                // Get the relationship object
                ContactGroupMemberInfo mi = ContactGroupMemberInfoProvider.GetContactGroupMemberInfoByData(cgi.ContactGroupID, contactId, ContactGroupMemberTypeEnum.Contact);
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
        string newValues = ValidationHelper.GetString(contactSelector.Value, null);
        string[] newItems = newValues.Split(new[]
        {
            ';'
        }, StringSplitOptions.RemoveEmptyEntries);

        // Get all selected items

        foreach (string item in newItems)
        {
            // Check if relation already exists
            int itemId = ValidationHelper.GetInteger(item, 0);
            ContactGroupMemberInfo cgmi = ContactGroupMemberInfoProvider.GetContactGroupMemberInfoByData(cgi.ContactGroupID, itemId,
                ContactGroupMemberTypeEnum.Contact);
            if (cgmi == null)
            {
                ContactGroupMemberInfoProvider.SetContactGroupMemberInfo(cgi.ContactGroupID, itemId,
                    ContactGroupMemberTypeEnum.Contact,
                    MemberAddedHowEnum.Manual);
            }
            else if (!cgmi.ContactGroupMemberFromManual)
            {
                cgmi.ContactGroupMemberFromManual = true;
                ContactGroupMemberInfoProvider.SetContactGroupMemberInfo(cgmi);
            }
        }

        gridElem.ReloadData();
        pnlUpdate.Update();
        contactSelector.Value = null;
    }


    /// <summary>
    /// Checks permissions for specified action and selected contacts.
    /// </summary>
    /// <param name="action">Type of action</param>
    private void CheckActionPermissions(Action action)
    {
        if (action == Action.ChangeStatus)
        {
            CheckModifyContactPermission();
        }
        else
        {
            CheckModifyPermissions();
        }
    }


    /// <summary>
    /// Checks modify contact permission for current contact group.
    /// If global, current site permission is also checked.
    /// </summary>
    private void CheckModifyContactPermission()
    {
        ContactHelper.AuthorizedModifyContact(contactGroupSiteID, true);
        if (contactGroupSiteID == 0)
        {
            ContactHelper.AuthorizedModifyContact(SiteContext.CurrentSiteID, true);
        }
    }


    /// <summary>
    /// Checks modify permission for contact group.
    /// </summary>
    private void CheckModifyPermissions()
    {
        // Check modify permission
        if ((contactGroupSiteID > 0) && !(CheckPermissions("cms.contactmanagement", "ModifyContactGroups")))
        {
            CMSPage.RedirectToAccessDenied(ModuleName.CONTACTMANAGEMENT, "ModifyContactGroups");
        }

        if ((contactGroupSiteID == 0) && !(CheckPermissions("cms.contactmanagement", "ModifyGlobalContactGroups")))
        {
            CMSPage.RedirectToAccessDenied(ModuleName.CONTACTMANAGEMENT, "ModifyGlobalContactGroups");
        }
    }


    protected void btnOk_Click(object sender, EventArgs e)
    {
        What what = (What)ValidationHelper.GetInteger(drpWhat.SelectedValue, 0);
        Action action = (Action)ValidationHelper.GetInteger(drpAction.SelectedValue, 0);

        // Check permissions for specified action
        CheckActionPermissions(action);

        // Set constraint for contact relations only
        var where = new WhereCondition()
            .WhereEquals("ContactGroupMemberType", 0)
            .WhereEquals("ContactGroupMemberContactGroupID", cgi.ContactGroupID);

        switch (what)
        {
            case What.All:
                var contactIds = ContactInfoProvider.GetContacts()
                    .Where(GetWhereCondition())
                    .Where(gridElem.WhereClause)
                    .AsIDQuery();
                where.WhereIn("ContactGroupMemberRelatedID", contactIds);
                break;

            case What.Selected:
                where.WhereIn("ContactGroupMemberRelatedID", gridElem.SelectedItems);
                break;
        }

        switch (action)
        {
            case Action.Remove:
                RemoveContacts(what, where.ToString(true));
                break;

            case Action.ChangeStatus:
                ChangeStatus(what);
                break;

            case Action.StartNewProcess:
                StartNewProcess(what, where.ToString(true));
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

    private void RemoveContacts(What what, string where)
    {
        ContactGroupMemberInfoProvider.DeleteContactGroupMembers(where, cgi.ContactGroupID, false, false);

        switch (what)
        {
            case What.All:
                ShowConfirmation(GetString("om.contact.massaction.removedall"));
                break;

            case What.Selected:
                ShowConfirmation(GetString("om.contact.massaction.removed"));
                break;
        }
    }


    private void ChangeStatus(What what)
    {
        int statusId = ValidationHelper.GetInteger(hdnIdentifier.Value, -1);
        string where = null;

        switch (what)
        {
            case What.All:
                {
                    where = SqlHelper.AddWhereCondition(gridElem.WhereCondition, gridElem.WhereClause);
                    where = "ContactID IN (SELECT ContactGroupMemberRelatedID FROM OM_ContactGroupMember WHERE " + where + ")";
                }
                break;

            case What.Selected:
                where = SqlHelper.GetWhereCondition<int>("ContactID", gridElem.SelectedItems, false);
                break;
        }

        ContactInfoProvider.UpdateContactStatus(statusId, where);

        ShowConfirmation(GetString("om.contact.massaction.statuschanged"));
    }


    private void StartNewProcess(What what, string where)
    {
        try
        {
            AutomationManager manager = AutomationManager.GetInstance(CurrentUser);

            List<string> contactIds = null;

            switch (what)
            {
                case What.All:
                    // Get selected IDs based on where condition
                    DataSet contacts = ContactGroupMemberInfoProvider.GetRelationships().Where(where).Column("ContactGroupMemberRelatedID");
                    if (!DataHelper.DataSourceIsEmpty(contacts))
                    {
                        contactIds = DataHelper.GetUniqueValues(contacts.Tables[0], "ContactGroupMemberRelatedID", true);
                    }
                    break;

                case What.Selected:
                    contactIds = gridElem.SelectedItems;
                    break;
            }

            if (contactIds != null)
            {
                string error = String.Empty;
                using (CMSActionContext context = new CMSActionContext())
                {
                    context.AllowAsyncActions = false;
                    int processId = ValidationHelper.GetInteger(hdnIdentifier.Value, 0);

                    foreach (string contactId in contactIds)
                    {
                        var contact = ContactInfoProvider.GetContactInfo(ValidationHelper.GetInteger(contactId, 0));

                        try
                        {
                            manager.StartProcess(contact, processId);
                        }
                        catch (ProcessRecurrenceException ex)
                        {
                            error += "<div>" + ex.Message + "</div>";
                        }
                    }
                }

                if (String.IsNullOrEmpty(error))
                {
                    string confirmation = GetString(what == What.All ? "ma.process.started" : "ma.process.startedselected");
                    ShowConfirmation(confirmation);
                }
                else
                {
                    ShowError(GetString("ma.process.error"), error, null);
                }
            }
        }
        catch (Exception ex)
        {
            LogAndShowError("Automation", "STARTPROCESS", ex);
        }
    }


    /// <summary>
    /// Returns WHERE condition
    /// </summary>
    private string GetWhereCondition()
    {
        string where = "((ContactSiteID IS NULL AND ContactGlobalContactID IS NULL) OR (ContactSiteID > 0 AND ContactMergedWithContactID IS NULL))";

        // Filter site objects
        if (contactGroupSiteID > 0)
        {
            if (readSiteContacts)
            {
                where = SqlHelper.AddWhereCondition(where, "(ContactSiteID = " + contactGroupSiteID + ")");
                contactSelector.SiteID = contactGroupSiteID;
            }
            else
            {
                CMSPage.RedirectToAccessDenied(ModuleName.CONTACTMANAGEMENT, "ReadContacts");
            }
        }
        // Current group is global object
        else if (contactGroupSiteID == 0)
        {
            // In CMS Desk display current site and global objects
            if (!ContactHelper.IsSiteManager)
            {
                if (readSiteContacts && readGlobalContacts)
                {
                    where = SqlHelper.AddWhereCondition(where, "(ContactSiteID IS NULL) OR (ContactSiteID = " + SiteContext.CurrentSiteID + ")");
                    contactSelector.SiteID = UniSelector.US_GLOBAL_AND_SITE_RECORD;
                }
                else if (readGlobalContacts)
                {
                    where = SqlHelper.AddWhereCondition(where, "(ContactSiteID IS NULL)");
                    contactSelector.SiteID = UniSelector.US_GLOBAL_RECORD;
                }
                else if (readSiteContacts)
                {
                    where = SqlHelper.AddWhereCondition(where, "ContactSiteID = " + SiteContext.CurrentSiteID);
                    contactSelector.SiteID = SiteContext.CurrentSiteID;
                }
                else
                {
                    CMSPage.RedirectToAccessDenied(ModuleName.CONTACTMANAGEMENT, "ReadGlobalContacts|ReadContacts");
                }
            }
            // In Site manager display for global contact group all site and global contacts
            else
            {
                // No WHERE condition required = displaying all data

                // Set contact selector only
                contactSelector.SiteID = UniSelector.US_ALL_RECORDS;
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

        // Register script to open dialogs for contact editing
        script.Append(@"
function SelectStatus(queryParameters){
    modalDialog('" + ResolveUrl(CONTACT_STATUS_DIALOG) + @"' + queryParameters, 'selectStatus', '660px', '590px');
}
function StartNewProcess(queryParameters) {
    modalDialog('", ResolveUrl(SELECTION_DIALOG), @"' + queryParameters, 'selectProcess', '750px', '630px');
}
function Refresh() {
    __doPostBack('", pnlUpdate.ClientID, @"', '');
}
function SelectValue_" + ClientID + @"(valueID) {
    document.getElementById('" + hdnIdentifier.ClientID + @"').value = valueID;"
                                                                                                                                                                                                                                                                                                                                                                                                                       + ControlsHelper.GetPostBackEventReference(btnOk, null) + @";
}
function US_SelectItems_(valueID) {
    SelectValue_" + ClientID + @"(valueID);
    ShowUpdatePanel();
}
function ShowUpdatePanel(){
     $cmsj('#" + loading.ClientID + @"').css('display', 'inline');
}
function PerformAction(selectionFunction, selectionField, actionId, actionLabel, whatId) {
    var confirmed = true;
    var label = document.getElementById(actionLabel);
    var action = document.getElementById(actionId).value;
    var whatDrp = document.getElementById(whatId).value;
    var selectionFieldElem = document.getElementById(selectionField);
    label.innerHTML = '';
    if (action == '", (int)Action.SelectAction, @"') {
        label.innerHTML = ", ScriptHelper.GetLocalizedString("MassAction.SelectSomeAction"), @"
    }
    else if (eval(selectionFunction) && (whatDrp == '", (int)What.Selected, @"')) {
        label.innerHTML = ", ScriptHelper.GetLocalizedString("om.contact.massaction.select"), @";
    }
    else {
        var param = 'massaction;' + whatDrp;
        if (whatDrp == '", (int)What.Selected, @"') {
            param = param + '#' + selectionFieldElem.value;
        }
        switch(action) {
            case '", (int)Action.Remove, @"':
                if (whatDrp == ", (int)What.Selected, @") {
                    return confirm(", (!string.IsNullOrEmpty(cgi.ContactGroupDynamicCondition)) ? ScriptHelper.GetLocalizedString("om.contactgroupmember.confirmremove") : ScriptHelper.GetLocalizedString("General.ConfirmRemove"), @");
                }
                else {
                    return confirm(", (!string.IsNullOrEmpty(cgi.ContactGroupDynamicCondition)) ? ScriptHelper.GetLocalizedString("om.contactgroupmember.confirmremoveall") : ScriptHelper.GetLocalizedString("General.ConfirmRemoveAll"), @");
                }
                break;
            case '", (int)Action.ChangeStatus, @"':
                dialogParams_", ClientID, @" = param + ';", (int)Action.ChangeStatus, @"';", Page.ClientScript.GetCallbackEventReference(this, "dialogParams_" + ClientID, "SelectStatus", null), @";
                break;
            case '", (int)Action.StartNewProcess, @"':
                if (whatDrp == ", (int)What.All, @") {
                    confirmed = confirm(", ScriptHelper.GetLocalizedString("om.contactgroupmember.confirmstartnewprocessforall"), @")                
                }
                else {
                    confirmed = confirm(", ScriptHelper.GetLocalizedString("om.contactgroupmember.confirmstartnewprocess"), @")
                }
                if(confirmed){
                    dialogParams_", ClientID, @" = param + ';", (int)Action.StartNewProcess, @"';", Page.ClientScript.GetCallbackEventReference(this, "dialogParams_" + ClientID, "StartNewProcess", null), @";
                }
                break;
            default:
                break;
        }
    }
    return false;
}");

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "MassActions", ScriptHelper.GetScript(script.ToString()));

        btnOk.OnClientClick = "return PerformAction('" + gridElem.GetCheckSelectionScript() + "','" + gridElem.GetSelectionFieldClientID() + "','" + drpAction.ClientID + "','" + lblInfo.ClientID + "','" + drpWhat.ClientID + "');";
    }

    #endregion


    #region "ICallbackEventHandler Members"

    /// <summary>
    /// Gets callback result.
    /// </summary>
    public string GetCallbackResult()
    {
        string queryString = null;

        if (!string.IsNullOrEmpty(CallbackArgument))
        {
            // Prepare parameters...
            mParameters = new Hashtable();
            mParameters["issitemanager"] = ContactHelper.IsSiteManager;

            // ...for mass action
            if (CallbackArgument.StartsWithCSafe("massaction;", true))
            {
                // Get values of callback argument
                string[] selection = CallbackArgument.Split(new[]
                {
                    ";"
                }, StringSplitOptions.RemoveEmptyEntries);
                if (selection.Length != 3)
                {
                    return null;
                }

                // Get selected actions from DD-list
                Action action = (Action)ValidationHelper.GetInteger(selection[2], 0);
                switch (action)
                {
                    case Action.ChangeStatus:
                        mParameters["allownone"] = true;
                        mParameters["clientid"] = ClientID;
                        break;

                    case Action.StartNewProcess:
                        mParameters["SelectionMode"] = SelectionModeEnum.SingleButton;
                        mParameters["ObjectType"] = WorkflowInfo.OBJECT_TYPE_AUTOMATION;
                        mParameters["WhereCondition"] = "WorkflowEnabled = 1";
                        break;

                    default:
                        return null;
                }
            }
            // ...for unigrid action
            else
            {
                mParameters["where"] = SqlHelper.GetWhereCondition<int>("ContactID", new string[]
                {
                    CallbackArgument
                }, false);
            }

            mParameters["siteid"] = cgi.ContactGroupSiteID;

            WindowHelper.Add(Identifier, mParameters);

            queryString = "?params=" + Identifier;
            queryString = URLHelper.AddParameterToUrl(queryString, "hash", QueryHelper.GetHash(queryString));
        }

        return queryString;
    }


    /// <summary>
    /// Raise callback method.
    /// </summary>
    public void RaiseCallbackEvent(string eventArgument)
    {
        CallbackArgument = eventArgument;
    }

    #endregion
}