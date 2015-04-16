using System;

using CMS.Base;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.LicenseProvider;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.WebAnalytics;

[Security(Resource = "CMS.WebAnalytics", Permission = "Read")]
[UIElement("CMS.WebAnalytics", "CMSWebAnalytics")]
public partial class CMSModules_Reporting_WebAnalytics_Analytics_ManageData : CMSToolsModalPage
{
    #region "Variables"

    private const string MULTILINGUAL_SUFFIX = ".multilingual";
    private string statCodeName = String.Empty;

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Check license
        if (DataHelper.GetNotEmpty(RequestContext.CurrentDomain, "") != "")
        {
            LicenseHelper.CheckFeatureAndRedirect(RequestContext.CurrentDomain, FeatureEnum.WebAnalytics);
        }

        // If deletion is in progress
        if (StatisticsInfoProvider.DataDeleterIsRunning)
        {
            timeRefresh.Enabled = true;
            ViewState["DeleterStarted"] = true;
            EnableControls(false);
            ReloadInfoPanel();
        }
        // If deletion has just end - add close script
        else if (ValidationHelper.GetBoolean(ViewState["DeleterStarted"], false))
        {
            ScriptHelper.RegisterStartupScript(this, typeof(string), "CloseScript", ScriptHelper.GetScript("wopener.RefreshPage(); CloseDialog();"));
        }

        if (!RequestHelper.IsPostBack())
        {
            usCampaigns.Value = "";
            ucConversions.Value = "";
        }

        string title = GetString("AnayticsManageData.ManageData");
        Page.Title = title;
        PageTitle.TitleText = title;
        // Confirmation message for deleting
        string deleteFromToMessage = ScriptHelper.GetString(GetString("webanal.deletefromtomsg"));
        deleteFromToMessage = deleteFromToMessage.Replace("##FROM##", "' + elemFromStr + '");
        deleteFromToMessage = deleteFromToMessage.Replace("##TO##", "' + elemToStr + '");

        string script =
            " var elemTo = document.getElementById('" + pickerTo.ClientID + "_txtDateTime'); " +
            " var elemFrom = document.getElementById('" + pickerFrom.ClientID + "_txtDateTime'); " +
            " var elemToStr = " + ScriptHelper.GetString(GetString("webanal.now")) + "; " +
            " var elemFromStr = " + ScriptHelper.GetString(GetString("webanal.beginning")) + "; " +
            " var deleteAll = 1; " +
            " if (elemTo.value != '') { deleteAll = 0; elemToStr = elemTo.value; }; " +
            " if (elemFrom.value != '') { deleteAll = 0; elemFromStr = elemFrom.value; }; " +
            " if (deleteAll == 1) { return confirm(" + ScriptHelper.GetString(GetString("webanal.deleteall")) + "); } " +
            " else { return confirm(" + deleteFromToMessage + "); }; ";
        btnDelete.OnClientClick = script + ";  return false;";

        statCodeName = QueryHelper.GetString("statCodeName", String.Empty);

        switch (statCodeName)
        {
            case "campaigns":
                pnlCampaigns.Visible = true;
                break;

            case "conversion":
                pnlConversions.Visible = true;
                break;
        }
    }


    /// <summary>
    /// Enable/disable controls
    /// </summary>
    /// <param name="enabled">If true, controls are enabled</param>
    private void EnableControls(bool enabled)
    {
        usCampaigns.UniSelector.Enabled = enabled;
        pickerFrom.Enabled = enabled;
        pickerTo.Enabled = enabled;
    }


    /// <summary>
    /// Displays generator info panel
    /// </summary>
    private void ReloadInfoPanel()
    {
        if (StatisticsInfoProvider.DataDeleterIsRunning)
        {
            pnlProgress.Visible = true;
            ltrProgress.Text = ScriptHelper.GetLoaderInlineHtml(Page, GetString("analyt.settings.deleterinprogress"));
        }
    }


    public void btnDelete_Click(object sender, EventArgs e)
    {
        // Check 'ManageData' permission
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.WebAnalytics", "ManageData"))
        {
            RedirectToAccessDenied("CMS.WebAnalytics", "ManageData");
        }

        if (statCodeName == String.Empty)
        {
            return;
        }

        DateTime fromDate = pickerFrom.SelectedDateTime;
        DateTime toDate = pickerTo.SelectedDateTime;

        if (!pickerFrom.IsValidRange() || !pickerTo.IsValidRange())
        {
            ShowError(GetString("general.errorinvaliddatetimerange"));
            return;
        }

        if ((fromDate > toDate) && (toDate != DateTimeHelper.ZERO_TIME))
        {
            ShowError(GetString("analt.invalidinterval"));
            return;
        }

        String where = String.Empty;

        // Manage campaigns
        if (statCodeName == "campaigns")
        {
            string campaign = ValidationHelper.GetString(usCampaigns.Value, String.Empty);
            if (campaign == String.Empty)
            {
                ShowError(GetString("campaigns.pleaseselect"));
                return;
            }

            if (campaign == usCampaigns.AllRecordValue)
            {
                where = "(StatisticsCode='campaign' OR StatisticsCode LIKE 'campconversion;%')";
            }
            else
            {
                where = " ((StatisticsCode='campaign' AND StatisticsObjectName ='" + SqlHelper.EscapeQuotes(campaign) + "') OR StatisticsCode LIKE 'campconversion;" + SqlHelper.EscapeLikeText(SqlHelper.EscapeQuotes(campaign)) + "')";
            }
        }

        if ((statCodeName == "conversion") || statCodeName.StartsWithCSafe("singleconversion", true))
        {
            String defaultWhere = "(StatisticsCode='conversion' OR StatisticsCode LIKE 'campconversion;%' OR StatisticsCode LIKE 'abconversion;%' OR StatisticsCode LIKE 'mvtconversion;%')";
            if (!statCodeName.StartsWithCSafe("singleconversion", true))
            {
                string conversion = ValidationHelper.GetString(ucConversions.Value, String.Empty);
                if (conversion == String.Empty)
                {
                    ShowError(GetString("conversions.pleaseselect"));
                    return;
                }

                if (conversion == usCampaigns.AllRecordValue)
                {
                    where = defaultWhere;
                }
                else
                {
                    String saveConv = SqlHelper.EscapeQuotes(conversion);
                    where = String.Format("((StatisticsObjectName = '{0}') AND {1})", saveConv, defaultWhere);
                }
            }
            else
            {
                string[] arr = statCodeName.Split(';');
                if (arr.Length == 2)
                {
                    String saveConv = SqlHelper.EscapeQuotes(arr[1]);
                    where = String.Format("((StatisticsObjectName = '{0}') AND {1})", saveConv, defaultWhere);
                }
            }
        }

        // Delete one campaign (set from url)
        if (statCodeName.StartsWithCSafe("singlecampaign", true))
        {
            string[] arr = statCodeName.Split(';');
            if (arr.Length == 2)
            {
                String campaign = arr[1];
                where = "(StatisticsCode='campaign' AND StatisticsObjectName ='" + SqlHelper.EscapeQuotes(campaign) + "') OR StatisticsCode LIKE 'campconversion;" + SqlHelper.EscapeLikeText(SqlHelper.EscapeQuotes(campaign)) + "'";
            }
        }

        // Ingore multilingual suffix (multilingual stats use the same data as "base" stats)
        if (statCodeName.ToLowerCSafe().EndsWithCSafe(MULTILINGUAL_SUFFIX))
        {
            statCodeName = statCodeName.Remove(statCodeName.Length - MULTILINGUAL_SUFFIX.Length);
        }

        // Add where condition based on stat code name
        if (where == String.Empty)
        {
            where = "StatisticsCode LIKE '" + SqlHelper.EscapeLikeText(SqlHelper.EscapeQuotes(statCodeName)) + "'";
        }

        // In case of any error - (this page don't allow deleting all statistics)
        if (where == String.Empty)
        {
            return;
        }

        // Stats for visitors needs special manipulation (it consist of two types
        // of statistics with different code names - new visitor and returning visitor)
        if (statCodeName.ToLowerCSafe() != HitLogProvider.VISITORS_FIRST)
        {
            StatisticsInfoProvider.RemoveAnalyticsDataAsync(fromDate, toDate, SiteContext.CurrentSiteID, where);
        }
        else
        {
            where = "(StatisticsCode = '" + HitLogProvider.VISITORS_FIRST + "' OR StatisticsCode ='" + HitLogProvider.VISITORS_RETURNING + "')";
            StatisticsInfoProvider.RemoveAnalyticsDataAsync(fromDate, toDate, SiteContext.CurrentSiteID, where);
        }

        // Manage async delete info
        timeRefresh.Enabled = true;
        EnableControls(false);
        ReloadInfoPanel();
        ViewState.Add("DeleterStarted", 1);
    }

    #endregion
}