<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DevTools.ascx.cs" Inherits="CMSAdminControls_UI_Development_DevTools" %>
    <div class="dev-tools cms-bootstrap">
        <div class="dev-tools-content">
            <asp:Literal runat="server" ID="ltlActions" EnableViewState="False" />
            <asp:Literal runat="server" ID="ltlDebug" EnableViewState="False" />
            <cms:ContextMenuButton runat="server" ID="btnLocalize"><asp:Literal runat="server" ID="ltlLocalize" EnableViewState="False" /></cms:ContextMenuButton>
        </div>
    </div>
