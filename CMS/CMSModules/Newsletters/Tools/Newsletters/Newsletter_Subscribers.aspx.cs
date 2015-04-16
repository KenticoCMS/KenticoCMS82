using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Core;
using CMS.FormControls;
using CMS.Helpers;
using CMS.Helpers.Markup;
using CMS.LicenseProvider;
using CMS.Newsletters;
using CMS.Base;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;
using CMS.ExtendedControls;
using CMS.DataEngine;

[UIElement(ModuleName.NEWSLETTER, "Newsletter.Subscribers")]
[EditedObject(NewsletterInfo.OBJECT_TYPE, "objectid")]
public partial class CMSModules_Newsletters_Tools_Newsletters_Newsletter_Subscribers : CMSNewsletterPage
{
    #region "Variables"

    private const string SELECT = "SELECT";
    private const string UNSUBSCRIBE = "UNSUBSCRIBE";
    private const string SUBSCRIBE = "SUBSCRIBE";
    private const string APPROVE = "APPROVE";
    private const string REMOVE = "REMOVE";
    private const string BLOCK = "BLOCK";
    private const string UNBLOCK = "UNBLOCK";

    private int mBounceLimit;
    private bool mBounceInfoAvailable;
    private NewsletterInfo mNewsletter;

    /// <summary>
    /// Contact group selector.
    /// </summary>
    private FormEngineUserControl cgSelector;


    /// <summary>
    /// Contact selector.
    /// </summary>
    private FormEngineUserControl cSelector;


    /// <summary>
    /// Persona selector.
    /// </summary>
    private UniSelector personaSelector;

    #endregion


    #region "Methods"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        CurrentMaster.ActionsViewstateEnabled = true;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        mNewsletter = EditedObject as NewsletterInfo;
        if (mNewsletter == null)
        {
            RedirectToAccessDenied(GetString("general.invalidparameters"));
        }

        if (!mNewsletter.CheckPermissions(PermissionsEnum.Read, CurrentSiteName, CurrentUser))
        {
            RedirectToAccessDenied(mNewsletter.TypeInfo.ModuleName, "ManageSubscribers");
        }

        ScriptHelper.RegisterDialogScript(this);

        CurrentMaster.DisplayActionsPanel = true;
        chkRequireOptIn.CheckedChanged += chkRequireOptIn_CheckedChanged;

        string currentSiteName = SiteContext.CurrentSiteName;
        mBounceLimit = NewsletterHelper.BouncedEmailsLimit(currentSiteName);
        mBounceInfoAvailable = NewsletterHelper.MonitorBouncedEmails(currentSiteName);
        
        // Check if newsletter enables double opt-in
        if (!mNewsletter.NewsletterEnableOptIn)
        {
            chkRequireOptIn.Visible = false;
        }
        
        if (!RequestHelper.IsPostBack())
        {
            chkSendConfirmation.Checked = false;
        }

        // Initialize unigrid
        UniGridSubscribers.WhereCondition = "NewsletterID = " + mNewsletter.NewsletterID;
        UniGridSubscribers.OnAction += UniGridSubscribers_OnAction;
        UniGridSubscribers.OnExternalDataBound += UniGridSubscribers_OnExternalDataBound;

        // Initialize selectors and mass actions
        SetupSelectors();
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        // Hide columns with bounced emails if bounce info is not available
        UniGridSubscribers.NamedColumns["blocked"].Visible =
            UniGridSubscribers.NamedColumns["bounces"].Visible = mBounceInfoAvailable;

        pnlActions.Visible = !DataHelper.DataSourceIsEmpty(UniGridSubscribers.GridView.DataSource);
    }


    /// <summary>
    /// Configures selectors.
    /// </summary>
    private void SetupSelectors()
    {
        // Setup role selector
        selectRole.CurrentSelector.SelectionMode = SelectionModeEnum.MultipleButton;
        selectRole.CurrentSelector.OnItemsSelected += RolesSelector_OnItemsSelected;
        selectRole.CurrentSelector.ReturnColumnName = "RoleID";
        selectRole.ShowSiteFilter = false;
        selectRole.CurrentSelector.ResourcePrefix = "addroles";
        selectRole.IsLiveSite = false;
        selectRole.UseCodeNameForSelection = false;
        selectRole.GlobalRoles = false;

        // Setup user selector
        selectUser.SelectionMode = SelectionModeEnum.MultipleButton;
        selectUser.UniSelector.OnItemsSelected += UserSelector_OnItemsSelected;
        selectUser.UniSelector.ReturnColumnName = "UserID";
        selectUser.UniSelector.DisplayNameFormat = "{%FullName%} ({%Email%})";
        selectUser.UniSelector.AdditionalSearchColumns = "UserName, Email";
        selectUser.ShowSiteFilter = false;
        selectUser.ResourcePrefix = "newsletteraddusers";
        selectUser.IsLiveSite = false;

        // Setup subscriber selector
        selectSubscriber.UniSelector.SelectionMode = SelectionModeEnum.MultipleButton;
        selectSubscriber.UniSelector.OnItemsSelected += SubscriberSelector_OnItemsSelected;
        selectSubscriber.UniSelector.ReturnColumnName = "SubscriberID";
        selectSubscriber.ShowSiteFilter = false;
        selectSubscriber.IsLiveSite = false;
        selectSubscriber.UniSelector.RemoveMultipleCommas = true;

        // Setup contact group and contact selectors
        if (ModuleEntryManager.IsModuleLoaded(ModuleName.ONLINEMARKETING) && LicenseHelper.CheckFeature(RequestContext.CurrentDomain, FeatureEnum.ContactManagement))
        {
            plcSelectCG.Controls.Clear();

            // Check read permission for contact groups
            if (MembershipContext.AuthenticatedUser.IsAuthorizedPerResource(ModuleName.CONTACTMANAGEMENT, "ReadContactGroups"))
            {
                // Load selector control and initialize it
                cgSelector = (FormEngineUserControl)Page.LoadUserControl("~/CMSModules/ContactManagement/FormControls/ContactGroupSelector.ascx");
                if (cgSelector != null)
                {
                    cgSelector.ID = "selectCG";
                    cgSelector.ShortID = "scg";
                    // Get inner uniselector control
                    UniSelector selector = (UniSelector)cgSelector.GetValue("uniselector");
                    if (selector != null)
                    {
                        // Bind an event handler on 'items selected' event
                        selector.OnItemsSelected += CGSelector_OnItemsSelected;
                        selector.ResourcePrefix = "contactgroupsubscriber";
                    }
                    // Insert selector to the header
                    plcSelectCG.Controls.Add(cgSelector);
                }
            }

            // Check read permission for contacts
            if (MembershipContext.AuthenticatedUser.IsAuthorizedPerResource(ModuleName.CONTACTMANAGEMENT, "ReadContacts"))
            {
                // Load selector control and initialize it
                cSelector = (FormEngineUserControl)Page.LoadUserControl("~/CMSModules/ContactManagement/FormControls/ContactSelector.ascx");
                if (cSelector != null)
                {
                    cSelector.ID = "slContact";
                    cSelector.ShortID = "sc";
                    // Set where condition to filter contacts with email addresses
                    cSelector.SetValue("wherecondition", "NOT (ContactEmail IS NULL OR ContactEmail LIKE '')");
                    // Set site ID
                    cSelector.SetValue("siteid", SiteContext.CurrentSiteID);
                    // Get inner uniselector control
                    UniSelector selector = (UniSelector)cSelector.GetValue("uniselector");
                    if (selector != null)
                    {
                        // Bind an event handler on 'items selected' event
                        selector.OnItemsSelected += ContactSelector_OnItemsSelected;
                        selector.SelectionMode = SelectionModeEnum.MultipleButton;
                        selector.ResourcePrefix = "contactsubscriber";
                        selector.DisplayNameFormat = "{%ContactFirstName%} {%ContactLastName%} ({%ContactEmail%})";
                        selector.AdditionalSearchColumns = "ContactFirstName,ContactMiddleName,ContactEmail";
                    }
                    // Insert selector to the header
                    plcSelectCG.Controls.Add(cSelector);
                }
            }
        }

        // Setup persona selectors
        if (ModuleEntryManager.IsModuleLoaded(ModuleName.PERSONAS) && LicenseHelper.CheckFeature(RequestContext.CurrentDomain, FeatureEnum.Personas))
        {
            // Check read permission for contact groups
            if (MembershipContext.AuthenticatedUser.IsAuthorizedPerResource(ModuleName.PERSONAS, "Read"))
            {
                // Load selector control and initialize it
                personaSelector = (UniSelector)Page.LoadUserControl("~/CMSAdminControls/UI/Uniselector/Uniselector.ascx");
                if (personaSelector != null)
                {
                    personaSelector.ID = "personaSelector";
                    personaSelector.ShortID = "ps";
                    personaSelector.ObjectType = PredefinedObjectType.PERSONA;
                    personaSelector.ReturnColumnName = "PersonaID";
                    personaSelector.WhereCondition = "PersonaSiteID = " + SiteContext.CurrentSiteID;
                    personaSelector.SelectionMode = SelectionModeEnum.MultipleButton;
                    personaSelector.DisplayNameFormat = "{%PersonaDisplayName%}";
                    personaSelector.ResourcePrefix = "personasubscriber";
                    personaSelector.IsLiveSite = false;

                    // Bind an event handler on 'items selected' event
                    personaSelector.OnItemsSelected += PersonaSelector_OnItemsSelected;

                    // Add selector to the header
                    plcSelectCG.Controls.Add(personaSelector);
                }
            }
        }

        // Initialize mass actions
        if (drpActions.Items.Count == 0)
        {
            drpActions.Items.Add(new ListItem(GetString("general.selectaction"), SELECT));
            drpActions.Items.Add(new ListItem(GetString("newsletter.unsubscribelink"), UNSUBSCRIBE));
            drpActions.Items.Add(new ListItem(GetString("newsletter.renewsubscription"), SUBSCRIBE));
            drpActions.Items.Add(new ListItem(GetString("newsletter.approvesubscription"), APPROVE));
            drpActions.Items.Add(new ListItem(GetString("newsletter.deletesubscription"), REMOVE));
        }
    }


    /// <summary>
    /// Unigrid external databound event handler.
    /// </summary>
    protected object UniGridSubscribers_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        string sourceNameUpper = sourceName.ToUpperInvariant();

        switch (sourceNameUpper)
        {
            case SUBSCRIBE:
            case UNSUBSCRIBE:
            
                var subscribeDataItem = ((DataRowView)((GridViewRow)parameter).DataItem);
                bool subscriptionEnabled = ValidationHelper.GetBoolean(subscribeDataItem.Row["SubscriptionEnabled"], true);
                
                ((CMSGridActionButton)sender).Visible =
                    ((sourceNameUpper == SUBSCRIBE) && !subscriptionEnabled) ||
                    ((sourceNameUpper == UNSUBSCRIBE) && subscriptionEnabled);

                var user = GetSubscriberUser(subscribeDataItem);
                if (user != null)
                {
                    ((CMSGridActionButton)sender).Enabled = user.UserEnabled;
                }
                break;

            case BLOCK:
                var gridViewRow = parameter as GridViewRow;
                if (gridViewRow != null)
                {
                    return SetBlockAction(sender, (gridViewRow.DataItem) as DataRowView);
                }
                break;

            case UNBLOCK:
                var viewRow = parameter as GridViewRow;
                if (viewRow != null)
                {
                    return SetUnblockAction(sender, (viewRow.DataItem) as DataRowView);
                }
                break;

            case APPROVE:
                bool approved = ValidationHelper.GetBoolean(((DataRowView)((GridViewRow)parameter).DataItem).Row["SubscriptionApproved"], false);
                bool enabled = ValidationHelper.GetBoolean(((DataRowView)((GridViewRow)parameter).DataItem).Row["SubscriptionEnabled"], true);
                if (approved || !enabled)
                {
                    CMSGridActionButton button = ((CMSGridActionButton)sender);
                    button.Visible = false;
                }
                break;

            case "EMAIL":
                return GetEmail(parameter as DataRowView);

            case "STATUS":
                return GetSubscriptionStatus(parameter as DataRowView);

            case "BLOCKED":
                return GetBlocked(parameter as DataRowView);

            case "BOUNCES":
                return GetBounces(parameter as DataRowView);
        }

        return null;
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that threw event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void UniGridSubscribers_OnAction(string actionName, object actionArgument)
    {
        // Check 'manage subscribers' permission
        CheckAuthorization();

        int subscriberId = ValidationHelper.GetInteger(actionArgument, 0);

        DoSubscriberAction(subscriberId, actionName);
    }


    /// <summary>
    /// Displays/hides block action button in unigrid.
    /// </summary>
    private object SetBlockAction(object sender, DataRowView rowView)
    {
        int bounces = GetBouncesFromRow(rowView);

        var imageButton = sender as CMSGridActionButton;
        if (imageButton != null)
        {
            imageButton.Visible = mBounceInfoAvailable && !IsMultiSubscriber(rowView)
            && ((mBounceLimit > 0 && bounces < mBounceLimit) || (mBounceLimit == 0 && bounces < int.MaxValue));
        }

        return null;
    }


    /// <summary>
    /// Displays/hides un-block action button in unigrid.
    /// </summary>
    private object SetUnblockAction(object sender, DataRowView rowView)
    {
        int bounces = GetBouncesFromRow(rowView);

        var imageButton = sender as CMSGridActionButton;
        if (imageButton != null)
        {
            imageButton.Visible = mBounceInfoAvailable && !IsMultiSubscriber(rowView)
            && ((mBounceLimit > 0 && bounces >= mBounceLimit) || (mBounceLimit == 0 && bounces == int.MaxValue));
        }

        return null;
    }


    /// <summary>
    /// Returns subscriber's e-mail address.
    /// </summary>
    private object GetEmail(DataRowView rowView)
    {
        // Try to get subscriber email
        string email = ValidationHelper.GetString(rowView.Row["SubscriberEmail"], string.Empty);
        if (string.IsNullOrEmpty(email))
        {
            // Try to get user email
            email = ValidationHelper.GetString(rowView.Row["Email"], string.Empty);
        }

        if (string.IsNullOrEmpty(email) && ValidationHelper.GetString(rowView.Row["SubscriberType"], string.Empty).EqualsCSafe(PredefinedObjectType.CONTACT))
        {
            // Add the field transformation control that handles the translation
            var tr = new ObjectTransformation("om.contact", ValidationHelper.GetInteger(rowView.Row["SubscriberRelatedID"], 0));
            tr.Transformation = "ContactEmail";

            return tr;
        }

        return email;
    }


    /// <summary>
    /// Returns colored status of the subscription.
    /// </summary>
    private FormattedText GetSubscriptionStatus(DataRowView rowView)
    {
        var user = GetSubscriberUser(rowView);
        if (user != null && !user.UserEnabled)
        {
            return new FormattedText(GetString("newsletterview.subscriberuserdisabled")).ColorRed();   
        }

        bool approved = ValidationHelper.GetBoolean(DataHelper.GetDataRowValue(rowView.Row, "SubscriptionApproved"), false);
        bool enabled = ValidationHelper.GetBoolean(DataHelper.GetDataRowValue(rowView.Row, "SubscriptionEnabled"), true);

        if (!enabled)
        {
            return new FormattedText(GetString("newsletterview.headerunsubscribed"))
                .ColorRed();
        }

        if (approved)
        {
            return new FormattedText(GetString("general.approved"))
                .ColorGreen();
        }
        else
        {
            return new FormattedText(GetString("administration.users_header.myapproval"))
                .ColorOrange();
        }
    }


    /// <summary>
    /// Checks whether the given subscriber is of type User. If so, returns proper User info; otherwise, null.
    /// </summary>
    /// <param name="subscriber">Subscriber info to be checked</param>
    /// <returns>UserInfo related to the given subscriber or null</returns>
    private UserInfo GetSubscriberUser(SubscriberInfo subscriber)
    {
        if (subscriber.SubscriberType == UserInfo.OBJECT_TYPE)
        {
            return UserInfoProvider.GetUserInfo(subscriber.SubscriberRelatedID);
        }

        return null;
    }


    /// <summary>
    /// Checks whether the given subscriber is of type User. If so, returns proper User info; otherwise, null.
    /// </summary>
    /// <param name="rowView">Subscriber to be checked</param>
    /// <returns>UserInfo related to the given subscriber or null</returns>
    private UserInfo GetSubscriberUser(DataRowView rowView)
    {
        string subscriptionType = ValidationHelper.GetString(rowView.Row["SubscriberType"], string.Empty);
        if (subscriptionType == UserInfo.OBJECT_TYPE)
        {
            int userId = ValidationHelper.GetInteger(rowView.Row["SubscriberRelatedID"], 0);
            return UserInfoProvider.GetUserInfo(userId);
        }

        return null;
    }


    /// <summary>
    /// Returns colored yes/no or nothing according to subscriber's blocked info.
    /// </summary>
    private string GetBlocked(DataRowView rowView)
    {
        // Do not handle if bounce email monitoring is not available
        if (!mBounceInfoAvailable)
        {
            return null;
        }

        // If bounce limit is not a natural number, then the feature is considered disabled
        if (mBounceLimit < 0)
        {
            return UniGridFunctions.ColoredSpanYesNoReversed(false);
        }

        if (IsMultiSubscriber(rowView))
        {
            return null;
        }

        int bounces = GetBouncesFromRow(rowView);

        return UniGridFunctions.ColoredSpanYesNoReversed((mBounceLimit > 0 && bounces >= mBounceLimit) || (mBounceLimit == 0 && bounces == int.MaxValue));
    }


    /// <summary>
    /// Returns number of bounces or nothing according to subscriber's bounce info.
    /// </summary>
    private string GetBounces(DataRowView rowView)
    {
        // Do not handle if bounce email monitoring is not available
        if (!mBounceInfoAvailable)
        {
            return null;
        }

        int bounces = GetBouncesFromRow(rowView);

        if (bounces == 0 || bounces == int.MaxValue || IsMultiSubscriber(rowView))
        {
            return null;
        }

        return bounces.ToString();
    }


    /// <summary>
    /// Checks if the user has permission to manage subscribers.
    /// </summary>
    private static void CheckAuthorization()
    {
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.newsletter", "managesubscribers"))
        {
            RedirectToAccessDenied("cms.newsletter", "managesubscribers");
        }
    }


    /// <summary>
    /// Checkbox 'Require double opt-in' state changed.
    /// </summary>
    protected void chkRequireOptIn_CheckedChanged(object sender, EventArgs e)
    {
        if (chkRequireOptIn.Checked)
        {
            chkSendConfirmation.Enabled = false;
            chkSendConfirmation.Checked = false;
        }
        else
        {
            chkSendConfirmation.Enabled = true;
        }
    }


    /// <summary>
    /// Roles control items changed event.
    /// </summary>
    protected void RolesSelector_OnItemsSelected(object sender, EventArgs e)
    {
        // Check permissions
        CheckAuthorization();

        int siteId = SiteContext.CurrentSiteID;

        // Get new items from selector
        string newValues = ValidationHelper.GetString(selectRole.Value, null);

        // Get added items
        string[] newItems = newValues.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string item in newItems)
        {
            // Check limited number of subscribers
            if (!SubscriberInfoProvider.LicenseVersionCheck(RequestContext.CurrentDomain, FeatureEnum.Subscribers, ObjectActionEnum.Insert))
            {
                ShowError(GetString("licenselimitations.subscribers.errormultiple"));
                break;
            }

            int roleID = ValidationHelper.GetInteger(item, 0);

            // Get subscriber
            SubscriberInfo sb = SubscriberInfoProvider.GetSubscriberInfo(RoleInfo.OBJECT_TYPE, roleID, siteId);
            if (sb == null)
            {
                // Get role info and copy display name to new subscriber
                RoleInfo ri = RoleInfoProvider.GetRoleInfo(roleID);
                if ((ri == null) || (ri.SiteID != siteId))
                {
                    continue;
                }

                // Create new subscriber of role type
                sb = new SubscriberInfo();
                sb.SubscriberFirstName = ri.DisplayName;
                // Full name consists of "role " and role display name
                sb.SubscriberFullName = new SubscriberFullNameFormater().GetRoleSubscriberName(ri.DisplayName);
                sb.SubscriberSiteID = siteId;
                sb.SubscriberType = RoleInfo.OBJECT_TYPE;
                sb.SubscriberRelatedID = roleID;

                CheckPermissionsForSubscriber(sb);

                SubscriberInfoProvider.SetSubscriberInfo(sb);
            }

            // If subscriber exists and is not subscribed, subscribe him
            if (!SubscriberInfoProvider.IsSubscribed(sb.SubscriberID, mNewsletter.NewsletterID))
            {
                try
                {
                    SubscriberInfoProvider.Subscribe(sb.SubscriberID, mNewsletter.NewsletterID, DateTime.Now, chkSendConfirmation.Checked, false);
                }
                catch (InvalidOperationException ex)
                {
                    ShowError(ex.Message);
                }
            }
        }

        selectRole.Value = null;
        UniGridSubscribers.ReloadData();
        pnlUpdate.Update();
    }


    /// <summary>
    /// User control items changed event.
    /// </summary>
    protected void UserSelector_OnItemsSelected(object sender, EventArgs e)
    {
        // Check permissions
        CheckAuthorization();

        int siteId = SiteContext.CurrentSiteID;

        // Get new items from selector
        string newValues = ValidationHelper.GetString(selectUser.Value, null);

        string[] newItems = newValues.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string item in newItems)
        {
            // Check limited number of subscribers
            if (!SubscriberInfoProvider.LicenseVersionCheck(RequestContext.CurrentDomain, FeatureEnum.Subscribers, ObjectActionEnum.Insert))
            {
                ShowError(GetString("licenselimitations.subscribers.errormultiple"));
                break;
            }

            int userID = ValidationHelper.GetInteger(item, 0);

            // Get subscriber
            SubscriberInfo sb = SubscriberInfoProvider.GetSubscriberInfo(UserInfo.OBJECT_TYPE, userID, siteId);
            if (sb == null)
            {
                // Get user info
                UserInfo ui = UserInfoProvider.GetUserInfo(userID);
                if (ui == null)
                {
                    continue;
                }

                // Create new subscriber of user type
                sb = new SubscriberInfo();
                sb.SubscriberFirstName = ui.FullName;
                sb.SubscriberFullName = new SubscriberFullNameFormater().GetUserSubscriberName(ui.FullName);
                sb.SubscriberSiteID = siteId;
                sb.SubscriberType = UserInfo.OBJECT_TYPE;
                sb.SubscriberRelatedID = userID;

                CheckPermissionsForSubscriber(sb);

                SubscriberInfoProvider.SetSubscriberInfo(sb);
            }

            // If subscriber exists and is not subscribed, subscribe him
            if (!SubscriberInfoProvider.IsSubscribed(sb.SubscriberID, mNewsletter.NewsletterID))
            {
                try
                {
                    SubscriberInfoProvider.Subscribe(sb.SubscriberID, mNewsletter.NewsletterID, DateTime.Now, chkSendConfirmation.Checked, chkRequireOptIn.Checked);
                }
                catch (InvalidOperationException ex)
                {
                    ShowError(ex.Message);
                }
            }
        }

        selectUser.Value = null;
        UniGridSubscribers.ReloadData();
        pnlUpdate.Update();
    }


    /// <summary>
    /// Checks whether the current user has permissions to add the subscriber. If not, redirect to access denied page.
    /// </summary>
    /// <param name="subscriber">Subscriber to be checked</param>
    private void CheckPermissionsForSubscriber(SubscriberInfo subscriber)
    {
        if (!subscriber.CheckPermissions(PermissionsEnum.Modify, CurrentSiteName, CurrentUser))
        {
            RedirectToAccessDenied(ModuleName.NEWSLETTER, "ManageSubscribers");
        }
    }


    /// <summary>
    /// Subscriber control items changed event.
    /// </summary>
    protected void SubscriberSelector_OnItemsSelected(object sender, EventArgs e)
    {
        // Check permissions
        CheckAuthorization();

        // Get new items from selector
        string newValues = ValidationHelper.GetString(selectSubscriber.Value, null);

        string[] newItems = newValues.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

        // Add all new items to site
        foreach (string item in newItems)
        {
            int subscriberID = ValidationHelper.GetInteger(item, 0);

            // Get subscriber
            SubscriberInfo sb = SubscriberInfoProvider.GetSubscriberInfo(subscriberID);

            // If subscriber exists and is not subscribed, subscribe him
            if ((sb != null) && (!SubscriberInfoProvider.IsSubscribed(sb.SubscriberID, mNewsletter.NewsletterID)))
            {
                CheckPermissionsForSubscriber(sb);

                try
                {
                    SubscriberInfoProvider.Subscribe(sb.SubscriberID, mNewsletter.NewsletterID, DateTime.Now, chkSendConfirmation.Checked, chkRequireOptIn.Checked);
                }
                catch (InvalidOperationException ex)
                {
                    ShowError(ex.Message);
                }
            }
        }

        selectSubscriber.Value = null;
        UniGridSubscribers.ReloadData();
        pnlUpdate.Update();
    }


    /// <summary>
    /// Contact group items selected event handler.
    /// </summary>
    protected void CGSelector_OnItemsSelected(object sender, EventArgs e)
    {
        // Check permissions
        CheckAuthorization();

        if (cgSelector == null)
        {
            return;
        }
        int siteId = SiteContext.CurrentSiteID;

        // Get new items from selector
        string newValues = ValidationHelper.GetString(cgSelector.Value, null);

        // Get added items
        string[] newItems = newValues.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string item in newItems)
        {
            // Check limited number of subscribers
            if (!SubscriberInfoProvider.LicenseVersionCheck(RequestContext.CurrentDomain, FeatureEnum.Subscribers, ObjectActionEnum.Insert))
            {
                ShowError(GetString("licenselimitations.subscribers.errormultiple"));
                break;
            }

            // Get group ID
            int groupID = ValidationHelper.GetInteger(item, 0);

            // Try to get subscriber
            SubscriberInfo sb = SubscriberInfoProvider.GetSubscriberInfo(PredefinedObjectType.CONTACTGROUP, groupID, siteId);
            if (sb == null)
            {
                // Get contact group display name
                string cgName = ModuleCommands.OnlineMarketingGetContactGroupName(groupID);
                if (string.IsNullOrEmpty(cgName))
                {
                    continue;
                }

                // Create new subscriber of contact group type
                sb = new SubscriberInfo();
                sb.SubscriberFirstName = cgName;
                // Full name consists of "contact group " and display name
                sb.SubscriberFullName = new SubscriberFullNameFormater().GetContactGroupSubscriberName(cgName);
                sb.SubscriberSiteID = siteId;
                sb.SubscriberType = PredefinedObjectType.CONTACTGROUP;
                sb.SubscriberRelatedID = groupID;

                CheckPermissionsForSubscriber(sb);

                SubscriberInfoProvider.SetSubscriberInfo(sb);
            }

            // If subscriber exists and is not subscribed, subscribe him
            if (!SubscriberInfoProvider.IsSubscribed(sb.SubscriberID, mNewsletter.NewsletterID))
            {
                try
                {
                    SubscriberInfoProvider.Subscribe(sb.SubscriberID, mNewsletter.NewsletterID, DateTime.Now, chkSendConfirmation.Checked, false);
                }
                catch (InvalidOperationException ex)
                {
                    ShowError(ex.Message);
                }
            }
        }

        cgSelector.Value = null;
        UniGridSubscribers.ReloadData();
        pnlUpdate.Update();
    }


    /// <summary>
    /// Contact items selected event handler.
    /// </summary>
    protected void ContactSelector_OnItemsSelected(object sender, EventArgs e)
    {
        // Check permissions
        CheckAuthorization();

        if (cSelector == null)
        {
            return;
        }
        int siteId = SiteContext.CurrentSiteID;

        // Get new items from selector
        string newValues = ValidationHelper.GetString(cSelector.Value, null);

        // Get added items
        string[] newItems = newValues.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string item in newItems)
        {
            // Check limited number of subscribers
            if (!SubscriberInfoProvider.LicenseVersionCheck(RequestContext.CurrentDomain, FeatureEnum.Subscribers, ObjectActionEnum.Insert))
            {
                ShowError(GetString("licenselimitations.subscribers.errormultiple"));
                break;
            }

            // Get contact ID
            int contactID = ValidationHelper.GetInteger(item, 0);

            // Try to get subscriber
            SubscriberInfo sb = SubscriberInfoProvider.GetSubscriberInfo(PredefinedObjectType.CONTACT, contactID, siteId);
            if (sb == null)
            {
                // Get contact's info
                DataSet contactData = ModuleCommands.OnlineMarketingGetContactForNewsletters(contactID, "ContactFirstName,ContactMiddleName,ContactLastName,ContactEmail");
                if (DataHelper.DataSourceIsEmpty(contactData))
                {
                    continue;
                }

                string firstName = ValidationHelper.GetString(contactData.Tables[0].Rows[0]["ContactFirstName"], string.Empty);
                string lastName = ValidationHelper.GetString(contactData.Tables[0].Rows[0]["ContactLastName"], string.Empty);
                string middleName = ValidationHelper.GetString(contactData.Tables[0].Rows[0]["ContactMiddleName"], string.Empty);
                string email = ValidationHelper.GetString(contactData.Tables[0].Rows[0]["ContactEmail"], string.Empty);

                // Create new subscriber of contact type
                sb = new SubscriberInfo();
                sb.SubscriberFirstName = firstName;
                sb.SubscriberLastName = lastName;
                sb.SubscriberEmail = email;
                sb.SubscriberFullName = new SubscriberFullNameFormater().GetContactSubscriberName(firstName, middleName, lastName);
                sb.SubscriberSiteID = siteId;
                sb.SubscriberType = PredefinedObjectType.CONTACT;
                sb.SubscriberRelatedID = contactID;

                CheckPermissionsForSubscriber(sb);

                SubscriberInfoProvider.SetSubscriberInfo(sb);
            }

            // Subscribe the existing or created subscriber
            if (!SubscriberInfoProvider.IsSubscribed(sb.SubscriberID, mNewsletter.NewsletterID))
            {
                try
                {
                    SubscriberInfoProvider.Subscribe(sb.SubscriberID, mNewsletter.NewsletterID, DateTime.Now, chkSendConfirmation.Checked, chkRequireOptIn.Checked);
                }
                catch (InvalidOperationException ex)
                {
                    ShowError(ex.Message);
                }
            }
        }

        cSelector.Value = null;
        UniGridSubscribers.ReloadData();
        pnlUpdate.Update();
    }


    /// <summary>
    /// Persona items selected event handler.
    /// </summary>
    protected void PersonaSelector_OnItemsSelected(object sender, EventArgs e)
    {
        // Check permissions
        CheckAuthorization();

        if (personaSelector == null)
        {
            return;
        }

        int siteId = SiteContext.CurrentSiteID;

        // Get new items from selector
        string newValues = ValidationHelper.GetString(personaSelector.Value, null);

        // Get added items
        string[] newItems = newValues.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string item in newItems)
        {
            // Check limited number of subscribers
            if (!SubscriberInfoProvider.LicenseVersionCheck(RequestContext.CurrentDomain, FeatureEnum.Personas, ObjectActionEnum.Insert))
            {
                ShowError(GetString("licenselimitations.subscribers.errormultiple"));
                break;
            }

            // Get persona ID
            int personaID = ValidationHelper.GetInteger(item, 0);

            // Try to get subscriber
            SubscriberInfo subscriber = SubscriberInfoProvider.GetSubscriberInfo(PredefinedObjectType.PERSONA, personaID, siteId);
            if (subscriber == null)
            {
                // Get persona display name
                var persona = BaseAbstractInfoProvider.GetInfoById(PredefinedObjectType.PERSONA, personaID);
                string personaName = ValidationHelper.GetString(persona.GetValue("PersonaDisplayName"), string.Empty);
                if (string.IsNullOrEmpty(personaName))
                {
                    continue;
                }

                // Create new subscriber of persona type
                subscriber = new SubscriberInfo();

                subscriber.SubscriberFirstName = personaName;

                // Full name consists of "persona" and display name
                subscriber.SubscriberFullName = new SubscriberFullNameFormater().GetPersonaSubscriberName(personaName);

                subscriber.SubscriberSiteID = siteId;
                subscriber.SubscriberType = PredefinedObjectType.PERSONA;
                subscriber.SubscriberRelatedID = personaID;

                CheckPermissionsForSubscriber(subscriber);

                SubscriberInfoProvider.SetSubscriberInfo(subscriber);
            }

            // If subscriber exists and is not subscribed, subscribe him
            if (!SubscriberInfoProvider.IsSubscribed(subscriber.SubscriberID, mNewsletter.NewsletterID))
            {
                try
                {
                    SubscriberInfoProvider.Subscribe(subscriber.SubscriberID, mNewsletter.NewsletterID, DateTime.Now, chkSendConfirmation.Checked, false);
                }
                catch (InvalidOperationException ex)
                {
                    ShowError(ex.Message);
                }
            }
        }

        personaSelector.Value = null;
        UniGridSubscribers.ReloadData();
        pnlUpdate.Update();
    }


    /// <summary>
    /// Returns if type of the subscriber is role, persona or contact group.
    /// </summary>
    private static bool IsMultiSubscriber(DataRowView rowView)
    {
        string type = ValidationHelper.GetString(DataHelper.GetDataRowValue(rowView.Row, "SubscriberType"), string.Empty);
        return (type.EqualsCSafe(RoleInfo.OBJECT_TYPE, true) || type.EqualsCSafe(PredefinedObjectType.CONTACTGROUP, true) || type.EqualsCSafe(PredefinedObjectType.PERSONA));
    }


    /// <summary>
    /// Returns number of bounces of the subscriber.
    /// </summary>
    private static int GetBouncesFromRow(DataRowView rowView)
    {
        return ValidationHelper.GetInteger(DataHelper.GetDataRowValue(rowView.Row, "SubscriberBounces"), 0);
    }


    /// <summary>
    /// Handles multiple selector actions.
    /// </summary>
    protected void btnOk_Clicked(object sender, EventArgs e)
    {
        // Check permissions
        CheckAuthorization();

        if (drpActions.SelectedValue != SELECT)
        {
            // Go through all selected items
            if (UniGridSubscribers.SelectedItems.Count != 0)
            {
                foreach (string subscriberId in UniGridSubscribers.SelectedItems)
                {
                    int subscriberIdInt = ValidationHelper.GetInteger(subscriberId, 0);

                    DoSubscriberAction(subscriberIdInt, drpActions.SelectedValue);
                }
            }
        }
        UniGridSubscribers.ResetSelection();
        UniGridSubscribers.ReloadData();
    }


    /// <summary>
    /// Performs action on given subscriber.
    /// </summary>
    /// <param name="subscriberId">Id of subscriber</param>
    /// <param name="actionName">Name of action</param>
    private void DoSubscriberAction(int subscriberId, string actionName)
    {
        try
        {
            // Check manage subscribers permission
            var subscriber = SubscriberInfoProvider.GetSubscriberInfo(subscriberId);
            if (!subscriber.CheckPermissions(PermissionsEnum.Modify, SiteContext.CurrentSiteName, MembershipContext.AuthenticatedUser))
            {
                RedirectToAccessDenied(subscriber.TypeInfo.ModuleName, "ManageSubscribers");
            }

            Func<bool> subscriberIsUserAndIsDisabled = () =>
            {
                var user = GetSubscriberUser(subscriber);
                return ((user != null) && !user.UserEnabled);
            };

            switch (actionName.ToUpperInvariant())
            {
                // Subscribe
                case SUBSCRIBE:
                    if (subscriberIsUserAndIsDisabled())
                    {
                        return;
                    }

                    var subscription = SubscriberNewsletterInfoProvider.GetSubscriberNewsletterInfo(subscriberId, mNewsletter.NewsletterID);
                    if ((subscription == null) || subscription.SubscriptionEnabled)
                    {
                        return;
                    }

                    SubscriberInfoProvider.RenewSubscription(subscriberId, mNewsletter.NewsletterID, chkSendConfirmation.Checked);
                    SubscriberNewsletterInfoProvider.SetApprovalStatus(subscriberId, mNewsletter.NewsletterID, !chkRequireOptIn.Checked);
                    if (chkRequireOptIn.Checked)
                    {
                        IssueInfoProvider.SendDoubleOptInEmail(subscriberId, mNewsletter.NewsletterID);
                    }
                    break;

                // Unsubscribe
                case UNSUBSCRIBE:
                    if (subscriberIsUserAndIsDisabled())
                    {
                        return;
                    }

                    SubscriberInfoProvider.Unsubscribe(subscriberId, mNewsletter.NewsletterID, chkSendConfirmation.Checked);
                    break;

                // Remove subscription
                case REMOVE:
                    SubscriberInfoProvider.DeleteSubscription(subscriberId, mNewsletter.NewsletterID, chkSendConfirmation.Checked);
                    break;

                // Approve subscription
                case APPROVE:
                    if (subscriberIsUserAndIsDisabled())
                    {
                        return;
                    }

                    SubscriberNewsletterInfoProvider.ApproveSubscription(subscriberId, mNewsletter.NewsletterID);
                    break;

                // Block selected subscriber
                case BLOCK:
                    SubscriberInfoProvider.BlockSubscriber(subscriberId);
                    break;

                // Un-block selected subscriber
                case UNBLOCK:
                    SubscriberInfoProvider.UnblockSubscriber(subscriberId);
                    break;
            }
        }
        catch (Exception e)
        {
            LogAndShowError("Newsletter subscriber", "NEWSLETTERS", e);
        }
    }

    #endregion
}