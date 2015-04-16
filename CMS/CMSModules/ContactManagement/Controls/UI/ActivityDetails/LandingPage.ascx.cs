using System;

using CMS.OnlineMarketing;

public partial class CMSModules_ContactManagement_Controls_UI_ActivityDetails_LandingPage : ActivityDetail
{
    #region "Methods"

    public override bool LoadData(ActivityInfo ai)
    {
        return uc.LoadData(ai);
    }

    #endregion
}