<%@ Page Title="" Language="C#" MasterPageFile="~/CMSAPIExamples/Pages/APIExamplesPage.Master"
    Theme="Default" AutoEventWireup="true" Inherits="CMSAPIExamples_Code_Documents_Basics_Default" CodeFile="Default.aspx.cs" %>

<%@ Register Src="~/CMSAPIExamples/Controls/APIExample.ascx" TagName="APIExample" TagPrefix="cms" %>
<asp:Content ID="contentLeft" ContentPlaceHolderID="plcLeftContainer" runat="server">
    <%-- Creating pages --%>
    <cms:LocalizedHeading ID="headCreateDocument" runat="server" Text="Creating pages" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreateDocumentStructure" runat="server" ButtonText="Create page structure" InfoMessage="Page structure for the API example created successfully." ErrorMessage="Site root node not found." />
    <cms:APIExample ID="apiCreateDocument" runat="server" ButtonText="Create page" InfoMessage="Page 'My new page' created successfully." ErrorMessage="Parent page not found." />
    <cms:APIExample ID="apiCreateNewCultureVersion" runat="server" ButtonText="Create new culture version" InfoMessage="Page 'My new page' translated successfully." ErrorMessage="Page 'My new page' was not found." />
    <cms:APIExample ID="apiCreateLinkedDocument" runat="server" ButtonText="Create linked page" InfoMessage="Link to page 'My new page' was created successfully." ErrorMessage="Parent page not found." />
    <%-- Managing pages --%>
    <cms:LocalizedHeading ID="headManageDocuments" runat="server" Text="Managing pages" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiGetAndUpdateDocuments" runat="server" ButtonText="Get and update pages" APIExampleType="ManageAdditional" InfoMessage="All menu items under the API Example folder were updated." ErrorMessage="No pages were found." />
    <cms:APIExample ID="apiCopyDocument" runat="server" ButtonText="Copy page" APIExampleType="ManageAdditional" InfoMessage="Page 'My new page' successfully copied under 'API Example/Source'." ErrorMessage="The page to be copied or the target page could not be found." />
    <cms:APIExample ID="apiMoveDocument" runat="server" ButtonText="Move page" APIExampleType="ManageAdditional" InfoMessage="Page '/API Example/Source/My new page' successfully moved under 'API Example/Target'." ErrorMessage="The page to be moved or the target page could not be found." />
    <cms:APIExample ID="apiRetrieveDocuments" runat="server" ButtonText="Retrieve pages" APIExampleType="ManageAdditional" InfoMessage="Operation finished successfully." ErrorMessage="No pages to retrieved." />
</asp:Content>
<asp:Content ID="contentRight" ContentPlaceHolderID="plcRightContainer" runat="server">
    <%-- Deleting pages --%>
    <cms:LocalizedHeading ID="headDeleleDocument" runat="server" Text="Pages" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteLinkedDocuments" runat="server" ButtonText="Delete linked pages" APIExampleType="CleanUpMain" InfoMessage="All links to page 'My new page' deleted successfully." ErrorMessage="The page could not be found." />
    <cms:APIExample ID="apiDeleteCultureVersion" runat="server" ButtonText="Delete culture version" APIExampleType="CleanUpMain" InfoMessage="Culture version of the page deleted successfully." ErrorMessage="The page could not be found." />
    <cms:APIExample ID="apiDeleteDocument" runat="server" ButtonText="Delete page" APIExampleType="CleanUpMain" InfoMessage="Page 'My new page' deleted successfully." ErrorMessage="The page could not be found." />
    <cms:APIExample ID="apiDeleteDocumentStructure" runat="server" ButtonText="Delete page structure" APIExampleType="CleanUpMain" InfoMessage="The page structure deleted successfully." />
</asp:Content>
