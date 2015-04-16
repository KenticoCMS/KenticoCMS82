using System;
using System.Data;

using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSAPIExamples_Code_Documents_DocumentAliases_Default : CMSAPIExamplePage
{
    #region "Initialization"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Document alias
        apiCreateDocumentAlias.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateDocumentAlias);
        apiGetAndUpdateDocumentAlias.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndUpdateDocumentAlias);
        apiGetAndBulkUpdateDocumentAliases.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndBulkUpdateDocumentAliases);
        apiDeleteDocumentAlias.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteDocumentAlias);
    }

    #endregion


    #region "Mass actions"

    /// <summary>
    /// Runs all creating and managing examples.
    /// </summary>
    public override void RunAll()
    {
        base.RunAll();

        // Document alias
        apiCreateDocumentAlias.Run();
        apiGetAndUpdateDocumentAlias.Run();
        apiGetAndBulkUpdateDocumentAliases.Run();
    }


    /// <summary>
    /// Runs all cleanup examples.
    /// </summary>
    public override void CleanUpAll()
    {
        base.CleanUpAll();

        // Document alias
        apiDeleteDocumentAlias.Run();
    }

    #endregion


    #region "API examples - Document alias"

    /// <summary>
    /// Creates document alias. Called when the "Create alias" button is pressed.
    /// </summary>
    private bool CreateDocumentAlias()
    {
        // Get "Home" document
        TreeNode document = TreeHelper.GetDocument(SiteContext.CurrentSiteName, "/Home", CultureHelper.GetPreferredCulture(), true, "CMS.MenuItem", false);

        if (document != null)
        {
            // Create new document alias object
            DocumentAliasInfo newAlias = new DocumentAliasInfo();

            // Set the properties
            newAlias.AliasURLPath = "/MyNewAlias";
            newAlias.AliasNodeID = document.NodeID;
            newAlias.AliasSiteID = SiteContext.CurrentSiteID;

            // Save the document alias
            DocumentAliasInfoProvider.SetDocumentAliasInfo(newAlias, SiteContext.CurrentSiteName);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and updates document alias. Called when the "Get and update alias" button is pressed.
    /// Expects the CreateDocumentAlias method to be run first.
    /// </summary>
    private bool GetAndUpdateDocumentAlias()
    {
        // Prepare the parameters
        string orderBy = "";
        string where = "AliasURLPath = N'/MyNewAlias'";

        // Get the data
        DataSet aliases = DocumentAliasInfoProvider.GetDocumentAliases(where, orderBy);
        if (!DataHelper.DataSourceIsEmpty(aliases))
        {
            DocumentAliasInfo updateAlias = new DocumentAliasInfo(aliases.Tables[0].Rows[0]);

            // Update the properties
            updateAlias.AliasURLPath = updateAlias.AliasURLPath.ToLower();

            // Save the changes
            DocumentAliasInfoProvider.SetDocumentAliasInfo(updateAlias, SiteContext.CurrentSiteName);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates document aliases. Called when the "Get and bulk update aliases" button is pressed.
    /// Expects the CreateDocumentAlias method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateDocumentAliases()
    {
        // Prepare the parameters
        string orderBy = "";
        string where = "AliasURLPath = N'/MyNewAlias'";

        // Get the data
        DataSet aliases = DocumentAliasInfoProvider.GetDocumentAliases(where, orderBy);
        if (!DataHelper.DataSourceIsEmpty(aliases))
        {
            // Loop through the individual items
            foreach (DataRow aliasDr in aliases.Tables[0].Rows)
            {
                // Create object from DataRow
                DocumentAliasInfo modifyAlias = new DocumentAliasInfo(aliasDr);

                // Update the properties
                modifyAlias.AliasURLPath = modifyAlias.AliasURLPath.ToUpper();

                // Save the changes
                DocumentAliasInfoProvider.SetDocumentAliasInfo(modifyAlias, SiteContext.CurrentSiteName);
            }

            return true;
        }

        return false;
    }


    /// <summary>
    /// Deletes document alias. Called when the "Delete alias" button is pressed.
    /// Expects the CreateDocumentAlias method to be run first.
    /// </summary>
    private bool DeleteDocumentAlias()
    {
        // Prepare the parameters
        string orderBy = "";
        string where = "AliasURLPath = N'/MyNewAlias'";

        // Get the data
        DataSet aliases = DocumentAliasInfoProvider.GetDocumentAliases(where, orderBy);
        if (!DataHelper.DataSourceIsEmpty(aliases))
        {
            DocumentAliasInfo deleteAlias = new DocumentAliasInfo(aliases.Tables[0].Rows[0]);

            // Delete the document alias
            DocumentAliasInfoProvider.DeleteDocumentAliasInfo(deleteAlias);

            return true;
        }

        return false;
    }

    #endregion
}