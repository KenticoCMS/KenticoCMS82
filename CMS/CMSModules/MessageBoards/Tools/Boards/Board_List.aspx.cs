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

public partial class CMSModules_MessageBoards_Tools_Boards_Board_List : CMSMessageBoardBoardsPage
{
    private int mGroupId = 0;


    protected override void OnPreInit(EventArgs e)
    {
        if (mGroupId > 0)
        {
            Page.MasterPageFile = "~/CMSMasterPages/UI/SimplePage.master";
        }

        // Must be called after the master page file is set
        base.OnPreInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        mGroupId = QueryHelper.GetInteger("groupid", 0);

        boardList.IsLiveSite = false;
        boardList.GroupID = mGroupId;
        boardList.OnAction += new CommandEventHandler(boardList_OnAction);
    }


    private void boardList_OnAction(object sender, CommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "edit":
                int boardId = ValidationHelper.GetInteger(e.CommandArgument, 0);

                // Create a target site URL and pass the category ID as a parameter
                string editUrl = UIContextHelper.GetElementUrl("CMS.MessageBoards", "EditBoards");
                editUrl = URLHelper.AddParameterToUrl(editUrl, "objectid", boardId.ToString());
                editUrl = URLHelper.AddParameterToUrl(editUrl, "displaytitle", "false");
                URLHelper.Redirect(editUrl);
                break;

            default:
                break;
        }
    }
}