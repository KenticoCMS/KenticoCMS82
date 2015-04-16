using System;

using CMS.OnlineMarketing;
using CMS.WebAnalytics;

public partial class CMSModules_ContactManagement_Controls_UI_ActivityDetails_ExternalSearch : ActivityDetail
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
            case PredefinedActivityType.EXTERNAL_SEARCH:
            case PredefinedActivityType.INTERNAL_SEARCH:
                break;
            default:
                return false;
        }

        // Load additional info
        SearchInfo si = SearchInfoProvider.GetSearchInfoByActivityID(ai.ActivityID);
        if (si == null)
        {
            return false;
        }

        ucDetails.AddRow("om.activitydetails.keywords", si.SearchKeywords);
        if (ai.ActivityType == PredefinedActivityType.EXTERNAL_SEARCH)
        {
            ucDetails.AddRow("om.activitydetails.provider", si.SearchProvider);
        }

        return ucDetails.IsDataLoaded;
    }

    #endregion
}