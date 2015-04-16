using CMS.UIControls;

public partial class CMSModules_Scheduler_FormControls_Cloning_CMS_ScheduledTaskSettings : CloneSettingsControl
{
    #region "Properties"

    /// <summary>
    /// Returns close script which should be run when cloning is sucessfully done.
    /// </summary>
    public override string CloseScript
    {
        get
        {
            return "wopener.RefreshGrid(); CloseDialog();";
        }
    }


    /// <summary>
    /// Hide control.
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