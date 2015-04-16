using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.DataEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.Base;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.WebAnalytics;

[EditedObject(CampaignInfo.OBJECT_TYPE, "campaignId")]

[UIElement("CMS.WebAnalytics", "Campaign.Reports")]
public partial class CMSModules_WebAnalytics_Pages_Tools_Campaign_Tab_Reports : CMSCampaignPage
{
    #region "Variables"

    private bool mIsSaved;
    private bool mReportLoaded;
    private string mCampaignName = String.Empty;
    private CampaignInfo mCampaignInfo;

    private IDisplayReport mUcDisplayReport;

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        reportHeaderActions.ActionPerformed += HeaderActions_ActionPerformed;
        mUcDisplayReport = (IDisplayReport)LoadUserControl("~/CMSModules/Reporting/Controls/DisplayReport.ascx");
        pnlContent.Controls.Add((Control)mUcDisplayReport);

        CurrentMaster.PanelContent.CssClass = string.Empty;
        UIHelper.AllowUpdateProgress = false;
        ScriptHelper.RegisterDialogScript(Page);

        // Campaign Info
        mCampaignInfo = EditedObject as CampaignInfo;
        if (mCampaignInfo == null)
        {
            return;
        }

        // Validate SiteID for non administrators
        if (!MembershipContext.AuthenticatedUser.IsGlobalAdministrator)
        {
            if (mCampaignInfo.CampaignSiteID != SiteContext.CurrentSiteID)
            {
                RedirectToAccessDenied(GetString("cmsmessages.accessdenied"));
            }
        }

        mCampaignName = mCampaignInfo.CampaignName;

        ucGraphType.ProcessChartSelectors(false);

        if (!RequestHelper.IsPostBack())
        {
            // Check the first radio button
            rbViews.Checked = true;
        }
    }


    /// <summary>
    /// Display report
    /// </summary>
    /// <param name="reload">If true, display report control is reloaded</param>
    private void DisplayReport(bool reload)
    {
        if (mReportLoaded)
        {
            return;
        }

        ucGraphType.ProcessChartSelectors(false);

        // Set reports name
        const string VIEWS = "campaign.yearreport;campaign.monthreport;campaign.weekreport;campaign.dayreport;campaign.hourreport";
        const string CONVERSION_COUNT = "campaignconversioncount.yearreport;campaignconversioncount.monthreport;campaignconversioncount.weekreport;campaignconversioncount.dayreport;campaignconversioncount.hourreport";
        const string CONVERSION_VALUE = "campaignconversionvalue.yearreport;campaignconversionvalue.monthreport;campaignconversionvalue.weekreport;campaignconversionvalue.dayreport;campaignconversionvalue.hourreport";
        const string DETAILS = "campaigns.singledetails";
        const string VISITORS_GOAL = "goalsnumberofvisitors.yearreport;goalsnumberofvisitors.monthreport;goalsnumberofvisitors.weekreport;goalsnumberofvisitors.dayreport;goalsnumberofvisitors.hourreport";
        const string VALUE_PER_VISITOR = "goalsvaluepervisit.yearreport;goalsvaluepervisit.monthreport;goalsvaluepervisit.weekreport;goalsvaluepervisit.dayreport;goalsvaluepervisit.hourreport";
        const string VALUE_GOAL = "goalsvalueofconversions.yearreport;goalsvalueofconversions.monthreport;goalsvalueofconversions.weekreport;goalsvalueofconversions.dayreport;goalsvalueofconversions.hourreport";
        const string COUNT_GOAL = "goalsnumberofconversions.yearreport;goalsnumberofconversions.monthreport;goalsnumberofconversions.weekreport;goalsnumberofconversions.dayreport;goalsnumberofconversions.hourreport";

        String codeName = String.Empty;
        pnlConversions.Visible = false;
        ucGraphType.EnableDateTimePicker = true;

        if (rbViews.Checked)
        {
            CheckWebAnalyticsUI("campaign.overview");
            codeName = "campaign";
            mUcDisplayReport.ReportName = ucGraphType.GetReportName(VIEWS);
        }

        if (rbCount.Checked)
        {
            CheckWebAnalyticsUI("CampaignConversionCount");
            pnlConversions.Visible = true;
            codeName = "campconversion";
            mUcDisplayReport.ReportName = ucGraphType.GetReportName(CONVERSION_COUNT);
        }

        if (rbValue.Checked)
        {
            CheckWebAnalyticsUI("campaignsConversionValue");
            pnlConversions.Visible = true;
            codeName = "campconversion";
            mUcDisplayReport.ReportName = ucGraphType.GetReportName(CONVERSION_VALUE);
        }

        if (rbDetail.Checked)
        {
            CheckWebAnalyticsUI("CampaignDetails");
            mUcDisplayReport.ReportName = ucGraphType.GetReportName(DETAILS);
            ucGraphType.EnableDateTimePicker = false;
        }

        if (rbGoalView.Checked)
        {
            CheckWebAnalyticsUI("goals.numberofvisitors");
            mUcDisplayReport.ReportName = ucGraphType.GetReportName(VISITORS_GOAL);
        }

        if (rbGoalCount.Checked)
        {
            CheckWebAnalyticsUI("goals.numberofconversions");
            mUcDisplayReport.ReportName = ucGraphType.GetReportName(COUNT_GOAL);
        }

        if (rbGoalValue.Checked)
        {
            CheckWebAnalyticsUI("goals.totalvalueofconversions");
            mUcDisplayReport.ReportName = ucGraphType.GetReportName(VALUE_GOAL);
        }

        if (rbValuePerVisitor.Checked)
        {
            CheckWebAnalyticsUI("goals.valuepervisitor");
            mUcDisplayReport.ReportName = ucGraphType.GetReportName(VALUE_PER_VISITOR);
        }

        // Load conversions fit to campaign
        if ((pnlConversions.Visible) && (mCampaignInfo != null))
        {
            ucConversions.PostbackOnDropDownChange = true;
            if (!mCampaignInfo.CampaignUseAllConversions)
            {
                ucConversions.WhereCondition = "ConversionID  IN (SELECT ConversionID FROM Analytics_ConversionCampaign WHERE CampaignID =" + mCampaignInfo.CampaignID + ")";
            }

            ucConversions.WhereCondition = SqlHelper.AddWhereCondition(ucConversions.WhereCondition, "ConversionSiteID =" + SiteContext.CurrentSiteID);
            ucConversions.ReloadData(true);
        }

        String conversion = ValidationHelper.GetString(ucConversions.Value, String.Empty);
        if (conversion == ucConversions.AllRecordValue)
        {
            conversion = String.Empty;
        }

        // General report data
        mUcDisplayReport.LoadFormParameters = false;
        mUcDisplayReport.DisplayFilter = false;
        mUcDisplayReport.GraphImageWidth = 100;
        mUcDisplayReport.IgnoreWasInit = true;
        mUcDisplayReport.TableFirstColumnWidth = Unit.Percentage(30);
        mUcDisplayReport.UseExternalReload = true;
        mUcDisplayReport.UseProgressIndicator = true;

        // Resolve report macros 
        DataTable reportParameters = new DataTable();
        reportParameters.Columns.Add("FromDate", typeof(DateTime));
        reportParameters.Columns.Add("ToDate", typeof(DateTime));
        reportParameters.Columns.Add("CodeName", typeof(string));
        reportParameters.Columns.Add("CampaignName", typeof(string));
        reportParameters.Columns.Add("ConversionName", typeof(string));

        object[] parameters = new object[5];
        parameters[0] = ucGraphType.From;
        parameters[1] = ucGraphType.To;
        parameters[2] = codeName;
        parameters[3] = mCampaignName;
        parameters[4] = conversion;

        reportParameters.Rows.Add(parameters);
        reportParameters.AcceptChanges();
        mUcDisplayReport.ReportParameters = reportParameters.Rows[0];

        if (reload)
        {
            mUcDisplayReport.ReloadData(true);
        }

        mReportLoaded = true;
    }


    /// <summary>
    /// Handles lnkSave click event.
    /// </summary>
    protected void lnkSave_Click(object sender, EventArgs e)
    {
        Save();
    }


    protected override void OnPreRender(EventArgs e)
    {
        DisplayReport(true);

        reportHeaderActions.ReportName = mUcDisplayReport.ReportName;
        reportHeaderActions.ReportParameters = mUcDisplayReport.ReportParameters;
        reportHeaderActions.SelectedInterval = ucGraphType.SelectedInterval;
        reportHeaderActions.ManageDataCodeName = "singlecampaign;" + mCampaignName;

        base.OnPreRender(e);
    }


    /// <summary>
    /// VerifyRenderingInServerForm.
    /// </summary>
    /// <param name="control">Control</param>
    public override void VerifyRenderingInServerForm(Control control)
    {
        if (!mIsSaved)
        {
            base.VerifyRenderingInServerForm(control);
        }
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


    /// <summary>
    /// Saves the graph report.
    /// </summary>
    private void Save()
    {
        // Check web analytics save permission
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.WebAnalytics", "SaveReports"))
        {
            RedirectToAccessDenied("CMS.WebAnalytics", "SaveReports");
        }

        DisplayReport(false);

        // Saves the report        
        mIsSaved = true;

        if (mUcDisplayReport.SaveReport() > 0)
        {
            lblInfo.Visible = true;
            lblInfo.Text = String.Format(GetString("Analytics_Report.ReportSavedTo"), mUcDisplayReport.ReportDisplayName + " - " + DateTime.Now.ToString());
        }

        mIsSaved = false;
    }

    #endregion
}