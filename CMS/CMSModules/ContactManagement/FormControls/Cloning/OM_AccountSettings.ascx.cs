using System;
using System.Collections;

using CMS.FormControls;
using CMS.Base;
using CMS.OnlineMarketing;
using CMS.UIControls;

public partial class CMSModules_ContactManagement_FormControls_Cloning_OM_AccountSettings : CloneSettingsControl
{
    #region "Properties"

    /// <summary>
    /// Gets properties hashtable.
    /// </summary>
    public override Hashtable CustomParameters
    {
        get
        {
            return GetProperties();
        }
    }


    /// <summary>
    /// Excluded binding types.
    /// </summary>
    public override string ExcludedBindingTypes
    {
        get
        {
            return ContactGroupMemberInfo.OBJECT_TYPE_ACCOUNT;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        lblMerged.ToolTip = GetString("clonning.settings.account.merged.tooltip");
        lblSubsidiaries.ToolTip = GetString("clonning.settings.account.subsidiaries.tooltip");
    }


    /// <summary>
    /// Returns properties hashtable.
    /// </summary>
    private Hashtable GetProperties()
    {
        Hashtable result = new Hashtable();
        result[AccountInfo.OBJECT_TYPE + ".merged"] = chkMerged.Checked;
        result[AccountInfo.OBJECT_TYPE + ".subsidiaries"] = chkSubsidiaries.Checked;
        result[AccountInfo.OBJECT_TYPE + ".contactgroup"] = chkContactGroup.Checked;
        return result;
    }

    #endregion
}