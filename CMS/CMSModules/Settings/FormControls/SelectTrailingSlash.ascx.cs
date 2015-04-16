using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.FormControls;
using CMS.Helpers;

public partial class CMSModules_Settings_FormControls_SelectTrailingSlash : FormEngineUserControl
{
    private string slash = "";


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
            drpSlash.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return ValidationHelper.GetString(drpSlash.SelectedValue, "");
        }
        set
        {
            slash = ValidationHelper.GetString(value, "");
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
    /// Returns ClientID of the DropDownList with the value.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            return drpSlash.ClientID;
        }
    }


    /// <summary>
    /// Loads drop down list with data.
    /// </summary>
    private void ReloadData()
    {
        if (drpSlash.Items.Count == 0)
        {
            drpSlash.Items.Add(new ListItem(GetString("trailingslashcheck.dontcare"), "DONTCARE"));
            drpSlash.Items.Add(new ListItem(GetString("trailingslashcheck.always"), "ALWAYS"));
            drpSlash.Items.Add(new ListItem(GetString("trailingslashcheck.never"), "NEVER"));
        }

        // Preselect value
        ListItem selectedItem = drpSlash.Items.FindByValue(slash);
        if (selectedItem != null)
        {
            drpSlash.ClearSelection();
            selectedItem.Selected = true;
        }
        else
        {
            drpSlash.SelectedIndex = 0;
        }
    }
}