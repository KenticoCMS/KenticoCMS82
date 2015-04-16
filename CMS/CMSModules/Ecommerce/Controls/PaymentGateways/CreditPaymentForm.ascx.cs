using System;

using CMS.EcommerceProvider;

public partial class CMSModules_Ecommerce_Controls_PaymentGateways_CreditPaymentForm : CreditPaymentForm
{
    protected void Page_Load(object sender, EventArgs e)
    {
        CMSCreditPaymentProvider provider = PaymentProvider as CMSCreditPaymentProvider;

        if (provider != null)
        {
            // If user is not authorized to finish payment
            if (!provider.IsUserAuthorizedToFinishPayment(!IsLiveSite))
            {
                // Display error message
                lblError.Visible = true;
                lblError.Text = provider.ErrorMessage;
            }
            else
            {
                // Reloads available credit
                provider.ReloadPaymentData();
                DisplayAvailableCredit();
            }
        }
    }


    /// <summary>
    /// Displays available credit.
    /// </summary>
    public override void DisplayAvailableCredit()
    {
        var cmsCreditProvider = PaymentProvider as CMSCreditPaymentProvider;

        if ((cmsCreditProvider == null) || (cmsCreditProvider.MainCurrencyObj == null) || (cmsCreditProvider.OrderCurrencyObj == null))
        {
            return;
        }

        // Order currency is different from main currency
        if (cmsCreditProvider.MainCurrencyObj.CurrencyID != cmsCreditProvider.OrderCurrencyObj.CurrencyID)
        {
            // Set available credit string
            lblCreditValue.Text = cmsCreditProvider.OrderCurrencyObj.FormatPrice(cmsCreditProvider.AvailableCreditInOrderCurrency);
            lblCreditValue.Text += "(" + cmsCreditProvider.MainCurrencyObj.FormatPrice(cmsCreditProvider.AvailableCreditInMainCurrency) + ")";
        }
        // Order currency is equal to main currency
        else
        {
            lblCreditValue.Text = cmsCreditProvider.MainCurrencyObj.FormatPrice(cmsCreditProvider.AvailableCreditInMainCurrency);
        }
    }
}