using System;

using CMS.Core;
using CMS.Ecommerce;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;

// Edited object
[EditedObject(CreditEventInfo.OBJECT_TYPE, "eventid")]
[ParentObject(CustomerInfo.OBJECT_TYPE, "customerid")]
// Breadcrumbs
[Breadcrumbs]
[Breadcrumb(0, "CreditEvent_Edit.ItemListLink", "Customer_Edit_Credit_List.aspx?customerid={%EditedObjectParent.ID%}&siteId={%EditedObjectParent.SiteID%}", null)]
[Breadcrumb(1, Text = "{%EditedObject.DisplayName%}", ExistingObject = true)]
[Breadcrumb(1, "CreditEvent_Edit.NewItemCaption", NewObject = true)]
// Help
[Help("newedit_credit_event", "helpTopic")]
// Security
[UIElement(ModuleName.ECOMMERCE, "Customers.Credit")]
public partial class CMSModules_Ecommerce_Pages_Tools_Customers_Customer_Edit_Credit_Edit : CMSCustomersPage
{
    private int creditSiteId = -1;


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        EditForm.OnBeforeValidate += EditForm_OnBeforeValidate;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        creditSiteId = ECommerceHelper.GetSiteID(SiteContext.CurrentSiteID, ECommerceSettings.USE_GLOBAL_CREDIT);
        
        CreditEventInfo creditEvent = EditedObject as CreditEventInfo;
        // Check site of credit event
        if ((creditEvent != null) && (creditEvent.EventID > 0) && (creditEvent.EventSiteID != creditSiteId))
        {
            EditedObject = null;
        }

        creditEvent = EditForm.Data as CreditEventInfo;
        if ((creditEvent != null) && (creditEvent.EventID == 0))
        {
            creditEvent.EventSiteID = creditSiteId;
        }

        CustomerInfo customer = EditedObjectParent as CustomerInfo;
        // Check if customer belongs to current site
        if (!CheckCustomerSiteID(customer))
        {
            EditedObject = null;
        }

        // Check presence of main currency
        CheckMainCurrency(creditSiteId);
        
        // Register check permissions
        EditForm.OnCheckPermissions += (s, args) => CheckPermissions();
    }


    protected void EditForm_OnBeforeValidate(object sender, EventArgs e)
    {
        var priceSelector = EditForm.FieldEditingControls["EventCreditChange"];

        // Round event change according site currency
        if (priceSelector != null)
        {
            var price = ValidationHelper.GetDouble(priceSelector.Value, 0.0);
            if (price != 0d)
            {
                priceSelector.Value = CurrencyInfoProvider.RoundTo(price, CurrencyInfoProvider.GetMainCurrency(creditSiteId));
            }
        }
    }


    /// <summary>
    /// Check if user is authorized to modify Customer's credit.
    /// </summary>
    private void CheckPermissions()
    {
        // Check modify permission
        if (!ECommerceContext.IsUserAuthorizedToModifyCustomer())
        {
            RedirectToAccessDenied("CMS.Ecommerce", "EcommerceModify OR ModifyCustomers");
        }

        // Check if using global credit
        if (creditSiteId <= 0)
        {
            // Check Ecommerce global modify permission
            if (!ECommerceContext.IsUserAuthorizedForPermission("EcommerceGlobalModify"))
            {
                RedirectToAccessDenied("CMS.Ecommerce", "EcommerceGlobalModify");
            }
        }
    }
}