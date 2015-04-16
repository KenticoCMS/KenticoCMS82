<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSWebParts_Wireframe_Components_Image" CodeFile="~/CMSWebParts/Wireframe/Components/Image.ascx.cs" %>
<cms:CMSPanel runat="server" id="pnlBox" RenderChildrenOnly="True" CssClass="WireframeImageBox">
    <cms:EditableWebPartImage runat="server" id="imgElem" CssClass="WireframeImage" PropertyName="ImageUrl" />
</cms:CMSPanel>
<cms:WebPartResizer runat="server" id="resElem" RenderEnvelope="true" />