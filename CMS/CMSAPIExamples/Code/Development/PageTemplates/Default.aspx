<%@ Page Title="" Language="C#" MasterPageFile="~/CMSAPIExamples/Pages/APIExamplesPage.Master"
    Theme="Default" AutoEventWireup="true" Inherits="CMSAPIExamples_Code_Development_PageTemplates_Default" CodeFile="Default.aspx.cs" %>

<%@ Register Src="~/CMSAPIExamples/Controls/APIExample.ascx" TagName="APIExample" TagPrefix="cms" %>
<asp:Content ID="contentLeft" ContentPlaceHolderID="plcLeftContainer" runat="server">
    <%-- Page template category --%>
    <cms:LocalizedHeading ID="headCreatePageTemplateCategory" runat="server" Text="Page template category" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreatePageTemplateCategory" runat="server" ButtonText="Create category" InfoMessage="Category 'My new category' was created." />
    <cms:APIExample ID="apiGetAndUpdatePageTemplateCategory" runat="server" ButtonText="Get and update category" APIExampleType="ManageAdditional" InfoMessage="Category 'My new category' was updated." ErrorMessage="Category 'My new category' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdatePageTemplateCategories" runat="server" ButtonText="Get and bulk update categories" APIExampleType="ManageAdditional" InfoMessage="All categories matching the condition were updated." ErrorMessage="Categories matching the condition were not found." />
    <%-- Page template --%>
    <cms:LocalizedHeading ID="headCreatePageTemplate" runat="server" Text="Page template" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreatePageTemplate" runat="server" ButtonText="Create template" InfoMessage="Template 'My new template' was created." ErrorMessage="Category 'My new category' was not found." />
    <cms:APIExample ID="apiGetAndUpdatePageTemplate" runat="server" ButtonText="Get and update template" APIExampleType="ManageAdditional" InfoMessage="Template 'My new template' was updated." ErrorMessage="Template 'My new template' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdatePageTemplates" runat="server" ButtonText="Get and bulk update templates" APIExampleType="ManageAdditional" InfoMessage="All templates matching the condition were updated." ErrorMessage="Templates matching the condition were not found." />
    <%-- Page template on site --%>
    <cms:LocalizedHeading ID="headAddPageTemplateToSite" runat="server" Text="Page template on site" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiAddPageTemplateToSite" runat="server" ButtonText="Add template to site" APIExampleType="ManageAdditional" InfoMessage="Template 'My new template' was added to site." ErrorMessage="Template 'My new template' was not found." />
    <%-- Page template scope --%>
    <cms:LocalizedHeading ID="headCreatePageTemplateScope" runat="server" Text="Page template scope" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreatePageTemplateScope" runat="server" ButtonText="Create scope" InfoMessage="Scope 'My new scope' was created." />
</asp:Content>
<asp:Content ID="contentRight" ContentPlaceHolderID="plcRightContainer" runat="server">
    <%-- Page template scope --%>
    <cms:LocalizedHeading ID="headDeletePageTemplateScope" runat="server" Text="Page template scope" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeletePageTemplateScope" runat="server" ButtonText="Delete scope" APIExampleType="CleanUpMain" InfoMessage="Scope 'My new scope' and all its dependencies were deleted." ErrorMessage="Scope 'My new scope' was not found." />
    <%-- Page template on site --%>
    <cms:LocalizedHeading ID="headRemovePageTemplateFromSite" runat="server" Text="Page template on site" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiRemovePageTemplateFromSite" runat="server" ButtonText="Remove template from site" APIExampleType="CleanUpMain" InfoMessage="Template 'My new template' was removed from site." ErrorMessage="Template 'My new template' was not found." />
    <%-- Page template --%>
    <cms:LocalizedHeading ID="headDeletePageTemplate" runat="server" Text="Page template" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeletePageTemplate" runat="server" ButtonText="Delete template" APIExampleType="CleanUpMain" InfoMessage="Template 'My new template' and all its dependencies were deleted." ErrorMessage="Template 'My new template' was not found." />
    <%-- Page template category --%>
    <cms:LocalizedHeading ID="headDeletePageTemplateCategory" runat="server" Text="Page template category" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeletePageTemplateCategory" runat="server" ButtonText="Delete category" APIExampleType="CleanUpMain" InfoMessage="Category 'My new category' and all its dependencies were deleted." ErrorMessage="Category 'My new category' was not found." />
</asp:Content>
