using System;

using CMS.Blogs;
using CMS.Helpers;
using CMS.Localization;
using CMS.Membership;
using CMS.DocumentEngine;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.DataEngine;

public partial class CMSModules_Blogs_CMSPages_Trackback : XMLPage
{
    #region "Private variables"

    private Guid postGuid = Guid.Empty;
    private string title;
    private string excerpt = string.Empty;
    private string url;
    private string blogName;
    private string culture = "";

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        // Get post document GUID
        postGuid = QueryHelper.GetGuid("guid", Guid.Empty);
        culture = QueryHelper.GetString("culture", LocalizationContext.PreferredCultureCode);

        // Try to receive GET parameters
        url = QueryHelper.GetString("url", "");
        if (!String.IsNullOrEmpty(url))
        {
            title = QueryHelper.GetString("title", "");
            excerpt = QueryHelper.GetString("excerpt", "");
            blogName = QueryHelper.GetString("blog_name", "");
        }
        // Try to receive POST parameters
        else
        {
            url = GetFormValue("url");
            if (!String.IsNullOrEmpty(url))
            {
                title = GetFormValue("title");
                excerpt = GetFormValue("excerpt");
                blogName = GetFormValue("blog_name");
            }
        }

        // If is there any URL then process request
        if (!String.IsNullOrEmpty(url))
        {
            ProcessParameters();
        }
        // Otherwise try to redirect user to blog post page
        else
        {
            RedirectToBlogPost();
        }
    }


    /// <summary>
    /// Redirects user to blog post page.
    /// </summary>
    private void RedirectToBlogPost()
    {
        TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);
        TreeNode node = tree.SelectSingleNode(postGuid, culture, SiteContext.CurrentSiteName);
        string path;

        // Check that requested blog post exists
        if (node != null)
        {
            path = DocumentURLProvider.GetUrl(node.NodeAliasPath);
            URLHelper.Redirect(path);
        }
        else
        {
            path = SettingsKeyInfoProvider.GetValue(SiteContext.CurrentSiteName + ".CMSDefaultAliasPath");
            URLHelper.Redirect(DocumentURLProvider.GetUrl(path));
        }
    }


    /// <summary>
    /// Checks if form parameter is present and returns its value.
    /// </summary>
    /// <param name="name">Name of parameter</param>
    /// <returns>Value of form parameter</returns>
    private string GetFormValue(string name)
    {
        string[] values = Request.Form.GetValues(name);
        if (values != null)
        {
            if (values.Length > 0)
            {
                return values[0];
            }
        }
        return null;
    }


    /// <summary>
    /// Process trackback parameters.
    /// </summary>
    private void ProcessParameters()
    {
        TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);
        TreeNode node = tree.SelectSingleNode(postGuid, culture, SiteContext.CurrentSiteName);

        // Check that requested blog post exists
        if (node != null)
        {
            node = TreeHelper.SelectSingleDocument(node.DocumentID);

            // Check if comment from given URL is not already inserted
            int existingComment = BlogCommentInfoProvider.GetBlogComments()
                                                 .Column("CommentID")
                                                 .WhereEquals("CommentPostDocumentID", node.DocumentID)
                                                 .WhereTrue("CommentIsTrackback")
                                                 .WhereEquals("CommentUrl", url)
                                                 .Count;

            if (existingComment == 0)
            {
                // Check if blog for blog post exists
                TreeNode blogNode = BlogHelper.GetParentBlog(node.DocumentID, false);
                if (blogNode != null)
                {
                    // Check if comments are opened
                    int days = ValidationHelper.GetInteger(blogNode.GetValue("BlogOpenCommentsFor"), 0);

                    // Check if comments are always opened
                    bool opened = (days == BlogProperties.OPEN_COMMENTS_ALWAYS);

                    // Check if comments are opened in present time
                    if ((ValidationHelper.GetDateTime(node.GetValue("BlogPostDate"), DateTime.Today).AddDays(days)) >= DateTime.Today)
                    {
                        opened = true;
                    }

                    // Check if comments are disabled
                    if (days == BlogProperties.OPEN_COMMENTS_DISABLE)
                    {
                        opened = false;
                    }

                    // Check if trackback comments are enabled, anonymous comments are enabled, comments are enabled in present time and blog post allow comments
                    if (ValidationHelper.GetBoolean(blogNode.GetValue("BlogEnableTrackbacks"), false) && (ValidationHelper.GetBoolean(blogNode.GetValue("BlogAllowAnonymousComments"), false)) && (opened) && (ValidationHelper.GetBoolean(node.GetValue("BlogPostAllowComments"), false)))
                    {
                        // Create new comment
                        BlogCommentInfo comment = new BlogCommentInfo();

                        comment.CommentUrl = url.Length > 450 ? url.Substring(0, 450) : url;
                        comment.CommentText = excerpt;
                        comment.CommentDate = DateTime.Now;
                        comment.CommentUserName = GetCommentUserName(blogName, title);
                        comment.CommentUserID = 0;
                        comment.CommentApprovedByUserID = 0;
                        comment.CommentPostDocumentID = node.DocumentID;
                        comment.CommentIsTrackback = true;
                        comment.CommentIsSpam = false;

                        // User IP address
                        comment.CommentInfo.IPAddress = RequestContext.UserHostAddress;
                        // User agent
                        comment.CommentInfo.Agent = Request.UserAgent;

                        // Check if comments are moderated
                        comment.CommentApproved = !ValidationHelper.GetBoolean(blogNode.GetValue("BlogModerateComments"), false);

                        // Save changes to database
                        BlogCommentInfoProvider.SetBlogCommentInfo(comment);

                        // Send OK response, no error message
                        SendResponse(null);
                    }
                    else
                    {
                        SendResponse("Blog doesn't enable trackbacks.");
                    }
                }
                else
                {
                    SendResponse("Blog not found.");
                }
            }
            else
            {
                SendResponse("Blog post with given URL is already referenced.");
            }
        }
        else
        {
            SendResponse("Blog post not found.");
        }
    }


    /// <summary>
    /// Returns separator between blog and blog post. If one of the names is not defined empty string is returned.
    /// </summary>
    /// <param name="blogName">Blog name</param>
    /// <param name="blogPostName">Blog post name</param>
    private static string GetCommentUserName(string blogName, string blogPostName)
    {
        string separator = "";
        if (!string.IsNullOrEmpty(blogName) && !string.IsNullOrEmpty(blogPostName))
        {
            separator = " - ";
        }
        return blogName + separator + blogPostName;
    }


    /// <summary>
    /// Sends response to client.
    /// </summary>
    /// <param name="message">If not empty then send error response with this message</param>
    private void SendResponse(string message)
    {
        // Clear response
        CookieHelper.ClearResponseCookies();
        Response.Clear();

        // Create response data
        Response.ContentType = "application/x-www-form-urlencoded";

        if (String.IsNullOrEmpty(message))
        {
            Response.Write("<?xml version=\"1.0\" encoding=\"iso-8859-1\"?><response><error>0</error></response>");
        }
        else
        {
            Response.Write("<?xml version=\"1.0\" encoding=\"iso-8859-1\"?><response><error>1</error><message>" + message + "</message></response>");
        }

        // Complete response
        RequestHelper.EndResponse();
    }
}