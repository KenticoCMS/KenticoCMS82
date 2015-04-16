using System;
using System.Web.UI.WebControls;
using System.Data;

using CMS.Helpers;
using CMS.ExtendedControls;
using CMS.UIControls;
using CMS.WebFarmSync;
using CMS.ExtendedControls.ActionsConfig;

public partial class CMSModules_WebFarm_Pages_WebFarm_AnonymousTask_List : GlobalAdminPage
{

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


    protected override void OnPreInit(EventArgs e)
    {
        ((Panel)CurrentMaster.PanelBody.FindControl("pnlContent")).CssClass = "";
        base.OnPreInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        UniGrid.OnAction += new OnActionEventHandler(uniGrid_OnAction);
        UniGrid.ZeroRowsText = GetString("WebFarmTasks_List.ZeroRows");
        UniGrid.GridView.DataBound += new EventHandler(GridView_DataBound);
        UniGrid.GridView.RowDataBound += GridView_RowDataBound;

        HeaderActions.AddAction(new HeaderAction { ButtonStyle = ButtonStyle.Primary, Text = GetString("WebFarmTasks_List.RunButton"), CommandName = "run" });
        HeaderActions.AddAction(new HeaderAction { ButtonStyle = ButtonStyle.Default, Text = GetString("WebFarmTasks_List.EmptyButton"), CommandName = "empty" }); 
        HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;
    }

    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "empty":
                // Delete anonymous tasks
                WebFarmTaskInfoProvider.DeleteAllTaskInfo(null);
                UniGrid.ReloadData();
                break;

            case "run":
                // Call synchronization method
                WebSyncHelper.ProcessMyTasks();
                UniGrid.ReloadData();

                // Show info label
                ShowInformation(GetString("webfarmtasks.taskexecuted"));
                break;
        }
    }


    protected void GridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            string code = ValidationHelper.GetString(((DataRowView)(e.Row.DataItem)).Row["TaskErrorMessage"], string.Empty);
            if (code != String.Empty)
            {
                string color = ((e.Row.RowIndex & 1) == 1) ? "#EEC9C9" : "#FFDADA";
                e.Row.Style.Add("background-color", color);
            }
        }
    }


    protected void GridView_DataBound(object sender, EventArgs e)
    {
        HeaderActions.Visible = !DataHelper.DataSourceIsEmpty(UniGrid.GridView.DataSource);
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
            // Delete task object
            WebFarmTaskInfoProvider.DeleteWebFarmTaskInfo(Convert.ToInt32(actionArgument));

            UniGrid.ReloadData();
        }
    }
}