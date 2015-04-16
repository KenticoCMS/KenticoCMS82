<%@ Page Language="C#" AutoEventWireup="true"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Project task list"
    Inherits="CMSModules_Groups_Tools_ProjectManagement_ProjectTask_List" Theme="Default" CodeFile="List.aspx.cs" %>
<%@ Register Src="~/CMSModules/ProjectManagement/Controls/UI/ProjectTask/List.ascx" TagName="ProjectTaskList" TagPrefix="cms" %>
    
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:ProjectTaskList ID="listElem" runat="server" IsLiveSite="false" />
</asp:Content>
