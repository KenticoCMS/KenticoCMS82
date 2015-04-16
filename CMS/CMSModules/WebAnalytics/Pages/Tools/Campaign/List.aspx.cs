using System;

using CMS.Helpers;
using CMS.UIControls;


[Action(0, "campaign.campaign.new", "new.aspx?{??}")]
public partial class CMSModules_WebAnalytics_Pages_Tools_Campaign_List : CMSCampaignPage
{
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        string uiTrace = QueryHelper.GetString("ui", string.Empty);
        if (uiTrace == "omanalytics")
        {
            // Check UI elements
            if (!CurrentUser.IsAuthorizedPerUIElement("CMS.WebAnalytics", "Analytics.Campaigns.Campaigns"))
            {
                RedirectToUIElementAccessDenied("CMS.WebAnalytics", "Analytics.Campaigns.Campaigns");
            }

            if (!CurrentUser.IsAuthorizedPerUIElement("CMS.WebAnalytics", "Campaigns"))
            {
                RedirectToUIElementAccessDenied("CMS.WebAnalytics", "Campaigns");
            }
        }
    }
}