<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/CMSWebParts/Wireframe/WireframeListComponent.ascx.cs" Inherits="CMSWebParts_Wireframe_WireframeListComponent" %>
<cms:CMSPanel runat="server" ID="pnlEnvelope" RenderChildrenOnly="true">
    <cms:EditableWebPartList runat="server" ID="ltlText" CssClass="WireframeText" PropertyName="Items" Type="TextArea" />
</cms:CMSPanel>
<cms:WebPartResizer runat="server" ID="resElem" RenderEnvelope="true" />