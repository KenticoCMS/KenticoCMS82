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

public partial class CMSWebParts_Chat_ChatErrors : CMSAbstractWebPart
{
    bool mIsSupport = false;

    #region Properties

    /// <summary>
    /// Gets or sets ItemTemplate property.
    /// </summary>
    public string ErrorTransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(this.GetValue("ErrorTransformationName"), ChatHelper.TransformationErrors);
        }
        set
        {
            this.SetValue("ErrorTransformationName", value);
        }
    }


    /// <summary>
    /// Gets or sets ButtonTemplate property.
    /// </summary>
    public string ButtonDeleteAllTransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(this.GetValue("ButtonDeleteAllTransformationName"), ChatHelper.TransformationErrorsDeleteAll);
        }
        set
        {
            this.SetValue("ButtonDeleteAllTransformationName", value);
        }
    }


    /// <summary>
    /// Gets or sets ShowButton property.
    /// </summary>
    public bool ShowDeleteAllBtn
    {
        get
        {
            return ValidationHelper.GetBoolean(this.GetValue("ShowDeleteAllBtn"), false);
        }
        set
        {
            this.SetValue("ShowDeleteAllBtn", value);
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


    protected void Page_Prerender(object sender, EventArgs e)
    {
        ChatFunctions.MakeWebpartEnvelope("ChatWebpartEnvelope ChatWebpartEnvelopeErrors", this, InnerContainerTitle, InnerContainerName);
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
        // Registration to chat webservice
        AbstractCMSPage cmsPage = Page as AbstractCMSPage;
        if (cmsPage != null)
        {
            ChatHelper.RegisterChatAJAXProxy(cmsPage);
        }

        // Script references insertion
        ChatHelper.RegisterChatManager(Page);
        ScriptHelper.RegisterJQueryTemplates(Page);
        ScriptHelper.RegisterScriptFile(Page, "~/CMSWebParts/Chat/ChatErrors_files/ChatErrors.js");

        // Run script
        string startupScript = String.Format("InitErrorsWebpart({{errorTemplate:{0},contentClientID:{1}, clientID:'{2}', showDeleteAll:{3}, envelopeID: '#envelope_{2}' }});",
            ScriptHelper.GetString(ChatHelper.GetWebpartTransformation(ErrorTransformationName,"chat.error.transformation.errorwp.error")), 
            ScriptHelper.GetString("#" + pnlChatErrors.ClientID), 
            ClientID,
            ScriptHelper.GetString( ShowDeleteAllBtn ? ChatHelper.GetWebpartTransformation(ButtonDeleteAllTransformationName,"chat.error.transformation.errorwp.deleteallbtn") : "")
            );

        ScriptHelper.RegisterStartupScript(Page, typeof(string), "ChatErrors_" + ClientID, startupScript, true);
    }

}
