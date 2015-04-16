using System;

using CMS.Core;
using CMS.OnlineMarketing;
using CMS.UIControls;

// Edited object
[EditedObject(ContactGroupInfo.OBJECT_TYPE, "groupId")]
[Security(Resource = ModuleName.ONLINEMARKETING, UIElements = "EditContactGroup;ContactGroupAccounts")]
public partial class CMSModules_ContactManagement_Pages_Tools_ContactGroup_Tab_Accounts : CMSContactManagementContactGroupsPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentMaster.PanelContent.CssClass = string.Empty;
    }
}