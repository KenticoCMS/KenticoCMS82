<%@ Page Title="" Language="C#" MasterPageFile="~/CMSAPIExamples/Pages/APIExamplesPage.Master"
    Theme="Default" AutoEventWireup="true" Inherits="CMSAPIExamples_Code_Development_Widgets_Default" CodeFile="Default.aspx.cs" %>

<%@ Register Src="~/CMSAPIExamples/Controls/APIExample.ascx" TagName="APIExample" TagPrefix="cms" %>
<asp:Content ID="contentLeft" ContentPlaceHolderID="plcLeftContainer" runat="server">
    <%-- Widget category --%>
    <cms:LocalizedHeading ID="headCreateWidgetCategory" runat="server" Text="Widget category" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreateWidgetCategory" runat="server" ButtonText="Create category" InfoMessage="Category 'My new category' was created." />
    <cms:APIExample ID="apiGetAndUpdateWidgetCategory" runat="server" ButtonText="Get and update category" APIExampleType="ManageAdditional" InfoMessage="Category 'My new category' was updated." ErrorMessage="Category 'My new category' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateWidgetCategories" runat="server" ButtonText="Get and bulk update categories" APIExampleType="ManageAdditional" InfoMessage="All categories matching the condition were updated." ErrorMessage="Categories matching the condition were not found." />
    <%-- Widget --%>
    <cms:LocalizedHeading ID="headCreateWidget" runat="server" Text="Widget" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreateWidget" runat="server" ButtonText="Create widget" InfoMessage="Widget 'My new widget' was created." ErrorMessage="Category 'My new category' was not found." />
    <cms:APIExample ID="apiGetAndUpdateWidget" runat="server" ButtonText="Get and update widget" APIExampleType="ManageAdditional" InfoMessage="Widget 'My new widget' was updated." ErrorMessage="Widget 'My new widget' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateWidgets" runat="server" ButtonText="Get and bulk update widgets" APIExampleType="ManageAdditional" InfoMessage="All widgets matching the condition were updated." ErrorMessage="Widgets matching the condition were not found." />
    <%-- Widget security --%>
    <cms:LocalizedHeading ID="headCreateWidgetRole" runat="server" Text="Widget security" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiAddWidgetToRole" runat="server" ButtonText="Add widget to role" APIExampleType="ManageAdditional" InfoMessage="Widget 'My new widget' was added to role 'CMS Desk Administrator'." ErrorMessage="Widget 'My new widget' was not found." />
    <cms:APIExample ID="apiSetSecurityLevel" runat="server" ButtonText="Set security level" APIExampleType="ManageAdditional" InfoMessage="Widget 'My new widget' has changed security settings." ErrorMessage="Widget 'My new widget' was not found." />
</asp:Content>
<asp:Content ID="contentRight" ContentPlaceHolderID="plcRightContainer" runat="server">
    <%-- Widget --%>
    <cms:LocalizedHeading ID="headRemoveRole" runat="server" Text="Widget role" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiRemoveWidgetFromRole" runat="server" ButtonText="Remove widget from role" APIExampleType="CleanUpMain" InfoMessage="Widget 'My new widget' was removed from 'CMS Desk administrator' role." ErrorMessage="Widget 'My new widget' was not found." />
    <%-- Widget --%>
    <cms:LocalizedHeading ID="headDeleteWidget" runat="server" Text="Widget" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteWidget" runat="server" ButtonText="Delete widget" APIExampleType="CleanUpMain" InfoMessage="Widget 'My new widget' and all its dependencies were deleted." ErrorMessage="Widget 'My new widget' was not found." />
    <%-- Widget category --%>
    <cms:LocalizedHeading ID="headDeleteWidgetCategory" runat="server" Text="Widget category" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteWidgetCategory" runat="server" ButtonText="Delete category" APIExampleType="CleanUpMain" InfoMessage="Category 'My new category' and all its dependencies were deleted." ErrorMessage="Category 'My new category' was not found." />
</asp:Content>
