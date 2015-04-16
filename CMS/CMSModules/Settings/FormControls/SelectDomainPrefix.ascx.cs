using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.FormControls;
using CMS.Helpers;
using CMS.DocumentEngine;

public partial class CMSModules_Settings_FormControls_SelectDomainPrefix : FormEngineUserControl
{
    #region "Variables"

    private string domainPrefix = string.Empty;

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
            drpDomainPrefix.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return ValidationHelper.GetString(drpDomainPrefix.SelectedValue, "");
        }
        set
        {
            domainPrefix = ValidationHelper.GetString(value, "");
            ReloadData();
        }
    }


    /// <summary>
    /// Returns ClientID of the CMSDropDownList with case check.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            return drpDomainPrefix.ClientID;
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
        if (drpDomainPrefix.Items.Count == 0)
        {
            drpDomainPrefix.Items.Add(new ListItem(GetString("seodomainprefix.none"), DocumentURLProvider.DOMAIN_PREFIX_NONE));
            drpDomainPrefix.Items.Add(new ListItem(GetString("seodomainprefix.withwww"), DocumentURLProvider.DOMAIN_PREFIX_WWW));
            drpDomainPrefix.Items.Add(new ListItem(GetString("seodomainprefix.withoutwww"), DocumentURLProvider.DOMAIN_PREFIX_WITHOUTWWW));
        }

        // Preselect value
        ListItem selectedItem = drpDomainPrefix.Items.FindByValue(domainPrefix);
        if (selectedItem != null)
        {
            drpDomainPrefix.ClearSelection();
            selectedItem.Selected = true;
        }
        else
        {
            drpDomainPrefix.SelectedIndex = 0;
        }
    }


    protected override void CreateChildControls()
    {
        base.CreateChildControls();
        ReloadData();
    }

    #endregion
}
