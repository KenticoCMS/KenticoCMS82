using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;

using CMS.PortalControls;
using CMS.UIControls;
using CMS.Chat;
using CMS.Helpers;
using CMS.Controls;
using CMS.ExtendedControls;
using CMS.DataEngine;
using CMS.Base;
using CMS.DocumentEngine;

public partial class CMSWebParts_Chat_ChatLogin : CMSAbstractWebPart
{
    #region Properties

    /// <summary>
    /// Gets or sets RedirectURLEnter property.
    /// </summary>
    public string RedirectURLLogout
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("RedirectURLLogout"), "");
        }
        set
        {
            this.SetValue("RedirectURLLogout", value);
        }
    }


    /// <summary>
    /// Gets or sets RedirectURLLogout property.
    /// </summary>
    public string RedirectURLEnter
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("RedirectURLEnter"), "");
        }
        set
        {
            this.SetValue("RedirectURLEnter", value);
        }
    }

    public string InnerContainerName { get; set; }
    public string InnerContainerTitle { get; set; }

    #endregion 


    #region "Page Events"

    protected void Page_Prerender(object sender, EventArgs e)
    {
        ChatFunctions.MakeWebpartEnvelope("ChatWebpartEnvelope ChatWebpartEnvelopeLogin", this, InnerContainerTitle, InnerContainerName);
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
        ScriptHelper.RegisterScriptFile(Page, "~/CMSWebParts/Chat/ChatLogin_files/ChatLogin.js");

        ScriptHelper.RegisterStartupScript(Page, typeof(string), "ChatLogin_" + ClientID, BuildStartupScript(), true);
        
    }

    #endregion


    #region "Methods"

    private string BuildStartupScript()
    {
        txtChatUserLoginRelogNickname.WatermarkText = ChatHelper.GuestPrefixSetting;

        string redirectURLLogout = RedirectURLLogout.Length > 0 ? RedirectURLLogout : ChatHelper.RedirectURLLogoutSetting;
        if (redirectURLLogout.Length > 0){
            redirectURLLogout =  ChatHelper.GetDocumentAbsoluteUrl(redirectURLLogout);
        }
        string redirectURLLogin =  RedirectURLEnter.Length > 0 ? RedirectURLEnter : ChatHelper.RedirectURLLoginSetting;
        if (redirectURLLogin.Length > 0)
        {
            redirectURLLogin = ChatHelper.GetDocumentAbsoluteUrl(redirectURLLogin);
        }

        // Set onclick events
        btnChatUserLoggedInChangeNickname.OnClientClick = "ChatManager.Login.DisplayChangeNicknameForm('" + ClientID + "'); return false;";
        btnChatUserChangeNicknameButton.OnClientClick = "ChatManager.Login.ChangeNickname($cmsj('#" + txtChatUserChangeNicknameInput.ClientID + "').val(), '" + ClientID + "'); return false;";
        btnChangeNicknameCancel.OnClientClick = "ChatManager.Login.ChangeNickname(null, null, true); return false;";

        JavaScriptSerializer sr = new JavaScriptSerializer();
        string json = sr.Serialize(
            new
            {
                pnlChatUserLoggedIn = pnlChatUserLoggedIn.ClientID, 
                pnlChatUserChangeNicknameForm = pnlChatUserChangeNicknameForm.ClientID, 
                lblChatUserLoggedInInfoValue = lblChatUserLoggedInInfoValue.ClientID, 
                btnChatUserChangeNicknameButton = btnChatUserChangeNicknameButton.ClientID, 
                pnlChatUserLoginError = pnlChatUserLoginError.ClientID, 
                lblChatUserLoginErrorText = lblChatUserLoginErrorText.ClientID, 
                txtChatUserChangeNicknameInput = txtChatUserChangeNicknameInput.ClientID,  
                clientID = ClientID, 
                pnlChatUserLoginRelog = pnlChatUserLoginRelog.ClientID, 
                btnLogout = btnChatUserLoggedInLogout.ClientID, 
                resStrLogout =ResHelper.GetString("chat.login.resStrLogoutAnonym"), 
                txtChatUserLoginRelogNickname = txtChatUserLoginRelogNickname.ClientID, 
                lblChatUserLoginRelogNickname = lblChatUserLoginRelogNickname.ClientID,
                lblChatUserLoginRelogText = lblChatUserLoginRelogText.ClientID, 
                redirectURLLogout = redirectURLLogout, 
                redirectURLEnter = redirectURLLogin, 
                resStrLogAsAnonym = ResHelper.GetString("chat.login.logasanonym"), 
                resStrLogAsCMS = ResHelper.GetString("chat.login.logascms"), 
                btnChatUserLoginRelog = btnChatUserLoginRelog.ClientID,
                resStrNoService = ResHelper.GetString("chat.servicenotavailable")
            }
        );
        return String.Format("InitChatLogin({0});", json);
        
    }

    #endregion

}
