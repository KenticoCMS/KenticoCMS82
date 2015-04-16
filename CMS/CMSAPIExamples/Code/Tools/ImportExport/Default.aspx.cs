using System;

using CMS.CMSImportExport;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.IO;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;

public partial class CMSAPIExamples_Code_Tools_ImportExport_Default : CMSAPIExamplePage
{
    #region "Initialization"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Import.
        apiImportObject.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(ImportObject);
        apiImportSite.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(ImportSite);

        // Export
        apiExportObject.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(ExportObject);
        apiExportSite.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(ExportSite);

        // Delete imported
        apiDeleteImportedObject.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteImportedObject);
        apiDeleteImportedSite.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteImportedSite);
        apiDeletePackages.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeletePackages);
    }

    #endregion


    #region "Mass actions"

    /// <summary>
    /// Runs all creating and managing examples.
    /// </summary>
    public override void RunAll()
    {
        base.RunAll();

        // Import
        apiImportObject.Run();
        apiImportSite.Run();

        // Export
        apiExportObject.Run();
        apiExportSite.Run();
    }


    /// <summary>
    /// Runs all cleanup examples.
    /// </summary>
    public override void CleanUpAll()
    {
        base.CleanUpAll();

        // Delete imported
        apiDeleteImportedObject.Run();
        apiDeleteImportedSite.Run();
        apiDeletePackages.Run();
    }

    #endregion


    #region "API examples - Import"

    /// <summary>
    /// Imports user object. Called when the "Import object" button is pressed.
    /// </summary>
    private bool ImportObject()
    {
        // Create site import settings
        SiteImportSettings settings = new SiteImportSettings(MembershipContext.AuthenticatedUser);

        // Initialize the settings
        settings.WebsitePath = Server.MapPath("~/");
        settings.SourceFilePath = settings.WebsitePath + "CMSAPIExamples\\Code\\Tools\\ImportExport\\Packages\\APIExample_User.zip";
        settings.ImportType = ImportTypeEnum.AllNonConflicting;
        settings.LoadDefaultSelection();

        // Import
        ImportProvider.ImportObjectsData(settings);

        // Delete temporary data
        ImportProvider.DeleteTemporaryFiles(settings, false);

        return true;
    }


    /// <summary>
    /// Imports site. Called when the "Import site" button is pressed.
    /// </summary>
    private bool ImportSite()
    {
        // Prepare the properties
        string websitePath = Server.MapPath("~/");
        string sourceFilePath = websitePath + "CMSAPIExamples\\Code\\Tools\\ImportExport\\Packages\\APIExample_Site.zip";
        string siteDisplayName = "My new imported site";
        string siteName = "MyNewImportedSite";
        string siteDomain = "127.0.0.1";

        // Ensure there is no site with the set name
        if (SiteInfoProvider.GetSiteInfo(siteName) == null)
        {
            // Import
            ImportProvider.ImportSite(siteName, siteDisplayName, siteDomain, sourceFilePath, websitePath, MembershipContext.AuthenticatedUser);

            return true;
        }

        return false;
    }

    #endregion


    #region "API examples - Export"

    /// <summary>
    /// Exports user object. Called when the "Export object" button is pressed.
    /// Expects the ImportUser method to be run first.
    /// </summary>
    private bool ExportObject()
    {
        // Delete temporary data
        try
        {
            ExportProvider.DeleteTemporaryFiles();
        }
        catch
        {
        }

        // Get user
        UserInfo exportedUser = UserInfoProvider.GetUserInfo("MyNewImportedUser");

        // Ensure that user exists
        if (exportedUser != null)
        {
            // Prepare the properties
            string websitePath = Server.MapPath("~/");
            string exportFileName = string.Format("APIExample_User_{0:yyyy-MM-dd_hh-mm}.zip", DateTime.Now);
            string exportFilePath = FileHelper.GetFullFilePhysicalPath(ImportExportHelper.GetSiteUtilsFolder(), websitePath) + "Export\\" + exportFileName;

            // Ensure there is no exported package with the same name
            if (!File.Exists(exportFilePath))
            {
                // Export
                ExportProvider.ExportObject(exportedUser, exportFilePath, websitePath, MembershipContext.AuthenticatedUser);

                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// Exports site. Called when the "Export site" button is pressed.
    /// Expects the ImportSite method to be run first.
    /// </summary>
    private bool ExportSite()
    {
        // Delete temporary data
        try
        {
            ExportProvider.DeleteTemporaryFiles();
        }
        catch
        {
        }

        // Prepare the properties
        string websitePath = Server.MapPath("~/");
        string exportFileName = string.Format("APIExample_Site_{0:yyyy-MM-dd_hh-mm}.zip", DateTime.Now);
        string exportFilePath = FileHelper.GetFullFolderPhysicalPath(ImportExportHelper.GetSiteUtilsFolder(), websitePath) + "Export\\" + exportFileName;
        string siteName = "MyNewImportedSite";

        // Ensure that site exists
        if (SiteInfoProvider.GetSiteInfo(siteName) != null)
        {
            // Ensure there is no exported package with the same name
            if (!File.Exists(exportFilePath))
            {
                // Export
                ExportProvider.ExportSite(siteName, exportFilePath, websitePath, false, MembershipContext.AuthenticatedUser);

                return true;
            }
        }

        return false;
    }

    #endregion


    #region "Delete imported objects"

    /// <summary>
    /// Deletes site. Called when the "Delete imported site" button is pressed.
    /// Expects the ImportSite method to be run first.
    /// </summary>
    private bool DeleteImportedSite()
    {
        // Get the site
        SiteInfo deleteSite = SiteInfoProvider.GetSiteInfo("MyNewImportedSite");

        if (deleteSite != null)
        {
            TreeProvider treeProvider = new TreeProvider(MembershipContext.AuthenticatedUser);

            // Delete documents belonging under the site
            DocumentHelper.DeleteSiteTree("MyNewImportedSite", treeProvider);

            // Delete the site
            SiteInfoProvider.DeleteSite(deleteSite);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Deletes user. Called when the "Delete imported object" button is pressed.
    /// Expects the ImportObject method to be run first.
    /// </summary>
    private bool DeleteImportedObject()
    {
        // Get the user
        UserInfo deleteUser = UserInfoProvider.GetUserInfo("MyNewImportedUser");

        // Delete the user
        UserInfoProvider.DeleteUser(deleteUser);

        return (deleteUser != null);
    }


    /// <summary>
    /// Deletes exported packages. Called when the "Delete exported packages" button is pressed.
    /// </summary>
    private bool DeletePackages()
    {
        // Prepare parameters
        string websitePath = Server.MapPath("~/");
        string exportPath = FileHelper.GetFullFolderPhysicalPath(ImportExportHelper.GetSiteUtilsFolder(), websitePath) + "Export\\";
        string filesToDelete = @"APIExample*.zip";

        // Get list of export packages
        string[] fileList = Directory.GetFiles(exportPath, filesToDelete);

        bool filesDeleted = true;

        // Delete each file
        foreach (string file in fileList)
        {
            try
            {
                File.Delete(file);
            }
            catch
            {
                // Deletion was unsuccessfull
                filesDeleted = false;
            }
        }

        return filesDeleted;
    }

    #endregion
}