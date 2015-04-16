<%@ Page Title="" Language="C#" MasterPageFile="~/CMSAPIExamples/Pages/APIExamplesPage.Master"
    Theme="Default" AutoEventWireup="true" Inherits="CMSAPIExamples_Code_Documents_Workflow_VersioningWithoutWorkflow_Default" CodeFile="Default.aspx.cs" %>

<%@ Register Src="~/CMSAPIExamples/Controls/APIExample.ascx" TagName="APIExample" TagPrefix="cms" %>
<asp:Content ID="contentLeft" ContentPlaceHolderID="plcLeftContainer" runat="server">
    <%-- Creating example objects --%>
    <cms:LocalizedHeading ID="headCreateExampleObjects" runat="server" Text="Creating example objects" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreateExampleFolder" runat="server" ButtonText="Create example folder" APIExampleType="ManageMain" InfoMessage="Example folder was created." ErrorMessage="Example folder cannot be created." />
    <cms:APIExample ID="apiCreateWorkflowScope" runat="server" ButtonText="Create scope" APIExampleType="ManageMain" InfoMessage="Scope was created." ErrorMessage="Workflow 'Versioning without workflow' was not found." />
    <%-- Managing pages --%>
    <cms:LocalizedHeading ID="headEditDocument" runat="server" Text="Managing pages" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreateDocument" runat="server" ButtonText="Create page" APIExampleType="ManageMain" InfoMessage="Page 'My new page' was created under the API Example folder." ErrorMessage="Workflow 'Versioning without workflow' was not found." />
    <cms:APIExample ID="apiUpdateDocument" runat="server" ButtonText="Update page" APIExampleType="ManageAdditional" InfoMessage="Page 'My new page' was updated." ErrorMessage="Workflow 'Versioning without workflow' was not found." />
</asp:Content>
<asp:Content ID="contentRight" ContentPlaceHolderID="plcRightContainer" runat="server">
    <%-- Cleanup --%>
    <cms:LocalizedHeading ID="headCleanUp" runat="server" Text="Cleanup" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteWorkflowScope" runat="server" ButtonText="Delete scope" APIExampleType="CleanUpMain" InfoMessage="Scope was deleted." ErrorMessage="Scope was not found." />
    <cms:APIExample ID="apiDeleteDocuments" runat="server" ButtonText="Delete example folder" APIExampleType="CleanUpMain" InfoMessage="All example pages were deleted." ErrorMessage="API example folder was not found." />
</asp:Content>
