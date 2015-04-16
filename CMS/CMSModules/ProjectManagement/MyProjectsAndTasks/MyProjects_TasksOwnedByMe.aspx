<%@ Page Language="C#" AutoEventWireup="true" Theme="Default" Inherits="CMSModules_ProjectManagement_MyProjectsAndTasks_MyProjects_TasksOwnedByMe"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="My projects and tasks - Tasks owned by me"
    CodeFile="MyProjects_TasksOwnedByMe.aspx.cs" %>

<%@ Register Src="~/CMSModules/ProjectManagement/Controls/UI/ProjectTask/List.ascx"
    TagName="List" TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:List ID="ucTaskList" runat="server" IgnoreCommunityGroup="true" IsLiveSite="false" />
</asp:Content>
