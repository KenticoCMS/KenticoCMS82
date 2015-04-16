using System;
using System.Collections;
using System.Security.Principal;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSImportExport;
using CMS.Core;
using CMS.EventLog;
using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.IO;
using CMS.Base;
using CMS.Localization;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;

using Directory = CMS.IO.Directory;
using IOExceptions = System.IO;

public partial class CMSModules_ImportExport_Controls_ImportWizard : CMSUserControl, ICallbackEventHandler
{
    #region "Constants"

    /// <summary>
    /// Short link to help page regarding disk permissions.
    /// </summary>
    private const string HELP_TOPIC_DISKPERMISSIONS_LINK = "disk_permission_problems";

    #endregion


    #region "Variables"

    private static readonly Hashtable mManagers = new Hashtable();
    private static object mLock = new object();

    private SiteImportSettings mImportSettings = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Redirection URL after finish button click.
    /// </summary>
    public string FinishUrl
    {
        get;
        set;
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
                // On Azure, the restart cannot be detected via Instace GUIDs since with more instances, each instace has a different one.
                if (!StorageHelper.IsExternalStorage(null))
                {
                    // Detect restart of the application
                    if (ApplicationInstanceGUID != SystemHelper.ApplicationInstanceGUID)
                    {
                        // Lock section to avoid multiple log same error
                        lock (mLock)
                        {
                            LogStatusEnum progressLog = ImportSettings.GetProgressState();
                            if (progressLog == LogStatusEnum.Info)
                            {
                                ImportSettings.LogProgressState(LogStatusEnum.UnexpectedFinish, GetString("SiteImport.ApplicationRestarted"));
                            }
                        }
                    }
                }

                ImportManager im = new ImportManager(ImportSettings);
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
    /// Wizard height.
    /// </summary>
    public int PanelHeight
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["PanelHeight"], 400);
        }
        set
        {
            ViewState["PanelHeight"] = value;
        }
    }


    /// <summary>
    /// Application instance GUID.
    /// </summary>
    public Guid ApplicationInstanceGUID
    {
        get
        {
            if (ViewState["ApplicationInstanceGUID"] == null)
            {
                ViewState["ApplicationInstanceGUID"] = SystemHelper.ApplicationInstanceGUID;
            }

            return ValidationHelper.GetGuid(ViewState["ApplicationInstanceGUID"], Guid.Empty);
        }
    }


    /// <summary>
    /// Import process GUID.
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
    /// Persistent settings key.
    /// </summary>
    public string PersistentSettingsKey
    {
        get
        {
            return "Import_" + ProcessGUID + "_Settings";
        }
    }


    /// <summary>
    /// Import settings stored in viewstate.
    /// </summary>
    public SiteImportSettings ImportSettings
    {
        get
        {
            if (mImportSettings == null)
            {
                SiteImportSettings settings = (SiteImportSettings)SiteExportSettings.GetFromPersistentStorage(PersistentSettingsKey);
                if (settings == null)
                {
                    if (wzdImport.ActiveStepIndex == 0)
                    {
                        settings = GetNewSettings();
                        PersistentStorageHelper.SetValue(PersistentSettingsKey, settings);
                    }
                    else
                    {
                        throw new Exception("[ImportWizard.ImportSettings]: Import settings has been lost.");
                    }
                }
                mImportSettings = settings;
            }
            return mImportSettings;
        }
        set
        {
            PersistentStorageHelper.SetValue(PersistentSettingsKey, value);
        }
    }

    #endregion


    #region "Finish step wizard buttons"

    /// <summary>
    /// Finish button.
    /// </summary>
    public LocalizedButton FinishButton
    {
        get
        {
            return wzdImport.FindControl("FinishNavigationTemplateContainerID").FindControl("StepFinishButton") as LocalizedButton;
        }
    }


    /// <summary>
    /// Previous button.
    /// </summary>
    public LocalizedButton PreviousButton
    {
        get
        {
            return wzdImport.FindControl("FinishNavigationTemplateContainerID").FindControl("StepPreviousButton") as LocalizedButton;
        }
    }


    /// <summary>
    /// Next button.
    /// </summary>
    public LocalizedButton NextButton
    {
        get
        {
            return wzdImport.FindControl("StartNavigationTemplateContainerID").FindControl("StepNextButton") as LocalizedButton;
        }
    }


    /// <summary>
    /// Cancel button.
    /// </summary>
    public LocalizedButton CancelButton
    {
        get
        {
            return wzdImport.FindControl("FinishNavigationTemplateContainerID").FindControl("StepCancelButton") as LocalizedButton;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Register script for pendingCallbacks repair
        ScriptHelper.FixPendingCallbacks(Page);

        // Handle Import settings
        if (!RequestHelper.IsCallback() && !RequestHelper.IsPostBack())
        {
            // Initialize import settings
            ImportSettings = GetNewSettings();
        }

        if (!RequestHelper.IsCallback())
        {
            if (!VirtualPathHelper.UsingVirtualPathProvider)
            {
                SetWarningLabel(GetString("ImportSite.VirtualPathProviderNotRunning"));
            }

            ctrlAsync.OnFinished += ctrlAsync_OnFinished;
            ctrlAsync.OnError += ctrlAsync_OnError;

            bool notTempPermissions = false;

            if (wzdImport.ActiveStepIndex < 3)
            {
                stpConfigImport.Settings = ImportSettings;
                stpSiteDetails.Settings = ImportSettings;
                stpImport.Settings = ImportSettings;

                // Ensure directory
                try
                {
                    DirectoryHelper.EnsureDiskPath(ImportSettings.TemporaryFilesPath + "\\temp.file", ImportSettings.WebsitePath);
                }
                catch (IOExceptions.IOException ex)
                {
                    pnlWrapper.Visible = false;
                    SetErrorLabel(ex.Message);
                    return;
                }

                // Check permissions
                notTempPermissions = !DirectoryHelper.CheckPermissions(ImportSettings.TemporaryFilesPath, true, true, false, false);
            }

            if (notTempPermissions)
            {
                pnlWrapper.Visible = false;
                SetErrorLabel(LocalizationHelper.GetStringFormat("ImportSite.ErrorPermissions", ImportSettings.TemporaryFilesPath, WindowsIdentity.GetCurrent().Name));
                pnlPermissions.Visible = true;
                lnkPermissions.Target = "_blank";
                lnkPermissions.Text = GetString("Install.ErrorPermissions");
                lnkPermissions.NavigateUrl = DocumentationHelper.GetDocumentationTopicUrl(HELP_TOPIC_DISKPERMISSIONS_LINK);
            }
            else
            {
                if (!RequestHelper.IsPostBack())
                {
                    // Delete temporary files
                    try
                    {
                        // Delete only folder structure if there is not special folder
                        bool onlyFolderStructure = !Directory.Exists(DirectoryHelper.CombinePath(ImportSettings.TemporaryFilesPath, ImportExportHelper.FILES_FOLDER));
                        ImportProvider.DeleteTemporaryFiles(ImportSettings, onlyFolderStructure);
                    }
                    catch (Exception ex)
                    {
                        pnlWrapper.Visible = false;
                        SetErrorLabel(GetString("ImportSite.ErrorDeletionTemporaryFiles") + ex.Message);
                        return;
                    }
                }

                // Javascript functions
                string script =
                    "var imMessageText = '';\n" +
                    "var imErrorText = '';\n" +
                    "var imWarningText = '';\n" +
                    "var imMachineName = '" + SystemContext.MachineName.ToLowerCSafe() + "';\n" +
                    "var getBusy = false;\n" +
                    "function GetImportState(cancel)\n" +
                    "{ if(window.Activity){window.Activity();} if (getBusy && !cancel) return; getBusy = true; var argument = cancel + ';' + imMessageText.length + ';' + imErrorText.length + ';' + imWarningText.length + ';' + imMachineName; return " + Page.ClientScript.GetCallbackEventReference(this, "argument", "SetImportStateMssg", "argument", "ProcessSetImportStateError", false) + " }\n";

                script +=
                    "function ProcessSetImportStateError(arg, context)\n" +
                    "{\n" +
                    "   getBusy = false;\n" +
                    "}\n";

                script +=
                    "function SetImportStateMssg(rValue, context)\n" +
                    "{\n" +
                    "   getBusy = false;\n" +
                    "   if(rValue != '')\n" +
                    "   {\n" +
                    "       var args = context.split(';');\n" +
                    "       var values = rValue.split('" + SiteExportSettings.SEPARATOR + "');\n" +
                    "       var messageElement = document.getElementById('" + lblProgress.ClientID + "');\n" +
                    "       var errorElement = document.getElementById('" + lblError.ClientID + "');\n" +
                    "       var warningElement = document.getElementById('" + lblWarning.ClientID + "');\n" +
                    "       var messageText = imMessageText;\n" +
                    "       messageText = values[1] + messageText.substring(messageText.length - args[1]);\n" +
                    "       if(messageText.length > imMessageText.length){ imMessageText = messageElement.innerHTML = messageText; }\n" +
                    "       var errorText = imErrorText;\n" +
                    "       errorText = values[2] + errorText.substring(errorText.length - args[2]);\n" +
                    "       if(errorText.length > imErrorText.length){ imErrorText = errorElement.innerHTML = errorText; document.getElementById('" + pnlError.ClientID + "').style.removeProperty('display'); }\n" +
                    "       var warningText = imWarningText;\n" +
                    "       warningText = values[3] + warningText.substring(warningText.length - args[3]);\n" +
                    "       if(warningText.length > imWarningText.length){ imWarningText = warningElement.innerHTML = warningText; document.getElementById('" + pnlWarning.ClientID + "').style.removeProperty('display'); }\n" +
                    "       if((values=='') || (values[0]=='F'))\n" +
                    "       {\n" +
                    "           StopImportStateTimer();\n" +
                    "           var actDiv = document.getElementById('actDiv'); \n" +
                    "           if (actDiv != null) { actDiv.style.display = 'none'; } \n" +
                    "           BTN_Disable('" + CancelButton.ClientID + "');\n" +
                    "           BTN_Enable('" + FinishButton.ClientID + "');\n" +
                    "       }\n" +
                    "   }\n" +
                    "}\n";

                // Register the script to perform get flags for showing buttons retrieval callback
                ScriptHelper.RegisterClientScriptBlock(this, GetType(), "GetSetImportState", ScriptHelper.GetScript(script));

                // Add cancel button attribute
                CancelButton.Attributes.Add("onclick", "BTN_Disable('" + CancelButton.ClientID + "');" + "return CancelImport();");

                wzdImport.NextButtonClick += wzdImport_NextButtonClick;
                wzdImport.PreviousButtonClick += wzdImport_PreviousButtonClick;
                wzdImport.FinishButtonClick += wzdImport_FinishButtonClick;

                if (!RequestHelper.IsPostBack())
                {
                    stpConfigImport.InitControl();
                }
            }
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (!RequestHelper.IsCallback())
        {
            InitializeHeader();

            // Button click script
            const string afterScript = "var imClicked = false; \n" +
                                       "function NextStepAction() \n" +
                                       "{ \n" +
                                       "   if(!imClicked) \n" +
                                       "   { \n" +
                                       "     imClicked = true; \n" +
                                       "     return true; \n" +
                                       "   } \n" +
                                       "   return false; \n" +
                                       "} \n";

            ltlScriptAfter.Text += ScriptHelper.GetScript(afterScript);

            // Ensure default button
            EnsureDefaultButton();

            InitAlertLabels();
        }
    }


    protected override void Render(HtmlTextWriter writer)
    {
        base.Render(writer);

        // Save the settings
        if (wzdImport.ActiveStep.StepType != WizardStepType.Finish)
        {
            ImportSettings.SavePersistent();
        }
    }

    #endregion


    #region "Button handling"

    protected void wzdImport_FinishButtonClick(object sender, WizardNavigationEventArgs e)
    {
        if (String.IsNullOrEmpty(FinishUrl))
        {
            FinishUrl = UIContextHelper.GetElementUrl(ModuleName.CMS, "Sites", false);
        }

        URLHelper.Redirect(FinishUrl);
    }


    protected void wzdImport_PreviousButtonClick(object sender, WizardNavigationEventArgs e)
    {
        if (wzdImport.ActiveStepIndex == 1 || e.NextStepIndex == 0)
        {
            wzdImport.ActiveStepIndex = 0;
            stpConfigImport.Settings.TemporaryFilesCreated = false;
        }
        else
        {
            wzdImport.ActiveStepIndex = e.NextStepIndex;
        }
    }


    protected void wzdImport_NextButtonClick(object sender, WizardNavigationEventArgs e)
    {
        switch (e.CurrentStepIndex)
        {
            case 0:
                // Apply settings
                if (!stpConfigImport.ApplySettings())
                {
                    e.Cancel = true;
                    return;
                }

                // Update settings
                ImportSettings = stpConfigImport.Settings;

                ltlScriptAfter.Text = ScriptHelper.GetScript(
                    "var actDiv = document.getElementById('actDiv'); \n" +
                    "if (actDiv != null) { actDiv.style.display='block'; } \n" +
                    "var buttonsDiv = document.getElementById('buttonsDiv'); if (buttonsDiv != null) { buttonsDiv.disabled=true; } \n" +
                    "BTN_Disable('" + NextButton.ClientID + "'); \n" +
                    "StartUnzipTimer();"
                    );

                // Create temporary files asynchronously
                ctrlAsync.RunAsync(CreateTemporaryFiles, WindowsIdentity.GetCurrent());

                e.Cancel = true;
                break;

            case 1:
                // Apply settings
                if (!stpSiteDetails.ApplySettings())
                {
                    e.Cancel = true;
                    return;
                }

                // Update settings
                ImportSettings = stpSiteDetails.Settings;
                //stpImport.SelectedNodeValue = CMSObjectHelper.GROUP_OBJECTS;
                stpImport.ReloadData(true);

                wzdImport.ActiveStepIndex++;
                break;

            case 2:
                // Apply settings
                if (!stpImport.ApplySettings())
                {
                    e.Cancel = true;
                    return;
                }

                ImportSettings = stpImport.Settings;
                ImportSettings.DefaultProcessObjectType = ProcessObjectEnum.Selected;

                if (!StartImport(ImportSettings))
                {
                    e.Cancel = true;

                    return;
                }

                wzdImport.ActiveStepIndex++;
                break;
        }

        ReloadSteps();
    }

    #endregion


    #region "Async control events"

    protected void ctrlAsync_OnError(object sender, EventArgs e)
    {
        if (((AsyncControl)sender).Worker.LastException != null)
        {
            // Show error message
            SetErrorLabel(((AsyncControl)sender).Worker.LastException.Message);
        }
        else
        {
            // Show general error message
            SetErrorLabel(String.Format(GetString("logon.erroroccurred"), GetString("general.seeeventlog")));
        }

        // Stop the timer
        ltlScript.Text += ScriptHelper.GetScript("StopUnzipTimer();");
    }


    protected void ctrlAsync_OnFinished(object sender, EventArgs e)
    {
        // Stop the timer
        const string script = "StopUnzipTimer();";

        // Check if a new module is being imported
        if (!String.IsNullOrWhiteSpace(ImportSettings.ModuleName))
        {
            // Start the import process immediately
            ImportSettings.DefaultProcessObjectType = ProcessObjectEnum.All;
            ImportSettings.CopyFiles = false;
            ImportSettings.CopyCodeFiles = false;
            if (!StartImport(ImportSettings))
            {
                return;
            }

            // Skip step for site and objects selection
            wzdImport.ActiveStepIndex++;

            // Skip step for object selection
            wzdImport.ActiveStepIndex++;

        } // Decide if importing site
        else if (ImportSettings.SiteIsIncluded)
        {
            // Single site import and no site exists
            if (ValidationHelper.GetBoolean(ImportSettings.GetInfo(ImportExportHelper.INFO_SINGLE_OBJECT), false) && (SiteInfoProvider.GetSitesCount() == 0))
            {
                SetErrorLabel(GetString("SiteImport.SingleSiteObjectNoSite"));
                return;
            }

            // Init control
            stpSiteDetails.ReloadData();
        }
        else
        {
            // Skip step for site selection
            wzdImport.ActiveStepIndex++;
            stpImport.ReloadData(true);
        }

        // Move to the next step
        wzdImport.ActiveStepIndex++;
        ReloadSteps();

        ltlScriptAfter.Text += ScriptHelper.GetScript(script);
    }


    protected void worker_OnError(object sender, EventArgs e)
    {
    }


    protected void worker_OnFinished(object sender, EventArgs e)
    {
    }

    #endregion


    #region "Other methods"

    protected void InitializeHeader()
    {
        // Make some step count corrections
        if ((wzdImport.ActiveStepIndex == 0) || ImportSettings.SiteIsIncluded)
        {
            ucHeader.Title = string.Format(GetString("ImportPanel.Title"), wzdImport.ActiveStepIndex + 1, wzdImport.WizardSteps.Count);
        }
        else
        {
            ucHeader.Title = string.Format(GetString("ImportPanel.Title"), wzdImport.ActiveStepIndex, wzdImport.WizardSteps.Count - 1);
        }

        switch (wzdImport.ActiveStepIndex)
        {
            case 0:
                ucHeader.Header = GetString("ImportPanel.ObjectsSettingsHeader");
                ucHeader.Description = GetString("ImportPanel.ObjectsSelectionSetting");
                break;

            case 1:
                ucHeader.Header = GetString("ImportPanel.ObjectsSiteDetailsHeader");
                if (ImportSettings.SiteIsIncluded && ValidationHelper.GetBoolean(ImportSettings.GetInfo(ImportExportHelper.INFO_SINGLE_OBJECT), false))
                {
                    ucHeader.Description = GetString("ImportPanel.SiteObjectImport");
                }
                else
                {
                    ucHeader.Description = GetString("ImportPanel.ObjectsSiteDetailsDescription");
                }
                break;

            case 2:
                ucHeader.Header = GetString("ImportPanel.ObjectsSelectionHeader");
                ucHeader.Description = GetString("ImportPanel.ObjectsSelectionDescription");
                break;

            case 3:
                ucHeader.Header = GetString("ImportPanel.ObjectsProgressHeader");
                ucHeader.Description = GetString("ImportPanel.ObjectsProgressDescription");
                break;
        }
    }


    // Create temporary files and preselect objects
    private void CreateTemporaryFiles(object parameter)
    {
        ImportProvider.CreateTemporaryFiles(ImportSettings);
        ImportSettings.LoadDefaultSelection();
    }


    /// <summary>
    /// Starts import process with given settings. Returns true if import was started successfully, false otherwise (error label is set in this case).
    /// </summary>
    /// <param name="importSettings">Import settings</param>
    /// <returns>Returns true if import was started successfully, false otherwise.</returns>
    private bool StartImport(SiteImportSettings importSettings)
    {
        // Check licences
        string error = ImportExportControl.CheckLicenses(importSettings);
        if (!string.IsNullOrEmpty(error))
        {
            SetErrorLabel(error);

            return false;
        }

        // Init the Mimetype helper (required for the Import)
        MimeTypeHelper.LoadMimeTypes();

        // Start asynchronnous Import
        if (importSettings.SiteIsIncluded)
        {
            importSettings.EventLogSource = string.Format(importSettings.GetAPIString("ImportSite.EventLogSiteSource", "Import '{0}' site"), ResHelper.LocalizeString(importSettings.SiteDisplayName));
        }
        ImportManager.Settings = importSettings;

        AsyncWorker worker = new AsyncWorker();
        worker.OnFinished += worker_OnFinished;
        worker.OnError += worker_OnError;
        worker.RunAsync(ImportManager.Import, WindowsIdentity.GetCurrent());

        return true;
    }


    protected void ReloadSteps()
    {
        if (wzdImport.ActiveStepIndex == 3)
        {
            ltlScript.Text = ScriptHelper.GetScript("StartImportStateTimer();");
        }
    }


    private void EnsureDefaultButton()
    {
        if (wzdImport.ActiveStep != null)
        {
            switch (wzdImport.ActiveStep.StepType)
            {
                case WizardStepType.Start:
                    Page.Form.DefaultButton =
                        wzdImport.FindControl("StartNavigationTemplateContainerID").FindControl("StepNextButton").
                            UniqueID;
                    break;

                case WizardStepType.Step:
                    Page.Form.DefaultButton =
                        wzdImport.FindControl("StepNavigationTemplateContainerID").FindControl("StepNextButton").
                            UniqueID;
                    break;

                case WizardStepType.Finish:
                    Page.Form.DefaultButton =
                        wzdImport.FindControl("FinishNavigationTemplateContainerID").FindControl("StepFinishButton").
                            UniqueID;
                    break;
            }
        }
    }


    /// <summary>
    /// Creates new settings object for Import.
    /// </summary>
    private SiteImportSettings GetNewSettings()
    {
        SiteImportSettings result = new SiteImportSettings(MembershipContext.AuthenticatedUser);

        result.WebsitePath = Server.MapPath("~/");
        result.PersistentSettingsKey = PersistentSettingsKey;

        return result;
    }


    /// <summary>
    /// Iniliazes (hides) alert labels
    /// </summary>
    private void InitAlertLabels()
    {
        // Do not use Visible property to hide this elements. They are used in JS.
        pnlError.Attributes.CssStyle.Add(HtmlTextWriterStyle.Display, String.IsNullOrEmpty(lblError.Text) ? "none" : "block");
        pnlWarning.Attributes.CssStyle.Add(HtmlTextWriterStyle.Display, String.IsNullOrEmpty(lblWarning.Text) ? "none" : "block");
    }


    /// <summary>
    /// Displays text in given altert lable
    /// </summary>
    /// <param name="label">Alert label</param>
    /// <param name="text">Text to display</param>
    private void SetAlertLabel(Label label, string text)
    {
        label.Text = text;
    }


    /// <summary>
    /// Displays error alert label with given text
    /// </summary>
    /// <param name="text">Text to display</param>
    private void SetErrorLabel(string text)
    {
        SetAlertLabel(lblError, text);
    }


    /// <summary>
    /// Displays warning alert label with given text
    /// </summary>
    /// <param name="text">Text to display</param>
    private void SetWarningLabel(string text)
    {
        SetAlertLabel(lblWarning, text);
    }

    #endregion


    #region "Callback handling"

    /// <summary>
    /// Callback event handler.
    /// </summary>
    /// <param name="argument">Callback argument</param>
    public void RaiseCallbackEvent(string argument)
    {
        // Get arguments
        string[] args = argument.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        bool cancel = ValidationHelper.GetBoolean(args[0], false);
        int messageLength = 0;
        int errorLength = 0;
        int warningLength = 0;
        string machineName = null;

        if (args.Length == 5)
        {
            messageLength = ValidationHelper.GetInteger(args[1], 0);
            errorLength = ValidationHelper.GetInteger(args[2], 0);
            warningLength = ValidationHelper.GetInteger(args[3], 0);
            machineName = ValidationHelper.GetString(args[4], null);
        }

        // Check if on same machine
        if (machineName == SystemContext.MachineName.ToLowerCSafe())
        {
            try
            {
                // Cancel Import
                if (cancel)
                {
                    ImportManager.Settings.Cancel();
                }

                hdnState.Value = ImportManager.Settings.GetLimitedProgressLog(messageLength, errorLength, warningLength);
            }
            catch (Exception ex)
            {
                EventLogProvider.LogException("ImportWizard", "IMPORT", ex);

                hdnState.Value = ImportManager.Settings.GetLimitedProgressLog(messageLength, errorLength, warningLength);
            }
            finally
            {
                if (ImportManager.Settings.GetProgressState() != LogStatusEnum.Info)
                {
                    // Delete presistent data
                    PersistentStorageHelper.RemoveValue(PersistentSettingsKey);
                }
            }
        }
    }


    /// <summary>
    /// Callback result retrieving handler.
    /// </summary>
    public string GetCallbackResult()
    {
        return hdnState.Value;
    }

    #endregion
}