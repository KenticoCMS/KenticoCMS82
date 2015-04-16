using System;

using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.Base;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.DataEngine;
using CMS.PortalEngine;

public partial class CMSModules_Objects_Controls_Versioning_VersionListMenu : CMSContextMenuControl
{
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        string menuId = ContextMenu.MenuID;
        string parentElemId = ContextMenu.ParentElementClientID;

        var info = UIContext.EditedObject as BaseInfo;

        if ((info != null) && info.CheckPermissions(PermissionsEnum.Modify, SiteContext.CurrentSiteName, MembershipContext.AuthenticatedUser))
        {
            pnlRestoreChilds.Attributes.Add("onclick", "if(confirm(" + ScriptHelper.GetLocalizedString("objectversioning.versionlist.confirmfullrollback") + ")) { ContextVersionAction_" + parentElemId + "('fullrollback', GetContextMenuParameter('" + menuId + "'));} return false;");
        }
        else
        {
            pnlRestoreChilds.AddCssClass("ItemDisabled");
        }
    }
}
