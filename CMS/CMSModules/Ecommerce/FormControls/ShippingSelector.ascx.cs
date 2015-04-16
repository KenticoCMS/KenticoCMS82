using System.Data;
using System.Web.UI.WebControls;

using CMS.DataEngine;
using CMS.Ecommerce;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_Ecommerce_FormControls_ShippingSelector : BaseObjectSelector
{
    #region "Variables"

    private bool mDisplayShippingOptionPrice = true;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Indicates the site the shipping options should be loaded from. If shopping cart is given use its site ID.
    /// Default value is current site ID.
    /// </summary>
    public override int SiteID
    {
        get
        {
            // If shopping cart given use its site ID
            if ((ShoppingCart != null) && (base.SiteID != ShoppingCart.ShoppingCartSiteID))
            {
                base.SiteID = ShoppingCart.ShoppingCartSiteID;
            }

            return base.SiteID;
        }
        set
        {
            base.SiteID = value;
        }
    }


    /// <summary>
    /// Shopping cart.
    /// </summary>
    public ShoppingCartInfo ShoppingCart
    {
        get
        {
            return GetValue("ShoppingCart") as ShoppingCartInfo;
        }
        set
        {
            SetValue("ShoppingCart", value);
        }
    }


    /// <summary>
    /// Decides if shipping option price should be displayed next to shipping option in the shipping selector. By default is true.
    /// </summary>
    public bool DisplayShippingOptionPrice
    {
        get
        {
            return mDisplayShippingOptionPrice;
        }
        set
        {
            mDisplayShippingOptionPrice = value;
        }
    }


    /// <summary>
    /// If true, selected value is ShippingOptionName, if false, selected value is ShippingOptionID.
    /// </summary>
    public override bool UseNameForSelection
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("UseShippingNameForSelection"), base.UseNameForSelection);
        }
        set
        {
            SetValue("UseShippingNameForSelection", value);
            base.UseNameForSelection = value;
        }
    }


    /// <summary>
    /// Allows to access selector object.
    /// </summary>
    public override UniSelector UniSelector
    {
        get
        {
            return uniSelector;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Display only site shipping options.
    /// </summary>
    protected override string AppendExclusiveWhere(string where)
    {
        return new WhereCondition(base.AppendExclusiveWhere(where))
            .WhereEquals(SiteIDColumn, SiteID)
            .ToString(true);
    }


    /// <summary>
    /// Convert given shipping name to its ID for specified site.
    /// </summary>
    /// <param name="name">Name of the shipping to be converted.</param>
    /// <param name="siteName">Name of the site of the shipping.</param>
    protected override int GetID(string name, string siteName)
    {
        var shipping = ShippingOptionInfoProvider.GetShippingOptionInfo(name, siteName);

        return (shipping != null) ? shipping.ShippingOptionID : 0;
    }


    /// <summary>
    /// Ensures that only applicable shipping options are displayed in selector.
    /// </summary>
    /// <param name="ds"></param>
    /// <returns></returns>
    protected override DataSet OnAfterRetrieveData(DataSet ds)
    {
        if ((ds == null) || (ShoppingCart == null))
        {
            return ds;
        }

        var shippingOptions = ds.Tables[0].Select();

        foreach (DataRow optionRow in shippingOptions)
        {
            int optionID = ValidationHelper.GetInteger(optionRow["ShippingOptionID"], 0);

            ShippingOptionInfo option = ShippingOptionInfoProvider.GetShippingOptionInfo(optionID);

            if ((option != null) && !ShippingOptionInfoProvider.IsShippingOptionApplicable(ShoppingCart, option))
            {
                ds.Tables[0].Rows.Remove(optionRow);
            }
        }

        return ds;
    }


    /// <summary>
    /// Adds price to individual shipping options when shopping cart object supplied.
    /// </summary>
    /// <param name="item">Shipping option item to add price to.</param>
    protected override void OnListItemCreated(ListItem item)
    {
        // Adding price to shipping option is not required
        if (!DisplayShippingOptionPrice)
        {
            return;
        }

        if ((item != null) && (ShoppingCart != null))
        {
            // Store original shipping option ID 
            int origShippingOptionId = ShoppingCart.ShoppingCartShippingOptionID;

            // Calculate hypothetical shipping cost for shipping option from supplied list item
            ShoppingCart.ShoppingCartShippingOptionID = ValidationHelper.GetInteger(item.Value, 0);

            // Get site name
            SiteInfo si = SiteInfoProvider.GetSiteInfo(ShoppingCart.ShoppingCartSiteID);
            if (si != null)
            {
                // Get shipping cost for currently processed shipping option
                double shipping = ShoppingCart.TotalShipping;

                string detailInfo = "";
                if (shipping > 0)
                {
                    detailInfo = "(" + CurrencyInfoProvider.GetFormattedPrice(shipping, ShoppingCart.Currency, false) + ")";
                }

                // Check if displaying in RTL culture
                bool rtl = IsLiveSite ? CultureHelper.IsPreferredCultureRTL() : CultureHelper.IsUICultureRTL();
                if (rtl)
                {
                    item.Text = ((detailInfo == "") ? "" : detailInfo + " ") + item.Text;
                }
                else
                {
                    item.Text += ((detailInfo == "") ? "" : " " + detailInfo);
                }
            }

            // Restore original shipping option ID
            ShoppingCart.ShoppingCartShippingOptionID = origShippingOptionId;
        }
    }

    #endregion
}