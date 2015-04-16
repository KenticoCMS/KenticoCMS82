using System;
using System.Web.UI.WebControls;

using CMS.Helpers;
using CMS.MessageBoards;
using CMS.Base;
using CMS.UIControls;
using CMS.ExtendedControls.ActionsConfig;

public partial class CMSModules_MessageBoards_Tools_Boards_Board_Edit_Subscriptions : CMSMessageBoardBoardsPage
{
    // Current board ID
    private int mBoardId = 0;
    private int mGroupId = 0;
    private bool changeMaster = false;


    protected void Page_Load(object sender, EventArgs e)
    {
        // Get current board and group ID
        mBoardId = QueryHelper.GetInteger("boardid", 0);
        mGroupId = QueryHelper.GetInteger("groupid", 0);

        // Get board info and chceck whether it belongs to current site
        BoardInfo board = BoardInfoProvider.GetBoardInfo(mBoardId);
        if (board != null)
        {
            CheckMessageBoardSiteID(board.BoardSiteID);
        }

        boardSubscriptions.GroupID = mGroupId;
        boardSubscriptions.Board = board;
        boardSubscriptions.ChangeMaster = changeMaster;
        boardSubscriptions.OnAction += new CommandEventHandler(boardSubscriptions_OnAction);

        // Initialize the master page
        InitializeMasterPage();
    }


    protected void boardSubscriptions_OnAction(object sender, CommandEventArgs e)
    {
        if (e.CommandName.ToLowerCSafe() == "edit")
        {
            // Redirect to edit page with subscription ID specified
            URLHelper.Redirect("Board_Edit_Subscription_Edit.aspx?subscriptionid=" + e.CommandArgument.ToString() + "&boardid=" + mBoardId.ToString() + "&changemaster=" + changeMaster
                               + ((mGroupId > 0) ? "&groupid=" + mGroupId : ""));
        }
    }


    #region "Private methods"

    /// <summary>
    /// Initializes the master page elements.
    /// </summary>
    private void InitializeMasterPage()
    {
        // Setup master page action element
        HeaderAction action = new HeaderAction();
        action.Text = GetString("board.subscriptions.newitem");
        action.RedirectUrl = ResolveUrl("~/CMSModules/MessageBoards/Tools/Boards/Board_Edit_Subscription_Edit.aspx?boardid=" + mBoardId.ToString() + "&changemaster=" + changeMaster);
        CurrentMaster.HeaderActions.AddAction(action);
    }

    #endregion
}