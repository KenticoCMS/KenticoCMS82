using System;
using System.Web.UI.WebControls;

using CMS.Ecommerce;
using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.Base;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;
using CMS.DataEngine;

public partial class CMSModules_Ecommerce_Controls_MyDetails_MyDetails : CMSAdminControl
{
    #region "Properties"

    /// <summary>
    /// Customer info object.
    /// </summary>
    public CustomerInfo Customer
    {
        get
        {
            return mCustomer;
        }
        set
        {
            mCustomer = value;
        }
    }


    /// <summary>
    /// If true, control does not process the data.
    /// </summary>
    public override bool StopProcessing
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["StopProcessing"], false);
        }
        set
        {
            ViewState["StopProcessing"] = value;
            drpPayment.StopProcessing = value;
            drpShipping.StopProcessing = value;
        }
    }


    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMessages;
        }
    }

    /// <summary>
    /// Indicates, if Business radio button option was selected.
    /// </summary>
    private bool IsBusiness
    {
        get
        {
            return (radAccType.SelectedValue == BUSINESS);
        }
    }

    #endregion


    #region "Delegates & Events"

    /// <summary>
    /// Inform on new customer creation.
    /// </summary>
    public delegate void CustomerCreated();

    /// <summary>
    /// Fired when new customer is created.
    /// </summary>
    public event CustomerCreated OnCustomerCrated;

    #endregion


    #region "Variables & Constants"

    private CustomerInfo mCustomer;

    private const string BUSINESS = "Business";
    private const string PERSONAL = "Personal";

    #endregion


    #region "Page events"

    /// <summary>
    /// On Init page event.
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        radAccType.Items.Add(new ListItem(GetString("com.customer.personalaccount"), PERSONAL));
        radAccType.Items.Add(new ListItem(GetString("com.customer.businessaccount"), BUSINESS));
    }

    /// <summary>
    /// Page load.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        // Preselect radio button on first load
        if (!RequestHelper.IsPostBack() && (Customer != null) && Customer.CustomerHasCompanyInfo)
        {
            radAccType.SelectedValue = BUSINESS;
        }

        if (StopProcessing)
        {
            return;
        }

        // Hide if user is not authenticated, setting up form data are not required
        if (!AuthenticationHelper.IsAuthenticated())
        {
            Visible = false;
            return;
        }

        btnOk.Text = GetString("General.OK");
 
        // WAI validation
        lblCustomerPreferredCurrency.AssociatedControlClientID = selectCurrency.InputClientID;
        lblCustomerPreferredShippingOption.AssociatedControlClientID = drpShipping.InputClientID;
        lblCustomerPrefferedPaymentOption.AssociatedControlClientID = drpPayment.InputClientID;

        // Displays/hides company info region
        plcCompanyInfo.Visible = IsBusiness;

        if (ECommerceSettings.ShowTaxRegistrationID(SiteContext.CurrentSite.SiteName))
        {
            lblTaxRegistrationID.Text = GetString("Customers_Edit.lblTaxRegistrationID");
        }
        else
        {
            plhTaxRegistrationID.Visible = false;
        }

        if (ECommerceSettings.ShowOrganizationID(SiteContext.CurrentSite.SiteName))
        {
            lblOrganizationID.Text = GetString("Customers_Edit.lblOrganizationID");
        }
        else
        {
            plhOrganizationID.Visible = false;
        }

        int siteId = SiteContext.CurrentSiteID;

        // Set site IDs
        drpPayment.SiteID = siteId;
        drpShipping.SiteID = siteId;
        selectCurrency.SiteID = siteId;

        if (mCustomer != null)
        {
            if (!RequestHelper.IsPostBack())
            {
                // Fill editing form
                LoadData();

                // Show that the customer was created or updated successfully
                if (QueryHelper.GetString("saved", String.Empty) == "1")
                {
                    ShowChangesSaved();
                }
            }
        }
        else
        {
            if (!RequestHelper.IsPostBack())
            {
                txtCustomerEmail.Text = MembershipContext.AuthenticatedUser.Email;
            }

            // Show message
            ShowInformation(GetString("MyAccount.MyDetails.CreateNewCustomer"));
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Load form data.
    /// </summary>
    public void LoadData()
    {
        // Fill the form
        txtCustomerCompany.Text = mCustomer.CustomerCompany;
        txtCustomerEmail.Text = mCustomer.CustomerEmail;
        txtCustomerFirstName.Text = mCustomer.CustomerFirstName;
        txtCustomerLastName.Text = mCustomer.CustomerLastName;
        txtCustomerPhone.Text = mCustomer.CustomerPhone;
        txtOraganizationID.Text = mCustomer.CustomerOrganizationID;
        txtTaxRegistrationID.Text = mCustomer.CustomerTaxRegistrationID;

        // Select customer preferences
        string currentSiteName = SiteContext.CurrentSiteName;
        int currencyId = (Customer.CustomerUser != null) ? Customer.CustomerUser.GetUserPreferredCurrencyID(currentSiteName) : 0;
        currencyId = (currencyId > 0) ? currencyId : Customer.CustomerPreferredCurrencyID;
        if (currencyId > 0)
        {
            selectCurrency.SelectedID = currencyId;
        }

        int paymentId = (Customer.CustomerUser != null) ? Customer.CustomerUser.GetUserPreferredPaymentOptionID(currentSiteName) : 0;
        paymentId = (paymentId > 0) ? paymentId : Customer.CustomerPreferredPaymentOptionID;
        if (paymentId > 0)
        {
            drpPayment.SelectedID = paymentId;
        }

        int shippingId = (Customer.CustomerUser != null) ? Customer.CustomerUser.GetUserPreferredShippingOptionID(currentSiteName) : 0;
        shippingId = (shippingId > 0) ? shippingId : Customer.CustomerPreferredShippingOptionID;
        if (shippingId > 0)
        {
            drpShipping.SelectedID = shippingId;
        }

        if (mCustomer.CustomerHasCompanyInfo)
        {
            radAccType.SelectedValue = BUSINESS;
            plcCompanyInfo.Visible = true;
        }
        else
        {
            radAccType.SelectedValue = PERSONAL;
        }
    }


    /// <summary>
    /// Sets data to database.
    /// </summary>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        string errorMessage = "";
        string siteName = SiteContext.CurrentSiteName;

        if ((txtCustomerCompany.Text.Trim() == "" || !IsBusiness) &&
            ((txtCustomerFirstName.Text.Trim() == "") || (txtCustomerLastName.Text.Trim() == "")))
        {
            errorMessage = GetString("Customers_Edit.errorInsert");
        }

        // At least company name has to be filled when company account is selected
        if (errorMessage == "" && IsBusiness)
        {
            errorMessage = new Validator().NotEmpty(txtCustomerCompany.Text, GetString("customers_edit.errorCompany")).Result;
        }

        // Check the following items if complete company info is required for company account
        if (errorMessage == "" && ECommerceSettings.RequireCompanyInfo(siteName) && IsBusiness)
        {
            errorMessage = new Validator().NotEmpty(txtOraganizationID.Text, GetString("customers_edit.errorOrganizationID"))
                .NotEmpty(txtTaxRegistrationID.Text, GetString("customers_edit.errorTaxRegID")).Result;
        }

        if (errorMessage == "")
        {
            errorMessage = new Validator().IsEmail(txtCustomerEmail.Text.Trim(), GetString("customers_edit.erroremailformat"))
                 .MatchesCondition(txtCustomerEmail.Text.Trim(), k => k.Length < 50, GetString("customers_edit.erroremailformat"))
                 .MatchesCondition(txtCustomerPhone.Text.Trim(), k => k.Length < 50, GetString("customers_edit.errorphoneformat")).Result;
        }

        plcCompanyInfo.Visible = IsBusiness;

        if (errorMessage == "")
        {
            // If customer doesn't already exist, create new one
            if (mCustomer == null)
            {
                mCustomer = new CustomerInfo();
                mCustomer.CustomerEnabled = true;
                mCustomer.CustomerUserID = MembershipContext.AuthenticatedUser.UserID;
            }

            int currencyId = selectCurrency.SelectedID;

            if (ECommerceContext.CurrentShoppingCart != null)
            {
                ECommerceContext.CurrentShoppingCart.ShoppingCartCurrencyID = currencyId;
            }

            mCustomer.CustomerEmail = txtCustomerEmail.Text.Trim();
            mCustomer.CustomerLastName = txtCustomerLastName.Text.Trim();
            mCustomer.CustomerPhone = txtCustomerPhone.Text.Trim();
            mCustomer.CustomerFirstName = txtCustomerFirstName.Text.Trim();
            mCustomer.CustomerCreated = DateTime.Now;

            // Set customer's preferences
            mCustomer.CustomerPreferredCurrencyID = (currencyId > 0) ? currencyId : 0;
            mCustomer.CustomerPreferredPaymentOptionID = drpPayment.SelectedID;
            mCustomer.CustomerPreferredShippingOptionID = drpShipping.SelectedID;

            // Check if customer is registered
            if (mCustomer.CustomerIsRegistered)
            {
                // Find user-site binding
                UserSiteInfo userSite = UserSiteInfoProvider.GetUserSiteInfo(Customer.CustomerUserID, SiteContext.CurrentSiteID);
                if (userSite != null)
                {
                    // Set user's preferences
                    userSite.UserPreferredCurrencyID = mCustomer.CustomerPreferredCurrencyID;
                    userSite.UserPreferredPaymentOptionID = mCustomer.CustomerPreferredPaymentOptionID;
                    userSite.UserPreferredShippingOptionID = mCustomer.CustomerPreferredShippingOptionID;

                    UserSiteInfoProvider.SetUserSiteInfo(userSite);
                }
            }

            if (IsBusiness)
            {
                mCustomer.CustomerCompany = txtCustomerCompany.Text.Trim();
                if (ECommerceSettings.ShowOrganizationID(siteName))
                {
                    mCustomer.CustomerOrganizationID = txtOraganizationID.Text.Trim();
                }
                if (ECommerceSettings.ShowTaxRegistrationID(siteName))
                {
                    mCustomer.CustomerTaxRegistrationID = txtTaxRegistrationID.Text.Trim();
                }
            }
            else
            {
                mCustomer.CustomerCompany = "";
                mCustomer.CustomerOrganizationID = "";
                mCustomer.CustomerTaxRegistrationID = "";
            }

            // Update customer data
            CustomerInfoProvider.SetCustomerInfo(mCustomer);

            // Update corresponding user email
            UserInfo user = mCustomer.CustomerUser;
            if (user != null)
            {
                user.Email = mCustomer.CustomerEmail;
                user.UserSettings.SetValue("UserPhone", Customer.CustomerPhone);
                UserInfoProvider.SetUserInfo(user);
            }

            // Update corresponding contact data
            ModuleCommands.OnlineMarketingUpdateContactFromExternalData(mCustomer, DataClassInfoProvider.GetDataClassInfo(CustomerInfo.TYPEINFO.ObjectClassName).ClassContactOverwriteEnabled,
            ModuleCommands.OnlineMarketingGetCurrentContactID());

            // Let others now that customer was created
            if (OnCustomerCrated != null)
            {
                OnCustomerCrated();

                ShowChangesSaved();
            }
            else
            {
                URLHelper.Redirect(URLHelper.AddParameterToUrl(RequestContext.CurrentURL, "saved", "1"));
            }
        }
        else
        {
            //Show error
            ShowError(errorMessage);
        }
    }


    /// <summary>
    /// Raises OnShippingChange event.
    /// </summary>
    protected void drpShipping_ShippingChange(object sender, EventArgs e)
    {
        drpPayment.ShippingOptionID = drpShipping.SelectedID;
        drpPayment.SelectedID = 0;
    }


    /// <summary>
    /// Overridden SetValue - because of MyAccount webpart.
    /// </summary>
    /// <param name="propertyName">Name of the property to set</param>
    /// <param name="value">Value to set</param>
    public override bool SetValue(string propertyName, object value)
    {
        base.SetValue(propertyName, value);

        switch (propertyName.ToLowerCSafe())
        {
            case "customer":
                GeneralizedInfo gi = value as GeneralizedInfo;
                if (gi != null)
                {
                    Customer = gi.MainObject as CustomerInfo;
                }
                break;
        }

        return true;
    }

    #endregion
}