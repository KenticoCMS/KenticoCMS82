using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Web.UI.WebControls;

using CMS.Core;
using CMS.DataEngine;
using CMS.ExtendedControls.ActionsConfig;
using CMS.FormControls;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.OnlineMarketing;
using CMS.Base;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.ExtendedControls;

[Guid("8EF3FF11-429C-4438-B922-A0884CED307F")]
public partial class CMSModules_ContactManagement_Controls_UI_Contact_Edit : CMSAdminEditControl
{
    #region "Variables"

    private int mSiteID;
    private ContactInfo parentContact;
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
    /// SiteID of current contact.
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

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        EditForm.OnBeforeDataLoad += EditForm_OnBeforeDataLoad;
        EditForm.OnAfterDataLoad += EditForm_OnAfterDataLoad;
        EditForm.OnBeforeSave += EditForm_OnBeforeSave;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        InitHeaderActions();
        SetButtonsVisibility();

        // Initialize redirection URL
        var url = UIContextHelper.GetElementUrl(ModuleName.ONLINEMARKETING, "EditContact");
        url = URLHelper.AddParameterToUrl(url, "objectid", "{%EditedObject.ID%}");
        url = URLHelper.AddParameterToUrl(url, "displaytitle", "0");
        url = URLHelper.AddParameterToUrl(url, "saved", "1");
        url = URLHelper.AddParameterToUrl(url, "siteid", SiteID.ToString());

        if (ContactHelper.IsSiteManager)
        {
            url = URLHelper.AddParameterToUrl(url, "issitemanager", "1");
        }
        EditForm.RedirectUrlAfterCreate = url;
    }

    #endregion


    #region "UIform events"

    /// <summary>
    /// OnBeforeDataLoad event handler.
    /// </summary>
    protected void EditForm_OnBeforeDataLoad(object sender, EventArgs e)
    {
        if (EditForm.FormInformation != null)
        {
            FormFieldInfo campaignField = EditForm.FormInformation.GetFormField("ContactCampaign");
            if (campaignField != null)
            {
                campaignField.Visible = MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.WebAnalytics", "read");
            }
        }
    }


    /// <summary>
    /// OnAfterDataLoad event handler.
    /// </summary>
    protected void EditForm_OnAfterDataLoad(object sender, EventArgs e)
    {
        if ((EditForm.EditedObject != null) && (ValidationHelper.GetInteger(EditForm.EditedObject.GetValue("ContactID"), 0) != 0))
        {
            SiteID = ValidationHelper.GetInteger(EditForm.Data["ContactSiteID"], 0);
        }

        // ContactStatusSelector
        SetControl("contactstatusid", ctrl => ctrl.SetValue("siteid", SiteID));
    }


    /// <summary>
    /// OnBeforeSave event handler.
    /// </summary>
    private void EditForm_OnBeforeSave(object sender, EventArgs e)
    {
        // Set site ID
        if (SiteID > 0)
        {
            EditForm.Data["ContactSiteID"] = SiteID;
        }
        else
        {
            EditForm.Data["ContactSiteID"] = null;
        }

        // Repairs (none) selector value
        // UniSelector returns '0' if (none) is selected, but 'null' is required
        string[] fieldNames = { "ContactCountryID", "ContactStatusID" };
        foreach (string fieldName in fieldNames)
        {
            if (ValidationHelper.GetInteger(EditForm.Data[fieldName], 0) <= 0)
            {
                EditForm.Data[fieldName] = null;
            }
        }
    }


    /// <summary>
    /// OnAfterSave event handler.
    /// </summary>
    protected void EditForm_OnAfterSave(object sender, EventArgs e)
    {
        ContactInfo contact = (ContactInfo)EditForm.EditedObject;

        // Refresh breadcrumbs
        ScriptHelper.RefreshTabHeader(Page, contact.ContactDescriptiveName);
    }

    #endregion


    #region "Methods"

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
            ButtonStyle = ButtonStyle.Default,
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
        ContactHelper.AuthorizedModifyContact(SiteID, true);

        switch (e.CommandName.ToLowerCSafe())
        {
            // Save contact
            case "save":
                if (EditForm.SaveData(null))
                {
                    SetButtonsVisibility();
                }
                break;

            // Split from parent contact
            case "split":
            {
                var mergedContact = (ContactInfo)UIContext.EditedObject;
                List<ContactInfo> mergedContactList = new List<ContactInfo>(1) { mergedContact };

                ContactHelper.SplitContacts(parentContact, mergedContactList, false, false, false);
                SetButtonsVisibility();
                ShowConfirmation(GetString("om.contact.splitted"));
                ScriptHelper.RefreshTabHeader(Page, mergedContact.ContactDescriptiveName);
            }
            break;
        }
    }


    /// <summary>
    /// Sets visibility of buttons that are connected to merged contact - split button and link to his parent.
    /// </summary>
    private void SetButtonsVisibility()
    {
        // Find out if current contact is merged into another site or global contact
        bool mergedIntoSite = ValidationHelper.GetInteger(EditForm.Data["ContactMergedWithContactID"], 0) != 0;
        bool mergedIntoGlobal = ValidationHelper.GetInteger(EditForm.Data["ContactGlobalContactID"], 0) != 0
                                && ContactHelper.AuthorizedReadContact(UniSelector.US_GLOBAL_RECORD, false);
        bool globalContactsVisible = SettingsKeyInfoProvider.GetBoolValue(SiteContext.CurrentSiteName + ".CMSCMGlobalContacts") || CurrentUser.IsGlobalAdministrator;

        if (mergedIntoSite || (mergedIntoGlobal && globalContactsVisible))
        {
            if (mergedIntoSite)
            {
                parentContact = ContactInfoProvider.GetContactInfo(ValidationHelper.GetInteger(EditForm.Data["ContactMergedWithContactID"], 0));
                headingMergedInto.ResourceString = "om.contact.mergedintosite";
            }
            else
            {
                parentContact = ContactInfoProvider.GetContactInfo(ValidationHelper.GetInteger(EditForm.Data["ContactGlobalContactID"], 0));
                headingMergedInto.ResourceString = "om.contact.mergedintoglobal";
            }

            lblMergedIntoContactName.Text = HTMLHelper.HTMLEncode(ContactInfoProvider.GetContactFullName(parentContact));

            string contactDetailDialogURL = UIContextHelper.GetElementDialogUrl(ModuleName.ONLINEMARKETING, "EditContact", parentContact.ContactID);
            string openDialogScript = ScriptHelper.GetModalDialogScript(contactDetailDialogURL, "ContactDetail");

            btnMergedContact.IconCssClass = "icon-edit";
            btnMergedContact.OnClientClick = openDialogScript;
            btnMergedContact.ToolTip = GetString("om.contact.viewdetail");
        }
        else
        {
            panelMergedContactDetails.Visible = btnSplit.Visible = false;
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

        // UserSelector
        SetControl("contactowneruserid", ctrl =>
            {
                ctrl.SetValue("siteid", SiteID);
                if (SiteID <= 0)
                {
                    ctrl.SetValue("wherecondition", "UserName NOT LIKE N'public'");
                }
            });

        // CampaignSelector
        SetControl("contactcampaign", ctrl =>
            {
                ctrl.SetValue("siteid", SiteID);
                ctrl.SetValue("nonerecordvalue", string.Empty);
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