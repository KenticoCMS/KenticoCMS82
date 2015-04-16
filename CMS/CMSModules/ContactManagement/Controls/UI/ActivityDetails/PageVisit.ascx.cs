using System;

using CMS.OnlineMarketing;
using CMS.WebAnalytics;

public partial class CMSModules_ContactManagement_Controls_UI_ActivityDetails_PageVisit : ActivityDetail
{
    #region "Methods"

    /// <summary>
    /// Loads activity info data.
    /// </summary>
    /// <param name="ai">ActivityInfo object</param>
    public override bool LoadData(ActivityInfo ai)
    {
        if (ai == null)
        {
            return false;
        }

        string mvtComb = null;
        string queryString = null;
        string abVariant = null;

        switch (ai.ActivityType)
        {
            case PredefinedActivityType.LANDING_PAGE:
            case PredefinedActivityType.PAGE_VISIT:
                PageVisitInfo pvi = PageVisitInfoProvider.GetPageVisitInfoByActivityID(ai.ActivityID);
                if (pvi == null)
                {
                    return false;
                }

                mvtComb = pvi.PageVisitMVTCombinationName;
                queryString = pvi.PageVisitDetail;
                abVariant = pvi.PageVisitABVariantName;
                break;
        }

        // Loads data to grid
        ucDetails.AddRow("om.activitydetails.query", queryString);
        ucDetails.AddRow("om.activitydetails.abvariant", abVariant);
        ucDetails.AddRow("om.activitydetails.mvtcomb", mvtComb);
        return ucDetails.IsDataLoaded;
    }

    #endregion
}