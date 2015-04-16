using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.Helpers;
using CMS.PortalEngine;
using CMS.UIControls;
using CMS.ExtendedControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.Modules;

public partial class CMSModules_Groups_Tools_MediaLibrary_Library_List : CMSGroupMediaLibraryPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        int groupId = QueryHelper.GetInteger("parentobjectid", 0);

        CheckGroupPermissions(groupId, CMSAdminControl.PERMISSION_READ);

        // Prevent display non-group libraries
        if (groupId == 0)
        {
            groupId = -1;
        }

        elemList.GroupID = groupId;

        // New item link
        CurrentMaster.HeaderActions.AddAction(new HeaderAction
        {  
            Text = GetString("media.list.newlibrary"),
            RedirectUrl = ResolveUrl("Library_New.aspx") + "?parentobjectid=" + elemList.GroupID,
        });

        elemList.OnEdit += new EventHandler(elemList_OnEdit);
    }


    #region "Private methods"

    private void elemList_OnEdit(object sender, EventArgs e)
    {
        URLHelper.Redirect(GetEditUrl());
    }


    /// <summary>
    /// Creates URL for editing.
    /// </summary>
    private String GetEditUrl()
    {
        UIElementInfo uiChild = UIElementInfoProvider.GetUIElementInfo("CMS.MediaLibrary", "Group.EditMediaLibrary");
        if (uiChild != null)
        {
            return URLHelper.AppendQuery(UIContextHelper.GetElementUrl(uiChild, UIContext), "displaytitle=false&objectid=" + elemList.SelectedItemID + "&parentobjectid=" + elemList.GroupID);
        }

        return String.Empty;
    }
    
    #endregion
}