using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.Base;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;
using CMS.DataEngine;

public partial class CMSModules_BizForms_FormControls_SelectBizForm : FormEngineUserControl
{
    #region "Variables"

    private int mSiteID = 0;

    #endregion

    #region "Public properties"

    /// <summary>
    /// Gets Value display name.
    /// </summary>
    public override string ValueDisplayName
    {
        get
        {
            return uniSelector.ValueDisplayName;
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
            if (uniSelector != null)
            {
                uniSelector.Enabled = value;
            }
        }
    }


    /// <summary>
    /// Returns ClientID of the textbox with selected bizforms.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            return uniSelector.TextBoxSelect.ClientID;
        }
    }


    /// <summary>
    /// Gets or sets the field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return uniSelector.Value;
        }
        set
        {
            if (uniSelector == null)
            {
                pnlUpdate.LoadContainer();
            }
            uniSelector.Value = value;
        }
    }


    /// <summary>
    /// Gets or sets the ID of the site for which the bizforms should be returned. 0 means current site.
    /// </summary>
    public int SiteID
    {
        get
        {
            return mSiteID;
        }
        set
        {
            mSiteID = value;
        }
    }


    /// <summary>
    /// Gets or sets if site filter should be shown or not.
    /// </summary>
    public bool ShowSiteFilter
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowSiteFilter"), true);
        }
        set
        {
            SetValue("ShowSiteFilter", value);
        }
    }


    /// <summary>
    /// Gets the inner UniSelector control.
    /// </summary>
    public UniSelector UniSelector
    {
        get
        {
            return GetValue("UniSelector") as UniSelector;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Init(object sender, EventArgs e)
    {
        SetValue("UniSelector", uniSelector);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            uniSelector.StopProcessing = true;
        }
        else
        {
            // If current control context is widget or livesite hide site selector
            if (ControlsHelper.CheckControlContext(this, ControlContext.WIDGET_PROPERTIES) || ControlsHelper.CheckControlContext(this, ControlContext.LIVE_SITE))
            {
                ShowSiteFilter = false;
            }

            ReloadData();
        }
    }


    /// <summary>
    /// Reloads the data in the selector.
    /// </summary>
    public void ReloadData()
    {
        uniSelector.IsLiveSite = IsLiveSite;

        // Return form name or ID according to type of field (if no field specified form name is returned)
        if ((FieldInfo != null) && DataTypeManager.IsInteger(TypeEnum.Field, FieldInfo.DataType))
        {
            // Store old value
            object value = uniSelector.Value;
            uniSelector.ReturnColumnName = "FormID";
            uniSelector.SelectionMode = SelectionModeEnum.SingleDropDownList;
            ShowSiteFilter = false;
            uniSelector.AllowEmpty = true;
            // Reset previously saved value
            uniSelector.Value = value;
        }
        else
        {
            uniSelector.ReturnColumnName = "FormName";
        }

        // Add sites filter
        if (ShowSiteFilter)
        {
            uniSelector.FilterControl = "~/CMSFormControls/Filters/SiteFilter.ascx";
            uniSelector.SetValue("DefaultFilterValue", (SiteID > 0) ? SiteID : SiteContext.CurrentSiteID);
            uniSelector.SetValue("FilterMode", "bizform");
        }
        // Select bizforms depending on a site if not filtered by uniselector site filter
        else
        {
            int siteId = (SiteID == 0) ? SiteContext.CurrentSiteID : SiteID;
            uniSelector.WhereCondition = SqlHelper.AddWhereCondition(uniSelector.WhereCondition, "FormSiteID = " + siteId);
        }
    }


    /// <summary>
    /// Returns WHERE condition for selected form.
    /// </summary>
    public override string GetWhereCondition()
    {
        // Return correct WHERE condition for integer if none value is selected
        if ((FieldInfo != null) && DataTypeManager.IsInteger(TypeEnum.Field, FieldInfo.DataType))
        {
            int id = ValidationHelper.GetInteger(uniSelector.Value, 0);
            if (id > 0)
            {
                return base.GetWhereCondition();
            }
        }
        return null;
    }

    #endregion
}