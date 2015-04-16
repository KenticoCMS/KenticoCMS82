using System;
using System.Text;
using System.Web.UI.WebControls;

using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.FormEngine;
using CMS.Helpers;


public partial class CMSFormControls_Basic_MultipleChoiceControl : FormEngineUserControl
{
    #region "Variables"

    private string[] mSelectedValues = null;
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
            return FormHelper.GetSelectedValuesFromListItemCollection(list.Items);
        }
        set
        {
            mSelectedValues = ValidationHelper.GetString(value, String.Empty).Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            LoadAndSelectList();
        }
    }


    /// <summary>
    /// Returns selected value display names separated with comma.
    /// </summary>
    public override string ValueDisplayName
    {
        get
        {
            StringBuilder text = new StringBuilder();
            bool first = true;
            foreach (ListItem item in list.Items)
            {
                if (item.Selected)
                {
                    if (!first)
                    {
                        text.Append(", ");
                    }
                    text.Append(item.Text);
                    first = false;
                }
            }
            return text.ToString();
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
        LoadAndSelectList(true);

        // Set control styles
        if (!String.IsNullOrEmpty(CssClass))
        {
            list.AddCssClass(CssClass);
            CssClass = null;
        }
        else if (String.IsNullOrEmpty(list.CssClass))
        {
            list.AddCssClass("CheckBoxListField");
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
    private void LoadAndSelectList(bool forceLoad = false)
    {
        if ((list.Items.Count == 0) && ((mSelectedValues.Length > 0) || forceLoad))
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

            FormHelper.SelectMultipleValues(mSelectedValues, list.Items, ListSelectionMode.Multiple);
        }
    }


    /// <summary>
    /// Displays exception control with current error.
    /// </summary>
    /// <param name="ex">Thrown exception</param>
    private void DisplayException(Exception ex)
    {
        FormControlError ctrlError = new FormControlError();
        ctrlError.FormControlName = FormFieldControlTypeCode.MULTIPLECHOICE;
        ctrlError.InnerException = ex;
        Controls.Add(ctrlError);
        list.Visible = false;
    }

    #endregion
}