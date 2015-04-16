using System;

using CMS.OnlineMarketing;
using CMS.WebAnalytics;


public partial class CMSModules_ContactManagement_Controls_UI_ActivityDetails_UserContribution : ActivityDetail
{
    #region "Methods"

    public override bool LoadData(ActivityInfo ai)
    {
        if (ai == null)
        {
            return false;
        }

        switch (ai.ActivityType)
        {
            case PredefinedActivityType.USER_CONTRIB_INSERT:
            case PredefinedActivityType.USER_CONTRIB_UPDATE:
            case PredefinedActivityType.USER_CONTRIB_DELETE:
                break;
            default:
                return false;
        }

        // Load data
        int nodeId = ai.ActivityNodeID;
        ucDetails.AddRow("om.activitydetails.documenturl", GetLinkForDocument(nodeId, ai.ActivityCulture), false);
        return ucDetails.IsDataLoaded;
    }

    #endregion
}