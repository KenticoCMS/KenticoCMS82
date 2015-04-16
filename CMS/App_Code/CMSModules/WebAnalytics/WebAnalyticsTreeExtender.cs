using System;
using System.Data;
using System.Text;
using System.Web.UI.WebControls;

using CMS;
using CMS.Base;
using CMS.DataEngine;
using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.Modules;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.WebAnalytics;


[assembly: RegisterCustomClass("WebAnalyticsTreeExtender", typeof(WebAnalyticsTreeExtender))]

/// <summary>
/// Extender for Web Analytics Tree
/// </summary>
public class WebAnalyticsTreeExtender : ControlExtender<UIMenu>
{
    #region "Variables"

    /// <summary>
    /// List of special conversions not displayed either in custom node list or in ui list.
    /// </summary>
    private const string additionalConversions = "'visitfirst';'visitreturn';'referringsite_direct';'referringsite_search';'referringsite_referring';'referringsite_local';'avgtimeonpage'";


    /// <summary>
    /// Where condition used for select custom conversions.
    /// </summary>
    private string customWhereCondition = String.Empty;

    #endregion


    #region "Page Events & Event Handlers"

    public override void OnInit()
    {
        Control.OnNodeCreated += Control_OnNodeCreated;
        
        // Get NOT custom conversions from UI elements 
        UIElementInfo root = UIElementInfoProvider.GetRootUIElementInfo("CMS.WebAnalytics");
        if (root != null)
        {
            // Get all UI elements to filter custom reports
            DataSet data = UIElementInfoProvider.GetUIElements("ElementIDPath LIKE '" + SqlHelper.EscapeLikeQueryPatterns(root.ElementIDPath) + "/%'", String.Empty, 0, "ElementName,ElementTargetUrl");
            if (!DataHelper.DataSourceIsEmpty(data) && (data.Tables.Count > 0))
            {
                // Condition for custom reports
                customWhereCondition = "StatisticsCode NOT IN (";
                foreach (DataRow dr in data.Tables[0].Rows)
                {
                    string codeName = ValidationHelper.GetString(dr["ElementName"], String.Empty);
                    customWhereCondition += "N'" + SqlHelper.EscapeQuotes(codeName) + "',";
                }

                // Add special cases - don't want to show them in UI or Custom report section
                customWhereCondition += additionalConversions.Replace(';', ',');

                customWhereCondition = customWhereCondition.TrimEnd(new[] { ',' });
                customWhereCondition += ")";

                // Filter AB Testing
                customWhereCondition += " AND (StatisticsCode NOT LIKE 'abconversion;%') AND (StatisticsCode NOT LIKE 'mvtconversion;%') AND (StatisticsCode NOT LIKE 'campconversion;%') " +
                    "AND (StatisticsCode NOT LIKE 'absessionconversionfirst;%') AND (StatisticsCode NOT LIKE 'absessionconversionrecurring;%') " +
                    "AND (StatisticsCode NOT LIKE 'abvisitfirst;%')  AND (StatisticsCode NOT LIKE 'abvisitreturn;%') AND (StatisticsCode <> N'campaign') and (StatisticsCode <> N'conversion') ";
            }
        }
    }


    protected TreeNode Control_OnNodeCreated(UIElementInfo uiElement, TreeNode defaultNode)
    {
        string elementName = uiElement.ElementName.ToLowerCSafe();

        // Add all custom reports
        if (elementName == "custom")
        {
            customWhereCondition = SqlHelper.AddWhereCondition(customWhereCondition, " StatisticsSiteID = " + SiteContext.CurrentSiteID);
            DataSet ds = StatisticsInfoProvider.GetCodeNames(customWhereCondition, "StatisticsCode ASC", 0);

            // If no custom reports found - hide Custom Reports node
            if (DataHelper.DataSourceIsEmpty(ds))
            {
                return null;
            }

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                TreeNode childNode = new TreeNode();

                string codeName = ValidationHelper.GetString(dr["StatisticsCode"], String.Empty).ToLowerCSafe();
                string name = ResHelper.GetString("analytics_codename." + codeName);
                string dataCodeName = GetDataCodeName(codeName);
                string reportCodeName = GetReportCodeNames(codeName);

                childNode.Text = "<span id=\"node_" + codeName + "\" class=\"ContentTreeItem\" name=\"treeNode\"><span class=\"Name\">" + name + "</span></span>";
                childNode.NavigateUrl = "~/CMSModules/WebAnalytics/Tools/Analytics_Report.aspx?statCodeName=" + codeName + "&dataCodeName=" + dataCodeName + "&reportCodeName=" + reportCodeName + "&isCustom=1";
                childNode.Target = Control.TargetFrame;

                defaultNode.ChildNodes.Add(childNode);
            }
        }

        return defaultNode;
    }

    #endregion


    #region "Methods"
    
    /// <summary>
    /// Returns generic report code names (based on analytics code name).
    /// </summary>
    /// <param name="statCodeName">Analytics code name (pageviews, pageviews.multilingual...)</param>
    private static string GetReportCodeNames(string statCodeName)
    {
        string result = "";
        result += statCodeName + ".yearreport";
        result += ";" + statCodeName + ".monthreport";
        result += ";" + statCodeName + ".weekreport";
        result += ";" + statCodeName + ".dayreport";
        result += ";" + statCodeName + ".hourreport";
        return result;
    }


    /// <summary>
    /// Returns data code name from analytics code name.
    /// </summary>
    /// <param name="statCodeName">Analytics code name (pageviews, pageviews.multilingual...)</param>
    private static string GetDataCodeName(string statCodeName)
    {
        int pos = statCodeName.IndexOfCSafe('.');
        if (pos > 0)
        {
            return statCodeName.Substring(0, pos);
        }

        return statCodeName;
    }

    #endregion
}
