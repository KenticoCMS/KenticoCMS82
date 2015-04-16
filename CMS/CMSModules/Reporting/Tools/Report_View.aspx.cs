using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Helpers;
using CMS.Membership;
using CMS.Reporting;
using CMS.UIControls;

[UIElement("CMS.Reporting", "ViewReport")]
public partial class CMSModules_Reporting_Tools_Report_View : CMSReportingPage
{
    #region "Variables"

    private int reportId;
    private bool isSaved;

    #endregion


    #region "Page events"

    /// <summary>
    /// OnInit override.
    /// </summary>
    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        // Ensure the script manager
        EnsureScriptManager();

        reportId = QueryHelper.GetInteger("ReportId", 0);
        ReportInfo ri = ReportInfoProvider.GetReportInfo(reportId);
        if (ri != null)
        {
            displayReport.ReportName = ri.ReportName;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        ScriptHelper.RegisterDialogScript(Page);
        reportHeader.ActionPerformed += HeaderActions_ActionPerformed;
    }


    /// <summary>
    /// On PreRender override.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        reportHeader.ReportName = displayReport.ReportName;
        reportHeader.ReportParameters = displayReport.ReportParameters;

        // Disable print option if form parameters aren't valid
        reportHeader.PrintEnabled = displayReport.ParametersForm.ValidateData();
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// VerifyRenderingInServerForm.
    /// </summary>
    public override void VerifyRenderingInServerForm(Control control)
    {
        if (!isSaved)
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
    /// Save click handler.
    /// </summary>
    protected void Save()
    {
        // Check 'SaveReports' permission
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.reporting", "SaveReports"))
        {
            RedirectToAccessDenied("cms.reporting", "SaveReports");
        }

        if (!displayReport.ParametersForm.ValidateData())
        {
            return;
        }

        isSaved = true;
        int savedReportId = displayReport.SaveReport();

        if (savedReportId != 0)
        {
            URLHelper.Redirect("SavedReports/SavedReport_View.aspx?reportId=" + savedReportId.ToString() + "&view=1");
        }
    }

    #endregion
}