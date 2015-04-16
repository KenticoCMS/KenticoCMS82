using System;
using System.Web.UI.WebControls;

using CMS.Controls;
using CMS.DataEngine;
using CMS.EmailEngine;
using CMS.ExtendedControls;
using CMS.Helpers;

public partial class CMSModules_EmailTemplates_Filters_EmailTemplateTypeFilter : CMSAbstractBaseFilterControl
{
    #region "Public properties"

    /// <summary>
    /// Where condition.
    /// </summary>
    public override string WhereCondition
    {
        get
        {
            if (!string.IsNullOrEmpty(drpEmailType.SelectedValue))
            {
                base.WhereCondition = new WhereCondition()
                    .WhereEquals("EmailTemplateType", drpEmailType.SelectedValue)
                    .ToString(true);
            }
            return base.WhereCondition;
        }
    }


    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return drpEmailType.SelectedValue;
        }
        set
        {
            drpEmailType.SelectedValue = ValidationHelper.GetString(value, string.Empty);
        }
    }


    /// <summary>
    /// Drop down control
    /// </summary>
    public CMSDropDownList DropDown
    {
        get
        {
            return drpEmailType;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Init(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            // Fill the e-mail type enumeration
            ReloadData();
        }
    }


    /// <summary>
    /// Reloads the data in the selector.
    /// </summary>
    public void ReloadData()
    {
        if (drpEmailType.Items.Count == 0)
        {
            drpEmailType.Items.Add(new ListItem(ResHelper.GetString("general.selectall"), string.Empty));
            ControlsHelper.FillListControlWithEnum<EmailTemplateTypeEnum>(drpEmailType, "emailtemplate.type", true, true);
        }
    }


    /// <summary>
    /// Reset filter.
    /// </summary>
    public override void ResetFilter()
    {
        drpEmailType.SelectedIndex = 0;
    }

    #endregion
}