using System;
using System.Linq;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Controls;
using CMS.DataEngine;
using CMS.DataEngine.Query;
using CMS.Ecommerce;
using CMS.Helpers;
using CMS.UIControls;
using CMS.ExtendedControls;

public partial class CMSModules_Ecommerce_Controls_Filters_DiscountFilter : CMSAbstractDataFilterControl
{
    #region "Public properties"

    /// <summary>
    /// Gets or sets the SQL condition for filtering the discount list.
    /// </summary>
    public override string WhereCondition
    {
        get
        {
            base.WhereCondition = GetFilterWhereCondition();
            return base.WhereCondition;
        }
        set
        {
            base.WhereCondition = value;
        }
    }

    #endregion


    #region "Private properties" 

    /// <summary>
    /// Indicates whether multibuy discounts are filtered.
    /// </summary>
    private bool IsMultibuy
    {
        get;
        set;
    }


    /// <summary>
    /// Discount prefix according discount object type.
    /// </summary>
    private string DiscountPrefix
    {
        get
        {
            return IsMultibuy ? "MultiBuy" : String.Empty;
        }
    }


    /// <summary>
    /// Coupon code object type.
    /// </summary>
    private string CouponCodeObjectType
    {
        get
        {
            return IsMultibuy ? MultiBuyCouponCodeInfo.OBJECT_TYPE : CouponCodeInfo.OBJECT_TYPE;
        }
    }


    /// <summary>
    /// Discount application type.
    /// </summary>
    private DiscountApplicationEnum? DiscountApplication
    {
        get
        {
            var enumValueIndex = QueryHelper.GetInteger("type", -1);
            if (enumValueIndex < 0)
            {
                return null;
            }
            return (DiscountApplicationEnum)enumValueIndex;
        }
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        InitializeDropDownList(drpStatus);


        var ug = FilteredControl as UniGrid;
        if (ug != null)
        {
            IsMultibuy = ug.ObjectType.EqualsCSafe(MultiBuyDiscountInfo.OBJECT_TYPE);
        }
    }

    #endregion


    #region "Event handlers"

    protected void btnFilter_Click(object sender, EventArgs e)
    {
        ApplyFilter(sender, e);
    }


    protected void btnReset_Click(object sender, EventArgs e)
    {
        UniGrid grid = FilteredControl as UniGrid;
        if (grid != null)
        {
            grid.Reset();
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Applies filter to unigrid.
    /// </summary>
    /// <param name="sender">Sender.</param>
    /// <param name="e">Event args.</param>
    private void ApplyFilter(object sender, EventArgs e)
    {
        UniGrid grid = FilteredControl as UniGrid;
        if (grid != null)
        {
            grid.ApplyFilter(sender, e);
        }
    }


    /// <summary>
    /// Initializes the specified controls with values.
    /// </summary>
    /// <param name="control">A control to initialize.</param>
    private void InitializeDropDownList(CMSDropDownList control)
    {
        control.Items.Add(new ListItem(GetString("general.selectall"), "-1"));
        control.Items.Add(new ListItem(GetString("com.discountstatus.active"), "0"));
        control.Items.Add(new ListItem(GetString("com.discountstatus.disabled"), "1"));
        control.Items.Add(new ListItem(GetString("com.discountstatus.finished"), "2"));
        control.Items.Add(new ListItem(GetString("com.discountstatus.notstarted"), "3"));

        // Catalog discount can not have incomplete status
        if (!DiscountApplication.HasValue || (DiscountApplication.Value != DiscountApplicationEnum.Products))
        {
            control.Items.Add(new ListItem(GetString("com.discountstatus.incomplete"), "4"));
        }
    }


    /// <summary>
    /// Builds a SQL condition for filtering the discount list, and returns it.
    /// </summary>
    /// <returns>A SQL condition for filtering the discount list.</returns>
    private string GetFilterWhereCondition()
    {
        string discountStatus = drpStatus.SelectedValue;
        var condition = new WhereCondition();


        /* Active discounts */
        if (discountStatus == "0")
        {
            condition.Where(GetActiveQuery());
        }
        /* Disabled discounts */
        else if (discountStatus == "1")
        {
            condition.WhereNot(GetEnabledDiscounts());
        }
        /* Finished discounts */
        else if (discountStatus == "2")
        {
            condition.Where(GetEnabledDiscounts())
                     .WhereLessThan(GetColumn("DiscountValidTo"), DateTime.Now)
                     .WhereNot(GetIncompleteDiscounts())
                     .Or(GetDiscountsWithCouponsExceeded());
        }
        /* Scheduled discounts */
        else if (discountStatus == "3")
        {
            condition.Where(GetEnabledDiscounts())
                     .WhereGreaterThan(GetColumn("DiscountValidFrom"), DateTime.Now)
                     .WhereNot(GetIncompleteDiscounts());
        }
        /* Incomplete discounts */
        else if (discountStatus == "4")
        {
            condition.Where(GetEnabledDiscounts())
                     .Where(GetIncompleteDiscounts());
        }

        return condition.ToString(true);
    }

    #endregion


    #region "State management"

    /// <summary>
    /// Resets filter to the default state.
    /// </summary>
    public override void ResetFilter()
    {
        drpStatus.SelectedValue = "-1";
    }


    /// <summary>
    /// Stores filter state to the specified object.
    /// </summary>
    /// <param name="state">The object that holds the filter state.</param>
    public override void StoreFilterState(FilterState state)
    {
        base.StoreFilterState(state);

        // Store additional state properties
        state.AddValue("drpStatus", drpStatus.SelectedValue);
    }


    /// <summary>
    /// Restores filter state from the specified object.
    /// </summary>
    /// <param name="state">The object that holds the filter state.</param>
    public override void RestoreFilterState(FilterState state)
    {
        base.RestoreFilterState(state);

        // Restore additional state properties
        drpStatus.SelectedValue = state.GetString("drpStatus", "-1");
    }

    #endregion
       
 
    #region "Status queries"

    /// <summary>
    /// Returns query to filter active discounts.
    /// </summary>
    private WhereCondition GetActiveQuery()
    {
        IWhereCondition activeQuery;
        if (IsMultibuy)
        {
            activeQuery = MultiBuyDiscountInfoProvider.GetRunningDiscounts(CurrentSite.SiteID, DateTime.Now);
        }
        else
        {
            activeQuery = DiscountInfoProvider.GetRunningDiscounts(CurrentSite.SiteID, DateTime.Now);
        }
        return new WhereCondition(activeQuery.Expand(activeQuery.WhereCondition));
    }


    /// <summary>
    /// Returns where condition to filter incomplete discounts.
    /// </summary>
    private WhereCondition GetIncompleteDiscounts()
    {
        return new WhereCondition()
                    .WhereTrue(GetColumn("DiscountUsesCoupons"))
                    .WhereNotIn(GetColumn("DiscountID"), new IDQuery(CouponCodeObjectType, GetColumn("CouponCode") + GetColumn("DiscountID")));
        
    }


    /// <summary>
    /// Returns where condition to filter discounts with exceeded coupon use.
    /// </summary>
    private WhereCondition GetDiscountsWithCouponsExceeded()
    {
        return new WhereCondition()
                    .WhereNot (GetIncompleteDiscounts())
                    .WhereTrue(GetColumn("DiscountUsesCoupons"))
                    .WhereNotIn(GetColumn("DiscountID"), new IDQuery(CouponCodeObjectType, GetColumn("CouponCode") + GetColumn("DiscountID"))
                                                                .WhereNull(GetColumn("CouponCodeUseLimit"))
                                                                .Or()
                                                                .WhereLessThan(GetColumn("CouponCodeUseCount"), GetColumn("CouponCodeUseLimit").AsColumn()));
    }


    /// <summary>
    /// Returns where condition to filter enabled discounts.
    /// </summary>
    private WhereCondition GetEnabledDiscounts()
    {
        return new WhereCondition()
                    .WhereTrue(GetColumn("DiscountEnabled"));
    }


    /// <summary>
    /// Returns right column name according discount object type.
    /// </summary>
    /// <param name="columnName">Column name.</param>
    private string GetColumn(string columnName)
    {
        return DiscountPrefix + columnName;
    }

    #endregion
}
