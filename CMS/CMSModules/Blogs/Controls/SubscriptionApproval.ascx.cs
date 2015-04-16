using System;
using System.Web.UI;

using CMS.ExtendedControls;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.Helpers;
using CMS.Blogs;
using CMS.WebAnalytics;
using CMS.DocumentEngine;

using TreeNode = CMS.DocumentEngine.TreeNode;

public partial class CMSModules_Blogs_Controls_SubscriptionApproval : CMSUserControl
{
    #region "Private variables"

    private string mSubscriptionHash = null;
    private string mRequestTime = null;
    private BlogPostSubscriptionInfo mSubscriptionObject = null;
    private TreeNode mSubscriptionSubject = null;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets successful approval text.
    /// </summary>
    public string SuccessfulConfirmationText
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets unsuccessful approval text.
    /// </summary>
    public string UnsuccessfulConfirmationText
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the confirmation info label text.
    /// </summary>
    public string ConfirmationInfoText
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the confirmation button text.
    /// </summary>
    public string ConfirmationButtonText
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the confirmation label CSS class.
    /// </summary>
    public string ConfirmationTextCssClass
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the confirmation button CSS class.
    /// </summary>
    public string ConfirmationButtonCssClass
    {
        get;
        set;
    }


    /// <summary>
    /// Get message board subscription object
    /// </summary>
    private BlogPostSubscriptionInfo SubscriptionObject
    {
        get
        {
            if (mSubscriptionObject == null)
            {
                mSubscriptionObject = BlogPostSubscriptionInfoProvider.GetBlogPostSubscriptionInfo(mSubscriptionHash);
            }

            return mSubscriptionObject;
        }
    }


    /// <summary>
    /// Get subject of subscription
    /// </summary>
    public TreeNode SubscriptionSubject
    {
        get
        {
            if ((mSubscriptionSubject == null) && (SubscriptionObject != null))
            {
                // Get blog post
                TreeProvider tp = new TreeProvider();
                mSubscriptionSubject = tp.SelectSingleDocument(SubscriptionObject.SubscriptionPostDocumentID);
            }

            return mSubscriptionSubject;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // If StopProcessing flag is set, do nothing
        if (StopProcessing)
        {
            Visible = false;
            return;
        }

        // Validate hash
        var settings = new HashSettings
        {
            UserSpecific = false
        };

        if (!QueryHelper.ValidateHash("hash", "aliaspath", settings))
        {
            URLHelper.Redirect(ResolveUrl("~/CMSMessages/Error.aspx?title=" + ResHelper.GetString("dialogs.badhashtitle") + "&text=" + ResHelper.GetString("dialogs.badhashtext")));
        }

        // Get data from query string
        mSubscriptionHash = QueryHelper.GetString("blogsubscriptionhash", string.Empty);
        mRequestTime = QueryHelper.GetString("datetime", string.Empty);
            
        bool controlPb = false;

        if (RequestHelper.IsPostBack())
        {
            Control pbCtrl = ControlsHelper.GetPostBackControl(Page);
            if (pbCtrl == btnConfirm)
            {
                controlPb = true;
            }
        }

        // Setup controls
        SetupControls(!controlPb);

        if (!controlPb)
        {
            CheckAndSubscribe(mSubscriptionHash, mRequestTime, true);
        }
    }


    /// <summary>
    /// Button confirmation click event
    /// </summary>
    protected void btnConfirm_Click(object sender, EventArgs e)
    {
        CheckAndSubscribe(mSubscriptionHash, mRequestTime, false);
    }


    /// <summary>
    /// Initialize controls properties
    /// </summary>
    private void SetupControls(bool forceReload)
    {
        lblInfo.CssClass = DataHelper.GetNotEmpty(ConfirmationTextCssClass, "InfoLabel");
        btnConfirm.CssClass = ConfirmationButtonCssClass;

        if (forceReload)
        {
            btnConfirm.Text = DataHelper.GetNotEmpty(ConfirmationButtonText, GetString("general.subscription_confirmbutton"));
            lblInfo.Text = DataHelper.GetNotEmpty(ConfirmationInfoText, GetString("general.subscription_confirmtext"));
        }
    }


    /// <summary>
    /// Check that subscription hash is valid and subscription didn't expire
    /// </summary>
    /// <param name="subscriptionHash">Subscription hash to check</param>
    /// <param name="requestTime">Date time of subscription request</param>
    /// <param name="checkOnly">Indicates if only check will be performed</param>
    private void CheckAndSubscribe(string subscriptionHash, string requestTime, bool checkOnly)
    {
        // Get date and time
        DateTime datetime = DateTimeHelper.ZERO_TIME;

        // Get date and time
        if (!string.IsNullOrEmpty(requestTime))
        {
            try
            {
                datetime = DateTime.ParseExact(requestTime, SecurityHelper.EMAIL_CONFIRMATION_DATETIME_FORMAT, null);
            }
            catch
            {
                DisplayError(DataHelper.GetNotEmpty(UnsuccessfulConfirmationText, GetString("general.subscription_failed")));
                return;
            }
        }

        // Initialize opt-in result
        OptInApprovalResultEnum result = OptInApprovalResultEnum.NotFound;

        // Check only data consistency
        if (checkOnly)
        {
            // Validate hash 
            result = BlogPostSubscriptionInfoProvider.ValidateHash(SubscriptionObject, subscriptionHash, SiteContext.CurrentSiteName, datetime);
            if ((result == OptInApprovalResultEnum.Success) && (SubscriptionObject.SubscriptionApproved))
            {
                result = OptInApprovalResultEnum.NotFound;
            }
        }
        else
        {
            // Try to approve subscription
            result = BlogPostSubscriptionInfoProvider.ApproveSubscription(SubscriptionObject, subscriptionHash, false, SiteContext.CurrentSiteName, datetime);
        }

        // Process result
        switch (result)
        {
            // Approving subscription was successful
            case OptInApprovalResultEnum.Success:
                if (!checkOnly)
                {
                    ShowInfo(DataHelper.GetNotEmpty(SuccessfulConfirmationText, GetString("general.subscription_approval")));
                    BlogPostSubscriptionInfoProvider.LogSubscriptionActivity(SubscriptionObject, QueryHelper.GetInteger("cid", 0), QueryHelper.GetInteger("siteid", 0), QueryHelper.GetText("url", ""), QueryHelper.GetText("camp", ""), PredefinedActivityType.SUBSCRIPTION_BLOG_POST, true);
                }
                break;

            // Subscription was already approved
            case OptInApprovalResultEnum.Failed:
                DisplayError(DataHelper.GetNotEmpty(UnsuccessfulConfirmationText, GetString("general.subscription_failed")));
                break;

            case OptInApprovalResultEnum.TimeExceeded:
                BlogPostSubscriptionInfoProvider.DeleteBlogPostSubscriptionInfo(SubscriptionObject);
                DisplayError(DataHelper.GetNotEmpty(UnsuccessfulConfirmationText, GetString("general.subscription_timeexceeded")));
                break;

            // Subscription not found
            default:
            case OptInApprovalResultEnum.NotFound:
                DisplayError(DataHelper.GetNotEmpty(UnsuccessfulConfirmationText, GetString("general.subscription_invalid")));
                break;
        }
    }

    #endregion


    #region "Helper methods"

    /// <summary>
    /// Display error message
    /// </summary>
    /// <param name="errorText">Error text to display</param>
    private void DisplayError(string errorText)
    {
        lblInfo.CssClass = "ErrorLabel";
        lblInfo.Text = errorText;
        btnConfirm.Visible = false;
    }


    /// <summary>
    /// Display information message
    /// </summary>
    /// <param name="infoText">Information to display</param>
    private void ShowInfo(string infoText)
    {
        lblInfo.CssClass = "InfoLabel";
        lblInfo.Text = infoText;
        btnConfirm.Visible = false;
    }

    #endregion
}