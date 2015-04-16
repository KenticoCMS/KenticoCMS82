using System;
using System.Text;
using System.Web.UI.WebControls;

using CMS.Ecommerce;
using CMS.EcommerceProvider;
using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.Localization;
using CMS.SiteProvider;
using CMS.EventLog;
using CMS.Globalization;

public partial class CMSModules_Ecommerce_Controls_ShoppingCart_ShoppingCartPreview : ShoppingCartStep
{
    #region "ViewState Constants"

    private const string ORDER_NOTE = "OrderNote";

    #endregion


    private SiteInfo currentSite;
    private int mAddressCount = 1;


    protected void Page_Load(object sender, EventArgs e)
    {
        currentSite = SiteContext.CurrentSite;

        if ((ShoppingCart != null) && (ShoppingCart.CountryID == 0) && (currentSite != null))
        {
            string countryName = ECommerceSettings.DefaultCountryName(currentSite.SiteName);
            CountryInfo ci = CountryInfoProvider.GetCountryInfo(countryName);
            ShoppingCart.CountryID = (ci != null) ? ci.CountryID : 0;
        }

        ShoppingCartControl.ButtonNext.Text = GetString("Ecommerce.OrderPreview.NextButtonText");

        // Addresses initialization
        FillBillingAddressForm(ShoppingCart.ShoppingCartBillingAddress);
        FillShippingAddressForm(ShoppingCart.ShoppingCartShippingAddress);

        // Load company address
        if (ShoppingCart.ShoppingCartCompanyAddress != null)
        {
            lblCompany.Text = OrderInfoProvider.GetAddress(ShoppingCart.ShoppingCartCompanyAddress);
            mAddressCount++;
            tdCompanyAddress.Visible = true;
        }
        else
        {
            tdCompanyAddress.Visible = false;
        }

        // Enable sending order notifications when creating order from CMSDesk
        if ((ShoppingCartControl.CheckoutProcessType == CheckoutProcessEnum.CMSDeskOrder) ||
            ShoppingCartControl.CheckoutProcessType == CheckoutProcessEnum.CMSDeskCustomer)
        {
            chkSendEmail.Visible = true;
            chkSendEmail.Checked = ECommerceSettings.SendOrderNotification(currentSite.SiteName);
            chkSendEmail.Text = GetString("ShoppingCartPreview.SendNotification");
        }

        // Show tax registration ID and organization ID
        InitializeIDs();

        // shopping cart content table initialization
        gridData.Columns[4].HeaderText = GetString("Ecommerce.ShoppingCartContent.SKUName");
        gridData.Columns[5].HeaderText = GetString("Ecommerce.ShoppingCartContent.SKUUnits");
        gridData.Columns[6].HeaderText = GetString("Ecommerce.ShoppingCartContent.UnitPrice");
        gridData.Columns[7].HeaderText = GetString("Ecommerce.ShoppingCartContent.UnitDiscount");
        gridData.Columns[8].HeaderText = GetString("Ecommerce.ShoppingCartContent.Tax");
        gridData.Columns[9].HeaderText = GetString("Ecommerce.ShoppingCartContent.Subtotal");

        // Product tax summary table initialization
        gridTaxSummary.Columns[0].HeaderText = GetString("Ecommerce.CartContent.TaxDisplayName");
        gridTaxSummary.Columns[1].HeaderText = GetString("Ecommerce.CartContent.TaxSummary");

        // Shipping tax summary table initialization
        gridShippingTaxSummary.Columns[0].HeaderText = GetString("com.CartContent.ShippingTaxDisplayName");
        gridShippingTaxSummary.Columns[1].HeaderText = GetString("Ecommerce.CartContent.TaxSummary");

        ReloadData();

        // Order note initialization
        if (!ShoppingCartControl.IsCurrentStepPostBack)
        {
            // Try to select payment from ViewState first
            object viewStateValue = ShoppingCartControl.GetTempValue(ORDER_NOTE);
            txtNote.Text = (viewStateValue != null) ? Convert.ToString(viewStateValue) : ShoppingCart.ShoppingCartNote;
        }

        // Display/Hide column with applied discount
        gridData.Columns[7].Visible = ShoppingCart.IsItemDiscountApplied;

        if (mAddressCount == 2)
        {
            tblAddressPreview.Attributes["class"] = "AddressPreviewWithTwoColumns";
        }
        else if (mAddressCount == 3)
        {
            tblAddressPreview.Attributes["class"] = "AddressPreviewWithThreeColumns";
        }
    }


    protected void Page_Prerender(object sender, EventArgs e)
    {
        // Hide columns with identifiers
        gridData.Columns[0].Visible = false;
        gridData.Columns[1].Visible = false;
        gridData.Columns[2].Visible = false;
        gridData.Columns[3].Visible = false;

        // Disable default button in the order preview to 
        // force approval of the order by mouse click
        if (ShoppingCartControl.ShoppingCartContainer != null)
        {
            ShoppingCartControl.ShoppingCartContainer.DefaultButton = "";
        }

        // Display/hide error message
        lblError.Visible = !string.IsNullOrEmpty(lblError.Text.Trim());
    }


    protected void ReloadData()
    {
        // Recalculate shopping cart
        ShoppingCartInfoProvider.EvaluateShoppingCart(ShoppingCart);

        gridData.DataSource = ShoppingCart.ContentTable;
        gridData.DataBind();

        gridTaxSummary.DataSource = ShoppingCart.ContentTaxesTable;
        gridTaxSummary.DataBind();

        gridShippingTaxSummary.DataSource = ShoppingCart.ShippingTaxesTable;
        gridShippingTaxSummary.DataBind();

        // Show order related discounts
        plcMultiBuyDiscountArea.Visible = ShoppingCart.OrderRelatedDiscountSummaryItems.Count > 0;
        ShoppingCart.OrderRelatedDiscountSummaryItems.ForEach(d =>
        {
            plcOrderRelatedDiscountNames.Controls.Add(new LocalizedLabel { Text = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(d.Name)) + ":", EnableViewState = false });
            plcOrderRelatedDiscountNames.Controls.Add(new Literal { Text = "<br />", EnableViewState = false });
            plcOrderRelatedDiscountValues.Controls.Add(new Label { Text = "- " + CurrencyInfoProvider.GetFormattedPrice(d.Value, ShoppingCart.Currency), EnableViewState = false });
            plcOrderRelatedDiscountValues.Controls.Add(new Literal { Text = "<br />", EnableViewState = false });
        });

        // shipping option, payment method initialization
        InitPaymentShipping();
    }


    /// <summary>
    /// Fills billing address form.
    /// </summary>
    /// <param name="address">Billing address</param>
    protected void FillBillingAddressForm(IAddress address)
    {
        lblBill.Text = OrderInfoProvider.GetAddress(address);
    }


    /// <summary>
    /// Fills shipping address form.
    /// </summary>
    /// <param name="address">Shipping address</param>
    protected void FillShippingAddressForm(IAddress address)
    {
        lblShip.Text = OrderInfoProvider.GetAddress(address);
    }


    /// <summary>
    /// Back button actions.
    /// </summary>
    public override void ButtonBackClickAction()
    {
        // Save the values to ShoppingCart ViewState
        ShoppingCartControl.SetTempValue(ORDER_NOTE, txtNote.Text);

        base.ButtonBackClickAction();
    }


    /// <summary>
    /// Validates shopping cart content.
    /// </summary>    
    public override bool IsValid()
    {
        // Force loading current values         
        ShoppingCartInfoProvider.EvaluateShoppingCart(ShoppingCart);

        // Check inventory
        var checkResult = ShoppingCartInfoProvider.CheckShoppingCart(ShoppingCart);

        if (checkResult.CheckFailed)
        {
            lblError.Text = checkResult.GetHTMLFormattedMessage();

            return false;
        }

        // Check if cart contains at least one product
        if (ShoppingCart.IsEmpty)
        {
            lblError.Text = GetString("com.checkout.cartisempty");

            return false;
        }
        
        return true;
    }


    /// <summary>
    /// Saves order information from ShoppingCartInfo object to database as new order.
    /// </summary>
    public override bool ProcessStep()
    {
        // Load first step if there is no address
        if (ShoppingCart.ShoppingCartBillingAddress == null)
        {
            ShoppingCartControl.LoadStep(0);
            return false;
        }

        // Check if customer is enabled
        if ((ShoppingCart.Customer != null) && (!ShoppingCart.Customer.CustomerEnabled))
        {
            lblError.Text = GetString("ecommerce.cartcontent.customerdisabled");
            return false;
        }

        // Deal with order note
        ShoppingCartControl.SetTempValue(ORDER_NOTE, null);
        ShoppingCart.ShoppingCartNote = txtNote.Text.Trim();

        try
        {
            // Set order culture
            ShoppingCart.ShoppingCartCulture = LocalizationContext.PreferredCultureCode;

            // Update customer preferences
            CustomerInfoProvider.SetCustomerPreferredSettings(ShoppingCart);

            // Create order
            ShoppingCartInfoProvider.SetOrder(ShoppingCart);
        }
        catch (Exception ex)
        {
            // Show error
            lblError.Text = GetString("Ecommerce.OrderPreview.ErrorOrderSave");

            // Log exception
            EventLogProvider.LogException("Shopping cart", "SAVEORDER", ex, ShoppingCart.ShoppingCartSiteID);
            return false;
        }

        if (!ShoppingCartControl.IsInternalOrder)
        {
            // Track order items conversions
            ECommerceHelper.TrackOrderItemsConversions(ShoppingCart);

            // Track order conversion        
            string name = ShoppingCartControl.OrderTrackConversionName;
            ECommerceHelper.TrackOrderConversion(ShoppingCart, name);
        }

        // Track order activity
        string siteName = SiteContext.CurrentSiteName;

        if (LogActivityForCustomer)
        {
            ShoppingCartControl.TrackActivityPurchasedProducts(ShoppingCart, siteName, ContactID);
            ShoppingCartControl.TrackActivityPurchase(ShoppingCart.OrderId, ContactID,
                                                      SiteContext.CurrentSiteName, RequestContext.CurrentRelativePath,
                                                      ShoppingCart.TotalPriceInMainCurrency, CurrencyInfoProvider.GetFormattedPrice(ShoppingCart.TotalPriceInMainCurrency,
                                                                                                                                           CurrencyInfoProvider.GetMainCurrency(SiteContext.CurrentSiteID)));
        }

        // Raise finish order event
        ShoppingCartControl.RaiseOrderCompletedEvent();

        // When in CMSDesk
        if (ShoppingCartControl.IsInternalOrder)
        {
            if (chkSendEmail.Checked)
            {
                // Send order notification emails
                OrderInfoProvider.SendOrderNotificationToAdministrator(ShoppingCart);
                OrderInfoProvider.SendOrderNotificationToCustomer(ShoppingCart);
            }
        }
        // When on the live site
        else if (ECommerceSettings.SendOrderNotification(SiteContext.CurrentSite.SiteName))
        {
            // Send order notification emails
            OrderInfoProvider.SendOrderNotificationToAdministrator(ShoppingCart);
            OrderInfoProvider.SendOrderNotificationToCustomer(ShoppingCart);
        }

        return true;
    }


    protected void InitPaymentShipping()
    {
        if (currentSite != null)
        {
            // get shipping option name
            ShippingOptionInfo shippingObj = ShoppingCart.ShippingOption;
            if (shippingObj != null)
            {
                mAddressCount++;
                //plcShippingAddress.Visible = true;
                tdShippingAddress.Visible = true;
                plcShipping.Visible = true;
                plcShippingOption.Visible = true;
                lblShippingOptionValue.Text = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(shippingObj.ShippingOptionDisplayName));
                lblShippingValue.Text = CurrencyInfoProvider.GetFormattedPrice(ShoppingCart.TotalShipping, ShoppingCart.Currency);
            }
            else
            {
                //plcShippingAddress.Visible = false;
                tdShippingAddress.Visible = false;
                plcShippingOption.Visible = false;
                plcShipping.Visible = false;
            }
        }

        // get payment method name
        PaymentOptionInfo paymentObj = PaymentOptionInfoProvider.GetPaymentOptionInfo(ShoppingCart.ShoppingCartPaymentOptionID);
        if (paymentObj != null)
        {
            lblPaymentMethodValue.Text = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(paymentObj.PaymentOptionDisplayName));
        }


        // total price initialization
        lblTotalPriceValue.Text = CurrencyInfoProvider.GetFormattedPrice(ShoppingCart.RoundedTotalPrice, ShoppingCart.Currency);
    }


    /// <summary>
    /// Displays product error message in shopping cart content table.
    /// </summary>
    /// <param name="skuErrorMessage">Error message to be displayed</param>
    protected string DisplaySKUErrorMessage(object skuErrorMessage)
    {
        string err = ValidationHelper.GetString(skuErrorMessage, "");
        if (err != "")
        {
            return "<br /><span class=\"ItemsNotAvailable\">" + err + "</span>";
        }
        return "";
    }


    /// <summary>
    /// Initializes tax registration ID and organization ID.
    /// </summary>
    protected void InitializeIDs()
    {
        SiteInfo si = SiteContext.CurrentSite;
        if (si != null)
        {
            if ((ECommerceSettings.ShowOrganizationID(si.SiteName)) && (ShoppingCart.Customer != null) && (ShoppingCart.Customer.CustomerOrganizationID != string.Empty))
            {
                // Initialize organization ID
                plcIDs.Visible = true;
                lblOrganizationIDVal.Text = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(ShoppingCart.Customer.CustomerOrganizationID));
            }
            else
            {
                lblOrganizationID.Visible = false;
                lblOrganizationIDVal.Visible = false;
            }
            if ((ECommerceSettings.ShowTaxRegistrationID(si.SiteName)) && (ShoppingCart.Customer != null) && (ShoppingCart.Customer.CustomerTaxRegistrationID != string.Empty))
            {
                // Initialize tax registration ID
                plcIDs.Visible = true;
                lblTaxRegistrationIDVal.Text = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(ShoppingCart.Customer.CustomerTaxRegistrationID));
            }
            else
            {
                lblTaxRegistrationID.Visible = false;
                lblTaxRegistrationIDVal.Visible = false;
            }
        }
    }


    /// <summary>
    /// Returns formatted value string.
    /// </summary>
    /// <param name="value">Value to format</param>
    protected string GetFormattedValue(object value)
    {
        double price = ValidationHelper.GetDouble(value, 0);
        return CurrencyInfoProvider.GetFormattedValue(price, ShoppingCart.Currency);
    }


    /// <summary>
    /// Returns formatted and localized SKU name.
    /// </summary>
    /// <param name="value">SKU name</param>
    /// <param name="isProductOption">Indicates if cart item is product option</param>
    /// <param name="isBundleItem">Indicates if cart item is bundle item</param>
    /// <param name="itemText">Text parameter of product. Will be appended in quotes at the end of SKU name</param>
    protected string GetSKUName(object value, object isProductOption, object isBundleItem, object itemText)
    {
        string name = ResHelper.LocalizeString((string)value);
        string text = itemText as string;

        // If it is a product option or bundle item
        if (ValidationHelper.GetBoolean(isProductOption, false) || ValidationHelper.GetBoolean(isBundleItem, false))
        {
            StringBuilder skuName = new StringBuilder("<span style=\"font-size:90%\"> - ");
            skuName.Append(HTMLHelper.HTMLEncode(name));

            if (!string.IsNullOrEmpty(text))
            {
                skuName.Append(" '" + HTMLHelper.HTMLEncode(ResHelper.LocalizeString(text)) + "'");
            }

            skuName.Append("</span>");
            return skuName.ToString();
        }
        // If it is a parent product
        else
        {
            return HTMLHelper.HTMLEncode(name);
        }
    }
}