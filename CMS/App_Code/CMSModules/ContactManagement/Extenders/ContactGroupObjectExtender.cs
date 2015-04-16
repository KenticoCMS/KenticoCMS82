using System;
using System.Linq;

using CMS;
using CMS.Base;
using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.Helpers;
using CMS.OnlineMarketing;

[assembly: RegisterCustomClass("ContactGroupObjectExtender", typeof(ContactGroupObjectExtender))]

/// <summary>
/// Extender for editing or adding new contact group.
/// </summary>
public class ContactGroupObjectExtender : ControlExtender<UIForm>
{
    private ContactGroupInfo ContactGroup
    {
        get
        {
            return Control.EditedObject as ContactGroupInfo;
        }
    }


    private bool FieldIsDynamicCondition
    {
        get
        {
            return Control.FieldControls["ContactGroupIsDynamicCondition"].Value.ToBoolean(false);
        }
        set
        {
            Control.FieldControls["ContactGroupIsDynamicCondition"].Value = value;
        }
    }


    private bool FieldIsAutomaticallyRebuilt
    {
        get
        {
            return Control.FieldControls["ContactGroupIsAutomaticallyRebuilt"].Value.ToBoolean(false);
        }
        set
        {
            Control.FieldControls["ContactGroupIsAutomaticallyRebuilt"].Value = value;
        }
    }


    private string FieldDynamicCondition
    {
        get
        {
            return Control.FieldControls["ContactGroupDynamicCondition"].Value.ToString("");
        }
    } 


    public override void OnInit()
    {
        Control.Load += ControlLoad;
    }


    /// <summary>
    /// Sets control handlers and change submit button state.
    /// </summary>
    private void ControlLoad(object sender, EventArgs e)
    {
        if (ContactGroup == null)
        {
            return;
        }
        
        Control.OnBeforeSave += Control_OnBeforeSave;
        Control.OnAfterSave += Control_OnAfterSave;
        Control.PreRender += CheckMacros;
        Control.OnBeforeValidate += CheckRebuilding;

        ComponentEvents.RequestEvents.RegisterForEvent("recalculate", SetSubmitButtonStatus);
        ComponentEvents.RequestEvents.RegisterForEvent(ComponentEvents.SAVE, SetContactGroupStatus);
        
        // Checks whether the contact group is rebuilding. If so, disables the save button.
        Control.SubmitButton.Enabled = ContactGroup.ContactGroupStatus != ContactGroupStatusEnum.Rebuilding;
    }


    /// <summary>
    /// Disables save button when recalculation button is hit.
    /// </summary>
    private void SetSubmitButtonStatus(object sender, EventArgs e)
    {
        Control.SubmitButton.Enabled = false;
    }


    /// <summary>
    /// Checks if contact group is rebuilding. If so, stop processing of form and show proper error.
    /// </summary>
    private void CheckRebuilding(object sender, EventArgs e)
    {
        if (ContactGroup.ContactGroupStatus == ContactGroupStatusEnum.Rebuilding)
        {
            Control.StopProcessing = true;
            Control.ShowError(ResHelper.GetString("om.contactgroup.alreadyrebuilding"));
        }
    }


    /// <summary>
    /// Sets contact group status to ConditionChanged if condition value differs from its original value.
    /// Fires, when save button is clicked.
    /// </summary>
    private void SetContactGroupStatus(object sender, EventArgs e)
    {
        // If dynamic condition has changed, change the status as well. Is used when displaying the status message
        if (FieldDynamicCondition != ContactGroup.ContactGroupDynamicCondition)
        {
            ContactGroup.ContactGroupStatus = ContactGroupStatusEnum.ConditionChanged;
        }
    }


    /// <summary>
    /// Check whether all macros in contact group dynamic condition are optimized.
    /// Show warning when aren't.
    /// </summary>
    private void CheckMacros(object sender, EventArgs e)
    {
        if (ContactGroup == null || RequestHelper.IsAsyncPostback())
        {
            return;
        }

        if (!string.IsNullOrEmpty(ContactGroup.ContactGroupDynamicCondition))
        {
            var macroTree = CachedMacroRuleTrees.GetParsedTree(ContactGroup.ContactGroupDynamicCondition);
            if ((macroTree == null) || !MacroRuleTreeAnalyzer.CanTreeBeTranslated(macroTree))
            {
                Control.ShowWarning(ResHelper.GetString("om.macros.macro.slow"));
            }
        }
    }


    private void Control_OnBeforeSave(object sender, EventArgs e)
    {
        // If dynamic condition field is left empty, contact group will be considered as the static one
        if (string.IsNullOrEmpty(FieldDynamicCondition))
        {
            FieldIsDynamicCondition = false;
        }
        
        // If contact group is not meant to be dynamic, delete all content in macro condition rule
        if (!FieldIsDynamicCondition)
        {
            FieldIsAutomaticallyRebuilt = false;
            ContactGroup.ContactGroupDynamicCondition = null;
        }
    }


    private void Control_OnAfterSave(object sender, EventArgs e)
    {
        if (FieldIsAutomaticallyRebuilt)
        {
            ContactGroupRebuildTaskManager.EnableScheduledTask(ContactGroup);
        }
        else
        {
            ContactGroupRebuildTaskManager.DisableScheduledTask(ContactGroup);
        }
    }
}
