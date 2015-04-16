<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSAdminControls_UI_UniGrid_Controls_ObjectMenu"
    CodeFile="ObjectMenu.ascx.cs" %>
    
<asp:Panel runat="server" ID="pnlObjectMenu" CssClass="PortalContextMenu WebPartContextMenu">
    <cms:ContextMenuItem runat="server" ID="iRelationships" />
    <cms:ContextMenuSeparator runat="server" ID="sep1" />
    <cms:ContextMenuItem runat="server" ID="iClone" /> 
    <cms:ContextMenuItem runat="server" ID="iDestroy" /> 
    <cms:ContextMenuSeparator runat="server" ID="sep2" />
    <cms:ContextMenuItem runat="server" ID="iExport" />
    <cms:ContextMenuItem runat="server" ID="iBackup" />
    <cms:ContextMenuItem runat="server" ID="iRestore" />
</asp:Panel>
