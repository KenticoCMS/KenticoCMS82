using System;

using CMS.Helpers;
using CMS.MessageBoards;
using CMS.UIControls;
using CMS.Base;
using CMS.Membership;

public partial class CMSModules_Groups_Tools_MessageBoards_Messages_Message_Edit : CMSModalPage
{
    private int mBoardId = 0;
    private int mMessageId = 0;
    private int mGroupId = 0;
    private CurrentUserInfo cu = null;


    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        // Check permissions for CMS Desk -> Tools -> Groups
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerUIElement("CMS.Groups", "Groups"))
        {
            RedirectToUIElementAccessDenied("CMS.Groups", "Groups");
        }

        mBoardId = QueryHelper.GetInteger("boardId", 0);
        mMessageId = QueryHelper.GetInteger("messageId", 0);
        mGroupId = QueryHelper.GetInteger("groupid", 0);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Check 'Read' permission
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.groups", CMSAdminControl.PERMISSION_READ))
        {
            RedirectToAccessDenied("cms.groups", CMSAdminControl.PERMISSION_READ);
        }

        cu = MembershipContext.AuthenticatedUser;

        messageEditElem.IsLiveSite = false;
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
    }


    private void messageEditElem_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        CheckLocalPermissions();
    }


    protected void CheckLocalPermissions()
    {
        int groupId = 0;
        int boardId = mBoardId;

        BoardMessageInfo bmi = BoardMessageInfoProvider.GetBoardMessageInfo(mMessageId);
        if (bmi != null)
        {
            boardId = bmi.MessageBoardID;
        }

        BoardInfo bi = BoardInfoProvider.GetBoardInfo(boardId);
        if (bi != null)
        {
            groupId = bi.BoardGroupID;
        }


        // Check 'Manage' permission
        if (!MembershipContext.AuthenticatedUser.IsGroupAdministrator(groupId))
        {
            if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.groups", CMSAdminControl.PERMISSION_MANAGE))
            {
                RedirectToAccessDenied("cms.groups", CMSAdminControl.PERMISSION_MANAGE);
            }
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
        CheckLocalPermissions();
    }
}