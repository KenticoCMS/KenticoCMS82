<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSWebParts_Wireframe_Components_Icon" CodeFile="~/CMSWebParts/Wireframe/Components/Icon.ascx.cs" %>
<div class="WireframeIcon">
    <cms:EditableWebPartImage runat="server" id="imgElem" CssClass="WireframeImage" PropertyName="ImageUrl" />
    <cms:EditableWebPartProperty runat="server" id="ltlText" PropertyName="Label" CssClass="WireframeIconLabel" />
</div>