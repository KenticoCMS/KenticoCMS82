using System;
using System.Data;
using System.Linq;
using System.Text;

using CMS.Core;
using CMS.Ecommerce;
using CMS.Helpers;
using CMS.UIControls;
using CMS.FormControls;
using CMS.Globalization;

// Edited object
[EditedObject("ecommerce.address", "addressId")]
// Parent object
[ParentObject(CustomerInfo.OBJECT_TYPE, "customerId")]
// Breadcrumbs
[Breadcrumbs]
[Breadcrumb(0, "Customer_Edit_Address_Edit.ItemListLink", "~/CMSModules/Ecommerce/Pages/Tools/Customers/Customer_Edit_Address_List.aspx?customerId={%EditedObjectParent.ID%}", null)]
[Breadcrumb(1, "customer_edit_address_new.headercaption", NewObject = true)]
[Breadcrumb(1, Text = "{%EditedObject.AddressName%}", ExistingObject = true)]
// Security
[UIElement(ModuleName.ECOMMERCE, "Customers.Addresses")]
// Help
[Help("newedit_address", "helpTopic")]
public partial class CMSModules_Ecommerce_Pages_Tools_Customers_Customer_Edit_Address_Edit : CMSCustomersPage
{
    #region "Properties"

    /// <summary>
    /// Gets Customer ID from query string.
    /// </summary>
    private int CustomerID
    {
        get
        {
            return QueryHelper.GetInteger("customerID", 0);
        }
    }

    #endregion


    #region "Page events"

    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        var addressObj = EditedObject as AddressInfo;

        // Object is created
        if (addressObj == null)
        {
            EditForm.OnAfterDataLoad += EditForm_OnAfterDataLoad;
        }

        // Check customer and his site binding 
        var customer = EditedObjectParent as CustomerInfo;
        if ((customer == null) || (!ECommerceContext.CheckCustomerSiteID(customer))
            || ((addressObj != null) && (addressObj.AddressCustomerID != customer.CustomerID)))
        {
            EditedObject = null;
        } 

        EditForm.OnBeforeSave += EditForm_OnBeforeSave;
        EditForm.OnItemValidation += EditForm_OnItemValidation;
        EditForm.OnCheckPermissions += EditForm_OnCheckPermissions;
    }


    void EditForm_OnCheckPermissions(object sender, EventArgs e)
    {
        // Check permissions
        if (!ECommerceContext.IsUserAuthorizedToModifyCustomer())
        {
            RedirectToAccessDenied("CMS.Ecommerce", "EcommerceModify OR ModifyCustomers");
        }
    }


    void EditForm_OnItemValidation(object sender, ref string errorMessage)
    {
        FormEngineUserControl ctrl = sender as FormEngineUserControl;

        // Checking country selector if some country was selected
        if ((ctrl != null) && (ctrl.FieldInfo.Name == "AddressCountryID"))
        {
            int countryId = ValidationHelper.GetInteger(ctrl.Value, 0);

            if (countryId == 0)
            {
                errorMessage = GetString("basicform.erroremptyvalue");
            }
        }
    }


    protected void EditForm_OnAfterDataLoad(object sender, EventArgs e)
    {
        // Fill default values to the form
        CustomerInfo customerInfo = CustomerInfoProvider.GetCustomerInfo(CustomerID);
        if (customerInfo != null)
        {
            if ((string.IsNullOrEmpty(customerInfo.CustomerFirstName)) && (string.IsNullOrEmpty(customerInfo.CustomerLastName)))
            {
                FillDefaultValue("AddressPersonalName", customerInfo.CustomerCompany);
            }
            else
            {
                FillDefaultValue("AddressPersonalName", customerInfo.CustomerFirstName + " " + customerInfo.CustomerLastName);
            }

            FillDefaultValue("AddressPhone", customerInfo.CustomerPhone);
            FillDefaultValue("AddressCountryID", customerInfo.CustomerCountryID);

            // Preselect state
            EditForm.Data.SetValue("AddressStateID", customerInfo.CustomerStateID);
        }
    }

    
    protected void EditForm_OnBeforeSave(object sender, EventArgs e)
    {
        AddressInfo editFormObj = EditForm.Data as AddressInfo;

        // Fill hidden AddressName column
        if (editFormObj != null)
        {
            editFormObj.AddressName = ComposeAddressName();
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Composes text for AddressName column in database.
    /// </summary>
    /// <returns>AddressName column text</returns>
    private string ComposeAddressName()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(EditForm.GetFieldValue("AddressPersonalName") + ", ");

        if (!string.IsNullOrEmpty(ValidationHelper.GetString(EditForm.GetFieldValue("AddressLine1"), string.Empty)))
        {
            sb.Append(EditForm.GetFieldValue("AddressLine1") + ", ");
        }

        if (!string.IsNullOrEmpty(ValidationHelper.GetString(EditForm.GetFieldValue("AddressLine2"), string.Empty)))
        {
            sb.Append(EditForm.GetFieldValue("AddressLine2") + ", ");
        }

        sb.Append(EditForm.GetFieldValue("AddressCity"));

        return TextHelper.LimitLength(sb.ToString(), 200);
    }


    /// <summary>
    /// Fill values to form
    /// </summary>
    /// <param name="columnName">Column name</param>
    /// <param name="value">Value to fill</param>
    private void FillDefaultValue(string columnName, object value)
    {
        if (EditForm.FieldEditingControls[columnName] != null)
        {
            EditForm.FieldEditingControls[columnName].Value = value;
        }
    }

    #endregion
}