using System;
using System.Data;

using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSAPIExamples_Code_Documents_Advanced_Default : CMSAPIExamplePage
{
    #region "Initialization"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Preparatioin
        apiCreateDocumentStructure.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateDocumentStructure);

        // Organizing documents
        apiMoveDocumentUp.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(MoveDocumentUp);
        apiMoveDocumentDown.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(MoveDocumentDown);
        apiSortDocumentsAlphabetically.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(SortDocumentsAlphabetically);
        apiSortDocumentsByDate.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(SortDocumentsByDate);

        // Recycle bin
        apiMoveToRecycleBin.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(MoveToRecycleBin);
        apiRestoreFromRecycleBin.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(RestoreFromRecycleBin);

        // Cleanup
        apiDeleteDocumentStructure.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteDocumentStructure);
    }

    #endregion


    #region "Mass actions"

    /// <summary>
    /// Runs all creating and managing examples.
    /// </summary>
    public override void RunAll()
    {
        base.RunAll();

        // Preparation
        apiCreateDocumentStructure.Run();

        // Organizing documents
        apiMoveDocumentUp.Run();
        apiMoveDocumentDown.Run();
        apiSortDocumentsAlphabetically.Run();
        apiSortDocumentsByDate.Run();

        // Recycle bin
        apiMoveToRecycleBin.Run();
        apiRestoreFromRecycleBin.Run();
    }


    /// <summary>
    /// Runs all cleanup examples.
    /// </summary>
    public override void CleanUpAll()
    {
        base.CleanUpAll();

        // Delete document structure
        apiDeleteDocumentStructure.Run();
    }

    #endregion


    #region "API Example - Preparation"

    /// <summary>
    /// Prepares the example document structure. Called when the "Prepare documents" button is pressed.
    /// </summary>
    private bool CreateDocumentStructure()
    {
        // Create an instance of the Tree provider
        TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);

        // First get the root node
        TreeNode parentNode = tree.SelectSingleNode(SiteContext.CurrentSiteName, "/", "en-us");

        if (parentNode != null)
        {
            // First create a folder
            TreeNode node = TreeNode.New("CMS.Folder", tree);

            node.DocumentName = "API Example";
            node.DocumentCulture = "en-us";

            node.Insert(parentNode);

            parentNode = node;

            // Create a few documents
            for (int i = 1; i <= 3; i++)
            {
                node = TreeNode.New("CMS.MenuItem", tree);

                node.DocumentName = "Page " + i;
                node.DocumentCulture = "en-us";

                node.Insert(parentNode);
            }

            return true;
        }

        return false;
    }

    #endregion


    #region "API Example - Organizing documents"

    /// <summary>
    /// Moves a document up in the content tree. Called when the "Move document up" button is pressed.
    /// Expects the "CreateDocumentStructure" method to be run first.
    /// </summary>
    private bool MoveDocumentUp()
    {
        // Create an instance of the Tree provider
        TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);

        // Select a node
        TreeNode node = tree.SelectSingleNode(SiteContext.CurrentSiteName, "/API-Example/Page-2", "en-us");

        if (node != null)
        {
            // Move the node up
            tree.MoveNodeUp(node.NodeID);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Moves a document down in the content tree. Called when the "Move document down" button is pressed.
    /// Expects the "CreateDocumentStructure" method to be run first.
    /// </summary>
    private bool MoveDocumentDown()
    {
        // Create an instance of the Tree provider
        TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);

        // Select a node
        TreeNode node = tree.SelectSingleNode(SiteContext.CurrentSiteName, "/API-Example/Page-1", "en-us");

        if (node != null)
        {
            // Move the node up
            tree.MoveNodeDown(node.NodeID);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Sorts a content tree subsection by document name from A to Z. Called when the "Sort documents alphabetically" button is pressed.
    /// Expects the "CreateDocumentStructure" method to be run first.
    /// </summary>
    private bool SortDocumentsAlphabetically()
    {
        // Create an instance of the Tree provider
        TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);

        // Select a node
        TreeNode node = tree.SelectSingleNode(SiteContext.CurrentSiteName, "/API-Example", "en-us");

        if (node != null)
        {
            // Sort its child nodes alphabetically - ascending
            tree.SortNodesAlphabetically(node.NodeID, node.NodeSiteID, true);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Sorts a content tree subsection by date from oldest to newest. Called when the "Sort documents by date" button is pressed.
    /// Expects the "CreateDocumentStructure" method to be run first.
    /// </summary>
    private bool SortDocumentsByDate()
    {
        // Create an instance of the Tree provider
        TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);

        // Select a node
        TreeNode node = tree.SelectSingleNode(SiteContext.CurrentSiteName, "/API-Example", "en-us");

        if (node != null)
        {
            // Sort its child nodes by date - descending
            tree.SortNodesByDate(node.NodeID, node.NodeSiteID, false);

            return true;
        }

        return false;
    }

    #endregion


    #region "API Example - Recycle bin"

    /// <summary>
    /// Deletes a document to the recycle bin. Called when the "Move document to recycle bin" button is pressed.
    /// Expects the "CreateDocumentStructure" method to be run first.
    /// </summary>
    private bool MoveToRecycleBin()
    {
        // Create an instance of the Tree provider
        TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);

        // Get the document
        TreeNode node = tree.SelectSingleNode(SiteContext.CurrentSiteName, "/API-Example/Page-1", "en-us");

        if (node != null)
        {
            // Delete the document without destroying its history
            DocumentHelper.DeleteDocument(node, tree, true, false, true);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Restores the document from the recycle bin. Called when the "Restore document" button is pressed.
    /// Expects the "CreateDocumentStructure" and "MoveDocumentToRecycleBin" methods to be run first.
    /// </summary>
    private bool RestoreFromRecycleBin()
    {
        // Prepare the where condition
        string where = "VersionNodeAliasPath LIKE N'/API-Example/Page-1'";

        // Get the recycled document
        DataSet recycleBin = VersionHistoryInfoProvider.GetRecycleBin(SiteContext.CurrentSiteID, 0, where, null, 0, "VersionHistoryID");

        if (!DataHelper.DataSourceIsEmpty(recycleBin))
        {
            // Create a new version history info object from the data row
            VersionHistoryInfo version = new VersionHistoryInfo(recycleBin.Tables[0].Rows[0]);

            // Create a new version manager instance and restore the document
            VersionManager manager = VersionManager.GetInstance(new TreeProvider(MembershipContext.AuthenticatedUser));
            manager.RestoreDocument(version.VersionHistoryID);

            return true;
        }

        return false;
    }

    #endregion


    #region "API Example - Cleanup"

    /// <summary>
    /// Deletes the example document structure. Called when the "Delete document structure" button is pressed.
    /// Expects the "CreateDocumentStructure" method to be run first.
    /// </summary>    
    private bool DeleteDocumentStructure()
    {
        // Create an instance of the Tree provider
        TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);

        // Get the API Example folder
        TreeNode node = tree.SelectSingleNode(SiteContext.CurrentSiteName, "/API-Example", "en-us");

        // Delete the folder including all dependencies and child documents
        node.Delete();

        return true;
    }

    #endregion
}