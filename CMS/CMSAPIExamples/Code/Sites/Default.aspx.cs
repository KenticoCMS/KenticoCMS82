using System;
using System.Data;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.Localization;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;

public partial class CMSAPIExamples_Code_Sites_Default : CMSAPIExamplePage
{
    #region "Initialization"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Site
        apiCreateSite.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateSite);
        apiGetAndUpdateSite.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndUpdateSite);
        apiGetAndBulkUpdateSites.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndBulkUpdateSites);
        apiDeleteSite.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteSite);

        // Culture on site
        apiAddCultureToSite.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(AddCultureToSite);
        apiRemoveCultureFromSite.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(RemoveCultureFromSite);

        // Site domain alias
        apiAddDomainAliasToSite.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(AddDomainAliasToSite);
        apiDeleteSiteDomainAlias.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteSiteDomainAlias);

        // Site actions
        apiRunSite.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(RunSite);
        apiStopSite.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(StopSite);
    }

    #endregion


    #region "Mass actions"

    /// <summary>
    /// Runs all creating and managing examples.
    /// </summary>
    public override void RunAll()
    {
        base.RunAll();

        // Site
        apiCreateSite.Run();
        apiGetAndUpdateSite.Run();
        apiGetAndBulkUpdateSites.Run();

        // Culture on site
        apiAddCultureToSite.Run();

        // Domain aliases
        apiAddDomainAliasToSite.Run();

        // Site actions
        apiRunSite.Run();
        apiStopSite.Run();
    }


    /// <summary>
    /// Runs all cleanup examples.
    /// </summary>
    public override void CleanUpAll()
    {
        base.CleanUpAll();

        // Domain aliases
        apiDeleteSiteDomainAlias.Run();

        // Culture on site
        apiRemoveCultureFromSite.Run();

        // Site
        apiDeleteSite.Run();
    }

    #endregion


    #region "API examples - Site"

    /// <summary>
    /// Creates site. Called when the "Create site" button is pressed.
    /// </summary>
    private bool CreateSite()
    {
        // Create new site object
        SiteInfo newSite = new SiteInfo();

        // Set the properties
        newSite.DisplayName = "My new site";
        newSite.SiteName = "MyNewSite";
        newSite.Status = SiteStatusEnum.Stopped;
        newSite.DomainName = "127.0.0.1";

        // Save the site
        SiteInfoProvider.SetSiteInfo(newSite);

        return true;
    }


    /// <summary>
    /// Gets and updates site. Called when the "Get and update site" button is pressed.
    /// Expects the CreateSite method to be run first.
    /// </summary>
    private bool GetAndUpdateSite()
    {
        // Get the site
        SiteInfo updateSite = SiteInfoProvider.GetSiteInfo("MyNewSite");
        if (updateSite != null)
        {
            // Update the properties
            updateSite.DisplayName = updateSite.DisplayName.ToLower();

            // Save the changes
            SiteInfoProvider.SetSiteInfo(updateSite);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates sites. Called when the "Get and bulk update sites" button is pressed.
    /// Expects the CreateSite method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateSites()
    {
        // Prepare the parameters
        string where = "SiteName LIKE N'MyNewSite%'";

        // Get the data
        DataSet sites = SiteInfoProvider.GetSites().Where(where);

        if (!DataHelper.DataSourceIsEmpty(sites))
        {
            // Loop through the individual items
            foreach (DataRow siteDr in sites.Tables[0].Rows)
            {
                // Create object from DataRow
                SiteInfo modifySite = new SiteInfo(siteDr);

                // Update the properties
                modifySite.DisplayName = modifySite.DisplayName.ToUpper();

                // Save the changes
                SiteInfoProvider.SetSiteInfo(modifySite);
            }

            return true;
        }

        return false;
    }


    /// <summary>
    /// Deletes site. Called when the "Delete site" button is pressed.
    /// Expects the CreateSite method to be run first.
    /// </summary>
    private bool DeleteSite()
    {
        // Get the site
        SiteInfo deleteSite = SiteInfoProvider.GetSiteInfo("MyNewSite");

        if (deleteSite != null)
        {
            TreeProvider treeProvider = new TreeProvider(MembershipContext.AuthenticatedUser);

            // Delete documents belonging under the site
            DocumentHelper.DeleteSiteTree("MyNewSite", treeProvider);

            // Delete the site
            SiteInfoProvider.DeleteSite(deleteSite);

            return true;
        }

        return false;
    }

    #endregion


    #region "API examples - Culture site"

    /// <summary>
    /// Creates culture site. Called when the "Create site" button is pressed.
    /// </summary>
    private bool AddCultureToSite()
    {
        // Get site and culture objects
        SiteInfo site = SiteInfoProvider.GetSiteInfo("MyNewSite");
        CultureInfo culture = CultureInfoProvider.GetCultureInfo("ar-sa");

        if ((site != null) && (culture != null))
        {
            // Add culture to site
            CultureSiteInfoProvider.AddCultureToSite(culture.CultureID, site.SiteID);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Remove culture from site. Called when the "Remove culture from site" button is pressed.
    /// Expects the CreateCultureSite method to be run first.
    /// </summary>
    private bool RemoveCultureFromSite()
    {
        // Get site and culture objects
        SiteInfo site = SiteInfoProvider.GetSiteInfo("MyNewSite");
        CultureInfo culture = CultureInfoProvider.GetCultureInfo("ar-sa");

        if ((site != null) && (culture != null))
        {
            // Delete the culture site
            CultureSiteInfoProvider.RemoveCultureFromSite(culture.CultureID, site.SiteID);

            return true;
        }

        return false;
    }

    #endregion


    #region "API examples - Site domain alias"

    /// <summary>
    /// Creates site domain alias. Called when the "Create alias" button is pressed.
    /// </summary>
    private bool AddDomainAliasToSite()
    {
        // Get the site object
        SiteInfo site = SiteInfoProvider.GetSiteInfo("MyNewSite");

        if (site != null)
        {
            // Create new site domain alias object
            SiteDomainAliasInfo newAlias = new SiteDomainAliasInfo();

            // Set the properties
            newAlias.SiteDomainAliasName = "127.0.0.1";
            newAlias.SiteID = site.SiteID;

            // Save the site domain alias
            SiteDomainAliasInfoProvider.SetSiteDomainAliasInfo(newAlias);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Deletes site domain alias. Called when the "Delete alias" button is pressed.
    /// Expects the CreateSiteDomainAlias method to be run first.
    /// </summary>
    private bool DeleteSiteDomainAlias()
    {
        // Get the site object
        SiteInfo site = SiteInfoProvider.GetSiteInfo("MyNewSite");

        if (site != null)
        {
            // Get the site domain alias
            SiteDomainAliasInfo deleteAlias = SiteDomainAliasInfoProvider.GetSiteDomainAliasInfo("127.0.0.1", site.SiteID);

            // Delete the site domain alias
            SiteDomainAliasInfoProvider.DeleteSiteDomainAliasInfo(deleteAlias);

            return (deleteAlias != null);
        }

        return false;
    }

    #endregion


    #region "API examples - Site actions"

    /// <summary>
    /// Runs the site. Called when the "Run site" button is pressed.
    /// </summary>
    private bool RunSite()
    {
        // Get the site
        SiteInfo site = SiteInfoProvider.GetSiteInfo("MyNewSite");
        if (site != null)
        {
            // Stop site
            SiteInfoProvider.RunSite(site.SiteName);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Stops the site. Called when the "Stop site" button is pressed.
    /// Expects the CreateSiteDomainAlias method to be run first.
    /// </summary>
    private bool StopSite()
    {
        // Get the site
        SiteInfo site = SiteInfoProvider.GetSiteInfo("MyNewSite");
        if (site != null)
        {
            // Stop site
            SiteInfoProvider.StopSite(site.SiteName);

            return true;
        }

        return false;
    }

    #endregion
}