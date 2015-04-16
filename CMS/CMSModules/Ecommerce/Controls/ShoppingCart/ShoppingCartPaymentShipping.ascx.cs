using System;

using CMS.Ecommerce;
using CMS.EcommerceProvider;
using CMS.Globalization;
using CMS.SiteProvider;

public partial class CMSModules_Ecommerce_Controls_ShoppingCart_ShoppingCartPaymentShipping : ShoppingCartStep
{
    #region "ViewState Constants"

    private const string SHIPPING_OPTION_ID = "OrderShippingOptionID";
    private const string PAYMENT_OPTION_ID = "OrderPaymentOptionID";

    #endregion


    #region "Variables"

    private bool? mIsShippingNeeded = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Returns true if shopping cart items need shipping.
    /// </summary>
    protected bool IsShippingNeeded
    {
        get
        {
            if (mIsShippingNeeded.HasValue)
            {
                return mIsShippingNeeded.Value;
            }

            if (ShoppingCart != null)
            {
                // Figure out from shopping cart
                mIsShippingNeeded = ShoppingCart.IsShippingNeeded;
                return mIsShippingNeeded.Value;
            }

            return true;
        }
    }

    #endregion


    #region "Page methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        selectShipping.IsLiveSite = IsLiveSite;
        selectPayment.IsLiveSite = IsLiveSite;

        if ((ShoppingCart != null) && (SiteContext.CurrentSite != null))
        {
            if (ShoppingCart.CountryID == 0)
            {
                string countryName = ECommerceSettings.DefaultCountryName(SiteContext.CurrentSite.SiteName);
                CountryInfo ci = CountryInfoProvider.GetCountryInfo(countryName);
                ShoppingCart.CountryID = (ci != null) ? ci.CountryID : 0;
            }

            selectShipping.ShoppingCart = ShoppingCart;
        }

        if (IsShippingNeeded)
        {
            if (!ShoppingCartControl.IsCurrentStepPostBack)
            {
                SelectShippingOption();
            }
        }
        else
        {
            // Don't use shipping selection
            selectShipping.StopProcessing = true;
            // Hide title
            headTitle.Visible = false;
            // Change current checkout process step caption
            ShoppingCartControl.CheckoutProcessSteps[ShoppingCartControl.CurrentStepIndex].Caption = GetString("order_new.paymentshipping.titlenoshipping");
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (selectShipping.HasData)
        {
            // Show shipping selection
            plcShipping.Visible = true;

            // Initialize payment options for selected shipping option
            selectPayment.ShippingOptionID = selectShipping.SelectedID;
            selectPayment.DisplayOnlyAllowedIfNoShipping = false;
        }
        else
        {
            selectPayment.DisplayOnlyAllowedIfNoShipping = true;
        }

        selectPayment.Reload();

        SelectPaymentOption();

        plcPayment.Visible = selectPayment.HasData;
    }

    #endregion


    /// <summary>
    /// Back button actions.
    /// </summary>
    public override void ButtonBackClickAction()
    {
        // Save the values to ShoppingCart ViewState
        ShoppingCartControl.SetTempValue(SHIPPING_OPTION_ID, selectShipping.SelectedID);
        ShoppingCartControl.SetTempValue(PAYMENT_OPTION_ID, selectPayment.SelectedID);

        base.ButtonBackClickAction();
    }


    public override bool ProcessStep()
    {
        try
        {
            // Cleanup the ShoppingCart ViewState
            ShoppingCartControl.SetTempValue(SHIPPING_OPTION_ID, null);
            ShoppingCartControl.SetTempValue(PAYMENT_OPTION_ID, null);

            ShoppingCart.ShoppingCartShippingOptionID = selectShipping.SelectedID;
            ShoppingCart.ShoppingCartPaymentOptionID = selectPayment.SelectedID;

            // Update changes in database only when on the live site
            if (!ShoppingCartControl.IsInternalOrder)
            {
                ShoppingCartInfoProvider.SetShoppingCartInfo(ShoppingCart);
            }
            return true;
        }
        catch (Exception ex)
        {
            lblError.Visible = true;
            lblError.Text = ex.Message;
            return false;
        }
    }


    /// <summary>
    /// Preselects payment option.
    /// </summary>
    protected void SelectPaymentOption()
    {
        try
        {
            // Try to select payment from ViewState first
            object viewStateValue = ShoppingCartControl.GetTempValue(PAYMENT_OPTION_ID);
            if (viewStateValue != null)
            {
                selectPayment.SelectedID = Convert.ToInt32(viewStateValue);
            }
            // Try to select payment option according to saved option in shopping cart object
            else if (ShoppingCart.ShoppingCartPaymentOptionID > 0)
            {
                selectPayment.SelectedID = ShoppingCart.ShoppingCartPaymentOptionID;
            }
            // Try to select payment option according to user preferred option
            else
            {
                CustomerInfo customer = ShoppingCart.Customer;
                int paymentOptionId = (customer.CustomerUser != null) ? customer.CustomerUser.GetUserPreferredPaymentOptionID(SiteContext.CurrentSiteName) : 0;
                if (paymentOptionId > 0)
                {
                    selectPayment.SelectedID = paymentOptionId;
                }
            }
        }
        catch
        {
        }
    }


    /// <summary>
    /// Preselects shipping option.
    /// </summary>
    protected void SelectShippingOption()
    {
        try
        {
            // Try to select shipping from ViewState first
            object viewStateValue = ShoppingCartControl.GetTempValue(SHIPPING_OPTION_ID);
            if (viewStateValue != null)
            {
                selectShipping.SelectedID = Convert.ToInt32(viewStateValue);
            }
            // Try to select shipping option according to saved option in shopping cart object
            else if (ShoppingCart.ShoppingCartShippingOptionID > 0)
            {
                selectShipping.SelectedID = ShoppingCart.ShoppingCartShippingOptionID;
            }
            // Try to select shipping option according to user preferred option
            else
            {
                CustomerInfo customer = ShoppingCart.Customer;
                int shippingOptionId = (customer.CustomerUser != null) ? customer.CustomerUser.GetUserPreferredShippingOptionID(SiteContext.CurrentSiteName) : 0;
                if (shippingOptionId > 0)
                {
                    selectShipping.SelectedID = shippingOptionId;
                }
            }
        }
        catch
        {
        }
    }


    public override bool IsValid()
    {
        string errorMessage = "";

        // If shipping is required
        if (plcShipping.Visible)
        {
            if (selectShipping.SelectedID <= 0)
            {
                errorMessage = GetString("Order_New.NoShippingOption");
            }
        }

        // If payment is required
        if (plcPayment.Visible)
        {
            if ((errorMessage == "") && (selectPayment.SelectedID <= 0))
            {
                errorMessage = GetString("Order_New.NoPaymentMethod");
            }
        }

        if (errorMessage == "")
        {
            // Form is valid
            return true;
        }

        // Form is not valid
        lblError.Visible = true;
        lblError.Text = errorMessage;
        return false;
    }


    protected void selectPayment_Changed(object sender, EventArgs e)
    {
        ShoppingCartControl.SetTempValue(PAYMENT_OPTION_ID, selectPayment.SelectedID);
    }
}