using System;

using CMS.Core;
using CMS.Ecommerce;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;

[EditedObject(CustomerInfo.OBJECT_TYPE, "customerId")]
[Action(0, "Customer_Edit_Address_List.NewItemCaption", "Customer_Edit_Address_Edit.aspx?customerId={%EditedObject.ID%}")]
[UIElement(ModuleName.ECOMMERCE, "Customers.Addresses")]
public partial class CMSModules_Ecommerce_Pages_Tools_Customers_Customer_Edit_Address_List : CMSCustomersPage
{
    #region "Variables"

    protected CustomerInfo customerObj = null;

    #endregion


    #region "Page Events"

    protected void Page_Load(object sender, EventArgs e)
    {
        customerObj = EditedObject as CustomerInfo;

        // Check if customer belongs to current site
        if (!CheckCustomerSiteID(customerObj))
        {
            EditedObject = null;
        }

        if (customerObj != null)
        {
            UniGrid.OnAction += uniGrid_OnAction;
            UniGrid.WhereCondition = "AddressCustomerID = " + customerObj.CustomerID;
        }
    }


    /// <summary>
    /// PreRender event handler
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Hide 'Use for company' column when disabled in settings
        if (UniGrid.NamedColumns.ContainsKey("AddressIsCompany"))
        {
            UniGrid.NamedColumns["AddressIsCompany"].Visible = ECommerceSettings.UseExtraCompanyAddress(SiteContext.CurrentSiteID);
        }
    }

    #endregion


    #region "Event Handlers"

    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void uniGrid_OnAction(string actionName, object actionArgument)
    {
        int addressId = ValidationHelper.GetInteger(actionArgument, 0);

        if (actionName == "edit")
        {
            URLHelper.Redirect("Customer_Edit_Address_Edit.aspx?customerId=" + customerObj.CustomerID + "&addressId=" + addressId);
        }
        else if (actionName == "delete")
        {
            if (!ECommerceContext.IsUserAuthorizedToModifyCustomer())
            {
                RedirectToAccessDenied("CMS.Ecommerce", "EcommerceModify OR ModifyCustomers");
            }

            var address = AddressInfoProvider.GetAddressInfo(addressId);

            // Check for the address dependencies            
            if ((address != null) && address.Generalized.CheckDependencies())
            {
                ShowError(ECommerceHelper.GetDependencyMessage(address));
                return;
            }

            // Delete AddressInfo object from database
            AddressInfoProvider.DeleteAddressInfo(address);
        }
    }

    #endregion
}