using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Core;
using CMS.ExtendedControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.Helpers;
using CMS.OnlineMarketing;
using CMS.Reporting;
using CMS.SiteProvider;
using CMS.UIControls;

[Security(Resource = ModuleName.ONLINEMARKETING, UIElements = "EditProcess;EditProcessReport")]
public partial class CMSModules_ContactManagement_Pages_Tools_Automation_Process_Tab_Report : CMSAutomationPage
{
    #region "Variables"

    private bool mSaving;

    #endregion


    #region "Event handlers"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        siteSelector.SiteID = SiteID;
        siteOrGlobalSelector.SiteID = SiteID;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        int siteId = 0;
        int processId = QueryHelper.GetInteger("processid", 0);

        // Handle site selectors visibility
        if (IsSiteManager)
        {
            CurrentMaster.DisplaySiteSelectorPanel = true;
            siteOrGlobalSelector.Visible = false;
            siteId = siteSelector.SiteID;
        }
        else
        {
            // Init site filter when user is authorized for global and site contacts
            if (AuthorizedForGlobalContacts && AuthorizedForSiteContacts)
            {
                CurrentMaster.DisplaySiteSelectorPanel = true;
                siteSelector.Visible = false;
                siteId = ValidationHelper.GetInteger(siteOrGlobalSelector.Value, SiteContext.CurrentSiteID);
            }
            else if (AuthorizedForSiteContacts)
            {
                // User is authorized only for site contacts so set current site id
                siteId = SiteID;
            }
            else if (AuthorizedForGlobalContacts)
            {
                // User can read only global contacts
                siteId = UniSelector.US_GLOBAL_RECORD;
            }
            else
            {
                // User has no permissions
                RedirectToAccessDenied(ModuleName.CONTACTMANAGEMENT, "ReadContacts");
            }
        }

        // Set report parameters
        ReportInfo report = ReportInfoProvider.GetReportInfo("Number_of_contacts_in_steps");
        if (report != null)
        {
            ucReport.ReportName = report.ReportName;
            reportHeader.ReportName = report.ReportName;
            reportHeader.DisplayManageData = false;

            ucReport.DisplayFilter = false;
            ucReport.LoadFormParameters = false;

            string parameterString = String.Format("AutomationProcessID;{0};SiteID;{1}", processId, siteId);
            DataRow parameters = ReportHelper.GetReportParameters(report, parameterString, null, CultureHelper.EnglishCulture, CultureHelper.PreferredUICultureInfo);
            ucReport.ReportParameters = parameters;
            reportHeader.ReportParameters = parameters;

            reportHeader.ActionPerformed += reportHeader_ActionPerformed;
        }

        // Create refresh action
        rightHeaderActions.AddAction(new HeaderAction()
        {
            Text = GetString("general.refresh"),
            RedirectUrl = AddSiteQuery("Tab_Report.aspx?processid=" + processId, siteId)
        });
    }


    private void reportHeader_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case ComponentEvents.SAVE:
                // Check 'SaveReports' permission
                if (!CurrentUser.IsAuthorizedPerResource("cms.reporting", "SaveReports"))
                {
                    RedirectToAccessDenied("cms.reporting", "SaveReports");
                }

                mSaving = true;

                if (ucReport.SaveReport() > 0)
                {
                    ShowConfirmation(String.Format(GetString("reporting.reportsavedto"), ucReport.ReportDisplayName + " - " + DateTime.Now));
                }
                else
                {
                    ShowError(GetString("reporting.savingreportfailed"));
                }

                mSaving = false;
                break;
        }
    }


    /// <summary>
    /// Verify rendering in server form only when not saving report.
    /// </summary>
    /// <param name="control">Control</param>
    public override void VerifyRenderingInServerForm(Control control)
    {
        if (!mSaving)
        {
            base.VerifyRenderingInServerForm(control);
        }
    }

    #endregion
}