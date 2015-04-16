using System;

using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.UIControls;

public partial class CMSModules_Forums_Content_Properties_Default : CMSForumsPage
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