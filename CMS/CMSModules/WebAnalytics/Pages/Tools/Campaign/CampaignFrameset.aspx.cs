using System;

using CMS.Helpers;
using CMS.UIControls;

public partial class CMSModules_WebAnalytics_Pages_Tools_Campaign_CampaignFrameset : CMSCampaignPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string reportUrl = ResolveUrl("CampaignReport.aspx" + RequestContext.CurrentQueryString);
        frmContent.Attributes["src"] = URLHelper.AddParameterToUrl(reportUrl, "displayTitle", "0");
        frmHeader.Attributes["src"] = "CampaignHeader.aspx" + RequestContext.CurrentQueryString;
    }
}