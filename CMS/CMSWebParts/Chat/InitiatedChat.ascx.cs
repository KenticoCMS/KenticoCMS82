using System;
using System.Linq;
using System.Web.Script.Serialization;

using CMS.PortalControls;
using CMS.PortalEngine;
using CMS.UIControls;
using CMS.Chat;
using CMS.Helpers;


public partial class CMSWebParts_Chat_InitiatedChat : CMSAbstractWebPart
{
    #region Properties

    /// <summary>
    /// Gets or sets ItemTemplate property.
    /// </summary>
    public string TransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(this.GetValue("TransformationName"), ChatHelper.TransformationInitiatedChat);
        }
        set
        {
            this.SetValue("TransformationName", value);
        }
    }


    /// <summary>
    /// Transformation name for messages in ChatRoomMessages child webpart
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
    /// Transformation name for errors in ChatErrors child webpart
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
    /// Transformation name for clear all errors button in ChatErrors child webpart
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
    /// Transformation name for users in ChatRoomUsers child webpart
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

    #endregion


    protected void Page_Prerender(object sender, EventArgs e)
    {
        ChatHelper.RegisterStylesheet(Page, false);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!ViewMode.IsOneOf(ViewModeEnum.LiveSite, ViewModeEnum.Preview))
        {
            return;
        }

        // Registration to chat webservice
        AbstractCMSPage cmsPage = Page as AbstractCMSPage;
        if (cmsPage != null)
        {
            ChatHelper.RegisterChatAJAXProxy(cmsPage);
        }

        // Script references insertion
        ScriptHelper.RegisterJQuery(Page);
        ScriptHelper.RegisterScriptFile(Page, "~/CMSModules/Chat/CMSPages/ChatSettings.ashx", false);

        ScriptHelper.RegisterJQueryTemplates(Page);
        ScriptHelper.RegisterScriptFile(Page, "~/CMSWebParts/Chat/InitiatedChat_files/InitiatedChat.js");

        int optID = ChatPopupWindowSettingsHelper.Store(ChatMessageTransformationName, ChatRoomUserTransformationName, ChatErrorTransformationName, ChatErrorDeleteAllButtonTransformationName);

        // Run script
        JavaScriptSerializer sr = new JavaScriptSerializer();
        string json = sr.Serialize(
            new
            {
                clientID = pnlInitiatedChat.ClientID,
                contentID = pnlContent.ClientID,
                pnlErrorID = pnlError.ClientID,
                lblErrorID = lblError.ClientID,
                windowURL = ChatHelper.GetChatRoomWindowURL(),
                trans = ChatHelper.GetWebpartTransformation(TransformationName, "chat.error.transformation.initiatedchat.error"),
                guid = optID,
                pingTick = ChatHelper.GlobalPingIntervalSetting * 1000
            }
        );
        string startupScript = string.Format("InitInitiatedChatManager({0});", json);
        ScriptHelper.RegisterStartupScript(Page, typeof(string), "ChatInitiatedChat_" + ClientID, startupScript, true);
    }

}
