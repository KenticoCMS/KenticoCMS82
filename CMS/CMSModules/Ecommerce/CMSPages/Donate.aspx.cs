using System;
using System.Text;
using System.Collections;

using CMS.Ecommerce;
using CMS.Helpers;
using CMS.UIControls;

[Title("com.donatedialog.title")]
public partial class CMSModules_Ecommerce_CMSPages_Donate : CMSLiveModalPage
{
    #region "Variables"

    private SKUInfo mDonationSKU;

    private string dialogIdentifier;
    private Hashtable dialogParameters;

    private Guid donationGuid = Guid.Empty;
    private double donationAmount;

    private string amountElementId;
    private string isPrivateElementId;
    private string unitsElementId;

    private string postBackEventReference;

    #endregion


    #region "Properties - protected"

    /// <summary>
    /// Donation SKU data.
    /// </summary>
    protected SKUInfo DonationSKU
    {
        get
        {
            return mDonationSKU ?? (mDonationSKU = SKUInfoProvider.GetSKUInfo(donationGuid));
        }
    }

    /// <summary>
    /// Indicates if donation has fixed donation amount.
    /// </summary>
    protected bool DonationHasFixedAmount
    {
        get
        {
            if (DonationSKU != null)
            {
                return ((DonationSKU.SKUMinPrice == DonationSKU.SKUPrice) && (DonationSKU.SKUMaxPrice == DonationSKU.SKUPrice));
            }

            return false;
        }
    }


    /// <summary>
    /// Error message.
    /// </summary>
    protected string ErrorMessage
    {
        get;
        set;
    }

    #endregion


    #region "Page methods"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Get dialog identifier from URL
        dialogIdentifier = QueryHelper.GetString("params", null);

        // Get dialog parameters
        dialogParameters = (Hashtable)WindowHelper.GetItem(dialogIdentifier);

        if (dialogParameters != null)
        {
            donationGuid = ValidationHelper.GetGuid(dialogParameters["DonationGUID"], Guid.Empty);
            donationAmount = ValidationHelper.GetDouble(dialogParameters["DonationAmount"], 0.0);

            amountElementId = ValidationHelper.GetString(dialogParameters["DonationAmountElementID"], null);
            isPrivateElementId = ValidationHelper.GetString(dialogParameters["DonationIsPrivateElementID"], null);
            unitsElementId = ValidationHelper.GetString(dialogParameters["DonationUnitsElementID"], null);

            donationPropertiesElem.ShowDonationAmount = ValidationHelper.GetBoolean(dialogParameters["ShowDonationAmount"], false);
            donationPropertiesElem.ShowCurrencyCode = ValidationHelper.GetBoolean(dialogParameters["ShowCurrencyCode"], false);
            donationPropertiesElem.ShowDonationUnits = ValidationHelper.GetBoolean(dialogParameters["ShowDonationUnits"], false);
            donationPropertiesElem.ShowDonationIsPrivate = ValidationHelper.GetBoolean(dialogParameters["ShowDonationIsPrivate"], false);

            postBackEventReference = ValidationHelper.GetString(dialogParameters["PostBackEventReference"], null);
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (DonationSKU == null)
        {
            // Redirect if DonationSKU object is not set
            URLHelper.Redirect(UIHelper.GetAccessDeniedUrl(ResHelper.GetString("dialogs.badhashtext")));
        }
        else
        {
            // Set localized SKU name
            lblSKUName.Text = GetString(DonationSKU.SKUName);

            donationPropertiesElem.SKU = DonationSKU;

            if (!donationPropertiesElem.DonationAmountInitialized && (donationAmount > 0))
            {
                // Convert from main to cart currency               
                donationPropertiesElem.DonationAmount = ECommerceContext.CurrentShoppingCart.ApplyExchangeRate(donationAmount);
            }
        }

        // Initialize buttons
        btnDonate.Click += btnDonate_Click;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Display description
        lblDescription.Visible = donationPropertiesElem.HasEditableFieldsVisible;

        // If donation amount field is not displayed
        if (!donationPropertiesElem.ShowDonationAmount)
        {
            // Get default amount in site currency
            double amount = donationAmount;
            if (amount <= 0)
            {
                // Get amount in site or global currency
                amount = DonationSKU.SKUPrice;
            }

            amount = ECommerceContext.CurrentShoppingCart.ApplyExchangeRate(amount, DonationSKU.IsGlobal);
            string formattedAmount = ECommerceContext.CurrentShoppingCart.GetFormattedPrice(amount, true);

            lblAmount.Text = String.Format(GetString("com.donatedialog.amount"), formattedAmount);
        }

        lblAmount.Visible = !String.IsNullOrEmpty(lblAmount.Text);

        // If donation has fixed donation amount and no donation properties fields are visible
        if (DonationHasFixedAmount || !donationPropertiesElem.HasEditableFieldsVisible)
        {
            plcMinMaxLabels.Visible = false;
        }
        else
        {
            // Initialize minimum and maximum donation amount labels
            if (DonationSKU.SKUMinPrice > 0.0)
            {
                double amount = ECommerceContext.CurrentShoppingCart.ApplyExchangeRate(DonationSKU.SKUMinPrice, DonationSKU.IsGlobal);
                string formattedAmount = ECommerceContext.CurrentShoppingCart.GetFormattedPrice(amount, true);
                lblMinimumAmount.Text = String.Format(GetString("com.donatedialog.minimumamount"), formattedAmount);
            }

            if (DonationSKU.SKUMaxPrice > 0.0)
            {
                double amount = ECommerceContext.CurrentShoppingCart.ApplyExchangeRate(DonationSKU.SKUMaxPrice, DonationSKU.IsGlobal);
                string formattedAmount = ECommerceContext.CurrentShoppingCart.GetFormattedPrice(amount, true);
                lblMaximumAmount.Text = String.Format(GetString("com.donatedialog.maximumamount"), formattedAmount);
            }
        }

        lblMinimumAmount.Visible = !String.IsNullOrEmpty(lblMinimumAmount.Text);
        lblMaximumAmount.Visible = !String.IsNullOrEmpty(lblMaximumAmount.Text);

        // Display error message
        lblError.Text = ErrorMessage;
        lblError.Visible = !String.IsNullOrEmpty(lblError.Text);
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Validates form and returns true if valid.
    /// </summary>
    public bool ValidateForm()
    {
        ErrorMessage = donationPropertiesElem.Validate();

        return String.IsNullOrEmpty(ErrorMessage);
    }


    private void btnDonate_Click(object sender, EventArgs e)
    {
        // If form is valid
        if (ValidateForm())
        {
            // Build script to add donation to shopping cart
            StringBuilder script = new StringBuilder();

            script.AppendLine(String.Format("wopener.setDonationParameter('{0}', '{1}');", amountElementId, donationPropertiesElem.DonationAmount));
            script.AppendLine(String.Format("wopener.setDonationParameter('{0}', '{1}');", unitsElementId, donationPropertiesElem.DonationUnits));
            script.AppendLine(String.Format("wopener.setDonationParameter('{0}', '{1}');", isPrivateElementId, donationPropertiesElem.DonationIsPrivate));
            script.AppendLine(String.Format("wopener.{0};", postBackEventReference));
            script.AppendLine("CloseDialog();");

            // Register as startup script
            ScriptHelper.RegisterStartupScript(Page, typeof(string), "AddToCart", ScriptHelper.GetScript(script.ToString()));
        }
    }

    #endregion
}