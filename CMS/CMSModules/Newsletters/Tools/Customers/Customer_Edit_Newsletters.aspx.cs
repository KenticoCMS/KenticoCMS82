using System;
using System.Data;

using CMS.Core;
using CMS.Helpers;
using CMS.Newsletters;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;
using CMS.DataEngine;

[Security(Resource = "CMS", UIElements = "CMSDesk.Ecommerce")]
[Security(Resource = "CMS.Ecommerce", UIElements = "Customers;Customers.Newsletters", ResourceSite = true)]
[Security(Resource = "CMS.Newsletter", Permission = "Read", ResourceSite = true)]
[CheckLicence(FeatureEnum.Newsletters)]
public partial class CMSModules_Newsletters_Tools_Customers_Customer_Edit_Newsletters : CMSDeskPage
{
    private int siteId = 0;
    private int customerSiteId = -1;
    private string currentValues = string.Empty;
    private string email = null;
    private string firstName = null;
    private string lastName = null;
    private int customerUserId = -1;


    protected void Page_Load(object sender, EventArgs e)
    {
        siteSelector.UniSelector.OnSelectionChanged += UniSelector_OnSelectionChanged;
        siteSelector.DropDownSingleSelect.AutoPostBack = true;
        if (!URLHelper.IsPostback())
        {
            siteSelector.SiteID = SiteContext.CurrentSiteID;
        }

        var user = MembershipContext.AuthenticatedUser;

        // Check e-commerce permission
        if (!user.IsAuthorizedPerResource("CMS.Ecommerce", "ReadCustomers") && !user.IsAuthorizedPerResource("CMS.Ecommerce", "EcommerceRead"))
        {
            RedirectToAccessDenied("CMS.Ecommerce", "ReadCustomers OR EcommerceRead");
        }

        // Skip loading customer data when ecommerce module is not present
        if (ModuleEntryManager.IsModuleLoaded(ModuleName.ECOMMERCE))
        {
            // Load customer data
            var customer = BaseAbstractInfoProvider.GetInfoById(PredefinedObjectType.CUSTOMER, QueryHelper.GetInteger("customerId", 0));
            if (customer != null)
            {
                // Check if customer is assigned to current site
                if (!user.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin) && !CustomerIsOnCurrentSite(customer))
                {
                    EditedObject = null;
                }

                email = customer.GetStringValue("CustomerEmail", string.Empty);
                firstName = customer.GetStringValue("CustomerFirstName", string.Empty);
                lastName = customer.GetStringValue("CustomerLastName", string.Empty);
                customerUserId = customer.GetIntegerValue("CustomerUserID", -1);
                customerSiteId = customer.GetIntegerValue("CustomerSiteID", SiteContext.CurrentSiteID);
            }
        }

        if ((email == null) || (email.Trim() == string.Empty) || (!ValidationHelper.IsEmail(email)))
        {
            headTitle.Visible = false;
            ShowError(GetString("ecommerce.customer.invalidemail"));
            usNewsletters.Visible = false;
        }

        usNewsletters.OnSelectionChanged += usNewsletters_OnSelectionChanged;

        SetWhereCondition();

        LoadSelection(false);
    }


    private void UniSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        SetWhereCondition();

        LoadSelection(true);
        usNewsletters.Reload(true);
    }


    private void usNewsletters_OnSelectionChanged(object sender, EventArgs e)
    {
        // Check 'EcommerceModify' permission
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Ecommerce", "EcommerceModify"))
        {
            // Check 'ModifyCustomers' permission if don't have general ecommerce modify permission
            if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Ecommerce", "ModifyCustomers"))
            {
                RedirectToAccessDenied("CMS.Ecommerce", "EcommerceModify OR ModifyCustomers");
            }
        }

        // Check if a customer has a valid email 
        if ((email != null) && (email.Trim() != string.Empty) && (ValidationHelper.IsEmail(email)))
        {
            // Check whether subscriber already exist
            var sb = GetSubscriber();
            if (sb == null)
            {
                // Create new subscriber
                sb = new SubscriberInfo
                     {
                         SubscriberEmail = email,
                         SubscriberFirstName = firstName,
                         SubscriberLastName = lastName,
                         SubscriberFullName = (firstName + " " + lastName).Trim(),
                         SubscriberSiteID = siteId,
                         SubscriberGUID = Guid.NewGuid()
                     };
                SubscriberInfoProvider.SetSubscriberInfo(sb);
            }

            var changed = false;

            // Remove old items
            string newValues = ValidationHelper.GetString(usNewsletters.Value, null);
            string items = DataHelper.GetNewItemsInList(newValues, currentValues);
            if (!String.IsNullOrEmpty(items))
            {
                string[] newItems = items.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string item in newItems)
                {
                    int newsletterId = ValidationHelper.GetInteger(item, 0);

                    // If subscriber is subscribed, unsubscribe him
                    if (SubscriberInfoProvider.IsSubscribed(sb.SubscriberID, newsletterId))
                    {
                        SubscriberInfoProvider.Unsubscribe(sb.SubscriberID, newsletterId);

                        changed = true;
                    }
                }
            }

            // Add new items
            items = DataHelper.GetNewItemsInList(currentValues, newValues);
            if (!String.IsNullOrEmpty(items))
            {
                string[] newItems = items.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string item in newItems)
                {
                    int newsletterId = ValidationHelper.GetInteger(item, 0);

                    // If subscriber is not subscribed, subscribe him
                    if (!SubscriberInfoProvider.IsSubscribed(sb.SubscriberID, newsletterId))
                    {
                        SubscriberInfoProvider.Subscribe(sb.SubscriberID, newsletterId, DateTime.Now);

                        changed = true;
                    }
                }
            }

            // Display information about successful (un)subscription if occurred
            if (changed)
            {
                ShowChangesSaved();
            }
        }
    }


    private void LoadSelection(bool force)
    {
        currentValues = string.Empty;

        var sb = GetSubscriber();

        if (sb != null)
        {
            // Get selected newsletters
            DataSet ds = SubscriberNewsletterInfoProvider.GetEnabledSubscriberNewsletters().WhereEquals("SubscriberID", sb.SubscriberID).Column("NewsletterID");
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                currentValues = TextHelper.Join(";", DataHelper.GetStringValues(ds.Tables[0], "NewsletterID"));
            }

            if (!RequestHelper.IsPostBack() || force)
            {
                // Load selected newsletters
                usNewsletters.Value = currentValues;
            }
        }
    }


    private void SetWhereCondition()
    {
        // Working with registered customer
        if (customerUserId > 0)
        {
            // Show site selector
            CurrentMaster.DisplaySiteSelectorPanel = true;

            // Show site selector for registered customer
            pnlSiteSelector.Visible = true;
            siteSelector.UserId = customerUserId;
            siteId = siteSelector.SiteID;
            // Show only selected site newsletters for registered customer
            if (siteId > 0)
            {
                usNewsletters.WhereCondition = "NewsletterSiteID = " + siteId;
            }
            // When "all sites" selected
            else
            {
                usNewsletters.WhereCondition = "NewsletterSiteID IN (SELECT SiteID FROM CMS_UserSite WHERE UserID = " + customerUserId + ")";
            }
        }
        else
        {
            siteId = SiteContext.CurrentSiteID;
            usNewsletters.WhereCondition = "NewsletterSiteID = " + ((customerSiteId > 0) ? customerSiteId : siteId);
        }

        usNewsletters.Enabled = siteId > 0;
    }


    private SubscriberInfo GetSubscriber()
    {
        var sb = SubscriberInfoProvider.GetSubscriberInfo(email, siteId);
        if (sb == null)
        {
            sb = SubscriberInfoProvider.GetSubscriberInfo(PredefinedObjectType.USER, customerUserId, siteId);
        }

        return sb;
    }


    private bool CustomerIsOnCurrentSite(BaseInfo customerInfo)
    {
        var currentSiteID = SiteContext.CurrentSiteID;
        if (customerInfo.GetIntegerValue("CustomerSiteID", 0) == currentSiteID)
        {
            return true;
        }

        var userID = customerInfo.GetIntegerValue("CustomerUserID", 0);

        return (userID > 0) && (UserSiteInfoProvider.GetUserSiteInfo(userID, currentSiteID) != null);
    }
}