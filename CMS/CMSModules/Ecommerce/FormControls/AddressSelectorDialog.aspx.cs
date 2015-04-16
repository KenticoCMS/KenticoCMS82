using System;
using System.Data;
using System.Text;

using CMS.DataEngine;
using CMS.Ecommerce;
using CMS.FormControls;
using CMS.Helpers;
using CMS.Globalization;
using CMS.UIControls;

// Parent object
[ParentObject("ecommerce.customer", "customerId")]
// Edited object
[EditedObject("ecommerce.address", "addressId")]

// Set Title Text
[Title("Order_Edit_Address.Title", ExistingObject = true)]
[Title("Order_Edit_Address.Title", NewObject = true)]
public partial class CMSModules_Ecommerce_FormControls_AddressSelectorDialog : CMSEcommerceModalPage
{
    #region "Properties"

    /// <summary>
    /// Gets or sets the customer ID.
    /// </summary>
    public int CustomerID
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the address selector client ID. This is used for calling right javascript function on submit.
    /// </summary>
    public string AddressSelectorClientID
    {
        get;
        set;
    }

    #endregion


    #region "Page Events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        Save += btnOK_Click;

        CustomerID = QueryHelper.GetInteger("customerId", 0);
        AddressSelectorClientID = QueryHelper.GetString("selectorid", string.Empty);

        // Register check permissions
        EditForm.OnCheckPermissions += EditForm_OnCheckPermissions;

        // Before save event
        EditForm.OnBeforeSave += EditForm_OnBeforeSave;

        // On item validation event
        EditForm.OnItemValidation += EditForm_OnItemValidation;

        // Check edited objects site id
        CustomerInfo customerInfo = EditedObjectParent as CustomerInfo;
        if (customerInfo != null)
        {
            if (!ECommerceContext.CheckCustomerSiteID(customerInfo))
            {
                EditedObject = null;
            }
        }

        // Check 'EcommerceRead' permission
        if (!ECommerceContext.IsUserAuthorizedForPermission("ReadCustomers"))
        {
            RedirectToAccessDenied("CMS.Ecommerce", "EcommerceRead OR ReadCustomers");
        }
    }

    #endregion


    #region "Event handlers"

    protected void EditForm_OnAfterDataLoad(object sender, EventArgs e)
    {
        // New objects only
        if ((EditedObject == null) || (((BaseInfo)EditedObject).Generalized.ObjectID <= 0))
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
    }


    protected void EditForm_OnBeforeSave(object sender, EventArgs e)
    {

        AddressInfo AddressObj = EditForm.Data as AddressInfo;
        if (AddressObj != null)
        {
            // Fill hidden AddressName column
            AddressObj.AddressName = ComposeAddressName();

            // When creating a new one set type of address
            if ((EditedObject as AddressInfo) == null)
            {
                // Set type of address
                int typeId = QueryHelper.GetInteger("typeId", 0);

                switch (typeId)
                {
                    // Shipping address selection
                    case 1:
                        AddressObj.AddressIsShipping = true;
                        break;
                    // Billing address selection
                    case 2:
                        AddressObj.AddressIsBilling = true;
                        break;
                    // Company address selection
                    case 3:
                        AddressObj.AddressIsCompany = true;
                        break;
                    default:
                        AddressObj.AddressIsShipping = true;
                        AddressObj.AddressIsBilling = true;
                        AddressObj.AddressIsCompany = true;
                        break;
                }
            }
        }
    }


    protected void EditForm_OnCheckPermissions(object sender, EventArgs args)
    {
        // Check 'EcommerceModify' permission
        if (!ECommerceContext.IsUserAuthorizedForPermission("ModifyCustomers"))
        {
            RedirectToAccessDenied("CMS.Ecommerce", "EcommerceModify OR ModifyCustomers");
        }
    }


    protected void EditForm_OnItemValidation(object sender, ref string errorMessage)
    {
        FormEngineUserControl ctrl = sender as FormEngineUserControl;

        // Checking countryselector if some country was selected
        if ((ctrl != null) && (ctrl.FieldInfo.Name == "AddressCountryID"))
        {
            int countryId = ValidationHelper.GetInteger(ctrl.Value, 0);

            if (countryId == 0)
            {
                errorMessage = GetString("basicform.erroremptyvalue");
            }
        }
    }


    protected void btnOK_Click(object sender, EventArgs e)
    {
        if (EditForm.ValidateData())
        {
            // Set data to database
            EditForm.RedirectUrlAfterCreate = "";
            EditForm.SaveData("");

            AddressInfo AddressObj = EditForm.Data as AddressInfo;
            if (AddressObj != null)
            {
                RegisterChangeSelector(AddressSelectorClientID, AddressObj.AddressID);
            }
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


    /// <summary>
    /// Registers script for changing the selector.
    /// </summary>
    /// <param name="selector">The selector client id, given retrieved from query string.</param>
    /// <param name="addressId">The address id.</param>
    private void RegisterChangeSelector(string selector, int addressId)
    {
        // Check for selector ID
        if (!string.IsNullOrEmpty(selector) && (addressId > 0))
        {
            // Add selector refresh
            string script = string.Format(@"if (wopener && wopener.US_SelectNewValue_{0}) {{wopener.US_SelectNewValue_{0}('{1}'); }} CloseDialog();",
                                          HTMLHelper.HTMLEncode(selector), addressId);
            ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "UpdateSelector", ScriptHelper.GetScript(script));
        }
    }

    #endregion
}
