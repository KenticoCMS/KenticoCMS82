using System;
using System.Web.UI.WebControls;

using CMS.FormControls;
using CMS.FormEngine;
using CMS.Helpers;

public partial class CMSFormControls_Basic_ListBoxControl : FormEngineUserControl
{
    #region "Variables"

    private string[] mSelectedValues = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return listbox.Enabled;
        }
        set
        {
            listbox.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets form control value.
    /// </summary>
    public override object Value
    {
        get
        {
            return FormHelper.GetSelectedValuesFromListItemCollection(listbox.Items);
        }
        set
        {
            mSelectedValues = ValidationHelper.GetString(value, String.Empty).Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            
            LoadAndSelectList();
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        LoadAndSelectList(true);

        // Set control style
        if (!String.IsNullOrEmpty(CssClass))
        {
            listbox.CssClass = CssClass;
            CssClass = null;
        }
        else if (String.IsNullOrEmpty(listbox.CssClass))
        {
            listbox.CssClass = "ListBoxField";
        }
        if (!String.IsNullOrEmpty(ControlStyle))
        {
            listbox.Attributes.Add("style", ControlStyle);
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
        if ((listbox.Items.Count == 0) && ((mSelectedValues.Length > 0) || forceLoad))
        {
            bool allowMultiple = GetValue("allowmultiplechoices", true);
            listbox.SelectionMode = allowMultiple ? ListSelectionMode.Multiple : ListSelectionMode.Single;

            string options = GetResolvedValue<string>("options", null);
            string query = ValidationHelper.GetString(GetValue("query"), null);

            try
            {
                FormHelper.LoadItemsIntoList(options, query, listbox.Items, FieldInfo, ContextResolver);
            }
            catch (Exception ex)
            {
                DisplayException(ex);
            }

            FormHelper.SelectMultipleValues(mSelectedValues, listbox.Items, listbox.SelectionMode);
        }
    }


    /// <summary>
    /// Displays exception control with current error.
    /// </summary>
    /// <param name="ex">Thrown exception</param>
    private void DisplayException(Exception ex)
    {
        FormControlError ctrlError = new FormControlError();
        ctrlError.FormControlName = FormFieldControlTypeCode.LISTBOX;
        ctrlError.InnerException = ex;
        Controls.Add(ctrlError);
        listbox.Visible = false;
    }

    #endregion
}