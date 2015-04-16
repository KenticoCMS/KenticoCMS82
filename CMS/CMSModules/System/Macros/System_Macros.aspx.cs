using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Principal;

using CMS.DataEngine;
using CMS.EventLog;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.Membership;
using CMS.Base;
using CMS.UIControls;

public partial class CMSModules_System_Macros_System_Macros : GlobalAdminPage
{
    private const string EVENTLOG_SOURCE_REFRESHSECURITYPARAMS = "Macros - Refresh security parameters";

    private NameValueCollection processedObjects = new NameValueCollection();


    /// <summary>
    /// Gets the log context for the async control.
    /// </summary>
    public LogContext AsyncLogContext
    {
        get
        {
            return EnsureAsyncLogContext();
        }
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        InitForm();
        InitAsyncDialog();
    }
    

    #region "Async log"

    /// <summary>
    /// Ensures and returns the log context for the async control.
    /// </summary>
    private LogContext EnsureAsyncLogContext()
    {
        var log = LogContext.EnsureLog(ctlAsyncLog.ProcessGUID);

        return log;
    }


    /// <summary>
    /// Inits the async dialog.
    /// </summary>
    private void InitAsyncDialog()
    {
        ctlAsyncLog.TitleText = GetString("macros.refreshsecurityparams.title");

        ctlAsyncLog.OnRequestLog += (sender, args) =>
        {
            ctlAsyncLog.LogContext = AsyncLogContext;
        };

        ctlAsyncLog.OnCancel += (sender, args) =>
        {
            EventLogProvider.LogEvent(EventType.INFORMATION, (string)ctlAsyncLog.Parameter, "CANCELLED");

            pnlAsyncLog.Visible = false;
            AsyncLogContext.Close();

            ShowConfirmation(GetString("general.actioncanceled"));
        };

        ctlAsyncLog.OnFinished += (sender, args) =>
        {
            EventLogProvider.LogEvent(EventType.INFORMATION, (string)ctlAsyncLog.Parameter, "FINISHED");

            pnlAsyncLog.Visible = false;
            AsyncLogContext.Close();

            ShowConfirmation(GetString("general.actionfinished"));
        };
    }


    /// <summary>
    /// Runs the specified action asynchronously.
    /// </summary>
    /// <param name="actionName">Action name</param>
    /// <param name="action">Action</param>
    private void RunAsync(string actionName, AsyncAction action)
    {
        // Set action name as process parameter
        ctlAsyncLog.Parameter = actionName;

        EnsureAsyncLogContext();

        // Log async action start
        EventLogProvider.LogEvent(EventType.INFORMATION, actionName, "STARTED");

        // Run async action
        ctlAsyncLog.RunAsync(action, WindowsIdentity.GetCurrent());
    }

    #endregion


    #region "Refresh security params"

    /// <summary>
    /// Inits the "Refresh security parameters" form.
    /// </summary>
    private void InitForm()
    {
        // Init old salt text box
        if (chkRefreshAll.Checked)
        {
            txtOldSalt.Enabled = false;
            txtOldSalt.Text = GetString("macros.refreshsecurityparams.refreshalldescription");
        }
        else
        {
            txtOldSalt.Enabled = true;
        }

        chkRefreshAll.CheckedChanged += (sender, args) =>
        {
            // Clear the textbox after enabling it
            if (!chkRefreshAll.Checked)
            {
                txtOldSalt.Text = null;
            }
        };

        // Init new salt text box
        if (chkUseCurrentSalt.Checked)
        {
            txtNewSalt.Enabled = false;

            var customSalt = SettingsHelper.AppSettings[ValidationHelper.APP_SETTINGS_HASH_STRING_SALT];
            if (string.IsNullOrEmpty(customSalt))
            {
                txtNewSalt.Text = GetString("macros.refreshsecurityparams.currentsaltisconnectionstring");
            }
            else
            {
                txtNewSalt.Text = GetString("macros.refreshsecurityparams.currentsaltiscustomvalue");
            }
        }
        else
        {
            txtNewSalt.Enabled = true;
        }

        chkUseCurrentSalt.CheckedChanged += (sender, args) =>
        {
            // Clear the textbox after enabling it
            if (!chkUseCurrentSalt.Checked)
            {
                txtNewSalt.Text = null;
            }
        };

        // Init submit button
        btnRefreshSecurityParams.Text = GetString("macros.refreshsecurityparams");
        btnRefreshSecurityParams.Click += (sender, args) =>
        {
            var oldSaltInput = txtOldSalt.Text.Trim();
            var newSaltInput = txtNewSalt.Text.Trim();

            if (!chkRefreshAll.Checked && string.IsNullOrEmpty(oldSaltInput))
            {
                ShowError(GetString("macros.refreshsecurityparams.oldsaltempty"));
                return;
            }

            if (!chkUseCurrentSalt.Checked && string.IsNullOrEmpty(newSaltInput))
            {
                ShowError(GetString("macros.refreshsecurityparams.newsaltempty"));
                return;
            }

            pnlAsyncLog.Visible = true;
            var objectTypes = Functions.GetObjectTypesWithMacros();

            RunAsync(EVENTLOG_SOURCE_REFRESHSECURITYPARAMS, p => RefreshSecurityParams(objectTypes, oldSaltInput, newSaltInput));
        };
    }


    /// <summary>
    /// Refreshes the security parameters in macros for all the objects of the specified object types.
    /// Signs all the macros with the current user if the old salt is not specified.
    /// </summary>
    /// <param name="objectTypes">Object types</param>
    /// <param name="oldSalt">Old salt </param>
    /// <param name="newSalt">New salt</param>
    private void RefreshSecurityParams(IEnumerable<string> objectTypes, string oldSalt, string newSalt)
    {
        var oldSaltSpecified = !string.IsNullOrEmpty(oldSalt) && !chkRefreshAll.Checked;
        var newSaltSpecified = !string.IsNullOrEmpty(newSalt) && !chkUseCurrentSalt.Checked;

        processedObjects.Clear();
        
        using (CMSActionContext context = new CMSActionContext())
        {
            context.LogEvents = false;
            context.LogSynchronization = false;

            foreach (var objectType in objectTypes)
            {
                var objectTypeResourceKey = TypeHelper.GetObjectTypeResourceKey(objectType);
                var niceObjectType = GetString(objectTypeResourceKey);
                if (niceObjectType == objectTypeResourceKey)
                {
                    if (objectType.StartsWithCSafe("bizformitem.bizform.", true))
                    {
                        DataClassInfo dci = DataClassInfoProvider.GetDataClassInfo(objectType.Substring("bizformitem.".Length));
                        if (dci != null)
                        {
                            niceObjectType = "on-line form " + dci.ClassDisplayName;
                        }
                    }
                    else
                    {
                        niceObjectType = objectType;
                    }
                }

                LogContext.AppendLine(string.Format(GetString("macros.refreshsecurityparams.processing"), niceObjectType));

                try
                {
                    var infos = new InfoObjectCollection(objectType);

                    foreach (var info in infos)
                    {
                        try
                        {
                            bool refreshed = false;
                            if (oldSaltSpecified)
                            {
                                refreshed = MacroSecurityProcessor.RefreshSecurityParameters(info, oldSalt, newSaltSpecified ? newSalt : ValidationHelper.HashStringSalt, true);
                            }
                            else
                            {
                                if (chkRefreshAll.Checked && newSaltSpecified)
                                {
                                    // Do not check integrity, but use new salt
                                    refreshed = MacroSecurityProcessor.RefreshSecurityParameters(info, MembershipContext.AuthenticatedUser.UserName, true, newSalt);
                                }
                                else
                                {
                                    // Do not check integrity, sign everything with current user
                                    refreshed = MacroSecurityProcessor.RefreshSecurityParameters(info, MembershipContext.AuthenticatedUser.UserName, true);
                                }
                            }

                            if (refreshed)
                            {
                                var objectName = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(info.Generalized.ObjectDisplayName));
                                processedObjects.Add(niceObjectType, objectName);
                            }
                        }
                        catch (Exception ex)
                        {
                            string message = "Signing " + TypeHelper.GetNiceObjectTypeName(info.TypeInfo.ObjectType) + " " + info.Generalized.ObjectDisplayName + " failed: " + ex.Message;
                            EventLogProvider.LogEvent(EventType.ERROR, "Import", "MACROSECURITY", message);
                        }
                    }
                }
                catch (Exception e)
                {
                    LogContext.AppendLine(e.Message);
                    EventLogProvider.LogException(EVENTLOG_SOURCE_REFRESHSECURITYPARAMS, "ERROR", e);
                }
            }
        }

        EventLogProvider.LogEvent(EventType.INFORMATION, EVENTLOG_SOURCE_REFRESHSECURITYPARAMS, "PROCESSEDOBJECTS", GetProcessedObjectsForEventLog());
    }


    /// <summary>
    /// Gets the list of processed objects formatted for use in the event log.
    /// </summary>
    private string GetProcessedObjectsForEventLog()
    {
        return processedObjects.AllKeys.SelectMany(processedObjects.GetValues, (k, v) => string.Format("{0} '{1}'", k, v)).Join("<br />");
    }

    #endregion
}
