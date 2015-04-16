<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Blogs_FormControls_NotPingedUrls" CodeFile="NotPingedUrls.ascx.cs" %>
<asp:Panel ID="pnlTextarea" runat="server">
    <cms:CMSTextArea ID="txtSendTo" runat="server" Width="100%" EnableViewState="false" Rows="3" />
    <cms:LocalizedLabel ID="lblSendTo" runat="server" ResourceString="blogs.trackbacks.sendto"
        EnableViewState="false" CssClass="explanation-text" />
</asp:Panel>
