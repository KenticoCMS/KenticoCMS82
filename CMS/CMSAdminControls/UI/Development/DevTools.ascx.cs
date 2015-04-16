using System;

using CMS.Helpers;
using CMS.Membership;
using CMS.PortalEngine;
using CMS.UIControls;
using CMS.Base;

public partial class CMSAdminControls_UI_Development_DevTools : CMSUserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        CMSPage page = Parent.Page as CMSPage;

        var developmentMode = (page != null) ? page.CurrentMaster.DevelopmentMode : SystemContext.DevelopmentMode;
        if (developmentMode && MembershipContext.AuthenticatedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin))
        {

            // UI Element
            var elem = UIContext.UIElement;
            if (elem != null)
            {
                ltlActions.Text += UIContextHelper.GetResourceUIElementsLink(elem.ElementResourceID, elem.ElementID);
            }

            // Debug
            string urlDebug = URLHelper.GetAbsoluteUrl("~/CMSModules/System/Debug/System_ViewRequest.aspx?guid=" + DebugContext.CurrentRequestLogs.RequestGUID);
            string textDebug = GetString("general.debug");
            ltlDebug.Text = String.Format("<a target=\"_blank\" href=\"{0}\">{1}</a>", urlDebug, UIHelper.GetAccessibleIconTag("icon-bug", textDebug, FontIconSizeEnum.Standard));

            // Localize
            string textLocalize = GetString("localizable.localize");
            ltlLocalize.Text = UIHelper.GetAccessibleIconTag("icon-earth", textLocalize, FontIconSizeEnum.Standard);
            btnLocalize.Image.Visible = false;

            // Do not move to the markup - could cause life cycle issues
            btnLocalize.HorizontalPosition = CMS.ExtendedControls.HorizontalPositionEnum.Right;
            btnLocalize.OffsetY = -20;
            btnLocalize.OffsetX = 1;
            btnLocalize.MouseButton = CMS.ExtendedControls.MouseButtonEnum.Both;
            btnLocalize.ContextMenuCssClass = "dev-tools-context-menu";
            btnLocalize.MenuControlPath = "~/CMSAdminControls/UI/Development/Localize.ascx";
        }
        else
        {
            Visible = false;
        }
    }
}
