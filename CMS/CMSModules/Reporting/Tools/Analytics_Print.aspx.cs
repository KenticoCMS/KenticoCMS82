using System;
using System.Data;
using System.Globalization;
using System.Threading;

using CMS.Helpers;
using CMS.Reporting;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;
using CMS.WebAnalytics;
using CMS.Modules;

public partial class CMSModules_Reporting_Tools_Analytics_Print : CMSWebAnalyticsPage
{
    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        if (QueryHelper.ValidateHash("hash", "UILang"))
        {
            // Check security
            CheckSecurity();

            // Set cultures
            SetCulture();
            CultureInfo currentCulture = CultureHelper.GetCultureInfo(Thread.CurrentThread.CurrentUICulture.IetfLanguageTag);

            // Get report info
            string reportName = QueryHelper.GetString("reportname", String.Empty);
            ReportInfo report = ReportInfoProvider.GetReportInfo(reportName);

            if (report != null)
            {
                // Get report parameters
                string parameters = QueryHelper.GetString("parameters", String.Empty);
                DataRow reportParameters = ReportHelper.GetReportParameters(report, parameters, AnalyticsHelper.PARAM_SEMICOLON, CultureHelper.EnglishCulture, currentCulture);

                // Init report
                if (reportParameters != null)
                {
                    displayReport.LoadFormParameters = false;
                    displayReport.ReportParameters = reportParameters;
                }

                displayReport.ReportName = report.ReportName;
                displayReport.DisplayFilter = false;

                Page.Title = GetString("Report_Print.lblPrintReport") + " " + HTMLHelper.HTMLEncode(report.ReportDisplayName);
            }
        }
    }


    protected override void OnLoad(EventArgs e)
    {
        ManagersContainer = plcMenu;
    }


    /// <summary>
    /// Checks security.
    /// </summary>
    private void CheckSecurity()
    {
        // Check site availability
        if (!ResourceSiteInfoProvider.IsResourceOnSite("CMS.Reporting", SiteContext.CurrentSiteName))
        {
            RedirectToResourceNotAvailableOnSite("CMS.Reporting");
        }

        // Check 'Read' permission
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Reporting", "Read"))
        {
            RedirectToAccessDenied("CMS.Reporting", "Read");
        }
    }
}