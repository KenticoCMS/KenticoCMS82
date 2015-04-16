using System;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Collections;
using System.Data;

using CMS.EventLog;
using CMS.Helpers;
using CMS.IO;
using CMS.MediaLibrary;
using CMS.Base;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.ExtendedControls;

public partial class CMSModules_MediaLibrary_Controls_MediaLibrary_FolderActions_CopyMoveFolder : CMSLiveModalPage
{
    #region "Variables"

    private static readonly Hashtable mInfos = new Hashtable();
    private Hashtable mParameters;

    private static string refreshScript;

    private MediaLibraryInfo mLibraryInfo;
    private SiteInfo mLibrarySiteInfo;
    private string mLibraryRootFolder;
    private string mLibraryPath = "";
    private bool mAllFiles;

    #endregion


    #region "Private properties"

    /// <summary>
    /// ID of the media library.
    /// </summary>
    private int MediaLibraryID
    {
        get;
        set;
    }


    /// <summary>
    /// Gets current library info.
    /// </summary>
    private MediaLibraryInfo LibraryInfo
    {
        get
        {
            if ((mLibraryInfo == null) && (MediaLibraryID > 0))
            {
                mLibraryInfo = MediaLibraryInfoProvider.GetMediaLibraryInfo(MediaLibraryID);
            }
            return mLibraryInfo;
        }
    }


    /// <summary>
    /// Gets info on site library belongs to.
    /// </summary>
    private SiteInfo LibrarySiteInfo
    {
        get
        {
            if ((mLibrarySiteInfo == null) && (LibraryInfo != null))
            {
                mLibrarySiteInfo = SiteInfoProvider.GetSiteInfo(LibraryInfo.LibrarySiteID);
            }
            return mLibrarySiteInfo;
        }
    }


    /// <summary>
    /// Type of the action.
    /// </summary>
    private string CopyMoveAction
    {
        get;
        set;
    }


    /// <summary>
    /// Media library Folder path.
    /// </summary>
    private string FolderPath
    {
        get;
        set;
    }


    /// <summary>
    /// Media library root folder path.
    /// </summary>
    private string RootFolder
    {
        get;
        set;
    }


    /// <summary>
    /// Path where the item(s) should be copied/moved.
    /// </summary>
    private string NewPath
    {
        get;
        set;
    }


    /// <summary>
    /// List of files to copy/move.
    /// </summary>
    private string Files
    {
        get;
        set;
    }


    /// <summary>
    /// Determines whether all files should be copied.
    /// </summary>
    private bool AllFiles
    {
        get
        {
            return mAllFiles;
        }
        set
        {
            mAllFiles = value;
        }
    }


    /// <summary>
    /// Current Error.
    /// </summary>
    private string CurrentError
    {
        get
        {
            return ValidationHelper.GetString(mInfos["ProcessingError_" + ctlAsyncLog.ProcessGUID], string.Empty);
        }
        set
        {
            mInfos["ProcessingError_" + ctlAsyncLog.ProcessGUID] = value;
        }
    }


    /// <summary>
    /// Current Info.
    /// </summary>
    private string CurrentInfo
    {
        get
        {
            return ValidationHelper.GetString(mInfos["ProcessingInfo_" + ctlAsyncLog.ProcessGUID], string.Empty);
        }
        set
        {
            mInfos["ProcessingInfo_" + ctlAsyncLog.ProcessGUID] = value;
        }
    }


    /// <summary>
    /// Indicates whether the properties are just loaded - no folder was previously selected.
    /// </summary>
    private bool IsLoad
    {
        get
        {
            return ValidationHelper.GetBoolean(Parameters["load"], false);
        }
    }

    /// <summary>
    /// Current log context.
    /// </summary>
    private LogContext CurrentLog
    {
        get
        {
            return EnsureLog();
        }
    }


    /// <summary>
    /// Hashtable containing dialog parameters.
    /// </summary>
    private Hashtable Parameters
    {
        get
        {
            if (mParameters == null)
            {
                string identifier = QueryHelper.GetString("params", null);
                mParameters = (Hashtable)WindowHelper.GetItem(identifier);
            }
            return mParameters;
        }
    }


    /// <summary>
    /// Gets true if page is used on the live site. False if is in administration.
    /// </summary>
    private bool IsLiveSite
    {
        get
        {
            return QueryHelper.GetBoolean("islivesite", false);
        }
    }

    #endregion


    #region "Public properties"

    /// <summary>
    /// Local page messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
    }


    /// <summary>
    /// Returns media library root folder path.
    /// </summary>
    public string LibraryRootFolder
    {
        get
        {
            if ((LibrarySiteInfo != null) && (mLibraryRootFolder == null))
            {
                mLibraryRootFolder = MediaLibraryHelper.GetMediaRootFolderPath(LibrarySiteInfo.SiteName);
            }
            return mLibraryRootFolder;
        }
    }


    /// <summary>
    /// Gets library relative url path.
    /// </summary>
    public string LibraryPath
    {
        get
        {
            if (String.IsNullOrEmpty(mLibraryPath))
            {
                if (LibraryInfo != null)
                {
                    mLibraryPath = LibraryRootFolder + LibraryInfo.LibraryFolder;
                }
            }
            return mLibraryPath;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Init(object sender, EventArgs e)
    {
        if (IsLiveSite)
        {
            SetLiveRTL();
        }
        else
        {
            SetRTL();
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!QueryHelper.ValidateHash("hash"))
        {
            return;
        }

        // Check if hashtable containing dialog parameters is not empty
        if ((Parameters == null) || (Parameters.Count == 0))
        {
            return;
        }

        // Initialize events
        ctlAsyncLog.OnFinished += ctlAsyncLog_OnFinished;
        ctlAsyncLog.OnError += ctlAsyncLog_OnError;
        ctlAsyncLog.OnRequestLog += ctlAsyncLog_OnRequestLog;
        ctlAsyncLog.OnCancel += ctlAsyncLog_OnCancel;

        // Get the source node
        MediaLibraryID = ValidationHelper.GetInteger(Parameters["libraryid"], 0);
        CopyMoveAction = ValidationHelper.GetString(Parameters["action"], string.Empty);
        FolderPath = Path.EnsureBackslashes(ValidationHelper.GetString(Parameters["path"], ""));
        Files = ValidationHelper.GetString(Parameters["files"], "").Trim('|');
        RootFolder = MediaLibraryHelper.GetMediaRootFolderPath(SiteContext.CurrentSiteName);
        AllFiles = ValidationHelper.GetBoolean(Parameters["allFiles"], false);
        NewPath = Path.EnsureBackslashes(ValidationHelper.GetString(Parameters["newpath"], ""));

        // Target folder
        string tarFolder = NewPath;
        if (string.IsNullOrEmpty(tarFolder) && (LibraryInfo != null))
        {
            tarFolder = LibraryInfo.LibraryFolder + " (root)";
        }
        lblFolder.Text = tarFolder;

        if (!IsLoad)
        {
            if (AllFiles || String.IsNullOrEmpty(Files))
            {
                if (AllFiles)
                {
                    lblFilesToCopy.ResourceString = "media.folder.filestoall" + CopyMoveAction.ToLowerCSafe();
                }
                else
                {
                    lblFilesToCopy.ResourceString = "media.folder.folderto" + CopyMoveAction.ToLowerCSafe();
                }

                // Source folder
                string srcFolder = FolderPath;
                if (string.IsNullOrEmpty(srcFolder) && (LibraryInfo != null))
                {
                    srcFolder = LibraryInfo.LibraryFolder + "&nbsp;(root)";
                }
                lblFileList.Text = HTMLHelper.HTMLEncode(srcFolder);
            }
            else
            {
                lblFilesToCopy.ResourceString = "media.folder.filesto" + CopyMoveAction.ToLowerCSafe();
                string[] fileList = Files.Split('|');
                foreach (string file in fileList)
                {
                    lblFileList.Text += HTMLHelper.HTMLEncode(DirectoryHelper.CombinePath(FolderPath.TrimEnd('\\'), file)) + "<br />";
                }
            }

            if (!RequestHelper.IsCallback() && !URLHelper.IsPostback())
            {
                bool performAction = ValidationHelper.GetBoolean(Parameters["performaction"], false);
                if (performAction)
                {
                    // Perform Move or Copy
                    PerformAction();
                }
            }

            pnlInfo.Visible = true;
            pnlEmpty.Visible = false;
        }
        else
        {
            pnlInfo.Visible = false;
            pnlEmpty.Visible = true;
            lblEmpty.Text = GetString("media.copymove.select");

            // Disable New folder button
            ScriptHelper.RegisterStartupScript(Page, typeof(Page), "DisableNewFolderOnLoad", ScriptHelper.GetScript("if ((window.parent != null) && window.parent.DisableNewFolderBtn) { window.parent.DisableNewFolderBtn(); }"));
        }
    }

    #endregion


    /// <summary>
    /// Moves document.
    /// </summary>
    private void PerformAction(object parameter)
    {
        AddLog(GetString(CopyMoveAction.ToLowerCSafe() == "copy" ? "media.copy.startcopy" : "media.move.startmove"));

        if (LibraryInfo != null)
        {
            // Library path (used in recursive copy process)
            string libPath = MediaLibraryInfoProvider.GetMediaLibraryFolderPath(SiteContext.CurrentSiteName, LibraryInfo.LibraryFolder);

            // Ensure libPath is in original path type
            libPath = Path.GetFullPath(libPath);

            // Original path on disk from query
            string origPath = Path.GetFullPath(DirectoryHelper.CombinePath(libPath, FolderPath));

            // Original path in DB
            string origDBPath = Path.EnsureSlashes(FolderPath);

            // New path in DB
            string newDBPath;

            AddLog(NewPath);

            // Check if requested folder is in library root folder
            if (!origPath.StartsWithCSafe(libPath, true))
            {
                CurrentError = GetString("media.folder.nolibrary");
                AddLog(CurrentError);
                return;
            }

            string origFolderName = Path.GetFileName(origPath);

            if ((String.IsNullOrEmpty(Files) && !mAllFiles) && string.IsNullOrEmpty(origFolderName))
            {
                NewPath = NewPath + "\\" + LibraryInfo.LibraryFolder;
                NewPath = NewPath.Trim('\\');
            }

            // New path on disk
            string newPath = NewPath;

            // Process current folder copy/move action
            if (String.IsNullOrEmpty(Files) && !AllFiles)
            {
                newPath = Path.EnsureEndBackslash(newPath) + origFolderName;
                newPath = newPath.Trim('\\');

                // Check if moving into same folder
                if ((CopyMoveAction.ToLowerCSafe() == "move") && (newPath == FolderPath))
                {
                    CurrentError = GetString("media.move.foldermove");
                    AddLog(CurrentError);
                    return;
                }

                // Error if moving folder into itself
                string newRootPath = Path.GetDirectoryName(newPath).Trim();
                string newSubRootFolder = Path.GetFileName(newPath).ToLowerCSafe().Trim();
                string originalSubRootFolder = Path.GetFileName(FolderPath).ToLowerCSafe().Trim();
                if (String.IsNullOrEmpty(Files) && (CopyMoveAction.ToLowerCSafe() == "move") && newPath.StartsWithCSafe(Path.EnsureEndBackslash(FolderPath))
                    && (originalSubRootFolder == newSubRootFolder) && (newRootPath == FolderPath))
                {
                    CurrentError = GetString("media.move.movetoitself");
                    AddLog(CurrentError);
                    return;
                }

                try
                {
                    // Get unique path for copy or move
                    string path = Path.GetFullPath(DirectoryHelper.CombinePath(libPath, newPath));
                    path = MediaLibraryHelper.EnsureUniqueDirectory(path);
                    newPath = path.Remove(0, (libPath.Length + 1));

                    // Get new DB path
                    newDBPath = Path.EnsureSlashes(newPath.Replace(Path.EnsureEndBackslash(libPath), ""));
                }
                catch (Exception ex)
                {
                    CurrentError = GetString("general.erroroccurred") + " " + ex.Message;
                    EventLogProvider.LogException("MediaFolder", CopyMoveAction, ex);
                    AddLog(CurrentError);
                    return;
                }
            }
            else
            {
                origDBPath = Path.EnsureSlashes(FolderPath);
                newDBPath = Path.EnsureSlashes(newPath.Replace(libPath, "")).Trim('/');
            }

            // Error if moving folder into its subfolder
            if ((String.IsNullOrEmpty(Files) && !AllFiles) && (CopyMoveAction.ToLowerCSafe() == "move") && newPath.StartsWithCSafe(Path.EnsureEndBackslash(FolderPath)))
            {
                CurrentError = GetString("media.move.parenttochild");
                AddLog(CurrentError);
                return;
            }

            // Error if moving files into same directory
            if ((!String.IsNullOrEmpty(Files) || AllFiles) && (CopyMoveAction.ToLowerCSafe() == "move") && (newPath.TrimEnd('\\') == FolderPath.TrimEnd('\\')))
            {
                CurrentError = GetString("media.move.fileserror");
                AddLog(CurrentError);
                return;
            }

            NewPath = newPath;
            refreshScript = @"
var topWin = GetTop();
if (topWin) {
    if ((topWin.opener) && (typeof(topWin.opener.RefreshLibrary) != 'undefined')) {
        topWin.opener.RefreshLibrary(" + ScriptHelper.GetString(NewPath.Replace('\\', '|')) + @");
    } 
    else if ((topWin.wopener) && (typeof(topWin.wopener.RefreshLibrary) != 'undefined')) { 
        topWin.wopener.RefreshLibrary(" + ScriptHelper.GetString(NewPath.Replace('\\', '|')) + @"); 
    } 
    CloseDialog();
}";

            // If mFiles is empty handle directory copy/move
            if (String.IsNullOrEmpty(Files) && !mAllFiles)
            {
                try
                {
                    switch (CopyMoveAction.ToLowerCSafe())
                    {
                        case "move":
                            MediaLibraryInfoProvider.MoveMediaLibraryFolder(SiteContext.CurrentSiteName, MediaLibraryID, origDBPath, newDBPath);
                            break;

                        case "copy":
                            MediaLibraryInfoProvider.CopyMediaLibraryFolder(SiteContext.CurrentSiteName, MediaLibraryID, origDBPath, newDBPath, CurrentUser.UserID);
                            break;
                    }
                }
                catch (UnauthorizedAccessException ex)
                {
                    CurrentError = GetString("general.erroroccurred") + " " + GetString("media.security.accessdenied");
                    EventLogProvider.LogException("MediaFolder", CopyMoveAction, ex);
                    AddLog(CurrentError);
                }
                catch (ThreadAbortException ex)
                {
                    string state = ValidationHelper.GetString(ex.ExceptionState, string.Empty);
                    if (state == CMSThread.ABORT_REASON_STOP)
                    {
                        // When canceled
                        CurrentInfo = GetString("general.actioncanceled");
                        AddLog(CurrentInfo);
                    }
                    else
                    {
                        // Log error
                        CurrentError = GetString("general.erroroccurred") + " " + ex.Message;
                        EventLogProvider.LogException("MediaFolder", CopyMoveAction, ex);
                        AddLog(CurrentError);
                    }
                }
                catch (Exception ex)
                {
                    CurrentError = GetString("general.erroroccurred") + " " + ex.Message;
                    EventLogProvider.LogException("MediaFolder", CopyMoveAction, ex);
                    AddLog(CurrentError);
                }
            }
            else
            {
                string origDBFilePath;
                string newDBFilePath;

                if (!mAllFiles)
                {
                    try
                    {
                        string[] files = Files.Split('|');
                        foreach (string filename in files)
                        {
                            origDBFilePath = (string.IsNullOrEmpty(origDBPath)) ? filename : origDBPath + "/" + filename;
                            newDBFilePath = (string.IsNullOrEmpty(newDBPath)) ? filename : newDBPath + "/" + filename;
                            AddLog(filename);
                            CopyMove(origDBFilePath, newDBFilePath);
                        }
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        CurrentError = GetString("general.erroroccurred") + " " + ResHelper.GetString("media.security.accessdenied");
                        EventLogProvider.LogException("MediaFile", CopyMoveAction, ex);
                        AddLog(CurrentError);
                    }
                    catch (ThreadAbortException ex)
                    {
                        string state = ValidationHelper.GetString(ex.ExceptionState, string.Empty);
                        if (state == CMSThread.ABORT_REASON_STOP)
                        {
                            // When canceled
                            CurrentInfo = GetString("general.actioncanceled");
                            AddLog(CurrentInfo);
                        }
                        else
                        {
                            // Log error
                            CurrentError = GetString("general.erroroccurred") + " " + ex.Message;
                            EventLogProvider.LogException("MediaFile", CopyMoveAction, ex);
                            AddLog(CurrentError);
                        }
                    }
                    catch (Exception ex)
                    {
                        CurrentError = GetString("general.erroroccurred") + " " + ex.Message;
                        EventLogProvider.LogException("MediaFile", CopyMoveAction, ex);
                        AddLog(CurrentError);
                    }
                }
                else
                {
                    HttpContext context = (parameter as HttpContext);
                    if (context != null)
                    {
                        HttpContext.Current = context;

                        DataSet files = GetFileSystemDataSource();
                        if (!DataHelper.IsEmpty(files))
                        {
                            foreach (DataRow file in files.Tables[0].Rows)
                            {
                                string fileName = ValidationHelper.GetString(file["FileName"], "");

                                AddLog(fileName);

                                origDBFilePath = (string.IsNullOrEmpty(origDBPath)) ? fileName : origDBPath + "/" + fileName;
                                newDBFilePath = (string.IsNullOrEmpty(newDBPath)) ? fileName : newDBPath + "/" + fileName;

                                // Clear current httpcontext for CopyMove action in threat
                                HttpContext.Current = null;

                                try
                                {
                                    CopyMove(origDBFilePath, newDBFilePath);
                                }
                                catch (UnauthorizedAccessException ex)
                                {
                                    CurrentError = GetString("general.erroroccurred") + " " + ResHelper.GetString("media.security.accessdenied");
                                    EventLogProvider.LogException("MediaFile", CopyMoveAction, ex);
                                    AddLog(CurrentError);
                                    return;
                                }
                                catch (ThreadAbortException ex)
                                {
                                    string state = ValidationHelper.GetString(ex.ExceptionState, string.Empty);
                                    if (state == CMSThread.ABORT_REASON_STOP)
                                    {
                                        // When canceled
                                        CurrentInfo = GetString("general.actioncanceled");
                                        AddLog(CurrentInfo);
                                    }
                                    else
                                    {
                                        // Log error
                                        CurrentError = GetString("general.erroroccurred") + " " + ex.Message;
                                        EventLogProvider.LogException("MediaFile", CopyMoveAction, ex);
                                        AddLog(CurrentError);
                                        return;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    CurrentError = GetString("general.erroroccurred") + " " + ex.Message;
                                    EventLogProvider.LogException("MediaFile", CopyMoveAction, ex);
                                    AddLog(CurrentError);
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }
    }


    /// <summary>
    /// Performs the Move of Copy action.
    /// </summary>
    public void PerformAction()
    {
        if (!IsLoad)
        {
            if (CheckPermissions())
            {
                pnlInfo.Visible = true;
                pnlEmpty.Visible = false;

                if (CopyMoveAction.ToLowerCSafe() == "copy")
                {
                    ctlAsyncLog.TitleText = GetString("media.copy.startcopy");
                }
                else
                {
                    ctlAsyncLog.TitleText = GetString("media.move.startmove");
                }
                RunAsync(PerformAction);
            }
        }
        else
        {
            pnlInfo.Visible = false;
            pnlEmpty.Visible = true;
            lblEmpty.Text = GetString("media.copymove.noselect");
        }
    }


    /// <summary>
    /// Performs action itself.
    /// </summary>
    /// <param name="origDBFilePath">Path of the file specified in DB</param>
    /// <param name="newDBFilePath">New path of the file being inserted into DB</param>
    private void CopyMove(string origDBFilePath, string newDBFilePath)
    {
        switch (CopyMoveAction.ToLowerCSafe())
        {
            case "move":
                MediaFileInfoProvider.MoveMediaFile(SiteContext.CurrentSiteName, MediaLibraryID, origDBFilePath, newDBFilePath);
                break;

            case "copy":
                MediaFileInfoProvider.CopyMediaFile(SiteContext.CurrentSiteName, MediaLibraryID, origDBFilePath, newDBFilePath, false, CurrentUser.UserID);
                break;
        }
    }


    /// <summary>
    /// Returns set of files in the file system.
    /// </summary>
    private DataSet GetFileSystemDataSource()
    {
        fileSystemDataSource.Path = LibraryPath + "/" + Path.EnsureSlashes(FolderPath) + "/";
        fileSystemDataSource.Path = Path.EnsureBackslashes(fileSystemDataSource.Path).Replace("|", "\\");

        return (DataSet)fileSystemDataSource.DataSource;
    }


    #region "Help methods"

    /// <summary>
    /// Ensures the logging context.
    /// </summary>
    protected LogContext EnsureLog()
    {
        LogContext currentLog = LogContext.EnsureLog(ctlAsyncLog.ProcessGUID);
        return currentLog;
    }


    /// <summary>
    /// Adds the script to the output request window.
    /// </summary>
    /// <param name="script">Script to add</param>
    public override void AddScript(string script)
    {
        ltlScript.Text += ScriptHelper.GetScript(script);
    }


    /// <summary>
    /// Check permissions for selected library.
    /// </summary>
    private bool CheckPermissions()
    {
        // If mFiles is empty handle directory copy/move
        if (String.IsNullOrEmpty(Files) && !mAllFiles)
        {
            if (CopyMoveAction.ToLowerCSafe().Trim() == "copy")
            {
                // Check 'Folder create' permission
                if (!MediaLibraryInfoProvider.IsUserAuthorizedPerLibrary(LibraryInfo, "foldercreate"))
                {
                    ShowError(MediaLibraryHelper.GetAccessDeniedMessage("foldercreate"));
                    return false;
                }
            }
            else
            {
                // Check 'Folder modify' permission
                if (!MediaLibraryInfoProvider.IsUserAuthorizedPerLibrary(LibraryInfo, "foldermodify"))
                {
                    ShowError(MediaLibraryHelper.GetAccessDeniedMessage("foldermodify"));
                    return false;
                }
            }
        }
        else
        {
            if (CopyMoveAction.ToLowerCSafe().Trim() == "copy")
            {
                // Check 'File create' permission
                if (!MediaLibraryInfoProvider.IsUserAuthorizedPerLibrary(LibraryInfo, "filecreate"))
                {
                    ShowError(MediaLibraryHelper.GetAccessDeniedMessage("filecreate"));
                    return false;
                }
            }
            else
            {
                // Check 'File modify' permission
                if (!MediaLibraryInfoProvider.IsUserAuthorizedPerLibrary(LibraryInfo, "filemodify"))
                {
                    ShowError(MediaLibraryHelper.GetAccessDeniedMessage("filemodify"));
                    return false;
                }
            }
        }
        return true;
    }

    #endregion


    #region "Handling async thread"

    private void ctlAsyncLog_OnCancel(object sender, EventArgs e)
    {
        CurrentInfo = (CopyMoveAction.ToLowerCSafe() == "copy") ? GetString("media.copy.canceled") : GetString("media.move.canceled");
        AddLog(CurrentInfo);

        pnlLog.Visible = false;
        pnlInfo.Visible = true;

        AddScript("var __pendingCallbacks = new Array();DestroyLog();");

        ShowConfirmation(CurrentInfo);
        HandlePossibleErrors();
        CurrentLog.Close();
    }


    private void ctlAsyncLog_OnRequestLog(object sender, EventArgs e)
    {
        ctlAsyncLog.LogContext = CurrentLog;
    }


    private void ctlAsyncLog_OnError(object sender, EventArgs e)
    {
        pnlLog.Visible = false;
        pnlInfo.Visible = true;

        AddScript("DestroyLog();");
        HandlePossibleErrors();
    }


    private void ctlAsyncLog_OnFinished(object sender, EventArgs e)
    {
        AddScript(!HandlePossibleErrors() ? refreshScript : "DestroyLog();");
        CurrentLog.Close();
    }


    /// <summary>
    /// Adds the log information.
    /// </summary>
    /// <param name="newLog">New log information</param>
    protected void AddLog(string newLog)
    {
        EnsureLog();
        LogContext.AppendLine(newLog);
    }


    /// <summary>
    /// Runs async thread.
    /// </summary>
    /// <param name="action">Method to run</param>
    protected void RunAsync(AsyncAction action)
    {
        pnlLog.Visible = true;
        pnlInfo.Visible = false;

        CurrentLog.Close();
        CurrentError = string.Empty;
        CurrentInfo = string.Empty;

        AddScript("InitializeLog();");

        // Ensure current user
        SessionHelper.SetValue("CurrentUser", CurrentUser);
        ctlAsyncLog.Parameter = HttpContext.Current;
        ctlAsyncLog.RunAsync(action, WindowsIdentity.GetCurrent());
    }


    /// <summary>
    /// Ensures any error or info is displayed to user.
    /// </summary>
    /// <returns>True if error occurred.</returns>
    protected bool HandlePossibleErrors()
    {
        if (!string.IsNullOrEmpty(CurrentError))
        {
            ShowError(CurrentError);
            ctlAsyncLog.LogContext = CurrentLog;
            AddScript("var __pendingCallbacks = new Array();DestroyLog();");
            pnlLog.Visible = false;
            pnlInfo.Visible = true;
            return true;
        }
        return false;
    }

    #endregion
}