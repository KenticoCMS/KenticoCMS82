<%@ Page Title="" Language="C#" MasterPageFile="~/CMSAPIExamples/Pages/APIExamplesPage.Master"
    Theme="Default" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="CMSAPIExamples_Code_Settings_Default" %>

<%@ Register Src="~/CMSAPIExamples/Controls/APIExample.ascx" TagName="APIExample" TagPrefix="cms" %>
<asp:Content ID="contentLeft" ContentPlaceHolderID="plcLeftContainer" runat="server">
    <%-- Settings category --%>
    <cms:LocalizedHeading ID="headCreateSettingsCategory" runat="server" Text="Settings category" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreateSettingsCategory" runat="server" ButtonText="Create category" InfoMessage="Category 'My new category' was created." ErrorMessage="Category 'CMS.CustomSettings' was not found." />
    <cms:APIExample ID="apiGetAndUpdateSettingsCategory" runat="server" ButtonText="Get and update category" APIExampleType="ManageAdditional" InfoMessage="Category 'My new category' was updated." ErrorMessage="Category 'My new category' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateSettingsCategories" runat="server" ButtonText="Get and bulk update categories" APIExampleType="ManageAdditional" InfoMessage="All categories matching the condition were updated." ErrorMessage="Categories matching the condition were not found." />
    <%-- Settings group --%>
    <cms:LocalizedHeading ID="headCreateSettingsGroup" runat="server" Text="Settings group" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreateSettingsGroup" runat="server" ButtonText="Create group" InfoMessage="Group 'My new group' was created." ErrorMessage="Category 'My new category' was not found." />
    <cms:APIExample ID="apiGetAndUpdateSettingsGroup" runat="server" ButtonText="Get and update group" APIExampleType="ManageAdditional" InfoMessage="Group 'My new group' was updated." ErrorMessage="Group 'My new group' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateSettingsGroups" runat="server" ButtonText="Get and bulk update groups" APIExampleType="ManageAdditional" InfoMessage="All groups matching the condition were updated." ErrorMessage="Groups matching the condition were not found." />
    <%-- Settings key --%>
    <cms:LocalizedHeading ID="headCreateSettingsKey" runat="server" Text="Settings key" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreateSettingsKey" runat="server" ButtonText="Create key" InfoMessage="Key 'My new key' was created." ErrorMessage="Group 'My new group' was not found." />
    <cms:APIExample ID="apiGetAndUpdateSettingsKey" runat="server" ButtonText="Get and update key" APIExampleType="ManageAdditional" InfoMessage="Key 'My new key' was updated." ErrorMessage="Key 'My new key' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateSettingsKeys" runat="server" ButtonText="Get and bulk update Keys" APIExampleType="ManageAdditional" InfoMessage="All keys matching the condition were updated." ErrorMessage="Keys matching the condition were not found." />
    <%-- Web.config key --%>
    <cms:LocalizedHeading ID="headWebConfigSetting" runat="server" Text="Web.config setting" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiGetWebConfigSetting" runat="server" ButtonText="Get web.config setting" APIExampleType="ManageAdditional" ErrorMessage="Web.config key was not found." />
</asp:Content>
<asp:Content ID="contentRight" ContentPlaceHolderID="plcRightContainer" runat="server">
    <%-- Settings key --%>
    <cms:LocalizedHeading ID="headDeleteSettingsKey" runat="server" Text="Settings key" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteSettingsKey" runat="server" ButtonText="Delete key" APIExampleType="CleanUpMain" InfoMessage="Key 'My new key' and all its dependencies were deleted." ErrorMessage="Key 'My new key' was not found." />
    <%-- Settings group --%>
    <cms:LocalizedHeading ID="headDeleteSettingsGroup" runat="server" Text="Settings group" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteSettingsGroup" runat="server" ButtonText="Delete group" APIExampleType="CleanUpMain" InfoMessage="Group 'My new group' and all its dependencies were deleted." ErrorMessage="Group 'My new group' was not found." />
    <%-- Settings category --%>
    <cms:LocalizedHeading ID="headDeleteSettingsCategory" runat="server" Text="Settings category" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteSettingsCategory" runat="server" ButtonText="Delete category" APIExampleType="CleanUpMain" InfoMessage="Category 'My new category' and all its dependencies were deleted." ErrorMessage="Category 'My new category' was not found." />
</asp:Content>
