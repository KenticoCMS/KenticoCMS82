using System;

using CMS.Helpers;
using CMS.MessageBoards;
using CMS.UIControls;
using CMS.Base;
using CMS.Membership;

public partial class CMSModules_Groups_CMSPages_Message_Edit : CMSLiveModalPage
{
    private int mBoardId = 0;
    private int mMessageId = 0;
    private int mGroupId = 0;
    private CurrentUserInfo cu = null;


    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        mBoardId = QueryHelper.GetInteger("boardId", 0);
        mMessageId = QueryHelper.GetInteger("messageId", 0);
        mGroupId = QueryHelper.GetInteger("groupid", 0);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        cu = MembershipContext.AuthenticatedUser;

        // Check 'Manage' permission
        if (!cu.IsGroupAdministrator(mGroupId) && !MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.groups", CMSAdminControl.PERMISSION_MANAGE))
        {
            RedirectToAccessDenied("cms.groups", CMSAdminControl.PERMISSION_MANAGE);
        }

        messageEditElem.AdvancedMode = true;
        messageEditElem.MessageID = mMessageId;
        messageEditElem.MessageBoardID = mBoardId;

        messageEditElem.OnCheckPermissions += new CMSAdminControl.CheckPermissionsEventHandler(messageEditElem_OnCheckPermissions);

        messageEditElem.OnBeforeMessageSaved += new OnBeforeMessageSavedEventHandler(messageEditElem_OnBeforeMessageSaved);
        messageEditElem.OnAfterMessageSaved += new OnAfterMessageSavedEventHandler(messageEditElem_OnAfterMessageSaved);

        // initializes page title control		
        if (mMessageId > 0)
        {
            PageTitle.TitleText = GetString("Board.MessageEdit.title");
        }
        else
        {
            PageTitle.TitleText = GetString("Board.MessageNew.title");
        }

        if (!URLHelper.IsPostback())
        {
            messageEditElem.ReloadData();
        }
    }


    private void messageEditElem_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        // Check 'Manage' permission
        if (!cu.IsGroupAdministrator(mGroupId) && !MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.groups", CMSAdminControl.PERMISSION_MANAGE))
        {
            RedirectToAccessDenied("cms.groups", CMSAdminControl.PERMISSION_MANAGE);
        }
    }


    private void messageEditElem_OnAfterMessageSaved(BoardMessageInfo message)
    {
        int queryMarkIndex = Request.RawUrl.IndexOfCSafe('?');
        string filterParams = Request.RawUrl.Substring(queryMarkIndex);

        ltlScript.Text = ScriptHelper.GetScript("wopener.RefreshBoardList('" + filterParams + "');CloseDialog();");
    }


    private void messageEditElem_OnBeforeMessageSaved()
    {
        bool isOwner = false;

        BoardMessageInfo message = BoardMessageInfoProvider.GetBoardMessageInfo(mMessageId);
        if (message != null)
        {
            // Check if the current user is allowed to modify the message
            isOwner = ((MembershipContext.AuthenticatedUser.IsGlobalAdministrator) || cu.IsGroupAdministrator(mGroupId) ||
                       (BoardModeratorInfoProvider.IsUserBoardModerator(MembershipContext.AuthenticatedUser.UserID, message.MessageBoardID)) ||
                       (message.MessageUserID == MembershipContext.AuthenticatedUser.UserID));
        }

        if (!isOwner && !cu.IsGroupAdministrator(mGroupId) && !MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.groups", CMSAdminControl.PERMISSION_MANAGE))
        {
            RedirectToAccessDenied(GetString("board.messageedit.notallowed"));
        }
    }
}