using System;
using System.Linq;

using CMS;
using CMS.Ecommerce;
using CMS.ExtendedControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;

[assembly: RegisterCustomClass("CouponCodesListPageExtender", typeof(CouponCodesListPageExtender))]

/// <summary>
/// Discount coupon list page extender
/// </summary>
public class CouponCodesListPageExtender : PageExtender<CMSPage>
{
    public override void OnInit()
    {
        Page.Load += Page_Load;
    }


    private void Page_Load(object sender, EventArgs e)
    {
        // Find parent discount object
        if (Page.EditedObjectParent == null)
        {
            return;
        }

        var discount = new Discount(Page.EditedObjectParent);

        // Check if user is allowed to read discount
        if (!DiscountInfoProvider.IsUserAuthorizedToReadDiscount(discount.DiscountSiteID, MembershipContext.AuthenticatedUser))
        {
            Page.EditedObjectParent = null;
        }

        var url = URLHelper.ResolveUrl("~/CMSModules/Ecommerce/Pages/Tools/Discount/Discount_Codes_Generator.aspx");
        url = URLHelper.AddParameterToUrl(url, "discountId", discount.DiscountID.ToString());
        // Inform generator about discount type in case of multi buy discount
        if ((MultiBuyDiscountInfo)discount != null)
        {
            url = URLHelper.AddParameterToUrl(url, "isMultiBuy", "1");
        }

        // Add action for coupon codes generation
        Page.AddHeaderAction(new HeaderAction
        {
            Text = ResHelper.GetString("com.discount.generatecoupons"),
            RedirectUrl = url,
            Index = 1,
            Enabled = discount.DiscountUsesCoupons && DiscountInfoProvider.IsUserAuthorizedToModifyDiscount(SiteContext.CurrentSiteName, MembershipContext.AuthenticatedUser),
            ButtonStyle = ButtonStyle.Default
        });
    }
}
