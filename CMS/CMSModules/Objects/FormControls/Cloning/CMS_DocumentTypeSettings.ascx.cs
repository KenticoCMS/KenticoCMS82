using System;
using System.Collections;

using CMS.FormControls;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.Base;
using CMS.DataEngine;
using CMS.UIControls;
using CMS.IO;

public partial class CMSModules_Objects_FormControls_Cloning_CMS_DocumentTypeSettings : CloneSettingsControl
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
    /// Excluded child types.
    /// </summary>
    public override string ExcludedChildTypes
    {
        get
        {
            return AlternativeFormInfo.OBJECT_TYPE;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        lblTableName.ToolTip = GetString("clonning.settings.class.tablename.tooltip");
        lblIcons.ToolTip = GetString("clonning.settings.documenttype.icons.tooltip");
        lblCloneAlternativeForms.ToolTip = GetString("clonning.settings.class.alternativeform");

        if (!RequestHelper.IsPostBack())
        {
            TableManager tm = new TableManager(null);

            string originalTableName = InfoToClone.GetStringValue("ClassTableName", "");
            if (string.IsNullOrEmpty(originalTableName))
            {
                plcTableName.Visible = false;
                plcAlternativeForms.Visible = false;
            }
            else
            {
                txtTableName.Text = tm.GetUniqueTableName(InfoToClone.GetStringValue("ClassTableName", ""));
            }
        }
    }


    /// <summary>
    /// Returns properties hashtable.
    /// </summary>
    private Hashtable GetProperties()
    {
        Hashtable result = new Hashtable();
        result[DataClassInfo.OBJECT_TYPE + ".data"] = false;
        result[DataClassInfo.OBJECT_TYPE + ".tablename"] = txtTableName.Text;
        result[DataClassInfo.OBJECT_TYPE + ".alternativeforms"] = chkCloneAlternativeForms.Checked;

        if (chkIcons.Checked)
        {
            // Set the icon names to copy
            string iconPath = Server.MapPath(GetImagePath("/DocumentTypeIcons/"));
            string fileName = TranslationHelper.GetSafeClassName(((DataClassInfo)InfoToClone).ClassName);
            string smallIcon = iconPath + fileName;
            string largeIcon = iconPath + "48x48\\" + fileName;

            result[DataClassInfo.OBJECT_TYPE_DOCUMENTTYPE + ".icons"] = smallIcon + ".png;" + smallIcon + ".gif;" + largeIcon + ".png;" + largeIcon + ".gif";
        }
        return result;
    }

    #endregion
}