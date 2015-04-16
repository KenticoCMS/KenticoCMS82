<%@ Page Language="C#" Theme="Default" AutoEventWireup="true" Inherits="CMSModules_ProjectManagement_MyProjectsAndTasks_MyProjects_TasksAssignedToMe"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="My projects and tasks - Tasks assigned to me"
    CodeFile="MyProjects_TasksAssignedToMe.aspx.cs" %>

<%@ Register Src="~/CMSModules/ProjectManagement/Controls/UI/ProjectTask/List.ascx"
    TagName="List" TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:List ID="ucTaskList" runat="server" IgnoreCommunityGroup="true" IsLiveSite="false" />
</asp:Content>
