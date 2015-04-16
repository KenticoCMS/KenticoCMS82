using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.FormControls;
using CMS.Helpers;

public partial class CMSModules_Settings_FormControls_ContactForVisitor : FormEngineUserControl
{
    private string mValue = "";


    protected void Page_Load(object sender, EventArgs e)
    {
        ReloadData();
    }


    /// <summary>
    /// Gets ClientID of the dropdownlist with stylesheets.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            return drpContact.ClientID;
        }
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
            drpContact.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return ValidationHelper.GetString(drpContact.SelectedValue, "");
        }
        set
        {
            mValue = ValidationHelper.GetString(value, "");
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
    /// Loads drop down list with data.
    /// </summary>
    private void ReloadData()
    {
        if (drpContact.Items.Count == 0)
        {
            drpContact.Items.Add(new ListItem(GetString("om.contact.lastcontact"), "last"));
            drpContact.Items.Add(new ListItem(GetString("om.contact.mostactive"), "active"));
            drpContact.Items.Add(new ListItem(GetString("om.contact.createnew"), "new"));
        }

        ListItem selectedItem = drpContact.Items.FindByValue(mValue);
        if (selectedItem != null)
        {
            drpContact.ClearSelection();
            selectedItem.Selected = true;
        }
        else
        {
            drpContact.SelectedIndex = 0;
        }
    }
}