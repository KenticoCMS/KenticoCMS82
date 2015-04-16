<%@ Page Title="" Language="C#" MasterPageFile="~/CMSAPIExamples/Pages/APIExamplesPage.Master"
    Theme="Default" AutoEventWireup="true" Inherits="CMSAPIExamples_Code_Community_Friends_Default" CodeFile="Default.aspx.cs" %>

<%@ Register Src="~/CMSAPIExamples/Controls/APIExample.ascx" TagName="APIExample" TagPrefix="cms" %>
<asp:Content ID="contentLeft" ContentPlaceHolderID="plcLeftContainer" runat="server">
    <%-- Friend --%>
    <cms:LocalizedHeading ID="headCreateFriend" runat="server" Text="Friend" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiRequestFriendship" runat="server" ButtonText="Request friendship" InfoMessage="Friendship with user 'My new friend' was requested." />
    <cms:APIExample ID="apiApproveFriendship" runat="server" ButtonText="Approve friendship" InfoMessage="Friendship was approved." ErrorMessage="Friend 'My new friend' was not found." />
    <cms:APIExample ID="apiRejectFriendship" runat="server" ButtonText="Reject friendship" InfoMessage="Friendship was rejected." ErrorMessage="Friend 'My new friend' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateFriends" runat="server" ButtonText="Get and bulk update friends" APIExampleType="ManageAdditional" InfoMessage="All friends matching the condition were updated." ErrorMessage="Friends matching the condition were not found." />
</asp:Content>
<asp:Content ID="contentRight" ContentPlaceHolderID="plcRightContainer" runat="server">
    <%-- Friend --%>
    <cms:LocalizedHeading ID="headDeleteFriends" runat="server" Text="Friend" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteFriends" runat="server" ButtonText="Delete friends" APIExampleType="CleanUpMain" InfoMessage="The user 'My new friend' and all her friends were deleted." ErrorMessage="Friend 'My new friend' was not found." />
</asp:Content>
