using System;
using System.Collections;
using System.Data;
using System.Security.Principal;
using System.Text;
using System.Threading;

using CMS.Base;
using CMS.EventLog;
using CMS.Helpers;
using CMS.OnlineMarketing;
using CMS.UIControls;

public partial class CMSModules_ContactManagement_Pages_Tools_Contact_Delete : CMSContactManagementContactsPage
{
    #region "Private variables"

    private int contactSiteId;
    private static readonly Hashtable mErrors = new Hashtable();
    private Hashtable mParameters;
    private string mReturnScript;
    private int mSiteID;
    private bool issitemanager;
    private DataSet ds;

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
    /// Returns script for returning back to list page.
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
            ctlAsyncLog.OnFinished += ctlAsyncLog_OnFinished;
            ctlAsyncLog.OnError += ctlAsyncLog_OnError;
            ctlAsyncLog.OnRequestLog += ctlAsyncLog_OnRequestLog;
            ctlAsyncLog.OnCancel += ctlAsyncLog_OnCancel;

            ctlAsyncLog.MaxLogLines = 1000;

            issitemanager = ValidationHelper.GetBoolean(Parameters["issitemanager"], false);

            if (!RequestHelper.IsCallback())
            {
                // Setup page title text and image
                PageTitle.TitleText = GetString("om.contact.deletetitle");
                ctlAsyncLog.TitleText = GetString("om.contact.deleting");
                // Set visibility of panels
                pnlContent.Visible = true;
                pnlLog.Visible = false;

                // Get names of deleted contacts
                ds = ContactInfoProvider.GetContacts()
                                        .TopN(500)
                                        .Where(WhereCondition)
                                        .OrderBy("ContactLastName");

                if (!DataHelper.DataSourceIsEmpty(ds))
                {
                    DataRowCollection rows = ds.Tables[0].Rows;

                    // Data set contains only one item...
                    if (rows.Count == 1)
                    {
                        // Get full contact name and use it in the title
                        string fullName = GetFullName(rows[0]);
                        if (!string.IsNullOrEmpty(fullName))
                        {
                            PageTitle.TitleText += " \"" + HTMLHelper.HTMLEncode(fullName) + "\"";
                        }
                        contactSiteId = ValidationHelper.GetInteger(DataHelper.GetDataRowValue(rows[0], "ContactSiteID"), 0);
                    }
                    else if (rows.Count > 1)
                    {
                        // Modify title and question for multiple items
                        PageTitle.TitleText = GetString("om.contact.deletetitlemultiple");
                        headQuestion.ResourceString = "om.contact.deletemultiplequestion";
                        // Display list with names of deleted items
                        pnlContactList.Visible = true;

                        string name = null;
                        StringBuilder builder = new StringBuilder();

                        // Display top 500 records
                        for (int i = 0; i < (rows.Count); i++)
                        {
                            builder.Append("<div>");
                            name = GetFullName(rows[i]);
                            if (!string.IsNullOrEmpty(name))
                            {
                                builder.Append(HTMLHelper.HTMLEncode(name));
                            }
                            else
                            {
                                builder.Append("N/A");
                            }
                            builder.Append("</div>");
                        }
                        // Display three dots after last record
                        if (rows.Count >= 500)
                        {
                            builder.Append("...");
                        }

                        lblContacts.Text = builder.ToString();
                        contactSiteId = SiteID;
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
        ContactHelper.AuthorizedModifyContact(contactSiteId, true);

        EnsureAsyncLog();
        RunAsyncDelete();
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Returns contact name in the form 'lastname firstname' or null.
    /// </summary>
    /// <param name="row">Data row with contact info</param>
    protected string GetFullName(DataRow row)
    {
        string fullName = null;

        if (row != null)
        {
            // Compose full contact name
            fullName = string.Format("{0} {1}", ValidationHelper.GetString(DataHelper.GetDataRowValue(row, "ContactLastName"), string.Empty),
                                     ValidationHelper.GetString(DataHelper.GetDataRowValue(row, "ContactFirstName"), string.Empty)).Trim();
        }

        return fullName;
    }


    /// <summary>
    /// Delete contacts on SQL server.
    /// </summary>
    private void BatchDeleteOnSql()
    {
        while (!DataHelper.DataSourceIsEmpty(ds))
        {
            ContactInfoProvider.DeleteContactInfos(WhereCondition, 200);
            ds = ContactInfoProvider.GetContacts()
                                    .TopN(1)
                                    .Column("ContactID")
                                    .Where(WhereCondition);
        }

        // Return to the list page with info label displayed
        ltlScript.Text += ScriptHelper.GetScript(ReturnScriptDeleteAsync);
    }


    /// <summary>
    /// Delete items one by one.
    /// </summary>
    private void DeleteItems()
    {
        while (!DataHelper.DataSourceIsEmpty(ds))
        {
            // Delete the contacts
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                var ci = new ContactInfo(dr);
                AddLog((ci.ContactLastName + " " + ci.ContactFirstName).Trim());
                ContactHelper.Delete(ci, chkChildren.Checked, chkMoveRelations.Checked);
            }
            ds = ContactInfoProvider.GetContacts()
                                    .TopN(500)
                                    .Where(WhereCondition)
                                    .OrderBy("ContactLastName");
        }
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
    /// Starts asynchronous deleting of contacts.
    /// </summary>
    private void RunAsyncDelete()
    {
        // Run the async method
        ctlAsyncLog.Parameter = ReturnScript;
        ctlAsyncLog.RunAsync(Delete, WindowsIdentity.GetCurrent());
    }

    #endregion


    #region "Async methods"

    /// <summary>
    /// Deletes document(s).
    /// </summary>
    private void Delete(object parameter)
    {
        if ((parameter == null) || (ds.Tables[0].Rows.Count < 1))
        {
            return;
        }

        try
        {
            // Begin log
            AddLog(GetString("om.contact.deleting"));
            AddLog(string.Empty);

            // Mass delete without logging items
            if (chkChildren.Checked && !chkMoveRelations.Checked && (ds.Tables[0].Rows.Count > 1))
            {
                BatchDeleteOnSql();
            }
            // Delete items
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


    private void ctlAsyncLog_OnCancel(object sender, EventArgs e)
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
        if (!String.IsNullOrEmpty(CurrentError))
        {
            ShowError(CurrentError);
        }
        CurrentLog.Close();
    }


    private void ctlAsyncLog_OnFinished(object sender, EventArgs e)
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
        EventLogProvider.LogException("Contact management", "DELETECONTACT", ex);
        AddError(GetString("om.contact.deletefailed") + ": " + ex.Message);
    }

    #endregion
}