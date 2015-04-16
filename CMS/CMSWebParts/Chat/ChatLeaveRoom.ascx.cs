using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.PortalControls;
using CMS.Helpers;
using CMS.UIControls;
using CMS.DocumentEngine;
using CMS.Chat;

public partial class CMSWebParts_Chat_ChatLeaveRoom : CMSAbstractWebPart
{
    #region Properties

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
    /// Gets or sets RedirectURL property.
    /// </summary>
    public string RedirectURL
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("RedirectURL"), "");
        }
        set
        {
            this.SetValue("RedirectURL", value);
        }
    }

    public string InnerContainerName { get; set; }
    public string InnerContainerTitle { get; set; }

    #endregion

    protected void Page_Prerender(object sender, EventArgs e)
    {
        ChatFunctions.MakeWebpartEnvelope("ChatWebpartEnvelope ChatWebpartEnvelopeLeaveRoom", this, InnerContainerTitle, InnerContainerName);
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
        ScriptHelper.RegisterScriptFile(Page, "~/CMSWebParts/Chat/ChatLeaveRoom_files/ChatLeaveRoom.js");

        string redirectURL = RedirectURL.Length > 0 ? RedirectURL : ChatHelper.RedirectURLLeaveSetting;

        // Prepare and run startup script
        string startupScript = String.Format("InitChatLeaveRoom({{groupID:{0}, clientID:'{1}', btnChatLeaveRoom:{2},pnlContent:{3}, redirectURL:{4}, envelopeID: '#envelope_{1}' }});",
            ScriptHelper.GetString(GroupID),
            ClientID,
            ScriptHelper.GetString("#" + btnChatLeaveRoom.ClientID),
            ScriptHelper.GetString('#' + pnlChatLeaveRoom.ClientID),
            redirectURL.Length > 0 ? ScriptHelper.GetString(ChatHelper.GetDocumentAbsoluteUrl(redirectURL)) : "\"\""
            );

        ScriptHelper.RegisterStartupScript(Page, typeof(string), "ChatLeaveRoom_" + ClientID, startupScript, true);
    }
}