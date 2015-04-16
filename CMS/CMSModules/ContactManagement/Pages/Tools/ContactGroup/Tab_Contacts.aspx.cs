using System;

using CMS.Core;
using CMS.OnlineMarketing;
using CMS.UIControls;

[EditedObject(ContactGroupInfo.OBJECT_TYPE, "groupId")]
[Security(Resource = ModuleName.ONLINEMARKETING, UIElements = "EditContactGroup;ContactGroupContacts")]
public partial class CMSModules_ContactManagement_Pages_Tools_ContactGroup_Tab_Contacts : CMSContactManagementContactGroupsPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentMaster.PanelContent.CssClass = string.Empty;
    }
}