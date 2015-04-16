<%@ Page Title="" Language="C#" MasterPageFile="~/CMSAPIExamples/Pages/APIExamplesPage.Master"
    Theme="Default" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="CMSAPIExamples_Code_Development_CustomTables_Default" %>

<%@ Register Src="~/CMSAPIExamples/Controls/APIExample.ascx" TagName="APIExample" TagPrefix="cms" %>
<asp:Content ID="contentLeft" ContentPlaceHolderID="plcLeftContainer" runat="server">
    <asp:Label ID="customTableLabel" runat="server" Text="These examples require a custom table 'SampleTable' to be present."></asp:Label>
    <%-- Custom table item --%>
    <cms:LocalizedHeading ID="headCreateCustomTableItem" runat="server" Text="Custom table item" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreateCustomTableItem" runat="server" ButtonText="Create item" InfoMessage="Custom table item with ItemText set to 'New text' was created." ErrorMessage="Custom table 'Sample table' was not found." />
    <cms:APIExample ID="apiGetAndUpdateCustomTableItem" runat="server" ButtonText="Get and update item" APIExampleType="ManageAdditional" InfoMessage="Custom table item was updated." ErrorMessage="Custom table 'SampleTable' or created custom table item was not found." />
    <cms:APIExample ID="apiGetAndMoveCustomTableItemDown" runat="server" ButtonText="Get and move item down" APIExampleType="ManageAdditional" InfoMessage="Custom table item was moved down." ErrorMessage="Custom table 'SampleTable' or created custom table item was not found." />
    <cms:APIExample ID="apiGetAndMoveCustomTableItemUp" runat="server" ButtonText="Get and move item up" APIExampleType="ManageAdditional" InfoMessage="Custom table item was moved up." ErrorMessage="Custom table 'SampleTable' or created custom table item was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateCustomTableItems" runat="server" ButtonText="Get and bulk update items" APIExampleType="ManageAdditional" InfoMessage="All custom table items matching the condition were updated." ErrorMessage="Custom table 'SampleTable' or custom table item matching the condition was not found." />
</asp:Content>
<asp:Content ID="contentRight" ContentPlaceHolderID="plcRightContainer" runat="server">
    <%-- Custom table item --%>
    <cms:LocalizedHeading ID="headDeleteCustomTableItem" runat="server" Text="Custom table item" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteCustomTableItem" runat="server" ButtonText="Delete item" APIExampleType="CleanUpMain" InfoMessage="Custom table item matching the condition was deleted." ErrorMessage="Custom table 'SampleTable' or custom table item matching the condition was not found." />
</asp:Content>
