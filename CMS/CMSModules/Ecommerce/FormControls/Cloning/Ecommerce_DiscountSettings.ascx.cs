using System;
using System.Linq;

using CMS.Ecommerce;
using CMS.UIControls;


public partial class CMSModules_Ecommerce_FormControls_Cloning_Ecommerce_DiscountSettings : CloneSettingsControl
{
    /// <summary>
    /// Exclude discount coupons from discounts cloning.
    /// </summary>
    public override string ExcludedChildTypes
    {
        get
        {
            return CouponCodeInfo.OBJECT_TYPE;
        }
    }


    /// <summary>
    /// Disable showing control in cloning pop-up.
    /// </summary>
    public override bool DisplayControl
    {
        get
        {
            return false;
        }
    }
}
