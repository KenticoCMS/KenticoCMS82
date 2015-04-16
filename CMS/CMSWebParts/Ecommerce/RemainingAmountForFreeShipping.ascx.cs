using System;
using System.Text;

using CMS.Ecommerce;
using CMS.PortalControls;
using CMS.Helpers;
using CMS.EventLog;

public partial class CMSWebParts_Ecommerce_RemainingAmountForFreeShipping : CMSAbstractWebPart
{
    #region "Public properties"

    /// <summary>
    /// Text for remaining value.
    /// </summary>
    public string LabelText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("LabelText"), "");
        }
        set
        {
            SetValue("LabelText", value);
        }
    }

    #endregion


    #region "Lifecycle"

    /// <summary>
    /// OnPreRender override
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (!StopProcessing)
        {
            SetText();

            if (!lblText.Visible)
            {
                ContentBefore = string.Empty;
                ContentAfter = string.Empty;
                ContainerTitle = string.Empty;
                ContainerName = string.Empty;
            }
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Set text of remaining amount for free shipping
    /// </summary>
    private void SetText()
    {
        ShoppingCartInfo cart = ECommerceContext.CurrentShoppingCart;
        CurrencyInfo currency = cart.Currency;

        // Hide text by default
        lblText.Visible = false;

        if ((cart != null) && !cart.IsEmpty)
        {
            // Select all applicable shipping discounts regardless the Minimum order amount
            var discounts = cart.CartDiscountsFilter.Filter(cart.ShippingDiscounts, Double.MaxValue);

            // Find the lowest Minimum order amount
            double minOrderAmount = -1;
            foreach (var discount in discounts)
            {
                if ((minOrderAmount > discount.DiscountItemMinOrderAmount) || (minOrderAmount < 0))
                {
                    minOrderAmount = discount.DiscountItemMinOrderAmount;
                }
            }

            double total = cart.TotalItemsPriceInMainCurrency - cart.OrderDiscountInMainCurrency;

            // Round using shopping cart currency
            total = CurrencyInfoProvider.RoundTo(total, currency);
            minOrderAmount = CurrencyInfoProvider.RoundTo(minOrderAmount, currency);

            // Check if the Free shipping hasn't already applied
            if (minOrderAmount > total)
            {
                // Get remaining amount in shopping cart currency
                double remainingAmount = cart.ApplyExchangeRate(minOrderAmount - total);

                try
                {
                    // Display text of remaining amount
                    lblText.Visible = true;

                    var sb = new StringBuilder(LabelText.Length + 8);
                    sb.AppendFormat(LabelText, currency.FormatPrice(remainingAmount));
                    lblText.Text = sb.ToString();
                }
                catch (Exception ex)
                {
                    // Log exception
                    EventLogProvider.LogException("Web part", "EXCEPTION", ex, cart.ShoppingCartSiteID);
                }
            }
        }
    }

    #endregion
}