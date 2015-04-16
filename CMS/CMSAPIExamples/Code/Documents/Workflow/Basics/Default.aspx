<%@ Page Title="" Language="C#" MasterPageFile="~/CMSAPIExamples/Pages/APIExamplesPage.Master"
    Theme="Default" AutoEventWireup="true" Inherits="CMSAPIExamples_Code_Documents_Workflow_Basics_Default" CodeFile="Default.aspx.cs" %>

<%@ Register Src="~/CMSAPIExamples/Controls/APIExample.ascx" TagName="APIExample" TagPrefix="cms" %>
<asp:Content ID="contentLeft" ContentPlaceHolderID="plcLeftContainer" runat="server">
    <%-- Creating pages --%>
    <cms:LocalizedHeading ID="headCreateDocument" runat="server" Text="Creating pages" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreateExampleObjects" runat="server" ButtonText="Create example objects" InfoMessage="Objects needed for the example were created." ErrorMessage="Site root not found." />
    <cms:APIExample ID="apiCreateDocument" runat="server" ButtonText="Create page" InfoMessage="Page was created." ErrorMessage="Example folder not found." />
    <cms:APIExample ID="apiCreateNewCultureVersion" runat="server" ButtonText="Create new culture version" InfoMessage="New culture version of the page was created." ErrorMessage="Page 'My new page' was not found." />
    <cms:APIExample ID="apiCreateLinkedDocument" runat="server" ButtonText="Create linked page" InfoMessage="Link to the page was created." ErrorMessage="Site root not found." />
    <%-- Managing pages --%>
    <cms:LocalizedHeading ID="headEditDocument" runat="server" Text="Editing pages" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiGetAndUpdateDocuments" runat="server" ButtonText="Get and update pages" APIExampleType="ManageAdditional" InfoMessage="All menu items under the API Example folder were updated." ErrorMessage="No pages were found." />
    <cms:APIExample ID="apiCopyDocument" runat="server" ButtonText="Copy page" APIExampleType="ManageAdditional" InfoMessage="Page 'My new page' successfully copied under 'API Example/Source'." ErrorMessage="The page to be copied or the target page could not be found." />
    <cms:APIExample ID="apiMoveDocument" runat="server" ButtonText="Move page" APIExampleType="ManageAdditional" InfoMessage="Page '/API Example/Source/My new page' successfully moved under 'API Example/Target'." ErrorMessage="The page to be moved or the target page could not be found." />
</asp:Content>
<asp:Content ID="contentRight" ContentPlaceHolderID="plcRightContainer" runat="server">
    <cms:LocalizedHeading ID="headCleanUp" runat="server" Text="Cleanup" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteDocuments" runat="server" ButtonText="Delete pages" APIExampleType="CleanUpMain" InfoMessage="All example pages were deleted." ErrorMessage="API example folder not found." />
    <cms:APIExample ID="apiDeleteObjects" runat="server" ButtonText="Delete objects" APIExampleType="CleanUpMain" InfoMessage="All example objects were deleted." ErrorMessage="" />
</asp:Content>
