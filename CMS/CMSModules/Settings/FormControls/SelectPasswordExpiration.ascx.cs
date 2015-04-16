using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.FormControls;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.Membership;

public partial class CMSModules_Settings_FormControls_SelectPasswordExpiration : FormEngineUserControl
{
    #region "Variables"

    private string expiration = "";

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
            base.Enabled = value;
            drpSelectExpiration.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return ValidationHelper.GetString(drpSelectExpiration.SelectedValue, "");
        }
        set
        {
            expiration = ValidationHelper.GetString(value, "");
            ReloadData();
        }
    }


    /// <summary>
    /// Returns ClientID of the CMSDropDownList with order.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            return drpSelectExpiration.ClientID;
        }
    }

    #endregion


    #region "Control methods"

    protected override void CreateChildControls()
    {
        base.CreateChildControls();
        ReloadData();
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
        if (drpSelectExpiration.Items.Count == 0)
        {
            drpSelectExpiration.Items.Add(new ListItem(GetString("settings.passwordexpiration.showwarning"), AuthenticationHelper.PASSWORD_EXPIRATION_WARNING));
            drpSelectExpiration.Items.Add(new ListItem(GetString("settings.passwordexpiration.lockaccount"), AuthenticationHelper.PASSWORD_EXPIRATION_LOCK));
        }

        // Preselect value
        ListItem selectedItem = drpSelectExpiration.Items.FindByValue(expiration);
        if (selectedItem != null)
        {
            drpSelectExpiration.ClearSelection();
            selectedItem.Selected = true;
        }
        else
        {
            drpSelectExpiration.SelectedIndex = 0;
        }
    }

    #endregion   
}