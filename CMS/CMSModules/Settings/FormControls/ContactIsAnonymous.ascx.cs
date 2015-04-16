using System;
using System.Web.UI.WebControls;

using CMS.FormControls;

public partial class CMSModules_Settings_FormControls_ContactIsAnonymous : FormEngineUserControl
{
    private string mValue;

    /// <summary>
    /// Value of the control.
    /// </summary>
    public override object Value
    {
        get
        {
            return list.SelectedValue;
        }
        set
        {
            mValue = value as string;
            ReloadData();
        }
    }


    /// <summary>
    /// Gets or sets a value indicating whether the Web server control is enabled.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return list.Enabled;
        }
        set
        {
            list.Enabled = value;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        ReloadData();
    }


    /// <summary>
    /// Reloads control.
    /// </summary>
    public void ReloadData()
    {
        if (list.Items.Count == 0)
        {
            list.Items.Add(new ListItem(GetString("om.contact.doesntmatter"), "0"));
            list.Items.Add(new ListItem(GetString("om.contact.isanonymous"), "1"));
            list.Items.Add(new ListItem(GetString("om.contact.notanonymous"), "2"));
        }

        ListItem selectedItem = list.Items.FindByValue(mValue);
        if (selectedItem != null)
        {
            list.ClearSelection();
            selectedItem.Selected = true;
        }
        else
        {
            list.SelectedIndex = 0;
        }
    }
}