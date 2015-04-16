<%@ Control Language="C#" AutoEventWireup="true" CodeFile="VersionListMenu.ascx.cs"
    Inherits="CMSModules_Objects_Controls_Versioning_VersionListMenu" %>
<%@ Register TagPrefix="cms" Namespace="CMS.UIControls" Assembly="CMS.UIControls" %>
<asp:Panel runat="server" ID="pnlMessageMenu" CssClass="PortalContextMenu WebPartContextMenu">
    <asp:Panel runat="server" ID="pnlRestoreChilds" CssClass="Item">
        <asp:Panel runat="server" ID="pnlRestoreChildsPadding" CssClass="ItemPadding">
            <cms:LocalizedLabel runat="server" ID="lblRestoreChild" CssClass="Name" EnableViewState="false"
                ResourceString="objectversioning.recyclebin.rollbackwithchilds" />
        </asp:Panel>
    </asp:Panel>
</asp:Panel>
