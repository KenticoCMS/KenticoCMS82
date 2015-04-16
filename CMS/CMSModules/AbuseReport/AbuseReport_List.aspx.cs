using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Core;
using CMS.Helpers;
using CMS.Membership;
using CMS.Base;
using CMS.UIControls;

[UIElement(ModuleName.ABUSEREPORT, "AbuseReport")]
public partial class CMSModules_AbuseReport_AbuseReport_List : CMSAbuseReportPage
{
    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Set the page title
        PageTitle.TitleText = GetString("abuse.reportabuse");
        ucAbuseReportList.OnCheckPermissions += new CMSAdminControl.CheckPermissionsEventHandler(ucAbuseReportList_OnCheckPermissions);
    }


    /// <summary>
    /// Check permission.
    /// </summary>
    /// <param name="permissionType">Permission type</param>
    /// <param name="sender">Sender</param>
    private void ucAbuseReportList_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.AbuseReport", permissionType))
        {
            sender.StopProcessing = true;
            RedirectToAccessDenied("CMS.AbuseReport", permissionType);
        }
    }

    #endregion
}