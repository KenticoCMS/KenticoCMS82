using System;
using System.Collections;
using System.Linq;
using System.Web.UI;

using CMS;
using CMS.AzureStorage;
using CMS.Base;
using CMS.ExtendedControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.UIControls;

[assembly: RegisterCustomClass("ModuleLibrarySelectionExtender", typeof(ModuleLibrarySelectionExtender))]

/// <summary>
/// Analytics page extender
/// </summary>
public class ModuleLibrarySelectionExtender : ControlExtender<CMSUserControl>
{
    private LocalizedButton btnSelect;

    private CMSUIPage page;


    public override void OnInit()
    {
        btnSelect = new LocalizedButton();
        page = ((CMSUIPage)Control.Page);
        btnSelect.ResourceString = "general.select";
        btnSelect.Click += btnSelectClick;
        btnSelect.ID = "btnSelect";
        page.DialogFooter.AddControl(btnSelect);
        page.DialogFooter.HideCancelButton();
        if (!RequestHelper.IsPostBack())
        {
            SessionHelper.Remove("DialogSelectedParameters");
        }
    }


    private void btnSelectClick(object sender, EventArgs e)
    {
        Hashtable selectedParameters = SessionHelper.GetValue("DialogSelectedParameters") as Hashtable;
        object filePath = null;

        if (selectedParameters != null)
        {
            filePath = selectedParameters[DialogParameters.ITEM_PATH];
        }

        if (filePath == null)
        {
            page.ShowError(ResHelper.GetString("cms.resourcelibrary.noselectionerror"), null, null, false);

            return;
        }

        ResourceLibraryInfo libraryInfo = new ResourceLibraryInfo()
        {
            ResourceLibraryPath = filePath.ToString(),
            ResourceLibraryResourceID = QueryHelper.GetInteger("resourceid", -1),
        };

        try
        {
            ResourceLibraryInfoProvider.SetResourceLibraryInfo(libraryInfo);
        }
        catch
        {
            page.ShowError(ResHelper.GetString("cms.resourcelibrary.collision"), null, null, false);

            return;
        }

        SessionHelper.Remove("DialogSelectedParameters");
        ScriptHelper.RegisterStartupScript(page, typeof(string), "Close dialog", ScriptHelper.GetScript("CloseDialog(true);"));
    }
}
