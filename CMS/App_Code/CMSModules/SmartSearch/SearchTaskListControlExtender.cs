using System;
using System.Linq;
using System.Data;

using CMS;
using CMS.Base;
using CMS.ExtendedControls;
using CMS.UIControls;
using CMS.Helpers;
using CMS.Search;
using CMS.EventLog;
using CMS.Localization;
using CMS.ExtendedControls.ActionsConfig;
using CMS.DataEngine;

[assembly: RegisterCustomClass("SearchTaskListControlExtender", typeof(SearchTaskListControlExtender))]

/// <summary>
/// Search task list control extender
/// </summary>
public class SearchTaskListControlExtender : ControlExtender<UniGrid>
{
    #region "Constants"

    private const string PROCESS = "processtasks";
    private const string REFRESH = "refresh";

    #endregion


    #region "Private properties"

    private CMSThread mIndexerThread;
    private HeaderAction mProcessTasksAction;
    private HeaderAction mRefreshAction;

    #endregion


    #region "Private methods"

    /// <summary>
    /// Indexer thread or null if indexer is not running. 
    /// </summary>
    private CMSThread IndexerThread
    {
        get
        {
            return mIndexerThread ?? (mIndexerThread = CMSThread.GetThread(SearchTaskInfoProvider.IndexerThreadGuid));
        }
    }

    /// <summary>
    /// Indicates whether indexer thread is running.
    /// </summary>
    private bool IsIndexerThreadRunning
    {
        get
        {
            return SearchTaskInfoProvider.IsIndexThreadRunning && SearchTaskInfoProvider.IndexerThreadGuid != Guid.Empty;
        }
    }

    /// <summary>
    /// OnInit event handler
    /// </summary>
    public override void OnInit()
    {
        Control.OnExternalDataBound += OnExternalDataBound;

        // Add header actions
        InitHeaderActions();

        // Header action execution occurs in RaisePostBackEvent after Load event and before PreRender. 
        // We need to enable header action and display message after the the action is executed.  
        Control.PreRender += (sender, args) => {

            //Enable header action
            if (mProcessTasksAction != null)
            {
                mProcessTasksAction.Enabled = !IsIndexerThreadRunning && (Control.RowsCount > 0);
                Control.HeaderActions.ReloadData();
            }

            // Displays message when task processor is running
            DisplayTaskRunningMessage();

            ScriptHelper.RegisterDialogScript(Control.Page);
        };
    }


    /// <summary>
    /// OnExternalDataBound event handler
    /// </summary>
    private object OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "result":
                return RenderResult(GetDataRowViewFromParameter(parameter));
            case "taskrelatedobject":
                return RenderRelatedObject(GetDataRowViewFromParameter(parameter));
            case "tasktype":
                string taskType = ValidationHelper.GetString(parameter, String.Empty);
                return Control.GetString("smartsearch.tasktype." + taskType.ToLowerCSafe());
        }

        return null;
    }


    /// <summary>
    /// Return DataRowView from OnExternalDataBound paramater when unigrid column source is ##ALL##.
    /// </summary>
    /// <param name="parameter">OnExternalDataBound parameter</param>
    /// <returns>DataViewRow</returns>
    private DataRowView GetDataRowViewFromParameter(object parameter)
    {
        DataRowView row = null;
        if (parameter is System.Web.UI.WebControls.GridViewRow)
        {
            row = (DataRowView)((System.Web.UI.WebControls.GridViewRow)parameter).DataItem;
        }
        else if (parameter is System.Data.DataRowView)
        {
            row = (DataRowView)parameter;
        }

        return row;
    }


    /// <summary>
    /// Returns the content of results column. Results column contains message "failed" with link to error message when search task execution has failed.
    /// </summary>
    /// <param name="row">Grid row</param>
    /// <returns>Content of result column</returns>
    private object RenderResult(DataRowView row)
    {
        if (row == null)
        {
            return String.Empty;
        }

        // Check if task has error status
        string status = ValidationHelper.GetString(row["SearchTaskStatus"], "");
        if (String.IsNullOrEmpty(status) || status.ToEnum<SearchTaskStatusEnum>() != SearchTaskStatusEnum.Error)
        {
            return String.Empty;
        }

        // Check task ID, render failed message if task id cannot be obtained
        int taskID = ValidationHelper.GetInteger(row["SearchTaskID"], 0);
        if (taskID == 0)
        {
            return LocalizationHelper.GetString("smartsearch.resultfailed");
        }

        // Render failed message with link to modal dialog with full report
        string resultUrl = CMS.Helpers.URLHelper.ResolveUrl("~/CMSModules/SmartSearch/SearchTask_Report.aspx?taskid=") + taskID;
        return String.Format("<a target=\"_blank\" href=\"{0}\" onclick=\"modalDialog('{0}', 'taskresult', 700, 500); return false;\">{1}</a>", resultUrl, LocalizationHelper.GetString("smartsearch.resultfailed"));
    }


    /// <summary>
    /// Get the transformation to interpret task related object data.
    /// </summary>
    /// <param name="row">Grid row</param>
    /// <returns>Transformation that displays object identified by object type and ID.</returns>
    private object RenderRelatedObject(DataRowView row)
    {
        if (row == null)
        {
            return String.Empty;
        }

        int objectId = ValidationHelper.GetInteger(row.Row["SearchTaskRelatedObjectID"], 0);
        string taskTypeRaw = ValidationHelper.GetString(row.Row["SearchTaskType"], "");
        
        SearchTaskTypeEnum taskType;
        string objectType = null;
        

        // try to get search task type. Type doesn't have a default value. 
        try
        {
            taskType = ValidationHelper.GetString(taskTypeRaw, "").ToEnum<SearchTaskTypeEnum>();
        }
        catch (Exception ex)
        {
            EventLogProvider.LogEvent(
                EventType.ERROR,
                "Smart search",
                "LISTSEARCHTASKS",
                "Unknown search task type: " + taskTypeRaw + ". Original exception:" + Environment.NewLine + EventLogProvider.GetExceptionLogMessage(ex)
            );

            return String.Empty;
        }

        // Object type
        objectType = SearchTaskInfoProvider.GetSearchTaskRelatedObjectType(ValidationHelper.GetString(row.Row["SearchTaskObjectType"], ""), taskType);

        // Object cannot be interpreted
        if (String.IsNullOrEmpty(objectType) || (objectId == 0))
        {
            return String.Empty;
        }

        // create transformation
        ObjectTransformation transformation = new ObjectTransformation(objectType, objectId);
        transformation.Transformation = String.Format("{{% Object.GetFullObjectName(false, true) %}}");

        ObjectTypeInfo typeInfo = ObjectTypeManager.GetTypeInfo(objectType);
        if (typeInfo != null)
        {
            transformation.NoDataTransformation = LocalizationHelper.GetStringFormat("smartsearch.searchtaskrelatedobjectnotexist", typeInfo.GetNiceObjectTypeName(), objectId);
        }

        return transformation;
    }


    /// <summary>
    /// Adds header actions to page.
    /// </summary>
    private void InitHeaderActions()
    {
        // Tasks on azure are processed by special worker role  or by scheduler
        // Scheduler may be set to run the task only on one CMS server at the time (shared index files)
        // In this cases we cant allow user to start task processing on the current server.
        if (!SystemContext.IsRunningOnAzure && !SearchTaskInfoProvider.ProcessSearchTasksByScheduler)
        {
            // Add process tasks action
            mProcessTasksAction = new HeaderAction
            {
                Text = Control.GetString("smartsearch.task.processtasks"),
                CommandName = PROCESS
            };
            Control.AddHeaderAction(mProcessTasksAction);
            ComponentEvents.RequestEvents.RegisterForEvent(PROCESS, (sender, args) => SearchTaskInfoProvider.ProcessTasks());
        }

        // Add refresh action
        mRefreshAction = new HeaderAction
        {
            Text = Control.GetString("general.refresh"),
            CommandName = REFRESH
        };
        Control.AddHeaderAction(mRefreshAction);
    }


    /// <summary>
    /// Displays info message when indexer thread is running.
    /// </summary>
    private void DisplayTaskRunningMessage()
    {
        // Check that thread is running
        if (IsIndexerThreadRunning)
        {
            string url = URLHelper.ResolveUrl("~/CMSModules/System/Debug/System_ViewLog.aspx");
            url = URLHelper.UpdateParameterInUrl(url, "threadGuid", IndexerThread.ThreadGUID.ToString());
            if (WebFarmHelper.WebFarmEnabled)
            {
                url = URLHelper.UpdateParameterInUrl(url, "serverName", WebFarmHelper.ServerName);
            }

            string message = LocalizationHelper.GetStringFormat("smartsearch.taskprocessingrunning",
                "<a href=\"javascript:void(0)\" onclick=\"modalDialog('" + url + "', 'ThreadProgress', '1000', '700');\" >",
                "</a>");

            Control.ShowInformation(message);
        }
    }

    #endregion
}