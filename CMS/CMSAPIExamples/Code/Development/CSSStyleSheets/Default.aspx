<%@ Page Title="" Language="C#" MasterPageFile="~/CMSAPIExamples/Pages/APIExamplesPage.Master"
    Theme="Default" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="CMSAPIExamples_Code_Development_CSSStyleSheets_Default" %>

<%@ Register Src="~/CMSAPIExamples/Controls/APIExample.ascx" TagName="APIExample" TagPrefix="cms" %>
<asp:Content ID="contentLeft" ContentPlaceHolderID="plcLeftContainer" runat="server">
    <%-- Css stylesheet --%>
    <cms:LocalizedHeading ID="headCreateCssStylesheet" runat="server" Text="Css stylesheet" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreateCssStylesheet" runat="server" ButtonText="Create stylesheet" InfoMessage="Stylesheet 'My new stylesheet' was created." />
    <cms:APIExample ID="apiGetAndUpdateCssStylesheet" runat="server" ButtonText="Get and update stylesheet" APIExampleType="ManageAdditional" InfoMessage="Stylesheet 'My new stylesheet' was updated." ErrorMessage="Stylesheet 'My new stylesheet' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateCssStylesheets" runat="server" ButtonText="Get and bulk update stylesheets" APIExampleType="ManageAdditional" InfoMessage="All stylesheets matching the condition were updated." ErrorMessage="Stylesheets matching the condition were not found." />
    <%-- Css stylesheet on site --%>
    <cms:LocalizedHeading ID="headAddCssStylesheetToSite" runat="server" Text="Css stylesheet on site" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiAddCssStylesheetToSite" runat="server" ButtonText="Add stylesheet to site" APIExampleType="ManageAdditional" InfoMessage="Stylesheet 'My new stylesheet' was added to site." ErrorMessage="Stylesheet 'My new stylesheet' was not found." />
</asp:Content>
<asp:Content ID="contentRight" ContentPlaceHolderID="plcRightContainer" runat="server">
    <%-- Css stylesheet on site --%>
    <cms:LocalizedHeading ID="headRemoveCssStylesheetFromSite" runat="server" Text="Css stylesheet on site" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiRemoveCssStylesheetFromSite" runat="server" ButtonText="Remove stylesheet from site" APIExampleType="CleanUpMain" InfoMessage="Stylesheet 'My new stylesheet' was removed from site." ErrorMessage="Stylesheet 'My new stylesheet' was not found." />
    <%-- Css stylesheet --%>
    <cms:LocalizedHeading ID="headDeleteCssStylesheet" runat="server" Text="Css stylesheet" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteCssStylesheet" runat="server" ButtonText="Delete stylesheet" APIExampleType="CleanUpMain" InfoMessage="Stylesheet 'My new stylesheet' and all its dependencies were deleted." ErrorMessage="Stylesheet 'My new stylesheet' was not found." />
</asp:Content>
