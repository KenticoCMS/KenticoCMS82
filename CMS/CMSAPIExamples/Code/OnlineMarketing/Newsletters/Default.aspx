<%@ Page Title="" Language="C#" MasterPageFile="~/CMSAPIExamples/Pages/APIExamplesPage.Master"
    Theme="Default" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="CMSAPIExamples_Code_OnlineMarketing_Newsletters_Default" %>

<%@ Register Src="~/CMSAPIExamples/Controls/APIExample.ascx" TagName="APIExample"
    TagPrefix="cms" %>
<asp:Content ID="contentLeft" ContentPlaceHolderID="plcLeftContainer" runat="server">
    <cms:LocalizedHeading ID="headaEmailTemplates" runat="server" Text="Email templates" Level="4" EnableViewState="false" />
    <%-- Subscription template --%>
    <cms:LocalizedHeading ID="headCreateSubscriptionTemplate" runat="server" Text="Subscription template" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiCreateSubscriptionTemplate" runat="server" ButtonText="Create template"
        InfoMessage="Template 'My new subscription template' was created." />
    <%-- Unsubscription template --%>
    <cms:LocalizedHeading ID="headCreateUnsubscriptionTemplate" runat="server" Text="Unsubscription template" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiCreateUnsubscriptionTemplate" runat="server" ButtonText="Create template"
        InfoMessage="Template 'My new unsubscription template' was created." />
    <%-- Issue template --%>
    <cms:LocalizedHeading ID="headCreateIssueTemplate" runat="server" Text="Issue template" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiCreateIssueTemplate" runat="server" ButtonText="Create template" InfoMessage="Template 'My new issue template' was created." />
    <cms:APIExample ID="apiGetAndUpdateIssueTemplate" runat="server" ButtonText="Get and update template"
        APIExampleType="ManageAdditional" InfoMessage="Template 'My new issue template' was updated."
        ErrorMessage="Template 'My new issue template' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateIssueTemplates" runat="server" ButtonText="Get and bulk update templates"
        APIExampleType="ManageAdditional" InfoMessage="All templates matching the condition were updated."
        ErrorMessage="Templates matching the condition were not found." />
    <cms:LocalizedHeading ID="headaNewsletters" runat="server" Text="Newsletters" Level="4" EnableViewState="false" />
    <%-- Static newsletter --%>
    <cms:LocalizedHeading ID="headCreateStaticNewsletter" runat="server" Text="Static newsletter" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiCreateStaticNewsletter" runat="server" ButtonText="Create newsletter" InfoMessage="Newsletter 'My new static newsletter' was created."
        ErrorMessage="Template 'My new subscription template', template 'My new unsubscription template' or template 'My new issue template' were not found." />
    <cms:APIExample ID="apiGetAndUpdateStaticNewsletter" runat="server" ButtonText="Get and update newsletter"
        APIExampleType="ManageAdditional" InfoMessage="Newsletter 'My new static newsletter' was updated."
        ErrorMessage="Newsletter 'My new static newsletter' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateStaticNewsletters" runat="server" ButtonText="Get and bulk update newsletters"
        APIExampleType="ManageAdditional" InfoMessage="All newsletters matching the condition were updated."
        ErrorMessage="Newsletters matching the condition were not found." />
    <%-- Dynamic newsletter --%>
    <cms:LocalizedHeading ID="headCreateDynamicNewsletter" runat="server" Text="Dynamic newsletter" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiCreateDynamicNewsletter" runat="server" ButtonText="Create newsletter" InfoMessage="Newsletter 'My new dynamic newsletter' was created."
        ErrorMessage="Template 'My new subscription template' or template 'My new unsubscription template' were not found." />
    <cms:APIExample ID="apiGetAndUpdateDynamicNewsletter" runat="server" ButtonText="Get and update newsletter"
        APIExampleType="ManageAdditional" InfoMessage="Newsletter 'My new dynamic newsletter' was updated."
        ErrorMessage="Newsletter 'My new dynamic newsletter' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateDynamicNewsletters" runat="server" ButtonText="Get and bulk update newsletters"
        APIExampleType="ManageAdditional" InfoMessage="All newsletters matching the condition were updated."
        ErrorMessage="Newsletters matching the condition were not found." />
    <cms:LocalizedHeading ID="headaSubscribers" runat="server" Text="Subscribers" Level="4" EnableViewState="false" />
    <%-- Subscriber --%>
    <cms:LocalizedHeading ID="headCreateSubscriber" runat="server" Text="Subscriber" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiCreateSubscriber" runat="server" ButtonText="Create subscriber"
        InfoMessage="Subscriber 'subscriber@localhost.local' was created." />
    <cms:APIExample ID="apiGetAndUpdateSubscriber" runat="server" ButtonText="Get and update subscriber"
        APIExampleType="ManageAdditional" InfoMessage="Subscriber 'subscriber@localhost.local' was updated."
        ErrorMessage="Subscriber 'subscriber@localhost.local' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateSubscribers" runat="server" ButtonText="Get and bulk update subscribers"
        APIExampleType="ManageAdditional" InfoMessage="All subscribers matching the condition were updated."
        ErrorMessage="Subscribers matching the condition were not found." />
    <%-- Newsletter subscriber --%>
    <cms:LocalizedHeading ID="headSubscribeToNewsletter" runat="server" Text="Newsletter subscriber" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiSubscribeToNewsletter" runat="server" ButtonText="Subscribe to newsletter" InfoMessage="Subscriber 'subscriber@localhost.local' was subscribed to 'My new static newsletter'."
        ErrorMessage="Subscriber 'subscriber@localhost.local' or newsletter 'My new static newsletter' were not found." />
    <cms:LocalizedHeading ID="headaIssues" runat="server" Text="Issues" Level="4" EnableViewState="false" />
    <%-- Static issue --%>
    <cms:LocalizedHeading ID="headCreateStaticIssue" runat="server" Text="Static issue" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiCreateStaticIssue" runat="server" ButtonText="Create issue" InfoMessage="Issue 'My new static issue' was created." ErrorMessage="Newsletter 'My new static newsletter' was not found." />
    <cms:APIExample ID="apiGetAndUpdateStaticIssue" runat="server" ButtonText="Get and update issue" APIExampleType="ManageAdditional" InfoMessage="Issue 'My new static issue' was updated." ErrorMessage="Issue 'My new static issue' or newsletter 'My new static newsletter' were not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateStaticIssues" runat="server" ButtonText="Get and bulk update issues" APIExampleType="ManageAdditional" InfoMessage="All issues matching the condition were updated." ErrorMessage="Issues matching the condition or newsletter 'My new static newsletter' were not found." />
    <%-- Dynamic issue --%>
    <cms:LocalizedHeading ID="headCreateDynamicIssue" runat="server" Text="Dynamic issue" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiCreateDynamicIssue" runat="server" ButtonText="Create issue" InfoMessage="Issue 'My new dynamic issue' was created." ErrorMessage="Newsletter 'My new dynamic newsletter' was not found" />
    <cms:APIExample ID="apiGetAndUpdateDynamicIssue" runat="server" ButtonText="Get and update issue" APIExampleType="ManageAdditional" InfoMessage="Issue 'My new dynamic issue' was updated." ErrorMessage="Issue 'My new dynamic issue' or newsletter 'My new dynamic newsletter' were not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateDynamicIssues" runat="server" ButtonText="Get and bulk update issues" APIExampleType="ManageAdditional" InfoMessage="All issues matching the condition were updated." ErrorMessage="Issues matching the condition or newsletter 'My new dynamic newsletter' were not found." />
</asp:Content>
<asp:Content ID="contentRight" ContentPlaceHolderID="plcRightContainer" runat="server">
    <cms:LocalizedHeading ID="headCleanIssues" runat="server" Text="Issues" Level="4" EnableViewState="false" />
    <%-- Dynamic issue --%>
    <cms:LocalizedHeading ID="headDeleteDynamicIssue" runat="server" Text="Dynamic issue" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteDynamicIssue" runat="server" ButtonText="Delete issue" APIExampleType="CleanUpMain" InfoMessage="Issue 'My new dynamic issue' and all its dependencies were deleted." ErrorMessage="Issue 'My new dynamic issue' or newsletter 'My new dynamic newsletter' were not found." />
    <%-- Static issue --%>
    <cms:LocalizedHeading ID="headDeleteStaticIssue" runat="server" Text="Static issue" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteStaticIssue" runat="server" ButtonText="Delete issue" APIExampleType="CleanUpMain" InfoMessage="Issue 'My new static issue' and all its dependencies were deleted." ErrorMessage="Issue 'My new static issue' or newsletter 'My new static newsletter' were not found." />
    <cms:LocalizedHeading ID="headCleanSubscribers" runat="server" Text="Subscribers" Level="4" EnableViewState="false" />
    <%-- Newsletter subscriber --%>
    <cms:LocalizedHeading ID="headUnsubscribeSubscriber" runat="server" Text="Newsletter subscriber" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiUnsubscribeFromNewsletter" runat="server" ButtonText="Unsubscribe from newsletter" APIExampleType="CleanUpMain" InfoMessage="Subscriber 'subscriber@localhost.local' was unsubscribed from 'My new static newsletter'."
        ErrorMessage="Subscriber 'subscriber@localhost.local' or newsletter 'My new static newsletter' were not found." />
    <%-- Subscriber --%>
    <cms:LocalizedHeading ID="headDeleteSubscriber" runat="server" Text="Subscriber" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteSubscriber" runat="server" ButtonText="Delete subscriber" APIExampleType="CleanUpMain"
        InfoMessage="Subscriber 'subscriber@localhost.local' and all its dependencies were deleted."
        ErrorMessage="Subscriber 'subscriber@localhost.local' was not found." />
    <cms:LocalizedHeading ID="headCleanNewsletters" runat="server" Text="Newsletters" Level="4" EnableViewState="false" />
    <%-- Dynamic newsletter --%>
    <cms:LocalizedHeading ID="headDeleteDynamicNewsletter" runat="server" Text="Dynamic newsletter" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteDynamicNewsletter" runat="server" ButtonText="Delete newsletter"
        APIExampleType="CleanUpMain" InfoMessage="Newsletter 'My new dynamic newsletter' and all its dependencies were deleted."
        ErrorMessage="Newsletter 'My new dynamic newsletter' was not found." />
    <%-- Static newsletter --%>
    <cms:LocalizedHeading ID="headDeleteStaticNewsletter" runat="server" Text="Static newsletter" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteStaticNewsletter" runat="server" ButtonText="Delete newsletter"
        APIExampleType="CleanUpMain" InfoMessage="Newsletter 'My new static newsletter' and all its dependencies were deleted."
        ErrorMessage="Newsletter 'My new static newsletter' was not found." />
    <cms:LocalizedHeading ID="headCleanEmailTemplates" runat="server" Text="Email templates" Level="4" EnableViewState="false" />
    <%-- Subscription template --%>
    <cms:LocalizedHeading ID="headDeleteSubscriptionTemplate" runat="server" Text="Subscription template" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteSubscriptionTemplate" runat="server" ButtonText="Delete template"
        APIExampleType="CleanUpMain" InfoMessage="Template 'My new subscription template' and all its dependencies were deleted."
        ErrorMessage="Template 'My new subscription template' was not found." />
    <%-- Unsubscription template --%>
    <cms:LocalizedHeading ID="headDeleteUnsubscriptionTemplate" runat="server" Text="Unsubscription template" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteUnsubscriptionTemplate" runat="server" ButtonText="Delete template"
        APIExampleType="CleanUpMain" InfoMessage="Template 'My new unsubscription template' and all its dependencies were deleted."
        ErrorMessage="Template 'My new unsubscription template' was not found." />
    <%-- Issue template --%>
    <cms:LocalizedHeading ID="headDeleteIssueTemplate" runat="server" Text="Issue template" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteIssueTemplate" runat="server" ButtonText="Delete template"
        APIExampleType="CleanUpMain" InfoMessage="Template 'My new issue template' and all its dependencies were deleted."
        ErrorMessage="Template 'My new issue template' was not found." />
</asp:Content>
