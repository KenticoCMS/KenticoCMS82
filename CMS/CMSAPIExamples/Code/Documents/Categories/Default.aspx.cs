using System;
using System.Data;

using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;
using CMS.Taxonomy;

public partial class CMSAPIExamples_Code_Documents_Categories_Default : CMSAPIExamplePage
{
    #region "Initialization"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Category
        apiCreateCategory.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateCategory);
        apiGetAndUpdateCategory.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndUpdateCategory);
        apiGetAndBulkUpdateCategories.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndBulkUpdateCategories);
        apiDeleteCategory.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteCategory);

        // Subcategory
        apiCreateSubcategory.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateSubcategory);
        apiDeleteSubcategory.RunExample +=new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteSubcategory);

        // Document in category
        apiAddDocumentToCategory.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(AddDocumentToCategory);
        apiRemoveDocumentFromCategory.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(RemoveDocumentFromCategory);
    }

    #endregion


    #region "Mass actions"

    /// <summary>
    /// Runs all creating and managing examples.
    /// </summary>
    public override void RunAll()
    {
        base.RunAll();

        // Category
        apiCreateCategory.Run();
        apiGetAndUpdateCategory.Run();
        apiGetAndBulkUpdateCategories.Run();

        // Subcategory
        apiCreateSubcategory.Run();

        // Document in category
        apiAddDocumentToCategory.Run();
    }


    /// <summary>
    /// Runs all cleanup examples.
    /// </summary>
    public override void CleanUpAll()
    {
        base.CleanUpAll();

        // Document in category
        apiRemoveDocumentFromCategory.Run();

        // Subcategory
        apiDeleteSubcategory.Run();

        // Category
        apiDeleteCategory.Run();
    }

    #endregion


    #region "API examples - Category"

    /// <summary>
    /// Creates category. Called when the "Create category" button is pressed.
    /// </summary>
    private bool CreateCategory()
    {
        // Create new category object
        CategoryInfo newCategory = new CategoryInfo();

        // Set the properties
        newCategory.CategoryDisplayName = "My new category";
        newCategory.CategoryName = "MyNewCategory";
        newCategory.CategoryDescription = "My new category description";
        newCategory.CategorySiteID = SiteContext.CurrentSiteID;
        newCategory.CategoryCount = 0;
        newCategory.CategoryEnabled = true;

        // Save the category
        CategoryInfoProvider.SetCategoryInfo(newCategory);

        return true;
    }


    /// <summary>
    /// Gets and updates category. Called when the "Get and update category" button is pressed.
    /// Expects the CreateCategory method to be run first.
    /// </summary>
    private bool GetAndUpdateCategory()
    {
        // Get the category
        CategoryInfo updateCategory = CategoryInfoProvider.GetCategoryInfo("MyNewCategory", SiteContext.CurrentSiteName);
        if (updateCategory != null)
        {
            // Update the properties
            updateCategory.CategoryDisplayName = updateCategory.CategoryDisplayName.ToLower();

            // Save the changes
            CategoryInfoProvider.SetCategoryInfo(updateCategory);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates categories. Called when the "Get and bulk update categories" button is pressed.
    /// Expects the CreateCategory method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateCategories()
    {
        // Prepare the parameters
        string where = "CategoryName LIKE N'MyNewCategory%'";

        // Get the data
        DataSet categories = CategoryInfoProvider.GetCategories(where, null);
        if (!DataHelper.DataSourceIsEmpty(categories))
        {
            // Loop through the individual items
            foreach (DataRow categoryDr in categories.Tables[0].Rows)
            {
                // Create object from DataRow
                CategoryInfo modifyCategory = new CategoryInfo(categoryDr);

                // Update the properties
                modifyCategory.CategoryDisplayName = modifyCategory.CategoryDisplayName.ToUpper();

                // Save the changes
                CategoryInfoProvider.SetCategoryInfo(modifyCategory);
            }

            return true;
        }

        return false;
    }


    /// <summary>
    /// Deletes category. Called when the "Delete category" button is pressed.
    /// Expects the CreateCategory method to be run first.
    /// </summary>
    private bool DeleteCategory()
    {
        // Get the category
        CategoryInfo deleteCategory = CategoryInfoProvider.GetCategoryInfo("MyNewCategory", SiteContext.CurrentSiteName);

        // Delete the category
        CategoryInfoProvider.DeleteCategoryInfo(deleteCategory);

        return (deleteCategory != null);
    }

    #endregion


    #region "Subcategory"

    /// <summary>
    /// Creates subcategory. Called when the "Create subcategory" button is pressed.
    /// </summary>
    private bool CreateSubcategory()
    {
        // Get the parent category
        CategoryInfo parentCategory = CategoryInfoProvider.GetCategoryInfo("MyNewCategory", SiteContext.CurrentSiteName);
        if (parentCategory != null)
        {
            // Create new category object
            CategoryInfo newSubcategory = new CategoryInfo();

            // Set the properties
            newSubcategory.CategoryDisplayName = "My new subcategory";
            newSubcategory.CategoryName = "MyNewSubcategory";
            newSubcategory.CategoryDescription = "My new subcategory description";
            newSubcategory.CategorySiteID = SiteContext.CurrentSiteID;
            newSubcategory.CategoryCount = 0;
            newSubcategory.CategoryEnabled = true;
            newSubcategory.CategoryParentID = parentCategory.CategoryID;

            // Save the category
            CategoryInfoProvider.SetCategoryInfo(newSubcategory);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Deletes subcategory. Called when the "Delete subcategory" button is pressed.
    /// Expects the CreateSubcategory method to be run first.
    /// </summary>
    private bool DeleteSubcategory()
    {
        // Get the category
        CategoryInfo deleteCategory = CategoryInfoProvider.GetCategoryInfo("MyNewSubcategory", SiteContext.CurrentSiteName);

        // Delete the category
        CategoryInfoProvider.DeleteCategoryInfo(deleteCategory);

        return (deleteCategory != null);
    }

    #endregion


    #region "API examples - Document in category"

    /// <summary>
    /// Adds document to category. Called when the button "Add document from category" is pressed.
    /// </summary>
    private bool AddDocumentToCategory()
    {
        // Get the category
        CategoryInfo category = CategoryInfoProvider.GetCategoryInfo("MyNewCategory", SiteContext.CurrentSiteName);
        if (category != null)
        {
            // Get the tree structure
            TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);

            // Get the root document
            TreeNode root = tree.SelectSingleNode(SiteContext.CurrentSiteName, "/", null, true);

            // Add document to category
            DocumentCategoryInfoProvider.AddDocumentToCategory(root.DocumentID, category.CategoryID);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Removes document from category. Called when the button "Remove document from category" is pressed.
    /// Expects the method AddDocumentToCategory to be run first.
    /// </summary>
    private bool RemoveDocumentFromCategory()
    {
        // Get the category
        CategoryInfo category = CategoryInfoProvider.GetCategoryInfo("MyNewCategory", SiteContext.CurrentSiteName);
        if (category != null)
        {
            // Get the tree structure
            TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);

            // Get the root document
            TreeNode root = tree.SelectSingleNode(SiteContext.CurrentSiteName, "/", null, true);

            // Get the document category relationship
            DocumentCategoryInfo documentCategory = DocumentCategoryInfoProvider.GetDocumentCategoryInfo(root.DocumentID, category.CategoryID);

            if (documentCategory != null)
            {
                // Remove document from category
                DocumentCategoryInfoProvider.DeleteDocumentCategoryInfo(documentCategory);

                return true;
            }
        }

        return false;
    }

    #endregion
}