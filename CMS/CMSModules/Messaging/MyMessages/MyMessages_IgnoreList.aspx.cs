using System;

using CMS.ExtendedControls;
using CMS.UIControls;

[UIElement("CMS.Messaging", "MyMessages.IgnoreList")]
public partial class CMSModules_Messaging_MyMessages_MyMessages_IgnoreList : CMSContentManagementPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Header actions, etc. are done manually in contained control, so PageContent class that adds a padding to the whole page
        // is not wanted
        CurrentMaster.PanelContent.RemoveCssClass("PageContent");
    }


    protected void CheckPermissions(string permissionType, CMSAdminControl sender)
    {
        // Do not check permissions since user can always manage her messages
    }
}