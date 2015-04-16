using System;
using System.Data;
using CMS.Helpers;
using CMS.Localization;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSAPIExamples_Code_Development_Cultures_Default : CMSAPIExamplePage
{
    #region "Initialization"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Culture
        apiCreateCulture.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateCulture);
        apiGetAndUpdateCulture.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndUpdateCulture);
        apiGetAndBulkUpdateCultures.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndBulkUpdateCultures);
        apiDeleteCulture.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteCulture);

        // Culture on site
        apiAddCultureToSite.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(AddCultureToSite);
        apiRemoveCultureFromSite.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(RemoveCultureFromSite);
    }

    #endregion


    #region "Mass actions"

    /// <summary>
    /// Runs all creating and managing examples.
    /// </summary>
    public override void RunAll()
    {
        base.RunAll();

        // Culture
        apiCreateCulture.Run();
        apiGetAndUpdateCulture.Run();
        apiGetAndBulkUpdateCultures.Run();

        // Culture on site
        apiAddCultureToSite.Run();
    }


    /// <summary>
    /// Runs all cleanup examples.
    /// </summary>
    public override void CleanUpAll()
    {
        base.CleanUpAll();

        // Culture on site
        apiRemoveCultureFromSite.Run();

        // Culture
        apiDeleteCulture.Run();
    }

    #endregion


    #region "API examples - Culture"

    /// <summary>
    /// Creates culture. Called when the "Create culture" button is pressed.
    /// </summary>
    private bool CreateCulture()
    {
        // Create new culture object
        CultureInfo newCulture = new CultureInfo();

        // Set the properties
        newCulture.CultureName = "My new culture";
        newCulture.CultureCode = "MyNewCulture";
        newCulture.CultureShortName = "MyCultureShortName";

        // Create the culture
        CultureInfoProvider.SetCultureInfo(newCulture);

        return true;
    }


    /// <summary>
    /// Gets and updates culture. Called when the "Get and update culture" button is pressed.
    /// Expects the CreateCulture method to be run first.
    /// </summary>
    private bool GetAndUpdateCulture()
    {
        // Get the culture
        CultureInfo updateCulture = CultureInfoProvider.GetCultureInfo("MyNewCulture");
        if (updateCulture != null)
        {
            // Update the property
            updateCulture.CultureName = updateCulture.CultureName.ToLower();

            // Update the culture
            CultureInfoProvider.SetCultureInfo(updateCulture);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates cultures. Called when the "Get and bulk update cultures" button is pressed.
    /// Expects the CreateCulture method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateCultures()
    {
        // Prepare the parameters
        string where = "CultureCode LIKE N'MyNewCulture%'";

        // Get the data
        DataSet cultures = CultureInfoProvider.GetCultures(where, null);
        if (!DataHelper.DataSourceIsEmpty(cultures))
        {
            // Loop through the individual items
            foreach (DataRow cultureDr in cultures.Tables[0].Rows)
            {
                // Create object from DataRow
                CultureInfo modifyCulture = new CultureInfo(cultureDr);

                // Update the properties
                modifyCulture.CultureName = modifyCulture.CultureName.ToUpper();

                // Update the culture
                CultureInfoProvider.SetCultureInfo(modifyCulture);
            }

            return true;
        }

        return false;
    }


    /// <summary>
    /// Deletes culture. Called when the "Delete culture" button is pressed.
    /// Expects the CreateCulture method to be run first.
    /// </summary>
    private bool DeleteCulture()
    {
        // Get the culture
        CultureInfo deleteCulture = CultureInfoProvider.GetCultureInfo("MyNewCulture");

        // Delete the culture
        CultureInfoProvider.DeleteCultureInfo(deleteCulture);

        return (deleteCulture != null);
    }

    #endregion


    #region "API examples - Culture on site"

    /// <summary>
    /// Adds culture to site. Called when the "Add culture to site" button is pressed.
    /// Expects the CreateCulture method to be run first.
    /// </summary>
    private bool AddCultureToSite()
    {
        // Get the culture
        CultureInfo culture = CultureInfoProvider.GetCultureInfo("MyNewCulture");
        if (culture != null)
        {
            // Add culture to the site
            CultureSiteInfoProvider.AddCultureToSite(culture.CultureID, SiteContext.CurrentSiteID);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Removes culture from site. Called when the "Remove culture from site" button is pressed.
    /// Expects the AddCultureToSite method to be run first.
    /// </summary>
    private bool RemoveCultureFromSite()
    {
        // Get the culture
        CultureInfo removeCulture = CultureInfoProvider.GetCultureInfo("MyNewCulture");
        if (removeCulture != null)
        {
            // Get the binding
            CultureSiteInfo cultureSite = CultureSiteInfoProvider.GetCultureSiteInfo(removeCulture.CultureID, SiteContext.CurrentSiteID);

            if (cultureSite != null)
            {
                // Delete the binding
                CultureSiteInfoProvider.DeleteCultureSiteInfo(cultureSite);

                return true;
            }
        }

        return false;
    }

    #endregion
}