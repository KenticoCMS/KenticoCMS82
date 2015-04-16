<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSWebParts_Wireframe_Text_Note" CodeFile="~/CMSWebParts/Wireframe/Text/Note.ascx.cs" %>
<div class="WireframeNoteHeaderBack">
    <cms:EditableWebPartColor runat="server" CssClass="WireframeNoteHeader" id="colElem">
        <cms:EditableWebPartProperty runat="server" id="ltlMark" PropertyName="MarkText" Type="TextBox" />
    </cms:EditableWebPartColor>
</div>
<cms:EditableWebPartProperty runat="server" id="ltlText" CssClass="WireframeNote" PropertyName="Text" Type="TextArea" />
<cms:WebPartResizer runat="server" id="resElem" RenderEnvelope="true" />
