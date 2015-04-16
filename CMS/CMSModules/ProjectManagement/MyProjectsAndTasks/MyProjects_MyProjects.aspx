<%@ Page Language="C#" AutoEventWireup="true" Theme="Default"
    Inherits="CMSModules_ProjectManagement_MyProjectsAndTasks_MyProjects_MyProjects"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="My projects and tasks - My projects" CodeFile="MyProjects_MyProjects.aspx.cs" %>
<%@ Register Src="~/CMSModules/ProjectManagement/Controls/UI/Project/List.ascx" TagName="ProjectList"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:ProjectList runat="server" ID="ucProjectList" IgnoreCommunityGroup="true" IsLiveSite="false" />
</asp:Content>
