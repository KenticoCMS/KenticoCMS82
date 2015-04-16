using System;
using System.Data;

using CMS.Helpers;
using CMS.Base;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;
using CMS.PortalEngine;
using CMS.Synchronization;

public partial class CMSAPIExamples_Code_Development_ObjectVersioning_Default : CMSAPIExamplePage
{
    #region "Initialization"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Versioning
        apiCreateVersionedObject.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateVersionedObject);
        apiCreateVersion.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateVersion);
        apiRollbackVersion.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(RollbackVersion);
        apiDestroyVersion.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DestroyVersion);
        apiDestroyHistory.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DestroyHistory);
        apiEnsureVersion.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(EnsureVersion);

        // Recycle bin
        apiDeleteObject.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteObject);
        apiRestoreObject.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(RestoreObject);

        // Deleting
        apiDestroyObject.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DestroyObject);
    }

    #endregion


    #region "Mass actions"

    /// <summary>
    /// Runs all creating and managing examples.
    /// </summary>
    public override void RunAll()
    {
        base.RunAll();

        // Versioning
        apiCreateVersionedObject.Run();
        apiCreateVersion.Run();
        apiRollbackVersion.Run();
        apiDestroyVersion.Run();
        apiDestroyHistory.Run();
        apiEnsureVersion.Run();

        // Recycle bin
        apiDeleteObject.Run();
        apiRestoreObject.Run();
    }


    /// <summary>
    /// Runs all cleanup examples.
    /// </summary>
    public override void CleanUpAll()
    {
        base.CleanUpAll();

        // Versioning
        apiDestroyObject.Run();
    }

    #endregion


    #region "API examples - Object versioning"

    /// <summary>
    /// Creates versioned css stylesheet. Called when the "Create versioned object" button is pressed.
    /// </summary>
    private bool CreateVersionedObject()
    {
        // Create new css stylesheet object
        CssStylesheetInfo newStylesheet = new CssStylesheetInfo();

        // Check if object versioning of stylesheet objects is allowed on current site
        if (ObjectVersionManager.AllowObjectVersioning(newStylesheet))
        {
            // Set the properties
            newStylesheet.StylesheetDisplayName = "My new versioned stylesheet";
            newStylesheet.StylesheetName = "MyNewVersionedStylesheet";
            newStylesheet.StylesheetText = "Some versioned CSS code";

            // Save the css stylesheet
            CssStylesheetInfoProvider.SetCssStylesheetInfo(newStylesheet);

            // Add css stylesheet to site
            int stylesheetId = newStylesheet.StylesheetID;
            int siteId = SiteContext.CurrentSiteID;

            CssStylesheetSiteInfoProvider.AddCssStylesheetToSite(stylesheetId, siteId);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Creates new version of the object. Called when the "Create version" button is pressed.
    /// Expects the CreateVersionedObject method to be run first.
    /// </summary>
    private bool CreateVersion()
    {
        // Get the css stylesheet
        CssStylesheetInfo newStylesheetVersion = CssStylesheetInfoProvider.GetCssStylesheetInfo("MyNewVersionedStylesheet");
        if (newStylesheetVersion != null)
        {
            // Check if object versioning of stylesheet objects is allowed on current site
            if (ObjectVersionManager.AllowObjectVersioning(newStylesheetVersion))
            {
                // Update the properties
                newStylesheetVersion.StylesheetDisplayName = newStylesheetVersion.StylesheetDisplayName.ToLowerCSafe();

                // Create new version
                ObjectVersionManager.CreateVersion(newStylesheetVersion, true);

                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// Provides version rollback. Called when the "Rollback version" button is pressed.
    /// Expects the CreateVersionedObject method to be run first.
    /// </summary>
    private bool RollbackVersion()
    {
        // Get the css stylesheet
        CssStylesheetInfo stylesheet = CssStylesheetInfoProvider.GetCssStylesheetInfo("MyNewVersionedStylesheet");
        if (stylesheet != null)
        {
            // Prepare query parameters
            string where = "VersionObjectID =" + stylesheet.StylesheetID + " AND VersionObjectType = '" + stylesheet.TypeInfo.ObjectType + "'";
            string orderBy = "VersionModifiedWhen ASC";
            int topN = 1;

            // Get dataset with versions according to the parameters
            DataSet versionDS = ObjectVersionHistoryInfoProvider.GetVersionHistories(where, orderBy, topN, null);

            if (!DataHelper.DataSourceIsEmpty(versionDS))
            {
                // Get version
                ObjectVersionHistoryInfo version = new ObjectVersionHistoryInfo(versionDS.Tables[0].Rows[0]);

                // Roll back
                ObjectVersionManager.RollbackVersion(version.VersionID);

                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// Destroys latest version from history. Called when the "Destroy version" button is pressed.
    /// Expects the CreateVersionedObject method to be run first.
    /// </summary>
    private bool DestroyVersion()
    {
        // Get the css stylesheet
        CssStylesheetInfo stylesheet = CssStylesheetInfoProvider.GetCssStylesheetInfo("MyNewVersionedStylesheet");
        if (stylesheet != null)
        {
            // Get the latest version
            ObjectVersionHistoryInfo version = ObjectVersionManager.GetLatestVersion(stylesheet.TypeInfo.ObjectType, stylesheet.StylesheetID);

            if (version != null)
            {
                // Destroy the latest version
                ObjectVersionManager.DestroyObjectVersion(version.VersionID);

                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// Destroys version history. Called when the "Destroy history" button is pressed.
    /// Expects the CreateVersionedObject method to be run first.
    /// </summary>
    private bool DestroyHistory()
    {
        // Get the css stylesheet
        CssStylesheetInfo stylesheet = CssStylesheetInfoProvider.GetCssStylesheetInfo("MyNewVersionedStylesheet");
        if (stylesheet != null)
        {
            // Destroy version history
            ObjectVersionManager.DestroyObjectHistory(stylesheet.TypeInfo.ObjectType, stylesheet.StylesheetID);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Destroys object (without creating new version for recycle bin). Called when the "DestroyObject" button is pressed.
    /// Expects the CreateVersionedObject method to be run first.
    /// </summary>
    private bool DestroyObject()
    {
        // Get the css stylesheet
        CssStylesheetInfo destroyStylesheet = CssStylesheetInfoProvider.GetCssStylesheetInfo("MyNewVersionedStylesheet");

        if (destroyStylesheet != null)
        {
            // Destroy the object (in action context with disabled creating of new versions for recycle bin)
            using (CMSActionContext context = new CMSActionContext())
            {
                // Disable creating of new versions
                context.CreateVersion = false;

                // Destroy the css stylesheet
                CssStylesheetInfoProvider.DeleteCssStylesheetInfo(destroyStylesheet);

                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// Creates new version of the object. Called when the "Ensure version" button is pressed.
    /// Expects the CreateVersionedObject method to be run first.
    /// </summary>
    private bool EnsureVersion()
    {
        // Get the css stylesheet
        CssStylesheetInfo stylesheet = CssStylesheetInfoProvider.GetCssStylesheetInfo("MyNewVersionedStylesheet");
        if (stylesheet != null)
        {
            // Check if object versioning of stylesheet objects is allowed on current site
            if (ObjectVersionManager.AllowObjectVersioning(stylesheet))
            {
                // Ensure version
                ObjectVersionManager.EnsureVersion(stylesheet, false);

                return true;
            }
        }

        return false;
    }

    #endregion


    #region "API examples - Object recycle bin"

    /// <summary>
    /// Deletes object. Called when the "Delete object" button is pressed.
    /// Expects the CreateVersionedObject method to be run first.
    /// </summary>
    private bool DeleteObject()
    {
        // Get the css stylesheet
        CssStylesheetInfo deleteStylesheet = CssStylesheetInfoProvider.GetCssStylesheetInfo("MyNewVersionedStylesheet");

        if (deleteStylesheet != null)
        {
            // Check if restoring from recycle bin is allowed on current site
            if (ObjectVersionManager.AllowObjectRestore(deleteStylesheet))
            {
                // Delete the css stylesheet
                CssStylesheetInfoProvider.DeleteCssStylesheetInfo(deleteStylesheet);

                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// Restores object from recycle bin. Called when the "Restore object" button is pressed.
    /// Expects the DeleteObject method to be run first.
    /// </summary>
    private bool RestoreObject()
    {
        // Prepare query parameters
        string where = "VersionObjectType = '" + CssStylesheetInfo.OBJECT_TYPE + "' AND VersionDeletedByUserID = " + MembershipContext.AuthenticatedUser.UserID;
        string orderBy = "VersionDeletedWhen DESC";
        int topN = 1;

        // Get dataset with versions according to the parameters
        DataSet versionDS = ObjectVersionHistoryInfoProvider.GetVersionHistories(where, orderBy, topN, null);

        if (!DataHelper.DataSourceIsEmpty(versionDS))
        {
            // Get version
            ObjectVersionHistoryInfo version = new ObjectVersionHistoryInfo(versionDS.Tables[0].Rows[0]);

            // Restore the object
            ObjectVersionManager.RestoreObject(version.VersionID, true);

            return true;
        }

        return false;
    }

    #endregion
}