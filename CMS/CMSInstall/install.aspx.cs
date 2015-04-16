using System;
using System.Collections;
using System.Net;
using System.Security.Principal;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.CMSImportExport;
using CMS.Core;
using CMS.DataEngine;
using CMS.ExtendedControls;
using CMS.Globalization;
using CMS.Helpers;
using CMS.IO;
using CMS.LicenseProvider;
using CMS.Localization;
using CMS.MacroEngine;
using CMS.Membership;
using CMS.PortalEngine;
using CMS.UIControls;

using MessageTypeEnum = CMS.DataEngine.MessageTypeEnum;
using ProcessStatus = CMS.Base.ProcessStatus;


#region "InstallInfo"

/// <summary>
/// Installation info.
/// </summary>
[Serializable]
public class InstallInfo
{
    #region "Variables"

    public const string SEPARATOR = "<#>";

    private const string LOG = "I" + SEPARATOR + SEPARATOR + SEPARATOR;

    // Deletion log
    private string mInstallLog = LOG;
    private string mScriptsFullPath;
    private string mConnectionString;
    private string mDBSchema;

    #endregion


    #region "Properties"

    /// <summary>
    /// Keep information about installation progress.
    /// </summary>
    public string InstallLog
    {
        get
        {
            return mInstallLog;
        }
        set
        {
            mInstallLog = value;
        }
    }


    /// <summary>
    /// Connection string.
    /// </summary>
    public string ConnectionString
    {
        get
        {
            return mConnectionString;
        }

        set
        {
            mConnectionString = value;
        }
    }


    /// <summary>
    /// Scripts full path.
    /// </summary>
    public string ScriptsFullPath
    {
        get
        {
            return mScriptsFullPath;
        }

        set
        {
            mScriptsFullPath = value;
        }
    }


    /// <summary>
    /// Database schema.
    /// </summary>
    public string DBSchema
    {
        get
        {
            return mDBSchema;
        }
        set
        {
            mDBSchema = value;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Clear log.
    /// </summary>
    public void ClearLog()
    {
        InstallLog = LOG;
    }


    /// <summary>
    /// Gets limited progress log for callback.
    /// </summary>
    /// <param name="reqMessageLength">Requested message part length</param>
    /// <param name="reqErrorLength">Requested error part length</param>
    /// <param name="reqWarningLength">Requested warning part length</param>
    public string GetLimitedProgressLog(int reqMessageLength, int reqErrorLength, int reqWarningLength)
    {
        if (mInstallLog != null)
        {
            string[] parts = mInstallLog.Split(new string[] { SEPARATOR }, StringSplitOptions.None);

            if (parts.Length != 4)
            {
                return "F" + SEPARATOR + "Wrong internal log." + SEPARATOR + SEPARATOR;
            }

            string message = parts[1];
            string error = parts[2];
            string warning = parts[3];

            // Message part
            int messageLength = message.Length;
            if (reqMessageLength > messageLength)
            {
                reqMessageLength = messageLength;
            }

            // Error part
            int errorLength = error.Length;
            if (reqErrorLength > errorLength)
            {
                reqErrorLength = errorLength;
            }

            // Warning part
            int warningLength = warning.Length;
            if (reqWarningLength > warningLength)
            {
                reqWarningLength = warningLength;
            }

            return parts[0] + SEPARATOR + message.Substring(0, messageLength - reqMessageLength) + SEPARATOR + parts[2].Substring(0, errorLength - reqErrorLength) + SEPARATOR + parts[3].Substring(0, warningLength - reqWarningLength);
        }
        return "F" + SEPARATOR + "Internal error." + SEPARATOR + SEPARATOR;
    }

    #endregion
}

#endregion


public partial class CMSInstall_install : CMSPage, ICallbackEventHandler
{
    #region "Constants"

    private const string WWAG_KEY = "CMSWWAGInstallation";
    private const string COLLATION_CASE_INSENSITIVE = "SQL_Latin1_General_CP1_CI_AS";
    /// <summary>
    /// Short link to help topic page.
    /// </summary>
    private const string HELP_TOPIC_LINK = "database_installation_additional";


    /// <summary>
    /// Short link to help topic page regarding disk permissions.
    /// </summary>
    private const string HELP_TOPIC_DISK_PERMISSIONS_LINK = "disk_permission_problems";


    /// <summary>
    /// Short link to help topic page regarding SQL error.
    /// </summary>
    private const string HELP_TOPIC_SQL_ERROR_LINK = HELP_TOPIC_LINK;

    #endregion


    #region "Variables"

    private static readonly Hashtable mInstallInfos = new Hashtable();
    private static readonly Hashtable mManagers = new Hashtable();
    private string hostName = RequestContext.URL.Host.ToLowerCSafe();
    private static bool dbReady = false;
    private static bool writePermissions = true;
    private UserInfo mImportUser = null;
    private LocalizedButton mNextButton;
    private LocalizedButton mPreviousButton;
    private LocalizedButton mStartNextButton;

    #endregion


    #region "Properties"

    /// <summary>
    /// User for actions context
    /// </summary>
    protected UserInfo ImportUser
    {
        get
        {
            if (mImportUser == null)
            {
                mImportUser = UserInfoProvider.GetUserInfo("administrator");
                CMSActionContext.CurrentUser = mImportUser;
            }

            return mImportUser;
        }
    }


    /// <summary>
    /// Database is created.
    /// </summary>
    private bool DBCreated
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["DBCreated"], false);
        }

        set
        {
            ViewState["DBCreated"] = value;
        }
    }


    /// <summary>
    /// Database is installed.
    /// </summary>
    private bool DBInstalled
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["DBInstalled"], false);
        }

        set
        {
            ViewState["DBInstalled"] = value;
        }
    }


    /// <summary>
    /// Process GUID.
    /// </summary>
    public Guid ProcessGUID
    {
        get
        {
            if (ViewState["ProcessGUID"] == null)
            {
                ViewState["ProcessGUID"] = Guid.NewGuid();
            }

            return ValidationHelper.GetGuid(ViewState["ProcessGUID"], Guid.Empty);
        }
    }


    /// <summary>
    /// Install info.
    /// </summary>
    public InstallInfo InstallInfo
    {
        get
        {
            string key = "instInfos_" + ProcessGUID;
            if (mInstallInfos[key] == null)
            {
                InstallInfo instInfo = new InstallInfo();
                mInstallInfos[key] = instInfo;
            }
            return (InstallInfo)mInstallInfos[key];
        }
        set
        {
            string key = "instInfos_" + ProcessGUID;
            mInstallInfos[key] = value;
        }
    }


    /// <summary>
    /// Authentication type.
    /// </summary>
    public SQLServerAuthenticationModeEnum authenticationType
    {
        get
        {
            if (ViewState["authentication"] == null)
            {
                if (RequestHelper.IsPostBack())
                {
                    throw new Exception("Connection information was lost!");
                }
            }
            return (SQLServerAuthenticationModeEnum)ViewState["authentication"];
        }
        set
        {
            ViewState["authentication"] = value;
        }
    }


    /// <summary>
    /// Database name.
    /// </summary>
    public string Database
    {
        get
        {
            return ValidationHelper.GetString(ViewState["Database"], "");
        }
        set
        {
            ViewState["Database"] = value;
        }
    }


    /// <summary>
    /// Import manager.
    /// </summary>
    public ImportManager ImportManager
    {
        get
        {
            string key = "imManagers_" + ProcessGUID;
            if (mManagers[key] == null)
            {
                SiteImportSettings imSettings = new SiteImportSettings(ImportUser);
                imSettings.IsWebTemplate = true;
                imSettings.ImportType = ImportTypeEnum.AllNonConflicting;
                imSettings.CopyFiles = false;
                imSettings.EnableSearchTasks = false;
                ImportManager im = new ImportManager(imSettings);
                mManagers[key] = im;
            }
            return (ImportManager)mManagers[key];
        }
        set
        {
            string key = "imManagers_" + ProcessGUID;
            mManagers[key] = value;
        }
    }


    /// <summary>
    /// New site domain.
    /// </summary>
    public string Domain
    {
        get
        {
            return ValidationHelper.GetString(ViewState["Domain"], "");
        }

        set
        {
            ViewState["Domain"] = value;
        }
    }


    /// <summary>
    /// New site site name.
    /// </summary>
    public string SiteName
    {
        get
        {
            return ValidationHelper.GetString(ViewState["SiteName"], "");
        }

        set
        {
            ViewState["SiteName"] = value;
        }
    }


    /// <summary>
    /// Connection string.
    /// </summary>
    public string ConnectionString
    {
        get
        {
            if (ViewState["connString"] == null)
            {
                ViewState["connString"] = "";
            }
            return (string)ViewState["connString"];
        }

        set
        {
            ViewState["connString"] = value;
        }
    }


    /// <summary>
    /// Step index.
    /// </summary>
    public int StepIndex
    {
        get
        {
            if (ViewState["stepIndex"] == null)
            {
                ViewState["stepIndex"] = 1;
            }
            return (int)ViewState["stepIndex"];
        }

        set
        {
            ViewState["stepIndex"] = value;
        }
    }


    private string mResult
    {
        get
        {
            if (ViewState["result"] == null)
            {
                if (RequestHelper.IsPostBack())
                {
                    throw new Exception("Information was lost!");
                }
            }
            return (string)ViewState["result"];
        }
        set
        {
            ViewState["result"] = value;
        }
    }


    private bool mDisplayLog
    {
        get
        {
            if (ViewState["displLog"] == null)
            {
                if (RequestHelper.IsPostBack())
                {
                    throw new Exception("Information was lost!");
                }
                return false;
            }
            return (bool)ViewState["displLog"];
        }
        set
        {
            ViewState["displLog"] = value;
        }
    }


    /// <summary>
    /// Flag - indicate whether DB objects will be created.
    /// </summary>
    private bool CreateDBObjects
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["CreateDBObjects"], true);
        }
        set
        {
            ViewState["CreateDBObjects"] = value;
        }
    }


    /// <summary>
    /// Help control displayed on the navigation for the first step.
    /// </summary>
    protected HelpControl StartHelp
    {
        get
        {
            Control startStepNavigation = wzdInstaller.FindControl("StartNavigationTemplateContainerID$startStepNavigation");
            return (HelpControl)startStepNavigation.FindControl("hlpContext");
        }
    }


    /// <summary>
    /// Help control displayed on the navigation for all remaining steps.
    /// </summary>
    protected HelpControl Help
    {
        get
        {
            Control stepNavigation = wzdInstaller.FindControl("StepNavigationTemplateContainerID$stepNavigation");
            return (HelpControl)stepNavigation.FindControl("hlpContext");
        }
    }


    /// <summary>
    /// Previous step index.
    /// </summary>
    private int PreviousStep
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["PreviousStep"], 0);
        }
        set
        {
            ViewState["PreviousStep"] = value;
        }
    }

    /// <summary>
    /// Current step index.
    /// </summary>
    private int ActualStep
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["ActualStep"], 0);
        }
        set
        {
            ViewState["ActualStep"] = value;
        }
    }


    private int StepOperation
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["StepOperation"], 0);
        }
        set
        {
            ViewState["StepOperation"] = value;
        }
    }


    /// <summary>
    ///  User password.
    /// </summary>
    private string Password
    {
        get
        {
            return Convert.ToString(ViewState["install.password"]);
        }
        set
        {
            ViewState["install.password"] = value;
        }
    }

    #endregion


    #region "Step wizard buttons"

    /// <summary>
    /// Previous button.
    /// </summary>
    public LocalizedButton PreviousButton
    {
        get
        {
            return mPreviousButton ?? (mPreviousButton = wzdInstaller.FindControl("StepNavigationTemplateContainerID").FindControl("stepNavigation").FindControl("StepPrevButton") as LocalizedButton);
        }
    }


    /// <summary>
    /// Next button.
    /// </summary>
    public LocalizedButton NextButton
    {
        get
        {
            return mNextButton ?? (mNextButton = wzdInstaller.FindControl("StepNavigationTemplateContainerID").FindControl("stepNavigation").FindControl("StepNextButton") as LocalizedButton);
        }
    }


    /// <summary>
    /// Next button.
    /// </summary>
    public LocalizedButton StartNextButton
    {
        get
        {
            return mStartNextButton ?? (mStartNextButton = wzdInstaller.FindControl("StartNavigationTemplateContainerID").FindControl("startStepNavigation").FindControl("StepNextButton") as LocalizedButton);
        }
    }

    #endregion


    protected void Page_Load(Object sender, EventArgs e)
    {
        // Disable CSS minification
        CSSHelper.MinifyCurrentRequest = false;
        ScriptHelper.MinifyCurrentRequestScripts = false;

        SetBrowserClass(false);

        if (!RequestHelper.IsCallback())
        {
            EnsureApplicationConfiguration();

            ucAsyncControl.OnFinished += worker_OnFinished;
            ucDBAsyncControl.OnFinished += workerDB_OnFinished;
            databaseDialog.ServerName = userServer.ServerName;

            // Register script for pendingCallbacks repair
            // Cannot use ScriptHelper.FixPendingCallbacks as during installation the DB is not available
            ScriptManager.RegisterClientScriptInclude(this, GetType(), "cms.js", URLHelper.ResolveUrl("~/CMSScripts/cms.js"));
            ScriptManager.RegisterClientScriptBlock(this, GetType(), "fixPendingCallbacks", "WebForm_CallbackComplete = WebForm_CallbackComplete_SyncFixed", true);

            // Javascript functions
            string jsFunctions =
                "var iMessageText = '';\n" +
                "var iErrorText = '';\n" +
                "var iWarningText = '';\n" +
                "var getBusy = false; \n" +
                "function GetInstallState(argument)\n" +
                "{ if (getBusy) return; getBusy = true; setTimeout(\"getBusy = false;\", 2000); if(window.Activity){window.Activity();} var arg = argument + ';' + iMessageText.length + ';' + iErrorText.length + ';' + iWarningText.length; return " + Page.ClientScript.GetCallbackEventReference(this, "arg", "SetInstallStateMssg", "arg", true) + " } \n";

            jsFunctions +=
                "function SetInstallStateMssg(rValue, context)\n" +
                "{\n" +
                "   getBusy = false; \n" +
                "   if (rValue != '')\n" +
                "   {\n" +
                "       var args = context.split(';');\n" +
                "       var values = rValue.split('" + AbstractImportExportSettings.SEPARATOR + "');\n" +
                "       var messageElement = document.getElementById('lblProgress');\n" +
                "       var errorElement = document.getElementById('" + errorPanel.ErrorLabelClientID + "');\n" +
                "       var warningElement = document.getElementById('" + warningPanel.WarningLabelClientID + "');\n" +
                "       var messageText = iMessageText;\n" +
                "       messageText = values[1] + messageText.substring(messageText.length - args[2]);\n" +
                "       if(messageText.length > iMessageText.length){ iMessageText = messageElement.innerHTML = messageText; }\n" +
                "       var errorText = iErrorText;\n" +
                "       errorText = values[2] + errorText.substring(errorText.length - args[3]);\n" +
                "       if(errorText.length > iErrorText.length){ iErrorText = errorElement.innerHTML = errorText; errorElement.className=''; errorElement.className='ErrorLabel'; }\n" +
                "       var warningText = iWarningText;\n" +
                "       warningText = values[3] + warningText.substring(warningText.length - args[4]);\n" +
                "       if(warningText.length > iWarningText.length){ iWarningText = warningElement.innerHTML = warningText; warningElement.className=''; warningElement.className='ErrorLabel'; }\n" +
                "       if((values == '') || (values[0] == 'F'))\n" +
                "       {\n" +
                "           StopInstallStateTimer();\n" +
                "           if(values[2] != '')\n" +
                "           {\n" +
                "               BTN_Disable('" + NextButton.ClientID + "');\n" +
                "               BTN_Enable('" + PreviousButton.ClientID + "');\n" +
                "           }\n" +
                "           else\n" +
                "           {\n" +
                "               BTN_Disable('" + NextButton.ClientID + "');\n" +
                "               BTN_Disable('" + PreviousButton.ClientID + "');\n" +
                "           }\n" +
                "       }\n" +
                "   }\n" +
                "}\n";

            // JS for advanced options link
            jsFunctions += "function ShowHideElement(elemid, show) { \n" +
                           " var elem = document.getElementById(elemid); \n" +
                           " if (elem) { \n" +
                           "   if (show=='1') { elem.style.display = ''; } else { elem.style.display = 'none'; } \n" +
                           " } \n" +
                           " } \n" +
                           " function AdvancedOptions(state) { \n" +
                           "   var elem = document.getElementById('" + hdnAdvanced.ClientID + "'); \n" +
                           "   if (elem) { \n " +
                           "      if (state=='1' || state=='?' && (elem.value == '' || elem.value == '0')) { elem.value = '1'; } else { elem.value = '0'; } \n" +
                           "       ShowHideElement('" + databaseDialog.SchemaClientID + "', elem.value); ShowHideElement('" + databaseDialog.SchemaLabelClientID + "', elem.value); \n" +
                           "       var label = document.getElementById('" + databaseDialog.AdvancedLabelClientID + "'); \n" +
                           "       if (label) { " +
                           "         if (elem.value == '1') { label.innerHTML = " + ScriptHelper.GetString(ResHelper.GetFileString("install.HideAdvancedOptions")) + "; } \n" +
                           "         else { label.innerHTML = " + ScriptHelper.GetString(ResHelper.GetFileString("install.ShowAdvancedOptions")) + "; } \n" +
                           "       } \n" +
                           "   } \n " +
                           " } \n"
                ;


            // Register the script to perform get flags for showing buttons retrieval callback
            ScriptHelper.RegisterClientScriptBlock(this, GetType(), "InstallFunctions", ScriptHelper.GetScript(jsFunctions));

            StartHelp.Tooltip = ResHelper.GetFileString("install.tooltip");
            StartHelp.TopicName = HELP_TOPIC_LINK;
            StartHelp.IconCssClass = "cms-icon-80";

            Response.Cache.SetNoStore();

            Help.Tooltip = ResHelper.GetFileString("install.tooltip");
            Help.IconCssClass = "cms-icon-80";


            btnPermissionTest.Click += btnPermissionTest_Click;
            btnPermissionSkip.Click += btnPermissionSkip_Click;
            btnPermissionContinue.Click += btnPermissionContinue_Click;

            // If the connection string is set, redirect
            if (!RequestHelper.IsPostBack())
            {
                if (ConnectionHelper.ConnectionAvailable)
                {
                    URLHelper.Redirect("~/default.aspx");
                }

                bool checkPermission = QueryHelper.GetBoolean("checkpermission", true);
                bool testAgain = QueryHelper.GetBoolean("testagain", false);

                string dir = HttpContext.Current.Server.MapPath("~/");

                // Do not test write permissions in WWAG mode
                if (!ValidationHelper.GetBoolean(SettingsHelper.AppSettings[WWAG_KEY], false))
                {
                    if (!DirectoryHelper.CheckPermissions(dir) && checkPermission)
                    {
                        writePermissions = false;
                        pnlWizard.Visible = false;
                        pnlHeaderImages.Visible = false;
                        pnlPermission.Visible = true;
                        pnlButtons.Visible = true;

                        lblPermission.Text = string.Format(ResHelper.GetFileString("Install.lblPermission"), WindowsIdentity.GetCurrent().Name, dir);
                        btnPermissionSkip.Text = ResHelper.GetFileString("Install.btnPermissionSkip");
                        btnPermissionTest.Text = ResHelper.GetFileString("Install.btnPermissionTest");

                        // Show troubleshoot link
                        errorPanel.DisplayError("Install.ErrorPermissions", HELP_TOPIC_DISK_PERMISSIONS_LINK);
                        return;
                    }

                    if (testAgain)
                    {
                        pnlWizard.Visible = false;
                        pnlHeaderImages.Visible = false;
                        pnlPermission.Visible = false;
                        pnlButtons.Visible = false;
                        pnlPermissionSuccess.Visible = true;
                        lblPermissionSuccess.Text = ResHelper.GetFileString("Install.lblPermissionSuccess");
                        btnPermissionContinue.Text = ResHelper.GetFileString("Install.btnPermissionContinue");
                        writePermissions = true;
                        return;
                    }
                }
            }

            pnlWizard.Visible = true;
            pnlPermission.Visible = false;
            pnlButtons.Visible = false;

            if (!RequestHelper.IsPostBack())
            {
                if ((HttpContext.Current != null) && !ValidationHelper.GetBoolean(SettingsHelper.AppSettings["CMSWWAGInstallation"], false))
                {
                    userServer.ServerName = SystemContext.MachineName;
                }
                authenticationType = SQLServerAuthenticationModeEnum.SQLServerAuthentication;

                wzdInstaller.ActiveStepIndex = 0;
            }
            else
            {
                if (Password == null)
                {
                    Password = userServer.DBPassword;
                }
            }

            // Load the strings
            mDisplayLog = false;


            ltlAdvanced.Text = ScriptHelper.GetScript(" AdvancedOptions('" + (hdnAdvanced.Value == "1" ? "1" : "0") + "'); ");

            lblCompleted.Text = ResHelper.GetFileString("Install.DBSetupOK");
            lblMediumTrustInfo.Text = ResHelper.GetFileString("Install.MediumTrustInfo");

            ltlScript.Text = ScriptHelper.GetScript(
                "function NextStep(btnNext,elementDiv)\n" +
                "{\n" +
                "   btnNext.disabled=true;\n" +
                "   try{BTN_Disable('" + PreviousButton.ClientID + "');}catch(err){}\n" +
                ClientScript.GetPostBackEventReference(btnHiddenNext, null) +
                "}\n" +
                "function PrevStep(btnPrev,elementDiv)\n" +
                "{" +
                "   btnPrev.disabled=true;\n" +
                "   try{BTN_Disable('" + NextButton.ClientID + "');}catch(err){}\n" +
                ClientScript.GetPostBackEventReference(btnHiddenBack, null) +
                "}\n"
                );
            mResult = "";

            // Sets connection string panel
            lblConnectionString.Text = ResHelper.GetFileString("Install.lblConnectionString");
            wzdInstaller.StartNextButtonText = ResHelper.GetFileString("general.next") + " >";
            wzdInstaller.FinishCompleteButtonText = ResHelper.GetFileString("Install.Finish");
            wzdInstaller.FinishPreviousButtonText = ResHelper.GetFileString("Install.BackStep");
            wzdInstaller.StepNextButtonText = ResHelper.GetFileString("general.next") + " >";
            wzdInstaller.StepPreviousButtonText = ResHelper.GetFileString("Install.BackStep");

            // Show WWAG dialog instead of license dialog (if running in WWAG mode)
            if (ValidationHelper.GetBoolean(SettingsHelper.AppSettings[WWAG_KEY], false))
            {
                ucLicenseDialog.Visible = false;
                ucWagDialog.Visible = true;
            }
        }

        // Set the active step as 1 if connection string already initialized
        if (!RequestHelper.IsPostBack() && ConnectionHelper.IsConnectionStringInitialized)
        {
            wzdInstaller.ActiveStepIndex = 1;
            databaseDialog.UseExistingChecked = true;
        }

        NextButton.Attributes.Remove("disabled");
        PreviousButton.Attributes.Remove("disabled");

        wzdInstaller.ActiveStepChanged += wzdInstaller_ActiveStepChanged;
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (dbReady || ConnectionHelper.ConnectionAvailable)
        {
            ucSiteCreationDialog.StopProcessing = false;
            ucSiteCreationDialog.ReloadData();
        }

        // Display the log if result filled
        if (mDisplayLog)
        {
            logPanel.DisplayLog(mResult);
        }

        InitializeHeader(wzdInstaller.ActiveStepIndex);
        EnsureDefaultButton();

        PreviousButton.Visible = !ConnectionHelper.IsConnectionStringInitialized && (wzdInstaller.ActiveStepIndex != 0) && (wzdInstaller.ActiveStepIndex != 4) ||
            (wzdInstaller.ActiveStepIndex == 6);
    }


    private void wzdInstaller_ActiveStepChanged(object sender, EventArgs e)
    {
        switch (wzdInstaller.ActiveStepIndex)
        {
            case 1:
                break;
            // Finish step
            case 7:
                // Set current user default culture of the site
                LocalizationContext.PreferredCultureCode = SettingsKeyInfoProvider.GetValue(SiteName + ".CMSDefaultCultureCode");

                // Ensure virtual path provider registration if enabled
                VirtualPathHelper.RegisterVirtualPathProvider();

                // Check whether virtual path provider is running
                if (!VirtualPathHelper.UsingVirtualPathProvider)
                {
                    btnWebSite.Text = ResHelper.GetFileString("Install.lnkMediumTrust");
                    lblMediumTrustInfo.Visible = true;
                }
                else
                {
                    btnWebSite.Text = ResHelper.GetFileString("Install.lnkWebsite");
                }
                break;
        }
    }


    private void btnPermissionContinue_Click(object sender, EventArgs e)
    {
        URLHelper.Redirect(RequestContext.URL.GetLeftPart(UriPartial.Path));
    }


    private void btnPermissionSkip_Click(object sender, EventArgs e)
    {
        URLHelper.Redirect(RequestContext.URL.GetLeftPart(UriPartial.Path) + "?checkpermission=0");
    }


    private void btnPermissionTest_Click(object sender, EventArgs e)
    {
        URLHelper.Redirect(RequestContext.URL.GetLeftPart(UriPartial.Path) + "?testagain=1");
    }


    protected void btnWebSite_onClick(object sender, EventArgs e)
    {
        if (!VirtualPathHelper.UsingVirtualPathProvider)
        {
            AuthenticationHelper.AuthenticateUser("administrator", false);
            URLHelper.Redirect(UIContextHelper.GetApplicationUrl("cms", "administration"));
        }
        else
        {
            URLHelper.Redirect(ResolveUrl("~/default.aspx"));
        }
    }


    protected void btnHiddenBack_onClick(object sender, EventArgs e)
    {
        StepOperation = -1;
        if ((wzdInstaller.ActiveStepIndex == 8) || (wzdInstaller.ActiveStepIndex == 3))
        {
            StepIndex = 2;
            wzdInstaller.ActiveStepIndex = 1;
        }
        else
        {
            StepIndex--;
            wzdInstaller.ActiveStepIndex--;
        }
    }


    protected void btnHiddenNext_onClick(object sender, EventArgs e)
    {
        StepOperation = 1;
        StepIndex++;

        switch (wzdInstaller.ActiveStepIndex)
        {
            case 0:
                Password = userServer.DBPassword;

                // Set the authentication type
                authenticationType = userServer.WindowsAuthenticationChecked ? SQLServerAuthenticationModeEnum.WindowsAuthentication : SQLServerAuthenticationModeEnum.SQLServerAuthentication;

                // Check the server name
                if (userServer.ServerName == String.Empty)
                {
                    HandleError(ResHelper.GetFileString("Install.ErrorServerEmpty"));
                    return;
                }

                // Check if it is possible to connect to the database
                string res = ConnectionHelper.TestConnection(authenticationType, userServer.ServerName, String.Empty, userServer.DBUsername, Password);
                if (!string.IsNullOrEmpty(res))
                {
                    HandleError(res, "Install.ErrorSqlTroubleshoot", HELP_TOPIC_SQL_ERROR_LINK);
                    return;
                }

                // Set credentials for the next step
                databaseDialog.AuthenticationType = authenticationType;
                databaseDialog.Password = Password;
                databaseDialog.Username = userServer.DBUsername;
                databaseDialog.ServerName = userServer.ServerName;

                // Move to the next step
                wzdInstaller.ActiveStepIndex = 1;
                break;

            case 1:
            case 8:
                // Get database name
                Database = TextHelper.LimitLength((databaseDialog.CreateNewChecked ? databaseDialog.NewDatabaseName : databaseDialog.ExistingDatabaseName), 100);

                if (string.IsNullOrEmpty(Database))
                {
                    HandleError(ResHelper.GetFileString("Install.ErrorDBNameEmpty"));
                    return;
                }

                // Set up the connection string
                if (ConnectionHelper.IsConnectionStringInitialized)
                {
                    ConnectionString = ConnectionHelper.ConnectionString;
                }
                else
                {
                    ConnectionString = ConnectionHelper.GetConnectionString(authenticationType, userServer.ServerName, Database, userServer.DBUsername, Password, SqlInstallationHelper.DB_CONNECTION_TIMEOUT, false);
                }

                // Check if existing DB has the same version as currently installed CMS
                if (databaseDialog.UseExistingChecked && !databaseDialog.CreateDatabaseObjects)
                {
                    string dbVersion = null;
                    try
                    {
                        dbVersion = SqlInstallationHelper.GetDatabaseVersion(ConnectionString);
                    }
                    catch
                    {
                    }

                    if (String.IsNullOrEmpty(dbVersion))
                    {
                        // Unable to get DB version => DB objects missing
                        HandleError(ResHelper.GetFileString("Install.DBObjectsMissing"));
                        return;
                    }

                    if (dbVersion != CMSVersion.MainVersion)
                    {
                        // Get wrong version number
                        HandleError(ResHelper.GetFileString("Install.WrongDBVersion"));
                        return;
                    }
                }

                // Set DB schema
                string dbSchema = null;
                if (hdnAdvanced.Value == "1")
                {
                    dbSchema = databaseDialog.SchemaText;
                }

                InstallInfo.DBSchema = dbSchema;

                // Use existing database
                if (databaseDialog.UseExistingChecked)
                {
                    // Check if DB exists
                    if (!DatabaseHelper.DatabaseExists(ConnectionString))
                    {
                        HandleError(string.Format(ResHelper.GetFileString("Install.ErrorDatabseDoesntExist"), Database));
                        return;
                    }

                    // Check if DB schema exists
                    if (!SqlInstallationHelper.CheckIfSchemaExist(ConnectionString, dbSchema))
                    {
                        HandleError(string.Format(ResHelper.GetFileString("Install.ErrorDatabseSchemaDoesnotExist"), dbSchema, SqlInstallationHelper.GetCurrentDefaultSchema(ConnectionString)));
                        return;
                    }

                    // Get collation of existing DB
                    string collation = DatabaseHelper.GetDatabaseCollation(ConnectionString);
                    string dbCollation = collation;
                    DatabaseHelper.DatabaseCollation = collation;

                    if (wzdInstaller.ActiveStepIndex != 8)
                    {
                        // Check target database collation (ask the user if it is not fully supported)
                        if (CMSString.Compare(dbCollation, COLLATION_CASE_INSENSITIVE, true) != 0)
                        {
                            lblCollation.Text = ResHelper.GetFileString("install.databasecollation");
                            rbLeaveCollation.Text = string.Format(ResHelper.GetFileString("install.leavecollation"), collation);
                            rbChangeCollationCI.Text = string.Format(ResHelper.GetFileString("install.changecollation"), COLLATION_CASE_INSENSITIVE);
                            wzdInstaller.ActiveStepIndex = 8;
                            return;
                        }
                    }
                    else
                    {
                        // Change database collation
                        if (!rbLeaveCollation.Checked)
                        {
                            if (rbChangeCollationCI.Checked)
                            {
                                DatabaseHelper.ChangeDatabaseCollation(ConnectionString, Database, COLLATION_CASE_INSENSITIVE);
                            }
                        }
                    }
                }
                else
                {
                    // Create a new database
                    if (!CreateDatabase(null))
                    {
                        HandleError(string.Format(ResHelper.GetFileString("Install.ErrorCreateDB"), databaseDialog.NewDatabaseName));
                        return;
                    }

                    databaseDialog.ExistingDatabaseName = databaseDialog.NewDatabaseName;
                    databaseDialog.CreateNewChecked = false;
                    databaseDialog.UseExistingChecked = true;
                }

                if ((!SystemContext.IsRunningOnAzure && writePermissions) || ConnectionHelper.IsConnectionStringInitialized)
                {
                    if (databaseDialog.CreateDatabaseObjects)
                    {
                        if (DBInstalled && DBCreated)
                        {
                            ucDBAsyncControl.RaiseFinished(this, EventArgs.Empty);
                        }
                        else
                        {
                            // Run SQL installation
                            RunSQLInstallation(dbSchema);
                        }
                    }
                    else
                    {
                        CreateDBObjects = false;

                        // Set connection string
                        if (SettingsHelper.SetConnectionString(ConnectionHelper.ConnectionStringName, ConnectionString))
                        {
                            // Set the application connection string
                            SetAppConnectionString();

                            // Check if license key for current domain is present
                            LicenseKeyInfo lki = LicenseKeyInfoProvider.GetLicenseKeyInfo(hostName);
                            wzdInstaller.ActiveStepIndex = (lki == null) ? 4 : 5;
                            ucLicenseDialog.SetLicenseExpired();
                        }
                        else
                        {
                            ManualConnectionStringInsertion();
                        }
                    }
                }
                else
                {
                    ManualConnectionStringInsertion();
                }

                break;

            // After connection string save error
            case 2:
                // If connection strings don't match
                if ((SettingsHelper.ConnectionStrings[ConnectionHelper.ConnectionStringName] == null) ||
                    (SettingsHelper.ConnectionStrings[ConnectionHelper.ConnectionStringName].ConnectionString == null) ||
                    (SettingsHelper.ConnectionStrings[ConnectionHelper.ConnectionStringName].ConnectionString.Trim() == "") ||
                    (SettingsHelper.ConnectionStrings[ConnectionHelper.ConnectionStringName].ConnectionString != ConnectionString))
                {
                    HandleError(ResHelper.GetFileString("Install.ErrorAddConnString"));
                    return;
                }

                if (CreateDBObjects)
                {
                    if (DBInstalled)
                    {
                        SetAppConnectionString();

                        // Continue with next step
                        CheckLicense();

                        RequestMetaFile();
                    }
                    else
                    {
                        // Run SQL installation
                        RunSQLInstallation(null);
                    }
                }
                else
                {
                    // If this is installation to existing DB and objects are not created
                    if ((hostName != "localhost") && (hostName != "127.0.0.1"))
                    {
                        wzdInstaller.ActiveStepIndex = 4;
                    }
                    else
                    {
                        wzdInstaller.ActiveStepIndex = 5;
                    }
                }
                break;

            // After DB install
            case 3:
                break;

            // After license entering
            case 4:
                try
                {
                    if (ucLicenseDialog.Visible)
                    {
                        ucLicenseDialog.SetLicenseKey();
                        wzdInstaller.ActiveStepIndex = 5;
                    }
                    else if (ucWagDialog.ProcessRegistration(ConnectionString))
                    {
                        wzdInstaller.ActiveStepIndex = 5;
                    }
                }
                catch (Exception ex)
                {
                    HandleError(ex.Message);
                }
                break;

            // Site creation
            case 5:
                switch (ucSiteCreationDialog.CreationType)
                {
                    case CMSInstall_Controls_WizardSteps_SiteCreationDialog.CreationTypeEnum.Template:
                        {
                            if (ucSiteCreationDialog.TemplateName == "")
                            {
                                HandleError(ResHelper.GetFileString("install.notemplate"));
                                return;
                            }

                            // Settings preparation
                            SiteImportSettings settings = new SiteImportSettings(ImportUser);
                            settings.IsWebTemplate = true;
                            settings.ImportType = ImportTypeEnum.AllNonConflicting;
                            settings.CopyFiles = false;
                            settings.EnableSearchTasks = false;
                            settings.CreateVersion = false;

                            if (HttpContext.Current != null)
                            {
                                const string www = "www.";
                                if (hostName.StartsWithCSafe(www))
                                {
                                    hostName = hostName.Remove(0, www.Length);
                                }

                                if (!RequestContext.URL.IsDefaultPort)
                                {
                                    hostName += ":" + RequestContext.URL.Port;
                                }

                                settings.SiteDomain = hostName;
                                Domain = hostName;
                            }

                            // Create site
                            WebTemplateInfo ti = WebTemplateInfoProvider.GetWebTemplateInfo(ucSiteCreationDialog.TemplateName);
                            if (ti == null)
                            {
                                HandleError("[Install]: Template not found.");
                                return;
                            }

                            settings.SiteName = ti.WebTemplateName;
                            settings.SiteDisplayName = ti.WebTemplateDisplayName;

                            if (HttpContext.Current != null)
                            {
                                string path = HttpContext.Current.Server.MapPath(ti.WebTemplateFileName);
                                if (File.Exists(path + "\\template.zip"))
                                {                                    // Template from zip file
                                    path += "\\" + ZipStorageProvider.GetZipFileName("template.zip");
                                    settings.TemporaryFilesPath = path;
                                    settings.SourceFilePath = path;
                                    settings.TemporaryFilesCreated = true;
                                }
                                else
                                {
                                    settings.SourceFilePath = path;
                                }
                                settings.WebsitePath = HttpContext.Current.Server.MapPath("~/");
                            }

                            settings.SetSettings(ImportExportHelper.SETTINGS_DELETE_SITE, true);
                            settings.SetSettings(ImportExportHelper.SETTINGS_DELETE_TEMPORARY_FILES, false);

                            SiteName = settings.SiteName;

                            // Init the Mimetype helper (required for the Import)
                            MimeTypeHelper.LoadMimeTypes();

                            // Import the site asynchronously
                            ImportManager.Settings = settings;

                            ucAsyncControl.RunAsync(ImportManager.Import, WindowsIdentity.GetCurrent());
                            NextButton.Attributes.Add("disabled", "true");
                            PreviousButton.Attributes.Add("disabled", "true");
                            wzdInstaller.ActiveStepIndex = 6;

                            ltlInstallScript.Text = ScriptHelper.GetScript("StartInstallStateTimer('IM');");
                        }
                        break;

                    // Else redirect to the site import
                    case CMSInstall_Controls_WizardSteps_SiteCreationDialog.CreationTypeEnum.ExistingSite:
                        {
                            AuthenticationHelper.AuthenticateUser("administrator", false);
                            URLHelper.Redirect(UIContextHelper.GetApplicationUrl("cms", "sites", "action=import"));
                        }
                        break;

                    // Else redirect to the new site wizard
                    case CMSInstall_Controls_WizardSteps_SiteCreationDialog.CreationTypeEnum.NewSiteWizard:
                        {
                            AuthenticationHelper.AuthenticateUser("administrator", false);
                            URLHelper.Redirect(UIContextHelper.GetApplicationUrl("cms", "sites", "action=new"));
                        }
                        break;
                }
                break;

            default:
                wzdInstaller.ActiveStepIndex++;
                break;
        }
    }


    /// <summary>
    /// Runs SQL installation scripts
    /// </summary>
    /// <param name="dbSchema">Database schema</param>
    private void RunSQLInstallation(string dbSchema)
    {
        // Setup the installation
        InstallInfo.ScriptsFullPath = SqlInstallationHelper.GetSQLInstallPath();
        InstallInfo.ConnectionString = ConnectionString;

        if (dbSchema != null)
        {
            InstallInfo.DBSchema = dbSchema;
        }

        InstallInfo.ClearLog();

        // Start the installation process
        ucDBAsyncControl.RunAsync(InstallDatabase, WindowsIdentity.GetCurrent());

        NextButton.Attributes.Add("disabled", "true");
        PreviousButton.Attributes.Add("disabled", "true");
        wzdInstaller.ActiveStepIndex = 3;

        ltlInstallScript.Text = ScriptHelper.GetScript("StartInstallStateTimer('DB');");
    }


    private void worker_OnFinished(object sender, EventArgs e)
    {
        DBCreated = true;

        // If the import finished without error
        if ((ImportManager.ImportStatus != ProcessStatus.Error) && (ImportManager.ImportStatus != ProcessStatus.Restarted))
        {
            wzdInstaller.ActiveStepIndex = 7;
        }
        else
        {
            string log = ImportManager.Settings.ProgressLog;
            string[] messages = log.Split(new[] { InstallInfo.SEPARATOR }, StringSplitOptions.None);
            errorPanel.ErrorLabelText = messages[2];
            siteProgress.ProgressText = messages[1];
            NextButton.Enabled = false;
        }
    }


    private void workerDB_OnFinished(object sender, EventArgs e)
    {
        CreateDBObjects = databaseDialog.CreateDatabaseObjects;

        DBInstalled = true;

        // Try to set connection string into db only if not running on Azure
        bool setConnectionString = !SystemContext.IsRunningOnAzure && writePermissions;

        // Connection string could not be saved to web.config
        if (!ConnectionHelper.IsConnectionStringInitialized && (!setConnectionString || !SettingsHelper.SetConnectionString(ConnectionHelper.ConnectionStringName, ConnectionString)))
        {
            ManualConnectionStringInsertion();
            return;
        }

        SetAppConnectionString();

        // Recalculate time zone daylight saving start and end.
        TimeZoneInfoProvider.GenerateTimeZoneRules();

        CheckLicense();

        RequestMetaFile();
    }


    private void RequestMetaFile()
    {
        // Request meta file
        try
        {
            WebClient client = new WebClient();
            string url = RequestContext.CurrentScheme + "://" + RequestContext.URL.Host + SystemContext.ApplicationPath.TrimEnd('/') + "/CMSPages/GetMetaFile.aspx";
            client.DownloadData(url);
            client.Dispose();
        }
        catch
        {
        }
    }


    /// <summary>
    /// Check if license for current domain is valid. Try to add trial license if possible.
    /// </summary>
    private void CheckLicense()
    {
        // Add license keys
        bool licensesAdded = true;

        // Try to add trial license
        if (CreateDBObjects && (ucSiteCreationDialog.CreationType != CMSInstall_Controls_WizardSteps_SiteCreationDialog.CreationTypeEnum.ExistingSite))
        {
            licensesAdded = AddTrialLicenseKeys(ConnectionString);
        }

        if (licensesAdded)
        {
            if ((hostName != "localhost") && (hostName != "127.0.0.1"))
            {
                // Check if license key for current domain is present
                LicenseKeyInfo lki = LicenseKeyInfoProvider.GetLicenseKeyInfo(hostName);
                wzdInstaller.ActiveStepIndex = (lki == null) ? 4 : 5;
            }
            else
            {
                wzdInstaller.ActiveStepIndex = 5;
            }
        }
        else
        {
            wzdInstaller.ActiveStepIndex = 4;
            ucLicenseDialog.SetLicenseExpired();
        }
    }


    /// <summary>
    /// Sets step, that prompts user to enter connection string manually to web.config. ConnectionString is built inside the method.
    /// </summary>
    private void ManualConnectionStringInsertion()
    {
        string connectionString = ConnectionHelper.GetConnectionString(authenticationType, userServer.ServerName, Database, userServer.DBUsername, Password, SqlInstallationHelper.DB_CONNECTION_TIMEOUT, true, azure: SystemContext.IsRunningOnAzure);

        // Set error message
        string connectionStringEntry = "&lt;add name=\"CMSConnectionString\" connectionString=\"" + connectionString + "\"/&gt;";
        string applicationSettingsEntry = "&lt;Setting name=\"CMSConnectionString\" value=\"" + connectionString + "\"/&gt;";

        string errorMessage = SystemContext.IsRunningOnAzure ? string.Format(ResHelper.GetFileString("Install.ConnectionStringAzure"), connectionStringEntry, applicationSettingsEntry) : string.Format(ResHelper.GetFileString("Install.ConnectionStringError"), connectionStringEntry);
        lblErrorConnMessage.Text = errorMessage;

        // Set step that prompts user to enter connection string to web.config
        wzdInstaller.ActiveStepIndex = 2;

        if (!SystemContext.IsRunningOnAzure)
        {
            // Show troubleshoot link
            errorPanel.DisplayError("Install.ErrorPermissions", HELP_TOPIC_DISK_PERMISSIONS_LINK);
        }
    }


    /// <summary>
    /// Sets the application connection string and initializes the application.
    /// </summary>
    private void SetAppConnectionString()
    {
        ConnectionHelper.ConnectionString = ConnectionString;
        dbReady = true;

        // Init core
        CMSApplication.Init();
    }


    /// <summary>
    /// Ensures required web.config keys.
    /// </summary>
    private void EnsureApplicationConfiguration()
    {
        // Ensure hash salt in web.config
        if (String.IsNullOrEmpty(ValidationHelper.GetDefaultHashStringSalt()))
        {
            SettingsHelper.SetConfigValue(ValidationHelper.APP_SETTINGS_HASH_STRING_SALT, Guid.NewGuid().ToString());
        }

        // Ensure application GUID in web.config
        if (String.IsNullOrEmpty(CoreServices.AppSettings[SystemHelper.APP_GUID_KEY_NAME]))
        {
            SettingsHelper.SetConfigValue(SystemHelper.APP_GUID_KEY_NAME, Guid.NewGuid().ToString());
        }
    }


    protected void wzdInstaller_PreviousButtonClick(object sender, WizardNavigationEventArgs e)
    {
        --StepIndex;
        wzdInstaller.ActiveStepIndex -= 1;
    }


    /// <summary>
    /// Adds trial license keys to DB. No license is added when running in web application gallery mode.
    /// </summary>
    /// <param name="connectionString">Connection string</param>
    private bool AddTrialLicenseKeys(string connectionString)
    {
        // Skip creation of trial license keys if running in WWAG mode
        if (ValidationHelper.GetBoolean(SettingsHelper.AppSettings[WWAG_KEY], false))
        {
            return false;
        }

        string licenseKey = ValidationHelper.GetString(SettingsHelper.AppSettings["CMSTrialKey"], String.Empty);
        if (licenseKey != String.Empty)
        {
            return LicenseHelper.AddTrialLicenseKeys(licenseKey, true, false);
        }
        else
        {
            errorPanel.ErrorLabelText = ResHelper.GetFileString("Install.ErrorTrialLicense");
        }

        return false;
    }


    /// <summary>
    /// Initialize wizard header
    /// </summary>
    /// <param name="index">Step index</param>
    private void InitializeHeader(int index)
    {
        Help.Visible = true;
        StartHelp.Visible = true;
        StartHelp.TopicName = Help.TopicName = HELP_TOPIC_LINK;

        lblHeader.Text = ResHelper.GetFileString("Install.Step") + " - ";

        string[] stepIcons = { " icon-cogwheel", " icon-database", " icon-layout", " icon-check-circle icon-style-allow" };
        string[] stepTitles = { GetString("install.sqlsetting"), GetString("install.lbldatabase"), GetString("install.step5"), GetString("install.finishstep") };

        // Set common properties to each step icon
        for (var i = 0; i < stepIcons.Length; i++)
        {
            // Step panel
            var pnlStepIcon = new Panel();
            pnlStepIcon.ID = "stepPanel" + i;
            pnlStepIcon.CssClass = "install-step-panel";
            pnlHeaderImages.Controls.Add(pnlStepIcon);

            // Step icon
            var icon = new CMSIcon();
            icon.ID = "stepIcon" + i;
            icon.CssClass = "install-step-icon cms-icon-200" + stepIcons[i];
            icon.Attributes.Add("aria-hidden", "true");
            pnlStepIcon.Controls.Add(icon);

            // Step icon title
            var title = new HtmlGenericControl("title");
            title.ID = "stepTitle" + i;
            title.InnerText = stepTitles[i];
            title.Attributes.Add("class", "install-step-title");
            pnlStepIcon.Controls.Add(title);

            // Render separator only between step icons
            if (i < stepIcons.Length - 1)
            {
                // Separator panel
                var pnlSeparator = new Panel();
                pnlSeparator.ID = "separatorPanel" + i;
                pnlSeparator.CssClass = "install-step-icon-separator";
                pnlHeaderImages.Controls.Add(pnlSeparator);

                // Separator icon
                var separatorIcon = new CMSIcon();
                separatorIcon.CssClass = "icon-arrow-right cms-icon-150";
                separatorIcon.Attributes.Add("aria-hidden", "true");
                pnlSeparator.Controls.Add(separatorIcon);
            }
        }

        switch (index)
        {
            // SQL server and authentication mode
            case 0:
                {
                    lblHeader.Text += ResHelper.GetFileString("Install.Step0");
                    SetSelectedCSSClass("stepPanel0");
                    break;
                }
            // Database
            case 1:
                {
                    lblHeader.Text += ResHelper.GetFileString("Install.Step1");
                    SetSelectedCSSClass("stepPanel1");
                    break;
                }
            // web.config permissions
            case 2:
                {
                    StartHelp.Visible = Help.Visible = false;
                    lblHeader.Text += ResHelper.GetFileString("Install.Step3");
                    SetSelectedCSSClass("stepPanel1");
                    break;
                }

            // Database creation log
            case 3:
                {
                    StartHelp.Visible = Help.Visible = false;
                    lblHeader.Text += ResHelper.GetFileString("Install.Step2");
                    SetSelectedCSSClass("stepPanel1");
                    break;
                }

            // License import
            case 4:
                {
                    lblHeader.Text += ResHelper.GetFileString("Install.Step4");
                    SetSelectedCSSClass("stepPanel1");
                    break;
                }

            // Starter site selection
            case 5:
                {
                    lblHeader.Text += ResHelper.GetFileString("Install.Step5");
                    SetSelectedCSSClass("stepPanel2");
                    break;
                }

            // Import log
            case 6:
                {
                    StartHelp.Visible = Help.Visible = false;
                    lblHeader.Text += ResHelper.GetFileString("Install.Step6");
                    SetSelectedCSSClass("stepPanel2");
                    break;
                }

            // Finish step
            case 7:
                {
                    lblHeader.Text += ResHelper.GetFileString("Install.Step7");
                    SetSelectedCSSClass("stepPanel3");
                    break;
                }

            case 8:
                {
                    lblHeader.Text += ResHelper.GetFileString("Install.Step8");
                    SetSelectedCSSClass("stepPanel1");
                    break;
                }
        }

        // Calculate step number
        if (PreviousStep == index)
        {
            StepOperation = 0;
        }
        ActualStep += StepOperation;
        lblHeader.Text = string.Format(lblHeader.Text, ActualStep + 1);
        PreviousStep = index;
    }


    private void SetSelectedCSSClass(string stepPanel)
    {
        var selectedPanel = pnlHeaderImages.FindControl(stepPanel) as Panel;
        selectedPanel.CssClass += " install-step-icon-selected";
    }


    private void EnsureDefaultButton()
    {
        if (wzdInstaller.ActiveStep != null)
        {
            Page.Form.DefaultButton = (wzdInstaller.ActiveStep.StepType == WizardStepType.Start) ? StartNextButton.UniqueID : NextButton.UniqueID;
        }
    }


    #region "Installation methods"

    public bool CreateDatabase(string collation)
    {
        try
        {
            string message = ResHelper.GetFileString("Installer.LogCreatingDatabase") + " " + databaseDialog.NewDatabaseName;
            AddResult(message);
            LogProgressState(LogStatusEnum.Info, message);

            string connectionString = ConnectionHelper.GetConnectionString(authenticationType, userServer.ServerName, "", userServer.DBUsername, Password, SqlInstallationHelper.DB_CONNECTION_TIMEOUT, false);

            // Use default collation, if none specified
            if (String.IsNullOrEmpty(collation))
            {
                collation = DatabaseHelper.DatabaseCollation;
            }

            if (!DBCreated)
            {
                SqlInstallationHelper.CreateDatabase(databaseDialog.NewDatabaseName, connectionString, collation);
            }

            return true;
        }
        catch (Exception ex)
        {
            mDisplayLog = true;
            string message = ResHelper.GetFileString("Intaller.LogErrorCreateDB") + " " + ex.Message;
            AddResult(message);
            LogProgressState(LogStatusEnum.Error, message);
        }

        return false;
    }


    /// <summary>
    /// Logs message to install log.
    /// </summary>
    /// <param name="message">Message</param>
    /// <param name="type">Type of message ("E" - error, "I" - info)</param>
    public void Log(string message, MessageTypeEnum type)
    {
        AddResult(message);
        switch (type)
        {
            case MessageTypeEnum.Error:
                LogProgressState(LogStatusEnum.Error, message);
                break;

            case MessageTypeEnum.Info:
                LogProgressState(LogStatusEnum.Info, message);
                break;
        }
    }


    /// <summary>
    /// Installs database (table structure + default data).
    /// </summary>
    /// <param name="parameter">Async action param</param>
    private void InstallDatabase(object parameter)
    {
        if (!DBInstalled)
        {
            SqlInstallationHelper.AfterDataGet += OnAfterGetDefaultData;

            bool success = SqlInstallationHelper.InstallDatabase(InstallInfo.ConnectionString, InstallInfo.ScriptsFullPath, ResHelper.GetFileString("Installer.LogErrorCreateDBObjects"), ResHelper.GetFileString("Installer.LogErrorDefaultData"), Log, InstallInfo.DBSchema);

            SqlInstallationHelper.AfterDataGet -= OnAfterGetDefaultData;

            if (success)
            {
                LogProgressState(LogStatusEnum.Finish, ResHelper.GetFileString("Installer.DBInstallFinished"));
            }
            else
            {
                throw new Exception("[InstallDatabase]: Error during database creation.");
            }
        }
    }


    private void OnAfterGetDefaultData(object sender, DataSetPostProcessingEventArgs args)
    {
        MacroSecurityProcessor.RefreshSecurityParameters(args.Data, "administrator");
    }


    #endregion


    #region "Error handling methods"

    protected void HandleError(string message, WizardNavigationEventArgs e)
    {
        if (StepIndex > 1)
        {
            --StepIndex;
        }
        errorPanel.ErrorLabelText = message;
        e.Cancel = true;
    }


    protected void HandleError(string message)
    {
        if (StepIndex > 1)
        {
            --StepIndex;
        }
        errorPanel.ErrorLabelText = message;
    }


    protected void HandleError(string message, string resourceString, string topic)
    {
        if (StepIndex > 1)
        {
            --StepIndex;
        }
        errorPanel.ErrorLabelText = message;
        errorPanel.DisplayError(resourceString, topic);
    }

    #endregion


    #region "Logging methods"

    /// <summary>
    /// Appends the result string to the result message.
    /// </summary>
    /// <param name="result">String to append</param>
    public void AddResult(string result)
    {
        mResult = result + "\n" + mResult;
    }


    /// <summary>
    /// Logs progress state.
    /// </summary>
    /// <param name="type">Type of the message</param>
    /// <param name="message">Message to be logged</param>
    public void LogProgressState(LogStatusEnum type, string message)
    {
        string[] status = InstallInfo.InstallLog.Split(new[] { InstallInfo.SEPARATOR }, StringSplitOptions.None);

        // Wrong format of the internal status
        if (status.Length != 4)
        {
            InstallInfo.InstallLog = String.Format("F{0}Wrong internal log.{0}{0}", InstallInfo.SEPARATOR);
        }

        message = HTMLHelper.HTMLEncode(message);

        switch (type)
        {
            case LogStatusEnum.Info:
                status[0] = "I";
                status[1] = String.Format("{0}<br />{1}", message, status[1]);
                break;

            case LogStatusEnum.Error:
                status[0] = "F";
                status[2] = String.Format("{0}<strong>{1}</strong>{2}<br />", status[2], ResHelper.GetFileString("Global.ErrorSign"), message);
                break;

            case LogStatusEnum.Warning:
                status[3] = String.Format("{0}<strong>{1}</strong>{2}<br />", status[3], ResHelper.GetFileString("Global.Warning"), message);
                break;

            case LogStatusEnum.Finish:
                status[0] = "F";
                status[1] = String.Format("<strong>{0}</strong><br /><br />{1}", message, status[1]);
                break;
        }

        InstallInfo.InstallLog = status[0] + InstallInfo.SEPARATOR + status[1] + InstallInfo.SEPARATOR + status[2] + InstallInfo.SEPARATOR + status[3];
    }

    #endregion


    #region "ICallbackEventHandler Members"

    public string GetCallbackResult()
    {
        return hdnState.Value;
    }


    public void RaiseCallbackEvent(string eventArgument)
    {
        // Get arguments
        string[] args = eventArgument.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        bool cancel = ValidationHelper.GetBoolean(args[0], false);
        bool import = (args[1] == "IM");
        int messageLength = 0;
        int errorLength = 0;
        int warningLength = 0;

        if (args.Length == 5)
        {
            messageLength = ValidationHelper.GetInteger(args[2], 0);
            errorLength = ValidationHelper.GetInteger(args[3], 0);
            warningLength = ValidationHelper.GetInteger(args[4], 0);
        }

        if (import)
        {
            try
            {
                // Cancel
                if (cancel)
                {
                    ImportManager.Settings.Cancel();
                }

                hdnState.Value = ImportManager.Settings.GetLimitedProgressLog(messageLength, errorLength, warningLength);
            }
            catch
            {
                ImportManager.Settings.LogProgressState(LogStatusEnum.Finish, ResHelper.GetFileString("SiteImport.Applicationrestarted"));
                hdnState.Value = ImportManager.Settings.GetLimitedProgressLog(messageLength, errorLength, warningLength);
            }
        }
        else
        {
            try
            {
                hdnState.Value = InstallInfo.GetLimitedProgressLog(messageLength, errorLength, warningLength);
            }
            catch
            {
                LogProgressState(LogStatusEnum.Finish, ResHelper.GetFileString("SiteImport.Applicationrestarted"));
                hdnState.Value = InstallInfo.GetLimitedProgressLog(messageLength, errorLength, warningLength);
            }
        }
    }

    #endregion
}