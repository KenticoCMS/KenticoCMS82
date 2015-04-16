using System;
using System.Web.Script.Serialization;

using CMS.PortalControls;
using CMS.PortalEngine;
using CMS.UIControls;
using CMS.Chat;
using CMS.Helpers;
using CMS.MacroEngine;


public partial class CMSWebParts_Chat_AutoInitiatedChat : CMSAbstractWebPart
{
    #region Properties

    /// <summary>
    /// Gets or sets InitiaterName property.
    /// </summary>
    public string InitiatorName
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("InitiatorName"), "");
        }
        set
        {
            this.SetValue("InitiatorName", value);
        }
    }


    /// <summary>
    /// Gets or sets Messages property.
    /// </summary>
    public string Messages
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("Messages"), "");
        }
        set
        {
            this.SetValue("Messages", value);
        }
    }


    /// <summary>
    /// Gets or sets Delay property.
    /// </summary>
    public int Delay
    {
        get
        {
            return ValidationHelper.GetInteger(this.GetValue("Delay"), 0);
        }
        set
        {
            this.SetValue("Delay", value);
        }
    }


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
        if (!StopProcessing)
        {
            ChatHelper.RegisterStylesheet(Page, false);
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing || !ViewMode.IsOneOf(ViewModeEnum.LiveSite, ViewModeEnum.Preview))
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
        ScriptHelper.RegisterJQueryCookie(Page);
        ScriptHelper.RegisterScriptFile(Page, "~/CMSModules/Chat/CMSPages/ChatSettings.ashx", false);

        ScriptHelper.RegisterJQueryTemplates(Page);
        ScriptHelper.RegisterScriptFile(Page, "~/CMSWebParts/Chat/AutoInitiatedChat_files/AutoInitiatedChat.js");

        int optID = ChatPopupWindowSettingsHelper.Store(ChatMessageTransformationName, ChatRoomUserTransformationName, ChatErrorTransformationName, ChatErrorDeleteAllButtonTransformationName);

        // Run script
        JavaScriptSerializer sr = new JavaScriptSerializer();
        string json = sr.Serialize(
            new
            {
                wpGUID = InstanceGUID,
                clientID = pnlInitiatedChat.ClientID,
                contentID = pnlContent.ClientID,
                pnlErrorID = pnlError.ClientID,
                lblErrorID = lblError.ClientID,
                windowURL = ChatHelper.GetChatRoomWindowURL(),
                trans = ChatHelper.GetWebpartTransformation(TransformationName, "chat.error.transformation.initiatedchat.error"),
                guid = optID,
                delay = Delay * 1000,
                initiatorName = InitiatorName,
                messages = MacroResolver.Resolve(Messages).Split('\n')
            }
        );
        string startupScript = string.Format("InitAutoInitiatedChat({0});", json);
        ScriptHelper.RegisterStartupScript(Page, typeof(string), "ChatAutoInitiatedChat_" + ClientID, startupScript, true);
    }
}
