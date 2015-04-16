using System;
using CMS.Controls;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.SiteProvider;

public partial class CMSFormControls_Filters_ObjectFilter : CMSAbstractBaseFilterControl
{
    #region "Private variables"

    private GeneralizedInfo currentObject;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets where condition to uni selector.
    /// </summary>
    public override string WhereCondition
    {
        get
        {
            var ti = currentObject.TypeInfo;

            if (plcParentObject.Visible)
            {
                return (ValidationHelper.GetString(parentSelector.Value, "") != "") ? ti.ParentIDColumn + " = " + parentSelector.Value : String.Empty;
            }

            return (ti.SiteIDColumn != ObjectTypeInfo.COLUMN_NAME_UNKNOWN) ? siteSelector.GetWhereCondition(ti.SiteIDColumn) : String.Empty;
        }
        set
        {
            base.WhereCondition = value;
        }
    }

    #endregion


    #region "Events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        if (!StopProcessing)
        {
            SetupControl();
        }
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        GeneralizedInfo parentObject = ModuleManager.GetObject(currentObject.ParentObjectType);

        // Check if object or parent are site related
        if (((parentObject != null) && (parentObject.TypeInfo.SiteIDColumn != ObjectTypeInfo.COLUMN_NAME_UNKNOWN)) || (currentObject.TypeInfo.SiteIDColumn != ObjectTypeInfo.COLUMN_NAME_UNKNOWN))
        {
            // Check if global is allowed else select current site
            if (((parentObject != null) && parentObject.TypeInfo.SupportsGlobalObjects) || (currentObject.TypeInfo.SupportsGlobalObjects))
            {
                siteSelector.AllowGlobal = true;
            }
            else
            {
                if (!URLHelper.IsPostback())
                {
                    siteSelector.Value = SiteContext.CurrentSiteID;
                }
            }
            
            // Initialize site selector
            siteSelector.Reload(false);
            siteSelector.PostbackOnDropDownChange = true;
            siteSelector.UniSelector.OnSelectionChanged += DropDownSingleSelect_SelectedIndexChanged;
        }
        else
        {
            plcSite.Visible = false;
        }

        if ((parentObject != null) && (parentObject.TypeInfo.SiteIDColumn != ObjectTypeInfo.COLUMN_NAME_UNKNOWN))
        {
            // Get site where condition
            parentSelector.WhereCondition = siteSelector.GetWhereCondition(parentObject.TypeInfo.SiteIDColumn);
            parentSelector.Reload(true);
        }
    }


    /// <summary>
    /// Initialize filter controls.
    /// </summary>
    private void SetupControl()
    {
        if ((Parameters != null) && (Parameters["ObjectType"] != null))
        {
            // Get current object
            currentObject = ModuleManager.GetObject(ValidationHelper.GetString(Parameters["ObjectType"], String.Empty));

            // Check if object is not null and has parent object
            if ((currentObject != null) && !String.IsNullOrEmpty(currentObject.ParentObjectType))
            {
                lblParent.ResourceString = "objecttype." + currentObject.ParentObjectType.Replace(".", "_");

                // Set parent object selector properties
                parentSelector.ObjectType = currentObject.ParentObjectType;
                parentSelector.DropDownSingleSelect.AutoPostBack = true;
                parentSelector.OnSelectionChanged += parentSelector_OnSelectionChanged;
            }
            else
            {
                plcParentObject.Visible = false;
            }
        }
    }


    protected void DropDownSingleSelect_SelectedIndexChanged(object sender, EventArgs e)
    {
        FilterChanged = true;
        RaiseOnFilterChanged();
    }


    protected void parentSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        FilterChanged = true;
        RaiseOnFilterChanged();
    }

    #endregion
}
