using System;
using System.Data;

using CMS.Core;
using CMS.Helpers;
using CMS.Base;
using CMS.Membership;
using CMS.UIControls;
using CMS.ExtendedControls;

[Action(0, "avat.newavatar", "Avatar_Edit.aspx")]
[UIElement(ModuleName.CMS, "Administration.Avatars")]
public partial class CMSModules_Avatars_Avatar_List : GlobalAdminPage
{
    private CurrentUserInfo currentUser = null;


    protected void Page_Load(object sender, EventArgs e)
    {
        currentUser = MembershipContext.AuthenticatedUser;

        if (currentUser == null)
        {
            return;
        }

        // Page title
        PageTitle.TitleText = GetString("avat.title");
        // Set up unigrid options
        unigridAvatarList.OrderBy = "AvatarName";
        unigridAvatarList.OnExternalDataBound += unigridAvatarList_OnExternalDataBound;
        unigridAvatarList.OnAction += unigridAvatarList_OnAction;
        unigridAvatarList.GridView.AddCssClass("rows-middle-vertical-align");
        unigridAvatarList.GridView.PageSize = 10;
        unigridAvatarList.ZeroRowsText = GetString("general.nodatafound");
        unigridAvatarList.HideFilterButton = true;
    }


    protected object unigridAvatarList_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "avatartype":
                return GetString("avat.type" + (string)parameter);
            case "imagepreview":
                return "<img src=\"" + HTMLHelper.EncodeForHtmlAttribute(ResolveUrl("~/CMSModules/Avatars/CMSPages/GetAvatar.aspx?avatarguid=" + parameter + "&maxsidesize=50")) + "\" alt=\"\" /> ";
            case "avatarname":
                DataRowView dr = (DataRowView)parameter;
                string avatarName = HTMLHelper.HTMLEncode(dr.Row["AvatarName"].ToString());
                if (string.IsNullOrEmpty((avatarName)))
                {
                    string name = HTMLHelper.HTMLEncode(dr.Row["AvatarFilename"].ToString());
                    name = name.Substring(0, name.LastIndexOfCSafe("."));
                    return name;
                }
                else
                {
                    return avatarName;
                }
            default:
                return "";
        }
    }


    protected void unigridAvatarList_OnAction(string actionName, object actionArgument)
    {
        // Edit action
        if (DataHelper.GetNotEmpty(actionName, String.Empty) == "edit")
        {
            URLHelper.Redirect("Avatar_Edit.aspx?avatarid=" + (string)actionArgument);
        }
        // Delete action
        else if (DataHelper.GetNotEmpty(actionName, String.Empty) == "delete")
        {
            int avatarId = ValidationHelper.GetInteger(actionArgument, 0);
            if (avatarId > 0)
            {
                AvatarInfoProvider.DeleteAvatarInfo(avatarId);
            }
        }
    }
}