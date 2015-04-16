using System;

using CMS.Automation;
using CMS.Core;
using CMS.OnlineMarketing;
using CMS.Helpers;
using CMS.WorkflowEngine;
using CMS.Base;
using CMS.UIControls;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.DataEngine;

[EditedObject("om.contact", "objectid")]
[Security(Resource = ModuleName.ONLINEMARKETING, UIElements = "EditContact;ContactProcesses")]
public partial class CMSModules_ContactManagement_Pages_Tools_Contact_Tab_Processes : CMSContactManagementContactsPage
{
    #region "Properties"

    /// <summary>
    /// Currently edited contact.
    /// </summary>
    public ContactInfo Contact 
    { 
        get
        {
            return (ContactInfo)EditedObject;
        }
    }

    #endregion

    
    #region "Methods"
    
    /// <summary>
    /// Whether current user is authorized to edit the contact.
    /// </summary>
    /// <returns>True if so</returns>
    private bool IsAuthorized()
    {
        bool isSiteAuthorized = MembershipContext.AuthenticatedUser.IsAuthorizedPerResource(ModuleName.CONTACTMANAGEMENT, "ReadContacts", SiteInfoProvider.GetSiteName(Contact.ContactSiteID));
        bool isGlobalAuthorized = MembershipContext.AuthenticatedUser.IsAuthorizedPerResource(ModuleName.CONTACTMANAGEMENT, "ReadGlobalContacts", SiteInfoProvider.GetSiteName(Contact.ContactSiteID));
        bool isContactGlobal = Contact.ContactSiteID == 0;

        return isSiteAuthorized || (isContactGlobal && isGlobalAuthorized);
    }

    #endregion


    #region "Event handlers"

    /// <summary>
    /// PreInit event handler
    /// </summary>
    protected override void OnPreInit(EventArgs e)
    {
        RequiresDialog = false;
        base.OnPreInit(e);
    }


    protected void Page_Init(object sender, EventArgs e)
    {
        if (Contact == null)
        {
            RedirectToAccessDenied(GetString("general.invalidparameters"));
        }

        if (!IsAuthorized())
        {
            RedirectToAccessDenied(GetString("general.nopermission"));
        }

        // Initialize process selector
        ucSelector.UniSelector.SelectionMode = SelectionModeEnum.SingleButton;

        // Check permissions
        if (WorkflowStepInfoProvider.CanUserStartAutomationProcess(CurrentUser, SiteInfoProvider.GetSiteName(Contact.ContactSiteID)))
        {
            ucSelector.UniSelector.OnItemsSelected += UniSelector_OnItemsSelected;
            ucSelector.UniSelector.SetValue("SiteID", SiteID);
            ucSelector.UniSelector.SetValue("IsLiveSite", false);
            ucSelector.IsSiteManager = ContactHelper.IsSiteManager;
            ucSelector.Enabled = true;
        }
        else
        {
            ucSelector.Enabled = false;
        }


        listElem.ObjectID = Contact.ContactID;
        listElem.ObjectType = ContactInfo.OBJECT_TYPE;
        listElem.EditActionUrl = "Process_Detail.aspx?stateid={0}" + (IsSiteManager ? "&issitemanager=1" : String.Empty);
    }


    void UniSelector_OnItemsSelected(object sender, EventArgs e)
    {
        try
        {
            int processId = ValidationHelper.GetInteger(ucSelector.Value, 0);
            AutomationManager manager = AutomationManager.GetInstance(CurrentUser);
            var infoObj = BaseAbstractInfoProvider.GetInfoById(listElem.ObjectType, listElem.ObjectID);
            using (CMSActionContext context = new CMSActionContext())
            {
                context.AllowAsyncActions = false;

                manager.StartProcess(infoObj, processId);
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

        listElem.UniGrid.ReloadData();
        pnlUpdate.Update();
    }

    #endregion
}
