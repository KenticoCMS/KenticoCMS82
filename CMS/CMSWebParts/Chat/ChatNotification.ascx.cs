using System;
using System.Linq;
using System.Web.UI.WebControls;

using CMS.PortalControls;
using CMS.Helpers;
using CMS.UIControls;
using CMS.Chat;

using System.Web.Script.Serialization;
using CMS.PortalEngine;

public partial class CMSWebParts_Chat_ChatNotification : CMSAbstractWebPart
{
    #region "Properties"

    /// <summary>
    /// Gets or sets NotificationTransformation property.
    /// </summary>
    public string NotificationTransformation
    {
        get
        {
            return DataHelper.GetNotEmpty(this.GetValue("NotificationTransformation"), ChatHelper.TransformationNotifications);
        }
        set
        {
            this.SetValue("NotificationTransformation", value);
        }
    }


    /// <summary>
    /// Gets or sets ChatMessageTransformationName property.
    /// </summary>
    public string ChatMessageTransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(this.GetValue("ChatMessageTransformationName"), ChatHelper.TransformationRoomMessages);
        }
        set
        {
            this.SetValue("ChatMessageTransformationName", value);
        }
    }


    /// <summary>
    /// Gets or sets ChatErrorTransformationName property.
    /// </summary>
    public string ChatErrorTransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(this.GetValue("ChatErrorTransformationName"), ChatHelper.TransformationErrors);
        }
        set
        {
            this.SetValue("ChatErrorTransformationName", value);
        }
    }


    /// <summary>
    /// Gets or sets ChatErrorDeleteAllButtonTransformationName property.
    /// </summary>
    public string ChatErrorDeleteAllButtonTransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(this.GetValue("ChatErrorDeleteAllButtonTransformationName"), ChatHelper.TransformationErrorsDeleteAll);
        }
        set
        {
            this.SetValue("ChatErrorDeleteAllButtonTransformationName", value);
        }
    }


    /// <summary>
    /// Gets or sets ChatRoomUserTransformationName property.
    /// </summary>
    public string ChatRoomUserTransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(this.GetValue("ChatRoomUserTransformationName"), ChatHelper.TransformationRoomUsers);
        }
        set
        {
            this.SetValue("ChatRoomUserTransformationName", value);
        }
    }

    
    /// <summary>
    /// Gets or sets EnableNotificationBubble property.
    /// </summary>
    public bool EnableNotificationBubble
    {
        get
        {
            return ValidationHelper.GetBoolean(this.GetValue("EnableNotificationBubble"), true);
        }
        set
        {
            this.SetValue("EnableNotificationBubble", value);
        }
    }


    public string InnerContainerName { get; set; }
    public string InnerContainerTitle { get; set; }


    #endregion


    #region "Page events"

    protected void Page_Prerender(object sender, EventArgs e)
    {
        ChatFunctions.MakeWebpartEnvelope("ChatWebpartEnvelope ChatWebpartEnvelopeNotification", this, InnerContainerTitle, InnerContainerName);
        ChatHelper.RegisterStylesheet(Page);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Register chat webservice
        AbstractCMSPage cmsPage = Page as AbstractCMSPage;
        if (cmsPage != null)
        {
            ChatHelper.RegisterChatAJAXProxy(cmsPage);
        }

        // Insert script references
        ChatHelper.RegisterChatManager(Page);
        ChatHelper.RegisterChatNotificationManager(Page);
        ScriptHelper.RegisterJQueryTemplates(Page);
        ScriptHelper.RegisterScriptFile(Page, "~/CMSWebParts/Chat/ChatNotification_files/ChatNotification.js");

        btnRemoveAllNotifications.OnClientClick = "ChatManager.RemoveAllNotifications(); return false;";
        btnShow.Text = ResHelper.GetString("chat.notification.bubble.show");
        btnClose.Text = ResHelper.GetString("chat.notification.bubble.close");
        
        // Prepare and run startup script
        ScriptHelper.RegisterStartupScript(Page, typeof(string), "ChatNotification_" + ClientID, BuildStartupScript(), true);

        imgChatRoomsPrompt.ImageUrl = GetImageUrl("CMSModules/CMS_Chat/message_information24.png");
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Converts control's clientID to javascript jQuery selector string
    /// </summary>
    /// <param name="control">Web control</param>
    private string GetString(WebControl control)
    {
        return "#" + control.ClientID;
    }


    /// <summary>
    /// Builds startup script.
    /// </summary>
    private string BuildStartupScript()
    {
        // Set all the transformation settings for chat room window
        int roomSettingsId = ChatPopupWindowSettingsHelper.Store(ChatMessageTransformationName, ChatRoomUserTransformationName, ChatErrorTransformationName, ChatErrorDeleteAllButtonTransformationName);
        bool isLiveSite = ViewMode.IsOneOf(ViewModeEnum.LiveSite, ViewModeEnum.Preview);

        JavaScriptSerializer sr = new JavaScriptSerializer();
        string json = sr.Serialize(
            new
            {
                template = ChatHelper.GetWebpartTransformation(NotificationTransformation, "chat.error.transformation.notification"), 
                clientID = ClientID,
                chatRoomGUID = roomSettingsId, 
                pnlChatNotificationEmpty = GetString(pnlChatNotificationEmpty), 
                pnlChatNotificationFull = GetString(pnlChatNotificationFull), 
                btnChatNotificationFullLink = GetString(btnChatNotificationFullLink), 
                lblChatNotificationFullTextNumber = GetString(lblChatNotificationFullTextNumber),
                pnlChatNotificationNotifications = GetString(pnlChatNotificationNotifications), 
                pnlChatNotificationNotificationsList = GetString(pnlChatNotificationNotificationsList), 
                btnChatNotificationPromptClose = GetString(btnChatNotificationPromptClose),  
                wpPanelID = GetString(pnlWPNotifications), 
                envelopeID = "#envelope_" + ClientID,
                bubbleBtnShow = GetString(btnShow),
                bubbleBtnClose = GetString(btnClose),
                bubbleLabel = GetString(lblInfoMessage),
                bubblePanel = GetString(pnlNotificationInfoBubble),
                strNoNotif = ResHelper.GetString("chat.notification.empty"),
                resNewNotif = ResHelper.GetString("chat.notification.youhave"),
                bubbleEnabled = EnableNotificationBubble && isLiveSite,
                isPreview = ViewMode.IsPreview(),
                notificationManagerOptions = new
                {
                    eventName = "newnotification",
                    soundFile = ChatHelper.EnableSoundLiveChat ? ResolveUrl("~/CMSModules/Chat/CMSPages/Sound/Chat_notification.mp3") : String.Empty,
                    notifyTitle = ResHelper.GetString("chat.notification.bubble.header")
                }
            }
        );
        return String.Format("InitChatNotification({0});", json);
    }

    #endregion
}