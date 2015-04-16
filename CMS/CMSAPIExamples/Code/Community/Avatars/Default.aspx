<%@ Page Title="" Language="C#" MasterPageFile="~/CMSAPIExamples/Pages/APIExamplesPage.Master"
    Theme="Default" AutoEventWireup="true" Inherits="CMSAPIExamples_Code_Community_Avatars_Default" CodeFile="Default.aspx.cs" %>

<%@ Register Src="~/CMSAPIExamples/Controls/APIExample.ascx" TagName="APIExample" TagPrefix="cms" %>
<asp:Content ID="contentLeft" ContentPlaceHolderID="plcLeftContainer" runat="server">
    <%-- Avatar --%>
    <cms:LocalizedHeading ID="headCreateAvatar" runat="server" Text="Avatar" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreateAvatar" runat="server" ButtonText="Create avatar" InfoMessage="Avatar 'MyNewAvatar' was created." />
    <cms:APIExample ID="apiGetAndUpdateAvatar" runat="server" ButtonText="Get and update avatar" APIExampleType="ManageAdditional" InfoMessage="Avatar 'MyNewAvatar' was updated." ErrorMessage="Avatar 'MyNewAvatar' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateAvatars" runat="server" ButtonText="Get and bulk update avatars" APIExampleType="ManageAdditional" InfoMessage="All avatars matching the condition were updated." ErrorMessage="Avatars matching the condition were not found." />
    <cms:LocalizedHeading ID="headUsersAvatar" runat="server" Text="Avatar on user" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiAddAvatarToUser" runat="server" ButtonText="Add avatar to user" APIExampleType="ManageAdditional" InfoMessage="Avatar 'MyNewAvatar' was added to current user." ErrorMessage="Avatar 'MyNewAvatar' was not found." />
</asp:Content>
<asp:Content ID="contentRight" ContentPlaceHolderID="plcRightContainer" runat="server">
    <%-- Avatar on user--%>
    <cms:LocalizedHeading ID="headDelUserAvatar" runat="server" Text="Avatar on user" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiRemoveAvatarFromUser" runat="server" ButtonText="Remove avatar from user" APIExampleType="CleanUpMain" InfoMessage="Avatar 'MyNewAvatar' was removed from current user." ErrorMessage="Avatar 'MyNewAvatar' was not found." />
    <%-- Avatar --%>
    <cms:LocalizedHeading ID="headDeleteAvatar" runat="server" Text="Avatar" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteAvatar" runat="server" ButtonText="Delete avatar" APIExampleType="CleanUpMain" InfoMessage="Avatar 'MyNewAvatar' and all its dependencies were deleted." ErrorMessage="Avatar 'MyNewAvatar' was not found." />
</asp:Content>
