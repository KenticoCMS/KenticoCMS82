<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSWebParts_Wireframe_Forms_MultilineButton" CodeFile="~/CMSWebParts/Wireframe/Forms/MultilineButton.ascx.cs" %>
<asp:Panel runat="server" ID="pnlButton" CssClass="WireframeMultilineButton">
    <cms:EditableWebPartProperty runat="server" id="ltlMainText" CssClass="MultilineButtonMain" PropertyName="MainText" Type="TextBox" />
    <cms:EditableWebPartProperty runat="server" id="ltlText" PropertyName="Text" Type="TextArea" />
</asp:Panel>
<cms:WebPartResizer runat="server" id="resElem" />