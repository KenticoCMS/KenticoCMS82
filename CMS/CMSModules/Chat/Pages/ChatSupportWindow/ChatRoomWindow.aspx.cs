using System;
using System.Linq;

using CMS.UIControls;
using CMS.Chat;
using CMS.Helpers;
using System.Web.Script.Serialization;


/// <summary>
/// Partial class for support ChatRoomWindow opened from Desk only.
/// </summary>
public partial class CMSModules_Chat_Pages_ChatSupportWindow_ChatRoomWindow : CMSModalPage
{

    #region "Private methods"

    private void SetWebpartsUp(bool isSupport)
    {
        // Set errors webpart up.
        ChatErrorsElem.ErrorTransformationName = "Chat.Transformations.CMSChatError";
        ChatErrorsElem.ButtonDeleteAllTransformationName = "Chat.Transformations.CMSChatErrorDeleteAllButton";
        ChatErrorsElem.ShowDeleteAllBtn = true;
        ChatErrorsElem.IsSupport = isSupport;

        // Set messages webpart up.
        ChatRoomMessagesElem.ChatMessageTransformationName = "Chat.Transformations.CMSChatMessage";
        ChatRoomMessagesElem.Direction = ChatRoomMessagesDirectionEnum.Up;
        ChatRoomMessagesElem.Count = ChatHelper.FirstLoadMessagesCountSetting;
        ChatRoomMessagesElem.IsSupport = isSupport;

        // Set users webpart up.
        ChatRoomUsersElem.ChatUserTransformationName = "Chat.Transformations.CMSChatRoomUser";
        ChatRoomUsersElem.EnablePaging = true;
        ChatRoomUsersElem.PagingItems = 5;
        ChatRoomUsersElem.EnableFiltering = false;
        ChatRoomUsersElem.SortByStatus = true;
        ChatRoomUsersElem.InviteEnabled = false;
        ChatRoomUsersElem.IsSupport = isSupport;

        // Set send webpart up
        ChatMessageSendElem.IsSupport = isSupport;
        ChatMessageSendElem.HideUserPicker();
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Get information about chat room this window has been opened for.
        int roomId = QueryHelper.GetInteger("windowroomid", 0);
        try
        {
            ChatUserHelper.VerifyChatUserHasJoinRoomRights(roomId);
        }
        catch (ChatServiceException)
        {
            DisplayError();

            return;
        }
        ChatRoomInfo room = ChatRoomInfoProvider.GetChatRoomInfo(roomId);
        if (room == null)
        {
            DisplayError();

            return;
        }

        // Script references insertion
        ScriptHelper.RegisterJQuery(Page);
        ChatHelper.RegisterChatManager(Page);
        ChatHelper.RegisterChatNotificationManager(Page);
        ScriptHelper.RegisterScriptFile(Page, "~/CMSScripts/jquery/jquery-resize.js");
        ScriptHelper.RegisterScriptFile(Page, "~/CMSModules/Chat/Pages/Scripts/ChatRoomWindow.js");

        SetWebpartsUp(room.ChatRoomIsSupport);
        lblTitle.Text = HTMLHelper.HTMLEncode(room.ChatRoomDisplayName) + " – " + ResHelper.GetString(room.ChatRoomIsSupport ? "chat.title.support" : "chat.title.privateconversation");
        JavaScriptSerializer sr = new JavaScriptSerializer();
        string json = sr.Serialize(
            new
            {
                roomId,
                pnlChatRoomWindowClientId = pnlChatRoomWindow.ClientID,
                pnlTopClientId = pnlTop.ClientID,
                pnlBottomClientId = pnlBottom.ClientID,
                ChatRoomMessagesClientId = ChatRoomMessagesElem.ClientID,
                btnCloseClientId = btnCloseWindow.ClientID,
                isSupport = room.ChatRoomIsSupport,
                notificationManagerOptions = new
                {
                    eventName = "newmessage",
                    soundFile = ChatHelper.EnableSoundSupportChat ? ResolveUrl("~/CMSModules/Chat/CMSPages/Sound/Chat_message.mp3") : String.Empty,
                    notifyTitle = ResHelper.GetString("chat.general.newmessages")
                },
                title = lblTitle.Text
            }
        );
        string startupScript = String.Format("ChatSupportWindow({0});", json);
        ScriptHelper.RegisterStartupScript(Page, typeof(string), "ChatSupportWindow", startupScript, true);
    }


    private void DisplayError(string message = null)
    {
        lblError.Text = ResHelper.GetString("chat.error.window.badroomid");
        lblError.Visible = true;
        pnlBody.Visible = false;
    }

    #endregion
}
