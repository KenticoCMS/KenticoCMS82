using System;
using System.Web.UI.WebControls;

using CMS.DataEngine;
using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.FormEngine;
using CMS.Helpers;

public partial class CMSFormControls_Basic_RadioButtonsControl : FormEngineUserControl
{
    #region "Variables"

    private string mSelectedValue;
    private RepeatDirection mRepeatDirection = RepeatDirection.Vertical;
    private RepeatLayout mRepeatLayout = RepeatLayout.Flow;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the enabled state of the control.
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


    /// <summary>
    /// Gets or sets form control value.
    /// </summary>
    public override object Value
    {
        get
        {
            return list.SelectedValue;
        }
        set
        {
            LoadAndSelectList();
            
            if ((value != null) || ((FieldInfo != null) && FieldInfo.AllowEmpty))
            {
                if (FieldInfo != null)
                {
                    // Convert the value to a proper type
                    value = ConvertInputValue(value);
                }

                mSelectedValue = ValidationHelper.GetString(value, String.Empty);

                list.ClearSelection();
                FormHelper.SelectSingleValue(mSelectedValue, list);
            }
        }
    }


    /// <summary>
    /// Returns display name of the value.
    /// </summary>
    public override string ValueDisplayName
    {
        get
        {
            return (list.SelectedItem == null ? list.Text : list.SelectedItem.Text);
        }
    }


    /// <summary>
    /// Specifies the direction in which items of a list control are displayed.
    /// </summary>
    public RepeatDirection RepeatDirection
    {
        get
        {
            string direction = ValidationHelper.GetString(GetValue("repeatdirection"), String.Empty);
            if (!Enum.TryParse<RepeatDirection>(direction, true, out mRepeatDirection))
            {
                mRepeatDirection = RepeatDirection.Vertical;
            }

            return mRepeatDirection;
        }
        set
        {
            mRepeatDirection = value;
        }
    }


    /// <summary>
    /// Specifies the layout of items in a list control.
    /// </summary>
    public RepeatLayout RepeatLayout
    {
        get
        {
            string layout = ValidationHelper.GetString(GetValue("RepeatLayout"), String.Empty);
            if (!Enum.TryParse<RepeatLayout>(layout, true, out mRepeatLayout))
            {
                mRepeatLayout = RepeatLayout.Flow;
            }

            return mRepeatLayout;
        }
        set
        {
            mRepeatLayout = value;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        LoadAndSelectList();

        list.SelectedIndexChanged += (s, ea) => RaiseOnChanged();

        // Apply styles
        if (!String.IsNullOrEmpty(CssClass))
        {
            list.AddCssClass(CssClass);
            CssClass = null;
        }
        else if (String.IsNullOrEmpty(list.CssClass))
        {
            list.AddCssClass("RadioButtonList");
        }
        if (!String.IsNullOrEmpty(ControlStyle))
        {
            list.Attributes.Add("style", ControlStyle);
            ControlStyle = null;
        }

        CheckRegularExpression = true;
        CheckFieldEmptiness = true;
    }


    /// <summary>
    /// Loads and selects control.
    /// </summary>
    private void LoadAndSelectList()
    {
        if (list.Items.Count == 0)
        {
            // Set control direction
            list.RepeatDirection = RepeatDirection;

            // Set control layout
            list.RepeatLayout = RepeatLayout;

            string options = GetResolvedValue<string>("options", null);
            string query = ValidationHelper.GetString(GetValue("query"), null);

            try
            {
                FormHelper.LoadItemsIntoList(options, query, list.Items, FieldInfo, ContextResolver);
            }
            catch (Exception ex)
            {
                DisplayException(ex);
            }

            FormHelper.SelectSingleValue(mSelectedValue, list);
        }
    }


    /// <summary>
    /// Displays exception control with current error.
    /// </summary>
    /// <param name="ex">Thrown exception</param>
    private void DisplayException(Exception ex)
    {
        FormControlError ctrlError = new FormControlError();
        ctrlError.FormControlName = FormFieldControlTypeCode.RADIOBUTTONS;
        ctrlError.InnerException = ex;
        Controls.Add(ctrlError);
        list.Visible = false;
    }

    #endregion
}