using System;

using CMS.OnlineMarketing;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.WebAnalytics;
using CMS.Protection;

public partial class CMSModules_ContactManagement_Controls_UI_ActivityDetails_AbuseReport : ActivityDetail
{
    #region "Methods"

    public override bool LoadData(ActivityInfo ai)
    {
        if ((ai == null) || ai.ActivityType != PredefinedActivityType.ABUSE_REPORT)
        {
            return false;
        }

        int nodeId = ai.ActivityNodeID;
        lblDocIDVal.Text = GetLinkForDocument(nodeId, ai.ActivityCulture);

        AbuseReportInfo ari = AbuseReportInfoProvider.GetAbuseReportInfo(ai.ActivityItemID);
        if (ari != null)
        {
            txtComment.Text = ari.ReportComment;
        }

        return true;
    }

    #endregion
}