using System;
using System.Linq;
using System.Data;
using System.Web.UI.WebControls;
using System.Collections.Generic;

using CMS.DataEngine;
using CMS.UIControls;
using CMS.Newsletters;
using CMS.Helpers;
using CMS.Scheduler;

public partial class CMSModules_Newsletters_Controls_VariantMailout : CMSAdminControl
{
    #region "Constants"

    private const string WINNER_BACKGROUND_COLOR = "#B9FFB9";
    private const string DDLIST_SETSELECTED = "-1";
    private const string DDLIST_SETALL = "0";

    #endregion


    #region "Private variables"

    private bool mShowSelectionColumn = true;
    private bool mEnableMailoutTimeSetting = true;
    private bool mShowSelectWinnerAction;
    private DateTime mHighestMailoutTime = DateTime.MinValue;
    private bool? mOnlineMarketingEnabled;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets parent issue ID.
    /// </summary>
    public int ParentIssueID
    {
        get;
        set;
    }


    /// <summary>
    /// Shows/hides selection column in the grid
    /// </summary>
    public bool ShowSelectionColumn
    {
        get { return mShowSelectionColumn; }
        set { mShowSelectionColumn = value; }
    }


    /// <summary>
    /// Enables/disables mailout setting.
    /// </summary>
    public bool EnableMailoutTimeSetting
    {
        get { return mEnableMailoutTimeSetting; }
        set { mEnableMailoutTimeSetting = value; }
    }


    /// <summary>
    /// Shows/hides action column with winner selection.
    /// </summary>
    public bool ShowSelectWinnerAction
    {
        get { return mShowSelectWinnerAction; }
        set { mShowSelectWinnerAction = value; }
    }


    /// <summary>
    /// Shows/hides issue status column.
    /// </summary>
    public bool ShowIssueStatus
    {
        get;
        set;
    }


    /// <summary>
    /// Shows/hides opened e-mail counters.
    /// </summary>
    public bool ShowOpenedEmails
    {
        get;
        set;
    }


    /// <summary>
    /// Shows/hides unique clicks counters.
    /// </summary>
    public bool ShowUniqueClicks
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets value, that indicates whether grouping text for entire control is used
    /// instead of ordinary label.
    /// </summary>
    public bool UseGroupingText
    {
        get;
        set;
    }


    /// <summary>
    /// Highest mail-out time in the grid.
    /// </summary>
    public DateTime HighestMailoutTime
    {
        get { return mHighestMailoutTime; }
    }


    /// <summary>
    /// Gets or sets ID of the winner.
    /// </summary>
    private int WinnerIssueID
    {
        get;
        set;
    }


    /// <summary>
    /// Determines if Online Marketing is enabled.
    /// </summary>
    private bool OnlineMarketingEnabled
    {
        get
        {
            if (mOnlineMarketingEnabled == null)
            {
                mOnlineMarketingEnabled = NewsletterHelper.OnlineMarketingAvailable(CurrentSite.SiteName);
            }

            return (bool)mOnlineMarketingEnabled;
        }
    }

    #endregion


    #region "Events"

    /// <summary>
    /// Occurs when error message is shown.
    /// </summary>
    public event EventHandler OnShowError;


    /// <summary>
    /// Occurs when mailout time has been changed.
    /// </summary>
    public event EventHandler OnChanged;

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        ReloadData();
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        // Show/hide additional columns as needed
        grdElem.NamedColumns["opened"].Visible = ShowOpenedEmails;
        grdElem.NamedColumns["clicks"].Visible = ShowUniqueClicks;
        grdElem.NamedColumns["status"].Visible = ShowIssueStatus;
    }

    public override void ReloadData(bool forceReload)
    {
        if (StopProcessing && !forceReload)
        {
            return;
        }
        InitControls(forceReload);

        // Javascript for handling winner mailout time
        string scriptBlock = string.Format(@"function SelWinner(id) {{ modalDialog('{0}?objectid=' + id, 'NewsletterWinnerMailout', '700px', '425px'); return false; }}
function ShowOpenedBy(id) {{ modalDialog('{1}?objectid=' + id, 'NewsletterIssueOpenedBy', '900px', '700px');  return false; }}
function ViewClickedLinks(id) {{ modalDialog('{2}?objectid=' + id, 'NewsletterTrackedLinks', '900px', '700px'); return false; }}",
                                           ResolveUrl(@"~\CMSModules\Newsletters\Tools\Newsletters\Newsletter_Issue_WinnerMailout.aspx"),
                                           ResolveUrl(@"~\CMSModules\Newsletters\Tools\Newsletters\Newsletter_Issue_OpenedBy.aspx"),
                                           ResolveUrl(@"~\CMSModules\Newsletters\Tools\Newsletters\Newsletter_Issue_TrackedLinks.aspx"));
        ScriptHelper.RegisterDialogScript(Page);
        ScriptHelper.RegisterClientScriptBlock(this, GetType(), "Actions", scriptBlock, true);

        // Register handlers
        grdElem.OnExternalDataBound -= grdElem_OnExternalDataBound;
        grdElem.OnExternalDataBound += grdElem_OnExternalDataBound;
        grdElem.GridView.RowDataBound -= GridView_RowDataBound;
        grdElem.GridView.RowDataBound += GridView_RowDataBound;

        // Get winner ID if any
        ABTestInfo abi = ABTestInfoProvider.GetABTestInfoForIssue(ParentIssueID);
        if (abi != null)
        {
            WinnerIssueID = abi.TestWinnerIssueID;
        }

        grdElem.WhereCondition = GetWhereCondition(ParentIssueID, false);
        grdElem.ShowActionsMenu = false;
        grdElem.ShowObjectMenu = false;
        if (!ShowSelectWinnerAction)
        {
            grdElem.GridActions = null;
        }
        grdElem.ReloadData();
    }


    protected void btnOk_Click(object sender, EventArgs e)
    {
        // Validate date/time (blank date/time textbox is allowed)
        if ((dtpMailout.SelectedDateTime == DateTimeHelper.ZERO_TIME) && !String.IsNullOrEmpty(dtpMailout.DateTimeTextBox.Text.Trim()))
        {
            ShowErrorInternal(GetString("newsletterissue_send.invaliddatetime"));
            return;
        }

        // Get variants (IDs) to set
        var selItems = new List<string>();

        if (drpAllSelected.SelectedValue == DDLIST_SETALL)
        {
            selItems = IssueInfoProvider.GetIssues()
                .Where(GetWhereCondition(ParentIssueID, true))
                .Column("IssueID")
                .Select(s => s.IssueID.ToString())
                .ToList();
        }
        else if (drpAllSelected.SelectedValue == DDLIST_SETSELECTED)
        {
            selItems = grdElem.SelectedItems;
        }
        else
        {
            selItems.Add(drpAllSelected.SelectedValue);
        }

        // Check selected variants
        if (!selItems.Any())
        {
            if (drpAllSelected.SelectedValue != DDLIST_SETALL)
            {
                ShowErrorInternal(GetString("newsletterissue_send.novariantsselected"));
            }
            else
            {
                pMOut.Visible = false;
            }
            return;
        }

        DateTime when = (dtpMailout.SelectedDateTime == DateTimeHelper.ZERO_TIME) ? DateTime.Now : dtpMailout.SelectedDateTime;
        foreach (string itemId in selItems)
        {
            var issue = IssueInfoProvider.GetIssueInfo(ValidationHelper.GetInteger(itemId, 0));
            if (issue == null) continue;
            TaskInfo task = NewsletterTasksManager.EnsureMailoutTask(issue, DateTime.Now, false);
            task.TaskNextRunTime = when;
            TaskInfoProvider.SetTaskInfo(task);
            if (issue.IssueScheduledTaskID != task.TaskID)
            {
                issue.IssueScheduledTaskID = task.TaskID;
                IssueInfoProvider.SetIssueInfo(issue);
            }
        }

        mHighestMailoutTime = DateTime.MinValue;
        grdElem.ResetSelection();
        grdElem.ReloadData();

        if (OnChanged != null)
        {
            OnChanged(this, EventArgs.Empty);
        }
    }


    #region "Unigrid handlers"

    protected object grdElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName)
        {
            case "IssueVariantName":
                {
                    DataRowView drv = (DataRowView)parameter;
                    string result = ValidationHelper.GetString(drv["IssueVariantName"], string.Empty);

                    // Issue has not been sent yet => get mail out time from scheduled task
                    if (ValidationHelper.GetInteger(drv["IssueID"], 0) == WinnerIssueID)
                    {
                        result += " " + GetString("newsletterabtest.winner");
                    }

                    return HTMLHelper.HTMLEncode(result);
                }

            case "MailoutTime":
                {
                    string result;
                    DataRowView drv = (DataRowView)parameter;
                    DateTime dt = ValidationHelper.GetDateTime(drv["IssueMailoutTime"], DateTimeHelper.ZERO_TIME);

                    if (dt == DateTimeHelper.ZERO_TIME)
                    {
                        // Issue has not been sent yet => get mail out time from scheduled task
                        int taskId = ValidationHelper.GetInteger(drv["IssueScheduledTaskID"], 0);
                        TaskInfo task = TaskInfoProvider.GetTaskInfo(taskId);
                        if (task != null)
                        {
                            if (task.TaskNextRunTime < DateTime.Now)
                            {
                                result = String.Format("{0} {1}", task.TaskNextRunTime.ToString(), GetString("newsletterissue_send.asap"));
                            }
                            else
                            {
                                result = task.TaskNextRunTime.ToString();
                            }
                            dt = task.TaskNextRunTime;
                        }
                        else
                        {
                            result = GetString("general.na");
                        }
                    }
                    else
                    {
                        result = dt.ToString();
                    }

                    if (mHighestMailoutTime < dt)
                    {
                        mHighestMailoutTime = dt;
                    }

                    return HTMLHelper.HTMLEncode(result);
                }

            case "IssueStatus":
                IssueStatusEnum status = IssueStatusEnum.Idle;
                if ((parameter != DBNull.Value) && (parameter != null))
                {
                    status = (IssueStatusEnum)parameter;
                }
                return IssueHelper.GetStatusFriendlyName(status, null);

            case "IssueOpenedEmails":
                return GetOpenedEmails(parameter as DataRowView);

            case "UniqueClicks":
                int issueId = ValidationHelper.GetInteger(parameter, 0);
                return GetUniqueClicks(IssueHelper.GetIssueTotalUniqueClicks(issueId), issueId);
        }
        return parameter;
    }


    protected void GridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (WinnerIssueID == ValidationHelper.GetInteger(((DataRowView)(e.Row.DataItem)).Row["IssueID"], 0))
            {
                e.Row.Style.Add("background-color", WINNER_BACKGROUND_COLOR);
            }
        }
    }

    #endregion


    /// <summary>
    /// Initializes controls
    /// </summary>
    protected void InitControls(bool forceReload)
    {
        grdElem.GridOptions.ShowSelection = ShowSelectionColumn;
        pMOut.Visible = EnableMailoutTimeSetting;

        if (EnableMailoutTimeSetting)
        {
            if (forceReload || (drpAllSelected.Items.Count <= 0))
            {
                drpAllSelected.Items.Clear();
                if (ShowSelectionColumn)
                {
                    drpAllSelected.Items.Add(new ListItem(GetString("general.selectall"), DDLIST_SETALL));
                    drpAllSelected.Items.Add(new ListItem(GetString("newsletterissue_send.selected"), DDLIST_SETSELECTED));
                }

                List<IssueABVariantItem> items = IssueHelper.GetIssueVariants(ParentIssueID, "IssueMailoutTime IS NULL");
                if ((items != null) && (items.Count > 0))
                {
                    foreach (IssueABVariantItem item in items)
                    {
                        drpAllSelected.Items.Add(new ListItem(item.IssueVariantName, item.IssueID.ToString()));
                    }
                }
            }

            // Hide entire panel if no variant available
            if (drpAllSelected.Items.Count <= 0)
            {
                pMOut.Visible = false;
            }
        }

        if (UseGroupingText)
        {
            pnlMailoutHeading.ResourceString = "newsletterissue_send.schedulemailout";
        }
        else
        {
            pnlMailoutHeading.ResourceString = "newsletterissue_send.testresults";
        }
    }


    /// <summary>
    /// Shows error message.
    /// </summary>
    /// <param name="errorMessage">Error message</param>
    private void ShowErrorInternal(string errorMessage)
    {
        ErrorMessage = errorMessage;
        if (OnShowError != null)
        {
            OnShowError(this, EventArgs.Empty);
        }
        else
        {
            ShowError(errorMessage);
        }
    }


    /// <summary>
    /// Returns WHERE condition
    /// </summary>
    /// <param name="issueId">Issue ID</param>
    /// <param name="notSentOnly">If TRUE additional condition is included</param>
    private string GetWhereCondition(int issueId, bool notSentOnly)
    {
        return String.Format("IssueVariantOfIssueID={0}{1}", issueId, (notSentOnly ? " AND IssueMailoutTime IS NULL" : String.Empty));
    }


    /// <summary>
    /// Gets a clickable opened emails counter based on the values from datasource.
    /// </summary>
    /// <param name="rowView">A <see cref="DataRowView" /> that represents one row from UniGrid's source</param>
    private string GetOpenedEmails(DataRowView rowView)
    {
        // Get issue ID
        int issueId = ValidationHelper.GetInteger(DataHelper.GetDataRowViewValue(rowView, "IssueID"), 0);

        // Get opened emails count from issue record
        int openedEmails = ValidationHelper.GetInteger(DataHelper.GetDataRowViewValue(rowView, "IssueOpenedEmails"), 0);

        if (OnlineMarketingEnabled)
        {
            // Get number of emails opened by contact group and persona members
            openedEmails += OpenedEmailInfoProvider.GetMultiSubscriberOpenedIssueActivityCount(issueId);
        }

        if (openedEmails > 0)
        {
            return string.Format(@"<a href=""#"" onclick=""ShowOpenedBy({0})"">{1}</a>", issueId, openedEmails);
        }
        else
        {
            return "0";
        }
    }


    /// <summary>
    /// Gets a clickable click links counter based on the values from datasource.
    /// </summary>
    /// <param name="clicks">Number of unique clicks</param>
    /// <param name="issueId">Issue ID</param>
    private string GetUniqueClicks(int clicks, int issueId)
    {
        if (clicks > 0)
        {
            return string.Format(@"<a href=""#"" onclick=""ViewClickedLinks({0})"">{1}</a>", issueId, clicks);
        }
        else
        {
            return "0";
        }
    }
}
