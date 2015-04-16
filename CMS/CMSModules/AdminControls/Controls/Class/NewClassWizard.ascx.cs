using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;

using CMS.CustomTables;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.Base;
using CMS.Search;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.EventLog;
using CMS.Synchronization;
using CMS.Modules;

public partial class CMSModules_AdminControls_Controls_Class_NewClassWizard : CMSUserControl
{
    #region "Constants"

    /// <summary>
    /// Default Icon CSS class for coupled document types.
    /// </summary>
    private const string DEFAULT_COUPLED_CLASS_ICON = "icon-doc-o";

    /// <summary>
    /// Default icon CSS class for container (doesn't contain any fields) document types.
    /// </summary>
    private const string DEFAULT_CLASS_ICON = "icon-folder-o";

    #endregion


    #region "Private fields"

    private NewClassWizardModeEnum mMode = NewClassWizardModeEnum.DocumentType;

    private string mStep6Description = "documenttype_new_step6.description";

    #endregion


    #region "Private properties"

    /// <summary>
    /// Edited DataClassInfo
    /// </summary>
    private DataClassInfo DataClassInfo
    {
        get;
        set;
    }


    /// <summary>
    /// Name of the new created class.
    /// </summary>
    private string ClassName
    {
        get
        {
            object obj = ViewState["ClassName"];
            return (obj == null) ? string.Empty : (string)obj;
        }

        set
        {
            ViewState["ClassName"] = value;
        }
    }


    /// <summary>
    /// Indicates whether steps 3 and 4 were omitted or not.
    /// </summary>
    private bool SomeStepsOmitted
    {
        get
        {
            object obj = ViewState["SomeStepsOmitted"];
            return (obj != null) && (bool)obj;
        }

        set
        {
            ViewState["SomeStepsOmitted"] = value;
        }
    }


    /// <summary>
    /// Indicates whether steps 3 and 4 were omitted or not.
    /// </summary>
    private bool IsContainer
    {
        get
        {
            object obj = ViewState["IsContainer"];
            return (obj != null) && (bool)obj;
        }

        set
        {
            ViewState["IsContainer"] = value;
        }
    }


    /// <summary>
    /// Gets steps count for selected mode.
    /// </summary>
    private int StepsCount
    {
        get
        {
            switch (Mode)
            {
                case NewClassWizardModeEnum.Class:
                    return 4;

                case NewClassWizardModeEnum.CustomTable:
                    return 5;

                default:
                    return IsContainer ? 5 : 7;
            }
        }
    }

    #endregion


    #region "Public properties"

    /// <summary>
    /// Current theme applied.
    /// </summary>
    public string Theme
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates whether wizard sets the inner field editor to development mode - can edit system fields.
    /// </summary>
    public bool SystemDevelopmentMode
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the wizard mode - what item is created.
    /// </summary>
    public NewClassWizardModeEnum Mode
    {
        get
        {
            return mMode;
        }

        set
        {
            mMode = value;
        }
    }


    /// <summary>
    /// Description resource string used in sixth step.
    /// </summary>
    public string Step6Description
    {
        get
        {
            return mStep6Description;
        }
        set
        {
            mStep6Description = value;
        }
    }


    /// <summary>
    /// Gets or sets the module where new class belongs to.
    /// </summary>
    public int ModuleID
    {
        get;
        set;
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Set controls of the first step
        if (!RequestHelper.IsPostBack())
        {
            wzdStep1.EnableViewState = true;
            wzdStep2.EnableViewState = false;
            wzdStep3.EnableViewState = false;
            wzdStep4.EnableViewState = false;
            wzdStep5.EnableViewState = false;
            wzdStep6.EnableViewState = false;
            wzdStep7.EnableViewState = false;

            switch (Mode)
            {
                // If the wizard is running as new document type wizard     
                case NewClassWizardModeEnum.DocumentType:
                    {
                        lblDisplayName.Text = GetString("DocumentType_New.DisplayName");
                        lblFullCodeName.Text = GetString("DocumentType_New.FullCodeName");
                        lblNamespaceName.Text = GetString("DocumentType_New.NamespaceName");
                        lblCodeName.Text = GetString("DocumentType_New.CodeName");

                        // Set validators' error messages
                        rfvDisplayName.ErrorMessage = GetString("DocumentType_New.ErrorEmptyDisplayName");
                        rfvCodeName.ErrorMessage = GetString("DocumentType_New.ErrorEmptyCodeName");
                        rfvNamespaceName.ErrorMessage = GetString("DocumentType_New.ErrorEmptyNamespaceName");
                        revNameSpaceName.ErrorMessage = GetString("DocumentType_New.NamespaceNameIdentifier");
                        revCodeName.ErrorMessage = GetString("DocumentType_New.CodeNameIdentifier");

                        txtNamespaceName.Text = "custom";

                        ucHeader.Description = GetString("DocumentType_New_Step1.Description");
                    }
                    break;

                // If the wizzard is running as new data class wizzard    
                case NewClassWizardModeEnum.Class:
                    {
                        lblDisplayName.Text = GetString("sysdev.class_new.DisplayName");
                        lblFullCodeName.Text = GetString("sysdev.class_new.FullCodeName");
                        lblNamespaceName.Text = GetString("DocumentType_New.NamespaceName");
                        lblCodeName.Text = GetString("sysdev.class_new.CodeName");

                        // Set validators' error messages
                        rfvDisplayName.ErrorMessage = GetString("sysdev.class_new.ErrorEmptyDisplayName");
                        rfvCodeName.ErrorMessage = GetString("sysdev.class_new.ErrorEmptyCodeName");
                        rfvNamespaceName.ErrorMessage = GetString("sysdev.class_new.ErrorEmptyNamespaceName");
                        revNameSpaceName.ErrorMessage = GetString("sysdev.class_new.NamespaceNameIdentifier");
                        revCodeName.ErrorMessage = GetString("sysdev.class_new.CodeNameIdentifier");

                        // Get current module name
                        string moduleName = BaseAbstractInfoProvider.GetCodeName(ResourceInfo.OBJECT_TYPE, ModuleID);

                        txtNamespaceName.Text = moduleName.Contains(".") ? moduleName.Substring(0, moduleName.IndexOf('.')) : moduleName;
                        ucHeader.Description = GetString("sysdev.class_new_Step1.Description");
                    }
                    break;

                // If the wizzard is running as new custom table wizzard
                case NewClassWizardModeEnum.CustomTable:
                    {
                        lblDisplayName.Text = GetString("customtable.newwizzard.DisplayName");
                        lblFullCodeName.Text = GetString("customtable.newwizzard.FullCodeName");
                        lblNamespaceName.Text = GetString("customtable.newwizzard.NamespaceName");
                        lblCodeName.Text = GetString("customtable.newwizzard.CodeName");

                        // Set validators' error messages
                        rfvDisplayName.ErrorMessage = GetString("customtable.newwizzard.ErrorEmptyDisplayName");
                        rfvCodeName.ErrorMessage = GetString("customtable.newwizzard.ErrorEmptyCodeName");
                        rfvNamespaceName.ErrorMessage = GetString("customtable.newwizzard.ErrorEmptyNamespaceName");
                        revNameSpaceName.ErrorMessage = GetString("customtable.newwizzard.NamespaceNameIdentifier");
                        revCodeName.ErrorMessage = GetString("customtable.newwizzard.CodeNameIdentifier");

                        txtNamespaceName.Text = "customtable";
                        ucHeader.Description = GetString("customtable.newwizzard.Step1Description");
                    }
                    break;
            }

            // Set regular expression for identifier validation
            revNameSpaceName.ValidationExpression = ValidationHelper.IdentifierRegExp.ToString();
            revCodeName.ValidationExpression = ValidationHelper.IdentifierRegExp.ToString();

            wzdStep1.Title = GetString("general.general");
        }
        else
        {
            // Disable regular expression validations in next steps
            revNameSpaceName.Enabled = false;
            revCodeName.Enabled = false;
        }

        var objectType = GetObjectType();

        DataClassInfo = DataClassInfo.New(objectType);
        DataClassInfo.ClassFormDefinition = FormInfo.GetEmptyFormDocument().OuterXml;

        // Set edited object
        EditedObject = DataClassInfo;

        // Set FieldEditor's properties
        FieldEditor.ClassName = ClassName;
        FieldEditor.Mode = FieldEditorModeEnum.ClassFormDefinition;

        // Restrict controls if custom tables
        if (Mode == NewClassWizardModeEnum.CustomTable)
        {
            FieldEditor.Mode = FieldEditorModeEnum.CustomTable;
        }

        // Set field editor's development mode
        FieldEditor.DevelopmentMode = SystemDevelopmentMode;
        FieldEditor.IsWizard = true;

        if (!SystemDevelopmentMode && (Mode == NewClassWizardModeEnum.DocumentType))
        {
            FieldEditor.EnableSystemFields = true;
        }

        wzdNewDocType.ActiveStepChanged += wzdNewDocType_ActiveStepChanged;

        // Set buttons' text                
        wzdNewDocType.StartNextButtonText = GetString("general.next");
        wzdNewDocType.StepNextButtonText = wzdNewDocType.StartNextButtonText;
        wzdNewDocType.FinishCompleteButtonText = GetString("general.Finish");

        // Do not provide suffix for breadcrumbs
        UIHelper.SetBreadcrumbsSuffix("");
    }


    /// <summary>
    /// Gets the object type of the created class
    /// </summary>
    private string GetObjectType()
    {
        string objectType = DataClassInfo.OBJECT_TYPE;

        switch (Mode)
        {
            case NewClassWizardModeEnum.DocumentType:
                objectType = DataClassInfo.OBJECT_TYPE_DOCUMENTTYPE;
                break;

            case NewClassWizardModeEnum.CustomTable:
                objectType = DataClassInfo.OBJECT_TYPE_CUSTOMTABLE;
                break;
        }

        return objectType;
    }


    void wzdNewDocType_ActiveStepChanged(object sender, EventArgs e)
    {
        // Field editor needs to be visible to be able to reload controls properly
        if (wzdNewDocType.ActiveStep == wzdStep3)
        {
            FieldEditor.Reload(null);
        }
    }


    protected void Page_PreRender()
    {
        // Set current step title
        ucHeader.Header = GetCurrentStepTitle(wzdNewDocType.ActiveStepIndex);
        ucHeader.Title = string.Format(GetString("DocumentType_New.Step"), wzdNewDocType.ActiveStepIndex + 1, StepsCount);

        // Manage steps by mode
        switch (Mode)
        {
            case NewClassWizardModeEnum.DocumentType:
                if (IsContainer)
                {
                    if (wzdNewDocType.ActiveStepIndex == 4)
                    {
                        ucHeader.Title = string.Format(GetString("DocumentType_New.Step"), 3, StepsCount);
                    }
                    else if (wzdNewDocType.ActiveStepIndex == 5)
                    {
                        ucHeader.Title = string.Format(GetString("DocumentType_New.Step"), 4, StepsCount);
                    }
                    else if (wzdNewDocType.ActiveStepIndex == 6)
                    {
                        ucHeader.Title = string.Format(GetString("DocumentType_New.Step"), 5, StepsCount);
                    }
                }
                break;

            case NewClassWizardModeEnum.Class:
                if (wzdNewDocType.ActiveStepIndex == 6)
                {
                    ucHeader.Title = string.Format(GetString("DocumentType_New.Step"), 4, StepsCount);
                }
                break;

            case NewClassWizardModeEnum.CustomTable:
                if (wzdNewDocType.ActiveStepIndex == 5)
                {
                    ucHeader.Title = string.Format(GetString("DocumentType_New.Step"), 4, StepsCount);
                }

                if (wzdNewDocType.ActiveStepIndex == 6)
                {
                    ucHeader.Title = string.Format(GetString("DocumentType_New.Step"), 5, StepsCount);
                }
                break;
        }
    }


    protected void radExistingTable_CheckedChanged(object sender, EventArgs e)
    {
        if (radNewTable.Checked)
        {
            txtPKName.Text = "ItemID";

            txtTableName.Visible = true;
            drpExistingTables.Visible = false;

            chkItemGUID.Checked = true;
            chkItemOrder.Checked = true;
            chkItemCreatedBy.Checked = true;
            chkItemCreatedWhen.Checked = true;
            chkItemModifiedBy.Checked = true;
            chkItemModifiedWhen.Checked = true;
        }
        else
        {
            txtPKName.Text = ResHelper.GetString("General.Automatic");

            txtTableName.Visible = false;
            drpExistingTables.Visible = true;

            LoadAvailableTables();

            chkItemGUID.Checked = false;
            chkItemOrder.Checked = false;
            chkItemCreatedBy.Checked = false;
            chkItemCreatedWhen.Checked = false;
            chkItemModifiedBy.Checked = false;
            chkItemModifiedWhen.Checked = false;
        }
    }

    #endregion


    #region "Step processing"

    /// <summary>
    /// 'Next' button is clicked.
    /// </summary>
    protected void wzdNewDocType_NextButtonClick(object sender, WizardNavigationEventArgs e)
    {
        switch (e.CurrentStepIndex)
        {
            // Step 1   
            case 0:
                ProcessStep1(e);
                break;

            // Step 2
            case 1:
                ProcessStep2(e);
                break;

            // Step 3
            case 2:
                ProcessStep3(e);
                break;

            // Step 4
            case 3:
                ProcessStep4(e);
                break;

            // Step 5
            case 4:
                ProcessStep5(e);
                break;

            // Step 6
            case 5:
                ProcessStep6(e);
                break;
        }
    }


    /// <summary>
    /// Processes the step 1 of the wizard
    /// </summary>
    private void ProcessStep1(WizardNavigationEventArgs e)
    {
        // Actions after next button click

        // Validate checkboxes first
        string errorMessage = null;

        // Display proper error message based on development mode wizard setting
        switch (Mode)
        {
            case NewClassWizardModeEnum.DocumentType:
                errorMessage = new Validator().NotEmpty(txtDisplayName.Text.Trim(), GetString("DocumentType_New.ErrorEmptyDisplayName")).
                    NotEmpty(txtCodeName.Text.Trim(), GetString("DocumentType_New.ErrorEmptyCodeName")).
                    NotEmpty(txtNamespaceName.Text.Trim(), GetString("DocumentType_New.ErrorEmptyNamespaceName")).
                    IsCodeName(txtCodeName.Text.Trim(), GetString("DocumentType_New.CodeNameIdentifier")).
                    IsIdentifier(txtNamespaceName.Text.Trim(), GetString("DocumentType_New.NamespaceNameIdentifier")).Result;
                break;

            case NewClassWizardModeEnum.Class:
                errorMessage = new Validator().NotEmpty(txtDisplayName.Text.Trim(), GetString("sysdev.class_new.ErrorEmptyDisplayName")).
                    NotEmpty(txtCodeName.Text.Trim(), GetString("sysdev.class_new.ErrorEmptyCodeName")).
                    NotEmpty(txtNamespaceName.Text.Trim(), GetString("sysdev.class_new.ErrorEmptyNamespaceName")).
                    IsCodeName(txtCodeName.Text.Trim(), GetString("sysdev.class_new.CodeNameIdentifier")).
                    IsIdentifier(txtNamespaceName.Text.Trim(), GetString("sysdev.class_new.NamespaceNameIdentifier")).Result;
                break;

            case NewClassWizardModeEnum.CustomTable:
                errorMessage = new Validator().NotEmpty(txtDisplayName.Text.Trim(), GetString("customtable.newwizzard.ErrorEmptyDisplayName")).
                    NotEmpty(txtCodeName.Text.Trim(), GetString("customtable.newwizzard.ErrorEmptyCodeName")).
                    NotEmpty(txtNamespaceName.Text.Trim(), GetString("customtable.newwizzard.ErrorEmptyNamespaceName")).
                    IsCodeName(txtCodeName.Text.Trim(), GetString("customtable.newwizzard.CodeNameIdentifier")).
                    IsIdentifier(txtNamespaceName.Text.Trim(), GetString("customtable.newwizzard.NamespaceNameIdentifier")).Result;
                break;
        }


        if (String.IsNullOrEmpty(errorMessage))
        {
            string className = txtNamespaceName.Text.Trim() + "." + txtCodeName.Text.Trim();
            if (DataClassInfoProvider.GetDataClassInfo(className) != null)
            {
                errorMessage = GetString("sysdev.class_edit_gen.codenameunique");
            }
            else
            {
                // Set new class info
                DataClassInfo.ClassDisplayName = txtDisplayName.Text.Trim();
                DataClassInfo.ClassName = className;

                // Set class type according development mode setting
                switch (Mode)
                {
                    case NewClassWizardModeEnum.DocumentType:
                        DataClassInfo.ClassIsDocumentType = true;
                        DataClassInfo.ClassUsePublishFromTo = true;
                        break;

                    case NewClassWizardModeEnum.Class:
                        DataClassInfo.ClassShowAsSystemTable = false;
                        DataClassInfo.ClassShowTemplateSelection = false;
                        DataClassInfo.ClassIsDocumentType = false;
                        DataClassInfo.ClassCreateSKU = false;
                        DataClassInfo.ClassIsProduct = false;
                        DataClassInfo.ClassIsMenuItemType = false;
                        DataClassInfo.ClassUsesVersioning = false;
                        DataClassInfo.ClassUsePublishFromTo = false;
                        DataClassInfo.ClassResourceID = ModuleID;
                        break;

                    case NewClassWizardModeEnum.CustomTable:
                        DataClassInfo.ClassShowAsSystemTable = false;
                        DataClassInfo.ClassShowTemplateSelection = false;
                        DataClassInfo.ClassIsDocumentType = false;
                        DataClassInfo.ClassCreateSKU = false;
                        DataClassInfo.ClassIsProduct = false;
                        DataClassInfo.ClassIsMenuItemType = false;
                        DataClassInfo.ClassUsesVersioning = false;
                        DataClassInfo.ClassUsePublishFromTo = false;
                        // Sets custom table
                        DataClassInfo.ClassIsCustomTable = true;
                        break;
                }

                var errorMsg = String.Empty;

                try
                {
                    using (var tr = new CMSTransactionScope())
                    {
                        // Insert new class into DB
                        DataClassInfoProvider.SetDataClassInfo(DataClassInfo);

                        // Set permissions and queries
                        switch (Mode)
                        {
                            case NewClassWizardModeEnum.DocumentType:
                                // Ensure default permissions
                                PermissionNameInfoProvider.CreateDefaultClassPermissions(DataClassInfo.ClassID);
                                break;

                            case NewClassWizardModeEnum.Class:
                                break;

                            case NewClassWizardModeEnum.CustomTable:
                                // Ensure default custom table permissions
                                PermissionNameInfoProvider.CreateDefaultCustomTablePermissions(DataClassInfo.ClassID);
                                break;
                        }

                        tr.Commit();
                    }
                }
                catch (Exception ex)
                {
                    // No movement to the next step
                    e.Cancel = true;

                    // Class with the same class name already exists
                    pnlMessages1.Visible = true;

                    errorMsg = ex.Message;

                    pnlMessages1.ShowError(errorMsg);

                    EventLogProvider.LogException("NewClassWizard", "CREATE", ex);
                }

                // Prepare next step (2)       
                if (errorMsg == "")
                {
                    // Disable previous steps' view states
                    DisablePreviousStepsViewStates(e.CurrentStepIndex);

                    // Enable next step's view state
                    EnableNextStepViewState(e.CurrentStepIndex);

                    // Save ClassName to viewstate to use in the next steps
                    ClassName = DataClassInfo.ClassName;

                    // Prefill textboxes in the next step with default values
                    txtTableName.Text = txtNamespaceName.Text.Trim() + "_" + txtCodeName.Text.Trim();
                    txtPKName.Text = TextHelper.FirstLetterToUpper(txtCodeName.Text.Trim() + "ID");

                    wzdStep2.Title = GetString("DocumentType_New_Step2.Title");

                    // Prepare next step by mode setting
                    switch (Mode)
                    {
                        case NewClassWizardModeEnum.DocumentType:
                            {
                                // Document type
                                lblFullCodeName.ResourceString = "DocumentType_New.FullCodeName";
                                lblPKName.Text = GetString("DocumentType_New.PrimaryKeyName");
                                lblTableName.Text = GetString("DocumentType_New.TableName");
                                radCustom.Text = GetString("DocumentType_New.Custom");

                                // Display container option based on the development mode setting
                                radContainer.Text = GetString("DocumentType_New.Container");
                                radContainer.Visible = true;

                                ucHeader.Description = GetString("DocumentType_New_Step2.Description");

                                // Setup the inheritance selector
                                plcDocTypeOptions.Visible = true;

                                selInherits.WhereCondition = string.Format("ClassIsCoupledClass = 1 AND ClassID <> {0} AND (ClassInheritsFromClassID IS NULL OR ClassInheritsFromClassID <> {0})", DataClassInfo.ClassID);
                                selInherits.ReloadData();
                            }
                            break;

                        case NewClassWizardModeEnum.Class:
                            {
                                // Standard class
                                lblFullCodeName.ResourceString = "sysdev.class_new.fullcodename";
                                lblPKName.Text = GetString("sysdev.class_new.PrimaryKeyName");
                                lblTableName.Text = GetString("sysdev.class_new.TableName");
                                radCustom.Text = GetString("sysdev.class_new.Custom");
                                lblIsMNTable.Text = GetString("sysdev.class_new.MNTable");

                                radContainer.Visible = false;
                                plcMNClassOptions.Visible = true;

                                ucHeader.Description = GetString("sysdev.class_new_Step2.Description");
                            }
                            break;

                        case NewClassWizardModeEnum.CustomTable:
                            {
                                // Custom table
                                lblFullCodeName.ResourceString = "customtable.newwizzard.FullCodeName";
                                lblPKName.Text = GetString("customtable.newwizzard.PrimaryKeyName");
                                lblTableName.Text = GetString("customtable.newwizzard.TableName");

                                radCustom.Visible = false;
                                radContainer.Visible = false;

                                radNewTable.Text = GetString("customtable.newwizard.newtable");
                                radExistingTable.Text = GetString("customtable.newwizard.existingtable");

                                plcExisting.Visible = true;

                                // Custom tables have always ItemID as primary key
                                txtPKName.Text = "ItemID";

                                // Primary key name can't be edited
                                txtPKName.Enabled = false;

                                // Show custom tables columns options
                                plcCustomTablesOptions.Visible = true;

                                lblItemGUID.Text = GetString("customtable.newwizzard.lblItemGUID");
                                lblItemCreatedBy.Text = GetString("customtable.newwizzard.lblItemCreatedBy");
                                lblItemCreatedWhen.Text = GetString("customtable.newwizzard.lblItemCreatedWhen");
                                lblItemModifiedBy.Text = GetString("customtable.newwizzard.lblItemModifiedBy");
                                lblItemModifiedWhen.Text = GetString("customtable.newwizzard.lblItemModifiedWhen");
                                lblItemOrder.Text = GetString("customtable.newwizzard.lblItemOrder");

                                ucHeader.Description = GetString("customtable.newwizzard.Step2Description");
                            }
                            break;
                    }
                }
            }
        }

        if (!String.IsNullOrEmpty(errorMessage))
        {
            // No movement to the next step
            e.Cancel = true;

            // Textboxes are not filled correctly
            pnlMessages1.Visible = true;
            pnlMessages1.ShowError(errorMessage);
        }
    }


    /// <summary>
    /// Processes the step 2 of the wizard
    /// </summary>
    private void ProcessStep2(WizardNavigationEventArgs e)
    {
        var dci = DataClassInfoProvider.GetDataClassInfo(ClassName);

        if (dci != null)
        {
            var tm = new TableManager(null);

            using (var tr = new CMSTransactionScope())
            {
                // New document type has custom attributes -> no wizard steps will be omitted
                if (radCustom.Checked)
                {
                    // Actions after next button click
                    bool fromExisting = (Mode == NewClassWizardModeEnum.CustomTable) && radExistingTable.Checked;

                    string tableName = (fromExisting) ? drpExistingTables.SelectedValue : txtTableName.Text.Trim();

                    // Validate checkboxes first
                    string tableNameError = new Validator()
                        .NotEmpty(tableName, GetString("DocumentType_New.ErrorEmptyTableName"))
                        .IsIdentifier(tableName, GetString("class.ErrorIdentifier"))
                        .Result;

                    string primaryKeyNameEmpty = new Validator().NotEmpty(txtPKName.Text.Trim(), GetString("DocumentType_New.ErrorEmptyPKName")).Result;

                    bool columnExists = DocumentHelper.ColumnExistsInDocumentView(txtPKName.Text.Trim());

                    // Textboxes are filled correctly
                    if ((tableNameError == "") && (primaryKeyNameEmpty == "") && (!columnExists))
                    {
                        try
                        {
                            bool tableExists = tm.TableExists(tableName);
                            if (fromExisting)
                            {
                                // Custom table from existing table - validate the table name
                                if (!tableExists)
                                {
                                    e.Cancel = true;

                                    // Table with the same name already exists
                                    ShowError(GetString("customtable.newwizard.tablenotexists"));
                                }

                                // Check primary key
                                List<string> primaryKeys = tm.GetPrimaryKeyColumns(tableName);
                                if ((primaryKeys == null) || (primaryKeys.Count != 1))
                                {
                                    e.Cancel = true;

                                    ShowError(GetString("customtable.newwizard.musthaveprimarykey"));
                                }
                                else if (!IsIdentityColumn(tableName, primaryKeys.First()))
                                {
                                    e.Cancel = true;
                                    ShowError(GetString("customtable.newwizard.mustbeidentitypk"));
                                }
                            }
                            else if (tableExists)
                            {
                                // Check if given table name already exists in database
                                e.Cancel = true;
                                ShowError(GetString("sysdev.class_edit_gen.tablenameunique"));
                            }
                            else if (Mode == NewClassWizardModeEnum.Class)
                            {
                                // Standard class in development mode
                                tm.CreateTable(tableName, txtPKName.Text.Trim(), !chbIsMNTable.Checked);
                            }
                            else
                            {
                                tm.CreateTable(tableName, txtPKName.Text.Trim());
                            }
                        }
                        catch (Exception ex)
                        {
                            // No movement to the next step
                            e.Cancel = true;

                            // Show error message if something caused unhandled exception
                            ShowError(ex.Message);
                        }

                        if ((pnlMessages2.ErrorLabel.Text == "") && !e.Cancel)
                        {
                            // Change table owner                        
                            try
                            {
                                string owner = "";

                                // Get site related DB object owner setting when creating new wizard and global otherwise
                                switch (Mode)
                                {
                                    case NewClassWizardModeEnum.DocumentType:
                                    case NewClassWizardModeEnum.Class:
                                    case NewClassWizardModeEnum.CustomTable:
                                        owner = SqlHelper.GetDBSchema(SiteContext.CurrentSiteName);
                                        break;
                                }

                                if ((owner != "") && (owner.ToLowerCSafe() != "dbo"))
                                {
                                    tm.ChangeDBObjectOwner(tableName, owner);
                                    tableName = SqlHelper.GetSafeOwner(owner) + "." + tableName;
                                }
                            }
                            catch
                            {
                                // Suppress error
                            }

                            FormInfo fi;
                            if (fromExisting)
                            {
                                // From existing DB table
                                dci.ClassXmlSchema = tm.GetXmlSchema(tableName);

                                string formDef = FormHelper.GetXmlFormDefinitionFromXmlSchema(dci.ClassXmlSchema, false);
                                fi = new FormInfo(formDef);
                            }
                            else
                            {
                                // Create empty form info
                                fi = CreateEmptyFormInfo();

                                dci.ClassXmlSchema = tm.GetXmlSchema(tableName);
                            }

                            dci.ClassTableName = tableName;
                            dci.ClassFormDefinition = fi.GetXmlDefinition();
                            dci.ClassIsCoupledClass = true;

                            dci.ClassInheritsFromClassID = ValidationHelper.GetInteger(selInherits.Value, 0);

                            // Update class in DB
                            using (var context = new CMSActionContext())
                            {
                                // Disable logging into event log
                                context.LogEvents = false;

                                DataClassInfoProvider.SetDataClassInfo(dci);

                                UpdateInheritedClass(dci);
                            }

                            if (Mode == NewClassWizardModeEnum.CustomTable)
                            {
                                try
                                {
                                    InitCustomTable(dci, fi, tm);
                                }
                                catch (Exception ex)
                                {
                                    // Do not move to next step.
                                    e.Cancel = true;

                                    EventLogProvider.LogException("NewClassWizard", "CREATE", ex);

                                    string message = null;
                                    if (ex is MissingSQLTypeException)
                                    {
                                        var missingSqlType = (MissingSQLTypeException)ex;
                                        if (DataTypeManager.IsType<byte[]>(TypeEnum.SQL, missingSqlType.RecommendedType))
                                        {
                                            message = String.Format(GetString("customtable.sqltypenotsupportedwithoutreplacement"), missingSqlType.UnsupportedType, missingSqlType.ColumnName);
                                        }
                                        else
                                        {
                                            message = String.Format(GetString("customtable.sqltypenotsupported"), missingSqlType.UnsupportedType, missingSqlType.ColumnName, missingSqlType.RecommendedType);
                                        }
                                    }
                                    else
                                    {
                                        message = ex.Message;
                                    }

                                    pnlMessages2.ShowError(message);
                                    pnlMessages2.Visible = true;
                                }
                            }

                            if (!e.Cancel)
                            {
                                // Remember that no steps were omitted
                                SomeStepsOmitted = false;

                                // Prepare next step (3)

                                // Disable previous steps' viewstates
                                DisablePreviousStepsViewStates(e.CurrentStepIndex);

                                // Enable next step's viewstate
                                EnableNextStepViewState(e.CurrentStepIndex);

                                // Set field editor class name
                                FieldEditor.ClassName = ClassName;

                                // Fill field editor in the next step
                                FieldEditor.Reload(null);

                                wzdStep3.Title = GetString("general.fields");

                                // Set new step header based on the development mode setting
                                switch (Mode)
                                {
                                    case NewClassWizardModeEnum.DocumentType:
                                        ucHeader.Description = GetString("DocumentType_New_Step3.Description");
                                        break;

                                    case NewClassWizardModeEnum.Class:
                                        ucHeader.Description = GetString("sysdev.class_new_Step3.Description");
                                        break;

                                    case NewClassWizardModeEnum.CustomTable:
                                        ucHeader.Description = GetString("customtable.newwizzard.Step3Description");
                                        break;
                                }
                            }
                        }
                    }
                    // Some textboxes are not filled correctly
                    else
                    {
                        // Prepare current step (2)

                        // No movement to the next step
                        e.Cancel = true;

                        // Show errors
                        if (!String.IsNullOrEmpty(tableNameError))
                        {
                            lblTableNameError.Text = tableNameError;
                            lblTableNameError.Visible = true;
                        }
                        else
                        {
                            lblTableNameError.Visible = false;
                        }

                        if (!String.IsNullOrEmpty(primaryKeyNameEmpty))
                        {
                            lblPKNameError.Visible = true;
                            lblPKNameError.Text = primaryKeyNameEmpty;
                        }
                        else
                        {
                            lblPKNameError.Visible = false;
                        }

                        if (columnExists)
                        {
                            pnlMessages2.ShowError(GetString("DocumentType_New_Step2.ErrorColumnExists"));
                            pnlMessages2.Visible = true;
                        }

                        wzdStep2.Title = GetString("DocumentType_New_Step2.Title");

                        // Reset the header
                        switch (Mode)
                        {
                            case NewClassWizardModeEnum.DocumentType:
                                ucHeader.Description = GetString("DocumentType_New_Step2.Description");
                                break;

                            case NewClassWizardModeEnum.Class:
                                ucHeader.Description = GetString("sysdev.class_new_Step2.Description");
                                break;

                            case NewClassWizardModeEnum.CustomTable:
                                ucHeader.Description = GetString("customtable.newwizzard.Step2Description");
                                break;
                        }
                    }
                }
                // New document type is only the container -> some wizard steps will be omitted
                else
                {
                    // Actions after next button click

                    dci.ClassIsCoupledClass = false;

                    // Update class in DB
                    using (CMSActionContext context = new CMSActionContext())
                    {
                        // Disable logging into event log
                        context.LogEvents = false;

                        DataClassInfoProvider.SetDataClassInfo(dci);
                    }

                    // Remember that some steps were omitted
                    SomeStepsOmitted = true;
                    IsContainer = true;


                    // Prepare next step (5) - skip steps 3 and 4

                    // Disable previous steps' viewstates
                    DisablePreviousStepsViewStates(3);

                    // Enable next step's viewstate
                    EnableNextStepViewState(3);

                    PrepareStep5();
                    // Go to the step 5 (indexed from 0)  
                    wzdNewDocType.ActiveStepIndex = 4;
                }

                // Create new icon if the wizard is used to create new document type
                if (Mode == NewClassWizardModeEnum.DocumentType)
                {
                    // Setup icon class for new doc. type
                    string iconClass = (SomeStepsOmitted) ? DEFAULT_CLASS_ICON : DEFAULT_COUPLED_CLASS_ICON;
                    dci.SetValue("ClassIconClass", iconClass);
                }

                if (!e.Cancel)
                {
                    tr.Commit();
                }
            }
        }
    }


    /// <summary>
    /// Updates the inherited class fields if the class is inherited
    /// </summary>
    /// <param name="dci">DataClassInfo to update</param>
    private static void UpdateInheritedClass(DataClassInfo dci)
    {
        // Ensure inherited fields
        if (dci.ClassInheritsFromClassID > 0)
        {
            var parentCi = DataClassInfoProvider.GetDataClassInfo(dci.ClassInheritsFromClassID);
            if (parentCi != null)
            {
                FormHelper.UpdateInheritedClass(parentCi, dci);
            }
        }
    }


    /// <summary>
    /// Initializes the custom table
    /// </summary>
    /// <param name="dci">DataClassInfo of the custom table</param>
    /// <param name="fi">Form info</param>
    /// <param name="tm">Table manager</param>
    private void InitCustomTable(DataClassInfo dci, FormInfo fi, TableManager tm)
    {
        // Created by
        if (chkItemCreatedBy.Checked && !fi.FieldExists("ItemCreatedBy"))
        {
            FormFieldInfo ffi = new FormFieldInfo();

            // Fill FormInfo object
            ffi.Name = "ItemCreatedBy";
            ffi.SetPropertyValue(FormFieldPropertyEnum.FieldCaption, "Created by");
            ffi.DataType = FieldDataType.Integer;
            ffi.SetPropertyValue(FormFieldPropertyEnum.DefaultValue, string.Empty);
            ffi.SetPropertyValue(FormFieldPropertyEnum.FieldDescription, string.Empty);
            ffi.FieldType = FormFieldControlTypeEnum.CustomUserControl;
            ffi.Settings["controlname"] = Enum.GetName(typeof(FormFieldControlTypeEnum), FormFieldControlTypeEnum.LabelControl).ToLowerCSafe();
            ffi.PrimaryKey = false;
            ffi.System = true;
            ffi.Visible = false;
            ffi.Size = 0;
            ffi.AllowEmpty = true;

            fi.AddFormItem(ffi);
        }

        // Created when
        if (chkItemCreatedWhen.Checked && !fi.FieldExists("ItemCreatedWhen"))
        {
            FormFieldInfo ffi = new FormFieldInfo();

            // Fill FormInfo object
            ffi.Name = "ItemCreatedWhen";
            ffi.SetPropertyValue(FormFieldPropertyEnum.FieldCaption, "Created when");
            ffi.DataType = FieldDataType.DateTime;
            ffi.SetPropertyValue(FormFieldPropertyEnum.DefaultValue, string.Empty);
            ffi.SetPropertyValue(FormFieldPropertyEnum.FieldDescription, string.Empty);
            ffi.FieldType = FormFieldControlTypeEnum.CustomUserControl;
            ffi.Settings["controlname"] = Enum.GetName(typeof(FormFieldControlTypeEnum), FormFieldControlTypeEnum.LabelControl).ToLowerCSafe();
            ffi.PrimaryKey = false;
            ffi.System = true;
            ffi.Visible = false;
            ffi.Size = 0;
            ffi.AllowEmpty = true;

            fi.AddFormItem(ffi);
        }

        // Modified by
        if (chkItemModifiedBy.Checked && !fi.FieldExists("ItemModifiedBy"))
        {
            FormFieldInfo ffi = new FormFieldInfo();

            // Fill FormInfo object
            ffi.Name = "ItemModifiedBy";
            ffi.SetPropertyValue(FormFieldPropertyEnum.FieldCaption, "Modified by");
            ffi.DataType = FieldDataType.Integer;
            ffi.SetPropertyValue(FormFieldPropertyEnum.DefaultValue, string.Empty);
            ffi.SetPropertyValue(FormFieldPropertyEnum.FieldDescription, string.Empty);
            ffi.FieldType = FormFieldControlTypeEnum.CustomUserControl;
            ffi.Settings["controlname"] = Enum.GetName(typeof(FormFieldControlTypeEnum), FormFieldControlTypeEnum.LabelControl).ToLowerCSafe();
            ffi.PrimaryKey = false;
            ffi.System = true;
            ffi.Visible = false;
            ffi.Size = 0;
            ffi.AllowEmpty = true;

            fi.AddFormItem(ffi);
        }

        // Modified when
        if (chkItemModifiedWhen.Checked && !fi.FieldExists("ItemModifiedWhen"))
        {
            FormFieldInfo ffi = new FormFieldInfo();

            // Fill FormInfo object
            ffi.Name = "ItemModifiedWhen";
            ffi.SetPropertyValue(FormFieldPropertyEnum.FieldCaption, "Modified when");
            ffi.DataType = FieldDataType.DateTime;
            ffi.SetPropertyValue(FormFieldPropertyEnum.DefaultValue, string.Empty);
            ffi.SetPropertyValue(FormFieldPropertyEnum.FieldDescription, string.Empty);
            ffi.FieldType = FormFieldControlTypeEnum.CustomUserControl;
            ffi.Settings["controlname"] = Enum.GetName(typeof(FormFieldControlTypeEnum), FormFieldControlTypeEnum.LabelControl).ToLowerCSafe();
            ffi.PrimaryKey = false;
            ffi.System = true;
            ffi.Visible = false;
            ffi.Size = 0;
            ffi.AllowEmpty = true;

            fi.AddFormItem(ffi);
        }

        // Item order
        if (chkItemOrder.Checked && !fi.FieldExists("ItemOrder"))
        {
            FormFieldInfo ffi = new FormFieldInfo();

            // Fill FormInfo object
            ffi.Name = "ItemOrder";
            ffi.SetPropertyValue(FormFieldPropertyEnum.FieldCaption, "Order");
            ffi.DataType = FieldDataType.Integer;
            ffi.SetPropertyValue(FormFieldPropertyEnum.DefaultValue, string.Empty);
            ffi.SetPropertyValue(FormFieldPropertyEnum.FieldDescription, string.Empty);
            ffi.FieldType = FormFieldControlTypeEnum.CustomUserControl;
            ffi.Settings["controlname"] = Enum.GetName(typeof(FormFieldControlTypeEnum), FormFieldControlTypeEnum.LabelControl).ToLowerCSafe();
            ffi.PrimaryKey = false;
            ffi.System = true;
            ffi.Visible = false;
            ffi.Size = 0;
            ffi.AllowEmpty = true;

            fi.AddFormItem(ffi);
        }

        // GUID
        if (chkItemGUID.Checked && !fi.FieldExists("ItemGUID"))
        {
            var ffiGuid = CreateGuidField();

            fi.AddFormItem(ffiGuid);
        }

        // Update table structure - columns could be added
        bool old = TableManager.UpdateSystemFields;

        TableManager.UpdateSystemFields = true;

        string schema = fi.GetXmlDefinition();
        tm.UpdateTableByDefinition(dci.ClassTableName, schema);

        TableManager.UpdateSystemFields = old;

        // Update xml schema and form definition
        dci.ClassFormDefinition = schema;
        dci.ClassXmlSchema = tm.GetXmlSchema(dci.ClassTableName);

        using (CMSActionContext context = new CMSActionContext())
        {
            // Disable logging into event log
            context.LogEvents = false;

            DataClassInfoProvider.SetDataClassInfo(dci);
        }
    }


    /// <summary>
    /// Creates the GUID field
    /// </summary>
    private static FormFieldInfo CreateGuidField()
    {
        FormFieldInfo ffiGuid = new FormFieldInfo();

        // Fill FormInfo object
        ffiGuid.Name = "ItemGUID";
        ffiGuid.SetPropertyValue(FormFieldPropertyEnum.FieldCaption, "GUID");
        ffiGuid.DataType = FieldDataType.Guid;
        ffiGuid.SetPropertyValue(FormFieldPropertyEnum.DefaultValue, string.Empty);
        ffiGuid.SetPropertyValue(FormFieldPropertyEnum.FieldDescription, String.Empty);
        ffiGuid.FieldType = FormFieldControlTypeEnum.CustomUserControl;
        ffiGuid.Settings["controlname"] = Enum.GetName(typeof(FormFieldControlTypeEnum), FormFieldControlTypeEnum.LabelControl).ToLowerCSafe();
        ffiGuid.PrimaryKey = false;
        ffiGuid.System = true;
        ffiGuid.Visible = false;
        ffiGuid.Size = 0;
        ffiGuid.AllowEmpty = false;

        return ffiGuid;
    }


    /// <summary>
    /// Creates an empty form info for the new class
    /// </summary>
    private FormInfo CreateEmptyFormInfo()
    {
        // Create empty form definition
        var fi = new FormInfo();

        var ffiPK = new FormFieldInfo();

        // Fill FormInfo object
        ffiPK.Name = txtPKName.Text;
        ffiPK.SetPropertyValue(FormFieldPropertyEnum.FieldCaption, txtPKName.Text);
        ffiPK.DataType = FieldDataType.Integer;
        ffiPK.SetPropertyValue(FormFieldPropertyEnum.DefaultValue, string.Empty);
        ffiPK.SetPropertyValue(FormFieldPropertyEnum.FieldDescription, string.Empty);
        ffiPK.FieldType = FormFieldControlTypeEnum.CustomUserControl;
        ffiPK.Settings["controlname"] = Enum.GetName(typeof(FormFieldControlTypeEnum), FormFieldControlTypeEnum.LabelControl).ToLowerCSafe();
        ffiPK.PrimaryKey = true;
        ffiPK.System = false;
        ffiPK.Visible = false;
        ffiPK.Size = 0;
        ffiPK.AllowEmpty = false;

        // Add field to form definition
        fi.AddFormItem(ffiPK);

        return fi;
    }


    /// <summary>
    /// Processes the step 3 of the wizard
    /// </summary>
    private void ProcessStep3(WizardNavigationEventArgs e)
    {
        // Actions after next button click
        var dci = DataClassInfoProvider.GetDataClassInfo(ClassName);

        // Ensure actual form info
        FormHelper.ClearFormInfos(true);

        // Get and load form definition
        var fi = FormHelper.GetFormInfo(dci.ClassName, false);

        if (fi.GetFields(true, true, includeDummyFields: false).Count() < 2)
        {
            e.Cancel = true;
            FieldEditor.ShowError(GetString("DocumentType_New_Step3.TableMustHaveCustomField"));
        }
        else
        {
            // Different behavior by mode
            switch (Mode)
            {
                case NewClassWizardModeEnum.DocumentType:
                    {
                        TableManager tm = new TableManager(null);

                        // Create new view if doesn't exist
                        string viewName = SqlHelper.GetViewName(dci.ClassTableName, null);

                        // Create view for document types
                        if (!tm.ViewExists(viewName))
                        {
                            tm.CreateView(viewName, SqlGenerator.GetSqlQuery(ClassName, SqlOperationTypeEnum.SelectView, null));
                        }

                        // If new document type is created prepare next step otherwise skip steps 4, 5 and 6

                        // Disable previous steps' viewstates
                        DisablePreviousStepsViewStates(e.CurrentStepIndex);

                        // Enable next step's viewstate
                        EnableNextStepViewState(e.CurrentStepIndex);

                        // Add implicit value to the list
                        lstFields.Items.Add(new ListItem(GetString("DocumentType_New_Step4.ImplicitDocumentName"), ""));

                        // Get all fields
                        List<FormFieldInfo> ffiFields = fi.GetFields(true, true);

                        if (ffiFields != null)
                        {
                            bool selected = false;

                            // Add all text fields' names to the list except primary-key field
                            foreach (FormFieldInfo ffi in ffiFields)
                            {
                                if (!ffi.PrimaryKey && !ffi.AllowEmpty && ((ffi.DataType == FieldDataType.Text) || (ffi.DataType == FieldDataType.LongText)))
                                {
                                    lstFields.Items.Add(new ListItem(ffi.Name, ffi.Name));

                                    // Select the first text field
                                    if (!selected)
                                    {
                                        string controlName = ValidationHelper.GetString(ffi.Settings["controlname"], null);

                                        // Preselect only textbox
                                        if (CMSString.Compare(controlName, Enum.GetName(typeof(FormFieldControlTypeEnum), FormFieldControlTypeEnum.TextBoxControl), StringComparison.InvariantCultureIgnoreCase) == 0)
                                        {
                                            lstFields.SelectedValue = ffi.Name;
                                            selected = true;
                                        }
                                    }
                                }
                            }
                        }

                        lblSelectField.Text = GetString("DocumentType_New_Step4.DocumentName");
                        wzdStep4.Title = GetString("DocumentType_New_Step4.Title");
                        ucHeader.Description = GetString("DocumentType_New_Step4.Description");
                    }
                    break;

                case NewClassWizardModeEnum.Class:
                    {
                        // Update class in DB
                        using (CMSActionContext context = new CMSActionContext())
                        {
                            // Disable logging into event log
                            context.LogEvents = false;

                            DataClassInfoProvider.SetDataClassInfo(dci);
                        }

                        // Remember that some steps were omitted
                        SomeStepsOmitted = true;

                        // Prepare next step (7) - skip steps 4, 5 and 6

                        // Disable previous steps' viewstates
                        DisablePreviousStepsViewStates(5);

                        // Enable next step's viewstate
                        EnableNextStepViewState(5);

                        PrepareStep7();
                        // Go to the step 7 (indexed from 0)  
                        wzdNewDocType.ActiveStepIndex = 6;
                    }
                    break;

                case NewClassWizardModeEnum.CustomTable:
                    {
                        // Update class in DB
                        using (CMSActionContext context = new CMSActionContext())
                        {
                            // Disable logging into event log
                            context.LogEvents = false;

                            DataClassInfoProvider.SetDataClassInfo(dci);
                        }

                        // Remember that some steps were omitted, 
                        SomeStepsOmitted = true;

                        // Prepare next step (6) - skip steps 4, 5 

                        // Disable previous steps' viewstates
                        DisablePreviousStepsViewStates(4);

                        // Enable next step's viewstate
                        EnableNextStepViewState(4);

                        PrepareStep6();
                        // Go to the step 6 (indexed from 0) 
                        wzdNewDocType.ActiveStepIndex = 5;
                    }
                    break;
            }
        }
    }


    /// <summary>
    /// Processes the step 4 of the wizard
    /// </summary>
    private void ProcessStep4(WizardNavigationEventArgs e)
    {
        // Actions after next button click
        var dci = DataClassInfoProvider.GetDataClassInfo(ClassName);

        dci.ClassNodeNameSource = lstFields.SelectedValue;

        // Update node name source in DB
        using (CMSActionContext context = new CMSActionContext())
        {
            // Disable logging into event log
            context.LogEvents = false;

            DataClassInfoProvider.SetDataClassInfo(dci);
        }

        // Prepare next step (5)

        // Disable previous steps' viewstates
        DisablePreviousStepsViewStates(e.CurrentStepIndex);

        // Enable next step's viewstate
        EnableNextStepViewState(e.CurrentStepIndex);

        PrepareStep5();
    }


    /// <summary>
    /// Processes the step 5 of the wizard
    /// </summary>
    private void ProcessStep5(WizardNavigationEventArgs e)
    {
        // Actions after next button click

        int childClassID = DataClassInfoProvider.GetDataClassInfo(ClassName).ClassID;

        // Add parent classes
        string selectedClasses = ValidationHelper.GetString(usParentTypes.Value, String.Empty);
        if (!String.IsNullOrEmpty(selectedClasses))
        {
            string[] classes = selectedClasses.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            // Add all new items to site
            foreach (string item in classes)
            {
                int parentClassID = ValidationHelper.GetInteger(item, 0);
                AllowedChildClassInfoProvider.AddAllowedChildClass(parentClassID, childClassID);
            }
        }

        // Prepare next step (6)

        // Disable previous steps' viewstates
        DisablePreviousStepsViewStates(e.CurrentStepIndex);

        // Enable next step's viewstate
        EnableNextStepViewState(e.CurrentStepIndex);

        PrepareStep6();
    }


    /// <summary>
    /// Processes the step 5 of the wizard
    /// </summary>
    private void ProcessStep6(WizardNavigationEventArgs e)
    {
        // Actions after next button click
        var dci = DataClassInfoProvider.GetDataClassInfo(ClassName);

        int classId = dci.ClassID;
        bool isCustomTable = dci.ClassIsCustomTable;
        bool licenseCheck = true;

        string selectedSite = ValidationHelper.GetString(usSites.Value, String.Empty);
        if (selectedSite == "0")
        {
            selectedSite = "";
        }
        string[] sites = selectedSite.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (string site in sites)
        {
            int siteId = ValidationHelper.GetInteger(site, 0);

            SiteInfo si = SiteInfoProvider.GetSiteInfo(siteId);
            if (si != null)
            {
                if (isCustomTable)
                {
                    if (!CustomTableItemProvider.LicenseVersionCheck(si.DomainName, ObjectActionEnum.Insert))
                    {
                        pnlMessages6.Visible = true;
                        pnlMessages6.ShowError(GetString("LicenseVersion.CustomTables"));
                        e.Cancel = true;
                        licenseCheck = false;
                    }
                }

                if (licenseCheck)
                {
                    ClassSiteInfoProvider.AddClassToSite(classId, siteId);

                    // Clear custom tables count
                    if (isCustomTable)
                    {
                        CustomTableItemProvider.ClearLicensesCount();
                    }
                }
            }
        }

        // If is moving to the new step
        if (licenseCheck)
        {
            // Create default transformations
            switch (Mode)
            {
                case NewClassWizardModeEnum.DocumentType:

                    if (!SomeStepsOmitted)
                    {
                        CreateDefaultTransformations(classId, true);
                    }
                    else
                    {
                        // Apply on if new document type was created
                        lblTableCreated.Visible = false;
                    }
                    break;

                case NewClassWizardModeEnum.CustomTable:
                    CreateDefaultTransformations(classId, false);
                    break;
            }
        }
        else
        {
            PrepareStep5();
            return;
        }

        // Save default search settings
        if (((Mode == NewClassWizardModeEnum.DocumentType) && (dci.ClassIsCoupledClass)) || (Mode == NewClassWizardModeEnum.CustomTable))
        {
            dci.ClassSearchEnabled = true;
            dci.ClassSearchSettings = SearchHelper.GetDefaultSearchSettings(dci);
            SearchHelper.SetDefaultClassSearchColumns(dci);
            dci.Generalized.SetObject();
        }

        DisablePreviousStepsViewStates(6);
        EnableNextStepViewState(6);
        PrepareStep7();

        // Explicitly log the synchronization with all changes
        using (CMSActionContext context = new CMSActionContext())
        {
            // Disable logging into event log
            context.LogEvents = false;

            SynchronizationHelper.LogObjectUpdate(dci);
        }

    }


    /// <summary>
    /// Finish button is clicked.
    /// </summary>
    protected void wzdNewDocType_FinishButtonClick(object sender, EventArgs e)
    {
        string editUrl = "";
        int newObjId = DataClassInfoProvider.GetDataClassInfo(ClassName).ClassID;

        // Redirect to the document type edit site
        switch (Mode)
        {
            case NewClassWizardModeEnum.DocumentType:
                editUrl = GetEditUrl("CMS.DocumentEngine", "EditDocumentType", newObjId);
                break;

            case NewClassWizardModeEnum.Class:
                editUrl = URLHelper.AppendQuery(GetEditUrl("CMS", "EditClass", newObjId, ModuleID), "&moduleid=" + ModuleID);
                break;

            case NewClassWizardModeEnum.CustomTable:
                editUrl = GetEditUrl("CMS.CustomTables", "EditCustomTable", newObjId);
                break;
        }

        URLHelper.Redirect(editUrl);
    }

    #endregion


    #region "Step preparing methods"

    /// <summary>
    /// Prepares and fills controls in the step 5.
    /// </summary>
    private void PrepareStep5()
    {
        wzdStep5.Title = GetString("DocumentType_New_Step5.Title");
        ucHeader.Description = GetString("DocumentType_New_Step5.Description");

        if (Mode == NewClassWizardModeEnum.DocumentType)
        {
            // Preselect Menu items document types
            DataSet ds = DataClassInfoProvider.GetClasses()
                .WhereTrue("ClassIsDocumentType")
                .WhereTrue("ClassIsMenuItemType")
                .Column("ClassID");

            usParentTypes.Value = TextHelper.Join(";", DataHelper.GetStringValues(ds.Tables[0], "ClassID"));
            usParentTypes.Reload(true);
        }
    }


    /// <summary>
    /// Prepares and fills controls in the step 6.
    /// </summary>
    private void PrepareStep6()
    {
        wzdStep6.Title = GetString("DocumentType_New_Step6.Title");
        ucHeader.Description = GetString(Step6Description);

        // Preselect current site
        usSites.Value = SiteContext.CurrentSiteID;

        // Reload to have preselect site visible
        usSites.Reload(true);
    }


    /// <summary>
    /// Prepares and fills controls in the step 8.
    /// </summary>
    private void PrepareStep7()
    {
        ucHeader.DescriptionVisible = false;
        wzdStep7.Title = GetString("documenttype_new_step8.title");

        // Display final messages based on mode
        switch (Mode)
        {
            case NewClassWizardModeEnum.DocumentType:
                lblEditingFormCreated.Text = GetString("documenttype_new_step8_finished.editingformcreated");
                lblQueryCreated.Text = GetString("documenttype_new_step8_finished.querycreated");
                lblDocumentCreated.Text = GetString("documenttype_new_step8_finished.documentcreated");
                lblChildTypesAdded.Text = GetString("documenttype_new_step8_finished.parenttypesadded");
                lblSitesSelected.Text = GetString("documenttype_new_step8_finished.sitesselected");
                lblTransformationCreated.Text = GetString("documenttype_new_step8_finished.transformationcreated");
                lblPermissionNameCreated.Text = GetString("documenttype_new_step8_finished.permissionnamecreated");
                lblDefaultIconCreated.Text = GetString("documenttype_new_step8_finished.defaulticoncreated");
                lblSearchSpecificationCreated.Text = GetString("documenttype_new_step8_finished.searchspecificationcreated");

                // Hide some messages if creating container document type
                lblTransformationCreated.Visible = !IsContainer;
                lblSearchSpecificationCreated.Visible = !IsContainer;
                break;

            case NewClassWizardModeEnum.Class:
                lblQueryCreated.Text = GetString("documenttype_new_step8_finished.querycreated");
                lblDocumentCreated.Text = GetString("sysdev.class_new_step8_finished.documentcreated");
                lblTableCreated.Text = GetString("sysdev.class_new_step8_finished.tablecreated");
                break;

            case NewClassWizardModeEnum.CustomTable:
                lblDocumentCreated.Text = GetString("customtable.newwizzard.CustomTableCreated");
                lblSitesSelected.Text = GetString("customtable.newwizzard.SitesSelected");
                lblTransformationCreated.Text = GetString("customtable.newwizzard.TransformationCreated");
                lblQueryCreated.Text = GetString("customtable.newwizzard.QueryCreated");
                lblPermissionNameCreated.Text = GetString("customtable.newwizzard.PermissionNameCreated");
                lblSearchSpecificationCreated.Text = GetString("customtable.newwizzard.searchspecificationcreated");
                break;
        }
    }

    #endregion


    #region "Helper methods"

    /// <summary>
    /// Creates URL for editing.
    /// </summary>
    /// <param name="resourceName">Resource name</param>
    /// <param name="elementName">Element name</param>
    /// <param name="newID">ID of current created table</param>
    /// <param name="parentID">ID of parent object type</param>
    /// <param name="displayTitle">Indicates if 'displaytitle=false' should be part of the URL</param>
    private String GetEditUrl(string resourceName, string elementName, int newID, int parentID = 0, bool displayTitle = false)
    {
        UIElementInfo uiChild = UIElementInfoProvider.GetUIElementInfo(resourceName, elementName);
        if (uiChild != null)
        {
            return URLHelper.AppendQuery(UIContextHelper.GetElementUrl(uiChild, UIContext), "objectid=" + newID
                + ((parentID > 0) ? "&parentobjectid=" + parentID : String.Empty)) + (!displayTitle ? "&displaytitle=false" : String.Empty);
        }

        return String.Empty;
    }


    /// <summary>
    /// Creates default transformations for the classid.
    /// </summary>
    /// <param name="classId">Class id</param>
    /// <param name="isDocument">Indicates if transformations for document should be created</param>
    private void CreateDefaultTransformations(int classId, bool isDocument)
    {
        using (CMSActionContext context = new CMSActionContext())
        {
            // Disable logging of tasks
            context.DisableLogging();

            // Create default transformation
            TransformationInfo ti = new TransformationInfo();

            string classFormDefinition = DataClassInfoProvider.GetDataClassInfo(ClassName).ClassFormDefinition;

            ti.TransformationName = ValidationHelper.GetCodeName(GetString("TransformationName.Default"));
            ti.TransformationFullName = ClassName + "." + ti.TransformationName;
            ti.TransformationCode = TransformationInfoProvider.GenerateTransformationCode(classFormDefinition, TransformationTypeEnum.Ascx, ClassName);
            ti.TransformationType = TransformationTypeEnum.Ascx;
            ti.TransformationClassID = classId;

            // Set default transformation in DB
            TransformationInfoProvider.SetTransformation(ti);

            // Create preview transformation which has the same transformation code
            ti = ti.Clone(true);

            ti.TransformationName = ValidationHelper.GetCodeName(GetString("TransformationName.Preview"));
            ti.TransformationFullName = ClassName + "." + ti.TransformationName;

            // Set default transformation in DB
            TransformationInfoProvider.SetTransformation(ti);

            // Test if class is standard document type
            if (isDocument)
            {
                // Create RSS transformation
                ti = ti.Clone(true);

                ti.TransformationName = ValidationHelper.GetCodeName(GetString("TransformationName.RSS"));
                ti.TransformationFullName = ClassName + "." + ti.TransformationName;
                ti.TransformationCode = TransformationInfoProvider.GenerateTransformationCode(classFormDefinition, TransformationTypeEnum.Ascx, ClassName, DefaultTransformationTypeEnum.RSS);

                // Set RSS transformation in DB
                TransformationInfoProvider.SetTransformation(ti);

                // Create Atom transformation
                ti = ti.Clone(true);

                ti.TransformationName = ValidationHelper.GetCodeName(GetString("TransformationName.Atom"));
                ti.TransformationFullName = ClassName + "." + ti.TransformationName;
                ti.TransformationCode = TransformationInfoProvider.GenerateTransformationCode(classFormDefinition, TransformationTypeEnum.Ascx, ClassName, DefaultTransformationTypeEnum.Atom);

                // Set Atom transformation in DB
                TransformationInfoProvider.SetTransformation(ti);
            }
        }
    }


    /// <summary>
    /// Disable Viewstates of the current and previous steps.
    /// </summary>
    private void DisablePreviousStepsViewStates(int currentStep)
    {
        switch (currentStep)
        {
            // Step 1
            case 0:
                wzdStep1.EnableViewState = false;
                break;

            // Step 2
            case 1:
                wzdStep1.EnableViewState = false;
                wzdStep2.EnableViewState = false;
                break;

            // Step 3
            case 2:
                wzdStep1.EnableViewState = false;
                wzdStep2.EnableViewState = false;
                wzdStep3.EnableViewState = false;
                break;

            // Step 4
            case 3:
                wzdStep1.EnableViewState = false;
                wzdStep2.EnableViewState = false;
                wzdStep3.EnableViewState = false;
                wzdStep4.EnableViewState = false;
                break;

            // Step 5
            case 4:
                wzdStep1.EnableViewState = false;
                wzdStep2.EnableViewState = false;
                wzdStep3.EnableViewState = false;
                wzdStep4.EnableViewState = false;
                wzdStep5.EnableViewState = false;
                break;

            // Step 6
            case 5:
                wzdStep1.EnableViewState = false;
                wzdStep2.EnableViewState = false;
                wzdStep3.EnableViewState = false;
                wzdStep4.EnableViewState = false;
                wzdStep5.EnableViewState = false;
                wzdStep6.EnableViewState = false;
                break;

            // Step 7
            case 6:
                wzdStep1.EnableViewState = false;
                wzdStep2.EnableViewState = false;
                wzdStep3.EnableViewState = false;
                wzdStep4.EnableViewState = false;
                wzdStep5.EnableViewState = false;
                wzdStep6.EnableViewState = false;
                wzdStep7.EnableViewState = false;
                break;

            // Step 8
            case 7:
                wzdStep1.EnableViewState = false;
                wzdStep2.EnableViewState = false;
                wzdStep3.EnableViewState = false;
                wzdStep4.EnableViewState = false;
                wzdStep5.EnableViewState = false;
                wzdStep6.EnableViewState = false;
                wzdStep7.EnableViewState = false;
                break;
        }
    }


    /// <summary>
    /// Enable Viewstate of the next step.
    /// </summary>
    private void EnableNextStepViewState(int actualStep)
    {
        switch (actualStep)
        {
            case 0:
                wzdStep2.EnableViewState = true;
                break;

            case 1:
                wzdStep3.EnableViewState = true;
                break;

            case 2:
                wzdStep4.EnableViewState = true;
                break;

            case 3:
                wzdStep5.EnableViewState = true;
                break;

            case 4:
                wzdStep6.EnableViewState = true;
                break;

            case 5:
                wzdStep7.EnableViewState = true;
                break;
        }
    }


    /// <summary>
    /// Returns title of the current step.
    /// </summary>
    /// <param name="currentStep">Current step index (counted from 0)</param>
    private string GetCurrentStepTitle(int currentStep)
    {
        string currentStepTitle = "";

        switch (currentStep)
        {
            case 0:
                currentStepTitle = wzdStep1.Title;
                break;

            case 1:
                currentStepTitle = wzdStep2.Title;
                break;

            case 2:
                currentStepTitle = wzdStep3.Title;
                break;

            case 3:
                currentStepTitle = wzdStep4.Title;
                break;

            case 4:
                currentStepTitle = wzdStep5.Title;
                break;

            case 5:
                currentStepTitle = wzdStep6.Title;
                break;

            case 6:
                currentStepTitle = wzdStep7.Title;
                break;
        }

        return currentStepTitle;
    }


    protected void LoadAvailableTables()
    {
        var tm = new TableManager(null);

        var where = new WhereCondition()
            .WhereNotIn("TABLE_NAME", new ObjectQuery<DataClassInfo>().Column("ClassTableName").WhereNotNull("ClassTableName"))
            .WhereNotIn("TABLE_NAME", new List<string> { "Analytics_Index", "sysdiagrams", "Temp_WebPart" });

        drpExistingTables.DataSource = tm.GetTables(where.ToString());
        drpExistingTables.DataBind();
    }


    /// <summary>
    /// Indicates if table has identity column defined
    /// </summary>
    /// <param name="tableName">Table name</param>
    /// <param name="columnName">Column name</param>
    /// <returns>Returns TRUE if table has identity column</returns>
    private static bool IsIdentityColumn(string tableName, string columnName)
    {
        const string queryText = @"SELECT COLUMNPROPERTY(OBJECT_ID(@tableName), @columnName, 'IsIdentity')";

        var queryData = new QueryDataParameters
        {
            { "tableName", tableName },
            { "columnName", columnName }
        };

        var result = ConnectionHelper.ExecuteScalar(queryText, queryData, QueryTypeEnum.SQLQuery);
        return ValidationHelper.GetBoolean(result, false);
    }

    #endregion
}