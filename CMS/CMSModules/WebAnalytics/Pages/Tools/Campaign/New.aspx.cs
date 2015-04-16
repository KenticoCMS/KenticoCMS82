using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.FormControls;
using CMS.Helpers;
using CMS.UIControls;
using CMS.WebAnalytics;

[Help("campaign_general", "helpTopic")]

[Breadcrumbs()]
[Breadcrumb(0, ResourceString = "campaign.campaign.list", TargetUrl = "~/CMSModules/WebAnalytics/Pages/Tools/Campaign/List.aspx?displayreport={?displayreport|false?}")]
[Breadcrumb(1, ResourceString = "campaign.campaign.new")]
public partial class CMSModules_WebAnalytics_Pages_Tools_Campaign_New : CMSCampaignPage
{
}