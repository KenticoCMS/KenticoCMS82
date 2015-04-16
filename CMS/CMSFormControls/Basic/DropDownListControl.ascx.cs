using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.FormEngine;
using CMS.Helpers;

public partial class CMSFormControls_Basic_DropDownListControl : FormEngineUserControl
{
    #region "Variables"

    private string mSelectedValue;
    private bool? mEditText;
    private string mOnChangeScript;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return dropDownList.Enabled;
        }
        set
        {
            dropDownList.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets form control value.
    /// </summary>
    public override object Value
    {
        get
        {
            if (EditText)
            {
                return txtCombo.Text;
            }
            else
            {
                return dropDownList.SelectedValue;
            }
        }
        set
        {
            LoadAndSelectList();
            
            if ((value != null) || ((FieldInfo != null) && FieldInfo.AllowEmpty))
            {
                if (FieldInfo != null)
                {
                    value = ConvertInputValue(value);
                }

                mSelectedValue = ValidationHelper.GetString(value, String.Empty);

                EnsureActualValueAsItem();

                if (EditText)
                {
                    txtCombo.Text = mSelectedValue;
                }
                else
                {
                    dropDownList.ClearSelection();
                    ListItem item = dropDownList.Items.FindByValue(mSelectedValue);
                    if (item != null)
                    {
                        item.Selected = true;
                    }
                }
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
            return (EditText || (dropDownList.SelectedItem == null) ? txtCombo.Text : dropDownList.SelectedItem.Text);
        }
    }


    /// <summary>
    /// Gets or sets selected value.
    /// </summary>
    public string SelectedValue
    {
        get
        {
            if (EditText)
            {
                return txtCombo.Text;
            }
            else
            {
                return dropDownList.SelectedValue;
            }
        }
        set
        {
            if (EditText)
            {
                txtCombo.Text = value;
            }
            else
            {
                dropDownList.SelectedValue = value;
            }
        }
    }


    /// <summary>
    /// Gets or sets selected index. Returns -1 if no element is selected.
    /// </summary>
    public int SelectedIndex
    {
        get
        {
            if (EditText)
            {
                if (dropDownList.Items.FindByValue(txtCombo.Text) != null)
                {
                    return dropDownList.SelectedIndex;
                }
                return -1;
            }
            else
            {
                return dropDownList.SelectedIndex;
            }
        }
        set
        {
            dropDownList.SelectedIndex = value;
            if (EditText)
            {
                txtCombo.Text = dropDownList.SelectedValue;
            }
        }
    }


    /// <summary>
    /// Enables to edit text from textbox and select values from dropdown list.
    /// </summary>
    public bool EditText
    {
        get
        {
            return mEditText ?? ValidationHelper.GetBoolean(GetValue("edittext"), false);
        }
        set
        {
            mEditText = value;
        }
    }


    /// <summary>
    /// Gets dropdown list control.
    /// </summary>
    public CMSDropDownList DropDownList
    {
        get
        {
            return dropDownList;
        }
    }


    /// <summary>
    /// Gets textbox control.
    /// </summary>
    public CMSTextBox TextBoxControl
    {
        get
        {
            return txtCombo;
        }
    }


    /// <summary>
    /// Indicates whether or not to use first value as default if default value is empty.
    /// </summary>
    [Obsolete("This property is not used anymore.")]
    public bool FirstAsDefault
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets Javascript code that is executed when selected item is changed.
    /// </summary>
    public string OnChangeClientScript
    {
        get
        {
            return mOnChangeScript ?? ValidationHelper.GetString(GetValue("OnChangeClientScript"), String.Empty);
        }
        set
        {
            mOnChangeScript = value;
        }
    }


    /// <summary>
    /// Text pattern containing macro. Valid when the data source is query or macro.
    /// </summary>
    public string TextFormat
    {
        get
        {
            return GetValue("TextFormat", String.Empty);
        }
        set
        {
            SetValue("TextFormat", value);
        }
    }


    /// <summary>
    /// Value pattern containing macro. Valid when the data source is query or macro.
    /// </summary>
    public string ValueFormat
    {
        get
        {
            return GetValue("ValueFormat", String.Empty);
        }
        set
        {
            SetValue("ValueFormat", value);
        }
    }


    /// <summary>
    /// Macro source.
    /// </summary>
    public string MacroSource
    {
        get
        {
            return GetValue("macro", String.Empty);
        }
        set
        {
            SetValue("macro", value);
        }
    }


    /// <summary>
    /// Indicates whether the items load to the DDL will be alphabetically sorted.
    /// </summary>
    public bool SortItems
    {
        get
        {
            return GetValue("SortItems", false);
        }
        set
        {
            SetValue("SortItems", value);
        }
    }


    /// <summary>
    /// Indicates whether actual value (that is not present among options) will be displayed as DDL item.
    /// </summary>
    public bool DisplayActualValueAsItem
    {
        get
        {
            return GetValue("DisplayActualValueAsItem", false);
        }
        set
        {
            SetValue("DisplayActualValueAsItem", value);
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // When DependsOnAnotherField is turned on we need force reload because of loading macro values from another fields which may be loaded from ViewState
        LoadAndSelectList(DependsOnAnotherField);

        txtCombo.Visible = EditText;
        dropDownList.Visible = !EditText;

        CheckRegularExpression = true;
        CheckFieldEmptiness = true;

        if (EditText)
        {
            ApplyCssClassAndStyles(txtCombo);
        }
        else
        {
            ApplyCssClassAndStyles(dropDownList);
        }
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (StopProcessing || !Enabled)
        {
            //Do nothing
            return;
        }

        if (EditText)
        {
            btnAutocomplete.Visible = true;

            ScriptHelper.RegisterJQueryUI(Page);
            ScriptHelper.RegisterStartupScript(Page, typeof(string), "Autocomplete_" + ClientID, ScriptHelper.GetScript(
@"
var txtCombo" + txtCombo.ClientID + @" = $cmsj('#" + txtCombo.ClientID + @"');

// Perform autocomplete
txtCombo" + txtCombo.ClientID + @".autocomplete({ 
    source: " + GetDataAsJsArray() + @",
    minLength: 0,
    appendTo: '#" + autoComplete.ClientID + @"'
});

// Open dropdown list
txtCombo" + txtCombo.ClientID + @".add($cmsj('#" + btnAutocomplete.ClientID + @"')).on('click', function () {
    txtCombo" + txtCombo.ClientID + @".autocomplete('search', '');
    txtCombo" + txtCombo.ClientID + @".focus();
});

// Close dropdown list if scrolled outside the list
$cmsj(document).bind('mousewheel DOMMouseScroll', function (e) {
    if (!txtCombo" + txtCombo.ClientID + @".autocomplete('widget').is(':hover')) {
            txtCombo" + txtCombo.ClientID + @".autocomplete('close');
    }          
});"
              ));
        }
        else
        {
            if (!String.IsNullOrEmpty(OnChangeClientScript))
            {
                dropDownList.Attributes.Add("onchange", OnChangeClientScript);

                // Remember originally selected index of the drop-down list.
                string originalSelectionScript = "var originalSelection_" + dropDownList.ClientID + " = " + dropDownList.SelectedIndex + ";";
                ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "DropDownListSelectedIndex", ScriptHelper.GetScript(originalSelectionScript));
            }
        }
    }


    /// <summary>
    /// Loads and selects control.
    /// </summary>
    /// <param name="forceReload">Indicates if items should be reloaded even if control contains some values</param>
    private void LoadAndSelectList(bool forceReload = false)
    {
        if (forceReload)
        {
            // Keep selected value
            mSelectedValue = dropDownList.SelectedValue;
            
            // Clears values if forced reload is requested
            dropDownList.Items.Clear();
        }

        if (dropDownList.Items.Count == 0)
        {
            string options = GetResolvedValue<string>("options", null);
            string query = ValidationHelper.GetString(GetValue("query"), null);
            string macro = MacroSource;

            try
            {
                if (string.IsNullOrEmpty(macro))
                {
                    // Load from options or query
                    FormHelper.LoadItemsIntoList(options, query, dropDownList.Items, FieldInfo, ContextResolver, SortItems);
                }
                else
                {
                    // Load from macro source
                    var def = new SpecialFieldsDefinition(null, FieldInfo, ContextResolver, SortItems);
                    def.LoadFromMacro(macro, ValueFormat, TextFormat);
                    def.FillItems(dropDownList.Items);
                }
            }
            catch (Exception ex)
            {
                DisplayException(ex);
            }

            FormHelper.SelectSingleValue(mSelectedValue, dropDownList, true);
        }
    }


    private void ApplyCssClassAndStyles(WebControl control)
    {
        if (!String.IsNullOrEmpty(CssClass))
        {
            control.AddCssClass(CssClass);
            CssClass = null;
        }

        if (!String.IsNullOrEmpty(ControlStyle))
        {
            control.Attributes.Add("style", ControlStyle);
            ControlStyle = null;
        }
    }


    /// <summary>
    /// Returns data as JavaScript array (e.g.: ['value1', 'value2']).
    /// </summary>
    private string GetDataAsJsArray()
    {
        List<string> array = new List<string>();

        foreach (var item in dropDownList.Items)
        {
            array.Add(((ListItem)item).Text);
        }

        return new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(array);
    }


    /// <summary>
    /// Displays exception control with current error.
    /// </summary>
    /// <param name="ex">Thrown exception</param>
    private void DisplayException(Exception ex)
    {
        FormControlError ctrlError = new FormControlError();
        ctrlError.FormControlName = FormFieldControlTypeCode.DROPDOWNLIST;
        ctrlError.InnerException = ex;
        Controls.Add(ctrlError);
        dropDownList.Visible = false;
    }


    /// <summary>
    /// Ensures that a value which is not among DDL items but is present in the database is added to DDL items collection.
    /// </summary>
    private void EnsureActualValueAsItem()
    {
        if (DisplayActualValueAsItem)
        {
            var item = dropDownList.Items.FindByValue(mSelectedValue);
            if (item == null)
            {
                dropDownList.Items.Add(new ListItem(mSelectedValue));

                if (SortItems)
                {
                    dropDownList.SortItems();
                }
            }
        }
    }

    #endregion
}