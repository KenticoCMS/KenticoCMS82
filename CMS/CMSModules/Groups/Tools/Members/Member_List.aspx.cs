using System;
using System.Web.UI.WebControls;

using CMS.Helpers;
using CMS.Base;
using CMS.UIControls;
using CMS.ExtendedControls;

[Action(0, "group.member.addmember", "~/CMSModules/Groups/Tools/Members/Member_New.aspx?groupId={?groupId?}")]
[Action(1, "group.member.invitemember", "~/CMSModules/Groups/Tools/Members/Member_Invite.aspx?groupId={?groupId?}")]
public partial class CMSModules_Groups_Tools_Members_Member_List : CMSGroupPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        int groupId = QueryHelper.GetInteger("groupId", 0);

        // Register scripts
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "EditGroupMember", ScriptHelper.GetScript(
            "function editMember(memberId) {" +
            "    location.replace('Member_Edit.aspx?groupId=" + groupId + "&memberId=' + memberId);" +
            "}"));

        memberListElem.GroupID = groupId;
        memberListElem.OnAction += memberListElem_OnAction;
    }


    protected void memberListElem_OnAction(object sender, CommandEventArgs e)
    {
        switch (e.CommandName.ToLowerCSafe())
        {
            case "edit":
                URLHelper.Redirect("~/CMSModules/Groups/Tools/Members/Member_Edit.aspx?groupId=" + memberListElem.GroupID + "&memberId=" + e.CommandArgument);
                break;

            case "approve":
                ShowConfirmation(GetString("group.member.userhasbeenapproved"));
                break;

            case "reject":
                ShowConfirmation(GetString("group.member.userhasbeenrejected"));
                break;
        }
    }
}