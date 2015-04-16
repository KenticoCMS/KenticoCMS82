using System;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;
using System.Text;

using CMS.DataEngine;
using CMS.Ecommerce;
using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.Globalization;


public partial class CMSWebParts_Ecommerce_Checkout_Forms_CustomerAddress : CMSCheckoutWebPart
{
    #region "Constants"

    private const string BILLING = "billingaddress";
    private const string SHIPPING = "shippingaddress";
    private const string COMPANY = "companyaddress";

    #endregion


    #region "Private fields"

    private AddressInfo lastUsedAddress;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Address type (BILLING is default)
    /// </summary>
    public string AddressType
    {
        get
        {
            return ValidationHelper.GetString(GetValue("AddressType"), BILLING);
        }
        set
        {
            SetValue("AddressType", value);
        }
    }


    /// <summary>
    /// The caption for the display checkbox displaying or hiding the web part..
    /// </summary>
    public string CheckboxCaption
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CheckboxCaption"), "");
        }
        set
        {
            SetValue("CheckboxCaption", value);
            chkShowAddress.Text = value;
        }
    }


    /// <summary>
    /// Alternative form name for this address web part.
    /// </summary>
    public string AddressForm
    {
        get
        {
            return ValidationHelper.GetString(GetValue("AddressForm"), "");
        }
        set
        {
            SetValue("AddressForm", value);
        }
    }


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

    #endregion


    #region "Private properties"

    private IAddress CurrentCartAddress
    {
        get
        {
            switch (AddressType)
            {
                case BILLING:
                    return ShoppingCart.ShoppingCartBillingAddress;
                case SHIPPING:
                    return ShoppingCart.ShoppingCartShippingAddress;
                case COMPANY:
                    return ShoppingCart.ShoppingCartCompanyAddress;
                default:
                    return null;
            }
        }
        set
        {
            switch (AddressType)
            {
                case BILLING:
                    ShoppingCart.ShoppingCartBillingAddress = value;
                    break;
                case SHIPPING:
                    ShoppingCart.ShoppingCartShippingAddress = value;
                    break;
                case COMPANY:
                    ShoppingCart.ShoppingCartCompanyAddress = value;
                    break;
            }
        }
    }


    private AddressInfo LastUsedAddress
    {
        get
        {
            return lastUsedAddress ?? (lastUsedAddress = GetLastUsedAddress());
        }
    }

    #endregion


    #region "Page methods"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        string[] splitFormName = AddressForm.Split('.');
        // UIForm cant process full path of alternative form if object type is already specified.
        addressForm.AlternativeFormName = splitFormName.LastOrDefault();
        addressForm.OnBeforeSave += addressForm_OnBeforeSave;
        addressForm.OnAfterDataLoad += addressForm_OnAfterDataLoad;
        // Hide default submit button
        addressForm.SubmitButton.Visible = false;

        InitializeAddress();
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        // For normal page load (not postback) set new address to the UIForm
        if ((!RequestHelper.IsPostBack()) && (CurrentCartAddress != null) && (CurrentCartAddress.AddressID == 0))
        {
            addressForm.EditedObject = CurrentCartAddress as AddressInfo;
            addressForm.ReloadData();
        }

        // Hide drop down for empty selection
        if (drpAddresses.Items.Count == 0)
        {
            pnlAddressSelector.Visible = false;
        }
    }

    #endregion


    #region "Events"

    void addressForm_OnAfterDataLoad(object sender, EventArgs e)
    {
        CustomerInfo customer = ShoppingCart.Customer;
        AddressInfo address = addressForm.EditedObject as AddressInfo;
        // Preset personal name filed for new address and logged in user
        if ((customer != null) && (address != null) && (address.AddressID == 0) && string.IsNullOrEmpty(address.AddressPersonalName))
        {
            address.AddressPersonalName = String.Format("{0} {1}", customer.CustomerFirstName, customer.CustomerLastName);
        }

        if (PropagateChangesOnPostback)
        {
            // Propagate changes on postback if there is address info in UIForm
            if (((CurrentCartAddress != null) || (address != null))
                && RequestHelper.IsPostBack())
            {
                // Set first time user current address for postback tax recalculation
                if (CurrentCartAddress == null)
                {
                    CurrentCartAddress = address;
                }

                // Get selected state from country selector in UIForm
                var countrySelectorControl = addressForm.FieldControls["AddressCountryID"];
                var addressStateID = countrySelectorControl != null ? countrySelectorControl.GetOtherValue("AddressStateID") : null;
                // Get selected country from UIForm
                var addressCountryId = addressForm.GetFieldValue("AddressCountryID");

                CurrentCartAddress.AddressCountryID = ValidationHelper.GetInteger(addressCountryId, CurrentCartAddress.AddressCountryID);
                CurrentCartAddress.AddressStateID = ValidationHelper.GetInteger(addressStateID, CurrentCartAddress.AddressStateID);

                ShoppingCart.InvalidateCalculations();
                ComponentEvents.RequestEvents.RaiseEvent(null, e, SHOPPING_CART_CHANGED);
            }
        }
    }


    protected void addressForm_OnBeforeSave(object sender, EventArgs e)
    {
        // Cancel saving, just set current filed values into EditableObject through UIForm.SaveData method
        addressForm.StopProcessing = true;
    }


    protected void drpAddresses_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Get selected address
        int selectedAddressId = ValidationHelper.GetInteger(drpAddresses.SelectedValue, 0);
        AddressInfo address = AddressInfoProvider.GetAddressInfo(selectedAddressId);

        // Set address as current and propagate into UIForm
        if (address != null)
        {
            CurrentCartAddress = address;
        }

        SetupSelectedAddress();

        addressForm.ReloadData();
    }

    #endregion


    #region "Data methods"

    protected override void LoadStep(object sender, StepEventArgs e)
    {
        base.LoadStep(sender, e);

        // Hide/show display form checkbox
        EnsureOptionalCheckBox();

        // Clear shipping address if show check box is not checked and recalculate cart price
        if (PropagateChangesOnPostback && (AddressType == SHIPPING) && (!chkShowAddress.Checked))
        {
            CurrentCartAddress = null;
            ShoppingCart.InvalidateCalculations();
            drpAddresses.SelectedValue = "0";
        }

        if (!StopProcessing)
        {
            SetupSelectedAddress();
            // Set default error label
            addressForm.ErrorLabel = lblError;

            drpAddresses.SelectedIndexChanged += drpAddresses_SelectedIndexChanged;
        }
    }


    /// <summary>
    /// Validates data
    /// </summary>
    protected override void ValidateStepData(object sender, StepEventArgs e)
    {
        base.ValidateStepData(sender, e);

        if (!StopProcessing)
        {
            if (!addressForm.ValidateData())
            {
                e.CancelEvent = true;
                return;
            }
            // Just set current filed values into EditableObject, saving was canceled in OnBeforeSave
            addressForm.SaveData(null, false);

            AddressInfo ai = addressForm.EditedObject as AddressInfo;
            if (ai != null)
            {
                // Validate state
                if (!DataHelper.DataSourceIsEmpty(StateInfoProvider.GetCountryStates(ai.AddressCountryID)))
                {
                    if (ai.AddressStateID < 1)
                    {
                        e.CancelEvent = true;
                        addressForm.DisplayErrorLabel("AddressCountryID", ResHelper.GetString("com.address.nostate"));
                    }
                }
            }
        }
    }


    /// <summary>
    /// Saves data
    /// </summary>
    protected override void SaveStepData(object sender, StepEventArgs e)
    {
        base.SaveStepData(sender, e);

        if (!StopProcessing)
        {
            // Set current address
            AddressInfo ai = addressForm.EditedObject as AddressInfo;
            if (ai != null)
            {
                CurrentCartAddress = ai;

                if (string.IsNullOrEmpty(ai.AddressPersonalName) && (ShoppingCart.Customer != null))
                {
                    ai.AddressPersonalName = TextHelper.LimitLength(string.Format("{0} {1}", ShoppingCart.Customer.CustomerFirstName, ShoppingCart.Customer.CustomerLastName), 200);
                }
            }
        }

        // Clear shipping address if shipping checkbox is unchecked
        if ((AddressType == SHIPPING) && (!chkShowAddress.Checked))
        {
            ShoppingCart.ShoppingCartShippingAddress = null;
        }

        // Set shopping cart with current address change
        ShoppingCartInfoProvider.SetShoppingCartInfo(ShoppingCart);
    }

    #endregion


    #region "Helper methods"

    private void EnsureOptionalCheckBox()
    {
        // Show optional checkbox
        if (AddressType == SHIPPING)
        {
            chkShowAddress.Visible = true;
            chkShowAddress.Text = CheckboxCaption;

            pnlUiContext.Visible = true;
            drpAddresses.Visible = true;
            lblAddress.Visible = true;

            // Check show address checkbox in non-postback load (postback has view state) if there is selected shipping
            if ((!RequestHelper.IsPostBack()) && (ShoppingCart.ShoppingCartShippingAddress != null))
            {
                chkShowAddress.Checked = true;
            }

            // Hide web part content if show checkbox is not checked and controls are not hidden
            if (!chkShowAddress.Checked)
            {
                StopProcessing = true;
                pnlUiContext.Visible = false;
                drpAddresses.Visible = false;
                lblAddress.Visible = false;
            }
        }
    }


    /// <summary>
    /// Initialize customer's addresses in billing and shipping drop-down lists.
    /// </summary>
    private void InitializeAddress()
    {
        if (Customer == null)
        {
            drpAddresses.Visible = false;
            lblAddress.Visible = false;
            return;
        }

        if (drpAddresses.Items.Count == 0)
        {
            // Specifies addresses to display for the given customer, select all the given address types and addresses associated with the given shopping cart
            DataSet ds = AddressInfoProvider.GetAddresses(Customer.CustomerID, (AddressType == BILLING), (AddressType == SHIPPING), (AddressType == COMPANY), true);
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                drpAddresses.DataSource = ds;
                drpAddresses.DataBind();

                // List item for the new item <(new), 0> item in the drop-down menu
                ListItem li = new ListItem(GetString("ShoppingCartOrderAddresses.NewAddress"), "0");
                drpAddresses.Items.Insert(0, li);
            }
        }
    }


    private void SetupSelectedAddress()
    {
        AddressInfo address = CurrentCartAddress as AddressInfo;

        // Set DDL value only for classic load, CurrentCartAddress in postback is not ready yet
        if (!RequestHelper.IsPostBack())
        {
            int addressId = 0;
            // In case of PropagateChangesOnPostback don't set address id, because in this mode null address is replaced with empty one in addressForm_OnAfterDataLoad
            if ((address != null) && !(PropagateChangesOnPostback && (address.AddressID == 0)))
            {
                addressId = address.AddressID;
            }
            else
            {
                // Get last modified customer address
                address = LastUsedAddress;
                addressId = address != null ? address.AddressID : 0;
            }

            try
            {
                drpAddresses.SelectedValue = Convert.ToString(addressId);
            }
            // Eat exception in case of setting non-existing address in selector            
            catch
            { }
        }

        // Do not init UIForm for new address, or there will be still old address after selecting "new" from address DDL selector
        if ((address != null) && (drpAddresses.SelectedValue != "0"))
        {
            addressForm.EditedObject = address;
            CurrentCartAddress = address;
        }

        // Do not reload data in postback, save action is postback, and reload will erase fields with changed data before saving into CurrentCartAddress
        if (!RequestHelper.IsPostBack())
        {
            addressForm.ReloadData();
        }
    }


    private AddressInfo GetLastUsedAddress()
    {
        ObjectQuery<AddressInfo> lastAddress = null;

        switch (AddressType)
        {
            case BILLING:
                // Get last modified billing addresses
                lastAddress = AddressInfoProvider.GetAddresses(ShoppingCart.ShoppingCartCustomerID, true, false, false, true).TopN(1).OrderByDescending("AddressLastModified");
                break;

            case SHIPPING:
                lastAddress = AddressInfoProvider.GetAddresses(ShoppingCart.ShoppingCartCustomerID, false, true, false, true).TopN(1).OrderByDescending("AddressLastModified");
                break;

            case COMPANY:
                lastAddress = AddressInfoProvider.GetAddresses(ShoppingCart.ShoppingCartCustomerID, false, false, true, true).TopN(1).OrderByDescending("AddressLastModified");
                break;
        }

        // Load last used address if there is one
        if (!DataHelper.DataSourceIsEmpty(lastAddress))
        {
            return lastAddress.FirstObject;
        }

        return null;
    }

    #endregion
}