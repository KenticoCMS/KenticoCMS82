<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Content_Controls_Dialogs_General_MenuButton" CodeFile="MenuButton.ascx.cs" %>
<asp:Panel ID="pnlMain" runat="server" EnableViewState="false">
    <asp:Image ID="imgIcon" runat="server" EnableViewState="false" />
    <cms:LocalizedLabel ID="lblText" runat="server" EnableViewState="false" />
    <asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
</asp:Panel>
<asp:Button ID="btnMenu" runat="server" EnableViewState="false" OnClick="btnMenu_Click" CssClass="HiddenButton" />
