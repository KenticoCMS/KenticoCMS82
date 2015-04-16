using System;

using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.UIControls;

public partial class CMSModules_MessageBoards_Content_Properties_Default : CMSContentMessageBoardsPage
{
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (CultureHelper.IsUICultureRTL())
        {
            ControlsHelper.ReverseFrames(colsFrameset);
        }

        tree.Attributes["src"] = "tree.aspx" + RequestContext.CurrentQueryString;

        RegisterModalPageScripts();
    }
}