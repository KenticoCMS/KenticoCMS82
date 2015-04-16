using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Helpers;
using CMS.Membership;
using CMS.Base;
using CMS.UIControls;
using CMS.WebAnalytics;
using CMS.ExtendedControls;


public partial class CMSModules_WebAnalytics_Pages_Tools_Campaign_CampaignReport : CMSCampaignPage
{
    #region "Variables"

    private bool isSaved;
    private bool reportDisplayed;
    private String dataCodeName = String.Empty;
    private String reportCodeNames = String.Empty;
    private int conversionID;

    private const string allDetailReport = "campaigns.alldetails";
    private const string singleDetailReport = "campaigns.singledetails";
    private IDisplayReport mUcDisplayReport;

    private bool mAllowNoTimeSelection;
    private string mDeleteParam = "campaigns";

    #endregion


    #region "Properties"

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

    #endregion


    #region "Page Events"

    protected void Page_Load(object sender, EventArgs e)
    {
        string uiTrace = QueryHelper.GetString("ui", string.Empty);
        if (uiTrace == "omanalytics")
        {
            // Check UI elements
            if (!CurrentUser.IsAuthorizedPerUIElement("CMS.WebAnalytics", "Analytics.Campaigns.Overview"))
            {
                RedirectToUIElementAccessDenied("CMS.WebAnalytics", "Analytics.Campaigns.Overview");
            }

            if (!CurrentUser.IsAuthorizedPerUIElement("CMS.WebAnalytics", "Campaigns"))
            {
                RedirectToUIElementAccessDenied("CMS.WebAnalytics", "Campaigns");
            }
        }

        conversionID = QueryHelper.GetInteger("conversionid", 0);
        CurrentMaster.PanelContent.CssClass = "";
        UIHelper.AllowUpdateProgress = false;

        reportHeaderActions.ActionPerformed += HeaderActions_ActionPerformed;

        // Set disabled module info
        ucDisabledModule.SettingsKeys = "CMSAnalyticsEnabled;";
        ucDisabledModule.ParentPanel = pnlDisabled;

        mUcDisplayReport = (IDisplayReport)LoadUserControl("~/CMSModules/Reporting/Controls/DisplayReport.ascx");
        pnlDisplayReport.Controls.Add((Control)mUcDisplayReport);

        dataCodeName = QueryHelper.GetString("dataCodeName", String.Empty);
        reportCodeNames = QueryHelper.GetString("reportCodeName", String.Empty);

        // Control initialization (based on displayed report)
        switch (dataCodeName)
        {
            // Overview
            case "campaign":
                CheckWebAnalyticsUI("campaign.overview");
                ucReportHeader.CampaignAllowAll = true;
                ucReportHeader.ShowConversionSelector = false;
                break;

            // Conversion count 
            case "campconversioncount":
                dataCodeName = "campconversion";
                CheckWebAnalyticsUI("CampaignConversionCount");
                ucReportHeader.CampaignAllowAll = false;
                break;

            // Conversion value 
            case "campconversionvalue":
                dataCodeName = "campconversion";
                CheckWebAnalyticsUI("campaignsConversionValue");
                ucReportHeader.CampaignAllowAll = false;
                break;

            // Campaign compare
            case "campaigncompare":
                CheckWebAnalyticsUI("CampaignCompare");
                ucReportHeader.ShowCampaignSelector = false;
                dataCodeName = ucReportHeader.CodeNameByGoal;
                ucReportHeader.ShowGoalSelector = true;
                ucReportHeader.ShowSiteSelector = true;

                // Get table column name
                string name = "analytics.hits";
                switch (ucReportHeader.SelectedGoal.ToLowerCSafe())
                {
                    case "view":
                        name = "analytics.view";
                        break;
                    case "count":
                        name = "conversion.count";
                        break;
                    case "value":
                        name = "om.trackconversionvalue";
                        break;
                }

                string[,] dynamicMacros = new string[1, 2];
                dynamicMacros[0, 0] = "ColumnName";
                dynamicMacros[0, 1] = GetString(name);

                mUcDisplayReport.DynamicMacros = dynamicMacros;
                break;

            // Campaign detail
            case "campaigndetails":
                CheckWebAnalyticsUI("CampaignDetails");
                ucReportHeader.ShowConversionSelector = false;
                String selectedCampaign = ValidationHelper.GetString(ucReportHeader.SelectedCampaign, String.Empty);
                reportCodeNames = (selectedCampaign == ucReportHeader.AllRecordValue || selectedCampaign == String.Empty) ? allDetailReport : singleDetailReport;
                ucGraphType.ShowIntervalSelector = false;
                mAllowNoTimeSelection = true;
                ucGraphType.AllowEmptyDate = true;
                break;

            case "conversion":
                CheckWebAnalyticsUI("Conversion.Overview");
                ucReportHeader.ShowCampaignSelector = false;
                ucReportHeader.ShowGoalSelector = true;
                ucReportHeader.ShowVisitsInGoalSelector = false;
                mDeleteParam = "conversion";
                break;

            case "conversiondetail":
                CheckWebAnalyticsUI("Conversion.Details");
                ucReportHeader.ShowCampaignSelector = false;
                ucReportHeader.ShowConversionSelector = (conversionID == 0);
                ucGraphType.ShowIntervalSelector = false;
                mAllowNoTimeSelection = true;
                ucGraphType.AllowEmptyDate = true;
                mDeleteParam = "conversion";
                break;

            case "conversionpropertiesdetail":
                CheckWebAnalyticsUI("Conversions.Detail");
                ucReportHeader.ShowCampaignSelector = false;
                ucReportHeader.ShowConversionSelector = (conversionID == 0);
                ucGraphType.ShowIntervalSelector = false;
                mAllowNoTimeSelection = true;
                ucGraphType.AllowEmptyDate = true;
                mDeleteParam = "conversion";
                break;
        }

        // Set table same first column width for all time
        mUcDisplayReport.TableFirstColumnWidth = Unit.Percentage(20);
    }


    protected override void OnPreRender(EventArgs e)
    {
        DisplayReport();

        reportHeaderActions.ReportName = mUcDisplayReport.ReportName;
        reportHeaderActions.ReportParameters = mUcDisplayReport.ReportParameters;
        reportHeaderActions.SelectedInterval = ucGraphType.SelectedInterval;
        reportHeaderActions.ManageDataCodeName = mDeleteParam;

        base.OnPreRender(e);
    }

    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case ComponentEvents.SAVE:
                Save();
                break;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Displays the given report
    /// </summary>
    private void DisplayReport()
    {
        // If report was already displayed .. return
        if (reportDisplayed)
        {
            return;
        }

        ucGraphType.ProcessChartSelectors(false);

        // Prepare report parameters
        DataTable reportParameters = new DataTable();

        // In case of hidden datetime -> for save purpose pass from (to) as now to query parameter
        DateTime from = ((ucGraphType.From == DateTimeHelper.ZERO_TIME) && !pnlHeader.Visible) ? DateTime.Now : ucGraphType.From;
        DateTime to = ((ucGraphType.To == DateTimeHelper.ZERO_TIME) && !pnlHeader.Visible) ? DateTime.Now : ucGraphType.To;

        reportParameters.Columns.Add("FromDate", typeof(DateTime));
        reportParameters.Columns.Add("ToDate", typeof(DateTime));
        reportParameters.Columns.Add("CodeName", typeof(string));
        reportParameters.Columns.Add("CampaignName", typeof(string));
        reportParameters.Columns.Add("ConversionName", typeof(string));
        reportParameters.Columns.Add("Goal", typeof(string));
        reportParameters.Columns.Add("SiteID", typeof(int));

        object[] parameters = new object[7];

        parameters[0] = (mAllowNoTimeSelection && from == DateTimeHelper.ZERO_TIME) ? (DateTime?)null : from;
        parameters[1] = (mAllowNoTimeSelection && to == DateTimeHelper.ZERO_TIME) ? (DateTime?)null : to;
        parameters[2] = dataCodeName;
        parameters[3] = String.Empty;
        parameters[4] = String.Empty;
        parameters[5] = ucReportHeader.SelectedGoal;
        parameters[6] = ucReportHeader.SelectedSiteID;


        // Get report name from query
        String reportName = ucGraphType.GetReportName(reportCodeNames);

        // Filter campaign if any campaign selected
        string campaignName = ValidationHelper.GetString(ucReportHeader.SelectedCampaign, String.Empty);
        if ((campaignName != ucReportHeader.AllRecordValue) && (!String.IsNullOrEmpty(campaignName)))
        {
            parameters[3] = campaignName;
        }

        if (conversionID == 0)
        {
            // Filter conversion
            String conversionName = ValidationHelper.GetString(ucReportHeader.SelectedConversion, String.Empty);
            if ((conversionName != ucReportHeader.AllRecordValue) && (!String.IsNullOrEmpty(conversionName)))
            {
                parameters[4] = conversionName;
            }
        }
        else
        {
            ConversionInfo ci = ConversionInfoProvider.GetConversionInfo(conversionID);
            if (ci != null)
            {
                parameters[4] = ci.ConversionName;
                mDeleteParam = "singleconversion;" + ci.ConversionName;
            }
        }

        reportParameters.Rows.Add(parameters);
        reportParameters.AcceptChanges();

        mUcDisplayReport.ReportName = reportName;

        // Set display report
        if (!mUcDisplayReport.IsReportLoaded())
        {
            ShowError(String.Format(GetString("Analytics_Report.ReportDoesnotExist"), HTMLHelper.HTMLEncode(reportName)));
        }
        else
        {
            mUcDisplayReport.LoadFormParameters = false;
            mUcDisplayReport.DisplayFilter = false;
            mUcDisplayReport.ReportParameters = reportParameters.Rows[0];
            mUcDisplayReport.GraphImageWidth = 100;
            mUcDisplayReport.IgnoreWasInit = true;
            mUcDisplayReport.UseExternalReload = true;
            mUcDisplayReport.UseProgressIndicator = true;
            mUcDisplayReport.SelectedInterval = HitsIntervalEnumFunctions.HitsConversionToString(ucGraphType.SelectedInterval);
            mUcDisplayReport.ReloadData(true);
        }

        // Mark as report displayed
        reportDisplayed = true;
    }


    /// <summary>
    /// Used in rendering to control outside render stage (save method) 
    /// </summary>
    /// <param name="control">Control</param>
    public override void VerifyRenderingInServerForm(Control control)
    {
        if (!isSaved)
        {
            base.VerifyRenderingInServerForm(control);
        }
    }


    /// <summary>
    /// Saves the graph report.
    /// </summary>
    private void Save()
    {
        DisplayReport();

        // Check web analytics save permission
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.WebAnalytics", "SaveReports"))
        {
            RedirectToAccessDenied("CMS.WebAnalytics", "SaveReports");
        }

        isSaved = true;

        if (mUcDisplayReport.SaveReport() > 0)
        {
            ShowConfirmation(String.Format(GetString("Analytics_Report.ReportSavedTo"), mUcDisplayReport.ReportDisplayName + " - " + DateTime.Now.ToString()));
        }

        isSaved = false;
    }

    #endregion
}