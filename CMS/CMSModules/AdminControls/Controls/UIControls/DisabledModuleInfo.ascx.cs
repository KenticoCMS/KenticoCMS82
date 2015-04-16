using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.UIControls;
using CMS.Helpers;

public partial class CMSModules_AdminControls_Controls_UIControls_DisabledModuleInfo : CMSAbstractUIWebpart
{
    #region "Properties"

    /// <summary>
    /// Set keys for module check.
    /// </summary>
    public String SettingKeys
    {
        get
        {
            return GetStringContextValue("SettingKeys");
        }
        set
        {
            SetValue("SettingKeys", value);
        }
    }


    /// <summary>
    /// Set keys for module check.
    /// </summary>
    public DisabledModuleScope KeyScope
    {
        get
        {
            return GetStringContextValue("KeyScope", "Both").ToEnum<DisabledModuleScope>();
        }
        set
        {
            SetValue("SettingKeys", value);
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!String.IsNullOrEmpty(SettingKeys))
        {
            dModule.SettingsKeys = SettingKeys;
            dModule.KeyScope = KeyScope;
        }
        else
        {
            Visible = false;
            StopProcessing = true;
        }
    }

    #endregion
}
