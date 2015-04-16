using System;
using System.Collections;
using System.Linq;
using System.Security.Principal;
using System.Threading;

using CMS.Base;
using CMS.DataEngine;
using CMS.EventLog;
using CMS.Helpers;
using CMS.OnlineMarketing;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_ContactManagement_Pages_Tools_Activities_Activity_Delete : CMSContactManagementActivitiesPage
{
    #region "Variables"

    private Hashtable mParameters;
    private static readonly Hashtable mErrors = new Hashtable();
    private string mReturnScript;
    private int mSiteID;
    private int mContactID;

    #endregion


    #region "Properties"

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
            return ValidationHelper.GetString(mErrors["DeleteError_" + ctlAsyncLog.ProcessGUID], string.Empty);
        }
        set
        {
            mErrors["DeleteError_" + ctlAsyncLog.ProcessGUID] = value;
        }
    }


    /// <summary>
    /// Returns script for returning back to list page.
    /// </summary>
    private string ReturnScript
    {
        get
        {
            if (string.IsNullOrEmpty(mReturnScript) && (Parameters != null))
            {
                mReturnScript = ValidationHelper.GetString(Parameters["returnlocation"], null);
                if (String.IsNullOrEmpty(mReturnScript))
                {
                    mReturnScript = "document.location.href = 'List.aspx?siteid=" + SiteID + (SiteManager ? "&issitemanager=1" : string.Empty) + "';";
                }
                else
                {
                    mReturnScript = "document.location.href = '" + mReturnScript + "';";
                }
            }

            return mReturnScript;
        }
    }


    /// <summary>
    /// Where condition used for multiple actions.
    /// </summary>
    private string WhereCondition
    {
        get
        {
            string where = string.Empty;
            if (Parameters != null)
            {
                where = ValidationHelper.GetString(Parameters["where"], string.Empty);
            }
            return where;
        }
    }


    /// <summary>
    /// Site ID retrieved from dialog parameters.
    /// </summary>
    public override int SiteID
    {
        get
        {
            if ((mSiteID == 0) && (Parameters != null))
            {
                mSiteID = ValidationHelper.GetInteger(Parameters["siteid"], 0);
            }
            return mSiteID;
        }
    }


    /// <summary>
    /// Site manager flag retrieved from dialog parameters.
    /// </summary>
    private bool SiteManager
    {
        get
        {
            bool issitemanager = false;
            if (Parameters != null)
            {
                issitemanager = ValidationHelper.GetBoolean(Parameters["issitemanager"], false);
            }
            return issitemanager;
        }
    }


    /// <summary>
    /// Contact ID retrieved from dialog parameters.
    /// </summary>
    public int ContactID
    {
        get
        {
            if ((mContactID == 0) && (Parameters != null))
            {
                mContactID = ValidationHelper.GetInteger(Parameters["contactid"], 0);
            }
            return mContactID;
        }
    }

    #endregion


    #region "Page methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Check hash validity
        if (QueryHelper.ValidateHash("hash"))
        {
            // Initialize events
            ctlAsyncLog.OnFinished += ctlAsync_OnFinished;
            ctlAsyncLog.OnError += ctlAsync_OnError;
            ctlAsyncLog.OnRequestLog += ctlAsync_OnRequestLog;
            ctlAsyncLog.OnCancel += ctlAsync_OnCancel;

            ctlAsyncLog.MaxLogLines = 1000;

            pnlContent.Visible = true;
            pnlLog.Visible = false;

            if (!RequestHelper.IsCallback())
            {
                // Setup page title text and image
                PageTitle.TitleText = GetString("om.activity.deletetitle");
                ctlAsyncLog.TitleText = GetString("om.activity.deleting");
            }
        }
        else
        {
            pnlContent.Visible = false;
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        btnNo.OnClientClick = ReturnScript + "return false;";

        base.OnPreRender(e);
    }


    protected void btnOK_Click(object sender, EventArgs e)
    {
        ActivityHelper.AuthorizedManageActivity(SiteID, true);

        EnsureAsyncLog();
        RunAsyncDelete();
    }

    #endregion


    #region "Async control event handlers"

    private void ctlAsync_OnCancel(object sender, EventArgs e)
    {
        ctlAsyncLog.Parameter = null;
        string canceled = GetString("om.deletioncanceled");
        AddLog(canceled);
        ltlScript.Text += ScriptHelper.GetScript("var __pendingCallbacks = new Array();RefreshCurrent();");
        if (!String.IsNullOrEmpty(CurrentError))
        {
            ShowError(CurrentError);
        }
        ShowConfirmation(canceled);
        CurrentLog.Close();
    }


    private void ctlAsync_OnRequestLog(object sender, EventArgs e)
    {
        ctlAsyncLog.LogContext = CurrentLog;
    }


    private void ctlAsync_OnError(object sender, EventArgs e)
    {
        if (ctlAsyncLog.Status == AsyncWorkerStatusEnum.Running)
        {
            ctlAsyncLog.Stop();
        }
        ctlAsyncLog.Parameter = null;
        if (!String.IsNullOrEmpty(CurrentError))
        {
            ShowError(CurrentError);
        }
        CurrentLog.Close();
    }


    private void ctlAsync_OnFinished(object sender, EventArgs e)
    {
        CurrentLog.Close();

        if (!string.IsNullOrEmpty(CurrentError))
        {
            ctlAsyncLog.Parameter = null;
            if (!String.IsNullOrEmpty(CurrentError))
            {
                ShowError(CurrentError);
            }
        }

        if (ctlAsyncLog.Parameter != null)
        {
            // Return to the list page after successful deletion
            ltlScript.Text += ScriptHelper.GetScript(ctlAsyncLog.Parameter.ToString());
        }
    }

    #endregion


    #region "Delete methods"

    /// <summary>
    /// Starts asynchronous deleting of contacts.
    /// </summary>
    private void RunAsyncDelete()
    {
        ctlAsyncLog.Parameter = ReturnScript;
        ctlAsyncLog.RunAsync(Delete, WindowsIdentity.GetCurrent());
    }


    /// <summary>
    /// Deletes activities.
    /// </summary>
    private void Delete(object parameter)
    {
        var whereCondition = new WhereCondition(WhereCondition);
        try
        {
            var restrictedSitesCondition = CheckSitePermissions(whereCondition);
            DeleteActivities(whereCondition.Where(restrictedSitesCondition));
        }
        catch (ThreadAbortException ex)
        {
            string state = ValidationHelper.GetString(ex.ExceptionState, string.Empty);
            if (state != CMSThread.ABORT_REASON_STOP)
            {
                LogExceptionToEventLog(ex);
            }
        }
        catch (Exception ex)
        {
            LogExceptionToEventLog(ex);
        }
    }


    /// <summary>
    /// Checks activity permissions.
    /// Returns restricted sites condition.
    /// </summary>
    private WhereCondition CheckSitePermissions(IWhereCondition whereCondition)
    {
        var restrictedSitesCondition = new WhereCondition();
        var activitiesSites = ActivityInfoProvider.GetActivities()
                                                  .Distinct()
                                                  .Column("ActivitySiteID")
                                                  .Where(whereCondition);
        foreach (var activity in activitiesSites)
        {
            if (!CurrentUser.IsAuthorizedPerObject(PermissionsEnum.Modify, "om.activity", SiteInfoProvider.GetSiteName(activity.ActivitySiteID)))
            {
                SiteInfo notAllowedSite = SiteInfoProvider.GetSiteInfo(activity.ActivitySiteID);
                AddError(String.Format(GetString("accessdeniedtopage.info"), ResHelper.LocalizeString(notAllowedSite.DisplayName)));

                restrictedSitesCondition.WhereNotEquals("ActivitySiteID", activity.ActivitySiteID);
            }
        }

        return restrictedSitesCondition;
    }


    /// <summary>
    /// Delete items.
    /// </summary>
    private void DeleteActivities(IWhereCondition whereCondition)
    {
        var activitiesToDelete = ActivityInfoProvider.GetActivities()
                                                     .Columns("ActivityID", "ActivityType", "ActivityTitle")
                                                     .Where(whereCondition);
        foreach (var activity in activitiesToDelete)
        {
            LogContext.AppendLine(string.Format("{0} - {1}", activity.ActivityTitle, activity.ActivityType));
            ActivityInfoProvider.DeleteActivityInfo(activity);
        }
    }

    #endregion


    #region "Log methods"

    /// <summary>
    /// When exception occurs, log it to event log.
    /// </summary>
    /// <param name="ex">Exception to log</param>
    private void LogExceptionToEventLog(Exception ex)
    {
        EventLogProvider.LogException("Contact management", "DELETEACTIVITY", ex);
        AddError(GetString("om.activity.deletefailed") + ": " + ex.Message);
    }


    /// <summary>
    /// Adds the error to collection of errors.
    /// </summary>
    /// <param name="error">Error message</param>
    protected void AddError(string error)
    {
        AddLog(error);
        CurrentError = ("<div>" + error + "</div><div>" + CurrentError + "</div>");
    }


    /// <summary>
    /// Adds the log information.
    /// </summary>
    /// <param name="newLog">New log information</param>
    private void AddLog(string newLog)
    {
        EnsureLog();
        LogContext.AppendLine(newLog);
    }


    /// <summary>
    /// Ensures log for asynchronous control
    /// </summary>
    private void EnsureAsyncLog()
    {
        pnlLog.Visible = true;
        pnlContent.Visible = false;

        CurrentError = string.Empty;
        CurrentLog.Close();
        EnsureLog();
    }


    /// <summary>
    /// Ensures the logging context.
    /// </summary>
    protected LogContext EnsureLog()
    {
        LogContext log = LogContext.EnsureLog(ctlAsyncLog.ProcessGUID);

        return log;
    }

    #endregion
}