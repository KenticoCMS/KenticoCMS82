using System;
using System.Data;

using CMS.Helpers;
using CMS.IO;
using CMS.MediaLibrary;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;
using CMS.Modules;

public partial class CMSAPIExamples_Code_Tools_MediaLibrary_Default : CMSAPIExamplePage
{
    #region "Initialization"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Media library
        apiCreateMediaLibrary.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateMediaLibrary);
        apiGetAndUpdateMediaLibrary.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndUpdateMediaLibrary);
        apiGetAndBulkUpdateMediaLibraries.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndBulkUpdateMediaLibraries);
        apiDeleteMediaLibrary.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteMediaLibrary);

        // Media folder
        apiCreateMediaFolder.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateMediaFolder);
        apiDeleteMediaFolder.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteMediaFolder);

        // Media file
        apiCreateMediaFile.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateMediaFile);
        apiGetAndUpdateMediaFile.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndUpdateMediaFile);
        apiGetAndBulkUpdateMediaFiles.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndBulkUpdateMediaFiles);
        apiDeleteMediaFile.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteMediaFile);

        // Role permission in media library
        apiAddRolePermissionToLibrary.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(AddRolePermissionToLibrary);
        apiRemoveRolePermissionFromLibrary.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(RemoveRolePermissionFromLibrary);
    }

    #endregion


    #region "Mass actions"

    /// <summary>
    /// Runs all creating and managing examples.
    /// </summary>
    public override void RunAll()
    {
        base.RunAll();

        // Media library
        apiCreateMediaLibrary.Run();
        apiGetAndUpdateMediaLibrary.Run();
        apiGetAndBulkUpdateMediaLibraries.Run();

        // Media folder
        apiCreateMediaFolder.Run();

        // Media file
        apiCreateMediaFile.Run();
        apiGetAndUpdateMediaFile.Run();
        apiGetAndBulkUpdateMediaFiles.Run();

        // Role permission in media library
        apiAddRolePermissionToLibrary.Run();
    }


    /// <summary>
    /// Runs all cleanup examples.
    /// </summary>
    public override void CleanUpAll()
    {
        base.CleanUpAll();

        // Role permission in media library
        apiRemoveRolePermissionFromLibrary.Run();

        // Media file
        apiDeleteMediaFile.Run();

        // Media folder
        apiDeleteMediaFolder.Run();

        // Media library
        apiDeleteMediaLibrary.Run();
    }

    #endregion


    #region "API examples - Media library"

    /// <summary>
    /// Creates media library. Called when the "Create library" button is pressed.
    /// </summary>
    private bool CreateMediaLibrary()
    {
        // Create new media library object
        MediaLibraryInfo newLibrary = new MediaLibraryInfo();

        // Set the properties
        newLibrary.LibraryDisplayName = "My new library";
        newLibrary.LibraryName = "MyNewLibrary";
        newLibrary.LibraryDescription = "My new library description";
        newLibrary.LibraryFolder = "MyNewLibrary";
        newLibrary.LibrarySiteID = SiteContext.CurrentSiteID;
        newLibrary.LibraryGUID = Guid.NewGuid();
        newLibrary.LibraryLastModified = DateTime.Now;

        // Create the media library
        MediaLibraryInfoProvider.SetMediaLibraryInfo(newLibrary);

        return true;
    }


    /// <summary>
    /// Gets and updates media library. Called when the "Get and update library" button is pressed.
    /// Expects the CreateMediaLibrary method to be run first.
    /// </summary>
    private bool GetAndUpdateMediaLibrary()
    {
        // Get the media library
        MediaLibraryInfo updateLibrary = MediaLibraryInfoProvider.GetMediaLibraryInfo("MyNewLibrary", SiteContext.CurrentSiteName);
        if (updateLibrary != null)
        {
            // Update the property
            updateLibrary.LibraryDisplayName = updateLibrary.LibraryDisplayName.ToLower();

            // Update the media library
            MediaLibraryInfoProvider.SetMediaLibraryInfo(updateLibrary);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates media libraries. Called when the "Get and bulk update libraries" button is pressed.
    /// Expects the CreateMediaLibrary method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateMediaLibraries()
    {
        // Prepare the parameters
        string where = "LibraryName LIKE 'MyNew%'";

        // Get the data
        DataSet libraries = MediaLibraryInfoProvider.GetMediaLibraries(where, null);
        if (!DataHelper.DataSourceIsEmpty(libraries))
        {
            // Loop through the individual items
            foreach (DataRow libraryDr in libraries.Tables[0].Rows)
            {
                // Create object from DataRow
                MediaLibraryInfo modifyLibrary = new MediaLibraryInfo(libraryDr);

                // Update the property
                modifyLibrary.LibraryDisplayName = modifyLibrary.LibraryDisplayName.ToUpper();

                // Update the media library
                MediaLibraryInfoProvider.SetMediaLibraryInfo(modifyLibrary);
            }

            return true;
        }

        return false;
    }


    /// <summary>
    /// Deletes media library. Called when the "Delete library" button is pressed.
    /// Expects the CreateMediaLibrary method to be run first.
    /// </summary>
    private bool DeleteMediaLibrary()
    {
        // Get the media library
        MediaLibraryInfo deleteLibrary = MediaLibraryInfoProvider.GetMediaLibraryInfo("MyNewLibrary", SiteContext.CurrentSiteName);

        // Delete the media library
        MediaLibraryInfoProvider.DeleteMediaLibraryInfo(deleteLibrary);

        return (deleteLibrary != null);
    }

    #endregion


    #region "API examples - Media folder"

    /// <summary>
    /// Creates media folder. Called when the "Create folder" button is pressed.
    /// Expects the CreateMediaLibrary method to be run first.
    /// </summary>
    private bool CreateMediaFolder()
    {
        // Get media library
        MediaLibraryInfo library = MediaLibraryInfoProvider.GetMediaLibraryInfo("MyNewLibrary", SiteContext.CurrentSiteName);
        if (library != null)
        {
            // Create new media folder object
            MediaLibraryInfoProvider.CreateMediaLibraryFolder(CurrentSiteName, library.LibraryID, "MyNewFolder", false);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Deletes media folder. Called when the "Delete folder" button is pressed.
    /// Expects the CreateMediaFolder method to be run first.
    /// </summary>
    private bool DeleteMediaFolder()
    {
        // Get media library
        MediaLibraryInfo library = MediaLibraryInfoProvider.GetMediaLibraryInfo("MyNewLibrary", SiteContext.CurrentSiteName);
        if (library != null)
        {
            // Delete new media folder object
            MediaLibraryInfoProvider.DeleteMediaLibraryFolder(CurrentSiteName, library.LibraryID, "MyNewFolder", false);

            return true;
        }

        return false;
    }

    #endregion


    #region "API examples - Media file"

    /// <summary>
    /// Creates media file. Called when the "Create file" button is pressed.
    /// Expects the CreateMediaLibrary method to be run first.
    /// </summary>
    private bool CreateMediaFile()
    {
        // Prepare the parameters
        string filePath = "~/CMSAPIExamples/Code/Tools/MediaLibrary/Files/Powered_by_kentico2.gif";

        // Get media library
        MediaLibraryInfo library = MediaLibraryInfoProvider.GetMediaLibraryInfo("MyNewLibrary", SiteContext.CurrentSiteName);
        if (library != null)
        {
            // Create new media file object
            MediaFileInfo mediaFile = new MediaFileInfo(Server.MapPath(filePath), library.LibraryID);

            // Create file info
            FileInfo file = FileInfo.New(Server.MapPath(filePath));
            if (file != null)
            {
                // Set the properties
                mediaFile.FileName = "MyNewFile";
                mediaFile.FileTitle = "My new file title";
                mediaFile.FileDescription = "My new file description.";
                mediaFile.FilePath = "MyNewFolder/MyNewFile.gif";
                mediaFile.FileExtension = file.Extension;
                mediaFile.FileMimeType = "image/gif";
                mediaFile.FileSiteID = SiteContext.CurrentSiteID;
                mediaFile.FileLibraryID = library.LibraryID;
                mediaFile.FileSize = file.Length;

                // Create the media file
                MediaFileInfoProvider.SetMediaFileInfo(mediaFile);

                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// Gets and updates media file. Called when the "Get and update file" button is pressed.
    /// Expects the CreateMediaFile method to be run first.
    /// </summary>
    private bool GetAndUpdateMediaFile()
    {
        // Get the media file
        MediaFileInfo updateFile = MediaFileInfoProvider.GetMediaFileInfo(SiteContext.CurrentSiteName, "MyNewFolder/MyNewFile.gif", null);
        if (updateFile != null)
        {
            // Update the property
            updateFile.FileName = updateFile.FileName.ToLower();

            // Update the media file
            MediaFileInfoProvider.SetMediaFileInfo(updateFile);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates media files. Called when the "Get and bulk update files" button is pressed.
    /// Expects the CreateMediaFile method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateMediaFiles()
    {
        // Prepare the parameters
        string where = "FileName LIKE 'MyNew%'";

        // Get the data
        DataSet files = MediaFileInfoProvider.GetMediaFiles(where, null);
        if (!DataHelper.DataSourceIsEmpty(files))
        {
            // Loop through the individual items
            foreach (DataRow fileDr in files.Tables[0].Rows)
            {
                // Create object from DataRow
                MediaFileInfo modifyFile = new MediaFileInfo(fileDr);

                // Update the property
                modifyFile.FileName = modifyFile.FileName.ToUpper();

                // Update the media file
                MediaFileInfoProvider.SetMediaFileInfo(modifyFile);
            }

            return true;
        }

        return false;
    }


    /// <summary>
    /// Deletes media file. Called when the "Delete file" button is pressed.
    /// Expects the CreateMediaFile method to be run first.
    /// </summary>
    private bool DeleteMediaFile()
    {
        // Get the media file
        MediaFileInfo deleteFile = MediaFileInfoProvider.GetMediaFileInfo(SiteContext.CurrentSiteName, "MyNewFolder/MyNewFile.gif", null);

        // Delete the media file
        MediaFileInfoProvider.DeleteMediaFileInfo(deleteFile);

        return (deleteFile != null);
    }

    #endregion


    #region "API Examples - Role permission in media library"

    /// <summary>
    /// Adds role permission to media library. Called when the "Add role permission to library " button is pressed.
    /// Expects the CreateMediaLibrary method to be run first.
    /// </summary>
    private bool AddRolePermissionToLibrary()
    {
        // Get the media library
        MediaLibraryInfo mediaLibrary = MediaLibraryInfoProvider.GetMediaLibraryInfo("MyNewLibrary", SiteContext.CurrentSiteName);

        // Get the role
        RoleInfo libraryRole = RoleInfoProvider.GetRoleInfo("CMSDeskAdmin", SiteContext.CurrentSiteID);

        // Get the permission
        PermissionNameInfo libraryPermission = PermissionNameInfoProvider.GetPermissionNameInfo("FileCreate", "CMS.MediaLibrary", null);

        if ((mediaLibrary != null) && (libraryRole != null) && (libraryPermission != null))
        {
            // Create a new media library role permision info
            MediaLibraryRolePermissionInfo rolePermission = new MediaLibraryRolePermissionInfo();

            // Set the values
            rolePermission.LibraryID = mediaLibrary.LibraryID;
            rolePermission.RoleID = libraryRole.RoleID;
            rolePermission.PermissionID = libraryPermission.PermissionId;

            // Add role permission to media library
            MediaLibraryRolePermissionInfoProvider.SetMediaLibraryRolePermissionInfo(rolePermission);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Removes role permission from media library. Called when the "Remove role permission from library" button is pressed.
    /// Expects the AddRolePermissionToLibrary method to be run first.
    /// </summary>
    private bool RemoveRolePermissionFromLibrary()
    {
        // Get the media library
        MediaLibraryInfo mediaLibrary = MediaLibraryInfoProvider.GetMediaLibraryInfo("MyNewLibrary", SiteContext.CurrentSiteName);

        // Get the role
        RoleInfo libraryRole = RoleInfoProvider.GetRoleInfo("CMSDeskAdmin", SiteContext.CurrentSiteID);

        // Get the permission
        PermissionNameInfo libraryPermission = PermissionNameInfoProvider.GetPermissionNameInfo("FileCreate", "CMS.MediaLibrary", null);

        if ((mediaLibrary != null) && (libraryRole != null) && (libraryPermission != null))
        {
            // Get media library role permission info
            MediaLibraryRolePermissionInfo rolePermission = MediaLibraryRolePermissionInfoProvider.GetMediaLibraryRolePermissionInfo(mediaLibrary.LibraryID, libraryRole.RoleID, libraryPermission.PermissionId);

            // Remove role permission from media library
            MediaLibraryRolePermissionInfoProvider.DeleteMediaLibraryRolePermissionInfo(rolePermission);

            return (rolePermission != null);
        }

        return false;
    }

    #endregion
}