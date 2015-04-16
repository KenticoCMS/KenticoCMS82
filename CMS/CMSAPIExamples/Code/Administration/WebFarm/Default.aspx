<%@ Page Title="" Language="C#" MasterPageFile="~/CMSAPIExamples/Pages/APIExamplesPage.Master"
    Theme="Default" AutoEventWireup="true" Inherits="CMSAPIExamples_Code_Administration_WebFarm_Default" CodeFile="Default.aspx.cs" %>

<%@ Register Src="~/CMSAPIExamples/Controls/APIExample.ascx" TagName="APIExample" TagPrefix="cms" %>
<asp:Content ID="contentLeft" ContentPlaceHolderID="plcLeftContainer" runat="server">
    <%-- Web farm server --%>
    <cms:LocalizedHeading ID="pnlCreateWebFarmServer" runat="server" Text="Web farm server" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreateWebFarmServer" runat="server" ButtonText="Create server" InfoMessage="Server 'My new server' was created." />
    <cms:APIExample ID="apiGetAndUpdateWebFarmServer" runat="server" ButtonText="Get and update server" APIExampleType="ManageAdditional" InfoMessage="Server 'My new server' was updated." ErrorMessage="Server 'My new server' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateWebFarmServers" runat="server" ButtonText="Get and bulk update servers" APIExampleType="ManageAdditional" InfoMessage="All servers matching the condition were updated." ErrorMessage="Servers matching the condition were not found." />
    <%-- Web farm task --%>
    <cms:LocalizedHeading ID="pnlWebFarmTasks" runat="server" Text="Web farm tasks" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreateTask" runat="server" ButtonText="Create task" APIExampleType="ManageAdditional" InfoMessage="Task was created." />
    <cms:APIExample ID="apiRunMyTasks" runat="server" ButtonText="Run my tasks" APIExampleType="ManageAdditional" InfoMessage="All servers matching the condition were updated." ErrorMessage="Servers matching the condition were not found." />
</asp:Content>
<asp:Content ID="contentRight" ContentPlaceHolderID="plcRightContainer" runat="server">
    <%-- Web farm server --%>
    <cms:LocalizedHeading ID="pnlDeleteWebFarmServer" runat="server" Text="Web farm server" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteWebFarmServer" runat="server" ButtonText="Delete server" APIExampleType="CleanUpMain" InfoMessage="Server 'My new server' and all its dependencies were deleted." ErrorMessage="Server 'My new server' was not found." />
</asp:Content>
