using System;

using CMS.Helpers;
using CMS.Membership;
using CMS.MessageBoards;
using CMS.UIControls;
using CMS.Base;

public partial class CMSModules_MessageBoards_CMSPages_Message_Edit : CMSLiveModalPage
{
    private int mBoardId = 0;
    private int mMessageId = 0;


    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        mBoardId = QueryHelper.GetInteger("messageboardid", 0);
        mMessageId = QueryHelper.GetInteger("messageId", 0);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        messageEditElem.AdvancedMode = true;
        messageEditElem.CheckFloodProtection = true;
        messageEditElem.MessageID = mMessageId;
        messageEditElem.MessageBoardID = mBoardId;
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


    private void messageEditElem_OnAfterMessageSaved(BoardMessageInfo message)
    {
        int queryMarkIndex = Request.RawUrl.IndexOfCSafe('?');
        string filterParams = Request.RawUrl.Substring(queryMarkIndex);

        ltlScript.Text = ScriptHelper.GetScript("wopener.RefreshBoardList('" + filterParams + "'); CloseDialog();");
    }


    private void messageEditElem_OnBeforeMessageSaved()
    {
        bool isOwner = false;

        BoardInfo board = BoardInfoProvider.GetBoardInfo(messageEditElem.MessageBoardID);
        if (board != null)
        {
            // Check if the current user is allowed to modify the message
            isOwner = BoardInfoProvider.IsUserAuthorizedToManageMessages(board);
        }

        if (!isOwner && !MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.MessageBoards", "Modify"))
        {
            RedirectToAccessDenied(GetString("board.messageedit.notallowed"));
        }
    }
}