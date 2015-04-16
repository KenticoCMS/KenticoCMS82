<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="Project task properties" Inherits="CMSModules_ProjectManagement_Pages_Tools_ProjectTask_Edit" Theme="Default" CodeFile="Edit.aspx.cs" %>
<%@ Register Src="~/CMSModules/ProjectManagement/Controls/UI/ProjectTask/Edit.ascx"
    TagName="ProjectTaskEdit" TagPrefix="cms" %>
    
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:ProjectTaskEdit ID="editElem" runat="server" IsLiveSite="false" />
</asp:Content>