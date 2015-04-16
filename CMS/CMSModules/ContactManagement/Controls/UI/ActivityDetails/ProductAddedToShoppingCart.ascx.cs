using System;

using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.OnlineMarketing;
using CMS.Base;
using CMS.WebAnalytics;

public partial class CMSModules_ContactManagement_Controls_UI_ActivityDetails_ProductAddedToShoppingCart : ActivityDetail
{
    #region "Methods"

    public override bool LoadData(ActivityInfo ai)
    {
        if ((ai == null) || !ModuleManager.IsModuleLoaded(ModuleName.ECOMMERCE))
        {
            return false;
        }

        switch (ai.ActivityType)
        {
            case PredefinedActivityType.PRODUCT_ADDED_TO_WISHLIST:
            case PredefinedActivityType.PRODUCT_ADDED_TO_SHOPPINGCART:
            case PredefinedActivityType.PRODUCT_REMOVED_FROM_SHOPPINGCART:
                break;
            default:
                return false;
        }

        GeneralizedInfo sku = BaseAbstractInfoProvider.GetInfoById(PredefinedObjectType.SKU, ai.ActivityItemID);
        if (sku != null)
        {
            string productName = ValidationHelper.GetString(sku.GetValue("SKUName"), null);
            ucDetails.AddRow("om.activitydetails.product", productName);

            if (ai.ActivityType != PredefinedActivityType.PRODUCT_ADDED_TO_WISHLIST)
            {
                ucDetails.AddRow("om.activitydetails.productunits", ai.ActivityValue);
            }
        }

        return ucDetails.IsDataLoaded;
    }

    #endregion
}