using System;
using System.Data;

using CMS.Helpers;
using CMS.Localization;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;
using CMS.Protection;

public partial class CMSAPIExamples_Code_Tools_AbuseReport_Default : CMSAPIExamplePage
{
    #region "Initialization"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Abuse report
        apiCreateAbuseReport.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateAbuseReport);
        apiGetAndUpdateAbuseReport.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndUpdateAbuseReport);
        apiGetAndBulkUpdateAbuseReports.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndBulkUpdateAbuseReports);
        apiDeleteAbuseReport.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteAbuseReport);
    }

    #endregion


    #region "Mass actions"

    /// <summary>
    /// Runs all creating and managing examples.
    /// </summary>
    public override void RunAll()
    {
        base.RunAll();

        // Abuse report
        apiCreateAbuseReport.Run();
        apiGetAndUpdateAbuseReport.Run();
        apiGetAndBulkUpdateAbuseReports.Run();
    }


    /// <summary>
    /// Runs all cleanup examples.
    /// </summary>
    public override void CleanUpAll()
    {
        base.CleanUpAll();

        // Abuse report
        apiDeleteAbuseReport.Run();
    }

    #endregion


    #region "API examples - Abuse report"

    /// <summary>
    /// Creates abuse report. Called when the "Create report" button is pressed.
    /// </summary>
    private bool CreateAbuseReport()
    {
        // Create new abuse report object
        AbuseReportInfo newReport = new AbuseReportInfo();

        // Set the properties
        newReport.ReportTitle = "MyNewReport";
        newReport.ReportComment = "This is an example abuse report.";

        newReport.ReportURL = URLHelper.GetAbsoluteUrl(RequestContext.CurrentURL);
        newReport.ReportCulture = LocalizationContext.PreferredCultureCode;
        newReport.ReportSiteID = SiteContext.CurrentSiteID;
        newReport.ReportUserID = MembershipContext.AuthenticatedUser.UserID;
        newReport.ReportWhen = DateTime.Now;
        newReport.ReportStatus = AbuseReportStatusEnum.New;

        // Save the abuse report
        AbuseReportInfoProvider.SetAbuseReportInfo(newReport);

        return true;
    }


    /// <summary>
    /// Gets and updates abuse report. Called when the "Get and update report" button is pressed.
    /// Expects the CreateAbuseReport method to be run first.
    /// </summary>
    private bool GetAndUpdateAbuseReport()
    {
        string where = "ReportTitle LIKE N'MyNewReport%'";

        // Get the report
        DataSet reports = AbuseReportInfoProvider.GetAbuseReports(where, null);

        if (!DataHelper.DataSourceIsEmpty(reports))
        {
            // Create the object from DataRow
            AbuseReportInfo updateReport = new AbuseReportInfo(reports.Tables[0].Rows[0]);

            // Update the properties
            updateReport.ReportStatus = AbuseReportStatusEnum.Solved;

            // Save the changes
            AbuseReportInfoProvider.SetAbuseReportInfo(updateReport);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates abuse reports. Called when the "Get and bulk update reports" button is pressed.
    /// Expects the CreateAbuseReport method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateAbuseReports()
    {
        // Prepare the parameters
        string where = "ReportTitle LIKE N'MyNewReport%'";

        // Get the data
        DataSet reports = AbuseReportInfoProvider.GetAbuseReports(where, null);
        if (!DataHelper.DataSourceIsEmpty(reports))
        {
            // Loop through the individual items
            foreach (DataRow reportDr in reports.Tables[0].Rows)
            {
                // Create object from DataRow
                AbuseReportInfo modifyReport = new AbuseReportInfo(reportDr);

                // Update the properties
                modifyReport.ReportStatus = AbuseReportStatusEnum.Rejected;

                // Save the changes
                AbuseReportInfoProvider.SetAbuseReportInfo(modifyReport);
            }

            return true;
        }

        return false;
    }


    /// <summary>
    /// Deletes abuse report. Called when the "Delete report" button is pressed.
    /// Expects the CreateAbuseReport method to be run first.
    /// </summary>
    private bool DeleteAbuseReport()
    {
        string where = "ReportTitle LIKE N'MyNewReport%'";

        // Get the report
        DataSet reports = AbuseReportInfoProvider.GetAbuseReports(where, null);

        if (!DataHelper.DataSourceIsEmpty(reports))
        {
            // Create the object from DataRow
            AbuseReportInfo deleteReport = new AbuseReportInfo(reports.Tables[0].Rows[0]);

            // Delete the abuse report
            AbuseReportInfoProvider.DeleteAbuseReportInfo(deleteReport);

            return true;
        }

        return false;
    }

    #endregion
}