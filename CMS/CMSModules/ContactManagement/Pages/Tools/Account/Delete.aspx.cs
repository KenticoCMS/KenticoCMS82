using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Security.Principal;
using System.Text;
using System.Threading;

using CMS.EventLog;
using CMS.Helpers;
using CMS.OnlineMarketing;
using CMS.Base;
using CMS.UIControls;

public partial class CMSModules_ContactManagement_Pages_Tools_Account_Delete : CMSContactManagementAccountsPage
{
    #region "Private variables"

    private IList<string> accountIds = null;
    private int accountSiteId = 0;
    private static readonly Hashtable mErrors = new Hashtable();
    private Hashtable mParameters = null;
    private string mReturnScript = null;
    private int mSiteID = 0;
    private bool issitemanager = false;
    private int numberOfDeletedAccounts = 0;

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
            return ValidationHelper.GetString(mErrors["DeleteError_" + ctlAsyncLog.ProcessGUID], string.Empty);
        }
        set
        {
            mErrors["DeleteError_" + ctlAsyncLog.ProcessGUID] = value;
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
    /// Returns script for returning back to list page.
    /// </summary>
    private string ReturnScript
    {
        get
        {
            if (string.IsNullOrEmpty(mReturnScript) && (Parameters != null))
            {
                mReturnScript = "document.location.href = 'List.aspx?siteid=" + SiteID + (issitemanager ? "&issitemanager=1" : string.Empty) + "';";
            }

            return mReturnScript;
        }
    }


    /// <summary>
    /// Returns script for returning back to list page with information that deleting process has been started.
    /// </summary>
    private string ReturnScriptDeleteAsync
    {
        get
        {
            if (string.IsNullOrEmpty(mReturnScript) && (Parameters != null))
            {
                mReturnScript = "document.location.href = 'List.aspx?siteid=" + SiteID + (issitemanager ? "&issitemanager=1" : string.Empty) + "&deleteasync=1';";
            }

            return mReturnScript;
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
            ;
            return mSiteID;
        }
    }

    #endregion


    #region "Page events"

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

            issitemanager = ValidationHelper.GetBoolean(Parameters["issitemanager"], false);

            if (!RequestHelper.IsCallback())
            {
                // Setup page title text and image
                PageTitle.TitleText = GetString("om.account.deletetitle");

                ctlAsyncLog.TitleText = GetString("om.account.deleting");
                // Set visibility of panels
                pnlContent.Visible = true;
                pnlLog.Visible = false;

                // Get names of the accounts that are to be deleted
                DataSet ds = AccountInfoProvider.GetAccounts()
                                                .Where(WhereCondition)
                                                .OrderBy("AccountName")
                                                .TopN(1000)
                                                .Columns("AccountID", "AccountName", "AccountSiteID");

                if (!DataHelper.DataSourceIsEmpty(ds))
                {
                    DataRowCollection rows = ds.Tables[0].Rows;

                    // Data set contains only one item
                    if (rows.Count == 1)
                    {
                        PageTitle.TitleText += " \"" + HTMLHelper.HTMLEncode(ValidationHelper.GetString(DataHelper.GetDataRowValue(rows[0], "AccountName"), "N/A")) + "\"";
                        accountIds = new List<string>(1);
                        accountIds.Add(ValidationHelper.GetString(DataHelper.GetDataRowValue(rows[0], "AccountID"), string.Empty));
                        accountSiteId = ValidationHelper.GetInteger(DataHelper.GetDataRowValue(rows[0], "AccountSiteID"), 0);
                        numberOfDeletedAccounts = 1;
                    }
                    else if (rows.Count > 1)
                    {
                        // Modify title and question for multiple items
                        PageTitle.TitleText = GetString("om.account.deletetitlemultiple");
                        headQuestion.ResourceString = "om.account.deletemultiplequestion";

                        // Display list with names of deleted items
                        pnlAccountList.Visible = true;

                        string name = null;
                        StringBuilder builder = new StringBuilder();

                        for (int i = 0; i < rows.Count; i++)
                        {
                            name = ValidationHelper.GetString(DataHelper.GetDataRowValue(rows[i], "AccountName"), string.Empty);
                            builder.Append("<div>");
                            builder.Append(HTMLHelper.HTMLEncode(name));
                            builder.Append("</div>");
                        }
                        // Display three dots after last record
                        if (rows.Count == 1000)
                        {
                            builder.Append("...");
                        }

                        lblAccounts.Text = builder.ToString();

                        accountSiteId = SiteID;
                        // Get all IDs of deleted items
                        ds = AccountInfoProvider.GetAccounts()
                                                .Where(WhereCondition)
                                                .OrderBy("AccountID")
                                                .Column("AccountID");

                        accountIds = DataHelper.GetStringValues(ds.Tables[0], "AccountID");
                        numberOfDeletedAccounts = ds.Tables[0].Rows.Count;
                    }
                }
                else
                {
                    // Hide everything
                    pnlContent.Visible = false;
                }
            }
        }
        else
        {
            pnlDelete.Visible = false;
            ShowError(GetString("dialogs.badhashtext"));
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        btnNo.OnClientClick = ReturnScript + "return false;";

        base.OnPreRender(e);
    }

    #endregion


    #region "Button actions"

    protected void btnOK_Click(object sender, EventArgs e)
    {
        // Check permissions
        if (AccountHelper.AuthorizedModifyAccount(accountSiteId, true))
        {
            EnsureAsyncLog();
            RunAsyncDelete();
        }
    }

    #endregion


    #region "Methods"

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
    /// Starts asynchronous deleting of contacts.
    /// </summary>
    private void RunAsyncDelete()
    {
        // Run the async method
        ctlAsyncLog.Parameter = ReturnScript;
        ctlAsyncLog.RunAsync(Delete, WindowsIdentity.GetCurrent());
    }


    /// <summary>
    /// Deletes document(s).
    /// </summary>
    private void Delete(object parameter)
    {
        if (parameter == null || accountIds.Count < 1)
        {
            return;
        }

        try
        {
            // Begin log
            AddLog(GetString("om.account.deleting"));
            AddLog(string.Empty);

            // When deleting children and not removing relations then we can run
            if (chkChildren.Checked && (numberOfDeletedAccounts > 1))
            {
                BatchDeleteOnSql();
            }
            else
            {
                DeleteItems();
            }
        }
        catch (ThreadAbortException ex)
        {
            string state = ValidationHelper.GetString(ex.ExceptionState, string.Empty);
            if (state == CMSThread.ABORT_REASON_STOP)
            {
                // When canceled
                AddError(GetString("om.deletioncanceled"));
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
    /// Delete accounts on SQL server.
    /// </summary>
    private void BatchDeleteOnSql()
    {
        StringBuilder where = new StringBuilder("AccountID IN (");

        // Create where condition for deleting accounts
        int i;
        for (i = 0; i < accountIds.Count; i++)
        {
            where.Append(accountIds[i] + ",");

            // Delete accounts by 100's
            if ((i + 1) % 100 == 0)
            {
                BatchDeleteItems(where);
                where = new StringBuilder("AccountID IN (");
            }
        }

        // Delete rest of the accounts
        if (i % 100 != 0)
        {
            BatchDeleteItems(where);
        }

        // Return to the list page with info label displayed
        ltlScript.Text += ScriptHelper.GetScript(ReturnScriptDeleteAsync);
    }


    /// <summary>
    /// Deletes group of accounts on SQL server.
    /// </summary>
    /// <param name="where">WHERE specifying group of accounts</param>
    private void BatchDeleteItems(StringBuilder where)
    {
        where.Remove(where.Length - 1, 1);
        where.Append(")");
        AccountInfoProvider.DeleteAccountInfos(where.ToString(), chkBranches.Checked);
    }


    /// <summary>
    /// Delete items.
    /// </summary>
    private void DeleteItems()
    {
        // Delete the accounts
        AccountInfo ai = null;
        foreach (string accountId in accountIds)
        {
            ai = AccountInfoProvider.GetAccountInfo(ValidationHelper.GetInteger(accountId, 0));
            if (ai != null)
            {
                // Display name of deleted account
                AddLog(ai.AccountName);

                // Delete account with its dependencies
                AccountHelper.Delete(ai, chkChildren.Checked, chkBranches.Checked);
            }
        }
    }

    #endregion


    #region "Async methods"

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
            ShowError(CurrentError);
        }

        if (ctlAsyncLog.Parameter != null)
        {
            // Return to the list page after successful deletion
            ltlScript.Text += ScriptHelper.GetScript(ctlAsyncLog.Parameter.ToString());
        }
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
        CurrentError = ("<div>" + error + "</div><div>" + CurrentError + "</div>");
    }


    /// <summary>
    /// When exception occurs, log it to event log.
    /// </summary>
    /// <param name="ex">Exception to log</param>
    private void LogExceptionToEventLog(Exception ex)
    {
        EventLogProvider.LogException("Contact management", "DELETEACCOUNT", ex);
        AddError(GetString("om.account.deletefailed") + ": " + ex.Message);
    }

    #endregion
}