using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.UIControls;
using CMS.Chat;
using CMS.PortalControls;
using CMS.Helpers;
using CMS.Base;
using System.Web.Script.Serialization;

public partial class CMSWebParts_Chat_ChatSupportRequest : CMSAbstractWebPart
{
    #region "Properties"

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


    /// <summary>
    /// Transformation name for support chat request button
    /// </summary>
    public string ChatSupportRequestTransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(this.GetValue("ChatSupportRequestTransformationName"), ChatHelper.TransformationSupportRequest);
        }
        set
        {
            this.SetValue("ChatSupportRequestTransformationName", value);
        }
    }

    #endregion 


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        ChatHelper.RegisterStylesheet(Page);
    } 


    protected void Page_Load(object sender, EventArgs e)
    {
        // Registration to chat webservice
        AbstractCMSPage cmsPage = Page as AbstractCMSPage;
        if (cmsPage != null)
        {
            ChatHelper.RegisterChatAJAXProxy(cmsPage);
        }

        // Script references insertion

        ChatHelper.RegisterChatManager(Page);
        ScriptHelper.RegisterJQueryTemplates(Page);
        ScriptHelper.RegisterScriptFile(Page, "~/CMSWebParts/Chat/ChatSupportRequest_files/ChatSupportRequest.js");
                
        // Create and store settings for popup chat window.
        int id = ChatPopupWindowSettingsHelper.Store(ChatMessageTransformationName, ChatRoomUserTransformationName, ChatErrorTransformationName, ChatErrorDeleteAllButtonTransformationName);

        JavaScriptSerializer sr = new JavaScriptSerializer();
        string json = sr.Serialize(
            new
            {
                onlineUrl = ChatHelper.GetChatRoomWindowURL(),
                clientID = pnlSupportChatRequest.ClientID,
                guid = id,
                trans = ChatHelper.GetWebpartTransformation(string.IsNullOrEmpty(ChatSupportRequestTransformationName) ? ChatHelper.TransformationSupportRequest : ChatSupportRequestTransformationName, "chat.error.transformation.request"),
                mailEnabled = ChatHelper.IsSupportMailEnabledAndValid,
                pnlInformDialog = "#" + pnlChatSupportRequestInfoDialog.ClientID, 
                btnInformDialogClose = "#" + btnChatSupportRequestInformDialogClose.ClientID
            }
        );

        string startupScript = String.Format("ChatSupportRequest({0});", json);
        
        // Run startup script
        ScriptHelper.RegisterStartupScript(Page, typeof(string), "SupportChatRequest_" + ClientID, startupScript, true);
    }
}