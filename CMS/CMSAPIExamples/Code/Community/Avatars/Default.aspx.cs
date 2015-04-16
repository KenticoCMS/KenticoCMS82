using System;
using System.Data;

using CMS.Helpers;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;

public partial class CMSAPIExamples_Code_Community_Avatars_Default : CMSAPIExamplePage
{
    #region "Initialization"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Avatar
        apiCreateAvatar.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateAvatar);
        apiGetAndUpdateAvatar.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndUpdateAvatar);
        apiGetAndBulkUpdateAvatars.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndBulkUpdateAvatars);
        apiDeleteAvatar.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteAvatar);

        // Avatar on user
        apiAddAvatarToUser.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(AddAvatarToUser);
        apiRemoveAvatarFromUser.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(RemoveAvatarFromUser);
    }

    #endregion


    #region "Mass actions"

    /// <summary>
    /// Runs all creating and managing examples.
    /// </summary>
    public override void RunAll()
    {
        base.RunAll();

        // Avatar
        apiCreateAvatar.Run();
        apiGetAndUpdateAvatar.Run();
        apiGetAndBulkUpdateAvatars.Run();

        // Avatar on user
        apiAddAvatarToUser.Run();
    }


    /// <summary>
    /// Runs all cleanup examples.
    /// </summary>
    public override void CleanUpAll()
    {
        base.CleanUpAll();

        // Avatar on user
        apiRemoveAvatarFromUser.Run();

        // Avatar
        apiDeleteAvatar.Run();
    }

    #endregion


    #region "API examples - Avatar"

    /// <summary>
    /// Creates avatar. Called when the "Create avatar" button is pressed.
    /// </summary>
    private bool CreateAvatar()
    {
        // Create new avatar object
        AvatarInfo newAvatar = new AvatarInfo(Server.MapPath("~\\CMSAPIExamples\\Code\\Community\\Avatars\\Files\\avatar_man.jpg"));

        // Set the properties
        newAvatar.AvatarName = "MyNewAvatar";
        newAvatar.AvatarType = AvatarInfoProvider.GetAvatarTypeString(AvatarTypeEnum.All);
        newAvatar.AvatarIsCustom = false;

        // Save the avatar
        AvatarInfoProvider.SetAvatarInfo(newAvatar);

        return true;
    }


    /// <summary>
    /// Gets and updates avatar. Called when the "Get and update avatar" button is pressed.
    /// Expects the CreateAvatar method to be run first.
    /// </summary>
    private bool GetAndUpdateAvatar()
    {
        // Get the avatar
        AvatarInfo updateAvatar = AvatarInfoProvider.GetAvatarInfo("MyNewAvatar");
        if (updateAvatar != null)
        {
            // Update the properties
            updateAvatar.AvatarName = updateAvatar.AvatarName.ToLower();

            // Save the changes
            AvatarInfoProvider.SetAvatarInfo(updateAvatar);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates avatars. Called when the "Get and bulk update avatars" button is pressed.
    /// Expects the CreateAvatar method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateAvatars()
    {
        // Prepare the parameters
        string where = "AvatarName LIKE N'MyNewAvatar%'";

        // Get the data from database
        var avatars = AvatarInfoProvider.GetAvatars().Where(where);

        // Loop through the individual items
        foreach (AvatarInfo modifyAvatar in avatars)
        {
            // Update the properties
            modifyAvatar.AvatarName = modifyAvatar.AvatarName.ToUpper();

            // Save the changes
            AvatarInfoProvider.SetAvatarInfo(modifyAvatar);
        }

        // Return TRUE if any object was found and updated, FALSE otherwise
        return (avatars.Count > 0);
    }


    /// <summary>
    /// Deletes avatar. Called when the "Delete avatar" button is pressed.
    /// Expects the CreateAvatar method to be run first.
    /// </summary>
    private bool DeleteAvatar()
    {
        // Get the avatar
        AvatarInfo deleteAvatar = AvatarInfoProvider.GetAvatarInfo("MyNewAvatar");

        // Delete the avatar
        AvatarInfoProvider.DeleteAvatarInfo(deleteAvatar);

        return (deleteAvatar != null);
    }

    #endregion


    #region "Avatar on user"

    /// <summary>
    /// Adds avatar to user. Called when the "Add avatar to user" button is pressed.
    /// Expects the CreateAvatar method to be run first.
    /// </summary>
    private bool AddAvatarToUser()
    {
        // Get the avatar and user
        AvatarInfo avatar = AvatarInfoProvider.GetAvatarInfo("MyNewAvatar");
        UserInfo user = UserInfoProvider.GetUserInfo(MembershipContext.AuthenticatedUser.UserName);

        if ((avatar != null) && (user != null))
        {
            user.UserAvatarID = avatar.AvatarID;

            // Save edited object
            UserInfoProvider.SetUserInfo(user);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Removes avatar from user. Called when the "Remove avatar from user" button is pressed.
    /// </summary>
    private bool RemoveAvatarFromUser()
    {
        // Get the user
        UserInfo user = UserInfoProvider.GetUserInfo(MembershipContext.AuthenticatedUser.UserName);

        if (user != null)
        {
            user.UserAvatarID = 0;

            // Save edited object
            UserInfoProvider.SetUserInfo(user);

            return true;
        }

        return false;
    }

    #endregion
}