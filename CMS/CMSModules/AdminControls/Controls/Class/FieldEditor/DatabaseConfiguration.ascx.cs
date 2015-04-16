using System;
using System.Linq;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.DataEngine;
using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.TranslationServices;
using CMS.UIControls;

public partial class CMSModules_AdminControls_Controls_Class_FieldEditor_DatabaseConfiguration : CMSUserControl
{
    #region "Events"

    /// <summary>
    /// Event raised when drop-down list is changed.
    /// </summary>
    public event EventHandler DropChanged;


    /// <summary>
    /// Event raised when attribute is changed.
    /// </summary>
    public event EventHandler AttributeChanged;

    #endregion


    #region "Variables"

    private bool? mShowResolveDefaultValue;

    #endregion


    #region "Properties"

    /// <summary>
    /// Field editor mode.
    /// </summary>
    public FieldEditorModeEnum Mode
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
    /// Gets or sets value indicating if current field is primary.
    /// </summary>
    public bool IsFieldPrimary
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets value indicating if System fields should be enabled.
    /// </summary>
    public bool EnableSystemFields
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets value indicating if System fields should be displayed.
    /// </summary>
    public bool ShowSystemFields
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets attribute name.
    /// </summary>
    public string AttributeName
    {
        get
        {
            return txtAttributeName.Text.Trim();
        }
        set
        {
            txtAttributeName.Text = value;
        }
    }


    /// <summary>
    /// Gets value indicating new item is being edited.
    /// </summary>
    public bool IsNewItemEdited
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["IsNewItemEdited"], false);
        }
        set
        {
            ViewState["IsNewItemEdited"] = value;
        }
    }


    /// <summary>
    /// Indicates if document type is edited.
    /// </summary>
    public bool IsDocumentType
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
    /// Gets value which should be selected in group selector.
    /// </summary>
    public string GroupValue
    {
        get
        {
            return drpGroup.SelectedValue;
        }
    }


    /// <summary>
    /// Gets value which should be selected in system fields selector.
    /// </summary>
    public string SystemValue
    {
        get
        {
            return drpSystemFields.SelectedValue;
        }
    }


    /// <summary>
    /// Returns True if system fields are enabled and one of them is selected.
    /// </summary>
    public bool IsSystemFieldSelected
    {
        get
        {
            return (EnableSystemFields && plcGroup.Visible);
        }
    }


    /// <summary>
    /// Returns text value from Attribute size textbox.
    /// </summary>
    public string AttributeSize
    {
        get
        {
            return txtAttributeSize.Text.Trim();
        }
    }


    /// <summary>
    /// Returns text value from Attribute precision textbox.
    /// </summary>
    public string AttributePrecision
    {
        get
        {
            return txtAttributePrecision.Text.Trim();
        }
    }


    /// <summary>
    /// Gets or sets selected attribute type.
    /// </summary>
    public string AttributeType
    {
        get
        {
            return drpAttributeType.SelectedValue.ToLowerCSafe();
        }
        set
        {
            if (!String.IsNullOrEmpty(value))
            {
                drpAttributeType.SelectedValue = value;
            }
        }
    }


    /// <summary>
    /// Gets value indicating if field allows empty values.
    /// </summary>
    public bool AllowEmpty
    {
        get
        {
            return !chkRequired.Checked;
        }
    }


    /// <summary>
    /// Gets value indicating if field is system field.
    /// </summary>
    public bool IsSystem
    {
        get
        {
            return chkIsSystem.Checked;
        }
    }


    /// <summary>
    /// Gets value indicating whether field is unique
    /// </summary>
    public bool IsUnique
    {
        get
        {
            return chkUnique.Checked;
        }
    }


    /// <summary>
    /// Gets value indicating if a field is included to translation services export.
    /// </summary>
    public bool TranslateField
    {
        get
        {
            return chkTranslateField.Checked && plcTranslation.Visible;
        }
    }


    /// <summary>
    /// ObjectType to which the given field refers (for example as a foreign key).
    /// </summary>
    public string ReferenceToObjectType
    {
        get
        {
            return drpObjType.SelectedValue;
        }
    }


    /// <summary>
    /// Type of the reference (used only when ReferenceToObjectType is set).
    /// </summary>
    public ObjectDependencyEnum ReferenceType
    {
        get
        {
            return ValidationHelper.GetString(drpReferenceType.Value, "").ToEnum<ObjectDependencyEnum>();
        }
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
    /// Indicates if Field Editor is used as inherited form.
    /// </summary>
    public bool IsInheritedForm
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if field has no representation in database.
    /// </summary>
    public bool IsDummyField
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if dummy field is in original or alternative form.
    /// </summary>
    public bool IsDummyFieldFromMainForm
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if field is extra field (field is not in original form definition).
    /// </summary>
    public bool IsExtraField
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
    /// Coupled class name.
    /// </summary>
    public string CoupledClassName
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets macro resolver used in macro editor.
    /// </summary>
    public string ResolverName
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets associated control client ID of default value label.
    /// </summary>
    private string DefaultValueAssociatedControlClientID
    {
        get
        {
            return ValidationHelper.GetString(ViewState["DefaultValueAssociatedControlClientID"], String.Empty);
        }
        set
        {
            ViewState["DefaultValueAssociatedControlClientID"] = value;
        }
    }


    /// <summary>
    /// Indicates if 'Resolve default value' setting may be displayed.
    /// </summary>
    private bool ShowResolveDefaultValue
    {
        get
        {
            if (mShowResolveDefaultValue == null)
            {
                mShowResolveDefaultValue = (Mode == FieldEditorModeEnum.WebPartProperties)
                || (Mode == FieldEditorModeEnum.InheritedWebPartProperties)
                || (Mode == FieldEditorModeEnum.SystemWebPartProperties)
                || (Mode == FieldEditorModeEnum.FormControls)
                || (Mode == FieldEditorModeEnum.InheritedFormControl)
                || (Mode == FieldEditorModeEnum.PageTemplateProperties);
            }

            return mShowResolveDefaultValue.Value;
        }
    }

    #endregion


    #region "Control events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        txtDefaultValue.IsLiveSite = IsLiveSite;
        txtLargeDefaultValue.IsLiveSite = IsLiveSite;
        chkDefaultValue.IsLiveSite = IsLiveSite;
        rbDefaultValue.IsLiveSite = IsLiveSite;
        datetimeDefaultValue.IsLiveSite = IsLiveSite;
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        txtDefaultValue.ResolverName = ResolverName;
        txtLargeDefaultValue.ResolverName = ResolverName;
        chkDefaultValue.ResolverName = ResolverName;
        rbDefaultValue.ResolverName = ResolverName;
        datetimeDefaultValue.ResolverName = ResolverName;

        drpObjType.DropDownList.AutoPostBack = true;

        // Enable macros in Calendar control
        if (datetimeDefaultValue.NestedControl != null)
        {
            var calendarControl = ((FormEngineUserControl)datetimeDefaultValue.NestedControl);

            calendarControl.SetValue("AllowMacros", true);
        }

        // Reset associated control client ID of default value label
        lblDefaultValue.AssociatedControlClientID = DefaultValueAssociatedControlClientID;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        var attributeType = AttributeType;

        // Update the control settings
        UpdateDefaultValueControlSettings();

        chkRequired.AutoPostBack = (attributeType == FieldDataType.Boolean);

        if (ControlsHelper.CausedPostBack(chkRequired))
        {
            if (rbDefaultValue.Visible)
            {
                chkDefaultValue.Value = rbDefaultValue.Value;
            }
            else
            {
                rbDefaultValue.Value = chkDefaultValue.Value;
            }
            rbDefaultValue.Visible = AllowEmpty;
            chkDefaultValue.Visible = !AllowEmpty;
        }
        else if (ControlsHelper.CausedPostBack(drpObjType.DropDownList))
        {
            plcReferenceType.Visible = !String.IsNullOrEmpty(ValidationHelper.GetString(drpObjType.Value, String.Empty));
        }

        // Store associated control client ID of default value label
        DefaultValueAssociatedControlClientID = lblDefaultValue.AssociatedControlClientID;
    }


    private void UpdateDefaultValueControlSettings()
    {
        // Update edit time property so that it has the correct value on postback
        if (datetimeDefaultValue.NestedControl != null)
        {
            var calendarControl = ((FormEngineUserControl)datetimeDefaultValue.NestedControl);

            calendarControl.SetValue("EditTime", (AttributeType == FieldDataType.DateTime));
        }
    }


    /// <summary>
    /// Drop-down list event handler.
    /// </summary>
    protected void drpGroup_SelectedIndexChanged(object sender, EventArgs e)
    {
        SetSystemAttributeName(drpGroup.SelectedValue, String.Empty);
        if (DropChanged != null)
        {
            DropChanged(this, EventArgs.Empty);
        }
    }


    /// <summary>
    ///  Drop-down system fields event handler.
    /// </summary>
    protected void drpSystemFields_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (DropChanged != null)
        {
            DropChanged(this, EventArgs.Empty);
        }
    }


    /// <summary>
    /// Listbox attribute type event handler.
    /// </summary>
    protected void drpAttributeType_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (AttributeChanged != null)
        {
            txtAttributeSize.Text = txtAttributePrecision.Text = String.Empty;

            AttributeChanged(this, EventArgs.Empty);
        }
    }

    #endregion


    #region "Public methods"

    /// <summary>
    /// Loads group drop-down list with data.
    /// </summary>
    public void LoadGroupField()
    {
        // Set field types
        if (EnableSystemFields && (drpGroup.Items.Count == 0))
        {
            drpGroup.Items.Add(new ListItem(GetString("TemplateDesigner.DocumentAttributes"), "cms_document"));
            drpGroup.Items.Add(new ListItem(GetString("TemplateDesigner.NodeAttributes"), "cms_tree"));
        }
    }


    /// <summary>
    /// Loads field with values from FormFieldInfo object.
    /// </summary>
    public void Reload(string attributeName, bool enableSystemDDL)
    {
        // Resolving of default value macros may be controlled in specific modes
        plcResolveDefaultValue.Visible = ShowResolveDefaultValue;

        if (EnableSystemFields && ShowSystemFields)
        {
            plcGroup.Visible = true;
            drpSystemFields.Visible = true;
            txtAttributeName.Visible = false;
            lblAttributeName.AssociatedControlID = drpSystemFields.ID;
            drpGroup.Enabled = enableSystemDDL;
            drpSystemFields.Enabled = enableSystemDDL;

            // Get system table name
            string tableName;
            if ((string.IsNullOrEmpty(attributeName)) || (attributeName.ToLowerCSafe().StartsWithCSafe("document")))
            {
                // Fields from table CMS_Document
                tableName = "cms_document";
            }
            else
            {
                // Fields from table CMS_Node
                tableName = "cms_tree";
            }
            drpGroup.SelectedValue = tableName;

            // Select system attribute name in dropdownlist
            SetSystemAttributeName(tableName, attributeName);
        }
        else
        {
            // Show textbox only     
            plcGroup.Visible = false;
            drpSystemFields.Visible = false;
            txtAttributeName.Visible = true;
            txtAttributeName.Text = AttributeName;
        }

        if (FieldInfo != null)
        {
            lblGuidValue.Text = FieldInfo.Guid.ToString();

            txtAttributeName.Text = FieldInfo.Name;

            txtAttributeSize.Text = (FieldInfo.Size <= 0) ? String.Empty : Convert.ToString(FieldInfo.Size);
            txtAttributePrecision.Text = (FieldInfo.Precision < 0) ? String.Empty : Convert.ToString(FieldInfo.Precision);

            // Select attribute type
            string selectedType = FieldInfo.DataType;

            switch (Mode)
            {
                case FieldEditorModeEnum.BizFormDefinition:
                case FieldEditorModeEnum.SystemTable:
                case FieldEditorModeEnum.AlternativeSystemTable:
                case FieldEditorModeEnum.AlternativeBizFormDefinition:
                    if (FormHelper.HasFileUploadControl(FieldInfo))
            {
                        selectedType = FieldDataType.File;
            }
                    break;
            }

            LoadAndSelectAttributeType(selectedType);

            chkRequired.Checked = !FieldInfo.AllowEmpty;
            chkIsSystem.Checked = (FieldInfo.System || (FieldInfo.PrimaryKey && !IsDocumentType));
            chkUnique.Checked = FieldInfo.IsUnique || FieldInfo.PrimaryKey;
            plcIsSystem.Visible = DevelopmentMode;
            chkTranslateField.Checked = FieldInfo.TranslateField;
            chkResolveDefaultValue.Checked = FieldInfo.GetResolveDefaultValue(!ShowResolveDefaultValue);

            drpObjType.Value = DataHelper.GetNotEmpty(FieldInfo.ReferenceToObjectType, String.Empty);
            drpReferenceType.Value = FieldInfo.ReferenceType;
            plcReferenceType.Visible = !String.IsNullOrEmpty(ValidationHelper.GetString(drpObjType.Value, String.Empty));

            if (((Mode == FieldEditorModeEnum.ClassFormDefinition) || (Mode == FieldEditorModeEnum.AlternativeClassFormDefinition)) && !IsDocumentType)
            {
                plcUnique.Visible = true;
            }

            SetDefaultValue();
            ShowDefaultControl();
        }
        // Clear form in FormFieldInfo not specified
        else
        {
            lblGuidValue.Text = null;
            LoadAndSelectAttributeType();
            txtAttributeName.Text = null;
            txtDefaultValue.Value = null;
            txtLargeDefaultValue.Value = String.Empty;

            txtAttributeSize.Text = null;
            txtAttributePrecision.Text = null;

            chkDefaultValue.Value = false;
            rbDefaultValue.Value = null;
            chkRequired.Checked = false;
            chkIsSystem.Checked = DevelopmentMode && !IsDocumentType;
            datetimeDefaultValue.Value = DateTimeHelper.ZERO_TIME;
            plcIsSystem.Visible = DevelopmentMode;
            chkTranslateField.Checked = false;
            chkUnique.Checked = false;
            drpObjType.Value = String.Empty;
            drpReferenceType.Value = "Required";
            plcReferenceType.Visible = false;
            chkResolveDefaultValue.Checked = !ShowResolveDefaultValue;
        }

        // If key is primary, disable "Allow null" checkbox
        if (IsFieldPrimary)
        {
            chkRequired.Checked = true;
            chkUnique.Checked = true;
        }
        chkRequired.Enabled = !IsFieldPrimary;

        // Primary key not allowed to change
        // System field not allowed to change unless development mode
        if ((FieldInfo != null) && (FieldInfo.PrimaryKey || (FieldInfo.System && !DevelopmentMode)))
        {
            bool enableDefault = ((Mode != FieldEditorModeEnum.SystemTable) || IsAlternativeForm || IsInheritedForm) && !FieldInfo.PrimaryKey;
            DisableFieldEditing(FieldInfo.External, enableDefault);
        }
        else
        {
            EnableFieldEditing();
            EnableDisableSections();
        }

        chkIsSystem.Enabled &= !IsDocumentType;

        EnableOrDisableAttributeSize();
    }


    /// <summary>
    /// Sets default value according to attribute type.
    /// </summary>
    public void SetDefaultValue()
    {
        txtDefaultValue.Value = String.Empty;
        txtLargeDefaultValue.Value = String.Empty;
        chkDefaultValue.Value = false;
        datetimeDefaultValue.Value = DateTimeHelper.ZERO_TIME;

        if (FieldInfo != null)
        {
            bool isMacro;
            string defaultValue = FieldInfo.GetPropertyValue(FormFieldPropertyEnum.DefaultValue, out isMacro);

            switch (AttributeType)
            {
                case FieldDataType.DateTime:
                case FieldDataType.Date:
                    {
                        if (isMacro || MacroProcessor.ContainsMacro(defaultValue))
                        {
                            datetimeDefaultValue.SetValue(defaultValue, isMacro);
                        }
                        else if (DateTimeHelper.IsNowOrToday(defaultValue))
                        {
                            datetimeDefaultValue.SetValue(defaultValue);
                        }
                        else if (string.IsNullOrEmpty(defaultValue))
                        {
                            datetimeDefaultValue.Value = String.Empty;
                        }
                        else
                        {
                            datetimeDefaultValue.SetValue(ValidationHelper.GetDateTimeSystem(defaultValue, DateTimeHelper.ZERO_TIME));
                        }
                    }
                    break;

                case FieldDataType.Boolean:
                    if (AllowEmpty)
                    {
                        if (isMacro)
                        {
                            rbDefaultValue.SetValue(defaultValue, true);
                        }
                        else
                        {
                            // Set three state checkbox
                            bool? value = ValidationHelper.GetNullableBoolean(defaultValue, null);
                            if (value.HasValue)
                            {
                                rbDefaultValue.SetValue(value.Value ? 1 : 0);
                            }
                            else
                            {
                                rbDefaultValue.SetValue(-1);
                            }
                        }
                    }
                    else
                    {
                        chkDefaultValue.SetValue(defaultValue, isMacro);
                    }
                    break;

                case FieldDataType.LongText:
                    txtLargeDefaultValue.SetValue(defaultValue, isMacro);
                    break;

                case FieldDataType.Double:
                case FieldDataType.Decimal:
                    if (isMacro || MacroProcessor.ContainsMacro(defaultValue))
                    {
                        txtDefaultValue.SetValue(defaultValue, isMacro);
                    }
                    else
                    {
                        Double dblVal = ValidationHelper.GetDoubleSystem(defaultValue, Double.NaN);
                        txtDefaultValue.SetValue(!Double.IsNaN(dblVal) ? dblVal.ToString() : null);
                    }
                    break;

                default:
                    txtDefaultValue.SetValue(defaultValue, isMacro);
                    break;
            }
        }
    }


    /// <summary>
    /// Validates database configuration. Returns error message if validation fails.
    /// </summary>
    public string Validate()
    {
        var fieldType = AttributeType;

        var dataType = DataTypeManager.GetDataType(TypeEnum.Field, fieldType);
        if (dataType != null)
        {
            int attributeSize = ValidationHelper.GetInteger(AttributeSize, 0);

            if (dataType.VariableSize && (fieldType != FieldDataType.DocAttachments))
            {
                // Attribute size is invalid -> error
                if ((attributeSize <= 0) || (attributeSize > dataType.MaxSize))
                {
                    return String.Format(GetString("TemplateDesigner.ErrorInvalidAttributeSize"), dataType.MaxSize);
                }

                // Validate default value size for string field
                if (!txtDefaultValue.IsMacro && DataTypeManager.IsString(TypeEnum.Field, fieldType))
                {
                    var defValue = ValidationHelper.GetString(txtDefaultValue.Value, String.Empty);
                    if (defValue.Length > attributeSize)
                    {
                        return String.Format(GetString("TemplateDesigner.ErrorDefaultValueSize"), dataType.MaxSize);
                    }
                }
            }

            if (dataType.VariablePrecision)
            {
                int attributePrec = ValidationHelper.GetInteger(AttributePrecision, 0);

                var maxPrecision = dataType.MaxPrecision;
                if (dataType.VariableSize && (attributeSize < maxPrecision))
                {
                    maxPrecision = attributeSize;
                }

                // Attribute size is invalid -> error
                if ((attributePrec < 0) || (attributePrec > maxPrecision))
                {
                    return String.Format(GetString("TemplateDesigner.ErrorInvalidAttributePrecision"), maxPrecision);
                }
            }
        }

        UpdateDefaultValueControlSettings();

        // Get the default value
        var ctrl = GetDefaultValueControl();

        // Validate the value through control itself
        if (!ctrl.IsValid())
        {
            return GetString("TemplateDesigner.ErrorDefaultValue") + " " + ctrl.ValidationError;
        }

        // Validate the default value for proper type
        bool isMacro;
        string defaultValue = GetDefaultValue(out isMacro);

        if (!ctrl.IsMacro)
        {
            // Validate input value
            var checkType = new DataTypeIntegrity(defaultValue, AttributeType);

            var result = checkType.ValidateDataType();
            if (!String.IsNullOrEmpty(result))
            {
                return GetString("TemplateDesigner.ErrorDefaultValue") + " " + result;
            }
        }

        return null;
    }


    /// <summary>
    /// Gets the string default value
    /// </summary>
    public EditingFormControl GetDefaultValueControl()
    {
        switch (AttributeType)
        {
            case FieldDataType.DateTime:
            case FieldDataType.Date:
                return datetimeDefaultValue;

            case FieldDataType.Boolean:
                return AllowEmpty ? rbDefaultValue : chkDefaultValue;

            case FieldDataType.LongText:
                return txtLargeDefaultValue;

            default:
                return txtDefaultValue;
        }
    }


    /// <summary>
    /// Gets the string default value
    /// </summary>
    public string GetDefaultValue(out bool isMacro)
    {
        string defaultValue = null;
        isMacro = false;

        switch (AttributeType)
        {
            case FieldDataType.DateTime:
            case FieldDataType.Date:
                {
                    defaultValue = ValidationHelper.GetString(datetimeDefaultValue.Value, string.Empty);
                    isMacro = datetimeDefaultValue.IsMacro;
                }
                break;

            case FieldDataType.Boolean:
                if (AllowEmpty)
                {
                    isMacro = rbDefaultValue.IsMacro;
                    if (isMacro)
                    {
                        defaultValue = ValidationHelper.GetString(rbDefaultValue.Value, string.Empty);
                    }
                    else
                    {
                        // Positive choice checked - based on ThreeStateCheckBox.ascx
                        if (rbDefaultValue.Value.Equals(1))
                        {
                            defaultValue = "true";
                        }
                        // Negative choice checked - based on ThreeStateCheckBox.ascx
                        if (rbDefaultValue.Value.Equals(0))
                        {
                            defaultValue = "false";
                        }
                    }
                }
                else
                {
                    isMacro = chkDefaultValue.IsMacro;
                    defaultValue = ValidationHelper.GetString(chkDefaultValue.Value, string.Empty);
                }
                break;

            case FieldDataType.LongText:
                {
                    defaultValue = ValidationHelper.GetString(txtLargeDefaultValue.Value, string.Empty).Trim();
                    isMacro = txtLargeDefaultValue.IsMacro;
                }
                break;

            case FormFieldControlTypeCode.DOCUMENT_ATTACHMENTS:
                {
                    string defValue = ValidationHelper.GetString(txtDefaultValue.Value, string.Empty).Trim();
                    isMacro = txtDefaultValue.IsMacro;
                    defaultValue = (string.IsNullOrEmpty(defValue)) ? null : defValue;
                }
                break;

            default:
                if (txtDefaultValue.Visible)
                {
                    defaultValue = ValidationHelper.GetString(txtDefaultValue.Value, string.Empty).Trim();
                    isMacro = txtDefaultValue.IsMacro;
                }
                break;
        }

        return defaultValue;
    }


    /// <summary>
    /// Save settings to the form field info.
    /// </summary>
    public bool Save()
    {
        if (FieldInfo != null)
        {
            FieldInfo.Name = GetAttributeName();
            FieldInfo.AllowEmpty = AllowEmpty;
            FieldInfo.System = IsSystem;
            FieldInfo.TranslateField = TranslateField;
            FieldInfo.IsUnique = IsUnique;
            FieldInfo.ReferenceToObjectType = ReferenceToObjectType;
            FieldInfo.ReferenceType = ReferenceType;

            if (plcResolveDefaultValue.Visible)
            {
                FieldInfo.SetResolveDefaultValue(chkResolveDefaultValue.Checked);
            }

            // Get the default value and save it
            bool isMacro;
            string defaultValue = GetDefaultValue(out isMacro);

            FieldInfo.SetPropertyValue(FormFieldPropertyEnum.DefaultValue, defaultValue, isMacro);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Returns attribute name.
    /// </summary>    
    public string GetAttributeName()
    {
        if (drpSystemFields.Visible)
        {
            return drpSystemFields.SelectedValue;
        }
        return txtAttributeName.Text.Trim();
    }


    /// <summary>
    /// Visible or hides size attribute.
    /// </summary>
    public void EnableOrDisableAttributeSize()
    {
        var fieldType = AttributeType;

        // Check if data type has variable size
        var dataType = DataTypeManager.GetDataType(TypeEnum.Field, fieldType);
        if ((dataType != null) && dataType.VariableSize && (fieldType != FieldDataType.DocAttachments))
        {
            if (IsSystemFieldSelected)
            {
                plcAttributeSize.Visible = false;
            }
            else
            {
                plcAttributeSize.Visible = true;

                // Set default size
                if (String.IsNullOrEmpty(txtAttributeSize.Text) && (dataType.DefaultSize > 0))
                {
                    txtAttributeSize.Text = dataType.DefaultSize.ToString();
                }
            }
        }
        else
        {
            plcAttributeSize.Visible = false;
            txtAttributeSize.Text = String.Empty;
        }

        if ((dataType != null) && dataType.VariablePrecision)
        {
            if (IsSystemFieldSelected)
            {
                plcAttributePrecision.Visible = false;
            }
            else
            {
                plcAttributePrecision.Visible = true;

                // Set default size
                if (String.IsNullOrEmpty(txtAttributePrecision.Text) && (dataType.DefaultPrecision > 0))
                {
                    txtAttributePrecision.Text = dataType.DefaultPrecision.ToString();
                }
            }
        }
        else
        {
            plcAttributePrecision.Visible = false;
            txtAttributePrecision.Text = String.Empty;
        }
    }


    /// <summary>
    /// Disables field editing controls.
    /// </summary>
    /// <param name="enableName">Indicates if field name should remain enabled</param>
    /// <param name="enableDefault">Indicates if control for default value should remain enabled</param>
    public void DisableFieldEditing(bool enableName, bool enableDefault)
    {
        txtDefaultValue.Enabled = enableDefault;
        txtLargeDefaultValue.Enabled = enableDefault;
        chkDefaultValue.Enabled = enableDefault;
        rbDefaultValue.Enabled = enableDefault;
        datetimeDefaultValue.Enabled = enableDefault;
        chkResolveDefaultValue.Enabled = enableDefault;
        drpAttributeType.Enabled = false;
        EnableDisableAttributeName(enableName);
        chkIsSystem.Enabled = false;

        txtAttributeSize.Enabled = false;
        txtAttributePrecision.Enabled = false;

        chkUnique.Enabled = false;
    }


    /// <summary>
    /// Enables field editing controls, except field name.
    /// </summary>
    public void EnableFieldEditing()
    {
        txtDefaultValue.Enabled = true;
        txtLargeDefaultValue.Enabled = true;
        chkDefaultValue.Enabled = true;
        rbDefaultValue.Enabled = true;
        datetimeDefaultValue.Enabled = true;
        drpAttributeType.Enabled = true;
        chkIsSystem.Enabled = true;
        EnableDisableAttributeName(true);
        chkUnique.Enabled = true;
        chkResolveDefaultValue.Enabled = true;
    }


    /// <summary>
    /// Show default value control and required control according to attribute type.
    /// </summary>
    public void ShowDefaultControl()
    {
        plcDefaultValue.Visible = true;
        SetFieldForTranslations();
        SetReferenceToField();
        HandleRequiredVisiblity();

        switch (AttributeType)
        {
            case FieldDataType.DateTime:
            case FieldDataType.Date:
                {
                    datetimeDefaultValue.Visible = true;
                    chkDefaultValue.Visible = false;
                    rbDefaultValue.Visible = false;
                    txtLargeDefaultValue.Visible = false;
                    txtDefaultValue.Visible = false;

                    var calendarControl = ((FormEngineUserControl)datetimeDefaultValue.NestedControl);

                    lblDefaultValue.AssociatedControlClientID = EditingFormControl.GetInputClientID(calendarControl.Controls);
                }
                break;

            case FieldDataType.Boolean:
                {
                    chkDefaultValue.Visible = !AllowEmpty;
                    rbDefaultValue.Visible = AllowEmpty;
                    txtLargeDefaultValue.Visible = false;
                    txtDefaultValue.Visible = false;
                    datetimeDefaultValue.Visible = false;
                    lblDefaultValue.AssociatedControlClientID = AllowEmpty ? EditingFormControl.GetInputClientID(rbDefaultValue.NestedControl.Controls) : EditingFormControl.GetInputClientID(chkDefaultValue.NestedControl.Controls);
                }
                break;

            case FieldDataType.LongText:
                {
                    txtLargeDefaultValue.Visible = true;
                    chkDefaultValue.Visible = false;
                    rbDefaultValue.Visible = false;
                    txtDefaultValue.Visible = false;
                    datetimeDefaultValue.Visible = false;
                    lblDefaultValue.AssociatedControlClientID = EditingFormControl.GetInputClientID(txtLargeDefaultValue.NestedControl.Controls);
                }
                break;

            case FieldDataType.Binary:
                plcDefaultValue.Visible = false;
                break;

            case FieldDataType.File:
            case FieldDataType.DocAttachments:
                // Hide default value for File and Document attachment fields within Document types
                if ((Mode == FieldEditorModeEnum.ClassFormDefinition) || (Mode == FieldEditorModeEnum.AlternativeClassFormDefinition))
                {
                    plcDefaultValue.Visible = false;
                }
                // Display textbox otherwise
                else
                {
                    txtDefaultValue.Visible = true;
                    chkDefaultValue.Visible = false;
                    rbDefaultValue.Visible = false;
                    txtLargeDefaultValue.Visible = false;
                    datetimeDefaultValue.Visible = false;
                    lblDefaultValue.AssociatedControlClientID = EditingFormControl.GetInputClientID(txtDefaultValue.NestedControl.Controls);
                }
                break;

            default:
                {
                    txtDefaultValue.Visible = true;
                    chkDefaultValue.Visible = false;
                    rbDefaultValue.Visible = false;
                    txtLargeDefaultValue.Visible = false;
                    datetimeDefaultValue.Visible = false;
                    lblDefaultValue.AssociatedControlClientID = EditingFormControl.GetInputClientID(txtDefaultValue.NestedControl.Controls);
                }
                break;
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Fill attribute types list.
    /// </summary>
    /// <param name="selectedType">Type that will be selected after loading the list.</param>
    private void LoadAndSelectAttributeType(string selectedType = FieldDataType.Text)
        {
        var types = FormHelper.GetFieldTypes(IsDocumentType, DevelopmentMode).ToHashSet();

        // Remove file type for these objects 
        switch (Mode)
        {
            case FieldEditorModeEnum.WebPartProperties:
            case FieldEditorModeEnum.CustomTable:
            case FieldEditorModeEnum.SystemTable:
            case FieldEditorModeEnum.FormControls:
            case FieldEditorModeEnum.ProcessActions:
            case FieldEditorModeEnum.Widget:
            case FieldEditorModeEnum.AlternativeCustomTable:
            case FieldEditorModeEnum.AlternativeSystemTable:
            case FieldEditorModeEnum.InheritedFormControl:
                types.Remove(FieldDataType.File);
                break;
        }

        // Ensure selected type
        types.Add(selectedType);

        drpAttributeType.DataSource = ControlsHelper.GetDropDownListSource(types, "TemplateDesigner.FieldTypes");
        drpAttributeType.DataBind();

        if (drpAttributeType.Items.Count > 0)
        {
            drpAttributeType.SelectedValue = selectedType;
        }
    }


    /// <summary>
    /// Sets system attribute name.
    /// </summary>
    private void SetSystemAttributeName(string tableName, string attributeName)
    {
        TableManager tm = new TableManager(null);

        // Load system fields from specified table
        drpSystemFields.DataSource = tm.GetColumnInformation(tableName);
        drpSystemFields.DataBind();

        if (!string.IsNullOrEmpty(attributeName) && (drpSystemFields.Items.FindByValue(attributeName) != null))
        {
            drpSystemFields.SelectedValue = attributeName;
        }
        else if (drpSystemFields.Items.Count > 0)
        {
            drpSystemFields.SelectedIndex = 0;
        }
    }


    /// <summary>
    /// Enable or disable field for translation according to actual Attribute Type (from DropDown list)
    /// </summary>
    private void SetFieldForTranslations()
    {
        if (TranslationServiceHelper.IsFieldForTranslation(AttributeType) && !IsDummyField && !IsExtraField)
        {
            plcTranslation.Visible = true;
        }
        else
        {
            plcTranslation.Visible = false;
            chkTranslateField.Checked = false;
        }
    }


    /// <summary>
    /// Enable or disable field for setting FK references.
    /// </summary>
    private void SetReferenceToField()
    {
        if (AttributeType.EqualsCSafe(FieldDataType.Integer, true) &&
            !IsDummyField && !IsExtraField && !IsFieldPrimary &&
            (Mode == FieldEditorModeEnum.ClassFormDefinition) && !IsDocumentType)
        {
            plcReference.Visible = true;
        }
        else
        {
            plcReference.Visible = false;
        }
    }


    /// <summary>
    /// Enables or disables control related to attribute name.
    /// </summary>
    private void EnableDisableAttributeName(bool enable)
    {
        drpGroup.Enabled = enable && IsNewItemEdited;
        drpSystemFields.Enabled = enable && IsNewItemEdited;
        txtAttributeName.Enabled = enable;
    }


    /// <summary>
    /// Enables or disables sections of the unigrid according to the selected mode.
    /// </summary>
    private void EnableDisableSections()
    {
        bool enable = (!IsAlternativeForm && !IsInheritedForm) || (IsDummyField && !IsDummyFieldFromMainForm) || IsExtraField;

        EnableDisableAttributeName(enable);
        drpAttributeType.Enabled = enable && (!IsSystem || !IsSystemFieldSelected && DevelopmentMode);
        txtAttributeSize.Enabled = enable;
        txtAttributePrecision.Enabled = enable;
    }


    private void HandleRequiredVisiblity()
    {
        // Can change required value only for new fields in system tables.
        plcRequired.Visible = (Mode != FieldEditorModeEnum.SystemTable) || (IsNewItemEdited && SystemContext.DevelopmentMode);

        // Binary and document attachment fields can never be required.
        if ((AttributeType == FieldDataType.Binary) || (AttributeType == FieldDataType.DocAttachments))
        {
            plcRequired.Visible = false;
            chkRequired.Checked = false;
        }

        // Only not required can be made required in alternative forms, not vice versa, unless the field has no database representation
        if (!IsNewItemEdited && (IsAlternativeForm || IsInheritedForm) && !IsDummyField && !IsExtraField && !GetFormAllowEmpty())
        {
            plcRequired.Visible = false;
        }
    }


    /// <summary>
    /// Used only for alternative forms. If current field in class allows empty then it returns TRUE.
    /// </summary>
    private bool GetFormAllowEmpty()
    {
        // Check if field exists in class
        FormInfo fi = FormHelper.GetFormInfo(ClassName, false);
        FormFieldInfo ffi;
        if (fi != null)
        {
            ffi = fi.GetFormField(FieldInfo.Name);
            if (ffi != null)
            {
                return ffi.AllowEmpty;
            }
        }

        // Check if field exists in coupled class
        if (!String.IsNullOrEmpty(CoupledClassName))
        {
            fi = FormHelper.GetFormInfo(CoupledClassName, false);
            if (fi != null)
            {
                ffi = fi.GetFormField(FieldInfo.Name);
                if (ffi != null)
                {
                    return ffi.AllowEmpty;
                }
            }
        }

        return false;
    }

    #endregion
}