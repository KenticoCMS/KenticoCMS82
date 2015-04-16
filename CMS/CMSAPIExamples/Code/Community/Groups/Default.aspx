<%@ Page Title="" Language="C#" MasterPageFile="~/CMSAPIExamples/Pages/APIExamplesPage.Master"
    Theme="Default" AutoEventWireup="true" Inherits="CMSAPIExamples_Code_Community_Groups_Default" CodeFile="Default.aspx.cs" %>

<%@ Register Src="~/CMSAPIExamples/Controls/APIExample.ascx" TagName="APIExample" TagPrefix="cms" %>
<asp:Content ID="contentLeft" ContentPlaceHolderID="plcLeftContainer" runat="server">
    <%-- Group --%>
    <cms:LocalizedHeading ID="headCreateGroup" runat="server" Text="Group" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreateGroup" runat="server" ButtonText="Create group" InfoMessage="Group 'My new group' was created." />
    <cms:APIExample ID="apiGetAndUpdateGroup" runat="server" ButtonText="Get and update group" APIExampleType="ManageAdditional" InfoMessage="Group 'My new group' was updated." ErrorMessage="Group 'My new group' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateGroups" runat="server" ButtonText="Get and bulk update groups" APIExampleType="ManageAdditional" InfoMessage="All groups matching the condition were updated." ErrorMessage="Groups matching the condition were not found." />
    <%-- Group member --%>
    <cms:LocalizedHeading ID="headCreateGroupMember" runat="server" Text="Group member" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreateGroupMember" runat="server" ButtonText="Create member" InfoMessage="Member 'My new member' was created." ErrorMessage="Group 'My new group' was not found." />
    <cms:APIExample ID="apiGetAndUpdateGroupMember" runat="server" ButtonText="Get and update member" APIExampleType="ManageAdditional" InfoMessage="Member 'My new member' was updated." ErrorMessage="Member 'My new member' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateGroupMembers" runat="server" ButtonText="Get and bulk update members" APIExampleType="ManageAdditional" InfoMessage="All members matching the condition were updated." ErrorMessage="Members matching the condition were not found." />
    <cms:APIExample ID="apiCreateInvitation" runat="server" ButtonText="Create invitation" APIExampleType="ManageAdditional" InfoMessage="Invitation 'My new group' was created." ErrorMessage="Group 'My new group' was not found." />
</asp:Content>
<asp:Content ID="contentRight" ContentPlaceHolderID="plcRightContainer" runat="server">
    <%-- Group member --%>
    <cms:LocalizedHeading ID="headDeleteGroupMember" runat="server" Text="Group member" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteGroupMember" runat="server" ButtonText="Delete member" APIExampleType="CleanUpMain" InfoMessage="Member 'My new member' and all its dependencies were deleted." ErrorMessage="Member 'My new member' was not found." />
    <%-- Group --%>
    <cms:LocalizedHeading ID="headDeleteGroup" runat="server" Text="Group" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteGroup" runat="server" ButtonText="Delete group" APIExampleType="CleanUpMain" InfoMessage="Group 'My new group' and all its dependencies were deleted." ErrorMessage="Group 'My new group' was not found." />
</asp:Content>
