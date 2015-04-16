using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;
using System.Web.UI;

using CMS.FormControls;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.Controls;

public partial class CMSFormControls_System_UserControlTypeSelector : CMSAbstractBaseFilterControl
{
    #region "Variables"

    private string mSelectedValue = String.Empty;
    private bool mIncludeAllItem = true;
    private string mDataType = null;
    private FieldEditorControlsEnum mFieldEditorControls = FieldEditorControlsEnum.All;
    private bool mIsPrimary = false;

    #endregion


    #region "Properties"

    /// <summary>
    /// Where condition.
    /// </summary>
    public override string WhereCondition
    {
        get
        {
            // Retrieve WHERE condition according to filter
            string where = null;
            if (ValidationHelper.GetInteger(Value, -1) >= 0)
            {
                where = "UserControlType = " + ((int)ControlType).ToString();
            }

            base.WhereCondition = where;

            return base.WhereCondition;
        }
        set
        {
            base.WhereCondition = value;
        }
    }



    /// <summary>
    /// Gets the collection of arbitrary attributes (for rendering only) that do
    /// not correspond to properties on the control.
    /// </summary>
    public new AttributeCollection Attributes
    {
        get
        {
            return drpType.Attributes;
        }
    }


    /// <summary>
    /// Gets or sets the field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return drpType.SelectedValue;
        }
        set
        {
            mSelectedValue = (string)value;
            if (drpType.Items.FindByValue(mSelectedValue) != null)
            {
                drpType.SelectedValue = mSelectedValue;
            }
        }
    }


    /// <summary>
    /// Gets or sets the field value.
    /// </summary>
    public FormUserControlTypeEnum ControlType
    {
        get
        {
            return FormUserControlInfoProvider.GetTypeEnum(ValidationHelper.GetInteger(Value, 0));
        }
        set
        {
            Value = Convert.ToString((int)value);
        }
    }


    /// <summary>
    /// Gets or sets whether "all" item is present in the list.
    /// </summary>
    public bool IncludeAllItem
    {
        get
        {
            return mIncludeAllItem;
        }
        set
        {
            mIncludeAllItem = value;
        }
    }


    /// <summary>
    /// Gets or sets value indicating if drop-down is set to cause AutoPostBack.
    /// </summary>
    public bool AutoPostBack
    {
        get
        {
            return drpType.AutoPostBack;
        }
        set
        {
            drpType.AutoPostBack = value;
        }
    }


    /// <summary>
    /// Column data type to limit form control types.
    /// </summary>
    public string DataType
    {
        get
        {
            return mDataType;
        }
        set
        {
            mDataType = value;
        }
    }


    /// <summary>
    /// FieldEditorControlsEnum to limit form control types.
    /// </summary>
    public FieldEditorControlsEnum FieldEditorControls
    {
        get
        {
            return mFieldEditorControls;
        }
        set
        {
            mFieldEditorControls = value;
        }
    }


    /// <summary>
    /// Primary field to limit form control types.
    /// </summary>
    public bool IsPrimary
    {
        get
        {
            return mIsPrimary;
        }
        set
        {
            mIsPrimary = value;
        }
    }


    /// <summary>
    /// Indicates if field is External.
    /// </summary>
    public bool External
    {
        get;
        set;
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Initialize filter dropdownlist
        if (drpType.Items.Count <= 0)
        {
            ReLoadUserControl();
        }
        if (drpType.Items.FindByValue(mSelectedValue) != null)
        {
            drpType.SelectedValue = mSelectedValue;
        }

        drpType.SelectedIndexChanged += new EventHandler(drpType_SelectedIndexChanged);
    }


    protected void drpType_SelectedIndexChanged(object sender, EventArgs e)
    {
        FilterChanged = true;
        RaiseOnFilterChanged();
    }
    
    #endregion


    #region "Methods"

    /// <summary>
    /// Clears selection of drop-down list.
    /// </summary>
    public void ClearSelection()
    {
        drpType.ClearSelection();
    }


    /// <summary>
    /// Loads control with items.
    /// </summary>
    public void ReLoadUserControl()
    {
        drpType.Items.Clear();
        drpType.SelectedValue = null;
        drpType.ClearSelection();

        // Load control dynamicaly with limited set of available types
        if (!String.IsNullOrEmpty(DataType))
        {
            drpType.DataTextField = "text";
            drpType.DataValueField = "value";
            DataSet ds = FormHelper.GetAvailableControlTypes(DataType, FieldEditorControls, (IsPrimary && !External));
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                // Sort result by 'text' column
                ds.Tables[0].DefaultView.Sort = "text";
                drpType.DataSource = ds.Tables[0].DefaultView;
                drpType.DataBind();
            }
        }
        // Load control with all types
        else
        {
            foreach (FormUserControlTypeEnum en in new FormUserControlTypeEnum[]
                                                       {
                                                           FormUserControlTypeEnum.Captcha, FormUserControlTypeEnum.Filter, FormUserControlTypeEnum.Input, FormUserControlTypeEnum.Multifield,
                                                           FormUserControlTypeEnum.Selector, FormUserControlTypeEnum.Uploader, FormUserControlTypeEnum.Viewer, FormUserControlTypeEnum.Visibility
                                                       })
            {
                drpType.Items.Add(new ListItem(GetString("formcontrolstype." + en.ToString()), Convert.ToInt32(en).ToString()));
            }
        }

        // Include 'all' item in first position
        if (IncludeAllItem)
        {
            drpType.Items.Insert(0, new ListItem(GetString("general.selectall"), Convert.ToInt32(FormUserControlTypeEnum.Unspecified).ToString()));
        }
    }


    /// <summary>
    /// Loads control with items.
    /// </summary>
    /// <param name="controlTypes">Specified control types</param>
    public void ReLoadUserControl(List<FormUserControlTypeEnum> controlTypes)
    {
        drpType.Items.Clear();

        foreach (FormUserControlTypeEnum controlType in controlTypes)
        {
            drpType.Items.Add(new ListItem(GetString("formcontrolstype." + controlType.ToString()), Convert.ToInt32(controlType).ToString()));
        }

        // Include 'all' item in first position
        if (IncludeAllItem)
        {
            drpType.Items.Insert(0, new ListItem(GetString("general.selectall"), Convert.ToInt32(FormUserControlTypeEnum.Unspecified).ToString()));
        }
    }

    #endregion


    #region "State management"

    /// <summary>
    /// Resets filter to the default state.
    /// </summary>
    public override void ResetFilter()
    {
        ClearSelection();
    }


    /// <summary>
    /// Restores filter state from the specified object.
    /// </summary>
    /// <param name="state">The object that holds the filter state.</param>
    public override void RestoreFilterState(FilterState state)
    {
        base.RestoreFilterState(state);
        Value = state.GetString("Value");
    }


    /// <summary>
    /// Stores filter state to the specified object.
    /// </summary>
    /// <param name="state">The object that holds the filter state.</param>
    public override void StoreFilterState(FilterState state)
    {
        state.AddValue("Value", Value);
        base.StoreFilterState(state);
    }

    #endregion
}