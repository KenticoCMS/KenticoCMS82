using System;
using System.Data;

using CMS.Core;
using CMS.Ecommerce;
using CMS.Helpers;
using CMS.UIControls;

[EditedObject(OrderInfo.OBJECT_TYPE, "orderid")]
[UIElement(ModuleName.ECOMMERCE, "Orders.Billing")]
public partial class CMSModules_Ecommerce_Pages_Tools_Orders_Order_Edit_Billing : CMSEcommercePage
{
    #region "Variables"

    private bool showMembershipWarning;
    private bool showEproductWarning;
    private bool recalculateOrder;

    #endregion


    #region "Properties"

    /// <summary>
    /// Editing order object
    /// </summary>
    private OrderInfo Order
    {
        get
        {
            return EditedObject as OrderInfo;
        }
    }


    /// <summary>
    /// Represents value which user selected during editing. Property does not represents value stored in database.
    /// </summary>
    private bool OrderIsPaid
    {
        get
        {
            return ValidationHelper.GetBoolean(editOrderBilling.FieldControls["OrderIsPaid"].Value, false);
        }
    }

    #endregion


    #region "Life cycle"

    /// <summary>
    /// Registers custom event handlers.
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        // Redirect to object does not exist page.
        if (Order == null)
        {
            EditedObject = null;
        }

        base.OnInit(e);

        editOrderBilling.OnCheckPermissions += editOrderBilling_OnCheckPermissions;
        editOrderBilling.OnBeforeSave += editOrderBilling_OnBeforeSave;
        editOrderBilling.OnAfterSave += editOrderBilling_OnAfterSave;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Check if order is not edited from another site
        CheckEditedObjectSiteID(Order.OrderSiteID);

        // Hide Select and Clear button which are visible by default for UniSelector
        UniSelector billingAddressSelector = editOrderBilling.FieldControls["OrderBillingAddressID"] as UniSelector;

        if (billingAddressSelector != null)
        {
            billingAddressSelector.ButtonSelect.Visible = false;
            billingAddressSelector.ButtonClear.Visible = false;
        }
    }


    /// <summary>
    /// Generates confirmation message.
    /// </summary>
    /// <param name="e"></param>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (!OrderIsPaid && Order.OrderIsPaid)
        {
            string confirmationMessage = GetConfirmationMessage();
            if (!string.IsNullOrEmpty(confirmationMessage))
            {
                editOrderBilling.SubmitButton.OnClientClick = "return confirm('" + confirmationMessage + "')";
            }
        }
    }

    #endregion


    #region "Page events"

    /// <summary>
    /// Checks modify permission.
    /// </summary>
    protected void editOrderBilling_OnCheckPermissions(object sender, EventArgs e)
    {
        // Check 'EcommerceModify' permission
        if (!ECommerceContext.IsUserAuthorizedForPermission("ModifyOrders"))
        {
            RedirectToAccessDenied("CMS.Ecommerce", "EcommerceModify OR ModifyOrders");
        }
    }


    /// <summary>
    /// Saving billing requires recalculation of order. Save action is executed by this custom code.
    /// </summary>
    protected void editOrderBilling_OnBeforeSave(object sender, EventArgs e)
    {
        int origPaymentID = 0;
        int origCurrencyID = 0;

        // Load original data    
        var data = OrderInfoProvider.GetOrders(Order.OrderSiteID).Columns("OrderPaymentOptionID, OrderCurrencyID").WhereEquals("OrderID", Order.OrderID).Result;
        if (!DataHelper.DataSourceIsEmpty(data))
        {
            origPaymentID = ValidationHelper.GetInteger(data.Tables[0].Rows[0][0], 0);
            origCurrencyID = ValidationHelper.GetInteger(data.Tables[0].Rows[0][1], 0);
        }
        // Update data only if shopping cart data were changed
        int paymentID = ValidationHelper.GetInteger(editOrderBilling.FieldEditingControls["OrderPaymentOptionID"].DataValue, 0);
        int currencyID = ValidationHelper.GetInteger(editOrderBilling.FieldEditingControls["OrderCurrencyID"].DataValue, 0);

        // Check if recalculate order is needed
        recalculateOrder = (origPaymentID != paymentID) || (origCurrencyID != currencyID);
    }


    void editOrderBilling_OnAfterSave(object sender, EventArgs e)
    {
        if (recalculateOrder)
        {
            ShoppingCartInfo sci = ShoppingCartInfoProvider.GetShoppingCartInfoFromOrder(Order.OrderID);
            // Evaluate order data
            ShoppingCartInfoProvider.EvaluateShoppingCart(sci);
            // Update order data
            ShoppingCartInfoProvider.SetOrder(sci, true);
        }
    }

    #endregion


    #region "Helper methods"

    /// <summary>
    /// Checks if order contains e-product or membership product. If so generates a confirmation message.
    /// </summary>
    private string GetConfirmationMessage()
    {
        // Get order items for this order
        DataSet orderItems = OrderItemInfoProvider.GetOrderItems(Order.OrderID);

        foreach (DataRow orderItemRow in orderItems.Tables[0].Rows)
        {
            // Get order item
            var item = new OrderItemInfo(orderItemRow);

            if (item.OrderItemSKU != null)
            {
                switch (item.OrderItemSKU.SKUProductType)
                {
                    // If order item represents membership
                    case SKUProductTypeEnum.Membership:
                        showMembershipWarning = true;
                        break;

                    // If order item represents e-product
                    case SKUProductTypeEnum.EProduct:
                        showEproductWarning = true;
                        break;
                }
            }
        }

        // If one of the rollback warnings should be shown
        if (showEproductWarning || showMembershipWarning)
        {
            // Set standard warning message
            string paidUncheckWarning = GetString("order_edit_billing.orderispaiduncheckwarning");

            // Add memberships rollback warning message if required
            if (showMembershipWarning)
            {
                paidUncheckWarning += "\\n\\n- " + GetString("order_edit_billing.orderispaiduncheckwarningmemberships");
            }

            // Add e-products rollback warning message if required
            if (showEproductWarning)
            {
                paidUncheckWarning += "\\n\\n- " + GetString("order_edit_billing.orderispaiduncheckwarningeproducts");
            }

            return paidUncheckWarning;
        }

        return "";
    }

    #endregion
}