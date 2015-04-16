using System;
using System.Data;

using CMS.Core;
using CMS.Ecommerce;
using CMS.Helpers;
using CMS.Membership;
using CMS.DataEngine;
using CMS.UIControls;

[UIElement(ModuleName.ECOMMERCE, "DiscountCoupons.Products")]
public partial class CMSModules_Ecommerce_Pages_Tools_DiscountCoupons_DiscountCoupon_Edit_Product_List : CMSDiscountCouponsPage
{
    protected int mDiscountCouponId = 0;
    protected string mCurrentValues = string.Empty;
    protected DiscountCouponInfo mDiscountCouponInfoObj = null;


    protected void Page_Load(object sender, EventArgs e)
    {
        mDiscountCouponId = QueryHelper.GetInteger("objectid", 0);
        if (mDiscountCouponId > 0)
        {
            mDiscountCouponInfoObj = DiscountCouponInfoProvider.GetDiscountCouponInfo(mDiscountCouponId);
            EditedObject = mDiscountCouponInfoObj;

            if (mDiscountCouponInfoObj != null)
            {
                // Check if edited object belongs to configured site
                CheckEditedObjectSiteID(mDiscountCouponInfoObj.DiscountCouponSiteID);

                // Get the active skus
                DataSet ds = SKUInfoProvider.GetCouponProducts(mDiscountCouponId);
                if (!DataHelper.DataSourceIsEmpty(ds))
                {
                    mCurrentValues = TextHelper.Join(";", DataHelper.GetStringValues(ds.Tables[0], "SKUID"));
                }

                if (!RequestHelper.IsPostBack())
                {
                    uniSelector.Value = mCurrentValues;
                    radFollowing.Checked = true;
                    radExcept.Checked = false;

                    if (mDiscountCouponInfoObj.DiscountCouponIsExcluded)
                    {
                        radFollowing.Checked = false;
                        radExcept.Checked = true;
                    }
                }
            }
        }

        // Init selector
        uniSelector.OnSelectionChanged += uniSelector_OnSelectionChanged;
        uniSelector.WhereCondition = GetWhereCondition();

        radExcept.Text = GetString("Discount_Product.radExcept");
        radFollowing.Text = GetString("Discount_Product.radFollowing");
    }


    protected void uniSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        SaveItems();
    }


    protected void SaveItems()
    {
        if (mDiscountCouponInfoObj == null)
        {
            return;
        }

        // Check module permissions
        if (!ECommerceContext.IsUserAuthorizedToModifyDiscountCoupon(mDiscountCouponInfoObj))
        {
            if (mDiscountCouponInfoObj.IsGlobal)
            {
                RedirectToAccessDenied("CMS.Ecommerce", "EcommerceGlobalModify");
            }
            else
            {
                RedirectToAccessDenied("CMS.Ecommerce", "EcommerceModify OR ModifyDiscounts");
            }
        }

        // Remove old items
        string newValues = ValidationHelper.GetString(uniSelector.Value, null);
        string items = DataHelper.GetNewItemsInList(newValues, mCurrentValues);
        if (!String.IsNullOrEmpty(items))
        {
            string[] newItems = items.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            // Add all new items to sku
            foreach (string item in newItems)
            {
                int skuId = ValidationHelper.GetInteger(item, 0);
                SKUDiscountCouponInfoProvider.RemoveDiscountCouponFromSKU(skuId, mDiscountCouponId);
            }
        }

        // Add new items
        items = DataHelper.GetNewItemsInList(mCurrentValues, newValues);
        if (!String.IsNullOrEmpty(items))
        {
            string[] newItems = items.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            // Add all new items to sku
            foreach (string item in newItems)
            {
                int skuId = ValidationHelper.GetInteger(item, 0);
                SKUDiscountCouponInfoProvider.AddDiscountCouponToSKU(skuId, mDiscountCouponId);
            }
        }

        // Show message
        ShowChangesSaved();
    }


    protected void radFollowing_CheckedChanged(object sender, EventArgs e)
    {
        if (mDiscountCouponInfoObj != null)
        {
            // Check permissions
            if (!ECommerceContext.IsUserAuthorizedToModifyDiscountCoupon(mDiscountCouponInfoObj))
            {
                if (mDiscountCouponInfoObj.IsGlobal)
                {
                    RedirectToAccessDenied("CMS.Ecommerce", "EcommerceGlobalModify");
                }
                else
                {
                    RedirectToAccessDenied("CMS.Ecommerce", "EcommerceModify OR ModifyDiscounts");
                }
            }

            mDiscountCouponInfoObj.DiscountCouponIsExcluded = radExcept.Checked;

            // Set discount
            DiscountCouponInfoProvider.SetDiscountCouponInfo(mDiscountCouponInfoObj);

            // Show message
            ShowChangesSaved();
        }
    }


    /// <summary>
    /// Creates where condition for product selector.
    /// </summary>
    protected string GetWhereCondition()
    {
        // Select nothing
        string where = "(1=0)";

        if (mDiscountCouponInfoObj != null)
        {
            // Offer global products for global coupons or in case they are allowed
            if (mDiscountCouponInfoObj.IsGlobal || ECommerceSettings.AllowGlobalProducts(mDiscountCouponInfoObj.DiscountCouponSiteID))
            {
                where = SqlHelper.AddWhereCondition(where, "SKUSiteID IS NULL", "OR");
            }

            // Offer site product only for site coupons
            if (!mDiscountCouponInfoObj.IsGlobal)
            {
                where = SqlHelper.AddWhereCondition(where, "SKUSiteID = " + mDiscountCouponInfoObj.DiscountCouponSiteID, "OR");
            }
        }

        where = SqlHelper.AddWhereCondition(where, "(SKUEnabled = 1) AND (SKUProductType != 'DONATION')");

        // Include only products from user's departments
        if (!ECommerceContext.IsUserAuthorizedForPermission("AccessAllDepartments"))
        {
            where = SqlHelper.AddWhereCondition(where, String.Format("(SKUDepartmentID IN (SELECT DepartmentID FROM COM_UserDepartment WHERE UserID = {0}) OR (SKUDepartmentID IS NULL))", MembershipContext.AuthenticatedUser.UserID));
        }

        // Include selected values
        if (!string.IsNullOrEmpty(mCurrentValues))
        {
            string[] skuIds = mCurrentValues.Split(';');
            int[] intSkuIds = ValidationHelper.GetIntegers(skuIds, 0);

            where = SqlHelper.AddWhereCondition(where, SqlHelper.GetWhereCondition("SKUID", intSkuIds), "OR");
        }

        // Select only products - not product options
        where = SqlHelper.AddWhereCondition(where, "SKUOptionCategoryID IS NULL");

        return where;
    }
}