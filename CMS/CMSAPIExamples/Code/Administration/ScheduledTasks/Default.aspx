<%@ Page Title="" Language="C#" MasterPageFile="~/CMSAPIExamples/Pages/APIExamplesPage.Master"
    Theme="Default" AutoEventWireup="true" Inherits="CMSAPIExamples_Code_Administration_ScheduledTasks_Default" CodeFile="Default.aspx.cs" %>

<%@ Register Src="~/CMSAPIExamples/Controls/APIExample.ascx" TagName="APIExample" TagPrefix="cms" %>
<asp:Content ID="contentLeft" ContentPlaceHolderID="plcLeftContainer" runat="server">
    <%-- Scheduled task --%>
    <cms:LocalizedHeading ID="pnlCreateScheduledTask" runat="server" Text="Scheduled task" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreateScheduledTask" runat="server" ButtonText="Create task" InfoMessage="Task 'My new task' was created." />
    <cms:APIExample ID="apiGetAndUpdateScheduledTask" runat="server" ButtonText="Get and update task" APIExampleType="ManageAdditional" InfoMessage="Task 'My new task' was updated." ErrorMessage="Task 'My new task' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateScheduledTasks" runat="server" ButtonText="Get and bulk update tasks" APIExampleType="ManageAdditional" InfoMessage="All tasks matching the condition were updated." ErrorMessage="Tasks matching the condition were not found." />
    <cms:APIExample ID="apiRunTask" runat="server" ButtonText="Run task" APIExampleType="ManageAdditional" InfoMessage="The task has been executed." ErrorMessage="Task 'My new task' was not found." />
</asp:Content>
<asp:Content ID="contentRight" ContentPlaceHolderID="plcRightContainer" runat="server">
    <%-- Scheduled task --%>
    <cms:LocalizedHeading ID="pnlDeleteScheduledTask" runat="server" Text="Scheduled task" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteScheduledTask" runat="server" ButtonText="Delete task" APIExampleType="CleanUpMain" InfoMessage="Task 'My new task' and all its dependencies were deleted." ErrorMessage="Task 'My new task' was not found." />
</asp:Content>
