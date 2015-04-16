using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.FormControls;
using CMS.Helpers;

public partial class CMSModules_Settings_FormControls_SelectPermissions : FormEngineUserControl
{
    private string permissions = "";


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
            drpSelectPermissions.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return ValidationHelper.GetString(drpSelectPermissions.SelectedValue, "");
        }
        set
        {
            permissions = ValidationHelper.GetString(value, "");
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
    /// Returns ClientID of the DropDownList with permissions.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            return drpSelectPermissions.ClientID;
        }
    }


    /// <summary>
    /// Loads drop down list with data.
    /// </summary>
    private void ReloadData()
    {
        if (drpSelectPermissions.Items.Count == 0)
        {
            drpSelectPermissions.Items.Add(new ListItem(GetString("settings.allpages"), "ALL"));
            drpSelectPermissions.Items.Add(new ListItem(GetString("settings.nopage"), "NO"));
            drpSelectPermissions.Items.Add(new ListItem(GetString("settings.securedareas"), "SECUREDAREAS"));
        }

        // Preselect value
        ListItem selectedItem = drpSelectPermissions.Items.FindByValue(permissions);
        if (selectedItem != null)
        {
            drpSelectPermissions.ClearSelection();
            selectedItem.Selected = true;
        }
        else
        {
            drpSelectPermissions.SelectedIndex = 0;
        }
    }
}