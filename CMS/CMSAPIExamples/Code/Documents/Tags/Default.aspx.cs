using System;
using System.Data;

using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;
using CMS.Taxonomy;

public partial class CMSAPIExamples_Code_Documents_Tags_Default : CMSAPIExamplePage
{
    #region "Initialization"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Tag group
        apiCreateTagGroup.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateTagGroup);
        apiGetAndUpdateTagGroup.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndUpdateTagGroup);
        apiGetAndBulkUpdateTagGroups.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndBulkUpdateTagGroups);
        apiDeleteTagGroup.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteTagGroup);

        // Tag
        apiAddTagToDocument.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(AddTagToDocument);
        apiGetDocumentAndUpdateItsTags.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetDocumentAndUpdateItsTags);
        apiRemoveTagFromDocument.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(RemoveTagFromDocument);
    }

    #endregion


    #region "Mass actions"

    /// <summary>
    /// Runs all creating and managing examples.
    /// </summary>
    public override void RunAll()
    {
        base.RunAll();

        // Tag group
        apiCreateTagGroup.Run();
        apiGetAndUpdateTagGroup.Run();
        apiGetAndBulkUpdateTagGroups.Run();

        // Tag
        apiAddTagToDocument.Run();
        apiGetDocumentAndUpdateItsTags.Run();
    }


    /// <summary>
    /// Runs all cleanup examples.
    /// </summary>
    public override void CleanUpAll()
    {
        base.CleanUpAll();

        // Tag
        apiRemoveTagFromDocument.Run();

        // Tag group
        apiDeleteTagGroup.Run();
    }

    #endregion


    #region "API examples - Tag group"

    /// <summary>
    /// Creates tag group. Called when the "Create group" button is pressed.
    /// </summary>
    private bool CreateTagGroup()
    {
        // Create new tag group object
        TagGroupInfo newGroup = new TagGroupInfo();

        // Set the properties
        newGroup.TagGroupDisplayName = "My new group";
        newGroup.TagGroupName = "MyNewGroup";
        newGroup.TagGroupDescription = "";
        newGroup.TagGroupSiteID = SiteContext.CurrentSiteID;
        newGroup.TagGroupIsAdHoc = false;

        // Create the tag group
        TagGroupInfoProvider.SetTagGroupInfo(newGroup);

        return true;
    }


    /// <summary>
    /// Gets and updates tag group. Called when the "Get and update group" button is pressed.
    /// Expects the CreateTagGroup method to be run first.
    /// </summary>
    private bool GetAndUpdateTagGroup()
    {
        // Get the tag group
        TagGroupInfo updateGroup = TagGroupInfoProvider.GetTagGroupInfo("MyNewGroup", SiteContext.CurrentSiteID);
        if (updateGroup != null)
        {
            // Update the property
            updateGroup.TagGroupDisplayName = updateGroup.TagGroupDisplayName.ToLower();

            // Update the tag group
            TagGroupInfoProvider.SetTagGroupInfo(updateGroup);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates tag groups. Called when the "Get and bulk update groups" button is pressed.
    /// Expects the CreateTagGroup method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateTagGroups()
    {
        // Prepare the parameters
        string where = "TagGroupName LIKE 'MyNew%'";

        // Get the data
        DataSet groups = TagGroupInfoProvider.GetTagGroups(where, null);
        if (!DataHelper.DataSourceIsEmpty(groups))
        {
            // Loop through the individual items
            foreach (DataRow groupDr in groups.Tables[0].Rows)
            {
                // Create object from DataRow
                TagGroupInfo modifyGroup = new TagGroupInfo(groupDr);

                // Update the property
                modifyGroup.TagGroupDisplayName = modifyGroup.TagGroupDisplayName.ToUpper();

                // Update the tag group
                TagGroupInfoProvider.SetTagGroupInfo(modifyGroup);
            }

            return true;
        }

        return false;
    }


    /// <summary>
    /// Deletes tag group. Called when the "Delete group" button is pressed.
    /// Expects the CreateTagGroup method to be run first.
    /// </summary>
    private bool DeleteTagGroup()
    {
        // Get the tag group
        string where = "TagGroupName LIKE 'MyNew%'";

        // Get the data
        DataSet groups = TagGroupInfoProvider.GetTagGroups(where, null);
        if (!DataHelper.DataSourceIsEmpty(groups))
        {
            // Loop through the individual items
            foreach (DataRow groupDr in groups.Tables[0].Rows)
            {
                // Create object from DataRow
                TagGroupInfo deleteGroup = new TagGroupInfo(groupDr);

                // Delete the tag group
                TagGroupInfoProvider.DeleteTagGroupInfo(deleteGroup);
            }

            return true;
        }

        return false;
    }

    #endregion


    #region "API examples - Tag"

    /// <summary>
    /// Creates tag. Called when the "Create tag" button is pressed.
    /// </summary>
    private bool AddTagToDocument()
    {
        TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);

        // Get the root document
        TreeNode root = tree.SelectSingleNode(SiteContext.CurrentSiteName, "/", null, true);

        // Get tag group ID
        TagGroupInfo updateGroup = TagGroupInfoProvider.GetTagGroupInfo("MyNewGroup", SiteContext.CurrentSiteID);

        if ((root != null) && (updateGroup != null))
        {
            // Add tag to document
            root.DocumentTags = "\"My New Tag\"";

            // Add tag to document
            root.DocumentTagGroupID = updateGroup.TagGroupID;

            // Update document
            root.Update();

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and updates tag. Called when the "Get and update tag" button is pressed.
    /// Expects the CreateTag method to be run first.
    /// </summary>
    private bool GetDocumentAndUpdateItsTags()
    {
        TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);

        // Get root document
        TreeNode root = tree.SelectSingleNode(SiteContext.CurrentSiteName, "/", null, true);
        if (root != null)
        {
            if (!String.IsNullOrEmpty(root.DocumentTags))
            {
                // Update the tags
                root.DocumentTags = root.DocumentTags.ToUpper();

                // Update document
                root.Update();

                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// Deletes tag. Called when the "Delete tag" button is pressed.
    /// Expects the CreateTag method to be run first.
    /// </summary>
    private bool RemoveTagFromDocument()
    {
        TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);

        // Get root document
        TreeNode root = tree.SelectSingleNode(SiteContext.CurrentSiteName, "/", null, true);
        if (root != null)
        {
            if (!String.IsNullOrEmpty(root.DocumentTags))
            {
                // Remove tags from the document
                root.DocumentTags = "";

                // Update the document
                root.Update();

                return true;
            }
        }

        return false;
    }

    #endregion
}