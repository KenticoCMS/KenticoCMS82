using System;
using System.Data;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.Localization;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;
using CMS.WorkflowEngine;

public partial class CMSAPIExamples_Code_Documents_Workflow_Basics_Default : CMSAPIExamplePage
{
    #region "Initialization"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Creating documents
        apiCreateExampleObjects.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateExampleObjects);
        apiCreateDocument.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateDocument);
        apiCreateNewCultureVersion.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateNewCultureVersion);
        apiCreateLinkedDocument.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateLinkedDocument);

        // Managing documents
        apiGetAndUpdateDocuments.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndUpdateDocuments);
        apiCopyDocument.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CopyDocument);
        apiMoveDocument.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(MoveDocument);

        // Cleanup
        apiDeleteDocuments.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteDocuments);
        apiDeleteObjects.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteObjects);
    }

    #endregion


    #region "Mass actions"

    /// <summary>
    /// Runs all creating and managing examples.
    /// </summary>
    public override void RunAll()
    {
        base.RunAll();

        // Creating documents
        apiCreateExampleObjects.Run();
        apiCreateDocument.Run();
        apiCreateNewCultureVersion.Run();
        apiCreateLinkedDocument.Run();

        // Managing documents
        apiGetAndUpdateDocuments.Run();
        apiCopyDocument.Run();
        apiMoveDocument.Run();
    }


    /// <summary>
    /// Runs all cleanup examples.
    /// </summary>
    public override void CleanUpAll()
    {
        base.CleanUpAll();

        apiDeleteDocuments.Run();
        apiDeleteObjects.Run();
    }

    #endregion


    #region "API examples - Creating documents"

    /// <summary>
    /// Assigns another culture to the current site, then creates the document structure and workflow scope needed for this example. Called when the "Create example objects" button is pressed.
    /// </summary>
    private bool CreateExampleObjects()
    {
        // Add a new culture to the current site
        CultureInfo culture = CultureInfoProvider.GetCultureInfo("de-de");
        CultureSiteInfoProvider.AddCultureToSite(culture.CultureID, SiteContext.CurrentSiteID);

        // Create a new tree provider
        TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);

        // Get the root node
        TreeNode parent = tree.SelectSingleNode(SiteContext.CurrentSiteName, "/", "en-us");

        if (parent != null)
        {
            // Create the API example folder
            TreeNode node = TreeNode.New("CMS.Folder", tree);

            node.DocumentName = "API Example";
            node.DocumentCulture = "en-us";

            // Insert it to database
            DocumentHelper.InsertDocument(node, parent, tree);

            parent = node;

            // Create the Source folder for moving
            node = TreeNode.New("CMS.Folder", tree);

            node.DocumentName = "Source";
            node.DocumentCulture = "en-us";

            DocumentHelper.InsertDocument(node, parent, tree);

            // Create the Target folder for moving
            node = TreeNode.New("CMS.Folder", tree);

            node.DocumentName = "Target";
            node.DocumentCulture = "en-us";

            DocumentHelper.InsertDocument(node, parent, tree);

            // Get the default workflow
            WorkflowInfo workflow = WorkflowInfoProvider.GetWorkflowInfo("default");

            if (workflow != null)
            {
                // Get the example folder data
                node = DocumentHelper.GetDocument(parent, tree);

                // Create new workflow scope
                WorkflowScopeInfo scope = new WorkflowScopeInfo();

                // Assign to the default workflow and current site and set starting alias path to the example document
                scope.ScopeWorkflowID = workflow.WorkflowID;
                scope.ScopeStartingPath = node.NodeAliasPath;
                scope.ScopeSiteID = SiteContext.CurrentSiteID;

                // Save the scope into the database
                WorkflowScopeInfoProvider.SetWorkflowScopeInfo(scope);

                return true;
            }
            else
            {
                apiCreateExampleObjects.ErrorMessage = "The default workflow was not found.";
            }
        }

        return false;
    }


    /// <summary>
    /// Creates a document under workflow. Called when the "Create document" button is pressed.
    /// Expects the "CreateExampleObjects" method to be run first.
    /// </summary>
    private bool CreateDocument()
    {
        // Create new tree provider
        TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);

        // Prepare parameters
        string siteName = SiteContext.CurrentSiteName;
        string aliasPath = "/API-Example";
        string culture = "en-us";
        bool combineWithDefaultCulture = false;
        string classNames = TreeProvider.ALL_CLASSNAMES;
        string where = null;
        string orderBy = null;
        int maxRelativeLevel = -1;
        bool selectOnlyPublished = false;
        string columns = null;

        // Get the example folder
        TreeNode parentNode = DocumentHelper.GetDocument(siteName, aliasPath, culture, combineWithDefaultCulture, classNames, where, orderBy, maxRelativeLevel, selectOnlyPublished, columns, tree);

        if (parentNode != null)
        {
            // Create a new node
            TreeNode node = TreeNode.New("CMS.MenuItem", tree);

            // Set the required document properties
            node.DocumentName = "My new page";
            node.DocumentCulture = "en-us";

            // Insert the document
            DocumentHelper.InsertDocument(node, parentNode, tree);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Creates a new culture version of the document under workflow. Called when the "Create new culture version" button is pressed.
    /// Expects the "CreateExampleObjects" and "CreateDocument" methods to be run first.
    /// </summary>
    private bool CreateNewCultureVersion()
    {
        // Create an instance of the Tree provider first
        TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);

        // Prepare parameters
        string siteName = SiteContext.CurrentSiteName;
        string aliasPath = "/API-Example/My-new-page";
        string culture = "en-us";
        bool combineWithDefaultCulture = false;
        string classNames = TreeProvider.ALL_CLASSNAMES;
        string where = null;
        string orderBy = null;
        int maxRelativeLevel = -1;
        bool selectOnlyPublished = false;
        string columns = null;

        // Get the document in the English culture
        TreeNode node = DocumentHelper.GetDocument(siteName, aliasPath, culture, combineWithDefaultCulture, classNames, where, orderBy, maxRelativeLevel, selectOnlyPublished, columns, tree);

        if (node != null)
        {
            // Translate its name
            node.DocumentName = "My new translation";
            node.SetValue("MenuItemName", "My new translation");

            // Insert into database
            DocumentHelper.InsertNewCultureVersion(node, tree, "de-de");

            return true;
        }

        return false;
    }


    /// <summary>
    /// Creates a link to the document under workflow. Called when the "Create linked document" button is pressed.
    /// Expects the "CreateExampleObjects" and "CreateDocument" methods to be run first.
    /// </summary>
    private bool CreateLinkedDocument()
    {
        // Create new tree provider
        TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);

        // Prepare parameters
        string siteName = SiteContext.CurrentSiteName;
        string aliasPath = "/API-Example";
        string culture = "en-us";
        bool combineWithDefaultCulture = false;
        string classNames = TreeProvider.ALL_CLASSNAMES;
        string where = null;
        string orderBy = null;
        int maxRelativeLevel = -1;
        bool selectOnlyPublished = false;
        string columns = null;

        // Get the example folder
        TreeNode parentNode = DocumentHelper.GetDocument(siteName, aliasPath, culture, combineWithDefaultCulture, classNames, where, orderBy, maxRelativeLevel, selectOnlyPublished, columns, tree);

        if (parentNode != null)
        {
            // Change the alias path
            aliasPath += "/My-new-page";

            // Get the original document
            TreeNode node = DocumentHelper.GetDocument(siteName, aliasPath, culture, combineWithDefaultCulture, classNames, where, orderBy, maxRelativeLevel, selectOnlyPublished, columns, tree);

            if (node != null)
            {
                // Insert the link
                DocumentHelper.InsertDocumentAsLink(node, parentNode, tree);

                return true;
            }
            else
            {
                apiCreateLinkedDocument.ErrorMessage = "Page 'My new page' was not found.";
            }
        }

        return false;
    }

    #endregion


    #region "API examples - Managing documents"

    /// <summary>
    /// Gets a dataset of documents in the example section and updates them. Called when the "Get and update documents" button is pressed.
    /// Expects the "CreateExampleObjects" and "CreateDocument" methods to be run first.
    /// </summary>
    private bool GetAndUpdateDocuments()
    {
        // Create an instance of the Tree provider first
        TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);

        // Prepare parameters
        string siteName = SiteContext.CurrentSiteName;
        string aliasPath = "/API-Example/%";
        string culture = "en-us";
        bool combineWithDefaultCulture = false;
        string classNames = "CMS.MenuItem";
        string where = null;
        string orderBy = null;
        int maxRelativeLevel = -1;
        bool selectOnlyPublished = false;

        // Fill dataset with documents
        DataSet documents = DocumentHelper.GetDocuments(siteName, aliasPath, culture, combineWithDefaultCulture, classNames, where, orderBy, maxRelativeLevel, selectOnlyPublished, tree);

        if (!DataHelper.DataSourceIsEmpty(documents))
        {
            // Create a new Version manager instance
            VersionManager manager = VersionManager.GetInstance(tree);

            // Loop through all documents
            foreach (DataRow documentRow in documents.Tables[0].Rows)
            {
                // Create a new Tree node from the data row
                TreeNode editDocument = TreeNode.New("CMS.MenuItem", documentRow, tree);

                // Check out the document
                manager.CheckOut(editDocument);

                string newName = editDocument.DocumentName.ToLower();

                // Change document data
                editDocument.DocumentName = newName;

                // Change coupled data
                editDocument.SetValue("MenuItemName", newName);

                // Save the changes
                DocumentHelper.UpdateDocument(editDocument, tree);

                // Check in the document
                manager.CheckIn(editDocument, null, null);
            }

            return true;
        }

        return false;
    }


    /// <summary>
    /// Copies the document under workflow to a different section. Called when the "Copy document" button is pressd.
    /// Expects the "CreateExampleObjects" and "CreateDocument" methods to be run first.
    /// </summary>
    private bool CopyDocument()
    {
        // Create an instance of the Tree provider first
        TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);

        // Prepare parameters
        string siteName = SiteContext.CurrentSiteName;
        string aliasPath = "/API-Example/My-new-page";
        string culture = "en-us";
        bool combineWithDefaultCulture = false;
        string classNames = TreeProvider.ALL_CLASSNAMES;
        string where = null;
        string orderBy = null;
        int maxRelativeLevel = -1;
        bool selectOnlyPublished = false;
        string columns = null;

        // Get the example folder
        TreeNode node = DocumentHelper.GetDocument(siteName, aliasPath, culture, combineWithDefaultCulture, classNames, where, orderBy, maxRelativeLevel, selectOnlyPublished, columns, tree);

        aliasPath = "/API-Example/Source";

        // Get the new parent document
        TreeNode parentNode = DocumentHelper.GetDocument(siteName, aliasPath, culture, combineWithDefaultCulture, classNames, where, orderBy, maxRelativeLevel, selectOnlyPublished, columns, tree);

        if ((node != null) && (parentNode != null))
        {
            // Copy the document
            DocumentHelper.CopyDocument(node, parentNode, false, tree);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Moves the document under workflow to a different section. Called when the "Move document" button is pressed.
    /// Expects the "CreateExampleObjects", "CreateDocument" and "CopyDocument" methods to be run first.
    /// </summary>
    private bool MoveDocument()
    {
        // Create an instance of the Tree provider first
        TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);

        // Prepare parameters
        string siteName = SiteContext.CurrentSiteName;
        string aliasPath = "/API-Example/My-new-page";
        string culture = "en-us";
        bool combineWithDefaultCulture = false;
        string classNames = TreeProvider.ALL_CLASSNAMES;
        string where = null;
        string orderBy = null;
        int maxRelativeLevel = -1;
        bool selectOnlyPublished = false;
        string columns = null;

        // Get the example folder
        TreeNode node = DocumentHelper.GetDocument(siteName, aliasPath, culture, combineWithDefaultCulture, classNames, where, orderBy, maxRelativeLevel, selectOnlyPublished, columns, tree);

        aliasPath = "/API-Example/Target";

        // Get the new parent document
        TreeNode parentNode = DocumentHelper.GetDocument(siteName, aliasPath, culture, combineWithDefaultCulture, classNames, where, orderBy, maxRelativeLevel, selectOnlyPublished, columns, tree);

        if ((node != null) && (parentNode != null))
        {
            // Move the document
            DocumentHelper.MoveDocument(node, parentNode, tree);

            return true;
        }

        return false;
    }

    #endregion


    #region "API examples - Cleanup"

    /// <summary>
    /// Deletes the document structure used for this example. Called when the "Delete documents" button is pressed.
    /// Expects the "CreateExampleObjects" and "CreateDocument" methods to be run first.
    /// </summary>
    private bool DeleteDocuments()
    {
        // Create new tree provider
        TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);

        // Prepare parameters
        string siteName = SiteContext.CurrentSiteName;
        string aliasPath = "/API-Example";
        string culture = "en-us";
        bool combineWithDefaultCulture = false;
        string classNames = TreeProvider.ALL_CLASSNAMES;
        string where = null;
        string orderBy = null;
        int maxRelativeLevel = -1;
        bool selectOnlyPublished = false;
        string columns = null;

        // Get the example folder
        TreeNode node = DocumentHelper.GetDocument(siteName, aliasPath, culture, combineWithDefaultCulture, classNames, where, orderBy, maxRelativeLevel, selectOnlyPublished, columns, tree);

        if (node != null)
        {
            // Prepare delete parameters
            bool deleteAllCultures = true;
            bool destroyHistory = true;
            bool deleteProduct = false;

            // Delete all culture versions of the document and destroy its version history. This method also destroys all child documents.
            DocumentHelper.DeleteDocument(node, tree, deleteAllCultures, destroyHistory, deleteProduct);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Deletes the workflow scope(s) and culture assignments used for this example. Called when the "Delete objects" button is pressed.
    /// Expects the "CreateExampleObjects" method to be run first.
    /// </summary>
    private bool DeleteObjects()
    {
        CultureInfo culture = CultureInfoProvider.GetCultureInfo("de-de");

        // Remove the example culture from the site
        CultureSiteInfoProvider.RemoveCultureFromSite(culture.CultureID, SiteContext.CurrentSiteID);

        // Prepare parameters
        string where = "ScopeStartingPath LIKE '/API-Example%'";
        string orderBy = null;
        int topN = 0;
        string columns = null;

        DataSet scopes = WorkflowScopeInfoProvider.GetWorkflowScopes(where, orderBy, topN, columns);

        if (!DataHelper.DataSourceIsEmpty(scopes))
        {
            // Loop through all the scopes in case more identical scopes were accidentally created
            foreach (DataRow scopeRow in scopes.Tables[0].Rows)
            {
                // Create scope info object
                WorkflowScopeInfo scope = new WorkflowScopeInfo(scopeRow);

                // Delete the scope
                scope.Delete();
            }

            return true;
        }

        return false;
    }

    #endregion
}