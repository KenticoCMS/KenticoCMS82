using System;
using System.Collections.Generic;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.Blogs;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.Base;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.DocumentEngine;
using CMS.UIControls;
using CMS.WebAnalytics;
using CMS.WorkflowEngine;

using TreeNode = CMS.DocumentEngine.TreeNode;
using CMS.Protection;

public partial class CMSModules_Blogs_Controls_NewSubscription : CMSUserControl
{
    #region "Private variables"

    private BlogProperties mBlogProperties = null;
    private int mDocumentId = 0;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Document ID.
    /// </summary>
    public int DocumentID
    {
        get
        {
            return mDocumentId;
        }
        set
        {
            mDocumentId = value;
        }
    }


    /// <summary>
    /// Gets or sets docuemnt node ID.
    /// </summary>
    public int NodeID
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets document culture.
    /// </summary>
    public string Culture
    {
        get;
        set;
    }


    /// <summary>
    /// Properties passed from the upper control.
    /// </summary>
    public BlogProperties BlogProperties
    {
        get
        {
            return mBlogProperties;
        }
        set
        {
            mBlogProperties = value;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        string valGroup = UniqueID;

        lblEmail.ResourceString = "blog.subscription.email";
        btnOk.ResourceString = "blog.subscription.subscribe";
        btnOk.ValidationGroup = valGroup;

        rfvEmailRequired.ErrorMessage = GetString("blog.subscription.noemail");
        rfvEmailRequired.ValidationGroup = valGroup;

        revEmailValid.ValidationGroup = valGroup;
        revEmailValid.ErrorMessage = GetString("general.correctemailformat");
        revEmailValid.ValidationExpression = ValidationHelper.EmailRegExp.ToString();
    }


    /// <summary>
    /// Pre-fill user e-mail.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (!RequestHelper.IsPostBack())
        {
            // Pre-fill user e-mail address to empty textbox for the first time
            if ((txtEmail.Text.Trim() == "") && (MembershipContext.AuthenticatedUser != null))
            {
                txtEmail.Text = MembershipContext.AuthenticatedUser.Email;
            }
        }
    }


    /// <summary>
    /// OK click handler.
    /// </summary>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        // Check banned IP
        if (!BannedIPInfoProvider.IsAllowed(SiteContext.CurrentSiteName, BanControlEnum.AllNonComplete))
        {
            lblError.Visible = true;
            lblError.Text = GetString("General.BannedIP");
            return;
        }

        // Check input fields
        string email = txtEmail.Text.Trim();
        string result = new Validator().NotEmpty(email, rfvEmailRequired.ErrorMessage)
            .IsEmail(email, GetString("general.correctemailformat")).Result;

        // Try to subscribe new subscriber
        if (result == "")
        {
            if (DocumentID > 0)
            {
                BlogPostSubscriptionInfo bpsi = BlogPostSubscriptionInfoProvider.GetBlogPostSubscriptionInfo(email, DocumentID);

                // Check for duplicit subscriptions
                if ((bpsi == null) || !bpsi.SubscriptionApproved)
                {
                    bpsi = new BlogPostSubscriptionInfo();
                    bpsi.SubscriptionPostDocumentID = DocumentID;
                    bpsi.SubscriptionEmail = email;

                    // Update user id for logged users (except the public users)
                    if ((MembershipContext.AuthenticatedUser != null) && (!MembershipContext.AuthenticatedUser.IsPublic()))
                    {
                        bpsi.SubscriptionUserID = MembershipContext.AuthenticatedUser.UserID;
                    }

                    BlogPostSubscriptionInfoProvider.Subscribe(bpsi, DateTime.Now, true, true);

                    lblInfo.Visible = true;
                    if (bpsi.SubscriptionApproved)
                    {
                        lblInfo.Text = GetString("blog.subscription.beensubscribed");
                        LogActivity(bpsi);
                    }
                    else
                    {
                        lblInfo.Text = GetString("general.subscribed.doubleoptin");
                        int optInInterval = BlogHelper.GetBlogDoubleOptInInterval(SiteContext.CurrentSiteName);
                        if (optInInterval > 0)
                        {
                            lblInfo.Text += "<br />" + string.Format(GetString("general.subscription_timeintervalwarning"), optInInterval);
                        }

                    }
                    // Clear form after successful subscription
                    txtEmail.Text = "";


                }
                else
                {
                    result = GetString("blog.subscription.emailexists");
                }
            }
            else
            {
                result = GetString("general.invalidid");
            }
        }

        if (result != String.Empty)
        {
            lblError.Visible = true;
            lblError.Text = result;
        }
    }

    #endregion


    /// <summary>
    /// Logs activity.
    /// </summary>
    /// <param name="bpsi">Blog subscription info</param>
    private void LogActivity(BlogPostSubscriptionInfo bpsi)
    {
        if ((bpsi != null) && (bpsi.SubscriptionPostDocumentID > 0))
        {
            TreeNode blogPost = DocumentHelper.GetDocument(bpsi.SubscriptionPostDocumentID, new TreeProvider());
            TreeNode blogNode = BlogHelper.GetParentBlog(bpsi.SubscriptionPostDocumentID, false);
            string blogName = (blogNode != null) ? blogNode.GetDocumentName() : null;

            Activity activity = new ActivitySubscriptionBlogPost(blogName, blogNode, blogPost, bpsi, AnalyticsContext.ActivityEnvironmentVariables);
            activity.Log();
        }
    }
}