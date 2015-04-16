using System;
using System.Data;
using CMS.Helpers;
using CMS.Globalization;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;

public partial class CMSAPIExamples_Code_Development_Countries_Default : CMSAPIExamplePage
{
    #region "Initialization"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Country
        apiCreateCountry.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateCountry);
        apiGetAndUpdateCountry.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndUpdateCountry);
        apiGetAndBulkUpdateCountries.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndBulkUpdateCountries);
        apiDeleteCountry.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteCountry);

        // State
        apiCreateState.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateState);
        apiGetAndUpdateState.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndUpdateState);
        apiGetAndBulkUpdateStates.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndBulkUpdateStates);
        apiDeleteState.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteState);
    }

    #endregion


    #region "Mass actions"

    /// <summary>
    /// Runs all creating and managing examples.
    /// </summary>
    public override void RunAll()
    {
        base.RunAll();

        // Country
        apiCreateCountry.Run();
        apiGetAndUpdateCountry.Run();
        apiGetAndBulkUpdateCountries.Run();

        // State
        apiCreateState.Run();
        apiGetAndUpdateState.Run();
        apiGetAndBulkUpdateStates.Run();
    }


    /// <summary>
    /// Runs all cleanup examples.
    /// </summary>
    public override void CleanUpAll()
    {
        base.CleanUpAll();

        // State
        apiDeleteState.Run();

        // Country
        apiDeleteCountry.Run();
    }

    #endregion


    #region "API examples - Country"

    /// <summary>
    /// Creates country. Called when the "Create country" button is pressed.
    /// </summary>
    private bool CreateCountry()
    {
        // Create new country object
        CountryInfo newCountry = new CountryInfo();

        // Set the properties
        newCountry.CountryDisplayName = "My new country";
        newCountry.CountryName = "MyNewCountry";

        // Create the country
        CountryInfoProvider.SetCountryInfo(newCountry);

        return true;
    }


    /// <summary>
    /// Gets and updates country. Called when the "Get and update country" button is pressed.
    /// Expects the CreateCountry method to be run first.
    /// </summary>
    private bool GetAndUpdateCountry()
    {
        // Get the country
        CountryInfo updateCountry = CountryInfoProvider.GetCountryInfo("MyNewCountry");
        if (updateCountry != null)
        {
            // Update the property
            updateCountry.CountryDisplayName = updateCountry.CountryDisplayName.ToLower();

            // Update the country
            CountryInfoProvider.SetCountryInfo(updateCountry);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates countries. Called when the "Get and bulk update countries" button is pressed.
    /// Expects the CreateCountry method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateCountries()
    {
        // Get the data
        DataSet countries = CountryInfoProvider.GetCountries().WhereStartsWith("CountryName", "MyNewCountry");
        if (!DataHelper.DataSourceIsEmpty(countries))
        {
            // Loop through the individual items
            foreach (DataRow countryDr in countries.Tables[0].Rows)
            {
                // Create object from DataRow
                CountryInfo modifyCountry = new CountryInfo(countryDr);

                // Update the property
                modifyCountry.CountryDisplayName = modifyCountry.CountryDisplayName.ToUpper();

                // Update the country
                CountryInfoProvider.SetCountryInfo(modifyCountry);
            }

            return true;
        }

        return false;
    }


    /// <summary>
    /// Deletes country. Called when the "Delete country" button is pressed.
    /// Expects the CreateCountry method to be run first.
    /// </summary>
    private bool DeleteCountry()
    {
        // Get the country
        CountryInfo deleteCountry = CountryInfoProvider.GetCountryInfo("MyNewCountry");

        // Delete the country
        CountryInfoProvider.DeleteCountryInfo(deleteCountry);

        return (deleteCountry != null);
    }

    #endregion


    #region "API examples - State"

    /// <summary>
    /// Creates state. Called when the "Create state" button is pressed.
    /// </summary>
    private bool CreateState()
    {
        // Get the country
        CountryInfo country = CountryInfoProvider.GetCountryInfo("MyNewCountry");
        if (country != null)
        {
            // Create new state object
            StateInfo newState = new StateInfo();

            // Set the properties
            newState.StateDisplayName = "My new state";
            newState.StateName = "MyNewState";
            newState.CountryID = country.CountryID;

            // Create the state
            StateInfoProvider.SetStateInfo(newState);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and updates state. Called when the "Get and update state" button is pressed.
    /// Expects the CreateState method to be run first.
    /// </summary>
    private bool GetAndUpdateState()
    {
        // Get the state
        StateInfo updateState = StateInfoProvider.GetStateInfo("MyNewState");
        if (updateState != null)
        {
            // Update the property
            updateState.StateDisplayName = updateState.StateDisplayName.ToLower();

            // Update the state
            StateInfoProvider.SetStateInfo(updateState);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates states. Called when the "Get and bulk update states" button is pressed.
    /// Expects the CreateState method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateStates()
    {
        // Get the country
        CountryInfo country = CountryInfoProvider.GetCountryInfo("MyNewCountry");
        if (country != null)
        {
            // Get the data
            DataSet states = StateInfoProvider.GetCountryStates(country.CountryID);
            if (!DataHelper.DataSourceIsEmpty(states))
            {
                // Loop through the individual items
                foreach (DataRow stateDr in states.Tables[0].Rows)
                {
                    // Create object from DataRow
                    StateInfo modifyState = new StateInfo(stateDr);

                    // Update the property
                    modifyState.StateDisplayName = modifyState.StateDisplayName.ToUpper();

                    // Update the state
                    StateInfoProvider.SetStateInfo(modifyState);
                }

                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// Deletes state. Called when the "Delete state" button is pressed.
    /// Expects the CreateState method to be run first.
    /// </summary>
    private bool DeleteState()
    {
        // Get the state
        StateInfo deleteState = StateInfoProvider.GetStateInfo("MyNewState");

        // Delete the state
        StateInfoProvider.DeleteStateInfo(deleteState);

        return (deleteState != null);
    }

    #endregion
}