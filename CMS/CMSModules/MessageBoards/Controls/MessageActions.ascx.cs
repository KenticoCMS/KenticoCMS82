using System;

using CMS.Helpers;
using CMS.MessageBoards;

public partial class CMSModules_MessageBoards_Controls_MessageActions : BoardMessageActions
{
    protected void Page_PreRender(object sender, EventArgs e)
    {
        // Initialize control elements
        SetupControl();
    }


    #region "Private methods"

    private void SetupControl()
    {
        // Initialize link button labels
        lnkApprove.Text = GetString("general.approve");
        lnkDelete.Text = GetString("general.delete");
        lnkEdit.Text = GetString("general.edit");
        lnkReject.Text = GetString("general.reject");

        // Set visibility according to the properties
        lnkEdit.Visible = ShowEdit;
        lnkDelete.Visible = ShowDelete;
        lnkApprove.Visible = ShowApprove;
        lnkReject.Visible = ShowReject;

        // Get client script
        ProcessData();

        // Register client script
        ScriptHelper.RegisterDialogScript(Page);
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "DeleteBoardMessageConfirmation", ScriptHelper.GetScript("function ConfirmDelete(){ return confirm(" + ScriptHelper.GetString(GetString("general.confirmdelete")) + ");}"));
    }


    /// <summary>
    /// Generate a client JavaScript for displaying modal window for message editing.
    /// </summary>
    private void ProcessData()
    {
        lnkEdit.OnClientClick = "EditBoardMessage('" + ResolveUrl("~/CMSModules/MessageBoards/CMSPages/Message_Edit.aspx") + "?messageid=" + MessageID.ToString() + "&messageboardid=" + MessageBoardID.ToString() + "');return false;";
    }

    #endregion


    #region "Event handlers"

    protected void lnkDelete_Click(object sender, EventArgs e)
    {
        FireOnBoardMessageAction("delete", MessageID);
    }


    protected void lnkApprove_Click(object sender, EventArgs e)
    {
        FireOnBoardMessageAction("approve", MessageID);
    }


    protected void lnkReject_Click(object sender, EventArgs e)
    {
        FireOnBoardMessageAction("reject", MessageID);
    }

    #endregion
}