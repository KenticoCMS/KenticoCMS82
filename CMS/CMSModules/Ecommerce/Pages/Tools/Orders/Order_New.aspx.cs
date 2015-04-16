using System;

using CMS.Core;
using CMS.Ecommerce;
using CMS.EcommerceProvider;
using CMS.Helpers;
using CMS.Base;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;

[Breadcrumb(1, "Order_New.HeaderCaption", "", "")]
[UIElement(ModuleName.ECOMMERCE, "Orders", false, true)]
public partial class CMSModules_Ecommerce_Pages_Tools_Orders_Order_New : CMSEcommercePage
{
    private int customerId = 0;


    /// <summary>
    /// Shopping cart to use.
    /// </summary>
    private ShoppingCartInfo ShoppingCart
    {
        get
        {
            ShoppingCartInfo sci = SessionHelper.GetValue(SessionKey) as ShoppingCartInfo;
            if (sci == null)
            {
                sci = GetNewCart();
                SessionHelper.SetValue(SessionKey, sci);
            }

            return sci;
        }
        set
        {
            SessionHelper.SetValue(SessionKey, value);
        }
    }


    /// <summary>
    /// Shopping cart session key.
    /// </summary>
    private string SessionKey
    {
        get
        {
            return (customerId > 0) ? "CMSDeskNewCustomerOrderShoppingCart" : "CMSDeskNewOrderShoppingCart";
        }
    }


    protected ShoppingCartInfo GetNewCart()
    {
        ShoppingCartInfo newCart = ShoppingCartInfoProvider.CreateShoppingCartInfo(SiteContext.CurrentSite.SiteID);
        if (customerId > 0)
        {
            CustomerInfo ci = CustomerInfoProvider.GetCustomerInfo(customerId);
            if (ci != null)
            {
                if (ci.CustomerIsRegistered)
                {
                    newCart.User = UserInfoProvider.GetUserInfo(ci.CustomerUserID);
                }

                newCart.ShoppingCartCustomerID = customerId;
            }
        }

        return newCart;
    }


    protected override void OnPreInit(EventArgs e)
    {
        customerId = QueryHelper.GetInteger("customerid", 0);

        if (customerId > 0)
        {
            if(!ECommerceContext.CheckCustomerSiteID(CustomerInfoProvider.GetCustomerInfo(customerId)))
            {
                customerId = 0;
            }
        }
        base.OnPreInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!RequestHelper.IsPostBack())
        {
            ShoppingCart = GetNewCart();
        }

        Cart.LocalShoppingCart = ShoppingCart;
        Cart.EnableProductPriceDetail = true;
        Cart.OnPaymentCompleted += (s, ea) => GoToOrderDetail();
        Cart.OnPaymentSkipped += (s, ea) => GoToOrderDetail();
        Cart.OnCheckPermissions += Cart_OnCheckPermissions;
        Cart.RequiredFieldsMark = CMS.EcommerceProvider.ShoppingCart.DEFAULT_REQUIRED_FIELDS_MARK;

        Cart.CheckoutProcessType = (customerId > 0) ? CheckoutProcessEnum.CMSDeskCustomer : CheckoutProcessEnum.CMSDeskOrder;
    }


    private void Cart_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        // Check ecommerce permission
        if (!ECommerceContext.IsUserAuthorizedForPermission(permissionType))
        {
            string message = permissionType;
            if (permissionType.ToLowerCSafe().StartsWithCSafe("modify"))
            {
                message = "EcommerceModify OR " + message;
            }

            RedirectToAccessDenied("CMS.Ecommerce", message);
        }
    }


    private void GoToOrderDetail()
    {
        URLHelper.Redirect(UIContextHelper.GetElementUrl("CMS.Ecommerce", "OrderProperties", false, ShoppingCart.OrderId) + "&customerid=" + customerId);
    }


    protected void Page_Prerender()
    {
        // Create breadcrumbs
        CreateBreadcrumbs();
    }


    /// <summary>
    /// Creates breadcrumbs
    /// </summary>
    private void CreateBreadcrumbs()
    {
        string url = (customerId > 0) ? "~/CMSModules/Ecommerce/Pages/Tools/Orders/Order_List.aspx?customerId=" + customerId : "~/CMSModules/Ecommerce/Pages/Tools/Orders/Order_List.aspx";

        // Set breadcrumb
        PageBreadcrumbs.AddBreadcrumb(new BreadcrumbItem()
        {
            Index = 0,
            Text = GetString("Order_New.Orders"),
            RedirectUrl = ResolveUrl(url),
        });

        if (customerId <= 0)
        {
            PageTitle.TitleText = GetString("Order_New.HeaderCaption");
        }

        UIHelper.SetBreadcrumbsSuffix("");
    }
}