using System;
using System.Collections;
using System.ComponentModel;
using System.Text;

using CMS.DocumentEngine;
using CMS.Ecommerce;
using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.PortalControls;
using CMS.Base;

public partial class CMSWebParts_Ecommerce_Donate : CMSAbstractWebPart
{
    #region "Variables"

    private Guid mDonationGUID = Guid.Empty;
    private SKUInfo mDonationSKU = null;
    private bool mShowInDialog = false;
    private double mDonationAmount = 0.0;
    private bool mShowAmountTextbox = false;
    private bool mShowCurrencyCode = false;
    private bool mShowUnitsTextbox = false;
    private bool mAllowPrivateDonation = false;

    private string mDonationsPagePath = null;

    private string mControlType = "BUTTON";
    private string mControlText = null;
    private string mControlImage = null;
    private string mControlTooltip = null;
    private string mDescription = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// GUID of the selected donation product.
    /// </summary>
    public Guid DonationGUID
    {
        get
        {
            return ValidationHelper.GetGuid(GetValue("DonationGUID"), mDonationGUID);
        }
        set
        {
            SetValue("DonationGUID", value);
            mDonationGUID = value;

            // Invalidate product data
            mDonationSKU = null;
        }
    }


    /// <summary>
    /// Data of the selected donation product.
    /// </summary>
    private SKUInfo DonationSKU
    {
        get
        {
            if (mDonationSKU == null)
            {
                mDonationSKU = SKUInfoProvider.GetSKUInfo(DonationGUID);
            }

            return mDonationSKU;
        }
    }


    /// <summary>
    /// Indicates if donate action opens donate form in dialog window.
    /// </summary>
    public bool ShowInDialog
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowInDialog"), mShowInDialog);
        }
        set
        {
            SetValue("ShowInDialog", value);
            mShowInDialog = value;
        }
    }


    /// <summary>
    /// Overrides donation amount value of selected donation product. It is in site main currency.
    /// </summary>
    public double DonationAmount
    {
        get
        {
            return ValidationHelper.GetDoubleSystem(GetValue("DonationAmount"), mDonationAmount);
        }
        set
        {
            SetValue("DonationAmount", value);
            mDonationAmount = value;
        }
    }


    /// <summary>
    /// Show donation amount textbox
    /// </summary>
    public bool ShowAmountTextbox
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowAmountTextbox"), mShowAmountTextbox);
        }
        set
        {
            SetValue("ShowAmountTextbox", value);
            mShowAmountTextbox = value;
        }
    }


    /// <summary>
    /// Show currency code
    /// </summary>
    public bool ShowCurrencyCode
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowCurrencyCode"), mShowCurrencyCode);
        }
        set
        {
            SetValue("ShowCurrencyCode", value);
            mShowCurrencyCode = value;
        }
    }


    /// <summary>
    /// Indicates if units textbox will be displayed in donate dialog and therefore if it will be possible to change number of units added to the shopping cart.
    /// </summary>
    public bool ShowUnitsTextbox
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowUnitsTextbox"), mShowUnitsTextbox);
        }
        set
        {
            SetValue("ShowUnitsTextbox", value);
            mShowUnitsTextbox = value;
        }
    }


    /// <summary>
    /// Allow private donation
    /// </summary>
    public bool AllowPrivateDonation
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowPrivateDonation"), mAllowPrivateDonation);
        }
        set
        {
            SetValue("AllowPrivateDonation", value);
            mAllowPrivateDonation = value;
        }
    }


    /// <summary>
    /// Path to the page with list of available donations.
    /// </summary>
    public string DonationsPagePath
    {
        get
        {
            return ValidationHelper.GetString(GetValue("DonationsPagePath"), mDonationsPagePath);
        }
        set
        {
            SetValue("DonationsPagePath", value);
            mDonationsPagePath = value;
        }
    }


    /// <summary>
    /// Type of the donate control.
    /// Possible values: 'BUTTON' - button control, 'LINK' - text link control.
    /// </summary>
    public string ControlType
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ControlType"), mControlType);
        }
        set
        {
            SetValue("ControlType", value);
            mControlType = value;
        }
    }


    /// <summary>
    /// Text of the donate control.
    /// </summary>
    public string ControlText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ControlText"), mControlText);
        }
        set
        {
            SetValue("ControlText", value);
            mControlText = value;
        }
    }


    /// <summary>
    /// Image of the donate control.
    /// </summary>
    public string ControlImage
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ControlImage"), mControlImage);
        }
        set
        {
            SetValue("ControlImage", value);
            mControlImage = value;
        }
    }


    /// <summary>
    /// Tooltip text of the donate control.
    /// </summary>
    public string ControlTooltip
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ControlTooltip"), mControlTooltip);
        }
        set
        {
            SetValue("ControlTooltip", value);
            mControlTooltip = value;
        }
    }


    /// <summary>
    /// Text that is be displayed along with donate control.
    /// </summary>
    public string Description
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Description"), mDescription);
        }
        set
        {
            SetValue("Description", value);
            mDescription = value;
        }
    }

    #endregion


    #region "Protected properties"

    /// <summary>
    /// Gets dialog identifier.
    /// </summary>
    protected string DialogIdentifier
    {
        get
        {
            if (String.IsNullOrEmpty(hdnDialogIdentifier.Value))
            {
                if (String.IsNullOrEmpty(Request.Form[hdnDialogIdentifier.UniqueID]))
                {
                    hdnDialogIdentifier.Value = Guid.NewGuid().ToString();
                }
            }

            return HTMLHelper.HTMLEncode(hdnDialogIdentifier.Value);
        }
    }


    /// <summary>
    /// Indicates if selected donation has fixed parameters.
    /// </summary>
    protected bool DonationIsFixed
    {
        get
        {
            if (DonationSKU != null)
            {
                return (!DonationSKU.SKUPrivateDonation && (DonationSKU.SKUMinPrice == DonationSKU.SKUPrice) && (DonationSKU.SKUMaxPrice == DonationSKU.SKUPrice));
            }

            return false;
        }
    }

    #endregion


    #region "Page methods"

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        SetupControl();
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        lblError.Visible = !String.IsNullOrEmpty(lblError.Text);
        lblDescription.Visible = !String.IsNullOrEmpty(lblDescription.Text);

        plcDonationProperties.Visible = donationProperties.Visible;
        plcFieldLabel.Visible = donationProperties.HasEditableFieldsVisible;

        // Register dialog script
        ScriptHelper.RegisterDialogScript(Page);

        // Register script to open donate dialog
        StringBuilder openDonateDialogScript = new StringBuilder();

        openDonateDialogScript.AppendLine("function openDonateDialog(url) {");
        openDonateDialogScript.AppendLine("    modalDialog(url, 'Donate', 710, 380);");
        openDonateDialogScript.AppendLine("};");

        ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "OpenDonateDialog", ScriptHelper.GetScript(openDonateDialogScript.ToString()));

        // Register script to set donation parameter
        StringBuilder setDonationParameterScript = new StringBuilder();

        setDonationParameterScript.AppendLine("function setDonationParameter(elementId, value) {");
        setDonationParameterScript.AppendLine("    var element = document.getElementById(elementId);");
        setDonationParameterScript.AppendLine("    if (element != null) {");
        setDonationParameterScript.AppendLine("        element.value = value;");
        setDonationParameterScript.AppendLine("    };");
        setDonationParameterScript.AppendLine("};");

        ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "SetDonationParameter", ScriptHelper.GetScript(setDonationParameterScript.ToString()));
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            shoppingCartItemSelector.StopProcessing = true;
            return;
        }

        // Set description
        lblDescription.Text = HTMLHelper.HTMLEncode(Description);

        // Initialize donation properties
        donationProperties.SKU = DonationSKU;
        donationProperties.Visible = (!ShowInDialog && (DonationSKU != null));
        donationProperties.ShowDonationAmount = ShowAmountTextbox;
        donationProperties.ShowCurrencyCode = ShowCurrencyCode;
        donationProperties.ShowDonationUnits = ShowUnitsTextbox;
        donationProperties.ShowDonationIsPrivate = AllowPrivateDonation;

        if ((DonationAmount > 0) && !donationProperties.DonationAmountInitialized && !URLHelper.IsPostback())
        {
            // Get amount in cart currency
            double amount = ECommerceContext.CurrentShoppingCart.ApplyExchangeRate(DonationAmount);

            donationProperties.DonationAmount = amount;
        }

        // Initialize shopping cart item selector control
        if (DonationSKU != null)
        {
            shoppingCartItemSelector.SKUID = DonationSKU.SKUID;
        }

        if (!String.IsNullOrEmpty(ControlImage))
        {
            shoppingCartItemSelector.AddToCartImageButton = ControlImage;
        }
        else
        {
            if (ControlType.ToUpperCSafe() == "BUTTON")
            {
                shoppingCartItemSelector.AddToCartText = ControlText;
            }
            else
            {
                shoppingCartItemSelector.AddToCartLinkText = HTMLHelper.HTMLEncode(ControlText);
            }
        }

        shoppingCartItemSelector.AddToCartTooltip = ControlTooltip;
        shoppingCartItemSelector.SKUEnabled = true;
        shoppingCartItemSelector.OnAddToShoppingCart += new CancelEventHandler(shoppingCartItemSelector_OnAddToShoppingCart);
    }


    /// <summary>
    /// Reloads the control data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();

        SetupControl();
    }


    protected void shoppingCartItemSelector_OnAddToShoppingCart(object sender, CancelEventArgs e)
    {
        // If donations page path specified
        if (!String.IsNullOrEmpty(DonationsPagePath))
        {
            // Redirect to donations page
            URLHelper.Redirect(DocumentURLProvider.GetUrl(DonationsPagePath));

            // Cancel further processing
            e.Cancel = true;
            return;
        }

        // If donation not selected
        if (DonationGUID == Guid.Empty)
        {
            // Show alert
            ScriptHelper.Alert(Page, GetString("com.donate.donationnotspecified"));

            // Cancel further processing
            e.Cancel = true;
            return;
        }

        // If donate form should be opened in dialog and donation parameters are not fixed
        if (ShowInDialog && !DonationIsFixed)
        {
            // Get donation parameters from hidden fields
            double donationAmount = ValidationHelper.GetDoubleSystem(hdnDonationAmount.Value, 0.0);
            bool donationIsPrivate = ValidationHelper.GetBoolean(hdnDonationIsPrivate.Value, false);
            int donationUnits = ValidationHelper.GetInteger(hdnDonationUnits.Value, 1);

            // If donation parameters set
            if (donationAmount > 0.0)
            {
                // Set donation properties for item to be added
                shoppingCartItemSelector.SetDonationProperties(donationAmount, donationIsPrivate, donationUnits);

                // Clear hidden fields
                hdnDonationAmount.Value = "";
                hdnDonationIsPrivate.Value = "";
                hdnDonationUnits.Value = "";
            }
            else
            {
                // Set dialog parameters
                Hashtable dialogParameters = new Hashtable();

                dialogParameters["DonationGUID"] = DonationGUID.ToString();
                dialogParameters["DonationAmount"] = DonationAmount;

                dialogParameters["DonationAmountElementID"] = hdnDonationAmount.ClientID;
                dialogParameters["DonationIsPrivateElementID"] = hdnDonationIsPrivate.ClientID;
                dialogParameters["DonationUnitsElementID"] = hdnDonationUnits.ClientID;

                dialogParameters["ShowDonationAmount"] = ShowAmountTextbox.ToString();
                dialogParameters["ShowCurrencyCode"] = ShowCurrencyCode.ToString();
                dialogParameters["ShowDonationUnits"] = ShowUnitsTextbox.ToString();
                dialogParameters["ShowDonationIsPrivate"] = AllowPrivateDonation.ToString();

                dialogParameters["PostBackEventReference"] = ControlsHelper.GetPostBackEventReference(shoppingCartItemSelector.AddToCartControl, null);

                WindowHelper.Add(DialogIdentifier, dialogParameters);

                // Register startup script that opens donate dialog
                string url = URLHelper.ResolveUrl("~/CMSModules/Ecommerce/CMSPages/Donate.aspx");
                url = URLHelper.AddParameterToUrl(url, "params", DialogIdentifier);

                string startupScript = String.Format("openDonateDialog('{0}')", url);

                ScriptHelper.RegisterStartupScript(Page, typeof(string), "StartupDialogOpen", ScriptHelper.GetScript(startupScript));

                // Cancel further processing
                e.Cancel = true;
            }

            return;
        }

        // If donation properties form is valid
        if (String.IsNullOrEmpty(donationProperties.Validate()))
        {
            // Set donation properties for item to be added
            shoppingCartItemSelector.SetDonationProperties(donationProperties.DonationAmount, donationProperties.DonationIsPrivate, donationProperties.DonationUnits);
        }
        else
        {
            // Display error message on page
            lblError.Text = donationProperties.ErrorMessage;

            // Cancel further processing
            e.Cancel = true;
        }
    }

    #endregion
}