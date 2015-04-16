<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSWebParts_Wireframe_Components_Rectangle" CodeFile="~/CMSWebParts/Wireframe/Components/Rectangle.ascx.cs" %>
<cms:EditableWebPartColor runat="server" CssClass="WireframeRectangle" id="colElem">
    <asp:Panel runat="server" ID="pnlImage" CssClass="WireframeRectangleImage">
        &nbsp;
    </asp:Panel>
</cms:EditableWebPartColor>
<cms:WebPartResizer runat="server" id="resElem" RenderEnvelope="true" />
