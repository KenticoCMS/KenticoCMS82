<%@ Page Title="" Language="C#" MasterPageFile="~/CMSAPIExamples/Pages/APIExamplesPage.Master"
    Theme="Default" AutoEventWireup="true" Inherits="CMSAPIExamples_Code_Administration_Membership_Default" CodeFile="Default.aspx.cs" %>

<%@ Register Src="~/CMSAPIExamples/Controls/APIExample.ascx" TagName="APIExample" TagPrefix="cms" %>
<asp:Content ID="contentLeft" ContentPlaceHolderID="plcLeftContainer" runat="server">
    <%-- User --%>
    <cms:LocalizedHeading ID="pnlCreateUser" runat="server" Text="User" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreateUser" runat="server" ButtonText="Create user" InfoMessage="User 'My new user' was created." />
    <cms:APIExample ID="apiGetAndUpdateUser" runat="server" ButtonText="Get and update user" APIExampleType="ManageAdditional" InfoMessage="User 'My new user' was updated." ErrorMessage="User 'My new user' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateUsers" runat="server" ButtonText="Get and bulk update users" APIExampleType="ManageAdditional" InfoMessage="All users matching the condition were updated." ErrorMessage="Users matching the condition were not found." />
    <cms:APIExample ID="apiAuthenticateUser" runat="server" ButtonText="Authenticate user" APIExampleType="ManageAdditional" InfoMessage="User 'My new user' was authenticated." ErrorMessage="User 'My new user' was not found or wasn't authenticated." />
    <%-- User on site --%>
    <cms:LocalizedHeading ID="pnlAddUserToSite" runat="server" Text="User on site" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiAddUserToSite" runat="server" ButtonText="Add user to site" APIExampleType="ManageAdditional" InfoMessage="User 'My new user' was added to site." ErrorMessage="User 'My new user' was not found." />
    <%-- Role --%>
    <cms:LocalizedHeading ID="pnlCreateRole" runat="server" Text="Role" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreateRole" runat="server" ButtonText="Create role" InfoMessage="Role 'My new role' was created." />
    <cms:APIExample ID="apiGetAndUpdateRole" runat="server" ButtonText="Get and update role" APIExampleType="ManageAdditional" InfoMessage="Role 'My new role' was updated." ErrorMessage="Role 'My new role' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateRoles" runat="server" ButtonText="Get and bulk update roles" APIExampleType="ManageAdditional" InfoMessage="All roles matching the condition were updated." ErrorMessage="Roles matching the condition were not found." />
    <%-- User role --%>
    <cms:LocalizedHeading ID="pnlCreateUserRole" runat="server" Text="User in role" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreateUserRole" runat="server" ButtonText="Add user to role" APIExampleType="ManageAdditional" InfoMessage="User was added to role 'My new role'." ErrorMessage="User or role were not found." />
    <%-- Membership --%>
    <cms:LocalizedHeading ID="pnlCreateMembership" runat="server" Text="Membership" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreateMembership" runat="server" ButtonText="Create membership" InfoMessage="Membership 'My new membership' was created." />
    <cms:APIExample ID="apiGetAndUpdateMembership" runat="server" ButtonText="Get and update membership" APIExampleType="ManageAdditional" InfoMessage="Membership 'My new membership' was updated." ErrorMessage="Membership 'My new membership' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateMemberships" runat="server" ButtonText="Get and bulk update memberships" APIExampleType="ManageAdditional" InfoMessage="All memberships matching the condition were updated." ErrorMessage="Memberships matching the condition were not found." />
    <%-- Membership role --%>
    <cms:LocalizedHeading ID="pnlCreateMembershipRole" runat="server" Text="Membership in role" Level="4" EnableViewState="false" />>
        <cms:APIExample ID="apiAddMembershipToRole" runat="server" ButtonText="Add membership to role" APIExampleType="ManageAdditional" InfoMessage="Membership 'My new membership' was added to role 'My new role'." ErrorMessage="Role or membership were not found." />
    <%-- Membership user --%>
    <cms:LocalizedHeading ID="pnlAddMembershipUser" runat="server" Text="Membership in user" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiAddMembershipToUser" runat="server" ButtonText="Add membership to user" APIExampleType="ManageAdditional" InfoMessage="Membership 'My new membership' was added to user 'My new user'." ErrorMessage="User or membership were not found." />
    <%-- Session management --%>
    <cms:LocalizedHeading ID="pnlOnlineUsers" runat="server" Text="Session management - requires 'Monitor on-line users' setting enabled." Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiGetOnlineUsers" runat="server" ButtonText="Get and bulk update on-line users" APIExampleType="ManageAdditional" InfoMessage="All on-line users matching the condition were updated." ErrorMessage="On-line users matching the condition were not found." />
    <cms:APIExample ID="apiIsUserOnline" runat="server" ButtonText="Is user on-line?" APIExampleType="ManageAdditional" InfoMessage="Current user is on-line." ErrorMessage="User is not on-line or 'Monitor on-line users' setting is not enabled." />
    <cms:APIExample ID="apiKickUser" runat="server" ButtonText="Kick user" APIExampleType="ManageAdditional" InfoMessage="User was kicked." ErrorMessage="User was not found." />
</asp:Content>
<asp:Content ID="contentRight" ContentPlaceHolderID="plcRightContainer" runat="server">
    <%-- User role --%>
    <cms:LocalizedHeading ID="pnlDeleteUserRole" runat="server" Text="User in role" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteUserRole" runat="server" ButtonText="Remove user from role" APIExampleType="CleanUpMain" InfoMessage="Role 'My new role' and all its dependencies were deleted." ErrorMessage="Role 'My new role' was not found." />
    <%-- Membership role --%>
    <cms:LocalizedHeading ID="pnlRemoveMembershipFromRole" runat="server" Text="Membership in role" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiRemoveMembershipFromRole" runat="server" ButtonText="Remove membership from role" APIExampleType="CleanUpMain" InfoMessage="Membership 'My new membership' was removed from role 'My new role'." ErrorMessage="Role or membership were not found." />
    <%-- Membership user --%>
    <cms:LocalizedHeading ID="pnlRemoveMembershipFromUser" runat="server" Text="Membership in user" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiRemoveMembershipFromUser" runat="server" ButtonText="Remove membership from user" APIExampleType="CleanUpMain" InfoMessage="Membership 'My new membership' was removed from user 'My new user'." ErrorMessage="User or membership were not found." />
    <%-- Membership --%>
    <cms:LocalizedHeading ID="pnlDeleteMembership" runat="server" Text="Membership" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteMembership" runat="server" ButtonText="Delete membership" APIExampleType="CleanUpMain" InfoMessage="Membership 'My new membership' and all its dependencies were deleted." ErrorMessage="Membership 'My new membership' was not found." />
    <%-- User on site --%>
    <cms:LocalizedHeading ID="pnlRemoveUserFromSite" runat="server" Text="User on site" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiRemoveUserFromSite" runat="server" ButtonText="Remove user from site" APIExampleType="CleanUpMain" InfoMessage="User 'My new user' was removed from site." ErrorMessage="User 'My new user' was not found." />
    <%-- User --%>
    <cms:LocalizedHeading ID="pnlDeleteUser" runat="server" Text="User" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteUser" runat="server" ButtonText="Delete user" APIExampleType="CleanUpMain" InfoMessage="User 'My new user' and all its dependencies were deleted." ErrorMessage="User 'My new user' was not found." />
    <%-- Role --%>
    <cms:LocalizedHeading ID="pnlDeleteRole" runat="server" Text="Role" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteRole" runat="server" ButtonText="Delete role" APIExampleType="CleanUpMain" InfoMessage="Role 'My new role' and all its dependencies were deleted." ErrorMessage="Role 'My new role' was not found." />
</asp:Content>
