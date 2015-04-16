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

public partial class CMSModules_Groups_Tools_Polls_Polls_List : CMSGroupPollsPage
{
    protected int groupID = 0;


    protected void Page_Load(object sender, EventArgs e)
    {
        // Get GroupID from query string
        groupID = QueryHelper.GetInteger("groupID", 0);

        CheckGroupPermissions(groupID, CMSAdminControl.PERMISSION_READ);

        if (CheckGroupPermissions(groupID, CMSAdminControl.PERMISSION_MANAGE, false))
        {
            HeaderAction action = new HeaderAction();
            action.Text = GetString("group.polls.newpoll");
            action.RedirectUrl = ResolveUrl("Polls_New.aspx") + "?groupid=" + groupID;
            CurrentMaster.HeaderActions.AddAction(action);

            pollsList.DeleteEnabled = true;
        }

        pollsList.OnEdit += new EventHandler(pollsList_OnEdit);
        pollsList.OnCheckPermissions += new CMSAdminControl.CheckPermissionsEventHandler(pollsList_OnCheckPermissions);
        pollsList.GroupId = groupID;
        pollsList.IsLiveSite = false;
    }


    private void pollsList_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        // Check permissions
        CheckPermissions(groupID, CMSAdminControl.PERMISSION_MANAGE);
    }


    /// <summary>
    /// Edit poll click handler.
    /// </summary>
    private void pollsList_OnEdit(object sender, EventArgs e)
    {
        string editActionUrl = URLHelper.AddParameterToUrl(UIContextHelper.GetElementUrl("CMS.Polls", "Groups.EditGroup.EditPoll", false, pollsList.SelectedItemID), "groupid", groupID.ToString());
        URLHelper.Redirect(editActionUrl);
    }
}