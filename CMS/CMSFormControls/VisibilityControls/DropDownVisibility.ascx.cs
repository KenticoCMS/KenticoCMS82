using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.FormControls;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.LicenseProvider;
using CMS.PortalEngine;
using CMS.Base;
using CMS.DataEngine;
using CMS.SiteProvider;

public partial class CMSFormControls_VisibilityControls_DropDownVisibility : FormEngineUserControl
{
    #region "Public properties"

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
            drpVisibility.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return ValidationHelper.GetString(drpVisibility.SelectedValue, null);
        }
        set
        {
            EnsureChildControls();
            drpVisibility.SelectedValue = ValidationHelper.GetString(value, null);
        }
    }

    #endregion


    #region "Methods"

    protected override void CreateChildControls()
    {
        base.CreateChildControls();
        if (!StopProcessing)
        {
            ReloadData();
        }
    }


    /// <summary>
    /// Loads the child controls at run-time.
    /// </summary>
    private void ReloadData()
    {
        drpVisibility.Items.Add(new ListItem(GetString("visibility.none"), FormFieldVisibilityTypeEnum.None.ToStringRepresentation()));
        drpVisibility.Items.Add(new ListItem(GetString("visibility.all"), FormFieldVisibilityTypeEnum.All.ToStringRepresentation()));
        drpVisibility.Items.Add(new ListItem(GetString("visibility.authenticated"), FormFieldVisibilityTypeEnum.Authenticated.ToStringRepresentation()));

        // Add friends if friends is available
        if (LicenseHelper.CheckFeature(RequestContext.CurrentDomain, FeatureEnum.Friends))
        {
            if (PortalContext.ViewMode != ViewModeEnum.LiveSite || UIHelper.IsFriendsModuleEnabled(SiteContext.CurrentSiteName))
            {
                drpVisibility.Items.Add(new ListItem(GetString("visibility.friends"), FormFieldVisibilityTypeEnum.Friends.ToStringRepresentation()));
            }
        }
    }

    #endregion
}