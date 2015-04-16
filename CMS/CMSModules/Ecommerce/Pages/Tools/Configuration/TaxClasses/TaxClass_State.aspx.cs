using System;
using System.Collections;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Core;
using CMS.Ecommerce;
using CMS.ExtendedControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.Helpers;
using CMS.Globalization;
using CMS.Base;

public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_TaxClasses_TaxClass_State : CMSTaxClassesPage
{
    #region "Variables"

    private int taxClassId;
    private int countryId;
    private string currencyCode = "";
    private readonly Hashtable changedTextBoxes = new Hashtable();
    private readonly Hashtable changedCheckBoxes = new Hashtable();

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        ScriptHelper.RegisterJQuery(this);

        // Check UI element
        CheckUIElementAccessHierarchical(ModuleName.ECOMMERCE, IsMultiStoreConfiguration ? "Tools.Ecommerce.StatesTaxClasses" : "Configuration.TaxClasses.States");

        // Get tax class Id from URL
        taxClassId = QueryHelper.GetInteger("objectid", 0);

        TaxClassInfo taxClass = TaxClassInfoProvider.GetTaxClassInfo(taxClassId);
        EditedObject = taxClass;
        // Check if configured tax class belongs to configured site
        if (taxClass != null)
        {
            // Check site id of tax class
            CheckEditedObjectSiteID(taxClass.TaxClassSiteID);

            currencyCode = CurrencyInfoProvider.GetMainCurrencyCode(taxClass.TaxClassSiteID);

            // Check presence of main currency
            CheckMainCurrency(taxClass.TaxClassSiteID);
        }

        if (!RequestHelper.IsPostBack())
        {
            // Fill the drpCountry with countries which have some states or colonies
            drpCountry.DataSource = CountryInfoProvider.GetCountriesWithStates();
            drpCountry.DataValueField = "CountryID";
            drpCountry.DataTextField = "CountryDisplayName";
            drpCountry.DataBind();
        }
        // Set grid view properties
        gvStates.Columns[0].HeaderText = GetString("taxclass_state.gvstates.state");
        gvStates.Columns[1].HeaderText = GetString("Code");
        gvStates.Columns[2].HeaderText = GetString("taxclass_state.gvstates.value");
        gvStates.Columns[3].HeaderText = GetString("taxclass_state.gvstates.isflat");
        gvStates.Columns[4].HeaderText = GetString("StateId");

        LoadGridViewData();

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
        CurrentMaster.DisplaySiteSelectorPanel = true;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        // Hide code column
        gvStates.Columns[1].Visible = false;
    }

    #endregion


    #region "Methods"

    protected void drpCountry_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadGridViewData();
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


    protected void gvStates_DataBound(object sender, EventArgs e)
    {
        foreach (GridViewRow row in gvStates.Rows)
        {
            // Copy id from 5th column to invisible label in last column
            Label id = new Label();
            id.Visible = false;
            id.Text = row.Cells[4].Text;
            row.Cells[4].Controls.Add(id);

            // Set unique text box ID
            TextBox txtValue = ControlsHelper.GetChildControl(row, typeof(TextBox), "txtTaxValue") as TextBox;
            if (txtValue != null)
            {
                txtValue.ID = "txtTaxValue" + id.Text;
            }

            // Set unique check box ID
            CMSCheckBox chkIsFlat = ControlsHelper.GetChildControl(row, typeof(CMSCheckBox), "chkIsFlatValue") as CMSCheckBox;
            if (chkIsFlat != null)
            {
                chkIsFlat.ID = "chkIsFlatValue" + id.Text;

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


    private void LoadGridViewData()
    {
        gvStates.Columns[4].Visible = true;
        countryId = ValidationHelper.GetInteger(drpCountry.SelectedValue, 0);
        DataSet ds = TaxClassStateInfoProvider.GetStatesAndTaxValues(taxClassId, countryId);
        gvStates.DataSource = ds.Tables[0];
        gvStates.DataBind();
        gvStates.Columns[4].Visible = false;
    }


    /// <summary>
    /// Saves values.
    /// </summary>
    private void Save()
    {
        // Check permissions
        CheckConfigurationModification();

        string errorMessage = String.Empty;
        bool saved = false;

        // Loop through states
        foreach (GridViewRow row in gvStates.Rows)
        {
            Label lblStateId = (Label)row.Cells[4].Controls[0];
            int stateId = ValidationHelper.GetInteger(lblStateId.Text, 0);

            if (stateId > 0)
            {
                string stateName = ((Label)row.Cells[0].Controls[1]).Text;
                TextBox txtValue = (TextBox)row.Cells[2].Controls[1];
                CMSCheckBox chkIsFlat = (CMSCheckBox)row.Cells[3].Controls[1];

                if ((changedTextBoxes[txtValue.ID] != null) || (changedCheckBoxes[chkIsFlat.ID] != null))
                {
                    // Remove state tax information if tax value is empty
                    if (String.IsNullOrEmpty(txtValue.Text))
                    {
                        TaxClassStateInfoProvider.RemoveStateTaxValue(taxClassId, stateId);
                        chkIsFlat.Checked = false;
                        saved = true;
                    }
                    // Update state tax information if tax value is not empty
                    else
                    {
                        // Valid percentage or flat tax value
                        if ((!chkIsFlat.Checked && ValidationHelper.IsInRange(0, 100, ValidationHelper.GetDouble(txtValue.Text, -1))) || (chkIsFlat.Checked && ValidationHelper.IsPositiveNumber(txtValue.Text)))
                        {
                            TaxClassStateInfo stateInfo = TaxClassStateInfoProvider.GetTaxClassStateInfo(taxClassId, stateId);
                            stateInfo = stateInfo ?? new TaxClassStateInfo();

                            stateInfo.StateID = stateId;
                            stateInfo.TaxClassID = taxClassId;
                            stateInfo.TaxValue = ValidationHelper.GetDouble(txtValue.Text, 0);
                            stateInfo.IsFlatValue = chkIsFlat.Checked;

                            TaxClassStateInfoProvider.SetTaxClassStateInfo(stateInfo);
                            saved = true;
                        }
                        // Invalid tax value
                        else
                        {
                            errorMessage += stateName + ", ";
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