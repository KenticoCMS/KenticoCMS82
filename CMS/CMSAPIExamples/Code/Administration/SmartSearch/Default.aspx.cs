using System;
using System.Data;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.Base;
using CMS.Localization;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;
using CMS.DataEngine;
using CMS.Search;

public partial class CMSAPIExamples_Code_Administration_SmartSearch_Default : CMSAPIExamplePage
{
    #region "Initialization"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Search index
        apiCreateSearchIndex.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateSearchIndex);
        apiGetAndUpdateSearchIndex.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndUpdateSearchIndex);
        apiGetAndBulkUpdateSearchIndexes.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndBulkUpdateSearchIndexes);
        apiDeleteSearchIndex.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteSearchIndex);
        apiCreateIndexSettings.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateIndexSettings);

        // Search index on site
        apiAddSearchIndexToSite.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(AddSearchIndexToSite);
        apiRemoveSearchIndexFromSite.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(RemoveSearchIndexFromSite);

        // Culture on search index
        apiAddCultureToSearchIndex.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(AddCultureToSearchIndex);
        apiRemoveCultureFromSearchIndex.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(RemoveCultureFromSearchIndex);

        // Search actions
        apiRebuildIndex.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(RebuildIndex);
        apiSearchText.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(SearchText);
        apiUpdateIndex.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(UpdateIndex);
    }

    #endregion


    #region "Mass actions"

    /// <summary>
    /// Runs all creating and managing examples.
    /// </summary>
    public override void RunAll()
    {
        base.RunAll();

        // Search index
        apiCreateSearchIndex.Run();
        apiGetAndUpdateSearchIndex.Run();
        apiGetAndBulkUpdateSearchIndexes.Run();
        apiCreateIndexSettings.Run();

        // Search index on site
        apiAddSearchIndexToSite.Run();

        // Culture on search index
        apiAddCultureToSearchIndex.Run();

        // Search actions
        apiRebuildIndex.Run();
        apiSearchText.Run();
        apiUpdateIndex.Run();
    }


    /// <summary>
    /// Runs all cleanup examples.
    /// </summary>
    public override void CleanUpAll()
    {
        base.CleanUpAll();

        // Culture on search index\
        apiRemoveCultureFromSearchIndex.Run();

        // Search index on site
        apiRemoveSearchIndexFromSite.Run();

        // Search index
        apiDeleteSearchIndex.Run();
    }

    #endregion


    #region "API examples - Search index"

    /// <summary>
    /// Creates search index. Called when the "Create index" button is pressed.
    /// </summary>
    private bool CreateSearchIndex()
    {
        // Create new search index object
        SearchIndexInfo newIndex = new SearchIndexInfo();

        // Set the properties
        newIndex.IndexDisplayName = "My new index";
        newIndex.IndexName = "MyNewIndex";
        newIndex.IndexIsCommunityGroup = false;
        newIndex.IndexType = TreeNode.OBJECT_TYPE;
        newIndex.IndexAnalyzerType = SearchAnalyzerTypeEnum.StandardAnalyzer;
        newIndex.StopWordsFile = "";

        // Save the search index
        SearchIndexInfoProvider.SetSearchIndexInfo(newIndex);

        return true;
    }


    /// <summary>
    /// Gets and updates search index. Called when the "Get and update index" button is pressed.
    /// Expects the CreateSearchIndex method to be run first.
    /// </summary>
    private bool GetAndUpdateSearchIndex()
    {
        // Get the search index
        SearchIndexInfo updateIndex = SearchIndexInfoProvider.GetSearchIndexInfo("MyNewIndex");
        if (updateIndex != null)
        {
            // Update the properties
            updateIndex.IndexDisplayName = updateIndex.IndexDisplayName.ToLowerCSafe();

            // Save the changes
            SearchIndexInfoProvider.SetSearchIndexInfo(updateIndex);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates search indexes. Called when the "Get and bulk update indexes" button is pressed.
    /// Expects the CreateSearchIndex method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateSearchIndexes()
    {
        // Prepare the parameters
        string where = "IndexName LIKE N'MyNewIndex%'";

        // Get the data
        DataSet indexes = SearchIndexInfoProvider.GetSearchIndexes(where, null);
        if (!DataHelper.DataSourceIsEmpty(indexes))
        {
            // Loop through the individual items
            foreach (DataRow indexDr in indexes.Tables[0].Rows)
            {
                // Create object from DataRow
                SearchIndexInfo modifyIndex = new SearchIndexInfo(indexDr);

                // Update the properties
                modifyIndex.IndexDisplayName = modifyIndex.IndexDisplayName.ToUpper();

                // Save the changes
                SearchIndexInfoProvider.SetSearchIndexInfo(modifyIndex);
            }

            return true;
        }

        return false;
    }


    /// <summary>
    /// Creates index settings. Called when the "Create index settings" button is pressed.
    /// Expects the CreateSearchIndex method to be run first.
    /// </summary>
    private bool CreateIndexSettings()
    {
        // Get the search index
        SearchIndexInfo index = SearchIndexInfoProvider.GetSearchIndexInfo("MyNewIndex");
        if (index != null)
        {
            // Create new index settings
            SearchIndexSettingsInfo indexSettings = new SearchIndexSettingsInfo();
            // Set setting properties
            indexSettings.IncludeBlogs = true;
            indexSettings.IncludeForums = true;
            indexSettings.IncludeMessageCommunication = true;
            indexSettings.ClassNames = ""; // for all document types
            indexSettings.Path = "/%";
            indexSettings.Type = SearchIndexSettingsInfo.TYPE_ALLOWED;
            indexSettings.ID = Guid.NewGuid();

            // Save index settings                     
            SearchIndexSettings settings = new SearchIndexSettings();
            settings.SetSearchIndexSettingsInfo(indexSettings);
            index.IndexSettings = settings;

            // Save to database
            SearchIndexInfoProvider.SetSearchIndexInfo(index);

            return true;
        }
        return false;
    }


    /// <summary>
    /// Deletes search index. Called when the "Delete index" button is pressed.
    /// Expects the CreateSearchIndex method to be run first.
    /// </summary>
    private bool DeleteSearchIndex()
    {
        // Get the search index
        SearchIndexInfo deleteIndex = SearchIndexInfoProvider.GetSearchIndexInfo("MyNewIndex");

        // Delete the search index
        SearchIndexInfoProvider.DeleteSearchIndexInfo(deleteIndex);

        return (deleteIndex != null);
    }

    #endregion


    #region "API examples - Search index on site"

    /// <summary>
    /// Adds search index to site. Called when the "Add index to site" button is pressed.
    /// Expects the CreateSearchIndex method to be run first.
    /// </summary>
    private bool AddSearchIndexToSite()
    {
        // Get the search index
        SearchIndexInfo index = SearchIndexInfoProvider.GetSearchIndexInfo("MyNewIndex");
        if (index != null)
        {
            int indexId = index.IndexID;
            int siteId = SiteContext.CurrentSiteID;

            // Save the binding
            SearchIndexSiteInfoProvider.AddSearchIndexToSite(indexId, siteId);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Removes search index from site. Called when the "Remove index from site" button is pressed.
    /// Expects the AddSearchIndexToSite method to be run first.
    /// </summary>
    private bool RemoveSearchIndexFromSite()
    {
        // Get the search index
        SearchIndexInfo removeIndex = SearchIndexInfoProvider.GetSearchIndexInfo("MyNewIndex");
        if (removeIndex != null)
        {
            int siteId = SiteContext.CurrentSiteID;

            // Get the binding
            SearchIndexSiteInfo indexSite = SearchIndexSiteInfoProvider.GetSearchIndexSiteInfo(removeIndex.IndexID, siteId);

            // Delete the binding
            SearchIndexSiteInfoProvider.DeleteSearchIndexSiteInfo(indexSite);

            return true;
        }

        return false;
    }

    #endregion


    #region "API examples - Search index on culture"

    /// <summary>
    /// Adds culture to search index. Called when the "Add culture to index" button is pressed.
    /// Expects the CreateSearchIndex method to be run first.
    /// </summary>
    private bool AddCultureToSearchIndex()
    {
        // Get the search index and culture
        SearchIndexInfo index = SearchIndexInfoProvider.GetSearchIndexInfo("MyNewIndex");
        CultureInfo culture = CultureInfoProvider.GetCultureInfo("en-us");

        if ((index != null) && (culture != null))
        {
            // Save the binding
            SearchIndexCultureInfoProvider.AddSearchIndexCulture(index.IndexID, culture.CultureID);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Removes culture from search index. Called when the "Remove culture from index" button is pressed.
    /// Expects the AddCultureToSearchIndex method to be run first.
    /// </summary>
    private bool RemoveCultureFromSearchIndex()
    {
        // Get the search index
        SearchIndexInfo removeIndex = SearchIndexInfoProvider.GetSearchIndexInfo("MyNewIndex");
        CultureInfo culture = CultureInfoProvider.GetCultureInfo("en-us");

        if ((removeIndex != null) && (culture != null))
        {
            // Get the binding
            SearchIndexCultureInfo indexCulture = SearchIndexCultureInfoProvider.GetSearchIndexCultureInfo(removeIndex.IndexID, culture.CultureID);

            // Delete the binding
            SearchIndexCultureInfoProvider.DeleteSearchIndexCultureInfo(indexCulture);

            return true;
        }

        return false;
    }

    #endregion


    #region "API examples - Search actions"

    /// <summary>
    /// Rebuilds the search index. Called when the "Rebuild index" button is pressed.
    /// Expects the CreateSearchIndex method to be run first.
    /// </summary>
    private bool RebuildIndex()
    {
        // Get the search index
        SearchIndexInfo index = SearchIndexInfoProvider.GetSearchIndexInfo("MyNewIndex");

        if (index != null)
        {
            // Create rebuild task 
            SearchTaskInfoProvider.CreateTask(SearchTaskTypeEnum.Rebuild, null, null, index.IndexName, index.IndexID);

            return true;
        }
        return false;
    }


    /// <summary>
    /// Searchs text. Called when the "Search text" button is pressed.
    /// Expects the CreateSearchIndex method to be run first.
    /// </summary>
    private bool SearchText()
    {
        // Get the search index
        SearchIndexInfo index = SearchIndexInfoProvider.GetSearchIndexInfo("MyNewIndex");

        if (index != null)
        {
            // Prepare parameters
            SearchParameters parameters = new SearchParameters()
            {
                SearchFor = "home",
                SearchSort = "##SCORE##",
                Path = "/%",
                ClassNames = "",
                CurrentCulture = "EN-US",
                DefaultCulture = CultureHelper.EnglishCulture.IetfLanguageTag,
                CombineWithDefaultCulture = false,
                CheckPermissions = false,
                SearchInAttachments = false,
                User = (UserInfo)MembershipContext.AuthenticatedUser,
                SearchIndexes = index.IndexName,
                StartingPosition = 0,
                DisplayResults = 100,
                NumberOfProcessedResults = 100,
                NumberOfResults = 0,
                AttachmentWhere = String.Empty,
                AttachmentOrderBy = String.Empty,
            };

            // Search
            DataSet results = SearchHelper.Search(parameters);

            // If found at least one item
            if (parameters.NumberOfResults > 0)
            {
                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// Adds search index to site. Called when the "Add index to site" button is pressed.
    /// Expects the CreateSearchIndex method to be run first.
    /// </summary>
    private bool UpdateIndex()
    {
        // Tree provider
        TreeProvider provider = new TreeProvider(MembershipContext.AuthenticatedUser);
        // Get document of specified site, aliaspath and culture
        TreeNode node = provider.SelectSingleNode(SiteContext.CurrentSiteName, "/", "en-us");

        // If node exists
        if ((node != null) && DocumentHelper.IsSearchTaskCreationAllowed(node))
        {
            // Edit and save document node
            node.NodeDocType += " changed";
            node.Update();

            // Create update task
            SearchTaskInfoProvider.CreateTask(SearchTaskTypeEnum.Update, TreeNode.OBJECT_TYPE, SearchFieldsConstants.ID, node.GetSearchID(), node.DocumentID);

            return true;
        }
        return false;
    }

    #endregion
}