using System;
using System.Data;

using CMS.Helpers;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSAPIExamples_Code_Development_Webparts_Default : CMSAPIExamplePage
{
    #region "Initialization"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Web part
        apiCreateWebPart.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateWebPart);
        apiGetAndUpdateWebPart.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndUpdateWebPart);
        apiGetAndBulkUpdateWebParts.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndBulkUpdateWebParts);
        apiDeleteWebPart.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteWebPart);

        // Web part layout
        apiCreateWebPartLayout.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateWebPartLayout);
        apiGetAndUpdateWebPartLayout.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndUpdateWebPartLayout);
        apiGetAndBulkUpdateWebPartLayouts.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndBulkUpdateWebPartLayouts);
        apiDeleteWebPartLayout.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteWebPartLayout);

        // Web part category
        apiCreateWebPartCategory.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateWebPartCategory);
        apiGetAndUpdateWebPartCategory.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndUpdateWebPartCategory);
        apiGetAndBulkUpdateWebPartCategories.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndBulkUpdateWebPartCategories);
        apiDeleteWebPartCategory.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteWebPartCategory);

        // Web part container
        apiCreateWebPartContainer.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateWebPartContainer);
        apiGetAndUpdateWebPartContainer.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndUpdateWebPartContainer);
        apiGetAndBulkUpdateWebPartContainers.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndBulkUpdateWebPartContainers);
        apiDeleteWebPartContainer.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteWebPartContainer);

        // Web part container on site
        apiAddWebPartContainerToSite.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(AddWebPartContainerToSite);
        apiRemoveWebPartContainerFromSite.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(RemoveWebPartContainerFromSite);
    }

    #endregion


    #region "Mass actions"

    /// <summary>
    /// Runs all creating and managing examples.
    /// </summary>
    public override void RunAll()
    {
        base.RunAll();

        // Web part category
        apiCreateWebPartCategory.Run();
        apiGetAndUpdateWebPartCategory.Run();
        apiGetAndBulkUpdateWebPartCategories.Run();

        // Web part
        apiCreateWebPart.Run();
        apiGetAndUpdateWebPart.Run();
        apiGetAndBulkUpdateWebParts.Run();

        // Web part layout
        apiCreateWebPartLayout.Run();
        apiGetAndUpdateWebPartLayout.Run();
        apiGetAndBulkUpdateWebPartLayouts.Run();

        // Web part container
        apiCreateWebPartContainer.Run();
        apiGetAndUpdateWebPartContainer.Run();
        apiGetAndBulkUpdateWebPartContainers.Run();

        // Web part container on site
        apiAddWebPartContainerToSite.Run();
    }


    /// <summary>
    /// Runs all cleanup examples.
    /// </summary>
    public override void CleanUpAll()
    {
        base.CleanUpAll();

        // Web part container on site
        apiRemoveWebPartContainerFromSite.Run();

        // Web part container
        apiDeleteWebPartContainer.Run();

        // Web part layout
        apiDeleteWebPartLayout.Run();

        // Web part
        apiDeleteWebPart.Run();

        // Web part category
        apiDeleteWebPartCategory.Run();
    }

    #endregion


    #region "API examples - Web part category"

    /// <summary>
    /// Creates web part category. Called when the "Create category" button is pressed.
    /// </summary>
    private bool CreateWebPartCategory()
    {
        WebPartCategoryInfo root = WebPartCategoryInfoProvider.GetWebPartCategoryInfoByCodeName("/");

        if (root != null)
        {
            // Create new web part category object
            WebPartCategoryInfo newCategory = new WebPartCategoryInfo();

            // Set the properties
            newCategory.CategoryDisplayName = "My new category";
            newCategory.CategoryName = "MyNewCategory";
            newCategory.CategoryParentID = root.CategoryID;

            // Save the web part category
            WebPartCategoryInfoProvider.SetWebPartCategoryInfo(newCategory);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and updates web part category. Called when the "Get and update category" button is pressed.
    /// Expects the CreateWebPartCategory method to be run first.
    /// </summary>
    private bool GetAndUpdateWebPartCategory()
    {
        // Get the web part category
        WebPartCategoryInfo updateCategory = WebPartCategoryInfoProvider.GetWebPartCategoryInfoByCodeName("MyNewCategory");
        if (updateCategory != null)
        {
            // Update the properties
            updateCategory.CategoryDisplayName = updateCategory.CategoryDisplayName.ToLower();

            // Save the changes
            WebPartCategoryInfoProvider.SetWebPartCategoryInfo(updateCategory);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates web part categories. Called when the "Get and bulk update categories" button is pressed.
    /// Expects the CreateWebPartCategory method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateWebPartCategories()
    {
        // Prepare the parameters
        string where = "CategoryDisplayName LIKE N'My new category%'";

        // Get the data
        DataSet categories = WebPartCategoryInfoProvider.GetCategories().Where(where);
        if (!DataHelper.DataSourceIsEmpty(categories))
        {
            // Loop through the individual items
            foreach (DataRow categoryDr in categories.Tables[0].Rows)
            {
                // Create object from DataRow
                WebPartCategoryInfo modifyCategory = new WebPartCategoryInfo(categoryDr);

                // Update the properties
                modifyCategory.CategoryDisplayName = modifyCategory.CategoryDisplayName.ToUpper();

                // Save the changes
                WebPartCategoryInfoProvider.SetWebPartCategoryInfo(modifyCategory);
            }

            return true;
        }

        return false;
    }


    /// <summary>
    /// Deletes web part category. Called when the "Delete category" button is pressed.
    /// Expects the CreateWebPartCategory method to be run first.
    /// </summary>
    private bool DeleteWebPartCategory()
    {
        // Get the web part category
        WebPartCategoryInfo deleteCategory = WebPartCategoryInfoProvider.GetWebPartCategoryInfoByCodeName("MyNewCategory");

        // Delete the web part category
        WebPartCategoryInfoProvider.DeleteCategoryInfo(deleteCategory);

        return (deleteCategory != null);
    }

    #endregion


    #region "API examples - Web part"

    /// <summary>
    /// Creates web part. Called when the "Create part" button is pressed.
    /// </summary>
    private bool CreateWebPart()
    {
        // Get parent category for web part
        WebPartCategoryInfo category = WebPartCategoryInfoProvider.GetWebPartCategoryInfoByCodeName("MyNewCategory");

        if (category != null)
        {
            // Create new web part object
            WebPartInfo newWebpart = new WebPartInfo();

            // Set the properties
            newWebpart.WebPartDisplayName = "My new web part";
            newWebpart.WebPartName = "MyNewWebpart";
            newWebpart.WebPartDescription = "This is my new web part.";
            newWebpart.WebPartFileName = "whatever";
            newWebpart.WebPartProperties = "<form></form>";
            newWebpart.WebPartCategoryID = category.CategoryID;

            // Save the web part
            WebPartInfoProvider.SetWebPartInfo(newWebpart);

            return true;
        }
        return false;
    }


    /// <summary>
    /// Gets and updates web part. Called when the "Get and update part" button is pressed.
    /// Expects the CreateWebPart method to be run first.
    /// </summary>
    private bool GetAndUpdateWebPart()
    {
        // Get the web part
        WebPartInfo updateWebpart = WebPartInfoProvider.GetWebPartInfo("MyNewWebpart");
        if (updateWebpart != null)
        {
            // Update the properties
            updateWebpart.WebPartDisplayName = updateWebpart.WebPartDisplayName.ToLower();

            // Save the changes
            WebPartInfoProvider.SetWebPartInfo(updateWebpart);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates web parts. Called when the "Get and bulk update parts" button is pressed.
    /// Expects the CreateWebPart method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateWebParts()
    {
        // Prepare the parameters
        string where = "WebPartName LIKE N'MyNewWebpart%'";

        // Get the data
        DataSet webparts = WebPartInfoProvider.GetWebParts().Where(where);
        if (!DataHelper.DataSourceIsEmpty(webparts))
        {
            // Loop through the individual items
            foreach (DataRow webpartDr in webparts.Tables[0].Rows)
            {
                // Create object from DataRow
                WebPartInfo modifyWebpart = new WebPartInfo(webpartDr);

                // Update the properties
                modifyWebpart.WebPartDisplayName = modifyWebpart.WebPartDisplayName.ToUpper();

                // Save the changes
                WebPartInfoProvider.SetWebPartInfo(modifyWebpart);
            }

            return true;
        }

        return false;
    }


    /// <summary>
    /// Deletes web part. Called when the "Delete part" button is pressed.
    /// Expects the CreateWebPart method to be run first.
    /// </summary>
    private bool DeleteWebPart()
    {
        // Get the web part
        WebPartInfo deleteWebpart = WebPartInfoProvider.GetWebPartInfo("MyNewWebpart");

        // Delete the web part
        WebPartInfoProvider.DeleteWebPartInfo(deleteWebpart);

        return (deleteWebpart != null);
    }

    #endregion


    #region "API examples - Web part layout"

    /// <summary>
    /// Creates web part layout. Called when the "Create layout" button is pressed.
    /// </summary>
    private bool CreateWebPartLayout()
    {
        // Get the web part
        WebPartInfo webpart = WebPartInfoProvider.GetWebPartInfo("MyNewWebpart");
        if (webpart != null)
        {
            // Create new web part layout object
            WebPartLayoutInfo newLayout = new WebPartLayoutInfo();

            // Set the properties
            newLayout.WebPartLayoutDisplayName = "My new layout";
            newLayout.WebPartLayoutCodeName = "MyNewLayout";
            newLayout.WebPartLayoutWebPartID = webpart.WebPartID;
            newLayout.WebPartLayoutCode = "This is the new layout of MyNewWebpart webpart.";

            // Save the web part layout
            WebPartLayoutInfoProvider.SetWebPartLayoutInfo(newLayout);

            return true;
        }
        return false;
    }


    /// <summary>
    /// Gets and updates web part layout. Called when the "Get and update layout" button is pressed.
    /// Expects the CreateWebPartLayout method to be run first.
    /// </summary>
    private bool GetAndUpdateWebPartLayout()
    {
        // Get the web part layout
        WebPartLayoutInfo updateLayout = WebPartLayoutInfoProvider.GetWebPartLayoutInfo("MyNewWebpart", "MyNewLayout");
        if (updateLayout != null)
        {
            // Update the properties
            updateLayout.WebPartLayoutDisplayName = updateLayout.WebPartLayoutDisplayName.ToLower();

            // Save the changes
            WebPartLayoutInfoProvider.SetWebPartLayoutInfo(updateLayout);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates web part layouts. Called when the "Get and bulk update layouts" button is pressed.
    /// Expects the CreateWebPartLayout method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateWebPartLayouts()
    {
        // Prepare the parameters
        string where = "WebPartLayoutCodeName LIKE N'MyNewLayout%'";

        // Get the data
        DataSet layouts = WebPartLayoutInfoProvider.GetWebPartLayouts().Where(where);
        if (!DataHelper.DataSourceIsEmpty(layouts))
        {
            // Loop through the individual items
            foreach (DataRow layoutDr in layouts.Tables[0].Rows)
            {
                // Create object from DataRow
                WebPartLayoutInfo modifyLayout = new WebPartLayoutInfo(layoutDr);

                // Update the properties
                modifyLayout.WebPartLayoutDisplayName = modifyLayout.WebPartLayoutDisplayName.ToUpper();

                // Save the changes
                WebPartLayoutInfoProvider.SetWebPartLayoutInfo(modifyLayout);
            }

            return true;
        }

        return false;
    }


    /// <summary>
    /// Deletes web part layout. Called when the "Delete layout" button is pressed.
    /// Expects the CreateWebPartLayout method to be run first.
    /// </summary>
    private bool DeleteWebPartLayout()
    {
        // Get the web part layout
        WebPartLayoutInfo deleteLayout = WebPartLayoutInfoProvider.GetWebPartLayoutInfo("MyNewWebpart", "MyNewLayout");

        // Delete the web part layout
        WebPartLayoutInfoProvider.DeleteWebPartLayoutInfo(deleteLayout);

        return (deleteLayout != null);
    }

    #endregion


    #region "API examples - Web part container"

    /// <summary>
    /// Creates web part container. Called when the "Create container" button is pressed.
    /// </summary>
    private bool CreateWebPartContainer()
    {
        // Create new web part container object
        WebPartContainerInfo newContainer = new WebPartContainerInfo();

        // Set the properties
        newContainer.ContainerDisplayName = "My new container";
        newContainer.ContainerName = "MyNewContainer";
        newContainer.ContainerTextBefore = "<div class=\"myNewContainer\">";
        newContainer.ContainerTextAfter = "</div>";

        // Save the web part container
        WebPartContainerInfoProvider.SetWebPartContainerInfo(newContainer);

        return true;
    }


    /// <summary>
    /// Gets and updates web part container. Called when the "Get and update container" button is pressed.
    /// Expects the CreateWebPartContainer method to be run first.
    /// </summary>
    private bool GetAndUpdateWebPartContainer()
    {
        // Get the web part container
        WebPartContainerInfo updateContainer = WebPartContainerInfoProvider.GetWebPartContainerInfo("MyNewContainer");
        if (updateContainer != null)
        {
            // Update the properties
            updateContainer.ContainerDisplayName = updateContainer.ContainerDisplayName.ToLower();

            // Save the changes
            WebPartContainerInfoProvider.SetWebPartContainerInfo(updateContainer);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates web part containers. Called when the "Get and bulk update containers" button is pressed.
    /// Expects the CreateWebPartContainer method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateWebPartContainers()
    {
        // Prepare the parameters
        string where = "ContainerName LIKE N'MyNewContainer%'";
        string orderBy = "";
        int topN = 0;
        string columns = "";

        // Get the data
        DataSet containers = WebPartContainerInfoProvider.GetContainers()
            .Where(where)
            .Columns(columns)
            .TopN(topN)
            .OrderBy(orderBy);

        if (!DataHelper.DataSourceIsEmpty(containers))
        {
            // Loop through the individual items
            foreach (DataRow containerDr in containers.Tables[0].Rows)
            {
                // Create object from DataRow
                WebPartContainerInfo modifyContainer = new WebPartContainerInfo(containerDr);

                // Update the properties
                modifyContainer.ContainerDisplayName = modifyContainer.ContainerDisplayName.ToUpper();

                // Save the changes
                WebPartContainerInfoProvider.SetWebPartContainerInfo(modifyContainer);
            }

            return true;
        }

        return false;
    }


    /// <summary>
    /// Deletes web part container. Called when the "Delete container" button is pressed.
    /// Expects the CreateWebPartContainer method to be run first.
    /// </summary>
    private bool DeleteWebPartContainer()
    {
        // Get the web part container
        WebPartContainerInfo deleteContainer = WebPartContainerInfoProvider.GetWebPartContainerInfo("MyNewContainer");

        // Delete the web part container
        WebPartContainerInfoProvider.DeleteWebPartContainerInfo(deleteContainer);

        return (deleteContainer != null);
    }

    #endregion


    #region "API examples - Web part container on site"

    /// <summary>
    /// Adds web part container to site. Called when the "Add container to site" button is pressed.
    /// Expects the CreateWebPartContainer method to be run first.
    /// </summary>
    private bool AddWebPartContainerToSite()
    {
        // Get the web part container
        WebPartContainerInfo container = WebPartContainerInfoProvider.GetWebPartContainerInfo("MyNewContainer");
        if (container != null)
        {
            int containerId = container.ContainerID;
            int siteId = SiteContext.CurrentSiteID;

            // Save the binding
            WebPartContainerSiteInfoProvider.AddContainerToSite(containerId, siteId);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Removes web part container from site. Called when the "Remove container from site" button is pressed.
    /// Expects the AddWebPartContainerToSite method to be run first.
    /// </summary>
    private bool RemoveWebPartContainerFromSite()
    {
        // Get the web part container
        WebPartContainerInfo removeContainer = WebPartContainerInfoProvider.GetWebPartContainerInfo("MyNewContainer");
        if (removeContainer != null)
        {
            int siteId = SiteContext.CurrentSiteID;

            // Delete the binding
            WebPartContainerSiteInfoProvider.RemoveContainerFromSite(removeContainer.ContainerID, siteId);

            return true;
        }

        return false;
    }

    #endregion
}