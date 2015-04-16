using CMS.Helpers;
using CMS.PortalEngine;
using CMS.UIControls;

public partial class CMSModules_Content_Controls_Dialogs_Properties_DocCopyMoveProperites : CMSDeskPage
{
    protected override void OnLoad(System.EventArgs e)
    {
        base.OnLoad(e);
        ScriptHelper.RegisterWOpenerScript(this);
    }
}