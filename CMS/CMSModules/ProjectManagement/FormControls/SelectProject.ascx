<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_ProjectManagement_FormControls_SelectProject" CodeFile="SelectProject.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<cms:CMSUpdatePanel runat="server" ID="pnlUpdate">
    <ContentTemplate>
        <cms:UniSelector ID="usProjects" runat="server" ObjectType="PM.Project" SelectionMode="MultipleTextBox"
            ReturnColumnName="ProjectName" />
    </ContentTemplate>
</cms:CMSUpdatePanel>
