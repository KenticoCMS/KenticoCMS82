using System;

using CMS.Helpers;
using CMS.PortalEngine;
using CMS.UIControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.Modules;

[UIElement("CMS.MediaLibrary", "MediaLibrary")]
public partial class CMSModules_MediaLibrary_Tools_Library_List : CMSMediaLibraryPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // New item link
        CurrentMaster.HeaderActions.AddAction(new HeaderAction
        {
            Text = GetString("media.list.newlibrary"),
            RedirectUrl = ResolveUrl("Library_New.aspx")
        });

        PageTitle.TitleText = GetString("media.list.medialibrary");
        elemList.OnEdit += new EventHandler(elemList_OnEdit);
    }


    private void elemList_OnEdit(object sender, EventArgs e)
    {
        URLHelper.Redirect(GetEditUrl());
    }


    /// <summary>
    /// Creates URL for editing.
    /// </summary>
    private String GetEditUrl()
    {
        UIElementInfo uiChild = UIElementInfoProvider.GetUIElementInfo("CMS.MediaLibrary", "EditMediaLibrary");
        if (uiChild != null)
        {
            return UIContextHelper.GetElementUrl(uiChild, false, elemList.SelectedItemID);
        }

        return String.Empty;
    }
}