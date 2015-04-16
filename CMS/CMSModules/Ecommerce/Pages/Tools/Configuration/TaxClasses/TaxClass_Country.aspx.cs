using System;
using System.Collections;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Core;
using CMS.Ecommerce;
using CMS.ExtendedControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.Helpers;
using CMS.Base;

public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_TaxClasses_TaxClass_Country : CMSTaxClassesPage
{
    #region "Variables"

    private int taxClassId;
    private string currencyCode = "";
    private readonly Hashtable changedTextBoxes = new Hashtable();
    private readonly Hashtable changedCheckBoxes = new Hashtable();

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        ScriptHelper.RegisterJQuery(this);

        // Check UI element
        CheckUIElementAccessHierarchical(ModuleName.ECOMMERCE, IsMultiStoreConfiguration ? "Tools.Ecommerce.CountriesTaxClasses" : "Configuration.TaxClasses.Countries");
        
        taxClassId = QueryHelper.GetInteger("objectid", 0);

        TaxClassInfo taxClassObj = TaxClassInfoProvider.GetTaxClassInfo(taxClassId);
        EditedObject = taxClassObj;
        // Check if configured tax class belongs to configured site
        if (taxClassObj != null)
        {
            CheckEditedObjectSiteID(taxClassObj.TaxClassSiteID);

            currencyCode = CurrencyInfoProvider.GetMainCurrencyCode(taxClassObj.TaxClassSiteID);

            // Check presence of main currency
            CheckMainCurrency(taxClassObj.TaxClassSiteID);
        }

        GridViewCountries.Columns[0].HeaderText = GetString("TaxClass_Country.Country");
        GridViewCountries.Columns[1].HeaderText = GetString("TaxClass_Country.Value");
        GridViewCountries.Columns[2].HeaderText = GetString("TaxClass_Country.IsFlat");

        DataSet ds = TaxClassCountryInfoProvider.GetCountriesAndTaxValues(taxClassId);
        GridViewCountries.Columns[3].Visible = true;
        GridViewCountries.DataSource = ds.Tables[0];
        GridViewCountries.DataBind();
        // After id is copied, the 4th column it's not needed anymore
        GridViewCountries.Columns[3].Visible = false;

        // Init scripts
        string currencySwitchScript = string.Format(@"
function switchCurrency(isFlatValue, labelId){{
  if(isFlatValue)
  {{
    $cmsj('#' + labelId).html('{0}');
  }}else{{
    $cmsj('#' + labelId).html('%');
  }}
}}", ScriptHelper.GetString(HTMLHelper.HTMLEncode(currencyCode), false));

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "CurrencySwitch", currencySwitchScript, true);
        ScriptHelper.RegisterStartupScript(this, typeof(string), "InitializeGrid", "$cmsj('input[id*=\"chkIsFlatValue\"]').change();", true);

        // Set header actions
        CurrentMaster.HeaderActions.AddAction(new SaveAction(this));
        CurrentMaster.HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;
    }

    #endregion


    #region Methods

    protected void GridViewCountries_DataBound(object sender, EventArgs e)
    {
        foreach (GridViewRow row in GridViewCountries.Rows)
        {
            // Copy id from 5th column to invisible label in last column
            Label id = new Label();
            id.Visible = false;
            id.Text = row.Cells[3].Text;
            row.Cells[3].Controls.Add(id);

            TextBox txtValue = ControlsHelper.GetChildControl(row, typeof(TextBox), "txtTaxValue") as TextBox;
            if (txtValue != null)
            {
                txtValue.ID = "txtTaxValue" + id.Text;
            }

            CMSCheckBox chkIsFlat = ControlsHelper.GetChildControl(row, typeof(CMSCheckBox), "chkIsFlatValue") as CMSCheckBox;
            if (chkIsFlat != null)
            {
                chkIsFlat.ID = "chkIsFlatValue" + id.Text;

                // Bind script for changing absolute/relative marks
                Label lblCurrency = ControlsHelper.GetChildControl(row, typeof(Label), "lblCurrency") as Label;
                if (lblCurrency != null)
                {
                    chkIsFlat.InputAttributes["onclick"] = "switchCurrency(this.checked, '" + lblCurrency.ClientID + "')";
                    chkIsFlat.InputAttributes["onchange"] = "switchCurrency(this.checked, '" + lblCurrency.ClientID + "')";
                }
            }
        }
    }


    protected void txtTaxValue_Changed(object sender, EventArgs e)
    {
        changedTextBoxes[((TextBox)sender).ID] = sender;
    }


    protected void chkIsFlatValue_Changed(object sender, EventArgs e)
    {
        changedCheckBoxes[((CMSCheckBox)sender).ID] = sender;
    }


    private void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName.ToLowerCSafe())
        {
            case "save":
                Save();
                break;
        }
    }


    /// <summary>
    /// Saves the taxes values.
    /// </summary>
    private void Save()
    {
        // Check permissions
        CheckConfigurationModification();

        string errorMessage = String.Empty;
        bool saved = false;

        // Loop through countries
        foreach (GridViewRow row in GridViewCountries.Rows)
        {
            Label lblCountryId = (Label)row.Cells[3].Controls[0];
            int countryId = ValidationHelper.GetInteger(lblCountryId.Text, 0);

            if (countryId > 0)
            {
                string countryName = ((Label)row.Cells[0].Controls[1]).Text;
                TextBox txtValue = (TextBox)row.Cells[1].Controls[1];
                CMSCheckBox chkIsFlat = (CMSCheckBox)row.Cells[2].Controls[1];

                if ((changedTextBoxes[txtValue.ID] != null) || (changedCheckBoxes[chkIsFlat.ID]) != null)
                {
                    // Remove country tax information if tax value is empty
                    if (String.IsNullOrEmpty(txtValue.Text))
                    {
                        TaxClassCountryInfoProvider.RemoveCountryTaxValue(countryId, taxClassId);
                        chkIsFlat.Checked = false;
                        saved = true;
                    }
                    // Update country tax information if tax value is not empty
                    else
                    {
                        // Valid percentage or flat tax value
                        if ((!chkIsFlat.Checked && ValidationHelper.IsInRange(0, 100, ValidationHelper.GetDouble(txtValue.Text, -1))) || (chkIsFlat.Checked && ValidationHelper.IsPositiveNumber(txtValue.Text)))
                        {
                            TaxClassCountryInfo countryInfo = TaxClassCountryInfoProvider.GetTaxClassCountryInfo(countryId, taxClassId);
                            countryInfo = countryInfo ?? new TaxClassCountryInfo();

                            countryInfo.CountryID = countryId;
                            countryInfo.TaxClassID = taxClassId;
                            countryInfo.TaxValue = ValidationHelper.GetDouble(txtValue.Text, 0);
                            countryInfo.IsFlatValue = chkIsFlat.Checked;

                            TaxClassCountryInfoProvider.SetTaxClassCountryInfo(countryInfo);
                            saved = true;
                        }
                        // Invalid tax value
                        else
                        {
                            errorMessage += countryName + ", ";
                        }
                    }
                }
            }
        }

        // Error message
        if (!String.IsNullOrEmpty(errorMessage))
        {
            // Remove last comma
            if (errorMessage.EndsWithCSafe(", "))
            {
                errorMessage = errorMessage.Remove(errorMessage.Length - 2, 2);
            }

            // Show error message
            ShowError(String.Format("{0} - {1}", errorMessage, GetString("Com.Error.TaxValue")));
        }

        // Display info message if some records were saved
        if (String.IsNullOrEmpty(errorMessage) || saved)
        {
            // Show message
            ShowChangesSaved();
        }
    }

    #endregion
}