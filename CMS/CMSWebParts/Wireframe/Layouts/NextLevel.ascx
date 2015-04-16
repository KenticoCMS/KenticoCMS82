<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSWebParts_Wireframe_Layouts_NextLevel" CodeFile="~/CMSWebParts/Wireframe/Layouts/NextLevel.ascx.cs" %>
<asp:Label ID="lblError" runat="server" EnableViewState="false" Visible="false" CssClass="ErrorLabel" />
<asp:Panel runat="server" ID="pnlClass">
    <asp:Panel runat="server" ID="pnlLevel" CssClass="WireframeNextLevel">
        <cms:LocalizedLabel runat="server" ID="lblNextLevel" CssClass="WireframeNextLevelLabel" ResourceString="Wireframe.NextPageLevel" Visible="false" />
        <cms:CMSPagePlaceholder ID="partPlaceholder" runat="server" HidePageTemplateName="true" />
    </asp:Panel>
</asp:Panel>
<cms:WebPartResizer runat="server" id="resElem" RenderEnvelope="true" />

