using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Automation;
using CMS.Core;
using CMS.Helpers;
using CMS.Membership;
using CMS.OnlineMarketing;
using CMS.Base;
using CMS.PortalEngine;
using CMS.UIControls;
using CMS.WorkflowEngine;
using CMS.SiteProvider;
using CMS.DataEngine;
using CMS.ExtendedControls;

public partial class CMSModules_ContactManagement_Controls_UI_Automation_PendingContacts : CMSAdminEditControl
{
    #region "Public properties"

    /// <summary>
    /// Gets or sets current site id.
    /// </summary>
    public int SiteID
    {
        get;
        set;
    }

    /// <summary>
    /// Indicates if control is used as widget.
    /// </summary>
    public bool IsWidget
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets default page size of list control.
    /// </summary>
    public int PageSize
    {
        get;
        set;
    }


    /// <summary>
    /// If true, only pending contacts for the current user are shown.
    /// Current user has to be set as the owner of the contacts.
    /// </summary>
    public bool ShowOnlyMyPendingContacts
    {
        get;
        set;
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            SetupControl();
            listElem.OnBeforeDataReload += listElem_OnBeforeDataReload;
        }
        else
        {
            listElem.StopProcessing = true;
            listElem.Visible = false;
        }
    }


    protected void listElem_OnAction(string actionName, object actionArgument)
    {
        switch (actionName)
        {
            case "delete":
                var stateID = ValidationHelper.GetInteger(actionArgument, 0);
                if (stateID > 0)
                {
                    var stateInfo = AutomationStateInfoProvider.GetAutomationStateInfo(stateID);
                    if ((stateInfo != null) && WorkflowStepInfoProvider.CanUserRemoveAutomationProcess(CurrentUser, SiteInfoProvider.GetSiteName(stateInfo.StateSiteID)))
                    {
                        AutomationStateInfoProvider.DeleteAutomationStateInfo(stateInfo);
                    }
                }
                break;
        }
    }


    /// <summary>
    /// Data source needs to be set up on before data reload as where clause is finally available.
    /// </summary>
    private void listElem_OnBeforeDataReload()
    {
        listElem.DataSource = GetPendingContacts(CurrentUser, SiteID, listElem.WhereClause).TypedResult;
    }


    /// <summary>
    /// Setup control.
    /// </summary>
    private void SetupControl()
    {
        if (!URLHelper.IsPostback() && (PageSize > 0))
        {
            listElem.Pager.DefaultPageSize = PageSize;
        }

        listElem.ZeroRowsText = GetString("ma.pendingcontacts.nowaitingcontacts");

        // Hide site column for site records
        if (SiteID != UniSelector.US_ALL_RECORDS)
        {
            // Site column
            listElem.GridColumns.Columns[5].Visible = false;
        }

        listElem.EditActionUrl = "Process_Detail.aspx?stateid={0}&siteid=" + SiteID + (ContactHelper.IsSiteManager ? "&issitemanager=1" : String.Empty);

        listElem.RememberStateByParam = String.Empty;

        listElem.OnExternalDataBound += listElem_OnExternalDataBound;

        // Register scripts for contact details dialog
        ScriptHelper.RegisterDialogScript(Page);
        ScriptHelper.RegisterWOpenerScript(Page);
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ViewContactDetails", ScriptHelper.GetScript(
            "function Refresh() { \n " +
            "window.location.href = window.location.href;\n" +
            "}"));

        // If widget register action for view process in dialog
        if (IsWidget)
        {
            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ViewPendingContactProcess", ScriptHelper.GetScript(
            "function viewPendingContactProcess(stateId) {" +
            "    modalDialog('" + URLHelper.ResolveUrl("~/CMSModules/ContactManagement/Pages/Tools/PendingContacts/Process_Detail.aspx") + "?dialog=1&stateId=' + stateId, 'ViewPendingContactProcess', '1024', '800');" +
            "}"));
        }
    }


    /// <summary>
    /// Returns object query of automation states for my pending contacts which can be used as datasource for unigrid.
    /// </summary>
    /// <param name="user">User for whom pending contacts are shown</param>
    /// <param name="siteID">Site id</param>
    /// <param name="contactsWhereCondition">Where condition for filtering contacts</param>
    private ObjectQuery<AutomationStateInfo> GetPendingContacts(UserInfo user, int siteID, string contactsWhereCondition)
    {
        // Get complete where condition for pending steps
        var condition = WorkflowStepInfoProvider.GetAutomationPendingStepsWhereCondition(user, siteID);

        // Get site condition
        condition.And(GetSiteCondition(siteID));

        // Get automation steps specified by condition with permission control
        var automationWorkflowSteps = WorkflowStepInfoProvider.GetWorkflowSteps()
                                                              .Where(condition)
                                                              .Column("StepID")
                                                              .WhereEquals("StepWorkflowType", (int)WorkflowTypeEnum.Automation);

        // Get all pending contacts from automation state where status is Pending and current user is the owner
        var allPendingContacts = AutomationStateInfoProvider.GetAutomationStates()
                                                            .WhereIn("StateStepID", automationWorkflowSteps)
                                                            .WhereEquals("StateStatus", (int)ProcessStatusEnum.Pending)
                                                            .WhereEquals("StateObjectType", ContactInfo.OBJECT_TYPE);

        var contactIDs = ContactInfoProvider.GetContacts()
                                            .Column("ContactID")
                                            .Where(contactsWhereCondition);
        if (ShowOnlyMyPendingContacts)
        {
            contactIDs.WhereEquals("ContactOwnerUserID", user.UserID);
        }

        return allPendingContacts.WhereIn("StateObjectID", contactIDs.AsMaterializedList("ContactID"));
    }


    /// <summary>
    /// Returns site condition for given site id.
    /// </summary>
    /// <param name="siteID">Site id</param>
    private static WhereCondition GetSiteCondition(int siteID)
    {
        var condition = new WhereCondition();

        switch (siteID)
        {
            case UniSelector.US_GLOBAL_RECORD:
                condition.WhereNull("StateSiteID");
                break;

            case UniSelector.US_ALL_RECORDS:
            case UniSelector.US_NONE_RECORD:
                break;

            default:
                condition.WhereEquals("StateSiteID", siteID);
                break;
        }

        return condition;
    }


    protected object listElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        CMSGridActionButton btn;
        switch (sourceName.ToLowerCSafe())
        {
            // Set visibility for edit button
            case "edit":
                if (IsWidget)
                {
                    btn = sender as CMSGridActionButton;
                    if (btn != null)
                    {
                        btn.Visible = false;
                    }
                }
                break;

            // Set visibility for dialog edit button
            case "dialogedit":
                btn = sender as CMSGridActionButton;
                if (btn != null)
                {
                    btn.Visible = IsWidget;
                }
                break;

            case "view":
                btn = (CMSGridActionButton)sender;
                // Ensure accountID parameter value;
                var objectID = ValidationHelper.GetInteger(btn.CommandArgument, 0);
                // Contact detail URL
                string contactURL = UIContextHelper.GetElementDialogUrl(ModuleName.ONLINEMARKETING, "EditContact", objectID, "isSiteManager=" + ContactHelper.IsSiteManager);
                // Add modal dialog script to onClick action
                btn.OnClientClick = ScriptHelper.GetModalDialogScript(contactURL, "ContactDetail");
                break;

            // Delete action
            case "delete":
                int siteId = SiteID;

                if (SiteID == UniSelector.US_GLOBAL_AND_SITE_RECORD)
                {
                    DataRowView drv = (parameter as GridViewRow).DataItem as DataRowView;
                    int contactSiteId = ValidationHelper.GetInteger(drv["StateSiteID"], 0);
                    if (contactSiteId > 0)
                    {
                        siteId = contactSiteId;
                    }
                }

                btn = (CMSGridActionButton)sender;
                btn.OnClientClick = "if(!confirm(" + ScriptHelper.GetString(string.Format(ResHelper.GetString("autoMenu.RemoveStateConfirmation"), HTMLHelper.HTMLEncode(TypeHelper.GetNiceObjectTypeName(ContactInfo.OBJECT_TYPE).ToLowerCSafe()))) + ")) { return false; }" + btn.OnClientClick;
                if (!WorkflowStepInfoProvider.CanUserRemoveAutomationProcess(CurrentUser, SiteInfoProvider.GetSiteName(siteId)))
                {
                    if (btn != null)
                    {
                        btn.Enabled = false;
                    }
                }
                break;
        }

        return null;
    }

    #endregion
}
