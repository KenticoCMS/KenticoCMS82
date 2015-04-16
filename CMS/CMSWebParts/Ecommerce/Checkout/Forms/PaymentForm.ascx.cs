using System;
using System.Linq;
using System.Web.UI;

using CMS.Ecommerce;
using CMS.EcommerceProvider;
using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.PortalControls;
using CMS.EventLog;
using CMS.PortalEngine;
using CMS.Membership;

public partial class CMSWebParts_Ecommerce_Checkout_Forms_PaymentForm : CMSAbstractWebPart
{
    #region "Variables"

    /// <summary>
    /// Payment gateway provider.
    /// </summary>
    private CMSPaymentGatewayProvider mPaymentGatewayProvider;
    private int orderId;
    private ShoppingCartInfo mShoppingCart;

    #endregion


    #region "Properties"

    private ShoppingCartInfo ShoppingCart
    {
        get
        {
            return mShoppingCart ?? (mShoppingCart = ShoppingCartInfoProvider.GetShoppingCartInfoFromOrder(orderId));
        }
    }


    /// <summary>
    /// Payment gateway provider instance.
    /// </summary>
    public CMSPaymentGatewayProvider PaymentGatewayProvider
    {
        get
        {
            if ((mPaymentGatewayProvider == null) && (ShoppingCart != null))
            {
                // Get payment gateway provider instance
                mPaymentGatewayProvider = CMSPaymentGatewayProvider.GetPaymentGatewayProvider(ShoppingCart.ShoppingCartPaymentOptionID);
            }

            return mPaymentGatewayProvider;
        }
        set
        {
            mPaymentGatewayProvider = value;
        }
    }


    /// <summary>
    /// Page where the user should be redirected after successful payment.
    /// </summary>
    public string RedirectAfterPurchase
    {
        get
        {
            return ValidationHelper.GetString(GetValue("RedirectAfterPurchase"), "");
        }
        set
        {
            SetValue("RedirectAfterPurchase", value);
        }
    }


    /// <summary>
    /// Button text to be displayed on Process payment button.
    /// </summary>
    public string ProcessPaymentButtonText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ProcessPaymentButtonText"), "");
        }
        set
        {
            SetValue("ProcessPaymentButtonText", value);
        }
    }

    #endregion


    #region "Page methods"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        EnsureChildControls();
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        var isLiveSite = (PortalContext.ViewMode == ViewModeEnum.LiveSite) || IsLiveSite;
        // Allow provider initialization and redirection only for live site attempts
        if (isLiveSite && ((PaymentGatewayProvider == null) && (orderId > 0)))
        {
            URLHelper.Redirect(RedirectAfterPurchase);
        }

        // Cancel setup for undefined cart.
        if (ShoppingCart != null)
        {
            SetupControl();
        }

        btnProcessPayment.Text = ProcessPaymentButtonText;
    }

    #endregion


    #region "Event handling"

    protected void btnProcessPayment_Click(object sender, EventArgs e)
    {
        if ((PaymentGatewayProvider != null) && (orderId > 0))
        {
            // Validate data if web part is not placed in wizard
            if (!String.IsNullOrEmpty(PaymentGatewayProvider.ValidateCustomData()))
            {
                // Do not continue if validation failed
                return;
            }

            PaymentGatewayProvider.ProcessCustomData();

            // Skip payment when already paid or user is not authorized
            if (!PaymentGatewayProvider.IsPaymentCompleted &&
                PaymentGatewayProvider.IsUserAuthorizedToFinishPayment(MembershipContext.AuthenticatedUser, ShoppingCart, !IsLiveSite))
            {
                // Process payment 
                PaymentGatewayProvider.ProcessPayment();
            }

            // Show info message
            if (PaymentGatewayProvider.InfoMessage != "")
            {
                lblInfo.Visible = true;
                lblInfo.Text = PaymentGatewayProvider.InfoMessage;
            }

            // Show error message
            if (PaymentGatewayProvider.ErrorMessage != "")
            {
                ShowError(PaymentGatewayProvider.ErrorMessage);
            }

            // Redirect after successful payment
            if (PaymentGatewayProvider.IsPaymentCompleted && !string.IsNullOrEmpty(RedirectAfterPurchase))
            {
                URLHelper.Redirect(RedirectAfterPurchase);
            }
        }
    }

    #endregion


    #region "Wizard methods"

    /// <summary>
    /// Validates data.
    /// </summary>
    protected override void ValidateStepData(object sender, StepEventArgs e)
    {
        base.ValidateStepData(sender, e);

        if (!StopProcessing && (PaymentGatewayProvider != null))
        {
            if (PaymentGatewayProvider.ValidateCustomData() != "")
            {
                e.CancelEvent = true;
            }
        }
    }


    /// <summary>
    /// Saves the data.
    /// </summary>
    protected override void SaveStepData(object sender, StepEventArgs e)
    {
        base.SaveStepData(sender, e);

        if (PaymentGatewayProvider != null)
        {
            // Process inserted custom payment data from the payment form
            PaymentGatewayProvider.ProcessCustomData();
        }
    }

    #endregion


    #region "Methods"

    protected override void CreateChildControls()
    {
        string orderHash = QueryHelper.GetString("o", string.Empty);
        orderId = ValidationHelper.GetInteger(WindowHelper.GetItem(orderHash), 0);

        base.CreateChildControls();

        var provider = PaymentGatewayProvider;
        if (provider != null)
        {
            try
            {
                // Init paymentDataContainer
                Control paymentDataForm = LoadControl(provider.GetPaymentDataFormPath());
                provider.InitializeGatewayControl(pnlPaymentDataContainer, paymentDataForm);
                provider.OrderId = orderId;
            }
            catch (Exception ex)
            {
                EventLogProvider.LogException("PaymentForm", "EXCEPTION", ex);
            }
        }
    }


    private void SetupControl()
    {
        // Payment and order summary
        lblTotalPriceValue.Text = CurrencyInfoProvider.GetFormattedPrice(ShoppingCart.RoundedTotalPrice, ShoppingCart.Currency);
        lblOrderIdValue.Text = Convert.ToString(orderId);

        if (ShoppingCart.PaymentOption != null)
        {
            lblPaymentValue.Text = GetString(ShoppingCart.PaymentOption.PaymentOptionDisplayName);
        }

        // Payment form is visible only if payment method is selected
        if (PaymentGatewayProvider == null)
        {
            ShowError(GetString("com.checkout.paymentoptionnotselected"));
        }
    }


    private void ShowError(string text)
    {
        pnlError.Visible = true;
        lblError.Text = text;
    }

    #endregion
}