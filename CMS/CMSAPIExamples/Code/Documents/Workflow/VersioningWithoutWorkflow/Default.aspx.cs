using System;
using System.Data;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;
using CMS.WorkflowEngine;

[Title(Text = "Versioning without workflow (no check-in/check-out support)")]
public partial class CMSAPIExamples_Code_Documents_Workflow_VersioningWithoutWorkflow_Default : CMSAPIExamplePage
{
    #region "Initialization"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Creating example objects
        apiCreateExampleFolder.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateExampleFolder);
        apiCreateWorkflowScope.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateWorkflowScope);

        // Managing documents
        apiCreateDocument.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateDocument);
        apiUpdateDocument.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(UpdateDocument);

        // Cleanup
        apiDeleteWorkflowScope.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteWorkflowScope);
        apiDeleteDocuments.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteDocuments);
    }

    #endregion


    #region "Mass actions"

    /// <summary>
    /// Runs all creating and managing examples.
    /// </summary>
    public override void RunAll()
    {
        base.RunAll();

        // Creating example objects
        apiCreateExampleFolder.Run();
        apiCreateWorkflowScope.Run();

        // Managing documents
        apiCreateDocument.Run();
        apiUpdateDocument.Run();
    }


    /// <summary>
    /// Runs all cleanup examples.
    /// </summary>
    public override void CleanUpAll()
    {
        base.CleanUpAll();

        apiDeleteWorkflowScope.Run();
        apiDeleteDocuments.Run();
    }

    #endregion


    #region "API examples - Creating example objects"

    /// <summary>
    /// Creates the document structure needed for this example. Called when the "Create example folder" button is pressed.
    /// </summary>
    private bool CreateExampleFolder()
    {
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

            return true;
        }

        return false;
    }

    /// <summary>
    /// Gets the default workflow "Versioning without workflow" and creates new workflow scope.
    /// Called when the "Create scope" button is pressed.
    /// </summary>
    private bool CreateWorkflowScope()
    {
        WorkflowInfo workflow = WorkflowInfoProvider.GetWorkflowInfo("versioningWithoutWorkflow");

        if (workflow != null)
        {
            // Create new workflow scope and set its properties
            WorkflowScopeInfo scope = new WorkflowScopeInfo()
            {
                ScopeWorkflowID = workflow.WorkflowID,
                ScopeStartingPath = "/API-Example/%",
                ScopeSiteID = SiteContext.CurrentSiteID
            };

            // Save the scope into the database
            WorkflowScopeInfoProvider.SetWorkflowScopeInfo(scope);

            return true;
        }

        return false;
    }

    #endregion


    #region "API examples - Managing documents"

    /// <summary>
    /// Creates new document under API Example folder and applies workflow. Called when the "Create document" button is pressed.
    /// Expects the "CreateExampleFolder" and "CreateWorkflowScope" methods to be run first.
    /// </summary>
    private bool CreateDocument()
    {
        // Get the "Versioning without workflow" workflow
        WorkflowInfo workflow = WorkflowInfoProvider.GetWorkflowInfo("versioningWithoutWorkflow");

        if (workflow != null)
        {
            // Workflow is configured to automatically publish changes
            if (workflow.WorkflowAutoPublishChanges)
            {
                // Workflow doesn't use check-in/check-out
                if (!workflow.UseCheckInCheckOut(CurrentSiteName))
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

                        // Document must be published manually if check-in/check-out is not used
                        node.MoveToPublishedStep();

                        return true;
                    }
                    else
                    {
                        apiCreateDocument.ErrorMessage = "Example folder was not found";
                    }
                }
                else
                {
                    apiCreateDocument.ErrorMessage = "Workflow 'Versioning without workflow' uses check-in/check-out. You must disable it before you run this example.";
                }
            }
            else
            {
                apiCreateDocument.ErrorMessage = "Workflow 'Versioning without workflow' is not configured to automatically publish changes.";
            }
        }

        return false;
    }


    /// <summary>
    /// Updates the document under workflow. Called when the "Update document" button is pressed.
    /// Expects the "CreateDocument" method to be run first.
    /// </summary>
    private bool UpdateDocument()
    {
        // Get the "Versioning without workflow" workflow
        WorkflowInfo workflow = WorkflowInfoProvider.GetWorkflowInfo("versioningWithoutWorkflow");

        if (workflow != null)
        {
            // Workflow is configured to automatically publish changes
            if (workflow.WorkflowAutoPublishChanges)
            {
                // Workflow doesn't use check-in/check-out
                if (!workflow.UseCheckInCheckOut(CurrentSiteName))
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

                    // Get the document "My new document"
                    TreeNode node = DocumentHelper.GetDocument(siteName, aliasPath, culture, combineWithDefaultCulture, classNames, where, orderBy, maxRelativeLevel, selectOnlyPublished, columns, tree);

                    if (node != null)
                    {
                        // Document must be checked-out manually if check-in/check-out is not used
                        node.CheckOut();

                        // Update document
                        node.DocumentMenuCaption = "My new page menu";
                        DocumentHelper.UpdateDocument(node);

                        // Document must be checked-in manually in order to create new version
                        node.CheckIn();

                        return true;
                    }
                    else
                    {
                        apiUpdateDocument.ErrorMessage = "Page 'My new page' was not found.";
                    }
                }
                else
                {
                    apiUpdateDocument.ErrorMessage = "Workflow 'Versioning without workflow' uses check-in/check-out. You must disable it before you run this example.";
                }
            }
            else
            {
                apiUpdateDocument.ErrorMessage = "Workflow 'Versioning without workflow' is not configured to automatically publish changes.";
            }
        }

        return false;
    }

    #endregion


    #region "API examples - Cleanup"

    /// <summary>
    /// Deletes the workflow scope(s). Called when the "Delete workflow scope" button is pressed.
    /// Expects the "CreateWorkflowScope" method to be run first.
    /// </summary>
    private bool DeleteWorkflowScope()
    {
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

    /// <summary>
    /// Deletes the document structure used for this example. Called when the "Delete documents" button is pressed.
    /// Expects the "CreateExampleFolder" method to be run first.
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

    #endregion
}