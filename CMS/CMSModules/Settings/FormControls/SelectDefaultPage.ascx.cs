using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.FormControls;
using CMS.Helpers;
using CMS.DocumentEngine;

public partial class CMSModules_Settings_FormControls_SelectDefaultPage : FormEngineUserControl
{
    #region "Variables"

    private string defaultType = string.Empty;

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
            drpDefaultType.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return ValidationHelper.GetString(drpDefaultType.SelectedValue, "");
        }
        set
        {
            defaultType = ValidationHelper.GetString(value, "");
            ReloadData();
        }
    }


    /// <summary>
    /// Returns ClientID of the DropDownList with case check.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            return drpDefaultType.ClientID;
        }
    }

    #endregion


    #region "Methods"

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
        if (drpDefaultType.Items.Count == 0)
        {
            drpDefaultType.Items.Add(new ListItem(GetString("seodefaulttype.none"), DocumentURLProvider.DEFAUL_PAGE_NONE));
            drpDefaultType.Items.Add(new ListItem(GetString("seodefaulttype.domain"), DocumentURLProvider.DEFAUL_PAGE_DOMAIN));
            drpDefaultType.Items.Add(new ListItem(GetString("seodefaulttype.page"), DocumentURLProvider.DEFAUL_PAGE_PAGE));
            drpDefaultType.Items.Add(new ListItem(GetString("seodefaulttype.default"), DocumentURLProvider.DEFAUL_PAGE_DEFAULT));
        }

        // Preselect value
        ListItem selectedItem = drpDefaultType.Items.FindByValue(defaultType);
        if (selectedItem != null)
        {
            drpDefaultType.ClearSelection();
            selectedItem.Selected = true;
        }
        else
        {
            drpDefaultType.SelectedIndex = 0;
        }
    }


    protected override void CreateChildControls()
    {
        base.CreateChildControls();
        ReloadData();
    }

    #endregion
}
