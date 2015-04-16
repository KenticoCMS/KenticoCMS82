using System;
using System.Data;
using System.Globalization;
using System.Threading;

using CMS.Base;
using CMS.Helpers;
using CMS.Reporting;
using CMS.UIControls;


public partial class CMSModules_Reporting_Tools_Ecommerce_Report_Print : CMSModalPage
{
    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        if (QueryHelper.ValidateHash("hash", "UILang"))
        {
            // Get report name
            string reportName = QueryHelper.GetString("reportName", null);

            // Check permissions
            bool isEcommerceReport = ReportInfoProvider.IsEcommerceReport(reportName);
            CMSEcommerceReportsPage.CheckPermissions(isEcommerceReport);

            SetCulture();

            ReportInfo report = ReportInfoProvider.GetReportInfo(reportName);
            if (report != null)
            {
                // Get report parameters
                string parameters = QueryHelper.GetString("parameters", String.Empty);
                CultureInfo currentCulture = CultureHelper.GetCultureInfo(Thread.CurrentThread.CurrentUICulture.IetfLanguageTag);
                DataRow reportParameters = ReportHelper.GetReportParameters(report, parameters, null, CultureHelper.EnglishCulture, currentCulture);

                // Init report
                if (reportParameters != null)
                {
                    DisplayReport1.LoadFormParameters = false;
                    DisplayReport1.ReportParameters = reportParameters;
                }

                DisplayReport1.ReportName = reportName;
                DisplayReport1.DisplayFilter = false;

                Page.Title = GetString("report_print.lblprintreport") + " " + HTMLHelper.HTMLEncode(report.ReportDisplayName);
            }
        }
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        ManagersContainer = pnlManager;
    }
}