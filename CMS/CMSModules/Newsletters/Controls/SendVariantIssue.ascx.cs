using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;

using CMS.Helpers;
using CMS.Newsletters;
using CMS.UIControls;
using CMS.SiteProvider;
using CMS.DataEngine;

public partial class CMSModules_Newsletters_Controls_SendVariantIssue : CMSAdminControl
{
    #region "Constants"

    private const int STATE_WAITING_TO_SEND_WIZARD = 1;
    private const int STATE_WAITING_TO_SEND_PAGE = 2;
    private const int STATE_TEST_WAITING_TO_SEL_WINNER = 3;
    private const int STATE_TEST_READY_FOR_SENDING = 4;
    private const int STATE_TEST_FINISHED = 5;

    #endregion

    /// <summary>
    /// Control mode
    /// </summary>
    public enum SendControlMode
    {
        /// <summary>
        /// Control is used in wizard step
        /// </summary>
        Wizard = 0,

        /// <summary>
        /// Control is used on page
        /// </summary>
        Send = 1
    }


    #region "Private variables"

    private IssueInfo parentIssue = null;
    private ABTestInfo abTest = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// ID of newsletter issue that should be sent, required for template based newsletters.
    /// </summary>
    public int IssueID
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets mode. Value changes control behaviour (wizard - control is on new issue wizard;
    /// send - control is on send page).
    /// </summary>
    public SendControlMode Mode
    {
        get;
        set;
    }


    /// <summary>
    /// Gets value that indicates whether sending issue is allowed or not.
    /// </summary>
    public bool SendingAllowed
    {
        get { return (CurrentState == STATE_WAITING_TO_SEND_PAGE) || (CurrentState == STATE_WAITING_TO_SEND_WIZARD); }
    }


    /// <summary>
    /// Flag - indicates if force reloaded is needed in the next reload.
    /// </summary>
    public bool ForceReloadNeeded
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets current state of control ("waiting to send", "testing finished" etc.)
    /// </summary>
    private int CurrentState
    {
        get;
        set;
    }

    #endregion


    #region "Events"

    /// <summary>
    /// Occurs when info message needs to be redrawn.
    /// </summary>
    public event EventHandler OnChanged;

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Visible || StopProcessing || (IssueID <= 0))
        {
            return;
        }

        ReloadData(ForceReloadNeeded);
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Reloads control data.
    /// </summary>
    /// <param name="forceReload">Indicates if force reload should be used</param>
    public override void ReloadData(bool forceReload)
    {
        if (StopProcessing && !forceReload)
        {
            return;
        }

        if (ForceReloadNeeded)
        {
            forceReload = true;
            ForceReloadNeeded = false;
        }

        int parentIssueId = 0;
        parentIssue = IssueInfoProvider.GetOriginalIssue(IssueID);
        if (parentIssue != null)
        {
            parentIssueId = parentIssue.IssueID;
        }

        // Get A/B test configuration
        abTest = ABTestInfoProvider.GetABTestInfoForIssue(parentIssueId);
        if (abTest == null)
        {
            // Ensure A/B test object with default settings
            abTest = new ABTestInfo() { TestIssueID = parentIssueId, TestSizePercentage = 50, TestWinnerOption = ABTestWinnerSelectionEnum.OpenRate, TestSelectWinnerAfter = 60 };
            ABTestInfoProvider.SetABTestInfo(abTest);
        }

        CurrentState = GetCurrentState(parentIssue);
        InitControls(CurrentState, forceReload);

        ucMailout.ParentIssueID = parentIssueId;
        ucMailout.ReloadData(forceReload);

        InfoMessage = GetInfoMessage(CurrentState, parentIssue, (abTest != null ? abTest.TestWinnerOption : ABTestWinnerSelectionEnum.OpenRate), GetPlannedMailoutTime(ucMailout.HighestMailoutTime));

        // Init test group slider
        List<IssueABVariantItem> variants = IssueHelper.GetIssueVariants(parentIssue, null);
        ucGroupSlider.Variants = variants;
        bool allVariantsSent = true;
        if (variants != null)
        {
            allVariantsSent = variants.TrueForAll(delegate(IssueABVariantItem item) { return item.IssueStatus == IssueStatusEnum.Finished; });
        }
        if ((parentIssue.IssueStatus == IssueStatusEnum.Finished) || allVariantsSent)
        {
            // Issue was sent long time ago => get number of subscribers from issue properties instead of current number of subscribers
            int perVariantEmails = 1;
            if (abTest != null)
            {
                perVariantEmails = abTest.TestNumberPerVariantEmails;
            }
            ucGroupSlider.NumberOfTestSubscribers = ucGroupSlider.Variants.Count * perVariantEmails;

            if (parentIssue.IssueStatus == IssueStatusEnum.Finished)
            {
                // Issue was sent => get number of subscribers from number of sent issues
                ucGroupSlider.NumberOfSubscribers = parentIssue.IssueSentEmails;
            }
            else
            {
                // Only variants was sent => get current number of subscribers
                ucGroupSlider.NumberOfSubscribers = NewsletterHelper.GetEmailAddressCount(parentIssue.IssueNewsletterID, parentIssue.IssueSiteID);
            }
        }
        else
        {
            ucGroupSlider.NumberOfSubscribers = NewsletterHelper.GetEmailAddressCount(parentIssue.IssueNewsletterID, parentIssue.IssueSiteID);
        }

        if (forceReload || !ucGroupSlider.Enabled)
        {
            ucGroupSlider.CurrentSize = (abTest != null ? abTest.TestSizePercentage : 10);
        }
        ucGroupSlider.ReloadData(forceReload);
        ucWO_OnChange(this, EventArgs.Empty);
    }


    /// <summary>
    /// Collects data from controls and fills A/B test info.
    /// </summary>
    /// <param name="abi">A/B test info</param>
    private bool SaveABTestInfo(ABTestInfo abi)
    {
        if (abi == null)
        {
            return false;
        }

        bool result = (abi.TestWinnerOption != ucWO.WinnerSelection) || (abi.TestSelectWinnerAfter != ucWO.TimeInterval)
            || (abi.TestSizePercentage != ucGroupSlider.CurrentSize);
        switch (Mode)
        {
            case SendControlMode.Wizard:
            case SendControlMode.Send:
                abi.TestWinnerOption = ucWO.WinnerSelection;
                abi.TestSelectWinnerAfter = ucWO.TimeInterval;
                break;
        }

        abi.TestSizePercentage = ucGroupSlider.CurrentSize;
        return result;
    }


    /// <summary>
    /// Initializes controls
    /// </summary>
    /// <param name="currState">Current state of the control (controls are initializes according this value)</param>
    /// <param name="forceReload">Indicates if force data reload should be performed</param>
    protected void InitControls(int currState, bool forceReload)
    {
        switch (currState)
        {
            case STATE_TEST_FINISHED:
                ucGroupSlider.Enabled = false;
                ucMailout.ShowSelectWinnerAction = false;
                ucMailout.ShowOpenedEmails = true;
                ucMailout.ShowUniqueClicks = true;
                ucMailout.ShowIssueStatus = true;
                ucMailout.ShowSelectionColumn = false;
                ucMailout.EnableMailoutTimeSetting = false;
                InitWinnerOption(false, abTest, forceReload);
                lblAdditionalInfo.Visible = true;
                lblAdditionalInfo.Text = GetString("newsletterissue_send.variantsendingfinished");
                break;
            case STATE_TEST_READY_FOR_SENDING:
                ucGroupSlider.Enabled = true;
                ucMailout.ShowSelectWinnerAction = false;
                ucMailout.ShowOpenedEmails = true;
                ucMailout.ShowUniqueClicks = true;
                ucMailout.ShowIssueStatus = true;
                ucMailout.ShowSelectionColumn = true;
                ucMailout.EnableMailoutTimeSetting = true;
                ucMailout.OnChanged -= new EventHandler(ucMailout_OnChanged);
                ucMailout.OnChanged += new EventHandler(ucMailout_OnChanged);
                InitWinnerOption(true, abTest, forceReload);
                lblAdditionalInfo.Visible = true;
                lblAdditionalInfo.Text = GetString("newsletterissue_send.infovariantsending");

                break;
            case STATE_TEST_WAITING_TO_SEL_WINNER:
                ucGroupSlider.Enabled = false;
                ucMailout.ShowSelectWinnerAction = true;
                ucMailout.ShowOpenedEmails = true;
                ucMailout.ShowUniqueClicks = true;
                ucMailout.ShowIssueStatus = true;
                ucMailout.ShowSelectionColumn = false;
                ucMailout.EnableMailoutTimeSetting = true;
                ucMailout.OnChanged -= new EventHandler(ucMailout_OnChanged);
                ucMailout.OnChanged += new EventHandler(ucMailout_OnChanged);
                InitWinnerOption(true, abTest, forceReload);
                lblAdditionalInfo.Visible = true;
                lblAdditionalInfo.Text = GetString("newsletterissue_send.infowaitingtoselwinner");
                break;

            case STATE_WAITING_TO_SEND_PAGE:
                ucMailout.UseGroupingText = true;
                ucGroupSlider.Enabled = true;
                ucMailout.OnChanged -= new EventHandler(ucMailout_OnChanged);
                ucMailout.OnChanged += new EventHandler(ucMailout_OnChanged);
                InitWinnerOption(true, abTest, forceReload);
                lblAdditionalInfo.Visible = false;
                break;

            case STATE_WAITING_TO_SEND_WIZARD:
                ucMailout.UseGroupingText = true;
                ucGroupSlider.Enabled = true;
                ucMailout.OnChanged -= new EventHandler(ucMailout_OnChanged);
                ucMailout.OnChanged += new EventHandler(ucMailout_OnChanged);
                InitWinnerOption(true, abTest, forceReload);
                lblAdditionalInfo.Visible = false;
                break;
        }
    }


    protected void ucMailout_OnChanged(object sender, EventArgs e)
    {
        InfoMessage = GetInfoMessage(CurrentState, parentIssue, abTest.TestWinnerOption, GetPlannedMailoutTime(ucMailout.HighestMailoutTime));
        ucWO_OnChange(this, EventArgs.Empty);
        if (OnChanged != null)
        {
            OnChanged(this, EventArgs.Empty);
        }
    }


    /// <summary>
    /// Returns information message according to current state, issue and A/B test.
    /// </summary>
    /// <param name="currState">Current state</param>
    /// <param name="issue">Issue</param>
    /// <param name="winnerOption">Winner option</param>
    private string GetInfoMessage(int currState, IssueInfo issue, ABTestWinnerSelectionEnum winnerOption, DateTime plannedMailoutTime)
    {
        if (issue == null)
        {
            return null;
        }

        switch (currState)
        {
            case STATE_WAITING_TO_SEND_WIZARD:
                return null;
            case STATE_WAITING_TO_SEND_PAGE:
                return GetString("Newsletter_Issue_Header.NotSentYet");
            case STATE_TEST_WAITING_TO_SEL_WINNER:
                switch (winnerOption)
                {
                    case ABTestWinnerSelectionEnum.Manual:
                        if (issue.IssueMailoutTime > DateTime.Now)
                        {
                            return String.Format(GetString("newsletterinfo.issuesentwaitingtosentwinner"), issue.IssueMailoutTime, (abTest != null ? abTest.TestWinnerSelected.ToString() : GetString("general.na")));
                        }
                        else
                        {
                            return String.Format(GetString("newsletterinfo.issuesentwaitingtoselwinnermanually"), issue.IssueMailoutTime);
                        }
                    case ABTestWinnerSelectionEnum.OpenRate:
                        return String.Format(GetString("newsletterinfo.issuesentwaitingtoselwinneropen"), issue.IssueMailoutTime, plannedMailoutTime);
                    case ABTestWinnerSelectionEnum.TotalUniqueClicks:
                        return String.Format(GetString("newsletterinfo.issuesentwaitingtoselwinnerclicks"), issue.IssueMailoutTime, plannedMailoutTime);
                }
                break;
            case STATE_TEST_READY_FOR_SENDING:
                return String.Format(GetString("newsletter_issue_header.issuesending"), plannedMailoutTime);
            case STATE_TEST_FINISHED:
                switch (winnerOption)
                {
                    case ABTestWinnerSelectionEnum.Manual:
                        return String.Format(GetString("newsletterinfo.issuesentwinnerselmanually"), issue.IssueMailoutTime, (abTest != null ? abTest.TestWinnerSelected.ToString() : GetString("general.na")));
                    case ABTestWinnerSelectionEnum.OpenRate:
                        return String.Format(GetString("newsletterinfo.issuesentwinnerselopen"), (abTest != null ? abTest.TestWinnerSelected.ToString() : GetString("general.na")));
                    case ABTestWinnerSelectionEnum.TotalUniqueClicks:
                        return String.Format(GetString("newsletterinfo.issuesentwinnerselclicks"), (abTest != null ? abTest.TestWinnerSelected.ToString() : GetString("general.na")));
                }
                break;
        }
        return null;
    }


    /// <summary>
    /// Initialize winner option control.
    /// </summary>
    /// <param name="enable">Control state (enabled/disabled)</param>
    /// <param name="abi">A-B test info to load to control</param>
    /// <param name="forceReload">TRUE if force load should be performed</param>
    private void InitWinnerOption(bool enable, ABTestInfo abi, bool forceReload)
    {
        // Register handlers
        ucWO.OnChange -= new EventHandler(ucWO_OnChange);
        ucWO.OnChange += new EventHandler(ucWO_OnChange);

        if (abi == null)
        {
            return;
        }

        ucWO.Visible = true;
        ucWO.Enabled = enable;
        if (forceReload)
        {
            ucWO.WinnerSelection = abi.TestWinnerOption;
            ucWO.TimeInterval = abi.TestSelectWinnerAfter;
        }
    }


    protected void ucWO_OnChange(object sender, EventArgs e)
    {
        bool visible = (Mode == SendControlMode.Wizard) && ucWO.Enabled;
        lblWillBeSent.Visible = visible;

        if (visible)
        {
            bool manualWinner = false;
            manualWinner = (ucWO.WinnerSelection == ABTestWinnerSelectionEnum.Manual);

            if (manualWinner)
            {
                lblWillBeSent.Text = GetString("newsletterissue_send.winnerwillbesentmanually");
            }
            else
            {
                lblWillBeSent.Text = String.Format(GetString("newsletterissue_send.winnerwillbesenton"), GetPlannedMailoutTime(ucMailout.HighestMailoutTime));
            }
        }
    }


    /// <summary>
    /// Calculates planned mail out time.
    /// </summary>
    private DateTime GetPlannedMailoutTime(DateTime highestMailoutTime)
    {
        if (highestMailoutTime == DateTimeHelper.ZERO_TIME)
        {
            highestMailoutTime = DateTime.Now;
        }
        if (Mode == SendControlMode.Send)
        {
            if ((abTest != null) && (abTest.TestWinnerOption != ABTestWinnerSelectionEnum.Manual))
            {
                return highestMailoutTime.AddMinutes(abTest.TestSelectWinnerAfter);
            }
        }
        else
        {
            if (ucWO.WinnerSelection != ABTestWinnerSelectionEnum.Manual)
            {
                return highestMailoutTime.AddMinutes(ucWO.TimeInterval);
            }
        }
        return highestMailoutTime;
    }


    /// <summary>
    /// "Sends" variant issue (enables all scheduled task associated to each variant).
    /// </summary>
    public bool SendIssue()
    {
        // Check current state before sending
        switch (CurrentState)
        {
            case STATE_TEST_READY_FOR_SENDING:
                ErrorMessage = GetString("newsletterissue_send.sendissuereadytobesent");
                return false;
            case STATE_TEST_WAITING_TO_SEL_WINNER:
            case STATE_TEST_FINISHED:
                ErrorMessage = GetString("newsletterissue_send.sendissuehasbeensent");
                return false;
        }

        if (!SaveIssue())
        {
            return false;
        }

        // Enable scheduled tasks and set 'Ready for sending' status to all variants
        NewsletterTasksManager.EnableVariantScheduledTasks(parentIssue);

        return true;
    }


    /// <summary>
    /// Saves current newsletter settings.
    /// </summary>
    public bool SaveIssue()
    {
        try
        {
            switch (CurrentState)
            {
                case STATE_WAITING_TO_SEND_PAGE:
                case STATE_WAITING_TO_SEND_WIZARD:
                case STATE_TEST_READY_FOR_SENDING:
                case STATE_TEST_WAITING_TO_SEL_WINNER:
                    if (abTest == null)
                    {
                        abTest = ABTestInfoProvider.GetABTestInfoForIssue(parentIssue.IssueID);
                    }

                    // Get A/B test settings from controls
                    bool abTestChanged = SaveABTestInfo(abTest);

                    if (abTest == null)
                    {
                        return false;
                    }

                    if (abTest.TestWinnerOption != ABTestWinnerSelectionEnum.Manual)
                    {
                        // Check minimal time interval
                        if (abTest.TestSelectWinnerAfter < 5)
                        {
                            ErrorMessage = GetString("newsletterissue_send.saveissuewrongwinnerselectioninterval");
                            return false;
                        }
                    }

                    // Check if test options has changed
                    if (abTestChanged)
                    {
                        if (abTest.TestWinnerIssueID > 0)
                        {
                            // Options has been changed => reset previously selected winner
                            NewsletterTasksManager.DeleteMailoutTask(parentIssue.IssueGUID, parentIssue.IssueSiteID);
                            abTest.TestWinnerIssueID = 0;
                            abTest.TestWinnerSelected = DateTimeHelper.ZERO_TIME;
                            // Hide/reload winner selection in issue mail-out grid
                            ucMailout.ReloadData(false);
                        }
                        IssueInfoProvider.SetIssueInfo(parentIssue);
                    }

                    ABTestInfoProvider.SetABTestInfo(abTest);

                    if (CurrentState == STATE_TEST_WAITING_TO_SEL_WINNER)
                    {
                        NewsletterTasksManager.EnsureWinnerSelectionTask(abTest, parentIssue, true, ucMailout.HighestMailoutTime);
                    }

                    // Update info message for parent control
                    int currentState = GetCurrentState(parentIssue);
                    InfoMessage = GetInfoMessage(currentState, parentIssue, abTest.TestWinnerOption, GetPlannedMailoutTime(ucMailout.HighestMailoutTime));
                    return true;
                case STATE_TEST_FINISHED:
                    ErrorMessage = GetString("newsletterissue_send.saveissuehasbeensent");
                    break;
            }
        }
        catch (Exception e)
        {
            ErrorMessage = e.Message;
            return false;
        }
        return true;
    }


    /// <summary>
    /// Determines current state of newsletter A/B test.
    /// </summary>
    /// <param name="issue">Parent issue</param>
    private int GetCurrentState(IssueInfo issue)
    {
        int currentState = 0;
        if (issue == null)
        {
            return currentState;
        }

        switch (issue.IssueStatus)
        {
            case IssueStatusEnum.Idle:
                if (Mode == SendControlMode.Send)
                {
                    currentState = STATE_WAITING_TO_SEND_PAGE;
                }
                else
                {
                    currentState = STATE_WAITING_TO_SEND_WIZARD;
                }
                break;
            case IssueStatusEnum.ReadyForSending:
                currentState = STATE_TEST_READY_FOR_SENDING;
                break;
            case IssueStatusEnum.TestPhase:
                currentState = STATE_TEST_WAITING_TO_SEL_WINNER;
                break;
            case IssueStatusEnum.PreparingData:
            case IssueStatusEnum.Sending:
            case IssueStatusEnum.Finished:
                currentState = STATE_TEST_FINISHED;
                break;
        }
        return currentState;
    }

    #endregion
}
