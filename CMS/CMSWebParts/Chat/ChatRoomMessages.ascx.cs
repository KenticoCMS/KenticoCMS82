using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.PortalControls;
using CMS.UIControls;
using CMS.Chat;
using CMS.Helpers;
using CMS.Controls;
using CMS.DataEngine;
using CMS.Base;
using System.Data;
using System.Web.Script.Serialization;

public partial class CMSWebParts_Chat_ChatRoomMessages: CMSAbstractWebPart
{
    bool mIsSupport = false;
    int mRoomID = -1;

    #region Properties

    /// <summary>
    /// Gets or sets ItemTemplate property.
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
    /// Gets or sets RoomName property.
    /// </summary>
    public string RoomName
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("RoomName"), "");
        }
        set
        {
            this.SetValue("RoomName", value);
        }
    }


    /// <summary>
    /// Gets or sets RoomID property.
    /// </summary>
    public int RoomID
    {
        get
        {
            if (mRoomID < 0)
            {
                ChatRoomInfo room = ChatRoomInfoProvider.GetChatRoomInfo(RoomName);
                if (room != null)
                {
                    mRoomID = room.ChatRoomID;
                }
                else
                {
                    mRoomID = 0;
                }
            }
            return mRoomID;
        }
        set
        {
            mRoomID = value;
        }
    }


    /// <summary>
    /// Gets or sets GroupID property.
    /// </summary>
    public string GroupID
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("GroupID"), "DefaultGroup");
        }
        set
        {
            this.SetValue("GroupID", value);
        }
    }


    /// <summary>
    /// Gets or sets Count property.
    /// </summary>
    public int Count
    {
        get
        {
            return ValidationHelper.GetInteger(this.GetValue("Count"), -1);
        }
        set
        {
            this.SetValue("Count", value);
        }
    }


    /// <summary>
    /// Gets or sets DisplayInline property.
    /// </summary>
    public bool DisplayInline
    {
        get
        {
            return ValidationHelper.GetBoolean(this.GetValue("DisplayInline"), false);
        }
        set
        {
            this.SetValue("DisplayInline", value);
        }
    }


    /// <summary>
    /// Gets or sets Direction property.
    /// </summary>
    public ChatRoomMessagesDirectionEnum Direction
    {
        get
        {
            return (ChatRoomMessagesDirectionEnum)ValidationHelper.GetInteger(this.GetValue("Direction"), (int)ChatRoomMessagesDirectionEnum.Up);
        }
        set
        {
            this.SetValue("Direction", (int)value);
        }
    }


    /// <summary>
    /// Gets or sets EnableBBCode property.
    /// </summary>
    public bool EnableBBCode
    {
        get
        {
            return ValidationHelper.GetBoolean(this.GetValue("EnableBBCode"), true);
        }
        set
        {
            this.SetValue("EnableBBCode", value);
        }
    }

    public string InnerContainerName { get; set; }
    public string InnerContainerTitle { get; set; }
    public bool IsSupport
    {
        get
        {
            return mIsSupport;
        }
        set
        {
            mIsSupport = value;
        }
    }

    #endregion 


    #region "Page events"


    protected void Page_Prerender(object sender, EventArgs e)
    {
        ChatFunctions.MakeWebpartEnvelope("ChatWebpartEnvelope ChatWebpartEnvelopeRoomMessages", this, InnerContainerTitle, InnerContainerName);
        if (IsSupport)
        {
            ChatHelper.RegisterStylesheet(Page, true);
        }
        else
        {
            ChatHelper.RegisterStylesheet(Page);
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Register to chat webservice
        AbstractCMSPage cmsPage = Page as AbstractCMSPage;
        if (cmsPage != null)
        {
            ChatHelper.RegisterChatAJAXProxy(cmsPage);
        }

        // Insert script references
        ChatHelper.RegisterChatManager(Page);
        ScriptHelper.RegisterJQueryTemplates(Page);
        ScriptHelper.RegisterScriptFile(Page, "~/CMSModules/Chat/CMSPages/Scripts/BBCodeParser.js");
        ScriptHelper.RegisterScriptFile(Page, "~/CMSModules/Chat/CMSPages/Scripts/SmileysResolver.js");
        ScriptHelper.RegisterScriptFile(Page, "~/CMSWebParts/Chat/ChatRoomMessages_files/ChatRoomMessages.js");

        imgInformationDialog.ImageUrl = GetImageUrl("General/Labels/Information.png");

        RoomID = ChatHelper.GetRoomIdFromQuery(RoomID, GroupID);

        // Prepare and run startup script
        ScriptHelper.RegisterStartupScript(Page, typeof(string), "ChatMessages_" + ClientID, BuildStartupScript(), true);
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Builds startup script.
    /// </summary>
    private string BuildStartupScript()
    {
        JavaScriptSerializer sr = new JavaScriptSerializer();
        string json = sr.Serialize(
            new
            {
                roomID = RoomID,
                chatMessageTemplate = ChatHelper.GetWebpartTransformation(ChatMessageTransformationName,"chat.error.transformation.messages.message"),
                count = (Count >= 0) ? Count : ChatHelper.FirstLoadMessagesCountSetting,
                contentClientID =  "#" + pnlChatRoomMessages.ClientID,
                displayInline = DisplayInline,
                groupID = GroupID,
                clientID = ClientID,
                direction = (int)Direction,
                enableBBCode = (ChatHelper.EnableBBCodeSetting && EnableBBCode),
                loadingDiv = ChatHelper.GetWebpartLoadingDiv("ChatMessagesWPLoading", "chat.wploading.messages"), 
                envelopeID = "#envelope_" + ClientID,
                pnlInformDialog = "#" + pnlChatRoomMessagesInfoDialog.ClientID, 
                btnInformDialogClose = "#" + btnChatMessageInformDialogClose.ClientID,
                container = "#" + pnlChatRoomWebpart.ClientID
            }
        );
        return String.Format("InitChatMessagesWebpart({0});", json);
    }

    #endregion
}
