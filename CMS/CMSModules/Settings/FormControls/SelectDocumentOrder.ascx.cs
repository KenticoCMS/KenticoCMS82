using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.FormControls;
using CMS.Helpers;

public partial class CMSModules_Settings_FormControls_SelectDocumentOrder : FormEngineUserControl
{
    private string order = "";


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
            drpSelectOrder.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return ValidationHelper.GetString(drpSelectOrder.SelectedValue, "");
        }
        set
        {
            order = ValidationHelper.GetString(value, "");
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
    /// Returns ClientID of the CMSDropDownList with order.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            return drpSelectOrder.ClientID;
        }
    }


    /// <summary>
    /// Loads drop down list with data.
    /// </summary>
    private void ReloadData()
    {
        if (drpSelectOrder.Items.Count == 0)
        {
            drpSelectOrder.Items.Add(new ListItem(GetString("settings.alphabetical"), "ALPHABETICAL"));
            drpSelectOrder.Items.Add(new ListItem(GetString("settings.first"), "FIRST"));
            drpSelectOrder.Items.Add(new ListItem(GetString("settings.last"), "LAST"));
        }

        // Preselect value
        ListItem selectedItem = drpSelectOrder.Items.FindByValue(order);
        if (selectedItem != null)
        {
            drpSelectOrder.ClearSelection();
            selectedItem.Selected = true;
        }
        else
        {
            drpSelectOrder.SelectedIndex = 0;
        }
    }
}