using System;
using System.Collections;

using CMS.Helpers;
using CMS.PortalEngine;
using CMS.Base;
using CMS.UIControls;

public partial class CMSModules_Objects_FormControls_Cloning_CMS_InlineControlSettings : CloneSettingsControl
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
        lblFiles.ToolTip = GetString("clonning.settings.inlinecontrol.files.tooltip");
        lblFileName.ToolTip = GetString("clonning.settings.inlinecontrol.filename.tooltip");

        if (!RequestHelper.IsPostBack())
        {
            InlineControlInfo control = InfoToClone as InlineControlInfo;
            if (control != null)
            {
                txtFileName.Text = FileHelper.GetUniqueFileName(control.ControlFileName);
            }
        }

        if (SystemContext.IsPrecompiledWebsite)
        {
            txtFileName.Text = "";
            chkFiles.Checked = false;

            txtFileName.Enabled = chkFiles.Enabled = false;
            txtFileName.ToolTip = chkFiles.ToolTip = GetString("general.copyfiles.precompiled");
        }
    }


    /// <summary>
    /// Returns properties hashtable.
    /// </summary>
    private Hashtable GetProperties()
    {
        Hashtable result = new Hashtable();
        result[InlineControlInfo.OBJECT_TYPE + ".filename"] = txtFileName.Text;
        result[InlineControlInfo.OBJECT_TYPE + ".files"] = chkFiles.Checked;
        return result;
    }

    #endregion
}