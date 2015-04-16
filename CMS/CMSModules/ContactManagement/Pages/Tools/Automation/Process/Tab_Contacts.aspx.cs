using System;

using CMS.Automation;
using CMS.Core;
using CMS.OnlineMarketing;
using CMS.Helpers;
using CMS.UIControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.Base;
using CMS.WorkflowEngine;
using CMS.SiteProvider;

[Security(Resource = ModuleName.ONLINEMARKETING, UIElements = "EditProcess;EditProcessContacts")]
public partial class CMSModules_ContactManagement_Pages_Tools_Automation_Process_Tab_Contacts : CMSAutomationPage
{
    #region "Variables"

    private int mProcessID = 0;

    #endregion


    #region "Properties"

    /// <summary>
    /// Current workflow ID
    /// </summary>
    public int ProcessID
    {
        get
        {
            if (mProcessID <= 0)
            {
                mProcessID = QueryHelper.GetInteger("processid", 0);
            }
            return mProcessID;
        }
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Set process identifier
        listContacts.ProcessID = ProcessID;
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
                    siteSelector.Visible = false;
                    siteOrGlobalSelector.SiteID = QueryHelper.GetInteger("siteid", SiteContext.CurrentSiteID);
                }

                listContacts.SiteID = siteOrGlobalSelector.SiteID;

                if (listContacts.SiteID == UniSelector.US_GLOBAL_AND_SITE_RECORD)
                {
                    lblWarnStart.Visible = true;
                }
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
            CurrentMaster.DisplaySiteSelectorPanel = true;

            if (siteSelector.SiteID == 0)
            {
                siteSelector.SiteID = SiteID;
            }

            siteOrGlobalSelector.Visible = false;
            listContacts.SiteID = siteSelector.SiteID;
        }

        // Add Refresh action button
        AddHeaderAction(new HeaderAction()
        {
            Text = GetString("general.Refresh"),
            RedirectUrl = "Tab_Contacts.aspx?processid=" + listContacts.ProcessID + (IsSiteManager ? "&issitemanager=1" : String.Empty) + "&siteid=" + listContacts.SiteID
        });

        ucSelector.UniSelector.DialogButton.ResourceString = "ma.automationprocess.select";

        InitContactSelector();
    }


    void UniSelector_OnItemsSelected(object sender, EventArgs e)
    {
        try
        {
            int contactId = ValidationHelper.GetInteger(ucSelector.Value, 0);
            AutomationManager manager = AutomationManager.GetInstance(CurrentUser);
            var infoObj = ContactInfoProvider.GetContactInfo(contactId);
            if (WorkflowStepInfoProvider.CanUserStartAutomationProcess(CurrentUser, SiteInfoProvider.GetSiteName(infoObj.ContactSiteID)))
            {
                using (CMSActionContext context = new CMSActionContext())
                {
                    context.AllowAsyncActions = false;

                    manager.StartProcess(infoObj, ProcessID);
                }
            }
        }
        catch (ProcessRecurrenceException ex)
        {
            ShowError(ex.Message);
        }
        catch (Exception ex)
        {
            LogAndShowError("Automation", "STARTPROCESS", ex);
        }

        listContacts.ReloadData();
        pnlUpdate.Update();
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Initializes contact selector.
    /// </summary>
    private void InitContactSelector()
    {
        // Initialize contact selector
        ucSelector.UniSelector.SelectionMode = SelectionModeEnum.SingleButton;

        WorkflowInfo process = WorkflowInfoProvider.GetWorkflowInfo(ProcessID);

        if (process == null)
        {
            RedirectToInformation("editedobject.notexists");            
        }

        // Check permissions
        if (WorkflowStepInfoProvider.CanUserStartAutomationProcess(CurrentUser, SiteInfoProvider.GetSiteName(listContacts.SiteID)) && (listContacts.SiteID != UniSelector.US_GLOBAL_AND_SITE_RECORD) && ((process != null) && process.WorkflowEnabled))
        {
            ucSelector.UniSelector.OnItemsSelected += UniSelector_OnItemsSelected;
            ucSelector.SiteID = listContacts.SiteID;
            ucSelector.IsLiveSite = false;
            ucSelector.IsSiteManager = ContactHelper.IsSiteManager;
            ucSelector.Enabled = true;
            ucSelector.UniSelector.DialogButton.ToolTipResourceString = "automenu.startstatedesc";
        }
        else
        {
            ucSelector.Enabled = false;
            ucSelector.UniSelector.DialogButton.ToolTipResourceString = process.WorkflowEnabled ? "general.nopermission" : "autoMenu.DisabledStateDesc";
        }
    }

    #endregion
}