<%@ Page Title="" Language="C#" MasterPageFile="~/CMSAPIExamples/Pages/APIExamplesPage.Master"
    Theme="Default" AutoEventWireup="true" Inherits="CMSAPIExamples_Code_Community_Messaging_Default" CodeFile="Default.aspx.cs" %>

<%@ Register Src="~/CMSAPIExamples/Controls/APIExample.ascx" TagName="APIExample" TagPrefix="cms" %>
<asp:Content ID="contentLeft" ContentPlaceHolderID="plcLeftContainer" runat="server">
    <%-- Message --%>
    <cms:LocalizedHeading ID="headCreateMessage" runat="server" Text="Message" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreateMessage" runat="server" ButtonText="Create message" InfoMessage="Message 'API example message' was created." ErrorMessage="Message 'API example message' was not created because sender or recipient doesn't exist." />
    <cms:APIExample ID="apiGetAndUpdateMessage" runat="server" ButtonText="Get and update message" APIExampleType="ManageAdditional" InfoMessage="Message 'API example message' was updated." ErrorMessage="Message 'API example message' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateMessages" runat="server" ButtonText="Get and bulk update messages" APIExampleType="ManageAdditional" InfoMessage="All messages matching the condition were updated." ErrorMessage="Messages matching the condition were not found." />
    <%-- Contact list --%>
    <cms:LocalizedHeading ID="headAddUserToContactList" runat="server" Text="Contact list" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiAddUserToContactList" runat="server" ButtonText="Add user to contact list" InfoMessage="User 'MyNewContact' was successfully added to your contact list. If the user was present in your Ignore list prior to this action, he was removed from it." ErrorMessage="User 'MyNewContact' already is in your contact list." />
    <%-- Ignore list --%>
    <cms:LocalizedHeading ID="headAddUserToIgnoreList" runat="server" Text="Ignore list" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiAddUserToIgnoreList" runat="server" ButtonText="Add user to ignore list" InfoMessage="User 'MyNewIgnoredUser' was successfully added to your ignore list. If the user was present in your Contact list prior to this action, he was removed from it." ErrorMessage="User 'MyNewIgnoredUser' already is in your ignore list." />
</asp:Content>
<asp:Content ID="contentRight" ContentPlaceHolderID="plcRightContainer" runat="server">
    <%-- Message --%>
    <cms:LocalizedHeading ID="headDeleteMessage" runat="server" Text="Message" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteMessage" runat="server" ButtonText="Delete message(s)" APIExampleType="CleanUpMain" InfoMessage="All 'API example message' messages and all their dependencies were deleted." ErrorMessage="No 'API example message' was found." />
    <%-- Contact list --%>
    <cms:LocalizedHeading ID="headRemoveUserFromContactList" runat="server" Text="Contact list" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiRemoveUserFromContactList" runat="server" ButtonText="Remove user from contact list" APIExampleType="CleanUpMain" InfoMessage="User 'MyNewContact' was removed from your contact list." ErrorMessage="User 'MyNewContact' is not present in your contact list." />
    <%-- Ignore list --%>
    <cms:LocalizedHeading ID="headRemoveUserFromIgnoreList" runat="server" Text="Ignore list" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiRemoveUserFromIgnoreList" runat="server" ButtonText="Remove user from ignore list" APIExampleType="CleanUpMain" InfoMessage="User 'MyNewIgnoredUser' was removed from your ignore list." ErrorMessage="User 'MyNewIgnoredUser' is not present in your ignore list." />
</asp:Content>
