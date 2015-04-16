<%@ Page Title="" Language="C#" MasterPageFile="~/CMSAPIExamples/Pages/APIExamplesPage.Master"
    Theme="Default" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="CMSAPIExamples_Code_Documents_Tags_Default" %>

<%@ Register Src="~/CMSAPIExamples/Controls/APIExample.ascx" TagName="APIExample" TagPrefix="cms" %>
<asp:Content ID="contentLeft" ContentPlaceHolderID="plcLeftContainer" runat="server">
    <%-- Tag group --%>
    <cms:LocalizedHeading ID="headCreateTagGroup" runat="server" Text="Tag group" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreateTagGroup" runat="server" ButtonText="Create group" InfoMessage="Group 'My new group' was created." />
    <cms:APIExample ID="apiGetAndUpdateTagGroup" runat="server" ButtonText="Get and update group" APIExampleType="ManageAdditional" InfoMessage="Group 'My new group' was updated." ErrorMessage="Group 'My new group' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateTagGroups" runat="server" ButtonText="Get and bulk update groups" APIExampleType="ManageAdditional" InfoMessage="All groups matching the condition were updated." ErrorMessage="Groups matching the condition were not found." />
    <%-- Tag --%>
    <cms:LocalizedHeading ID="headCreateTag" runat="server" Text="Tag on page" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiAddTagToDocument" runat="server" ButtonText="Add tag to page" InfoMessage="Tag 'My new tag' was added to page." ErrorMessage="Tag group 'My new group' was not found." />
    <cms:APIExample ID="apiGetDocumentAndUpdateItsTags" runat="server" ButtonText="Get page and update its tags" APIExampleType="ManageAdditional" InfoMessage="Tag 'My new tag' was updated." ErrorMessage="Tag 'My new tag' was not found." />
</asp:Content>
<asp:Content ID="contentRight" ContentPlaceHolderID="plcRightContainer" runat="server">
    <%-- Tag --%>
    <cms:LocalizedHeading ID="headRemoveTagFromDocument" runat="server" Text="Tag" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiRemoveTagFromDocument" runat="server" ButtonText="Remove tag from page" APIExampleType="CleanUpMain" InfoMessage="Tag 'My new tag' was removed from page." ErrorMessage="Tag 'My new tag', root page or their relationship were not found." />
    <%-- Tag group --%>
    <cms:LocalizedHeading ID="headDeleteTagGroup" runat="server" Text="Tag group" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteTagGroup" runat="server" ButtonText="Delete group" APIExampleType="CleanUpMain" InfoMessage="Group 'My new group' and all its dependencies were deleted." ErrorMessage="Group 'My new group' was not found." />
</asp:Content>
