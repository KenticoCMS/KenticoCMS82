<%@ Page Title="" Language="C#" MasterPageFile="~/CMSAPIExamples/Pages/APIExamplesPage.Master"
    Theme="Default" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="CMSAPIExamples_Code_Development_WebTemplates_Default" %>

<%@ Register Src="~/CMSAPIExamples/Controls/APIExample.ascx" TagName="APIExample" TagPrefix="cms" %>
<asp:Content ID="contentLeft" ContentPlaceHolderID="plcLeftContainer" runat="server">
    <%-- Web template --%>
    <cms:LocalizedHeading ID="headCreateWebTemplate" runat="server" Text="Web template" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreateWebTemplate" runat="server" ButtonText="Create template" InfoMessage="Template 'My new template' was created." />
    <cms:APIExample ID="apiGetAndUpdateWebTemplate" runat="server" ButtonText="Get and update template" APIExampleType="ManageAdditional" InfoMessage="Template 'My new template' was updated." ErrorMessage="Template 'My new template' was not found." />
    <cms:APIExample ID="apiGetAndMoveWebTemplateUp" runat="server" ButtonText="Get and move template up" APIExampleType="ManageAdditional" InfoMessage="Template 'My new template' was moved up." ErrorMessage="Template 'My new template' was not found." />
    <cms:APIExample ID="apiGetAndMoveWebTemplateDown" runat="server" ButtonText="Get and move template down" APIExampleType="ManageAdditional" InfoMessage="Template 'My new template' was moved down." ErrorMessage="Template 'My new template' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateWebTemplates" runat="server" ButtonText="Get and bulk update templates" APIExampleType="ManageAdditional" InfoMessage="All templates matching the condition were updated." ErrorMessage="Templates matching the condition were not found." />
</asp:Content>
<asp:Content ID="contentRight" ContentPlaceHolderID="plcRightContainer" runat="server">
    <%-- Web template --%>
    <cms:LocalizedHeading ID="headDeleteWebTemplate" runat="server" Text="Web template" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteWebTemplate" runat="server" ButtonText="Delete template" APIExampleType="CleanUpMain" InfoMessage="Template 'My new template' and all its dependencies were deleted." ErrorMessage="Template 'My new template' was not found." />
</asp:Content>
