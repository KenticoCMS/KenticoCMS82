using System;

using CMS.Core;
using CMS.Ecommerce;
using CMS.Helpers;
using CMS.UIControls;

[EditedObject(OrderInfo.OBJECT_TYPE, "orderId")]
[UIElement(ModuleName.ECOMMERCE, "Orders.Shipping")]
public partial class CMSModules_Ecommerce_Pages_Tools_Orders_Order_Edit_Shipping : CMSEcommercePage
{
    private ShoppingCartInfo mShoppingCartFromOrder;


    #region "Properties"

    /// <summary>
    /// Shopping cart created from edited order.
    /// </summary>
    private ShoppingCartInfo ShoppingCartFromOrder
    {
        get
        {
            return mShoppingCartFromOrder ?? (mShoppingCartFromOrder = ShoppingCartInfoProvider.GetShoppingCartInfoFromOrder(Order.OrderID));
        }
    }


    /// <summary>
    /// Editing order object
    /// </summary>
    private OrderInfo Order
    {
        get
        {
            return orderShippingForm.EditedObject as OrderInfo;
        }
    }


    private bool IsTaxBasedOnShippingAddress
    {
        get
        {
            return (ECommerceSettings.ApplyTaxesBasedOn(Order.OrderSiteID) == ApplyTaxBasedOnEnum.ShippingAddress);
        }
    }


    private BaseObjectSelector ShippingOptionSelector
    {
        get
        {
            return orderShippingForm.FieldControls["OrderShippingOptionID"] as BaseObjectSelector;
        }
    }


    private UniSelector ShippingAddressSelector
    {
        get
        {
            return orderShippingForm.FieldControls["OrderShippingAddressID"] as UniSelector;
        }
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        orderShippingForm.OnCheckPermissions += CheckPermissions;
        orderShippingForm.OnBeforeSave += OnBeforeSave;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Check if order is not edited from another site
        CheckEditedObjectSiteID(Order.OrderSiteID);

        // Hide Select and Clear button which are visible by default for UniSelector
        if (ShippingAddressSelector != null)
        {
            ShippingAddressSelector.ButtonSelect.Visible = false;
            ShippingAddressSelector.ButtonClear.Visible = false;
        }

        if (ShippingOptionSelector != null)
        {
            ShippingOptionSelector.SetValue("ShoppingCart", ShoppingCartFromOrder);
        }
    }

    #endregion


    #region "Event handlers"

    protected void CheckPermissions(object sender, EventArgs e)
    {
        if (!ECommerceContext.IsUserAuthorizedForPermission("ModifyOrders"))
        {
            RedirectToAccessDenied("CMS.Ecommerce", "EcommerceModify OR ModifyOrders");
        }
    }


    protected void OnBeforeSave(object sender, EventArgs e)
    {
        if ((Order == null) || (ShippingAddressSelector == null) || (ShippingOptionSelector == null))
        {
            return;
        }

        // Get current values
        int addressID = ValidationHelper.GetInteger(ShippingAddressSelector.Value, 0);
        int shippingOptionID = ValidationHelper.GetInteger(ShippingOptionSelector.Value, 0);

        // Is shipping needed?
        bool isShippingNeeded = ((ShoppingCartFromOrder != null) && ShoppingCartFromOrder.IsShippingNeeded);

        // If shipping address is required
        if (isShippingNeeded || IsTaxBasedOnShippingAddress)
        {
            // If shipping address is not set
            if (addressID <= 0)
            {
                // Show error message
                ShowError(GetString("Order_Edit_Shipping.NoAddress"));
                return;
            }
        }

        try
        {
            // Shipping option changed 
            if ((ShoppingCartFromOrder != null) && (Order.OrderShippingOptionID != shippingOptionID))
            {
                // Shipping option and payment method combination is not allowed 
                if (PaymentShippingInfoProvider.GetPaymentShippingInfo(Order.OrderPaymentOptionID, shippingOptionID) == null)
                {
                    PaymentOptionInfo payment = PaymentOptionInfoProvider.GetPaymentOptionInfo(Order.OrderPaymentOptionID);

                    // Check if payment is allowed with no shipping
                    if ((payment != null) && !(payment.PaymentOptionAllowIfNoShipping && shippingOptionID == 0))
                    {
                        // Set payment method to none and display warning
                        ShoppingCartFromOrder.ShoppingCartPaymentOptionID = 0;

                        string paymentMethodName = ResHelper.LocalizeString(payment.PaymentOptionDisplayName, null, true);
                        string shippingOptionName = HTMLHelper.HTMLEncode(ShippingOptionSelector.ValueDisplayName);

                        ShowWarning(String.Format(ResHelper.GetString("com.shippingoption.paymentsetnone"), paymentMethodName, shippingOptionName));
                    }

                }

                // Set order new properties
                ShoppingCartFromOrder.ShoppingCartShippingOptionID = shippingOptionID;

                // Evaluate order data
                ShoppingCartInfoProvider.EvaluateShoppingCart(ShoppingCartFromOrder);

                // Update order data
                ShoppingCartInfoProvider.SetOrder(ShoppingCartFromOrder, true);
            }

            // Update tracking number
            Order.OrderTrackingNumber = ValidationHelper.GetString(orderShippingForm.FieldEditingControls["OrderTrackingNumber"].DataValue, String.Empty).Trim();
            OrderInfoProvider.SetOrderInfo(Order);

            // Show message
            ShowChangesSaved();

            // Stop automatic saving action
            orderShippingForm.StopProcessing = true;
        }
        catch (Exception ex)
        {
            // Show error message
            ShowError(ex.Message);
        }
    }

    #endregion
}