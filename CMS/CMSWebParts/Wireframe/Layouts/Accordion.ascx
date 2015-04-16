<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/CMSWebParts/Wireframe/Layouts/Accordion.ascx.cs"
    Inherits="CMSWebParts_Wireframe_Layouts_Accordion" %>   
<asp:Panel runat="server" ID="pnlActions" CssClass="WireframeActions WireframeAccordionActions" EnableViewState="false" Visible="false">
    <asp:Panel runat="server" ID="pnlHandle" CssClass="WebPartHandle WireframeActionsInner">
        <asp:Literal runat="server" ID="ltlActions" EnableViewState="false" />
    </asp:Panel>
</asp:Panel>
<ajaxToolkit:Accordion runat="server" ID="acc" />
<cms:WebPartResizer runat="server" ID="resElem" RenderEnvelope="true" WidthPropertyName="Width" HorizontalOnly="true" />
