using System;
using System.Linq;

using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.Base;


/// <summary>
/// Discount coupon Web part
/// </summary>
public partial class CMSWebParts_Ecommerce_Checkout_InputFields_DiscountCoupon : CMSCheckoutWebPart
{
    #region "Public Properties"

    /// <summary>
    /// Gets or sets a value indicating whether [show update button].
    /// </summary>   
    public bool ShowUpdateButton
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowUpdateButton"), false);
        }
        set
        {
            SetValue("ShowUpdateButton", value);
        }
    }

    #endregion


    #region "Private Properties"

    private string CouponCode
    {
        get
        {
            return ValidationHelper.GetString(txtCouponField.Text, "").Trim();
        }
    }

    #endregion


    #region "Life cycle"

    /// <summary>
    /// Load event handler.
    /// </summary>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        SetupControl();
    }


    /// <summary>
    /// PreRender event handler.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // If there is already a discount applied, display it
        if (ShoppingCart.HasUsableCoupon)
        {
            // Fill textbox with discount coupon code            
            txtCouponField.Text = ShoppingCart.ShoppingCartCouponCode;
        }
    }

    #endregion


    #region "Wizard methods"

    protected override void ValidateStepData(object sender, StepEventArgs e)
    {
        base.ValidateStepData(sender, e);

        bool valid = ApplyDiscountCoupon();

        if (!valid)
        {
            pnlError.Visible = true;
            e.CancelEvent = true;
        }
        else
        {
            pnlError.Visible = false;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>    
    /// Checks whether entered discount coupon code is usable for this cart and applies it. Returns true if so.
    /// </summary>
    private bool ApplyDiscountCoupon()
    {
        string coupon = CouponCode;
        lblError.Text = string.Empty;

        if (!string.IsNullOrEmpty(coupon))
        {
            ShoppingCart.ShoppingCartCouponCode = coupon;

            if (!ShoppingCart.HasUsableCoupon)
            {
                // Clean coupon if it doesn't fit
                ShoppingCart.ShoppingCartCouponCode = string.Empty;
                // Discount coupon is not valid                
                lblError.Text = GetString("ecommerce.error.couponcodeisnotvalid");
                return false;
            }
        }
        else
        {
            ShoppingCart.ShoppingCartCouponCode = string.Empty;
        }

        return true;
    }


    /// <summary>
    /// Setups the control.
    /// </summary>
    public void SetupControl()
    {
        if (!StopProcessing)
        {
            // Update button visibility
            if (ShowUpdateButton)
            {
                btnUpdate.Visible = true;
                btnUpdate.Text = GetString("checkout.couponbutton");
            }
        }
    }

    #endregion


    #region "Event handling"

    protected void btnUpdate_Click(object sender, EventArgs e)
    {
        pnlError.Visible = !ApplyDiscountCoupon();
        ComponentEvents.RequestEvents.RaiseEvent(sender, e, SHOPPING_CART_CHANGED);
    }

    #endregion
}