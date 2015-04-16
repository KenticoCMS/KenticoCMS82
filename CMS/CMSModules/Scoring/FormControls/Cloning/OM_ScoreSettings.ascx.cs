using System;

using CMS.OnlineMarketing;
using CMS.UIControls;

public partial class CMSModules_Scoring_FormControls_Cloning_OM_ScoreSettings : CloneSettingsControl
{
    #region "Properties"

    /// <summary>
    /// Excluded other binding tasks.
    /// </summary>
    public override string ExcludedOtherBindingTypes
    {
        get
        {
            return ScoreContactRuleInfo.OBJECT_TYPE;
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
}