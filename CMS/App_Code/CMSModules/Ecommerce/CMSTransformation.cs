using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Data;

using CMS.Ecommerce;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.Search;
using CMS.SiteProvider;

namespace CMS.Controls
{
    /// <summary>
    /// SKU transformation methods
    /// </summary>
    public partial class CMSTransformation
    {
        #region "Variables"

        SKUInfo mSKU = null;

        #endregion


        #region "Properties"

        /// <summary>
        /// SKU data loaded from the current data row
        /// </summary>
        public SKUInfo SKU
        {
            get
            {
                if (mSKU == null)
                {
                    ShoppingCartItemInfo cartItem = DataItem as ShoppingCartItemInfo;

                    // Try to get the SKU object from the ShoppingCartItemInfo object at first if used
                    if (cartItem != null)
                    {
                        mSKU = cartItem.SKU;
                    }
                    else
                    {
                        // Get SKU object
                        DataRow row = (DataRowView != null) ? DataRowView.Row : null;
                        if (row != null)
                        {
                            #region "Smart search data row"

                            // Try get data row value from search result
                            DataColumnCollection col = row.Table.Columns;
                            if (col.Contains("id") && (col.Contains("score")))
                            {
                                // try get value
                                string id = ValidationHelper.GetString(Eval("id"), String.Empty);
                                if (!String.IsNullOrEmpty(id))
                                {
                                    // Try get data row
                                    var resultRows = SearchContext.CurrentSearchResults;
                                    if (resultRows != null)
                                    {
                                        row = resultRows[id];
                                    }
                                }
                            }

                            #endregion

                            mSKU = new SKUInfo(row);
                        }
                    }
                }

                return mSKU;
            }
        }


        /// <summary>
        /// Indicates if product prices are displayed including discounts
        /// </summary>
        public bool PriceIncludingDiscounts
        {
            get
            {
                return ECommerceSettings.DisplayPriceIncludingDiscounts(SiteContext.CurrentSiteName);
            }
        }


        /// <summary>
        /// Indicates if product prices are displayed including taxes
        /// </summary>
        public bool PriceIncludingTaxes
        {
            get
            {
                return ECommerceSettings.DisplayPriceIncludingTaxes(SiteContext.CurrentSiteName);
            }
        }

        #endregion


        #region "SKU price"

        /// <summary>
        /// Returns SKU catalog price based on the SKU data and the data of the current shopping cart. 
        /// Catalog discounts and/or taxes are included optionally.
        /// </summary>
        /// <param name="discounts">True - catalog discounts are applied to the price</param>
        /// <param name="taxes">True - catalog taxes are applied to the discounted price</param>
        /// <param name="column">SKU column from which the price should be returned. If empty, SKUPrice column is used.</param>
        public double GetSKUPrice(bool discounts, bool taxes, string column)
        {
            return ApplyExchangeRate(SKUInfoProvider.GetSKUPrice(SKU, null, discounts, taxes, false, column));
        }


        /// <summary>
        /// Returns SKU catalog price based on the SKU data and the data of the current shopping cart. 
        /// Catalog discounts and/or taxes are included optionally.
        /// </summary>
        /// <param name="discounts">True - catalog discounts are applied to the price</param>
        /// <param name="taxes">True - catalog taxes are applied to the discounted price</param>
        public double GetSKUPrice(bool discounts, bool taxes)
        {
            return GetSKUPrice(discounts, taxes, null);
        }


        /// <summary>
        /// Returns SKU catalog price based on the SKU data and the data of the current shopping cart. 
        /// Catalog discounts and/or taxes are included based on the site settings.
        /// </summary>
        /// <param name="column">SKU column from which the price should be returned. If empty, SKUPrice column is used.</param>
        public double GetSKUPrice(string column)
        {
            return GetSKUPrice(PriceIncludingDiscounts, PriceIncludingTaxes, column);
        }


        /// <summary>
        /// Returns SKU catalog price based on the SKU data and the data of the current shopping cart. 
        /// Catalog discounts and/or taxes are included based on the site settings.
        /// </summary>
        public double GetSKUPrice()
        {
            return GetSKUPrice(PriceIncludingDiscounts, PriceIncludingTaxes, null);
        }


        /// <summary>
        /// Returns formatted SKU catalog price based on the SKU data and the data of the current shopping cart. 
        /// Catalog discounts and/or taxes are included optionally.
        /// </summary>
        /// <param name="discounts">True - catalog discounts are applied to the price</param>
        /// <param name="taxes">True - catalog taxes are applied to the discounted price</param>
        /// <param name="column">SKU column from which the price should be returned. If empty, SKUPrice column is used.</param>
        public string GetSKUFormattedPrice(bool discounts, bool taxes, string column)
        {
            return SKUInfoProvider.GetSKUFormattedPrice(SKU, null, discounts, taxes, column);
        }


        /// <summary>
        /// Returns formatted SKU catalog price based on the SKU data and the data of the current shopping cart. 
        /// Catalog discounts and/or taxes are included optionally.
        /// </summary>
        /// <param name="discounts">True - catalog discounts are applied to the price</param>
        /// <param name="taxes">True - catalog taxes are applied to the discounted price</param>
        public string GetSKUFormattedPrice(bool discounts, bool taxes)
        {
            return GetSKUFormattedPrice(discounts, taxes, null);
        }


        /// <summary>
        /// Returns formatted product catalog price based on the SKU data and the data of the current shopping cart. 
        /// Catalog discounts and/or taxes are included based on the site settings.
        /// </summary>
        /// <param name="column">SKU column from which the price should be returned. If empty, SKUPrice column is used.</param>
        public string GetSKUFormattedPrice(string column)
        {
            return GetSKUFormattedPrice(PriceIncludingDiscounts, PriceIncludingTaxes, column);
        }


        /// <summary>
        /// Returns formatted product catalog price based on the SKU data and the data of the current shopping cart. 
        /// Catalog discounts and/or taxes are included based on the site settings.
        /// </summary>
        public string GetSKUFormattedPrice()
        {
            return GetSKUFormattedPrice(null);
        }

        #endregion


        #region "SKU original price"

        /// <summary>
        /// Returns SKU list price based on the SKU data and the data of the current shopping cart. 
        /// Catalog discounts are not included, taxes are included based on the site settings.
        /// </summary>
        public double GetSKUListPrice()
        {
            return GetSKUListPrice(PriceIncludingTaxes);
        }


        /// <summary>
        /// Returns SKU list price based on the SKU data and the data of the current shopping cart. 
        /// Catalog discounts are not included, taxes are included optionally.
        /// </summary>
        public double GetSKUListPrice(bool taxes)
        {
            return GetSKUPrice(false, taxes, "SKURetailPrice");
        }


        /// <summary>
        /// Returns formatted SKU list price based on the SKU data and the data of the current shopping cart. 
        /// Catalog discounts are not included, taxes are included based on the site settings.
        /// </summary>
        public string GetSKUFormattedListPrice()
        {
            return GetSKUFormattedListPrice(PriceIncludingTaxes);
        }


        /// <summary>
        /// Returns formatted SKU list price based on the SKU data and the data of the current shopping cart. 
        /// Catalog discounts are not included, taxes are included optionally.
        /// </summary>
        /// <param name="taxes">True - catalog taxes are applied to the list price</param>
        public string GetSKUFormattedListPrice(bool taxes)
        {
            return GetSKUFormattedPrice(false, taxes, "SKURetailPrice");
        }


        /// <summary>
        /// Returns SKURetailPrice if defined, otherwise returns price before discounts.
        /// Returns zero if price saving is zero.
        /// </summary>
        public double GetSKUOriginalPrice()
        {
            return ApplyExchangeRate(EcommerceTransformationFunctions.GetSKUOriginalPrice(SKU, PriceIncludingTaxes));
        }


        /// <summary>
        /// Returns formatted SKURetailPrice if defined, otherwise returns price before discounts.
        /// Returns zero if price saving is zero.
        /// </summary>
        public string GetSKUFormattedOriginalPrice()
        {
            return FormatPrice(GetSKUOriginalPrice());
        }

        #endregion


        #region "SKU price saving"

        /// <summary>
        /// Returns amount of saved money based on the difference between product seller price and product list price or price before discount.
        /// </summary> 
        /// <param name="discounts">Indicates if discounts should be applied to the seller price before the saved amount is calculated</param>
        /// <param name="taxes">Indicates if taxes should be applied to both list price and seller price before the saved amount is calculated</param>
        /// <param name="column1">Name of the column from which the seller price is retrieved, if empty SKUPrice column is used</param>
        /// <param name="column2">Name of the column from which the list price is retrieved, if empty SKURetailPrice column is used</param>
        /// <param name="percentage">True - result is percentage, False - result is in the current currency</param>
        public double GetSKUPriceSaving(bool discounts, bool taxes, string column1, string column2, bool percentage)
        {
            return EcommerceTransformationFunctions.GetSKUPriceSaving(SKU, discounts, taxes, column1, column2, percentage);
        }


        /// <summary>
        /// Returns amount of saved money based on the difference between product seller price and product list price or price before discount.
        /// </summary> 
        /// <param name="discounts">Indicates if discounts should be applied to the seller price before the saved amount is calculated</param>
        /// <param name="taxes">Indicates if taxes should be applied to both list price and seller price before the saved amount is calculated</param>
        /// <param name="percentage">True - result is percentage, False - result is in the current currency</param>
        public double GetSKUPriceSaving(bool discounts, bool taxes, bool percentage)
        {
            return GetSKUPriceSaving(discounts, taxes, null, null, percentage);
        }


        /// <summary>
        /// Returns amount of saved money based on the difference between product seller price and product list price or price before discount.
        /// Catalog discounts and/or taxes are included based on the site settings.
        /// </summary> 
        /// <param name="column1">Name of the column from which the seller price is retrieved, if empty SKUPrice column is used</param>
        /// <param name="column2">Name of the column from which the list price is retrieved, if empty SKURetailPrice column is used</param>
        /// <param name="percentage">True - result is percentage, False - result is in the current currency</param>
        public double GetSKUPriceSaving(string column1, string column2, bool percentage)
        {
            return GetSKUPriceSaving(PriceIncludingDiscounts, PriceIncludingTaxes, column1, column2, percentage);
        }


        /// <summary>
        /// Returns amount of saved money based on the difference between product seller price and product list price or price before discount.
        /// Catalog discounts and/or taxes are included based on the site settings.
        /// </summary> 
        /// <param name="percentage">True - result is percentage, False - result is in the current currency</param>
        public double GetSKUPriceSaving(bool percentage = false)
        {
            return GetSKUPriceSaving(null, null, percentage);
        }


        /// <summary>
        /// Returns formatted string representing amount of saved money based on the difference between product seller price and product list price or price before discount.
        /// </summary> 
        /// <param name="discounts">Indicates if discounts should be applied to the seller price before the saved amount is calculated</param>
        /// <param name="taxes">Indicates if taxes should be applied to both list price and seller price before the saved amount is calculated</param>
        /// <param name="column1">Name of the column from which the seller price is retrieved, if empty SKUPrice column is used</param>
        /// <param name="column2">Name of the column from which the list price is retrieved, if empty SKURetailPrice column is used</param>
        public string GetSKUFormattedPriceSaving(bool discounts, bool taxes, string column1, string column2)
        {
            return FormatPrice(GetSKUPriceSaving(discounts, taxes, column1, column2, false));
        }


        /// <summary>
        /// Returns formatted string representing amount of saved money based on the difference between product seller price and product list price or price before discount.
        /// </summary> 
        /// <param name="discounts">Indicates if discounts should be applied to the seller price before the saved amount is calculated</param>
        /// <param name="taxes">Indicates if taxes should be applied to both list price and seller price before the saved amount is calculated</param>
        public string GetSKUFormattedPriceSaving(bool discounts, bool taxes)
        {
            return GetSKUFormattedPriceSaving(discounts, taxes, null, null);
        }


        /// <summary>
        /// Returns formatted string representing amount of saved money based on the difference between product seller price and product list price or price before discount.
        /// Catalog discounts and/or taxes are included based on the site settings.
        /// </summary> 
        /// <param name="column1">Name of the column from which the seller price is retrieved, if empty SKUPrice column is used</param>
        /// <param name="column2">Name of the column from which the list price is retrieved, if empty SKURetailPrice column is used</param>
        public string GetSKUFormattedPriceSaving(string column1, string column2)
        {
            return GetSKUFormattedPriceSaving(PriceIncludingDiscounts, PriceIncludingTaxes, column1, column2);
        }


        /// <summary>
        /// Returns formatted string representing amount of saved money based on the difference between product seller price and product list price or price before discount.
        /// Catalog discounts and/or taxes are included based on the site settings.
        /// </summary> 
        public string GetSKUFormattedPriceSaving()
        {
            return GetSKUFormattedPriceSaving(null, null);
        }

        #endregion


        #region "SKU price formatting"

        /// <summary>
        /// Applies current exchange rate to the given price and returns the result.
        /// </summary>
        /// <param name="price">Price in site main currency the current exchange rate should be applied to</param>        
        public double ApplyExchangeRate(double price)
        {
            return ECommerceContext.CurrentShoppingCart.ApplyExchangeRate(price);
        }


        /// <summary>
        /// Returns price rounded and formatted according to the current currency properties.
        /// </summary>
        /// <param name="price">Price to be formatted</param> 
        /// <param name="round">True - price is rounded according to the current currency settings before formatting</param>
        public string FormatPrice(double price, bool round)
        {
            return ECommerceContext.CurrentShoppingCart.GetFormattedPrice(price, round);
        }


        /// <summary>
        /// Returns price rounded and formatted according to the current currency properties.
        /// </summary>
        /// <param name="price">Price to be formatted</param>        
        public string FormatPrice(double price)
        {
            return FormatPrice(price, true);
        }

        #endregion


        #region "SKU properties"

        /// <summary>
        /// Returns value of the specified product public status column.
        /// If the product is evaluated as a new product in the store, public status set by 'CMSStoreNewProductStatus' setting is used, otherwise product public status is used.
        /// </summary>
        /// <param name="column">Name of the product public status column the value should be retrieved from</param>
        public object GetSKUIndicatorProperty(string column)
        {
            return EcommerceTransformationFunctions.GetSKUIndicatorProperty(SKU, column);
        }


        /// <summary>
        /// Indicates if the given SKU can be bought by the customer based on the SKU inventory properties.
        /// </summary>       
        public bool IsSKUAvailableForSale()
        {
            return SKUInfoProvider.IsSKUAvailableForSale(SKU);
        }


        /// <summary>
        /// Indicates the real stock status of SKU based on SKU items available.
        /// </summary>        
        public bool IsSKUInStock()
        {
            return SKUInfoProvider.IsSKUInStock(SKU);
        }


        /// <summary>
        /// Gets the SKU node alias. If there are multiple nodes for this SKU the first occurrence is returned.
        /// If there is not a single one node for this SKU, empty string is returned.
        /// </summary>       
        public string GetSKUNodeAlias()
        {
            return EcommerceTransformationFunctions.GetSKUNodeAlias(SKU);
        }

        #endregion


        #region "SKU URLs"

        /// <summary>
        /// Returns SKU permanent URL.
        /// </summary>        
        public string GetSKUUrl()
        {
            return EcommerceTransformationFunctions.GetProductUrl(SKU.SKUGUID, ResHelper.LocalizeString(SKU.SKUName));
        }


        /// <summary>
        /// Returns SKU image URL including dimension's modifiers (width, height) and site name parameter if product is from different site than current. 
        /// If image URL is not specified, SKU default image URL is used.
        /// </summary>
        /// <param name="width">Image requested width</param>
        /// <param name="height">Image requested height</param>      
        public string GetSKUImageUrl(int width, int height)
        {
            return EcommerceTransformationFunctions.GetSKUImageUrl(SKU.SKUImagePath, width, height, 0, SKU.SKUSiteID);
        }


        /// <summary>
        /// Returns SKU image URL including dimension's modifiers (width, height) and site name parameter if product is from different site than current. 
        /// If image URL is not specified, SKU default image URL is used.
        /// </summary>
        /// <param name="maxSideSize">Image requested maximum side size</param>        
        public string GetSKUImageUrl(int maxSideSize)
        {
            return EcommerceTransformationFunctions.GetSKUImageUrl(SKU.SKUImagePath, 0, 0, maxSideSize, SKU.SKUSiteID);
        }


        /// <summary>
        /// Returns URL of the specified product.
        /// </summary>
        /// <param name="skuGUID">SKU Guid</param>
        /// <param name="skuName">SKU name</param>
        /// <param name="siteName">Site name</param>
        public string GetProductUrl(object skuGUID, object skuName, object siteName)
        {
            return EcommerceTransformationFunctions.GetProductUrl(skuGUID, skuName, siteName);
        }


        /// <summary>
        /// Returns URL of the specified product with feed parameter.
        /// </summary>
        /// <param name="skuGUID">SKU GUID</param>
        /// <param name="skuName">SKU name</param>
        /// <param name="siteName">Site name</param>
        public string GetProductUrlForFeed(object skuGUID, object skuName, object siteName)
        {
            return EcommerceTransformationFunctions.GetProductUrlForFeed(GetFeedName(), skuGUID, skuName, siteName);
        }

        #endregion


        /// <summary>
        /// Returns names of multi buy discounts for current cart item surrounded with li tag.
        /// </summary>        
        public string GetMultiBuyDiscountNames()
        {
            var cartItem = DataItem as ShoppingCartItemInfo;
            if ((cartItem == null))
            {
                return string.Empty;
            }

            var itemDiscountNames = cartItem.ShoppingCart.CartItemsMultiBuyDiscountNames;
            if ((itemDiscountNames != null) && itemDiscountNames.ContainsKey(cartItem.CartItemID))
            {
                // Join discount names as list items
                var sb = new StringBuilder();
                foreach (var name in itemDiscountNames[cartItem.CartItemID])
                {
                    sb.Append(string.Format("<li>{0}</li>", HTMLHelper.HTMLEncode(ResHelper.LocalizeString(name))));
                }

                return sb.ToString();
            }

            return string.Empty;
        }
    }
}
