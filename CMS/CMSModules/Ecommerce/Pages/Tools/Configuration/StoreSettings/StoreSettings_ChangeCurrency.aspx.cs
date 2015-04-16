using System;

using CMS.Ecommerce;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;

[Title("StoreSettings_ChangeCurrency.ChangeCurrencyTitle")]
public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_StoreSettings_StoreSettings_ChangeCurrency : CMSEcommerceModalPage
{
    private CurrencyInfo mainCurrency;
    private int editedSiteId = -1;


    protected void Page_Load(object sender, EventArgs e)
    {
        // Register JQuery
        ScriptHelper.RegisterJQuery(this);

        // Register bootstrap tooltip over help icons
        ScriptHelper.RegisterBootstrapTooltip(Page, ".info-icon > i");

        // Check permissions (only global admin can see this dialog)
        var ui = MembershipContext.AuthenticatedUser;

        if ((ui == null) || !ui.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin))
        {
            // Redirect to access denied
            RedirectToAccessDenied(GetString("StoreSettings_ChangeCurrency.AccessDenied"));
        }

        int siteId = QueryHelper.GetInteger("siteId", SiteContext.CurrentSiteID);

        if (ui.IsGlobalAdministrator)
        {
            editedSiteId = (siteId <= 0) ? 0 : siteId;
        }
        else
        {
            editedSiteId = SiteContext.CurrentSiteID;
        }

        mainCurrency = CurrencyInfoProvider.GetMainCurrency(editedSiteId);

        // Load the UI
        CurrentMaster.Page.Title = "Ecommerce - Change main currency";
        chkRecalculateFromGlobal.InputAttributes["onclick"] = "checkRecalculation()";
        chkCredit.InputAttributes["onclick"] = "checkRecalculation()";
        chkDocuments.InputAttributes["onclick"] = "checkRecalculation()";
        chkExchangeRates.InputAttributes["onclick"] = "checkRecalculation()";
        chkDiscounts.InputAttributes["onclick"] = "checkRecalculation()";
        chkFlatTaxes.InputAttributes["onclick"] = "checkRecalculation()";
        chkProductPrices.InputAttributes["onclick"] = "checkRecalculation()";
        chkShipping.InputAttributes["onclick"] = "checkRecalculation()";
        btnOk.Text = GetString("General.saveandclose");
        imgHelp.ToolTip = GetString("StoreSettings_ChangeCurrency.ExchangeRateHelp");
        imgRoundHelp.ToolTip = GetString("StoreSettings_ChangeCurrency.ExchangeRateRoundHelp");

        if (mainCurrency != null)
        {
            // Set confirmation message for OK button
            btnOk.Attributes["onclick"] = "OnButtonClicked()";
            lblOldMainCurrency.Text = HTMLHelper.HTMLEncode(mainCurrency.CurrencyDisplayName);
        }
        else
        {
            plcObjectsSelection.Visible = false;
            plcRecalculationDetails.Visible = false;
            plcOldCurrency.Visible = false;
        }

        currencyElem.AddSelectRecord = true;
        currencyElem.SiteID = editedSiteId;

        plcRecountDocuments.Visible = (editedSiteId != 0);
        plcRecalculateFromGlobal.Visible = (editedSiteId == 0);

        if (!URLHelper.IsPostback())
        {
            if (QueryHelper.GetBoolean("saved", false))
            {
                // Refresh the page
                ltlScript.Text = ScriptHelper.GetScript(@"
var loc = wopener.location;
if(!(/currencysaved=1/.test(loc))) {
    if(!(/siteId/.test(loc))) {
        loc += '?currencysaved=1';
    } else {
        loc += '&currencysaved=1';
    }
}
wopener.location.replace(loc); CloseDialog();");

                ShowChangesSaved();
            }
        }

        // Init script
        string checkRecalculationScript = @"
function checkRecalculation(){
  var recalcNeeded = false;
  var checkBoxes = $cmsj(" + "\"#recalculationDetail input[type='checkbox']\"" + @");

  for(i = 0; i < checkBoxes.length; i++)
  {
    if(checkBoxes[i].checked)
    {
        recalcNeeded = true;
        break;
    }
  }

  if(recalcNeeded)
  {
    $cmsj('#" + txtEchangeRate.ClientID + @"').parents('.form-group').show();
    $cmsj('#" + txtRound.ClientID + @"').parents('.form-group').show();
  }else
  {
    $cmsj('#" + txtEchangeRate.ClientID + @"').parents('.form-group').hide();
    $cmsj('#" + txtRound.ClientID + @"').parents('.form-group').hide();
  }
};";

        string btnSubmitScript = @"
function OnButtonClicked()
{
  var checkedCheckboxies = $cmsj('#recalculationDetail input:checkbox:checked');
  if (checkedCheckboxies.length > 0)
      return confirm(" + ScriptHelper.GetString(GetString("StoreSettings_ChangeCurrency.Confirmation")) + @");
};";

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "Scripts", checkRecalculationScript + btnSubmitScript, true);
        ScriptHelper.RegisterStartupScript(this, typeof(string), "CheckRecalculationStartup", "checkRecalculation();", true);
    }


    /// <summary>
    /// Changes the selected prices and other object fields.
    /// </summary>
    protected void btnOk_Click(object sender, EventArgs e)
    {
        string err = "";

        if ((mainCurrency != null) && RecalculationRequested())
        {
            err = new Validator().NotEmpty(txtEchangeRate.Text.Trim(), GetString("StoreSettings_ChangeCurrency.EmptyExchangeRate"))
                .NotEmpty(txtRound.Text.Trim(), GetString("StoreSettings_ChangeCurrency.InvalidRound"))
                .IsInteger(txtRound.Text.Trim(), GetString("StoreSettings_ChangeCurrency.InvalidRound"))
                .IsDouble(txtEchangeRate.Text.Trim(), GetString("StoreSettings_ChangeCurrency.InvalidExchangeRate")).Result;
        }

        // Check exchange rate value
        double newRate = ValidationHelper.GetDouble(txtEchangeRate.Text.Trim(), 1);
        if (string.IsNullOrEmpty(err) && (newRate <= 0))
        {
            err = GetString("StoreSettings_ChangeCurrency.NegativeExchangeRate");
        }

        // Check new currency ID
        int newCurrencyId = ValidationHelper.GetInteger(currencyElem.Value, 0);
        if (string.IsNullOrEmpty(err) && (newCurrencyId <= 0))
        {
            err = GetString("StoreSettings_ChangeCurrency.NoNewMainCurrency");
        }

        // Show error message if any
        if (err != "")
        {
            ShowError(err);
            return;
        }

        // Get the new main currency
        CurrencyInfo newCurrency = CurrencyInfoProvider.GetCurrencyInfo(newCurrencyId);
        if (newCurrency != null)
        {
            // Only select new main currency when no old main currency specified
            if (mainCurrency == null)
            {
                newCurrency.CurrencyIsMain = true;
                CurrencyInfoProvider.SetCurrencyInfo(newCurrency);

                // Refresh the page
                URLHelper.Redirect(URLHelper.AddParameterToUrl(RequestContext.CurrentURL, "saved", "1"));

                return;
            }

            // Set new main currency and recalculate requested objects
            double rate = 1 / newRate;
            int round = ValidationHelper.GetInteger(txtRound.Text.Trim(), 2);

            try
            {
                RecalculationSettings settings = new RecalculationSettings();
                settings.ExchangeRates = chkExchangeRates.Checked;
                settings.FromGlobalCurrencyRates = (plcRecalculateFromGlobal.Visible && chkRecalculateFromGlobal.Checked);
                settings.Products = chkProductPrices.Checked;
                settings.Taxes = chkFlatTaxes.Checked;
                settings.Discounts = chkDiscounts.Checked;
                settings.CreditEvents = chkCredit.Checked;
                settings.ShippingOptions = chkShipping.Checked;
                settings.Documents = (plcRecountDocuments.Visible && chkDocuments.Checked);
                // Do not recalculate orders after main currency changed
                settings.Orders = false;

                // Recalculates the values
                CurrencyInfoProvider.ChangeMainCurrency(editedSiteId, newCurrencyId, rate, round, settings);

                // Refresh the page
                URLHelper.Redirect(URLHelper.AddParameterToUrl(RequestContext.CurrentURL, "saved", "1"));
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }
        else
        {
            // Show error - No currency selected
            ShowError(GetString("StoreSettings_ChangeCurrency.NoNewMainCurrency"));
        }
    }


    /// <summary>
    /// Returns true when at least one checkox is checked.
    /// </summary>
    private bool RecalculationRequested()
    {
        return chkExchangeRates.Checked || (plcRecalculateFromGlobal.Visible && chkRecalculateFromGlobal.Checked) || chkProductPrices.Checked ||
               chkFlatTaxes.Checked || chkDiscounts.Checked || chkCredit.Checked ||
               chkShipping.Checked || (plcRecountDocuments.Visible && chkDocuments.Checked);
    }
}