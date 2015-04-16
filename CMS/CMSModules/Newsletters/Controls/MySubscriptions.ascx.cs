using System;
using System.Data;

using CMS.Helpers;
using CMS.Newsletters;
using CMS.Base;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;
using CMS.WebAnalytics;
using CMS.ExtendedControls;
using CMS.DataEngine;

public partial class CMSModules_Newsletters_Controls_MySubscriptions : CMSAdminControl
{
    #region "Variables"

    private SubscriberInfo sb = null;
    private bool mExternalUse = false;
    private int mCacheMinutes = 0;
    private string subscriberEmail = string.Empty;
    private bool userIsIdentified = false;
    private int mUserId = 0;
    private int mSiteId = 0;
    private string selectorValue = string.Empty;
    private string currentValues = string.Empty;
    private bool mSendConfirmationEmail = true;

    #endregion


    #region "Properties"


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
    /// Gets or sets the value that indicates whether send confirmation emails.
    /// </summary>
    public bool SendConfirmationEmail
    {
        get
        {
            return mSendConfirmationEmail;
        }
        set
        {
            mSendConfirmationEmail = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether this control is visible.
    /// </summary>
    public bool ForcedVisible
    {
        get
        {
            return plcMain.Visible;
        }
        set
        {
            plcMain.Visible = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether this control is used in other control.
    /// </summary>
    public bool ExternalUse
    {
        get
        {
            return mExternalUse;
        }
        set
        {
            mExternalUse = value;
        }
    }


    /// <summary>
    /// Gets or sets the WebPart cache minutes.
    /// </summary>
    public int CacheMinutes
    {
        get
        {
            return mCacheMinutes;
        }
        set
        {
            mCacheMinutes = value;
        }
    }


    /// <summary>
    /// Gets or sets current site ID.
    /// </summary>
    public int SiteID
    {
        get
        {
            return mSiteId;
        }
        set
        {
            mSiteId = value;
        }
    }


    /// <summary>
    /// Gets or sets current user ID.
    /// </summary>
    public int UserID
    {
        get
        {
            return mUserId;
        }
        set
        {
            mUserId = value;
        }
    }


    /// <summary>
    /// Indicates if selector control is on live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("IsLiveSite"), false);
        }
        set
        {
            SetValue("IsLiveSite", value);
            plcMess.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Last selector value.
    /// </summary>
    private string SelectorValue
    {
        get
        {
            if (string.IsNullOrEmpty(selectorValue))
            {
                // Try to get value from hidden field
                selectorValue = ValidationHelper.GetString(hdnValue.Value, string.Empty);
            }

            return selectorValue;
        }
        set
        {
            selectorValue = value;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// PageLoad.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (ExternalUse)
        {
            LoadData();
        }
    }


    /// <summary>
    /// Load data.
    /// </summary>
    public void LoadData()
    {
        if (StopProcessing)
        {
            // Hide control
            Visible = false;
        }
        else
        {
            SetContext();

            // Get specified user if used instead of current user
            UserInfo ui = null;
            if (UserID > 0)
            {
                ui = UserInfoProvider.GetUserInfo(UserID);
            }
            else
            {
                ui = MembershipContext.AuthenticatedUser;
            }

            // Get specified site ID instead of current site ID
            int siteId = 0;
            if (SiteID > 0)
            {
                siteId = SiteID;
            }
            else
            {
                siteId = SiteContext.CurrentSiteID;
            }

            usNewsletters.WhereCondition = "NewsletterSiteID = " + siteId;
            usNewsletters.OnSelectionChanged += new EventHandler(usNewsletters_OnSelectionChanged);
            usNewsletters.IsLiveSite = IsLiveSite;

            userIsIdentified = (ui != null) && (!ui.IsPublic()) && (ValidationHelper.IsEmail(ui.Email) || ValidationHelper.IsEmail(ui.UserName));
            if (userIsIdentified)
            {
                usNewsletters.Visible = true;

                // Try to get subscriber info with specified e-mail
                sb = SubscriberInfoProvider.GetSubscriberInfo(ui.Email, siteId);
                if (sb == null)
                {
                    // Try to get subscriber info according to user info
                    sb = SubscriberInfoProvider.GetSubscriberInfo(UserInfo.OBJECT_TYPE, ui.UserID, siteId);
                }

                // Get user e-mail address
                if (sb != null)
                {
                    subscriberEmail = sb.SubscriberEmail;

                    // Get selected newsletters
                    DataSet ds = SubscriberNewsletterInfoProvider.GetEnabledSubscriberNewsletters().WhereEquals("SubscriberID", sb.SubscriberID).Column("NewsletterID");
                    if (!DataHelper.DataSourceIsEmpty(ds))
                    {
                        currentValues = TextHelper.Join(";", DataHelper.GetStringValues(ds.Tables[0], "NewsletterID"));
                    }

                    // Load selected newsletters
                    if (!RequestHelper.IsPostBack() || !string.IsNullOrEmpty(DataHelper.GetNewItemsInList(SelectorValue, currentValues)))
                    {
                        usNewsletters.Value = currentValues;
                    }
                }

                // Try to get email address from user data
                if (string.IsNullOrEmpty(subscriberEmail))
                {
                    if (ValidationHelper.IsEmail(ui.Email))
                    {
                        subscriberEmail = ui.Email;
                    }
                    else if (ValidationHelper.IsEmail(ui.UserName))
                    {
                        subscriberEmail = ui.UserName;
                    }
                }
            }
            else
            {
                usNewsletters.Visible = false;

                if ((UserID > 0) && (MembershipContext.AuthenticatedUser.UserID == UserID))
                {
                    ShowInformation(GetString("MySubscriptions.CannotIdentify"));
                }
                else
                {
                    ShowInformation(GetString("MySubscriptions.CannotIdentifyUser"));
                }
            }

            ReleaseContext();
        }
    }


    /// <summary>
    /// Logs activity for subscribing/unsubscribing
    /// </summary>
    /// <param name="ui">User</param>
    /// <param name="newsletterId">Newsletter ID</param>
    /// <param name="subscribe">Subscribing/unsubscribing flag</param>
    private void LogActivity(UserInfo ui, int newsletterId, bool subscribe)
    {
        if ((sb == null) || (ui == null))
        {
            return;
        }

        // Log activity only if subscriber is User
        if ((sb.SubscriberType != null) && sb.SubscriberType.Equals(UserInfo.OBJECT_TYPE, StringComparison.InvariantCultureIgnoreCase))
        {
            NewsletterInfo news = NewsletterInfoProvider.GetNewsletterInfo(newsletterId);
            Activity activity;
            if (subscribe)
            {
                activity = new ActivityNewsletterSubscribing(sb, news, AnalyticsContext.ActivityEnvironmentVariables);
            }
            else
            {
                activity = new ActivityNewsletterUnsubscribing(sb, news, AnalyticsContext.ActivityEnvironmentVariables);
            }
            activity.Log();
        }
    }


    private void usNewsletters_OnSelectionChanged(object sender, EventArgs e)
    {
        if (RaiseOnCheckPermissions("ManageSubscribers", this))
        {
            if (StopProcessing)
            {
                return;
            }
        }

        // Get specified user if used instead of current user
        UserInfo ui = null;
        if (UserID > 0)
        {
            ui = UserInfoProvider.GetUserInfo(UserID);
        }
        else
        {
            ui = MembershipContext.AuthenticatedUser;
        }

        // Get specified site ID instead of current site ID
        int siteId = 0;
        if (SiteID > 0)
        {
            siteId = SiteID;
        }
        else
        {
            siteId = SiteContext.CurrentSiteID;
        }

        if ((sb == null) && (ui != null))
        {
            // Create new subsciber (bind to existing user account)
            if ((!ui.IsPublic()) && (ValidationHelper.IsEmail(ui.Email) || ValidationHelper.IsEmail(ui.UserName)))
            {
                sb = new SubscriberInfo();
                if (ui != null)
                {
                    if (!string.IsNullOrEmpty(ui.FirstName) && !string.IsNullOrEmpty(ui.LastName))
                    {
                        sb.SubscriberFirstName = ui.FirstName;
                        sb.SubscriberLastName = ui.LastName;
                    }
                    else
                    {
                        sb.SubscriberFirstName = ui.FullName;
                    }
                    // Full name consists of "user " and user full name
                    sb.SubscriberFullName = new SubscriberFullNameFormater().GetUserSubscriberName(ui.FullName);
                }

                sb.SubscriberSiteID = siteId;
                sb.SubscriberType = UserInfo.OBJECT_TYPE;
                sb.SubscriberRelatedID = ui.UserID;
                // Save subscriber to DB
                SubscriberInfoProvider.SetSubscriberInfo(sb);
            }
        }

        if (sb == null)
        {
            return;
        }

        // Create membership between current contact and subscriber
        ModuleCommands.OnlineMarketingCreateRelation(sb.SubscriberID, MembershipType.NEWSLETTER_SUBSCRIBER, ModuleCommands.OnlineMarketingGetCurrentContactID());

        // Remove old items
        int newsletterId = 0;
        string newValues = ValidationHelper.GetString(usNewsletters.Value, null);
        string items = DataHelper.GetNewItemsInList(newValues, currentValues);
        if (!String.IsNullOrEmpty(items))
        {
            string[] newItems = items.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (newItems != null)
            {
                foreach (string item in newItems)
                {
                    newsletterId = ValidationHelper.GetInteger(item, 0);

                    // If subscriber is subscribed, unsubscribe him
                    if (SubscriberInfoProvider.IsSubscribed(sb.SubscriberID, newsletterId))
                    {
                        SubscriberInfoProvider.Unsubscribe(sb.SubscriberID, newsletterId, SendConfirmationEmail);
                        // Log activity
                        LogActivity(ui, newsletterId, false);
                    }
                }
            }
        }

        // Add new items
        items = DataHelper.GetNewItemsInList(currentValues, newValues);
        if (!String.IsNullOrEmpty(items))
        {
            string[] newItems = items.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (newItems != null)
            {
                foreach (string item in newItems)
                {
                    newsletterId = ValidationHelper.GetInteger(item, 0);

                    // If subscriber is not subscribed, subscribe him
                    if (!SubscriberInfoProvider.IsSubscribed(sb.SubscriberID, newsletterId))
                    {
                        try
                        {
                            SubscriberInfoProvider.Subscribe(sb.SubscriberID, newsletterId, DateTime.Now, SendConfirmationEmail);
                            // Log activity
                            LogActivity(ui, newsletterId, true);
                        }
                        catch
                        {
                        }
                    }
                }
            }
        }

        // Display information about successful (un)subscription
        ShowChangesSaved();
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Display appropriate message
        if (userIsIdentified)
        {
            // There are some newsletters to display
            if (MembershipContext.AuthenticatedUser.UserID == UserID)
            {
                lblText.Text = GetString("MySubscriptions.MainText").Replace("##EMAIL##", HTMLHelper.HTMLEncode(subscriberEmail));
            }
            else
            {
                lblText.Text = GetString("MySubscriptions.MainTextUser").Replace("##EMAIL##", HTMLHelper.HTMLEncode(subscriberEmail));
            }
        }

        // Preserve selected values
        hdnValue.Value = ValidationHelper.GetString(usNewsletters.Value, string.Empty);
    }


    /// <summary>
    /// Overridden SetValue - because of MyAccount webpart.
    /// </summary>
    /// <param name="propertyName">Name of the property to set</param>
    /// <param name="value">Value to set</param>
    public override bool SetValue(string propertyName, object value)
    {
        base.SetValue(propertyName, value);

        switch (propertyName.ToLowerCSafe())
        {
            case "forcedvisible":
                ForcedVisible = ValidationHelper.GetBoolean(value, false);
                break;

            case "externaluse":
                ExternalUse = ValidationHelper.GetBoolean(value, false);
                break;

            case "cacheminutes":
                CacheMinutes = ValidationHelper.GetInteger(value, 0);
                break;

            case "reloaddata":
                // Special property which enables to call LoadData from MyAccount webpart
                LoadData();
                break;

            case "userid":
                UserID = ValidationHelper.GetInteger(value, 0);
                break;

            case "siteid":
                SiteID = ValidationHelper.GetInteger(value, 0);
                break;

            case "sendconfirmationemail":
                mSendConfirmationEmail = ValidationHelper.GetBoolean(value, true);
                break;
        }

        return true;
    }

    #endregion
}