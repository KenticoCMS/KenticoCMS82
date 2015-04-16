using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.PortalControls;
using CMS.Helpers;
using CMS.UIControls;
using CMS.Chat;
using CMS.Base;
using System.Web.Script.Serialization;

public partial class CMSWebParts_Chat_ChatRoomName : CMSAbstractWebPart
{
    #region Properties

    /// <summary>
    /// Chat room name transformation
    /// </summary>
    public string ChatRoomNameTransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("ChatRoomNameTransformationName"), ChatHelper.TransformationRoomName);
        }
        set
        {
            this.SetValue("ChatRoomNameTransformationName", value);
        }
    }


    /// <summary>
    /// Group ID
    /// </summary>
    public string GroupID
    {
        get
        {
            return ValidationHelper.GetString(GetValue("GroupID"), "DefaultGroup");
        }
        set
        {
            this.SetValue("GroupID", value);
        }
    }


    /// <summary>
    /// Wheather to display initial title or not
    /// </summary>
    public bool DisplayInitialTitle
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayInitialTitle"), true);
        }
        set
        {
            this.SetValue("DisplayInitialTitle", value);
        }
    }


    /// <summary>
    /// Initial title text
    /// </summary>
    public string InitialTitle
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("InitialTitle"), ResHelper.GetString("chat.roomname.initialtitle"));
        }
        set
        {
            this.SetValue("InitialTitle", value);
        }
    }
    

    public string InnerContainerName { get; set; }
    public string InnerContainerTitle { get; set; }

    #endregion


    protected void Page_Prerender(object sender, EventArgs e)
    {
        ChatFunctions.MakeWebpartEnvelope("ChatWebpartEnvelope ChatWebpartEnvelopeRoomName", this, InnerContainerTitle, InnerContainerName);
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
        ScriptHelper.RegisterScriptFile(Page, "~/CMSWebParts/Chat/ChatRoomName_files/ChatRoomName.js");

        // Run script
        JavaScriptSerializer sr = new JavaScriptSerializer();
        string json = sr.Serialize(
            new
            {
                roomNameTemplate = ChatHelper.GetWebpartTransformation(ChatRoomNameTransformationName,"chat.error.transformation.namewp.error"),
                contentClientID = "#" + pnlChatRoomName.ClientID,
                clientID = ClientID,
                conversationTitle = ResHelper.GetString("chat.title.privateconversation"),
                groupID = GroupID,
                displayInitialTitle = DisplayInitialTitle,
                noRoomTitle = InitialTitle,
                loadingDiv = ChatHelper.GetWebpartLoadingDiv("ChatRoomNameWPLoading", "chat.wploading.roomname"), 
                envelopeID = "#envelope_" + ClientID
            }
        );
        string startupScript = String.Format("InitChatRoomNameWebpart({0});", json);

        ScriptHelper.RegisterStartupScript(Page, typeof(string), "ChatRoomName_" + ClientID, startupScript, true);
    }
}