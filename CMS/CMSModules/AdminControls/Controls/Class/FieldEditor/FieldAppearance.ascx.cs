using System;

using CMS.Base;
using CMS.DataEngine;
using CMS.FormControls;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.UIControls;

public partial class CMSModules_AdminControls_Controls_Class_FieldEditor_FieldAppearance : CMSUserControl
{
    #region "Events"

    /// <summary>
    /// Fired when DDL with form controls has been changed.
    /// </summary>
    public event EventHandler OnFieldSelected
    {
        add
        {
            drpControl.OnSelectionChanged += value;
        }
        remove
        {
            drpControl.OnSelectionChanged -= value;
        }
    }

    #endregion


    #region "Variables"

    private FieldEditorControlsEnum mDisplayedControls = FieldEditorControlsEnum.ModeSelected;
    public Func<FieldEditorControlsEnum, FieldEditorModeEnum, bool, FieldEditorControlsEnum> GetControls;

    #endregion


    #region "Properties"

    /// <summary>
    /// Type of field.
    /// </summary>
    public string AttributeType
    {
        get
        {
            return ValidationHelper.GetString(ViewState["AttributeType"], null);
        }
        set
        {
            drpControl.DataType = value;
            ViewState["AttributeType"] = value;
        }
    }


    /// <summary>
    /// Gets or sets selected control type.
    /// </summary>
    public string FieldType
    {
        get
        {
            return drpControl.Text;
        }
        set
        {
            if (!String.IsNullOrEmpty(value))
            {
                drpControl.Value = value;
            }
        }
    }


    /// <summary>
    /// Field editor mode.
    /// </summary>
    public FieldEditorModeEnum Mode
    {
        get;
        set;
    }


    /// <summary>
    /// Class name.
    /// </summary>
    public string ClassName
    {
        get;
        set;
    }


    /// <summary>
    /// FormFieldInfo of given field.
    /// </summary>
    public FormFieldInfo FieldInfo
    {
        get;
        set;
    }


    /// <summary>
    /// Gets value from Field caption attribute.
    /// </summary>
    public string FieldCaption
    {
        get
        {
            return ValidationHelper.GetString(txtFieldCaption.Value, String.Empty);
        }
    }


    /// <summary>
    /// Gets or sets value indicating if user can change visibility of given field.
    /// </summary>
    public bool ChangeVisibility
    {
        get
        {
            return chkChangeVisibility.Checked;
        }
        set
        {
            chkChangeVisibility.Checked = value;
        }
    }


    /// <summary>
    /// Gets or sets value of visibility control.
    /// </summary>
    public FormFieldVisibilityTypeEnum Visibility
    {
        get
        {
            return ctrlVisibility.Value.ToEnum<FormFieldVisibilityTypeEnum>();
        }
        set
        {
            ctrlVisibility.Value = value.ToStringRepresentation();
        }
    }


    /// <summary>
    /// Gets or sets value of DDL visibility section.
    /// </summary>
    public string VisibilityDDL
    {
        get
        {
            return drpVisibilityControl.SelectedValue;
        }
        set
        {
            drpVisibilityControl.SelectedValue = value;
        }
    }


    /// <summary>
    /// Gets or sets value indicating if Public field checkbox is checked.
    /// </summary>
    public bool PublicField
    {
        get
        {
            return chkPublicField.Checked;
        }
        set
        {
            chkPublicField.Checked = value;
        }
    }


    /// <summary>
    /// Gets value which represents text field Description value.
    /// </summary>
    public string Description
    {
        get
        {
            return ValidationHelper.GetString(txtDescription.Value, String.Empty);
        }
    }


    /// <summary>
    /// Gets or sets field visibility.
    /// </summary>
    public bool ShowFieldVisibility
    {
        get;
        set;
    }


    /// <summary>
    /// Type of custom controls that can be selected from the control list in FieldEditor.
    /// </summary>
    public FieldEditorControlsEnum DisplayedControls
    {
        get
        {
            return mDisplayedControls;
        }
        set
        {
            mDisplayedControls = value;
        }
    }


    /// <summary>
    /// Gets or sets alternative form full name.
    /// </summary>
    public string AlternativeFormFullName
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if Field Editor is used as alternative form.
    /// </summary>
    public bool IsAlternativeForm
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if the form is alternative filter form.
    /// </summary>
    public bool IsAlternativeFilterForm
    {
        get
        {
            if (IsAlternativeForm && !string.IsNullOrEmpty(AlternativeFormFullName))
            {
                return AlternativeFormFullName.ToLowerCSafe().EndsWithCSafe(".filter");
            }

            return false;
        }
    }


    /// <summary>
    /// Gets or sets value indicating if control is inheritable or not.
    /// </summary>
    public bool ControlInheritable
    {
        get
        {
            return chkControlInheritable.Checked;
        }
        set
        {
            chkControlInheritable.Checked = value;
        }
    }


    /// <summary>
    /// Enables or disables to edit <see cref='CMS.FormEngine.FormFieldInfo.Inheritable'> settings.
    /// </summary>
    public bool ShowInheritanceSettings
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if field editor works in development mode.
    /// </summary>
    public bool DevelopmentMode
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets macroresolver used in macro editor.
    /// </summary>
    public string ResolverName
    {
        get;
        set;
    }

    #endregion


    #region "Methods"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        drpControl.ReturnColumnName = FormUserControlInfo.TYPEINFO.CodeNameColumn;

        txtFieldCaption.IsLiveSite = IsLiveSite;
        txtDescription.IsLiveSite = IsLiveSite;
        txtExplanationText.IsLiveSite = IsLiveSite;
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // Disable autosave on LocalizableTextBox controls
        FormEngineUserControl descriptionControl = (FormEngineUserControl)txtDescription.NestedControl;
        if (descriptionControl != null)
        {
            descriptionControl.SetValue("AutoSave", false);
        }
        FormEngineUserControl fieldCaptionControl = (FormEngineUserControl)txtFieldCaption.NestedControl;
        if (fieldCaptionControl != null)
        {
            fieldCaptionControl.SetValue("AutoSave", false);
        }
        FormEngineUserControl explanationTextControl = (FormEngineUserControl)txtExplanationText.NestedControl;
        if (explanationTextControl != null)
        {
            explanationTextControl.SetValue("AutoSave", false);
        }

        drpControl.DataType = AttributeType;

        // Public field option initialization    
        plcPublicField.Visible = (Mode == FieldEditorModeEnum.BizFormDefinition) || (Mode == FieldEditorModeEnum.AlternativeBizFormDefinition);
        plcInheritance.Visible = ShowInheritanceSettings;

        txtFieldCaption.ResolverName = ResolverName;
        txtDescription.ResolverName = ResolverName;
        txtExplanationText.ResolverName = ResolverName;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        lblFieldCaption.AssociatedControlClientID = EditingFormControl.GetInputClientID(txtFieldCaption.NestedControl.Controls);
        lblFieldDescription.AssociatedControlClientID = EditingFormControl.GetInputClientID(txtDescription.NestedControl.Controls);
        lblExplanationText.AssociatedControlClientID = EditingFormControl.GetInputClientID(txtExplanationText.NestedControl.Controls);
        lblField.AssociatedControlClientID = drpControl.InputClientID;

        // Check if the form control is allowed for selected data type
        var control = FormUserControlInfoProvider.GetFormUserControlInfo(drpControl.Value.ToString());
        
        string dataTypeColumn = FormHelper.GetDataTypeColumn(AttributeType);
        if ((control != null) && !String.IsNullOrEmpty(dataTypeColumn))
        {
            bool allowType = ValidationHelper.GetBoolean(control[dataTypeColumn], false);
            if (!allowType)
            {
                ShowWarning(String.Format(GetString("fieldeditor.controlnotallowedfordatatype"), control.UserControlDisplayName));
            }
        }
    }


    /// <summary>
    /// Loads field with values from FormFieldInfo.
    /// </summary>
    public void Reload()
    {
        if (FieldInfo != null)
        {
            bool isMacro;
            txtDescription.SetValue(FieldInfo.GetPropertyValue(FormFieldPropertyEnum.FieldDescription, out isMacro), isMacro);
            txtFieldCaption.SetValue(FieldInfo.GetPropertyValue(FormFieldPropertyEnum.FieldCaption, out isMacro), isMacro);
            txtExplanationText.SetValue(FieldInfo.GetPropertyValue(FormFieldPropertyEnum.ExplanationText, out isMacro), isMacro);

            if (ShowInheritanceSettings)
            {
                chkControlInheritable.Checked = FieldInfo.Inheritable;
            }

            // Field visibility
            if (ShowFieldVisibility)
            {
                chkChangeVisibility.Checked = FieldInfo.AllowUserToChangeVisibility;
                Visibility = FieldInfo.Visibility;

                // Load controls for user visibility
                drpVisibilityControl.DataSource = FormUserControlInfoProvider.GetFormUserControls("UserControlCodeName, UserControlDisplayName", "UserControlForVisibility = 1", "UserControlDisplayName");
                drpVisibilityControl.DataBind();

                try
                {
                    drpVisibilityControl.Items.FindByValue(FieldInfo.VisibilityControl);
                    drpVisibilityControl.SelectedValue = FieldInfo.VisibilityControl;
                }
                catch
                {
                }
            }

            plcVisibility.Visible = ShowFieldVisibility;

            if ((Mode == FieldEditorModeEnum.BizFormDefinition) || (Mode == FieldEditorModeEnum.AlternativeBizFormDefinition))
            {
                chkPublicField.Checked = FieldInfo.PublicField;
            }

            string selectedItem;
            // Get control name from settings
            if (!String.IsNullOrEmpty(Convert.ToString(FieldInfo.Settings["controlname"])))
            {
                selectedItem = Convert.ToString(FieldInfo.Settings["controlname"]);
            }
            // Or get control name from field type
            else
            {
                selectedItem = FormHelper.GetFormFieldControlTypeString(FieldInfo.FieldType);
            }

            FormUserControlInfo fi = FormUserControlInfoProvider.GetFormUserControlInfo(selectedItem);

            if (fi != null)
            {
                // Preselect options for specific control
                FieldType = selectedItem;
            }
            else
            {
                // Preselect default control for attribute type
                FieldType = FormHelper.GetFormFieldDefaultControlType(AttributeType);
            }

            LoadFieldTypes(FieldInfo.PrimaryKey);
        }
        // If FormFieldInfo is not specified then clear form
        else
        {
            chkPublicField.Checked = true;
            ctrlVisibility.Value = null;
            drpVisibilityControl.ClearSelection();
            chkChangeVisibility.Checked = false;
            txtFieldCaption.SetValue(null, false);
            txtDescription.SetValue(null, false);
            txtExplanationText.SetValue(null, false);
            drpControl.Reload(false);
            drpControl.Value = FormHelper.GetFormFieldDefaultControlType(AttributeType);
        }
    }


    /// <summary>
    /// Fill field types list. Form control types will be restricted to actual selection in Form control types drop-down list.
    /// </summary>
    /// <param name="isPrimary">Determines whether the attribute is primary key</param>
    public void LoadFieldTypes(bool isPrimary)
    {
        var controls = GetControls(DisplayedControls, Mode, DevelopmentMode);

        string filteredControlsWhere = FormHelper.GetWhereConditionForDataType(AttributeType, controls, isPrimary, FormUserControlTypeEnum.Unspecified);
        drpControl.WhereCondition = filteredControlsWhere;

        // Exception for blogposts in document types
        if ((controls != FieldEditorControlsEnum.DocumentTypes) || !ClassName.EqualsCSafe("cms.blogpost", true))
        {
            // Show trackbackpingedurls only when editing blogposts
            drpControl.WhereCondition = SqlHelper.AddWhereCondition(drpControl.WhereCondition, FormUserControlInfo.TYPEINFO.CodeNameColumn + " <> 'trackbackspingedurls'");
            drpControl.WhereCondition = SqlHelper.AddWhereCondition(drpControl.WhereCondition, FormUserControlInfo.TYPEINFO.CodeNameColumn + " <> 'trackbacksnotpingedurls'");
        }

        var originalValue = FieldType;

        drpControl.Reload(true);

        var newValue = FieldType;

        // If previously selected control is not available, load default control
        if (!originalValue.EqualsCSafe(newValue, true))
        {
            FieldType = FormHelper.GetFormFieldDefaultControlType(AttributeType);
        }
    }


    /// <summary>
    /// Save the properties to form field info.
    /// </summary>
    /// <returns>True if success</returns>
    public bool Save()
    {
        if (FieldInfo != null)
        {
            // Save LocalizableTextBox controls
            LocalizableFormEngineUserControl descriptionControl = (LocalizableFormEngineUserControl)txtDescription.NestedControl;
            if (descriptionControl != null)
            {
                descriptionControl.Save();
            }
            LocalizableFormEngineUserControl fieldCaptionControl = (LocalizableFormEngineUserControl)txtFieldCaption.NestedControl;
            if (fieldCaptionControl != null)
            {
                fieldCaptionControl.Save();
            }
            LocalizableFormEngineUserControl explanationTextControl = (LocalizableFormEngineUserControl)txtExplanationText.NestedControl;
            if (explanationTextControl != null)
            {
                explanationTextControl.Save();
            }

            FieldInfo.SetPropertyValue(FormFieldPropertyEnum.FieldCaption, ValidationHelper.GetString(txtFieldCaption.Value, String.Empty), txtFieldCaption.IsMacro);
            FieldInfo.SetPropertyValue(FormFieldPropertyEnum.FieldDescription, ValidationHelper.GetString(txtDescription.Value, String.Empty), txtDescription.IsMacro);
            FieldInfo.SetPropertyValue(FormFieldPropertyEnum.ExplanationText, ValidationHelper.GetString(txtExplanationText.Value, String.Empty), txtExplanationText.IsMacro);

            if (ShowFieldVisibility)
            {
                FieldInfo.AllowUserToChangeVisibility = ChangeVisibility;
                FieldInfo.Visibility = Visibility;
                FieldInfo.VisibilityControl = VisibilityDDL;
            }

            if (ShowInheritanceSettings)
            {
                FieldInfo.Inheritable = ControlInheritable;
            }

            if ((Mode == FieldEditorModeEnum.BizFormDefinition) ||
                (Mode == FieldEditorModeEnum.AlternativeBizFormDefinition) ||
                DisplayedControls == FieldEditorControlsEnum.Bizforms)
            {
                FieldInfo.PublicField = PublicField;
            }
            else
            {
                FieldInfo.PublicField = false;
            }

            return true;
        }

        return false;
    }

    #endregion
}