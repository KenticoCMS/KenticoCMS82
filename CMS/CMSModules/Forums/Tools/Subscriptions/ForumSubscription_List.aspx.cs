using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.ExtendedControls.ActionsConfig;
using CMS.Helpers;
using CMS.Base;
using CMS.UIControls;

public partial class CMSModules_Forums_Tools_Subscriptions_ForumSubscription_List : CMSForumsPage
{
    protected int forumId = 0;


    protected void Page_Load(object sender, EventArgs e)
    {
        forumId = QueryHelper.GetInteger("forumid", 0);

        subscriptionList.ForumID = forumId;
        subscriptionList.OnAction += new CommandEventHandler(subscriptionList_OnAction);
        subscriptionList.IsLiveSite = false;

        InitializeMasterPage(forumId);
    }


    protected void subscriptionList_OnAction(object sender, CommandEventArgs e)
    {
        switch (e.CommandName.ToLowerCSafe())
        {
            case "edit":
                URLHelper.Redirect("ForumSubscription_Edit.aspx?subscriptionId=" + e.CommandArgument);
                break;
            default:
                subscriptionList.ReloadData();
                break;
        }
    }


    /// <summary>
    /// Initializes Master Page.
    /// </summary>
    protected void InitializeMasterPage(int forumId)
    {
        Title = "Forums - Subscriptions List";

        HeaderAction action = new HeaderAction();
        action.Text = GetString("ForumSubscription_List.NewItemCaption");

        if (forumId > 0)
        {
            action.RedirectUrl = ResolveUrl("ForumSubscription_Edit.aspx?forumid=" + forumId);
        }

        CurrentMaster.HeaderActions.AddAction(action);
    }
}