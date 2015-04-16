using System;
using System.Collections;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.WebAnalytics;

public partial class CMSModules_WebAnalytics_Tools_Analytics_DataManagement : CMSWebAnalyticsPage
{
    #region "Variables"

    private const string MULTILINGUAL_SUFFIX = ".multilingual";

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        CheckWebAnalyticsUI("DataManagement");
        // Master template title, image and context help
        PageTitle.TitleText = GetString("analytics_codename.datamanagement");

        // Set disabled module info
        ucDisabledModule.SettingsKeys = "CMSAnalyticsEnabled;";
        ucDisabledModule.ParentPanel = pnlDisabled;

        // Display disabled information
        if (!AnalyticsHelper.AnalyticsEnabled(SiteContext.CurrentSiteName))
        {
            EnableControls(false);
            return;
        }

        // Initialize controls
        Initialize();
        ShowWarning(GetString("analytics.generator.dataoverwrite"));

        // Check whether generator is running and if so than disable controls and display appropriate message
        if (SampleDataGenerator.DataGeneratorIsRunning)
        {
            timeRefresh.Enabled = true;
            ViewState["GeneratorStarted"] = true;
            EnableControls(false);
            ReloadInfoPanel();
        }
        else if (StatisticsInfoProvider.DataDeleterIsRunning)
        {
            timeRefresh.Enabled = true;
            ViewState["DeleterStarted"] = true;
            EnableControls(false);
            ReloadInfoPanel();
        }
        else
        {
            // If generator has just end, display OK message and disable timer
            if (ValidationHelper.GetBoolean(ViewState["GeneratorStarted"], false))
            {
                ViewState.Remove("GeneratorStarted");

                // Fill objects to delete
                FillObjectsToDelete();

                // Display info message
                ShowConfirmation(GetString("anal.settings.datagenerated"));

                // Disable timer
                timeRefresh.Enabled = false;
            }

            // If data deleter has just end
            if (ValidationHelper.GetBoolean(ViewState["DeleterStarted"], false))
            {
                ViewState.Remove("DeleterStarted");

                FillObjectsToDelete();
                FillStatisticsBoundaries();

                ShowConfirmation(GetString("analyt.settings.datadeleted"));

                // Disable timer
                timeRefresh.Enabled = false;
            }

            EnableControls(true);
        }

        FillStatisticsBoundaries();
    }


    /// <summary>
    /// Fill statistics boundaries (from - to)
    /// </summary>
    private void FillStatisticsBoundaries()
    {
        // Fill statistics boundaries
        string statCodeName = drpDeleteObjects.SelectedValue;

        String where = GenerateWhereCondition(statCodeName);

        // Add 'AND' if where condition applied 
        if (!String.IsNullOrEmpty(where))
        {
            where = " AND " + where;
        }

        // Select data from current site and filter by codename
        string boundariesWhere = "HitsStatisticsID IN (SELECT StatisticsID FROM Analytics_Statistics WHERE StatisticsSiteID =" + SiteContext.CurrentSiteID + @where + ") ";

        // Set interval text for no records 
        lblIntervalInfo.Text = "-";

        DataSet ds = HitsDayInfoProvider.GetStatisticsBoundaries(boundariesWhere);
        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            DateTime dtFrom = ValidationHelper.GetDateTime(ds.Tables[0].Rows[0]["DateFrom"], DateTimeHelper.ZERO_TIME);
            DateTime dtTo = ValidationHelper.GetDateTime(ds.Tables[0].Rows[0]["DateTo"], DateTimeHelper.ZERO_TIME);
            if ((dtFrom != DateTimeHelper.ZERO_TIME) && (dtTo != DateTimeHelper.ZERO_TIME))
            {
                lblIntervalInfo.Text = dtFrom.ToString("d") + "  -  " + dtTo.ToString("d");
            }
        }
    }


    /// <summary>
    /// Generate where condition for given codename
    /// </summary>
    /// <param name="statCodeName">Code name of statistics</param>
    private string GenerateWhereCondition(string statCodeName)
    {
        // Special cases
        switch (statCodeName)
        {
            case "":
                return String.Empty;

            case "abtest":
                return " StatisticsCode LIKE 'ab%'";

            case "mvtest":
                return " StatisticsCode LIKE 'mvtconversion%'";

            case "campaign":
                return " (StatisticsCode LIKE 'campconversion%' OR StatisticsCode = 'campaign') ";

            default:
                return " StatisticsCode = N'" + statCodeName.Replace("'", "''") + "'";
        }
    }


    /// <summary>
    /// Displays generator info panel
    /// </summary>
    private void ReloadInfoPanel()
    {
        if (SampleDataGenerator.DataGeneratorIsRunning)
        {
            ltrProgress.Text = ScriptHelper.GetLoaderInlineHtml(Page, "&nbsp;" + GetString("analyt.settings.generatorinprogress"));
        }

        if (StatisticsInfoProvider.DataDeleterIsRunning)
        {
            ltrProgress.Text = ScriptHelper.GetLoaderInlineHtml(Page, "&nbsp;" + GetString("analyt.settings.deleterinprogress"));
        }
    }


    /// <summary>
    /// Initialize page controls
    /// </summary>
    private void Initialize()
    {
        // Check post back
        if (!RequestHelper.IsPostBack())
        {
            FillObjectsToDelete();

            SortedList list = new SortedList();

            // Loop thru all statistic pre-defined codenames
            foreach (string codeName in SampleDataGenerator.StatisticCodeNames)
            {
                // Try get display name
                string displayName = GetString("analytics_codename." + codeName);

                // If display name is not defined use codename
                if (displayName.EqualsCSafe("analytics_codename." + codeName, true))
                {
                    displayName = codeName;
                }

                // Add to the list collection
                if (!list.Contains(displayName))
                {
                    list.Add(displayName, new ListItem(displayName, codeName));
                }
                else
                {
                    // If display name already in collection - add special display name
                    list.Add(displayName + codeName, new ListItem(displayName + " (" + codeName + ")", codeName));
                }
            }

            // Add default (all) value
            drpGenerateObjects.Items.Insert(0, new ListItem(GetString("general.selectall"), ""));

            // Add values from sorted list
            foreach (ListItem li in list.Values)
            {
                drpGenerateObjects.Items.Add(li);
            }

            // Initialize sample data range selector for 2 months range
            DateTime date = DateTime.Now;
            ucSampleFrom.SelectedDateTime = date.AddMonths(-2);
            ucSampleTo.SelectedDateTime = date;
        }
    }


    /// <summary>
    /// Fills drop drop down field for deleted objects
    /// </summary>
    private void FillObjectsToDelete()
    {
        drpDeleteObjects.Items.Clear();

        SortedList list = new SortedList();

        var statistics =
            StatisticsInfoProvider.GetStatistics()
                                  .Column("StatisticsCode")
                                  .Distinct()
                                  .WhereEquals("StatisticsSiteID", SiteContext.CurrentSiteID)
                                  .WhereNotContains("StatisticsCode", ";");

        foreach (var statisticsInfo in statistics)
        {
            // Statistic codename
            string codeName = statisticsInfo.StatisticsCode;
            // Statistic dispaly name
            string displayName = GetString("analytics_codename." + codeName);
            // If resource string is not available use codename
            if (displayName.EqualsCSafe("analytics_codename." + codeName, true))
            {
                displayName = codeName;
            }

            if (!list.Contains(displayName))
            {
                // Add to the list collection
                list.Add(displayName, new ListItem(displayName, codeName));
            }
            else
            {
                // If display name already in collection - add special display name
                list.Add(displayName + codeName, new ListItem(displayName + " (" + codeName + ")", codeName));
            }
        }

        // Add A/B and M/V testing
        list.Add(GetString("analytics_codename.abtest"), new ListItem(GetString("analytics_codename.abtest"), "abtest"));
        list.Add(GetString("analytics_codename.mvtest"), new ListItem(GetString("analytics_codename.mvtest"), "mvtest"));

        // Add values from sorted list
        foreach (ListItem li in list.Values)
        {
            drpDeleteObjects.Items.Add(li);
        }

        // Add default (all) value
        drpDeleteObjects.Items.Insert(0, new ListItem(GetString("general.selectall"), ""));
    }


    /// <summary>
    /// Generate sample data button click handler
    /// </summary>
    protected void btnGenerate_Click(object sender, EventArgs e)
    {
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.WebAnalytics", "ManageData"))
        {
            RedirectToAccessDenied("CMS.WebAnalytics", "ManageData");
        }

        // Check whether range is defined
        if ((ucSampleFrom.SelectedDateTime == DateTimeHelper.ZERO_TIME) || (ucSampleTo.SelectedDateTime == DateTimeHelper.ZERO_TIME))
        {
            ShowError(GetString("analyt.settings.invalidrangegenerate"));
            return;
        }

        // Try start sample data generator
        if (SampleDataGenerator.GenerateSampleData(ucSampleFrom.SelectedDateTime, ucSampleTo.SelectedDateTime, SiteContext.CurrentSiteID, drpGenerateObjects.SelectedValue))
        {
            EnableControls(false);
            ViewState["GeneratorStarted"] = true;
        }

        // Start refresh timer
        timeRefresh.Enabled = true;

        // Display info label and loading image
        ReloadInfoPanel();
    }


    /// <summary>
    /// Delete analytics data button click handler
    /// </summary>
    protected void btnDelete_Click(object sender, EventArgs e)
    {
        // Check whether current user is authorized to manage analytics data
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.WebAnalytics", "ManageData"))
        {
            RedirectToAccessDenied("CMS.WebAnalytics", "ManageData");
        }

        string statCodeName = drpDeleteObjects.SelectedValue;

        DateTime fromDate = ucDeleteFrom.SelectedDateTime;
        DateTime toDate = ucDeleteTo.SelectedDateTime;

        // Remove all data
        if (String.IsNullOrEmpty(statCodeName))
        {
            StatisticsInfoProvider.RemoveAnalyticsDataAsync(fromDate, toDate, SiteContext.CurrentSiteID, String.Empty);
        }
        // Remove data from specific report
        else
        {
            String where;

            // Stats for visitors needs special manipulation (it consist of two types
            // of statistics with different code names - new visitor and returning visitor)
            if (statCodeName.ToLowerCSafe() != HitLogProvider.VISITORS_FIRST)
            {
                // Ignore multilingual suffix (multilingual stats use the same data as "base" stats)
                if (statCodeName.ToLowerCSafe().EndsWithCSafe(MULTILINGUAL_SUFFIX))
                {
                    statCodeName = statCodeName.Remove(statCodeName.Length - MULTILINGUAL_SUFFIX.Length);
                }

                // Add where condition based on stat code name
                where = GenerateWhereCondition(statCodeName);

                // Recalculate/delete ordinary stats
                StatisticsInfoProvider.RemoveAnalyticsDataAsync(fromDate, toDate, SiteContext.CurrentSiteID, where);
            }
            else
            {
                where = "(StatisticsCode = '" + HitLogProvider.VISITORS_FIRST + "' OR StatisticsCode ='" + HitLogProvider.VISITORS_RETURNING + "')";
                StatisticsInfoProvider.RemoveAnalyticsDataAsync(fromDate, toDate, SiteContext.CurrentSiteID, where);
            }
        }

        // Disable controls 
        EnableControls(false);
        ViewState["DeleterStarted"] = true;

        // Start refresh timer
        timeRefresh.Enabled = true;

        // Display info label and loading image
        ReloadInfoPanel();
    }


    protected override void OnPreRender(EventArgs e)
    {
        // Delete confirmation
        btnDelete.OnClientClick = "return confirmDialog ('" + ucDeleteFrom.DateTimeTextBox.ClientID + "','" + ucDeleteTo.DateTimeTextBox.ClientID + "','" + drpDeleteObjects.ClientID + "');";

        String script = @"
            function confirmDialog(txtFrom, txtTo, txtCode) {
                var from = document.getElementById(txtFrom).value;
                var to = document.getElementById(txtTo).value;
                var code = document.getElementById(txtCode);
                var codeText = code.options[code.selectedIndex].text;
                if (from != '' || to != '') {
                    var text = '" + ScriptHelper.GetLocalizedString("analytics.delete.confirmation", false) + @" \'' + codeText + '\' " + ScriptHelper.GetLocalizedString("analytics.delete.confirmation.interval", false) + @" ';
        
                    if (from == '') {
                        from = 'unspecified';
                    }
                    if (to == '') {
                        to = 'unspecified';
                    }

                    text += from + ' - ' + to+'.';
                }
                else {
                    // Text for no date specified
                    var text = ' " + ScriptHelper.GetLocalizedString("analytics.delete.confirmation.all", false) + @" \'' + codeText+'\'.';
                }

                return confirm(text);
           }";

        ScriptHelper.RegisterStartupScript(this, typeof(string), "deleteconfirmation", ScriptHelper.GetScript(script));

        base.OnPreRender(e);
    }


    /// <summary>
    /// Disable(enable) controls
    /// </summary>
    /// <param name="enable">Indicates whether enable or disable controls</param>
    private void EnableControls(bool enable)
    {
        btnDelete.Enabled = enable;
        btnGenerate.Enabled = enable;
        drpDeleteObjects.Enabled = enable;
        drpGenerateObjects.Enabled = enable;
        ucDeleteFrom.Enabled = enable;
        ucDeleteTo.Enabled = enable;
        ucSampleFrom.Enabled = enable;
        ucSampleTo.Enabled = enable;
    }

    #endregion
}