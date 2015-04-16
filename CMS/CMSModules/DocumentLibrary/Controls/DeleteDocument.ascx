<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DeleteDocument.ascx.cs"
    Inherits="CMSModules_DocumentLibrary_Controls_DeleteDocument" %>
<div class="DialogPageContent DialogScrollableContent dl-dialog-delete-document">
    <div class="PageBody">
        <asp:PlaceHolder ID="plcConfirmation" runat="server">
            <cms:LocalizedLabel ID="lblConfirmation" runat="server" EnableViewState="false" CssClass="ContentLabel" /><br />
            <cms:CMSCheckBox ID="chkAllCultures" runat="server" ResourceString="contentdelete.allcultures" />
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plcMessage" runat="server" Visible="false">
            <cms:LocalizedLabel ID="lblInfo" runat="server" EnableViewState="false" CssClass="InfoLabel" />
            <cms:LocalizedLabel ID="lblError" runat="server" EnableViewState="false" CssClass="ErrorLabel" />
        </asp:PlaceHolder>
    </div>
</div>
<div class="PageFooterLine">
    <div class="Buttons FloatRight">
        <cms:LocalizedButton ID="btnDelete" runat="server" EnableViewState="false" ResourceString="general.delete"
            OnClick="btnDelete_Click" ButtonStyle="Primary" DisableAfterSubmit="true" />
        <cms:LocalizedButton ID="btnCancel" runat="server" ButtonStyle="Primary" CausesValidation="false" /></div>
    <div class="ClearBoth">
        &nbsp;</div>
</div>
