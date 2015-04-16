using System;
using System.Data;

using CMS;
using CMS.Forums;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSAPIExamples_Code_Community_Forums_Default : CMSAPIExamplePage
{
    #region "Initialization"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Forum group
        apiCreateForumGroup.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateForumGroup);
        apiGetAndUpdateForumGroup.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndUpdateForumGroup);
        apiGetAndBulkUpdateForumGroups.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndBulkUpdateForumGroups);
        apiDeleteForumGroup.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteForumGroup);

        // Forum
        apiCreateForum.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateForum);
        apiGetAndUpdateForum.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndUpdateForum);
        apiGetAndBulkUpdateForums.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndBulkUpdateForums);
        apiDeleteForum.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteForum);

        // Forum post
        apiCreateForumPost.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateForumPost);
        apiGetAndUpdateForumPost.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndUpdateForumPost);
        apiGetAndBulkUpdateForumPosts.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndBulkUpdateForumPosts);
        apiDeleteForumPost.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteForumPost);
    }

    #endregion


    #region "Mass actions"

    /// <summary>
    /// Runs all creating and managing examples.
    /// </summary>
    public override void RunAll()
    {
        base.RunAll();

        // Forum group
        apiCreateForumGroup.Run();
        apiGetAndUpdateForumGroup.Run();
        apiGetAndBulkUpdateForumGroups.Run();

        // Forum
        apiCreateForum.Run();
        apiGetAndUpdateForum.Run();
        apiGetAndBulkUpdateForums.Run();

        // Forum post
        apiCreateForumPost.Run();
        apiGetAndUpdateForumPost.Run();
        apiGetAndBulkUpdateForumPosts.Run();
    }


    /// <summary>
    /// Runs all cleanup examples.
    /// </summary>
    public override void CleanUpAll()
    {
        base.CleanUpAll();

        // Forum post
        apiDeleteForumPost.Run();

        // Forum
        apiDeleteForum.Run();

        // Forum group
        apiDeleteForumGroup.Run();
    }

    #endregion


    #region "API examples - Forum group"

    /// <summary>
    /// Creates forum group. Called when the "Create group" button is pressed.
    /// </summary>
    private bool CreateForumGroup()
    {
        // Create new forum group object
        ForumGroupInfo newGroup = new ForumGroupInfo();

        // Set the properties
        newGroup.GroupDisplayName = "My new group";
        newGroup.GroupName = "MyNewGroup";
        newGroup.GroupSiteID = SiteContext.CurrentSiteID;
        newGroup.GroupAuthorDelete = true;
        newGroup.GroupAuthorEdit = true;
        newGroup.GroupDisplayEmails = true;

        // Save the forum group
        ForumGroupInfoProvider.SetForumGroupInfo(newGroup);

        return true;
    }


    /// <summary>
    /// Gets and updates forum group. Called when the "Get and update group" button is pressed.
    /// Expects the CreateForumGroup method to be run first.
    /// </summary>
    private bool GetAndUpdateForumGroup()
    {
        // Get the forum group
        ForumGroupInfo updateGroup = ForumGroupInfoProvider.GetForumGroupInfo("MyNewGroup", SiteContext.CurrentSiteID);
        if (updateGroup != null)
        {
            // Update the properties
            updateGroup.GroupDisplayName = updateGroup.GroupDisplayName.ToLower();

            // Save the changes
            ForumGroupInfoProvider.SetForumGroupInfo(updateGroup);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates forum groups. Called when the "Get and bulk update groups" button is pressed.
    /// Expects the CreateForumGroup method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateForumGroups()
    {
        // Prepare the parameters
        string where = "GroupName LIKE N'MyNewGroup%'";
        string orderBy = "";
        string columns = "";
        int topN = 10;

        // Get the data
        DataSet groups = ForumGroupInfoProvider.GetGroups(where, orderBy, topN, columns);
        if (!DataHelper.DataSourceIsEmpty(groups))
        {
            // Loop through the individual items
            foreach (DataRow groupDr in groups.Tables[0].Rows)
            {
                // Create object from DataRow
                ForumGroupInfo modifyGroup = new ForumGroupInfo(groupDr);

                // Update the properties
                modifyGroup.GroupDisplayName = modifyGroup.GroupDisplayName.ToUpper();

                // Save the changes
                ForumGroupInfoProvider.SetForumGroupInfo(modifyGroup);
            }

            return true;
        }

        return false;
    }


    /// <summary>
    /// Deletes forum group. Called when the "Delete group" button is pressed.
    /// Expects the CreateForumGroup method to be run first.
    /// </summary>
    private bool DeleteForumGroup()
    {
        // Get the forum group
        ForumGroupInfo deleteGroup = ForumGroupInfoProvider.GetForumGroupInfo("MyNewGroup", SiteContext.CurrentSiteID);

        // Delete the forum group
        ForumGroupInfoProvider.DeleteForumGroupInfo(deleteGroup);

        return (deleteGroup != null);
    }

    #endregion


    #region "API examples - Forum"

    /// <summary>
    /// Creates forum. Called when the "Create forum" button is pressed.
    /// </summary>
    private bool CreateForum()
    {
        // Get the forum group
        ForumGroupInfo group = ForumGroupInfoProvider.GetForumGroupInfo("MyNewGroup", SiteContext.CurrentSiteID);

        if (group != null)
        {
            // Create new forum object
            ForumInfo newForum = new ForumInfo();

            // Set the properties
            newForum.ForumDisplayName = "My new forum";
            newForum.ForumName = "MyNewForum";
            newForum.ForumGroupID = group.GroupID;
            newForum.ForumSiteID = group.GroupSiteID;
            newForum.AllowAccess = SecurityAccessEnum.AllUsers;
            newForum.AllowAttachFiles = SecurityAccessEnum.AuthenticatedUsers;
            newForum.AllowPost = SecurityAccessEnum.AllUsers;
            newForum.AllowReply = SecurityAccessEnum.AllUsers;
            newForum.AllowSubscribe = SecurityAccessEnum.AllUsers;
            newForum.ForumOpen = true;
            newForum.ForumModerated = false;
            newForum.ForumThreads = 0;
            newForum.ForumPosts = 0;


            // Save the forum
            ForumInfoProvider.SetForumInfo(newForum);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and updates forum. Called when the "Get and update forum" button is pressed.
    /// Expects the CreateForum method to be run first.
    /// </summary>
    private bool GetAndUpdateForum()
    {
        // Get the forum
        ForumInfo updateForum = ForumInfoProvider.GetForumInfo("MyNewForum", SiteContext.CurrentSiteID);
        if (updateForum != null)
        {
            // Update the properties
            updateForum.ForumDisplayName = updateForum.ForumDisplayName.ToLower();

            // Save the changes
            ForumInfoProvider.SetForumInfo(updateForum);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates forums. Called when the "Get and bulk update forums" button is pressed.
    /// Expects the CreateForum method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateForums()
    {
        // Prepare the parameters
        string where = "ForumName LIKE N'MyNewForum%'";
        string orderBy = "";
        string columns = "";
        int topN = 10;

        // Get the data
        DataSet forums = ForumInfoProvider.GetForums(where, orderBy, topN, columns);
        if (!DataHelper.DataSourceIsEmpty(forums))
        {
            // Loop through the individual items
            foreach (DataRow forumDr in forums.Tables[0].Rows)
            {
                // Create object from DataRow
                ForumInfo modifyForum = new ForumInfo(forumDr);

                // Update the properties
                modifyForum.ForumDisplayName = modifyForum.ForumDisplayName.ToUpper();

                // Save the changes
                ForumInfoProvider.SetForumInfo(modifyForum);
            }

            return true;
        }

        return false;
    }


    /// <summary>
    /// Deletes forum. Called when the "Delete forum" button is pressed.
    /// Expects the CreateForum method to be run first.
    /// </summary>
    private bool DeleteForum()
    {
        // Get the forum
        ForumInfo deleteForum = ForumInfoProvider.GetForumInfo("MyNewForum", SiteContext.CurrentSiteID);

        // Delete the forum
        ForumInfoProvider.DeleteForumInfo(deleteForum);

        return (deleteForum != null);
    }

    #endregion


    #region "API examples - Forum post"

    /// <summary>
    /// Creates forum post. Called when the "Create post" button is pressed.
    /// </summary>
    private bool CreateForumPost()
    {
        // Get the forum
        ForumInfo forum = ForumInfoProvider.GetForumInfo("MyNewForum", SiteContext.CurrentSiteID);
        if (forum != null)
        {
            // Create new forum post object
            ForumPostInfo newPost = new ForumPostInfo();

            // Set the properties
            newPost.PostUserID = MembershipContext.AuthenticatedUser.UserID;
            newPost.PostUserMail = MembershipContext.AuthenticatedUser.Email;
            newPost.PostUserName = MembershipContext.AuthenticatedUser.UserName;
            newPost.PostForumID = forum.ForumID;
            newPost.SiteId = forum.ForumSiteID;
            newPost.PostTime = DateTime.Now;
            newPost.PostApproved = true;
            newPost.PostText = "This is my new post";
            newPost.PostSubject = "My new post";

            // Save the forum post
            ForumPostInfoProvider.SetForumPostInfo(newPost);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and updates forum post. Called when the "Get and update post" button is pressed.
    /// Expects the CreateForumPost method to be run first.
    /// </summary>
    private bool GetAndUpdateForumPost()
    {
        // Prepare the parameters
        string where = "PostSubject LIKE N'My new post%'";
        string orderBy = "";
        string columns = "";
        int topN = 10;

        // Get the data
        DataSet posts = ForumPostInfoProvider.GetForumPosts(where, orderBy, topN, columns);
        if (!DataHelper.DataSourceIsEmpty(posts))
        {
            ForumPostInfo updatePost = new ForumPostInfo(posts.Tables[0].Rows[0]);

            // Update the properties
            updatePost.PostSubject = updatePost.PostSubject.ToLower();

            // Save the changes
            ForumPostInfoProvider.SetForumPostInfo(updatePost);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates forum posts. Called when the "Get and bulk update posts" button is pressed.
    /// Expects the CreateForumPost method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateForumPosts()
    {
        // Prepare the parameters
        string where = "PostSubject LIKE N'My new post%'";
        string orderBy = "";
        string columns = "";
        int topN = 10;

        // Get the data
        DataSet posts = ForumPostInfoProvider.GetForumPosts(where, orderBy, topN, columns);
        if (!DataHelper.DataSourceIsEmpty(posts))
        {
            // Loop through the individual items
            foreach (DataRow postDr in posts.Tables[0].Rows)
            {
                // Create object from DataRow
                ForumPostInfo modifyPost = new ForumPostInfo(postDr);

                // Update the properties
                modifyPost.PostSubject = modifyPost.PostSubject.ToUpper();

                // Save the changes
                ForumPostInfoProvider.SetForumPostInfo(modifyPost);
            }

            return true;
        }

        return false;
    }


    /// <summary>
    /// Deletes forum post. Called when the "Delete post" button is pressed.
    /// Expects the CreateForumPost method to be run first.
    /// </summary>
    private bool DeleteForumPost()
    {
        // Prepare the parameters
        string where = "PostSubject LIKE N'My new post%'";
        string orderBy = "";
        string columns = "";
        int topN = 10;

        // Get the data
        DataSet posts = ForumPostInfoProvider.GetForumPosts(where, orderBy, topN, columns);
        if (!DataHelper.DataSourceIsEmpty(posts))
        {
            // Get the forum post
            ForumPostInfo deletePost = new ForumPostInfo(posts.Tables[0].Rows[0]);

            // Delete the forum post
            ForumPostInfoProvider.DeleteForumPostInfo(deletePost);

            return true;
        }

        return false;
    }

    #endregion
}