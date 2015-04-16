using System;
using System.Web.UI.WebControls;

using CMS.FormControls;
using CMS.Helpers;

public partial class CMSModules_Settings_FormControls_SelectPOPAuthentication : FormEngineUserControl
{
    #region "Variables"

    private string authentication = string.Empty;

    #endregion


    #region "Properties"

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
            base.Enabled = drpSelectAuthentication.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return ValidationHelper.GetString(drpSelectAuthentication.SelectedValue, string.Empty);
        }
        set
        {
            authentication = ValidationHelper.GetString(value, string.Empty);
            ReloadData();
        }
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

    #endregion


    #region "Methods"

    /// <summary>
    /// Called by the ASP.NET page framework to notify server controls that use composition-based
    /// implementation to create any child controls they contain in preparation for posting back or rendering.
    /// </summary>
    protected override void CreateChildControls()
    {
        base.CreateChildControls();
        ReloadData();
    }


    /// <summary>
    /// Returns true if entered data is valid. If data is invalid, it returns false and displays an error message.
    /// </summary>
    /// <returns>true, if entered data is valid, otherwise false</returns>
    public override bool IsValid()
    {
        return true;
    }


    /// <summary>
    /// Loads a drop down list with available authentication methods.
    /// </summary>
    private void ReloadData()
    {
        if (drpSelectAuthentication.Items.Count == 0)
        {
            drpSelectAuthentication.Items.Add(new ListItem(GetString("settings.username"), "USERNAME"));
            drpSelectAuthentication.Items.Add(new ListItem(GetString("settings.apop"), "APOP"));
            drpSelectAuthentication.Items.Add(new ListItem(GetString("settings.cram_md5"), "CRAM-MD5"));
            drpSelectAuthentication.Items.Add(new ListItem(GetString("settings.auto"), "AUTO"));
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

    #endregion
}