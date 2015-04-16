<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/CMSWebParts/Wireframe/Layouts/Tabs.ascx.cs"
    Inherits="CMSWebParts_Wireframe_Layouts_Tabs" %>
<asp:Panel runat="server" ID="pnlActions" CssClass="WireframeActions WireframeAccordionActions"
    EnableViewState="false" Visible="false">
    <asp:Panel runat="server" ID="pnlHandle" CssClass="WebPartHandle WireframeActionsInner">
             <asp:Literal runat="server" ID="ltlActions" EnableViewState="false" />
    </asp:Panel>
</asp:Panel>
<ajaxToolkit:TabContainer runat="server" ID="tabs" />
<cms:WebPartResizer runat="server" ID="resElem" RenderEnvelope="true" WidthPropertyName="Width" HorizontalOnly="true" />
