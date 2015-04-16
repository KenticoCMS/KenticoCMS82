using System;
using System.Data;
using System.Linq;

using CMS.Core;
using CMS.Helpers;
using CMS.Base;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.DataEngine;

public partial class CMSModules_AdminControls_Controls_UIControls_BindingEditItem : CMSAbstractUIWebpart
{
    #region "Variables"

    private string mCurrentValues;
    private BaseInfo obj, objProvider, objTarget;

    #endregion


    #region "Properties"

    /// <summary>
    /// Current values
    /// </summary>
    private string CurrentValues
    {
        get
        {
            return mCurrentValues ?? (mCurrentValues = GetCurrentValues());
        }
    }


    /// <summary>
    /// Object type for M:N relationship
    /// </summary>
    public String BindingObjectType
    {
        get
        {
            return GetStringContextValue("BindingObjectType");
        }
        set
        {
            SetValue("BindingObjectType", value);
        }
    }


    /// <summary>
    /// Resource prefix for multi uni selector
    /// </summary>
    public String ResourcePrefix
    {
        get
        {
            return GetStringContextValue("ResourcePrefix");
        }
        set
        {
            SetValue("ResourcePrefix", value);
        }
    }


    /// <summary>
    /// Object type for M:N relationship (f.e. MembershipRole)
    /// </summary>
    public String TargetObjectType
    {
        get
        {
            return GetStringContextValue("TargetObjectType");
        }
        set
        {
            SetValue("TargetObjectType", value);
        }
    }


    /// <summary>
    /// Where condition
    /// </summary>
    public String WhereCondition
    {
        get
        {
            return GetStringContextValue("WhereCondition", String.Empty, true, true);
        }
        set
        {
            SetValue("WhereCondition", value);
        }
    }


    /// <summary>
    /// Dialog where condition
    /// </summary>
    public String DialogWhereCondition
    {
        get
        {
            return GetStringContextValue("DialogWhereCondition", String.Empty, true, true);
        }
        set
        {
            SetValue("DialogWhereCondition", value);
        }
    }


    /// <summary>
    /// Returns true if the control processing should be stopped
    /// </summary>
    public override bool StopProcessing
    {
        get
        {
            return base.StopProcessing;
        }
        set
        {
            base.StopProcessing = value;
            editElem.StopProcessing = value;
        }
    }

    #endregion


    #region "Methods"

    protected override void OnInit(EventArgs e)
    {
        editElem.ContextResolver.SetNamedSourceData("UIContext", UIContext);
        base.OnInit(e);

        if (UIContext.EditedObject == null)
        {
            ShowError(GetString("ui.editing.noobjecttype"));
            StopProcessing = true;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            // No actions if processing is stopped
        }
        else
        {
            editElem.OnSelectionChanged += editElem_OnSelectionChanged;

            // Create object types (BaseInfo)
            obj = ModuleManager.GetObject(ObjectType);
            objProvider = ModuleManager.GetObject(BindingObjectType);

            // Automatic compute target object type
            if (String.IsNullOrEmpty(TargetObjectType))
            {
                var providerTypeInfo = objProvider.TypeInfo;
                
                // Search for parent in TYPEINFO
                var parent = providerTypeInfo.ParentObjectType;
                if ((parent != String.Empty) && (parent != ObjectType))
                {
                    // If parent is different from control's object type use it.
                    TargetObjectType = parent;
                }
                else
                {
                    // Otherwise search in site object
                    var siteObject = providerTypeInfo.SiteIDColumn;
                    if ((siteObject != String.Empty) && (siteObject != ObjectTypeInfo.COLUMN_NAME_UNKNOWN))
                    {
                        TargetObjectType = SiteInfo.OBJECT_TYPE;
                    }
                    else
                    {
                        // If site object not specified use bindings. Find first binding dependency and use it's object type
                        var od = providerTypeInfo.ObjectDependencies.FirstOrDefault(x => x.DependencyType == ObjectDependencyEnum.Binding);
                        if (od != null)
                        {
                            TargetObjectType = od.DependencyObjectType;
                        }
                    }
                }
            }

            objTarget = ModuleManager.GetObject(TargetObjectType);

            //Check view permission
            if (!CheckViewPermissions(objProvider))
            {
                editElem.StopProcessing = true;
                editElem.Visible = false;
                return;
            }

            // Check edit permissions
            if (!CheckEditPermissions(objProvider))
            {
                editElem.Enabled = false;
                ShowError(GetString("ui.notauthorizemodified"));
            }

            // Validate input data
            if (!ValidateInputData())
            {
                return;
            }

            // Set uni selector
            editElem.ObjectType = TargetObjectType;
            editElem.ResourcePrefix = ResourcePrefix;
            editElem.WhereCondition = DialogWhereCondition;

            if (!RequestHelper.IsPostBack())
            {
                // Set values
                editElem.Value = CurrentValues;
            }
        }
    }


    /// <summary>
    /// Gets the current values from database
    /// </summary>
    private string GetCurrentValues()
    {
        // Get all items based on where condition
        DataSet ds = objProvider.Generalized.GetDataQuery(true, s => s.Where(WhereCondition).Column(objTarget.TypeInfo.IDColumn), false).Result;

        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            return TextHelper.Join(";", DataHelper.GetStringValues(ds.Tables[0], objTarget.TypeInfo.IDColumn));
        }

        return "";
    }


    /// <summary>
    /// Validates input data based on ui element settings
    /// </summary>
    private bool ValidateInputData()
    {
        if (objTarget == null)
        {
            ShowError(GetString("ui.editing.notargetobjecttype"));
            return false;
        }

        if (objProvider == null)
        {
            ShowError(GetString("ui.editing.nobindingobjecttype"));
            return false;
        }

        return true;
    }


    protected void editElem_OnSelectionChanged(object sender, EventArgs ea)
    {
        SaveData();
    }


    /// <summary>
    /// Returns binding column names for binding object type.
    /// 1. Try to search ParentObjectType (there should be first binding column name). 
    /// 2. Search for site ID column. In site bindings you will find column name for site.
    /// 3. If one of the columns is still not found, search all object's dependencies.
    /// </summary>
    public Tuple<String, String> GetBindingColumnNames()
    {
        String objCol = String.Empty,
            targetCol = String.Empty;

        GeneralizedInfo providerInfo = objProvider.Generalized;

        var providerTypeInfo = providerInfo.TypeInfo;

        var objTypeInfo = obj.TypeInfo;
        var targetTypeInfo = objTarget.TypeInfo;

        // 1. ParentObjectType
        if (providerInfo.ParentObjectType.EqualsCSafe(objTypeInfo.ObjectType, true))
        {
            objCol = providerTypeInfo.ParentIDColumn;
        }
        else if (providerInfo.ParentObjectType.EqualsCSafe(targetTypeInfo.ObjectType, true))
        {
            targetCol = providerTypeInfo.ParentIDColumn;
        }

        // 2. Site bindings
        if (providerTypeInfo.SiteIDColumn != ObjectTypeInfo.COLUMN_NAME_UNKNOWN)
        {
            if (String.IsNullOrEmpty(objCol) && objTypeInfo.ObjectType.EqualsCSafe(ModuleName.SITE, true))
            {
                objCol = providerTypeInfo.SiteIDColumn;
            }

            if (String.IsNullOrEmpty(targetCol) && targetTypeInfo.ObjectType.EqualsCSafe(ModuleName.SITE, true))
            {
                targetCol = providerTypeInfo.SiteIDColumn;
            }
        }

        // 3. Object's dependencies
        if (!String.IsNullOrEmpty(targetCol) || !String.IsNullOrEmpty(objCol))
        {
            // Check all object's dependencies
            if (providerTypeInfo.DependsOn != null)
            {
                foreach (ObjectDependency od in providerTypeInfo.DependsOn)
                {
                    if (od.DependencyObjectType.EqualsCSafe(objTypeInfo.ObjectType, true))
                    {
                        objCol = od.DependencyColumn;
                    }

                    if (od.DependencyObjectType.EqualsCSafe(targetTypeInfo.ObjectType, true))
                    {
                        targetCol = od.DependencyColumn;
                    }
                }
            }
        }

        return new Tuple<String, String>(objCol, targetCol);
    }


    /// <summary>
    /// Store selected (unselected) roles.
    /// </summary>
    private void SaveData()
    {
        if (!editElem.Enabled)
        {
            ShowError(GetString("ui.notauthorizemodified"));
            return;
        }

        // Remove old items
        string newValues = ValidationHelper.GetString(editElem.Value, null);
        string deletedItems = DataHelper.GetNewItemsInList(newValues, CurrentValues);
        string addedItems = DataHelper.GetNewItemsInList(CurrentValues, newValues);

        if (!String.IsNullOrEmpty(deletedItems) || !String.IsNullOrEmpty(addedItems))
        {
            bool saved = false;

            // Find column names for both binding
            Tuple<String, String> columns = GetBindingColumnNames();
            String objCol = columns.Item1;
            String targetCol = columns.Item2;

            if (!String.IsNullOrEmpty(targetCol) && !String.IsNullOrEmpty(objCol))
            {
                if (!String.IsNullOrEmpty(deletedItems))
                {
                    string[] newItems = deletedItems.Split(new [] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    if (newItems.Length > 0)
                    {
                        // If provider object has object ID column, retrieve all changed objects by single query
                        var providerTypeInfo = objProvider.TypeInfo;
                        if (providerTypeInfo.IDColumn != ObjectTypeInfo.COLUMN_NAME_UNKNOWN)
                        {
                            // For each retrieved object, create BaseInfo and delete it
                            var q = objProvider.Generalized.GetDataQuery(false, s => s
                                .WhereEquals(objCol, ObjectID)
                                .WhereIn(targetCol, newItems.ToList())
                                , false
                            );

                            DataSet ds = q.Result;
                            if (!DataHelper.DataSourceIsEmpty(ds))
                            {
                                foreach (DataRow dr in ds.Tables[0].Rows)
                                {
                                    // Load base info based on datarow
                                    BaseInfo bi = ModuleManager.GetObject(dr, providerTypeInfo.ObjectType);
                                    bi.Delete();

                                    saved = true;
                                }
                            }
                        }
                        else
                        {
                            foreach (string item in newItems)
                            {
                                int bindingObjectID = ValidationHelper.GetInteger(item, 0);

                                objProvider.SetValue(objCol, ObjectID);
                                objProvider.SetValue(targetCol, bindingObjectID);
                                objProvider.Delete();

                                saved = true;
                            }
                        }
                    }
                }

                // Add new items
                if (!String.IsNullOrEmpty(addedItems))
                {
                    string[] newItems = addedItems.Split(new [] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    
                    // Add all new items to site
                    foreach (string item in newItems)
                    {
                        int bindingObjectID = ValidationHelper.GetInteger(item, 0);

                        objProvider.SetValue(objCol, ObjectID);
                        objProvider.SetValue(targetCol, bindingObjectID);
                        objProvider.Insert();
                        saved = true;
                    }
                }

                if (saved)
                {
                    obj.TypeInfo.InvalidateAllObjects();
                    ShowChangesSaved();
                }
            }
        }
    }

    #endregion
}
