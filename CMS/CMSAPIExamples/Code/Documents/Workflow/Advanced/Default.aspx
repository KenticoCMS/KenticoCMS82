<%@ Page Title="" Language="C#" MasterPageFile="~/CMSAPIExamples/Pages/APIExamplesPage.Master"
    Theme="Default" AutoEventWireup="True" Inherits="CMSAPIExamples_Code_Documents_Workflow_Advanced_Default"
    CodeFile="Default.aspx.cs" %>

<%@ Register Src="~/CMSAPIExamples/Controls/APIExample.ascx" TagName="APIExample"
    TagPrefix="cms" %>
<asp:Content ID="contentLeft" ContentPlaceHolderID="plcLeftContainer" runat="server">
    <%-- Preparation --%>
    <cms:LocalizedHeading ID="headPreparation" runat="server" Text="Creating pages" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreateExampleObjects" runat="server" ButtonText="Create example objects"
        InfoMessage="Page and objects needed for the example were created." ErrorMessage="Site root not found." />
    <%-- Editing page --%>
    <cms:LocalizedHeading ID="headEditDocument" runat="server" Text="Editing pages" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCheckOut" runat="server" ButtonText="Check out page" APIExampleType="ManageAdditional"
        InfoMessage="Page was checked out." ErrorMessage="The page was not found." />
    <cms:APIExample ID="apiEditDocument" runat="server" ButtonText="Edit page" APIExampleType="ManageAdditional"
        InfoMessage="Page version was modified." ErrorMessage="The page was not founf." />
    <cms:APIExample ID="apiCheckIn" runat="server" ButtonText="Check in page" APIExampleType="ManageAdditional"
        InfoMessage="Page was checked in." ErrorMessage="The page was not found." />
    <cms:APIExample ID="apiUndoCheckout" runat="server" ButtonText="Undo check-out" APIExampleType="ManageAdditional"
        InfoMessage="All changes were discarded and the page was checked in." ErrorMessage="The page was not found." />
    <%-- Workflow process --%>
    <cms:LocalizedHeading ID="headWorkflowProcess" runat="server" Text="Workflow process" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiMoveToNextStep" runat="server" ButtonText="Move to next step"
        APIExampleType="ManageAdditional" InfoMessage="The page was moved to next step."
        ErrorMessage="The page was not found." />
    <cms:APIExample ID="apiMoveToPreviousStep" runat="server" ButtonText="Move to previous step"
        APIExampleType="ManageAdditional" InfoMessage="The page was moved to previous step."
        ErrorMessage="The page was not found." />
    <cms:APIExample ID="apiPublishDocument" runat="server" ButtonText="Publish page"
        APIExampleType="ManageAdditional" InfoMessage="The page was published." ErrorMessage="The page was not found." />
    <cms:APIExample ID="apiArchiveDocument" runat="server" ButtonText="Archive page"
        APIExampleType="ManageAdditional" InfoMessage="The page was archived." ErrorMessage="The page was not found." />
    <%-- Versions --%>
    <cms:LocalizedHeading ID="headVersioning" runat="server" Text="Versioning" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiRollbackVersion" runat="server" ButtonText="Rollback version"
        APIExampleType="ManageAdditional" InfoMessage="The page was rolled back to latest version."
        ErrorMessage="The page was not found." />
    <cms:APIExample ID="apiDeleteVersion" runat="server" ButtonText="Delete version"
        APIExampleType="ManageAdditional" InfoMessage="Page oldest version was deleted."
        ErrorMessage="The page was not found." />
    <cms:APIExample ID="apiDestroyHistory" runat="server" ButtonText="Destroy version history"
        APIExampleType="ManageAdditional" InfoMessage="Page version history was deleted."
        ErrorMessage="The page was not found." />
</asp:Content>
<asp:Content ID="contentRight" ContentPlaceHolderID="plcRightContainer" runat="server">
    <cms:LocalizedHeading ID="headCleanUp" runat="server" Text="Cleanup" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteExampleObjects" runat="server" ButtonText="Delete example objects"
        APIExampleType="CleanUpMain" InfoMessage="All example objects were deleted." />
</asp:Content>
