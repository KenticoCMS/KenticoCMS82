using System;

using CMS.Core;
using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.UIControls;

[UIElement(ModuleName.CMS, "APIExamples")]
public partial class CMSAPIExamples_Pages_Main : CMSAPIExamplePage
{
    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        frmMenu.Attributes["src"] = ResolveUrl("~/CMSAPIExamples/Pages/Menu.aspx") + RequestContext.CurrentQueryString;


        if (CultureHelper.IsUICultureRTL())
        {
            ControlsHelper.ReverseFrames(colsFrameset);
        }
    }

    #endregion
}