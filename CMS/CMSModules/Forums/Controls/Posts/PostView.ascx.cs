using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.ExtendedControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.Forums;
using CMS.Helpers;
using CMS.Membership;
using CMS.Base;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_Forums_Controls_Posts_PostView : CMSAdminEditControl
{
    #region "Private fields"

    private int mPostId = 0;
    private int mForumId = 0;
    private int mReply = 0;
    private string mListingPost = null;
    private ForumViewer ThreadMove1 = null;
    private ForumPostInfo mPostInfo = null;
    private string listingParameter = null;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
    }


    /// <summary>
    /// Indicates if control is used on live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            plcMess.IsLiveSite = value;
            base.IsLiveSite = value;
        }
    }


    /// <summary>
    /// ID of the currently processed post.
    /// </summary>
    public int PostID
    {
        get
        {
            return mPostId;
        }
        set
        {
            mPostId = value;
        }
    }


    /// <summary>
    /// ID of the currently processed forum.
    /// </summary>
    public int ForumID
    {
        get
        {
            return mForumId;
        }
        set
        {
            mForumId = value;
        }
    }


    /// <summary>
    /// Gets or sets the post id which leads to forum post listing
    /// Note: Value is combined as postid;forumid
    /// </summary>
    public string ListingPost
    {
        get
        {
            return mListingPost;
        }
        set
        {
            mListingPost = value;
        }
    }


    /// <summary>
    /// ID of the reply for some post.
    /// </summary>
    public int Reply
    {
        get
        {
            return mReply;
        }
        set
        {
            mReply = value;
        }
    }


    /// <summary>
    /// Gets the post info according to PostID property.
    /// </summary>
    private ForumPostInfo PostInfo
    {
        get
        {
            if ((mPostInfo == null) && (PostID > 0))
            {
                mPostInfo = ForumPostInfoProvider.GetForumPostInfo(PostID);
            }

            return mPostInfo;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        // Show back link if is opened from listing        
        if (!String.IsNullOrEmpty(ListingPost) && (PostInfo != null))
        {
            listingParameter = "&listingpost=" + HTMLHelper.HTMLEncode(ListingPost);
        }


        ThreadMove1 = Page.LoadUserControl("~/CMSModules/Forums/Controls/ThreadMove.ascx") as ForumViewer;
        ThreadMove1.ID = "ctrlThreadMove";
        ThreadMove1.IsLiveSite = IsLiveSite;
        plcThreadMove.Controls.Add(ThreadMove1);

        if (!Visible)
        {
            EnableViewState = false;
        }

        PostEdit1.OnCancelClick += ForumNewPost1_OnCancelClick;
        PostEdit1.OnCheckPermissions += PostEdit1_OnCheckPermissions;
        PostEdit1.IsLiveSite = false;

        if (Reply != 0)
        {
            NewThreadTitle.TitleText = GetString("ForumPost_View.PostTitleText");

            pnlBody.Visible = false;

            PanelNewThread.Visible = true;
            PostEdit1.ReplyToPostID = PostID;
            PostEdit1.ForumID = ForumID;
            PostEdit1.OnInsertPost += ForumNewPost1_OnInsertPost;
        }
        else
        {
            //New thread            
            if (ForumID != 0)
            {
                NewThreadTitle.TitleText = GetString("ForumPost_View.NewThreadHeaderCaption");

                pnlBody.Visible = false;

                PanelNewThread.Visible = true;
                PostEdit1.ForumID = ForumID;
                PostEdit1.OnInsertPost += ForumNewPost1_OnInsertPost;
            }
            else
            {
                ForumInfo fi = ForumInfoProvider.GetForumInfo(PostInfo.PostForumID);
                if (fi != null)
                {
                    ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ForumScripts",
                                                           ScriptHelper.GetScript(" function ReplyToPost(postId)\n" +
                                                                                  "{    if (postId != 0){\n" +
                                                                                  "location.href='ForumPost_View.aspx?postid=' + postId + '&reply=1&forumId=" + fi.ForumID + listingParameter + "' }}\n"));
                }

                //Create menu using inserted skmMenu control
                InitializeMenu();

                ltlScript.Text += ScriptHelper.GetScript(
                    "function DeletePost(postId) { if (confirm(" + ScriptHelper.GetString(GetString("ForumPost_View.DeleteConfirmation")) + ")) { " + Page.ClientScript.GetPostBackEventReference(btnDelete, null) + "; } } \n" +
                    "function ApprovePost(postId) { " + Page.ClientScript.GetPostBackEventReference(btnApprove, null) + "; } \n" +
                    "function ApproveSubTree(postId) { " + Page.ClientScript.GetPostBackEventReference(btnApproveSubTree, null) + "; } \n" +
                    "function RejectSubTree(postId) { " + Page.ClientScript.GetPostBackEventReference(btnRejectSubTree, null) + "; } \n" +
                    "function StickThread(postId) { " + Page.ClientScript.GetPostBackEventReference(btnStickThread, null) + "; } \n" +
                    "function SplitThread(postId) { " + Page.ClientScript.GetPostBackEventReference(btnSplitThread, null) + "; } \n" +
                    "function LockThread(postId) { " + Page.ClientScript.GetPostBackEventReference(btnLockThread, null) + "; } \n"
                    );
                ForumPost1.ForumID = ForumID;
                ForumPost1.PostID = PostID;
                ForumPost1.DisplayOnly = true;


                if ((PostInfo != null) && (PostInfo.PostAttachmentCount > 0))
                {
                    ReloadAttachmentData(PostID);
                }
            }
        }
    }


    private void ReloadAttachmentData(int PostId)
    {
        string where = "(AttachmentPostID = " + PostId + ")";

        // Load unigrid
        UniGrid.IsLiveSite = IsLiveSite;
        UniGrid.ObjectType = "Forums.ForumAttachment";
        UniGrid.Columns = "AttachmentID,AttachmentFileName,AttachmentFileSize,AttachmentGUID";
        UniGrid.WhereCondition = where;
        UniGrid.OnAction += UniGrid_OnAction;
        UniGrid.OnExternalDataBound += UniGrid_OnExternalDataBound;

        UniGrid.Visible = true;
    }


    private void PostEdit1_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        if (!CheckPermissions("cms.forums", permissionType))
        {
            sender.StopProcessing = true;
        }
    }


    /// <summary>
    /// Unigrid External bound event handler.
    /// </summary>
    private object UniGrid_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        // Display link instead of title
        switch (sourceName.ToLowerCSafe())
        {
            case "title":
                if (parameter != DBNull.Value)
                {
                    DataRowView row = (DataRowView)parameter;

                    // Get info
                    Guid attachmentGuid = ValidationHelper.GetGuid(row["AttachmentGUID"], Guid.Empty);

                    if (attachmentGuid != Guid.Empty)
                    {
                        string url = URLHelper.GetAbsoluteUrl("~/CMSModules/Forums/CMSPages/GetForumAttachment.aspx?fileguid=" + attachmentGuid);
                        string title = ValidationHelper.GetString(row["AttachmentFileName"], "");

                        // Create link to post attachment
                        HyperLink link = new HyperLink();
                        link.NavigateUrl = url;
                        link.Target = "_blank";
                        link.Text = HTMLHelper.HTMLEncode(title);
                        link.ToolTip = url;
                        return link;
                    }
                }
                break;

            case "filesize":
                return DataHelper.GetSizeString(ValidationHelper.GetLong(parameter, 0));
        }

        return parameter.ToString();
    }


    /// <summary>
    /// Unigrid Action event handler.
    /// </summary>
    private void UniGrid_OnAction(string actionName, object actionArgument)
    {
        if (actionName == "delete")
        {
            if (!CheckPermissions("cms.forums", PERMISSION_MODIFY))
            {
                return;
            }

            ForumAttachmentInfoProvider.DeleteForumAttachmentInfo(ValidationHelper.GetInteger(actionArgument, 0));
        }
    }


    protected void ForumNewPost1_OnCancelClick(object sender, EventArgs e)
    {
        URLHelper.Redirect("ForumPost_View.aspx?postid=" + PostID + listingParameter);
    }


    /// <summary>
    /// On insert.
    /// </summary>
    private void ForumNewPost1_OnInsertPost(object sender, EventArgs e)
    {
        ltlScript.Text += ScriptHelper.GetScript("parent.frames['posts_tree'].location.href = 'ForumPost_Tree.aspx?postid=" + PostEdit1.EditPostID + "&forumid=" + ForumID.ToString() + "';");
        ltlScript.Text += ScriptHelper.GetScript("parent.frames['posts_edit'].location.href = 'ForumPost_View.aspx?postid=" + PostEdit1.EditPostID + listingParameter + "';");
    }


    protected void InitializeMenu()
    {
        // Check if maximum post level is not exceeded        
        bool maxLevelExceeded = (PostInfo != null) && (PostInfo.PostLevel >= ForumPostInfoProvider.MaxPostLevel);

        var actionsElem = ((CMSPage)Page).HeaderActions;
        actionsElem.AddAction(new HeaderAction()
        {
            Text = GetString("general.edit"),
            Tooltip = GetString("ForumPost_View.EditToolTip"),
            OnClientClick = "EditPost(" + PostID + "); return false;"
        });
        actionsElem.AddAction(new HeaderAction()
        {
            Text = GetString("general.delete"),
            Tooltip = GetString("ForumPost_View.DeleteToolTip"),
            OnClientClick = "DeletePost(" + PostID + "); return false;"
        });
        actionsElem.AddAction(new HeaderAction()
        {
            Text = GetString("ForumPost_View.IconReply"),
            Tooltip = GetString("ForumPost_View.ReplyToolTip"),
            OnClientClick = maxLevelExceeded ? "return false;" : "ReplyToPost(" + PostID + "); return false;",
            Enabled = !maxLevelExceeded,
        });

        // Stick thread
        if (PostInfo.PostLevel == 0)
        {
            ThreadMove1.SetValue("AdminMode", true);
            ThreadMove1.SetValue("SelectedThreadID", PostInfo.PostId);

            actionsElem.AddAction(new HeaderAction()
            {
                Text = GetString("ForumPost_View.IconMove"),
                Tooltip = GetString("ForumPost_View.MoveToolTip"),
                OnClientClick = ControlsHelper.GetPostBackEventReference(btnMoveThread, null) + "; return false;"
            });

            bool isUnstick = (PostInfo.PostStickOrder > 0);
            actionsElem.AddAction(new HeaderAction()
            {
                Text = isUnstick ? GetString("ForumPost_View.IconUnStick") : GetString("ForumPost_View.IconStick"),
                Tooltip = isUnstick ? GetString("ForumPost_View.UnStickToolTip") : GetString("ForumPost_View.StickToolTip"),
                OnClientClick = "StickThread(" + PostID + "); return false;"
            });

            bool isLocked = PostInfo.PostIsLocked;
            actionsElem.AddAction(new HeaderAction()
            {
                Text = isLocked ? GetString("ForumPost_View.IconUnLock") : GetString("ForumPost_View.IconLock"),
                Tooltip = isLocked ? GetString("ForumPost_View.UnLockToolTip") : GetString("ForumPost_View.LockToolTip"),
                OnClientClick = "LockThread(" + PostID + "); return false;"
            });
        }
        else
        {
            actionsElem.AddAction(new HeaderAction()
            {
                Text = GetString("ForumPost_View.IconSplit"),
                Tooltip = GetString("ForumPost_View.SplitToolTip"),
                OnClientClick = "SplitThread(" + PostID + "); return false;"
            });

            //Register split confirmation script
            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ForumSplitConfirm",
                                                   ScriptHelper.GetScript(" function ForumSplitConfirm() {\n" +
                                                                          "if (confirm(" + ScriptHelper.GetString(GetString("ForumPost_View.SplitConfirmation")) + ")) { \n ;SplitThread(" + PostID.ToString() + "); return false; \n}else{ return false; }\n}"
                                                       ));
        }

        bool isApproved = PostInfo.PostApproved;
        actionsElem.AddAction(new HeaderAction()
        {
            Text = isApproved ? GetString("general.reject") : GetString("general.approve"),
            Tooltip = isApproved ? GetString("ForumPost_View.RejectToolTip") : GetString("ForumPost_View.ApproveToolTip"),
            OnClientClick = "ApprovePost(" + PostID + "); return false;"
        });


        if (isApproved)
        {
            actionsElem.AddAction(new HeaderAction()
            {
                Text = GetString("ForumPost_View.IconRejectSubTree"),
                Tooltip = GetString("ForumPost_View.RejectSubTreeToolTip"),
                OnClientClick = "RejectSubTree(" + PostID + "); return false;"
            });
        }
        else
        {
            actionsElem.AddAction(new HeaderAction()
            {
                Text = GetString("ForumPost_View.IconApproveSubTree"),
                Tooltip = GetString("ForumPost_View.ApproveSubTreeToolTip"),
                OnClientClick = "ApproveSubTree(" + PostID + "); return false;"
            });
        }
    }


    /// <summary>
    /// OnPrerender override.
    /// </summary>
    /// <param name="e">EventArgs</param>
    protected override void OnPreRender(EventArgs e)
    {
        if (ValidationHelper.GetBoolean(ThreadMove1.GetValue("TopicMoved"), false))
        {
            ltlScript.Text += ScriptHelper.GetScript("if(parent){ parent.location = parent.location} else {location = location}");
            pnlContent.Visible = false;
            pnlAttachmentContent.Visible = false;
            pnlAttachmentTitle.Visible = false;
        }

        base.OnPreRender(e);
    }


    /// <summary>
    /// Stick thread event handler.
    /// </summary>
    protected void btnSplitThread_Click(object sender, EventArgs e)
    {
        if (!CheckPermissions("cms.forums", PERMISSION_MODIFY))
        {
            return;
        }

        if (PostInfo != null)
        {
            ForumPostInfoProvider.SplitThread(PostInfo);

            ltlScript.Text += ScriptHelper.GetScript("parent.frames['posts_tree'].location.href = 'ForumPost_Tree.aspx?postid=" + PostInfo.PostId + "&forumid=" + PostInfo.PostForumID + "';");
            ltlScript.Text += ScriptHelper.GetScript("parent.frames['posts_edit'].location.href = 'ForumPost_View.aspx?postid=" + PostID + listingParameter + "';");
        }
    }


    /// <summary>
    /// Stick thread event handler.
    /// </summary>
    protected void btnStickThread_Click(object sender, EventArgs e)
    {
        if (!CheckPermissions("cms.forums", PERMISSION_MODIFY))
        {
            return;
        }

        if (PostInfo != null)
        {
            if (PostInfo.PostStickOrder > 0)
            {
                ForumPostInfoProvider.UnstickThread(PostInfo);
            }
            else
            {
                ForumPostInfoProvider.StickThread(PostInfo);
            }

            ltlScript.Text += ScriptHelper.GetScript("parent.frames['posts_tree'].location.href = 'ForumPost_Tree.aspx?postid=" + PostInfo.PostId + "&forumid=" + PostInfo.PostForumID + "';");
            ltlScript.Text += ScriptHelper.GetScript("parent.frames['posts_edit'].location.href = 'ForumPost_View.aspx?postid=" + PostID + listingParameter + "';");
        }
    }


    /// <summary>
    /// Lock thread event handler.
    /// </summary>
    protected void btnLockThread_Click(object sender, EventArgs e)
    {
        if (!CheckPermissions("cms.forums", PERMISSION_MODIFY))
        {
            return;
        }

        if (PostInfo != null)
        {
            PostInfo.PostIsLocked = !PostInfo.PostIsLocked;
            ForumPostInfoProvider.SetForumPostInfo(PostInfo);

            ltlScript.Text += ScriptHelper.GetScript("parent.frames['posts_tree'].location.href = 'ForumPost_Tree.aspx?postid=" + PostInfo.PostId + "&forumid=" + PostInfo.PostForumID + "';");
            ltlScript.Text += ScriptHelper.GetScript("parent.frames['posts_edit'].location.href = 'ForumPost_View.aspx?postid=" + PostID + listingParameter + "';");
        }
    }


    /// <summary>
    /// This function is executed by callback initiated by 'Delete' button in menu.
    /// </summary>
    protected void btnDelete_Click(object sender, EventArgs e)
    {
        if (!CheckPermissions("cms.forums", PERMISSION_MODIFY))
        {
            return;
        }

        ForumPostInfoProvider.DeleteForumPostInfo(PostID);

        if (PostInfo.PostParentID == 0)
        {
            //reload post edit frames with actual data
            ltlScript.Text += ScriptHelper.GetScript("parent.frames['posts_tree'].location.href = 'ForumPost_Tree.aspx?forumid=" + PostInfo.PostForumID + "';");
            ltlScript.Text += ScriptHelper.GetScript("parent.frames['posts_edit'].location.href = 'ForumPost_View.aspx?forumid=" + PostInfo.PostForumID + listingParameter + "';");
        }
        else
        {
            //reload post edit frames with actual data
            ltlScript.Text += ScriptHelper.GetScript("parent.frames['posts_tree'].location.href = 'ForumPost_Tree.aspx?postid=" + PostInfo.PostParentID + "&forumid=" + PostInfo.PostForumID + "';");
            ltlScript.Text += ScriptHelper.GetScript("parent.frames['posts_edit'].location.href = 'ForumPost_View.aspx?postid=" + PostInfo.PostParentID + listingParameter + "';");
        }
    }


    /// <summary>
    /// This function is executed by callback initiated by 'Approve' button in menu.
    /// It can be either Approve or Reject.
    /// </summary>
    protected void btnApprove_Click(object sender, EventArgs e)
    {
        if (!CheckPermissions("cms.forums", PERMISSION_MODIFY))
        {
            return;
        }

        // if post is approved
        if (PostInfo.PostApproved)
        {
            //reject post
            PostInfo.Reject();
        }
        else
        {
            PostInfo.Approve();
        }

        ltlScript.Text += ScriptHelper.GetScript("parent.frames['posts_tree'].location.href = 'ForumPost_Tree.aspx?postid=" + PostInfo.PostId + "&forumid=" + PostInfo.PostForumID + "';");
        ltlScript.Text += ScriptHelper.GetScript("parent.frames['posts_edit'].location.href = 'ForumPost_View.aspx?postid=" + PostID + listingParameter + "';");
    }


    /// <summary>
    /// Reject post and all child posts.
    /// </summary>
    protected void btnRejectSubTree_Click(object sender, EventArgs e)
    {
        if (!CheckPermissions("cms.forums", PERMISSION_MODIFY))
        {
            return;
        }

        if (PostInfo != null)
        {
            // Reject with sub-posts
            PostInfo.Reject(true);

            ltlScript.Text += ScriptHelper.GetScript("parent.frames['posts_tree'].location.href = 'ForumPost_Tree.aspx?postid=" + PostInfo.PostId + "&forumid=" + PostInfo.PostForumID + "';");
            ltlScript.Text += ScriptHelper.GetScript("parent.frames['posts_edit'].location.href = 'ForumPost_View.aspx?postid=" + PostID + listingParameter + "';");
        }
    }


    /// <summary>
    /// Approve post and all child posts.
    /// </summary>
    protected void btnApproveSubTree_Click(object sender, EventArgs e)
    {
        if (!CheckPermissions("cms.forums", PERMISSION_MODIFY))
        {
            return;
        }

        if ((PostInfo != null) && (MembershipContext.AuthenticatedUser != null))
        {
            // Approve with subtree
            PostInfo.Approve(MembershipContext.AuthenticatedUser.UserID, true);

            ltlScript.Text += ScriptHelper.GetScript("parent.frames['posts_tree'].location.href = 'ForumPost_Tree.aspx?postid=" + PostInfo.PostId + "&forumid=" + PostInfo.PostForumID + "';");
            ltlScript.Text += ScriptHelper.GetScript("parent.frames['posts_edit'].location.href = 'ForumPost_View.aspx?postid=" + PostID + listingParameter + "';");
        }
    }


    /// <summary>
    /// Upload click handler.
    /// </summary>
    protected void btnUpload_Click(object sender, EventArgs e)
    {
        if (!CheckPermissions("cms.forums", PERMISSION_MODIFY))
        {
            return;
        }

        if ((upload.HasFile) && (PostInfo != null))
        {
            // Check attachment extension
            if (!ForumAttachmentInfoProvider.IsExtensionAllowed(upload.FileName, SiteContext.CurrentSiteName))
            {
                ShowError(GetString("ForumAttachment.AttachmentIsNotAllowed"));
                return;
            }

            ForumInfo fi = ForumInfoProvider.GetForumInfo(PostInfo.PostForumID);
            if (fi != null)
            {
                ForumGroupInfo fgi = ForumGroupInfoProvider.GetForumGroupInfo(fi.ForumGroupID);
                if (fgi != null)
                {
                    ForumAttachmentInfo fai = new ForumAttachmentInfo(upload.PostedFile, 0, 0, fi.ForumImageMaxSideSize);
                    fai.AttachmentPostID = PostInfo.PostId;
                    fai.AttachmentSiteID = fgi.GroupSiteID;
                    ForumAttachmentInfoProvider.SetForumAttachmentInfo(fai);

                    ReloadAttachmentData(PostInfo.PostId);
                    UniGrid.ReloadData();
                }
            }
        }
    }


    /// <summary>
    /// Moves thread click handler.
    /// </summary>
    protected void btnMoveThread_Click(object sender, EventArgs e)
    {
        if (!CheckPermissions("cms.forums", PERMISSION_MODIFY))
        {
            return;
        }

        pnlMove.Visible = !pnlMove.Visible;
    }
}