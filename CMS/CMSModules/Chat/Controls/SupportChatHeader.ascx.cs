using System;
using System.Linq;
using System.Web.Script.Serialization;

using CMS.Helpers;
using CMS.UIControls;
using CMS.Chat;
using CMS.Membership;

public partial class CMSModules_Chat_Controls_SupportChatHeader : CMSUserControl
{
    #region "Private methods"

    /// <summary>
    /// Create startup script
    /// </summary>
    private string GetStartupScript()
    {
        JavaScriptSerializer sr = new JavaScriptSerializer();
        string json = sr.Serialize(
            new
            {
                headerIconId = headerIcon.ClientID, 
                btnLoginId = btnLogin.ClientID, 
                btnLoginShortcutId = btnLoginShortcut.ClientID, 
                btnLogoutId = btnLogout.ClientID, 
                btnSettingsId = btnSettings.ClientID, 
                loginShortcutWrapperId = loginShortcutWrapper.ClientID,
                lblNotificationNumberId = lblNotificationNumber.ClientID,
                ulActiveRequestsId = ulActiveRequests.ClientID,
                ulNewRequestsId = ulNewRequests.ClientID,
                lnkNewRequestsId = lnkNewRequests.ClientID,
                lblNewRequestsId = lblNewRequests.ClientID,

                resRoomNewMessagesFormat = ResHelper.GetString("chat.support.roomnewmessages"),
                resNewRequestsSingular = ResHelper.GetString("chat.support.newrequestssingular"),
                resNewRequestsPlural = ResHelper.GetString("chat.support.newrequestsplural"),

                settingsUrl = URLHelper.GetAbsoluteUrl("~/CMSModules/Chat/Pages/ChatSupportSettings.aspx"),
                notificationManagerOptions = new
                {
                    soundFileRequest = ChatHelper.EnableSoundSupportChat ? ResolveUrl("~/CMSModules/Chat/CMSPages/Sound/Chat_notification.mp3") : String.Empty,
                    soundFileMessage = ChatHelper.EnableSoundSupportChat ? ResolveUrl("~/CMSModules/Chat/CMSPages/Sound/Chat_message.mp3") : String.Empty,
                    notifyTitle = ResHelper.GetString("chat.general.newmessages")
                }
            }
        );

        return String.Format("$cmsj(function (){{ new ChatSupportHeader({0}); }});", json);
    }

    #endregion


    #region "Page events"

    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (!Visible || !ChatHelper.IsSupportChatPanelEnabled() || MembershipContext.SignOutPending)
        {
            Visible = false;

            return;
        }

        // Script references insertion
        ChatHelper.RegisterChatSupportManager(Page);
        ChatHelper.RegisterChatNotificationManager(Page);
        ScriptHelper.RegisterScriptFile(Page, "~/CMSModules/Chat/Controls/SupportChatHeader.js");

        // Create and launch startup script.
        ScriptHelper.RegisterStartupScript(Page, typeof(string), "SupportChatHeader_" + ClientID, GetStartupScript(), true);
    }
    
    #endregion
}
