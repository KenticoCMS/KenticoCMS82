using System;

using CMS.DataEngine;
using CMS.Newsletters;
using CMS.UIControls;

public partial class CMSModules_Newsletters_FormControls_Cloning_Newsletter_IssueSettings : CloneSettingsControl
{
    #region "Properties"

    /// <summary>
    /// Excluded other binding tasks.
    /// </summary>
    public override string ExcludedOtherBindingTypes
    {
        get
        {
            return OpenedEmailInfo.OBJECT_TYPE;
        }
    }


    /// <summary>
    /// Excluded other binding tasks.
    /// </summary>
    public override string ExcludedChildTypes
    {
        get
        {
            return LinkInfo.OBJECT_TYPE;
        }
    }


    /// <summary>
    /// Hide the control
    /// </summary>
    public override bool DisplayControl
    {
        get
        {
            return false;
        }
    }

    #endregion


    /// <summary>
    /// Returns true if custom settings are valid against given clone setting.
    /// </summary>
    /// <param name="settings">Clone settings</param>
    public override bool IsValid(CloneSettings settings)
    {
        if (InfoToClone.GetBooleanValue("IssueIsABTest", false))
        {
            if (!settings.IncludeChildren || (settings.MaxRelativeLevel == 0))
            {
                // It is not possible to clone ABTest issue without its children, because children are variants of the AB Tests and it makes no sense to clone without variants
                ShowError(GetString("newsletters.cannotcloneabtestissuewithoutchildren"));

                return false;
            }
        }

        return true;
    }
}