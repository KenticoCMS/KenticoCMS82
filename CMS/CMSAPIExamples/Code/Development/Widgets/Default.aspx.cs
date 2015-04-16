using System;
using System.Data;

using CMS.FormEngine;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;
using CMS.Modules;

public partial class CMSAPIExamples_Code_Development_Widgets_Default : CMSAPIExamplePage
{
    #region "Initialization"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Widget
        apiCreateWidget.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateWidget);
        apiGetAndUpdateWidget.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndUpdateWidget);
        apiGetAndBulkUpdateWidgets.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndBulkUpdateWidgets);
        apiDeleteWidget.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteWidget);

        // Widget category
        apiCreateWidgetCategory.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateWidgetCategory);
        apiGetAndUpdateWidgetCategory.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndUpdateWidgetCategory);
        apiGetAndBulkUpdateWidgetCategories.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndBulkUpdateWidgetCategories);
        apiDeleteWidgetCategory.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteWidgetCategory);

        // Widget security
        apiAddWidgetToRole.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(AddWidgetToRole);
        apiSetSecurityLevel.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(SetSecurityLevel);
        apiRemoveWidgetFromRole.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(RemoveWidgetFromRole);
    }

    #endregion


    #region "Mass actions"

    /// <summary>
    /// Runs all creating and managing examples.
    /// </summary>
    public override void RunAll()
    {
        base.RunAll();

        // Widget category
        apiCreateWidgetCategory.Run();
        apiGetAndUpdateWidgetCategory.Run();
        apiGetAndBulkUpdateWidgetCategories.Run();

        // Widget
        apiCreateWidget.Run();
        apiGetAndUpdateWidget.Run();
        apiGetAndBulkUpdateWidgets.Run();

        // Widget security
        apiAddWidgetToRole.Run();
        apiSetSecurityLevel.Run();
    }


    /// <summary>
    /// Runs all cleanup examples.
    /// </summary>
    public override void CleanUpAll()
    {
        base.CleanUpAll();

        // Widget role
        apiRemoveWidgetFromRole.Run();

        // Widget
        apiDeleteWidget.Run();

        // Widget category
        apiDeleteWidgetCategory.Run();
    }

    #endregion


    #region "API examples - Widget category"

    /// <summary>
    /// Creates widget category. Called when the "Create category" button is pressed.
    /// </summary>
    private bool CreateWidgetCategory()
    {
        // Create new widget category object
        WidgetCategoryInfo newCategory = new WidgetCategoryInfo();

        // Set the properties
        newCategory.WidgetCategoryDisplayName = "My new category";
        newCategory.WidgetCategoryName = "MyNewCategory";

        // Save the widget category
        WidgetCategoryInfoProvider.SetWidgetCategoryInfo(newCategory);

        return true;
    }


    /// <summary>
    /// Gets and updates widget category. Called when the "Get and update category" button is pressed.
    /// Expects the CreateWidgetCategory method to be run first.
    /// </summary>
    private bool GetAndUpdateWidgetCategory()
    {
        // Get the widget category
        WidgetCategoryInfo updateCategory = WidgetCategoryInfoProvider.GetWidgetCategoryInfo("MyNewCategory");
        if (updateCategory != null)
        {
            // Update the properties
            updateCategory.WidgetCategoryDisplayName = updateCategory.WidgetCategoryDisplayName.ToLower();

            // Save the changes
            WidgetCategoryInfoProvider.SetWidgetCategoryInfo(updateCategory);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates widget categories. Called when the "Get and bulk update categories" button is pressed.
    /// Expects the CreateWidgetCategory method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateWidgetCategories()
    {
        // Prepare the parameters
        string where = "WidgetCategoryName LIKE N'MyNewCategory%'";
        string orderBy = "";
        int topN = 0;
        string columns = "";

        // Get the data
        DataSet categories = WidgetCategoryInfoProvider.GetWidgetCategories()
            .Where(where)
            .OrderBy(orderBy)
            .TopN(topN)
            .Columns(columns);

        if (!DataHelper.DataSourceIsEmpty(categories))
        {
            // Loop through the individual items
            foreach (DataRow categoryDr in categories.Tables[0].Rows)
            {
                // Create object from DataRow
                WidgetCategoryInfo modifyCategory = new WidgetCategoryInfo(categoryDr);

                // Update the properties
                modifyCategory.WidgetCategoryDisplayName = modifyCategory.WidgetCategoryDisplayName.ToUpper();

                // Save the changes
                WidgetCategoryInfoProvider.SetWidgetCategoryInfo(modifyCategory);
            }

            return true;
        }

        return false;
    }


    /// <summary>
    /// Deletes widget category. Called when the "Delete category" button is pressed.
    /// Expects the CreateWidgetCategory method to be run first.
    /// </summary>
    private bool DeleteWidgetCategory()
    {
        // Get the widget category
        WidgetCategoryInfo deleteCategory = WidgetCategoryInfoProvider.GetWidgetCategoryInfo("MyNewCategory");

        // Delete the widget category
        WidgetCategoryInfoProvider.DeleteWidgetCategoryInfo(deleteCategory);

        return (deleteCategory != null);
    }

    #endregion


    #region "API examples - Widget"

    /// <summary>
    /// Creates widget. Called when the "Create widget" button is pressed.
    /// </summary>
    private bool CreateWidget()
    {
        // Get parent webpart and category for widget
        WebPartInfo webpart = WebPartInfoProvider.GetWebPartInfo("AbuseReport");
        WidgetCategoryInfo category = WidgetCategoryInfoProvider.GetWidgetCategoryInfo("MyNewCategory");

        // Widget cannot be created from inherited webpart
        if ((webpart != null) && (webpart.WebPartParentID == 0) && (category != null))
        {
            // Create new widget object 
            WidgetInfo newWidget = new WidgetInfo();

            // Set the properties from parent webpart
            newWidget.WidgetName = "MyNewWidget";
            newWidget.WidgetDisplayName = "My new widget";
            newWidget.WidgetDescription = webpart.WebPartDescription;

            newWidget.WidgetProperties = FormHelper.GetFormFieldsWithDefaultValue(webpart.WebPartProperties, "visible", "false");

            newWidget.WidgetWebPartID = webpart.WebPartID;
            newWidget.WidgetCategoryID = category.WidgetCategoryID;

            // Save new widget
            WidgetInfoProvider.SetWidgetInfo(newWidget);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and updates widget. Called when the "Get and update widget" button is pressed.
    /// Expects the CreateWidget method to be run first.
    /// </summary>
    private bool GetAndUpdateWidget()
    {
        // Get the widget
        WidgetInfo updateWidget = WidgetInfoProvider.GetWidgetInfo("MyNewWidget");
        if (updateWidget != null)
        {
            // Update the properties
            updateWidget.WidgetDisplayName = updateWidget.WidgetDisplayName.ToLower();

            // Save the changes
            WidgetInfoProvider.SetWidgetInfo(updateWidget);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates widgets. Called when the "Get and bulk update widgets" button is pressed.
    /// Expects the CreateWidget method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateWidgets()
    {
        // Prepare the parameters
        string where = "WidgetName LIKE N'MyNewWidget%'";
        string orderBy = "";
        int topN = 0;
        string columns = "";

        // Get the data
        DataSet widgets = WidgetInfoProvider
            .GetWidgets()
            .Where(where)
            .OrderBy(orderBy)
            .TopN(topN)
            .Columns(columns);

        if (!DataHelper.DataSourceIsEmpty(widgets))
        {
            // Loop through the individual items
            foreach (DataRow widgetDr in widgets.Tables[0].Rows)
            {
                // Create object from DataRow
                WidgetInfo modifyWidget = new WidgetInfo(widgetDr);

                // Update the properties
                modifyWidget.WidgetDisplayName = modifyWidget.WidgetDisplayName.ToUpper();

                // Save the changes
                WidgetInfoProvider.SetWidgetInfo(modifyWidget);
            }

            return true;
        }

        return false;
    }


    /// <summary>
    /// Deletes widget. Called when the "Delete widget" button is pressed.
    /// Expects the CreateWidget method to be run first.
    /// </summary>
    private bool DeleteWidget()
    {
        // Get the widget
        WidgetInfo deleteWidget = WidgetInfoProvider.GetWidgetInfo("MyNewWidget");

        // Delete the widget
        WidgetInfoProvider.DeleteWidgetInfo(deleteWidget);

        return (deleteWidget != null);
    }

    #endregion


    #region "API examples - Widget security"

    /// <summary>
    /// Add widget to role. Called when the "Add widget to role" button is pressed.
    /// Expects the CreateWidget method to be run first.
    /// </summary>
    private bool AddWidgetToRole()
    {
        // Get role, widget and permission object
        RoleInfo role = RoleInfoProvider.GetRoleInfo("CMSDeskAdmin", SiteContext.CurrentSiteID);
        WidgetInfo widget = WidgetInfoProvider.GetWidgetInfo("MyNewWidget");
        PermissionNameInfo permission = PermissionNameInfoProvider.GetPermissionNameInfo("AllowedFor", "Widgets", null);

        // If all exist
        if ((role != null) && (widget != null) && (permission != null))
        {
            // Add widget to role
            WidgetRoleInfoProvider.AddRoleToWidget(role.RoleID, widget.WidgetID, permission.PermissionId);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Remove widget from role. Called when the "Remove widget to role" button is pressed.
    /// Expects the CreateWidget method to be run first.
    /// </summary>
    private bool RemoveWidgetFromRole()
    {
        // Get role, widget and permission object
        RoleInfo role = RoleInfoProvider.GetRoleInfo("CMSDeskAdmin", SiteContext.CurrentSiteID);
        WidgetInfo widget = WidgetInfoProvider.GetWidgetInfo("MyNewWidget");
        PermissionNameInfo permission = PermissionNameInfoProvider.GetPermissionNameInfo("AllowedFor", "Widgets", null);

        // If all exist
        if ((role != null) && (widget != null) && (permission != null))
        {
            // Add widget to role
            WidgetRoleInfoProvider.RemoveRoleFromWidget(role.RoleID, widget.WidgetID, permission.PermissionId);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Sets Security level for widget. Called when the "Remove widget to role" button is pressed.
    /// Expects the CreateWidget method to be run first.
    /// </summary>
    private bool SetSecurityLevel()
    {
        // Get widget object
        WidgetInfo widget = WidgetInfoProvider.GetWidgetInfo("MyNewWidget");

        // If widget exists
        if (widget != null)
        {
            // Set security access
            widget.AllowedFor = SecurityAccessEnum.AuthenticatedUsers;

            WidgetInfoProvider.SetWidgetInfo(widget);

            return true;
        }

        return false;
    }

    #endregion
}