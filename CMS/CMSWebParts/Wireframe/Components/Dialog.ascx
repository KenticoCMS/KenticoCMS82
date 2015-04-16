<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSWebParts_Wireframe_Components_Dialog" CodeFile="~/CMSWebParts/Wireframe/Components/Dialog.ascx.cs" %>
<asp:Panel runat="server" id="pnlDialog" CssClass="WireframeBox">
    <cms:EditableWebPartProperty runat="server" id="ltlTitle" CssClass="WireframeBoxHeader" PropertyName="Title" Type="TextBox" />
    <cms:EditableWebPartProperty runat="server" id="ltlText" CssClass="WireframeBoxContent" PropertyName="Text" Type="TextArea" />
    <cms:EditableWebPartList runat="server" id="ltlButtons" RenderAsTag="ul" CssClass="WireframeButtons" PropertyName="Buttons" Type="TextArea" />
</asp:Panel>
<cms:WebPartResizer runat="server" id="resElem" RenderEnvelope="true" Visible="false" />