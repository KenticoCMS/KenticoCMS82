using CMS.OnlineMarketing;

public partial class CMSModules_ContactManagement_Controls_UI_ActivityDetails_UserContributionDelete : ActivityDetail
{
    #region "Methods"

    public override bool LoadData(ActivityInfo ai)
    {
        return uc.LoadData(ai);
    }

    #endregion
}