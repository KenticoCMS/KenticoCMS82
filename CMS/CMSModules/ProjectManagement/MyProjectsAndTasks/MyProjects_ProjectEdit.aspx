<%@ Page Language="C#" AutoEventWireup="true" Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Inherits="CMSModules_ProjectManagement_MyProjectsAndTasks_MyProjects_ProjectEdit" CodeFile="MyProjects_ProjectEdit.aspx.cs" %>

<%@ Register Src="~/CMSModules/ProjectManagement/Controls/UI/Project/Edit.ascx"
    TagName="ProjectEdit" TagPrefix="cms" %>
    
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:ProjectEdit ID="editElem" runat="server" ShowPageSelector="false" IsLiveSite="false" />
</asp:Content>
