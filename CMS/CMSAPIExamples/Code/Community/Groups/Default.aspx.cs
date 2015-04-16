using System;
using System.Data;

using CMS.Community;
using CMS.Helpers;
using CMS.LicenseProvider;
using CMS.Base;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;
using CMS.DataEngine;

public partial class CMSAPIExamples_Code_Community_Groups_Default : CMSAPIExamplePage
{
    #region "Initialization"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Check license
        LicenseHelper.CheckFeatureAndRedirect(RequestContext.CurrentDomain, FeatureEnum.Groups);

        // Group member
        apiCreateGroupMember.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateGroupMember);
        apiGetAndUpdateGroupMember.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndUpdateGroupMember);
        apiGetAndBulkUpdateGroupMembers.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndBulkUpdateGroupMembers);
        apiDeleteGroupMember.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteGroupMember);
        apiCreateInvitation.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateInvitation);

        // Group
        apiCreateGroup.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateGroup);
        apiGetAndUpdateGroup.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndUpdateGroup);
        apiGetAndBulkUpdateGroups.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndBulkUpdateGroups);
        apiDeleteGroup.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteGroup);
    }

    #endregion


    #region "Mass actions"

    /// <summary>
    /// Runs all creating and managing examples.
    /// </summary>
    public override void RunAll()
    {
        base.RunAll();

        // Group
        apiCreateGroup.Run();
        apiGetAndUpdateGroup.Run();
        apiGetAndBulkUpdateGroups.Run();

        // Group member
        apiCreateGroupMember.Run();
        apiGetAndUpdateGroupMember.Run();
        apiGetAndBulkUpdateGroupMembers.Run();
        apiCreateInvitation.Run();
    }


    /// <summary>
    /// Runs all cleanup examples.
    /// </summary>
    public override void CleanUpAll()
    {
        base.CleanUpAll();

        // Group member
        apiDeleteGroupMember.Run();

        // Group
        apiDeleteGroup.Run();
    }

    #endregion


    #region "API examples - Group"

    /// <summary>
    /// Creates group. Called when the "Create group" button is pressed.
    /// </summary>
    private bool CreateGroup()
    {
        // Create new group object
        GroupInfo newGroup = new GroupInfo();

        // Set the properties
        newGroup.GroupDisplayName = "My new group";
        newGroup.GroupName = "MyNewGroup";
        newGroup.GroupSiteID = SiteContext.CurrentSiteID;
        newGroup.GroupDescription = "";
        newGroup.GroupApproveMembers = GroupApproveMembersEnum.AnyoneCanJoin;
        newGroup.GroupAccess = SecurityAccessEnum.AllUsers;
        newGroup.GroupApproved = true;
        newGroup.GroupApprovedByUserID = CurrentUser.UserID;
        newGroup.GroupCreatedByUserID = CurrentUser.UserID;
        newGroup.AllowCreate = SecurityAccessEnum.GroupMembers;
        newGroup.AllowDelete = SecurityAccessEnum.GroupMembers;
        newGroup.AllowModify = SecurityAccessEnum.GroupMembers;
        newGroup.GroupNodeGUID = Guid.Empty;

        // Save the group
        GroupInfoProvider.SetGroupInfo(newGroup);

        return true;
    }


    /// <summary>
    /// Gets and updates group. Called when the "Get and update group" button is pressed.
    /// Expects the CreateGroup method to be run first.
    /// </summary>
    private bool GetAndUpdateGroup()
    {
        // Get the group
        GroupInfo updateGroup = GroupInfoProvider.GetGroupInfo("MyNewGroup", SiteContext.CurrentSiteName);
        if (updateGroup != null)
        {
            // Update the properties
            updateGroup.GroupDisplayName = updateGroup.GroupDisplayName.ToLowerCSafe();

            // Save the changes
            GroupInfoProvider.SetGroupInfo(updateGroup);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates groups. Called when the "Get and bulk update groups" button is pressed.
    /// Expects the CreateGroup method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateGroups()
    {
        // Prepare the parameters
        string where = "GroupName LIKE N'MyNewGroup%'";

        // Get the data
        DataSet groups = GroupInfoProvider.GetGroups(where, null);
        if (!DataHelper.DataSourceIsEmpty(groups))
        {
            // Loop through the individual items
            foreach (DataRow groupDr in groups.Tables[0].Rows)
            {
                // Create object from DataRow
                GroupInfo modifyGroup = new GroupInfo(groupDr);

                // Update the properties
                modifyGroup.GroupDisplayName = modifyGroup.GroupDisplayName.ToUpper();

                // Save the changes
                GroupInfoProvider.SetGroupInfo(modifyGroup);
            }

            return true;
        }

        return false;
    }


    /// <summary>
    /// Deletes group. Called when the "Delete group" button is pressed.
    /// Expects the CreateGroup method to be run first.
    /// </summary>
    private bool DeleteGroup()
    {
        // Get the group
        GroupInfo deleteGroup = GroupInfoProvider.GetGroupInfo("MyNewGroup", SiteContext.CurrentSiteName);

        // Delete the group
        GroupInfoProvider.DeleteGroupInfo(deleteGroup);

        return (deleteGroup != null);
    }

    #endregion


    #region "API examples - Group member"

    /// <summary>
    /// Creates group member. Called when the "Create member" button is pressed.
    /// </summary>
    private bool CreateGroupMember()
    {
        // Get the group 
        GroupInfo group = GroupInfoProvider.GetGroupInfo("MyNewGroup", SiteContext.CurrentSiteName);

        if (group != null)
        {
            // Get the user
            UserInfo user = UserInfoProvider.GetUserInfo(MembershipContext.AuthenticatedUser.UserName);

            if (user != null)
            {
                // Create new group member object
                GroupMemberInfo newMember = new GroupMemberInfo();

                //Set the properties
                newMember.MemberGroupID = group.GroupID;
                newMember.MemberApprovedByUserID = CurrentUser.UserID;
                newMember.MemberApprovedWhen = DateTime.Now;
                newMember.MemberInvitedByUserID = CurrentUser.UserID;
                newMember.MemberUserID = user.UserID;
                newMember.MemberJoined = DateTime.Now;
                newMember.MemberComment = "New member added by API example.";

                // Save the member
                GroupMemberInfoProvider.SetGroupMemberInfo(newMember);
            }

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and updates group member. Called when the "Get and update member" button is pressed.
    /// Expects the CreateGroupMember method to be run first.
    /// </summary>
    private bool GetAndUpdateGroupMember()
    {
        // Get the group 
        GroupInfo group = GroupInfoProvider.GetGroupInfo("MyNewGroup", SiteContext.CurrentSiteName);

        if (group != null)
        {
            // Get the user
            UserInfo user = UserInfoProvider.GetUserInfo(MembershipContext.AuthenticatedUser.UserName);

            if (user != null)
            {
                // Get the group member
                GroupMemberInfo updateMember = GroupMemberInfoProvider.GetGroupMemberInfo(user.UserID, group.GroupID);
                if (updateMember != null)
                {
                    // Update the properties
                    updateMember.MemberComment = updateMember.MemberComment.ToLowerCSafe();

                    // Save the changes
                    GroupMemberInfoProvider.SetGroupMemberInfo(updateMember);

                    return true;
                }
            }
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates group members. Called when the "Get and bulk update members" button is pressed.
    /// Expects the CreateGroupMember method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateGroupMembers()
    {
        // Prepare the parameters
        string where = "MemberUserID =" + MembershipContext.AuthenticatedUser.UserID;

        // Get the data
        DataSet members = GroupMemberInfoProvider.GetGroupMembers(where, null);
        if (!DataHelper.DataSourceIsEmpty(members))
        {
            // Loop through the individual items
            foreach (DataRow memberDr in members.Tables[0].Rows)
            {
                // Create object from DataRow
                GroupMemberInfo modifyMember = new GroupMemberInfo(memberDr);

                // Update the properties
                modifyMember.MemberComment = modifyMember.MemberComment.ToUpper();

                // Save the changes
                GroupMemberInfoProvider.SetGroupMemberInfo(modifyMember);
            }

            return true;
        }

        return false;
    }


    /// <summary>
    /// Deletes group member. Called when the "Delete member" button is pressed.
    /// Expects the CreateGroupMember method to be run first.
    /// </summary>
    private bool DeleteGroupMember()
    {
        // Get the group 
        GroupInfo group = GroupInfoProvider.GetGroupInfo("MyNewGroup", SiteContext.CurrentSiteName);

        if (group != null)
        {
            // Get the user
            UserInfo user = UserInfoProvider.GetUserInfo(MembershipContext.AuthenticatedUser.UserName);

            if (user != null)
            {
                // Get the group member
                GroupMemberInfo deleteMember = GroupMemberInfoProvider.GetGroupMemberInfo(user.UserID, group.GroupID);
                if (deleteMember != null)
                {
                    // Save the changes
                    GroupMemberInfoProvider.DeleteGroupMemberInfo(deleteMember);

                    return true;
                }
            }
        }

        return false;
    }


    /// <summary>
    /// Creates new invitation to group. Called when the "Add member to group" button is pressed.
    /// Expects the CreateGroup method to be run first.
    /// </summary>
    private bool CreateInvitation()
    {
        // Get the group 
        GroupInfo group = GroupInfoProvider.GetGroupInfo("MyNewGroup", SiteContext.CurrentSiteName);

        if (group != null)
        {
            // Create new invitation 
            InvitationInfo newInvitation = new InvitationInfo();

            // Set properties
            newInvitation.InvitationComment = "Invitation created by API example.";
            newInvitation.InvitationGroupID = group.GroupID;
            newInvitation.InvitationUserEmail = "admin@localhost.local";
            newInvitation.InvitedByUserID = MembershipContext.AuthenticatedUser.UserID;
            newInvitation.InvitationCreated = DateTime.Now;
            newInvitation.InvitationValidTo = DateTime.Now.AddDays(1);

            // Save object
            InvitationInfoProvider.SetInvitationInfo(newInvitation);

            return true;
        }

        return false;
    }

    #endregion
}