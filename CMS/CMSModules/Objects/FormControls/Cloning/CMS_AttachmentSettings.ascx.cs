using System;
using System.Collections;

using CMS.Helpers;
using CMS.Base;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;
using CMS.DocumentEngine;
using CMS.IO;
using CMS.DataEngine;

public partial class CMSModules_Objects_FormControls_Cloning_CMS_AttachmentSettings : CloneSettingsControl
{
    #region "Properties"

    /// <summary>
    /// Gets properties hashtable
    /// </summary>
    public override Hashtable CustomParameters
    {
        get
        {
            return GetProperties();
        }
    }


    /// <summary>
    /// Hides the code name
    /// </summary>
    public override bool HideCodeName
    {
        get
        {
            return true;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!RequestHelper.IsPostBack())
        {
            AttachmentInfo ai = (AttachmentInfo)InfoToClone;
            string uniqueName = ai.Generalized.GetUniqueName(ai.AttachmentName, 0, ai.Generalized.CodeNameColumn, "_{0}" + ai.AttachmentExtension, "([_](\\d+))?\\" + ai.AttachmentExtension +"$", true);
            txtFileName.Text = Path.GetFileNameWithoutExtension(uniqueName);
        }
    }


    /// <summary>
    /// Returns true if custom settings are valid against given clone setting.
    /// </summary>
    /// <param name="settings">Clone settings</param>
    public override bool IsValid(CloneSettings settings)
    {
        if (!ValidationHelper.IsFileName(txtFileName.Text))
        {
            ShowError(GetString("general.invalidfilename"));
            return false;
        }
        return true;
    }


    /// <summary>
    /// Returns properties hashtable.
    /// </summary>
    private Hashtable GetProperties()
    {
        Hashtable result = new Hashtable();
        result[AttachmentInfo.OBJECT_TYPE + ".filename"] = txtFileName.Text;
        return result;
    }

    #endregion
}