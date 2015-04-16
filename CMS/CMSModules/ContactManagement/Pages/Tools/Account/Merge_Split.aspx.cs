using System;

using CMS.Core;
using CMS.OnlineMarketing;
using CMS.UIControls;

// Edited object
[EditedObject(AccountInfo.OBJECT_TYPE, "objectid")]
public partial class CMSModules_ContactManagement_Pages_Tools_Account_Merge_Split : CMSContactManagementAccountsPage
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
        CheckUIElementAccessHierarchical(ModuleName.ONLINEMARKETING, "account.mergesplit");

        mergeSplit.ShowChildrenOption = true;

        // Check read permission for accounts
        int siteID = AccountHelper.ObjectSiteID(EditedObject);
        CheckReadPermission(siteID);
    }
}