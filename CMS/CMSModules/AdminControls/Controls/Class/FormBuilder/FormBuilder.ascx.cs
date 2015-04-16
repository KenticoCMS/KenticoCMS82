using System;
using System.Collections;
using System.Linq;
using System.Web.UI;

using CMS.Base;
using CMS.DataEngine;
using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_AdminControls_Controls_Class_FormBuilder_FormBuilder : BaseFieldEditor, ICallbackEventHandler
{
    #region "Variables"

    private string mCallbackResult = string.Empty;
    private bool mReloadField;
    private bool mReloadForm;

    #endregion


    #region "Properties"

    /// <summary>
    /// Control which is used to design the form.
    /// </summary>
    public BasicForm Form
    {
        get
        {
            return formElem;
        }
    }


    /// <summary>
    /// Field name.
    /// </summary>
    private string FieldName
    {
        get
        {
            return ValidationHelper.GetString(ViewState["FieldName"], string.Empty);
        }
        set
        {
            ViewState["FieldName"] = value;
        }
    }


    /// <summary>
    /// Gets or sets Name of the macro rule category(ies) which should be displayed in Rule designer. Items should be separated by semicolon.
    /// </summary>
    public string ValidationRulesCategory
    {
        get
        {
            return ValidationHelper.GetString(UIContext["ConditionsCategory"], string.Empty);
        }
        set
        {
            UIContext["ConditionsCategory"] = value;
        }
    }


    /// <summary>
    /// Messages placeholder.
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMessagesHolder;
        }
    }

    #endregion


    #region "Control events"

    protected override void LoadViewState(object savedState)
    {
        base.LoadViewState(savedState);

        if (ViewState["CurrentFormFields"] != null)
        {
            // Refresh UIContext data
            UIContext["CurrentFormFields"] = ViewState["CurrentFormFields"];
        }
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // Load form info
        FormInfo = FormHelper.GetFormInfo(ClassName, true);
        if (FormInfo != null)
        {
            ScriptHelper.RegisterJQueryUI(Page);
            ScriptHelper.RegisterScriptFile(Page, "~/CMSModules/AdminControls/Controls/Class/FormBuilder/FormBuilder.js");

            // Set up callback script
            String cbReference = Page.ClientScript.GetCallbackEventReference(this, "arg", "FormBuilder.receiveServerData", String.Empty);
            String callbackScript = "function doFieldAction(arg) {" + cbReference + "; }";
            ScriptHelper.RegisterClientScriptBlock(Page, GetType(), "FormBuilderCallback", callbackScript, true);

            // Prepare Submit button
            formElem.SubmitButton.RegisterHeaderAction = false;
            formElem.SubmitButton.OnClientClick = formElem.SubmitImageButton.OnClientClick = "return false;";

            formElem.FormInformation = FormInfo;
            formElem.Data = new DataRowContainer(FormInfo.GetDataRow());

            // Load form
            formElem.ReloadData();

            // Prepare error message label
            MessagesPlaceHolder.ErrorLabel.CssClass += " form-builder-error-hidden";
            MessagesPlaceHolder.ErrorText = GetString("FormBuilder.GeneralError");

            InitUIContext(FormInfo);
        }
        else
        {
            formElem.StopProcessing = true;
            ShowError(GetString("FormBuilder.ErrorLoadingForm"));
        }

        if (RequestHelper.IsPostBack())
        {
            ProcessAjaxPostBack();
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (FormInfo == null)
        {
            return;
        }

        // Reload selected field if required
        if (mReloadField && !string.IsNullOrEmpty(FieldName))
        {
            Form.ReloadData();
            if (Form.FieldUpdatePanels.ContainsKey(FieldName))
            {
                Form.FieldUpdatePanels[FieldName].Update();

                string script = String.Format("if (window.RecalculateFormWidth) {{RecalculateFormWidth('{0}');}}", formElem.ClientID);
                ScriptHelper.RegisterStartupScript(Page, pnlUpdateForm.GetType(), "recalculateFormWidth" + ClientID, ScriptHelper.GetScript(script));
            }
            else
            {
                Form.StopProcessing = true;
                MessagesPlaceHolder.ShowError(String.Format("{0} {1}", GetString("editedobject.notexists"), GetString("formbuilder.refresh")));

                MessagesPlaceHolder.ErrorLabel.CssClass += " form-builder-error";
                pnlUpdateForm.Update();
            }
        }

        // Reload whole form
        if (mReloadForm && !Form.StopProcessing)
        {
            formElem.ReloadData();
            pnlUpdateForm.Update();

            ScriptHelper.RegisterStartupScript(pnlUpdateForm, pnlUpdateForm.GetType(), "FormBuilderAddComponent", "FormBuilder.init(); FormBuilder.selectField('" + FieldName + "')", true);
        }

        // Display placeholder with message if form has no visible components and hide OK button
        if (FormInfo.GetFormElements(true, false).Count == 0)
        {
            ScriptHelper.RegisterStartupScript(pnlUpdateForm, pnlUpdateForm.GetType(), "FormBuilderShowPlaceholder", "FormBuilder.showEmptyFormPlaceholder();", true);
            formElem.SubmitButton.Visible = false;
        }

        // Set settings panel visibility
        pnlSettings.SetSettingsVisibility(!string.IsNullOrEmpty(FieldName));
    }

    #endregion


    #region "Methods"

    private void ProcessAjaxPostBack()
    {
        if (RequestHelper.IsPostBack())
        {
            string eventArgument = Request.Params.Get("__EVENTARGUMENT");

            if (!string.IsNullOrEmpty(eventArgument))
            {
                string[] data = eventArgument.Split(':');

                switch (data[0])
                {
                    case "loadSettings":
                        {
                            FieldName = data[1];
                            LoadSettings(FieldName);
                        }
                        break;

                    case "remove":
                        {
                            // Hide selected field from form
                            FieldName = string.Empty;
                            HideField(data[2]);
                            mReloadForm = true;
                            pnlSettings.Update();
                        }
                        break;

                    case "hideSettingsPanel":
                        {
                            FieldName = string.Empty;
                            pnlSettings.Update();
                        }
                        break;

                    case "saveSettings":
                        {
                            FormFieldInfo ffi = FormInfo.GetFormField(FieldName);
                            FormFieldInfo originalFieldInfo = (FormFieldInfo)ffi.Clone();
                            pnlSettings.SaveSettings(ffi);
                            
                            SaveFormDefinition(originalFieldInfo, ffi);
                            mReloadField = true;
                        }
                        break;

                    case "addField":
                        {
                            FormFieldInfo ffi = PrepareNewField(data[1]);
                            FieldName = ffi.Name;

                            var errorMessage = AddField(ffi, data[2], ValidationHelper.GetInteger(data[3], -1));
                            if (!String.IsNullOrEmpty(errorMessage))
                            {
                                ShowError(errorMessage);
                                return;
                            }

                            LoadSettings(FieldName);
                            mReloadForm = true;
                        }
                        break;
                }
            }
        }
    }


    /// <summary>
    /// Loads field settings to the Settings panel.
    /// </summary>
    /// <param name="fieldName">Field name</param>
    private void LoadSettings(string fieldName)
    {
        FormFieldInfo ffi = FormInfo.GetFormField(fieldName);
        pnlSettings.LoadSettings(ffi);
        pnlSettings.Update();
    }


    /// <summary>
    /// Hides field.
    /// </summary>
    /// <param name="fieldName">Name of field that should be hidden</param>
    /// <returns>Error message if an error occurred</returns>
    protected string HideField(string fieldName)
    {
        if (!string.IsNullOrEmpty(fieldName))
        {
            FormFieldInfo ffiSelected = FormInfo.GetFormField(fieldName);
            if (ffiSelected == null)
            {
                return GetString("editedobject.notexists");
            }

            ffiSelected.Visible = false;
            return SaveFormDefinition();
        }
        return string.Empty;
    }


    /// <summary>
    /// Saves form definition. Updates database column if both original and changed info is passed and the change requires database update.
    /// </summary>
    /// <param name="oldFieldInfo">Form field info prior to the change</param>
    /// <param name="updatedFieldInfo">Form field info after the change has been made.</param>
    /// <returns>Error message if an error occurred</returns>
    protected string SaveFormDefinition(FormFieldInfo oldFieldInfo = null, FormFieldInfo updatedFieldInfo = null)
    {
		if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.form", "EditForm"))
		{
			if (RequestHelper.IsCallback())
			{
				return GetString("formbuilder.missingeditpermission");
			}

			RedirectToAccessDenied("cms.form", "EditForm");
		}

	    DataClassInfo dci = DataClassInfoProvider.GetDataClassInfo(ClassName);

        if ((FormInfo != null) && (dci != null))
        {
            RaiseBeforeDefinitionUpdate();

            // Update database column of the changed field
            if (IsDatabaseChangeRequired(oldFieldInfo, updatedFieldInfo))
            {
                // Ensure the transaction
                using (var tr = new CMSLateBoundTransaction())
                {
                    TableManager tm = new TableManager(dci.ClassConnectionString);
                    tr.BeginTransaction();

                    UpdateDatabaseColumn(oldFieldInfo, updatedFieldInfo, tm, dci.ClassTableName);
                    
                    // Commit the transaction
                    tr.Commit();
                }
            }

            // Update form definition   
            dci.ClassFormDefinition = FormInfo.GetXmlDefinition();

            // Save the class data
            DataClassInfoProvider.SetDataClassInfo(dci);

            ClearHashtables();

            RaiseAfterDefinitionUpdate();

            // Update inherited classes with new fields
            FormHelper.UpdateInheritedClasses(dci);

            return string.Empty;
        }

        return GetString("FormBuilder.ErrorSavingForm");
    }


    /// <summary>
    /// Prepares new field.
    /// </summary>
    /// <param name="controlName">Code name of used control</param>
    private FormFieldInfo PrepareNewField(string controlName)
    {
        FormFieldInfo ffi = new FormFieldInfo();

        string[] controlDefaultDataType = FormUserControlInfoProvider.GetUserControlDefaultDataType(controlName);
        ffi.DataType = controlDefaultDataType[0];
        ffi.Size = ValidationHelper.GetInteger(controlDefaultDataType[1], 0);
        ffi.FieldType = FormFieldControlTypeEnum.CustomUserControl;

        FormUserControlInfo control = FormUserControlInfoProvider.GetFormUserControlInfo(controlName);
        if (control != null)
        {
            ffi.SetPropertyValue(FormFieldPropertyEnum.FieldCaption, control.UserControlDisplayName);
        }

        ffi.AllowEmpty = true;
        ffi.PublicField = true;
        ffi.Name = GetUniqueFieldName(controlName);
        ffi.Settings["controlname"] = controlName;

        // For list controls create three default options
        if (FormHelper.HasListControl(ffi))
        {
            SpecialFieldsDefinition optionDefinition = new SpecialFieldsDefinition();

            for (int i = 1; i <= 3; i++)
            {
                optionDefinition.Add(new SpecialField
                {
                    Value = OptionsDesigner.DEFAULT_OPTION + i,
                    Text = OptionsDesigner.DEFAULT_OPTION + i
                });
            }

            ffi.Settings["Options"] = optionDefinition.ToString();
        }

        if (controlName.EqualsCSafe("CalendarControl"))
        {
            ffi.Settings["EditTime"] = false;
        }

        return ffi;
    }


    /// <summary>
    /// Ensures unique field name.
    /// </summary>
    /// <param name="name">Field name</param>
    private string GetUniqueFieldName(string name)
    {
        int uniqueIndex = 1;
        bool unique = false;
        string uniqueName = name;

        while (!unique)
        {
            if (FormInfo.GetFormField(uniqueName) == null)
            {
                unique = true;
            }
            else
            {
                uniqueName = name + "_" + uniqueIndex;
                uniqueIndex++;
            }
        }
        return uniqueName;
    }


    /// <summary>
    /// Adds form field info to the form to the specified position.
    /// </summary>
    /// <param name="ffi">Form field info which will be added</param>
    /// <param name="category">Category name</param>
    /// <param name="position">Field position in the category</param>
    private string AddField(FormFieldInfo ffi, string category, int position)
    {
		if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.form", "EditForm"))
		{
			RedirectToAccessDenied("cms.form", "EditForm");
		}

        var dci = DataClassInfoProvider.GetDataClassInfo(ClassName);
        if (dci != null)
        {
            RaiseBeforeDefinitionUpdate();

            // Ensure the transaction
            using (var tr = new CMSLateBoundTransaction())
            {
                string columnType = DataTypeManager.GetSqlType(ffi.DataType, ffi.Size, ffi.Precision);

                TableManager tm = new TableManager(dci.ClassConnectionString);
                tr.BeginTransaction();

                // Add new column
                tm.AddTableColumn(dci.ClassTableName, ffi.Name, columnType, true, null);

                // Add field to form  
                FormInfo.AddFormItem(ffi);
                if (!String.IsNullOrEmpty(category) || position >= 0)
                {
                    FormInfo.MoveFormFieldToPositionInCategory(ffi.Name, category, position);
                }

                // Update form definition
                dci.ClassFormDefinition = FormInfo.GetXmlDefinition();

                // Update class schema
                dci.ClassXmlSchema = tm.GetXmlSchema(dci.ClassTableName);

                try
                {
                    // Save the class data
                    DataClassInfoProvider.SetDataClassInfo(dci);
                }
                catch (Exception)
                {
                    return GetString("FormBuilder.ErrorSavingForm");
                }

                // Generate default view
                SqlGenerator.GenerateDefaultView(dci, SiteContext.CurrentSiteName);
                QueryInfoProvider.ClearDefaultQueries(dci, true, true);

                // Hide field for alternative forms that require it
                HideFieldInAlternativeForms(ffi, dci);

                // Commit the transaction
                tr.Commit();
            }

            ClearHashtables();

            RaiseAfterDefinitionUpdate();

            // Update inherited classes with new fields
            FormHelper.UpdateInheritedClasses(dci);
        }
        else
        {
            return GetString("FormBuilder.ErrorSavingForm");
        }

        return string.Empty;
    }


    /// <summary>
    /// Clears HashTables.
    /// </summary>
    private void ClearHashtables()
    {
        // Clear the object type hashtable
        ProviderStringDictionary.ReloadDictionaries(ClassName, true);

        // Clear the classes hashtable
        ProviderStringDictionary.ReloadDictionaries("cms.class", true);

        // Clear class structures
        ClassStructureInfo.Remove(ClassName, true);

        // Clear form resolver
        FormControlsResolvers.ClearResolvers(true);
    }


    /// <summary>
    /// Initializes UI context.
    /// </summary>
    private void InitUIContext(FormInfo formInfo)
    {
        if (formInfo != null)
        {
            ArrayList result = new ArrayList();

            // Get all fields except the system ones
            var fields = formInfo.GetFields(true, true);

            foreach (FormFieldInfo ffi in fields)
            {
                string caption = ffi.Caption;
                if (String.IsNullOrEmpty(caption))
                {
                    caption = ffi.Name;
                }

                if (fields.Any(f => (f.Name != ffi.Name) && (f.Caption == caption)))
                {
                    // Add field name if more fields have similar caption
                    caption += String.Format(" [{0}]", ffi.Name);
                }
                result.Add(String.Format("{0};{1}", ffi.Guid, caption));
            }

            UIContext["CurrentFormFields"] = result;
            ViewState["CurrentFormFields"] = result;
        }
    }

    #endregion


    #region "ICallbackEventHandler Members"

    /// <summary>
    /// Callback result retrieving handler.
    /// </summary>
    public string GetCallbackResult()
    {
        return mCallbackResult;
    }


    /// <summary>
    /// Raise callback method.
    /// </summary>
    public void RaiseCallbackEvent(string eventArgument)
    {
        if (!string.IsNullOrEmpty(eventArgument) && (FormInfo != null))
        {
            string[] data = eventArgument.Split(':');

            // Check that data are in proper format
            if (data.Length >= 3)
            {
                switch (data[0])
                {
                    case "move":
                        int position = ValidationHelper.GetInteger(data[3], -1);
                        string category = data[2];
                        string fieldName = data[1];

                        // Check field existence
                        FormFieldInfo field = FormInfo.GetFormField(fieldName);
                        string errorMessage;
                        if (field != null)
                        {
                            // Move field to new position
                            FormInfo.MoveFormFieldToPositionInCategory(fieldName, category, position);
                            errorMessage = SaveFormDefinition();
                        }
                        else
                        {
                            errorMessage = GetString("editedobject.notexists");
                        }
                        mCallbackResult = PrepareCallbackResult(string.Empty, errorMessage);
                        break;
                }
            }
        }
    }


    /// <summary>
    /// Returns mCallbackResult or errorMessage in proper format if not empty.
    /// </summary>
    /// <param name="callbackResult">String which will be returned if errorMessage is empty</param>
    /// <param name="errorMessage">Error message</param>
    private string PrepareCallbackResult(string callbackResult, string errorMessage)
    {
        return string.IsNullOrEmpty(errorMessage) ? callbackResult : "error:" + String.Format("{0} {1}", errorMessage, GetString("formbuilder.refresh")).Replace(":", "##COLON##");
    }

    #endregion
}