using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Controls.Configuration;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_WebAnalytics_Pages_Tools_Campaign_CampaignHeader : CMSCampaignPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Get query parameters
        string tabName = GetString(QueryHelper.GetText("displayTab", String.Empty));
        string statName = QueryHelper.GetString("statCodeName", String.Empty);
        string titleText = GetString("analytics_codename.campaign");

        // Create title based on stat name, use default for 'campaigns'
        if (statName != "campaigns")
        {
            titleText += " - " + GetString(statName);
        }

        bool displayReport = QueryHelper.GetBoolean("displayReport", false);

        CurrentMaster.Tabs.AddTab(new UITabItem()
        {
            Text = (tabName != String.Empty) ? tabName : GetString("general.report"),
            RedirectUrl = ResolveUrl("CampaignReport.aspx" + RequestContext.CurrentQueryString + "&displayTitle=0")
        });

        CurrentMaster.Tabs.AddTab(new UITabItem()
        {
            Text = GetString("analytics_codename.campaign"),
            RedirectUrl = "list.aspx?displayReport=" + displayReport
        });
        CurrentMaster.Tabs.UrlTarget = "content";

        PageTitle title = PageTitle;
        title.TitleText = titleText;

        // Register script for unimenu button selection
        AddMenuButtonSelectScript(this, "Campaigns", null, "menu");
    }
}