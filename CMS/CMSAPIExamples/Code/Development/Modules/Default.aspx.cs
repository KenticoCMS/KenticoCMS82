using System;
using System.Data;

using CMS.Helpers;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;
using CMS.Modules;

public partial class CMSAPIExamples_Code_Development_Modules_Default : CMSAPIExamplePage
{
    #region "Initialization"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Module
        apiCreateModule.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateModule);
        apiGetAndUpdateModule.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndUpdateModule);
        apiGetAndBulkUpdateModules.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndBulkUpdateModules);
        apiDeleteModule.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteModule);

        // Module on site
        apiAddModuleToSite.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(AddModuleToSite);
        apiGetAndBulkUpdateSiteModules.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndBulkUpdateSiteModules);
        apiRemoveModuleFromSite.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(RemoveModuleFromSite);

        // Permission
        apiCreatePermission.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreatePermission);
        apiGetAndUpdatePermission.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndUpdatePermission);
        apiGetAndBulkUpdatePermissions.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndBulkUpdatePermissions);
        apiDeletePermission.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeletePermission);

        // Role permission
        apiAddPermissionToRole.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(AddPermissionToRole);
        apiRemovePermissionFromRole.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(RemovePermissionFromRole);

        // UI element
        apiCreateUIElement.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateUIElement);
        apiGetAndUpdateUIElement.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndUpdateUIElement);
        apiGetAndBulkUpdateUIElements.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndBulkUpdateUIElements);
        apiDeleteUIElement.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteUIElement);

        // Role UI element
        apiAddUIElementToRole.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(AddUIElementToRole);
        apiRemoveUIElementFromRole.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(RemoveUIElementFromRole);
    }

    #endregion


    #region "Mass actions"

    /// <summary>
    /// Runs all creating and managing examples.
    /// </summary>
    public override void RunAll()
    {
        base.RunAll();

        // Module
        apiCreateModule.Run();
        apiGetAndUpdateModule.Run();
        apiGetAndBulkUpdateModules.Run();

        // Module on site
        apiAddModuleToSite.Run();
        apiGetAndBulkUpdateSiteModules.Run();

        // Permission
        apiCreatePermission.Run();
        apiGetAndUpdatePermission.Run();
        apiGetAndBulkUpdatePermissions.Run();

        // Role permission
        apiAddPermissionToRole.Run();

        // UI element
        apiCreateUIElement.Run();
        apiGetAndUpdateUIElement.Run();
        apiGetAndBulkUpdateUIElements.Run();

        // Role UI element
        apiAddUIElementToRole.Run();
    }


    /// <summary>
    /// Runs all cleanup examples.
    /// </summary>
    public override void CleanUpAll()
    {
        base.CleanUpAll();

        // Role UI element
        apiRemoveUIElementFromRole.Run();

        // UI element
        apiDeleteUIElement.Run();

        // Role permission
        apiRemovePermissionFromRole.Run();

        // Permission
        apiDeletePermission.Run();

        // Module on site
        apiRemoveModuleFromSite.Run();

        // Module
        apiDeleteModule.Run();
    }

    #endregion


    #region "API examples - Module"

    /// <summary>
    /// Creates module. Called when the "Create module" button is pressed.
    /// </summary>
    private bool CreateModule()
    {
        // Create new module object
        ResourceInfo newModule = new ResourceInfo();

        // Set the properties
        newModule.ResourceDisplayName = "My new module";
        newModule.ResourceName = "MyNewModule";
        newModule.ResourceIsInDevelopment = true;

        // Save the module
        ResourceInfoProvider.SetResourceInfo(newModule);

        return true;
    }


    /// <summary>
    /// Gets and updates module. Called when the "Get and update module" button is pressed.
    /// Expects the CreateModule method to be run first.
    /// </summary>
    private bool GetAndUpdateModule()
    {
        // Get the module
        ResourceInfo updateModule = ResourceInfoProvider.GetResourceInfo("MyNewModule");
        if (updateModule != null)
        {
            // Update the properties
            updateModule.ResourceDisplayName = updateModule.ResourceDisplayName.ToLower();

            // Save the changes
            ResourceInfoProvider.SetResourceInfo(updateModule);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates modules. Called when the "Get and bulk update modules" button is pressed.
    /// Expects the CreateModule method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateModules()
    {
        // Prepare the parameters
        string where = "ResourceName LIKE N'MyNewModule%'";

        // Get the data
        DataSet modules = ResourceInfoProvider.GetResources(where, null);
        if (!DataHelper.DataSourceIsEmpty(modules))
        {
            // Loop through the individual items
            foreach (DataRow moduleDr in modules.Tables[0].Rows)
            {
                // Create object from DataRow
                ResourceInfo modifyModule = new ResourceInfo(moduleDr);

                // Update the properties
                modifyModule.ResourceDisplayName = modifyModule.ResourceDisplayName.ToUpper();

                // Save the changes
                ResourceInfoProvider.SetResourceInfo(modifyModule);
            }

            return true;
        }

        return false;
    }


    /// <summary>
    /// Deletes module. Called when the "Delete module" button is pressed.
    /// Expects the CreateModule method to be run first.
    /// </summary>
    private bool DeleteModule()
    {
        // Get the module
        ResourceInfo deleteModule = ResourceInfoProvider.GetResourceInfo("MyNewModule");

        // Delete the module
        ResourceInfoProvider.DeleteResourceInfo(deleteModule);

        return (deleteModule != null);
    }

    #endregion


    #region "API examples - Module on site"

    /// <summary>
    /// Adds module to site. Called when the "Add module to site" button is pressed.
    /// Expects the CreateModule method to be run first.
    /// </summary>
    private bool AddModuleToSite()
    {
        /// Get the module
        ResourceInfo module = ResourceInfoProvider.GetResourceInfo("MyNewModule");
        if (module != null)
        {
            int moduleId = module.ResourceId;
            int siteId = SiteContext.CurrentSiteID;

            // Save the binding
            ResourceSiteInfoProvider.AddResourceToSite(moduleId, siteId);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates site modules. Called when the "Get and bulk update site modules" button is pressed.
    /// Expects the AddModuleToSite method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateSiteModules()
    {
        int siteId = SiteContext.CurrentSiteID;
        string where = "ResourceName LIKE N'MyNewModule%'";

        // Get the data
        DataSet modules = ResourceInfoProvider.GetResources(where, null, 0, null, siteId);
        if (!DataHelper.DataSourceIsEmpty(modules))
        {
            // Loop through the individual items
            foreach (DataRow moduleDr in modules.Tables[0].Rows)
            {
                // Create object from DataRow
                ResourceInfo modifyModule = new ResourceInfo(moduleDr);

                // Update the properties
                modifyModule.ResourceDisplayName = modifyModule.ResourceDisplayName.ToLower();

                // Save the changes
                ResourceInfoProvider.SetResourceInfo(modifyModule);
            }

            return true;
        }

        return false;
    }


    /// <summary>
    /// Removes module from site. Called when the "Remove module from site" button is pressed.
    /// Expects the AddModuleToSite method to be run first.
    /// </summary>
    private bool RemoveModuleFromSite()
    {
        // Get the module
        ResourceInfo removeModule = ResourceInfoProvider.GetResourceInfo("MyNewModule");
        if (removeModule != null)
        {
            int siteId = SiteContext.CurrentSiteID;

            // Get the binding
            ResourceSiteInfo moduleSite = ResourceSiteInfoProvider.GetResourceSiteInfo(removeModule.ResourceId, siteId);

            // Delete the binding
            ResourceSiteInfoProvider.DeleteResourceSiteInfo(moduleSite);

            return true;
        }

        return false;
    }

    #endregion


    #region "API examples - Permission"

    /// <summary>
    /// Creates permission. Called when the "Create permission" button is pressed.
    /// Expects the CreateModule method to be run first.
    /// </summary>
    private bool CreatePermission()
    {
        // Get the resource
        ResourceInfo module = ResourceInfoProvider.GetResourceInfo("MyNewModule");
        if (module != null)
        {
            // Create new permission object
            PermissionNameInfo newPermission = new PermissionNameInfo();

            // Set the properties
            newPermission.PermissionDisplayName = "My new permission";
            newPermission.PermissionName = "MyNewPermission";
            newPermission.ResourceId = module.ResourceId;
            newPermission.PermissionDisplayInMatrix = true;

            // Save the permission
            PermissionNameInfoProvider.SetPermissionInfo(newPermission);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and updates permission. Called when the "Get and update permission" button is pressed.
    /// Expects the CreatePermission method to be run first.
    /// </summary>
    private bool GetAndUpdatePermission()
    {
        // Get the permission
        PermissionNameInfo updatePermission = PermissionNameInfoProvider.GetPermissionNameInfo("MyNewPermission", "MyNewModule", null);
        if (updatePermission != null)
        {
            // Update the properties
            updatePermission.PermissionDisplayName = updatePermission.PermissionDisplayName.ToLower();

            // Save the changes
            PermissionNameInfoProvider.SetPermissionInfo(updatePermission);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates permissions. Called when the "Get and bulk update permissions" button is pressed.
    /// Expects the CreatePermission method to be run first.
    /// </summary>
    private bool GetAndBulkUpdatePermissions()
    {
        // Prepare the parameters
        string where = "PermissionName LIKE N'MyNewPermission%'";

        // Get the data
        DataSet permissions = PermissionNameInfoProvider.GetPermissionNames(where, null, 0, null);
        if (!DataHelper.DataSourceIsEmpty(permissions))
        {
            // Loop through the individual items
            foreach (DataRow permissionDr in permissions.Tables[0].Rows)
            {
                // Create object from DataRow
                PermissionNameInfo modifyPermission = new PermissionNameInfo(permissionDr);

                // Update the properties
                modifyPermission.PermissionDisplayName = modifyPermission.PermissionDisplayName.ToUpper();

                // Save the changes
                PermissionNameInfoProvider.SetPermissionInfo(modifyPermission);
            }

            return true;
        }

        return false;
    }


    /// <summary>
    /// Deletes permission. Called when the "Delete permission" button is pressed.
    /// Expects the CreatePermission method to be run first.
    /// </summary>
    private bool DeletePermission()
    {
        // Get the permission
        PermissionNameInfo deletePermission = PermissionNameInfoProvider.GetPermissionNameInfo("MyNewPermission", "MyNewModule", null);

        // Delete the permission
        PermissionNameInfoProvider.DeletePermissionInfo(deletePermission);

        return (deletePermission != null);
    }

    #endregion


    #region "API examples - Role permission"

    /// <summary>
    /// Adds permission to role. Called when the "Add permission to role" button is pressed.
    /// Expects the CreatePermission method to be run first.
    /// </summary>
    private bool AddPermissionToRole()
    {
        // Get the permission
        PermissionNameInfo permission = PermissionNameInfoProvider.GetPermissionNameInfo("MyNewPermission", "MyNewModule", null);

        // Get the role
        RoleInfo role = RoleInfoProvider.GetRoleInfo("cmsdeskadmin", SiteContext.CurrentSiteID);

        if ((permission != null) && (role != null))
        {
            // Create new role permission object
            RolePermissionInfo newRolePermission = new RolePermissionInfo();

            // Set the properties
            newRolePermission.PermissionID = permission.PermissionId;
            newRolePermission.RoleID = role.RoleID;

            // Add permission to role
            RolePermissionInfoProvider.SetRolePermissionInfo(newRolePermission);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Removes permission from role. Called when the "Remove permission from role" button is pressed.
    /// Expects the AddPermissionToRole method to be run first.
    /// </summary>
    private bool RemovePermissionFromRole()
    {
        // Get the permission
        PermissionNameInfo permission = PermissionNameInfoProvider.GetPermissionNameInfo("MyNewPermission", "MyNewModule", null);

        // Get the role
        RoleInfo role = RoleInfoProvider.GetRoleInfo("cmsdeskadmin", SiteContext.CurrentSiteID);

        if ((permission != null) && (role != null))
        {
            // Get the role permission
            RolePermissionInfo deleteRolePermission = RolePermissionInfoProvider.GetRolePermissionInfo(role.RoleID, permission.PermissionId);

            // Remove permission from role
            RolePermissionInfoProvider.DeleteRolePermissionInfo(deleteRolePermission);

            return true;
        }

        return false;
    }

    #endregion


    #region "API examples - UI element"

    /// <summary>
    /// Creates UI element. Called when the "Create element" button is pressed.
    /// Expects the CreateModule method to be run first.
    /// </summary>
    private bool CreateUIElement()
    {
        // Get the module
        ResourceInfo module = ResourceInfoProvider.GetResourceInfo("MyNewModule");
        if (module != null)
        {
            // Get the parent UI element
            UIElementInfo rootElement = UIElementInfoProvider.GetRootUIElementInfo(module.ResourceId);
            if (rootElement == null)
            {
                // Create root UI element 
                rootElement = new UIElementInfo();
                rootElement.ElementResourceID = module.ResourceId;
                rootElement.ElementDisplayName = module.ResourceDisplayName;
                rootElement.ElementName = module.ResourceName.ToLower().Replace(".", "");
                rootElement.ElementIsCustom = false;

                // Save root UI element
                UIElementInfoProvider.SetUIElementInfo(rootElement);
            }

            // Create new UI element object
            UIElementInfo newElement = new UIElementInfo();

            // Set the properties
            newElement.ElementDisplayName = "My new element";
            newElement.ElementName = "MyNewElement";
            newElement.ElementResourceID = module.ResourceId;
            newElement.ElementIsCustom = true;
            newElement.ElementParentID = rootElement.ElementID;

            // Save the UI element
            UIElementInfoProvider.SetUIElementInfo(newElement);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and updates UI element. Called when the "Get and update element" button is pressed.
    /// Expects the CreateUIElement method to be run first.
    /// </summary>
    private bool GetAndUpdateUIElement()
    {
        // Get the UI element
        UIElementInfo updateElement = UIElementInfoProvider.GetUIElementInfo("MyNewModule", "MyNewElement");
        if (updateElement != null)
        {
            // Update the properties
            updateElement.ElementDisplayName = updateElement.ElementDisplayName.ToLower();

            // Save the changes
            UIElementInfoProvider.SetUIElementInfo(updateElement);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates UI elements. Called when the "Get and bulk update elements" button is pressed.
    /// Expects the CreateUIElement method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateUIElements()
    {
        // Prepare the parameters
        string where = "ElementName LIKE N'MyNewElement%'";

        // Get the data
        DataSet elements = UIElementInfoProvider.GetUIElements(where, null);
        if (!DataHelper.DataSourceIsEmpty(elements))
        {
            // Loop through the individual items
            foreach (DataRow elementDr in elements.Tables[0].Rows)
            {
                // Create object from DataRow
                UIElementInfo modifyElement = new UIElementInfo(elementDr);

                // Update the properties
                modifyElement.ElementDisplayName = modifyElement.ElementDisplayName.ToUpper();

                // Save the changes
                UIElementInfoProvider.SetUIElementInfo(modifyElement);
            }

            return true;
        }

        return false;
    }


    /// <summary>
    /// Deletes UI element. Called when the "Delete element" button is pressed.
    /// Expects the CreateUIElement method to be run first.
    /// </summary>
    private bool DeleteUIElement()
    {
        // Get the UI element
        UIElementInfo deleteElement = UIElementInfoProvider.GetUIElementInfo("MyNewModule", "MyNewElement");

        // Delete the UI element
        UIElementInfoProvider.DeleteUIElementInfo(deleteElement);

        return (deleteElement != null);
    }

    #endregion


    #region "API examples - Role UI element"

    /// <summary>
    /// Creates role UI element. Called when the "Add element to role" button is pressed.
    /// Expects the CreateUIElement method to be run first.
    /// </summary>
    private bool AddUIElementToRole()
    {
        // Get the role
        RoleInfo role = RoleInfoProvider.GetRoleInfo("cmsdeskadmin", SiteContext.CurrentSiteID);

        // Get the UI element
        UIElementInfo element = UIElementInfoProvider.GetUIElementInfo("MyNewModule", "MyNewElement");

        if ((role != null) && (element != null))
        {
            // Create new role UI element object
            RoleUIElementInfo newRoleElement = new RoleUIElementInfo();

            // Set the properties
            newRoleElement.RoleID = role.RoleID;
            newRoleElement.ElementID = element.ElementID;

            // Save the role UI element
            RoleUIElementInfoProvider.SetRoleUIElementInfo(newRoleElement);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Removes UI element from role. Called when the "Remove element from role" button is pressed.
    /// Expects the AddUIElementToRole method to be run first.
    /// </summary>
    private bool RemoveUIElementFromRole()
    {
        // Get the role
        RoleInfo role = RoleInfoProvider.GetRoleInfo("cmsdeskadmin", SiteContext.CurrentSiteID);

        // Get the UI element
        UIElementInfo element = UIElementInfoProvider.GetUIElementInfo("MyNewModule", "MyNewElement");

        if ((role != null) && (element != null))
        {
            // Get the role UI element
            RoleUIElementInfo deleteElement = RoleUIElementInfoProvider.GetRoleUIElementInfo(role.RoleID, element.ElementID);

            // Delete the role UI element
            RoleUIElementInfoProvider.DeleteRoleUIElementInfo(deleteElement);

            return (deleteElement != null);
        }

        return false;
    }

    #endregion
}