using System;

using CMS.DataEngine;
using CMS.Helpers;
using CMS.Base;
using CMS.UIControls;
using CMS.Modules;

[EditedObject(DataClassInfo.OBJECT_TYPE, "classid")]
public partial class CMSModules_Modules_Pages_Class_General : GlobalAdminPage
{
    #region "Private fields"

    private bool mTableNameChanged;
    private string mOldTableName = String.Empty;

    #endregion


    #region "Private properties"

    /// <summary>
    /// Gets the info on current class.
    /// </summary>
    private DataClassInfo CurrentClass
    {
        get
        {
            return (DataClassInfo)EditedObject;
        }
    }


    /// <summary>
    /// Indicates whether table name changed.
    /// </summary>
    private bool TableNameChanged
    {
        get
        {
            if ((!URLHelper.IsPostback()) && (!String.IsNullOrEmpty(QueryHelper.GetString("tablechanged", string.Empty))))
            {
                return true;
            }

            return mTableNameChanged;
        }
        set
        {
            mTableNameChanged = value;
        }
    }


    /// <summary>
    /// Indicates if module allows to edit this tab.
    /// </summary>
    private bool IsEditable
    {
        get
        {
            ResourceInfo resource = ResourceInfoProvider.GetResourceInfo(QueryHelper.GetInteger("moduleid", 0));
            return ((resource != null) && resource.ResourceIsInDevelopment) || SystemContext.DevelopmentMode;
        }
    }

    #endregion


    /// <summary>
    /// Page_Load event handler
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        // Initialize controls
        SetupControl();
    }


    #region "Private methods"

    /// <summary>
    /// Initializes the controls on the page.
    /// </summary>
    private void SetupControl()
    {
        if (!IsEditable)
        {
            editElem.SubmitButton.Enabled = editElem.Enabled = false;
            ShowInformation(GetString("resource.installedresourcewarning"));
        }
        else
        {
            if (!URLHelper.IsPostback() && TableNameChanged)
            {
                // Display important information on table name change
                ShowInformation(GetString("sysdev.class_edit_general.tablechanged"));
            }

            editElem.OnAfterSave += editElem_OnAfterSave;
            editElem.OnBeforeValidate += editElem_OnBeforeValidate;

            // Save old table name
            mOldTableName = CurrentClass.ClassTableName;
        }
    }


    /// <summary>
    /// OnBeforeValidate event handler.
    /// </summary>
    protected void editElem_OnBeforeValidate(object sender, EventArgs e)
    {
        string className = editElem.GetFieldValue("ClassName").ToString();

        // Validate class name
        string errMsg = new Validator().NotEmpty(className, GetString("sysdev.class_edit_gen.name")).IsCodeName(className, String.Format(GetString("general.codenamenotvalid"), HTMLHelper.HTMLEncode(className))).Result;
        if (!String.IsNullOrEmpty(errMsg))
        {
            ShowError(errMsg);
            editElem.StopProcessing = true;
        }
        else
        {
            DataClassInfo dci = DataClassInfoProvider.GetDataClassInfo(className);

            // Check if class with specified code name already exist
            if ((dci != null) && (dci.ClassID != CurrentClass.ClassID))
            {
                ShowError(GetString("sysdev.class_edit_gen.codenameunique"));
                editElem.StopProcessing = true;
            }

            // Check if table name was modified
            string currentTableName = editElem.GetFieldValue("ClassTableName").ToString();
            if (CurrentClass.ClassTableName != currentTableName)
            {
                TableManager tm = new TableManager(null);

                if (tm.TableExists(currentTableName))
                {
                    ShowError(GetString("sysdev.class_edit_gen.tablenameunique"));
                    editElem.StopProcessing = true;
                }
                else
                {
                    TableNameChanged = true;
                }
            }
        }
    }


    /// <summary>
    /// OnAfterSave event handler.
    /// </summary>
    protected void editElem_OnAfterSave(object sender, EventArgs e)
    {
        if (TableNameChanged)
        {
            // Rename existing class table for actual connection string
            TableManager tm = new TableManager(editElem.GetFieldValue("ClassConnectionString").ToString());
            tm.RenameTable(mOldTableName, CurrentClass.ClassTableName);

            // Clear default queries
            QueryInfoProvider.ClearDefaultQueries(CurrentClass, true, true);

            if (CurrentClass.ClassIsDocumentType)
            {
                // Generate default view for document type
                SqlGenerator.GenerateDefaultView(CurrentClass, null);
            }

            editElem.RedirectUrlAfterSave = URLHelper.AddParameterToUrl(RequestContext.CurrentURL, "tablechanged", "1");
        }
    }

    #endregion
}