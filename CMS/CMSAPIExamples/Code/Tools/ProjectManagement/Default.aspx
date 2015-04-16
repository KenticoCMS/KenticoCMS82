<%@ Page Title="" Language="C#" MasterPageFile="~/CMSAPIExamples/Pages/APIExamplesPage.Master"
    Theme="Default" AutoEventWireup="true" Inherits="CMSAPIExamples_Code_Tools_ProjectManagement_Default" CodeFile="Default.aspx.cs" %>

<%@ Register Src="~/CMSAPIExamples/Controls/APIExample.ascx" TagName="APIExample" TagPrefix="cms" %>
<asp:Content ID="contentLeft" ContentPlaceHolderID="plcLeftContainer" runat="server">
    <%-- Project --%>
    <cms:LocalizedHeading ID="headCreateProject" runat="server" Text="Project" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreateProject" runat="server" ButtonText="Create project" InfoMessage="Project 'My new project' was created." ErrorMessage="Project status not found." />
    <cms:APIExample ID="apiGetAndUpdateProject" runat="server" ButtonText="Get and update project" APIExampleType="ManageAdditional" InfoMessage="Project 'My new project' was updated." ErrorMessage="Project 'My new project' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateProjects" runat="server" ButtonText="Get and bulk update projects" APIExampleType="ManageAdditional" InfoMessage="All projects matching the condition were updated." ErrorMessage="Projects matching the condition were not found." />
    <%-- Project task --%>
    <cms:LocalizedHeading ID="headCreateProjectTask" runat="server" Text="Project task" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreateProjectTask" runat="server" ButtonText="Create task" InfoMessage="Task 'My new task' was created." />
    <cms:APIExample ID="apiGetAndUpdateProjectTask" runat="server" ButtonText="Get and update task" APIExampleType="ManageAdditional" InfoMessage="Task 'My new task' was updated." ErrorMessage="Task 'My new task' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateProjectTasks" runat="server" ButtonText="Get and bulk update tasks" APIExampleType="ManageAdditional" InfoMessage="All tasks matching the condition were updated." ErrorMessage="Tasks matching the condition were not found." />
    <%-- Project security --%>
    <cms:LocalizedHeading ID="headProjectSecurity" runat="server" Text="Project's security" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiSetSecurity" runat="server" ButtonText="Set project's security" APIExampleType="ManageAdditional" InfoMessage="Project 'My new project' was updated." ErrorMessage="Project 'My new project' was not found." />
    <cms:APIExample ID="apiAddAuthorizedRole" runat="server" ButtonText="Add authorized role" APIExampleType="ManageAdditional" InfoMessage="Project 'My new project' was updated." ErrorMessage="Project 'My new project' was not found." />
</asp:Content>
<asp:Content ID="contentRight" ContentPlaceHolderID="plcRightContainer" runat="server">
    <%-- Project security --%>
    <cms:LocalizedHeading ID="headRemoveProjectSecurity" runat="server" Text="Project's security" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiRemoveAuthorizedRole" runat="server" ButtonText="Remove authorized role" APIExampleType="CleanUpMain" InfoMessage="Role was removed from project 'My new project'." ErrorMessage="Project 'My new project' was not found." />
    <%-- Project task --%>
    <cms:LocalizedHeading ID="headDeleteProjectTask" runat="server" Text="Project task" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteProjectTask" runat="server" ButtonText="Delete task" APIExampleType="CleanUpMain" InfoMessage="Task 'My new task' and all its dependencies were deleted." ErrorMessage="Task 'My new task' was not found." />
    <%-- Project --%>
    <cms:LocalizedHeading ID="headDeleteProject" runat="server" Text="Project" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteProject" runat="server" ButtonText="Delete project" APIExampleType="CleanUpMain" InfoMessage="Project 'My new project' and all its dependencies were deleted." ErrorMessage="Project 'My new project' was not found." />
</asp:Content>
