<%@ Page Title="" Language="C#" MasterPageFile="~/CMSAPIExamples/Pages/APIExamplesPage.Master"
    Theme="Default" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="CMSAPIExamples_Code_Documents_Categories_Default" %>

<%@ Register Src="~/CMSAPIExamples/Controls/APIExample.ascx" TagName="APIExample" TagPrefix="cms" %>
<asp:Content ID="contentLeft" ContentPlaceHolderID="plcLeftContainer" runat="server">
    <%-- Category --%>
    <cms:LocalizedHeading ID="headCreateCategory" runat="server" Text="Category" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreateCategory" runat="server" ButtonText="Create category" InfoMessage="Category 'My new category' was created." />
    <cms:APIExample ID="apiGetAndUpdateCategory" runat="server" ButtonText="Get and update category" APIExampleType="ManageAdditional" InfoMessage="Category 'My new category' was updated." ErrorMessage="Category 'My new category' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateCategories" runat="server" ButtonText="Get and bulk update categories" APIExampleType="ManageAdditional" InfoMessage="All categories matching the condition were updated." ErrorMessage="Categories matching the condition were not found." />
    <%-- Subcategory --%>
    <cms:LocalizedHeading ID="headCreateSubcategory" runat="server" Text="Subcategory" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreateSubcategory" runat="server" ButtonText="Create subcategory" InfoMessage="Category 'My new subcategory' was created." ErrorMessage="Category 'My new category' was not found." />
    <%-- Page in category --%>
    <cms:LocalizedHeading ID="headAddDocumentToCategory" runat="server" Text="Page in category" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiAddDocumentToCategory" runat="server" ButtonText="Add page to category" APIExampleType="ManageAdditional" InfoMessage="Root page was assigned to category 'My new category'." ErrorMessage="Category 'My new category' was not found." />
</asp:Content>
<asp:Content ID="contentRight" ContentPlaceHolderID="plcRightContainer" runat="server">
    <%-- Page in category --%>
    <cms:LocalizedHeading ID="headRemoveDocumentFromCategory" runat="server" Text="Page in category" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiRemoveDocumentFromCategory" runat="server" ButtonText="Remove page from category" APIExampleType="CleanUpMain" InfoMessage="Root page was removed from category 'My new category'." ErrorMessage="Category 'My new category' was not found." />
    <%-- Subcategory --%>
    <cms:LocalizedHeading ID="headDeleteSubcategory" runat="server" Text="Subcategory" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteSubcategory" runat="server" ButtonText="Delete subcategory" APIExampleType="CleanUpMain" InfoMessage="Category 'My new subcategory' and all its dependencies were deleted." ErrorMessage="Category 'My new subcategory' was not found." />
    <%-- Category --%>
    <cms:LocalizedHeading ID="headDeleteCategory" runat="server" Text="Category" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteCategory" runat="server" ButtonText="Delete category" APIExampleType="CleanUpMain" InfoMessage="Category 'My new category' and all its dependencies were deleted." ErrorMessage="Category 'My new category' was not found." />
</asp:Content>
