using System;

using CMS.Ecommerce;
using CMS.Helpers;
using CMS.ExtendedControls;

/// <summary>
/// Currency selector web part
/// </summary>
public partial class CMSWebParts_Ecommerce_Checkout_Selectors_CurrencySelection : CMSCheckoutWebPart
{
    #region "Constants"

    private const string CURRENCY_NAME_FORMAT = "{%CurrencyDisplayName%}";
    private const string CURRENCY_CODE_FORMAT = "{%CurrencyCode%}";

    private const string CURRENCY_NAME = "currencyname";
    private const string CURRENCY_CODE = "currencycode";

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets a value indicating whether do full page reload after selection changed.
    /// </summary>    
    public bool ReloadPage
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ReloadPage"), false);
        }
        set
        {
            SetValue("ReloadPage", value);
        }
    }

    #endregion


    #region "Page events"

    /// <summary>
    /// Load event handler.
    /// </summary>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        // Get display name format property from web part
        string displayNameFormat = ValidationHelper.GetString(GetValue("DisplayNameFormat"), CURRENCY_CODE);

        // Set currency display format e.q. Code or Name etc
        switch (displayNameFormat)
        {
            case CURRENCY_NAME:
                selectCurrency.DisplayNameFormat = CURRENCY_NAME_FORMAT;
                break;
            default:
                selectCurrency.DisplayNameFormat = CURRENCY_CODE_FORMAT;
                break;
        }

        SetupControl();
    }


    /// <summary>
    /// Control initialization after postback events.
    /// </summary>  
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Hides the currency selector if there is only one currency to display or if the selector is empty
        pnlCurrency.Visible = (selectCurrency.HasData && (selectCurrency.UniSelector.DropDownItems != null) && (selectCurrency.UniSelector.DropDownItems.Count > 1));

        if (pnlCurrency.Visible)
        {
            selectCurrency.SelectedID = ShoppingCart.ShoppingCartCurrencyID;
        }
    }

    #endregion


    #region "Event handling"

    /// <summary>
    /// Handles the currency change event. Saving of the selected value into the shopping cart object.
    /// </summary>
    private void DropDownSingleSelect_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Set currency for the shopping cart according to the selected value
        ShoppingCart.ShoppingCartCurrencyID = selectCurrency.SelectedID;
        // Raise the change event for all subscribed web parts
        ComponentEvents.RequestEvents.RaiseEvent(sender, e, SHOPPING_CART_CHANGED);

        if (ReloadPage)
        {
            URLHelper.Redirect(RequestContext.CurrentURL);
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Preselects currency.
    /// </summary>
    protected void SelectCurrency()
    {
        // Try to set the currency to the currency preferred by the user
        if (ShoppingCart.ShoppingCartCurrencyID == 0)
        {
            // Set customer preferred options   
            CustomerInfo currentCustomer = ECommerceContext.CurrentCustomer;

            if ((currentCustomer != null) && (currentCustomer.CustomerUser != null))
            {
                ShoppingCart.ShoppingCartCurrencyID = currentCustomer.CustomerUser.GetUserPreferredCurrencyID(ShoppingCart.SiteName);
                // Invalidate the shopping cart
                ShoppingCart.InvalidateExchangeRate();
                ShoppingCart.InvalidateCalculations();
            }
        }

        // Set the currency to the site currency, if preferred currency isn't set by the user
        if (ShoppingCart.ShoppingCartCurrencyID == 0)
        {
            CurrencyInfo mainCurrency = CurrencyInfoProvider.GetMainCurrency(ShoppingCart.ShoppingCartSiteID);

            if ((ShoppingCart.SiteName != null) && (mainCurrency != null))
            {
                ShoppingCart.ShoppingCartCurrencyID = mainCurrency.CurrencyID;
                // Invalidate the shopping cart
                ShoppingCart.InvalidateExchangeRate();
                ShoppingCart.InvalidateCalculations();
            }
        }
    }


    /// <summary>
    /// Setting up the control.
    /// </summary>
    public void SetupControl()
    {
        if (!StopProcessing)
        {
            // Set currency selectors site ID
            selectCurrency.SiteID = ShoppingCart.ShoppingCartSiteID;

            // Subscribe to the selection change
            selectCurrency.Changed += DropDownSingleSelect_SelectedIndexChanged;

            if (ValidationHelper.GetBoolean(GetValue("ReloadPage"), false))
            {
                // Enable full page postback if necessary according to settings
                ControlsHelper.RegisterPostbackControl(selectCurrency);
            }

            // Preselect currency
            if (RequestHelper.IsPostBack())
            {
                SelectCurrency();
            }
            else
            {
                selectCurrency.SelectedID = ShoppingCart.ShoppingCartCurrencyID;
            }
        }
    }

    #endregion
}