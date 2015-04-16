using System;
using System.Linq;
using System.Web.UI;

using CMS;
using CMS.Base;
using CMS.ExtendedControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.UIControls;
using CMS.Modules;

[assembly: RegisterCustomClass("ModuleLibrariesExtender", typeof(ModuleLibrariesExtender))]

/// <summary>
/// Module library listing page extender
/// </summary>
public class ModuleLibrariesExtender : PageExtender<CMSPage>
{
    /// <summary>
    /// Initializes the page
    /// </summary>
    public override void OnInit()
    {
        Page.LoadComplete += page_LoadComplete;

        ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "RefreshWOpener",
            ScriptHelper.GetScript(@"function RefreshWOpener(){window.location = window.location;}"));
    }


    private void page_LoadComplete(object sender, EventArgs e)
    {
        var page = ((CMSPage)sender);
        page.AddHeaderAction(new HeaderAction()
        {
            OpenInDialog = true,
            RedirectUrl = URLHelper.AppendQuery(UIContextHelper.GetElementUrl("CMS", "LibrarySelectionDialog"), QueryHelper.BuildQuery("allowed_extensions", "dll", "starting_path", "~/bin", "resourceid", QueryHelper.GetString("parentobjectid", ""))),
            Text = ResHelper.GetString("cms.resourcelibrary.addnew"),
        });
    }
}
