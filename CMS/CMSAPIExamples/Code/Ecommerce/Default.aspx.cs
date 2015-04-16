using System;
using System.Collections.Generic;
using System.Linq;

using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Ecommerce;
using CMS.EcommerceProvider;
using CMS.Helpers;
using CMS.Globalization;
using CMS.Localization;
using CMS.Membership;
using CMS.Newsletters;
using CMS.Base;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSAPIExamples_Code_Ecommerce_Default : CMSAPIExamplePage
{
    #region "Initialization"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Section: Configuration

        // Invoice
        apiGetAndUpdateInvoice.RunExample += GetAndUpdateInvoice;

        // Checkout process step
        apiCreateCheckoutProcessStep.RunExample += CreateCheckoutProcessStep;
        apiGetAndUpdateCheckoutProcessStep.RunExample += GetAndUpdateCheckoutProcessStep;
        apiGetAndBulkUpdateCheckoutProcessSteps.RunExample += GetAndBulkUpdateCheckoutProcessSteps;
        apiGenerateDefaultCheckoutProcess.RunExample += GenerateDefaultCheckoutProcess;
        apiDeleteCheckoutProcessStep.RunExample += DeleteCheckoutProcessStep;

        // Tax class
        apiCreateTaxClass.RunExample += CreateTaxClass;
        apiGetAndUpdateTaxClass.RunExample += GetAndUpdateTaxClass;
        apiGetAndBulkUpdateTaxClasses.RunExample += GetAndBulkUpdateTaxClasses;
        apiDeleteTaxClass.RunExample += DeleteTaxClass;

        // Tax class value in country
        apiSetTaxClassValueInCountry.RunExample += SetTaxClassValueInCountry;
        apiGetAndUpdateTaxClassValueInCountry.RunExample += GetAndUpdateTaxClassValueInCountry;
        apiGetAndBulkUpdateTaxClassValuesInCountry.RunExample += GetAndBulkUpdateTaxClassValuesInCountry;
        apiDeleteTaxClassValueInCountry.RunExample += DeleteTaxClassValueInCountry;

        // Tax class value in state
        apiSetTaxClassValueInState.RunExample += SetTaxClassValueInState;
        apiGetAndUpdateTaxClassValueInState.RunExample += GetAndUpdateTaxClassValueInState;
        apiGetAndBulkUpdateTaxClassValuesInState.RunExample += GetAndBulkUpdateTaxClassValuesInState;
        apiDeleteTaxClassValueInState.RunExample += DeleteTaxClassValueInState;

        // Currency
        apiCreateCurrency.RunExample += CreateCurrency;
        apiGetAndUpdateCurrency.RunExample += GetAndUpdateCurrency;
        apiGetAndBulkUpdateCurrencies.RunExample += GetAndBulkUpdateCurrencies;
        apiDeleteCurrency.RunExample += DeleteCurrency;

        // Exchange table
        apiCreateExchangeTable.RunExample += CreateExchangeTable;
        apiGetAndUpdateExchangeTable.RunExample += GetAndUpdateExchangeTable;
        apiGetAndBulkUpdateExchangeTables.RunExample += GetAndBulkUpdateExchangeTables;
        apiDeleteExchangeTable.RunExample += DeleteExchangeTable;

        // Order status
        apiCreateOrderStatus.RunExample += CreateOrderStatus;
        apiGetAndUpdateOrderStatus.RunExample += GetAndUpdateOrderStatus;
        apiGetAndBulkUpdateOrderStatuses.RunExample += GetAndBulkUpdateOrderStatuses;
        apiDeleteOrderStatus.RunExample += DeleteOrderStatus;

        // Public status
        apiCreatePublicStatus.RunExample += CreatePublicStatus;
        apiGetAndUpdatePublicStatus.RunExample += GetAndUpdatePublicStatus;
        apiGetAndBulkUpdatePublicStatuses.RunExample += GetAndBulkUpdatePublicStatuses;
        apiDeletePublicStatus.RunExample += DeletePublicStatus;

        // Internal status
        apiCreateInternalStatus.RunExample += CreateInternalStatus;
        apiGetAndUpdateInternalStatus.RunExample += GetAndUpdateInternalStatus;
        apiGetAndBulkUpdateInternalStatuses.RunExample += GetAndBulkUpdateInternalStatuses;
        apiDeleteInternalStatus.RunExample += DeleteInternalStatus;

        // Department
        apiCreateDepartment.RunExample += CreateDepartment;
        apiGetAndUpdateDepartment.RunExample += GetAndUpdateDepartment;
        apiGetAndBulkUpdateDepartments.RunExample += GetAndBulkUpdateDepartments;
        apiDeleteDepartment.RunExample += DeleteDepartment;

        // Department user
        apiAddUserToDepartment.RunExample += AddUserToDepartment;
        apiRemoveUserFromDepartment.RunExample += RemoveUserFromDepartment;

        // Department default tax class
        apiAddTaxClassToDepartment.RunExample += AddTaxClassToDepartment;
        apiRemoveTaxClassFromDepartment.RunExample += RemoveTaxClassFromDepartment;

        // Shipping option
        apiCreateShippingOption.RunExample += CreateShippingOption;
        apiGetAndUpdateShippingOption.RunExample += GetAndUpdateShippingOption;
        apiGetAndBulkUpdateShippingOptions.RunExample += GetAndBulkUpdateShippingOptions;
        apiDeleteShippingOption.RunExample += DeleteShippingOption;
        apiAddShippingCostToOption.RunExample += AddShippingCostToOption;
        apiRemoveShippingCostFromOption.RunExample += RemoveShippingCostFromOption;
        apiAddTaxClassToOption.RunExample += AddTaxClassToOption;
        apiRemoveTaxClassFromOption.RunExample += RemoveTaxClassFromOption;

        // Payment method
        apiCreatePaymentMethod.RunExample += CreatePaymentMethod;
        apiGetAndUpdatePaymentMethod.RunExample += GetAndUpdatePaymentMethod;
        apiGetAndBulkUpdatePaymentMethods.RunExample += GetAndBulkUpdatePaymentMethods;
        apiDeletePaymentMethod.RunExample += DeletePaymentMethod;

        // Manufacturer
        apiCreateManufacturer.RunExample += CreateManufacturer;
        apiGetAndUpdateManufacturer.RunExample += GetAndUpdateManufacturer;
        apiGetAndBulkUpdateManufacturers.RunExample += GetAndBulkUpdateManufacturers;
        apiDeleteManufacturer.RunExample += DeleteManufacturer;

        // Supplier
        apiCreateSupplier.RunExample += CreateSupplier;
        apiGetAndUpdateSupplier.RunExample += GetAndUpdateSupplier;
        apiGetAndBulkUpdateSuppliers.RunExample += GetAndBulkUpdateSuppliers;
        apiDeleteSupplier.RunExample += DeleteSupplier;

        // Section: Products

        // Product
        apiCreateProduct.RunExample += CreateProduct;
        apiGetAndUpdateProduct.RunExample += GetAndUpdateProduct;
        apiGetAndBulkUpdateProducts.RunExample += GetAndBulkUpdateProducts;
        apiDeleteProduct.RunExample += DeleteProduct;

        //Product type
        apiCreateMembershipProduct.RunExample += CreateMembershipProduct;
        apiDeleteMembershipProduct.RunExample += DeleteMembershipProduct;
        apiCreateEProduct.RunExample += CreateEProduct;
        apiDeleteEProduct.RunExample += DeleteEProduct;
        apiCreateDonation.RunExample += CreateDonation;
        apiDeleteDonation.RunExample += DeleteDonation;
        apiCreateBundle.RunExample += CreateBundle;
        apiDeleteBundle.RunExample += DeleteBundle;

        // Shopping cart
        apiAddProductToShoppingCart.RunExample += AddProductToShoppingCart;
        apiUpdateShoppingCartItemUnits.RunExample += UpdateShoppingCartItemUnits;
        apiRemoveProductFromShoppingCart.RunExample += RemoveProductFromShoppingCart;

        // Product document
        apiCreateProductDocument.RunExample += CreateProductDocument;
        apiGetAndUpdateProductDocument.RunExample += GetAndUpdateProductDocument;
        apiDeleteProductDocument.RunExample += DeleteProductDocument;

        // Product tax class
        apiAddTaxClassToProduct.RunExample += AddTaxClassToProduct;
        apiRemoveTaxClassFromProduct.RunExample += RemoveTaxClassFromProduct;

        // Volume discount
        apiCreateVolumeDiscount.RunExample += CreateVolumeDiscount;
        apiGetAndUpdateVolumeDiscount.RunExample += GetAndUpdateVolumeDiscount;
        apiGetAndBulkUpdateVolumeDiscounts.RunExample += GetAndBulkUpdateVolumeDiscounts;
        apiDeleteVolumeDiscount.RunExample += DeleteVolumeDiscount;

        // Product option category
        apiCreateOptionCategory.RunExample += CreateOptionCategory;
        apiGetAndUpdateOptionCategory.RunExample += GetAndUpdateOptionCategory;
        apiGetAndBulkUpdateOptionCategories.RunExample += GetAndBulkUpdateOptionCategories;
        apiDeleteOptionCategory.RunExample += DeleteOptionCategory;

        // Product option
        apiCreateOption.RunExample += CreateOption;
        apiGetAndUpdateOption.RunExample += GetAndUpdateOption;
        apiGetAndBulkUpdateOptions.RunExample += GetAndBulkUpdateOptions;
        apiDeleteOption.RunExample += DeleteOption;

        // Option category on product
        apiAddCategoryToProduct.RunExample += AddCategoryToProduct;
        apiRemoveCategoryFromProduct.RunExample += RemoveCategoryFromProduct;

        // Option for product
        apiAllowOptionForProduct.RunExample += AllowOptionForProduct;
        apiRemoveOptionFromProduct.RunExample += RemoveOptionFromProduct;

        // Product variants
        apiCreateVariants.RunExample += CreateVariants;
        apiGetAndUpdateVariants.RunExample += GetAndUpdateVariants;
        apiDeleteVariants.RunExample += DeleteVariants;

        // Section: Discounts

        // Discount coupon
        apiCreateDiscountCoupon.RunExample += CreateDiscountCoupon;
        apiGetAndUpdateDiscountCoupon.RunExample += GetAndUpdateDiscountCoupon;
        apiGetAndBulkUpdateDiscountCoupons.RunExample += GetAndBulkUpdateDiscountCoupons;
        apiDeleteDiscountCoupon.RunExample += DeleteDiscountCoupon;

        // Discount coupon products
        apiAddProductToCoupon.RunExample += AddProductToCoupon;
        apiRemoveProductFromCoupon.RunExample += RemoveProductFromCoupon;

        // Section: Customers

        // Customer
        apiCreateAnonymousCustomer.RunExample += CreateAnonymousCustomer;
        apiCreateRegisteredCustomer.RunExample += CreateRegisteredCustomer;
        apiGetAndUpdateCustomer.RunExample += GetAndUpdateCustomer;
        apiGetAndBulkUpdateCustomers.RunExample += GetAndBulkUpdateCustomers;
        apiDeleteCustomer.RunExample += DeleteCustomer;

        // Customer address
        apiCreateAddress.RunExample += CreateAddress;
        apiGetAndUpdateAddress.RunExample += GetAndUpdateAddress;
        apiGetAndBulkUpdateAddresses.RunExample += GetAndBulkUpdateAddresses;
        apiDeleteAddress.RunExample += DeleteAddress;

        // Customer credit event
        apiCreateCreditEvent.RunExample += CreateCreditEvent;
        apiGetAndUpdateCreditEvent.RunExample += GetAndUpdateCreditEvent;
        apiGetAndBulkUpdateCreditEvents.RunExample += GetAndBulkUpdateCreditEvents;
        apiGetTotalCredit.RunExample += GetTotalCredit;
        apiDeleteCreditEvent.RunExample += DeleteCreditEvent;

        // Customer newsletter
        apiSubscribeCustomerToNewsletter.RunExample += SubscribeCustomerToNewsletter;
        apiUnsubscribeCustomerFromNewsletter.RunExample += UnsubscribeCustomerFromNewsletter;

        // Customer wishlist
        apiAddProductToWishlist.RunExample += AddProductToWishlist;
        apiRemoveProductFromWishlist.RunExample += RemoveProductFromWishlist;

        // Section: Orders

        // Order
        apiCreateOrder.RunExample += CreateOrder;
        apiGetAndUpdateOrder.RunExample += GetAndUpdateOrder;
        apiGetAndBulkUpdateOrders.RunExample += GetAndBulkUpdateOrders;
        apiDeleteOrder.RunExample += DeleteOrder;

        // Order item
        apiCreateOrderItem.RunExample += CreateOrderItem;
        apiGetAndUpdateOrderItem.RunExample += GetAndUpdateOrderItem;
        apiGetAndBulkUpdateOrderItems.RunExample += GetAndBulkUpdateOrderItems;
        apiDeleteOrderItem.RunExample += DeleteOrderItem;

        // Order status history
        apiChangeOrderStatus.RunExample += ChangeOrderStatus;
        apiDeleteHistory.RunExample += DeleteHistory;
    }

    #endregion


    #region "Mass actions"

    /// <summary>
    /// Runs all creating and managing examples.
    /// </summary>
    public override void RunAll()
    {
        base.RunAll();

        // Invoice
        apiGetAndUpdateInvoice.Run();

        // Checkout process step
        apiGenerateDefaultCheckoutProcess.Run();
        apiCreateCheckoutProcessStep.Run();
        apiGetAndUpdateCheckoutProcessStep.Run();
        apiGetAndBulkUpdateCheckoutProcessSteps.Run();

        // Tax class
        apiCreateTaxClass.Run();
        apiGetAndUpdateTaxClass.Run();
        apiGetAndBulkUpdateTaxClasses.Run();

        // Tax class value in country
        apiSetTaxClassValueInCountry.Run();
        apiGetAndUpdateTaxClassValueInCountry.Run();
        apiGetAndBulkUpdateTaxClassValuesInCountry.Run();

        // Tax class value in state
        apiSetTaxClassValueInState.Run();
        apiGetAndUpdateTaxClassValueInState.Run();
        apiGetAndBulkUpdateTaxClassValuesInState.Run();

        // Currency
        apiCreateCurrency.Run();
        apiGetAndUpdateCurrency.Run();
        apiGetAndBulkUpdateCurrencies.Run();

        // Exchange table
        apiCreateExchangeTable.Run();
        apiGetAndUpdateExchangeTable.Run();
        apiGetAndBulkUpdateExchangeTables.Run();

        // Order status
        apiCreateOrderStatus.Run();
        apiGetAndUpdateOrderStatus.Run();
        apiGetAndBulkUpdateOrderStatuses.Run();

        // Public status
        apiCreatePublicStatus.Run();
        apiGetAndUpdatePublicStatus.Run();
        apiGetAndBulkUpdatePublicStatuses.Run();

        // Internal status
        apiCreateInternalStatus.Run();
        apiGetAndUpdateInternalStatus.Run();
        apiGetAndBulkUpdateInternalStatuses.Run();

        // Department
        apiCreateDepartment.Run();
        apiGetAndUpdateDepartment.Run();
        apiGetAndBulkUpdateDepartments.Run();

        // Department user
        apiAddUserToDepartment.Run();

        // Department default tax class
        apiAddTaxClassToDepartment.Run();

        // Shipping option
        apiCreateShippingOption.Run();
        apiGetAndUpdateShippingOption.Run();
        apiGetAndBulkUpdateShippingOptions.Run();
        apiAddShippingCostToOption.Run();
        apiAddTaxClassToOption.Run();

        // Payment method
        apiCreatePaymentMethod.Run();
        apiGetAndUpdatePaymentMethod.Run();
        apiGetAndBulkUpdatePaymentMethods.Run();

        // Manufacturer
        apiCreateManufacturer.Run();
        apiGetAndUpdateManufacturer.Run();
        apiGetAndBulkUpdateManufacturers.Run();

        // Supplier
        apiCreateSupplier.Run();
        apiGetAndUpdateSupplier.Run();
        apiGetAndBulkUpdateSuppliers.Run();

        // Product
        apiCreateProduct.Run();
        apiGetAndUpdateProduct.Run();
        apiGetAndBulkUpdateProducts.Run();

        // Product document
        apiCreateProductDocument.Run();
        apiGetAndUpdateProductDocument.Run();

        //Product type
        apiCreateMembershipProduct.Run();
        apiCreateEProduct.Run();
        apiCreateDonation.Run();
        apiCreateBundle.Run();

        // Shopping cart
        apiAddProductToShoppingCart.Run();
        apiUpdateShoppingCartItemUnits.Run();

        // Product tax class
        apiAddTaxClassToProduct.Run();

        // Volume discount
        apiCreateVolumeDiscount.Run();
        apiGetAndUpdateVolumeDiscount.Run();
        apiGetAndBulkUpdateVolumeDiscounts.Run();

        // Product option category
        apiCreateOptionCategory.Run();
        apiGetAndUpdateOptionCategory.Run();
        apiGetAndBulkUpdateOptionCategories.Run();

        // Product option
        apiCreateOption.Run();
        apiGetAndUpdateOption.Run();
        apiGetAndBulkUpdateOptions.Run();

        // Option category on product
        apiAddCategoryToProduct.Run();

        // Option for product
        apiAllowOptionForProduct.Run();

        // Product variants
        apiCreateVariants.Run();
        apiGetAndUpdateVariants.Run();

        // Discount coupon
        apiCreateDiscountCoupon.Run();
        apiGetAndUpdateDiscountCoupon.Run();
        apiGetAndBulkUpdateDiscountCoupons.Run();

        // Discount coupon products
        apiAddProductToCoupon.Run();

        // Customer
        apiCreateAnonymousCustomer.Run();
        apiCreateRegisteredCustomer.Run();
        apiGetAndUpdateCustomer.Run();
        apiGetAndBulkUpdateCustomers.Run();

        // Address
        apiCreateAddress.Run();
        apiGetAndUpdateAddress.Run();
        apiGetAndBulkUpdateAddresses.Run();

        // Credit event
        apiCreateCreditEvent.Run();
        apiGetAndUpdateCreditEvent.Run();
        apiGetAndBulkUpdateCreditEvents.Run();
        apiGetTotalCredit.Run();

        // Customer newsletter
        apiSubscribeCustomerToNewsletter.Run();

        // Customer wishlist
        apiAddProductToWishlist.Run();

        // Order
        apiCreateOrder.Run();
        apiGetAndUpdateOrder.Run();
        apiGetAndBulkUpdateOrders.Run();

        // Order item
        apiCreateOrderItem.Run();
        apiGetAndUpdateOrderItem.Run();
        apiGetAndBulkUpdateOrderItems.Run();

        // Order status history
        apiChangeOrderStatus.Run();
    }


    /// <summary>
    /// Runs all cleanup examples.
    /// </summary>
    public override void CleanUpAll()
    {
        base.CleanUpAll();

        // Order status history
        apiDeleteHistory.Run();

        // Order item
        apiDeleteOrderItem.Run();

        // Order
        apiDeleteOrder.Run();

        // Customer wishlist
        apiRemoveProductFromWishlist.Run();

        // Customer newsletter
        apiUnsubscribeCustomerFromNewsletter.Run();

        // Credit event
        apiDeleteCreditEvent.Run();

        // Address
        apiDeleteAddress.Run();

        // Customer
        apiDeleteCustomer.Run();

        // Discount coupon products
        apiRemoveProductFromCoupon.Run();

        // Discount coupon
        apiDeleteDiscountCoupon.Run();

        // Option on product
        apiRemoveOptionFromProduct.Run();

        // Option category on product
        apiRemoveCategoryFromProduct.Run();

        // Product option
        apiDeleteOption.Run();

        // Option category
        apiDeleteOptionCategory.Run();

        // Product variants
        apiDeleteVariants.Run();

        // Volume discount
        apiDeleteVolumeDiscount.Run();

        // Product tax class
        apiRemoveTaxClassFromProduct.Run();

        // Shopping cart
        apiRemoveProductFromShoppingCart.Run();

        //Product type
        apiDeleteBundle.Run();
        apiDeleteDonation.Run();
        apiDeleteEProduct.Run();
        apiDeleteMembershipProduct.Run();

        // Product document
        apiDeleteProductDocument.Run();

        // Product
        apiDeleteProduct.Run();

        // Supplier
        apiDeleteSupplier.Run();

        // Manufacturer
        apiDeleteManufacturer.Run();

        // Payment method
        apiDeletePaymentMethod.Run();

        // Shipping option
        apiRemoveTaxClassFromOption.Run();
        apiRemoveShippingCostFromOption.Run();
        apiDeleteShippingOption.Run();

        // Department default tax class
        apiRemoveTaxClassFromDepartment.Run();

        // Department user
        apiRemoveUserFromDepartment.Run();

        // Department
        apiDeleteDepartment.Run();

        // Internal status
        apiDeleteInternalStatus.Run();

        // Public status
        apiDeletePublicStatus.Run();

        // Order status
        apiDeleteOrderStatus.Run();

        // Exchange table
        apiDeleteExchangeTable.Run();

        // Currency
        apiDeleteCurrency.Run();

        // Tax class value in state
        apiDeleteTaxClassValueInState.Run();

        // Tax class value in country
        apiDeleteTaxClassValueInCountry.Run();

        // Checkout process step
        apiDeleteCheckoutProcessStep.Run();

        // Tax class
        apiDeleteTaxClass.Run();
    }

    #endregion


    #region "API examples - Invoice"

    /// <summary>
    /// Gets and updates invoice. Called when the "Get and update invoice" button is pressed.
    /// </summary>
    private bool GetAndUpdateInvoice()
    {
        // Get site name
        string siteName = SiteContext.CurrentSiteName;

        // Get the invoice
        string invoice = ECommerceSettings.InvoiceTemplate(siteName);

        if (!String.IsNullOrEmpty(invoice))
        {
            // Add string to the invoice
            invoice = "<h1>My updated invoice</h1>" + invoice;

            // Update the invoice
            SettingsKeyInfoProvider.SetValue(siteName + "." + ECommerceSettings.INVOICE_TEMPLATE, invoice);

            return true;
        }

        return false;
    }

    #endregion


    #region "API examples - Checkout process step"

    /// <summary>
    /// Generates default checkout process. Called when the "Generate default process" button is pressed.
    /// </summary>
    private bool GenerateDefaultCheckoutProcess()
    {
        // Create new checkout process object
        CheckoutProcessInfo checkoutProcess = new CheckoutProcessInfo();

        // Load default checkout process xml definition
        checkoutProcess.LoadXmlDefinition(ShoppingCart.DEFAULT_CHECKOUT_PROCESS);

        // Save the data
        SettingsKeyInfoProvider.SetValue(SiteContext.CurrentSiteName + "." + ECommerceSettings.CHECKOUT_PROCESS, checkoutProcess.GetXmlDefinition());

        return true;
    }


    /// <summary>
    /// Creates checkout process step. Called when the "Create step" button is pressed.
    /// </summary>
    private bool CreateCheckoutProcessStep()
    {
        // Create new checkout process step object
        CheckoutProcessStepInfo newStep = new CheckoutProcessStepInfo();

        // Set the properties
        newStep.Name = "MyNewStep";
        newStep.Caption = "My new step";
        newStep.ControlPath = "";
        newStep.ShowInCMSDeskCustomer = true;
        newStep.ShowInCMSDeskOrder = true;
        newStep.ShowOnLiveSite = true;
        newStep.ShowInCMSDeskOrderItems = true;

        // Insert node
        CheckoutProcessInfo checkoutProcess = new CheckoutProcessInfo();
        checkoutProcess.LoadXmlDefinition(ECommerceSettings.CheckoutProcess(SiteContext.CurrentSiteName));
        checkoutProcess.SetCheckoutProcessStepNode(newStep);

        // Create the checkout process
        SettingsKeyInfoProvider.SetValue(SiteContext.CurrentSiteName + "." + ECommerceSettings.CHECKOUT_PROCESS, checkoutProcess.GetXmlDefinition());

        return true;
    }


    /// <summary>
    /// Gets and updates checkout process step. Called when the "Get and update step" button is pressed.
    /// Expects the CreateCheckoutProcessStep method to be run first.
    /// </summary>
    private bool GetAndUpdateCheckoutProcessStep()
    {
        CheckoutProcessInfo checkoutProcess = new CheckoutProcessInfo();
        checkoutProcess.LoadXmlDefinition(ECommerceSettings.CheckoutProcess(SiteContext.CurrentSiteName));

        CheckoutProcessStepInfo updateStep = checkoutProcess.GetCheckoutProcessStepInfo("MyNewStep");

        if (updateStep != null)
        {
            // Update the property
            updateStep.Caption = updateStep.Caption.ToLowerCSafe();

            // Update xml definition
            checkoutProcess.SetCheckoutProcessStepNode(updateStep);

            // Update the checkout process
            SettingsKeyInfoProvider.SetValue(SiteContext.CurrentSiteName + "." + ECommerceSettings.CHECKOUT_PROCESS, checkoutProcess.GetXmlDefinition());

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk update checkout process steps. Called when the "Get and bulk update steps" button is pressed.
    /// Expects the CreateCheckoutProcessStep method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateCheckoutProcessSteps()
    {
        bool success = false;

        CheckoutProcessInfo checkoutProcess = new CheckoutProcessInfo();
        checkoutProcess.LoadXmlDefinition(ECommerceSettings.CheckoutProcess(SiteContext.CurrentSiteName));

        // Get the list
        List<CheckoutProcessStepInfo> steps = checkoutProcess.GetCheckoutProcessSteps(CheckoutProcessEnum.CMSDeskOrder);
        foreach (CheckoutProcessStepInfo updateStep in steps)
        {
            if (updateStep.Name == "MyNewStep")
            {
                // Update the property
                updateStep.Caption = updateStep.Caption.ToUpper();

                // Update xml definition
                checkoutProcess.SetCheckoutProcessStepNode(updateStep);

                success = true;
            }
        }

        // Update the checkout process
        SettingsKeyInfoProvider.SetValue(SiteContext.CurrentSiteName + "." + ECommerceSettings.CHECKOUT_PROCESS, checkoutProcess.GetXmlDefinition());

        return success;
    }


    /// <summary>
    /// Deletes checkout process step. Called when the "Delete step" button is pressed.
    /// Expects the CreateCheckoutProcessStep method to be run first.
    /// </summary>
    private bool DeleteCheckoutProcessStep()
    {
        // Create new checkout process
        CheckoutProcessInfo checkoutProcess = new CheckoutProcessInfo();

        // Load checkout process xml definition
        checkoutProcess.LoadXmlDefinition(ECommerceSettings.CheckoutProcess(SiteContext.CurrentSiteName));

        // Get 'My new step' checkout process step
        CheckoutProcessStepInfo deleteStep = checkoutProcess.GetCheckoutProcessStepInfo("MyNewStep");

        if (deleteStep != null)
        {
            // Delete the step
            checkoutProcess.RemoveCheckoutProcessStepNode("MyNewStep");

            // Save the data
            SettingsKeyInfoProvider.SetValue(SiteContext.CurrentSiteName + "." + ECommerceSettings.CHECKOUT_PROCESS, checkoutProcess.GetXmlDefinition());

            return true;
        }

        return false;
    }

    #endregion


    #region "API examples - Tax class"

    /// <summary>
    /// Creates tax class. Called when the "Create class" button is pressed.
    /// </summary>
    private bool CreateTaxClass()
    {
        // Create new tax class
        TaxClassInfo newClass = new TaxClassInfo();

        // Set the properties
        newClass.TaxClassDisplayName = "My new class";
        newClass.TaxClassName = "MyNewClass";
        newClass.TaxClassSiteID = SiteContext.CurrentSiteID;

        // Create the tax class
        TaxClassInfoProvider.SetTaxClassInfo(newClass);

        return true;
    }


    /// <summary>
    /// Gets and updates tax class. Called when the "Get and update class" button is pressed.
    /// Expects the CreateTaxClass method to be run first.
    /// </summary>
    private bool GetAndUpdateTaxClass()
    {
        // Get the tax class
        TaxClassInfo updateClass = TaxClassInfoProvider.GetTaxClassInfo("MyNewClass", SiteContext.CurrentSiteName);
        if (updateClass != null)
        {
            // Update the properties
            updateClass.TaxClassDisplayName = updateClass.TaxClassDisplayName.ToLowerCSafe();

            // Update the tax class
            TaxClassInfoProvider.SetTaxClassInfo(updateClass);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates tax classes. Called when the "Get and bulk update classes" button is pressed.
    /// Expects the CreateTaxClass method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateTaxClasses()
    {
        // Get the data
        var classes = TaxClassInfoProvider.GetTaxClasses().WhereStartsWith("TaxClassName", "MyNewClass");
        var success = false;

        // Loop through the individual items
        foreach (var modifyClass in classes)
        {
            // Update the properties
            modifyClass.TaxClassDisplayName = modifyClass.TaxClassDisplayName.ToUpper();

            // Update the tax class
            TaxClassInfoProvider.SetTaxClassInfo(modifyClass);

            success = true;
        }

        return success;
    }


    /// <summary>
    /// Deletes tax class. Called when the "Delete class" button is pressed.
    /// Expects the CreateTaxClass method to be run first.
    /// </summary>
    private bool DeleteTaxClass()
    {
        // Get the tax class
        TaxClassInfo deleteClass = TaxClassInfoProvider.GetTaxClassInfo("MyNewClass", SiteContext.CurrentSiteName);

        // Delete the tax class
        TaxClassInfoProvider.DeleteTaxClassInfo(deleteClass);

        return (deleteClass != null);
    }

    #endregion


    #region "API examples - Tax class value in country"

    /// <summary>
    /// Sets tax class value in country. Called when the "Set value" button is pressed.
    /// </summary>
    private bool SetTaxClassValueInCountry()
    {
        // Get the tax class
        TaxClassInfo taxClass = TaxClassInfoProvider.GetTaxClassInfo("MyNewClass", SiteContext.CurrentSiteName);

        // Get the country
        CountryInfo country = CountryInfoProvider.GetCountryInfo("USA");

        if ((taxClass != null) && (country != null))
        {
            // Create new tax class country object
            TaxClassCountryInfo newTaxCountry = new TaxClassCountryInfo();

            // Set the properties
            newTaxCountry.TaxClassID = taxClass.TaxClassID;
            newTaxCountry.CountryID = country.CountryID;
            newTaxCountry.TaxValue = 10;
            newTaxCountry.IsFlatValue = true;

            // Set the tax class value in country
            TaxClassCountryInfoProvider.SetTaxClassCountryInfo(newTaxCountry);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and updates tax class value in country. Called when the "Get and update value" button is pressed.
    /// Expects the SetTaxClassValueInCountry method to be run first.
    /// </summary>
    private bool GetAndUpdateTaxClassValueInCountry()
    {
        // Get the tax class
        TaxClassInfo taxClass = TaxClassInfoProvider.GetTaxClassInfo("MyNewClass", SiteContext.CurrentSiteName);

        // Get the country
        CountryInfo country = CountryInfoProvider.GetCountryInfo("USA");

        if ((taxClass != null) && (country != null))
        {
            // Get the tax class country
            TaxClassCountryInfo updateTaxCountry = TaxClassCountryInfoProvider.GetTaxClassCountryInfo(country.CountryID, taxClass.TaxClassID);
            if (updateTaxCountry != null)
            {
                // Update the property
                updateTaxCountry.TaxValue = 100;

                // Update the tax class value in country
                TaxClassCountryInfoProvider.SetTaxClassCountryInfo(updateTaxCountry);

                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates tax class values in country. Called when the "Get and bulk update values" button is pressed.
    /// Expects the SetTaxClassValueInCountry method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateTaxClassValuesInCountry()
    {
        // Get the tax class
        TaxClassInfo taxClass = TaxClassInfoProvider.GetTaxClassInfo("MyNewClass", SiteContext.CurrentSiteName);

        // Get the country
        CountryInfo country = CountryInfoProvider.GetCountryInfo("USA");

        var success = false;

        if ((taxClass != null) && (country != null))
        {
            // Get the data
            var classCountries = TaxClassCountryInfoProvider.GetTaxClassCountries()
                                         .WhereEquals("TaxClassID", taxClass.TaxClassID)
                                         .WhereEquals("CountryID", country.CountryID);

            // Loop through the individual items
            foreach (var modifyClassCountry in classCountries)
            {
                // Update the properties
                modifyClassCountry.TaxValue = 50;

                // Save the changes
                TaxClassCountryInfoProvider.SetTaxClassCountryInfo(modifyClassCountry);

                success = true;
            }
        }

        return success;
    }


    /// <summary>
    /// Deletes tax class value in country. Called when the "Delete value" button is pressed.
    /// Expects the SetTaxClassValueInCountry method to be run first.
    /// </summary>
    private bool DeleteTaxClassValueInCountry()
    {
        // Get the tax class
        TaxClassInfo taxClass = TaxClassInfoProvider.GetTaxClassInfo("MyNewClass", SiteContext.CurrentSiteName);

        // Get the country
        CountryInfo country = CountryInfoProvider.GetCountryInfo("USA");

        if ((taxClass != null) && (country != null))
        {
            // Get the tax class country
            TaxClassCountryInfo deleteTaxCountry = TaxClassCountryInfoProvider.GetTaxClassCountryInfo(country.CountryID, taxClass.TaxClassID);
            if (deleteTaxCountry != null)
            {
                // Delete the tax class value in country
                TaxClassCountryInfoProvider.DeleteTaxClassCountryInfo(deleteTaxCountry);

                return true;
            }
        }

        return false;
    }

    #endregion


    #region "API examples - Tax class value in state"

    /// <summary>
    /// Sets tax class value in state. Called when the "Set value" button is pressed.
    /// </summary>
    private bool SetTaxClassValueInState()
    {
        // Get the tax class
        TaxClassInfo taxClass = TaxClassInfoProvider.GetTaxClassInfo("MyNewClass", SiteContext.CurrentSiteName);

        // Get the state
        StateInfo state = StateInfoProvider.GetStateInfo("Alabama");

        if ((taxClass != null) && (state != null))
        {
            // Create new tax class state
            TaxClassStateInfo newTaxState = new TaxClassStateInfo();

            // Set the properties
            newTaxState.TaxClassID = taxClass.TaxClassID;
            newTaxState.StateID = state.StateID;
            newTaxState.TaxValue = 10;
            newTaxState.IsFlatValue = true;

            // Set the tax class value in state
            TaxClassStateInfoProvider.SetTaxClassStateInfo(newTaxState);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and updates tax class value in state. Called when the "Get and update values" button is pressed.
    /// Expects the SetTaxClassValueInState method to be run first.
    /// </summary>
    private bool GetAndUpdateTaxClassValueInState()
    {
        // Get the tax class
        TaxClassInfo taxClass = TaxClassInfoProvider.GetTaxClassInfo("MyNewClass", SiteContext.CurrentSiteName);

        // Get the state
        StateInfo state = StateInfoProvider.GetStateInfo("Alabama");

        if ((taxClass != null) && (state != null))
        {
            // Get the tax class state
            TaxClassStateInfo updateTaxState = TaxClassStateInfoProvider.GetTaxClassStateInfo(taxClass.TaxClassID, state.StateID);
            if (updateTaxState != null)
            {
                // Update the property
                updateTaxState.TaxValue = 100;

                // Update the tax class value in state
                TaxClassStateInfoProvider.SetTaxClassStateInfo(updateTaxState);

                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates tax class values in state. Called when the "Get and bulk update values" button is pressed.
    /// Expects the SetTaxClassValueInState method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateTaxClassValuesInState()
    {
        // Get the tax class
        TaxClassInfo taxClass = TaxClassInfoProvider.GetTaxClassInfo("MyNewClass", SiteContext.CurrentSiteName);

        // Get the state
        StateInfo state = StateInfoProvider.GetStateInfo("Alabama");

        var success = false;

        if ((taxClass != null) && (state != null))
        {
            // Get the data
            var classStates = TaxClassStateInfoProvider.GetTaxClassStates()
                                      .WhereEquals("TaxClassID", taxClass.TaxClassID)
                                      .WhereEquals("StateID", state.StateID);

            // Loop through the individual items
            foreach (var modifyClassState in classStates)
            {
                // Update the properties
                modifyClassState.TaxValue = 50;

                // Save the changes
                TaxClassStateInfoProvider.SetTaxClassStateInfo(modifyClassState);

                success = true;
            }
        }

        return success;
    }


    /// <summary>
    /// Deletes tax class value in state. Called when the "Delete value" button is pressed.
    /// Expects the SetTaxClassValueInState method to be run first.
    /// </summary>
    private bool DeleteTaxClassValueInState()
    {
        // Get the tax class
        TaxClassInfo taxClass = TaxClassInfoProvider.GetTaxClassInfo("MyNewClass", SiteContext.CurrentSiteName);

        // Get the state
        StateInfo state = StateInfoProvider.GetStateInfo("Alabama");

        if ((taxClass != null) && (state != null))
        {
            // Get the tax class state
            TaxClassStateInfo deleteTaxState = TaxClassStateInfoProvider.GetTaxClassStateInfo(taxClass.TaxClassID, state.StateID);
            if (deleteTaxState != null)
            {
                // Delete the tax class value in state
                TaxClassStateInfoProvider.DeleteTaxClassStateInfo(deleteTaxState);

                return true;
            }
        }

        return false;
    }

    #endregion


    #region "API examples - Currency"

    /// <summary>
    /// Creates currency. Called when the "Create currency" button is pressed.
    /// </summary>
    private bool CreateCurrency()
    {
        // Create new currency object
        CurrencyInfo newCurrency = new CurrencyInfo();

        // Set the properties
        newCurrency.CurrencyDisplayName = "My new currency";
        newCurrency.CurrencyName = "MyNewCurrency";
        newCurrency.CurrencyCode = "MNC";
        newCurrency.CurrencySiteID = SiteContext.CurrentSiteID;
        newCurrency.CurrencyEnabled = true;
        newCurrency.CurrencyFormatString = "{0:F} MNC";
        newCurrency.CurrencyIsMain = false;

        // Create the currency
        CurrencyInfoProvider.SetCurrencyInfo(newCurrency);

        return true;
    }


    /// <summary>
    /// Gets and updates currency. Called when the "Get and update currency" button is pressed.
    /// Expects the CreateCurrency method to be run first.
    /// </summary>
    private bool GetAndUpdateCurrency()
    {
        // Get the currency
        CurrencyInfo updateCurrency = CurrencyInfoProvider.GetCurrencyInfo("MyNewCurrency", SiteContext.CurrentSiteName);
        if (updateCurrency != null)
        {
            // Update the properties
            updateCurrency.CurrencyDisplayName = updateCurrency.CurrencyDisplayName.ToLowerCSafe();

            // Update the currency
            CurrencyInfoProvider.SetCurrencyInfo(updateCurrency);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates currencies. Called when the "Get and bulk update currencies" button is pressed.
    /// Expects the CreateCurrency method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateCurrencies()
    {
        // Get the data
        var currencies = CurrencyInfoProvider.GetCurrencies().WhereEquals("CurrencyName", "MyNewCurrency");

        bool success = false;

        // Loop through the individual items
        foreach (var modifyCurrency in currencies)
        {
            // Update the properties
            modifyCurrency.CurrencyDisplayName = modifyCurrency.CurrencyDisplayName.ToUpper();

            // Update the currency
            CurrencyInfoProvider.SetCurrencyInfo(modifyCurrency);

            success = true;
        }

        return success;
    }


    /// <summary>
    /// Deletes currency. Called when the "Delete currency" button is pressed.
    /// Expects the CreateCurrency method to be run first.
    /// </summary>
    private bool DeleteCurrency()
    {
        // Get the currency
        CurrencyInfo deleteCurrency = CurrencyInfoProvider.GetCurrencyInfo("MyNewCurrency", SiteContext.CurrentSiteName);
        if (deleteCurrency != null)
        {
            // Delete the currency
            CurrencyInfoProvider.DeleteCurrencyInfo(deleteCurrency);

            return true;
        }

        return false;
    }

    #endregion


    #region "API examples - Exchange table"

    /// <summary>
    /// Creates exchange table. Called when the "Create table" button is pressed.
    /// </summary>
    private bool CreateExchangeTable()
    {
        // Create new exchange table object
        ExchangeTableInfo newTable = new ExchangeTableInfo();

        // Set the properties
        newTable.ExchangeTableDisplayName = "My new table";
        newTable.ExchangeTableSiteID = SiteContext.CurrentSiteID;
        newTable.ExchangeTableValidFrom = DateTime.Now;
        newTable.ExchangeTableValidTo = DateTime.Now;

        // Create the exchange table
        ExchangeTableInfoProvider.SetExchangeTableInfo(newTable);

        return true;
    }


    /// <summary>
    /// Gets and updates exchange table. Called when the "Get and update table" button is pressed.
    /// Expects the CreateExchangeTable method to be run first.
    /// </summary>
    private bool GetAndUpdateExchangeTable()
    {
        // Get the exchange table
        ExchangeTableInfo updateTable = ExchangeTableInfoProvider.GetExchangeTableInfo("My new table", SiteContext.CurrentSiteName);
        if (updateTable != null)
        {
            // Set time
            TimeSpan time = TimeSpan.FromDays(7);

            // Update the properties
            updateTable.ExchangeTableValidFrom = updateTable.ExchangeTableValidFrom.Add(time);
            updateTable.ExchangeTableValidTo = updateTable.ExchangeTableValidTo.Add(time);

            // Update the exchange table
            ExchangeTableInfoProvider.SetExchangeTableInfo(updateTable);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates exchange tables. Called when the "Get and bulk update tables" button is pressed.
    /// Expects the CreateExchangeTable method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateExchangeTables()
    {
        // Get the data
        var tables = ExchangeTableInfoProvider.GetExchangeTables().OnSite(SiteContext.CurrentSiteID).WhereStartsWith("ExchangeTableDisplayName", "My new table");

        var success = false;

        // Set the time
        TimeSpan time = TimeSpan.FromDays(14);

        // Loop through the individual items
        foreach (var modifyTable in tables)
        {
            // Update the properties
            modifyTable.ExchangeTableValidFrom = modifyTable.ExchangeTableValidFrom.Add(time);
            modifyTable.ExchangeTableValidTo = modifyTable.ExchangeTableValidTo.Add(time);

            // Update the exchange table
            ExchangeTableInfoProvider.SetExchangeTableInfo(modifyTable);

            success = true;
        }

        return success;
    }


    /// <summary>
    /// Deletes exchange table. Called when the "Delete table" button is pressed.
    /// Expects the CreateExchangeTable method to be run first.
    /// </summary>
    private bool DeleteExchangeTable()
    {
        // Get the exchange table
        ExchangeTableInfo deleteTable = ExchangeTableInfoProvider.GetExchangeTableInfo("My new table", SiteContext.CurrentSiteName);

        // Delete the exchange table
        ExchangeTableInfoProvider.DeleteExchangeTableInfo(deleteTable);

        return (deleteTable != null);
    }

    #endregion


    #region "API examples - Order status"

    /// <summary>
    /// Creates order status. Called when the "Create status" button is pressed.
    /// </summary>
    private bool CreateOrderStatus()
    {
        // Create new order status object
        OrderStatusInfo newStatus = new OrderStatusInfo();

        // Set the properties
        newStatus.StatusDisplayName = "My new status";
        newStatus.StatusName = "MyNewStatus";
        newStatus.StatusEnabled = true;
        newStatus.StatusSiteID = SiteContext.CurrentSiteID;
        newStatus.StatusOrder = 1;

        // Create the order status
        OrderStatusInfoProvider.SetOrderStatusInfo(newStatus);

        return true;
    }


    /// <summary>
    /// Gets and updates order status. Called when the "Get and update status" button is pressed.
    /// Expects the CreateOrderStatus method to be run first.
    /// </summary>
    private bool GetAndUpdateOrderStatus()
    {
        // Get the order status
        OrderStatusInfo updateStatus = OrderStatusInfoProvider.GetOrderStatusInfo("MyNewStatus", SiteContext.CurrentSiteName);
        if (updateStatus != null)
        {
            // Update the properties
            updateStatus.StatusDisplayName = updateStatus.StatusDisplayName.ToLowerCSafe();

            // Update the order status
            OrderStatusInfoProvider.SetOrderStatusInfo(updateStatus);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates order statuses. Called when the "Get and bulk update statuses" button is pressed.
    /// Expects the CreateOrderStatus method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateOrderStatuses()
    {
        // Get the data
        var statuses = OrderStatusInfoProvider.GetOrderStatuses()
                           .OnSite(SiteContext.CurrentSiteID)
                           .WhereStartsWith("StatusName", "MyNewStatus");

        var success = false;

        // Loop through the individual items
        foreach (var modifyStatus in statuses)
        {
            // Update the properties
            modifyStatus.StatusDisplayName = modifyStatus.StatusDisplayName.ToUpper();

            // Update the order status
            OrderStatusInfoProvider.SetOrderStatusInfo(modifyStatus);

            success = true;
        }


        return success;
    }


    /// <summary>
    /// Deletes order status. Called when the "Delete status" button is pressed.
    /// Expects the CreateOrderStatus method to be run first.
    /// </summary>
    private bool DeleteOrderStatus()
    {
        // Get the order status
        OrderStatusInfo deleteStatus = OrderStatusInfoProvider.GetOrderStatusInfo("MyNewStatus", SiteContext.CurrentSiteName);

        // Delete the order status
        OrderStatusInfoProvider.DeleteOrderStatusInfo(deleteStatus);

        return (deleteStatus != null);
    }

    #endregion


    #region "API examples - Public status"

    /// <summary>
    /// Creates public status. Called when the "Create status" button is pressed.
    /// </summary>
    private bool CreatePublicStatus()
    {
        // Create new public status object
        PublicStatusInfo newStatus = new PublicStatusInfo();

        // Set the properties
        newStatus.PublicStatusDisplayName = "My new status";
        newStatus.PublicStatusName = "MyNewStatus";
        newStatus.PublicStatusEnabled = true;
        newStatus.PublicStatusSiteID = SiteContext.CurrentSiteID;

        // Create the public status
        PublicStatusInfoProvider.SetPublicStatusInfo(newStatus);

        return true;
    }


    /// <summary>
    /// Gets and updates public status. Called when the "Get and update status" button is pressed.
    /// Expects the CreatePublicStatus method to be run first.
    /// </summary>
    private bool GetAndUpdatePublicStatus()
    {
        // Get the public status
        PublicStatusInfo updateStatus = PublicStatusInfoProvider.GetPublicStatusInfo("MyNewStatus", SiteContext.CurrentSiteName);
        if (updateStatus != null)
        {
            // Update the properties
            updateStatus.PublicStatusDisplayName = updateStatus.PublicStatusDisplayName.ToLowerCSafe();

            // Update the public status
            PublicStatusInfoProvider.SetPublicStatusInfo(updateStatus);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates public statuses. Called when the "Get and bulk update statuses" button is pressed.
    /// Expects the CreatePublicStatus method to be run first.
    /// </summary>
    private bool GetAndBulkUpdatePublicStatuses()
    {
        // Get the data
        var statuses = PublicStatusInfoProvider.GetPublicStatuses()
                               .OnSite(SiteContext.CurrentSiteID)
                               .WhereStartsWith("PublicStatusName", "MyNewStatus");

        bool success = false;

        // Loop through the individual items
        foreach (var modifyStatus in statuses)
        {
            // Update the properties
            modifyStatus.PublicStatusDisplayName = modifyStatus.PublicStatusDisplayName.ToUpper();

            // Update the public status
            PublicStatusInfoProvider.SetPublicStatusInfo(modifyStatus);

            success = true;
        }

        return success;
    }


    /// <summary>
    /// Deletes public status. Called when the "Delete status" button is pressed.
    /// Expects the CreatePublicStatus method to be run first.
    /// </summary>
    private bool DeletePublicStatus()
    {
        // Get the public status
        PublicStatusInfo deleteStatus = PublicStatusInfoProvider.GetPublicStatusInfo("MyNewStatus", SiteContext.CurrentSiteName);

        // Delete the public status
        PublicStatusInfoProvider.DeletePublicStatusInfo(deleteStatus);

        return (deleteStatus != null);
    }

    #endregion


    #region "API examples - Internal status"

    /// <summary>
    /// Creates internal status. Called when the "Create status" button is pressed.
    /// </summary>
    private bool CreateInternalStatus()
    {
        // Create new internal status object
        InternalStatusInfo newStatus = new InternalStatusInfo();

        // Set the properties
        newStatus.InternalStatusDisplayName = "My new status";
        newStatus.InternalStatusName = "MyNewStatus";
        newStatus.InternalStatusEnabled = true;
        newStatus.InternalStatusSiteID = SiteContext.CurrentSiteID;

        // Create the internal status
        InternalStatusInfoProvider.SetInternalStatusInfo(newStatus);

        return true;
    }


    /// <summary>
    /// Gets and updates internal status. Called when the "Get and update status" button is pressed.
    /// Expects the CreateInternalStatus method to be run first.
    /// </summary>
    private bool GetAndUpdateInternalStatus()
    {
        // Get the internal status
        InternalStatusInfo updateStatus = InternalStatusInfoProvider.GetInternalStatusInfo("MyNewStatus", SiteContext.CurrentSiteName);
        if (updateStatus != null)
        {
            // Update the properties
            updateStatus.InternalStatusDisplayName = updateStatus.InternalStatusDisplayName.ToLowerCSafe();

            // Update the internal status
            InternalStatusInfoProvider.SetInternalStatusInfo(updateStatus);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates internal statuses. Called when the "Get and bulk update statuses" button is pressed.
    /// Expects the CreateInternalStatus method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateInternalStatuses()
    {
        // Get the data
        var statuses = InternalStatusInfoProvider.GetInternalStatuses()
                               .OnSite(SiteContext.CurrentSiteID)
                               .WhereStartsWith("InternalStatusName", "MyNewStatus");

        var success = false;

        // Loop through the individual items
        foreach (var modifyStatus in statuses)
        {
            // Update the properties
            modifyStatus.InternalStatusDisplayName = modifyStatus.InternalStatusDisplayName.ToUpper();

            // Update the internal status
            InternalStatusInfoProvider.SetInternalStatusInfo(modifyStatus);

            success = true;
        }

        return success;
    }


    /// <summary>
    /// Deletes internal status. Called when the "Delete status" button is pressed.
    /// Expects the CreateInternalStatus method to be run first.
    /// </summary>
    private bool DeleteInternalStatus()
    {
        // Get the internal status
        InternalStatusInfo deleteStatus = InternalStatusInfoProvider.GetInternalStatusInfo("MyNewStatus", SiteContext.CurrentSiteName);

        // Delete the internal status
        InternalStatusInfoProvider.DeleteInternalStatusInfo(deleteStatus);

        return (deleteStatus != null);
    }

    #endregion


    #region "API examples - Department"

    /// <summary>
    /// Creates department. Called when the "Create department" button is pressed.
    /// </summary>
    private bool CreateDepartment()
    {
        // Create new department object
        DepartmentInfo newDepartment = new DepartmentInfo();

        // Set the properties
        newDepartment.DepartmentDisplayName = "My new department";
        newDepartment.DepartmentName = "MyNewDepartment";
        newDepartment.DepartmentSiteID = SiteContext.CurrentSiteID;

        // Create the department
        DepartmentInfoProvider.SetDepartmentInfo(newDepartment);

        return true;
    }


    /// <summary>
    /// Gets and updates department. Called when the "Get and update department" button is pressed.
    /// Expects the CreateDepartment method to be run first.
    /// </summary>
    private bool GetAndUpdateDepartment()
    {
        // Get the department
        DepartmentInfo updateDepartment = DepartmentInfoProvider.GetDepartmentInfo("MyNewDepartment", SiteContext.CurrentSiteName);
        if (updateDepartment != null)
        {
            // Update the properties
            updateDepartment.DepartmentDisplayName = updateDepartment.DepartmentDisplayName.ToLowerCSafe();

            // Update the department
            DepartmentInfoProvider.SetDepartmentInfo(updateDepartment);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates departments. Called when the "Get and bulk update departments" button is pressed.
    /// Expects the CreateDepartment method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateDepartments()
    {
        var success = false;

        // Get the data
        var departments = DepartmentInfoProvider.GetDepartments().WhereStartsWith("DepartmentName", "MyNewDepartment");

        // Loop through the individual items
        foreach (var modifyDepartment in departments)
        {
            // Update the properties
            modifyDepartment.DepartmentDisplayName = modifyDepartment.DepartmentDisplayName.ToUpper();

            // Update the department
            DepartmentInfoProvider.SetDepartmentInfo(modifyDepartment);

            success = true;
        }

        return success;
    }


    /// <summary>
    /// Deletes department. Called when the "Delete department" button is pressed.
    /// Expects the CreateDepartment method to be run first.
    /// </summary>
    private bool DeleteDepartment()
    {
        // Get the department
        DepartmentInfo deleteDepartment = DepartmentInfoProvider.GetDepartmentInfo("MyNewDepartment", SiteContext.CurrentSiteName);

        // Delete the department
        DepartmentInfoProvider.DeleteDepartmentInfo(deleteDepartment);

        return (deleteDepartment != null);
    }

    #endregion


    #region "API examples - Department user"

    /// <summary>
    /// Adds user to department. Called when the "Add user to department" button is pressed.
    /// Expects the CreateDepartment method to be run first.
    /// </summary>
    private bool AddUserToDepartment()
    {
        // Get the department
        DepartmentInfo department = DepartmentInfoProvider.GetDepartmentInfo("MyNewDepartment", SiteContext.CurrentSiteName);
        if (department != null)
        {
            // Add user to department
            UserDepartmentInfoProvider.AddUserToDepartment(department.DepartmentID, MembershipContext.AuthenticatedUser.UserID);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Removes user from department. Called when the "Remove user from department" button is pressed.
    /// Expects the AddUserToDepartment method to be run first.
    /// </summary>
    private bool RemoveUserFromDepartment()
    {
        // Get the department
        DepartmentInfo department = DepartmentInfoProvider.GetDepartmentInfo("MyNewDepartment", SiteContext.CurrentSiteName);
        if (department != null)
        {
            // Get the user department
            UserDepartmentInfo deleteUserDepartment = UserDepartmentInfoProvider.GetUserDepartmentInfo(department.DepartmentID, MembershipContext.AuthenticatedUser.UserID);

            if (deleteUserDepartment != null)
            {
                // Remove user from department
                UserDepartmentInfoProvider.DeleteUserDepartmentInfo(deleteUserDepartment);

                return true;
            }
        }

        return false;
    }

    #endregion


    #region "API examples - Department default tax class"

    /// <summary>
    /// Adds tax class to department. Called when the "Add tax class to department" button is pressed.
    /// Expects CreateDepartment and CreateTaxClass methods to be run first.
    /// </summary>
    private bool AddTaxClassToDepartment()
    {
        // Get the department
        DepartmentInfo department = DepartmentInfoProvider.GetDepartmentInfo("MyNewDepartment", SiteContext.CurrentSiteName);

        // Get the tax class
        TaxClassInfo taxClass = TaxClassInfoProvider.GetTaxClassInfo("MyNewClass", SiteContext.CurrentSiteName);

        if ((department != null) && (taxClass != null))
        {
            // Add tax class to department
            DepartmentTaxClassInfoProvider.AddTaxClassToDepartment(taxClass.TaxClassID, department.DepartmentID);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Removes tax class from department. Called when the "Remove user from department" button is pressed.
    /// Expects the AddUserToDepartment method to be run first.
    /// </summary>
    private bool RemoveTaxClassFromDepartment()
    {
        // Get the department
        DepartmentInfo department = DepartmentInfoProvider.GetDepartmentInfo("MyNewDepartment", SiteContext.CurrentSiteName);

        // Get the tax class
        TaxClassInfo taxClass = TaxClassInfoProvider.GetTaxClassInfo("MyNewClass", SiteContext.CurrentSiteName);

        if ((department != null) && (taxClass != null))
        {
            // Get the department tax class
            DepartmentTaxClassInfo departmentTax = DepartmentTaxClassInfoProvider.GetDepartmentTaxClassInfo(department.DepartmentID, taxClass.TaxClassID);

            if ((departmentTax != null))
            {
                // Remove tax class from department
                DepartmentTaxClassInfoProvider.DeleteDepartmentTaxClassInfo(departmentTax);

                return true;
            }
        }

        return false;
    }

    #endregion


    #region "API examples - Shipping option"

    /// <summary>
    /// Creates shipping option. Called when the "Create option" button is pressed.
    /// </summary>
    private bool CreateShippingOption()
    {
        // Create new shipping option object
        ShippingOptionInfo newOption = new ShippingOptionInfo();

        // Set the properties
        newOption.ShippingOptionDisplayName = "My new option";
        newOption.ShippingOptionName = "MyNewOption";
        newOption.ShippingOptionSiteID = SiteContext.CurrentSiteID;
        newOption.ShippingOptionCharge = 1.0;
        newOption.ShippingOptionEnabled = true;

        // Create the shipping option
        ShippingOptionInfoProvider.SetShippingOptionInfo(newOption);

        return true;
    }


    /// <summary>
    /// Gets and updates shipping option. Called when the "Get and update option" button is pressed.
    /// Expects the CreateShippingOption method to be run first.
    /// </summary>
    private bool GetAndUpdateShippingOption()
    {
        // Get the shipping option
        ShippingOptionInfo updateOption = ShippingOptionInfoProvider.GetShippingOptionInfo("MyNewOption", SiteContext.CurrentSiteName);
        if (updateOption != null)
        {
            // Update the properties
            updateOption.ShippingOptionDisplayName = updateOption.ShippingOptionDisplayName.ToLowerCSafe();

            // Update the shipping option
            ShippingOptionInfoProvider.SetShippingOptionInfo(updateOption);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates shipping options. Called when the "Get and bulk update options" button is pressed.
    /// Expects the CreateShippingOption method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateShippingOptions()
    {
        // Get the data
        var options = ShippingOptionInfoProvider.GetShippingOptions()
                          .OnSite(SiteContext.CurrentSiteID)
                          .WhereStartsWith("ShippingOptionName", "MyNewOption");

        var success = false;

        // Loop through the individual items
        foreach (var option in options)
        {
            // Update the properties
            option.ShippingOptionDisplayName = option.ShippingOptionDisplayName.ToUpper();

            // Update the shipping option
            ShippingOptionInfoProvider.SetShippingOptionInfo(option);

            success = true;
        }

        return success;
    }


    /// <summary>
    /// Deletes shipping option. Called when the "Delete option" button is pressed.
    /// Expects the CreateShippingOption method to be run first.
    /// </summary>
    private bool DeleteShippingOption()
    {
        // Get the shipping option
        ShippingOptionInfo deleteOption = ShippingOptionInfoProvider.GetShippingOptionInfo("MyNewOption", SiteContext.CurrentSiteName);

        if (deleteOption != null)
        {
            // Delete the shipping option
            ShippingOptionInfoProvider.DeleteShippingOptionInfo(deleteOption);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Adds shipping cost to shipping option. Called when the "Add shipping cost" button is pressed.
    /// Expects the CreateShippingOption method to be run first.
    /// </summary>
    private bool AddShippingCostToOption()
    {
        // Get the shipping option
        ShippingOptionInfo option = ShippingOptionInfoProvider.GetShippingOptionInfo("MyNewOption", SiteContext.CurrentSiteName);
        if (option != null)
        {
            // Create new shipping cost object
            ShippingCostInfo cost = new ShippingCostInfo();

            // Set the properties
            cost.ShippingCostMinWeight = 10;
            cost.ShippingCostValue = 9.9;
            cost.ShippingCostShippingOptionID = option.ShippingOptionID;

            // Set the shipping cost
            ShippingCostInfoProvider.SetShippingCostInfo(cost);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Remove shipping cost from shipping option. Called when the "Remove shipping cost" button is pressed.
    /// Expects the AddShippingCostToOption method to be run first.
    /// </summary>
    private bool RemoveShippingCostFromOption()
    {
        // Get the shipping option
        ShippingOptionInfo option = ShippingOptionInfoProvider.GetShippingOptionInfo("MyNewOption", SiteContext.CurrentSiteName);
        if (option != null)
        {
            // Get the shipping cost
            ShippingCostInfo deleteCost = ShippingCostInfoProvider.GetShippingCostInfo(option.ShippingOptionID, 10);

            // Delete shipping cost
            ShippingCostInfoProvider.DeleteShippingCostInfo(deleteCost);

            return (deleteCost != null);
        }

        return false;
    }


    /// <summary>
    /// Adds tax class to shipping option. Called when the "Add tax class" button is pressed.
    /// Expects the CreateShippingOption method to be run first.
    /// </summary>
    private bool AddTaxClassToOption()
    {
        // Get the shipping option
        ShippingOptionInfo option = ShippingOptionInfoProvider.GetShippingOptionInfo("MyNewOption", SiteContext.CurrentSiteName);
        // Get the tax class
        TaxClassInfo taxClass = TaxClassInfoProvider.GetTaxClassInfo("MyNewClass", SiteContext.CurrentSiteName);

        if ((option != null) && (taxClass != null))
        {
            // Create new object
            ShippingOptionTaxClassInfo optionTax = new ShippingOptionTaxClassInfo();

            // Set the properties
            optionTax.TaxClassID = taxClass.TaxClassID;
            optionTax.ShippingOptionID = option.ShippingOptionID;

            // Set the object
            ShippingOptionTaxClassInfoProvider.SetShippingOptionTaxClassInfo(optionTax);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Remove tax class from shipping option. Called when the "Remove tax class" button is pressed.
    /// Expects the AddTaxClassToOption method to be run first.
    /// </summary>
    private bool RemoveTaxClassFromOption()
    {
        // Get the shipping option
        ShippingOptionInfo option = ShippingOptionInfoProvider.GetShippingOptionInfo("MyNewOption", SiteContext.CurrentSiteName);

        // Get the tax class
        TaxClassInfo taxClass = TaxClassInfoProvider.GetTaxClassInfo("MyNewClass", SiteContext.CurrentSiteName);

        if ((option != null) && (taxClass != null))
        {
            // Get the object
            ShippingOptionTaxClassInfo deleteOptionTax = ShippingOptionTaxClassInfoProvider.GetShippingOptionTaxClassInfo(option.ShippingOptionID, taxClass.TaxClassID);

            // Delete object
            ShippingOptionTaxClassInfoProvider.DeleteShippingOptionTaxClassInfo(deleteOptionTax);

            return (deleteOptionTax != null);
        }

        return false;
    }

    #endregion


    #region "API examples - Payment method"

    /// <summary>
    /// Creates payment method. Called when the "Create method" button is pressed.
    /// </summary>
    private bool CreatePaymentMethod()
    {
        // Create new payment method object
        PaymentOptionInfo newMethod = new PaymentOptionInfo();

        // Set the properties
        newMethod.PaymentOptionDisplayName = "My new method";
        newMethod.PaymentOptionName = "MyNewMethod";
        newMethod.PaymentOptionSiteID = SiteContext.CurrentSiteID;
        newMethod.PaymentOptionEnabled = true;

        // Create the payment method
        PaymentOptionInfoProvider.SetPaymentOptionInfo(newMethod);

        return true;
    }


    /// <summary>
    /// Gets and updates payment method. Called when the "Get and update method" button is pressed.
    /// Expects the CreatePaymentMethod method to be run first.
    /// </summary>
    private bool GetAndUpdatePaymentMethod()
    {
        // Get the payment method
        PaymentOptionInfo updateMethod = PaymentOptionInfoProvider.GetPaymentOptionInfo("MyNewMethod", SiteContext.CurrentSiteName);
        if (updateMethod != null)
        {
            // Update the properties
            updateMethod.PaymentOptionDisplayName = updateMethod.PaymentOptionDisplayName.ToLowerCSafe();

            // Update the payment method
            PaymentOptionInfoProvider.SetPaymentOptionInfo(updateMethod);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates payment methods. Called when the "Get and bulk update methods" button is pressed.
    /// Expects the CreatePaymentMethod method to be run first.
    /// </summary>
    private bool GetAndBulkUpdatePaymentMethods()
    {
        // Get the data
        var methods = PaymentOptionInfoProvider.GetPaymentOptions()
                          .OnSite(SiteContext.CurrentSiteID)
                          .WhereStartsWith("PaymentOptionName", "MyNewMethod");

        var success = false;

        // Loop through the individual items
        foreach (var method in methods)
        {
            // Update the properties
            method.PaymentOptionDisplayName = method.PaymentOptionDisplayName.ToUpper();

            // Update the payment method
            PaymentOptionInfoProvider.SetPaymentOptionInfo(method);

            success = true;
        }

        return success;
    }


    /// <summary>
    /// Deletes payment method. Called when the "Delete method" button is pressed.
    /// Expects the CreatePaymentMethod method to be run first.
    /// </summary>
    private bool DeletePaymentMethod()
    {
        // Get the payment method
        PaymentOptionInfo deleteMethod = PaymentOptionInfoProvider.GetPaymentOptionInfo("MyNewMethod", SiteContext.CurrentSiteName);
        if (deleteMethod != null)
        {
            // Delete the payment method
            PaymentOptionInfoProvider.DeletePaymentOptionInfo(deleteMethod.PaymentOptionID);

            return true;
        }

        return false;
    }

    #endregion


    #region "API examples - Manufacturer"

    /// <summary>
    /// Creates manufacturer. Called when the "Create manufacturer" button is pressed.
    /// </summary>
    private bool CreateManufacturer()
    {
        // Create new manufacturer object
        ManufacturerInfo newManufacturer = new ManufacturerInfo();

        // Set the properties
        newManufacturer.ManufacturerDisplayName = "My new manufacturer";
        newManufacturer.ManufacturerName = "MyNewManufacturer";
        newManufacturer.ManufacturerHomepage = "www.mynewmanufacturer.com";
        newManufacturer.ManufacturerSiteID = SiteContext.CurrentSiteID;
        newManufacturer.ManufacturerEnabled = true;

        // Create the manufacturer
        ManufacturerInfoProvider.SetManufacturerInfo(newManufacturer);

        return true;
    }


    /// <summary>
    /// Gets and updates manufacturer. Called when the "Get and update manufacturer" button is pressed.
    /// Expects the CreateManufacturer method to be run first.
    /// </summary>
    private bool GetAndUpdateManufacturer()
    {
        // Get the manufacturer
        ManufacturerInfo updateManufacturer = ManufacturerInfoProvider.GetManufacturerInfo("MyNewManufacturer", SiteContext.CurrentSiteName);
        if (updateManufacturer != null)
        {
            // Update the properties
            updateManufacturer.ManufacturerDisplayName = updateManufacturer.ManufacturerDisplayName.ToLowerCSafe();

            // Update the manufacturer
            ManufacturerInfoProvider.SetManufacturerInfo(updateManufacturer);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates manufacturers. Called when the "Get and bulk update manufacturers" button is pressed.
    /// Expects the CreateManufacturer method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateManufacturers()
    {
        // Get the data
        var manufacturers = ManufacturerInfoProvider.GetManufacturers()
                                 .OnSite(SiteContext.CurrentSiteID)
                                 .WhereStartsWith("ManufacturerName", "MyNewManufacturer");
        var success = false;

        // Loop through the individual items
        foreach (var modifyManufacturer in manufacturers)
        {
            // Update the properties
            modifyManufacturer.ManufacturerDisplayName = modifyManufacturer.ManufacturerDisplayName.ToUpper();

            // Update the manufacturer
            ManufacturerInfoProvider.SetManufacturerInfo(modifyManufacturer);

            success = true;
        }

        return success;
    }


    /// <summary>
    /// Deletes manufacturer. Called when the "Delete manufacturer" button is pressed.
    /// Expects the CreateManufacturer method to be run first.
    /// </summary>
    private bool DeleteManufacturer()
    {
        // Get the manufacturer
        ManufacturerInfo deleteManufacturer = ManufacturerInfoProvider.GetManufacturerInfo("MyNewManufacturer", SiteContext.CurrentSiteName);

        // Delete the manufacturer
        ManufacturerInfoProvider.DeleteManufacturerInfo(deleteManufacturer);

        return (deleteManufacturer != null);
    }

    #endregion


    #region "API examples - Supplier"

    /// <summary>
    /// Creates supplier. Called when the "Create supplier" button is pressed.
    /// </summary>
    private bool CreateSupplier()
    {
        // Create new supplier object
        SupplierInfo newSupplier = new SupplierInfo();

        // Set the properties
        newSupplier.SupplierDisplayName = "My new supplier";
        newSupplier.SupplierName = "MyNewSupplier";
        newSupplier.SupplierEmail = "mynewsupplier@supplier.com";
        newSupplier.SupplierSiteID = SiteContext.CurrentSiteID;
        newSupplier.SupplierPhone = "";
        newSupplier.SupplierFax = "";
        newSupplier.SupplierEnabled = true;

        // Create the supplier
        SupplierInfoProvider.SetSupplierInfo(newSupplier);

        return true;
    }


    /// <summary>
    /// Gets and updates supplier. Called when the "Get and update supplier" button is pressed.
    /// Expects the CreateSupplier method to be run first.
    /// </summary>
    private bool GetAndUpdateSupplier()
    {
        // Get the supplier
        SupplierInfo updateSupplier = SupplierInfoProvider.GetSupplierInfo("MyNewSupplier", SiteContext.CurrentSiteName);
        if (updateSupplier != null)
        {
            // Update the properties
            updateSupplier.SupplierDisplayName = updateSupplier.SupplierDisplayName.ToLowerCSafe();

            // Update the supplier
            SupplierInfoProvider.SetSupplierInfo(updateSupplier);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates suppliers. Called when the "Get and bulk update suppliers" button is pressed.
    /// Expects the CreateSupplier method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateSuppliers()
    {
        // Get the data
        var suppliers = SupplierInfoProvider.GetSuppliers()
                            .OnSite(SiteContext.CurrentSiteID)
                            .WhereStartsWith("SupplierName", "MyNewSupplier");

        var success = false;

        // Loop through the individual items
        foreach (var modifySupplier in suppliers)
        {
            // Update the properties
            modifySupplier.SupplierDisplayName = modifySupplier.SupplierDisplayName.ToUpper();

            // Update the supplier
            SupplierInfoProvider.SetSupplierInfo(modifySupplier);

            success = true;
        }

        return success;
    }


    /// <summary>
    /// Deletes supplier. Called when the "Delete supplier" button is pressed.
    /// Expects the CreateSupplier method to be run first.
    /// </summary>
    private bool DeleteSupplier()
    {
        // Get the supplier
        SupplierInfo deleteSupplier = SupplierInfoProvider.GetSupplierInfo("MyNewSupplier", SiteContext.CurrentSiteName);

        // Delete the supplier
        SupplierInfoProvider.DeleteSupplierInfo(deleteSupplier);

        return (deleteSupplier != null);
    }

    #endregion


    #region "API examples - Product"

    /// <summary>
    /// Creates product. Called when the "Create product" button is pressed.
    /// </summary>
    private bool CreateProduct()
    {
        // Get the deparment
        DepartmentInfo department = DepartmentInfoProvider.GetDepartmentInfo("MyNewDepartment", SiteContext.CurrentSiteName);

        // Create new product object
        SKUInfo newProduct = new SKUInfo();

        // Set the properties
        newProduct.SKUName = "MyNewProduct";
        newProduct.SKUPrice = 120;
        newProduct.SKUEnabled = true;
        if (department != null)
        {
            newProduct.SKUDepartmentID = department.DepartmentID;
        }
        newProduct.SKUSiteID = SiteContext.CurrentSiteID;

        // Create the product
        SKUInfoProvider.SetSKUInfo(newProduct);

        return true;
    }


    /// <summary>
    /// Gets and updates product. Called when the "Get and update product" button is pressed.
    /// Expects the CreateProduct method to be run first.
    /// </summary>
    private bool GetAndUpdateProduct()
    {
        // Get the product
        var updateProduct = SKUInfoProvider.GetSKUs()
                                .WhereStartsWith("SKUName", "MyNewProduct")
                                .WhereNull("SKUOptionCategoryID")
                                .FirstOrDefault();

        if (updateProduct != null)
        {
            // Update the product name
            updateProduct.SKUName = updateProduct.SKUName.ToLowerCSafe();

            // Update the product
            SKUInfoProvider.SetSKUInfo(updateProduct);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates products. Called when the "Get and bulk update products" button is pressed.
    /// Expects the CreateProduct method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateProducts()
    {
        // Get the data
        var products = SKUInfoProvider.GetSKUs()
                                .WhereStartsWith("SKUName", "MyNewProduct")
                                .WhereNull("SKUOptionCategoryID");

        var success = false;

        // Loop through the individual items
        foreach (var modifyProduct in products)
        {
            // Update the product name
            modifyProduct.SKUName = modifyProduct.SKUName.ToUpper();

            // Update the product
            SKUInfoProvider.SetSKUInfo(modifyProduct);

            success = true;
        }

        return success;
    }


    /// <summary>
    /// Deletes product. Called when the "Delete product" button is pressed.
    /// Expects the CreateProduct method to be run first.
    /// </summary>
    private bool DeleteProduct()
    {
        // Get the product
        var deleteProduct = SKUInfoProvider.GetSKUs()
                                .WhereStartsWith("SKUName", "MyNewProduct")
                                .WhereNull("SKUOptionCategoryID")
                                .FirstOrDefault();

        if (deleteProduct != null)
        {
            // Delete the product
            SKUInfoProvider.DeleteSKUInfo(deleteProduct);

            return true;
        }

        return false;
    }

    #endregion


    #region "API examples - Product document"

    /// <summary>
    /// Creates product document. Called when the "Create document" button is pressed.
    /// Expects the CreateProduct method to be run first.
    /// </summary>
    private bool CreateProductDocument()
    {
        // Get the product
        var product = SKUInfoProvider.GetSKUs()
                                .WhereStartsWith("SKUName", "MyNewProduct")
                                .FirstOrDefault();

        TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);

        // Get the parent document
        TreeNode parent = tree.SelectSingleNode(SiteContext.CurrentSiteName, "/Products", TreeProvider.ALL_CULTURES, true, "cms.menuitem");

        if ((parent != null) && (product != null))
        {
            // Create a new product document 
            SKUTreeNode node = (SKUTreeNode)TreeNode.New("CMS.Product", tree);

            // Set the document properties
            node.DocumentSKUName = "MyNewProduct";
            node.DocumentCulture = LocalizationContext.PreferredCultureCode;

            // Assign product to document
            node.NodeSKUID = product.SKUID;

            // Save the product document
            node.Insert(parent);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and updates product document. Called when the "Get and update document" button is pressed.
    /// Expects the CreateProductDocument method to be run first.
    /// </summary>
    private bool GetAndUpdateProductDocument()
    {
        TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);

        // Get the product document
        SKUTreeNode node = (SKUTreeNode)tree.SelectSingleNode(SiteContext.CurrentSiteName, "/Products/MyNewProduct", null, true);

        if (node != null)
        {
            // Set the properties
            node.DocumentSKUDescription = "Product was updated.";
            node.DocumentName = node.DocumentName.ToLowerCSafe();

            // Update the product document
            DocumentHelper.UpdateDocument(node, tree);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Deletes product document. Called when the "Delete document" button is pressed.
    /// Expects the CreateProductDocument method to be run first.
    /// </summary>
    private bool DeleteProductDocument()
    {
        TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);

        // Get the product document
        TreeNode node = tree.SelectSingleNode(SiteContext.CurrentSiteName, "/Products/MyNewProduct", null, true);

        if (node != null)
        {
            // Delete the product document
            DocumentHelper.DeleteDocument(node, tree, true, true);

            return true;
        }

        return false;
    }

    #endregion


    #region "API examples - Product - Membership"

    /// <summary>
    /// Creates membership product. Called when the "Create membership product" button is pressed.
    /// </summary>
    private bool CreateMembershipProduct()
    {
        // Get the site
        string siteName = SiteContext.CurrentSiteName;

        // Get the department
        DepartmentInfo department = DepartmentInfoProvider.GetDepartmentInfo("MyNewDepartment", siteName);

        // Get the membership group
        MembershipInfo membership = MembershipInfoProvider.GetMembershipInfo("MyNewMembership", siteName);

        // Create a membership if "MyNewMembership" does not exist
        if (membership == null)
        {
            // Set the properties
            membership = new MembershipInfo();
            membership.MembershipDisplayName = "My New Membership";
            membership.MembershipName = "MyNewMembership";
            membership.MembershipSiteID = SiteContext.CurrentSiteID;

            // Set new membership object
            MembershipInfoProvider.SetMembershipInfo(membership);
        }

        // Create new product object
        SKUInfo newProduct = new SKUInfo();

        // Set the properties
        if (department != null)
        {
            newProduct.SKUDepartmentID = department.DepartmentID;
        }
        newProduct.SKUName = "MyNewMembershipProduct";
        newProduct.SKUPrice = 69;
        newProduct.SKUEnabled = true;
        newProduct.SKUSiteID = SiteContext.CurrentSiteID;
        newProduct.SKUProductType = SKUProductTypeEnum.Membership;
        newProduct.SKUMembershipGUID = membership.MembershipGUID;
        newProduct.SKUValidity = ValidityEnum.Months;
        newProduct.SKUValidFor = 3;

        // Create the product
        SKUInfoProvider.SetSKUInfo(newProduct);

        return true;
    }


    /// <summary>
    /// Deletes membership product and membership group. Called when the "Delete membership product" button is pressed.
    /// Expects the CreateMembershipProduct method to be run first.
    /// </summary>
    private bool DeleteMembershipProduct()
    {
        bool productFound = false;

        // Get the data
        var products = SKUInfoProvider.GetSKUs()
                           .WhereStartsWith("SKUName", "MyNewMembershipProduct")
                           .WhereNull("SKUOptionCategoryID");

        // Loop through the individual items
        foreach (var deleteProduct in products)
        {
            // Delete the membership product
            SKUInfoProvider.DeleteSKUInfo(deleteProduct);

            productFound = true;
        }

        // Get the membership group
        var membership = MembershipInfoProvider.GetMembershipInfo("MyNewMembership", SiteContext.CurrentSiteName);

        // Delete membership group
        if (membership != null)
        {
            // Delete the membership group
            MembershipInfoProvider.DeleteMembershipInfo(membership);
        }

        return productFound;
    }

    #endregion


    #region "API examples - Product - eProduct"

    /// <summary>
    /// Creates an eProduct. Called when the "Create eProduct" button is pressed.
    /// </summary>
    private bool CreateEProduct()
    {
        // Get the department
        DepartmentInfo department = DepartmentInfoProvider.GetDepartmentInfo("MyNewDepartment", SiteContext.CurrentSiteName);

        // Create a new product object
        SKUInfo newProduct = new SKUInfo();

        // Set the properties
        if (department != null)
        {
            newProduct.SKUDepartmentID = department.DepartmentID;
        }
        newProduct.SKUName = "MyNewEProduct";
        newProduct.SKUPrice = 169;
        newProduct.SKUEnabled = true;
        newProduct.SKUSiteID = SiteContext.CurrentSiteID;
        newProduct.SKUProductType = SKUProductTypeEnum.EProduct;
        newProduct.SKUValidity = ValidityEnum.Until;
        newProduct.SKUValidUntil = DateTime.Today.AddDays(10d);

        // Create an eProduct
        SKUInfoProvider.SetSKUInfo(newProduct);

        // Path of a file to be uploaded
        string eProductFile = Server.MapPath("Files/file.png");

        // Create and set a metafile to the eProduct         
        MetaFileInfo metafile = new MetaFileInfo(eProductFile, newProduct.SKUID, "ecommerce.sku", "E-product");
        MetaFileInfoProvider.SetMetaFileInfo(metafile);

        // Create and set a SKUFile to the eProduct 
        SKUFileInfo skuFile = new SKUFileInfo();
        skuFile.FileSKUID = newProduct.SKUID;
        skuFile.FilePath = "~/getmetafile/" + metafile.MetaFileGUID.ToString().ToLower() + "/" + metafile.MetaFileName + ".aspx";
        skuFile.FileType = "MetaFile";
        skuFile.FileName = metafile.MetaFileName;
        skuFile.FileMetaFileGUID = metafile.MetaFileGUID;
        SKUFileInfoProvider.SetSKUFileInfo(skuFile);

        return true;
    }


    /// <summary>
    /// Deletes eProduct and its metafiles. Called when the "Delete eProduct" button is pressed.
    /// Expects the CreateEProduct method to be run first.
    /// </summary>
    private bool DeleteEProduct()
    {
        // Get the data
        var products = SKUInfoProvider.GetSKUs().WhereStartsWith("SKUName", "MyNewEProduct");

        var success = false;

        // Loop through the individual items
        foreach (var deleteEProduct in products)
        {
            // Get SKUfiles assigned to an eProduct
            var deleteFiles = SKUFileInfoProvider.GetSKUFiles().WhereEquals("FileSKUID", deleteEProduct.SKUID);

            // Delete SKUFiles
            foreach (var deleteSKUFile in deleteFiles)
            {
                SKUFileInfoProvider.DeleteSKUFileInfo(deleteSKUFile);
            }

            // Delete metafile assigned to an eProduct
            MetaFileInfoProvider.DeleteFiles(deleteEProduct.SKUID, "ecommerce.sku");

            // Delete an eProduct
            SKUInfoProvider.DeleteSKUInfo(deleteEProduct);

            success = true;
        }

        return success;
    }

    #endregion


    #region "API examples - Product - donation"

    /// <summary>
    /// Creates a donation. Called when the "Create donation" button is pressed.
    /// </summary>
    private bool CreateDonation()
    {
        // Get the department
        DepartmentInfo department = DepartmentInfoProvider.GetDepartmentInfo("MyNewDepartment", SiteContext.CurrentSiteName);

        // Create a new product object
        SKUInfo newProduct = new SKUInfo();

        // Set the properties
        if (department != null)
        {
            newProduct.SKUDepartmentID = department.DepartmentID;
        }
        newProduct.SKUName = "MyNewDonationProduct";
        newProduct.SKUPrice = 169;
        newProduct.SKUEnabled = true;
        newProduct.SKUSiteID = SiteContext.CurrentSiteID;
        newProduct.SKUProductType = SKUProductTypeEnum.Donation;
        newProduct.SKUPrivateDonation = true;
        newProduct.SKUMaxPrice = 200;
        newProduct.SKUMinPrice = 100;

        // Create a donation
        SKUInfoProvider.SetSKUInfo(newProduct);

        return true;
    }


    /// <summary>
    /// Deletes donation product. Called when the "Delete donation" button is pressed.
    /// </summary>
    private bool DeleteDonation()
    {
        // Get the data
        var products = SKUInfoProvider.GetSKUs()
                               .WhereStartsWith("SKUName", "MyNewDonationProduct")
                               .WhereNull("SKUOptionCategoryID");

        var success = false;

        // Loop through the individual items
        foreach (var deleteProduct in products)
        {
            // Delete the donation product
            SKUInfoProvider.DeleteSKUInfo(deleteProduct);

            success = true;
        }

        return success;
    }

    #endregion


    #region "API examples - Product - bundle"

    /// <summary>
    /// Creates a bundle product. Called when the "Create bundle" button is pressed.
    /// </summary>
    private bool CreateBundle()
    {
        // Get the product
        var product = SKUInfoProvider.GetSKUs()
                                .WhereStartsWith("SKUName", "MyNewProduct")
                                .FirstOrDefault();

        if (product != null)
        {
            // Get the department
            DepartmentInfo department = DepartmentInfoProvider.GetDepartmentInfo("MyNewDepartment", SiteContext.CurrentSiteName);

            // Create a new bundle product object
            SKUInfo newBundle = new SKUInfo
                                {
                                    SKUName = "MyNewBundleProduct",
                                    SKUPrice = 50,
                                    SKUEnabled = true,
                                    SKUSiteID = SiteContext.CurrentSiteID,
                                    SKUProductType = SKUProductTypeEnum.Bundle,
                                    SKUBundleInventoryType = BundleInventoryTypeEnum.RemoveBundle
                                };

            // Set the properties
            if (department != null)
            {
                newBundle.SKUDepartmentID = department.DepartmentID;

            }

            // Create the bundle
            SKUInfoProvider.SetSKUInfo(newBundle);

            // Add the product to the bundle
            BundleInfoProvider.AddSKUToBundle(newBundle.SKUID, product.SKUID);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Deletes bundle product and its dependecies. Called when the "Delete bundle" button is pressed.
    /// </summary>
    private bool DeleteBundle()
    {
        // Get the data
        var product = SKUInfoProvider.GetSKUs()
                           .WhereStartsWith("SKUName", "MyNewBundleProduct")
                           .WhereNull("SKUOptionCategoryID")
                           .FirstOrDefault();

        // Delete bundle product
        if (product != null)
        {
            // Get the product
            var bundleProducts = BundleInfoProvider.GetBundles().WhereEquals("BundleID", product.SKUID);
            foreach (var bundleProduct in bundleProducts)
            {
                BundleInfoProvider.DeleteBundleInfo(bundleProduct);
            }

            // Delete the bundle product
            SKUInfoProvider.DeleteSKUInfo(product);

            return true;
        }

        return false;
    }

    #endregion


    #region "API examples - Shopping cart"

    /// <summary>
    /// Adds product to shopping cart. Called when the "Add product to shopping cart" button is pressed.
    /// Expects the CreateProduct method to be run first.
    /// </summary>
    private bool AddProductToShoppingCart()
    {
        // Get the data
        var product = SKUInfoProvider.GetSKUs()
                           .WhereStartsWith("SKUName", "MyNewProduct")
                           .WhereNull("SKUOptionCategoryID")
                           .FirstOrDefault();

        if (product != null)
        {
            // Get current shopping cart
            ShoppingCartInfo cart = ECommerceContext.CurrentShoppingCart;

            // Ensure cart in database            
            ShoppingCartInfoProvider.SetShoppingCartInfo(cart);

            // Add item to cart object
            ShoppingCartItemParameters param = new ShoppingCartItemParameters(product.SKUID, 1);
            ShoppingCartItemInfo cartItem = cart.SetShoppingCartItem(param);

            // Save item to database
            ShoppingCartItemInfoProvider.SetShoppingCartItemInfo(cartItem);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Updates shopping cart item units. Called when the "Update itewm units" button is pressed.
    /// Expects the AddProductToShoppingCart methods to be run first.
    /// </summary>
    private bool UpdateShoppingCartItemUnits()
    {
        // Get the data
        var product = SKUInfoProvider.GetSKUs()
                           .WhereStartsWith("SKUName", "MyNewProduct")
                           .WhereNull("SKUOptionCategoryID")
                           .FirstOrDefault();

        if (product != null)
        {

            ShoppingCartItemInfo item = null;

            // Get current shopping cart
            ShoppingCartInfo cart = ECommerceContext.CurrentShoppingCart;

            // Loop through the individual items
            foreach (var cartItem in cart.CartItems)
            {
                if (cartItem.SKUID == product.SKUID)
                {
                    // Remember found item
                    item = cartItem;
                    break;
                }
            }

            if (item != null)
            {
                // Update item
                ShoppingCartItemParameters param = new ShoppingCartItemParameters(product.SKUID, 2);
                item = cart.SetShoppingCartItem(param);

                // Ensure cart in database
                ShoppingCartInfoProvider.SetShoppingCartInfo(cart);

                // Update item in database
                ShoppingCartItemInfoProvider.SetShoppingCartItemInfo(item);

                return true;
            }

            return false;
        }

        return false;
    }


    /// <summary>
    /// Removes product from shopping cart. Called when the "Remove product from shopping cart" button is pressed.
    /// Expects the AddProductToShopping cart method to be run first.
    /// </summary>
    private bool RemoveProductFromShoppingCart()
    {
        // Get current shopping cart
        ShoppingCartInfo cart = ECommerceContext.CurrentShoppingCart;

        // Get the data
        var product = SKUInfoProvider.GetSKUs()
                           .WhereStartsWith("SKUName", "MyNewProduct")
                           .WhereNull("SKUOptionCategoryID")
                           .FirstOrDefault();

        if ((cart != null) && (product != null))
        {
            var item = ShoppingCartItemInfoProvider.GetShoppingCartItems()
                           .WhereEquals("SKUID", product.SKUID)
                           .WhereEquals("ShoppingCartID", cart.ShoppingCartID).FirstOrDefault();

            if (item != null)
            {
                // Remove item from cart object
                cart.RemoveShoppingCartItem(item.CartItemID);

                // Remove item form database
                ShoppingCartItemInfoProvider.DeleteShoppingCartItemInfo(item);

                return true;
            }
        }

        return false;
    }

    #endregion


    #region "API examples - Product tax class"

    /// <summary>
    /// Adds tax class to product. Called when the "Create class" button is pressed.
    /// Expects the CreateTaxClass and CreateProduct methods to be run first.
    /// </summary>
    private bool AddTaxClassToProduct()
    {
        // Get the data
        var product = SKUInfoProvider.GetSKUs()
                           .WhereStartsWith("SKUName", "MyNew")
                           .FirstOrDefault();

        // Get the tax class
        TaxClassInfo taxClass = TaxClassInfoProvider.GetTaxClassInfo("MyNewClass", SiteContext.CurrentSiteName);

        if ((product != null) && (taxClass != null))
        {
            // Add tax class to product
            SKUTaxClassInfoProvider.AddTaxClassToSKU(taxClass.TaxClassID, product.SKUID);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Removes tax class from product. Called when the "Remove tax class from product" button is pressed.
    /// Expects the AddTaxClassToProduct method to be run first.
    /// </summary>
    private bool RemoveTaxClassFromProduct()
    {
        // Get the tax class
        TaxClassInfo taxClass = TaxClassInfoProvider.GetTaxClassInfo("MyNewClass", SiteContext.CurrentSiteName);

        // Get the data
        var product = SKUInfoProvider.GetSKUs()
                           .WhereStartsWith("SKUName", "MyNew")
                           .FirstOrDefault();

        if ((product != null) && (taxClass != null))
        {
            // Get the tax class added to product
            SKUTaxClassInfo skuTaxClass = SKUTaxClassInfoProvider.GetSKUTaxClassInfo(taxClass.TaxClassID, product.SKUID);

            // Remove tax class from product
            SKUTaxClassInfoProvider.DeleteSKUTaxClassInfo(skuTaxClass);

            return (skuTaxClass != null);
        }

        return false;
    }

    #endregion


    #region "API examples - Volume discount"

    /// <summary>
    /// Creates volume discount. Called when the "Create discount" button is pressed.
    /// Expects the CreateProduct method to be run first.
    /// </summary>
    private bool CreateVolumeDiscount()
    {
        // Get the data
        var product = SKUInfoProvider.GetSKUs()
                           .WhereStartsWith("SKUName", "MyNew")
                           .FirstOrDefault();

        if (product != null)
        {
            // Create new volume discount object
            VolumeDiscountInfo newDiscount = new VolumeDiscountInfo();

            // Set the properties
            newDiscount.VolumeDiscountMinCount = 100;
            newDiscount.VolumeDiscountValue = 20;
            newDiscount.VolumeDiscountSKUID = product.SKUID;
            newDiscount.VolumeDiscountIsFlatValue = false;

            // Create the volume discount
            VolumeDiscountInfoProvider.SetVolumeDiscountInfo(newDiscount);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and updates volume discount. Called when the "Get and update discount" button is pressed.
    /// Expects the CreateVolumeDiscount method to be run first.
    /// </summary>
    private bool GetAndUpdateVolumeDiscount()
    {
        // Get the data
        var product = SKUInfoProvider.GetSKUs()
                           .WhereStartsWith("SKUName", "MyNew")
                           .FirstOrDefault();

        if (product != null)
        {
            // Get the volume discount
            var discount = VolumeDiscountInfoProvider.GetVolumeDiscounts(product.SKUID).FirstOrDefault();
            if (discount != null)
            {
                // Update the value
                discount.VolumeDiscountMinCount = 800;

                // Update the volume discount
                VolumeDiscountInfoProvider.SetVolumeDiscountInfo(discount);

                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates volume discounts. Called when the "Get and bulk update discounts" button is pressed.
    /// Expects the CreateVolumeDiscount method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateVolumeDiscounts()
    {
        // Get the data
        var product = SKUInfoProvider.GetSKUs()
                           .WhereStartsWith("SKUName", "MyNew")
                           .FirstOrDefault();

        var success = false;

        if (product != null)
        {
            // Get the volume discount
            var discounts = VolumeDiscountInfoProvider.GetVolumeDiscounts(product.SKUID);

            foreach (var discount in discounts)
            {
                // Update the value
                discount.VolumeDiscountMinCount = 500;

                // Update the volume discount
                VolumeDiscountInfoProvider.SetVolumeDiscountInfo(discount);

                success = true;
            }
        }

        return success;
    }


    /// <summary>
    /// Deletes volume discount. Called when the "Delete discount" button is pressed.
    /// Expects the CreateVolumeDiscount method to be run first.
    /// </summary>
    private bool DeleteVolumeDiscount()
    {
        // Get the data
        var product = SKUInfoProvider.GetSKUs()
                           .WhereStartsWith("SKUName", "MyNew")
                           .FirstOrDefault();

        if (product != null)
        {
            // Get the volume discount
            var discount = VolumeDiscountInfoProvider.GetVolumeDiscounts(product.SKUID).FirstOrDefault();
            if (discount != null)
            {
                // Delete the volume discount
                VolumeDiscountInfoProvider.DeleteVolumeDiscountInfo(discount);

                return true;
            }
        }

        return false;
    }

    #endregion


    #region "API examples - Product option category"

    /// <summary>
    /// Creates option category. Called when the "Create category" button is pressed.
    /// </summary>
    private bool CreateOptionCategory()
    {
        // Create new option category object
        OptionCategoryInfo newCategory = new OptionCategoryInfo();

        // Set the properties
        newCategory.CategoryDisplayName = "My new category";
        newCategory.CategoryName = "MyNewCategory";
        newCategory.CategoryType = OptionCategoryTypeEnum.Products;
        newCategory.CategorySelectionType = OptionCategorySelectionTypeEnum.Dropdownlist;
        newCategory.CategoryDisplayPrice = true;
        newCategory.CategoryEnabled = true;
        newCategory.CategoryDefaultRecord = "";
        newCategory.CategorySiteID = SiteContext.CurrentSiteID;

        // Create the option category
        OptionCategoryInfoProvider.SetOptionCategoryInfo(newCategory);

        return true;
    }


    /// <summary>
    /// Gets and updates option category. Called when the "Get and update category" button is pressed.
    /// Expects the CreateOptionCategory method to be run first.
    /// </summary>
    private bool GetAndUpdateOptionCategory()
    {
        // Get the option category
        OptionCategoryInfo updateCategory = OptionCategoryInfoProvider.GetOptionCategoryInfo("MyNewCategory", SiteContext.CurrentSiteName);
        if (updateCategory != null)
        {
            // Update the properties
            updateCategory.CategoryDisplayName = updateCategory.CategoryDisplayName.ToLowerCSafe();

            // Update the option category
            OptionCategoryInfoProvider.SetOptionCategoryInfo(updateCategory);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates option categories. Called when the "Get and bulk update categories" button is pressed.
    /// Expects the CreateOptionCategory method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateOptionCategories()
    {
        // Get the data
        var categories = OptionCategoryInfoProvider.GetOptionCategories()
                             .WhereStartsWith("CategoryName", "MyNewCategory");

        var success = false;

        // Loop through the individual items
        foreach (var modifyCategory in categories)
        {
            // Update the properties
            modifyCategory.CategoryDisplayName = modifyCategory.CategoryDisplayName.ToUpper();

            // Update the option category
            OptionCategoryInfoProvider.SetOptionCategoryInfo(modifyCategory);

            success = true;
        }

        return success;
    }


    /// <summary>
    /// Deletes option category. Called when the "Delete category" button is pressed.
    /// Expects the CreateOptionCategory method to be run first.
    /// </summary>
    private bool DeleteOptionCategory()
    {
        // Get the option category
        OptionCategoryInfo deleteCategory = OptionCategoryInfoProvider.GetOptionCategoryInfo("MyNewCategory", SiteContext.CurrentSiteName);

        // Delete the option category
        OptionCategoryInfoProvider.DeleteOptionCategoryInfo(deleteCategory);

        return (deleteCategory != null);
    }

    #endregion


    #region "API examples - Option"

    /// <summary>
    /// Creates option. Called when the "Create option" button is pressed.
    /// Expects the CreateOptionCategory method to be run first.
    /// </summary>
    private bool CreateOption()
    {
        // Get the department
        DepartmentInfo department = DepartmentInfoProvider.GetDepartmentInfo("MyNewDepartment", SiteContext.CurrentSiteName);

        // Get the option category
        OptionCategoryInfo category = OptionCategoryInfoProvider.GetOptionCategoryInfo("MyNewCategory", SiteContext.CurrentSiteName);

        if ((department != null) && (category != null))
        {
            // Create new product option object
            SKUInfo newOption = new SKUInfo();

            // Set the properties
            newOption.SKUName = "MyNewProductOption";
            newOption.SKUPrice = 199;
            newOption.SKUEnabled = true;
            newOption.SKUDepartmentID = department.DepartmentID;
            newOption.SKUOptionCategoryID = category.CategoryID;
            newOption.SKUSiteID = SiteContext.CurrentSiteID;
            newOption.SKUProductType = SKUProductTypeEnum.Product;

            // Create the product option
            SKUInfoProvider.SetSKUInfo(newOption);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and updates option. Called when the "Get and update option" button is pressed.
    /// Expects the CreateOption method to be run first.
    /// </summary>
    private bool GetAndUpdateOption()
    {
        // Get the product option
        var option = SKUInfoProvider.GetSKUs()
                          .WhereStartsWith("SKUName", "MyNewProduct")
                          .WhereNotNull("SKUOptionCategoryID")
                          .FirstOrDefault();

        if (option != null)
        {
            // Update the product option
            option.SKUName = option.SKUName.ToLowerCSafe();

            // Update the product option
            SKUInfoProvider.SetSKUInfo(option);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates options. Called when the "Get and bulk update options" button is pressed.
    /// Expects the CreateOption method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateOptions()
    {
        // Get the data
        var options = SKUInfoProvider.GetSKUs()
                           .WhereStartsWith("SKUName", "MyNewProduct")
                           .WhereNotNull("SKUOptionCategoryID");

        var success = false;

        // Loop through the individual items
        foreach (var modifyOption in options)
        {
            // Update the properties
            modifyOption.SKUName = modifyOption.SKUName.ToUpper();

            // Update the product option
            SKUInfoProvider.SetSKUInfo(modifyOption);

            success = true;
        }

        return success;
    }


    /// <summary>
    /// Deletes option. Called when the "Delete option" button is pressed.
    /// Expects the CreateOption method to be run first.
    /// </summary>
    private bool DeleteOption()
    {
        // Get the product option
        var option = SKUInfoProvider.GetSKUs()
                          .WhereStartsWith("SKUName", "MyNewProduct")
                          .WhereNotNull("SKUOptionCategoryID")
                          .FirstOrDefault();

        if (option != null)
        {
            // Delete the product option
            SKUInfoProvider.DeleteSKUInfo(option);

            return true;
        }

        return false;
    }

    #endregion


    #region "API examples - Option category on product"

    /// <summary>
    /// Adds option category to product. Called when the "Add category to product" button is pressed.
    /// Expects the CreateOptionCategory and CreateProduct methods to be run first.
    /// </summary>
    private bool AddCategoryToProduct()
    {
        // Get the data
        var product = SKUInfoProvider.GetSKUs()
                           .WhereStartsWith("SKUName", "MyNewProduct")
                           .WhereNull("SKUOptionCategoryID")
                           .FirstOrDefault();

        // Get the option category
        OptionCategoryInfo category = OptionCategoryInfoProvider.GetOptionCategoryInfo("MyNewCategory", SiteContext.CurrentSiteName);

        if ((product != null) && (category != null))
        {
            // Add category to product
            SKUOptionCategoryInfoProvider.AddOptionCategoryToSKU(category.CategoryID, product.SKUID);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Removes option category from product. Called when the "Remove category from product" button is pressed.
    /// Expects the AddCategoryToProduct method to be run first.
    /// </summary>
    private bool RemoveCategoryFromProduct()
    {
        // Get the data
        var product = SKUInfoProvider.GetSKUs()
                           .WhereStartsWith("SKUName", "MyNewProduct")
                           .FirstOrDefault();

        // Get the option category
        OptionCategoryInfo category = OptionCategoryInfoProvider.GetOptionCategoryInfo("MyNewCategory", SiteContext.CurrentSiteName);

        if ((product != null) && (category != null))
        {
            // Remove option category from product
            ProductHelper.RemoveOptionCategory(product.SKUID, category.CategoryID);

            return true;
        }

        return false;
    }

    #endregion


    #region "API examples - Option for product"

    /// <summary>
    /// Allows option for product. Called when the "Allow option for product" button is pressed.
    /// Expects the CreateOptionCategory, CreateOption and CreateProduct methods to be run first.
    /// </summary>
    private bool AllowOptionForProduct()
    {
        // Get the data
        var product = SKUInfoProvider.GetSKUs()
                           .WhereStartsWith("SKUName", "MyNewProduct")
                           .WhereNull("SKUOptionCategoryID")
                           .FirstOrDefault();

        // List of options IDs
        List<int> optionIds = new List<int>();

        // Get the data
        var option = SKUInfoProvider.GetSKUs()
                           .WhereStartsWith("SKUName", "MyNewProduct")
                           .WhereNotNull("SKUOptionCategoryID")
                           .FirstOrDefault();

        // Get the option
        if (option != null)
        {
            optionIds.Add(option.SKUID);
        }

        // Get the option category
        OptionCategoryInfo category = OptionCategoryInfoProvider.GetOptionCategoryInfo("MyNewCategory", SiteContext.CurrentSiteName);

        if ((product != null) && (option != null) && (category != null))
        {
            // Allow options for product
            ProductHelper.AllowOptions(product.SKUID, category.CategoryID, optionIds);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Removes option from product. Called when the "Remove option from product" button is pressed.
    /// Expects the AddOptionToProduct method to be run first.
    /// </summary>
    private bool RemoveOptionFromProduct()
    {
        // Get the data
        var product = SKUInfoProvider.GetSKUs()
                           .WhereStartsWith("SKUName", "MyNewProduct")
                           .WhereNull("SKUOptionCategoryID")
                           .FirstOrDefault();

        // List of options IDs
        List<int> optionIds = new List<int>();

        // Get the data
        var option = SKUInfoProvider.GetSKUs()
                           .WhereStartsWith("SKUName", "MyNewProduct")
                           .WhereNotNull("SKUOptionCategoryID")
                           .FirstOrDefault();

        // Get the option
        if (option != null)
        {
            optionIds.Add(option.SKUID);
        }

        // Get the option category
        OptionCategoryInfo category = OptionCategoryInfoProvider.GetOptionCategoryInfo("MyNewCategory", SiteContext.CurrentSiteName);

        if ((product != null) && (category != null) && optionIds.Count > 0)
        {
            // Remove options from product
            ProductHelper.RemoveOptions(product.SKUID, category.CategoryID, optionIds);

            return true;
        }

        return false;
    }

    #endregion


    #region "API - Examples - Product variants"

    /// <summary>
    /// Creates variants for product. Called when the "Create product variants" button is pressed.
    /// Expects the CreateProduct method to be run first.
    /// </summary>
    private bool CreateVariants()
    {
        // Get product
        var product = SKUInfoProvider.GetSKUs()
                           .WhereStartsWith("SKUName", "MyNewProduct")
                           .WhereNull("SKUOptionCategoryID")
                           .FirstOrDefault();

        if (product == null)
        {
            return false;
        }

        // List of categories
        List<int> categoryIDs = new List<int>();

        // Create two attribute option categories with options
        for (int i = 1; i <= 2; i++)
        {
            OptionCategoryInfo newCategory = new OptionCategoryInfo
            {
                CategoryDisplayName = "My new attribute category "+ i,
                CategoryName = "MyNewAttributteCategory" + i,
                CategoryType = OptionCategoryTypeEnum.Attribute,
                CategorySelectionType = OptionCategorySelectionTypeEnum.Dropdownlist,
                CategoryDisplayPrice = true,
                CategoryEnabled = true,
                CategoryDefaultRecord = "",
                CategorySiteID = SiteContext.CurrentSiteID
            };

            // Set category and add to product
            OptionCategoryInfoProvider.SetOptionCategoryInfo(newCategory);
            SKUOptionCategoryInfoProvider.AddOptionCategoryToSKU(newCategory.CategoryID, product.SKUID);
            categoryIDs.Add(newCategory.CategoryID);

            // Create two product options for new attribute category
            foreach (var color in new[] { "Black", "White" })
            {
                SKUInfo newOption = new SKUInfo
                {
                    SKUName = "MyNewColorOption" + color,
                    SKUPrice = 0,
                    SKUEnabled = true,
                    SKUOptionCategoryID = newCategory.CategoryID,
                    SKUSiteID = SiteContext.CurrentSiteID,
                    SKUProductType = SKUProductTypeEnum.Product
                };

                // Set option and add to product
                SKUInfoProvider.SetSKUInfo(newOption);
                SKUAllowedOptionInfoProvider.AddOptionToProduct(product.SKUID, newOption.SKUID);
            }
        }

        // Generate variants
        List<ProductVariant> variants = VariantHelper.GetAllPossibleVariants(product.SKUID, categoryIDs);
        
        // Set variants
        foreach (var variant in variants)
        {
            VariantHelper.SetProductVariant(variant);
        }

        return true;
    }


    /// <summary>
    /// Gets and updates product variants. Called when the "Get and update variants" button is pressed.
    /// Expects the CreateProduct and CreateVariants method to be run first.
    /// </summary>
    private bool GetAndUpdateVariants()
    {
        // Get product
        var product = SKUInfoProvider.GetSKUs()
                           .WhereStartsWith("SKUName", "MyNewProduct")
                           .WhereNull("SKUOptionCategoryID")
                           .FirstOrDefault();

        if (product == null)
        {
            return false;
        }

        // Get variants
        var variants = VariantHelper.GetVariants(product.SKUID);

        if (variants.Count > 0)
        {
            // Update variants
            foreach (var updateVariant in variants)
            {
                updateVariant.SKUName = updateVariant.SKUName.ToLowerCSafe();
                SKUInfoProvider.SetSKUInfo(updateVariant);
            }
            
            return true;
        }

        return false;
    }


    /// <summary>
    /// Deletes product variants. Called when the "Delete product variants" button is pressed.
    /// Expects the CreateProduct and CreateVariants method to be run first.
    /// </summary>
    private bool DeleteVariants()
    {
        // Get product
        var product = SKUInfoProvider.GetSKUs()
                           .WhereStartsWith("SKUName", "MyNewProduct")
                           .WhereNull("SKUOptionCategoryID")
                           .FirstOrDefault();
        
        if (product == null)
        {
            return false;
        }

        // Delete variants
        VariantHelper.DeleteAllVariants(product.SKUID);

        // Get options
        var options = SKUInfoProvider.GetSKUs().Where("SKUName", QueryOperator.Like, "MyNewColorOption%");

        foreach (SKUInfo option in options)
        {
            // Delete options
            SKUInfoProvider.DeleteSKUInfo(option);
        }

        // Get categories
        var categories = OptionCategoryInfoProvider.GetOptionCategories().Where("CategoryName", QueryOperator.Like, "MyNewAttributteCategory%");

        foreach (OptionCategoryInfo categoryObj in categories)
        {
            // Delete categories
            OptionCategoryInfoProvider.DeleteOptionCategoryInfo(categoryObj);
        }

        return true;
    }

    #endregion


    #region "API examples - Discount coupon"

    /// <summary>
    /// Creates discount coupon. Called when the "Create coupon" button is pressed.
    /// </summary>
    private bool CreateDiscountCoupon()
    {
        // Create new discount coupon object
        DiscountCouponInfo newCoupon = new DiscountCouponInfo();

        // Set the properties
        newCoupon.DiscountCouponDisplayName = "My new coupon";
        newCoupon.DiscountCouponCode = "MyNewCoupon";
        newCoupon.DiscountCouponIsExcluded = true;
        newCoupon.DiscountCouponIsFlatValue = true;
        newCoupon.DiscountCouponValue = 200;
        newCoupon.DiscountCouponValidFrom = DateTime.Now;
        newCoupon.DiscountCouponSiteID = SiteContext.CurrentSiteID;

        // Create the discount coupon
        DiscountCouponInfoProvider.SetDiscountCouponInfo(newCoupon);

        return true;
    }


    /// <summary>
    /// Gets and updates discount coupon. Called when the "Get and update coupon" button is pressed.
    /// Expects the CreateDiscountCoupon method to be run first.
    /// </summary>
    private bool GetAndUpdateDiscountCoupon()
    {
        // Get the discount coupon
        DiscountCouponInfo updateCoupon = DiscountCouponInfoProvider.GetDiscountCouponInfo("MyNewCoupon", SiteContext.CurrentSiteName);
        if (updateCoupon != null)
        {
            // Update the properties
            updateCoupon.DiscountCouponDisplayName = updateCoupon.DiscountCouponDisplayName.ToLowerCSafe();

            // Update the discount coupon
            DiscountCouponInfoProvider.SetDiscountCouponInfo(updateCoupon);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates discount coupons. Called when the "Get and bulk update coupons" button is pressed.
    /// Expects the CreateDiscountCoupon method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateDiscountCoupons()
    {
        // Get the data
        var coupons = DiscountCouponInfoProvider.GetDiscountCoupons()
                          .WhereStartsWith("DiscountCouponCode", "MyNewCoupon");

        var success = false;

        // Loop through the individual items
        foreach (var modifyCoupon in coupons)
        {
            // Update the properties
            modifyCoupon.DiscountCouponDisplayName = modifyCoupon.DiscountCouponDisplayName.ToUpper();

            // Update the discount coupon
            DiscountCouponInfoProvider.SetDiscountCouponInfo(modifyCoupon);

            success = true;
        }

        return success;
    }


    /// <summary>
    /// Deletes discount coupon. Called when the "Delete coupon" button is pressed.
    /// Expects the CreateDiscountCoupon method to be run first.
    /// </summary>
    private bool DeleteDiscountCoupon()
    {
        // Get the discount coupon
        DiscountCouponInfo deleteCoupon = DiscountCouponInfoProvider.GetDiscountCouponInfo("MyNewCoupon", SiteContext.CurrentSiteName);

        if (deleteCoupon != null)
        {
            // Delete the discount coupon
            DiscountCouponInfoProvider.DeleteDiscountCouponInfo(deleteCoupon);

            return true;
        }

        return false;
    }

    #endregion


    #region "API examples - Discount coupon products"

    /// <summary>
    /// Adds product to discount coupon. Called when the "Add product to coupon" button is pressed.
    /// </summary>
    private bool AddProductToCoupon()
    {
        // Get the data
        var product = SKUInfoProvider.GetSKUs()
                           .WhereStartsWith("SKUName", "MyNewProduct")
                           .WhereNull("SKUOptionCategoryID")
                           .FirstOrDefault();

        // Get the discount coupon
        DiscountCouponInfo discountCoupon = DiscountCouponInfoProvider.GetDiscountCouponInfo("MyNewCoupon", SiteContext.CurrentSiteName);

        if ((discountCoupon != null) && (product != null))
        {
            // Add Product to coupon
            SKUDiscountCouponInfoProvider.AddDiscountCouponToSKU(product.SKUID, discountCoupon.DiscountCouponID);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Removes product from discount coupon. Called when the "Remove product from coupon" button is pressed.
    /// Expects the AddProductToCoupon method to be run first.
    /// </summary>
    private bool RemoveProductFromCoupon()
    {
        // Get the data
        var product = SKUInfoProvider.GetSKUs()
                           .WhereStartsWith("SKUName", "MyNewProduct")
                           .WhereNull("SKUOptionCategoryID")
                           .FirstOrDefault();

        // Get the discount coupon
        DiscountCouponInfo discountCoupon = DiscountCouponInfoProvider.GetDiscountCouponInfo("MyNewCoupon", SiteContext.CurrentSiteName);

        if ((discountCoupon != null) && (product != null))
        {
            // Get product added to coupon
            SKUDiscountCouponInfo skuDiscount = SKUDiscountCouponInfoProvider.GetSKUDiscountCouponInfo(product.SKUID, discountCoupon.DiscountCouponID);

            // Remove product from coupon
            SKUDiscountCouponInfoProvider.DeleteSKUDiscountCouponInfo(skuDiscount);

            return (skuDiscount != null);
        }

        return false;
    }

    #endregion


    #region "API examples - Customer"

    /// <summary>
    /// Creates anonoymous customer. Called when the "Create anonymous customer" button is pressed.
    /// </summary>
    private bool CreateAnonymousCustomer()
    {
        // Create new customer object
        CustomerInfo newCustomer = new CustomerInfo();

        // Set the properties
        newCustomer.CustomerFirstName = "";
        newCustomer.CustomerLastName = "My new anonymous customer";
        newCustomer.CustomerEmail = "MyEmail@localhost.local";
        newCustomer.CustomerEnabled = true;
        newCustomer.CustomerSiteID = SiteContext.CurrentSiteID;

        // Create the anonymous customer
        CustomerInfoProvider.SetCustomerInfo(newCustomer);

        return true;
    }


    /// <summary>
    /// Creates registered customer. Called when the "Create registered customer" button is pressed.
    /// </summary>
    private bool CreateRegisteredCustomer()
    {
        // Create a new user
        UserInfo newUser = new UserInfo();

        // Set the user properties
        newUser.UserName = "My new user";
        newUser.UserEnabled = true;
        newUser.SetPrivilegeLevel(UserPrivilegeLevelEnum.Editor);

        // Save the user
        UserInfoProvider.SetUserInfo(newUser);

        // Add user to current site
        UserInfoProvider.AddUserToSite(newUser.UserName, SiteContext.CurrentSiteName);

        // Create new customer object
        CustomerInfo newCustomer = new CustomerInfo
        {
            CustomerFirstName = "",
            CustomerLastName = "My new registered customer",
            CustomerEmail = "MyEmail@localhost.local",
            CustomerEnabled = true,
            CustomerSiteID = SiteContext.CurrentSiteID,
            CustomerUserID = newUser.UserID
        };

        // Create the registered customer
        CustomerInfoProvider.SetCustomerInfo(newCustomer);

        return true;
    }


    /// <summary>
    /// Gets and updates customer. Called when the "Get and update customer" button is pressed.
    /// Expects the CreateCustomer method to be run first.
    /// </summary>
    private bool GetAndUpdateCustomer()
    {
        // Get the data
        var customer = CustomerInfoProvider.GetCustomers().WhereStartsWith("CustomerLastName", "My New Registered").FirstOrDefault();
        if (customer != null)
        {
            // Update the properties
            customer.CustomerLastName = customer.CustomerLastName.ToLowerCSafe();

            // Update the customer
            CustomerInfoProvider.SetCustomerInfo(customer);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates customers. Called when the "Get and bulk update customers" button is pressed.
    /// Expects the CreateCustomer method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateCustomers()
    {
        // Get the data
        var customers = CustomerInfoProvider.GetCustomers().WhereStartsWith("CustomerLastName", "My New Registered");
        var success = false;

        // Loop through the individual items
        foreach (var modifyCustomer in customers)
        {
            // Update the properties
            modifyCustomer.CustomerLastName = modifyCustomer.CustomerLastName.ToUpper();

            // Update the customer
            CustomerInfoProvider.SetCustomerInfo(modifyCustomer);

            success = true;
        }


        return success;
    }


    /// <summary>
    /// Deletes customer. Called when the "Delete customer" button is pressed.
    /// Expects the CreateCustomer method to be run first.
    /// </summary>
    private bool DeleteCustomer()
    {
        // Delete user
        UserInfo user = UserInfoProvider.GetUserInfo("My new user");
        UserInfoProvider.DeleteUser(user);

        // Get the data
        var customers = CustomerInfoProvider.GetCustomers().WhereStartsWith("CustomerLastName", "My New");
        var success = false;

        foreach (var deleteCustomer in customers)
        {
            // Delete the customer
            CustomerInfoProvider.DeleteCustomerInfo(deleteCustomer);

            success = true;
        }

        return success;
    }

    #endregion


    #region "API examples - Address"

    /// <summary>
    /// Creates address. Called when the "Create address" button is pressed.
    /// Expects the CreateRegisteredCustomer method to be run first.
    /// </summary>
    private bool CreateAddress()
    {
        // Get the customer
        CustomerInfo customer = CustomerInfoProvider.GetCustomers().WhereStartsWith("CustomerLastName", "My New Registered").FirstOrDefault();

        // Get the country
        CountryInfo country = CountryInfoProvider.GetCountryInfo("USA");

        // Get the state
        StateInfo state = StateInfoProvider.GetStateInfo("Alabama");

        if ((customer != null) && (country != null))
        {
            // Create new address object
            AddressInfo newAddress = new AddressInfo
                                     {
                                         AddressName = "My new address",
                                         AddressLine1 = "Address line 1",
                                         AddressLine2 = "Address line 2",
                                         AddressCity = "Address city",
                                         AddressZip = "Address ZIP code",
                                         AddressIsBilling = true,
                                         AddressIsShipping = false,
                                         AddressIsCompany = false,
                                         AddressEnabled = true,
                                         AddressPersonalName = customer.CustomerInfoName,
                                         AddressCustomerID = customer.CustomerID,
                                         AddressCountryID = country.CountryID,
                                         AddressStateID = state.StateID
                                     };

            // Create the address
            AddressInfoProvider.SetAddressInfo(newAddress);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and updates address. Called when the "Get and update address" button is pressed.
    /// Expects the CreateAddress method to be run first.
    /// </summary>
    private bool GetAndUpdateAddress()
    {
        // Get the address
        var address = AddressInfoProvider.GetAddresses()
                          .WhereStartsWith("AddressName", "My New")
                          .FirstOrDefault();

        if (address != null)
        {
            // Update the properties
            address.AddressName = address.AddressName.ToLowerCSafe();

            // Update the address
            AddressInfoProvider.SetAddressInfo(address);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates addresses. Called when the "Get and bulk update addresses" button is pressed.
    /// Expects the CreateAddress method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateAddresses()
    {
        // Get the data
        var addresses = AddressInfoProvider.GetAddresses().WhereStartsWith("AddressName", "My New");
        var success = false;

        // Loop through the individual items
        foreach (var modifyAddress in addresses)
        {
            // Update the properties
            modifyAddress.AddressName = modifyAddress.AddressName.ToUpper();

            // Update the address
            AddressInfoProvider.SetAddressInfo(modifyAddress);

            success = true;
        }

        return success;
    }


    /// <summary>
    /// Deletes address. Called when the "Delete address" button is pressed.
    /// Expects the CreateAddress method to be run first.
    /// </summary>
    private bool DeleteAddress()
    {
        // Get the address
        var address = AddressInfoProvider.GetAddresses()
                          .WhereStartsWith("AddressName", "My New")
                          .FirstOrDefault();

        if (address != null)
        {
            // Delete the address
            AddressInfoProvider.DeleteAddressInfo(address);

            return true;
        }

        return false;
    }

    #endregion


    #region "API examples - Credit event"

    /// <summary>
    /// Creates credit event. Called when the "Create event" button is pressed.
    /// Expects the CreateRegisteredCustomer method to be run first.
    /// </summary>
    private bool CreateCreditEvent()
    {
        // Get the customer
        CustomerInfo customer = CustomerInfoProvider.GetCustomers().WhereStartsWith("CustomerLastName", "My New Registered").FirstOrDefault();

        if (customer != null)
        {
            // Create new credit event object
            CreditEventInfo newEvent = new CreditEventInfo
                                       {
                                           EventName = "My new event",
                                           EventCreditChange = 500,
                                           EventDate = DateTime.Now,
                                           EventDescription = "Credit event description.",
                                           EventCustomerID = customer.CustomerID,
                                           EventSiteID = SiteContext.CurrentSiteID
                                       };

            // Create the credit event
            CreditEventInfoProvider.SetCreditEventInfo(newEvent);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and updates credit event. Called when the "Get and update event" button is pressed.
    /// Expects the CreateCreditEvent method to be run first.
    /// </summary>
    private bool GetAndUpdateCreditEvent()
    {
        // Get the credit event
        var updateCredit = CreditEventInfoProvider.GetCreditEvents()
                                .WhereStartsWith("EventName", "My New")
                                .FirstOrDefault();

        if (updateCredit != null)
        {
            // Update the properties
            updateCredit.EventName = updateCredit.EventName.ToLowerCSafe();

            // Update the credit event
            CreditEventInfoProvider.SetCreditEventInfo(updateCredit);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates credit events. Called when the "Get and bulk update events" button is pressed.
    /// Expects the CreateCreditEvent method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateCreditEvents()
    {
        // Get the data
        var credits = CreditEventInfoProvider.GetCreditEvents().WhereStartsWith("EventName", "My New");
        var success = false;

        // Loop through the individual items
        foreach (var updateCredit in credits)
        {
            // Update the properties
            updateCredit.EventName = updateCredit.EventName.ToUpper();

            // Update the credit event
            CreditEventInfoProvider.SetCreditEventInfo(updateCredit);

            success = true;
        }

        return success;
    }


    /// <summary>
    /// Gets total credit. Called when the "get total credit" button is pressed.
    /// Expects the CreateCreditEvent method to be run first.
    /// </summary>
    private bool GetTotalCredit()
    {
        double totalCredit = 0;

        // Get the customer
        CustomerInfo customer = CustomerInfoProvider.GetCustomers().WhereStartsWith("CustomerLastName", "My New Registered").FirstOrDefault();

        if (customer != null)
        {
            // Get total credit
            totalCredit = CreditEventInfoProvider.GetTotalCredit(customer.CustomerID, SiteContext.CurrentSiteID);

            // Show the total credit
            apiGetTotalCredit.InfoMessage = "Total customer credit is: " + totalCredit;

            return true;
        }

        return false;
    }


    /// <summary>
    /// Deletes credit event. Called when the "Delete event" button is pressed.
    /// Expects the CreateCreditEvent method to be run first.
    /// </summary>
    private bool DeleteCreditEvent()
    {
        // Get the credit event
        var deleteCredit = CreditEventInfoProvider.GetCreditEvents()
                                .WhereStartsWith("EventName", "My New")
                                .FirstOrDefault();

        if (deleteCredit != null)
        {
            // Delete credit event
            CreditEventInfoProvider.DeleteCreditEventInfo(deleteCredit);

            return true;
        }

        return false;
    }

    #endregion


    #region "API Examples - Customer newsletter"

    /// <summary>
    /// Subscribes customer to newsletter. Called when the "Subscribe customer to newsletter" button is pressed.
    /// Expects the CreateRegisteredCustomer method to be run first.
    /// </summary>
    private bool SubscribeCustomerToNewsletter()
    {
        // Get the customer
        CustomerInfo customer = CustomerInfoProvider.GetCustomers().WhereStartsWith("CustomerLastName", "My New Registered").FirstOrDefault();

        // Get the newsletter
        NewsletterInfo customerNewsletter = NewsletterInfoProvider.GetNewsletterInfo("CorporateNewsletter", SiteContext.CurrentSiteID);

        if ((customer != null) && (customerNewsletter != null))
        {
            // Create the new subscription object
            SubscriberNewsletterInfo subscription = new SubscriberNewsletterInfo();

            SubscriberInfo customerExistedSubscriber = SubscriberInfoProvider.GetSubscriberInfo("MyEmail@localhost.local", SiteContext.CurrentSiteID);

            // Check if customer is subscriber
            if (customerExistedSubscriber != null)
            {
                // Subscribe existed customer subscriber to newsletter
                subscription.SubscriberID = customerExistedSubscriber.SubscriberID;
            }
            else
            {
                // Create the new subscriber object
                SubscriberInfo customerSubscriber = new SubscriberInfo();

                // Set the properties
                customerSubscriber.SubscriberEmail = customer.CustomerEmail;
                customerSubscriber.SubscriberLastName = customer.CustomerLastName;
                customerSubscriber.SubscriberSiteID = SiteContext.CurrentSiteID;

                // Create the new subscriber
                SubscriberInfoProvider.SetSubscriberInfo(customerSubscriber);

                // Subscribe new customer subscriber to newsletter
                subscription.SubscriberID = customerSubscriber.SubscriberID;
            }

            subscription.NewsletterID = customerNewsletter.NewsletterID;
            subscription.SubscribedWhen = DateTime.Now;

            // Save the data
            SubscriberNewsletterInfoProvider.SetSubscriberNewsletterInfo(subscription);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Unsubscribes customer from newsletter. Called when the "Unsubscribe customer from newsletter" button is pressed.
    /// Expects the SubscribeCustomerToNewsletter method to be run first.
    /// </summary>
    private bool UnsubscribeCustomerFromNewsletter()
    {
        SubscriberInfo subscriber = SubscriberInfoProvider.GetSubscriberInfo("MyEmail@localhost.local", SiteContext.CurrentSiteID);

        if (subscriber != null)
        {
            var subscription = SubscriberNewsletterInfoProvider.GetEnabledSubscriberNewsletters()
                                   .WhereEquals("SubscriberID", subscriber.SubscriberID)
                                   .FirstOrDefault();

            if (subscription != null)
            {
                // Unsubscribe customer from newsletter
                SubscriberNewsletterInfoProvider.DeleteSubscriberNewsletterInfo(subscription);

                return true;
            }
        }

        return false;
    }

    #endregion


    #region "API examples - Customer wishlist"

    /// <summary>
    /// Adds product to wishlist. Called when the "Assign discount level to customer" button is pressed.
    /// Expects the CreateProduct and CreateRegisteredCustomer methods to be run first.
    /// </summary>
    private bool AddProductToWishlist()
    {
        // Get the data
        var product = SKUInfoProvider.GetSKUs()
                           .WhereStartsWith("SKUName", "MyNewProduct")
                           .FirstOrDefault();

        // Get the customer
        var customer = CustomerInfoProvider.GetCustomers()
                           .WhereStartsWith("CustomerLastName", "My New Registered")
                           .FirstOrDefault();

        if ((customer != null) && (product != null))
        {
            // Add product to wishlist
            WishlistItemInfoProvider.AddSKUToWishlist(customer.CustomerUserID, product.SKUID, SiteContext.CurrentSiteID);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Removes product from wishlist. Called when the "Remove product from wishlist" button is pressed.
    /// Expects the AddProductToWishlist method to be run first.
    /// </summary>
    private bool RemoveProductFromWishlist()
    {
        // Get the data
        var product = SKUInfoProvider.GetSKUs()
                           .WhereStartsWith("SKUName", "MyNewProduct")
                           .FirstOrDefault();

        // Get the customer
        CustomerInfo customer = CustomerInfoProvider.GetCustomers().WhereStartsWith("CustomerLastName", "My New Registered").FirstOrDefault();

        if ((customer != null) && (product != null))
        {
            // Get the product from wishlist
            WishlistItemInfo wishlistItem = WishlistItemInfoProvider.GetWishlistItemInfo(customer.CustomerUserID, product.SKUID, SiteContext.CurrentSiteID);

            // Remove product from wishlist
            WishlistItemInfoProvider.DeleteWishlistItemInfo(wishlistItem);

            return (wishlistItem != null);
        }

        return false;
    }

    #endregion


    #region "API examples - Order"

    /// <summary>
    /// Creates order. Called when the "Create order" button is pressed.
    /// Expects the CreateRegisteredCustomer, CreateAddress and CreateOrderStatus methods to be run first.
    /// </summary>
    private bool CreateOrder()
    {
        // Get the customer
        CustomerInfo customer = CustomerInfoProvider.GetCustomers().WhereStartsWith("CustomerLastName", "My New Registered").FirstOrDefault();

        // Create order addresses from customer address
        OrderAddressInfo orderBillingAddress = null;
        OrderAddressInfo orderShippingAddress = null;

        // Get the customer address
        AddressInfo customerAddress = AddressInfoProvider.GetAddresses().TopN(1).WhereStartsWith("AddressName", "My New").FirstOrDefault();

        if (customerAddress != null)
        {
            // Get the data from customer address
            orderBillingAddress = OrderAddressInfoProvider.CreateOrderAddressInfo(customerAddress);
            orderShippingAddress = OrderAddressInfoProvider.CreateOrderAddressInfo(customerAddress);

            // Set the order addresses
            OrderAddressInfoProvider.SetAddressInfo(orderBillingAddress);
            OrderAddressInfoProvider.SetAddressInfo(orderShippingAddress);
        }

        // Get the order status
        OrderStatusInfo orderStatus = OrderStatusInfoProvider.GetOrderStatusInfo("MyNewStatus", SiteContext.CurrentSiteName);

        // Get the currency
        CurrencyInfo currency = CurrencyInfoProvider.GetCurrencyInfo("MyNewCurrency", SiteContext.CurrentSiteName);

        if ((customer != null) && (orderStatus != null) && (currency != null) && (orderBillingAddress != null))
        {
            // Create new order object
            OrderInfo newOrder = new OrderInfo
            {
                OrderInvoiceNumber = "1",
                OrderBillingAddress = orderBillingAddress,
                OrderShippingAddress = orderShippingAddress,
                OrderTotalPrice = 200,
                OrderTotalTax = 30,
                OrderDate = DateTime.Now,
                OrderStatusID = orderStatus.StatusID,
                OrderCustomerID = customer.CustomerID,
                OrderSiteID = SiteContext.CurrentSiteID,
                OrderCurrencyID = currency.CurrencyID
            };

            // Create the order
            OrderInfoProvider.SetOrderInfo(newOrder);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and updates order. Called when the "Get and update order" button is pressed.
    /// Expects the CreateOrder method to be run first.
    /// </summary>
    private bool GetAndUpdateOrder()
    {
        // Get the customer
        var customer = CustomerInfoProvider.GetCustomers().WhereStartsWith("CustomerLastName", "My New Registered").FirstOrDefault();

        if (customer != null)
        {
            // Get the order
            var order = OrderInfoProvider.GetOrders().WhereEquals("OrderCustomerID", customer.CustomerID).FirstOrDefault();
            if (order != null)
            {
                // Update the property
                order.OrderTotalPrice = order.OrderTotalPrice + 200;

                // Update the order
                OrderInfoProvider.SetOrderInfo(order);

                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates orders. Called when the "Get and bulk update orders" button is pressed.
    /// Expects the CreateOrder method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateOrders()
    {
        // Get the customer
        CustomerInfo customer = CustomerInfoProvider.GetCustomers().WhereStartsWith("CustomerLastName", "My New Registered").FirstOrDefault();

        var success = false;

        if (customer != null)
        {
            // Get the order
            var orders = OrderInfoProvider.GetOrders().WhereEquals("OrderCustomerID", customer.CustomerID);
            foreach (var order in orders)
            {
                // Update the property
                order.OrderTotalPrice = order.OrderTotalPrice + 200;

                // Update the order
                OrderInfoProvider.SetOrderInfo(order);

                success = true;
            }
        }

        return success;
    }


    /// <summary>
    /// Deletes order. Called when the "Delete order" button is pressed.
    /// Expects the CreateOrder method to be run first.
    /// </summary>
    private bool DeleteOrder()
    {
        // Get the customer
        CustomerInfo customer = CustomerInfoProvider.GetCustomers().WhereStartsWith("CustomerLastName", "My New Registered").FirstOrDefault();

        if (customer != null)
        {
            var order = OrderInfoProvider.GetOrders().WhereEquals("OrderCustomerID", customer.CustomerID).FirstOrDefault();
            if (order != null)
            {
                // Delete the order
                OrderInfoProvider.DeleteOrderInfo(order);

                return true;
            }
        }

        return false;
    }

    #endregion


    #region "API examples - Order item"

    /// <summary>
    /// Creates order item. Called when the "Create item" button is pressed.
    /// </summary>
    private bool CreateOrderItem()
    {
        // Get the data
        var product = SKUInfoProvider.GetSKUs()
                           .WhereStartsWith("SKUName", "MyNewProduct")
                           .FirstOrDefault();

        // Get the customer
        var customer = CustomerInfoProvider.GetCustomers()
                           .WhereStartsWith("CustomerLastName", "My New Registered")
                           .FirstOrDefault();

        if (customer != null)
        {
            // Get the order
            var order = OrderInfoProvider.GetOrders()
                            .WhereEquals("OrderCustomerID", customer.CustomerID)
                            .FirstOrDefault();

            if ((order != null) && (product != null))
            {
                // Create new order item object
                OrderItemInfo newItem = new OrderItemInfo
                                        {
                                            OrderItemSKUName = "MyNewProduct",
                                            OrderItemOrderID = order.OrderID,
                                            OrderItemSKUID = product.SKUID,
                                            OrderItemUnitPrice = 200,
                                            OrderItemUnitCount = 1
                                        };

                // Create the order item
                OrderItemInfoProvider.SetOrderItemInfo(newItem);

                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// Gets and updates order item. Called when the "Get and update item" button is pressed.
    /// Expects the CreateOrderItem method to be run first.
    /// </summary>
    private bool GetAndUpdateOrderItem()
    {
        // Get the customer
        CustomerInfo customer = CustomerInfoProvider.GetCustomers().WhereStartsWith("CustomerLastName", "My New Registered").FirstOrDefault();

        if (customer != null)
        {
            var order = OrderInfoProvider.GetOrders().WhereEquals("OrderCustomerID", customer.CustomerID).FirstOrDefault();

            if (order != null)
            {
                // Get the order item
                var orderItem = OrderItemInfoProvider.GetOrderItems(order.OrderID).FirstOrDefault();
                if (orderItem != null)
                {
                    // Update the property
                    orderItem.OrderItemSKUName = orderItem.OrderItemSKUName.ToLowerCSafe();

                    // Update the order item
                    OrderItemInfoProvider.SetOrderItemInfo(orderItem);

                    return true;
                }
            }
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates order items. Called when the "Get and bulk update items" button is pressed.
    /// Expects the CreateOrderItem method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateOrderItems()
    {
        // Get the customer
        CustomerInfo customer = CustomerInfoProvider.GetCustomers().WhereStartsWith("CustomerLastName", "My New Registered").FirstOrDefault();

        if (customer != null)
        {
            var order = OrderInfoProvider.GetOrders().WhereEquals("OrderCustomerID", customer.CustomerID).FirstOrDefault();

            if (order != null)
            {
                // Get the order item
                var orderItems = OrderItemInfoProvider.GetOrderItems(order.OrderID);

                foreach (var orderItem in orderItems)
                {
                    // Update the property
                    orderItem.OrderItemSKUName = orderItem.OrderItemSKUName.ToUpper();

                    // Update the order item
                    OrderItemInfoProvider.SetOrderItemInfo(orderItem);
                }

                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// Deletes order item. Called when the "Delete item" button is pressed.
    /// Expects the CreateOrderItem method to be run first.
    /// </summary>
    private bool DeleteOrderItem()
    {
        // Get the customer
        CustomerInfo customer = CustomerInfoProvider.GetCustomers().WhereStartsWith("CustomerLastName", "My New Registered").FirstOrDefault();

        if (customer != null)
        {
            var order = OrderInfoProvider.GetOrders().WhereEquals("OrderCustomerID", customer.CustomerID).FirstOrDefault();

            if (order != null)
            {
                // Get the order item
                var orderItem = OrderItemInfoProvider.GetOrderItems(order.OrderID).FirstOrDefault();
                if (orderItem != null)
                {
                    // Delete the order item
                    OrderItemInfoProvider.DeleteOrderItemInfo(orderItem);

                    return true;
                }
            }
        }

        return false;
    }

    #endregion


    #region "API Examples - Order status history"

    /// <summary>
    /// Changes order status. Called when the "Change order status" button is pressed.
    /// Expects the CreateOrder method to be run first.
    /// </summary>
    private bool ChangeOrderStatus()
    {
        // Get the customer
        CustomerInfo customer = CustomerInfoProvider.GetCustomers().WhereStartsWith("CustomerLastName", "My New Registered").FirstOrDefault();

        if (customer != null)
        {
            var order = OrderInfoProvider.GetOrders().WhereEquals("OrderCustomerID", customer.CustomerID).FirstOrDefault();

            if (order != null)
            {
                // Get next enabled order status
                OrderStatusInfo nextOrderStatus = OrderStatusInfoProvider.GetNextEnabledStatus(order.OrderStatusID);

                if (nextOrderStatus != null)
                {
                    // Create new order status user object
                    OrderStatusUserInfo newUserStatus = new OrderStatusUserInfo();

                    // Set the properties
                    newUserStatus.OrderID = order.OrderID;
                    newUserStatus.ChangedByUserID = MembershipContext.AuthenticatedUser.UserID;
                    newUserStatus.FromStatusID = order.OrderStatusID;
                    newUserStatus.ToStatusID = nextOrderStatus.StatusID;
                    newUserStatus.Date = DateTime.Now;

                    // Set the order status user
                    OrderStatusUserInfoProvider.SetOrderStatusUserInfo(newUserStatus);

                    // Set next order status to order
                    order.OrderStatusID = nextOrderStatus.StatusID;

                    // Change the order status
                    OrderInfoProvider.SetOrderInfo(order);

                    return true;
                }
            }
        }

        return false;
    }


    /// <summary>
    /// Deletes order status history. Called when the "Delete history" button is pressed.
    /// Expects the ChangeOrderStatus method to be run first.
    /// </summary>
    private bool DeleteHistory()
    {
        // Get the customer
        var customer = CustomerInfoProvider.GetCustomers().WhereStartsWith("CustomerLastName", "My New Registered").FirstOrDefault();

        var success = false;

        if (customer != null)
        {
            var order = OrderInfoProvider.GetOrders().WhereEquals("OrderCustomerID", customer.CustomerID).FirstOrDefault();

            if (order != null)
            {
                // Get the order statuses
                var statuses = OrderStatusUserInfoProvider.GetOrderStatusHistory(order.OrderID);
                foreach (var status in statuses)
                {
                    // Delete the order status
                    OrderStatusUserInfoProvider.DeleteOrderStatusUserInfo(status);

                    success = true;
                }
            }
        }

        return success;
    }

    #endregion
}
