using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.FormControls;
using CMS.Helpers;

public partial class CMSModules_Settings_FormControls_SelectCaseCheck : FormEngineUserControl
{
    private string caseCheck = "";


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
            drpSelectCaseCheck.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return ValidationHelper.GetString(drpSelectCaseCheck.SelectedValue, "");
        }
        set
        {
            caseCheck = ValidationHelper.GetString(value, "");
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
    /// Returns ClientID of the DropDownList with case check.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            return drpSelectCaseCheck.ClientID;
        }
    }


    /// <summary>
    /// Loads drop down list with data.
    /// </summary>
    private void ReloadData()
    {
        if (drpSelectCaseCheck.Items.Count == 0)
        {
            drpSelectCaseCheck.Items.Add(new ListItem(GetString("urlcasecheck.none"), "NONE"));
            drpSelectCaseCheck.Items.Add(new ListItem(GetString("urlcasecheck.exact"), "EXACT"));
            drpSelectCaseCheck.Items.Add(new ListItem(GetString("urlcasecheck.lowercase"), "LOWERCASE"));
            drpSelectCaseCheck.Items.Add(new ListItem(GetString("urlcasecheck.uppercase"), "UPPERCASE"));
        }

        // Preselect value
        ListItem selectedItem = drpSelectCaseCheck.Items.FindByValue(caseCheck);
        if (selectedItem != null)
        {
            drpSelectCaseCheck.ClearSelection();
            selectedItem.Selected = true;
        }
        else
        {
            drpSelectCaseCheck.SelectedIndex = 0;
        }
    }
}