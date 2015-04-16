<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Theme="Default"
    Inherits="CMSModules_ProjectManagement_MyProjectsAndTasks_MyProjects_TaskEdit" CodeFile="MyProjects_TaskEdit.aspx.cs" %>

<%@ Register Src="~/CMSModules/ProjectManagement/Controls/UI/ProjectTask/Edit.ascx"
    TagName="ProjectTaskEdit" TagPrefix="cms" %>
    
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:ProjectTaskEdit ID="editElem" runat="server" IsLiveSite="false" />
</asp:Content>
