<%@ Page Title="" Language="C#" MasterPageFile="~/CMSAPIExamples/Pages/APIExamplesPage.Master"
    Theme="Default" AutoEventWireup="true" Inherits="CMSAPIExamples_Code_Documents_Advanced_Default" CodeFile="Default.aspx.cs" %>

<%@ Register Src="~/CMSAPIExamples/Controls/APIExample.ascx" TagName="APIExample" TagPrefix="cms" %>
<asp:Content ID="contentLeft" ContentPlaceHolderID="plcLeftContainer" runat="server">
    <%-- Example preparation --%>
    <cms:LocalizedHeading ID="headPreparation" runat="server" Text="Example preparation" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreateDocumentStructure" runat="server" ButtonText="Create page structure" InfoMessage="Pages prepared successfully." ErrorMessage="Site root node not found." />
    <%-- Organizing pages --%>
    <cms:LocalizedHeading ID="headCreateDocument" runat="server" Text="Organizing pages" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiMoveDocumentUp" runat="server" APIExampleType="ManageAdditional" ButtonText="Move page up" InfoMessage="Page moved up." ErrorMessage="Page not found." />
    <cms:APIExample ID="apiMoveDocumentDown" runat="server" APIExampleType="ManageAdditional" ButtonText="Move page down" InfoMessage="Page moved down." ErrorMessage="Page not found." />
    <cms:APIExample ID="apiSortDocumentsAlphabetically" runat="server" APIExampleType="ManageAdditional" ButtonText="Sort pages alphabetically" InfoMessage="Pages sorted from A to Z." ErrorMessage="API Example folder not found." />
    <cms:APIExample ID="apiSortDocumentsByDate" runat="server" APIExampleType="ManageAdditional" ButtonText="Sort pages by date" InfoMessage="Pages sorted from oldest to newest." ErrorMessage="API Example folder not found." />
    <%-- Recycle bin --%>
    <cms:LocalizedHeading ID="headRecycleBin" runat="server" Text="Recycle bin" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiMoveToRecycleBin" runat="server" ButtonText="Move page to recycle bin" InfoMessage="Page moved to the recycle bin." ErrorMessage="Page not found." />
    <cms:APIExample ID="apiRestoreFromRecycleBin" runat="server" ButtonText="Restore page" InfoMessage="Page restored successfully." ErrorMessage="Page not found in the recycle bin." />
</asp:Content>
<asp:Content ID="contentRight" ContentPlaceHolderID="plcRightContainer" runat="server">
    <%-- Deleting pages --%>
    <cms:LocalizedHeading ID="pnlDeleleDocumentStructure" runat="server" Text="Page structure" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteDocumentStructure" runat="server" ButtonText="Delete page structure" APIExampleType="CleanUpMain" InfoMessage="Page structure successfully deleted." />
</asp:Content>
