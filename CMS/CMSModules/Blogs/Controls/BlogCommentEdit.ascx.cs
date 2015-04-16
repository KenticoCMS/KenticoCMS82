using System;
using System.Collections.Generic;

using CMS.Blogs;
using CMS.Globalization;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.Base;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.DocumentEngine;
using CMS.UIControls;
using CMS.WebAnalytics;
using CMS.Protection;

public partial class CMSModules_Blogs_Controls_BlogCommentEdit : CMSUserControl
{
    #region "Variables"

    private int mPostDocumentId;
    private int mCommentId;
    private string mOkButtonText;
    private string mCommentSavedText;
    private bool mClearFormAfterSave;
    private bool mUseCaptcha;
    protected string mValidationGroup = null;


    public CMSModules_Blogs_Controls_BlogCommentEdit()
    {
        EnableSubscriptions = false;
        AdvancedMode = false;
        RequireEmails = false;
    }


    public event OnBeforeCommentSavedEventHandler OnBeforeCommentSaved;
    public event OnAfterCommentSavedEventHandler OnAfterCommentSaved;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Post document ID. Set when creating new comment to the post.
    /// </summary>
    public int PostDocumentId
    {
        get
        {
            return mPostDocumentId;
        }
        set
        {
            mPostDocumentId = value;
        }
    }


    /// <summary>
    /// Gets or sets document node ID.
    /// </summary>
    public int PostNodeId
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets document culture.
    /// </summary>
    public string PostCulture
    {
        get;
        set;
    }


    /// <summary>
    /// Comment ID. Set when editing existing comment.
    /// </summary>
    public int CommentId
    {
        get
        {
            return mCommentId;
        }
        set
        {
            mCommentId = value;
        }
    }


    /// <summary>
    /// Ok button text.
    /// </summary>
    public string OkButtonText
    {
        get
        {
            return mOkButtonText;
        }
        set
        {
            mOkButtonText = value;
        }
    }


    /// <summary>
    /// Comment saved text.
    /// </summary>
    public string CommentSavedText
    {
        get
        {
            return mCommentSavedText;
        }
        set
        {
            mCommentSavedText = value;
        }
    }


    /// <summary>
    /// Indicates whether form fields should be cleared after data are saved.
    /// </summary>
    public bool ClearFormAfterSave
    {
        get
        {
            return mClearFormAfterSave;
        }
        set
        {
            mClearFormAfterSave = value;
        }
    }


    /// <summary>
    /// Indicates whether security code control should be displayed.
    /// </summary>
    public bool UseCaptcha
    {
        get
        {
            return mUseCaptcha;
        }
        set
        {
            mUseCaptcha = value;
        }
    }


    /// <summary>
    /// Indicates whether advanced mode controls should be displayed.
    /// </summary>
    public bool AdvancedMode
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates whether subscription is allowed.
    /// </summary>
    public bool EnableSubscriptions
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates whether e-mail is required.
    /// </summary>
    public bool RequireEmails
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if buttons should be displayed.
    /// </summary>
    public bool DisplayButtons
    {
        get
        {
            return plcButtons.Visible;
        }
        set
        {
            plcButtons.Visible = value;
        }
    }


    /// <summary>
    /// Validation group of controls.
    /// </summary>
    public string ValidationGroup
    {
        get
        {
            return mValidationGroup ?? (mValidationGroup = "CommentEdit_" + Guid.NewGuid().ToString("N"));
        }
        set
        {
            mValidationGroup = value;
        }
    }

    #endregion


    #region "Private properties"

    /// <summary>
    /// Indicates whether the control is displayed within the insert mode.
    /// </summary>
    private bool IsInsertMode
    {
        get
        {
            // Insert mode
            if (mPostDocumentId > 0)
            {
                return true;
            }
            // Edit mode
            else
            {
                return false;
            }
        }
    }


    /// <summary>
    /// CSS used for the live site mode.
    /// </summary>
    protected string LiveSiteCss
    {
        get
        {
            if (PortalContext.ViewMode != ViewModeEnum.LiveSite)
            {
                return "BlogBreakLine";
            }
            return null;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        mOkButtonText = GetString("general.add");
        mCommentSavedText = GetString("Blog.CommentEdit.CommentSaved");

        // Basic control initialization
        lblName.Text = GetString("Blog.CommentEdit.lblName");
        lblUrl.Text = GetString("Blog.CommentEdit.lblUrl");
        lblComments.Text = GetString("Blog.CommentEdit.lblComments");
        btnOk.Text = mOkButtonText;

        // Validators initialization
        rfvComments.ErrorMessage = GetString("Blog.CommentEdit.CommentEmpty");
        rfvName.ErrorMessage = GetString("Blog.CommentEdit.NameEmpty");
        revEmailValid.ErrorMessage = GetString("general.correctemailformat");
        rfvEmail.ErrorMessage = GetString("blog.commentedit.rfvemail");
        rfvEmail.Enabled = RequireEmails;

        // Show or hide security code control
        plcCaptcha.Visible = mUseCaptcha;

        chkApproved.ResourceString = GetString("Blog.CommentEdit.Approved");
        chkSpam.ResourceString = GetString("Blog.CommentEdit.Spam");
        lblInserted.Text = GetString("Blog.CommentEdit.Inserted");

        revEmailValid.ValidationExpression = ValidationHelper.EmailRegExp.ToString();

        plcChkSubscribe.Visible = EnableSubscriptions;

        revEmailValid.ValidationGroup = ValidationGroup;
        rfvComments.ValidationGroup = ValidationGroup;
        rfvEmail.ValidationGroup = ValidationGroup;
        rfvName.ValidationGroup = ValidationGroup;
        btnOk.ValidationGroup = ValidationGroup;

        if (!RequestHelper.IsPostBack())
        {
            var currentUser = MembershipContext.AuthenticatedUser;

            // New comment
            if (IsInsertMode)
            {
                // Prefill current user fullname
                if ((currentUser != null) && (!currentUser.IsPublic()))
                {
                    txtName.Text = currentUser.UserNickName != "" ? currentUser.UserNickName : currentUser.FullName;
                    txtEmail.Text = currentUser.Email;
                }
            }
            // Existing comment
            else
            {
                LoadCommentData();
            }

            // Advaced mode in CMSDesk
            if (AdvancedMode)
            {
                plcAdvancedMode.Visible = true;
            }
        }

        // Ensure information is displayed to the user
        bool savedRequiresApprove = QueryHelper.GetBoolean("saveda", false);
        bool saved = QueryHelper.GetBoolean("saved", false);
        if (saved || savedRequiresApprove)
        {
            if (savedRequiresApprove)
            {
                CommentSavedText = GetString("blog.comments.requiresmoderationafteraction");
            }

            lblInfo.Text = CommentSavedText;
            lblInfo.Visible = true;
        }
    }


    /// <summary>
    /// Fill form with the comment data.
    /// </summary>
    protected void LoadCommentData()
    {
        // Get comment info from database
        BlogCommentInfo bci = BlogCommentInfoProvider.GetBlogCommentInfo(mCommentId);
        if (bci != null)
        {
            txtName.Text = bci.CommentUserName;
            txtUrl.Text = bci.CommentUrl;
            txtComments.Text = bci.CommentText;
            txtEmail.Text = bci.CommentEmail;
            chkApproved.Checked = bci.CommentApproved;
            chkSpam.Checked = bci.CommentIsSpam;

            if (PortalContext.ViewMode.IsLiveSite() && (MembershipContext.AuthenticatedUser != null))
            {
                lblInsertedDate.Text = TimeZoneMethods.ConvertDateTime(bci.CommentDate, this).ToString();
            }
            else
            {
                lblInsertedDate.Text = bci.CommentDate.ToString();
            }
        }
    }


    protected void btnOk_Click(object sender, EventArgs e)
    {
        PerformAction();
    }


    public void PerformAction()
    {
        // Check banned ip
        if (!BannedIPInfoProvider.IsAllowed(SiteContext.CurrentSiteName, BanControlEnum.AllNonComplete))
        {
            lblError.Visible = true;
            lblError.Text = GetString("General.BannedIP");
            return;
        }

        if (OnBeforeCommentSaved != null)
        {
            OnBeforeCommentSaved();
        }

        // Validate form
        string errorMessage = ValidateForm();
        if (errorMessage == "")
        {
            // Check flooding when message being inserted through the LiveSite
            if (IsLiveSite && FloodProtectionHelper.CheckFlooding(SiteContext.CurrentSiteName, MembershipContext.AuthenticatedUser))
            {
                lblError.Visible = true;
                lblError.Text = GetString("General.FloodProtection");
                return;
            }

            var currentUser = MembershipContext.AuthenticatedUser;

            // Create new comment
            BlogCommentInfo bci;
            if (IsInsertMode)
            {
                bci = new BlogCommentInfo();
                bci.CommentDate = DateTime.Now;
                bci.CommentPostDocumentID = mPostDocumentId;

                // User IP address
                bci.CommentInfo.IPAddress = RequestContext.UserHostAddress;
                // User agent
                bci.CommentInfo.Agent = Request.UserAgent;

                if (!currentUser.IsPublic())
                {
                    bci.CommentUserID = currentUser.UserID;
                }
                bci.CommentIsTrackback = false;
            }
            // Get existing comment
            else
            {
                bci = BlogCommentInfoProvider.GetBlogCommentInfo(mCommentId);
            }

            // Update basic comment properties
            if (bci != null)
            {
                // Add http:// if needed
                string url = txtUrl.Text.Trim();
                if (url != "")
                {
                    if ((!url.ToLowerCSafe().StartsWithCSafe("http://")) && (!url.ToLowerCSafe().StartsWithCSafe("https://")))
                    {
                        url = "http://" + url;
                    }
                }

                bci.CommentIsSpam = chkSpam.Checked;
                bci.CommentApproved = chkApproved.Checked;
                bci.CommentUserName = txtName.Text.Trim();
                bci.CommentUrl = url;
                bci.CommentText = txtComments.Text.Trim();
                bci.CommentUrl = bci.CommentUrl.ToLowerCSafe().Replace("javascript", "_javascript");
                bci.CommentEmail = txtEmail.Text.Trim();
            }

            if (IsInsertMode)
            {
                // Auto approve owner comments
                if (bci != null)
                {
                    TreeNode blogNode = BlogHelper.GetParentBlog(bci.CommentPostDocumentID, false);
                    if ((currentUser != null) && (blogNode != null))
                    {
                        bool isAuthorized = BlogHelper.IsUserAuthorizedToManageComments(blogNode);
                        if (isAuthorized)
                        {
                            bci.CommentApprovedByUserID = blogNode.NodeOwner;
                            bci.CommentApproved = true;
                        }
                        else
                        {
                            // Is blog moderated ?
                            bool moderated = ValidationHelper.GetBoolean(blogNode.GetValue("BlogModerateComments"), false);

                            bci.CommentApprovedByUserID = 0;
                            bci.CommentApproved = !moderated;
                        }
                    }
                }
            }

            // Perform bad words check
            if (!BadWordInfoProvider.CanUseBadWords(MembershipContext.AuthenticatedUser, SiteContext.CurrentSiteName))
            {
                if (bci != null)
                {
                    // Prepare columns to check
                    Dictionary<string, int> columns = new Dictionary<string, int>();
                    columns.Add("CommentText", 0);
                    columns.Add("CommentUserName", 200);

                    // Perform bad words to check
                    errorMessage = BadWordsHelper.CheckBadWords(bci, columns, "CommentApproved", "CommentApprovedByUserID", bci.CommentText, MembershipContext.AuthenticatedUser.UserID, () => ValidateComment(bci));
                }
            }

            if (errorMessage == string.Empty)
            {
                if (bci != null)
                {
                    if (!ValidateComment(bci))
                    {
                        // Show error message
                        lblError.Visible = true;
                        lblError.Text = GetString("Blog.CommentEdit.EmptyBadWord");
                    }
                    else
                    {
                        // Subscribe new subscriber
                        if (chkSubscribe.Checked)
                        {
                            // Check for duplicate subscriptions
                            BlogPostSubscriptionInfo bpsi = BlogPostSubscriptionInfoProvider.GetBlogPostSubscriptionInfo(txtEmail.Text, mPostDocumentId);
                            if ((bpsi == null) || !bpsi.SubscriptionApproved)
                            {
                                bpsi = new BlogPostSubscriptionInfo();
                                bpsi.SubscriptionEmail = txtEmail.Text;
                                bpsi.SubscriptionPostDocumentID = mPostDocumentId;
                                bpsi.SubscriptionUserID = bci.CommentUserID;
                                BlogPostSubscriptionInfoProvider.Subscribe(bpsi, DateTime.Now, true, true);

                                if (bpsi.SubscriptionApproved)
                                {
                                    LogRegistrationActivity(bpsi);
                                }
                            }
                            else
                            {
                                errorMessage = GetString("blog.subscription.emailexists");
                            }
                        }

                        if (errorMessage == "")
                        {
                            // Save changes to database
                            BlogCommentInfoProvider.SetBlogCommentInfo(bci);

                            if (!bci.CommentApproved)
                            {
                                CommentSavedText = GetString("blog.comments.requiresmoderationafteraction");
                            }

                            // Inform user
                            lblInfo.Visible = true;
                            lblInfo.Text = CommentSavedText;

                            // Clear form when required
                            if (mClearFormAfterSave)
                            {
                                txtComments.Text = "";
                                txtUrl.Text = "";
                                ctrlCaptcha.Value = "";
                            }

                            LogCommentActivity(bci, PostNodeId, PostCulture);

                            if (OnAfterCommentSaved != null)
                            {
                                OnAfterCommentSaved(bci);
                            }
                        }
                    }
                }
            }
        }

        if (errorMessage != "")
        {
            // Show error message
            lblError.Visible = true;
            lblError.Text = errorMessage;
        }
    }


    private static bool ValidateComment(BlogCommentInfo commentInfo)
    {
        if ((commentInfo.CommentText != null) && (commentInfo.CommentUserName != null))
        {
            return (commentInfo.CommentText.Trim() != "") && (commentInfo.CommentUserName.Trim() != "");
        }
        return false;
    }


    /// <summary>
    /// Logs activity.
    /// </summary>
    /// <param name="bci">Blog comment info</param>
    /// <param name="nodeId">Document node ID</param>
    /// <param name="culture">Document culture</param>
    private void LogCommentActivity(BlogCommentInfo bci, int nodeId, string culture)
    {
        TreeNode blogPost = DocumentHelper.GetDocument(bci.CommentPostDocumentID, new TreeProvider());
        TreeNode blogNode = BlogHelper.GetParentBlog(bci.CommentPostDocumentID, false);
        string blogName = (blogNode != null) ? blogNode.GetDocumentName() : null;

        var blogComment = new ActivityBlogComment(bci, blogPost, AnalyticsContext.ActivityEnvironmentVariables);
        if (blogComment.Data != null)
        {
            blogComment.Data.TitleData = blogName;
            blogComment.Data.ItemDetailID = (blogNode != null ? blogNode.NodeID : 0);
            blogComment.Data.NodeID = nodeId;
            blogComment.Data.Culture = culture;
            blogComment.Log();
        }
    }


    /// <summary>
    /// Logs registration activity.
    /// </summary>
    /// <param name="bpsi">Blog subscription info</param>
    private void LogRegistrationActivity(BlogPostSubscriptionInfo bpsi)
    {
        if ((bpsi != null) && (bpsi.SubscriptionPostDocumentID > 0))
        {
            TreeNode blogPost = DocumentHelper.GetDocument(bpsi.SubscriptionPostDocumentID, new TreeProvider());
            TreeNode blogNode = BlogHelper.GetParentBlog(bpsi.SubscriptionPostDocumentID, false);
            string blogName = blogNode != null ? blogNode.GetDocumentName() : null;

            Activity activity = new ActivitySubscriptionBlogPost(blogName, blogNode, blogPost, bpsi, AnalyticsContext.ActivityEnvironmentVariables);
            activity.Log();
        }
    }


    /// <summary>
    /// Validates form.
    /// </summary>
    protected string ValidateForm()
    {
        string errorMessage = new Validator().NotEmpty(txtComments.Text.Trim(), rfvComments.ErrorMessage).NotEmpty(txtName.Text.Trim(), rfvName.ErrorMessage).Result;

        if ((mUseCaptcha) && (errorMessage == ""))
        {
            // Check whether security code is correct
            if (!ctrlCaptcha.IsValid())
            {
                errorMessage = ctrlCaptcha.ValidationError;
            }
        }

        // Check if e-mail address is required
        if ((errorMessage == "") && (RequireEmails || chkSubscribe.Checked))
        {
            errorMessage = new Validator().NotEmpty(txtEmail.Text, GetString("blog.subscription.noemail")).Result;
        }

        // Check e-mail address format if some e-mail adrress is specified
        if ((errorMessage == "") && !String.IsNullOrEmpty(txtEmail.Text))
        {
            errorMessage = new Validator().IsEmail(txtEmail.Text, GetString("general.correctemailformat")).Result;
        }

        return errorMessage;
    }
}