<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSWebParts_Wireframe_Components_WYSIWYG"
    CodeFile="~/CMSWebParts/Wireframe/Components/WYSIWYG.ascx.cs" %>
<asp:Panel runat="server" ID="pnlEditor" CssClass="WireframeWYSIWYG">
    <cms:EditableWebPartColor runat="server" CssClass="WireframeWYSIWYGToolbar" ID="toolbarElem">
        &nbsp;
    </cms:EditableWebPartColor>
    <cms:WebPartResizer runat="server" ID="resToolbar" VerticalOnly="true" RenderEnvelope="true" HeightPropertyName="ToolbarHeight" />
    <cms:EditableWebPartProperty runat="server" ID="textElem" CssClass="WireframeWYSIWYGContent"
        Type="FormControl" FormControl="WireframeHTMLArea" Encode="false" PropertyName="Text" />
</asp:Panel>
<cms:WebPartResizer runat="server" ID="resElem" RenderEnvelope="true" />
