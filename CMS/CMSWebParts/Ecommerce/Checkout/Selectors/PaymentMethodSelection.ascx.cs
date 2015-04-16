using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using CMS.Ecommerce;
using CMS.ExtendedControls;
using CMS.Helpers;

/// <summary>
/// Payment selector web part
/// </summary>
public partial class CMSWebParts_Ecommerce_Checkout_Selectors_PaymentMethodSelection : CMSCheckoutWebPart
{
    #region "Event handling"

    /// <summary>
    /// OnInit event handler.
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Enable post back for the drop-down list and subscribe to the selection change
        drpPayment.AutoPostBack = true;
        // Subscribe to the wizard events
        SubscribeToWizardEvents();
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        SetupControl();
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        // Set currency for the shopping cart according to the selected value
        ShoppingCart.ShoppingCartPaymentOptionID = drpPayment.SelectedID;
    }


    /// <summary>
    /// Updates the web part according to the new shopping cart values.
    /// </summary>
    public void Update(object sender, EventArgs e)
    {
        SetupControl();
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Validates the data.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">The StepEventArgs instance containing the event data.</param>
    protected override void ValidateStepData(object sender, StepEventArgs e)
    {
        base.ValidateStepData(sender, e);

        bool valid = true;
        lblError.Visible = false;
        string message;

        if (drpPayment.SelectedID == 0)
        {
            message = ResHelper.GetString("com.checkoutprocess.paymentneeded");
            valid = false;
        }
        // Check whether payment option is valid for user.
        else if (!CheckPaymentOptionIsValidForUser(ShoppingCart, out message))
        {
            valid = false;
        }

        if (!valid)
        {
            e.CancelEvent = true;
            lblError.Text = message;
            lblError.Visible = true;
        }
    }


    /// <summary>
    /// Subscribes the web part to the wizard events.
    /// </summary>
    private void SubscribeToWizardEvents()
    {
        ComponentEvents.RequestEvents.RegisterForEvent(SHOPPING_CART_CHANGED, Update);
    }


    /// <summary>
    /// Resets the selector.
    /// </summary>
    protected void ResetSelector()
    {
        drpPayment.Reload();
        ShoppingCart.ShoppingCartPaymentOptionID = 0;
        drpPayment.SelectedID = ShoppingCart.ShoppingCartPaymentOptionID;
    }


    /// <summary>
    /// Preselects payment option.
    /// </summary>
    protected void PreselectPaymentOption()
    {
        // Try to select payment option according to saved option in shopping cart object
        CustomerInfo customer = ShoppingCart.Customer;
        int paymentOptionId = 0;

        // At first try to select preselected payment option according to ShoppingCart object
        if (ShoppingCart.ShoppingCartPaymentOptionID > 0)
        {
            paymentOptionId = ShoppingCart.ShoppingCartPaymentOptionID;
        }
        else if (customer != null)
        {
            // Try to select the payment option according to the customer object
            if (customer.CustomerPreferredPaymentOptionID > 0)
            {
                paymentOptionId = customer.CustomerPreferredPaymentOptionID;
            }
            else
            {
                // Try to get at least preferred site payment option
                paymentOptionId = (customer.CustomerUser != null) ? customer.CustomerUser.GetUserPreferredPaymentOptionID(ShoppingCart.SiteName) : 0;
            }
        }

        // If a payment option was detected, set it on the UniSelector and on the ShoppingCart object
        if (paymentOptionId > 0)
        {
            drpPayment.Reload();
            ShoppingCart.ShoppingCartPaymentOptionID = paymentOptionId;
            drpPayment.SelectedID = paymentOptionId;
        }
    }


    /// <summary>
    /// Control initialization.
    /// </summary>
    public void SetupControl()
    {
        if (!StopProcessing)
        {
            // Set up empty record text. The macro ResourcePrefix + .empty represents empty record value.
            drpPayment.UniSelector.ResourcePrefix = "com.livesiteselector";

            DataSet dsOptions;

            // Get correct payment options if shipping is set or not
            if (ShoppingCart.ShippingOption != null)
            {
                dsOptions = PaymentOptionInfoProvider.GetPaymentOptionsForShipping(ShoppingCart.ShippingOption.ShippingOptionID, true)
                                .Column("PaymentOptionID")
                                .OrderBy("PaymentOptionDisplayName");
            }
            else
            {
                dsOptions = PaymentOptionInfoProvider.GetPaymentOptions(ShoppingCart.ShoppingCartSiteID, true)
                                .Column("PaymentOptionID")
                                .WhereTrue("PaymentOptionAllowIfNoShipping")
                                .OrderBy("PaymentOptionDisplayName");
            }

            IList<int> paymentIds = new List<int>();

            if (!DataHelper.DataSourceIsEmpty(dsOptions))
            {
                paymentIds = DataHelper.GetIntegerValues(dsOptions.Tables[0], "PaymentOptionID");
            }

            // If there is only one payment method set it.
            if (paymentIds.Count == 1)
            {
                ShoppingCart.ShoppingCartPaymentOptionID = paymentIds.FirstOrDefault();
                drpPayment.SelectedID = ShoppingCart.ShoppingCartPaymentOptionID;
            }

            // Set selected shipping option to determine available payment options
            drpPayment.ShippingOptionID = ShoppingCart.ShoppingCartShippingOptionID;
            drpPayment.DisplayOnlyAllowedIfNoShipping = (drpPayment.ShippingOptionID <= 0);

            if ((ShoppingCart.ShoppingCartPaymentOptionID != 0) && !paymentIds.Contains(ShoppingCart.ShoppingCartPaymentOptionID))
            {
                // Reset selector on shipping changed event if selected payment is not allowed for current shipping (zero shipping id is Please select state).
                ResetSelector();
            }

            // Update selection
            if (!RequestHelper.IsPostBack())
            {
                PreselectPaymentOption();
            }

            drpPayment.Reload();
        }
    }

    #endregion
}