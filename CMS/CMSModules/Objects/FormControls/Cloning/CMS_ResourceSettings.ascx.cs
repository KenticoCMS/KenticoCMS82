using System;
using System.Data;
using System.Collections;

using CMS.FormControls;
using CMS.Base;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;
using CMS.Modules;

public partial class CMSModules_Objects_FormControls_Cloning_CMS_ResourceSettings : CloneSettingsControl
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
    /// Excluded child types
    /// </summary>
    public override string ExcludedChildTypes
    {
        get
        {
            return PermissionNameInfo.OBJECT_TYPE_RESOURCE;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        lblClonePermissions.ToolTip = GetString("clonning.settings.resource.permissions.tooltip");
    }


    /// <summary>
    /// Returns properties hashtable.
    /// </summary>
    private Hashtable GetProperties()
    {
        Hashtable result = new Hashtable();
        result[ResourceInfo.OBJECT_TYPE + ".permissions"] = chkClonePermissions.Checked;
        return result;
    }

    #endregion
}