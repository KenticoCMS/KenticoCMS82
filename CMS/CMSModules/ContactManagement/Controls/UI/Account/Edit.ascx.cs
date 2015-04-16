using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

using CMS.Core;
using CMS.Helpers;
using CMS.Membership;
using CMS.OnlineMarketing;
using CMS.Base;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.FormControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.DataEngine;
using CMS.PortalEngine;

public partial class CMSModules_ContactManagement_Controls_UI_Account_Edit : CMSAdminEditControl
{
    #region "Variables"

    private int mSiteID;
    private AccountInfo ai;
    private AccountInfo parentAccount;
    private HeaderAction btnSplit;

    #endregion


    #region "Properties"

    /// <summary>
    /// Event that fires after saving the form.
    /// </summary>
    public event EventHandler OnAfterSave
    {
        add
        {
            EditForm.OnAfterSave += value;
        }
        remove
        {
            EditForm.OnAfterSave -= value;
        }
    }


    /// <summary>
    /// SiteID of current account.
    /// </summary>
    public int SiteID
    {
        get
        {
            return mSiteID;
        }
        set
        {
            mSiteID = value;

            if ((mSiteID > 0) && !MembershipContext.AuthenticatedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin))
            {
                mSiteID = SiteContext.CurrentSiteID;
            }

            DistributeParams();
        }
    }

    #endregion


    #region "Page events"

    /// <summary>
    /// OnInit event handler.
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        EditForm.OnAfterDataLoad += EditForm_OnAfterDataLoad;
        EditForm.OnBeforeSave += EditForm_OnBeforeSave;
    }


    /// <summary>
    /// OnAfterDataLoad event handler.
    /// </summary>
    protected void EditForm_OnAfterDataLoad(object sender, EventArgs e)
    {
        ai = (AccountInfo)EditForm.EditedObject;

        if ((EditForm.EditedObject != null) && (ai.AccountID != 0))
        {
            SiteID = ValidationHelper.GetInteger(EditForm.Data["AccountSiteID"], 0);
        }

        // AccountStatusSelector
        SetControl("accountstatusid", ctrl => ctrl.SetValue("siteid", SiteID));
    }


    /// <summary>
    /// OnBeforeSave event handler.
    /// </summary>
    private void EditForm_OnBeforeSave(object sender, EventArgs e)
    {
        // Set site ID
        if (SiteID > 0)
        {
            EditForm.Data["AccountSiteID"] = SiteID;
        }
        else
        {
            EditForm.Data["AccountSiteID"] = null;
        }

        // Repairs (none) selector value
        string[] editforms = { "AccountCountryID", "accountprimarycontactid", "accountsecondarycontactid", "accountsubsidiaryofid", "accountstatusid" };
        foreach (string editFormName in editforms)
        {
            if (ValidationHelper.GetInteger(EditForm.Data[editFormName], 0) <= 0)
            {
                EditForm.Data[editFormName] = null;
            }
        }
        
        int subsidiaryID = ValidationHelper.GetInteger(EditForm.EditedObject.GetValue("accountsubsidiaryofid"), -1);

        // When subsidiary account does not exist anymore, reset UI form Subsidiary of field
        if (AccountInfoProvider.GetAccountInfo(subsidiaryID) == null)
        {
            EditForm.EditedObject.SetValue("accountsubsidiaryofid", null);
            ((UniSelector)EditForm.FieldControls["accountsubsidiaryofid"]).Reload(true);
        }

        AssignContacts();
    }


    /// <summary>
    /// OnAfterSave event handler.
    /// </summary>
    protected void EditForm_OnAfterSave(object sender, EventArgs e)
    {
        AccountInfo account = (AccountInfo)EditForm.EditedObject;

        // Refresh breadcrumbs
        ScriptHelper.RefreshTabHeader(Page, account.AccountDescriptiveName);
    }


    /// <summary>
    /// Page_Load event handler.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        InitHeaderActions();
        SetButtonsVisibility();

        // Initialize redirection URL
        string url = UIContextHelper.GetElementUrl(ModuleName.ONLINEMARKETING, "EditAccount", false);
        url = URLHelper.AddParameterToUrl(url, "objectid", "{%EditedObject.ID%}");
        url = URLHelper.AddParameterToUrl(url, "siteid", SiteID.ToString());
        if (ContactHelper.IsSiteManager)
        {
            url = URLHelper.AddParameterToUrl(url, "issitemanager", "1");
        }
        url = URLHelper.AddParameterToUrl(url, "saved", "1");

        EditForm.RedirectUrlAfterCreate = url;

        // Connect role selector and contact selector
        ((UniSelector)EditForm.FieldControls["accountprimarycontactid"]).OnSelectionChanged += (s, ea) => SetContactRoleID((FormEngineUserControl)s, EditForm.FieldControls["accountprimarycontactroleid"]);
        ((UniSelector)EditForm.FieldControls["accountsecondarycontactid"]).OnSelectionChanged += (s, ea) => SetContactRoleID((FormEngineUserControl)s, EditForm.FieldControls["accountsecondarycontactroleid"]);
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Reload values in contact role selectors
        SetContactRoleID(EditForm.FieldControls["accountprimarycontactid"], EditForm.FieldControls["accountprimarycontactroleid"]);
        SetContactRoleID(EditForm.FieldControls["accountsecondarycontactid"], EditForm.FieldControls["accountsecondarycontactroleid"]);

        // Hide primary contact field when no contacts are in account. Other fields in Contacts region has visibility condition based on this field.
        if (!((UniSelector)EditForm.FieldControls["accountprimarycontactid"]).HasData)
        {
            EditForm.FieldControls["accountprimarycontactid"].Visible = false;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Sets contact role ID to given role selector control based on contact selector value and edited account.
    /// </summary>
    private void SetContactRoleID(FormEngineUserControl contactSelector, FormEngineUserControl roleSelector)
    {
        int contactID = ValidationHelper.GetInteger(contactSelector.Value, 0);
        int accountID = ai.AccountID;
        var accountContactInfo = AccountContactInfoProvider.GetAccountContactInfo(accountID, contactID);
        roleSelector.Value = (accountContactInfo != null) ? accountContactInfo.ContactRoleID : UniSelector.US_NONE_RECORD;
    }


    /// <summary>
    /// Initializes header action control.
    /// </summary>
    private void InitHeaderActions()
    {
        // Initialize SPLIT button
        btnSplit = btnSplit ?? new HeaderAction
        {
            Text = GetString("om.contact.splitfromparent"),
            CommandName = "split",
            CommandArgument = "false",
        };

        HeaderActions.AddAction(btnSplit);
        HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;
    }


    /// <summary>
    /// Actions handler.
    /// </summary>
    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        // Check permission
        AccountHelper.AuthorizedModifyAccount(SiteID, true);

        switch (e.CommandName.ToLowerCSafe())
        {
            // Save account
            case "save":
                if (EditForm.SaveData(null))
                {
                    SetButtonsVisibility();
                }

                break;

            // Split from parent account
            case "split":
                var mergedAccount = (AccountInfo)UIContext.EditedObject;
                List<AccountInfo> mergedAccountList = new List<AccountInfo>(1) { mergedAccount };
                AccountHelper.Split(parentAccount, mergedAccountList, false, false, false);
                
                SetButtonsVisibility();
                ShowConfirmation(GetString("om.account.splitted"));
                ScriptHelper.RefreshTabHeader(Page, mergedAccount.AccountDescriptiveName);
                break;
        }
    }


    /// <summary>
    /// Sets visibility of buttons that are connected to merged account - split button and link to his parent.
    /// </summary>
    private void SetButtonsVisibility()
    {
        // Find out if current account is merged into another site or account contact
        bool mergedIntoSite = ValidationHelper.GetInteger(EditForm.Data["AccountMergedWithAccountID"], 0) != 0;
        bool mergedIntoGlobal = ValidationHelper.GetInteger(EditForm.Data["AccountGlobalAccountID"], 0) != 0
                                && AccountHelper.AuthorizedReadAccount(UniSelector.US_GLOBAL_RECORD, false);
        bool globalAccountsVisible = SettingsKeyInfoProvider.GetBoolValue(SiteContext.CurrentSiteName + ".CMSCMGlobalAccounts") || CurrentUser.IsGlobalAdministrator;

        if ((mergedIntoGlobal && globalAccountsVisible) || mergedIntoSite)
        {
            // Get parent account
            if (mergedIntoSite)
            {
                parentAccount = AccountInfoProvider.GetAccountInfo(ValidationHelper.GetInteger(EditForm.Data["AccountMergedWithAccountID"], 0));
                headingMergedInto.ResourceString = "om.account.mergedintosite";
            }
            else
            {
                parentAccount = AccountInfoProvider.GetAccountInfo(ValidationHelper.GetInteger(EditForm.Data["AccountGlobalAccountID"], 0));
                headingMergedInto.ResourceString = "om.account.mergedintoglobal";
            }

            lblMergedIntoAccountName.Text = HTMLHelper.HTMLEncode(parentAccount.AccountName.Trim());

            string accountDetailDialogURL = UIContextHelper.GetElementDialogUrl(ModuleName.ONLINEMARKETING, "EditAccount", parentAccount.AccountID, "isSiteManager=" + ContactHelper.IsSiteManager);
            string openDialogScript = ScriptHelper.GetModalDialogScript(accountDetailDialogURL, "AccountDetail");

            btnMergedAccount.IconCssClass = "icon-edit";
            btnMergedAccount.OnClientClick = openDialogScript;
            btnMergedAccount.ToolTip = GetString("om.account.viewdetail");
        }
        else
        {
            panelMergedAccountDetails.Visible = btnSplit.Visible = false;            
        }
    }


    /// <summary>
    /// Sets primary and secondary contacts.
    /// </summary>
    private void AssignContacts()
    {
        ContactInfo contact;
        AccountContactInfo accountContact;

        // Assign primary contact to account and/or assign role
        int contactID = ValidationHelper.GetInteger(EditForm.EditedObject.GetValue("accountprimarycontactid"), -1);
        int contactRoleID = ValidationHelper.GetInteger(EditForm.FieldControls["accountprimarycontactroleid"].Value, -1);

        if (contactID > 0)
        {
            contact = ContactInfoProvider.GetContactInfo(contactID);
            if (contact != null)
            {
                accountContact = AccountContactInfoProvider.GetAccountContactInfo(ai.AccountID, contactID);

                // Update relation
                if (accountContact != null)
                {
                    accountContact.ContactRoleID = contactRoleID;
                    AccountContactInfoProvider.SetAccountContactInfo(accountContact);
                }
                else
                {
                    EditForm.EditedObject.SetValue("accountprimarycontactid", null);
                    ((UniSelector)EditForm.FieldControls["accountprimarycontactid"]).Reload(true);
                }
            }
            // Selected contact doesn't exist
            else
            {
                ShowError(GetString("om.contact.primarynotexists"));
                return;
            }
        }

        // Assign secondary contact to account and/or assign role
        contactID = ValidationHelper.GetInteger(EditForm.EditedObject.GetValue("accountsecondarycontactid"), -1);
        contactRoleID = ValidationHelper.GetInteger(EditForm.FieldControls["accountsecondarycontactroleid"].Value, -1);

        // Assign secondary contact to account and/or assign role
        if (contactID > 0)
        {
            contact = ContactInfoProvider.GetContactInfo(contactID);
            if (contact != null)
            {
                accountContact = AccountContactInfoProvider.GetAccountContactInfo(ai.AccountID, contactID);

                // Update relation
                if (accountContact != null)
                {
                    accountContact.ContactRoleID = contactRoleID;
                    AccountContactInfoProvider.SetAccountContactInfo(accountContact);
                }
                else
                {
                    EditForm.EditedObject.SetValue("accountsecondarycontactid", null);
                    ((UniSelector)EditForm.FieldControls["accountsecondarycontactid"]).Reload(true);
                }
            }
            else
            {
                ShowError(GetString("om.contact.secondarynotexists"));
            }
        }
    }


    /// <summary>
    /// Distributes SiteID and other parameters to form controls.
    /// </summary>
    private void DistributeParams(object sender = null, EventArgs eventArgs = null)
    {
        if (EditForm.FieldControls == null)
        {
            // Try to call that later, if controls are not initialized yet
            EditForm.Load += DistributeParams;
            return;
        }

        // ContactRoleSelector
        SetControl("AccountPrimaryContactRoleID", ctrl => ctrl.SetValue("siteid", SiteID));
        // ContactRoleSelector
        SetControl("AccountSecondaryContactRoleID", ctrl => ctrl.SetValue("siteid", SiteID));
        // UniSelector
        SetControl("accountstatusid", ctrl =>
            {
                ctrl.SetValue("siteid", SiteID);
                if (SiteID > 0)
                {
                    ctrl.SetValue("wherecondition", "AccountStatusSiteID = " + SiteID + " OR AccountStatusSiteID IS NULL");
                }
                else
                {
                    ctrl.SetValue("wherecondition", "AccountStatusSiteID IS NULL");
                }
            });
        // UniSelector
        SetControl("accountsubsidiaryofid", ctrl =>
            {
                ctrl.SetValue("siteid", SiteID);
                if (SiteID > 0)
                {
                    ctrl.SetValue("wherecondition", "(AccountID NOT IN (SELECT * FROM Func_OM_Account_GetSubsidiaries(" + ValidationHelper.GetInteger(EditForm.EditedObject.GetValue("AccountID"), 0) + ", 1)) AND AccountSiteID = " + SiteID + ")");
                }
                else
                {
                    ctrl.SetValue("wherecondition", "(AccountID NOT IN (SELECT * FROM Func_OM_Account_GetSubsidiaries(" + ValidationHelper.GetInteger(EditForm.EditedObject.GetValue("AccountID"), 0) + ", 1)) AND AccountSiteID IS NULL)");
                }
            });
        // UserSelector
        SetControl("accountowneruserid", ctrl =>
            {
                ctrl.SetValue("siteid", SiteID);
                if (SiteID <= 0)
                {
                    ctrl.SetValue("wherecondition", "UserName NOT LIKE N'public'");
                }
            });
        // UniSelector
        SetControl("accountprimarycontactid", ctrl =>
            {
                ctrl.SetValue("siteid", SiteID);
                ctrl.SetValue("wherecondition", "(ContactID IN (SELECT ContactID FROM OM_AccountContact WHERE AccountID = " + ValidationHelper.GetInteger(EditForm.EditedObject.GetValue("AccountID"), 0) + "))");

            });
        // UniSelector
        SetControl("accountsecondarycontactid", ctrl =>
            {
                ctrl.SetValue("siteid", SiteID);
                ctrl.SetValue("wherecondition", "(ContactID IN (SELECT ContactID FROM OM_AccountContact WHERE AccountID = " + ValidationHelper.GetInteger(EditForm.EditedObject.GetValue("AccountID"), 0) + "))");
            });
    }


    /// <summary>
    /// Performs an action on found control.
    /// </summary>
    private void SetControl(string controlName, Action<FormEngineUserControl> action)
    {
        var control = EditForm.FieldControls[controlName];
        if (control != null)
        {
            action(control);
        }
    }

    #endregion
}