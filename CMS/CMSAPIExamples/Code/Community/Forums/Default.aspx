<%@ Page Title="" Language="C#" MasterPageFile="~/CMSAPIExamples/Pages/APIExamplesPage.Master"
    Theme="Default" AutoEventWireup="true" Inherits="CMSAPIExamples_Code_Community_Forums_Default" CodeFile="Default.aspx.cs" %>

<%@ Register Src="~/CMSAPIExamples/Controls/APIExample.ascx" TagName="APIExample" TagPrefix="cms" %>
<asp:Content ID="contentLeft" ContentPlaceHolderID="plcLeftContainer" runat="server">
    <%-- Forum group --%>
    <cms:LocalizedHeading ID="headCreateForumGroup" runat="server" Text="Forum group" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreateForumGroup" runat="server" ButtonText="Create group" InfoMessage="Group 'My new group' was created." />
    <cms:APIExample ID="apiGetAndUpdateForumGroup" runat="server" ButtonText="Get and update group" APIExampleType="ManageAdditional" InfoMessage="Group 'My new group' was updated." ErrorMessage="Group 'My new group' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateForumGroups" runat="server" ButtonText="Get and bulk update groups" APIExampleType="ManageAdditional" InfoMessage="All groups matching the condition were updated." ErrorMessage="Groups matching the condition were not found." />
    <%-- Forum --%>
    <cms:LocalizedHeading ID="headCreateForum" runat="server" Text="Forum" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreateForum" runat="server" ButtonText="Create forum" InfoMessage="Forum 'My new forum' was created." ErrorMessage="Group 'My new group' was not found." />
    <cms:APIExample ID="apiGetAndUpdateForum" runat="server" ButtonText="Get and update forum" APIExampleType="ManageAdditional" InfoMessage="Forum 'My new forum' was updated." ErrorMessage="Forum 'My new forum' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateForums" runat="server" ButtonText="Get and bulk update forums" APIExampleType="ManageAdditional" InfoMessage="All forums matching the condition were updated." ErrorMessage="Forums matching the condition were not found." />
    <%-- Forum post --%>
    <cms:LocalizedHeading ID="headCreateForumPost" runat="server" Text="Forum post" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreateForumPost" runat="server" ButtonText="Create post" InfoMessage="Post 'My new post' was created." ErrorMessage="Forum 'My new forum' was not found." />
    <cms:APIExample ID="apiGetAndUpdateForumPost" runat="server" ButtonText="Get and update post" APIExampleType="ManageAdditional" InfoMessage="Post 'My new post' was updated." ErrorMessage="Post 'My new post' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateForumPosts" runat="server" ButtonText="Get and bulk update posts" APIExampleType="ManageAdditional" InfoMessage="All posts matching the condition were updated." ErrorMessage="Posts matching the condition were not found." />
</asp:Content>
<asp:Content ID="contentRight" ContentPlaceHolderID="plcRightContainer" runat="server">
    <%-- Forum post --%>
    <cms:LocalizedHeading ID="headDeleteForumPost" runat="server" Text="Forum post" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteForumPost" runat="server" ButtonText="Delete post" APIExampleType="CleanUpMain" InfoMessage="Post 'My new post' and all its dependencies were deleted." ErrorMessage="Post 'My new post' was not found." />
    <%-- Forum --%>
    <cms:LocalizedHeading ID="headDeleteForum" runat="server" Text="Forum" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteForum" runat="server" ButtonText="Delete forum" APIExampleType="CleanUpMain" InfoMessage="Forum 'My new forum' and all its dependencies were deleted." ErrorMessage="Forum 'My new forum' was not found." />
    <%-- Forum group --%>
    <cms:LocalizedHeading ID="headDeleteForumGroup" runat="server" Text="Forum group" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteForumGroup" runat="server" ButtonText="Delete group" APIExampleType="CleanUpMain" InfoMessage="Group 'My new group' and all its dependencies were deleted." ErrorMessage="Group 'My new group' was not found." />
</asp:Content>
