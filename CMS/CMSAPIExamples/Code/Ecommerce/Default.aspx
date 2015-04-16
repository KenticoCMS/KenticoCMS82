<%@ Page Title="" Language="C#" MasterPageFile="~/CMSAPIExamples/Pages/APIExamplesPage.Master"
    Theme="Default" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="CMSAPIExamples_Code_Ecommerce_Default" %>

<%@ Register Src="~/CMSAPIExamples/Controls/APIExample.ascx" TagName="APIExample" TagPrefix="cms" %>
<asp:Content ID="contentLeft" ContentPlaceHolderID="plcLeftContainer" runat="server">
    <%-- Section: Configuration --%>
    <cms:LocalizedHeading ID="headManConfiguration" runat="server" Text="Configuration" Level="4" EnableViewState="false" />
    <%-- Invoice --%>
    <cms:LocalizedHeading ID="headGetAndUpdateInvoice" runat="server" Text="Invoice" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiGetAndUpdateInvoice" runat="server" ButtonText="Get and update invoice" InfoMessage="Invoice was updated." ErrorMessage="Invoice was not found." />
    <%-- Checkout process step --%>
    <cms:LocalizedHeading ID="headCreateCheckoutProcessStep" runat="server" Text="Checkout process step" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiGenerateDefaultCheckoutProcess" runat="server" ButtonText="Generate default process" InfoMessage="Default process was generated." />
    <cms:APIExample ID="apiCreateCheckoutProcessStep" runat="server" ButtonText="Create step" InfoMessage="Step 'My new step' was created." />
    <cms:APIExample ID="apiGetAndUpdateCheckoutProcessStep" runat="server" ButtonText="Get and update step" APIExampleType="ManageAdditional" InfoMessage="Step 'My new step' was updated." ErrorMessage="Step 'My new step' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateCheckoutProcessSteps" runat="server" ButtonText="Get and bulk update steps" APIExampleType="ManageAdditional" InfoMessage="All steps matching the condition were updated." ErrorMessage="Steps matching the condition were not found." />
    <%-- Tax class --%>
    <cms:LocalizedHeading ID="headCreateTaxClass" runat="server" Text="Tax class" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiCreateTaxClass" runat="server" ButtonText="Create class" InfoMessage="Class 'My new class' was created." />
    <cms:APIExample ID="apiGetAndUpdateTaxClass" runat="server" ButtonText="Get and update class" APIExampleType="ManageAdditional" InfoMessage="Class 'My new class' was updated." ErrorMessage="Class 'My new class' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateTaxClasses" runat="server" ButtonText="Get and bulk update classes" APIExampleType="ManageAdditional" InfoMessage="All classes matching the condition were updated." ErrorMessage="Classes matching the condition were not found." />
    <%-- Tax class value in country --%>
    <cms:LocalizedHeading ID="headSetTaxClassValueInCountry" runat="server" Text="Tax class value in country" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiSetTaxClassValueInCountry" runat="server" ButtonText="Set value" InfoMessage="Value was set." ErrorMessage="Class 'My new class' or country 'USA' were not found." />
    <cms:APIExample ID="apiGetAndUpdateTaxClassValueInCountry" runat="server" ButtonText="Get and update value" APIExampleType="ManageAdditional" InfoMessage="Value was updated." ErrorMessage="Class 'My new class', country 'USA' or their relationship were not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateTaxClassValuesInCountry" runat="server" ButtonText="Get and bulk update values" APIExampleType="ManageAdditional" InfoMessage="All values matching the condition were updated." ErrorMessage="Values matching the condition were not found." />
    <%-- Tax class value in state --%>
    <cms:LocalizedHeading ID="headApiSetTaxClassValueInState" runat="server" Text="Tax class value in state" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiSetTaxClassValueInState" runat="server" ButtonText="Set value" InfoMessage="Value was set." ErrorMessage="Class 'My new class' or state 'Alabama' were not found." />
    <cms:APIExample ID="apiGetAndUpdateTaxClassValueInState" runat="server" ButtonText="Get and update value" APIExampleType="ManageAdditional" InfoMessage="Value was updated." ErrorMessage="Class 'My new class', state 'Alabama' or their relationship were not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateTaxClassValuesInState" runat="server" ButtonText="Get and bulk update values" APIExampleType="ManageAdditional" InfoMessage="All values matching the condition were updated." ErrorMessage="Values matching the condition were not found." />
    <%-- Currency --%>
    <cms:LocalizedHeading ID="headCreateCurrency" runat="server" Text="Currency" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiCreateCurrency" runat="server" ButtonText="Create currency" InfoMessage="Currency 'My new currency' was created." />
    <cms:APIExample ID="apiGetAndUpdateCurrency" runat="server" ButtonText="Get and update currency" APIExampleType="ManageAdditional" InfoMessage="Currency 'My new currency' was updated." ErrorMessage="Currency 'My new currency' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateCurrencies" runat="server" ButtonText="Get and bulk update currencies" APIExampleType="ManageAdditional" InfoMessage="All currencies matching the condition were updated." ErrorMessage="Currencies matching the condition were not found." />
    <%-- Exchange table --%>
    <cms:LocalizedHeading ID="headCreateExchangeTable" runat="server" Text="Exchange table" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiCreateExchangeTable" runat="server" ButtonText="Create table" InfoMessage="Table 'My new table' was created." />
    <cms:APIExample ID="apiGetAndUpdateExchangeTable" runat="server" ButtonText="Get and update table" APIExampleType="ManageAdditional" InfoMessage="Table 'My new table' was updated." ErrorMessage="Table 'My new table' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateExchangeTables" runat="server" ButtonText="Get and bulk update tables" APIExampleType="ManageAdditional" InfoMessage="All tables matching the condition were updated." ErrorMessage="Tables matching the condition were not found." />
    <%-- Order status --%>
    <cms:LocalizedHeading ID="headCreateOrderStatus" runat="server" Text="Order status" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiCreateOrderStatus" runat="server" ButtonText="Create status" InfoMessage="Status 'My new status' was created." />
    <cms:APIExample ID="apiGetAndUpdateOrderStatus" runat="server" ButtonText="Get and update status" APIExampleType="ManageAdditional" InfoMessage="Status 'My new status' was updated." ErrorMessage="Status 'My new status' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateOrderStatuses" runat="server" ButtonText="Get and bulk update statuses" APIExampleType="ManageAdditional" InfoMessage="All statuses matching the condition were updated." ErrorMessage="Statuses matching the condition were not found." />
    <%-- Public status --%>
    <cms:LocalizedHeading ID="headCreatePublicStatus" runat="server" Text="Public status" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiCreatePublicStatus" runat="server" ButtonText="Create status" InfoMessage="Status 'My new status' was created." />
    <cms:APIExample ID="apiGetAndUpdatePublicStatus" runat="server" ButtonText="Get and update status" APIExampleType="ManageAdditional" InfoMessage="Status 'My new status' was updated." ErrorMessage="Status 'My new status' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdatePublicStatuses" runat="server" ButtonText="Get and bulk update statuses" APIExampleType="ManageAdditional" InfoMessage="All statuses matching the condition were updated." ErrorMessage="Statuses matching the condition were not found." />
    <%-- Internal status --%>
    <cms:LocalizedHeading ID="headCreateInternalStatus" runat="server" Text="Internal status" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiCreateInternalStatus" runat="server" ButtonText="Create status" InfoMessage="Status 'My new status' was created." />
    <cms:APIExample ID="apiGetAndUpdateInternalStatus" runat="server" ButtonText="Get and update status" APIExampleType="ManageAdditional" InfoMessage="Status 'My new status' was updated." ErrorMessage="Status 'My new status' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateInternalStatuses" runat="server" ButtonText="Get and bulk update statuses" APIExampleType="ManageAdditional" InfoMessage="All statuses matching the condition were updated." ErrorMessage="Statuses matching the condition were not found." />
    <%-- Department --%>
    <cms:LocalizedHeading ID="headCreateDepartment" runat="server" Text="Department" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiCreateDepartment" runat="server" ButtonText="Create department" InfoMessage="Department 'My new department' was created." />
    <cms:APIExample ID="apiGetAndUpdateDepartment" runat="server" ButtonText="Get and update department" APIExampleType="ManageAdditional" InfoMessage="Department 'My new department' was updated." ErrorMessage="Department 'My new department' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateDepartments" runat="server" ButtonText="Get and bulk update departments" APIExampleType="ManageAdditional" InfoMessage="All departments matching the condition were updated." ErrorMessage="Departments matching the condition were not found." />
    <%-- Department user --%>
    <cms:LocalizedHeading ID="headAddUserToDepartment" runat="server" Text="Department user" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiAddUserToDepartment" runat="server" ButtonText="Add user to department" InfoMessage="Current user was added to department 'My new department'." ErrorMessage="Department 'My new department' was not found." />
    <%-- Department default tax class --%>
    <cms:LocalizedHeading ID="headAddTaxClassToDepartment" runat="server" Text="Department default tax class" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiAddTaxClassToDepartment" runat="server" ButtonText="Add tax class to department" InfoMessage="Default tax class was added to department 'My new department'." ErrorMessage="Department 'My new department' or class 'My new class' were not found." />
    <%-- Shipping option --%>
    <cms:LocalizedHeading ID="headCreateShippingOption" runat="server" Text="Shipping option" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiCreateShippingOption" runat="server" ButtonText="Create option" InfoMessage="Option 'My new option' was created." />
    <cms:APIExample ID="apiGetAndUpdateShippingOption" runat="server" ButtonText="Get and update option" APIExampleType="ManageAdditional" InfoMessage="Option 'My new option' was updated." ErrorMessage="Option 'My new option' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateShippingOptions" runat="server" ButtonText="Get and bulk update options" APIExampleType="ManageAdditional" InfoMessage="All options matching the condition were updated." ErrorMessage="Options matching the condition were not found." />
    <cms:APIExample ID="apiAddShippingCostToOption" runat="server" ButtonText="Add cost to option" InfoMessage="Shipping cost was added to option 'My new option'." ErrorMessage="Option 'My new option' was not found." />
    <cms:APIExample ID="apiAddTaxClassToOption" runat="server" ButtonText="Add tax to option" InfoMessage="Class 'My new class' was added to option 'My new option'." ErrorMessage="Option 'My new option' or class 'My new class' were not found." />
    <%-- Payment method --%>
    <cms:LocalizedHeading ID="headCreatePaymentMethod" runat="server" Text="Payment method" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiCreatePaymentMethod" runat="server" ButtonText="Create method" InfoMessage="Method 'My new method' was created." />
    <cms:APIExample ID="apiGetAndUpdatePaymentMethod" runat="server" ButtonText="Get and update method" APIExampleType="ManageAdditional" InfoMessage="Method 'My new method' was updated." ErrorMessage="Method 'My new method' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdatePaymentMethods" runat="server" ButtonText="Get and bulk update methods" APIExampleType="ManageAdditional" InfoMessage="All methods matching the condition were updated." ErrorMessage="Methods matching the condition were not found." />
    <%-- Manufacturer --%>
    <cms:LocalizedHeading ID="headCreateManufacturer" runat="server" Text="Manufacturer" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiCreateManufacturer" runat="server" ButtonText="Create manufacturer" InfoMessage="Manufacturer 'My new manufacturer' was created." />
    <cms:APIExample ID="apiGetAndUpdateManufacturer" runat="server" ButtonText="Get and update manufacturer" APIExampleType="ManageAdditional" InfoMessage="Manufacturer 'My new manufacturer' was updated." ErrorMessage="Manufacturer 'My new manufacturer' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateManufacturers" runat="server" ButtonText="Bulk update manufacturers" APIExampleType="ManageAdditional" InfoMessage="All manufacturers matching the condition were updated." ErrorMessage="Manufacturers matching the condition were not found." />
    <%-- Supplier --%>
    <cms:LocalizedHeading ID="headCreateSupplier" runat="server" Text="Supplier" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiCreateSupplier" runat="server" ButtonText="Create supplier" InfoMessage="Supplier 'My new supplier' was created." />
    <cms:APIExample ID="apiGetAndUpdateSupplier" runat="server" ButtonText="Get and update supplier" APIExampleType="ManageAdditional" InfoMessage="Supplier 'My new supplier' was updated." ErrorMessage="Supplier 'My new supplier' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateSuppliers" runat="server" ButtonText="Get and bulk update suppliers" APIExampleType="ManageAdditional" InfoMessage="All suppliers matching the condition were updated." ErrorMessage="Suppliers matching the condition were not found." />
    <%-- Section: Products --%>
    <cms:LocalizedHeading ID="headManProducts" runat="server" Text="Products" Level="4" EnableViewState="false" />
    <%-- Product --%>
    <cms:LocalizedHeading ID="headCreateProduct" runat="server" Text="Product" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiCreateProduct" runat="server" ButtonText="Create product" InfoMessage="Product 'My new product' was created." ErrorMessage="Department 'My new department' was not found." />
    <cms:APIExample ID="apiGetAndUpdateProduct" runat="server" ButtonText="Get and update product" APIExampleType="ManageAdditional" InfoMessage="Product 'My new product' was updated." ErrorMessage="Product 'My new product' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateProducts" runat="server" ButtonText="Get and bulk update products" APIExampleType="ManageAdditional" InfoMessage="All products matching the condition were updated." ErrorMessage="Products matching the condition were not found." />
    <%-- Product page --%>
    <cms:LocalizedHeading ID="headCreateProductDocument" runat="server" Text="Product page" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiCreateProductDocument" runat="server" ButtonText="Create page" InfoMessage="Page 'My new page' was created." ErrorMessage="Product 'My new product' was not found." />
    <cms:APIExample ID="apiGetAndUpdateProductDocument" runat="server" ButtonText="Get and update page" APIExampleType="ManageAdditional" InfoMessage="Page 'My new page' was updated." ErrorMessage="Page 'My new page' was not found." />
    <%-- Product membership --%>
    <cms:LocalizedHeading ID="headCreateMembershipProduct" runat="server" Text="Product - membership" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiCreateMembershipProduct" runat="server" ButtonText="Create membership product" InfoMessage="Membership product 'My new membership product' was created." />
    <%-- Product eProduct --%>
    <cms:LocalizedHeading ID="headCreateEProduct" runat="server" Text="Product - eProduct" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiCreateEProduct" runat="server" ButtonText="Create eProduct" InfoMessage="EProduct 'My new eProduct' was created." ErrorMessage="EProduct file was not found." />
    <%-- Product donation --%>
    <cms:LocalizedHeading ID="headCreateDonation" runat="server" Text="Product - donation" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiCreateDonation" runat="server" ButtonText="Create donation" InfoMessage="Donation 'My new Donation' was created." />
    <%-- Product bundle --%>
    <cms:LocalizedHeading ID="headCreateBundle" runat="server" Text="Product - bundle" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiCreateBundle" runat="server" ButtonText="Create bundle" InfoMessage="Bundle 'My new Bundle' was created." ErrorMessage="Product 'My new product' was not found." />
    <%-- Shopping cart --%>
    <cms:LocalizedHeading ID="headAddProductToShoppingCart" runat="server" Text="Shopping cart" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiAddProductToShoppingCart" runat="server" ButtonText="Add product to cart" InfoMessage="Product 'My new product' was added to shopping cart." ErrorMessage="Product 'My new product' has not been found." />
    <cms:APIExample ID="apiUpdateShoppingCartItemUnits" runat="server" ButtonText="Update item units" APIExampleType="ManageAdditional" InfoMessage="Shopping cart item 'My new product' was updated." ErrorMessage="Shopping cart item 'My new product' was not found." />
    <%-- Product tax class --%>
    <cms:LocalizedHeading ID="headAddTaxClassToProduct" runat="server" Text="Tax class" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiAddTaxClassToProduct" runat="server" ButtonText="Add tax class to product" InfoMessage="Tax class 'My new tax class' was added to product 'My new product'." ErrorMessage="Tax class 'My new tax class' or product 'My new product' were not found." />
    <%-- Volume discount --%>
    <cms:LocalizedHeading ID="headCreateVolumeDiscount" runat="server" Text="Volume discount" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiCreateVolumeDiscount" runat="server" ButtonText="Create discount" InfoMessage="Discount 'My new discount' was created." ErrorMessage="Product 'My new product' was not found." />
    <cms:APIExample ID="apiGetAndUpdateVolumeDiscount" runat="server" ButtonText="Get and update discount" APIExampleType="ManageAdditional" InfoMessage="Discount 'My new discount' was updated." ErrorMessage="Discount 'My new discount' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateVolumeDiscounts" runat="server" ButtonText="Get and bulk update discounts" APIExampleType="ManageAdditional" InfoMessage="All discounts matching the condition were updated." ErrorMessage="Discounts matching the condition were not found." />
    <%-- Product option category --%>
    <cms:LocalizedHeading ID="headCreateOptionCategory" runat="server" Text="Product option category" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiCreateOptionCategory" runat="server" ButtonText="Create category" InfoMessage="Category 'My new category' was created." />
    <cms:APIExample ID="apiGetAndUpdateOptionCategory" runat="server" ButtonText="Get and update category" APIExampleType="ManageAdditional" InfoMessage="Category 'My new category' was updated." ErrorMessage="Category 'My new category' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateOptionCategories" runat="server" ButtonText="Get and bulk update categories" APIExampleType="ManageAdditional" InfoMessage="All categories matching the condition were updated." ErrorMessage="Categories matching the condition were not found." />
    <%-- Product option --%>
    <cms:LocalizedHeading ID="headCreateOption" runat="server" Text="Product option" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiCreateOption" runat="server" ButtonText="Create option" InfoMessage="Option 'My new option' was created." ErrorMessage="Department 'My new department' or option category 'My new category' were not found." />
    <cms:APIExample ID="apiGetAndUpdateOption" runat="server" ButtonText="Get and update option" APIExampleType="ManageAdditional" InfoMessage="Option 'My new option' was updated." ErrorMessage="Option 'My new option' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateOptions" runat="server" ButtonText="Get and bulk update options" APIExampleType="ManageAdditional" InfoMessage="All options matching the condition were updated." ErrorMessage="Options matching the condition were not found." />
    <%-- Option category on product --%>
    <cms:LocalizedHeading ID="headAddCategoryToProduct" runat="server" Text="Option category on product" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiAddCategoryToProduct" runat="server" ButtonText="Add category to product" InfoMessage="Category 'My new category' was added to product 'My new product'." ErrorMessage="Product 'My new product' or option category 'My new category' were not found." />
    <%-- Option on product --%>
    <cms:LocalizedHeading ID="headAllowOptionForProduct" runat="server" Text="Option for product" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiAllowOptionForProduct" runat="server" ButtonText="Allow option for product" InfoMessage="Option 'My new option' was allowed for product 'My new product'." ErrorMessage="Product 'My new product' or option 'My new option' were not found." />
    <%-- Product variants --%>
    <cms:LocalizedHeading ID="headCreateVariants" runat="server" Text="Product variants" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiCreateVariants" runat="server" ButtonText="Create product variants" InfoMessage="Variants for product 'My new product' were created." ErrorMessage="Product 'My new product' was not found." />
    <cms:APIExample ID="apiGetAndUpdateVariants" runat="server" ButtonText="Get and update variants" APIExampleType="ManageAdditional" InfoMessage="Product variants for 'My new product' were updated." ErrorMessage="Product 'My new product' or his variants were not found." />
    <%-- Section: Discounts --%>
    <cms:LocalizedHeading ID="headManDiscounts" runat="server" Text="Discounts" Level="4" EnableViewState="false" />
    <%-- Product coupon --%>
    <cms:LocalizedHeading ID="headCreateDiscountCoupon" runat="server" Text="Product coupon" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiCreateDiscountCoupon" runat="server" ButtonText="Create coupon" InfoMessage="Coupon 'My new coupon' was created." />
    <cms:APIExample ID="apiGetAndUpdateDiscountCoupon" runat="server" ButtonText="Get and update coupon" APIExampleType="ManageAdditional" InfoMessage="Coupon 'My new coupon' was updated." ErrorMessage="Coupon 'My new coupon' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateDiscountCoupons" runat="server" ButtonText="Get and bulk update coupons" APIExampleType="ManageAdditional" InfoMessage="All coupons matching the condition were updated." ErrorMessage="Coupons matching the condition were not found." />
    <%-- Product coupon products --%>
    <cms:LocalizedHeading ID="headAddProductToCoupon" runat="server" Text="Product coupon products" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiAddProductToCoupon" runat="server" ButtonText="Add product to coupon" InfoMessage="Product 'My new product' was added to coupon 'My new coupon'." ErrorMessage="Product 'My new product' or coupon 'My new coupon' were not found." />
    <%-- Section: Customers --%>
    <cms:LocalizedHeading ID="headManCustomers" runat="server" Text="Customers" Level="4" EnableViewState="false" />
    <%-- Customer --%>
    <cms:LocalizedHeading ID="headCreateCustomer" runat="server" Text="Customer" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiCreateAnonymousCustomer" runat="server" ButtonText="Create anonymous customer" InfoMessage="Customer 'My new anonymous customer' was created." />
    <cms:APIExample ID="apiCreateRegisteredCustomer" runat="server" ButtonText="Create registered customer" InfoMessage="Customer 'My new registered customer' was created." />
    <cms:APIExample ID="apiGetAndUpdateCustomer" runat="server" ButtonText="Get and update customer" APIExampleType="ManageAdditional" InfoMessage="Customer 'My new registered customer' was updated." ErrorMessage="Customer 'My new registered customer' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateCustomers" runat="server" ButtonText="Get and bulk update customers" APIExampleType="ManageAdditional" InfoMessage="All customers matching the condition were updated." ErrorMessage="Customers matching the condition were not found." />
    <%-- Customer address --%>
    <cms:LocalizedHeading ID="headCreateAddress" runat="server" Text="Address" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiCreateAddress" runat="server" ButtonText="Create address" InfoMessage="Address 'My new address' was created." ErrorMessage="Customer 'My new registered customer' or country 'USA' were not found." />
    <cms:APIExample ID="apiGetAndUpdateAddress" runat="server" ButtonText="Get and update address" APIExampleType="ManageAdditional" InfoMessage="Address 'My new address' was updated." ErrorMessage="Address 'My new address' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateAddresses" runat="server" ButtonText="Get and bulk update addresses" APIExampleType="ManageAdditional" InfoMessage="All addresses matching the condition were updated." ErrorMessage="Addresses matching the condition were not found." />
    <%-- Customer credit event --%>
    <cms:LocalizedHeading ID="headCreateCreditEvent" runat="server" Text="Credit event" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiCreateCreditEvent" runat="server" ButtonText="Create event" InfoMessage="Event 'My new event' was created." ErrorMessage="Customer 'My new registered customer' was not found." />
    <cms:APIExample ID="apiGetAndUpdateCreditEvent" runat="server" ButtonText="Get and update event" APIExampleType="ManageAdditional" InfoMessage="Event 'My new event' was updated." ErrorMessage="Event 'My new event' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateCreditEvents" runat="server" ButtonText="Get and bulk update events" APIExampleType="ManageAdditional" InfoMessage="All events matching the condition were updated." ErrorMessage="Events matching the condition were not found." />
    <cms:APIExample ID="apiGetTotalCredit" runat="server" ButtonText="Get total credit" APIExampleType="ManageAdditional" InfoMessage="Total credit of 'My new event'." ErrorMessage="Event 'My new event' was not found." />
    <%-- Customer newsletter --%>
    <cms:LocalizedHeading ID="headSubscribeCustomerToNewsletter" runat="server" Text="Customer newsletter" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiSubscribeCustomerToNewsletter" runat="server" ButtonText="Subscribe customer" InfoMessage="Customer 'My registered customer' was subscribed to newsletter 'Corporate newsletter'." ErrorMessage="Customer 'My new registered customer' or newsletter 'Corporate newsletter' were not found." />
    <%-- Customer wishlist --%>
    <cms:LocalizedHeading ID="headAddProductToWishlist" runat="server" Text="Customer wishlist" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiAddProductToWishlist" runat="server" ButtonText="Add product to wishlist" InfoMessage="Product 'My new product' was added to wishlist." ErrorMessage="Product 'My new product' or customer 'My new registered customer' were not found." />
    <%-- Section: Orders --%>
    <cms:LocalizedHeading ID="headManOrders" runat="server" Text="Orders" Level="4" EnableViewState="false" />
    <%-- Order --%>
    <cms:LocalizedHeading ID="headCreateOrder" runat="server" Text="Order" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiCreateOrder" runat="server" ButtonText="Create order" InfoMessage="Order 'My new order' was created." ErrorMessage="Customer 'My new registered customer', address 'My new address', currency 'My new currency' or order status 'My new order status' were not found." />
    <cms:APIExample ID="apiGetAndUpdateOrder" runat="server" ButtonText="Get and update order" APIExampleType="ManageAdditional" InfoMessage="Order 'My new order' was updated." ErrorMessage="Order 'My new order' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateOrders" runat="server" ButtonText="Get and bulk update orders" APIExampleType="ManageAdditional" InfoMessage="All orders matching the condition were updated." ErrorMessage="Orders matching the condition were not found." />
    <%-- Order item --%>
    <cms:LocalizedHeading ID="headCreateOrderItem" runat="server" Text="Order item" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiCreateOrderItem" runat="server" ButtonText="Create item" InfoMessage="Order item 'My new item' was created." ErrorMessage="Order 'My new order' was not found." />
    <cms:APIExample ID="apiGetAndUpdateOrderItem" runat="server" ButtonText="Get and update item" APIExampleType="ManageAdditional" InfoMessage="Order item 'My new item' was updated." ErrorMessage="Item 'My new item' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateOrderItems" runat="server" ButtonText="Get and bulk update items" APIExampleType="ManageAdditional" InfoMessage="All order items matching the condition were updated." ErrorMessage="Items matching the condition were not found." />
    <%-- Order status history --%>
    <cms:LocalizedHeading ID="headChangeOrderStatus" runat="server" Text="Order status history" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiChangeOrderStatus" runat="server" ButtonText="Change order status" APIExampleType="ManageAdditional" InfoMessage="Status 'My new status' was changed." ErrorMessage="Order 'My new order' or status 'My new status' or target status were not found." />
</asp:Content>
<asp:Content ID="contentRight" ContentPlaceHolderID="plcRightContainer" runat="server">
    <%-- Section: Orders --%>
    <cms:LocalizedHeading ID="headCleanOrders" runat="server" Text="Orders" Level="4" EnableViewState="false" />
    <%-- Order status history --%>
    <cms:LocalizedHeading ID="headDeleteHistory" runat="server" Text="Order status history" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteHistory" runat="server" ButtonText="Delete history" APIExampleType="CleanUpMain" InfoMessage="Order status history for order 'My new order' was deleted." ErrorMessage="Order 'My new order' was not found." />
    <%-- Order item --%>
    <cms:LocalizedHeading ID="headDeleteOrderItem" runat="server" Text="Order item" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteOrderItem" runat="server" ButtonText="Delete item" APIExampleType="CleanUpMain" InfoMessage="Order item 'My new item' and all its dependencies were deleted." ErrorMessage="Order item 'My new item' was not found." />
    <%-- Order --%>
    <cms:LocalizedHeading ID="headDeleteOrder" runat="server" Text="Order" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteOrder" runat="server" ButtonText="Delete order" APIExampleType="CleanUpMain" InfoMessage="Order 'My new order' and all its dependencies were deleted." ErrorMessage="Order 'My new order' was not found." />
    <%-- Section: Customers --%>
    <cms:LocalizedHeading ID="headCleanCustomers" runat="server" Text="Customers" Level="4" EnableViewState="false" />
    <%-- Customer discount level --%>
    <cms:LocalizedHeading ID="headRemoveProductFromWishlist" runat="server" Text="Customer wishlist" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiRemoveProductFromWishlist" runat="server" ButtonText="Remove product from wishlist" APIExampleType="CleanUpMain" InfoMessage="Product 'My new product' was removed from wishlist." ErrorMessage="Product 'My new product', customer 'My new registered customer' were not found or wishlist is empty." />
    <%-- Customer newsletter --%>
    <cms:LocalizedHeading ID="headUnsubscribeCustomerFromNewsletter" runat="server" Text="Customer newsletter" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiUnsubscribeCustomerFromNewsletter" runat="server" ButtonText="Unsubscribe customer" APIExampleType="CleanUpMain" InfoMessage="Customer 'My new registered customer' was unsubscribed from newsletter 'Corporate newsletter'." ErrorMessage="Customer 'My new registered customer', newsletter 'Corporate newsletter' or their relationship were not found." />
    <%-- Customer credit event --%>
    <cms:LocalizedHeading ID="headDeleteCreditEvent" runat="server" Text="Credit event" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteCreditEvent" runat="server" ButtonText="Delete event" APIExampleType="CleanUpMain" InfoMessage="Event 'My new event' and all its dependencies were deleted." ErrorMessage="Event 'My new event' was not found." />
    <%-- Customer address --%>
    <cms:LocalizedHeading ID="headDeleteAddress" runat="server" Text="Address" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteAddress" runat="server" ButtonText="Delete address" APIExampleType="CleanUpMain" InfoMessage="Address 'My new address' and all its dependencies were deleted." ErrorMessage="Address 'My new address' was not found." />
    <%-- Customer --%>
    <cms:LocalizedHeading ID="headDeleteCustomer" runat="server" Text="Customer" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteCustomer" runat="server" ButtonText="Delete customer" APIExampleType="CleanUpMain" InfoMessage="Customers 'My new registered customer', 'My new anonymous customer' and all its dependencies were deleted." ErrorMessage="Customer 'My new registered customer' was not found." />
    <%-- Section: Discounts --%>
    <cms:LocalizedHeading ID="headCleanDiscounts" runat="server" Text="Discounts" Level="4" EnableViewState="false" />
    <%-- Product coupon products --%>
    <cms:LocalizedHeading ID="headRemoveProductFromCoupon" runat="server" Text="Product coupon products" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiRemoveProductFromCoupon" runat="server" ButtonText="Remove product from coupon" APIExampleType="CleanUpMain" InfoMessage="Product 'My new product' was removed from the coupon 'My new coupon'." ErrorMessage="Product 'My new product', coupon 'My new coupon' or their relationship were not found." />
    <%-- Product coupon --%>
    <cms:LocalizedHeading ID="headDeleteDiscountCoupon" runat="server" Text="Product coupon" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteDiscountCoupon" runat="server" ButtonText="Delete coupon" APIExampleType="CleanUpMain" InfoMessage="Coupon 'My new coupon' and all its dependencies were deleted." ErrorMessage="Coupon 'My new coupon' was not found." />
    <%-- Section: Products --%>
    <cms:LocalizedHeading ID="headCleanProducts" runat="server" Text="Products" Level="4" EnableViewState="false" />
    <%-- Option on product --%>
    <cms:LocalizedHeading ID="headRemoveOptionFromProduct" runat="server" Text="Option for product" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiRemoveOptionFromProduct" runat="server" ButtonText="Remove option from product" APIExampleType="CleanUpMain" InfoMessage="Option 'My new option' was removed from product 'My new product'." ErrorMessage="Option 'My new option', product 'My new product' or their relationship were not found." />
    <%-- Option category on product --%>
    <cms:LocalizedHeading ID="headRemoveCategoryFromProduct" runat="server" Text="Option category on product" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiRemoveCategoryFromProduct" runat="server" ButtonText="Remove category from product" APIExampleType="CleanUpMain" InfoMessage="Category 'My new category' was removed from product 'My new product'." ErrorMessage="Category 'My new category', product 'My new product' or their relationship were not found." />
    <%-- Product option --%>
    <cms:LocalizedHeading ID="headDeleteOption" runat="server" Text="Product option" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteOption" runat="server" ButtonText="Delete option" APIExampleType="CleanUpMain" InfoMessage="Option 'My new option' and all its dependencies were deleted." ErrorMessage="Option 'My new option' was not found." />
    <%-- Product option category --%>
    <cms:LocalizedHeading ID="headDeleteOptionCategory" runat="server" Text="Product option category" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteOptionCategory" runat="server" ButtonText="Delete category" APIExampleType="CleanUpMain" InfoMessage="Category 'My new category' and all its dependencies were deleted." ErrorMessage="Category 'My new category' was not found." />
    <%-- Product variants --%>
    <cms:LocalizedHeading ID="headDeleteVariants" runat="server" Text="Product variants" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteVariants" runat="server" ButtonText="Delete product variants" APIExampleType="CleanUpMain" InfoMessage="Variants for product 'My new product' were deleted." ErrorMessage="Product 'My new product' was not found." />
    <%-- Volume discount --%>
    <cms:LocalizedHeading ID="headDeleteVolumeDiscount" runat="server" Text="Volume discount" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteVolumeDiscount" runat="server" ButtonText="Delete discount" APIExampleType="CleanUpMain" InfoMessage="Discount 'My new discount' and all its dependencies were deleted." ErrorMessage="Discount 'My new discount' was not found." />
    <%-- Product tax class --%>
    <cms:LocalizedHeading ID="headRemoveTaxClassFromProduct" runat="server" Text="Tax class" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiRemoveTaxClassFromProduct" runat="server" ButtonText="Remove tax class from product" APIExampleType="CleanUpMain" InfoMessage="Class 'My new tax class' was removed from product 'My new product'." ErrorMessage="Class 'My new tax class', product 'My new product' or their relationship were not found." />
    <%-- Shopping cart --%>
    <cms:LocalizedHeading ID="headRemoveProductFromShoppingCart" runat="server" Text="Shopping cart" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiRemoveProductFromShoppingCart" runat="server" ButtonText="Remove product from cart" APIExampleType="CleanUpMain" InfoMessage="Product 'My new product' was removed from shopping cart." ErrorMessage="Product 'My new product', shopping cart item, shopping cart or their relationship were not found." />
    <%-- Product bundle --%>
    <cms:LocalizedHeading ID="headDeleteBundle" runat="server" Text="Bundle" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteBundle" runat="server" ButtonText="Delete Bundle" APIExampleType="CleanUpMain" InfoMessage="Bundle 'My new bundle' with all its dependencies were deleted." ErrorMessage="Bundle 'My new bundle' was not found." />
    <%-- Product donation --%>
    <cms:LocalizedHeading ID="headDeleteDonation" runat="server" Text="Donation" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteDonation" runat="server" ButtonText="Delete Donation" APIExampleType="CleanUpMain" InfoMessage="Donation 'My new donation' with all its dependencies were deleted." ErrorMessage="Donation 'My new donation' was not found." />
    <%-- Product eProduct --%>
    <cms:LocalizedHeading ID="headDeleteEProduct" runat="server" Text="EProduct" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteEProduct" runat="server" ButtonText="Delete Eproduct" APIExampleType="CleanUpMain" InfoMessage="Eproduct 'My new eProduct' with all its dependencies were deleted." ErrorMessage="Product 'My new eProduct' was not found." />
    <%-- Product membership --%>
    <cms:LocalizedHeading ID="headDeleteMembershipProduct" runat="server" Text="Membership" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteMembershipProduct" runat="server" ButtonText="Delete membership product" APIExampleType="CleanUpMain" InfoMessage="Membership 'My new membership' and product 'My new membership product' with all its dependencies were deleted." ErrorMessage="Product 'My new membership product' was not found." />
    <%-- Product page --%>
    <cms:LocalizedHeading ID="headDeleteProductDocument" runat="server" Text="Product page" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteProductDocument" runat="server" ButtonText="Delete page" APIExampleType="CleanUpMain" InfoMessage="Page 'My new page' and all its dependencies were deleted." ErrorMessage="Page 'My new page' was not found." />
    <%-- Product --%>
    <cms:LocalizedHeading ID="headDeleteProduct" runat="server" Text="Product" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteProduct" runat="server" ButtonText="Delete product" APIExampleType="CleanUpMain" InfoMessage="Product 'My new product' and all its dependencies were deleted." ErrorMessage="Product 'My new product' was not found." />
    <%-- Section: Configuration --%>
    <cms:LocalizedHeading ID="headCleanConfiguration" runat="server" Text="Configuration" Level="4" EnableViewState="false" />
    <%-- Supplier --%>
    <cms:LocalizedHeading ID="headDeleteSupplier" runat="server" Text="Supplier" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteSupplier" runat="server" ButtonText="Delete supplier" APIExampleType="CleanUpMain" InfoMessage="Supplier 'My new supplier' and all its dependencies were deleted." ErrorMessage="Supplier 'My new supplier' was not found." />
    <%-- Manufacturer --%>
    <cms:LocalizedHeading ID="headDeleteManufacturer" runat="server" Text="Manufacturer" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteManufacturer" runat="server" ButtonText="Delete manufacturer" APIExampleType="CleanUpMain" InfoMessage="Manufacturer 'My new manufacturer' and all its dependencies were deleted." ErrorMessage="Manufacturer 'My new manufacturer' was not found." />
    <%-- Payment method --%>
    <cms:LocalizedHeading ID="headDeletePaymentMethod" runat="server" Text="Payment method" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiDeletePaymentMethod" runat="server" ButtonText="Delete method" APIExampleType="CleanUpMain" InfoMessage="Method 'My new method' and all its dependencies were deleted." ErrorMessage="Method 'My new method' was not found." />
    <%-- Shipping option --%>
    <cms:LocalizedHeading ID="headDeleteShippingOption" runat="server" Text="Shipping option" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiRemoveTaxClassFromOption" runat="server" ButtonText="Remove tax from option" APIExampleType="CleanUpMain" InfoMessage="Tax class 'My new class' was removed from option 'My new option'." ErrorMessage="Tax class 'My new class', option 'My new option' or their relationship were not found." />
    <cms:APIExample ID="apiRemoveShippingCostFromOption" runat="server" ButtonText="Remove cost from option" APIExampleType="CleanUpMain" InfoMessage="Shipping cost was removed from option 'My new option'." ErrorMessage="Shipping cost, option 'My new option' or their relationship were not found." />
    <cms:APIExample ID="apiDeleteShippingOption" runat="server" ButtonText="Delete option" APIExampleType="CleanUpMain" InfoMessage="Option 'My new option' and all its dependencies were deleted." ErrorMessage="Option 'My new option' was not found." />
    <%-- Department default tax class --%>
    <cms:LocalizedHeading ID="headRemoveTaxClassFromDepartment" runat="server" Text="Department default tax class" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiRemoveTaxClassFromDepartment" runat="server" ButtonText="Remove tax from department" APIExampleType="CleanUpMain" InfoMessage=" Class 'My new class' was removed from department 'My new department'." ErrorMessage="Class 'My new class', department 'My new department' or their relationship were not found." />
    <%-- Department user --%>
    <cms:LocalizedHeading ID="headRemoveUserFromDepartment" runat="server" Text="Department user" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiRemoveUserFromDepartment" runat="server" ButtonText="Remove user from department" APIExampleType="CleanUpMain" InfoMessage="Current user was removed from department 'My new department'" ErrorMessage="Department 'My new department' or its relationship to the current user were not found." />
    <%-- Department --%>
    <cms:LocalizedHeading ID="headDeleteDepartment" runat="server" Text="Department" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteDepartment" runat="server" ButtonText="Delete department" APIExampleType="CleanUpMain" InfoMessage="Department 'My new department' and all its dependencies were deleted." ErrorMessage="Department 'My new department' was not found." />
    <%-- Internal status --%>
    <cms:LocalizedHeading ID="headDeleteInternalStatus" runat="server" Text="Internal status" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteInternalStatus" runat="server" ButtonText="Delete status" APIExampleType="CleanUpMain" InfoMessage="Status 'My new status' and all its dependencies were deleted." ErrorMessage="Status 'My new status' was not found." />
    <%-- Public status --%>
    <cms:LocalizedHeading ID="headDeletePublicStatus" runat="server" Text="Public status" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiDeletePublicStatus" runat="server" ButtonText="Delete status" APIExampleType="CleanUpMain" InfoMessage="Status 'My new status' and all its dependencies were deleted." ErrorMessage="Status 'My new status' was not found." />
    <%-- Order status --%>
    <cms:LocalizedHeading ID="headDeleteOrderStatus" runat="server" Text="Order status" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteOrderStatus" runat="server" ButtonText="Delete status" APIExampleType="CleanUpMain" InfoMessage="Status 'My new status' and all its dependencies were deleted." ErrorMessage="Status 'My new status' was not found." />
    <%-- Exchange table --%>
    <cms:LocalizedHeading ID="headDeleteExchangeTable" runat="server" Text="Exchange table" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteExchangeTable" runat="server" ButtonText="Delete table" APIExampleType="CleanUpMain" InfoMessage="Table 'My new table' and all its dependencies were deleted." ErrorMessage="Table 'My new table' was not found." />
    <%-- Currency --%>
    <cms:LocalizedHeading ID="headDeleteCurrency" runat="server" Text="Currency" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteCurrency" runat="server" ButtonText="Delete currency" APIExampleType="CleanUpMain" InfoMessage="Currency 'My new currency' and all its dependencies were deleted." ErrorMessage="Currency 'My new currency' was not found." />
    <%-- Tax class value in state --%>
    <cms:LocalizedHeading ID="headDeleteTaxClassValueInState" runat="server" Text="Tax class value in state" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteTaxClassValueInState" runat="server" ButtonText="Delete value" APIExampleType="CleanUpMain" InfoMessage="Value was deleted." ErrorMessage="Class 'My new class', state 'Alabama' or their relationship were not found." />
    <%-- Tax class value in country --%>
    <cms:LocalizedHeading ID="headDeleteTaxClassValueInCountry" runat="server" Text="Tax class value in country" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteTaxClassValueInCountry" runat="server" ButtonText="Delete value" APIExampleType="CleanUpMain" InfoMessage="Value was deleted." ErrorMessage="Class 'My new class', country 'USA' or their relationship were not found." />
    <%-- Tax class --%>
    <cms:LocalizedHeading ID="headDeleteTaxClass" runat="server" Text="Tax class" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteTaxClass" runat="server" ButtonText="Delete class" APIExampleType="CleanUpMain" InfoMessage="Class 'My new class' and all its dependencies were deleted." ErrorMessage="Class 'My new class' was not found." />
    <%-- Checkout process step --%>
    <cms:LocalizedHeading ID="headDeleteCheckoutProcessStep" runat="server" Text="Checkout process step" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteCheckoutProcessStep" runat="server" ButtonText="Delete step" APIExampleType="CleanUpMain" InfoMessage="Step 'My new step' and all its dependencies were deleted." ErrorMessage="Step 'My new step' was not found." />
</asp:Content>
