using System;
using System.Collections;
using System.Security.Principal;
using System.Threading;

using CMS.Base;
using CMS.DataEngine;
using CMS.EventLog;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.UIControls.Internal;

using IOExceptions = System.IO;

public partial class CMSModules_System_System_Deployment : GlobalAdminPage
{
    #region "Variables"

    DeploymentManager mManager = null;
    private static readonly Hashtable mErrors = new Hashtable();

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets the instance of the current deployment manager
    /// </summary>
    private DeploymentManager CurrentDeploymentManager
    {
        get
        {
            return mManager ?? (mManager = new DeploymentManager());
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
    /// Current Error.
    /// </summary>
    private string CurrentError
    {
        get
        {
            return ValidationHelper.GetString(mErrors["TranslateError_" + ctlAsyncLog.ProcessGUID], string.Empty);
        }
        set
        {
            mErrors["TranslateError_" + ctlAsyncLog.ProcessGUID] = value;
        }
    }

    #endregion


    #region "Page events"

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // Handle Deployment manager events (Info/Error messages)
        CurrentDeploymentManager.Log += CurrentDeploymentManager_Log;
        CurrentDeploymentManager.Error += CurrentDeploymentManager_Error;

        // Handle Async control
        ctlAsyncLog.OnFinished += ctlAsyncLog_OnFinished;
        ctlAsyncLog.OnError += ctlAsyncLog_OnError;
        ctlAsyncLog.OnRequestLog += ctlAsyncLog_OnRequestLog;
        ctlAsyncLog.OnCancel += ctlAsyncLog_OnCancel;

        ctlAsyncLog.TitleText = GetString("Deployment.Processing");
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        chkSaveCSS.Checked = CssStylesheetInfoProvider.StoreCSSStyleSheetsInExternalStorage;
        chkSaveLayouts.Checked = LayoutInfoProvider.StoreLayoutsInExternalStorage;
        chkSavePageTemplate.Checked = PageTemplateInfoProvider.StorePageTemplatesInExternalStorage;
        chkSaveWebpartLayout.Checked = WebPartLayoutInfoProvider.StoreWebPartLayoutsInExternalStorage;
        chkSaveTransformation.Checked = TransformationInfoProvider.StoreTransformationsInExternalStorage;
        chkSaveWebpartContainer.Checked = WebPartContainerInfoProvider.StoreWebPartContainersInExternalStorage;
        chkSaveAltFormLayouts.Checked = AlternativeFormInfoProvider.StoreAlternativeFormsInExternalStorage;
        chkSaveFormLayouts.Checked = DataClassInfoProvider.StoreFormLayoutsInExternalStorage;

        if (chkSaveCSS.Checked || chkSaveLayouts.Checked || chkSavePageTemplate.Checked || chkSaveWebpartLayout.Checked
            || chkSaveTransformation.Checked || chkSaveWebpartContainer.Checked || chkSaveAltFormLayouts.Checked || chkSaveFormLayouts.Checked)
        {
            lblSynchronization.Visible = true;
            btnSynchronize.Visible = true;
        }

        bool deploymentMode = SettingsKeyInfoProvider.DeploymentMode;
        chkSaveLayouts.Enabled = chkSavePageTemplate.Enabled = chkSaveTransformation.Enabled = chkSaveWebpartLayout.Enabled
            = chkSaveAltFormLayouts.Enabled = chkSaveFormLayouts.Enabled = !deploymentMode;

        if (SystemContext.IsRunningOnAzure)
        {
            ShowWarning(GetString("Deployment.AzureDisabled"), null, null);
            btnSaveAll.Enabled = false;
            btnSourceControl.Enabled = false;
            chkSaveCSS.Enabled = chkSaveLayouts.Enabled = chkSavePageTemplate.Enabled = chkSaveTransformation.Enabled = chkSaveWebpartLayout.Enabled
                = chkSaveAltFormLayouts.Enabled = chkSaveFormLayouts.Enabled = false;
        }

        if (SettingsKeyInfoProvider.DeploymentMode)
        {
            lblDeploymentInfo.Text = GetString("Deployment.SaveAllToDBInfo");
            btnSaveAll.ResourceString = "Deployment.SaveAllToDB";
            lblSourceControlInfo.Text = GetString("Deployment.SourceControlInfoDeploymentMode");
        }
        else
        {
            lblDeploymentInfo.Text = GetString("Deployment.SaveAllInfo");
            btnSaveAll.ResourceString = "Deployment.SaveAll";
            lblSourceControlInfo.Text = GetString("Deployment.SourceControlInfo");
        }

        if (!SystemContext.IsFullTrustLevel)
        {
            // Disable the form in Medium Trust and tell user what's wrong
            chkSaveCSS.Enabled = chkSaveLayouts.Enabled = chkSavePageTemplate.Enabled = chkSaveTransformation.Enabled
                = chkSaveWebpartContainer.Enabled = chkSaveWebpartLayout.Enabled = chkSaveAltFormLayouts.Enabled = chkSaveFormLayouts.Enabled
                = btnSaveAll.Enabled = btnSourceControl.Enabled = btnSynchronize.Enabled = false;

            ShowInformation(GetString("deployment.fulltrustrequired"));
        }

        if (SystemContext.DevelopmentMode)
        {
            ShowInformation(GetString("Deployment.DevelopmentMode"));
            btnSaveAll.Enabled = btnSourceControl.Enabled = btnSynchronize.Enabled = false;
            chkSaveCSS.Enabled = chkSaveLayouts.Enabled = chkSavePageTemplate.Enabled = chkSaveWebpartLayout.Enabled = false;
            chkSaveTransformation.Enabled = chkSaveWebpartContainer.Enabled = chkSaveAltFormLayouts.Enabled = chkSaveFormLayouts.Enabled = false;
        }
    }

    #endregion


    #region "Deployment methods"


    protected void btnSynchronize_Click(object sender, EventArgs e)
    {
        RunAsyncInternal(Synchronize);
    }


    protected void btnSourceControl_Click(object sender, EventArgs e)
    {
        RunAsyncInternal(SaveExternally);
    }


    protected void btnSaveAll_Click(object sender, EventArgs e)
    {
        RunAsyncInternal(Deploy);
    }


    protected void btnTest_Click(object sender, EventArgs e)
    {
        RunAsyncInternal(Test);
    }

    private void RunAsyncInternal(AsyncAction action)
    {
        pnlLog.Visible = true;

        CurrentError = string.Empty;
        CurrentLog.Close();
        EnsureLog();
        ctlAsyncLog.Parameter = GetParameters();

        ctlAsyncLog.RunAsync(action, WindowsIdentity.GetCurrent());
    }
    #endregion


    #region "Helper methods"

    /// <summary>
    /// Gets the parameters obejct for current controls
    /// </summary>
    private DeploymentParameters GetParameters()
    {
        return new DeploymentParameters()
        {
            SaveAlternativeFormLayout = chkSaveAltFormLayouts.Checked,
            SaveFormLayout = chkSaveFormLayouts.Checked,
            SavePageTemplate = chkSavePageTemplate.Checked,
            SaveCss = chkSaveCSS.Checked,
            SaveLayout = chkSaveLayouts.Checked,
            SaveTransformation = chkSaveTransformation.Checked,
            SaveWebPartContainer = chkSaveWebpartContainer.Checked,
            SaveWebPartLayout = chkSaveWebpartLayout.Checked
        };
    }


    /// <summary>
    /// Encapsulates the action with try catch block and logging
    /// </summary>
    /// <param name="action">Action to execute</param>
    /// <param name="cancelResString">Optional resource string used for canceled info message</param>
    private void RunWithTryCatch(Action action, string cancelResString = "Deployment.DeploymentCanceled")
    {
        try
        {
            action();
        }
        catch (ThreadAbortException ex)
        {
            string state = ValidationHelper.GetString(ex.ExceptionState, string.Empty);
            if (state == CMSThread.ABORT_REASON_STOP)
            {
                // When canceled
                AddError(ResHelper.GetString(cancelResString));
            }
            else
            {
                // Log error
                LogExceptionToEventLog(ex);
            }
        }
        catch (IOExceptions.IOException ex)
        {
            LogExceptionToEventLog(ex);
        }
        catch (Exception ex)
        {
            // Log error
            LogExceptionToEventLog(ex);
        }
    }


    private void Deploy(object parameter)
    {
        RunWithTryCatch(() =>
        {
            CurrentDeploymentManager.Deploy(parameter as DeploymentParameters);
        });
    }


    private void Test(object parameter)
    {
        RunWithTryCatch(() =>
        {
            CurrentDeploymentManager.CompileVirtualObjects(null);
        }, "general.actioncanceled");
    }


    private void SaveExternally(object parameter)
    {
        RunWithTryCatch(() =>
        {
            CurrentDeploymentManager.SaveExternally(parameter as DeploymentParameters);
        });
    }


    private void Synchronize(object parameter)
    {
        RunWithTryCatch(() =>
        {
            CurrentDeploymentManager.Synchronize(parameter as DeploymentParameters);
        });
    }

    #endregion


    #region "Deployment manager events"

    private void CurrentDeploymentManager_Error(object sender, DeploymentManagerLogEventArgs e)
    {
        AddError(e.Message);
    }

    private void CurrentDeploymentManager_Log(object sender, DeploymentManagerLogEventArgs e)
    {
        AddLog(e.Message);
    }

    #endregion


    #region "Async methods"
     
    /// <summary>
    /// When exception occurs, log it to event log.
    /// </summary>
    private void LogExceptionToEventLog(Exception ex)
    {
        EventLogProvider.LogEvent(EventType.ERROR, "System deployment", "DEPLOYMENT", EventLogProvider.GetExceptionLogMessage(ex), RequestContext.RawURL, CurrentUser.UserID, CurrentUser.UserName, 0, null, RequestContext.UserHostAddress, SiteContext.CurrentSiteID);
        AddError(ResHelper.GetString("Deployment.DeploymentFailed") + ": " + ex.Message);
    }


    private void ctlAsyncLog_OnCancel(object sender, EventArgs e)
    {
        pnlLog.Visible = false;

        ctlAsyncLog.Parameter = null;
        string cancel = GetString("general.actioncanceled");
        AddLog(cancel);
        ltlScript.Text += ScriptHelper.GetScript("var __pendingCallbacks = new Array(); RefreshCurrent();");

        CurrentLog.Close();

        if (!string.IsNullOrEmpty(CurrentError))
        {
            ShowError(CurrentError);
            return;
        }

        ShowConfirmation(cancel);
    }


    private void ctlAsyncLog_OnRequestLog(object sender, EventArgs e)
    {
        ctlAsyncLog.LogContext = CurrentLog;
    }


    private void ctlAsyncLog_OnError(object sender, EventArgs e)
    {
        if (ctlAsyncLog.Status == AsyncWorkerStatusEnum.Running)
        {
            ctlAsyncLog.Stop();
        }
        ctlAsyncLog.Parameter = null;
        ShowError(CurrentError);
        CurrentLog.Close();

        pnlLog.Visible = false;
    }


    private void ctlAsyncLog_OnFinished(object sender, EventArgs e)
    {
        CurrentLog.Close();
        pnlLog.Visible = false;

        if (!string.IsNullOrEmpty(CurrentError))
        {
            ctlAsyncLog.Parameter = null;
            ShowError(CurrentError);
            return;
        }

        if (SettingsKeyInfoProvider.DeploymentMode)
        {
            ShowConfirmation(GetString("Deployment.ObjectsSavedSuccessfully"));
        }
        else
        {
            ShowChangesSaved();
        }
    }


    /// <summary>
    /// Ensures the logging context.
    /// </summary>
    protected LogContext EnsureLog()
    {
        return LogContext.EnsureLog(ctlAsyncLog.ProcessGUID);
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
        CurrentError = (error + "<br />" + CurrentError);
    }

    #endregion
}