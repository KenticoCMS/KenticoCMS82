<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="Project properties" Inherits="CMSModules_ProjectManagement_Pages_Tools_Project_Edit" Theme="Default" CodeFile="Edit.aspx.cs" %>
<%@ Register Src="~/CMSModules/ProjectManagement/Controls/UI/Project/Edit.ascx"
    TagName="ProjectEdit" TagPrefix="cms" %>
    
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <asp:Literal ID="ltlItem" runat="server"></asp:Literal>
    <cms:ProjectEdit ID="editElem" runat="server" IsLiveSite="false" />
</asp:Content>
