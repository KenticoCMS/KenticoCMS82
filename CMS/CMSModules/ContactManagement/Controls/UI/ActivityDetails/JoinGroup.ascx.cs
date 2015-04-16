using CMS.Core;
using CMS.DataEngine;
using CMS.OnlineMarketing;
using CMS.Base;
using CMS.WebAnalytics;

public partial class CMSModules_ContactManagement_Controls_UI_ActivityDetails_JoinGroup : ActivityDetail
{
    #region "Methods"

    public override bool LoadData(ActivityInfo ai)
    {
        if ((ai == null) || !ModuleManager.IsModuleLoaded(ModuleName.COMMUNITY))
        {
            return false;
        }

        switch (ai.ActivityType)
        {
            case PredefinedActivityType.JOIN_GROUP:
            case PredefinedActivityType.LEAVE_GROUP:
                break;
            default:
                return false;
        }

        if (ai.ActivityItemID > 0)
        {
            BaseInfo binfo = ModuleCommands.CommunityGetGroupInfo(ai.ActivityItemID);
            if (binfo != null)
            {
                string groupDisplayName = binfo.GetStringValue("GroupDisplayName", GetString("general.na"));
                ucDetails.AddRow("om.activitydetails.groupname", groupDisplayName);
            }
        }

        return true;
    }

    #endregion
}