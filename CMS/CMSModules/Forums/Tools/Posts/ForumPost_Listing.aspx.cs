using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.Forums;
using CMS.Helpers;
using CMS.UIControls;
using CMS.ExtendedControls;
using CMS.ExtendedControls.ActionsConfig;

public partial class CMSModules_Forums_Tools_Posts_ForumPost_Listing : CMSForumsPage
{
    private int postId = 0;
    private int forumId = 0;
    private ForumPostInfo postInfo = null;


    protected void Page_Load(object sender, EventArgs e)
    {
        string currentForumPost = "";

        string[] post = QueryHelper.GetString("postid", "").Split(';');
        postId = ValidationHelper.GetInteger(post[0], 0);
        ForumContext.CheckSite(0, 0, postId);

        if (post.Length >= 2)
        {
            forumId = ValidationHelper.GetInteger(post[1], 0);
            postListing.ForumId = forumId;
        }

        // Show current post
        postInfo = ForumPostInfoProvider.GetForumPostInfo(postId);
        if (postInfo != null)
        {
            currentForumPost = HTMLHelper.HTMLEncode(postInfo.PostSubject);
            postListing.PostInfo = postInfo;
        }
        // If not post, show current forum
        else if (forumId > 0)
        {
            ForumInfo fi = ForumInfoProvider.GetForumInfo(forumId);
            if (fi != null)
            {
                currentForumPost = fi.ForumDisplayName;
            }
        }

        postListing.IsLiveSite = false;

        InitializeMasterPage(currentForumPost);
    }


    /// <summary>
    /// Initializes Master Page.
    /// </summary>
    protected void InitializeMasterPage(string currentForumPost)
    {
        Title = "Forum post listing";
        PageTitle.TitleText = GetString("Forums.Listing.Title");
        if (postInfo != null)
        {
            HeaderAction action = new HeaderAction();
            action.Text = GetString("Forums.ParentPost");
            action.OnClientClick = "SelectPost(" + postInfo.PostParentID + ", " + postInfo.PostForumID + ");";
            action.RedirectUrl = null;
            CurrentMaster.HeaderActions.AddAction(action);
        }
    }
}