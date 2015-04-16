using System;
using System.Linq;

using CMS.DataEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.OnlineMarketing;
using CMS.Base;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.WebAnalytics;


[CheckLicence(FeatureEnum.ContactManagement)]
public partial class CMSAPIExamples_Code_OnlineMarketing_ContactManagement_Default : CMSAPIExamplePage
{
    #region "Initialization"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Contact role
        apiCreateContactRole.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateContactRole);
        apiGetAndUpdateContactRole.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndUpdateContactRole);
        apiGetAndBulkUpdateContactRoles.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndBulkUpdateContactRoles);
        apiDeleteContactRole.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteContactRole);

        // Contact status
        apiCreateContactStatus.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateContactStatus);
        apiGetAndUpdateContactStatus.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndUpdateContactStatus);
        apiGetAndBulkUpdateContactStatuses.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndBulkUpdateContactStatuses);
        apiDeleteContactStatus.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteContactStatus);

        // Account status
        apiCreateAccountStatus.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateAccountStatus);
        apiGetAndUpdateAccountStatus.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndUpdateAccountStatus);
        apiGetAndBulkUpdateAccountStatuses.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndBulkUpdateAccountStatuses);
        apiDeleteAccountStatus.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteAccountStatus);

        // Contact
        apiCreateContact.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateContact);
        apiGetAndUpdateContact.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndUpdateContact);
        apiGetAndBulkUpdateContacts.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndBulkUpdateContacts);
        apiDeleteContact.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteContact);

        // Contact status
        apiAddContactStatusToContact.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(AddContactStatusToContact);
        apiRemoveContactStatusFromContact.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(RemoveContactStatusFromContact);

        // Contact membership
        apiAddMembership.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(AddMembership);
        apiRemoveMembership.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(RemoveMembership);

        // Contact IP address
        apiAddIPAddress.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(AddIPAddress);
        apiRemoveIPAddress.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(RemoveIPAddress);

        // Contact user agent
        apiAddUserAgent.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(AddUserAgent);
        apiRemoveUserAgent.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(RemoveUserAgent);

        // Account
        apiCreateAccount.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateAccount);
        apiGetAndUpdateAccount.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndUpdateAccount);
        apiGetAndBulkUpdateAccounts.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndBulkUpdateAccounts);
        apiDeleteAccount.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteAccount);

        // Account status
        apiAddAccountStatusToAccount.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(AddAccountStatusToAccount);
        apiRemoveAccountStatusFromAccount.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(RemoveAccountStatusFromAccount);

        // Account contacts
        apiAddContactToAccount.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(AddContactToAccount);
        apiRemoveContactFromAccount.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(RemoveContactFromAccount);

        // Contact group
        apiCreateContactGroup.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateContactGroup);
        apiGetAndUpdateContactGroup.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndUpdateContactGroup);
        apiGetAndBulkUpdateContactGroups.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndBulkUpdateContactGroups);
        apiDeleteContactGroup.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteContactGroup);

        // Contact in contact group
        apiAddContactToGroup.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(AddContactToGroup);
        apiRemoveContactFromGroup.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(RemoveContactFromGroup);

        // Account in contact group
        apiAddAccountToGroup.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(AddAccountToGroup);
        apiRemoveAccountFromGroup.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(RemoveAccountFromGroup);

        // Activity
        apiCreateActivity.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateActivity);
        apiGetAndUpdateActivity.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndUpdateActivity);
        apiGetAndBulkUpdateActivities.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndBulkUpdateActivities);
        apiDeleteActivity.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteActivity);
    }

    #endregion


    #region "Mass actions"

    /// <summary>
    /// Runs all creating and managing examples.
    /// </summary>
    public override void RunAll()
    {
        base.RunAll();

        // Contact role
        apiCreateContactRole.Run();
        apiGetAndUpdateContactRole.Run();
        apiGetAndBulkUpdateContactRoles.Run();

        // Contact status
        apiCreateContactStatus.Run();
        apiGetAndUpdateContactStatus.Run();
        apiGetAndBulkUpdateContactStatuses.Run();

        // Account status
        apiCreateAccountStatus.Run();
        apiGetAndUpdateAccountStatus.Run();
        apiGetAndBulkUpdateAccountStatuses.Run();

        // Contact
        apiCreateContact.Run();
        apiGetAndUpdateContact.Run();
        apiGetAndBulkUpdateContacts.Run();

        // Contact with contact status
        apiAddContactStatusToContact.Run();

        // Contact memebership
        apiAddMembership.Run();

        // Contact IP address
        apiAddIPAddress.Run();

        // Contact user agent
        apiAddUserAgent.Run();

        // Account
        apiCreateAccount.Run();
        apiGetAndUpdateAccount.Run();
        apiGetAndBulkUpdateAccounts.Run();

        // Account with account status
        apiAddAccountStatusToAccount.Run();

        // Account with contact
        apiAddContactToAccount.Run();

        // Contact group
        apiCreateContactGroup.Run();
        apiGetAndUpdateContactGroup.Run();
        apiGetAndBulkUpdateContactGroups.Run();

        // Contact in group
        apiAddContactToGroup.Run();

        // Account in group
        apiAddAccountToGroup.Run();

        // Activity
        apiCreateActivity.Run();
        apiGetAndUpdateActivity.Run();
        apiGetAndBulkUpdateActivities.Run();
    }


    /// <summary>
    /// Runs all cleanup examples.
    /// </summary>
    public override void CleanUpAll()
    {
        base.CleanUpAll();

        // Activity
        apiDeleteActivity.Run();

        // Contact group
        apiRemoveContactFromGroup.Run();
        apiRemoveAccountFromGroup.Run();
        apiDeleteContactGroup.Run();

        // Account
        apiRemoveContactFromAccount.Run();
        apiRemoveAccountStatusFromAccount.Run();
        apiDeleteAccount.Run();

        // Contact
        apiRemoveIPAddress.Run();
        apiRemoveUserAgent.Run();
        apiRemoveMembership.Run();
        apiRemoveContactStatusFromContact.Run();
        apiDeleteContact.Run();

        // Account status
        apiDeleteAccountStatus.Run();

        // Contact status
        apiDeleteContactStatus.Run();

        // Contact role
        apiDeleteContactRole.Run();
    }

    #endregion


    #region "API examples - Contact role"

    /// <summary>
    /// Creates contact role. Called when the "Create role" button is pressed.
    /// </summary>
    private bool CreateContactRole()
    {
        // Create new contact role object
        ContactRoleInfo newRole = new ContactRoleInfo()
                                      {
                                          ContactRoleDisplayName = "My new role",
                                          ContactRoleName = "MyNewRole",
                                          ContactRoleSiteID = SiteContext.CurrentSiteID
                                      };

        // Save the contact role
        ContactRoleInfoProvider.SetContactRoleInfo(newRole);

        return true;
    }


    /// <summary>
    /// Gets and updates contact role. Called when the "Get and update role" button is pressed.
    /// Expects the CreateContactRole method to be run first.
    /// </summary>
    private bool GetAndUpdateContactRole()
    {
        // Get the contact role
        ContactRoleInfo updateRole = ContactRoleInfoProvider.GetContactRoleInfo("MyNewRole", SiteContext.CurrentSiteName);
        if (updateRole != null)
        {
            // Update a property
            updateRole.ContactRoleDisplayName = updateRole.ContactRoleDisplayName.ToLowerCSafe();

            // Save the changes
            ContactRoleInfoProvider.SetContactRoleInfo(updateRole);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates contact roles. Called when the "Get and bulk update roles" button is pressed.
    /// Expects the CreateContactRole method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateContactRoles()
    {
        // Get the contact roles dataset
        var roles = ContactRoleInfoProvider.GetContactRoles().Where("ContactRoleName", QueryOperator.Like, "MyNewRole%");

        if (roles.Any())
        {
            foreach (ContactRoleInfo role in roles)
            {
                // Update the properties
                role.ContactRoleDisplayName = role.ContactRoleDisplayName.ToUpper();

                // Save the changes
                ContactRoleInfoProvider.SetContactRoleInfo(role);
            }

            return true;
        }

        return false;
    }


    /// <summary>
    /// Deletes contact role. Called when the "Delete role" button is pressed.
    /// Expects the CreateContactRole method to be run first.
    /// </summary>
    private bool DeleteContactRole()
    {
        // Get the contact role
        ContactRoleInfo deleteRole = ContactRoleInfoProvider.GetContactRoleInfo("MyNewRole", SiteContext.CurrentSiteName);

        if (deleteRole != null)
        {
            // Delete the contact role
            ContactRoleInfoProvider.DeleteContactRoleInfo(deleteRole);

            return true;
        }

        return false;
    }

    #endregion


    #region "API examples - Contact status"

    /// <summary>
    /// Creates contact status. Called when the "Create status" button is pressed.
    /// </summary>
    private bool CreateContactStatus()
    {
        // Create new contact status object
        ContactStatusInfo newStatus = new ContactStatusInfo()
                                          {
                                              ContactStatusDisplayName = "My new status",
                                              ContactStatusName = "MyNewStatus",
                                              ContactStatusSiteID = SiteContext.CurrentSiteID
                                          };

        // Save the contact status
        ContactStatusInfoProvider.SetContactStatusInfo(newStatus);

        return true;
    }


    /// <summary>
    /// Gets and updates contact status. Called when the "Get and update status" button is pressed.
    /// Expects the CreateContactStatus method to be run first.
    /// </summary>
    private bool GetAndUpdateContactStatus()
    {
        // Get the contact status
        ContactStatusInfo updateStatus = ContactStatusInfoProvider.GetContactStatusInfo("MyNewStatus", SiteContext.CurrentSiteName);
        if (updateStatus != null)
        {
            // Update a property
            updateStatus.ContactStatusDisplayName = updateStatus.ContactStatusDisplayName.ToLowerCSafe();

            // Save the changes
            ContactStatusInfoProvider.SetContactStatusInfo(updateStatus);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates contact statuses. Called when the "Get and bulk update statuses" button is pressed.
    /// Expects the CreateContactStatus method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateContactStatuses()
    {
        // Get the contact statuses dataset
        var statuses = ContactStatusInfoProvider.GetContactStatuses().Where("ContactStatusName", QueryOperator.Like, "MyNewStatus%");

        if (statuses.Any())
        {
            foreach (ContactStatusInfo contactStatus in statuses)
            {
                // Update a property
                contactStatus.ContactStatusDisplayName = contactStatus.ContactStatusDisplayName.ToUpper();

                // Save the changes
                ContactStatusInfoProvider.SetContactStatusInfo(contactStatus);
            }

            return true;
        }

        return false;
    }


    /// <summary>
    /// Deletes contact status. Called when the "Delete status" button is pressed.
    /// Expects the CreateContactStatus method to be run first.
    /// </summary>
    private bool DeleteContactStatus()
    {
        // Get the contact status
        ContactStatusInfo deleteStatus = ContactStatusInfoProvider.GetContactStatusInfo("MyNewStatus", SiteContext.CurrentSiteName);

        if (deleteStatus != null)
        {
            // Delete the contact status
            ContactStatusInfoProvider.DeleteContactStatusInfo(deleteStatus);

            return true;
        }

        return false;
    }

    #endregion


    #region "API examples - Account status"

    /// <summary>
    /// Creates account status. Called when the "Create status" button is pressed.
    /// </summary>
    private bool CreateAccountStatus()
    {
        // Create new account status object
        AccountStatusInfo newStatus = new AccountStatusInfo()
                                          {
                                              AccountStatusDisplayName = "My new status",
                                              AccountStatusName = "MyNewStatus",
                                              AccountStatusSiteID = SiteContext.CurrentSiteID
                                          };

        // Save the account status
        AccountStatusInfoProvider.SetAccountStatusInfo(newStatus);

        return true;
    }


    /// <summary>
    /// Gets and updates account status. Called when the "Get and update status" button is pressed.
    /// Expects the CreateAccountStatus method to be run first.
    /// </summary>
    private bool GetAndUpdateAccountStatus()
    {
        // Get the account status
        AccountStatusInfo updateStatus = AccountStatusInfoProvider.GetAccountStatusInfo("MyNewStatus", SiteContext.CurrentSiteName);
        if (updateStatus != null)
        {
            // Update a property
            updateStatus.AccountStatusDisplayName = updateStatus.AccountStatusDisplayName.ToLowerCSafe();

            // Save the changes
            AccountStatusInfoProvider.SetAccountStatusInfo(updateStatus);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates account statuses. Called when the "Get and bulk update statuses" button is pressed.
    /// Expects the CreateAccountStatus method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateAccountStatuses()
    {
        // Get the account status dataset
        var statuses = AccountStatusInfoProvider.GetAccountStatuses().Where("AccountStatusName", QueryOperator.Like, "MyNewStatus%");

        if (statuses.Any())
        {
            foreach (AccountStatusInfo accountStatus in statuses)
            {
                // Update a property
                accountStatus.AccountStatusDisplayName = accountStatus.AccountStatusDisplayName.ToUpper();

                // Save the changes
                AccountStatusInfoProvider.SetAccountStatusInfo(accountStatus);
            }

            return true;
        }

        return false;
    }


    /// <summary>
    /// Deletes account status. Called when the "Delete status" button is pressed.
    /// Expects the CreateAccountStatus method to be run first.
    /// </summary>
    private bool DeleteAccountStatus()
    {
        // Get the account status
        AccountStatusInfo deleteStatus = AccountStatusInfoProvider.GetAccountStatusInfo("MyNewStatus", SiteContext.CurrentSiteName);

        if (deleteStatus != null)
        {
            // Delete the account status
            AccountStatusInfoProvider.DeleteAccountStatusInfo(deleteStatus);

            return true;
        }

        return false;
    }

    #endregion


    #region "API examples - Contact"

    /// <summary>
    /// Creates contact. Called when the "Create contact" button is pressed.
    /// </summary>
    private bool CreateContact()
    {
        // Create new contact object
        ContactInfo newContact = new ContactInfo()
                                     {
                                         ContactLastName = "My New Contact",
                                         ContactFirstName = "My New Firstname",
                                         ContactSiteID = SiteContext.CurrentSiteID,
                                         ContactIsAnonymous = true
                                     };

        // Save the contact
        ContactInfoProvider.SetContactInfo(newContact);

        return true;
    }


    /// <summary>
    /// Gets and updates contact. Called when the "Get and update contact" button is pressed.
    /// Expects the CreateContact method to be run first.
    /// </summary>
    private bool GetAndUpdateContact()
    {
        // Get dataset of contacts
        string where = "ContactLastName LIKE N'My New Contact%'";
        var contacts = ContactInfoProvider.GetContacts().Where(where).TopN(1);

        if (!DataHelper.DataSourceIsEmpty(contacts))
        {
            // Get the contact from dataset
            ContactInfo contact = contacts.First<ContactInfo>();

            // Update a property
            contact.ContactLastName = contact.ContactLastName.ToLowerCSafe();

            // Save the changes
            ContactInfoProvider.SetContactInfo(contact);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates contacts. Called when the "Get and bulk update contacts" button is pressed.
    /// Expects the CreateContact method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateContacts()
    {
        // Get dataset of contacts
        string where = "ContactLastName LIKE N'My New Contact%'";
        var contacts = ContactInfoProvider.GetContacts().Where(where);

        if (!DataHelper.DataSourceIsEmpty(contacts))
        {
            foreach (ContactInfo contact in contacts)
            {
                // Update a property of each contact
                contact.ContactLastName = contact.ContactLastName.ToUpper();

                // And save them
                ContactInfoProvider.SetContactInfo(contact);
            }

            return true;
        }

        return false;
    }


    /// <summary>
    /// Adds contact status to contact. Called when the "Add contact status to contact" button is pressed.
    /// Expects the CreateContact and CreateContactStatus methods to be run first.
    /// </summary>
    private bool AddContactStatusToContact()
    {
        // Get dataset of contacts
        string where = "ContactLastName LIKE N'My New Contact%'";
        var contacts = ContactInfoProvider.GetContacts().Where(where).TopN(1);

        // Get the contact status
        ContactStatusInfo contactStatus = ContactStatusInfoProvider.GetContactStatusInfo("MyNewStatus", SiteContext.CurrentSiteName);

        if (!DataHelper.DataSourceIsEmpty(contacts) && (contactStatus != null))
        {
            // Get the contact from dataset
            ContactInfo contact = contacts.First<ContactInfo>();

            // If relationship doesn't already exist
            if (contact.ContactStatusID != contactStatus.ContactStatusID)
            {
                // Add contact status to contact
                contact.ContactStatusID = contactStatus.ContactStatusID;

                // Save the changes
                ContactInfoProvider.SetContactInfo(contact);

                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// Removes contact status from contact. Called when the "Remove status from contact" button is pressed.
    /// Expects the CreateContact, CreateContactStatus and AddContactStatusToContact methods to be run first.
    /// </summary>
    private bool RemoveContactStatusFromContact()
    {
        // Get dataset of contacts
        string where = "ContactLastName LIKE N'My New Contact%'";
        var contacts = ContactInfoProvider.GetContacts().Where(where).TopN(1);

        // Get the contact status
        ContactStatusInfo contactStatus = ContactStatusInfoProvider.GetContactStatusInfo("MyNewStatus", SiteContext.CurrentSiteName);

        if (!DataHelper.DataSourceIsEmpty(contacts) && (contactStatus != null))
        {
            // Get the contact from dataset
            ContactInfo contact = contacts.First<ContactInfo>();

            // If relationship exists
            if (contact.ContactStatusID == contactStatus.ContactStatusID)
            {
                // Remove the status
                contact.ContactStatusID = 0;

                // Save the changes
                ContactInfoProvider.SetContactInfo(contact);

                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// Adds current user to contact. Called when the "Add membership to contact" button is pressed.
    /// Expects the CreateContact method to be run first.
    /// </summary>
    private bool AddMembership()
    {
        // Get dataset of contacts
        string where = "ContactLastName LIKE N'My New Contact%'";
        var contacts = ContactInfoProvider.GetContacts().Where(where).TopN(1);

        if (!DataHelper.DataSourceIsEmpty(contacts))
        {
            // Get the contact from dataset
            ContactInfo contact = contacts.First<ContactInfo>();

            // Set relationship to user
            ContactMembershipInfoProvider.SetRelationship(
                MembershipContext.AuthenticatedUser.UserID,
                MemberTypeEnum.CmsUser,
                contact.ContactID,
                contact.ContactID,
                false);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Removes current user from contact. Called when the "Remove membership from contact" button is pressed.
    /// Expects the CreateContact and AddMembership methods to be run first.
    /// </summary>
    private bool RemoveMembership()
    {
        // Get dataset of contacts
        string where = "ContactLastName LIKE N'My New Contact%'";
        var contacts = ContactInfoProvider.GetContacts().Where(where).TopN(1);

        if (!DataHelper.DataSourceIsEmpty(contacts))
        {
            // Get the contact from dataset
            ContactInfo contact = contacts.First<ContactInfo>();

            // Get the membership
            ContactMembershipInfo contactMembership = ContactMembershipInfoProvider.GetMembershipInfo(contact.ContactID, contact.ContactID, MembershipContext.AuthenticatedUser.UserID, MemberTypeEnum.CmsUser);

            // Delete the membership
            ContactMembershipInfoProvider.DeleteRelationship(contactMembership.MembershipID);

            return (contactMembership != null);
        }

        return false;
    }


    /// <summary>
    /// Adds IP address to contact. Called when the "Add IP to contact" button is pressed.
    /// Expects the CreateContact method to be run first.
    /// </summary>
    private bool AddIPAddress()
    {
        // Get dataset of contacts
        string where = "ContactLastName LIKE N'My New Contact%'";
        var contacts = ContactInfoProvider.GetContacts().Where(where).TopN(1);

        if (!DataHelper.DataSourceIsEmpty(contacts))
        {
            // Get the contact from dataset
            ContactInfo contact = contacts.First<ContactInfo>();

            // Create new IP address
            IPInfo newIP = new IPInfo()
                               {
                                   IPAddress = "127.0.0.1",
                                   IPOriginalContactID = contact.ContactID,
                                   IPActiveContactID = contact.ContactID
                               };

            // Save the IP info
            IPInfoProvider.SetIPInfo(newIP);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Removes IP address from contact. Called when the "Remove IP from contact" button is pressed.
    /// Expects the CreateContact and AddIPAddress methods to be run first.
    /// </summary>
    private bool RemoveIPAddress()
    {
        // Get dataset of contacts
        string where = "ContactLastName LIKE N'My New Contact%'";
        var contacts = ContactInfoProvider.GetContacts().Where(where).TopN(1);

        if (!DataHelper.DataSourceIsEmpty(contacts))
        {
            // Get the contact from dataset
            ContactInfo contact = contacts.First<ContactInfo>();

            // Get contact's IP
            InfoDataSet<IPInfo> deleteIPs = 
                IPInfoProvider.GetIps()
                .WhereEquals("IPOriginalContactID", contact.ContactID)
                    .And()
                    .WhereEquals("IPAddress", "127.0.0.1")
                .TopN(1)
                .Column("IPID").TypedResult;
 
            if (!DataHelper.DataSourceIsEmpty(deleteIPs))
            {
                // Delete IP
                IPInfoProvider.DeleteIPInfo(deleteIPs.First<IPInfo>());

                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// Adds user agent to contact. Called when the "Add user agent to contact" button is pressed.
    /// Expects the CreateContact method to be run first.
    /// </summary>
    private bool AddUserAgent()
    {
        // Get dataset of contacts
        string where = "ContactLastName LIKE N'My New Contact%'";
        var contacts = ContactInfoProvider.GetContacts().Where(where).TopN(1);

        if (!DataHelper.DataSourceIsEmpty(contacts))
        {
            // Get the contact from dataset
            ContactInfo contact = contacts.First<ContactInfo>();

            // Create new agent info
            UserAgentInfo agentInfo = new UserAgentInfo()
                                          {
                                              UserAgentActiveContactID = contact.ContactID,
                                              UserAgentOriginalContactID = contact.ContactID,
                                              UserAgentString = "My User Agent"
                                          };

            // Save the agent info
            UserAgentInfoProvider.SetUserAgentInfo(agentInfo);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Removes user agent from contact. Called when the "Remove user agent from contact" button is pressed.
    /// Expects the CreateContact and AddUserAgent methods to be run first.
    /// </summary>
    private bool RemoveUserAgent()
    {
        // Get dataset of contacts
        string where = "ContactLastName LIKE N'My New Contact%'";
        var contacts = ContactInfoProvider.GetContacts().Where(where).TopN(1);

        if (!DataHelper.DataSourceIsEmpty(contacts))
        {
            // Get the contact from dataset
            ContactInfo contact = contacts.First<ContactInfo>();

            // Get the user agent info
            InfoDataSet<UserAgentInfo> deleteAgents = 
                UserAgentInfoProvider.GetUserAgents()
                    .WhereEquals("UserAgentOriginalContactID", contact.ContactID)
                        .And()
                        .WhereEquals("UserAgentString", "My User Agent")
                        .TopN(1).TypedResult;

            if (!DataHelper.DataSourceIsEmpty(deleteAgents))
            {
                // Delete the user agent info
                UserAgentInfoProvider.DeleteUserAgentInfo(deleteAgents.First<UserAgentInfo>());

                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// Deletes contact. Called when the "Delete contact" button is pressed.
    /// Expects the CreateContact method to be run first.
    /// </summary>
    private bool DeleteContact()
    {
        // Get dataset of contacts
        string where = "ContactLastName LIKE N'My New Contact%'";
        var contacts = ContactInfoProvider.GetContacts().Where(where).TopN(1);

        if (!DataHelper.DataSourceIsEmpty(contacts))
        {
            // Get the contact from dataset
            ContactInfo contact = contacts.First<ContactInfo>();

            // Delete the contact
            ContactInfoProvider.DeleteContactInfo(contact);

            return true;
        }

        return false;
    }

    #endregion


    #region "API examples - Account"

    /// <summary>
    /// Creates account. Called when the "Create account" button is pressed.
    /// </summary>
    private bool CreateAccount()
    {
        // Create new account object
        AccountInfo newAccount = new AccountInfo()
                                     {
                                         AccountName = "My New Account",
                                         AccountSiteID = SiteContext.CurrentSiteID
                                     };

        // Save the account
        AccountInfoProvider.SetAccountInfo(newAccount);

        return true;
    }


    /// <summary>
    /// Gets and updates account. Called when the "Get and update account" button is pressed.
    /// Expects the CreateAccount method to be run first.
    /// </summary>
    private bool GetAndUpdateAccount()
    {
        // Get the accounts
        var updateAccounts = AccountInfoProvider.GetAccounts().WhereEquals("AccountName", "My New Account");

        if (updateAccounts.Any())
        {
            // Get account
            AccountInfo updateAccount = updateAccounts.First<AccountInfo>();

            // Update a property
            updateAccount.AccountName = updateAccount.AccountName.ToLowerCSafe();

            // And save it
            AccountInfoProvider.SetAccountInfo(updateAccount);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates accounts. Called when the "Get and bulk update accounts" button is pressed.
    /// Expects the CreateAccount method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateAccounts()
    {
        // Get dataset of accounts
        var accounts = AccountInfoProvider.GetAccounts().Where("AccountName", QueryOperator.Like, "My New Account%");

        if (accounts.Any())
        {
            foreach (AccountInfo account in accounts)
            {
                // Update each one's property
                account.AccountName = account.AccountName.ToUpper();

                // And save it
                AccountInfoProvider.SetAccountInfo(account);
            }

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates accounts. Called when the "Get and bulk update accounts" button is pressed.
    /// Expects the CreateAccount method to be run first.
    /// </summary>
    private bool AddAccountStatusToAccount()
    {
        // Get the accounts
        var accounts = AccountInfoProvider.GetAccounts().WhereEquals("AccountName", "My New Account");

        // Get the account status
        AccountStatusInfo accountStatus = AccountStatusInfoProvider.GetAccountStatusInfo("MyNewStatus", SiteContext.CurrentSiteName);

        if ((accounts.Any()) && (accountStatus != null))
        {
            // Get the account
            AccountInfo account = accounts.First();

            // Check that account doesn't have this status 
            if (account.AccountStatusID != accountStatus.AccountStatusID)
            {
                // Set new status
                account.AccountStatusID = accountStatus.AccountStatusID;

                // Save changes to the object
                AccountInfoProvider.SetAccountInfo(account);

                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// Removes account status from account. Called when the "Remove status from account" button is pressed.
    /// Expects the CreateAccount, CreateAccountStatus and AddAccountStatusToAccount methods to be run first.
    /// </summary>
    private bool RemoveAccountStatusFromAccount()
    {
        // Get the accounts
        var accounts = AccountInfoProvider.GetAccounts().WhereEquals("AccountName", "My New Account");

        // Get the account status
        AccountStatusInfo accountStatus = AccountStatusInfoProvider.GetAccountStatusInfo("MyNewStatus", SiteContext.CurrentSiteName);

        if ((accounts.Any()) && (accountStatus != null))
        {
            // Get the account
            AccountInfo account = accounts.First();

            // Check if account has this status set
            if (account.AccountStatusID == accountStatus.AccountStatusID)
            {
                // Remove the status from account
                account.AccountStatusID = 0;

                // Save the object
                AccountInfoProvider.SetAccountInfo(account);

                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// Adds contact under account.
    /// </summary>
    private bool AddContactToAccount()
    {
        // Get dataset of contacts
        string where = "ContactLastName LIKE N'My New Contact%'";
        var contacts = ContactInfoProvider.GetContacts().Where(where).TopN(1);

        // Get dataset of accounts
        var accounts = AccountInfoProvider.GetAccounts().WhereEquals("AccountName", "My New Account");

        // Get the role
        ContactRoleInfo role = ContactRoleInfoProvider.GetContactRoleInfo("MyNewRole", SiteContext.CurrentSiteName);

        if (!DataHelper.DataSourceIsEmpty(contacts) && (accounts.Any()) && (role != null))
        {
            // Get the contact from dataset
            ContactInfo contact = contacts.First<ContactInfo>();

            // Get the account from dataset
            AccountInfo account = accounts.First();

            // Create new account - contact relationship
            AccountContactInfo accountContact = new AccountContactInfo()
                                                    {
                                                        AccountID = account.AccountID,
                                                        ContactID = contact.ContactID,
                                                        ContactRoleID = role.ContactRoleID
                                                    };

            // And save it
            AccountContactInfoProvider.SetAccountContactInfo(accountContact);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Removes contact from account.
    /// </summary>
    private bool RemoveContactFromAccount()
    {
        // Get dataset of contacts
        string where = "ContactLastName LIKE N'My New Contact%'";
        var contacts = ContactInfoProvider.GetContacts().Where(where).TopN(1);

        // Get dataset of accounts
        var accounts = AccountInfoProvider.GetAccounts().WhereEquals("AccountName", "My New Account");

        if (!DataHelper.DataSourceIsEmpty(contacts) && (accounts.Any()))
        {
            // Get the contact from dataset
            ContactInfo contact = contacts.First<ContactInfo>();

            // Get the account from dataset
            AccountInfo account = accounts.First();

            // Find account - contact relationship
            AccountContactInfo accountContact = AccountContactInfoProvider.GetAccountContactInfo(account.AccountID, contact.ContactID);

            if (accountContact != null)
            {
                // Delete the object
                AccountContactInfoProvider.DeleteAccountContactInfo(accountContact);

                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// Deletes account. Called when the "Delete account" button is pressed.
    /// Expects the CreateAccount method to be run first.
    /// </summary>
    private bool DeleteAccount()
    {
        // Get the account
        var deleteAccounts = AccountInfoProvider.GetAccounts().WhereEquals("AccountName", "My New Account");

        if (deleteAccounts.Any())
        {
            // Delete all found accounts
            foreach (var account in deleteAccounts)
            {
                AccountInfoProvider.DeleteAccountInfo(account);
            }

            return true;
        }

        return false;
    }

    #endregion


    #region "API examples - Contact group"

    /// <summary>
    /// Creates contact group. Called when the "Create group" button is pressed.
    /// </summary>
    private bool CreateContactGroup()
    {
        // Create new contact group object
        ContactGroupInfo newGroup = new ContactGroupInfo()
                                        {
                                            ContactGroupDisplayName = "My new group",
                                            ContactGroupName = "MyNewGroup",
                                            ContactGroupSiteID = SiteContext.CurrentSiteID,
                                            ContactGroupDynamicCondition = "{%Contact.ContactLastName.Contains(\"My new\")%}"
                                        };

        // Save the contact group to database
        ContactGroupInfoProvider.SetContactGroupInfo(newGroup);

        return true;
    }


    /// <summary>
    /// Gets and updates contact group. Called when the "Get and update group" button is pressed.
    /// Expects the CreateContactGroup method to be run first.
    /// </summary>
    private bool GetAndUpdateContactGroup()
    {
        // Get the contact group
        ContactGroupInfo updateGroup = ContactGroupInfoProvider.GetContactGroupInfo("MyNewGroup", SiteContext.CurrentSiteName);
        if (updateGroup != null)
        {
            // Update contact group's properties
            updateGroup.ContactGroupDisplayName = updateGroup.ContactGroupDisplayName.ToLowerCSafe();

            // Save the contact group
            ContactGroupInfoProvider.SetContactGroupInfo(updateGroup);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates contact groups. Called when the "Get and bulk update groups" button is pressed.
    /// Expects the CreateContactGroup method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateContactGroups()
    {
        // Get the contact groups
        string where = "ContactGroupName LIKE N'MyNewGroup%'";
        var groups = ContactGroupInfoProvider.GetContactGroups().Where(where);

        if (!DataHelper.DataSourceIsEmpty(groups))
        {
            foreach (ContactGroupInfo group in groups)
            {
                // Update a property
                group.ContactGroupDisplayName = group.ContactGroupDisplayName.ToUpper();

                // Save the contact group
                ContactGroupInfoProvider.SetContactGroupInfo(group);
            }

            return true;
        }

        return false;
    }


    /// <summary>
    /// Adds contact to group. Called when the "Add contact to group" button is pressed.
    /// Exepects the CreateContact and CreateContactGroup methods to be run first.
    /// </summary>
    private bool AddContactToGroup()
    {
        // Get dataset of contacts
        string where = "ContactLastName LIKE N'My New Contact%'";
        var contacts = ContactInfoProvider.GetContacts().Where(where).TopN(1);

        // Get the contact group
        ContactGroupInfo group = ContactGroupInfoProvider.GetContactGroupInfo("MyNewGroup", SiteContext.CurrentSiteName);

        if (!DataHelper.DataSourceIsEmpty(contacts) && (group != null))
        {
            // Get the contact from dataset
            ContactInfo contact = contacts.First<ContactInfo>();

            // Create the contact - contactgroup relationship
            ContactGroupMemberInfo newContactGroupMember = new ContactGroupMemberInfo()
                                                               {
                                                                   ContactGroupMemberContactGroupID = group.ContactGroupID,
                                                                   ContactGroupMemberType = ContactGroupMemberTypeEnum.Contact,
                                                                   ContactGroupMemberRelatedID = contact.ContactID,
                                                                   ContactGroupMemberFromManual = true
                                                               };

            // Save the contact group
            ContactGroupMemberInfoProvider.SetContactGroupMemberInfo(newContactGroupMember);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Removes contact from group. Called when the "Remove contact from group" button is pressed.
    /// Expects the CreateContact, CreateContactGroup and AddContactToGroup methods to be run first.
    /// </summary>
    private bool RemoveContactFromGroup()
    {
        // Get dataset of contacts
        string where = "ContactLastName LIKE N'My New Contact%'";
        var contacts = ContactInfoProvider.GetContacts().Where(where).TopN(1);

        // Get the contact group
        ContactGroupInfo group = ContactGroupInfoProvider.GetContactGroupInfo("MyNewGroup", SiteContext.CurrentSiteName);

        if (!DataHelper.DataSourceIsEmpty(contacts) && (group != null))
        {
            // Get the contact from dataset
            ContactInfo contact = contacts.First<ContactInfo>();

            // Get the contact - contactgroup relationship
            ContactGroupMemberInfo deleteContactGroupMember = ContactGroupMemberInfoProvider.GetContactGroupMemberInfoByData(group.ContactGroupID, contact.ContactID, ContactGroupMemberTypeEnum.Contact);

            if (deleteContactGroupMember != null)
            {
                // Delete the info
                ContactGroupMemberInfoProvider.DeleteContactGroupMemberInfo(deleteContactGroupMember);

                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// Adds account to group. Called when the "Add account to group" button is pressed.
    /// Expects the CreateAccount and CreateGroup methods to be run first.
    /// </summary>
    private bool AddAccountToGroup()
    {
        // Get the accounts
        var accounts = AccountInfoProvider.GetAccounts().WhereEquals("AccountName", "My New Account");

        // Get the contact group
        ContactGroupInfo group = ContactGroupInfoProvider.GetContactGroupInfo("MyNewGroup", SiteContext.CurrentSiteName);

        if ((!DataHelper.DataSourceIsEmpty(accounts)) && (group != null))
        {
            // Get account
            AccountInfo account = accounts.First<AccountInfo>();

            // Create new account - contact group relationship
            ContactGroupMemberInfo newContactGroupMember = new ContactGroupMemberInfo()
                                                               {
                                                                   ContactGroupMemberContactGroupID = group.ContactGroupID,
                                                                   ContactGroupMemberType = ContactGroupMemberTypeEnum.Account,
                                                                   ContactGroupMemberRelatedID = account.AccountID
                                                               };

            // Save the object
            ContactGroupMemberInfoProvider.SetContactGroupMemberInfo(newContactGroupMember);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Removes account from group. Called when the "Remove account from group" button is pressed.
    /// Expects the CreateAccount, CreateGroup and AddContactToGroup methods to be run first.
    /// </summary>
    private bool RemoveAccountFromGroup()
    {
        // Get the accounts
        var accounts = AccountInfoProvider.GetAccounts().WhereEquals("AccountName", "My New Account");

        // Get the contact group
        ContactGroupInfo group = ContactGroupInfoProvider.GetContactGroupInfo("MyNewGroup", SiteContext.CurrentSiteName);

        if ((!DataHelper.DataSourceIsEmpty(accounts)) && (group != null))
        {
            // Get the account
            AccountInfo account = accounts.First<AccountInfo>();

            // Get the account - contactgroup relationship
            ContactGroupMemberInfo deleteContactGroupMember = ContactGroupMemberInfoProvider.GetContactGroupMemberInfoByData(group.ContactGroupID, account.AccountID, ContactGroupMemberTypeEnum.Account);

            if (deleteContactGroupMember != null)
            {
                // Delete the info
                ContactGroupMemberInfoProvider.DeleteContactGroupMemberInfo(deleteContactGroupMember);

                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// Deletes contact group. Called when the "Delete group" button is pressed.
    /// Expects the CreateContactGroup method to be run first.
    /// </summary>
    private bool DeleteContactGroup()
    {
        // Get the contact group
        ContactGroupInfo deleteGroup = ContactGroupInfoProvider.GetContactGroupInfo("MyNewGroup", SiteContext.CurrentSiteName);

        if (deleteGroup != null)
        {
            // Delete the contact group
            ContactGroupInfoProvider.DeleteContactGroupInfo(deleteGroup);

            return true;
        }

        return false;
    }

    #endregion


    #region "API examples - Activity"

    /// <summary>
    /// Creates activity. Called when the "Create activity" button is pressed.
    /// </summary>
    private bool CreateActivity()
    {
        // Get dataset of contacts
        string where = "ContactLastName LIKE N'My New Contact%'";
        var contacts = ContactInfoProvider.GetContacts().Where(where).TopN(1);

        if (!DataHelper.DataSourceIsEmpty(contacts))
        {
            // Get the contact from dataset
            ContactInfo contact = contacts.First();

            // Get an activity type
            var activityType = ActivityTypeInfoProvider.GetActivityTypes().First();

            // Create new activity object
            ActivityInfo newActivity = new ActivityInfo
                                           {
                                               ActivityType = activityType.ActivityTypeName,
                                               ActivityTitle = "My new activity",
                                               ActivitySiteID = SiteContext.CurrentSiteID,
                                               ActivityOriginalContactID = contact.ContactID,
                                               ActivityActiveContactID = contact.ContactID
                                           };

            // Save the activity
            ActivityInfoProvider.SetActivityInfo(newActivity);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and updates activity. Called when the "Get and update activity" button is pressed.
    /// Expects the CreateActivity method to be run first.
    /// </summary>
    private bool GetAndUpdateActivity()
    {
        // Get dataset of contacts
        string where = "ContactLastName LIKE N'My New Contact%'";
        var contacts = ContactInfoProvider.GetContacts().Where(where).TopN(1);

        if (!DataHelper.DataSourceIsEmpty(contacts))
        {
            // Get the contact from dataset
            ContactInfo contact = contacts.First<ContactInfo>();

            // Get all activities associated with user
            var updateActivities = ActivityInfoProvider.GetActivities().WhereEquals("ActivityActiveContactID", contact.ContactID);

            if (updateActivities.Any())
            {
                // Get just the first activity
                ActivityInfo activity = updateActivities.First();

                // Update the activity
                activity.ActivityTitle = activity.ActivityTitle.ToLowerCSafe();

                // Save the activity
                ActivityInfoProvider.SetActivityInfo(activity);

                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates activities. Called when the "Get and bulk update activities" button is pressed.
    /// Expects the CreateActivity method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateActivities()
    {
        // Get dataset of contacts
        string where = "ContactLastName LIKE N'My New Contact%'";
        var contacts = ContactInfoProvider.GetContacts().Where(where).TopN(1);

        if (!DataHelper.DataSourceIsEmpty(contacts))
        {
            // Get the contact from dataset
            ContactInfo contact = contacts.First<ContactInfo>();

            // Get all activities associated with contact
            var updateActivities = ActivityInfoProvider.GetActivities().WhereEquals("ActivityActiveContactID", contact.ContactID);

            if (updateActivities.Any())
            {
                foreach (ActivityInfo activity in updateActivities)
                {
                    // Update activity content
                    activity.ActivityTitle = activity.ActivityTitle.ToUpper();

                    // Save the activity
                    ActivityInfoProvider.SetActivityInfo(activity);
                }

                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// Deletes activity. Called when the "Delete activity" button is pressed.
    /// Expects the CreateActivity method to be run first.
    /// </summary>
    private bool DeleteActivity()
    {
        // Get dataset of contacts
        string where = "ContactLastName LIKE N'My New Contact%'";
        var contacts = ContactInfoProvider.GetContacts().Where(where).TopN(1);

        if (!DataHelper.DataSourceIsEmpty(contacts))
        {
            // Get the contact from dataset
            ContactInfo contact = contacts.First<ContactInfo>();

            // Get all activities associated with contact
            var activities = ActivityInfoProvider.GetActivities().WhereEquals("ActivityOriginalContactID", contact.ContactID);
            if (activities.Any())
            {
                foreach (ActivityInfo activity in activities)
                {
                    // Delete the object
                    ActivityInfoProvider.DeleteActivityInfo(activity);
                }

                return true;
            }
        }

        return false;
    }

    #endregion
}