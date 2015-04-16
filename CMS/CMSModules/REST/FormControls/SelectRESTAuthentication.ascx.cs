using System.Web.UI.WebControls;

using CMS.FormControls;
using CMS.Helpers;

public partial class CMSModules_REST_FormControls_SelectRESTAuthentication : FormEngineUserControl
{
    private string authentication = "";


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
            drpSelectAuthentication.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return ValidationHelper.GetString(drpSelectAuthentication.SelectedValue, "");
        }
        set
        {
            authentication = ValidationHelper.GetString(value, "");
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
    /// Returns ClientID of the DropDownList with authentication.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            return drpSelectAuthentication.ClientID;
        }
    }


    /// <summary>
    /// Loads drop down list with data.
    /// </summary>
    private void ReloadData()
    {
        if (drpSelectAuthentication.Items.Count == 0)
        {
            drpSelectAuthentication.Items.Add(new ListItem(GetString("rest.basicauthentication"), "basic"));
            drpSelectAuthentication.Items.Add(new ListItem(GetString("rest.formsauthentication"), "forms"));
        }

        // Preselect value
        ListItem selectedItem = drpSelectAuthentication.Items.FindByValue(authentication);
        if (selectedItem != null)
        {
            drpSelectAuthentication.ClearSelection();
            selectedItem.Selected = true;
        }
        else
        {
            drpSelectAuthentication.SelectedIndex = 0;
        }
    }
}