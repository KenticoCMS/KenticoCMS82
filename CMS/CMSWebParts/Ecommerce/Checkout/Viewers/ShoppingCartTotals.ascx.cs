using System;
using System.Linq;
using System.Web.UI;

using CMS.Controls;
using CMS.Ecommerce;
using CMS.Helpers;
using CMS.PortalEngine;

/// <summary>
/// Shopping cart totals web part
/// </summary>
public partial class CMSWebParts_Ecommerce_Checkout_Viewers_ShoppingCartTotals : CMSCheckoutWebPart
{
    #region "Constructor"

    /// <summary>
    /// Initializes a new instance of the CMSWebParts_Ecommerce_Checkout_Viewers_ShoppingCartTotals" class.
    /// </summary>
    public CMSWebParts_Ecommerce_Checkout_Viewers_ShoppingCartTotals()
    {
        // Do not resolve Visible field in configuration
        base.NotResolveProperties = string.Format("{0};Visible;", base.NotResolveProperties);
    }

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets the visibility condition. If the condition is true web part is visible.
    /// </summary>
    public string VisibilityCondition
    {
        get
        {
            // return macro string without macro brackets
            return ValidationHelper.GetString(GetValue("Visible"), "").Replace("{%", "").Replace("%}", "");
        }
    }


    /// <summary>
    /// Gets or sets the name of the transformation which is used for MultiBuy discount summary.
    /// </summary>
    public string OrderDiscountSummaryTransformationName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("OrderDiscountSummaryTransformationName"), "");
        }
        set
        {
            SetValue("OrderDiscountSummaryTransformationName", value);
        }
    }

    #endregion


    #region "Event handling"

    /// <summary>
    /// OnInit event handler.
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Subscribe to the wizard events
        SubscribeToWizardEvents();
    }


    /// <summary>
    /// Load event handler.
    /// </summary>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        SetupControl();

        if (UpdatePanel != null)
        {
            UpdatePanel.UpdateMode = UpdatePanelUpdateMode.Always;
        }
    }


    /// <summary>
    /// PreRender event handler.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (!String.IsNullOrEmpty(VisibilityCondition))
        {
            // Check condition value, if it is false, hide web part and also envelope
            var res = ContextResolver.ResolveMacroExpression(VisibilityCondition, true);
            if ((res == null) || !ValidationHelper.GetBoolean(res.Result, false))
            {
                totalViewer.Visible = false;
                HideWebPartContent();
            }
        }
    }


    /// <summary>
    /// Updates web part.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The EventArgs instance containing the event data.</param>
    public void Update(object sender, EventArgs e)
    {
        SetupControl();
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Subscribes the web part to the wizard events.
    /// </summary>
    private void SubscribeToWizardEvents()
    {
        ComponentEvents.RequestEvents.RegisterForEvent(SHOPPING_CART_CHANGED, Update);
    }


    /// <summary>
    /// Setups the control.
    /// </summary>
    public void SetupControl()
    {
        double value = 0;
        string stringFormat = ValidationHelper.GetString(GetValue("StringFormat"), "");

        // Try to use the stringFormat format, to check, if it's a valid one
        try
        {
            String.Format(stringFormat, value);
        }
        catch (Exception ex)
        {
            CMS.EventLog.EventLogProvider.LogException("Checkout process", "ERROR", ex, CurrentSite.SiteID, "The StringFormat property of the web part isn't in a correct format: '" + stringFormat + "'");
            // Recovery
            stringFormat = "";
        }

        string type = ValidationHelper.GetString(GetValue("TotalToDisplay"), "TotalPriceOfProducts");

        // Display the correct value according to the TotalToDisplay property of the web part
        switch (type)
        {
            case "TotalPriceOfOrder":
                value = ShoppingCart.TotalPrice;
                DisplayValue(GetFormattedPriceToDisplay(value, stringFormat));
                break;

            case "TotalPriceOfProducts":
                value = ShoppingCart.TotalItemsPrice;
                DisplayValue(GetFormattedPriceToDisplay(value, stringFormat));
                break;

            case "TotalPriceOfProductsWithoutTax":
                value = ShoppingCart.TotalItemsPrice - ShoppingCart.TotalItemsTax;
                DisplayValue(GetFormattedPriceToDisplay(value, stringFormat));
                break;

            case "TotalShippingWithoutTax":
                if (ShoppingCart.ShippingOption != null)
                {
                    value = ShoppingCart.ApplyExchangeRate(ShippingOptionInfoProvider.CalculateShipping(ShoppingCart));
                }

                DisplayValue(GetFormattedPriceToDisplay(value, stringFormat));
                break;

            case "TotalShipping":
                if (ShoppingCart.ShippingOption != null)
                {
                    value = ShoppingCart.TotalShipping;
                }

                DisplayValue(GetFormattedPriceToDisplay(value, stringFormat));
                break;

            case "TotalWeight":
                value = ShoppingCart.TotalItemsWeight;
                DisplayValue(stringFormat == "" ? ShippingOptionInfoProvider.GetFormattedWeight(value, CurrentSiteName) : String.Format(stringFormat, value));
                break;

            case "TotalProductTax":
                value = ShoppingCart.TotalTax;
                DisplayValue(GetFormattedPriceToDisplay(value, stringFormat));
                break;

            case "TotalOrderTax":
                value = ShoppingCart.TotalTax;

                if (ShoppingCart.ShippingOption != null)
                {
                    value += ShoppingCart.ApplyExchangeRate(ShippingOptionInfoProvider.CalculateShippingTax(ShoppingCart));
                }

                DisplayValue(GetFormattedPriceToDisplay(value, stringFormat));
                break;

            case "TotalShippingTax":
                if (!DataHelper.DataSourceIsEmpty(ShoppingCart.ShippingTaxesTable))
                {
                    value = ValidationHelper.GetDoubleSystem(ShoppingCart.ShippingTaxesTable.Rows[0]["TaxSummary"], 0);
                }

                DisplayValue(GetFormattedPriceToDisplay(value, stringFormat));
                break;

            case "TotalDiscount":
                value = ShoppingCart.OrderDiscount + ShoppingCart.ItemsDiscount + ShoppingCart.ShippingDiscount;
                DisplayValue(GetFormattedPriceToDisplay(value, stringFormat));
                break;

            case "TotalOrderDiscount":
                value = ShoppingCart.OrderDiscount;
                DisplayValue(GetFormattedPriceToDisplay(value, stringFormat));
                break;

            case "OrderDiscountSummary":
                DisplayOrderDiscountSummary();
                break;
        }
    }


    private string GetFormattedPriceToDisplay(double value, string stringFormat)
    {
        return stringFormat == "" ? ShoppingCart.GetFormattedPrice(value) : String.Format(stringFormat, value);
    }


    /// <summary>
    /// Displays the passed value and manages the label visibility.
    /// </summary>
    /// <param name="value">Value to display</param>
    private void DisplayValue(string value)
    {
        totalViewer.Visible = true;

        string label = ValidationHelper.GetString(GetValue("Label"), "");
        label = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(label));

        if (!string.IsNullOrEmpty(label))
        {
            lblLabel.Text = label;
            lblLabel.Visible = true;
        }

        lblValue.Text = value;
    }


    /// <summary>
    /// Displays multibuy and order discounts summary based on provided transformation. 
    /// </summary>
    private void DisplayOrderDiscountSummary()
    {
        if (!string.IsNullOrEmpty(OrderDiscountSummaryTransformationName))
        {
            TransformationInfo ti = TransformationInfoProvider.GetTransformation(OrderDiscountSummaryTransformationName);

            if (ti == null)
            {
                return;
            }

            uvMultiBuySummary.Visible = true;
            uvMultiBuySummary.DataSource = ShoppingCart.OrderRelatedDiscountSummaryItems;
            uvMultiBuySummary.ItemTemplate = CMSAbstractDataProperties.LoadTransformation(uvMultiBuySummary, ti.TransformationFullName);

            // Makes sure new data is loaded if the date changes and transformation needs to be reloaded
            uvMultiBuySummary.DataBind();
            uvMultiBuySummary.ReloadData(true);
        }
    }

    #endregion
}