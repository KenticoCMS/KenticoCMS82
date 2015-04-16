using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.UI;

using CMS;
using CMS.Base;
using CMS.ExtendedControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.FormControls;
using CMS.Helpers;
using CMS.OnlineMarketing;
using CMS.UIControls;

[assembly: RegisterCustomClass("ContactGroupGeneralExtender", typeof(ContactGroupGeneralExtender))]

public class ContactGroupGeneralExtender : PageExtender<CMSPage>
{
    private ContactGroupInfo ContactGroup
    {
        get
        {
            return (ContactGroupInfo)Page.EditedObject;
        }
    }


    private HeaderAction RebuildHeaderAction
    {
        get
        {
            var rebuiltHeaderAction = Page.HeaderActions.ActionsList.SingleOrDefault(c => c.CommandName == "recalculate");
            if (rebuiltHeaderAction == null)
            {
                rebuiltHeaderAction = new HeaderAction
                {
                    CommandName = "recalculate",
                    CommandArgument = "false",
                    ButtonStyle = ButtonStyle.Default,
                    Tooltip = ResHelper.GetString("om.contactgroup.rebuild.tooltip")
                };

                Page.HeaderActions.AddAction(rebuiltHeaderAction);
            }

            return rebuiltHeaderAction;
        }
    }


    private FormEngineUserControl StatusLabel
    {
        get
        {
            AbstractUserControl statusLabel = Page.HeaderActions.AdditionalControls.SingleOrDefault(c => c.ID == "statusLabel");
            if (statusLabel == null)
            {
                statusLabel = (AbstractUserControl)Page.LoadUserControl("~/CMSFormControls/Basic/LabelControl.ascx");
                statusLabel.ID = "statusLabel";

                Page.HeaderActions.AdditionalControls.Add(statusLabel);

                Page.HeaderActions.AdditionalControlsCssClass = "header-actions-label control-group-inline";
                Page.HeaderActions.ReloadAdditionalControls();
            }

            return (FormEngineUserControl)statusLabel;
        }
    }


    /// <summary>
    /// Initializes the extender.
    /// </summary>
    public override void OnInit()
    {
        Page.Load += Page_Load;
        Page.PreRender += Page_PreRender;
    }


    /// <summary>
    /// Initializes both recalculation button and the status label. 
    /// </summary>
    void Page_PreRender(object sender, EventArgs e)
    {
        if (ContactGroup != null)
        {
            UpdateRecalculationButton();
            UpdateStatus();
        }
    }


    /// <summary>
    /// Registers to the header actions events.
    /// </summary>
    private void Page_Load(object sender, EventArgs e)
    {
        // Update panel timer is located in webpart nested in the page. 
        // Use mode always to update buttons and status with the number of contacts as well.
        Page.HeaderActions.UpdatePanel.UpdateMode = UpdatePanelUpdateMode.Always;

        ComponentEvents.RequestEvents.RegisterForEvent("recalculate", RecalculateContactGroup);
    }


    /// <summary>
    /// Updates status text according to the contact group status.
    /// </summary>
    private void UpdateStatus()
    {
        bool isRebuilding = ContactGroup.ContactGroupStatus == ContactGroupStatusEnum.Rebuilding;
        StatusLabel.Visible = !isRebuilding && !string.IsNullOrEmpty(ContactGroup.ContactGroupDynamicCondition);
        SetContactGroupStatus();
    }


    /// <summary>
    /// Updates text and state of the button according to the contact group status.
    /// </summary>
    private void UpdateRecalculationButton()
    {
        RebuildHeaderAction.Visible = !string.IsNullOrEmpty(ContactGroup.ContactGroupDynamicCondition);

        bool isRebuilding = ContactGroup.ContactGroupStatus == ContactGroupStatusEnum.Rebuilding;
        string buttonText = isRebuilding ?
            ResHelper.GetString("om.contactgroup.rebuilding") :
            ResHelper.GetString("om.contactgroup.rebuild");

        RebuildHeaderAction.Enabled = !isRebuilding;
        RebuildHeaderAction.Text = buttonText;
    }


    /// <summary>
    /// Sets status according to the current contact group status state.
    /// </summary>
    private void SetContactGroupStatus()
    {
        switch (ContactGroup.ContactGroupStatus)
        {
            case ContactGroupStatusEnum.Ready:
                // 'Ready' status
                StatusLabel.Value = "<span class=\"StatusEnabled\">" + ResHelper.GetString("om.contactgroup.rebuildnotrequired") + "</span>";
                break;

            case ContactGroupStatusEnum.ConditionChanged:
                // 'Condition changed' status
                StatusLabel.Value = "<span class=\"StatusDisabled\">" + ResHelper.GetString("om.contactgroup.rebuildrequired") + "</span>";
                break;
            default:
                // In other cases do not display any status
                StatusLabel.Value = "";
                break;
        }
    }


    /// <summary>
    /// Performs contact group recalculation.
    /// Fires, when recalculate button is clicked.
    /// </summary>
    private void RecalculateContactGroup(object sender, EventArgs e)
    {
        if (ContactGroup == null || ContactGroup.ContactGroupStatus == ContactGroupStatusEnum.Rebuilding)
        {
            return;
        }

        if (ContactGroupHelper.AuthorizedModifyContactGroup(ContactGroup.ContactGroupSiteID, true))
        {
            RebuildHeaderAction.Text = ResHelper.GetString("om.contactgroup.rebuilding");
            RebuildHeaderAction.Enabled = false;
            StatusLabel.Visible = false;

            // Set status that the contact group is being rebuilt
            ContactGroup.ContactGroupStatus = ContactGroupStatusEnum.Rebuilding;
            ContactGroupInfoProvider.SetContactGroupInfo(ContactGroup);

            // Evaluate the membership of the contact group
            ContactGroupEvaluator evaluator = new ContactGroupEvaluator();
            evaluator.ContactGroup = ContactGroup;

            Task.Factory.StartNew(CMSThread.Wrap(evaluator.Run), TaskCreationOptions.LongRunning);
        }
    }
}