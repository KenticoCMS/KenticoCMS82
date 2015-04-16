<%@ Page Title="" Language="C#" MasterPageFile="~/CMSAPIExamples/Pages/APIExamplesPage.Master"
    Theme="Default" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="CMSAPIExamples_Code_Community_MessageBoards_Default" %>

<%@ Register Src="~/CMSAPIExamples/Controls/APIExample.ascx" TagName="APIExample" TagPrefix="cms" %>
<asp:Content ID="contentLeft" ContentPlaceHolderID="plcLeftContainer" runat="server">
    <%-- Message board --%>
    <cms:LocalizedHeading ID="headCreateMessageBoard" runat="server" Text="Message board" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreateMessageBoard" runat="server" ButtonText="Create board" InfoMessage="Board 'My new board' was created." ErrorMessage="Root page was not found." />
    <cms:APIExample ID="apiGetAndUpdateMessageBoard" runat="server" ButtonText="Get and update board" APIExampleType="ManageAdditional" InfoMessage="Board 'My new board' was updated." ErrorMessage="Board 'My new board' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateMessageBoards" runat="server" ButtonText="Get and bulk update boards" APIExampleType="ManageAdditional" InfoMessage="All boards matching the condition were updated." ErrorMessage="Boards matching the condition were not found." />
    <%-- Message --%>
    <cms:LocalizedHeading ID="headCreateMessage" runat="server" Text="Message" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreateMessage" runat="server" ButtonText="Create message" InfoMessage="Message 'My new message' was created." ErrorMessage="Board 'My new board' was not found." />
    <cms:APIExample ID="apiGetAndUpdateMessage" runat="server" ButtonText="Get and update message" APIExampleType="ManageAdditional" InfoMessage="Message 'My new message' was updated." ErrorMessage="Board 'My new board' or message 'My new message' were not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateMessages" runat="server" ButtonText="Get and bulk update messages" APIExampleType="ManageAdditional" InfoMessage="All messages matching the condition were updated." ErrorMessage="Messages matching the condition were not found." />
    <%-- Message board subscription --%>
    <cms:LocalizedHeading ID="headCreateMessageBoardSubscription" runat="server" Text="Message board subscription" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreateMessageBoardSubscription" runat="server" ButtonText="Create subscription" InfoMessage="Subscription 'My new subscription' was created." ErrorMessage="Board 'My new board' was not found." />
    <cms:APIExample ID="apiGetAndUpdateMessageBoardSubscription" runat="server" ButtonText="Get and update subscription" APIExampleType="ManageAdditional" InfoMessage="Subscription 'My new subscription' was updated." ErrorMessage="Board 'My new board' or subscription 'My new subscription' were not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateMessageBoardSubscriptions" runat="server" ButtonText="Get and bulk update subscriptions" APIExampleType="ManageAdditional" InfoMessage="All subscriptions matching the condition were updated." ErrorMessage="Subscriptions matching the condition were not found." />
    <%-- Role on message board --%>
    <cms:LocalizedHeading ID="headAddRoleToMessageBoard" runat="server" Text="Role on message board" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiAddRoleToMessageBoard" runat="server" ButtonText="Add role to board" InfoMessage="Role 'CMSDeskAdmin' was added to board 'My new board'." ErrorMessage="Board 'My new board' or role 'CMSDeskAdmin' were not found." />
    <%-- Message board moderator --%>
    <cms:LocalizedHeading ID="headAddModeratorToMessageBoard" runat="server" Text="Message board moderator" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiAddModeratorToMessageBoard" runat="server" ButtonText="Add moderator to board" InfoMessage="Moderator was added to board 'My new board'." ErrorMessage="Board 'My new board' was not found." />
</asp:Content>
<asp:Content ID="contentRight" ContentPlaceHolderID="plcRightContainer" runat="server">
    <%-- Message board moderator --%>
    <cms:LocalizedHeading ID="headRemoveModeratorFromMessageBoard" runat="server" Text="Message board moderator" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiRemoveModeratorFromMessageBoard" runat="server" ButtonText="Remove moderator from board" APIExampleType="CleanUpMain" InfoMessage="Moderator was removed from board 'My new board'." ErrorMessage="Board 'My new board' was not found." />
    <%-- Role on message board --%>
    <cms:LocalizedHeading ID="headRemoveRoleFromMessageBoard" runat="server" Text="Role on message board" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiRemoveRoleFromMessageBoard" runat="server" ButtonText="Remove role from board" APIExampleType="CleanUpMain" InfoMessage="Role 'CMSDeskAdmin' was removed from board 'My new board'." ErrorMessage="Board 'My new board' or role 'CMSDeskAdmin' were not found." />
    <%-- Message board subscription --%>
    <cms:LocalizedHeading ID="headDeleteMessageBoardSubscription" runat="server" Text="Message board subscription" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteMessageBoardSubscription" runat="server" ButtonText="Delete subscription" APIExampleType="CleanUpMain" InfoMessage="Subscription 'My new subscription' and all its dependencies were deleted." ErrorMessage="Board 'My new board' or subscription 'My new subscription' was not found." />
    <%-- Message --%>
    <cms:LocalizedHeading ID="headDeleteMessage" runat="server" Text="Message" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteMessage" runat="server" ButtonText="Delete message" APIExampleType="CleanUpMain" InfoMessage="Message 'My new message' and all its dependencies were deleted." ErrorMessage="Board 'My new board' or message 'My new message' was not found." />
    <%-- Message board --%>
    <cms:LocalizedHeading ID="headDeleteMessageBoard" runat="server" Text="Message board" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteMessageBoard" runat="server" ButtonText="Delete board" APIExampleType="CleanUpMain" InfoMessage="Board 'My new board' and all its dependencies were deleted." ErrorMessage="Board 'My new board' was not found." />
</asp:Content>
