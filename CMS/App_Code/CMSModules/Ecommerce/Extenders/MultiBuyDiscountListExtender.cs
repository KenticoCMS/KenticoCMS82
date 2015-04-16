using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using CMS;
using CMS.Base;
using CMS.DataEngine;
using CMS.Ecommerce;
using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.UIControls;

[assembly: RegisterCustomClass("MultiBuyDiscountListExtender", typeof(MultiBuyDiscountListExtender))]

/// <summary>
/// MultiBuy discount list extender
/// </summary>
public class MultiBuyDiscountListExtender : ControlExtender<UniGrid>
{
    private ObjectTransformationDataProvider couponCountsDataProvider;

    public override void OnInit()
    {
        couponCountsDataProvider = new ObjectTransformationDataProvider();
        couponCountsDataProvider.SetDefaultDataHandler(GetCountsDataHandler);

        if (Control != null)
        {
            Control.OnExternalDataBound += OnExternalDataBound;
        }
    }


    /// <summary>
    /// External data bound handler.
    /// </summary>
    protected object OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        DataRowView discountRow = parameter as DataRowView;
        MultiBuyDiscountInfo discountInfo = null;

        if (discountRow != null)
        {
            discountInfo = new MultiBuyDiscountInfo(discountRow.Row);
        }
        if (discountInfo == null)
        {
            return String.Empty;
        }

        switch (sourceName.ToLowerCSafe())
        {
            case "status":
                // Ensure correct values for unigrid export
                if (sender == null)
                {
                    return discountInfo.Status.ToLocalizedString("com.discountstatus");
                }   
                return new DiscountStatusTag(couponCountsDataProvider, discountInfo);

            case "application":
                // Display dash if discount don't use coupons 
                if (!discountInfo.MultiBuyDiscountUsesCoupons)
                {
                    return "&mdash;";
                }

                var tr = new ObjectTransformation("CouponsCounts", discountInfo.MultiBuyDiscountID)
                {
                    DataProvider = couponCountsDataProvider,
                    Transformation = "{% FormatString(GetResourceString(\"com.couponcode.appliedxofy\"), Convert.ToString(Uses, \"0\"), (UnlimitedCodeCount != 0)? GetResourceString(\"com.couponcode.unlimited\") : Convert.ToString(Limit, \"0\")) %}",
                    NoDataTransformation = "{$com.discount.notcreated$}",
                    EncodeOutput = false
                };

                return tr;

            case "discountpriority":
                // Ensure correct values for unigrid export
                if ((sender == null) || !ECommerceContext.IsUserAuthorizedToModifyDiscount())
                {
                    return discountInfo.MultiBuyDiscountPriority;
                }

                return new PriorityInlineEdit
                {
                    PrioritizableObject = discountInfo,
                    Unigrid = Control
                };
        }

        return parameter;
    }


    /// <summary>
    /// Returns dictionary of discount coupon use count and limit. Key of the dictionary is the ID of discount.
    /// </summary>
    /// <param name="type">Object type (ignored).</param>
    /// <param name="discountIDs">IDs of discount which the dictionary is to be filled with.</param>
    protected SafeDictionary<int, IDataContainer> GetCountsDataHandler(string type, IEnumerable<int> discountIDs)
    {
        DataSet countsDs = MultiBuyCouponCodeInfoProvider.GetCouponCodeUseCount(discountIDs);

        return countsDs.ToDictionaryById("MultiBuyCouponCodeMultiBuyDiscountID");
    }
}