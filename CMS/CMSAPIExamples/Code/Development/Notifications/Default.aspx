<%@ Page Title="" Language="C#" MasterPageFile="~/CMSAPIExamples/Pages/APIExamplesPage.Master"
    Theme="Default" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="CMSAPIExamples_Code_Development_Notifications_Default" %>

<%@ Register Src="~/CMSAPIExamples/Controls/APIExample.ascx" TagName="APIExample" TagPrefix="cms" %>
<asp:Content ID="contentLeft" ContentPlaceHolderID="plcLeftContainer" runat="server">
    <%-- Notification gateway --%>
    <cms:LocalizedHeading ID="headCreateNotificationGateway" runat="server" Text="Notification gateway" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreateNotificationGateway" runat="server" ButtonText="Create gateway" InfoMessage="Gateway 'My new gateway' was created." />
    <cms:APIExample ID="apiGetAndUpdateNotificationGateway" runat="server" ButtonText="Get and update gateway" APIExampleType="ManageAdditional" InfoMessage="Gateway 'My new gateway' was updated." ErrorMessage="Gateway 'My new gateway' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateNotificationGateways" runat="server" ButtonText="Get and bulk update gateways" APIExampleType="ManageAdditional" InfoMessage="All gateways matching the condition were updated." ErrorMessage="Gateways matching the condition were not found." />
    <%-- Notification template --%>
    <cms:LocalizedHeading ID="headCreateNotificationTemplate" runat="server" Text="Notification template" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreateNotificationTemplate" runat="server" ButtonText="Create template" InfoMessage="Template 'My new template' was created." />
    <cms:APIExample ID="apiGetAndUpdateNotificationTemplate" runat="server" ButtonText="Get and update template" APIExampleType="ManageAdditional" InfoMessage="Template 'My new template' was updated." ErrorMessage="Template 'My new template' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateNotificationTemplates" runat="server" ButtonText="Get and bulk update templates" APIExampleType="ManageAdditional" InfoMessage="All templates matching the condition were updated." ErrorMessage="Templates matching the condition were not found." />
    <%-- Notification template text --%>
    <cms:LocalizedHeading ID="headCreateNotificationTemplateText" runat="server" Text="Notification template text" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreateNotificationTemplateText" runat="server" ButtonText="Create text" InfoMessage="Text 'My new text' was created." ErrorMessage="Gateway 'My new gateway' or template 'My new template' were not found." />
    <cms:APIExample ID="apiGetAndUpdateNotificationTemplateText" runat="server" ButtonText="Get and update text" APIExampleType="ManageAdditional" InfoMessage="Text 'My new text' was updated." ErrorMessage="Text 'My new text' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateNotificationTemplateTexts" runat="server" ButtonText="Get and bulk update texts" APIExampleType="ManageAdditional" InfoMessage="All texts matching the condition were updated." ErrorMessage="Texts matching the condition were not found." />
    <%-- Notification subscription --%>
    <cms:LocalizedHeading ID="headCreateNotificationSubscription" runat="server" Text="Notification subscription" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreateNotificationSubscription" runat="server" ButtonText="Create subscription" InfoMessage="Subscription 'My new subscription' was created." ErrorMessage="Gateway 'My new gateway' or template 'My new template' were not found." />
    <cms:APIExample ID="apiGetAndUpdateNotificationSubscription" runat="server" ButtonText="Get and update subscription" APIExampleType="ManageAdditional" InfoMessage="Subscription 'My new subscription' was updated." ErrorMessage="Subscription 'My new subscription' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateNotificationSubscriptions" runat="server" ButtonText="Get and bulk update subscriptions" APIExampleType="ManageAdditional" InfoMessage="All subscriptions matching the condition were updated." ErrorMessage="Subscriptions matching the condition were not found." />
</asp:Content>
<asp:Content ID="contentRight" ContentPlaceHolderID="plcRightContainer" runat="server">
    <%-- Notification subscription --%>
    <cms:LocalizedHeading ID="headDeleteNotificationSubscription" runat="server" Text="Notification subscription" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteNotificationSubscription" runat="server" ButtonText="Delete subscription" APIExampleType="CleanUpMain" InfoMessage="Subscription 'My new subscription' and all its dependencies were deleted." ErrorMessage="Subscription 'My new subscription' was not found." />
    <%-- Notification template text --%>
    <cms:LocalizedHeading ID="headDeleteNotificationTemplateText" runat="server" Text="Notification template text" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteNotificationTemplateText" runat="server" ButtonText="Delete text" APIExampleType="CleanUpMain" InfoMessage="Text 'My new text' and all its dependencies were deleted." ErrorMessage="Text 'My new text' was not found." />
    <%-- Notification template --%>
    <cms:LocalizedHeading ID="headDeleteNotificationTemplate" runat="server" Text="Notification template" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteNotificationTemplate" runat="server" ButtonText="Delete template" APIExampleType="CleanUpMain" InfoMessage="Template 'My new template' and all its dependencies were deleted." ErrorMessage="Template 'My new template' was not found." />
    <%-- Notification gateway --%>
    <cms:LocalizedHeading ID="headDeleteNotificationGateway" runat="server" Text="Notification gateway" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteNotificationGateway" runat="server" ButtonText="Delete gateway" APIExampleType="CleanUpMain" InfoMessage="Gateway 'My new gateway' and all its dependencies were deleted." ErrorMessage="Gateway 'My new gateway' was not found." />
</asp:Content>
