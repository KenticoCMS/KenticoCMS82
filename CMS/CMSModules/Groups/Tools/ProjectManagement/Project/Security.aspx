<%@ Page Language="C#" AutoEventWireup="true"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Project list"
    Inherits="CMSModules_Groups_Tools_ProjectManagement_Project_Security" Theme="Default" CodeFile="Security.aspx.cs" %>
<%@ Register Src="~/CMSModules/ProjectManagement/Controls/UI/Project/Security.ascx"
    TagName="ProjectSecurity" TagPrefix="cms" %>
    
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:ProjectSecurity ID="security" runat="server" />
</asp:Content>
