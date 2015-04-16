using System;

using CMS.Helpers;
using CMS.OnlineMarketing;
using CMS.Base;
using CMS.WebAnalytics;

public partial class CMSModules_ContactManagement_Controls_UI_ActivityDetails_LeaveGroup : ActivityDetail
{
    #region "Methods"

    public override bool LoadData(ActivityInfo ai)
    {
        return uc.LoadData(ai);
    }

    #endregion
}