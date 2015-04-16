using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Core;
using CMS.Helpers;
using CMS.Membership;
using CMS.Modules;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.Base;
using CMS.BannerManagement;
using CMS.DataEngine;

public partial class CMSModules_Reporting_Tools_BannerManagement_Reports : CMSBannerManagementEditPage
{
    #region "Private fields"

    private bool mParamsLoaded;
    private bool mIsSaved;

    #endregion


    #region "Properties"

    public override void VerifyRenderingInServerForm(Control control)
    {
        if (!mIsSaved)
        {
            base.VerifyRenderingInServerForm(control);
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Init(object sender, EventArgs e)
    {
        MessagesPlaceHolder = plcMess;

        string objectType = String.Empty;
        string elementName = String.Empty;

        switch (QueryHelper.GetString("parameterName", String.Empty).ToLowerCSafe())
        {
            // It is banner
            case "bannerid":
                objectType = BannerInfo.OBJECT_TYPE;
                elementName = "Report_1";
                break;

            // It is banner category
            case "bannercategoryid":
                objectType = BannerCategoryInfo.OBJECT_TYPE;
                elementName = "Report";
                break;

            default:
                RedirectToInformation(GetString("bannermanagement.error.internal"));
                break;
        }


        // Check UI personalization
        var uiElement = new UIElementAttribute(ResourceName, elementName);
        uiElement.Check(this);
        
        // Check Reporting permissions
        CheckReportingAvailability();

        // Get the ID
        int id = QueryHelper.GetInteger("parameterValue", 0);

        SetEditedObject(BaseAbstractInfoProvider.GetInfoById(objectType, id), String.Empty);
    }


    /// <summary>
    /// Checks permissions for Reporting module.
    /// </summary>
    private void CheckReportingAvailability()
    {
        if (!ModuleEntryManager.IsModuleLoaded(ModuleName.REPORTING))
        {
            RedirectToUINotAvailable();
        }

        if (!ResourceSiteInfoProvider.IsResourceOnSite("CMS.Reporting", SiteContext.CurrentSiteName))
        {
            RedirectToResourceNotAvailableOnSite("CMS.Reporting");
        }

        // Check additional read permission to Reporting module
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Reporting", "Read"))
        {
            RedirectToAccessDenied("CMS.Reporting", "Read");
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Set disabled module info
        ucDisabledModule.ParentPanel = pnlDisabled;

        reportHeader.ActionPerformed += reportHeader_ActionPerformed;
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        LoadAttributes();

        ucDisplayReport.ReloadData(false);
    }

    #endregion


    #region "Private methods"

    private void reportHeader_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case ComponentEvents.SAVE:
                LoadAttributes();

                mIsSaved = true;

                // Save report
                if (ucDisplayReport.SaveReport() > 0)
                {
                    ShowConfirmation(String.Format(GetString("Ecommerce_Report.ReportSavedTo"), ucDisplayReport.ReportName + " - " + DateTime.Now.ToString()));
                }

                mIsSaved = false;
                break;
        }
    }


    private void LoadAttributes()
    {
        if (mParamsLoaded)
        {
            return;
        }

        if (URLHelper.IsPostback() && !IsValidInterval())
        {
            ShowError(GetString("analt.invalidinterval"));
            return;
        }
        
        string reportsCodeName = QueryHelper.GetString("reportCodeName", String.Empty);


        if (!reportsCodeName.Contains(";"))
        {
            ucDisplayReport.ReportName = reportsCodeName;
            ucGraphTypePeriod.GraphTypeVisible = false;
        }
        else
        {
            ucDisplayReport.ReportName = ucGraphTypePeriod.GetReportName(reportsCodeName);
        }

        ucGraphTypePeriod.ProcessChartSelectors(false);

        // Prepare report parameters
        DataTable dtp = new DataTable();

        dtp.Columns.Add("FromDate", typeof(DateTime));
        dtp.Columns.Add("ToDate", typeof(DateTime));
        dtp.Columns.Add(QueryHelper.GetString("parameterName", String.Empty), typeof(int));

        object[] parameters = new object[3];

        parameters[0] = ucGraphTypePeriod.From;
        parameters[1] = ucGraphTypePeriod.To;
        parameters[2] = QueryHelper.GetInteger("parameterValue", 0);

        dtp.Rows.Add(parameters);
        dtp.AcceptChanges();

        ucDisplayReport.LoadFormParameters = false;
        ucDisplayReport.DisplayFilter = false;
        ucDisplayReport.ReportParameters = dtp.Rows[0];
        ucDisplayReport.UseExternalReload = true;
        ucDisplayReport.UseProgressIndicator = true;

        reportHeader.ReportName = ucDisplayReport.ReportName;
        reportHeader.ReportParameters = ucDisplayReport.ReportParameters;
        reportHeader.SelectedInterval = ucGraphTypePeriod.SelectedInterval;
        reportHeader.DisplayManageData = false;

        mParamsLoaded = true;
    }


    /// <summary>
    /// Returns true if selected interval is valid.
    /// </summary>
    private bool IsValidInterval()
    {
        var from = ucGraphTypePeriod.From;
        var to = ucGraphTypePeriod.To;

        if ((from == DateTimeHelper.ZERO_TIME) || (to == DateTimeHelper.ZERO_TIME))
        {
            return false;
        }

        return from <= to;
    }

    #endregion
}