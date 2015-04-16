using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Helpers;
using CMS.Membership;
using CMS.Base;
using CMS.UIControls;
using CMS.ExtendedControls;

public partial class CMSModules_WebAnalytics_Pages_Tools_Campaign_CampaignGoals : CMSCampaignPage
{
    #region "Variables"

    private bool mIsSaved;
    private bool mReportDisplayed;
    private string mStatCodeName = String.Empty;
    private string mDataCodeName = String.Empty;
    private IDisplayReport mUcDisplayReport;

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


    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentMaster.PanelContent.CssClass = "";
        reportHeader.ActionPerformed += HeaderActions_ActionPerformed;

        mStatCodeName = QueryHelper.GetString("statCodeName", String.Empty);
        mDataCodeName = QueryHelper.GetString("dataCodeName", String.Empty);

        mUcDisplayReport = (IDisplayReport)LoadUserControl("~/CMSModules/Reporting/Controls/DisplayReport.ascx");
        pnlDisplayReport.Controls.Add((Control)mUcDisplayReport);

        CheckWebAnalyticsUI(mDataCodeName);

        // Set disabled module info
        ucDisabledModule.SettingsKeys = "CMSAnalyticsEnabled;";
        ucDisabledModule.ParentPanel = pnlDisabled;

        PageTitle.TitleText = GetString("analytics_codename.campaign") + " - " + GetString("analytics_codename." + HTMLHelper.HTMLEncode(mStatCodeName));
    }


    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case ComponentEvents.SAVE:
                DisplayReport(true);
                Save();
                break;
        }
    }


    /// <summary>
    /// OnPreRender - Display report
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        DisplayReport(true);

        reportHeader.ReportName = mUcDisplayReport.ReportName;
        reportHeader.ReportParameters = mUcDisplayReport.ReportParameters;
        reportHeader.SelectedInterval = ucGraphType.SelectedInterval;
        reportHeader.DisplayManageData = false;

        base.OnPreRender(e);
    }


    /// <summary>
    /// Used in rendering to control outside render stage (save method) 
    /// </summary>
    /// <param name="control">Control</param>
    public override void VerifyRenderingInServerForm(Control control)
    {
        if (!mIsSaved)
        {
            base.VerifyRenderingInServerForm(control);
        }
    }


    /// <summary>
    /// Displays the given report
    /// </summary>
    private void DisplayReport(bool reloadInnerReport)
    {
        // If report was already displayed .. return
        if (mReportDisplayed)
        {
            return;
        }

        ucGraphType.ProcessChartSelectors(false);

        // Prepare report parameters
        DataTable reportParameters = new DataTable();

        reportParameters.Columns.Add("FromDate", typeof(DateTime));
        reportParameters.Columns.Add("ToDate", typeof(DateTime));
        reportParameters.Columns.Add("CampaignName", typeof(string));

        object[] parameters = new object[3];

        parameters[0] = ucGraphType.From;
        parameters[1] = ucGraphType.To;
        parameters[2] = "";

        // Get report name from query
        string reportName = ucGraphType.GetReportName(QueryHelper.GetString("reportCodeName", String.Empty));

        if (CMSString.Compare(Convert.ToString(ucSelectCampaign.Value), "-1", true) != 0)
        {
            parameters[2] = ucSelectCampaign.Value;
        }
        else
        {
            reportName = "all" + reportName;
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
            if (reloadInnerReport)
            {
                mUcDisplayReport.ReloadData(true);
            }
        }

        if (reloadInnerReport)
        {
            // Mark as report displayed
            mReportDisplayed = true;
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

        mIsSaved = true;

        if (mUcDisplayReport.SaveReport() > 0)
        {
            ShowConfirmation(String.Format(GetString("Analytics_Report.ReportSavedTo"), mUcDisplayReport.ReportDisplayName + " - " + DateTime.Now.ToString()));
        }

        mIsSaved = false;
    }
}