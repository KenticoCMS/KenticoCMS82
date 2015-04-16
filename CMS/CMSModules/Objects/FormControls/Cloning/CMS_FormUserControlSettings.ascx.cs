using System;
using System.Collections;

using CMS.Base;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.UIControls;

public partial class CMSModules_Objects_FormControls_Cloning_CMS_FormUserControlSettings : CloneSettingsControl
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
        if (!RequestHelper.IsPostBack())
        {
            FormUserControlInfo control = InfoToClone as FormUserControlInfo;
            if (control != null)
            {
                if (!String.IsNullOrEmpty(control.UserControlFileName) && !control.UserControlFileName.EqualsCSafe("inherited", true))
                {
                    txtFileName.Text = FileHelper.GetUniqueFileName(control.UserControlFileName);
                }
            }
        }
    }


    /// <summary>
    /// Returns properties hashtable.
    /// </summary>
    private Hashtable GetProperties()
    {
        Hashtable result = new Hashtable();
        result[FormUserControlInfo.OBJECT_TYPE + ".filename"] = txtFileName.Text;
        result[FormUserControlInfo.OBJECT_TYPE + ".files"] = chkFiles.Checked;
        return result;
    }

    #endregion
}