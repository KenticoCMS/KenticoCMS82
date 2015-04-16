using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.EventLog;
using CMS.ExtendedControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.IO;
using CMS.LicenseProvider;
using CMS.Localization;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;

using TreeNode = CMS.DocumentEngine.TreeNode;

[Security(Resource = "CMS.FileImport", Permission = "ImportFiles", ResourceSite = true)]
[UIElement("CMS.FileImport", "ImportFromServer")]
public partial class CMSModules_FileImport_Tools_ImportFromServer : CMSDeskPage
{
    #region "Variables"

    private List<string> filesList = new List<string>();
    protected string targetAliasPath = "";
    protected long filesCount = 0;
    protected long allowedFilesCount = 0;
    protected string rootPath = "~/cmsimportfiles/";

    private static List<string[]> resultListValues = new List<string[]>();
    private static List<string> errorFiles = new List<string>();

    private static readonly Hashtable mErrors = new Hashtable();

    #endregion


    #region "Properties"

    /// <summary>
    /// Current log context.
    /// </summary>
    public LogContext CurrentLog
    {
        get
        {
            return EnsureLog();
        }
    }

    /// <summary>
    /// Current Error.
    /// </summary>
    private string CurrentError
    {
        get
        {
            return ValidationHelper.GetString(mErrors["FileImport_" + ctlAsyncLog.ProcessGUID], string.Empty);
        }
        set
        {
            mErrors["FileImport_" + ctlAsyncLog.ProcessGUID] = value;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Register the main CMS script
        ScriptHelper.RegisterCMS(Page);

        // Register script for pendingCallbacks repair
        ScriptHelper.FixPendingCallbacks(Page);
        ctlAsyncLog.OnFinished += ctlAsyncLog_OnFinished;
        ctlAsyncLog.OnError += ctlAsyncLog_OnError;
        ctlAsyncLog.OnRequestLog += ctlAsyncLog_OnRequestLog;
        ctlAsyncLog.OnCancel += ctlAsyncLog_OnCancel;

        CurrentMaster.HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;

        ucFilter.Column = "FileName";
        btnFilter.Text = ResHelper.GetString("general.search");

        if (!RequestHelper.IsCallback())
        {
            string path = GetPath();
            if (StorageHelper.IsExternalStorage(path))
            {
                // Add action for prepare for import
                CurrentMaster.HeaderActions.AddAction(new HeaderAction
                {
                    Text = GetString("dialogs.mediaview.azureprepare"),
                    CommandName = "prepareforimport",
                    ButtonStyle = ButtonStyle.Default
                });
            }
            else
            {
                CurrentMaster.HeaderActionsPlaceHolder.Visible = false;
                if (!Directory.Exists(path))
                {
                    ShowError(String.Format(ResHelper.GetString("Tools.FileImport.DirectoryDoesNotExist"), rootPath));
                }
            }

            var user = MembershipContext.AuthenticatedUser;

            // Check permissions for CMS Desk -> Tools -> File Import
            if (!user.IsAuthorizedPerUIElement("CMS.FileImport", "FileImport"))
            {
                RedirectToUIElementAccessDenied("CMS.FileImport", "FileImport");
            }

            if (!user.IsAuthorizedPerResource("CMS.FileImport", "ImportFiles"))
            {
                RedirectToAccessDenied("CMS.FileImport", "ImportFiles");
            }

            // Set visibility of panels
            pnlContent.Visible = true;
            pnlLog.Visible = false;

            ScriptHelper.RegisterCMS(Page);

            // Initialize culture selector
            cultureSelector.SiteID = SiteContext.CurrentSiteID;
            pathElem.SiteID = SiteContext.CurrentSiteID;

            // Prepare unigrid
            gridImport.DataSource = GetFileSystemDataSource(ucFilter.WhereCondition);
            gridImport.OnExternalDataBound += gridImport_OnExternalDataBound;
            gridImport.SelectionJavascript = "UpdateCount";
            gridImport.ZeroRowsText = GetString("Tools.FileImport.NoFiles");
            gridImport.OnShowButtonClick += gridImport_OnShowButtonClick;

            // Prepare async panel
            ctlAsyncLog.TitleText = GetString("tools.fileimport.importing");

            lblTitle.Text = GetString("Tools.FileImport.ImportedFiles") + " " + rootPath + ": ";
            lblSelected.Text = string.Format(GetString("Tools.FileImport.SelectedCount"), filesCount);

            if (!RequestHelper.IsPostBack())
            {
                cultureSelector.Value = LocalizationContext.PreferredCultureCode;

                // Initialize temporary lists
                resultListValues.Clear();
                errorFiles.Clear();
            }

            ltlScript.Text += ScriptHelper.GetScript(@"
function UpdateCount(id, checked) {
    var hidden = document.getElementById('" + hdnSelected.ClientID + @"')
    var label =  document.getElementById('" + lblSelectedValue.ClientID + @"')
    if (hidden.value.indexOf('|' + id + '|') != -1) {
        if (checked == false) {
            hidden.value = hidden.value.replace('|' + id + '|', '');
        }
    } else {
        if (checked == true) {
            hidden.value = hidden.value + '|' + id + '|';
        }
    }
    label.innerHTML = (hidden.value.split('|').length - 1) / 2;
}");
        }
    }


    private void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName.ToLowerCSafe())
        {
            case "prepareforimport":
                Directory.PrepareFilesForImport(rootPath);
                URLHelper.Redirect(RequestContext.CurrentURL);
                break;
        }
    }


    protected void gridImport_OnShowButtonClick(object sender, EventArgs e)
    {
        gridImport.ResetSelection();
        hdnSelected.Value = string.Empty;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        if ((errorFiles != null) && (errorFiles.Count > 0))
        {
            gridImport.SelectedItems = errorFiles;
        }

        gridImport.ReloadData();

        // Disable controls if no files found
        if (DataHelper.DataSourceIsEmpty(gridImport.DataSource))
        {
            btnStartImport.Enabled = false;
            pathElem.Enabled = false;
            pnlImportControls.Enabled = false;
        }
        else
        {
            btnStartImport.Enabled = true;
            pathElem.Enabled = true;
            pnlImportControls.Enabled = true;
            filesCount = ((DataSet)gridImport.DataSource).Tables[0].Rows.Count;
        }

        // Set labels
        string count = gridImport.SelectedItems.Count.ToString();
        hdnValue.Value = count;
        lblSelectedValue.Text = count;
        lblTotal.Text = string.Format(GetString("Tools.FileImport.TotalCount"), filesCount);

        errorFiles.Clear();
    }


    /// <summary>
    /// Gets path from current application settings.
    /// </summary>
    private string GetPath()
    {
        // If import folder for current site is not specified in settings set its path as rootpath
        if (!String.IsNullOrEmpty(SiteContext.CurrentSiteName))
        {
            // Get import folder path from settings
            string path = SettingsKeyInfoProvider.GetValue(SiteContext.CurrentSiteName + ".CMSFileImportFolder").Trim();
            if (!string.IsNullOrEmpty(path))
            {
                rootPath = Path.EnsureBackslashes(path, true);
            }
        }

        // Path starting with local driver letter
        if ((char.IsLetter(rootPath.ToLowerCSafe(), 0)) && (rootPath[1] == ':'))
        {
            if (rootPath[2] != '\\')
            {
                rootPath = rootPath[0] + ":\\" + rootPath.Substring(2, rootPath.Length - 2);
            }
        }
        // Relative path
        else if (!((rootPath[0] == '\\') && (rootPath[1] == '\\')))
        {
            try
            {
                rootPath = HttpContext.Current.Server.MapPath(rootPath);
            }
            catch (Exception ex)
            {
                plcImportContent.Visible = false;
                btnStartImport.Enabled = false;
                ShowError(String.Format(GetString("Tools.FileImport.InvalidFolder"), rootPath), ex.Message, null);
                return null;
            }
        }

        if (!String.IsNullOrEmpty(rootPath))
        {
            rootPath = Path.EnsureEndBackslash(rootPath);
        }

        return rootPath;
    }


    /// <summary>
    /// Removes all N (at the beginning of expression) from where condition
    /// (e.g. "column LIKE N'word'" => "column LIKE 'word'").
    /// </summary>
    /// <param name="where">WHERE condition</param>
    private string RemoveNFromWhereCondition(string where)
    {
        if (!String.IsNullOrEmpty(where))
        {
            // Remove all N (at the beginning of expression) from where condition (e.g. "column LIKE N'word'" => "column LIKE 'word'")
            bool inString = false;
            char prev = ' ';
            for (int i = 0; i < where.Length; i++)
            {
                if (where[i] == '\'')
                {
                    if (!inString && (prev == 'N'))
                    {
                        where = where.Remove(i - 1, 1);
                        where = where.Insert(i - 1, " ");
                    }
                    inString = !inString;
                }
                prev = where[i];
            }
        }

        return where;
    }


    /// <summary>
    /// Renames columns in WHERE condition.
    /// </summary>
    /// <param name="where">WHERE condition</param>
    /// <param name="oldColName">Old col name</param>
    /// <param name="newColName">New col name</param>
    private string RenameColumn(string where, string oldColName, string newColName)
    {
        if (!String.IsNullOrEmpty(where))
        {
            string[] strs = where.Split('\'');
            bool inString = where.StartsWithCSafe("'");
            where = "";
            for (int i = 0; i < strs.Length; i++)
            {
                // Rename/replace column name (avoid string literals)
                if (!inString && !String.IsNullOrEmpty(strs[i]))
                {
                    strs[i] = strs[i].Replace(oldColName, newColName);
                }

                // Create new WHERE condition
                where += (inString ? "'" : "") + strs[i] + (inString ? "'" : "");

                inString = !inString;
            }
        }

        return where;
    }


    /// <summary>
    /// Returns set of files in the file system.
    /// </summary>
    private DataSet GetFileSystemDataSource(string where)
    {
        string whereCond = RemoveNFromWhereCondition(where);
        whereCond = RenameColumn(whereCond, "[FilePath]", "[FileName]");
        fileSystemDataSource.WhereCondition = whereCond;
        fileSystemDataSource.Path = rootPath;

        try
        {
            return (DataSet)fileSystemDataSource.DataSource;
        }
        catch (Exception e)
        {
            ShowError(e.Message);
            return null;
        }
    }


    /// <summary>
    /// File list external databound handler.
    /// </summary>
    protected object gridImport_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        string filePath = ValidationHelper.GetString(parameter, string.Empty);
        switch (sourceName.ToLowerCSafe())
        {
            case "filename":
                if (filePath.StartsWithCSafe(rootPath))
                {
                    return filePath.Substring(rootPath.Length);
                }
                break;

            case "result":
                string result = string.Empty;
                if ((resultListValues != null) && (resultListValues.Count > 0))
                {
                    string[] values = resultListValues.Find(delegate(string[] arr) { return ((arr != null) && (arr[0].EqualsCSafe(filePath, StringComparison.InvariantCultureIgnoreCase))); });
                    if (values != null)
                    {
                        if (ValidationHelper.GetBoolean(values[2], false))
                        {
                            result = UniGridFunctions.ColoredSpanMsg(GetString("Tools.FileImport.Imported"), true);
                        }
                        else
                        {
                            result = UniGridFunctions.ColoredSpanMsg(values[1], false);
                        }
                    }
                    else
                    {
                        result = GetString("Tools.FileImport.Skipped");
                    }
                }
                return result;
        }

        return (parameter != null) ? parameter.ToString() : null;
    }


    /// <summary>
    /// BtnStartImport click event handler.
    /// </summary>
    protected void btnStartImport_Click(Object sender, EventArgs e)
    {
        // Check license limitations
        if (!CheckFilesCount(gridImport.SelectedItems.Count))
        {
            ShowError(string.Format(GetString("Tools.FileImport.MaximumCountExceeded"), allowedFilesCount));
        }
        else
        {
            if (gridImport.SelectedItems.Count > 0)
            {
                string path = pathElem.Value.ToString().Trim();
                if (!String.IsNullOrEmpty(path))
                {
                    // Set visibility of panels
                    pnlLog.Visible = true;
                    pnlContent.Visible = false;

                    CurrentError = string.Empty;
                    CurrentLog.Close();
                    EnsureLog();

                    ctlAsyncLog.Parameter = new[] { gridImport.SelectedItems.ToArray(), path, cultureSelector.Value, MembershipContext.AuthenticatedUser };
                    ctlAsyncLog.RunAsync(Import, WindowsIdentity.GetCurrent());
                }
                else
                {
                    ShowError(GetString("Tools.FileImport.AliasPathNotFound"));
                }
            }
            else
            {
                ShowError(GetString("tools.fileimport.nofilesselected"));
            }
        }
    }


    /// <summary>
    /// Import files.
    /// </summary>
    private void Import(object parameter)
    {
        try
        {
            object[] parameters = (object[])parameter;
            string[] items = (string[])parameters[0];
            CurrentUserInfo currentUser = (CurrentUserInfo)parameters[3];

            if ((items.Length > 0) && (currentUser != null))
            {
                resultListValues.Clear();
                errorFiles.Clear();
                hdnValue.Value = null;
                hdnSelected.Value = null;
                string siteName = SiteContext.CurrentSiteName;
                string targetAliasPath = ValidationHelper.GetString(parameters[1], null);

                bool imported = false; // Flag - true if one file was imported at least
                bool importError = false; // Flag - true when import failed

                TreeProvider tree = new TreeProvider(currentUser);
                TreeNode tn = tree.SelectSingleNode(siteName, targetAliasPath, TreeProvider.ALL_CULTURES, true, null, false);
                if (tn != null)
                {
                    // Check if CMS.File document type exist and check if document contains required columns (FileName, FileAttachment)
                    DataClassInfo fileClassInfo = DataClassInfoProvider.GetDataClassInfo("CMS.File");
                    if (fileClassInfo == null)
                    {
                        AddError(GetString("newfile.classcmsfileismissing"));
                        return;
                    }
                    else
                    {
                        FormInfo fi = new FormInfo(fileClassInfo.ClassFormDefinition);
                        FormFieldInfo fileFfi = null;
                        FormFieldInfo attachFfi = null;
                        if (fi != null)
                        {
                            fileFfi = fi.GetFormField("FileName");
                            attachFfi = fi.GetFormField("FileAttachment");
                        }
                        if ((fi == null) || (fileFfi == null) || (attachFfi == null))
                        {
                            AddError(GetString("newfile.someofrequiredfieldsmissing"));
                            return;
                        }
                    }

                    DataClassInfo dci = DataClassInfoProvider.GetDataClassInfo(tn.NodeClassName);

                    if (dci != null)
                    {
                        // Check if "file" and "folder" are allowed as a child document under selected document type
                        bool fileAllowed = false;
                        bool folderAllowed = false;
                        DataClassInfo folderClassInfo = DataClassInfoProvider.GetDataClassInfo("CMS.Folder");
                        if ((fileClassInfo != null) || (folderClassInfo != null))
                        {
                            string[] paths;
                            foreach (string fullFileName in items)
                            {
                                // Check if the file is located under default root path
                                if (!fullFileName.StartsWithCSafe(rootPath, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    AddError(GetString("Tools.FileImport.NotUnderRootPath"));
                                    return;
                                }

                                paths = fullFileName.Substring(rootPath.Length).Split('\\');
                                // Check file
                                if (paths.Length == 1)
                                {
                                    if (!fileAllowed && (fileClassInfo != null) && !DocumentHelper.IsDocumentTypeAllowed(tn, fileClassInfo.ClassID))
                                    {
                                        AddError(GetString("Tools.FileImport.NotAllowedChildClass"));
                                        return;
                                    }
                                    else
                                    {
                                        fileAllowed = true;
                                    }
                                }

                                // Check folder
                                if (paths.Length > 1)
                                {
                                    if (!folderAllowed && (folderClassInfo != null) && !DocumentHelper.IsDocumentTypeAllowed(tn, folderClassInfo.ClassID))
                                    {
                                        AddError(GetString("Tools.FileImport.FolderNotAllowedChildClass"));
                                        return;
                                    }
                                    else
                                    {
                                        folderAllowed = true;
                                    }
                                }

                                if (fileAllowed && folderAllowed)
                                {
                                    break;
                                }
                            }
                        }

                        // Check if user is allowed to create new file document
                        if (fileAllowed && !currentUser.IsAuthorizedToCreateNewDocument(tn, "CMS.File"))
                        {
                            AddError(GetString("accessdenied.notallowedtocreatedocument"));
                            return;
                        }

                        // Check if user is allowed to create new folder document
                        if (folderAllowed && !currentUser.IsAuthorizedToCreateNewDocument(tn, "CMS.Folder"))
                        {
                            AddError(GetString("accessdenied.notallowedtocreatedocument"));
                            return;
                        }
                    }

                    string cultureCode = ValidationHelper.GetString(parameters[2], "");
                    string[] fileList = new string[1];
                    string[] relativePathList = new string[1];

                    // Begin log
                    AddLog(GetString("tools.fileimport.importingprogress"));

                    string msgImported = GetString("Tools.FileImport.Imported");
                    string msgFailed = GetString("Tools.FileImport.Failed");

                    // Insert files selected in datagrid to list of files to import
                    foreach (string fullFileName in items)
                    {
                        // Import selected files only
                        fileList[0] = fullFileName;
                        relativePathList[0] = fullFileName.Substring(rootPath.Length);

                        // Remove extension if needed
                        if (!chkIncludeExtension.Checked)
                        {
                            relativePathList[0] = Regex.Replace(relativePathList[0], "(.*)\\..*", "$1");
                        }

                        try
                        {
                            FileImport.ImportFiles(siteName, targetAliasPath, cultureCode, fileList, relativePathList, currentUser.UserID, chkDeleteImported.Checked);

                            // Import of a file succeeded, fill the output lists
                            resultListValues.Add(new string[] { fullFileName, msgImported, true.ToString() });

                            imported = true; // One file was imported
                            AddLog(HTMLHelper.HTMLEncode(fullFileName));
                        }
                        catch (Exception ex)
                        {
                            // File import failed
                            errorFiles.Add(fullFileName);
                            importError = true;

                            // Fill the output lists
                            resultListValues.Add(new string[] { fullFileName, msgFailed + " (" + HTMLHelper.HTMLEncode(ex.Message) + ")", false.ToString() });

                            AddError(msgFailed + " (" + HTMLHelper.HTMLEncode(ex.Message) + ")");

                            // Abort importing the rest of files for serious exceptions
                            if (!(ex is UnauthorizedAccessException))
                            {
                                return;
                            }
                        }
                    }
                }
                // Specified alias path not found
                else
                {
                    AddError(GetString("Tools.FileImport.AliasPathNotFound"));
                    return;
                }

                if (filesList.Count > 0)
                {
                    if (!importError)
                    {
                        if (imported)
                        {
                            AddError(GetString("Tools.FileImport.FilesWereImported"));
                            return;
                        }
                    }
                    else
                    {
                        AddError(GetString("Tools.FileImport.FilesNotImported"));
                        return;
                    }
                }
            }
            // No items selected to import
            else
            {
                AddError(GetString("Tools.FileImport.FilesNotImported"));
                return;
            }
        }
        catch (ThreadAbortException ex)
        {
            string state = ValidationHelper.GetString(ex.ExceptionState, string.Empty);
            if (state == CMSThread.ABORT_REASON_STOP)
            {
                // When canceled
            }
            else
            {
                // Log error
                LogExceptionToEventLog(ex);
            }
        }
        catch (Exception ex)
        {
            // Log error
            LogExceptionToEventLog(ex);
        }
    }


    /// <summary>
    /// Checks the total count of files to not exceed the license limitations.
    /// </summary>
    /// <param name="selectedFilesCount">Selected files count</param>
    private bool CheckFilesCount(long selectedFilesCount)
    {
        long currentDocumentsCount = 0;
        DataSet dsDocuments = TreeHelper.GetDocuments(SiteContext.CurrentSiteName, "/%", TreeProvider.ALL_CULTURES, true, null, null, null, TreeProvider.ALL_LEVELS, false, -1, TreeProvider.SELECTNODES_REQUIRED_COLUMNS);
        if (!DataHelper.DataSourceIsEmpty(dsDocuments))
        {
            currentDocumentsCount += DataHelper.GetItemsCount(dsDocuments);
        }
        int versionLimitations = LicenseKeyInfoProvider.VersionLimitations(LicenseContext.CurrentLicenseInfo, FeatureEnum.Documents);
        allowedFilesCount = (versionLimitations - currentDocumentsCount);
        return !((versionLimitations != 0) && (versionLimitations < (currentDocumentsCount + selectedFilesCount)));
    }


    /// <summary>
    /// When exception occurs, log it to event log.
    /// </summary>
    /// <param name="ex">Exception to log</param>
    private void LogExceptionToEventLog(Exception ex)
    {
        EventLogProvider.LogEvent(EventType.ERROR, "Content", "IMPORTFILE", EventLogProvider.GetExceptionLogMessage(ex), RequestContext.RawURL, MembershipContext.AuthenticatedUser.UserID, MembershipContext.AuthenticatedUser.UserName, 0, null, RequestContext.UserHostAddress, SiteContext.CurrentSiteID);

        AddError(GetString("tools.fileimport.failed") + " (" + ex.Message + ")");
    }


    /// <summary>
    /// Adds the script to the output request window.
    /// </summary>
    /// <param name="script">Script to add</param>
    public override void AddScript(string script)
    {
        ltlScript.Text += ScriptHelper.GetScript(script);
    }

    #endregion


    #region "Handling async thread"

    private void ctlAsyncLog_OnCancel(object sender, EventArgs e)
    {
        ctlAsyncLog.Parameter = null;
        HandlePossibleErrors();
    }


    private void ctlAsyncLog_OnRequestLog(object sender, EventArgs e)
    {
        ctlAsyncLog.LogContext = CurrentLog;
    }


    private void ctlAsyncLog_OnError(object sender, EventArgs e)
    {
        HandlePossibleErrors();
    }


    private void ctlAsyncLog_OnFinished(object sender, EventArgs e)
    {
        HandlePossibleErrors();
    }


    /// <summary>
    /// Ensures the logging context.
    /// </summary>
    protected LogContext EnsureLog()
    {
        LogContext log = LogContext.EnsureLog(ctlAsyncLog.ProcessGUID);
        return log;
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
    /// Adds the error to collection of errors.
    /// </summary>
    /// <param name="error">Error message</param>
    protected void AddError(string error)
    {
        AddLog(error);
        if (String.IsNullOrEmpty(CurrentError))
        {
            CurrentError = error;
        }
        else
        {
            CurrentError += "<br />" + error;
        }
    }


    private void HandlePossibleErrors()
    {
        CurrentLog.Close();
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "terminatePendingCallbacks", ScriptHelper.GetScript("var __pendingCallbacks = new Array();"));
        if (!String.IsNullOrEmpty(CurrentError))
        {
            ShowError(CurrentError);
        }
    }

    #endregion
}