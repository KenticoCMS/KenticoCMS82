using System;
using System.Data;

using CMS.Helpers;
using CMS.Base;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;

public partial class CMSAPIExamples_Code_Administration_Badges_Default : CMSAPIExamplePage
{
    #region "Initialization"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Badge
        apiCreateBadge.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateBadge);
        apiGetAndUpdateBadge.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndUpdateBadge);
        apiGetAndBulkUpdateBadges.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndBulkUpdateBadges);
        apiDeleteBadge.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteBadge);
        apiAddBadgeToUser.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(AddBadgeToUser);
        apiUpdateActivityPoints.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(UpdateActivityPoints);
        apiRemoveBadgeFromUser.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(RemoveBadgeFromUser);
    }

    #endregion


    #region "Mass actions"

    /// <summary>
    /// Runs all creating and managing examples.
    /// </summary>
    public override void RunAll()
    {
        base.RunAll();

        // Badge
        apiCreateBadge.Run();
        apiGetAndUpdateBadge.Run();
        apiGetAndBulkUpdateBadges.Run();

        // Badge on user
        apiAddBadgeToUser.Run();
        apiUpdateActivityPoints.Run();
    }


    /// <summary>
    /// Runs all cleanup examples.
    /// </summary>
    public override void CleanUpAll()
    {
        base.CleanUpAll();

        // Badge on user
        apiRemoveBadgeFromUser.Run();
        // Badge
        apiDeleteBadge.Run();
    }

    #endregion


    #region "API examples - Badge"

    /// <summary>
    /// Creates badge. Called when the "Create badge" button is pressed.
    /// </summary>
    private bool CreateBadge()
    {
        // Create new badge object
        BadgeInfo newBadge = new BadgeInfo();

        // Set the properties
        newBadge.BadgeDisplayName = "My new badge";
        newBadge.BadgeName = "MyNewBadge";
        newBadge.BadgeTopLimit = 50;
        newBadge.BadgeImageURL = GetImageUrl("Objects/CMS_Badge/Default/siteadmin.gif");
        newBadge.BadgeIsAutomatic = true;

        // Save the badge
        BadgeInfoProvider.SetBadgeInfo(newBadge);

        return true;
    }


    /// <summary>
    /// Gets and updates badge. Called when the "Get and update badge" button is pressed.
    /// Expects the CreateBadge method to be run first.
    /// </summary>
    private bool GetAndUpdateBadge()
    {
        // Get the badge
        BadgeInfo updateBadge = BadgeInfoProvider.GetBadgeInfo("MyNewBadge");
        if (updateBadge != null)
        {
            // Update the properties
            updateBadge.BadgeDisplayName = updateBadge.BadgeDisplayName.ToLowerCSafe();

            // Save the changes
            BadgeInfoProvider.SetBadgeInfo(updateBadge);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates badges. Called when the "Get and bulk update badges" button is pressed.
    /// Expects the CreateBadge method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateBadges()
    {
        // Prepare the parameters
        string where = "BadgeName LIKE N'MyNewBadge%'";

        // Get the data from database
        var badges = BadgeInfoProvider.GetBadges().Where(where);

        // Loop through the individual items
        foreach (BadgeInfo modifyBadge in badges)
        {
            // Update the properties
            modifyBadge.BadgeDisplayName = modifyBadge.BadgeDisplayName.ToUpper();

            // Save the changes
            BadgeInfoProvider.SetBadgeInfo(modifyBadge);
        }

        // Return TRUE if any object was found and updated, FALSE otherwise
        return (badges.Count > 0);
    }


    /// <summary>
    /// Deletes badge. Called when the "Delete badge" button is pressed.
    /// Expects the CreateBadge method to be run first.
    /// </summary>
    private bool DeleteBadge()
    {
        // Get the badge
        BadgeInfo deleteBadge = BadgeInfoProvider.GetBadgeInfo("MyNewBadge");

        // Delete the badge
        BadgeInfoProvider.DeleteBadgeInfo(deleteBadge);

        return (deleteBadge != null);
    }


    /// <summary>
    /// Adds badge to user. Called when the "Add badge to user" button is pressed.
    /// Expects the CreateBadge method to be run first.
    /// </summary>
    private bool AddBadgeToUser()
    {
        // Get user object
        UserInfo user = UserInfoProvider.GetUserInfo(MembershipContext.AuthenticatedUser.UserName);

        // Get badge object
        BadgeInfo myBadge = BadgeInfoProvider.GetBadgeInfo("MyNewBadge");

        if ((user != null) && (myBadge != null))
        {
            // Add badge to user settings
            user.UserSettings.UserBadgeID = myBadge.BadgeID;

            // Update user object in database
            UserInfoProvider.SetUserInfo(user);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Updates user's activity points. Called when the "Update user's activity points" button is pressed.
    /// </summary>
    private bool UpdateActivityPoints()
    {
        // Get user
        UserInfo user = UserInfoProvider.GetUserInfo(MembershipContext.AuthenticatedUser.UserName);

        // If user exists
        if (user != null)
        {
            // Update activity points to user
            BadgeInfoProvider.UpdateActivityPointsToUser(ActivityPointsEnum.BlogCommentPost, user.UserID, SiteContext.CurrentSiteName, true);

            return true;
        }

        return false;
    }


    private bool RemoveBadgeFromUser()
    {
        // Get user
        UserInfo user = UserInfoProvider.GetUserInfo(MembershipContext.AuthenticatedUser.UserName);

        // If user exists
        if (user != null)
        {
            user.UserSettings.UserBadgeID = 0;

            // Save updates
            UserInfoProvider.SetUserInfo(user);

            return true;
        }

        return false;
    }

    #endregion
}