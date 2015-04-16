using System;

using CMS.Ecommerce;
using CMS.Helpers;
using CMS.UIControls;

public partial class CMSModules_Ecommerce_Controls_ProductOptions_DonationProperties : CMSUserControl
{
    #region "Variables"

    private ShoppingCartInfo mShoppingCart;

    private bool mShowDonationAmount = true;
    private bool mShowCurrencyCode = true;
    private bool mShowDonationUnits = true;
    private bool mShowDonationIsPrivate = true;

    #endregion


    #region "Properties"

    /// <summary>
    /// Shopping cart data used for price calculation and formatting.
    /// </summary>
    public ShoppingCartInfo ShoppingCart
    {
        get
        {
            return mShoppingCart ?? (mShoppingCart = ECommerceContext.CurrentShoppingCart);
        }
        set
        {
            mShoppingCart = value;
        }
    }


    /// <summary>
    /// Donation SKU data.
    /// </summary>
    public SKUInfo SKU
    {
        get;
        set;
    }


    /// <summary>
    /// Donation amount in shopping cart currency.
    /// </summary>
    public double DonationAmount
    {
        get
        {
            return amountPriceSelector.Price;
        }
        set
        {
            amountPriceSelector.Price = value;
            DonationAmountInitialized = true;
        }
    }


    /// <summary>
    /// Indicates if donation amount value was initialized already.
    /// </summary>
    public bool DonationAmountInitialized
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["DonationAmountInitialized"], false);
        }
        protected set
        {
            ViewState["DonationAmountInitialized"] = value;
        }
    }


    /// <summary>
    /// Indicates if donation is private.
    /// </summary>
    public bool DonationIsPrivate
    {
        get
        {
            return chkIsPrivate.Checked;
        }
        set
        {
            chkIsPrivate.Checked = value;
        }
    }


    /// <summary>
    /// Donation units.
    /// </summary>
    public int DonationUnits
    {
        get
        {
            return ValidationHelper.GetInteger(txtUnits.Text, 1);
        }
        set
        {
            txtUnits.Text = ValidationHelper.GetString(value, "1");
        }
    }


    /// <summary>
    /// Validation error message.
    /// </summary>
    public string ErrorMessage
    {
        get;
        set;
    }

    #endregion


    #region "Properties - Layout"

    /// <summary>
    /// Indicates if donation amount input field is displayed.
    /// </summary>
    public bool ShowDonationAmount
    {
        get
        {
            return mShowDonationAmount;
        }
        set
        {
            mShowDonationAmount = value;
            plcAmount.Visible = value;
        }
    }


    /// <summary>
    /// Indicates if currency code is displayed next to donation amount input field.
    /// </summary>
    public bool ShowCurrencyCode
    {
        get
        {
            return mShowCurrencyCode;
        }
        set
        {
            mShowCurrencyCode = value;
            amountPriceSelector.ShowCurrencyCode = value;
        }
    }


    /// <summary>
    /// Indicates if donation units input field is displayed.
    /// </summary>
    public bool ShowDonationUnits
    {
        get
        {
            return mShowDonationUnits;
        }
        set
        {
            mShowDonationUnits = value;
            plcUnits.Visible = value;
        }
    }


    /// <summary>
    /// Indicates if checkbox for private donation is displayed.
    /// </summary>
    public bool ShowDonationIsPrivate
    {
        get
        {
            return mShowDonationIsPrivate;
        }
        set
        {
            mShowDonationIsPrivate = value;
            plcIsPrivate.Visible = value;
        }
    }


    /// <summary>
    /// Indicates if at least one of donation properties editable fields is visible.
    /// </summary>
    public bool HasEditableFieldsVisible
    {
        get
        {
            return (plcAmount.Visible || plcUnits.Visible || plcIsPrivate.Visible);
        }
    }

    #endregion


    #region "Page methods"

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        
        if (!StopProcessing)
        {
            ReloadData();
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Reloads donation properties data.
    /// </summary>
    public void ReloadData()
    {
        // If SKU is donation
        if ((SKU != null) && (SKU.SKUProductType == SKUProductTypeEnum.Donation))
        {
            amountPriceSelector.Currency = ShoppingCart.Currency;
            amountPriceSelector.ValidationErrorMessage = GetString("com.donationproperties.amountinvalid");
            amountPriceSelector.EmptyErrorMessage = GetString("com.donationproperties.amountinvalid");

            rvUnits.ErrorMessage = GetString("com.donationproperties.unitsinvalid");
            if (!DonationAmountInitialized && (DonationAmount == 0))
            {
                // Set donation default amount in cart currency                
                DonationAmount = ShoppingCart.RoundTo(ShoppingCart.ApplyExchangeRate(SKU.SKUPrice, SKU.IsGlobal));
            }

            ShowDonationAmount &= !(((SKU.SKUMinPrice == SKU.SKUPrice) && (SKU.SKUMaxPrice == SKU.SKUPrice)) && (SKU.SKUPrice != 0.0));
            ShowDonationIsPrivate &= SKU.SKUPrivateDonation;
        }
    }


    /// <summary>
    /// Validates form and returns error message in case form is not valid.
    /// </summary>
    public string Validate()
    {
        lblPriceRangeError.Visible = false;

        // Validate donation amount
        if (String.IsNullOrEmpty(ErrorMessage))
        {
            ErrorMessage = amountPriceSelector.Validate(false);
            rfvPriceSelector.Text = ErrorMessage;
        }

        // Validate donation amount range
        if (String.IsNullOrEmpty(ErrorMessage) && (SKU != null))
        {
            // Get min/max prices in cart currency
            double minPrice = ShoppingCart.ApplyExchangeRate(SKU.SKUMinPrice, SKU.IsGlobal);
            double maxPrice = ShoppingCart.ApplyExchangeRate(SKU.SKUMaxPrice, SKU.IsGlobal);

            if ((minPrice > 0) && (amountPriceSelector.Price < minPrice) || ((maxPrice > 0) && (amountPriceSelector.Price > maxPrice)))
            {
                // Get formatted min/max prices
                string fMinPrice = ShoppingCart.GetFormattedPrice(minPrice);
                string fMaxPrice = ShoppingCart.GetFormattedPrice(maxPrice);

                // Set appropriate error message
                if ((minPrice > 0.0) && (maxPrice > 0.0))
                {
                    ErrorMessage = String.Format(GetString("com.donationproperties.amountrange"), fMinPrice, fMaxPrice);
                }
                else if (minPrice > 0.0)
                {
                    ErrorMessage = String.Format(GetString("com.donationproperties.amountrangemin"), fMinPrice);
                }
                else if (maxPrice > 0.0)
                {
                    ErrorMessage = String.Format(GetString("com.donationproperties.amountrangemax"), fMaxPrice);
                }
            }
        }
        
        lblPriceRangeError.Text = ErrorMessage;
        lblPriceRangeError.Visible = !string.IsNullOrEmpty(ErrorMessage);

        // Validate donation units
        if (String.IsNullOrEmpty(ErrorMessage))
        {
            rvUnits.Validate();
            if (!rvUnits.IsValid)
            {
                ErrorMessage = rvUnits.ErrorMessage;
            }
        }

        return ErrorMessage;
    }

    #endregion
}