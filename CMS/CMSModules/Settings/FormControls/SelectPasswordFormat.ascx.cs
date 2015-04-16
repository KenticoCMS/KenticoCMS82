using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.FormControls;
using CMS.Helpers;

public partial class CMSModules_Settings_FormControls_SelectPasswordFormat : FormEngineUserControl
{
    private string format = "";


    protected void Page_Load(object sender, EventArgs e)
    {
        //ReloadData();
    }


    protected override void CreateChildControls()
    {
        base.CreateChildControls();
        ReloadData();
    }


    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return base.Enabled;
        }
        set
        {
            base.Enabled = value;
            drpSelectFormat.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return ValidationHelper.GetString(drpSelectFormat.SelectedValue, "");
        }
        set
        {
            format = ValidationHelper.GetString(value, "");
            ReloadData();
        }
    }


    /// <summary>
    /// Returns true if user control is valid.
    /// </summary>
    public override bool IsValid()
    {
        return true;
    }


    /// <summary>
    /// Returns ClientID of the CMSDropDownList with format.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            return drpSelectFormat.ClientID;
        }
    }


    /// <summary>
    /// Loads drop down list with data.
    /// </summary>
    private void ReloadData()
    {
        if (drpSelectFormat.Items.Count == 0)
        {
            drpSelectFormat.Items.Add(new ListItem(GetString("general.plaintext"), ""));
            drpSelectFormat.Items.Add(new ListItem(GetString("settings.sha1"), "SHA1"));
            drpSelectFormat.Items.Add(new ListItem(GetString("settings.sha2"), "SHA2"));
            drpSelectFormat.Items.Add(new ListItem(GetString("settings.sha2salt"), "SHA2SALT"));
        }

        // Preselect value
        ListItem selectedItem = drpSelectFormat.Items.FindByValue(format);
        if (selectedItem != null)
        {
            drpSelectFormat.ClearSelection();
            selectedItem.Selected = true;
        }
        else
        {
            drpSelectFormat.SelectedIndex = 0;
        }
    }
}