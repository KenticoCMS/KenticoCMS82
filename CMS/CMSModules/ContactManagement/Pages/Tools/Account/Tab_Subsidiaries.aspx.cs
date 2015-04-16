using System;

using CMS.Core;
using CMS.OnlineMarketing;
using CMS.UIControls;

[EditedObject("om.account", "objectid")]
public partial class CMSModules_ContactManagement_Pages_Tools_Account_Tab_Subsidiaries : CMSContactManagementAccountsPage
{
    /// <summary>
    /// Hides close button
    /// </summary>
    protected override void OnPreInit(EventArgs e)
    {
        RequiresDialog = false;
        base.OnPreInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Check UI elements
        CheckUIElementAccessHierarchical(ModuleName.ONLINEMARKETING, "account.subsidiaries");

        CurrentMaster.PanelContent.CssClass = "";
        // Check read permission for account
        int siteID = AccountHelper.ObjectSiteID(EditedObject);
        CheckReadPermission(siteID);
    }
}