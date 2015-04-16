using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;

using CMS.Core;
using CMS.Ecommerce;
using CMS.EcommerceProvider;
using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.WebAnalytics;
using CMS.EventLog;
using CMS.Globalization;

public partial class CMSModules_Ecommerce_Controls_ShoppingCart_ShoppingCartContent : ShoppingCartStep
{
    #region "Variables"

    protected Button btnReload = null;
    protected Button btnAddProduct = null;
    protected HiddenField hidProductID = null;
    protected HiddenField hidQuantity = null;
    protected HiddenField hidOptions = null;

    protected bool? mEnableEditMode = null;
    protected bool checkInventory = false;
    protected bool? mCanReadProducts = null;

    #endregion


    #region Properties

    /// <summary>
    /// Indicates whether there are another checkout process steps after the current step, except payment.
    /// </summary>
    private bool ExistAnotherStepsExceptPayment
    {
        get
        {
            return (ShoppingCartControl.CurrentStepIndex + 2 <= ShoppingCartControl.CheckoutProcessSteps.Count - 1);
        }
    }


    /// <summary>
    /// Indicates if current user is authorized to read product details.
    /// </summary>
    private bool CanReadProducts
    {
        get
        {
            if (!mCanReadProducts.HasValue)
            {
                mCanReadProducts = ECommerceHelper.IsUserAuthorizedForPermission(EcommercePermissions.PRODUCTS_READ, SiteContext.CurrentSiteName, CurrentUser);
            }

            return mCanReadProducts.Value;
        }
    }


    /// <summary>
    /// Indicates that order exists and is paid.
    /// </summary>
    private bool OrderIsPaid
    {
        get
        {
            return (ShoppingCart.Order != null) && ShoppingCart.Order.OrderIsPaid;
        }
    }

    #endregion


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        selectCurrency.Changed += (sender, args) => btnUpdate_Click1(null, null);

        // Add product button
        btnAddProduct = new CMSButton();
        btnAddProduct.Attributes["style"] = "display: none";
        Controls.Add(btnAddProduct);
        btnAddProduct.Click += btnAddProduct_Click;

        // Add the hidden fields for productId, quantity and product options
        hidProductID = new HiddenField { ID = "hidProductID" };
        Controls.Add(hidProductID);

        hidQuantity = new HiddenField { ID = "hidQuantity" };
        Controls.Add(hidQuantity);

        hidOptions = new HiddenField { ID = "hidOptions" };
        Controls.Add(hidOptions);
    }


    protected override void OnPreRender(EventArgs e)
    {
        // Register add product script
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "AddProductScript",
                                               ScriptHelper.GetScript(
                                                   "function setProduct(val) { document.getElementById('" + hidProductID.ClientID + "').value = val; } \n" +
                                                   "function setQuantity(val) { document.getElementById('" + hidQuantity.ClientID + "').value = val; } \n" +
                                                   "function setOptions(val) { document.getElementById('" + hidOptions.ClientID + "').value = val; } \n" +
                                                   "function setPrice(val) { document.getElementById('" + hdnPrice.ClientID + "').value = val; } \n" +
                                                   "function setIsPrivate(val) { document.getElementById('" + hdnIsPrivate.ClientID + "').value = val; } \n" +
                                                   "function AddProduct(productIDs, quantities, options, price, isPrivate) { \n" +
                                                   "setProduct(productIDs); \n" +
                                                   "setQuantity(quantities); \n" +
                                                   "setOptions(options); \n" +
                                                   "setPrice(price); \n" +
                                                   "setIsPrivate(isPrivate); \n" +
                                                   Page.ClientScript.GetPostBackEventReference(btnAddProduct, null) +
                                                   ";} \n" +
                                                   "function RefreshCart() {" +
                                                   Page.ClientScript.GetPostBackEventReference(btnAddProduct, null) +
                                                   ";} \n"
                                                   ));

        // Register dialog script
        ScriptHelper.RegisterDialogScript(Page);

        // Hide columns with identifiers
        gridData.Columns[0].Visible = false;
        gridData.Columns[1].Visible = false;
        gridData.Columns[2].Visible = false;
        gridData.Columns[3].Visible = false;

        // Hide actions column
        gridData.Columns[5].Visible = (ShoppingCartControl.CheckoutProcessType == CheckoutProcessEnum.CMSDeskOrderItems) ||
                                      (ShoppingCartControl.CheckoutProcessType == CheckoutProcessEnum.CMSDeskOrder);

        // Disable specific controls
        if (!Enabled)
        {
            lnkNewItem.Enabled = false;
            lnkNewItem.OnClientClick = "";
            selectCurrency.Enabled = false;
            btnEmpty.Enabled = false;
            btnUpdate.Enabled = false;
            txtCoupon.Enabled = false;
            chkSendEmail.Enabled = false;
        }

        // Show/Hide dropdownlist with currencies
        pnlCurrency.Visible &= (selectCurrency.HasData && selectCurrency.DropDownSingleSelect.Items.Count > 1);

        // Check session parameters for inventory check
        if (ValidationHelper.GetBoolean(SessionHelper.GetValue("checkinventory"), false))
        {
            checkInventory = true;
            SessionHelper.Remove("checkinventory");
        }

        // Check inventory
        if (checkInventory)
        {
            ShoppingCartCheckResult checkResult = ShoppingCartInfoProvider.CheckShoppingCart(ShoppingCart);

            if (checkResult.CheckFailed)
            {
                lblError.Text = checkResult.GetHTMLFormattedMessage();
            }
        }

        // Display messages if required
        lblError.Visible = !string.IsNullOrEmpty(lblError.Text.Trim());
        lblInfo.Visible = !string.IsNullOrEmpty(lblInfo.Text.Trim());

        base.OnPreRender(e);
    }


    protected override void OnLoad(EventArgs e)
    {
        // Show message when reordering and not all reordered product were added to cart
        if (!URLHelper.IsPostback() && QueryHelper.GetBoolean("notallreordered", false))
        {
            lblError.Text = GetString("com.notallreordered");
        }

        if ((ShoppingCart != null) && IsLiveSite)
        {
            // Get order information
            OrderInfo oi = OrderInfoProvider.GetOrderInfo(ShoppingCart.OrderId);
            // If order is paid
            if ((oi != null) && (oi.OrderIsPaid))
            {
                // Clean shopping cart if paid order cart is still in customers current cart on LS
                ShoppingCartControl.CleanUpShoppingCart();
            }
        }

        if ((ShoppingCart != null) && (ShoppingCart.CountryID == 0) && (SiteContext.CurrentSite != null))
        {
            string countryName = ECommerceSettings.DefaultCountryName(SiteContext.CurrentSiteName);
            CountryInfo ci = CountryInfoProvider.GetCountryInfo(countryName);
            ShoppingCart.CountryID = (ci != null) ? ci.CountryID : 0;

            // Set currency selectors site ID
            selectCurrency.SiteID = ShoppingCart.ShoppingCartSiteID;
        }

        lnkNewItem.Text = GetString("ecommerce.shoppingcartcontent.additem");
        btnUpdate.Text = GetString("ecommerce.shoppingcartcontent.btnupdate");
        btnEmpty.Text = GetString("ecommerce.shoppingcartcontent.btnempty");
        btnEmpty.OnClientClick = string.Format("return confirm({0});", ScriptHelper.GetString(GetString("ecommerce.shoppingcartcontent.emptyconfirm")));

        // Add new product dialog
        string addProductUrl = UIContextHelper.GetElementDialogUrl(ModuleName.ECOMMERCE, "order.addorderitems", 0, GetCMSDeskShoppingCartSessionNameQuery());
        lnkNewItem.OnClientClick = ScriptHelper.GetModalDialogScript(addProductUrl, "addproduct", 1000, 620);

        gridData.Columns[4].HeaderText = GetString("general.remove");
        gridData.Columns[5].HeaderText = GetString("ecommerce.shoppingcartcontent.actions");
        gridData.Columns[6].HeaderText = GetString("ecommerce.shoppingcartcontent.skuname");
        gridData.Columns[7].HeaderText = GetString("ecommerce.shoppingcartcontent.skuunits");
        gridData.Columns[8].HeaderText = GetString("ecommerce.shoppingcartcontent.unitprice");
        gridData.Columns[9].HeaderText = GetString("ecommerce.shoppingcartcontent.unitdiscount");
        gridData.Columns[10].HeaderText = GetString("ecommerce.shoppingcartcontent.tax");
        gridData.Columns[11].HeaderText = GetString("ecommerce.shoppingcartcontent.subtotal");
        gridData.RowDataBound += gridData_RowDataBound;

        // Hide "add product" action for live site order
        if (!ShoppingCartControl.IsInternalOrder)
        {
            pnlNewItem.Visible = false;

            ShoppingCartControl.ButtonBack.Text = GetString("ecommerce.cartcontent.buttonbacktext");
            ShoppingCartControl.ButtonBack.ButtonStyle = ButtonStyle.Default;
            ShoppingCartControl.ButtonNext.Text = GetString("ecommerce.cartcontent.buttonnexttext");

            if (!ShoppingCartControl.IsCurrentStepPostBack)
            {
                // Get shopping cart item parameters from URL
                ShoppingCartItemParameters itemParams = ShoppingCartItemParameters.GetShoppingCartItemParameters();

                // Set item in the shopping cart
                AddProducts(itemParams);
            }
        }

        // Set sending order notification when editing existing order according to the site settings
        if (ShoppingCartControl.CheckoutProcessType == CheckoutProcessEnum.CMSDeskOrderItems)
        {
            if (!ShoppingCartControl.IsCurrentStepPostBack)
            {
                if (!string.IsNullOrEmpty(ShoppingCart.SiteName))
                {
                    chkSendEmail.Checked = ECommerceSettings.SendOrderNotification(ShoppingCart.SiteName);
                }
            }
            // Show send email checkbox
            chkSendEmail.Visible = true;
            chkSendEmail.Text = GetString("shoppingcartcontent.sendemail");

            // Setup buttons
            ShoppingCartControl.ButtonBack.Visible = false;
            ShoppingCart.CheckAvailableItems = false;
            ShoppingCartControl.ButtonNext.Text = GetString("general.next");

            if ((!ExistAnotherStepsExceptPayment) && (ShoppingCartControl.PaymentGatewayProvider == null))
            {
                ShoppingCartControl.ButtonNext.Text = GetString("general.ok");
            }
        }

        // Fill dropdownlist
        if (!ShoppingCartControl.IsCurrentStepPostBack)
        {
            if (!ShoppingCart.IsEmpty || ShoppingCartControl.IsInternalOrder)
            {
                if (ShoppingCart.ShoppingCartCurrencyID == 0)
                {
                    // Select customer preferred currency                    
                    if (ShoppingCart.Customer != null)
                    {
                        CustomerInfo customer = ShoppingCart.Customer;
                        ShoppingCart.ShoppingCartCurrencyID = (customer.CustomerUser != null) ? customer.CustomerUser.GetUserPreferredCurrencyID(SiteContext.CurrentSiteName) : 0;
                    }
                }

                if (ShoppingCart.ShoppingCartCurrencyID == 0)
                {
                    if (SiteContext.CurrentSite != null)
                    {
                        var mainCurrency = CurrencyInfoProvider.GetMainCurrency(SiteContext.CurrentSiteID);
                        if (mainCurrency != null)
                        {
                            ShoppingCart.ShoppingCartCurrencyID = mainCurrency.CurrencyID;
                        }
                    }
                }

                selectCurrency.SelectedID = ShoppingCart.ShoppingCartCurrencyID;

                // Fill textbox with discount coupon code
                if (!String.IsNullOrEmpty(ShoppingCart.ShoppingCartCouponCode))
                {
                    txtCoupon.Text = ShoppingCart.ShoppingCartCouponCode;
                }
            }

            ReloadData();
        }

        // Check if customer is enabled
        if ((ShoppingCart.Customer != null) && (!ShoppingCart.Customer.CustomerEnabled))
        {
            HideCartContent(GetString("ecommerce.cartcontent.customerdisabled"));
        }

        // Ensure order currency in selector
        if ((ShoppingCartControl.CheckoutProcessType == CheckoutProcessEnum.CMSDeskOrderItems) && (ShoppingCart.Order != null))
        {
            selectCurrency.AdditionalItems += ShoppingCart.Order.OrderCurrencyID + ";";
        }

        // Turn on available items checking after content is loaded
        ShoppingCart.CheckAvailableItems = true;

        base.OnLoad(e);
    }


    private void gridData_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            // Set enabled for order item units editing
            e.Row.Cells[7].Enabled = Enabled;
        }
    }


    protected void btnUpdate_Click1(object sender, EventArgs e)
    {
        if (ShoppingCart != null)
        {
            // Do not update order if it is already paid
            if (OrderIsPaid)
            {
                return;
            }

            ShoppingCart.ShoppingCartCurrencyID = ValidationHelper.GetInteger(selectCurrency.SelectedID, 0);

            // Skip if method was called by btnAddProduct
            if (sender != btnAddProduct)
            {
                foreach (GridViewRow row in gridData.Rows)
                {
                    // Get shopping cart item Guid
                    Guid cartItemGuid = ValidationHelper.GetGuid(((Label)row.Cells[1].Controls[1]).Text, Guid.Empty);

                    // Try to find shopping cart item in the list
                    ShoppingCartItemInfo cartItem = ShoppingCart.GetShoppingCartItem(cartItemGuid);
                    if (cartItem != null)
                    {
                        // If product and its product options should be removed
                        if (((CMSCheckBox)row.Cells[4].Controls[1]).Checked && (sender != null))
                        {
                            // Remove product and its product option from list
                            ShoppingCart.RemoveShoppingCartItem(cartItemGuid);

                            if (!ShoppingCartControl.IsInternalOrder)
                            {
                                // Log activity
                                if (!cartItem.IsProductOption && !cartItem.IsBundleItem)
                                {
                                    Activity activity = new ActivityProductRemovedFromShoppingCart(cartItem, ResHelper.LocalizeString(cartItem.SKU.SKUName), ContactID, AnalyticsContext.ActivityEnvironmentVariables);
                                    activity.Log();
                                }

                                // Delete product from database
                                ShoppingCartItemInfoProvider.DeleteShoppingCartItemInfo(cartItem);
                            }
                        }
                        // If product units has changed
                        else if (!cartItem.IsProductOption)
                        {
                            // Get number of units
                            int itemUnits = ValidationHelper.GetInteger(((TextBox)(row.Cells[7].Controls[1])).Text.Trim(), 0);
                            if ((itemUnits > 0) && (cartItem.CartItemUnits != itemUnits))
                            {
                                // Update units of the parent product
                                ShoppingCartItemInfoProvider.UpdateShoppingCartItemUnits(cartItem, itemUnits);
                            }
                        }
                    }
                }
            }

            CheckDiscountCoupon();

            // Recalculate shopping cart
            ShoppingCartInfoProvider.EvaluateShoppingCart(ShoppingCart);

            ReloadData();

            if ((ShoppingCart.ShoppingCartDiscountCouponID > 0) && (!ShoppingCart.IsDiscountCouponApplied))
            {
                // Discount coupon code is valid but not applied to any product of the shopping cart
                lblError.Text = GetString("shoppingcart.discountcouponnotapplied");
            }

            // Inventory should be checked
            checkInventory = true;
        }
    }


    protected void btnEmpty_Click1(object sender, EventArgs e)
    {
        if (ShoppingCart != null)
        {
            // Do not empty cart if order is paid
            if (OrderIsPaid)
            {
                return;
            }

            // Log activity "product removed" for all items in shopping cart
            string siteName = SiteContext.CurrentSiteName;
            if (!ShoppingCartControl.IsInternalOrder)
            {
                ShoppingCartControl.TrackActivityAllProductsRemovedFromShoppingCart(ShoppingCart, siteName, ContactID);
            }

            ShoppingCartInfoProvider.EmptyShoppingCart(ShoppingCart);

            ReloadData();
        }
    }


    /// <summary>
    /// Validates this step.
    /// </summary>
    public override bool IsValid()
    {
        // Check modify permissions
        if (ShoppingCartControl.CheckoutProcessType == CheckoutProcessEnum.CMSDeskOrderItems)
        {
            // Check 'ModifyOrders' permission
            if (!ECommerceContext.IsUserAuthorizedForPermission("ModifyOrders"))
            {
                CMSEcommercePage.RedirectToAccessDenied("CMS.Ecommerce", "EcommerceModify OR ModifyOrders");
            }
        }

        // Allow to go to the next step only if shopping cart contains some products
        bool IsValid = !ShoppingCart.IsEmpty;

        if (!IsValid)
        {
            HideCartContent();
        }

        if (ShoppingCart.IsCreatedFromOrder)
        {
            IsValid = true;
        }

        if (!IsValid)
        {
            lblError.Text = GetString("ecommerce.error.insertsomeproducts");
        }

        return IsValid;
    }


    /// <summary>
    /// Process this step.
    /// </summary>
    public override bool ProcessStep()
    {
        // Do not process step if order is paid
        if (OrderIsPaid)
        {
            return false;
        }

        // Shopping cart units are already saved in database (on "Update" or on "btnAddProduct_Click" actions) 
        bool isOK = false;

        if (ShoppingCart != null)
        {
            // Reload data
            ReloadData();

            // Check available items before "Check out"
            ShoppingCartCheckResult checkResult = ShoppingCartInfoProvider.CheckShoppingCart(ShoppingCart);

            if (checkResult.CheckFailed)
            {
                lblError.Text = checkResult.GetHTMLFormattedMessage();
            }
            else if (ShoppingCartControl.CheckoutProcessType == CheckoutProcessEnum.CMSDeskOrderItems)
            {
                // Indicates whether order saving process is successful
                isOK = true;

                try
                {
                    ShoppingCartInfoProvider.SetOrder(ShoppingCart);
                }
                catch (Exception ex)
                {
                    // Log exception
                    EventLogProvider.LogException("Shopping cart", "SAVEORDER", ex, ShoppingCart.ShoppingCartSiteID);
                    isOK = false;
                }

                if (isOK)
                {
                    lblInfo.Text = GetString("general.changessaved");

                    // Send order notification when editing existing order
                    if (ShoppingCartControl.CheckoutProcessType == CheckoutProcessEnum.CMSDeskOrderItems)
                    {
                        if (chkSendEmail.Checked)
                        {
                            OrderInfoProvider.SendOrderNotificationToAdministrator(ShoppingCart);
                            OrderInfoProvider.SendOrderNotificationToCustomer(ShoppingCart);
                        }
                    }
                    // Send order notification emails when on the live site
                    else if (ECommerceSettings.SendOrderNotification(SiteContext.CurrentSite.SiteName))
                    {
                        OrderInfoProvider.SendOrderNotificationToAdministrator(ShoppingCart);
                        OrderInfoProvider.SendOrderNotificationToCustomer(ShoppingCart);
                    }
                }
                else
                {
                    lblError.Text = GetString("ecommerce.orderpreview.errorordersave");
                }
            }
            // Go to the next step
            else
            {
                // Save other options
                if (!ShoppingCartControl.IsInternalOrder)
                {
                    ShoppingCartInfoProvider.SetShoppingCartInfo(ShoppingCart);
                }

                isOK = true;
            }
        }

        return isOK;
    }


    private void btnAddProduct_Click(object sender, EventArgs e)
    {
        // Do not add item if order is paid
        if (OrderIsPaid)
        {
            return;
        }

        // Get strings with productIDs and quantities separated by ';'
        string productIDs = ValidationHelper.GetString(hidProductID.Value, "");
        string quantities = ValidationHelper.GetString(hidQuantity.Value, "");
        string options = ValidationHelper.GetString(hidOptions.Value, "");
        double price = ValidationHelper.GetDouble(hdnPrice.Value, -1);
        bool isPrivate = ValidationHelper.GetBoolean(hdnIsPrivate.Value, false);

        // Add new products to shopping cart
        if ((productIDs != "") && (quantities != ""))
        {
            int[] arrID = ValidationHelper.GetIntegers(productIDs.TrimEnd(';').Split(';'), 0);
            int[] arrQuant = ValidationHelper.GetIntegers(quantities.TrimEnd(';').Split(';'), 0);
            int[] intOptions = ValidationHelper.GetIntegers(options.Split(','), 0);

            // Check site binding
            if (!CheckSiteBinding(arrID) || !CheckSiteBinding(intOptions))
            {
                return;
            }

            lblError.Text = "";

            for (int i = 0; i < arrID.Length; i++)
            {
                int skuId = arrID[i];

                SKUInfo skuInfo = SKUInfoProvider.GetSKUInfo(skuId);
                if ((skuInfo != null) && !skuInfo.IsProductOption)
                {
                    int quantity = arrQuant[i];

                    ShoppingCartItemParameters cartItemParams = new ShoppingCartItemParameters(skuId, quantity, intOptions);

                    // If product is donation
                    if (skuInfo.SKUProductType == SKUProductTypeEnum.Donation)
                    {
                        // Get donation properties
                        if (price < 0)
                        {
                            cartItemParams.Price = SKUInfoProvider.GetSKUPrice(skuInfo, ShoppingCart, false, false);
                        }
                        else
                        {
                            cartItemParams.Price = price;
                        }

                        cartItemParams.IsPrivate = isPrivate;
                    }

                    // Add product to the shopping cart
                    ShoppingCart.SetShoppingCartItem(cartItemParams);

                    // Log activity
                    string siteName = SiteContext.CurrentSiteName;
                    if (!ShoppingCartControl.IsInternalOrder)
                    {
                        ShoppingCartControl.TrackActivityProductAddedToShoppingCart(skuId, ResHelper.LocalizeString(skuInfo.SKUName), ContactID, siteName, RequestContext.CurrentRelativePath, quantity);
                    }

                    // Show empty button
                    btnEmpty.Visible = true;
                }
            }
        }

        // Invalidate values
        hidProductID.Value = "";
        hidOptions.Value = "";
        hidQuantity.Value = "";
        hdnPrice.Value = "";

        // Update values in table
        btnUpdate_Click1(btnAddProduct, e);

        // Hide cart content when empty
        if (DataHelper.DataSourceIsEmpty(ShoppingCart.ContentTable))
        {
            HideCartContent();
        }
        else
        {
            // Inventory should be checked
            checkInventory = true;
        }
    }


    /// <summary>
    /// Checks whether skuIds are from shopping cart site.
    /// </summary>
    private bool CheckSiteBinding(IEnumerable<int> skuIds)
    {
        foreach (var skuId in skuIds)
        {
            SKUInfo skuInfo = SKUInfoProvider.GetSKUInfo(skuId);
            if (skuInfo != null)
            {
                // Check if product belongs to site
                if (!skuInfo.IsGlobal && (skuInfo.SKUSiteID != ShoppingCart.ShoppingCartSiteID))
                {
                    return false;
                }
            }
        }

        return true;
    }


    /// <summary>
    /// Checks whether entered discount coupon code is usable for this cart. Returns true if so.
    /// </summary>
    private void CheckDiscountCoupon()
    {
        if (txtCoupon.Text.Trim() != "")
        {
            ShoppingCart.ShoppingCartCouponCode = txtCoupon.Text.Trim();

            if (!ShoppingCart.HasUsableCoupon)
            {
                // Discount coupon is not valid                
                lblError.Text = GetString("ecommerce.error.couponcodeisnotvalid");
            }
        }
        else
        {
            ShoppingCart.ShoppingCartCouponCode = string.Empty;
        }
    }


    // Displays total price
    protected void DisplayTotalPrice()
    {
        pnlPrice.Visible = true;
        lblTotalPriceValue.Text = CurrencyInfoProvider.GetFormattedPrice(ShoppingCart.RoundedTotalPrice, ShoppingCart.Currency);
        lblTotalPrice.Text = GetString("ecommerce.cartcontent.totalpricelabel");

        lblShippingPrice.Text = GetString("ecommerce.cartcontent.shippingpricelabel");
        lblShippingPriceValue.Text = CurrencyInfoProvider.GetFormattedPrice(ShoppingCart.TotalShipping, ShoppingCart.Currency);
    }


    /// <summary>
    /// Sets product in the shopping cart.
    /// </summary>
    /// <param name="itemParams">Shopping cart item parameters</param>
    protected void AddProducts(ShoppingCartItemParameters itemParams)
    {
        // Get main product info
        int productId = itemParams.SKUID;
        int quantity = itemParams.Quantity;

        if ((productId > 0) && (quantity > 0))
        {
            // Check product/options combination
            if (ShoppingCartInfoProvider.CheckNewShoppingCartItems(ShoppingCart, itemParams))
            {
                // Get requested SKU info object from database
                SKUInfo skuObj = SKUInfoProvider.GetSKUInfo(productId);
                if (skuObj != null)
                {
                    // On the live site
                    if (!ShoppingCartControl.IsInternalOrder)
                    {
                        bool updateCart = false;

                        // Assign current shopping cart to current user
                        var ui = MembershipContext.AuthenticatedUser;
                        if (!ui.IsPublic())
                        {
                            ShoppingCart.User = ui;
                            updateCart = true;
                        }

                        // Shopping cart is not saved yet
                        if (ShoppingCart.ShoppingCartID == 0)
                        {
                            updateCart = true;
                        }

                        // Update shopping cart when required
                        if (updateCart)
                        {
                            ShoppingCartInfoProvider.SetShoppingCartInfo(ShoppingCart);
                        }

                        // Set item in the shopping cart
                        ShoppingCartItemInfo product = ShoppingCart.SetShoppingCartItem(itemParams);

                        // Update shopping cart item in database
                        ShoppingCartItemInfoProvider.SetShoppingCartItemInfo(product);

                        // Update product options in database
                        foreach (ShoppingCartItemInfo option in product.ProductOptions)
                        {
                            ShoppingCartItemInfoProvider.SetShoppingCartItemInfo(option);
                        }

                        // Update bundle items in database
                        foreach (ShoppingCartItemInfo bundleItem in product.BundleItems)
                        {
                            ShoppingCartItemInfoProvider.SetShoppingCartItemInfo(bundleItem);
                        }

                        // Track add to shopping cart conversion
                        ECommerceHelper.TrackAddToShoppingCartConversion(product);
                    }
                    // In CMSDesk
                    else
                    {
                        // Set item in the shopping cart
                        ShoppingCart.SetShoppingCartItem(itemParams);
                    }
                }
            }

            // Avoid adding the same product after page refresh
            if (lblError.Text == "")
            {
                string url = RequestContext.CurrentURL;

                if (!string.IsNullOrEmpty(URLHelper.GetUrlParameter(url, "productid")) ||
                    !string.IsNullOrEmpty(URLHelper.GetUrlParameter(url, "quantity")) ||
                    !string.IsNullOrEmpty(URLHelper.GetUrlParameter(url, "options")))
                {
                    // Remove parameters from URL
                    url = URLHelper.RemoveParameterFromUrl(url, "productid");
                    url = URLHelper.RemoveParameterFromUrl(url, "quantity");
                    url = URLHelper.RemoveParameterFromUrl(url, "options");
                    URLHelper.Redirect(url);
                }
            }
        }
    }


    /// <summary>
    /// Hides cart content controls and displays given message.
    /// </summary>
    /// <param name="message">Message to be shown. When null 'Shopping cart is empty' message is shown.</param>
    protected void HideCartContent(string message = null)
    {
        pnlNewItem.Visible = ShoppingCartControl.IsInternalOrder;
        pnlPrice.Visible = false;
        btnEmpty.Visible = false;
        plcCoupon.Visible = false;

        if (!ShoppingCartControl.IsInternalOrder)
        {
            pnlCurrency.Visible = false;
            gridData.Visible = false;
            ShoppingCartControl.ButtonNext.Visible = false;

            message = message ?? GetString("ecommerce.shoppingcartempty");
            lblInfo.Text = message + "<br />";
        }
    }


    /// <summary>
    /// Reloads the form data.
    /// </summary>
    protected void ReloadData()
    {
        //gridData.DataSource = ShoppingCartInfoObj.ShoppingCartContentTable;
        gridData.DataSource = ShoppingCart.ContentTable;

        // Bind data
        gridData.DataBind();

        if (!DataHelper.DataSourceIsEmpty(ShoppingCart.ContentTable))
        {
            // Display total price
            DisplayTotalPrice();

            // Display/hide discount column
            gridData.Columns[9].Visible = ShoppingCart.IsItemDiscountApplied;
        }
        else
        {
            // Hide some items
            HideCartContent();
        }

        // Show order related discounts
        plcMultiBuyDiscountArea.Visible = ShoppingCart.OrderRelatedDiscountSummaryItems.Count > 0;
        ShoppingCart.OrderRelatedDiscountSummaryItems.ForEach(d =>
        {
            plcOrderRelatedDiscountNames.Controls.Add(new LocalizedLabel { Text = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(d.Name)) + ":", EnableViewState = false });
            plcOrderRelatedDiscountNames.Controls.Add(new Literal { Text = "<br />", EnableViewState = false });
            plcOrderRelatedDiscountValues.Controls.Add(new Label { Text = "- " + CurrencyInfoProvider.GetFormattedPrice(d.Value, ShoppingCart.Currency), EnableViewState = false });
            plcOrderRelatedDiscountValues.Controls.Add(new Literal { Text = "<br />", EnableViewState = false });
        });

        //lblMultiBuyDiscountName.Text
        if (!ShoppingCart.IsShippingNeeded)
        {
            plcShippingPrice.Visible = false;
        }
    }


    /// <summary>
    /// Returns price detail link.
    /// </summary>
    protected string GetPriceDetailLink(object value)
    {
        var priceDetailElemHtml = "";

        if ((ShoppingCartControl.EnableProductPriceDetail) && (ShoppingCart.ContentTable != null))
        {
            Guid cartItemGuid = ValidationHelper.GetGuid(value, Guid.Empty);
            if (cartItemGuid != Guid.Empty)
            {
                var query = "itemguid=" + cartItemGuid + GetCMSDeskShoppingCartSessionNameQuery();

                if (IsLiveSite)
                {
                    string liveSiteUrl = URLHelper.ResolveUrl("~/CMSModules/Ecommerce/CMSPages/ShoppingCartSKUPriceDetail.aspx?" + query);

                    priceDetailElemHtml = string.Format("<img src=\"{0}\" onclick=\"{1}\" alt=\"{2}\" class=\"ProductPriceDetailImage\" style=\"cursor:pointer;\" />",
                    GetImageUrl("Design/Controls/UniGrid/Actions/detail.png"),
                    ScriptHelper.GetModalDialogScript(liveSiteUrl, "ProductPriceDetail", 750, 570),
                    GetString("shoppingcart.productpricedetail"));
                }
                else
                {
                    string adminUrl = UIContextHelper.GetElementDialogUrl(ModuleName.ECOMMERCE, "order.productpricedetail", 0, query);

                    var priceDetailButton = new CMSGridActionButton
                    {
                        IconCssClass = "icon-eye",
                        ToolTip = GetString("shoppingcart.productpricedetail"),
                        OnClientClick = ScriptHelper.GetModalDialogScript(adminUrl, "ProductPriceDetail", 750, 570)
                    };

                    priceDetailElemHtml = priceDetailButton.GetRenderedHTML();
                }
            }
        }

        return priceDetailElemHtml;
    }


    /// <summary>
    /// Returns shopping cart session name in cart query parameter.
    /// </summary>
    private string GetCMSDeskShoppingCartSessionNameQuery()
    {
        var cartSessionName = "";

        switch (ShoppingCartControl.CheckoutProcessType)
        {
            case CheckoutProcessEnum.CMSDeskOrder:
                cartSessionName = "CMSDeskNewOrderShoppingCart";
                break;
            case CheckoutProcessEnum.CMSDeskCustomer:
                cartSessionName = "CMSDeskNewCustomerOrderShoppingCart";
                break;
            case CheckoutProcessEnum.CMSDeskOrderItems:
                cartSessionName = "CMSDeskOrderItemsShoppingCart";
                break;
        }

        return (!string.IsNullOrEmpty(cartSessionName)) ? "&cart=" + cartSessionName : "";
    }


    public override void ButtonBackClickAction()
    {
        if ((!ShoppingCartControl.IsInternalOrder) && (ShoppingCartControl.CurrentStepIndex == 0))
        {
            // Continue shopping
            URLHelper.Redirect(ShoppingCartControl.PreviousPageUrl);
        }
        else
        {
            // Standard action
            base.ButtonBackClickAction();
        }
    }


    #region "Binding methods"

    /// <summary>
    /// Returns formatted currency value.
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
    /// <param name="skuId">SKU ID</param>
    /// <param name="skuSiteId">SKU site ID</param>
    /// <param name="cartItemIsPrivate">SKU name</param>
    /// <param name="itemText">Indicates if cart item is product option</param>
    /// <param name="productType">Indicates if cart item is bundle item</param>
    protected string GetSKUName(object skuId, object skuSiteId, object value, object isProductOption, object isBundleItem, object cartItemIsPrivate, object itemText, object productType)
    {
        string name = ResHelper.LocalizeString((string)value);
        bool isPrivate = ValidationHelper.GetBoolean(cartItemIsPrivate, false);
        string text = itemText as string;
        StringBuilder skuName = new StringBuilder();
        SKUProductTypeEnum type = ValidationHelper.GetString(productType, "").ToEnum<SKUProductTypeEnum>();

        // If it is a product option or bundle item
        if (ValidationHelper.GetBoolean(isProductOption, false) || ValidationHelper.GetBoolean(isBundleItem, false))
        {
            skuName.Append("<span style=\"font-size: 90%\"> - ");
            skuName.Append(HTMLHelper.HTMLEncode(name));

            if (!string.IsNullOrEmpty(text))
            {
                skuName.Append(" '" + HTMLHelper.HTMLEncode(text) + "'");
            }

            skuName.Append("</span>");
        }
        // If it is a parent product
        else
        {
            // Add private donation suffix
            if ((type == SKUProductTypeEnum.Donation) && (isPrivate))
            {
                name += string.Format(" ({0})", GetString("com.shoppingcartcontent.privatedonation"));
            }

            // In CMS Desk
            if (ShoppingCartControl.IsInternalOrder)
            {
                // Display SKU name
                skuName.Append(HTMLHelper.HTMLEncode(name));
            }
            // On live site
            else
            {
                if (type == SKUProductTypeEnum.Donation)
                {
                    // Display donation name without link
                    skuName.Append(HTMLHelper.HTMLEncode(name));
                }
                else
                {
                    // Display link to product page
                    skuName.AppendFormat("<a href=\"{0}?productid={1}\" class=\"CartProductDetailLink\">{2}</a>", ResolveUrl("~/CMSPages/Ecommerce/GetProduct.aspx"), skuId, HTMLHelper.HTMLEncode(name));
                }
            }
        }

        return skuName.ToString();
    }


    protected bool GetBoolean(object value)
    {
        return ValidationHelper.GetBoolean(value, false);
    }


    /// <summary>
    /// Returns order item edit action HTML.
    /// </summary>
    protected string GetOrderItemEditAction(object value)
    {
        var editOrderItemElemHtml = "";
        Guid itemGuid = ValidationHelper.GetGuid(value, Guid.Empty);

        if (itemGuid != Guid.Empty)
        {
            var query = "itemguid=" + itemGuid + GetCMSDeskShoppingCartSessionNameQuery();
            var editItemUrl = UIContextHelper.GetElementDialogUrl(ModuleName.ECOMMERCE, "order.OrderItemProperties", 0, query);

            var priceDetailButton = new CMSGridActionButton
            {
                IconCssClass = "icon-edit",
                IconStyle = GridIconStyle.Allow,
                ToolTip = GetString("shoppingcart.editorderitem"),
                OnClientClick = ScriptHelper.GetModalDialogScript(editItemUrl, "OrderItemEdit", 720, 420)
            };

            editOrderItemElemHtml = priceDetailButton.GetRenderedHTML();
        }

        return editOrderItemElemHtml;
    }


    /// <summary>
    /// Returns SKU edit action HTML.
    /// </summary>
    protected string GetSKUEditAction(object skuId, object skuSiteId, object skuParentSkuId, object isProductOption, object isBundleItem)
    {
        var editSKUElemHtml = "";

        if (!ValidationHelper.GetBoolean(isProductOption, false) && !ValidationHelper.GetBoolean(isBundleItem, false) && ShoppingCartControl.IsInternalOrder)
        {
            // Do not render product detail link, when not authorized
            if (!CanReadProducts)
            {
                return editSKUElemHtml;
            }

            // Show variants tab for product variant, otherwise general tab
            int parentSkuId = ValidationHelper.GetInteger(skuParentSkuId, 0);
            string productIdParam = (parentSkuId == 0) ? skuId.ToString() : parentSkuId.ToString();
            string tabParam = (parentSkuId == 0) ? "Products.General" : "Products.Variants";

            var query = URLHelper.AddParameterToUrl("", "productid", productIdParam);
            query = URLHelper.AddParameterToUrl(query, "tabName", tabParam);
            query = URLHelper.AddParameterToUrl(query, "siteid", skuSiteId.ToString());
            query = URLHelper.AddParameterToUrl(query, "dialog", "1");
            var url = UIContextHelper.GetElementDialogUrl("CMS.ECommerce", "Products.Properties", additionalQuery: query);

            // Different tooltip for product than for product variant
            string tooltip = (parentSkuId == 0) ? GetString("shoppingcart.editproduct") : GetString("shoppingcart.editproductvariant");

            var priceDetailButton = new CMSGridActionButton
            {
                IconCssClass = "icon-box",
                ToolTip = tooltip,
                OnClientClick = ScriptHelper.GetModalDialogScript(url, "SKUEdit", "95%", "95%"),
            };

            editSKUElemHtml = priceDetailButton.GetRenderedHTML();
        }

        return editSKUElemHtml;
    }


    /// <summary>
    /// Returns formatted child cart item units. Returns empty string if it is not product option or bundle item.
    /// </summary>
    /// <param name="skuUnits">SKU units</param>
    /// <param name="isProductOption">Indicates if cart item is product option</param>
    /// <param name="isBundleItem">Indicates if cart item is bundle item</param>
    protected static string GetChildCartItemUnits(object skuUnits, object isProductOption, object isBundleItem)
    {
        if (ValidationHelper.GetBoolean(isProductOption, false) || ValidationHelper.GetBoolean(isBundleItem, false))
        {
            return string.Format("<span>{0}</span>", skuUnits);
        }

        return "";
    }

    #endregion
}

