using System;
using System.Linq;

using CMS.Base;
using CMS.Ecommerce;
using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.Helpers;


/// <summary>
/// Customer registration web part for checkout process
/// </summary>
public partial class CMSWebParts_Ecommerce_Checkout_Forms_CustomerDetail : CMSCheckoutWebPart
{
    #region "Constants"

    const string COMPANY_TYPE = "Company";

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets a value indicating whether propagating changes on postback is allowed.
    /// </summary>    
    public bool PropagateChangesOnPostback
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("PropagateChangesOnPostback"), false);
        }
        set
        {
            SetValue("PropagateChangesOnPostback", value);
        }
    }


    /// <summary>
    /// Alternative form name for this web part.
    /// </summary>
    public string AlternativeFormName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("AlternativeFormName"), "");
        }
        set
        {
            SetValue("AlternativeFormName", value);
        }
    }


    /// <summary>
    /// Gets the customer type selector [Personal/Company]. Returns null if alternative form does not include this field.
    /// </summary>    
    private FormEngineUserControl TypeSelector
    {
        get
        {
            return customerForm.FieldControls["AccountType"];
        }
    }

    #endregion


    #region "Life cycle"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        string[] splitFormName = AlternativeFormName.Split('.');
        // UIForm cant process full path of alternative form if object type is already specified.
        customerForm.AlternativeFormName = splitFormName.LastOrDefault();
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        customerForm.OnBeforeSave += customerForm_OnBeforeSave;

        // Set company type for customer with specified company name. Ignore in postback to ensure change selection is possible.
        if (!RequestHelper.IsPostBack())
        {
            FormEngineUserControl typeSelector = TypeSelector;
            string customerCompany = ShoppingCart.Customer == null ? "" : ShoppingCart.Customer.CustomerCompany;

            if ((typeSelector != null) && !string.IsNullOrEmpty(customerCompany))
            {
                typeSelector.Value = COMPANY_TYPE;
            }
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        customerForm.SubmitButton.Visible = false;
    }

    #endregion


    #region "Form events"

    protected void customerForm_OnBeforeSave(object sender, EventArgs e)
    {
        // Cancel saving, just set current filed values into EditableObject through UIForm.SaveData method
        customerForm.StopProcessing = true;
    }

    #endregion


    #region "Wizard methods"

    protected override void StepLoaded(object sender, StepEventArgs e)
    {
        base.StepLoaded(sender, e);

        if (ShoppingCart.Customer != null)
        {
            customerForm.EditedObject = ShoppingCart.Customer;
        }

        if (!RequestHelper.IsPostBack())
        {
            customerForm.ReloadData();
        }

        // Set first time user customer for postback tax recalculation
        if (PropagateChangesOnPostback && (ShoppingCart.Customer == null))
        {
            ShoppingCart.Customer = customerForm.EditedObject as CustomerInfo;
        }

        // Propagate changes on postback if there is customer with company type and some tax registration id
        if (PropagateChangesOnPostback && (ShoppingCart.Customer != null))
        {
            FormEngineUserControl typeSelector = TypeSelector;
            bool isPersonalType = (typeSelector != null) && (!typeSelector.Value.Equals(COMPANY_TYPE));
            var customerTaxRegistrationID = customerForm.GetFieldValue("CustomerTaxRegistrationID");

            if ((customerTaxRegistrationID == null) || isPersonalType)
            {
                ShoppingCart.Customer.CustomerTaxRegistrationID = string.Empty;
            }
            else
            {
                ShoppingCart.Customer.CustomerTaxRegistrationID = customerTaxRegistrationID.ToString();
            }

            ShoppingCart.InvalidateCalculations();
            ComponentEvents.RequestEvents.RaiseEvent(null, e, SHOPPING_CART_CHANGED);
        }
    }


    protected override void ValidateStepData(object sender, StepEventArgs e)
    {
        base.ValidateStepData(sender, e);

        if (!customerForm.ValidateData())
        {
            if (e != null)
            {
                e.CancelEvent = true;
            }
        }
    }


    protected override void SaveStepData(object sender, StepEventArgs e)
    {
        base.SaveStepData(sender, e);

        // Just set current filed values into EditableObject, saving was canceled in OnBeforeSave
        customerForm.SaveData(null, false);

        CustomerInfo customer = customerForm.EditedObject as CustomerInfo;
        FormEngineUserControl typeSelector = TypeSelector;

        // Clear company fields for non-company type
        if ((typeSelector != null) && (!typeSelector.Value.Equals(COMPANY_TYPE)))
        {
            customer.CustomerCompany = "";
            customer.CustomerOrganizationID = "";
            customer.CustomerTaxRegistrationID = "";
        }

        ShoppingCart.Customer = customer;
    }

    #endregion
}