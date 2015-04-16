using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CMS.ExtendedControls;
using CMS.PortalControls;
using CMS.Ecommerce;
using CMS.Base;
using CMS.DataEngine;
using CMS.EventLog;
using CMS.Helpers;
using CMS.Localization;
using CMS.Protection;
using CMS.SiteProvider;
using CMS.WebAnalytics;
using CMS.Membership;
using CMS.EcommerceProvider;

/// <summary>
/// Base class for checkout web parts.
/// </summary>
public class CMSCheckoutWebPart : CMSAbstractWebPart
{
    #region "Event names constants"

    public const string SHOPPING_CART_CHANGED = "ShoppingCartChanged";
    public const string MESSAGE_RAISED = "ShoppingCartMessageRised";

    #endregion


    #region "Constants"

    private const string CHOP_FINALIZED_KEY = "ChopFinalized";
    private const string LOG_OFF_VALIDATION = "LogoffValidation";

    private const string EVENT_CODE_VALIDATION = "VALIDATION";
    private const string EVENT_CODE_SAVING = "SAVEOBJ";
    private const string EVENT_CODE_EXCEPTION = "EXCEPTION";

    private const string EVENT_SOURCE = "Checkout";

    private const string HTML_SEPARATOR = "<br />";

    #endregion


    #region "Variables"

    private int mContactID;

    private readonly List<Tuple<string, string>> loggedErrors = new List<Tuple<string, string>>();
    private readonly List<Exception> loggedExceptions = new List<Exception>();
    private string registrationBanned = String.Empty;

    #endregion


    #region "Properties"

    /// <summary>
    /// Current ShoppingCart
    /// </summary>
    public ShoppingCartInfo ShoppingCart
    {
        get
        {
            return ECommerceContext.CurrentShoppingCart;
        }
    }


    /// <summary>
    /// Current customer.
    /// </summary>
    public CustomerInfo Customer
    {
        get
        {
            return ShoppingCart.Customer;
        }
    }


    /// <summary>
    /// Contact ID passed between shopping cart steps.
    /// </summary>
    public int ContactID
    {
        get
        {
            if (mContactID <= 0)
            {
                mContactID = ModuleCommands.OnlineMarketingGetCurrentContactID();
            }
            return mContactID;
        }
        set
        {
            mContactID = value;
        }
    }

    #endregion


    #region "Wizard methods"

    protected override void LoadStep(object sender, StepEventArgs e)
    {
        base.LoadStep(sender, e);

        // Ensure log off check only once
        if (e.GetValue(LOG_OFF_VALIDATION) == null)
        {
            e.SetValue(LOG_OFF_VALIDATION, true);
            // Get current shopping cart user id, 0 for null (public user)
            int currentUserId = ShoppingCart.User == null ? 0 : ShoppingCart.User.UserID;
            // Get last known user saved in previous load of WP. In case there is no record in session current user id is retrieved.
            object userIdSessionObject = SessionHelper.GetValue("CheckoutUserID");
            int lastKnownUserID = ValidationHelper.GetInteger(userIdSessionObject, currentUserId);

            // Reset checkout process in case of different users. Possible log-off or login as another user
            if (lastKnownUserID != currentUserId)
            {
                DocumentWizardManager.ResetWizard();

                SessionHelper.Remove("CheckoutUserID");
                // Refresh page for wizard to jump at first step
                URLHelper.Redirect(RequestContext.CurrentURL);
            }

            // Set user id to session for non public users. Change from public user to authorized does not reset checkout process.
            if (currentUserId != 0)
            {
                SessionHelper.SetValue("CheckoutUserID", currentUserId);
            }
            // Clean session entry (if there was one) for public user in case of log-off action to remove previous user id.
            else if (userIdSessionObject != null)
            {
                SessionHelper.Remove("CheckoutUserID");
            }
        }
    }


    protected override void StepFinished(object sender, StepEventArgs e)
    {
        base.StepFinished(sender, e);
        // We are on last step and checkout process has not been finalized yet by any web part
        if (e.IsLastStep && (e.GetValue(CHOP_FINALIZED_KEY) == null))
        {
            // Handle automatic selection of payment and shipping options
            HandleAutomaticSelections(ShoppingCart);

            string validationMessage;
            // Validate cart; in case of failure user is able to go through checkout process and fix errors
            if (!ValidateShoppingCart(ShoppingCart, out validationMessage))
            {
                e.CancelEvent = true;
                ShowError(validationMessage);
                e.SetValue(CHOP_FINALIZED_KEY, true);
                return;
            }

            if (FinalizeCheckout())
            {
                int orderId = ShoppingCart.OrderId;
                string orderHash = ShoppingCart.GetHashCode().ToString();
                WindowHelper.Add(orderHash, orderId);
                // Create URL for payment page with order id hidden in hash
                e.FinalStepUrl = URLHelper.AddParameterToUrl(e.FinalStepUrl, "o", orderHash);
            }
            else
            {
                // Log events created in transaction
                foreach (Tuple<string, string> error in loggedErrors)
                {
                    EventLogProvider.LogEvent(EventType.ERROR, EVENT_SOURCE, error.Item2, error.Item1);
                }

                foreach (Exception ex in loggedExceptions)
                {
                    EventLogProvider.LogException(EVENT_SOURCE, EVENT_CODE_EXCEPTION, ex);
                }

                e.CancelEvent = true;
                // Get error text
                string errorMessage = HTMLHelper.HTMLEncode(ResHelper.GetString("ecommerce.orderpreview.errorordersave"));

                if (!string.IsNullOrEmpty(registrationBanned))
                {
                    errorMessage += HTMLHelper.HTMLEncode(Environment.NewLine + registrationBanned);
                }

                ShowError(errorMessage);
            }

            CleanUpShoppingCart();
            DocumentWizardManager.ResetWizard();

            e.SetValue(CHOP_FINALIZED_KEY, true);
        }
    }

    #endregion


    #region "Methods"

    private void ShowError(string errorMessage)
    {
        // Try to show message through Message Panel web part
        CMSEventArgs<string> args = new CMSEventArgs<string>();
        args.Parameter = errorMessage;
        ComponentEvents.RequestEvents.RaiseEvent(this, args, MESSAGE_RAISED);

        // If Message Panel web part is not present (Parameter is cleared by web part after successful handling), show message through alert script
        if (!string.IsNullOrEmpty(args.Parameter))
        {
            errorMessage = errorMessage.Replace(HTML_SEPARATOR, Environment.NewLine);
            ScriptHelper.Alert(Page, errorMessage);
        }
    }


    /// <summary>
    /// Hides the user-defined content of the web part (envelope, title...).
    /// </summary>
    protected void HideWebPartContent()
    {
        ContentBefore = string.Empty;
        ContentAfter = string.Empty;
        ContainerTitle = string.Empty;
        ContainerName = string.Empty;
    }


    private bool FinalizeCheckout()
    {
        using (var scope = new CMSTransactionScope())
        {
            ShoppingCartInfo currentShoppingCart = ShoppingCart;

            // Validate breaking errors (No recovery)
            if (!CheckBreakingErrors(currentShoppingCart))
            {
                return false;
            }
            // Ensure customer is saved
            if (!EnsureCartCustomer(currentShoppingCart))
            {
                return false;
            }
            // Ensure customers address is saved
            if (!EnsureCustomerAddresses(currentShoppingCart))
            {
                return false;
            }
            // Create and save Order
            if (!CreateOrder(currentShoppingCart))
            {
                return false;
            }

            if (!HandleAutoRegistration(currentShoppingCart))
            {
                return false;
            }

            HandleOrderNotification(currentShoppingCart);
            scope.Commit();
        }

        return true;
    }


    private void HandleAutomaticSelections(ShoppingCartInfo currentShoppingCart)
    {
        if ((currentShoppingCart.ShippingOption == null) && currentShoppingCart.IsShippingNeeded)
        {
            // Try to select shipping option if there is only one in system
            var query = ShippingOptionInfoProvider.GetShippingOptions(currentShoppingCart.ShoppingCartSiteID, true).Column("ShippingOptionID");
            if (query.Count == 1)
            {
                currentShoppingCart.ShoppingCartShippingOptionID = DataHelper.GetIntegerValues(query.Tables[0], "ShippingOptionID").FirstOrDefault();
            }
        }

        if (currentShoppingCart.PaymentOption == null)
        {
            // Try to select payment option if there is only one in system
            var query = PaymentOptionInfoProvider.GetPaymentOptions(currentShoppingCart.ShoppingCartSiteID, true).Column("PaymentOptionID");
            if (query.Count == 1)
            {
                int paymentOptionId = DataHelper.GetIntegerValues(query.Tables[0], "PaymentOptionID").FirstOrDefault();
                // Check if payment is allowed for shipping, or shipping is not set
                if (CheckPaymentIsAllowedForShipping(currentShoppingCart, paymentOptionId))
                {
                    currentShoppingCart.ShoppingCartPaymentOptionID = paymentOptionId;
                }
            }
        }
    }


    private static bool CheckPaymentIsAllowedForShipping(ShoppingCartInfo currentShoppingCart, int paymentOptionId)
    {
        return (currentShoppingCart.ShoppingCartShippingOptionID == 0) ||
               (PaymentOptionInfoProvider.GetPaymentOptionsForShipping(currentShoppingCart.ShoppingCartShippingOptionID, true)
                                         .Where("PaymentOptionID", QueryOperator.Equals, paymentOptionId)
                                         .TopN(1)
                                         .Any());
    }


    /// <summary>
    /// Validates the shopping cart. In case of error user is able to go through CHOP again with the same cart object.
    /// </summary>
    /// <param name="shoppingCart">The shopping cart.</param>
    /// <param name="errorMessage">The error message.</param>    
    private bool ValidateShoppingCart(ShoppingCartInfo shoppingCart, out string errorMessage)
    {
        bool valid = true;
        StringBuilder sb = new StringBuilder();

        // Check shopping cart items.
        // The following conditions must be met to pass the check:
        // 1)All shopping cart items are enabled 2)Max units in one order are not exceeded 
        // 3)There is enough units in the inventory 4) Customer is registered, if there is a membership type product in the cart
        // 5)Product validity is valid, if there is a membership or e-product type product in the cart
        // 6)Selected shipping option is applicable to cart
        var result = ShoppingCartInfoProvider.CheckShoppingCart(shoppingCart);
        if (result.CheckFailed || shoppingCart.IsEmpty)
        {
            valid = false;
            sb.Append(result.GetHTMLFormattedMessage());
            sb.Append(HTML_SEPARATOR);
        }

        // Check PaymentOption
        if (shoppingCart.PaymentOption == null)
        {
            valid = false;
            sb.Append(GetString("com.checkout.paymentoptionnotselected"));
            sb.Append(HTML_SEPARATOR);
        }

        // Check whether payment option is valid for user.
        string message;
        if (!CheckPaymentOptionIsValidForUser(shoppingCart, out message))
        {
            valid = false;
            sb.Append(message);
            sb.Append(HTML_SEPARATOR);
        }

        // Check payment is valid for cart shipping
        if (!CheckPaymentIsAllowedForShipping(shoppingCart, shoppingCart.ShoppingCartPaymentOptionID))
        {
            valid = false;
            sb.Append(GetString("com.checkout.paymentoptionnotapplicable"));
            sb.Append(HTML_SEPARATOR);
        }

        // If there is at least one product that needs shipping and shipping is not selected
        if (shoppingCart.IsShippingNeeded && (shoppingCart.ShippingOption == null))
        {
            valid = false;
            sb.Append(GetString("com.checkoutprocess.shippingneeded"));
            sb.Append(HTML_SEPARATOR);
        }

        errorMessage = TextHelper.TrimEndingWord(sb.ToString(), HTML_SEPARATOR);
        return valid;
    }


    private bool CheckBreakingErrors(ShoppingCartInfo shoppingCart)
    {
        bool valid = true;
        // Check currency
        if (shoppingCart.Currency == null)
        {
            valid = false;
            LogError("Missing currency", EVENT_CODE_VALIDATION);
        }

        // Check customer
        if (shoppingCart.Customer == null)
        {
            valid = false;
            LogError("Missing customer", EVENT_CODE_VALIDATION);
        }

        // Check BillingAddress
        if (shoppingCart.ShoppingCartBillingAddress == null)
        {
            valid = false;
            LogError("Missing billing address", EVENT_CODE_VALIDATION);
        }

        return valid;
    }


    private bool EnsureCartCustomer(ShoppingCartInfo shoppingCart)
    {
        CustomerInfo customer = shoppingCart.Customer;
        customer.CustomerSiteID = shoppingCart.ShoppingCartSiteID;
        customer.CustomerUserID = shoppingCart.ShoppingCartUserID;

        CustomerInfoProvider.SetCustomerInfo(customer);
        shoppingCart.ShoppingCartCustomerID = customer.CustomerID;

        if (customer.CustomerID > 0)
        {
            // Track successful registration conversion
            string name = ECommerceSettings.RegistrationConversionName(shoppingCart.SiteName);
            ECommerceHelper.TrackRegistrationConversion(shoppingCart.SiteName, name);
            // Log customer registration activity
            var activityCustomerRegistration = new ActivityCustomerRegistration(customer, MembershipContext.AuthenticatedUser, AnalyticsContext.ActivityEnvironmentVariables);
            if (activityCustomerRegistration.Data != null)
            {
                if ((ContactID <= 0) && (customer.CustomerUser != null))
                {
                    activityCustomerRegistration.Data.ContactID = ModuleCommands.OnlineMarketingGetUserLoginContactID(customer.CustomerUser);
                }
                else
                {
                    activityCustomerRegistration.Data.ContactID = ContactID;
                }

                activityCustomerRegistration.Log();
            }

            return true;
        }
        else
        {
            LogError("Save customer action failed", EVENT_CODE_SAVING);
            return false;
        }
    }


    private bool EnsureCustomerAddresses(ShoppingCartInfo shoppingCart)
    {
        // Billing
        if (shoppingCart.ShoppingCartBillingAddress != null)
        {
            shoppingCart.ShoppingCartBillingAddress = SaveAddress(shoppingCart.ShoppingCartBillingAddress, shoppingCart.Customer);

            if (shoppingCart.ShoppingCartBillingAddress.AddressID < 1)
            {
                LogError("Save billing address action failed", EVENT_CODE_SAVING);
                return false;
            }

            // Update current contact's address
            MapContactAddress(shoppingCart.ShoppingCartBillingAddress);
        }

        // Shipping        
        if (shoppingCart.ShoppingCartShippingAddress != null)
        {
            shoppingCart.ShoppingCartShippingAddress = SaveAddress(shoppingCart.ShoppingCartShippingAddress, shoppingCart.Customer);

            if (shoppingCart.ShoppingCartShippingAddress.AddressID < 1)
            {
                LogError("Save shipping address action failed", EVENT_CODE_SAVING);
                return false;
            }
        }

        // Company
        if (shoppingCart.ShoppingCartCompanyAddress != null)
        {
            shoppingCart.ShoppingCartCompanyAddress = SaveAddress(shoppingCart.ShoppingCartCompanyAddress, shoppingCart.Customer);

            if (shoppingCart.ShoppingCartCompanyAddress.AddressID < 1)
            {
                LogError("Save company address action failed", EVENT_CODE_SAVING);
                return false;
            }
        }

        return true;
    }


    private IAddress SaveAddress(IAddress addressObject, CustomerInfo customer)
    {
        AddressInfo address = addressObject as AddressInfo;

        if (address == null)
            return null;

        if (string.IsNullOrEmpty(address.AddressPersonalName))
        {
            address.AddressPersonalName = TextHelper.LimitLength(string.Format("{0} {1}", customer.CustomerFirstName, customer.CustomerLastName), 200);
        }

        address.AddressCustomerID = customer.CustomerID;
        address.AddressName = AddressInfoProvider.GetAddressName(address);
        AddressInfoProvider.SetAddressInfo(address);

        return address;
    }


    private bool CreateOrder(ShoppingCartInfo shoppingCart)
    {
        try
        {
            // Set order culture
            shoppingCart.ShoppingCartCulture = LocalizationContext.PreferredCultureCode;
            // Update customer preferences
            CustomerInfoProvider.SetCustomerPreferredSettings(shoppingCart);
            // Create order
            ShoppingCartInfoProvider.SetOrder(shoppingCart);
        }
        catch (Exception ex)
        {
            // Log exception
            loggedExceptions.Add(ex);
            return false;
        }

        if (shoppingCart.OrderId > 0)
        {
            // Track order conversion        
            var name = ECommerceSettings.OrderConversionName(shoppingCart.SiteName);
            ECommerceHelper.TrackOrderConversion(shoppingCart, name);
            ECommerceHelper.TrackOrderItemsConversions(shoppingCart);
            // Log purchase activity
            if (ActivitySettingsHelper.ActivitiesEnabledForThisUser(MembershipContext.AuthenticatedUser))
            {
                var totalPriceInMainCurrency = shoppingCart.TotalPriceInMainCurrency;
                var formattedPrice = CurrencyInfoProvider.GetFormattedPrice(totalPriceInMainCurrency, CurrencyInfoProvider.GetMainCurrency(shoppingCart.ShoppingCartSiteID));

                TrackActivityPurchasedProducts(shoppingCart, ContactID);
                TrackActivityPurchase(shoppingCart.OrderId,
                                      ContactID,
                                      totalPriceInMainCurrency,
                                      formattedPrice);
            }

            return true;
        }

        LogError("Save order action failed", EVENT_CODE_SAVING);
        return false;
    }


    private static void HandleOrderNotification(ShoppingCartInfo shoppingCart)
    {
        if (ECommerceSettings.SendOrderNotification(shoppingCart.SiteName))
        {
            // Send order notification emails
            OrderInfoProvider.SendOrderNotificationToAdministrator(shoppingCart);
            OrderInfoProvider.SendOrderNotificationToCustomer(shoppingCart);
        }
    }


    private bool HandleAutoRegistration(ShoppingCartInfo currentShoppingCart)
    {
        registrationBanned = string.Empty;
        var customer = currentShoppingCart.Customer;

        if ((customer == null) || customer.CustomerIsRegistered)
        {
            return true;
        }

        if (ECommerceSettings.AutomaticCustomerRegistration(currentShoppingCart.ShoppingCartSiteID) || currentShoppingCart.RegisterAfterCheckout)
        {
            // Ban IP addresses which are blocked for registration
            var registrationBan = !BannedIPInfoProvider.IsAllowed(currentShoppingCart.SiteName, BanControlEnum.Registration);
            var allUserActionBan = !BannedIPInfoProvider.IsAllowed(currentShoppingCart.SiteName, BanControlEnum.AllNonComplete);

            if (registrationBan || allUserActionBan)
            {
                registrationBanned = GetString("banip.ipisbannedregistration");
                LogError(registrationBanned, EVENT_CODE_VALIDATION);
                return false;
            }
            // Auto-register user and send mail notification
            CustomerInfoProvider.RegisterAndNotify(customer, currentShoppingCart.RegisterAfterCheckoutTemplate);
        }

        return true;
    }


    private void LogError(string errorMessage, string code)
    {
        loggedErrors.Add(new Tuple<string, string>(errorMessage, code));
    }


    /// <summary>
    /// Removes current shopping cart data from database and from session.
    /// </summary>
    private void CleanUpShoppingCart()
    {
        if (ShoppingCart != null)
        {
            ShoppingCartInfoProvider.DeleteShoppingCartInfo(ShoppingCart.ShoppingCartID);
            ECommerceContext.CurrentShoppingCart = null;
        }
    }


    /// <summary>
    /// Checks whether the payment option is valid for current user.
    /// </summary>
    /// <param name="shoppingCart">The shopping cart.</param>
    /// <param name="message">The message in case of failure.</param>    
    protected bool CheckPaymentOptionIsValidForUser(ShoppingCartInfo shoppingCart, out string message)
    {
        message = string.Empty;
        CMSPaymentGatewayProvider provider = CMSPaymentGatewayProvider.GetPaymentGatewayProvider(shoppingCart.ShoppingCartPaymentOptionID);

        if ((provider != null) && (!provider.IsUserAuthorizedToFinishPayment(shoppingCart.User, shoppingCart)))
        {
            message = provider.ErrorMessage;
            return false;
        }

        return true;
    }


    /// <summary>
    /// Logs activity "purchase" for all items.
    /// </summary>
    /// <param name="shoppingCartInfoObj">Shopping cart</param>
    /// <param name="contactId">Contact ID</param>
    private void TrackActivityPurchasedProducts(ShoppingCartInfo shoppingCartInfoObj, int contactId)
    {
        // Check if shopping contains any items
        if ((shoppingCartInfoObj == null) || (shoppingCartInfoObj.IsEmpty))
        {
            return;
        }
        // Loop through all products and log activity
        var variables = AnalyticsContext.ActivityEnvironmentVariables;
        foreach (ShoppingCartItemInfo cartItem in shoppingCartInfoObj.CartProducts)
        {
            string skuName = ResHelper.LocalizeString(cartItem.SKU.SKUName) + " (ID#:" + cartItem.SKUID + ")";
            Activity activity = new ActivityPurchasedProduct(skuName, cartItem, variables);
            if (activity.Data != null)
            {
                activity.Data.ContactID = contactId;
                activity.CheckViewMode = false;
                activity.Log();
            }
        }
    }


    /// <summary>
    /// Logs activity "purchase".
    /// </summary>
    /// <param name="orderId">Order ID</param>
    /// <param name="contactId">Contact ID</param>
    /// <param name="totalPrice">Total price</param>
    /// <param name="totalPriceAsString">Total price user friendly formatted</param>
    private void TrackActivityPurchase(int orderId, int contactId, double totalPrice, string totalPriceAsString)
    {
        Activity activity = new ActivityPurchase(totalPriceAsString, orderId, totalPrice, AnalyticsContext.ActivityEnvironmentVariables);
        if (activity.Data != null)
        {
            activity.Data.ContactID = contactId;
            activity.CheckViewMode = false;
            activity.Log();
        }
    }


    /// <summary>
    /// Updates contact's address.
    /// </summary>
    /// <param name="addressInfo">Billing address</param>
    private void MapContactAddress(IAddress addressInfo)
    {
        try
        {
            if ((addressInfo == null) || !SettingsKeyInfoProvider.GetBoolValue(SiteContext.CurrentSiteName + ".CMSEnableOnlineMarketing"))
            {
                return;
            }

            GeneralizedInfo contactInfo = BaseAbstractInfoProvider.GetInfoById(PredefinedObjectType.CONTACT, ContactID);

            // Check that current contact has not yet filled in address
            if ((contactInfo != null) && String.IsNullOrEmpty(ValidationHelper.GetString(contactInfo.GetValue("ContactAddress1"), "")))
            {
                Func<int, int?> getIntIfValid = i => i > 0 ? i : (int?)null;

                contactInfo.SetValue("ContactAddress1", addressInfo.AddressLine1);
                contactInfo.SetValue("ContactAddress2", addressInfo.AddressLine2);
                contactInfo.SetValue("ContactCity", addressInfo.AddressCity);
                contactInfo.SetValue("ContactZIP", addressInfo.AddressZip);
                contactInfo.SetValue("ContactMobilePhone", addressInfo.AddressPhone);
                contactInfo.SetValue("ContactCountryID", getIntIfValid(addressInfo.AddressCountryID));
                contactInfo.SetValue("ContactStateID", getIntIfValid(addressInfo.AddressStateID));
                contactInfo.SetObject();
            }
        }
        catch (Exception ex)
        {
            // Exception could happen when max length of contact parameters is exceeded
            EventLogProvider.LogException("ShoppingCartOrderAddresses.MapContactAddress", "UPDATECONTACT", ex);
        }
    }

    #endregion
}