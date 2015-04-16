using System;

using CMS.FormControls;
using CMS.Helpers;
using CMS.Membership;
using CMS.UIControls;
using CMS.Chat;

public partial class CMSModules_Chat_Controls_UI_ChatSupportCannedResponse_Edit : CMSAdminEditControl
{
    #region "Properties"

    private int siteID = QueryHelper.GetInteger("siteid", 0);

    /// <summary>
    /// SiteID of a new canned response taken from query string.
    /// 
    /// NULL means global.
    /// </summary>
    public int? SiteID
    {
        get
        {
            // Global
            if (siteID <= 0)
            {
                return null;
            }

            return siteID;
        }
    }

    /// <summary>
    /// Returns edited object info.
    /// </summary>
    public ChatSupportCannedResponseInfo TypedEditedObject
    {
        get
        {
            return (ChatSupportCannedResponseInfo)UIContext.EditedObject;
        }
    }


    public UIForm EditForm
    {
        get
        {
            return editForm;
        }
    }


    /// <summary>
    /// Indicates if canned response is personal.
    /// </summary>
    public bool Personal { get; set; }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        EditForm.OnBeforeSave += UIFormControl_OnBeforeSave;
        EditForm.OnCheckPermissions += UIFormControl_OnCheckPermissions;


    }


    void UIFormControl_OnCheckPermissions(object sender, EventArgs e)
    {
        if (Personal)
        {
            var currentUser = MembershipContext.AuthenticatedUser;

            if (currentUser == null || !currentUser.IsAuthorizedPerResource("CMS.Chat", "EnterSupport"))
            {
                RedirectToAccessDenied("CMS.Chat", "EnterSupport");
            }

            // This should be always false, but prevents exceptions
            if (TypedEditedObject != null)
            {
                // The chat user ID is inserted after permission check in OnBeforeSave. If the ID has no value, there is no point in checking it because it will be filled in aftewards
                if (TypedEditedObject.ChatSupportCannedResponseChatUserID.HasValue && TypedEditedObject.ChatSupportCannedResponseChatUserID.Value != ChatUserHelper.GetChatUserFromCMSUser().ChatUserID)
                {
                    RedirectToAccessDenied(GetString("chat.error.cannedresponsenotyours"));
                }
            }
        }
        else
        {
            // Editing existing canned response or create a new one
            int? siteIDToCheck = ((TypedEditedObject != null) && (TypedEditedObject.ChatSupportCannedResponseID > 0)) ? TypedEditedObject.ChatSupportCannedResponseSiteID : SiteID;

            ((CMSChatPage)Page).CheckModifyPermission(siteIDToCheck);
        }
    }


    void UIFormControl_OnBeforeSave(object sender, EventArgs e)
    {
        // If creating a new canned response
        if (TypedEditedObject == null || TypedEditedObject.ChatSupportCannedResponseID <= 0)
        {
            if (!Personal)
            {
                EditForm.Data["ChatSupportCannedResponseChatUserID"] = null;
                EditForm.Data["ChatSupportCannedResponseSiteID"] = SiteID;
            }
            else
            {
                EditForm.Data["ChatSupportCannedResponseChatUserID"] = ChatUserHelper.GetChatUserFromCMSUser().ChatUserID;
                EditForm.Data["ChatSupportCannedResponseSiteID"] = null;
            }
        }
    }

    #endregion
}