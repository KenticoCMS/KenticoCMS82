using System;
using System.Data;

using CMS.Blogs;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.Localization;
using CMS.Membership;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSAPIExamples_Code_Documents_Blogs_Default : CMSAPIExamplePage
{
    #region "Initialization"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Blog
        apiCreateBlog.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateBlog);
        apiGetAndUpdateBlog.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndUpdateBlog);
        apiGetAndBulkUpdateBlogs.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndBulkUpdateBlogs);
        apiDeleteBlog.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteBlog);

        // Blog post
        apiCreateBlogPost.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateBlogPost);
        apiGetAndUpdateBlogPost.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndUpdateBlogPost);
        apiGetAndBulkUpdateBlogPosts.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndBulkUpdateBlogPosts);
        apiDeleteBlogPost.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteBlogPost);

        // Blog comment
        apiCreateBlogComment.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateBlogComment);
        apiGetAndUpdateBlogComment.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndUpdateBlogComment);
        apiGetAndBulkUpdateBlogComments.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndBulkUpdateBlogComments);
        apiDeleteBlogComment.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteBlogComment);

        // Blog post subscription
        apiCreateBlogPostSubscription.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateBlogPostSubscription);
        apiGetAndUpdateBlogPostSubscription.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndUpdateBlogPostSubscription);
        apiGetAndBulkUpdateBlogPostSubscriptions.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndBulkUpdateBlogPostSubscriptions);
        apiDeleteBlogPostSubscription.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteBlogPostSubscription);
    }

    #endregion


    #region "Mass actions"

    /// <summary>
    /// Runs all creating and managing examples.
    /// </summary>
    public override void RunAll()
    {
        base.RunAll();

        // Blog
        apiCreateBlog.Run();
        apiGetAndUpdateBlog.Run();
        apiGetAndBulkUpdateBlogs.Run();

        // Blog post
        apiCreateBlogPost.Run();
        apiGetAndUpdateBlogPost.Run();
        apiGetAndBulkUpdateBlogPosts.Run();

        // Blog comment
        apiCreateBlogComment.Run();
        apiGetAndUpdateBlogComment.Run();
        apiGetAndBulkUpdateBlogComments.Run();

        // Blog post subscription
        apiCreateBlogPostSubscription.Run();
        apiGetAndUpdateBlogPostSubscription.Run();
        apiGetAndBulkUpdateBlogPostSubscriptions.Run();
    }


    /// <summary>
    /// Runs all cleanup examples.
    /// </summary>
    public override void CleanUpAll()
    {
        base.CleanUpAll();

        // Blog post subscription
        apiDeleteBlogPostSubscription.Run();

        // Blog comment
        apiDeleteBlogComment.Run();

        // Blog post
        apiDeleteBlogPost.Run();

        // Blog
        apiDeleteBlog.Run();
    }

    #endregion


    #region "API examples - Blog"

    /// <summary>
    /// Creates blog. Called when the "Create blog" button is pressed.
    /// </summary>
    private bool CreateBlog()
    {
        TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);

        // Get root document
        TreeNode root = tree.SelectSingleNode(SiteContext.CurrentSiteName, "/", null, true);
        if (root != null)
        {
            // Create a new document node 
            TreeNode node = TreeNode.New("CMS.Blog", tree);

            // Set values
            node.DocumentName = "MyNewBlog";
            node.DocumentCulture = LocalizationContext.PreferredCultureCode;
            node.SetValue("BlogName", "MyNewBlog");
            node.SetValue("BlogDescription", "My blog description");
            node.SetValue("BlogOpenCommentsFor", "##ALL##");
            node.SetValue("BlogAllowAnonymousComments", "true");
            node.SetValue("BlogModerateComments", "");
            node.SetValue("BlogOpenCommentsFor", "");
            node.SetValue("BlogUseCAPTCHAForComments", "");
            node.SetValue("BlogEnableSubscriptions", "true");

            // Get blog page template
            PageTemplateInfo pageTemplate = PageTemplateInfoProvider.GetPageTemplateInfo("CorporateSite.BlogDetail");
            if (pageTemplate != null)
            {
                node.SetDefaultPageTemplateID(pageTemplate.PageTemplateId);
            }

            // Create the blog
            DocumentHelper.InsertDocument(node, root, tree);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and updates blog. Called when the "Get and update blog" button is pressed.
    /// Expects the CreateBlog method to be run first.
    /// </summary>
    private bool GetAndUpdateBlog()
    {
        TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);

        // Get blog document
        TreeNode node = tree.SelectSingleNode(SiteContext.CurrentSiteName, "/MyNewBlog", null, true);
        if (node != null)
        {
            // Change blog document name
            node.DocumentName = node.DocumentName.ToLower();

            // Update the blog
            DocumentHelper.UpdateDocument(node, tree);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates blogs. Called when the "Get and bulk update blogs" button is pressed.
    /// Expects the CreateBlog method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateBlogs()
    {
        TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);

        // Get the blog documents
        DataSet blogs = BlogHelper.GetBlogs(SiteContext.CurrentSiteName, 0, null, null, "NodeAliasPath LIKE N'/MyNewBlog'");
        if (!DataHelper.DataSourceIsEmpty(blogs))
        {
            foreach (DataRow blogDr in blogs.Tables[0].Rows)
            {
                // Create object from DataRow
                TreeNode blogNode = TreeNode.New("cms.blog", blogDr);

                // Change blog document name
                blogNode.DocumentName = blogNode.DocumentName.ToUpper();

                // Update the blog
                DocumentHelper.UpdateDocument(blogNode, tree);
            }

            return true;
        }

        return false;
    }


    /// <summary>
    /// Deletes blog. Called when the "Delete blog" button is pressed.
    /// Expects the CreateBlog method to be run first.
    /// </summary>
    private bool DeleteBlog()
    {
        TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);

        // Get the blog document
        TreeNode node = tree.SelectSingleNode(SiteContext.CurrentSiteName, "/MyNewBlog", null, true);
        if (node != null)
        {
            // Delete the blog
            DocumentHelper.DeleteDocument(node, tree, true, true, true);

            return true;
        }

        return false;
    }

    #endregion


    #region "API examples - Blog post"

    /// <summary>
    /// Creates blog post. Called when the "Create post" button is pressed.
    /// Expects the CreateBlog method to be run first.
    /// </summary>
    private bool CreateBlogPost()
    {
        TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);

        // Get root document
        TreeNode rootDocument = tree.SelectSingleNode(SiteContext.CurrentSiteName, "/MyNewBlog", null, true);
        if (rootDocument != null)
        {
            // Create a new blog document 
            TreeNode node = TreeNode.New("CMS.BlogPost", tree);

            // Set values
            node.DocumentName = "MyNewBlogPost";
            node.SetValue("BlogPostTitle", "MyNewBlogPost");
            node.SetValue("BlogPostDate", DateTime.Now);
            node.SetValue("BlogPostSummary", "Blog post summary");
            node.SetValue("BlogPostBody", "Blog post body");
            node.SetValue("BlogPostAllowComments", "true");
            node.DocumentCulture = LocalizationContext.PreferredCultureCode;

            // Ensure blog month
            var parent = DocumentHelper.EnsureBlogPostHierarchy(node, rootDocument, tree);

            // Create the blog post
            DocumentHelper.InsertDocument(node, parent, tree);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and updates blog post. Called when the "Get and update post" button is pressed.
    /// Expects the CreateBlogPost method to be run first.
    /// </summary>
    private bool GetAndUpdateBlogPost()
    {
        TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);

        // Get the blog posts
        DataSet posts = BlogHelper.GetBlogPosts(SiteContext.CurrentSiteName, "/MyNewBlog", null, true, null, null, true);
        if (!DataHelper.DataSourceIsEmpty(posts))
        {
            // Create object from DataRow
            TreeNode modifyNode = TreeNode.New("cms.blogpost", posts.Tables[0].Rows[0], tree);

            // Change the properties
            modifyNode.SetValue("BlogPostBody", "Blog post body was updated.");

            // Update the blog post
            DocumentHelper.UpdateDocument(modifyNode, tree);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates blog posts. Called when the "Get and bulk update posts" button is pressed.
    /// Expects the CreateBlogPost method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateBlogPosts()
    {
        TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);

        DataSet posts = BlogHelper.GetBlogPosts(SiteContext.CurrentSiteName, "/MyNewBlog", null, true, null, null, true);
        if (!DataHelper.DataSourceIsEmpty(posts))
        {
            // Loop through the individual items
            foreach (DataRow groupDr in posts.Tables[0].Rows)
            {
                // Create object from DataRow
                TreeNode modifyNode = TreeNode.New("cms.blogpost", groupDr, tree);

                // Update the property
                modifyNode.SetValue("BlogPostBody", "Blog post body was updated.");

                // Update the blog post
                DocumentHelper.UpdateDocument(modifyNode, tree);
            }

            return true;
        }

        return false;
    }


    /// <summary>
    /// Deletes blog post. Called when the "Delete post" button is pressed.
    /// Expects the CreateBlogPost method to be run first.
    /// </summary>
    private bool DeleteBlogPost()
    {
        TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);

        DataSet posts = BlogHelper.GetBlogPosts(SiteContext.CurrentSiteName, "/MyNewBlog", null, true, null, null, true);
        if (!DataHelper.DataSourceIsEmpty(posts))
        {
            // Create object from DataRow
            TreeNode deleteNode = TreeNode.New("cms.blogpost", posts.Tables[0].Rows[0], tree);

            // Delete the blog post
            DocumentHelper.DeleteDocument(deleteNode, tree, true, true, true);

            return true;
        }

        return false;
    }

    #endregion


    #region "API examples - Blog comment"

    /// <summary>
    /// Creates blog comment. Called when the "Create comment" button is pressed.
    /// </summary>
    private bool CreateBlogComment()
    {
        // Prepare the parameters
        TreeNode blogPostNode = null;

        // Get the content tree
        TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);

        // Get the post
        DataSet posts = BlogHelper.GetBlogPosts(SiteContext.CurrentSiteName, "/MyNewBlog", null, true, null, null, true);
        if (!DataHelper.DataSourceIsEmpty(posts))
        {
            // Create object from DataRow
            blogPostNode = TreeNode.New("cms.blogpost", posts.Tables[0].Rows[0], tree);
        }

        if (blogPostNode != null)
        {
            // Create new blog comment object
            BlogCommentInfo newComment = new BlogCommentInfo();

            // Set the properties
            newComment.CommentText = "My new comment";
            newComment.CommentUserName = MembershipContext.AuthenticatedUser.UserName;
            newComment.CommentUserID = MembershipContext.AuthenticatedUser.UserID;
            newComment.CommentApprovedByUserID = MembershipContext.AuthenticatedUser.UserID;
            newComment.CommentPostDocumentID = blogPostNode.DocumentID;
            newComment.CommentDate = DateTime.Now;

            // Create the blog comment
            BlogCommentInfoProvider.SetBlogCommentInfo(newComment);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and updates blog comment. Called when the "Get and update comment" button is pressed.
    /// Expects the CreateBlogComment method to be run first.
    /// </summary>
    private bool GetAndUpdateBlogComment()
    {
        // Prepare the parameters
        string where = "CommentText LIKE 'My new%'";
        string blogWhere = "NodeName LIKE 'MyNewBlog%'";

        // Get the blog comment
        DataSet comments = BlogCommentInfoProvider.GetComments(where, blogWhere);
        if (!DataHelper.DataSourceIsEmpty(comments))
        {
            // Create object from DataRow
            BlogCommentInfo modifyComment = new BlogCommentInfo(comments.Tables[0].Rows[0]);

            // Update the property
            modifyComment.CommentText = modifyComment.CommentText.ToUpper();

            // Update the blog comment
            BlogCommentInfoProvider.SetBlogCommentInfo(modifyComment);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates blog comments. Called when the "Get and bulk update comments" button is pressed.
    /// Expects the CreateBlogComment method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateBlogComments()
    {
        // Prepare the parameters
        string where = "CommentText LIKE 'My new%'";
        string blogWhere = "NodeName LIKE 'MyNewBlog%'";

        // Get the data
        DataSet comments = BlogCommentInfoProvider.GetComments(where, blogWhere);
        if (!DataHelper.DataSourceIsEmpty(comments))
        {
            // Loop through the individual items
            foreach (DataRow commentDr in comments.Tables[0].Rows)
            {
                // Create object from DataRow
                BlogCommentInfo modifyComment = new BlogCommentInfo(commentDr);

                // Update the properties
                modifyComment.CommentText = modifyComment.CommentText.ToUpper();

                // Update the blog comment
                BlogCommentInfoProvider.SetBlogCommentInfo(modifyComment);
            }

            return true;
        }

        return false;
    }


    /// <summary>
    /// Deletes blog comment. Called when the "Delete comment" button is pressed.
    /// Expects the CreateBlogComment method to be run first.
    /// </summary>
    private bool DeleteBlogComment()
    {
        // Prepare the parameters
        string where = "CommentText LIKE 'My new%'";
        string blogWhere = "NodeName LIKE 'MyNewBlog%'";

        // Get the data
        DataSet comments = BlogCommentInfoProvider.GetComments(where, blogWhere);
        if (!DataHelper.DataSourceIsEmpty(comments))
        {
            // Create object from DataRow
            BlogCommentInfo modifyComment = new BlogCommentInfo(comments.Tables[0].Rows[0]);

            // Delete the blog comment
            BlogCommentInfoProvider.DeleteBlogCommentInfo(modifyComment);

            return true;
        }

        return false;
    }

    #endregion


    #region "API examples - Blog post subscription"

    /// <summary>
    /// Creates blog post subscription. Called when the "Create subscription" button is pressed.
    /// Expects the CreateBlogPost method to be run first.
    /// </summary>
    private bool CreateBlogPostSubscription()
    {
        // Prepare the parameters
        TreeNode documentNode = null;

        // Get the blog posts
        DataSet posts = BlogHelper.GetBlogPosts(SiteContext.CurrentSiteName, "/MyNewBlog", null, true, null, null, true);
        if (!DataHelper.DataSourceIsEmpty(posts))
        {
            TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);

            // Create object from DataRow
            documentNode = TreeNode.New("cms.blogpost", posts.Tables[0].Rows[0], tree);
        }

        if (documentNode != null)
        {
            // Create new blog post subscription object
            BlogPostSubscriptionInfo newSubscription = new BlogPostSubscriptionInfo();

            // Set the properties
            newSubscription.SubscriptionPostDocumentID = documentNode.DocumentID;
            newSubscription.SubscriptionUserID = MembershipContext.AuthenticatedUser.UserID;
            newSubscription.SubscriptionEmail = "administrator@localhost.local";

            // Create the blog post subscription
            BlogPostSubscriptionInfoProvider.SetBlogPostSubscriptionInfo(newSubscription);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and updates blog post subscription. Called when the "Get and update subscription" button is pressed.
    /// Expects the CreateBlogPostSubscription method to be run first.
    /// </summary>
    private bool GetAndUpdateBlogPostSubscription()
    {
        // Prepare the parameters
        TreeNode documentNode = null;

        // Get the blog posts
        DataSet posts = BlogHelper.GetBlogPosts(SiteContext.CurrentSiteName, "/MyNewBlog", null, true, null, null, true);
        if (!DataHelper.DataSourceIsEmpty(posts))
        {
            TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);

            // Create object from DataRow
            documentNode = TreeNode.New("cms.blogpost", posts.Tables[0].Rows[0], tree);
        }

        if (documentNode != null)
        {
            // Get the blog post subscription
            BlogPostSubscriptionInfo updateSubscription = BlogPostSubscriptionInfoProvider.GetBlogPostSubscriptionInfo("administrator@localhost.local", documentNode.DocumentID);
            if (updateSubscription != null)
            {
                // Change the properties
                updateSubscription.SubscriptionEmail = updateSubscription.SubscriptionEmail.ToLower();

                // Update blog post subscription
                BlogPostSubscriptionInfoProvider.SetBlogPostSubscriptionInfo(updateSubscription);

                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates blog post subscriptions. Called when the "Get and bulk update subscriptions" button is pressed.
    /// Expects the CreateBlogPostSubscription method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateBlogPostSubscriptions()
    {
        // Prepare the parameters
        string where = "SubscriptionEmail LIKE 'administrator@localhost.local'";
        string orderBy = "SubscriptionEmail";

        // Get the blog post subscriptions
        DataSet subscriptions = BlogPostSubscriptionInfoProvider.GetBlogPostSubscriptions(where, orderBy);
        if (!DataHelper.DataSourceIsEmpty(subscriptions))
        {
            // Loop through the individual items
            foreach (DataRow subscriptionDr in subscriptions.Tables[0].Rows)
            {
                // Create object from DataRow
                BlogPostSubscriptionInfo modifySubscription = new BlogPostSubscriptionInfo(subscriptionDr);

                // Update the property
                modifySubscription.SubscriptionEmail = modifySubscription.SubscriptionEmail.ToUpper();

                // Update blog post subscription
                BlogPostSubscriptionInfoProvider.SetBlogPostSubscriptionInfo(modifySubscription);
            }

            return true;
        }

        return false;
    }


    /// <summary>
    /// Deletes blog post subscription. Called when the "Delete subscription" button is pressed.
    /// Expects the CreateBlogPostSubscription method to be run first.
    /// </summary>
    private bool DeleteBlogPostSubscription()
    {
        // Prepare the parameters
        TreeNode documentNode = null;

        // Get the blog posts
        DataSet posts = BlogHelper.GetBlogPosts(SiteContext.CurrentSiteName, "/MyNewBlog", null, true, null, null, true);
        if (!DataHelper.DataSourceIsEmpty(posts))
        {
            TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);

            // Create object from DataRow
            documentNode = TreeNode.New("cms.blogpost", posts.Tables[0].Rows[0], tree);
        }

        if (documentNode != null)
        {
            // Get the blog post subscription
            BlogPostSubscriptionInfo deleteSubscription = BlogPostSubscriptionInfoProvider.GetBlogPostSubscriptionInfo("administrator@localhost.local", documentNode.DocumentID);
            if (deleteSubscription != null)
            {
                // Delete the blog post subscription
                BlogPostSubscriptionInfoProvider.DeleteBlogPostSubscriptionInfo(deleteSubscription);

                return true;
            }
        }

        return false;
    }

    #endregion
}