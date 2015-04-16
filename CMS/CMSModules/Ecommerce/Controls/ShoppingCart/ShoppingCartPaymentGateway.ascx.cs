using System;
using System.Web.UI;

using CMS.Ecommerce;
using CMS.EcommerceProvider;
using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.DocumentEngine;
using CMS.Membership;

public partial class CMSModules_Ecommerce_Controls_ShoppingCart_ShoppingCartPaymentGateway : ShoppingCartStep
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // No payment provider loaded -> skip payment
        if (ShoppingCartControl.PaymentGatewayProvider == null)
        {
            // Clean current order payment result when editing existing order and payment was skipped
            if ((ShoppingCartControl.CheckoutProcessType == CheckoutProcessEnum.CMSDeskOrderItems) &&
                !ShoppingCartControl.IsCurrentStepPostBack)
            {
                CleanUpOrderPaymentResult();
            }

            // Raise payment skipped
            ShoppingCartControl.RaisePaymentSkippedEvent();

            // When on the live site
            if (!ShoppingCartControl.IsInternalOrder)
            {
                // Get Url the user should be redirected to
                string url = ShoppingCartControl.GetRedirectAfterPurchaseUrl();

                // Remove shopping cart data from database and from session
                ShoppingCartControl.CleanUpShoppingCart();

                URLHelper.Redirect(!string.IsNullOrEmpty(url) ? url : ShoppingCartControl.PreviousPageUrl);
            }
        }
        else if (ShoppingCart != null)
        {
            LoadData();
        }
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        // Set buttons properties
        if (!(ShoppingCartControl.PaymentGatewayProvider.IsPaymentCompleted) ||
            (ShoppingCartControl.CheckoutProcessType == CheckoutProcessEnum.CMSDeskOrderItems))
        {
            if (ShoppingCartControl.IsInternalOrder)
            {
                // Show 'Skip payment' button
                ShoppingCartControl.ButtonBack.ButtonStyle = ButtonStyle.Default;
                ShoppingCartControl.ButtonBack.Text = GetString("ShoppingCart.PaymentGateway.SkipPayment");
            }
            else
            {
                ShoppingCartControl.ButtonBack.Visible = false;
            }

            // Show 'Finish payment' button
            ShoppingCartControl.ButtonNext.ButtonStyle = ButtonStyle.Primary;
            ShoppingCartControl.ButtonNext.Text = GetString("ShoppingCart.PaymentGateway.FinishPayment");
        }
    }


    public override void ButtonNextClickAction()
    {
        // Standard action - Process payment
        base.ButtonNextClickAction();

        if (ShoppingCartControl.PaymentGatewayProvider.IsPaymentCompleted)
        {
            // Remove current shopping cart data from session and from database
            ShoppingCartControl.CleanUpShoppingCart();

            // Live site
            if (!ShoppingCartControl.IsInternalOrder)
            {
                string url = DocumentURLProvider.GetUrl(ShoppingCartControl.RedirectAfterPurchase != "" ? ShoppingCartControl.RedirectAfterPurchase : "/");
                URLHelper.Redirect(url);
            }
        }
    }


    public override void ButtonBackClickAction()
    {
        // Clean current order payment result when editing existing order and payment was skipped
        //if (this.ShoppingCartControl.CheckoutProcessType == CheckoutProcessEnum.CMSDeskOrderItems)
        //{
        //    CleanUpOrderPaymentResult();
        //}

        // Payment was skipped
        ShoppingCartControl.RaisePaymentSkippedEvent();

        // Remove current shopping cart data from session and from database
        ShoppingCartControl.CleanUpShoppingCart();

        // Live site - skip payment
        if (!ShoppingCartControl.IsInternalOrder)
        {
            string url = DocumentURLProvider.GetUrl(ShoppingCartControl.RedirectAfterPurchase != "" ? ShoppingCartControl.RedirectAfterPurchase : "/");
            URLHelper.Redirect(url);
        }
    }


    public override bool IsValid()
    {
        return ((ShoppingCartControl.PaymentGatewayProvider != null) && (ShoppingCartControl.PaymentGatewayProvider.ValidateCustomData() == ""));
    }


    public override bool ProcessStep()
    {
        if (ShoppingCartControl.PaymentGatewayProvider != null)
        {
            // Process current step payment gateway custom data
            ShoppingCartControl.PaymentGatewayProvider.ProcessCustomData();

            // Skip payment when already paid except when editing existing order or user is not authorized
            if (((!ShoppingCartControl.PaymentGatewayProvider.IsPaymentCompleted) ||
                (ShoppingCartControl.CheckoutProcessType == CheckoutProcessEnum.CMSDeskOrderItems)) &&
                ShoppingCartControl.PaymentGatewayProvider.IsUserAuthorizedToFinishPayment(MembershipContext.AuthenticatedUser, ShoppingCartControl.ShoppingCartInfoObj, !IsLiveSite))
            {
                // Process payment 
                ShoppingCartControl.PaymentGatewayProvider.ProcessPayment();
            }

            // Show info message
            if (ShoppingCartControl.PaymentGatewayProvider.InfoMessage != "")
            {
                lblInfo.Visible = true;
                lblInfo.Text = ShoppingCartControl.PaymentGatewayProvider.InfoMessage;
            }

            // Show error message
            if (ShoppingCartControl.PaymentGatewayProvider.ErrorMessage != "")
            {
                lblError.Visible = true;
                lblError.Text = ShoppingCartControl.PaymentGatewayProvider.ErrorMessage;
                return false;
            }

            if (ShoppingCartControl.PaymentGatewayProvider.IsPaymentCompleted)
            {
                // Delete current shopping cart after successful payment attempt
                ShoppingCartControl.CleanUpShoppingCart();
                // Raise payment completed event
                ShoppingCartControl.RaisePaymentCompletedEvent();
                return true;
            }
        }

        return false;
    }


    private void LoadData()
    {
        // Payment summary
        lblTotalPriceValue.Text = CurrencyInfoProvider.GetFormattedPrice(ShoppingCart.RoundedTotalPrice, ShoppingCart.Currency);
        lblOrderIdValue.Text = Convert.ToString(ShoppingCart.OrderId);
        if (ShoppingCart.PaymentOption != null)
        {
            lblPaymentValue.Text = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(ShoppingCart.PaymentOption.PaymentOptionDisplayName));
        }

        // Add payment gateway custom data
        Control paymentDataForm = LoadControl(ShoppingCartControl.PaymentGatewayProvider.GetPaymentDataFormPath());
        ShoppingCartControl.PaymentGatewayProvider.InitializeGatewayControl(PaymentDataContainer, paymentDataForm);

        UserInfo customerUser = null;
        if ((ShoppingCartControl.ShoppingCartInfoObj != null) && (ShoppingCartControl.ShoppingCartInfoObj.Customer != null))
        {
            customerUser = ShoppingCartControl.ShoppingCartInfoObj.Customer.CustomerUser;
        }

        // Disable next button for unauthorized payment method
        if (!ShoppingCartControl.PaymentGatewayProvider.IsUserAuthorizedToFinishPayment(customerUser, ShoppingCartControl.ShoppingCartInfoObj, !IsLiveSite))
        {
            ShoppingCartControl.ButtonNext.Enabled = false;
        }

        // Show "Order saved" info message
        if (!ShoppingCartControl.IsCurrentStepPostBack)
        {
            lblInfo.Text = GetString("ShoppingCart.PaymentGateway.OrderSaved");
            lblInfo.Visible = true;
        }
        else
        {
            lblInfo.Text = "";
        }
    }


    /// <summary>
    /// Clean up current order payment result.
    /// </summary>
    private void CleanUpOrderPaymentResult()
    {
        if (ShoppingCart != null)
        {
            OrderInfo oi = OrderInfoProvider.GetOrderInfo(ShoppingCart.OrderId);
            if (oi != null)
            {
                oi.OrderPaymentResult = null;
                OrderInfoProvider.SetOrderInfo(oi);
            }
        }
    }
}