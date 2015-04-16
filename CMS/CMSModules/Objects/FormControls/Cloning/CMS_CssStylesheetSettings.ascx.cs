using System;
using System.Collections;

using CMS.FormControls;
using CMS.Base;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;
using CMS.PortalEngine;

public partial class CMSModules_Objects_FormControls_Cloning_CMS_CssStylesheetSettings : CloneSettingsControl
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

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        lblFiles.ToolTip = GetString("clonning.settings.layouts.appthemesfolder.tooltip");
    }


    /// <summary>
    /// Returns properties hashtable.
    /// </summary>
    private Hashtable GetProperties()
    {
        Hashtable result = new Hashtable();
        result[CssStylesheetInfo.OBJECT_TYPE + ".appthemes"] = chkFiles.Checked;
        return result;
    }

    #endregion
}