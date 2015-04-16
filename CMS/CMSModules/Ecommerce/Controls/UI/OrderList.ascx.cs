using System;
using System.Data;

using CMS.Ecommerce;
using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.Base;
using CMS.PortalEngine;
using CMS.UIControls;

public partial class CMSModules_Ecommerce_Controls_UI_OrderList : CMSAdminListControl
{
    #region "Variables"

    private int customerId = 0;

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        customerId = QueryHelper.GetInteger("customerId", 0);

        gridElem.IsLiveSite = IsLiveSite;
        gridElem.OnExternalDataBound += gridElem_OnExternalDataBound;
        gridElem.OnAction += gridElem_OnAction;
        gridElem.WhereCondition = GetWhereCondition();
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (customerId > 0)
        {
            gridElem.NamedColumns["Customer"].Visible = false;
        }

        gridElem.NamedColumns["OrderPrice"].Visible = ECommerceContext.MoreCurrenciesUsedOnSite;
    }

    #endregion


    #region "Event handlers"

    private object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        DataRowView dr = null;

        switch (sourceName.ToLowerCSafe())
        {
            case "idandinvoice":
                dr = (DataRowView)parameter;
                int orderId = ValidationHelper.GetInteger(dr["OrderID"], 0);
                string invoiceNumber = ValidationHelper.GetString(dr["OrderInvoiceNumber"], "");

                // Show OrderID and invoice number in brackets if InvoiceNumber is different from OrderID
                if (!string.IsNullOrEmpty(invoiceNumber) && (invoiceNumber != orderId.ToString()))
                {
                    return HTMLHelper.HTMLEncode(orderId + " (" + invoiceNumber + ")");
                }
                return orderId;

            case "totalpriceinmaincurrency":
                dr = (DataRowView)parameter;
                double totalPriceInMainCurrency = ValidationHelper.GetDouble(dr["OrderTotalPriceInMainCurrency"], 0);
                int siteId = ValidationHelper.GetInteger(dr["OrderSiteID"], 0);

                // Format currency
                var priceInMainCurrencyFormatted = CurrencyInfoProvider.GetFormattedPrice(totalPriceInMainCurrency, siteId);

                return HTMLHelper.HTMLEncode(priceInMainCurrencyFormatted);

            case "totalpriceinorderprice":
                dr = (DataRowView)parameter;
                int currencyId = ValidationHelper.GetInteger(dr["OrderCurrencyID"], 0);
                var currency = CurrencyInfoProvider.GetCurrencyInfo(currencyId);

                // If order is not in main currency, show order price
                if ((currency != null) && !currency.CurrencyIsMain)
                {
                    var orderTotalPrice = ValidationHelper.GetDouble(dr["OrderTotalPrice"], 0);
                    var priceFormatted = currency.FormatPrice(orderTotalPrice);

                    // Formatted currency
                    return HTMLHelper.HTMLEncode(priceFormatted);
                }
                return string.Empty;

            case "note":
                string note = ValidationHelper.GetString(parameter, "");

                // Display link, note is in tooltip
                if (!string.IsNullOrEmpty(note))
                {
                    return "<a>" + GetString("general.view") + "</a>";
                }
                return parameter;

            case "statusname":
                var statusId = ValidationHelper.GetInteger(parameter, 0);
                var status = OrderStatusInfoProvider.GetOrderStatusInfo(statusId);
                if (status != null)
                {
                    return new Tag()
                           {
                               Text = status.StatusDisplayName,
                               Color = status.StatusColor
                           };
                }

                return string.Empty;
        }
        return parameter;
    }


    /// <summary>
    /// Handles the grid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void gridElem_OnAction(string actionName, object actionArgument)
    {
        int orderId = ValidationHelper.GetInteger(actionArgument, 0);
        OrderInfo oi = null;
        OrderStatusInfo osi = null;

        switch (actionName.ToLowerCSafe())
        {
            case "edit":
                string redirectToUrl = UIContextHelper.GetElementUrl("CMS.Ecommerce", "OrderProperties", false, orderId);
                if (customerId > 0)
                {
                    redirectToUrl += "&customerId=" + customerId;
                }
                URLHelper.Redirect(redirectToUrl);
                break;


            case "delete":
                // Check 'ModifyOrders' and 'EcommerceModify' permission
                if (!ECommerceContext.IsUserAuthorizedForPermission("ModifyOrders"))
                {
                    AccessDenied("CMS.Ecommerce", "EcommerceModify OR ModifyOrders");
                }

                // delete OrderInfo object from database
                OrderInfoProvider.DeleteOrderInfo(orderId);
                break;

            case "previous":
                // Check 'ModifyOrders' and 'EcommerceModify' permission
                if (!ECommerceContext.IsUserAuthorizedForPermission("ModifyOrders"))
                {
                    AccessDenied("CMS.Ecommerce", "EcommerceModify OR ModifyOrders");
                }

                oi = OrderInfoProvider.GetOrderInfo(orderId);
                if (oi != null)
                {
                    osi = OrderStatusInfoProvider.GetPreviousEnabledStatus(oi.OrderStatusID);
                    if (osi != null)
                    {
                        oi.OrderStatusID = osi.StatusID;
                        // Save order status changes
                        OrderInfoProvider.SetOrderInfo(oi);
                    }
                }
                break;

            case "next":
                // Check 'ModifyOrders' and 'EcommerceModify' permission
                if (!ECommerceContext.IsUserAuthorizedForPermission("ModifyOrders"))
                {
                    AccessDenied("CMS.Ecommerce", "EcommerceModify OR ModifyOrders");
                }

                oi = OrderInfoProvider.GetOrderInfo(orderId);
                if (oi != null)
                {
                    osi = OrderStatusInfoProvider.GetNextEnabledStatus(oi.OrderStatusID);
                    if (osi != null)
                    {
                        oi.OrderStatusID = osi.StatusID;
                        // Save order status changes
                        OrderInfoProvider.SetOrderInfo(oi);
                    }
                }
                break;
        }
    }

    #endregion


    #region "Other methods"

    /// <summary>
    /// Creates where condition based on query string.
    /// </summary>
    private string GetWhereCondition()
    {
        string where = "";
        if (customerId > 0)
        {
            where = "OrderCustomerID = " + customerId;
        }

        return where;
    }

    #endregion
}