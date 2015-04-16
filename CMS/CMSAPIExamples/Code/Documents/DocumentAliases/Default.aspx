<%@ Page Title="" Language="C#" MasterPageFile="~/CMSAPIExamples/Pages/APIExamplesPage.Master"
    Theme="Default" AutoEventWireup="true" Inherits="CMSAPIExamples_Code_Documents_DocumentAliases_Default" CodeFile="Default.aspx.cs" %>

<%@ Register Src="~/CMSAPIExamples/Controls/APIExample.ascx" TagName="APIExample" TagPrefix="cms" %>
<asp:Content ID="contentLeft" ContentPlaceHolderID="plcLeftContainer" runat="server">
    <%-- Page alias --%>
    <cms:LocalizedHeading ID="headCreateDocumentAlias" runat="server" Text="Page alias" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreateDocumentAlias" runat="server" ButtonText="Create alias" InfoMessage="Alias 'My new alias' was created." />
    <cms:APIExample ID="apiGetAndUpdateDocumentAlias" runat="server" ButtonText="Get and update alias" APIExampleType="ManageAdditional" InfoMessage="Alias 'My new alias' was updated." ErrorMessage="Alias 'My new alias' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateDocumentAliases" runat="server" ButtonText="Get and bulk update aliases" APIExampleType="ManageAdditional" InfoMessage="All aliases matching the condition were updated." ErrorMessage="Aliases matching the condition were not found." />
</asp:Content>
<asp:Content ID="contentRight" ContentPlaceHolderID="plcRightContainer" runat="server">
    <%-- Page alias --%>
    <cms:LocalizedHeading ID="headDeleteDocumentAlias" runat="server" Text="Page alias" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteDocumentAlias" runat="server" ButtonText="Delete alias" APIExampleType="CleanUpMain" InfoMessage="Alias 'My new alias' and all its dependencies were deleted." ErrorMessage="Alias 'My new alias' was not found." />
</asp:Content>
