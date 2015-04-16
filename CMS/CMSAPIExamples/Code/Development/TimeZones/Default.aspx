<%@ Page Title="" Language="C#" MasterPageFile="~/CMSAPIExamples/Pages/APIExamplesPage.Master"
    Theme="Default" AutoEventWireup="true" Inherits="CMSAPIExamples_Code_Development_TimeZones_Default" CodeFile="Default.aspx.cs" %>

<%@ Register Src="~/CMSAPIExamples/Controls/APIExample.ascx" TagName="APIExample" TagPrefix="cms" %>
<asp:Content ID="contentLeft" ContentPlaceHolderID="plcLeftContainer" runat="server">
    <%-- Timezone --%>
    <cms:LocalizedHeading ID="headCreateTimezone" runat="server" Text="Timezone" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreateTimezone" runat="server" ButtonText="Create timezone" InfoMessage="Timezone 'My new timezone' was created." />
    <cms:APIExample ID="apiGetAndUpdateTimezone" runat="server" ButtonText="Get and update timezone" APIExampleType="ManageAdditional" InfoMessage="Timezone 'My new timezone' was updated." ErrorMessage="Timezone 'My new timezone' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateTimezones" runat="server" ButtonText="Get and bulk update timezones" APIExampleType="ManageAdditional" InfoMessage="All timezones matching the condition were updated." ErrorMessage="Timezones matching the condition were not found." />
    <cms:APIExample ID="apiConvertTime" runat="server" ButtonText="Convert time by user's time zone" APIExampleType="ManageAdditional" InfoMessage="Time was converted." ErrorMessage="Timezone 'My new timezone' was not found." />
</asp:Content>
<asp:Content ID="contentRight" ContentPlaceHolderID="plcRightContainer" runat="server">
    <%-- Timezone --%>
    <cms:LocalizedHeading ID="headDeleteTimezone" runat="server" Text="Timezone" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteTimezone" runat="server" ButtonText="Delete timezone" APIExampleType="CleanUpMain" InfoMessage="Timezone 'My new timezone' and all its dependencies were deleted." ErrorMessage="Timezone 'My new timezone' was not found." />
</asp:Content>
