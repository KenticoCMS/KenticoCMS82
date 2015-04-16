<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MultiObjectBindingControl.ascx.cs" Inherits="CMSFormControls_System_MultiObjectBindingControl" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>

<cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
    <ContentTemplate>
        <cms:UniSelector ID="uniSelector" runat="server" SelectionMode="Multiple" />
    </ContentTemplate>
</cms:CMSUpdatePanel>
