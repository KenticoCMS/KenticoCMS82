<%@ Page Title="" Language="C#" MasterPageFile="~/CMSAPIExamples/Pages/APIExamplesPage.Master"
    Theme="Default" AutoEventWireup="true" Inherits="CMSAPIExamples_Code_Development_Webparts_Default" CodeFile="Default.aspx.cs" %>

<%@ Register Src="~/CMSAPIExamples/Controls/APIExample.ascx" TagName="APIExample" TagPrefix="cms" %>
<asp:Content ID="contentLeft" ContentPlaceHolderID="plcLeftContainer" runat="server">
    <%-- Web part category --%>
    <cms:LocalizedHeading ID="headCreateWebPartCategory" runat="server" Text="Web part category" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreateWebPartCategory" runat="server" ButtonText="Create category" InfoMessage="Category 'My new category' was created." />
    <cms:APIExample ID="apiGetAndUpdateWebPartCategory" runat="server" ButtonText="Get and update category" APIExampleType="ManageAdditional" InfoMessage="Category 'My new category' was updated." ErrorMessage="Category 'My new category' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateWebPartCategories" runat="server" ButtonText="Get and bulk update categories" APIExampleType="ManageAdditional" InfoMessage="All categories matching the condition were updated." ErrorMessage="Categories matching the condition were not found." />
    <%-- Web part --%>
    <cms:LocalizedHeading ID="headCreateWebPart" runat="server" Text="Web part" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreateWebPart" runat="server" ButtonText="Create web part" InfoMessage="Webpart 'My new web part' was created." ErrorMessage="Category 'My new category' was not found." />
    <cms:APIExample ID="apiGetAndUpdateWebPart" runat="server" ButtonText="Get and update web part" APIExampleType="ManageAdditional" InfoMessage="Webpart 'My new web part' was updated." ErrorMessage="Webpart 'My new web part' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateWebParts" runat="server" ButtonText="Get and bulk update web parts" APIExampleType="ManageAdditional" InfoMessage="All parts matching the condition were updated." ErrorMessage="Webparts matching the condition were not found." />
    <%-- Web part layout --%>
    <cms:LocalizedHeading ID="headCreateWebPartLayout" runat="server" Text="Web part layout" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreateWebPartLayout" runat="server" ButtonText="Create layout" InfoMessage="Layout 'My new layout' was created." ErrorMessage="Webpart 'My new web part' was not found." />
    <cms:APIExample ID="apiGetAndUpdateWebPartLayout" runat="server" ButtonText="Get and update layout" APIExampleType="ManageAdditional" InfoMessage="Layout 'My new layout' was updated." ErrorMessage="Layout 'My new layout' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateWebPartLayouts" runat="server" ButtonText="Get and bulk update layouts" APIExampleType="ManageAdditional" InfoMessage="All layouts matching the condition were updated." ErrorMessage="Layouts matching the condition were not found." />
    <%-- Web part container --%>
    <cms:LocalizedHeading ID="headCreateWebPartContainer" runat="server" Text="Web part container" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreateWebPartContainer" runat="server" ButtonText="Create container" InfoMessage="Container 'My new container' was created." />
    <cms:APIExample ID="apiGetAndUpdateWebPartContainer" runat="server" ButtonText="Get and update container" APIExampleType="ManageAdditional" InfoMessage="Container 'My new container' was updated." ErrorMessage="Container 'My new container' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateWebPartContainers" runat="server" ButtonText="Get and bulk update containers" APIExampleType="ManageAdditional" InfoMessage="All containers matching the condition were updated." ErrorMessage="Containers matching the condition were not found." />
    <%-- Web part container on site --%>
    <cms:LocalizedHeading ID="headAddWebPartContainerToSite" runat="server" Text="Web part container on site" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiAddWebPartContainerToSite" runat="server" ButtonText="Add container to site" InfoMessage="Container 'My new container' was added to site." ErrorMessage="Container 'My new container' was not found." />
</asp:Content>
<asp:Content ID="contentRight" ContentPlaceHolderID="plcRightContainer" runat="server">
    <%-- Web part container on site --%>
    <cms:LocalizedHeading ID="headRemoveWebPartContainerFromSite" runat="server" Text="Web part container on site" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiRemoveWebPartContainerFromSite" runat="server" ButtonText="Remove container from site" APIExampleType="CleanUpMain" InfoMessage="Container 'My new container' was removed from site." ErrorMessage="Container 'My new container' was not found." />
    <%-- Web part container --%>
    <cms:LocalizedHeading ID="headDeleteWebPartContainer" runat="server" Text="Web part container" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteWebPartContainer" runat="server" ButtonText="Delete container" APIExampleType="CleanUpMain" InfoMessage="Container 'My new container' and all its dependencies were deleted." ErrorMessage="Container 'My new container' was not found." />
    <%-- Web part layout --%>
    <cms:LocalizedHeading ID="headDeleteWebPartLayout" runat="server" Text="Web part layout" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteWebPartLayout" runat="server" ButtonText="Delete layout" APIExampleType="CleanUpMain" InfoMessage="Layout 'My new layout' and all its dependencies were deleted." ErrorMessage="Layout 'My new layout' was not found." />
    <%-- Web part --%>
    <cms:LocalizedHeading ID="headDeleteWebPart" runat="server" Text="Web web part" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteWebPart" runat="server" ButtonText="Delete web part" APIExampleType="CleanUpMain" InfoMessage="Webpart 'My new web part' and all its dependencies were deleted." ErrorMessage="Webpart 'My new web part' was not found." />
    <%-- Web part category --%>
    <cms:LocalizedHeading ID="headDeleteWebPartCategory" runat="server" Text="Web part category" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteWebPartCategory" runat="server" ButtonText="Delete category" APIExampleType="CleanUpMain" InfoMessage="Category 'My new category' and all its dependencies were deleted." ErrorMessage="Category 'My new category' was not found." />
</asp:Content>
