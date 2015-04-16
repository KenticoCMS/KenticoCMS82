using System;
using System.Linq;

using CMS;
using CMS.Ecommerce;
using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.Membership;
using CMS.SiteProvider;

[assembly: RegisterCustomClass("OrderAddressEditExtender", typeof(OrderAddressEditExtender))]

/// <summary>
/// Order address edit extender
/// </summary>
public class OrderAddressEditExtender : ControlExtender<UIForm>
{
    #region "Variables"

    private OrderInfo order;

    #endregion


    #region "Properties"

    /// <summary>
    /// Edited address.
    /// </summary>
    public OrderAddressInfo Address
    {
        get
        {
            return Control.EditedObject as OrderAddressInfo;
        }
    }


    /// <summary>
    /// Child order for order address.
    /// </summary>
    public OrderInfo Order
    {
        get
        {
            if (order == null)
            {
                if (Address == null)
                {
                    return null;
                }

                // Try to find edited address order (edited address may be shipping or billing or company address)
                order = OrderInfoProvider.GetOrders().WhereEquals("OrderBillingAddressID", Address.AddressID)
                                                     .Or()
                                                     .WhereEquals("OrderShippingAddressID", Address.AddressID)
                                                     .Or()
                                                     .WhereEquals("OrderCompanyAddressID", Address.AddressID).FirstOrDefault();
            }

            return order;
        }
    }

    #endregion


    #region "Page events"

    public override void OnInit()
    {
        Control.Page.Load += Page_Load;
    }


    private void Page_Load(object sender, EventArgs e)
    {
        // Check if user has permission to edit object on current site
        if ((!MembershipContext.AuthenticatedUser.IsGlobalAdministrator) && (Order != null) && (Order.OrderSiteID != SiteContext.CurrentSiteID))
        {
            Control.EditedObject = null;
        }
    }

    #endregion
}