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

public partial class CMSWebParts_Chat_ChatRoomUsers : CMSAbstractWebPart 
{
    bool mIsSupport = false;
    int mRoomID = -1;

    #region "Properties"

    /// <summary>
    /// Gets or sets ItemTemplate property. 
    /// </summary>
    public string ChatUserTransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("ChatUserTransformationName"), ChatHelper.TransformationRoomUsers);
        }
        set
        {
            SetValue("ChatUserTransformationName", value);
        }
    }


    /// <summary>
    /// Gets or sets SortByStatus property. 
    /// </summary>
    public bool SortByStatus
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("SortByStatus"), true);
        }
        set
        {
            SetValue("SortByStatus", value);
        }
    }


    /// <summary>
    /// Gets or sets InviteEnabled property. 
    /// </summary>
    public bool InviteEnabled
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("InviteUserEnabled"), true);
        }
        set
        {
            SetValue("InviteUserEnabled", value);
        }
    }


    /// <summary>
    /// Gets or sets InviteSearchMode property. 
    /// </summary>
    public bool InviteSearchMode
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("InviteSearchMode"), ChatHelper.WPInviteModeSearchMode);
        }
        set
        {
            SetValue("InviteSearchMode", value);
        }
    }


    /// <summary>
    /// Gets or sets InviteSearchModeMaxUsers property. 
    /// </summary>
    public int InviteSearchModeMaxUsers
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("InviteSearchModeMaxUsers"), -1);
        }
        set
        {
            SetValue("InviteSearchModeMaxUsers", value);
        }
    }


    /// <summary>
    /// Gets or sets ChatMessageTemplate property.
    /// </summary>
    public string ChatMessageTransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("ChatMessageTransformationName"), ChatHelper.TransformationRoomMessages);
        }
        set
        {
            SetValue("ChatMessageTransformationName", value);
        }
    }


    /// <summary>
    /// Gets or sets ItemTemplateModified property.
    /// </summary>
    public string ChatErrorTransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("ChatErrorTransformationName"), ChatHelper.TransformationErrors);
        }
        set
        {
            SetValue("ChatErrorTransformationName", value);
        }
    }


    /// <summary>
    /// Gets or sets ItemTemplateRejected property.
    /// </summary>
    public string ChatErrorDeleteAllButtonTransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("ChatErrorDeleteAllButtonTransformationName"), ChatHelper.TransformationErrorsDeleteAll);
        }
        set
        {
            SetValue("ChatErrorDeleteAllButtonTransformationName", value);
        }
    }


    /// <summary>
    /// Gets or sets ItemTemplateSystem property.
    /// </summary>
    public string ChatRoomUserTransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("ChatRoomUserTransformationName"), ChatHelper.TransformationRoomUsers);
        }
        set
        {
            SetValue("ChatRoomUserTransformationName", value);
        }
    }


    /// <summary>
    /// Gets or sets RoomName property.
    /// </summary>
    public string RoomName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("RoomName"), "");
        }
        set
        {
            SetValue("RoomName", value);
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
            return ValidationHelper.GetString(GetValue("GroupID"), "DefaultGroup");
        }
        set
        {
            SetValue("GroupID", value);
        }
    }


    /// <summary>
    /// Gets or sets EnablePaging property.
    /// </summary>
    public bool EnablePaging
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("EnablePaging"), false);
        }
        set
        {
            SetValue("EnablePaging", value);
        }
    }


    /// <summary>
    /// Gets or sets EnableFiltering property.
    /// </summary>
    public bool EnableFiltering
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("EnableFiltering"), false);
        }
        set
        {
            SetValue("EnableFiltering", value);
        }
    }


    /// <summary>
    /// Gets or sets PagingItems property.
    /// </summary>
    public int PagingItems
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("PagingItems"), -1);
        }
        set
        {
            SetValue("PagingItems", value);
        }
    }


    /// <summary>
    /// Gets or sets GroupPagesBy property.
    /// </summary>
    public int GroupPagesBy
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("GroupPagesBy"), -1);
        }
        set
        {
            SetValue("GroupPagesBy", value);
        }
    }


    /// <summary>
    /// Gets or sets ShowFilterItems property.
    /// </summary>
    public int ShowFilterItems
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("ShowFilterItems"), -1);
        }
        set
        {
            SetValue("ShowFilterItems", value);
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
        ChatFunctions.MakeWebpartEnvelope("ChatWebpartEnvelope ChatWebpartEnvelopeRoomUsers", this, InnerContainerTitle, InnerContainerName);
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

        // Insert cript references
        ChatHelper.RegisterChatManager(Page);
        ScriptHelper.RegisterJQueryTemplates(Page);
        ScriptHelper.RegisterScriptFile(Page, "~/CMSModules/Chat/CMSPages/Scripts/ListPaging.js");
        ScriptHelper.RegisterScriptFile(Page, "~/CMSModules/Chat/CMSPages/Scripts/ChatDialogs.js");
        ScriptHelper.RegisterScriptFile(Page, "~/CMSWebParts/Chat/ChatRoomUsers_files/ChatRoomUsers.js");

        RoomID = ChatHelper.GetRoomIdFromQuery(RoomID, GroupID);
        
        if (EnableFiltering)
        {
            pnlChatRoomUsersFiltering.Visible = true;
        }

        // Set properties to invite webpart
        ChatSearchOnlineUsers.InviteMode = ChatOnlineUsersElem.InviteMode = true;
        ChatSearchOnlineUsers.IsSupport = ChatOnlineUsersElem.IsSupport = IsSupport;
        ChatSearchOnlineUsers.GroupName = ChatOnlineUsersElem.GroupName = GroupID;
        ChatSearchOnlineUsers.OnlineUserTransformationName = ChatOnlineUsersElem.OnlineUserTransformationName = "Chat.Transformations.ChatOnlineUser";
        ChatSearchOnlineUsers.PagingEnabled = ChatOnlineUsersElem.EnablePaging = true;
        int invitePagingItems = (ChatHelper.WPInviteModePagingItems > 0) ? ChatHelper.WPInviteModePagingItems : ChatHelper.WPPagingItems;
        if (!(invitePagingItems > 0))
        {
            invitePagingItems = PagingItems;
        }
        ChatOnlineUsersElem.ShowFilterItems = invitePagingItems + 1;
        ChatSearchOnlineUsers.PagingItems = ChatOnlineUsersElem.PagingItems = invitePagingItems;
        ChatSearchOnlineUsers.ResponseMaxUsers = (InviteSearchModeMaxUsers >= 0) ? InviteSearchModeMaxUsers : ChatHelper.WPSearchModeMaxUsers;
        ChatOnlineUsersElem.EnableFiltering = true;

        if (InviteSearchMode == true)
        {
            ChatOnlineUsersElem.Visible = false;
        }
        else
        {
            ChatSearchOnlineUsers.Visible = false;
        }   

        // Run startup script
        ScriptHelper.RegisterStartupScript(Page, typeof(string), "ChatUsers_" + ClientID, BuildStartupScript() , true);

        imgChatRoomUsersInvitePrompt.ImageUrl = GetImageUrl("CMSModules/CMS_Chat/add24.png");
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Builds startup script.
    /// </summary>
    private string BuildStartupScript()
    {
        int id = ChatPopupWindowSettingsHelper.Store(ChatMessageTransformationName, ChatRoomUserTransformationName, ChatErrorTransformationName, ChatErrorDeleteAllButtonTransformationName);

        JavaScriptSerializer sr = new JavaScriptSerializer();
        string json = sr.Serialize(
            new
            {
                roomID = RoomID,
                chatUserTemplate = ChatHelper.GetWebpartTransformation(ChatUserTransformationName,"chat.error.transformation.users.user"),
                oneToOneURL = ChatHelper.GetChatRoomWindowURL(Page),
                contentClientID = pnlChatRoomUsers.ClientID,
                clientID = ClientID,
                groupID = GroupID,
                sortByStatus = SortByStatus,
                GUID = id,
                pnlChatRoomUsersInvitePrompt = pnlChatRoomUsersInvitePrompt.ClientID,
                pnlChatRoomUsersInvite = pnlChatRoomUsersInvite.ClientID, 
                chatOnlineUsersElem = ChatOnlineUsersElem.ClientID, 
                chatSearchOnlineUsersElem = ChatSearchOnlineUsers.ClientID,
                loadingDiv = ChatHelper.GetWebpartLoadingDiv("ChatRoomUsersWPLoading", "chat.wploading.roomusers"),
                btnChatRoomUsersInvite = btnChatRoomUsersInvite.ClientID,
                btnChatRoomsDeletePromptClose = btnChatRoomsDeletePromptClose.ClientID, 
                pnlFilterClientID = pnlChatRoomUsersFiltering.ClientID, 
                pnlPagingClientID = pnlChatRoomUsersPaging.ClientID, 
                pagingItems = PagingItems > 0 ? PagingItems : ChatHelper.WPPagingItems,
                groupPagesBy = GroupPagesBy >= 0 ? GroupPagesBy : ChatHelper.WPGroupPagesBy,
                pagingEnabled = EnablePaging, 
                btnFilter = btnChatRoomUsersFilter.ClientID, 
                txtFilter = txtChatRoomUsersFilter.ClientID, 
                filterEnabled = EnableFiltering, 
                pnlInfo = pnlChaRoomUsersInfo.ClientID, 
                resStrNoFound = ResHelper.GetString("chat.searchonlineusers.notfound"), 
                resStrResults = ResHelper.GetString("chat.searchonlineusers.results"), 
                filterCount = ShowFilterItems >= 0 ? ShowFilterItems : ChatHelper.WPShowFilterLimit, 
                envelopeID = "envelope_" + ClientID,
                inviteSearchMode = InviteSearchMode,
                inviteEnabled = InviteEnabled
            }
        );
        return String.Format("InitChatUsersWebpart({0});", json);
    }
    
    #endregion
}
