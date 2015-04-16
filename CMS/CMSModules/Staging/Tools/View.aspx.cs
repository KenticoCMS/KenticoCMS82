using System;

using CMS.Helpers;
using CMS.Membership;
using CMS.UIControls;

public partial class CMSModules_Staging_Tools_View : CMSStagingPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!QueryHelper.ValidateHash("hash"))
        {
            URLHelper.Redirect(ResolveUrl("~/CMSMessages/Error.aspx?title=" + GetString("dialogs.badhashtitle") + "&text=" + GetString("dialogs.badhashtext") + "&cancel=1"));
        }
        else
        {
            string element = QueryHelper.GetString("taskType", null);

            // Check permissions for CMS Desk -> Tools -> Staging
            var user = MembershipContext.AuthenticatedUser;
            if (!user.IsAuthorizedPerUIElement("cms.staging", element))
            {
                RedirectToUIElementAccessDenied("cms.staging", element);
            }

            // Check 'Manage tasks' permission
            if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.staging", "Manage" + element + "Tasks"))
            {
                RedirectToAccessDenied("cms.staging", "Manage" + element + "Tasks");
            }

            PageTitle.IsDialog = false;
            PageTitle.TitleText = GetString("Task.ViewHeader");
            int taskId = QueryHelper.GetInteger("taskid", 0);
            ucViewTask.TaskId = taskId;
        }
    }
}