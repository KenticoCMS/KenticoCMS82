<%@ Page Title="" Language="C#" MasterPageFile="~/CMSAPIExamples/Pages/APIExamplesPage.Master"
    Theme="Default" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="CMSAPIExamples_Code_OnlineMarketing_ContactManagement_Default" %>

<%@ Register Src="~/CMSAPIExamples/Controls/APIExample.ascx" TagName="APIExample"
    TagPrefix="cms" %>
<asp:Content ID="contentLeft" ContentPlaceHolderID="plcLeftContainer" runat="server">
    <%-- headtion: Configuration --%>
    <cms:LocalizedHeading ID="headManConfiguration" runat="server" Text="Configuration" Level="4" EnableViewState="false" />
    <%-- Contact role --%>
    <cms:LocalizedHeading ID="headCreateContactRole" runat="server" Text="Contact role" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiCreateContactRole" runat="server" ButtonText="Create role"
        InfoMessage="Role 'My new role' was created." />
    <cms:APIExample ID="apiGetAndUpdateContactRole" runat="server" ButtonText="Get and update role"
        APIExampleType="ManageAdditional" InfoMessage="Role 'My new role' was updated."
        ErrorMessage="Role 'My new role' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateContactRoles" runat="server" ButtonText="Get and bulk update roles"
        APIExampleType="ManageAdditional" InfoMessage="All roles matching the condition were updated."
        ErrorMessage="Roles matching the condition were not found." />
    <%-- Contact status --%>
    <cms:LocalizedHeading ID="headCreateContactStatus" runat="server" Text="Contact status" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiCreateContactStatus" runat="server" ButtonText="Create status"
        InfoMessage="Status 'My new status' was created." />
    <cms:APIExample ID="apiGetAndUpdateContactStatus" runat="server" ButtonText="Get and update status"
        APIExampleType="ManageAdditional" InfoMessage="Status 'My new status' was updated."
        ErrorMessage="Status 'My new status' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateContactStatuses" runat="server" ButtonText="Get and bulk update statuses"
        APIExampleType="ManageAdditional" InfoMessage="All statuses matching the condition were updated."
        ErrorMessage="Statuses matching the condition were not found." />
    <%-- Account status --%>
    <cms:LocalizedHeading ID="headCreateAccountStatus" runat="server" Text="Account status" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiCreateAccountStatus" runat="server" ButtonText="Create status"
        InfoMessage="Status 'My new status' was created." />
    <cms:APIExample ID="apiGetAndUpdateAccountStatus" runat="server" ButtonText="Get and update status"
        APIExampleType="ManageAdditional" InfoMessage="Status 'My new status' was updated."
        ErrorMessage="Status 'My new status' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateAccountStatuses" runat="server" ButtonText="Get and bulk update statuses"
        APIExampleType="ManageAdditional" InfoMessage="All statuses matching the condition were updated."
        ErrorMessage="Statuses matching the condition were not found." />
    <%-- headtion: Contact Management --%>
    <cms:LocalizedHeading ID="headManContactManagement" runat="server" Text="Contact Management" Level="4" EnableViewState="false" />
    <%-- Contact --%>
    <cms:LocalizedHeading ID="headCreateContact" runat="server" Text="Contact" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiCreateContact" runat="server" ButtonText="Create contact"
        InfoMessage="Contact 'My new contact' was created." />
    <cms:APIExample ID="apiGetAndUpdateContact" runat="server" ButtonText="Get and update contact"
        APIExampleType="ManageAdditional" InfoMessage="Contact 'My new contact' was updated."
        ErrorMessage="Contact 'My new contact' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateContacts" runat="server" ButtonText="Get and bulk update contacts"
        APIExampleType="ManageAdditional" InfoMessage="All contacts matching the condition were updated."
        ErrorMessage="Contacts matching the condition were not found." />
    <%-- Contact status --%>
    <cms:LocalizedHeading ID="headAddContactStatusToContact" runat="server" Text="Contact status" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiAddContactStatusToContact" runat="server" ButtonText="Add status to contact"
        InfoMessage="Status 'My new status' was assigned to contact." ErrorMessage="Contact 'My new contact' or contact status 'My new status' were not found or relationship already exists." />
    <%-- Contact membership --%>
    <cms:LocalizedHeading ID="headAddMembership" runat="server" Text="Contact membership" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiAddMembership" runat="server" ButtonText="Add membership to contact"
        InfoMessage="Current user was assigned to contact." ErrorMessage="Contact 'My new contact' was not found." />
    <%-- Contact IP address--%>
    <cms:LocalizedHeading ID="headAddIPAddress" runat="server" Text="Contact IP address" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiAddIPAddress" runat="server" ButtonText="Add IP to contact"
        InfoMessage="IP address was assigned to contact." ErrorMessage="Contact 'My new contact' was not found." />
    <%-- Contact user agent info--%>
    <cms:LocalizedHeading ID="headAddUserAgent" runat="server" Text="Contact user agent" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiAddUserAgent" runat="server" ButtonText="Add user agent to contact"
        InfoMessage="User agent was assigned to contact." ErrorMessage="Contact 'My new contact' was not found." />
    <%-- Account --%>
    <cms:LocalizedHeading ID="headCreateAccount" runat="server" Text="Account" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiCreateAccount" runat="server" ButtonText="Create account"
        InfoMessage="Account 'My new account' was created." />
    <cms:APIExample ID="apiGetAndUpdateAccount" runat="server" ButtonText="Get and update account"
        APIExampleType="ManageAdditional" InfoMessage="Account 'My new account' was updated."
        ErrorMessage="Account 'My new account' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateAccounts" runat="server" ButtonText="Get and bulk update accounts"
        APIExampleType="ManageAdditional" InfoMessage="All accounts matching the condition were updated."
        ErrorMessage="Accounts matching the condition were not found." />
    <%-- Account status --%>
    <cms:LocalizedHeading ID="headAddAccountStatusToAccount" runat="server" Text="Account status" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiAddAccountStatusToAccount" runat="server" ButtonText="Add status to account"
        InfoMessage="Status 'My new status' was assigned to account." ErrorMessage="Account 'My new contact' or account status 'My new status' were not found or relationship already exists." />
    <%-- Account contacts --%>
    <cms:LocalizedHeading ID="headAddContactToAccount" runat="server" Text="Account contacts" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiAddContactToAccount" runat="server" ButtonText="Add contact to account"
        InfoMessage="Contact 'My new contact' was assigned to account." ErrorMessage="Contact 'My new contact' or account 'My new account' were not found." />
    <%-- headtion: Segmentation --%>
    <cms:LocalizedHeading ID="headManSegmentation" runat="server" Text="Segmentation" Level="4" EnableViewState="false" />
    <%-- Contact group --%>
    <cms:LocalizedHeading ID="headCreateContactGroup" runat="server" Text="Contact group" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiCreateContactGroup" runat="server" ButtonText="Create group"
        InfoMessage="Group 'My new group' was created." />
    <cms:APIExample ID="apiGetAndUpdateContactGroup" runat="server" ButtonText="Get and update group"
        APIExampleType="ManageAdditional" InfoMessage="Group 'My new group' was updated."
        ErrorMessage="Group 'My new group' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateContactGroups" runat="server" ButtonText="Get and bulk update groups"
        APIExampleType="ManageAdditional" InfoMessage="All groups matching the condition were updated."
        ErrorMessage="Groups matching the condition were not found." />
    <%-- Contact in contact group --%>
    <cms:LocalizedHeading ID="headAddContactToGroup" runat="server" Text="Contact in group" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiAddContactToGroup" runat="server" ButtonText="Add contact to group"
        InfoMessage="Contact 'My new contact' was assigned to group." ErrorMessage="Contact 'My new contact' or group 'My new group' were not found." />
    <%-- Account in contact group --%>
    <cms:LocalizedHeading ID="headAddAccountToGroup" runat="server" Text="Account in group" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiAddAccountToGroup" runat="server" ButtonText="Add account to group"
        InfoMessage="Account 'My new account' was assigned to account." ErrorMessage="Account 'My new account' or group 'My new group' were not found." />
    <%-- headtion: Activities --%>
    <cms:LocalizedHeading ID="headManActivities" runat="server" Text="Activities" Level="4" EnableViewState="false" />
    <%-- Activity --%>
    <cms:LocalizedHeading ID="headCreateActivity" runat="server" Text="Activity" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiCreateActivity" runat="server" ButtonText="Create activity"
        InfoMessage="Activity 'My new activity' was created." ErrorMessage="Contact 'My new contact' was not found." />
    <cms:APIExample ID="apiGetAndUpdateActivity" runat="server" ButtonText="Get and update activity"
        APIExampleType="ManageAdditional" InfoMessage="Activity 'My new activity' was updated."
        ErrorMessage="Activity 'My new activity' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateActivities" runat="server" ButtonText="Get and bulk update activities"
        APIExampleType="ManageAdditional" InfoMessage="All activities matching the condition were updated."
        ErrorMessage="Activities matching the condition were not found." />
</asp:Content>
<asp:Content ID="contentRight" ContentPlaceHolderID="plcRightContainer" runat="server">
    <%-- headtion: Activities --%>
    <cms:LocalizedHeading ID="headClearActivities" runat="server" Text="Activities" Level="4" EnableViewState="false" />
    <%-- Activity --%>
    <cms:LocalizedHeading ID="headDeleteActivity" runat="server" Text="Activity" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteActivity" runat="server" ButtonText="Delete activity"
        APIExampleType="CleanUpMain" InfoMessage="Activity 'My new activity' and all its dependencies were deleted."
        ErrorMessage="Activity 'My new activity' was not found." />
    <%-- headtion: Segmentation --%>
    <cms:LocalizedHeading ID="headClearSegmentation" runat="server" Text="Segmentation" Level="4" EnableViewState="false" />
    <%-- Account in contact group --%>
    <cms:LocalizedHeading ID="headRemoveAccountFromGroup" runat="server" Text="Account in group" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiRemoveAccountFromGroup" runat="server" ButtonText="Remove account from group"
        APIExampleType="CleanUpMain" InfoMessage="Account 'My new account' was removed from the group."
        ErrorMessage="Account 'My new account', group 'My new group' or their relationship were not found." />
    <%-- Contact in contact group --%>
    <cms:LocalizedHeading ID="headRemoveContactFromGroup" runat="server" Text="Contact in group" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiRemoveContactFromGroup" runat="server" APIExampleType="CleanUpMain" ButtonText="Remove contact from group"
        InfoMessage="Contact 'My new contact' was removed form the group." ErrorMessage="Contact 'My new contact', group 'My new group' or their relationship were not found." />
    <%-- Contact group --%>
    <cms:LocalizedHeading ID="headDeleteContactGroup" runat="server" Text="Contact group" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteContactGroup" runat="server" APIExampleType="CleanUpMain" ButtonText="Delete group"
        InfoMessage="Group 'My new group' was deleted." ErrorMessage="Group 'My new group' was not found." />
    <%-- headtion: Contact Management --%>
    <cms:LocalizedHeading ID="headClearContactManagement" runat="server" Text="Contact Management" Level="4" EnableViewState="false" />
    <%-- Account contacts --%>
    <cms:LocalizedHeading ID="headRemoveContactFromAccount" runat="server" Text="Account contacts" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiRemoveContactFromAccount" runat="server" APIExampleType="CleanUpMain" ButtonText="Remove contact from account"
        InfoMessage="Contact 'My new contact' was removed from account." ErrorMessage="Contact 'My new contact', account 'My new account' or their relationship were not found." />
    <%-- Account status --%>
    <cms:LocalizedHeading ID="headRemoveAccountStatusFromAccount" runat="server" Text="Account status" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiRemoveAccountStatusFromAccount" runat="server" APIExampleType="CleanUpMain" ButtonText="Remove status from account"
        InfoMessage="Status 'My new status' was removed from account." ErrorMessage="Account 'My new contact', account status 'My new status' or their relationship were not found." />
    <%-- Account --%>
    <cms:LocalizedHeading ID="headDeleteAccount" runat="server" Text="Account" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteAccount" runat="server" ButtonText="Delete account"
        APIExampleType="CleanUpMain" InfoMessage="Account 'My new account' and all its dependencies were deleted."
        ErrorMessage="Account 'My new account' was not found." />
    <%-- Contact IP address--%>
    <cms:LocalizedHeading ID="headRemoveIPAddress" runat="server" Text="Contact IP address" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiRemoveIPAddress" runat="server" APIExampleType="CleanUpMain" ButtonText="Remove IP from contact"
        InfoMessage="IP address was removed from contact." ErrorMessage="Contact 'My new contact' or its IP address were not found." />
    <%-- Contact user agent--%>
    <cms:LocalizedHeading ID="headRemoveUserAgent" runat="server" Text="Contact user agent" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiRemoveUserAgent" runat="server" APIExampleType="CleanUpMain" ButtonText="Remove contact's user agent"
        InfoMessage="User agent was removed from contact." ErrorMessage="Contact 'My new contact' or its user agent address were not found." />
    <%-- Contact membership --%>
    <cms:LocalizedHeading ID="headRemoveMembership" runat="server" Text="Contact membership" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiRemoveMembership" runat="server" APIExampleType="CleanUpMain" ButtonText="Remove contact's membership"
        InfoMessage="Current user was removed from contact." ErrorMessage="Membership, contact or their relationship were not found." />
    <%-- Contact status --%>
    <cms:LocalizedHeading ID="headRemoveContactStatusFromContact" runat="server" Text="Contact status" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiRemoveContactStatusFromContact" APIExampleType="CleanUpMain" runat="server" ButtonText="Remove status from contact"
        InfoMessage="Status 'My new status' was removed from contact." ErrorMessage="Contact 'My new contact', contact status 'My new status' or their relationship were not found." />
    <%-- Contact --%>
    <cms:LocalizedHeading ID="headDeleteContact" runat="server" Text="Contact" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteContact" runat="server" ButtonText="Delete contact"
        APIExampleType="CleanUpMain" InfoMessage="Contact 'My new contact' and all its dependencies were deleted."
        ErrorMessage="Contact 'My new contact' was not found." />
    <%-- headtion: Configuration --%>
    <cms:LocalizedHeading ID="headCleanConfiguration" runat="server" Text="Configuration" Level="4" EnableViewState="false" />
    <%-- Account status --%>
    <cms:LocalizedHeading ID="headDeleteAccountStatus" runat="server" Text="Account status" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteAccountStatus" runat="server" ButtonText="Delete status"
        APIExampleType="CleanUpMain" InfoMessage="Status 'My new status' and all its dependencies were deleted."
        ErrorMessage="Status 'My new status' was not found." />
    <%-- Contact status --%>
    <cms:LocalizedHeading ID="headDeleteContactStatus" runat="server" Text="Contact status" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteContactStatus" runat="server" ButtonText="Delete status"
        APIExampleType="CleanUpMain" InfoMessage="Status 'My new status' and all its dependencies were deleted."
        ErrorMessage="Status 'My new status' was not found." />
    <%-- Contact role --%>
    <cms:LocalizedHeading ID="headDeleteContactRole" runat="server" Text="Contact role" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteContactRole" runat="server" ButtonText="Delete role"
        APIExampleType="CleanUpMain" InfoMessage="Role 'My new role' and all its dependencies were deleted."
        ErrorMessage="Role 'My new role' was not found." />
</asp:Content>
