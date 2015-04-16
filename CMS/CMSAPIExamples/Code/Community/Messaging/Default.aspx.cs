using System;
using System.Data;

using CMS.Helpers;
using CMS.LicenseProvider;
using CMS.Messaging;
using CMS.Base;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;
using CMS.DataEngine;

public partial class CMSAPIExamples_Code_Community_Messaging_Default : CMSAPIExamplePage
{
    #region "Initialization"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Check license
        LicenseHelper.CheckFeatureAndRedirect(RequestContext.CurrentDomain, FeatureEnum.Messaging);

        // Message
        apiCreateMessage.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateMessage);
        apiGetAndUpdateMessage.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndUpdateMessage);
        apiGetAndBulkUpdateMessages.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndBulkUpdateMessages);
        apiDeleteMessage.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteMessage);

        // Contact list
        apiAddUserToContactList.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(AddUserToContactList);
        apiRemoveUserFromContactList.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(RemoveUserFromContactList);

        // Ignore list
        apiAddUserToIgnoreList.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(AddUserToIgnoreList);
        apiRemoveUserFromIgnoreList.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(RemoveUserFromIgnoreList);
    }

    #endregion


    #region "Mass actions"

    /// <summary>
    /// Runs all creating and managing examples.
    /// </summary>
    public override void RunAll()
    {
        base.RunAll();

        // Message
        apiCreateMessage.Run();
        apiGetAndUpdateMessage.Run();
        apiGetAndBulkUpdateMessages.Run();

        // Contact list
        apiAddUserToContactList.Run();

        // Ignore list
        apiAddUserToIgnoreList.Run();
    }


    /// <summary>
    /// Runs all cleanup examples.
    /// </summary>
    public override void CleanUpAll()
    {
        base.CleanUpAll();

        // Message
        apiDeleteMessage.Run();

        // Contact list
        apiRemoveUserFromContactList.Run();

        // Ignore list
        apiRemoveUserFromIgnoreList.Run();
    }

    #endregion


    #region "API examples - Message"

    /// <summary>
    /// Creates a message. Called when the "Create message" button is pressed.
    /// </summary>
    private bool CreateMessage()
    {
        // Create new message object
        MessageInfo newMessage = new MessageInfo();

        // Set the properties
        newMessage.MessageSubject = "API example message";
        newMessage.MessageBody = "Hello! This is a sample message created by Kentico API.";

        // Get sender and recipient of the message
        UserInfo sender = MembershipContext.AuthenticatedUser;
        UserInfo recipient = UserInfoProvider.GetUserInfo("administrator");

        // Check if both sender and recipient exist
        if ((sender != null) && (recipient != null))
        {
            newMessage.MessageSenderUserID = sender.UserID;
            newMessage.MessageSenderNickName = sender.UserNickName;
            newMessage.MessageRecipientUserID = recipient.UserID;
            newMessage.MessageRecipientNickName = recipient.UserNickName;
            newMessage.MessageSent = DateTime.Now;

            // Save the message
            MessageInfoProvider.SetMessageInfo(newMessage);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and updates the message created by the previous method. Called when the "Get and update message" button is pressed.
    /// Expects the CreateMessage method to be run first.
    /// </summary>
    private bool GetAndUpdateMessage()
    {
        // Prepare the parameters
        string where = "[MessageSubject] = 'API example message'";

        // Get the data
        DataSet messages = MessageInfoProvider.GetMessages(where, null, 1, null);
        if (!DataHelper.DataSourceIsEmpty(messages))
        {
            // Get the message from the DataSet
            MessageInfo modifyMessage = new MessageInfo(messages.Tables[0].Rows[0]);

            // Update the properties
            modifyMessage.MessageBody = modifyMessage.MessageBody.ToUpper();

            // Save the changes
            MessageInfoProvider.SetMessageInfo(modifyMessage);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates messages created by multiple executions of the first method. Called when the "Get and bulk update messages" button is pressed.
    /// Expects the CreateMessage method to be run first and multiple times to demonstrate the full functionality.
    /// </summary>
    private bool GetAndBulkUpdateMessages()
    {
        // Prepare the parameters
        string where = "[MessageSubject] = 'API example message'";

        // Get the data
        DataSet messages = MessageInfoProvider.GetMessages(where, null);
        if (!DataHelper.DataSourceIsEmpty(messages))
        {
            // Loop through the individual items
            foreach (DataRow messageDr in messages.Tables[0].Rows)
            {
                // Create object from DataRow
                MessageInfo modifyMessage = new MessageInfo(messageDr);

                // Update the properties
                modifyMessage.MessageBody = modifyMessage.MessageBody.ToLowerCSafe();

                // Save the changes
                MessageInfoProvider.SetMessageInfo(modifyMessage);
            }

            return true;
        }

        return false;
    }


    /// <summary>
    /// Deletes message(s). Called when the "Delete message" button is pressed.
    /// Expects the CreateMessage method to be run first.
    /// </summary>
    private bool DeleteMessage()
    {
        // Prepare the parameters
        string where = "[MessageSubject] = 'API example message'";

        // Get the message
        DataSet messages = MessageInfoProvider.GetMessages(where, null);

        if (!DataHelper.DataSourceIsEmpty(messages))
        {
            foreach (DataRow messageDr in messages.Tables[0].Rows)
            {
                // Create message object from DataRow
                MessageInfo deleteMessage = new MessageInfo(messageDr);

                // Delete the message
                MessageInfoProvider.DeleteMessageInfo(deleteMessage);
            }

            return true;
        }

        return false;
    }

    #endregion


    #region "API examples - Contact list"

    /// <summary>
    /// Adds the sample user to the current user's contact list. Called when the "Add user to contact list" button is pressed.
    /// </summary>
    private bool AddUserToContactList()
    {
        // First create a new user which will be added to the contact list
        UserInfo user = new UserInfo();
        user.UserName = "MyNewContact";
        user.FullName = "My new contact";
        user.UserGUID = Guid.NewGuid();

        UserInfoProvider.SetUserInfo(user);

        if (!ContactListInfoProvider.IsInContactList(MembershipContext.AuthenticatedUser.UserID, user.UserID))
        {
            // Adds "Andy" to the current user's contact list
            ContactListInfoProvider.AddToContactList(MembershipContext.AuthenticatedUser.UserID, user.UserID);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Removes the sample "Andy" user from the current user's contact list. Called when the "Remove user from contact list" button is pressed.
    /// Expects the AddUserToContactList method to be run first.
    /// </summary>
    private bool RemoveUserFromContactList()
    {
        // Gets "Andy" UserInfo object
        UserInfo user = UserInfoProvider.GetUserInfo("MyNewContact");

        if (ContactListInfoProvider.IsInContactList(MembershipContext.AuthenticatedUser.UserID, user.UserID))
        {
            // Removes "Andy" from the current user's contact list
            ContactListInfoProvider.RemoveFromContactList(MembershipContext.AuthenticatedUser.UserID, user.UserID);

            user.Delete();

            return true;
        }

        user.Delete();

        return false;
    }

    #endregion


    #region "API examples - Ignore list"

    /// <summary>
    /// Adds the sample user to the current user's ignore list. Called when the "Add user to ignore list" button is pressed.
    /// </summary>
    private bool AddUserToIgnoreList()
    {
        // First create a new user which will be added to the ignore list
        UserInfo user = new UserInfo();
        user.UserName = "MyNewIgnoredUser";
        user.FullName = "My new ignored user";
        user.UserGUID = Guid.NewGuid();

        UserInfoProvider.SetUserInfo(user);

        if (!IgnoreListInfoProvider.IsInIgnoreList(MembershipContext.AuthenticatedUser.UserID, user.UserID))
        {
            // Adds "Andy" to the current user's ignore list
            IgnoreListInfoProvider.AddToIgnoreList(MembershipContext.AuthenticatedUser.UserID, user.UserID);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Removes the sample "Andy" user from the current user's ignore list. Called when the "Remove user from ignore list" button is pressed.
    /// Expects the AddUserToIgnoreList method to be run first.
    /// </summary>
    private bool RemoveUserFromIgnoreList()
    {
        // Gets "Andy" UserInfo object
        UserInfo user = UserInfoProvider.GetUserInfo("MyNewIgnoredUser");

        if (IgnoreListInfoProvider.IsInIgnoreList(MembershipContext.AuthenticatedUser.UserID, user.UserID))
        {
            // Removes "Andy" from the current user's ignore list
            IgnoreListInfoProvider.RemoveFromIgnoreList(MembershipContext.AuthenticatedUser.UserID, user.UserID);

            user.Delete();

            return true;
        }

        user.Delete();

        return false;
    }

    #endregion
}