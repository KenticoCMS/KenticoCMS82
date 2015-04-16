using System;

using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.Helpers;
using CMS.UIControls;
using CMS.Base;

public partial class CMSModules_Settings_FormControls_SettingsKeyDefaultValue : FormEngineUserControl
{
    /// <summary>
    /// Gets or sets the default value of the setting.
    /// </summary>
    public override object Value
    {
        get
        {
            if (chkKeyValue.Visible)
            {
                return chkKeyValue.Checked;
            }
            else
            {
                return txtKeyValue.Text;
            }
        }
        set
        {
            txtKeyValue.Text = ValidationHelper.GetString(value, "");
            chkKeyValue.Checked = ValidationHelper.GetBoolean(value, false);
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            // Do nothing
        }
        else
        {
            SelectDefaultValueControl();
        }

    }


    /// <summary>
    /// Shows suitable default value edit control accordint to key type.
    /// </summary>
    private void SelectDefaultValueControl()
    {
        chkKeyValue.Visible = ValidationHelper.GetString(Form.GetFieldValue("KeyType"), "").EqualsCSafe("boolean");
        txtKeyValue.Visible = !chkKeyValue.Visible;
    }
}
