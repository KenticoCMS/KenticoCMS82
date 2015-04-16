using System;

using CMS.Ecommerce;
using CMS.Helpers;
using CMS.UIControls;

[Title("ProductPriceDetail.Title")]
public partial class CMSModules_Ecommerce_CMSPages_ShoppingCartSKUPriceDetail : CMSLiveModalPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Initialize product price detail
        ucSKUPriceDetail.CartItemGuid = QueryHelper.GetGuid("itemguid", Guid.Empty);
        ucSKUPriceDetail.IncludeOptions = QueryHelper.GetBoolean("includeoptions", false);
        ucSKUPriceDetail.ShoppingCart = ECommerceContext.CurrentShoppingCart;

        btnClose.Text = GetString("General.Close");
        btnClose.OnClientClick = "Close(); return false;";
    }
}