<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/CMSWebParts/Wireframe/WireframeComponent.ascx.cs" Inherits="CMSWebParts_Wireframe_WireframeComponent" %>
<cms:CMSPanel runat="server" ID="pnlComponents" RenderChildrenOnly="true">
    <cms:CMSPanel runat="server" ID="pnlProperty" RenderChildrenOnly="true">
        <cms:EditableWebPartProperty runat="server" ID="ltlText" PropertyName="Text" />
    </cms:CMSPanel>
    <cms:CMSPanel runat="server" ID="pnlImage" RenderChildrenOnly="true">
        <cms:EditableWebPartImage runat="server" ID="imgElem" PropertyName="ImageUrl" Visible="false" />
    </cms:CMSPanel>
</cms:CMSPanel>
<cms:WebPartResizer runat="server" ID="resElem" RenderEnvelope="true" />