using System;
using System.Linq;
using System.Web.UI.WebControls;
using System.Data;

using CMS.DataEngine;
using CMS.ExtendedControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.Helpers;
using CMS.Base;
using CMS.UIControls;
using CMS.WebFarmSync;
using CMS.FormEngine;


public partial class CMSModules_WebFarm_Pages_WebFarm_Task_List : GlobalAdminPage
{
    private const string allServers = "##ALL##";
    private string mSelectedServer;
    private HeaderAction runAction;
    private HeaderAction clearAction;
    private WebFarmServerInfo mSelectedServerInfo;


    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
    }


    /// <summary>
    /// Selected web farm server name
    /// </summary>
    private string SelectedServer
    {
        get
        {
            if (mSelectedServer != null)
            {
                return mSelectedServer;
            }

            if (RequestHelper.IsPostBack())
            {
                return mSelectedServer = ValidationHelper.GetString(uniSelector.Value, allServers);
            }

            return mSelectedServer = allServers;
        }
    }


    /// <summary>
    /// Selected web farm server info object
    /// </summary>
    private WebFarmServerInfo SelectedServerInfo
    {
        get
        {
            if (SelectedServer.EqualsCSafe(allServers))
            {
                return null;
            }

            // Get selected server info if server is not loaded yet
            if (mSelectedServerInfo == null)
            {
                mSelectedServerInfo = WebFarmServerInfoProvider.GetWebFarmServerInfo(SelectedServer);
            }

            // Selected server not found - clear cache and try again (server can be created on other instance)
            if (mSelectedServerInfo == null)
            {
                WebSyncHelper.Clear(false);
                mSelectedServerInfo = WebFarmServerInfoProvider.GetWebFarmServerInfo(SelectedServer);
            }

            return mSelectedServerInfo;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        UniGrid.OnAction += uniGrid_OnAction;
        UniGrid.ZeroRowsText = GetString("WebFarmTasks_List.ZeroRows");
        UniGrid.GridView.DataBound += GridView_DataBound;
        UniGrid.GridView.RowDataBound += GridView_RowDataBound;

        uniSelector.SpecialFields.Add(new SpecialField() { Text = GetString("WebFarmList.All"), Value = allServers });
        uniSelector.DropDownSingleSelect.AutoPostBack = true;

        if (SelectedServerInfo != null)
        {
            UniGrid.WhereCondition = new WhereCondition().WhereEquals("ServerName", SelectedServer).ToString(true);
        }

        // Prepare header actions
        runAction = new HeaderAction()
        {
            ButtonStyle = ButtonStyle.Primary,
            Text = GetString("WebFarmTasks_List.RunButton"),
            CommandName = "run"
        };
        clearAction = new HeaderAction()
        {
            ButtonStyle = ButtonStyle.Default,
            Text = GetString("WebFarmTasks_List.EmptyButton"),
            CommandName = "clear"
        };
        HeaderActions.AddAction(runAction);
        HeaderActions.AddAction(clearAction);
        HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        UniGrid.GridView.Columns[1].Visible = (SelectedServerInfo == null);
    }


    protected void GridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            string code = ValidationHelper.GetString(((DataRowView)(e.Row.DataItem)).Row["ErrorMessage"], string.Empty);
            if (!String.IsNullOrEmpty(code))
            {
                e.Row.Style.Add("background-color", "#FFDDDD");
            }
        }
    }


    protected void GridView_DataBound(object sender, EventArgs e)
    {
        runAction.Visible = clearAction.Visible = !DataHelper.DataSourceIsEmpty(UniGrid.GridView.DataSource);
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void uniGrid_OnAction(string actionName, object actionArgument)
    {
        if (actionName == "delete")
        {
            int taskId = ValidationHelper.GetInteger(actionArgument, 0);

            // Delete object from database
            if (SelectedServerInfo == null)
            {
                // Delete task object
                WebFarmTaskInfoProvider.DeleteWebFarmTaskInfo(taskId);
            }
            else
            {
                // Delete task binding to server
                WebFarmTaskInfoProvider.DeleteServerTask(SelectedServerInfo.ServerID, taskId);
            }

            UniGrid.ReloadData();
        }
    }


    /// <summary>
    /// Performs header actions.
    /// </summary>
    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName.ToLowerCSafe())
        {
            case "run":
                RunTasks();
                break;
            case "clear":
                EmptyTasks();
                break;
        }
    }


    /// <summary>
    /// Clear task list.
    /// </summary>
    private void EmptyTasks()
    {
        if (SelectedServerInfo == null)
        {
            // delete all task objects
            WebFarmTaskInfoProvider.DeleteAllTaskInfo();
        }
        else
        {
            // delete bindings to specified server
            WebFarmTaskInfoProvider.DeleteAllTaskInfo(SelectedServer);
        }

        UniGrid.ReloadData();
    }


    /// <summary>
    /// Run task list.
    /// </summary>
    private void RunTasks()
    {
        if (SelectedServerInfo == null)
        {
            WebSyncHelper.NotifyServers(true);
            WebSyncHelper.ProcessMyTasks();
        }
        else if (SelectedServerInfo.ServerEnabled)
        {
            if (SelectedServer.EqualsCSafe(WebSyncHelper.ServerName, true))
            {
                // Call synchronization method
                WebSyncHelper.ProcessMyTasks();
            }
            else
            {
                WebSyncHelper.NotifyServer(SelectedServerInfo.ServerID);
            }
        }

        UniGrid.ReloadData();

        // Show info label
        ShowInformation(GetString("webfarmtasks.taskexecuted"));
    }
}