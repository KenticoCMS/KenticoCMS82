using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Core;
using CMS.FormControls;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.Localization;
using CMS.Newsletters;
using CMS.PortalControls;
using CMS.PortalEngine;
using CMS.Base;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.WebAnalytics;
using CMS.Protection;
using CMS.DataEngine;
using CMS.MacroEngine;

public partial class CMSWebParts_Newsletters_CustomSubscriptionForm : CMSAbstractWebPart
{
    #region "Variables"

    private bool chooseMode = false;
    private bool isAuthenticated = false;

    #endregion


    #region "Layout properties"

    /// <summary>
    /// Full alternative form name ('classname.formname') for newsletter subscriber.
    /// Default value is newsletter.subscriber.SubscriptionForm
    /// </summary>
    public string AlternativeForm
    {
        get
        {
            return ValidationHelper.GetString(GetValue("AlternativeForm"), "newsletter.subscriber.SubscriptionForm");
        }
        set
        {
            SetValue("AlternativeForm", value);
            formElem.AlternativeFormFullName = value;
        }
    }


    /// <summary>
    /// Gets or sets value that indicates whether the captcha image should be displayed.
    /// </summary>
    public bool DisplayCaptcha
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayCaptcha"), false);
        }
        set
        {
            SetValue("DisplayCaptcha", value);
            plcCaptcha.Visible = value;
        }
    }


    /// <summary>
    /// Gets or sets the captcha label text.
    /// </summary>
    public string CaptchaText
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("CaptchaText"), "Webparts_Membership_RegistrationForm.Captcha");
        }
        set
        {
            SetValue("CaptchaText", value);
            lblCaptcha.ResourceString = value;
        }
    }


    /// <summary>
    /// Gets or sets the button text.
    /// </summary>
    public string ButtonText
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("ButtonText"), ResHelper.LocalizeString("{$NewsletterSubscription.Submit$}"));
        }
        set
        {
            SetValue("ButtonText", value);
            btnSubmit.Text = value;
        }
    }


    /// <summary>
    /// Gets or sets value which indicates if image button should be used instead of regular button.
    /// </summary>
    public bool UseImageButton
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("UseImageButton"), false);
        }
        set
        {
            SetValue("UseImageButton", value);
        }
    }


    /// <summary>
    /// Gets or sets image button URL.
    /// </summary>
    public string ImageButtonURL
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ImageButtonURL"), string.Empty);
        }
        set
        {
            SetValue("ImageButtonURL", value);
            btnImageSubmit.ImageUrl = value;
        }
    }

    #endregion


    #region "Other properties"

    /// <summary>
    /// Gets or sets value which indicates if authenticated users can subscribe to newsletter.
    /// </summary>
    public bool AllowUserSubscribers
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowUserSubscribers"), false);
        }
        set
        {
            SetValue("AllowUserSubscribers", value);
        }
    }


    /// <summary>
    /// Gets or sets the newsletter code name.
    /// </summary>
    public string NewsletterName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("NewsletterName"), string.Empty);
        }
        set
        {
            SetValue("NewsletterName", value);
        }
    }


    /// <summary>
    /// Gets or sets the conversion track name used after successful subscription.
    /// </summary>
    public string TrackConversionName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("TrackConversionName"), string.Empty);
        }
        set
        {
            if (value.Length > 400)
            {
                value = value.Substring(0, 400);
            }
            SetValue("TrackConversionName", value);
        }
    }


    /// <summary>
    /// Gets or sets the conversion value used after successful subscription.
    /// </summary>
    public double ConversionValue
    {
        get
        {
            return ValidationHelper.GetDoubleSystem(GetValue("ConversionValue"), 0);
        }
        set
        {
            SetValue("ConversionValue", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether confirmation email will be sent.
    /// </summary>
    public bool SendConfirmationEmail
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("SendConfirmationEmail"), true);
        }
        set
        {
            SetValue("SendConfirmationEmail", value);
        }
    }


    /// <summary>
    /// Gets or sets the message which is displayed after successful subscription.
    /// </summary>
    public string SubscriptionConfirmationMessage
    {
        get
        {
            string msg = ValidationHelper.GetString(GetValue("SubscriptionConfirmationMessage"), string.Empty);
            if (string.IsNullOrEmpty(msg))
            {
                msg = "NewsletterSubscription.Subscribed";
            }
            return GetString(msg);
        }
        set
        {
            SetValue("SubscriptionConfirmationMessage", value);
        }
    }


    /// <summary>
    /// Gets or sets the message which is displayed after subscription failed.
    /// </summary>
    public string SubscriptionErrorMessage
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SubscriptionErrorMessage"), string.Empty);
        }
        set
        {
            SetValue("SubscriptionErrorMessage", value);
        }
    }


    /// <summary>
    /// Gets or sets the message which is displayed if subscriber is already subscribed.
    /// </summary>
    public string MessageForAlreadySubscribed
    {
        get
        {
            return ValidationHelper.GetString(GetValue("MessageForAlreadySubscribed"), string.Empty);
        }
        set
        {
            SetValue("MessageForAlreadySubscribed", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether this webpart is used in other webpart or user control.
    /// </summary>
    public bool ExternalUse
    {
        get;
        set;
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Reloads data for partial caching.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
    }


    /// <summary>
    /// Page pre-render event handler.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Hide default form submit button
        if ((formElem != null) && (formElem != null))
        {
            formElem.SubmitButton.Visible = false;
        }
    }


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            formElem.StopProcessing = StopProcessing;
            Visible = false;
        }
        else
        {
            isAuthenticated = (CurrentUser != null) && AuthenticationHelper.IsAuthenticated();

            if (AllowUserSubscribers && isAuthenticated && (CurrentUser != null) && (!string.IsNullOrEmpty(CurrentUser.Email)))
            {
                // Hide form for authenticated user who has an email
                formElem.StopProcessing = true;
                formElem.Visible = false;
            }
            else
            {
                // Get alternative form info
                AlternativeFormInfo afi = AlternativeFormInfoProvider.GetAlternativeFormInfo(AlternativeForm);
                if (afi != null)
                {
                    // Init subscriber object
                    SubscriberInfo sb = new SubscriberInfo();
                    if (AllowUserSubscribers && isAuthenticated)
                    {
                        // Prepare user subscriber object for authenticated user without an email
                        // Try to get existing user subscriber
                        sb = SubscriberInfoProvider.GetSubscriberInfo(UserInfo.OBJECT_TYPE, CurrentUser.UserID, SiteContext.CurrentSiteID);
                        if (sb == null)
                        {
                            // Prepare new user subscriber with pre-filled data
                            sb = new SubscriberInfo()
                            {
                                SubscriberFirstName = CurrentUser.FirstName,
                                SubscriberLastName = CurrentUser.LastName,
                                SubscriberFullName = (CurrentUser.FirstName + " " + CurrentUser.LastName).Trim(),
                                SubscriberType = UserInfo.OBJECT_TYPE,
                                SubscriberRelatedID = CurrentUser.UserID
                            };
                        }
                    }

                    // Init the form
                    formElem.AlternativeFormFullName = AlternativeForm;
                    formElem.Info = sb;
                    formElem.ClearAfterSave = false;
                    formElem.Visible = true;
                    formElem.ValidationErrorMessage = SubscriptionErrorMessage;
                    formElem.IsLiveSite = true;

                    // Reload form if not in PortalEngine environment and if post back
                    if ((StandAlone) && (RequestHelper.IsPostBack()))
                    {
                        formElem.ReloadData();
                    }
                }
                else
                {
                    lblError.Text = String.Format(GetString("altform.formdoesntexists"), AlternativeForm);
                    lblError.Visible = true;
                    pnlSubscription.Visible = false;
                }
            }

            // Display/hide captcha
            plcCaptcha.Visible = DisplayCaptcha;
            lblCaptcha.ResourceString = CaptchaText;
            lblCaptcha.AssociatedControlClientID = scCaptcha.InputClientID;

            // Init submit buttons
            if ((UseImageButton) && (!String.IsNullOrEmpty(ImageButtonURL)))
            {
                pnlButtonSubmit.Visible = false;
                pnlImageSubmit.Visible = true;
                btnImageSubmit.ImageUrl = ImageButtonURL;
            }
            else
            {
                pnlButtonSubmit.Visible = true;
                pnlImageSubmit.Visible = false;
                btnSubmit.Text = ButtonText;
            }

            lblInfo.CssClass = "EditingFormInfoLabel";
            lblError.CssClass = "EditingFormErrorLabel";

            if (formElem != null)
            {
                // Set the live site context
                formElem.ControlContext.ContextName = CMS.ExtendedControls.ControlContext.LIVE_SITE;
            }

            // Init newsletter selector
            InitNewsletterSelector();
        }
    }


    protected void InitNewsletterSelector()
    {
        // Show/hide newsletter list
        plcNwsList.Visible = NewsletterName.EqualsCSafe("nwsletuserchoose", true);

        if (plcNwsList.Visible)
        {
            chooseMode = true;

            if ((!ExternalUse || !RequestHelper.IsPostBack()) && (chklNewsletters.Items.Count == 0))
            {
                DataSet ds = null;

                // Try to get data from cache
                using (var cs = new CachedSection<DataSet>(ref ds, CacheMinutes, true, CacheItemName, "newslettersubscription", SiteContext.CurrentSiteName))
                {
                    if (cs.LoadData)
                    {
                        // Get the data
                        ds = NewsletterInfoProvider.GetNewslettersForSite(SiteContext.CurrentSiteID).OrderBy("NewsletterDisplayName").Columns("NewsletterDisplayName", "NewsletterName");

                        // Add data to the cache
                        if (cs.Cached)
                        {
                            // Prepare cache dependency
                            cs.CacheDependency = CacheHelper.GetCacheDependency("newsletter.newsletter|all");
                        }

                        cs.Data = ds;
                    }
                }

                if (!DataHelper.DataSourceIsEmpty(ds))
                {
                    ListItem li = null;
                    string displayName = null;

                    // Fill checkbox list with newsletters
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        // Get localized string
                        displayName = ResHelper.LocalizeString(ValidationHelper.GetString(dr["NewsletterDisplayName"], string.Empty));

                        li = new ListItem(HTMLHelper.HTMLEncode(displayName), ValidationHelper.GetString(dr["NewsletterName"], string.Empty));
                        chklNewsletters.Items.Add(li);
                    }
                }
            }
        }
    }


    /// <summary>
    /// Submit button handler.
    /// </summary>
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        if (PortalContext.IsDesignMode(PortalManager.ViewMode) || (HideOnCurrentPage) || (!IsVisible))
        {
            // Do not process
            return;
        }

        // Check banned ip
        if (!BannedIPInfoProvider.IsAllowed(SiteContext.CurrentSiteName, BanControlEnum.AllNonComplete))
        {
            lblError.Visible = true;
            lblError.Text = GetString("General.BannedIP");
            return;
        }

        if (IsValid())
        {
            // Ensure subscriber - get existing or create new one
            SubscriberInfo subscriber = SaveSubscriber();

            if (chooseMode)
            {
                ValidChoose(subscriber);
            }
            else
            {
                // Hide subscription form if subscription was successful
                pnlSubscription.Visible = !Save(NewsletterName, subscriber);
            }
        }
    }


    /// <summary>
    /// Indicates whether the basic properties contain a valid data.
    /// </summary>
    /// <returns>Returns true if the basic data are valid; otherwise, false</returns>
    private bool IsValid()
    {
        string errorText = null;
        bool result = true;

        if (chooseMode)
        {
            if (chklNewsletters.SelectedIndex < 0)
            {
                errorText += GetString("NewsletterSubscription.NoneSelected") + "<br />";
                result = false;
            }
        }

        // Check if captcha is required and verifiy captcha text
        if (DisplayCaptcha && !scCaptcha.IsValid())
        {
            // Display error message if captcha text is not valid
            errorText += GetString("Webparts_Membership_RegistrationForm.captchaError");
            result = false;
        }

        // Assign validation result text.
        if (!string.IsNullOrEmpty(errorText))
        {
            lblError.Visible = true;
            lblError.Text = errorText;
        }

        return result;
    }


    /// <summary>
    /// Valid checkbox list, Indicates whether the subscriber is already subscribed to the selected newsletter.
    /// </summary>
    /// <param name="sb">Subscriber</param>
    private void ValidChoose(SubscriberInfo sb)
    {
        if (sb == null)
        {
            return;
        }

        bool wasWrong = true;

        // Save selected items
        for (int i = 0; i < chklNewsletters.Items.Count; i++)
        {
            ListItem item = chklNewsletters.Items[i];
            if (item != null && item.Selected)
            {
                wasWrong = wasWrong & (!Save(item.Value, sb));
            }
        }

        // Check subscription
        if ((chklNewsletters.Items.Count > 0) && (!wasWrong))
        {
            lblInfo.Visible = true;
            lblInfo.Text += SubscriptionConfirmationMessage;

            // Hide subscription form after successful subscription
            pnlSubscription.Visible = false;
        }
    }


    /// <summary>
    /// Save subscriber.
    /// </summary>
    /// <returns>Subscriber info object</returns>
    private SubscriberInfo SaveSubscriber()
    {
        string emailValue = null;
        int currentSiteId = SiteContext.CurrentSiteID;

        // Check if a subscriber exists first
        SubscriberInfo sb = null;
        if (AllowUserSubscribers && isAuthenticated)
        {
            // Try to get user subscriber
            sb = SubscriberInfoProvider.GetSubscriberInfo(UserInfo.OBJECT_TYPE, CurrentUser.UserID, currentSiteId);
        }
        else
        {
            EditingFormControl txtEmail = formElem.FieldEditingControls["SubscriberEmail"] as EditingFormControl;
            if (txtEmail != null)
            {
                emailValue = ValidationHelper.GetString(txtEmail.Value, String.Empty);
            }

            if (!string.IsNullOrEmpty(emailValue))
            {
                // Try to get subscriber by email address
                sb = SubscriberInfoProvider.GetSubscriberInfo(emailValue, currentSiteId);
            }
        }

        if ((sb == null) || (chooseMode))
        {
            // Create subscriber
            if (sb == null)
            {
                if (formElem.Visible)
                {
                    int formSiteId = formElem.Data.GetValue("SubscriberSiteID").ToInteger(0);
                    if (formSiteId == 0)
                    {
                        // Specify SiteID for the new subscriber
                        formElem.Data.SetValue("SubscriberSiteID", currentSiteId);
                    }

                    // Save form with new subscriber data
                    if (!formElem.Save())
                    {
                        return null;
                    }

                    // Get subscriber info from form
                    sb = (SubscriberInfo)formElem.Info;
                }
                else
                {
                    sb = new SubscriberInfo();
                }
            }

            // Handle authenticated user
            if (AllowUserSubscribers && isAuthenticated)
            {
                // Get user info and copy first name, last name or full name to new subscriber
                // if these properties were not set
                UserInfo ui = UserInfoProvider.GetUserInfo(CurrentUser.UserID);
                
                var subscriberUserProvider = Service<ISubscriberUserProvider>.Entry();

                subscriberUserProvider.UpdateSubscriberForUser(sb, ui, SiteContext.CurrentSiteID);

                if (!string.IsNullOrEmpty(sb.SubscriberEmail) && string.IsNullOrEmpty(ui.Email))
                {
                    // Update user email if it was not set
                    ui.Email = sb.SubscriberEmail;
                    UserInfoProvider.SetUserInfo(ui);

                    // Reset email for user subscriber
                    sb.SubscriberEmail = null;
                }
            }
            // Work with non-authenticated user
            else
            {
                if (string.IsNullOrEmpty(sb.SubscriberFullName))
                {
                    // Fill full name if it was not set via the form
                    sb.SubscriberFullName = (sb.SubscriberFirstName + " " + sb.SubscriberLastName).Trim();
                }
            }

            // Insert or update subscriber info
            SubscriberInfoProvider.SetSubscriberInfo(sb);

            // Check subscriber limits
            if (!SubscriberInfoProvider.LicenseVersionCheck(RequestContext.CurrentDomain, FeatureEnum.Subscribers, ObjectActionEnum.Insert))
            {
                // Remove created subscriber and display error message
                SubscriberInfoProvider.DeleteSubscriberInfo(sb);

                lblError.Visible = true;
                lblError.Text = GetString("LicenseVersionCheck.Subscribers");
                return null;
            }
        }

        if (sb != null)
        {
            // Create membership between current contact and subscriber
            ModuleCommands.OnlineMarketingCreateRelation(sb.SubscriberID, MembershipType.NEWSLETTER_SUBSCRIBER, ModuleCommands.OnlineMarketingGetCurrentContactID());
        }

        // Return subscriber info object
        return sb;
    }


    /// <summary>
    /// Saves the data.
    /// </summary>
    /// <param name="newsletterName">Newsletter name</param>
    /// <param name="sb">Subscriber</param>
    private bool Save(string newsletterName, SubscriberInfo sb)
    {
        bool toReturn = false;
        int siteId = SiteContext.CurrentSiteID;

        // Check if sunscriber info object exists
        if ((sb == null) || string.IsNullOrEmpty(newsletterName))
        {
            return false;
        }

        // Get nesletter info
        NewsletterInfo news = NewsletterInfoProvider.GetNewsletterInfo(newsletterName, siteId);
        if (news != null)
        {
            // Init webpart resolver
            object[] data = new object[3];
            data[0] = news;
            data[1] = sb;

            MacroResolver resolver = ContextResolver;
            resolver.SetNamedSourceData("Newsletter", news);
            resolver.SetNamedSourceData("Subscriber", sb);
            if (AllowUserSubscribers && isAuthenticated)
            {
                data[2] = CurrentUser;
                resolver.SetNamedSourceData("User", CurrentUser);
            }
            resolver.SetAnonymousSourceData(data);

            try
            {
                // Check if subscriber is not allready subscribed
                if (!SubscriberInfoProvider.IsSubscribed(sb.SubscriberGUID, news.NewsletterGUID, siteId))
                {
                    toReturn = true;

                    // Subscribe to the site
                    SubscriberInfoProvider.Subscribe(sb.SubscriberID, news.NewsletterID, DateTime.Now, SendConfirmationEmail);

                    if (!chooseMode)
                    {
                        // Display message about successful subscription
                        lblInfo.Visible = true;
                        lblInfo.Text = SubscriptionConfirmationMessage;
                    }

                    // Track successful subscription conversion
                    if (!string.IsNullOrEmpty(TrackConversionName))
                    {
                        string siteName = SiteContext.CurrentSiteName;

                        if (AnalyticsHelper.AnalyticsEnabled(siteName) && AnalyticsHelper.TrackConversionsEnabled(siteName) && !AnalyticsHelper.IsIPExcluded(siteName, RequestContext.UserHostAddress))
                        {
                            // Log conversion
                            HitLogProvider.LogConversions(siteName, LocalizationContext.PreferredCultureCode, TrackConversionName, 0, ConversionValue);
                        }
                    }

                    // Log newsletter subscription activity if double opt-in is not required
                    if (!news.NewsletterEnableOptIn)
                    {
                        Activity activity = new ActivityNewsletterSubscribing(sb, news, AnalyticsContext.ActivityEnvironmentVariables);
                        activity.Log();
                    }
                }
                else
                {
                    lblInfo.Visible = true;
                    string message = null;

                    if (string.IsNullOrEmpty(MessageForAlreadySubscribed))
                    {
                        if (!chooseMode)
                        {
                            message = GetString("NewsletterSubscription.SubscriberIsAlreadySubscribed");
                        }
                        else
                        {
                            message = string.Format("{0} {1}.<br />", GetString("NewsletterSubscription.SubscriberIsAlreadySubscribedXY"), HTMLHelper.HTMLEncode(ResHelper.LocalizeString(news.NewsletterDisplayName)));
                        }
                    }
                    else
                    {
                        message = MessageForAlreadySubscribed;
                    }

                    // Info message - subscriber is allready in site
                    if (!chooseMode)
                    {
                        lblInfo.Text = message;
                    }
                    else
                    {
                        lblInfo.Text += message;
                    }
                }
            }
            catch (Exception ex)
            {
                lblError.Visible = true;
                lblError.Text = ex.Message;
            }
        }
        else
        {
            lblError.Visible = true;
            lblError.Text = GetString("NewsletterSubscription.NewsletterDoesNotExist");
        }

        return toReturn;
    }


    /// <summary>
    /// Clears the cached items.
    /// </summary>
    public override void ClearCache()
    {
        string useCacheItemName = DataHelper.GetNotEmpty(CacheItemName, CacheHelper.BaseCacheKey + "|" + RequestContext.CurrentURL + "|" + ClientID);

        CacheHelper.ClearCache(useCacheItemName);
    }

    #endregion
}
