using System;
using System.Security.Principal;

using CMS.CMSImportExport;
using CMS.DataEngine;
using CMS.EventLog;
using CMS.Helpers;
using CMS.IO;
using CMS.Membership;
using CMS.Base;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_ImportExport_Pages_ExportObject : CMSModalPage
{
    #region "Variables"

    private bool? mIsBackupMode = null;
    private BaseInfo mExportedObjectInfo = null;
    private GeneralizedInfo mExportedTypeInfo = null;
    private string mExportedObjectDisplayName = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Indicates whether export is for backup purpose or just an export.
    /// </summary>
    private bool IsBackupMode
    {
        get
        {
            return mIsBackupMode ?? (mIsBackupMode = QueryHelper.GetBoolean("backup", false)).Value;
        }
    }


    /// <summary>
    /// Object to be exported from query parameter
    /// </summary>
    private BaseInfo ExportedObjectInfo
    {
        get
        {
            if ((mExportedObjectInfo == null) && (ExportedTypeInfo != null))
            {
                mExportedObjectInfo = ExportedTypeInfo.GetObject(QueryHelper.GetInteger("objectId", 0));
            }

            return mExportedObjectInfo;
        }
    }


    /// <summary>
    /// Object type to be exported from query parameter
    /// </summary>
    private GeneralizedInfo ExportedTypeInfo
    {
        get
        {
            return mExportedTypeInfo ?? (mExportedTypeInfo = ModuleManager.GetReadOnlyObject(QueryHelper.GetString("objectType", String.Empty)));
        }
    }


    /// <summary>
    /// Display name of the object to be exported
    /// </summary>
    private string ExportedObjectDisplayName
    {
        get
        {
            if ((mExportedObjectDisplayName == null) && (ExportedObjectInfo != null))
            {
                mExportedObjectDisplayName = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(ExportedObjectInfo.Generalized.ObjectDisplayName));
            }

            return mExportedObjectDisplayName;
        }
    }

    #endregion


    #region "Page events"

    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        if (!DebugHelper.DebugImportExport)
        {
            DisableDebugging();
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Check permissions
        string permissionName = IsBackupMode ? "BackupObjects" : "ExportObjects";

        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.globalpermissions", permissionName, SiteContext.CurrentSiteName))
        {
            RedirectToAccessDenied("cms.globalpermissions", permissionName);
        }

        // Register script for pendingCallbacks repair
        ScriptHelper.FixPendingCallbacks(Page);

        // Async control events binding
        ucAsyncControl.OnFinished += ucAsyncControl_OnFinished;
        ucAsyncControl.OnError += ucAsyncControl_OnError;

        if (!RequestHelper.IsCallback())
        {
            try
            {
                // Clean previous export files up
                ExportProvider.DeleteTemporaryFiles();
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }

            SetTitle(GetString(IsBackupMode ? "BackupObject.Title" : "ExportObject.Title"));
            btnOk.ResourceString = IsBackupMode ? "General.backup" : "General.export";
            btnOk.Click += btnOk_Click;

            // Display BETA warning
            lblBeta.Visible = CMSVersion.IsBetaVersion();
            lblBeta.Text = String.Format(GetString("export.BETAwarning"), CMSVersion.GetFriendlySystemVersion(false));

            string errorMessage = ValidateExportObject();
            if (!String.IsNullOrEmpty(errorMessage))
            {
                plcExportDetails.Visible = false;
                btnOk.Enabled = false;

                ShowError(errorMessage);
            }
            else
            {
                // Check permissions
                CheckObjectPermissions();

                lblIntro.Text = String.Format(GetString(IsBackupMode ? "BackupObject.Intro" : "ExportObject.Intro"), ExportedObjectDisplayName);

                if (!RequestHelper.IsPostBack())
                {
                    lblIntro.Visible = true;
                    txtFileName.Text = GetExportFileName(ExportedObjectInfo, IsBackupMode);
                }
            }
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        lblResult.Visible = !String.IsNullOrEmpty(lblResult.Text);

        base.OnPreRender(e);

        const string script = @"
var importTimerId = 0;

// End timer function
function StopTimer() {
    if (importTimerId) {
        clearInterval(importTimerId);
        importTimerId = 0;
        if (window.HideActivity) {
            window.HideActivity();
        }
    }
}

// Start timer function
function StartTimer() {
    if (window.Activity) {
        importTimerId = setInterval(function() { window.Activity() }, 500);
    }
}";
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "TimerFunctions", script, true);
    }

    #endregion


    private void DisplayError(Exception ex)
    {
        ucActivityBar.Visible = false;
        pnlDetails.Visible = false;
        btnOk.Enabled = false;
        pnlContent.Visible = true;

        ShowError(String.Format(GetString("exportobject.failure"), ExportedObjectDisplayName));

        EventLogProvider.LogException("Export", "ExportObject", ex);
    }


    private void btnOk_Click(object sender, EventArgs e)
    {
        // Init the Mimetype helper (required for the export)
        MimeTypeHelper.LoadMimeTypes();

        // Prepare the settings
        var exportSettings = new SiteExportSettings(MembershipContext.AuthenticatedUser)
        {
            WebsitePath = Server.MapPath("~/"),
            TargetPath = GetTargetFolder()
        };

        // Initialize
        ImportExportHelper.InitSingleObjectExportSettings(exportSettings, ExportedObjectInfo);

        string result = ImportExportHelper.ValidateExportFileName(exportSettings, txtFileName.Text);

        // Filename is valid
        if (!String.IsNullOrEmpty(result))
        {
            ShowError(result);
        }
        else
        {
            string fileName = txtFileName.Text.Trim();

            // Add extension
            if (Path.GetExtension(fileName).ToLowerCSafe() != ".zip")
            {
                fileName = fileName.TrimEnd('.') + ".zip";
            }
            txtFileName.Text = fileName;

            // Set the filename
            lblResult.Text = String.Format(GetString("ExportObject.ExportProgress"), ExportedObjectDisplayName);
            exportSettings.TargetFileName = txtFileName.Text;

            pnlContent.Visible = true;
            pnlDetails.Visible = false;
            btnOk.Enabled = false;
            ucActivityBar.Visible = true;

            try
            {
                // Export the data
                ScriptHelper.RegisterStartupScript(this, typeof(string), "StartTimer", "StartTimer();", true);
                ucAsyncControl.Parameter = exportSettings;
                ucAsyncControl.RunAsync(ExportSingleObject, WindowsIdentity.GetCurrent());
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }
    }


    private void ExportSingleObject(object parameter)
    {
        var settings = parameter as SiteExportSettings;
        if (settings == null)
        {
            throw new InvalidOperationException("Export settings have been lost.");
        }

        ExportProvider.ExportObjectsData(settings);
    }


    private void ucAsyncControl_OnError(object sender, EventArgs e)
    {
        ScriptHelper.RegisterStartupScript(this, typeof(string), "StartTimer", "StopTimer();", true);
        Exception ex = ((AsyncControl)sender).Worker.LastException;

        DisplayError(ex);
    }


    private void ucAsyncControl_OnFinished(object sender, EventArgs e)
    {
        ScriptHelper.RegisterStartupScript(this, typeof(string), "StartTimer", "StopTimer();", true);
        ucActivityBar.Visible = false;
        pnlContent.Visible = true;
        btnOk.Visible = false;

        string targetUrl = GetTargetUrl();
        string resultMessage = "ExportObject.BackupFinished";
        if (!IsBackupMode)
        {
            resultMessage = "ExportObject.lblResult";
            if (targetUrl != null)
            {
                btnDownload.OnClientClick = "window.open(" + ScriptHelper.GetString(targetUrl) + "); return false;";
                btnDownload.Visible = true;
            }
        }

        string path = GetDownloadPath(targetUrl);
        lblResult.Text = String.Format(GetString(resultMessage), ExportedObjectDisplayName, path);
    }


    /// <summary>
    /// Ensure user friendly file name
    /// </summary>
    /// <param name="infoObj">Object to be exported</param>
    /// <param name="backup">Indicates if export is treated as backup</param>
    private static string GetExportFileName(GeneralizedInfo infoObj, bool backup)
    {
        string prefix;

        // Get file name according to accessible object properties
        var ti = infoObj.TypeInfo;
        if (ti.CodeNameColumn != ObjectTypeInfo.COLUMN_NAME_UNKNOWN)
        {
            prefix = infoObj.ObjectCodeName;
        }
        else
        {
            string identifier = (ti.DisplayNameColumn != ObjectTypeInfo.COLUMN_NAME_UNKNOWN) ? infoObj.ObjectDisplayName : infoObj.ObjectGUID.ToString();

            prefix = ValidationHelper.GetCodeName(identifier);
        }

        // Backup use short file name, in other cases use long file name with object type
        if (!backup)
        {
            string objectType = ti.Inherited ? ti.OriginalObjectType : ti.ObjectType;

            prefix = objectType + "_" + prefix;
        }

        return ImportExportHelper.GenerateExportFileName(prefix.Replace(".", "_"));
    }


    private string GetTargetFolder()
    {
        string targetFolder = null;

        if (IsBackupMode)
        {
            string path = ImportExportHelper.GetObjectBackupFolder(ExportedObjectInfo);

            targetFolder = Server.MapPath(path);
        }
        else
        {
            targetFolder = ImportExportHelper.GetSiteUtilsFolder() + "Export";
        }

        return targetFolder;
    }


    private string GetTargetUrl()
    {
        string targetUrl = null;
        if (IsBackupMode)
        {
            string path = ImportExportHelper.GetObjectBackupFolder(ExportedObjectInfo);
            targetUrl = ResolveUrl(path) + "/" + txtFileName.Text;
        }
        else
        {
            string path = ImportExportHelper.GetSiteUtilsFolderRelativePath();
            if (path != null)
            {
                string fullPath = path + "Export/" + txtFileName.Text;

                string externalUrl = null;

                // Handle external storage URL
                if (StorageHelper.IsExternalStorage(fullPath))
                {
                    externalUrl = File.GetFileUrl(fullPath, SiteContext.CurrentSiteName);
                }

                // Ensure default target URL if not set
                targetUrl = String.IsNullOrEmpty(externalUrl) ? ResolveUrl(path) + "Export/" + txtFileName.Text : externalUrl;
            }
        }

        return targetUrl;
    }


    private string GetDownloadPath(string targetUrl)
    {
        string path = targetUrl;

        // Display full path
        string targetFolder = GetTargetFolder();
        if ((path == null) || StorageHelper.IsExternalStorage(targetFolder))
        {
            path = DirectoryHelper.CombinePath(targetFolder, txtFileName.Text);
            path = path.Replace(SystemContext.WebApplicationPhysicalPath, String.Empty);
        }

        return path;
    }


    private void CheckObjectPermissions()
    {
        if (!CurrentUser.IsGlobalAdministrator)
        {
            int objectSiteID = ExportedObjectInfo.Generalized.ObjectSiteID;
            string siteName = (objectSiteID > 0) ? SiteInfoProvider.GetSiteName(objectSiteID) : CurrentSiteName;

            bool allowed = false;
            if ((objectSiteID > 0) || ((ExportedObjectInfo.TypeInfo.SiteBindingObject == null) || (ExportedObjectInfo.AssignedSites[CurrentSiteName] != null)))
            {
                allowed = ExportedObjectInfo.CheckPermissions(PermissionsEnum.Read, siteName, CurrentUser);
            }

            if (!allowed)
            {
                RedirectToAccessDenied(ExportedObjectInfo.TypeInfo.ModuleName, PermissionsEnum.Read.ToString());
            }
        }
    }


    private string ValidateExportObject()
    {
        string errorMessage = null;

        // Check the object type
        if (ExportedTypeInfo == null)
        {
            errorMessage = "ExportObject.ObjectTypeNotFound";
        }

        // Check exported object
        if (ExportedObjectInfo == null)
        {
            errorMessage = "ExportObject.ObjectNotFound";
        }

        return GetString(errorMessage);
    }
}