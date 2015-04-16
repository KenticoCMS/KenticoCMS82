<%@ Page Title="" Language="C#" MasterPageFile="~/CMSAPIExamples/Pages/APIExamplesPage.Master"
    Theme="Default" AutoEventWireup="true" Inherits="CMSAPIExamples_Code_Documents_Attachments_Default"
    CodeFile="Default.aspx.cs" %>

<%@ Register Src="~/CMSAPIExamples/Controls/APIExample.ascx" TagName="APIExample"
    TagPrefix="cms" %>
<asp:Content ID="contentLeft" ContentPlaceHolderID="plcLeftContainer" runat="server">
    <%-- Preparation --%>
    <cms:LocalizedHeading ID="headPreparation" runat="server" Text="Preparation" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreateExampleDocument" runat="server" ButtonText="Create example page"
        InfoMessage="Example page was created." ErrorMessage="Site root not found." />
    <%-- Inserting attachments --%>
    <cms:LocalizedHeading ID="headInsertAttachment" runat="server" Text="Inserting attachments" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiInsertUnsortedAttachment" runat="server" ButtonText="Insert unsorted attachment"
        InfoMessage="An unsorted attachment was inserted." ErrorMessage="Page was not found." />
    <cms:APIExample ID="apiInsertFieldAttachment" runat="server" ButtonText="Insert field attachment"
        InfoMessage="An attachment was inserted to the MenuItemTeaserImage field." ErrorMessage="An error occurred while inserting the attachment." />
    <%-- Managing attachments --%>
    <cms:LocalizedHeading ID="headManageAttachments" runat="server" Text="Managing attachments" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiMoveAttachmentDown" runat="server" ButtonText="Move attachment down"
        APIExampleType="ManageAdditional" InfoMessage="Attachment was moved down." ErrorMessage="Page was not found." />
    <cms:APIExample ID="apiMoveAttachmentUp" runat="server" ButtonText="Move attachment up"
        APIExampleType="ManageAdditional" InfoMessage="Attachment was moved up." ErrorMessage="Page was not found." />
    <cms:APIExample ID="apiEditMetadata" runat="server" ButtonText="Edit attachment metadata"
        APIExampleType="ManageAdditional" InfoMessage="Attachment metadata was modified."
        ErrorMessage="Page was not found." />
</asp:Content>
<asp:Content ID="contentRight" ContentPlaceHolderID="plcRightContainer" runat="server">
    <%-- Page alias --%>
    <cms:LocalizedHeading ID="headDeleteDocumentAlias" runat="server" Text="Page alias" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteAttachments" runat="server" ButtonText="Delete attachments"
        APIExampleType="CleanUpMain" InfoMessage="All attachments have been deleted."
        ErrorMessage="Page was not found." />
    <cms:APIExample ID="apiDeleteExampleDocument" runat="server" ButtonText="Delete example page"
        APIExampleType="CleanUpMain" InfoMessage="The page has been deleted." ErrorMessage="Page was not found." />
</asp:Content>
