<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/CMSWebParts/Wireframe/Layouts/Area.ascx.cs"
    Inherits="CMSWebParts_Wireframe_Layouts_Area" %>  
<asp:Panel runat="server" ID="pnlActions" CssClass="WireframeActions WireframeAreaActions" EnableViewState="false" Visible="false">
    <asp:Panel runat="server" ID="pnlHandle" CssClass="WebPartHandle WireframeActionsInner"></asp:Panel>
</asp:Panel>
<asp:Panel runat="server" ID="pnlZone" />
<cms:WebPartResizer runat="server" id="resElem" RenderEnvelope="true" WidthPropertyName="Width" HeightPropertyName="Height" />

