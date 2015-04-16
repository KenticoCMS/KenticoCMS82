using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading;
using System.Web.UI.WebControls;
using System.Data;
using System.Collections;

using CMS.CustomTables;
using CMS.DataEngine;
using CMS.EventLog;
using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.Base;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.Synchronization;

[UIElement("CMS.Staging", "Data")]
public partial class CMSModules_Staging_Tools_Data_Tasks : CMSStagingPage
{
    #region "Protected variables"

    // Header action event name
    private const string SYNCHRONIZE_CURRENT = "SYNCCURRENTDATA";

    /// <summary>
    /// Message storage for async control
    /// </summary>
    protected static Hashtable mInfos = new Hashtable();

    private int serverId = 0;
    private string eventCode = null;
    private string eventType = null;

    protected CurrentUserInfo currentUser = null;
    protected GeneralConnection mConnection = null;

    protected string objectType = string.Empty;

    protected int currentSiteId = 0;

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
    public string CurrentError
    {
        get
        {
            return ValidationHelper.GetString(mInfos["SyncError_" + ctlAsyncLog.ProcessGUID], string.Empty);
        }
        set
        {
            mInfos["SyncError_" + ctlAsyncLog.ProcessGUID] = value;
        }
    }


    /// <summary>
    /// Current Info.
    /// </summary>
    public string CurrentInfo
    {
        get
        {
            return ValidationHelper.GetString(mInfos["SyncInfo_" + ctlAsyncLog.ProcessGUID], string.Empty);
        }
        set
        {
            mInfos["SyncInfo_" + ctlAsyncLog.ProcessGUID] = value;
        }
    }


    /// <summary>
    /// Gets or sets the cancel string.
    /// </summary>
    public string CanceledString
    {
        get
        {
            return ValidationHelper.GetString(mInfos["SyncCancel_" + ctlAsyncLog.ProcessGUID], string.Empty);
        }
        set
        {
            mInfos["SyncCancel_" + ctlAsyncLog.ProcessGUID] = value;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Initialize current user for the async actions
        currentUser = MembershipContext.AuthenticatedUser;

        // Check 'Manage data tasks' permission
        if (!currentUser.IsAuthorizedPerResource("cms.staging", "ManageDataTasks"))
        {
            RedirectToAccessDenied("cms.staging", "ManageDataTasks");
        }

        // Check enabled servers
        if (!ServerInfoProvider.IsEnabledServer(SiteContext.CurrentSiteID))
        {
            ShowInformation(GetString("ObjectStaging.NoEnabledServer"));
            CurrentMaster.PanelHeader.Visible = false;
            plcContent.Visible = false;
            pnlFooter.Visible = false;
            return;
        }

        // Register script for pendingCallbacks repair
        ScriptHelper.FixPendingCallbacks(Page);

        // Initialize current site ID
        currentSiteId = SiteContext.CurrentSiteID;

        // Setup server dropdown
        selectorElem.DropDownList.AutoPostBack = true;
        selectorElem.UniSelector.OnSelectionChanged += UniSelector_OnSelectionChanged;

        // Set server ID
        serverId = ValidationHelper.GetInteger(selectorElem.Value, QueryHelper.GetInteger("serverId", 0));

        // All servers
        if (serverId == UniSelector.US_ALL_RECORDS)
        {
            serverId = 0;
            selectorElem.Value = UniSelector.US_ALL_RECORDS;
        }
        else
        {
            selectorElem.Value = serverId.ToString();
        }

        ltlScript.Text += ScriptHelper.GetScript("var currentServerId = " + serverId + ";");

        HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;

        if (ControlsHelper.CausedPostBack(btnSyncComplete))
        {
            SyncComplete();
        }
        else
        {
            if (!RequestHelper.IsCallback())
            {
                // Check 'Manage object tasks' permission
                if (!currentUser.IsAuthorizedPerResource("cms.staging", "ManageDataTasks"))
                {
                    RedirectToAccessDenied("cms.staging", "ManageDataTasks");
                }

                ucDisabledModule.SettingsKeys = "CMSStagingLogDataChanges";
                ucDisabledModule.InfoTexts.Add(GetString("DataStaging.TaskSeparator"));
                ucDisabledModule.ParentPanel = pnlNotLogged;

                // Check logging
                if (!ucDisabledModule.Check())
                {
                    CurrentMaster.PanelHeader.Visible = false;
                    plcContent.Visible = false;
                    pnlFooter.Visible = false;
                    return;
                }

                // Register the dialog script
                ScriptHelper.RegisterDialogScript(this);

                // Get object type
                objectType = QueryHelper.GetString("objecttype", string.Empty);
                if (!String.IsNullOrEmpty(objectType))
                {
                    // Create header action
                    HeaderActions.AddAction(new HeaderAction
                    {
                        Text = GetString("ObjectTasks.SyncCurrent"),
                        EventName = SYNCHRONIZE_CURRENT
                    });

                    // Add CSS class to panels wrapper in order it could be stacked
                    CurrentMaster.PanelHeader.AddCssClass("header-container-multiple-panels");

                    // Set Complete synchronization button as default
                    btnComplete.ButtonStyle = ButtonStyle.Default;
                }

                // Setup title
                ctlAsyncLog.TitleText = GetString("Synchronization.Title");
                if (!ControlsHelper.CausedPostBack(HeaderActions, btnSyncSelected, btnSyncAll))
                {
                    plcContent.Visible = true;

                    // Initialize buttons
                    btnDeleteAll.OnClientClick = "return confirm(" + ScriptHelper.GetString(GetString("Tasks.ConfirmDeleteAll")) + ");";
                    btnDeleteSelected.OnClientClick = "return confirm(" + ScriptHelper.GetString(GetString("general.confirmdelete")) + ");";
                    btnSyncSelected.OnClientClick = "return !" + gridTasks.GetCheckSelectionScript();

                    ltlScript.Text += ScriptHelper.GetScript("function CompleteSync(){" + Page.ClientScript.GetPostBackEventReference(btnSyncComplete, null) + "}");

                    // Initialize grid
                    gridTasks.OrderBy = "TaskTime";
                    gridTasks.ZeroRowsText = GetString("Tasks.NoTasks");
                    gridTasks.OnAction += gridTasks_OnAction;
                    gridTasks.OnDataReload += gridTasks_OnDataReload;
                    gridTasks.OnExternalDataBound += gridTasks_OnExternalDataBound;
                    gridTasks.ShowActionsMenu = true;
                    gridTasks.Columns = "TaskID, TaskSiteID, TaskDocumentID, TaskNodeAliasPath, TaskTitle, TaskTime, TaskType, TaskObjectType, TaskObjectID, TaskRunning, (SELECT COUNT(*) FROM Staging_Synchronization WHERE SynchronizationTaskID = TaskID AND SynchronizationErrorMessage IS NOT NULL AND (SynchronizationServerID = @ServerID OR (@ServerID = 0 AND (@TaskSiteID = 0 OR SynchronizationServerID IN (SELECT ServerID FROM Staging_Server WHERE ServerSiteID = @TaskSiteID AND ServerEnabled=1))))) AS FailedCount";
                    StagingTaskInfo ti = new StagingTaskInfo();
                    gridTasks.AllColumns = SqlHelper.MergeColumns(ti.ColumnNames);

                    pnlLog.Visible = false;
                }
            }
        }

        ctlAsyncLog.OnFinished += ctlAsyncLog_OnFinished;
        ctlAsyncLog.OnError += ctlAsyncLog_OnError;
        ctlAsyncLog.OnRequestLog += ctlAsyncLog_OnRequestLog;
        ctlAsyncLog.OnCancel += ctlAsyncLog_OnCancel;
    }

    #endregion


    #region "Grid events & methods"

    protected void gridTasks_OnAction(string actionName, object actionArgument)
    {
        // Parse action argument
        int taskId = ValidationHelper.GetInteger(actionArgument, 0);
        eventType = EventType.INFORMATION;

        if (taskId > 0)
        {
            StagingTaskInfo task = StagingTaskInfoProvider.GetTaskInfo(taskId);

            if (task != null)
            {
                switch (actionName.ToLowerCSafe())
                {
                    case "delete":
                        // Delete task
                        eventCode = "DELETESELECTEDDATA";
                        AddEventLog(string.Format(ResHelper.GetAPIString("deletion.running", "Deleting '{0}' task"), HTMLHelper.HTMLEncode(task.TaskTitle)));
                        SynchronizationInfoProvider.DeleteSynchronizationInfo(taskId, serverId, currentSiteId);
                        break;

                    case "synchronize":
                        string result;
                        try
                        {
                            // Run task synchronization
                            eventCode = "SYNCSELECTEDDATA";
                            result = StagingHelper.RunSynchronization(taskId, serverId, true, currentSiteId);

                            if (string.IsNullOrEmpty(result))
                            {
                                ShowConfirmation(GetString("Tasks.SynchronizationOK"));
                            }
                            else
                            {
                                ShowError(GetString("Tasks.SynchronizationFailed"));
                                eventType = EventType.ERROR;
                            }
                        }
                        catch (Exception ex)
                        {
                            result = ex.Message;
                            ShowError(GetString("Tasks.SynchronizationFailed"));
                            eventType = EventType.ERROR;
                        }
                        // Log message
                        AddEventLog(result + string.Format(ResHelper.GetAPIString("synchronization.running", "Processing '{0}' task"), HTMLHelper.HTMLEncode(task.TaskTitle)));
                        break;
                }
            }
        }
    }


    protected object gridTasks_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        DataRowView drv = null;
        int taskId = 0;
        switch (sourceName.ToLowerCSafe())
        {
            case "taskresult":
                drv = parameter as DataRowView;
                int failedCount = ValidationHelper.GetInteger(drv["FailedCount"], 0);
                taskId = ValidationHelper.GetInteger(drv["TaskID"], 0);
                return GetResultLink(failedCount, taskId);

            case "view":
                if (sender is CMSGridActionButton)
                {
                    // Add view JavaScript
                    CMSGridActionButton btnView = (CMSGridActionButton)sender;
                    drv = UniGridFunctions.GetDataRowView((DataControlFieldCell)btnView.Parent);
                    taskId = ValidationHelper.GetInteger(drv["TaskID"], 0);
                    string url = ScriptHelper.ResolveUrl(String.Format("~/CMSModules/Staging/Tools/View.aspx?taskid={0}&tasktype=Data&hash={1}", taskId, QueryHelper.GetHash("?taskid=" + taskId + "&tasktype=Data")));
                    btnView.OnClientClick = "window.open('" + url + "');return false;";
                    return btnView;
                }
                else
                {
                    return string.Empty;
                }

            case "tasktime":
                return DateTime.Parse(parameter.ToString()).ToString();
        }
        return parameter;
    }


    protected DataSet gridTasks_OnDataReload(string completeWhere, string currentOrder, int currentTopN, string columns, int currentOffset, int currentPageSize, ref int totalRecords)
    {
        // Get the tasks
        string where = "TaskSiteID IS NULL";
        string classWhere = string.Empty;

        // Ensure only data task selection
        if (string.IsNullOrEmpty(objectType))
        {
            DataSet dsTables = CustomTableHelper.GetCustomTableClasses(currentSiteId).Column("ClassName");
            if (!DataHelper.DataSourceIsEmpty(dsTables))
            {
                foreach (DataRow dr in dsTables.Tables[0].Rows)
                {
                    classWhere += "N'" + SqlHelper.EscapeQuotes(CustomTableItemProvider.GetObjectType(dr["ClassName"].ToString())) + "',";
                }
                classWhere = classWhere.TrimEnd(new [] { ',' });
            }
            where = SqlHelper.AddWhereCondition(where, "TaskObjectType IN (" + classWhere + ")");
        }

        DataSet ds = null;

        // There are some custom tables assigned to the site, get the data
        if (!string.IsNullOrEmpty(classWhere) || !string.IsNullOrEmpty(objectType))
        {
            ds = StagingTaskInfoProvider.SelectObjectTaskList(currentSiteId, serverId, objectType, where, currentOrder, 0, columns, currentOffset, currentPageSize, ref totalRecords);
        }
        else
        {
            totalRecords = -1;
        }

        pnlFooter.Visible = (totalRecords > 0);
        return ds;
    }


    /// <summary>
    /// Returns the result link for the synchronization log.
    /// </summary>
    /// <param name="failedCount">Failed items count</param>
    /// <param name="taskId">Task ID</param>
    protected string GetResultLink(object failedCount, object taskId)
    {
        int count = ValidationHelper.GetInteger(failedCount, 0);
        if (count > 0)
        {
            string logUrl = ResolveUrl(String.Format("~/CMSModules/Staging/Tools/log.aspx?taskid={0}&serverId={1}&tasktype=Data", taskId, serverId));
            logUrl = URLHelper.AddParameterToUrl(logUrl, "hash", QueryHelper.GetHash(logUrl));
            return "<a target=\"_blank\" href=\"" + logUrl + "\" onclick=\"modalDialog('" + logUrl + "', 'tasklog', 700, 500); return false;\">" + GetString("Tasks.ResultFailed") + "</a>";
        }
        else
        {
            return string.Empty;
        }
    }


    protected void UniSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        gridTasks.ReloadData();
        pnlUpdate.Update();

        ScriptHelper.RegisterStartupScript(this, typeof(string), "changeServer", ScriptHelper.GetScript("ChangeServer(" + serverId + ");"));
    }

    #endregion


    #region "Async methods"

    /// <summary>
    /// Complete synchronization.
    /// </summary>
    public void SynchronizeComplete(object parameter)
    {
        string result = null;
        eventCode = "SYNCCOMPLETEDATA";
        CanceledString = GetString("Tasks.SynchronizationCanceled");
        try
        {
            int sid = serverId;
            if (sid <= 0)
            {
                sid = SynchronizationInfoProvider.ENABLED_SERVERS;
            }

            AddLog(GetString("Synchronization.LoggingTasks"));

            // Get custom tables object types
            string objectTypes = string.Empty;
            DataSet dsTables = CustomTableHelper.GetCustomTableClasses(currentSiteId).Column("ClassName");
            if (!DataHelper.DataSourceIsEmpty(dsTables))
            {
                DataTable table = dsTables.Tables[0];
                foreach (DataRow dr in table.Rows)
                {
                    objectTypes += CustomTableItemProvider.GetObjectType(dr["ClassName"].ToString()) + ";";
                }
            }

            // Create update tasks
            SynchronizationHelper.LogObjectChange(objectTypes.Trim(';'), 0, DateTimeHelper.ZERO_TIME, TaskTypeEnum.UpdateObject, true, false, false, false, false, currentSiteId, sid);

            AddLog(GetString("Synchronization.RunningTasks"));

            // Get the tasks
            DataSet ds = GetStagingDataTasks();

            // Run the synchronization
            result = StagingHelper.RunSynchronization(ds, serverId, true, currentSiteId, AddLog);

            // Log possible errors
            if (!string.IsNullOrEmpty(result))
            {
                CurrentError = GetString("Tasks.SynchronizationFailed");
                AddErrorLog(CurrentError, null);
            }
            else
            {
                CurrentInfo = GetString("Tasks.SynchronizationOK");
                AddLog(CurrentInfo);
            }
        }
        catch (ThreadAbortException ex)
        {
            string state = ValidationHelper.GetString(ex.ExceptionState, string.Empty);
            if (state == CMSThread.ABORT_REASON_STOP)
            {
                // Canceled by user
                CurrentInfo = CanceledString;
                AddLog(CurrentInfo);
            }
            else
            {
                CurrentError = GetString("Tasks.SynchronizationFailed");
                AddErrorLog(CurrentError, result);
            }
        }
        catch (Exception ex)
        {
            CurrentError = GetString("Tasks.SynchronizationFailed") + ": " + ex.Message;
            AddErrorLog(CurrentError);
        }
        finally
        {
            // Finalize log context
            FinalizeContext();
        }
    }


    /// <summary>
    /// All items synchronization.
    /// </summary>
    protected void SynchronizeAll(object parameter)
    {
        string result = string.Empty;
        eventCode = "SYNCALLDATA";
        CanceledString = GetString("Tasks.SynchronizationCanceled");
        try
        {
            string where = "TaskSiteID IS NULL";
            string classWhere = string.Empty;

            AddLog(GetString("Synchronization.RunningTasks"));

            // Ensure only data task selection
            if (string.IsNullOrEmpty(objectType))
            {
                DataSet dsTables = CustomTableHelper.GetCustomTableClasses(currentSiteId).Column("ClassName");
                if (!DataHelper.DataSourceIsEmpty(dsTables))
                {
                    foreach (DataRow dr in dsTables.Tables[0].Rows)
                    {
                        classWhere += "N'" + SqlHelper.EscapeQuotes(CustomTableItemProvider.GetObjectType(dr["ClassName"].ToString())) + "',";
                    }
                    classWhere = classWhere.TrimEnd(new [] { ',' });
                }
                where = SqlHelper.AddWhereCondition(where, "TaskObjectType IN (" + classWhere + ")");
            }

            // Get the tasks
            DataSet ds = StagingTaskInfoProvider.SelectObjectTaskList(currentSiteId, serverId, objectType, where, "TaskID", -1, "TaskID,TaskTitle");

            // Run the synchronization
            result = StagingHelper.RunSynchronization(ds, serverId, true, currentSiteId, AddLog);

            // Log possible error
            if (result != string.Empty)
            {
                CurrentError = GetString("Tasks.SynchronizationFailed");
                AddErrorLog(CurrentError, null);
            }
            else
            {
                CurrentInfo = GetString("Tasks.SynchronizationOK");
                AddLog(CurrentInfo);
            }
        }
        catch (ThreadAbortException ex)
        {
            string state = ValidationHelper.GetString(ex.ExceptionState, string.Empty);
            if (state == CMSThread.ABORT_REASON_STOP)
            {
                // Canceled by user
                CurrentInfo = CanceledString;
                AddLog(CurrentInfo);
            }
            else
            {
                CurrentError = GetString("Tasks.SynchronizationFailed");
                AddErrorLog(CurrentError, result);
            }
        }
        catch (Exception ex)
        {
            CurrentError = GetString("Tasks.SynchronizationFailed") + ": " + ex.Message;
            AddErrorLog(CurrentError);
        }
        finally
        {
            // Finalize log context
            FinalizeContext();
        }
    }


    /// <summary>
    /// Synchronization of selected items.
    /// </summary>
    /// <param name="parameter">List of selected items</param>
    public void SynchronizeSelected(object parameter)
    {
        List<String> list = parameter as List<String>;
        if (list == null)
        {
            return;
        }

        string result = string.Empty;
        eventCode = "SYNCSELECTEDDATA";
        CanceledString = GetString("Tasks.SynchronizationCanceled");
        try
        {
            AddLog(GetString("Synchronization.RunningTasks"));

            // Run the synchronization
            result = StagingHelper.RunSynchronization(list, serverId, true, currentSiteId, AddLog);

            // Log possible error
            if (result != string.Empty)
            {
                CurrentError = GetString("Tasks.SynchronizationFailed");
                AddErrorLog(CurrentError, null);
            }
            else
            {
                CurrentInfo = GetString("Tasks.SynchronizationOK");
                AddLog(CurrentInfo);
            }
        }
        catch (ThreadAbortException ex)
        {
            string state = ValidationHelper.GetString(ex.ExceptionState, string.Empty);
            if (state == CMSThread.ABORT_REASON_STOP)
            {
                // Canceled by user
                CurrentInfo = CanceledString;
                AddLog(CurrentInfo);
            }
            else
            {
                CurrentError = GetString("Tasks.SynchronizationFailed");
                AddErrorLog(CurrentError, result);
            }
        }
        catch (Exception ex)
        {
            CurrentError = GetString("Tasks.SynchronizationFailed") + ": " + ex.Message;
            AddErrorLog(CurrentError);
        }
        finally
        {
            // Finalize log context
            FinalizeContext();
        }
    }


    /// <summary>
    /// Synchronizes the current object type.
    /// </summary>
    private void SynchronizeCurrent(object parameter)
    {
        string result = string.Empty;
        eventCode = SYNCHRONIZE_CURRENT;
        CanceledString = GetString("Tasks.SynchronizationCanceled");
        try
        {
            int sid = serverId;
            if (sid <= 0)
            {
                sid = SynchronizationInfoProvider.ENABLED_SERVERS;
            }

            AddLog(GetString("Synchronization.LoggingTasks"));

            // Create update tasks
            SynchronizationHelper.LogObjectChange(objectType, 0, DateTimeHelper.ZERO_TIME, TaskTypeEnum.UpdateObject, true, false, false, false, false, currentSiteId, sid);

            AddLog(GetString("Synchronization.RunningTasks"));

            // Get the tasks
            DataSet ds = GetStagingDataTasks();

            // Run the synchronization
            result = StagingHelper.RunSynchronization(ds, serverId, true, currentSiteId, AddLog);

            // Log possible error
            if (!string.IsNullOrEmpty(result))
            {
                CurrentError = GetString("Tasks.SynchronizationFailed");
                AddErrorLog(CurrentError, null);
            }
            else
            {
                CurrentInfo = GetString("Tasks.SynchronizationOK");
                AddLog(CurrentInfo);
            }
        }
        catch (ThreadAbortException ex)
        {
            string state = ValidationHelper.GetString(ex.ExceptionState, string.Empty);
            if (state == CMSThread.ABORT_REASON_STOP)
            {
                // Canceled by user
                CurrentInfo = CanceledString;
                AddLog(CurrentInfo);
            }
            else
            {
                CurrentError = GetString("Tasks.SynchronizationFailed");
                AddErrorLog(CurrentError, result);
            }
        }
        catch (Exception ex)
        {
            CurrentError = GetString("Tasks.SynchronizationFailed") + ": " + ex.Message;
            AddErrorLog(CurrentError);
        }
        finally
        {
            // Finalizes log context
            FinalizeContext();
        }
    }


    /// <summary>
    /// Deletes selected tasks.
    /// </summary>
    protected void DeleteSelected(object parameter)
    {
        List<String> list = (List<String>)parameter;
        if (list == null)
        {
            return;
        }

        eventCode = "DELETESELECTEDDATA";
        CanceledString = GetString("Tasks.DeletionCanceled");
        try
        {
            AddLog(GetString("Synchronization.DeletingTasks"));

            foreach (string taskIdString in list)
            {
                int taskId = ValidationHelper.GetInteger(taskIdString, 0);
                if (taskId > 0)
                {
                    StagingTaskInfo task = StagingTaskInfoProvider.GetTaskInfo(taskId);

                    if (task != null)
                    {
                        AddLog(string.Format(ResHelper.GetAPIString("deletion.running", "Deleting '{0}' task"), HTMLHelper.HTMLEncode(task.TaskTitle)));
                        // Delete synchronization
                        SynchronizationInfoProvider.DeleteSynchronizationInfo(task.TaskID, serverId, currentSiteId);
                    }
                }
            }

            CurrentInfo = GetString("Tasks.DeleteOK");
            AddLog(CurrentInfo);
        }
        catch (ThreadAbortException ex)
        {
            string state = ValidationHelper.GetString(ex.ExceptionState, string.Empty);
            if (state == CMSThread.ABORT_REASON_STOP)
            {
                // Canceled by user
                CurrentInfo = CanceledString;
                AddLog(CurrentInfo);
            }
            else
            {
                CurrentError = GetString("Tasks.DeletionFailed");
                AddErrorLog(CurrentError);
            }
        }
        catch (Exception ex)
        {
            CurrentError = GetString("Tasks.DeletionFailed") + ": " + ex.Message;
            AddErrorLog(CurrentError);
        }
        finally
        {
            // Finalize log context
            FinalizeContext();
        }
    }


    /// <summary>
    /// Deletes all tasks.
    /// </summary>
    protected void DeleteAll(object parameter)
    {
        eventCode = "DELETEALLDATA";
        CanceledString = GetString("Tasks.DeletionCanceled");
        try
        {
            AddLog(GetString("Synchronization.DeletingTasks"));

            string where = "TaskSiteID IS NULL";
            string classWhere = string.Empty;

            // Ensure only data task selection
            if (string.IsNullOrEmpty(objectType))
            {
                DataSet dsTables = CustomTableHelper.GetCustomTableClasses(currentSiteId).Column("ClassName");
                if (!DataHelper.DataSourceIsEmpty(dsTables))
                {
                    foreach (DataRow dr in dsTables.Tables[0].Rows)
                    {
                        classWhere += "N'" + SqlHelper.EscapeQuotes(CustomTableItemProvider.GetObjectType(dr["ClassName"].ToString())) + "',";
                    }
                    classWhere = classWhere.TrimEnd(new char[] { ',' });
                }
                where = SqlHelper.AddWhereCondition(where, "TaskObjectType IN (" + classWhere + ")");
            }

            // Get the tasks
            DataSet ds = StagingTaskInfoProvider.SelectObjectTaskList(currentSiteId, serverId, objectType, where, "TaskID", 0, "TaskID, TaskTitle");
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    int taskId = ValidationHelper.GetInteger(row["TaskID"], 0);
                    if (taskId > 0)
                    {
                        string taskTitle = ValidationHelper.GetString(row["TaskTitle"], null);
                        AddLog(string.Format(ResHelper.GetAPIString("deletion.running", "Deleting '{0}' task"), HTMLHelper.HTMLEncode(taskTitle)));
                        // Delete synchronization
                        SynchronizationInfoProvider.DeleteSynchronizationInfo(taskId, serverId, currentSiteId);
                    }
                }
            }

            CurrentInfo = GetString("Tasks.DeleteOK");
            AddLog(CurrentInfo);
        }
        catch (ThreadAbortException ex)
        {
            string state = ValidationHelper.GetString(ex.ExceptionState, string.Empty);
            if (state == CMSThread.ABORT_REASON_STOP)
            {
                // Canceled by user
                CurrentInfo = CanceledString;
                AddLog(CurrentInfo);
            }
            else
            {
                CurrentError = GetString("Tasks.DeletionFailed");
                AddErrorLog(CurrentError);
            }
        }
        catch (Exception ex)
        {
            CurrentError = GetString("Tasks.DeletionFailed") + ": " + ex.Message;
            AddErrorLog(CurrentError);
        }
        finally
        {
            // Finalize log context
            FinalizeContext();
        }
    }


    private DataSet GetStagingDataTasks()
    {
        return StagingTaskInfoProvider.SelectObjectTaskList(currentSiteId, serverId, objectType, "TaskSiteID IS NULL", "TaskID", -1, "TaskID,TaskTitle");
    }

    #endregion


    #region "Button handling"

    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        if (e.CommandName == SYNCHRONIZE_CURRENT)
        {
            ctlAsyncLog.TitleText = GetString("Synchronization.Title");

            // Run asynchronous action
            RunAsync(SynchronizeCurrent);
        }
    }


    protected void btnSyncSelected_Click(object sender, EventArgs e)
    {
        if (gridTasks.SelectedItems.Count > 0)
        {
            ctlAsyncLog.TitleText = GetString("Synchronization.Title");
            ctlAsyncLog.Parameter = gridTasks.SelectedItems;

            // Run asynchronous action
            RunAsync(SynchronizeSelected);
        }
    }


    private void SyncComplete()
    {
        ctlAsyncLog.TitleText = GetString("Synchronization.Title");

        // Run asynchronous action
        RunAsync(SynchronizeComplete);
    }


    protected void btnSyncAll_Click(object sender, EventArgs e)
    {
        ctlAsyncLog.TitleText = GetString("Synchronization.Title");

        // Run asynchronous action
        RunAsync(SynchronizeAll);
    }


    protected void btnDeleteAll_Click(object sender, EventArgs e)
    {
        ctlAsyncLog.TitleText = GetString("Synchronization.DeletingTasksTitle");

        // Run asynchronous action
        RunAsync(DeleteAll);
    }


    protected void btnDeleteSelected_Click(object sender, EventArgs e)
    {
        if (gridTasks.SelectedItems.Count > 0)
        {
            ctlAsyncLog.TitleText = GetString("Synchronization.DeletingTasksTitle");
            ctlAsyncLog.Parameter = gridTasks.SelectedItems;

            // Run asynchronous action
            RunAsync(DeleteSelected);
        }
    }

    #endregion


    #region "Async processing"

    protected void ctlAsyncLog_OnRequestLog(object sender, EventArgs e)
    {
        // Set current log
        ctlAsyncLog.LogContext = CurrentLog;
    }


    protected void ctlAsyncLog_OnError(object sender, EventArgs e)
    {
        // Handle error
        if (!String.IsNullOrEmpty(CurrentError))
        {
            ShowError(CurrentError);
        }
        if (!String.IsNullOrEmpty(CurrentInfo))
        {
            ShowConfirmation(CurrentInfo);
        }
        FinalizeContext();
    }


    protected void ctlAsyncLog_OnFinished(object sender, EventArgs e)
    {
        if (!String.IsNullOrEmpty(CurrentError))
        {
            ShowError(CurrentError);
        }
        if (!String.IsNullOrEmpty(CurrentInfo))
        {
            ShowConfirmation(CurrentInfo);
        }
        FinalizeContext();
    }


    protected void ctlAsyncLog_OnCancel(object sender, EventArgs e)
    {
        CurrentInfo = CanceledString;
        if (!String.IsNullOrEmpty(CurrentError))
        {
            ShowError(CurrentError);
        }
        if (!String.IsNullOrEmpty(CurrentInfo))
        {
            ShowConfirmation(CurrentInfo);
        }
    }


    /// <summary>
    /// Executes given action asynchronously
    /// </summary>
    /// <param name="action">Action to run</param>
    protected void RunAsync(AsyncAction action)
    {
        pnlLog.Visible = true;
        CurrentLog.Close();
        EnsureLog();
        CurrentError = string.Empty;
        CurrentInfo = string.Empty;
        eventType = EventType.INFORMATION;
        plcContent.Visible = false;

        ctlAsyncLog.RunAsync(action, WindowsIdentity.GetCurrent());
    }

    #endregion


    #region "Log handling"

    /// <summary>
    /// Adds the log information.
    /// </summary>
    /// <param name="newLog">New log information</param>
    protected bool AddLog(string newLog)
    {
        EnsureLog();
        AddEventLog(newLog);
        LogContext.AppendLine(newLog);


        return true;
    }


    /// <summary>
    /// Adds the log error.
    /// </summary>
    /// <param name="newLog">New log information</param>
    protected void AddErrorLog(string newLog)
    {
        AddErrorLog(newLog, null);
    }


    /// <summary>
    /// Adds the log error.
    /// </summary>
    /// <param name="newLog">New log information</param>
    /// <param name="errorMessage">Error message</param>
    protected void AddErrorLog(string newLog, string errorMessage)
    {
        LogContext.AppendLine(newLog);

        string logMessage = newLog;
        if (errorMessage != null)
        {
            logMessage = errorMessage + "<br />" + logMessage;
        }
        eventType = EventType.ERROR;

        AddEventLog(logMessage);
    }


    /// <summary>
    /// Adds message to event log object and updates event type.
    /// </summary>
    /// <param name="logMessage">Message to log</param>
    protected void AddEventLog(string logMessage)
    {
        // Log event to event log
        LogContext.LogEvent(eventType, "Staging", eventCode, logMessage,
                            RequestContext.RawURL, currentUser.UserID, currentUser.UserName,
                            0, null, RequestContext.UserHostAddress, currentSiteId, SystemContext.MachineName, RequestContext.URLReferrer, RequestContext.UserAgent, DateTime.Now);
    }


    /// <summary>
    /// Closes log context and causes event log to save.
    /// </summary>
    protected void FinalizeContext()
    {
        // Close current log context
        CurrentLog.Close();
    }


    /// <summary>
    /// Ensures the logging context.
    /// </summary>
    protected LogContext EnsureLog()
    {
        LogContext log = LogContext.EnsureLog(ctlAsyncLog.ProcessGUID);
        log.LogSingleEvents = false;
        return log;
    }

    #endregion
}