using System;
using System.Linq;
using System.Text;

using CMS;
using CMS.Base;
using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.Helpers;
using CMS.Membership;
using CMS.Chat;

[assembly: RegisterCustomClass("ChatUserFormExtender", typeof(ChatUserFormExtender))]

/// <summary>
/// Extends UI forms used for chat users with additional abilities.
/// </summary>
public class ChatUserFormExtender : ControlExtender<UIForm>
{
    #region "Public methods"

    /// <summary>
    /// Initializes the extender.
    /// </summary>
    public override void OnInit()
    {
        Control.OnBeforeValidate += OnBeforeValidate;
        Control.OnAfterDataLoad += OnAfterDataLoad;
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Handles loaded data.
    /// </summary>
    private void OnAfterDataLoad(object sender, EventArgs e)
    {
        ChatUserInfo user = Control.EditedObject as ChatUserInfo;

        // Anonymous users cannot be edited
        if (!Control.IsInsertMode && (user.ChatUserID > 0) && (user.IsAnonymous || !user.ChatUserUserID.HasValue))
        {
            ShowErrorAndStopProcessing("chat.user.cannoteditanonymoususer");
            Control.Enabled = false;
            Control.SubmitButton.Enabled = false;

            return;
        }
    }


    /// <summary>
    /// Validates form.
    /// </summary>
    private void OnBeforeValidate(object sender, EventArgs e)
    {
        int selectedCMSUserId = ValidationHelper.GetInteger(Control.GetFieldValue("ChatUserUserID"), 0);
        UserInfo user = UserInfoProvider.GetUserInfo(selectedCMSUserId);
        string nickname = Control.GetFieldValue("ChatUserNickname") as string;

        // Validate form - user and nickname fields must be filled
        if ((user == null) || (String.IsNullOrWhiteSpace(nickname)))
        {
            ShowErrorAndStopProcessing("chat.user.erroridnickname");
                
            return;
        }

        if (user.IsPublic())
        {
            ShowErrorAndStopProcessing("chat.cantassociatechatusertopublic");
                
            return;
        }

        // Creating a new object
        if (Control.IsInsertMode)
        {
            // Check if userID is unique in ChatUser table if adding a new user
            if (ChatUserInfoProvider.GetChatUserByUserID(selectedCMSUserId) != null)
            {
                ShowErrorAndStopProcessing("chat.user.erroridunique");
                    
                return;
            }
        }

        // Check nickname only if text has been changed
        ChatUserInfo editedChatUser = Control.EditedObject as ChatUserInfo;
        if (Control.IsInsertMode || (nickname != editedChatUser.ChatUserNickname))
        {
            try
            {
                ChatUserHelper.VerifyNicknameIsValid(ref nickname);
            }
            catch (ChatServiceException ex)
            {
                ShowErrorAndStopProcessing(ex.StatusMessage);
                    
                return;
            }


            // Check if Nickname is unique in registered users
            if (!ChatUserHelper.IsNicknameAvailable(nickname))
            {
                ShowErrorAndStopProcessing("chat.user.errornickunique");
                    
                return;
            }
        }
    }


    /// <summary>
    /// Shows error and sets StopProcessing flag of UIFormControl to true.
    /// 
    /// Error is passed as resource string, which is resolved before displaying.
    /// </summary>
    /// <param name="resourceString">Error resource string</param>
    private void ShowErrorAndStopProcessing(string resourceString)
    {
        Control.ShowError(Control.GetString(resourceString));
        Control.StopProcessing = true;
    }

    #endregion

}

