using System;

using CMS.Core;
using CMS.OnlineMarketing;
using CMS.UIControls;

[EditedObject("om.account", "objectid")]
public partial class CMSModules_ContactManagement_Pages_Tools_Account_Tab_Contacts : CMSContactManagementAccountsPage
{
    /// <summary>
    /// PreInit event handler
    /// </summary>
    protected override void OnPreInit(EventArgs e)
    {
        RequiresDialog = false;
        base.OnPreInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Check UI elements
        CheckUIElementAccessHierarchical(ModuleName.ONLINEMARKETING, "account.contacts");

        CurrentMaster.PanelContent.CssClass = "";

        int siteID = AccountHelper.ObjectSiteID(EditedObject);

        // Check read permission 
        if (!AccountHelper.AuthorizedReadAccount(siteID, false) && !ContactHelper.AuthorizedReadContact(siteID, false))
        {
            RedirectToAccessDenied(ModuleName.CONTACTMANAGEMENT, "ReadAccounts");
        }
    }
}