using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Helpers;
using CMS.Membership;
using CMS.UIControls;

public partial class CMSModules_WebAnalytics_Tools_Analytics_Report : CMSWebAnalyticsPage
{
    #region "Variables"

    private bool isSaved;
    private string statCodeName;

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Check analytics UI
        CheckWebAnalyticsUI();

        reportHeader.ActionPerformed += HeaderActions_ActionPerformed;

        CurrentMaster.PanelContent.CssClass = "";
        ScriptHelper.RegisterDialogScript(Page);

        // Set disabled module info
        ucDisabledModule.SettingsKeys = "CMSAnalyticsEnabled;";
        ucDisabledModule.ParentPanel = pnlDisabled;

        UIHelper.AllowUpdateProgress = false;

        statCodeName = QueryHelper.GetString("statCodeName", String.Empty);
        string reportCodeName = QueryHelper.GetString("reportCodeName", String.Empty);
        string dataCodeName = QueryHelper.GetText("dataCodeName", String.Empty);

        ucReportViewer.DataName = dataCodeName;
        ucReportViewer.ReportsCodeName = reportCodeName;

        bool displayTitle = QueryHelper.GetBoolean("DisplayTitle", true);
        if (displayTitle)
        {
            PageTitle.TitleText = GetString("analytics_codename." + HTMLHelper.HTMLEncode(statCodeName));
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
    /// VerifyRenderingInServerForm.
    /// </summary>
    /// <param name="control">Control</param>
    public override void VerifyRenderingInServerForm(Control control)
    {
        if (!isSaved)
        {
            base.VerifyRenderingInServerForm(control);
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        ucReportViewer.DisplayReport(false);
        reportHeader.ReportName = ucReportViewer.ReportName;
        reportHeader.ReportParameters = ucReportViewer.ReportParameters;
        reportHeader.SelectedInterval = ucReportViewer.SelectedInterval;
        base.OnPreRender(e);
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

        isSaved = true;

        // Saves the report 
        ucReportViewer.SaveReport();

        isSaved = false;
    }

    #endregion
}