<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CopyDocument.ascx.cs"
    Inherits="CMSModules_DocumentLibrary_Controls_CopyDocument" %>
<div class="DialogPageContent DialogScrollableContent">
    <div class="PageBody">
        <asp:PlaceHolder ID="plcDocumentName" runat="server">
            <div class="form-horizontal">
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="lblDocumentName" runat="server" EnableViewState="false" ResourceString="general.documentname"
                            DisplayColon="true" AssociatedControlID="txtDocumentName" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSTextBox ID="txtDocumentName" runat="server" MaxLength="100" />
                        <cms:CMSRequiredFieldValidator ID="rfvDocumentName" runat="server" ControlToValidate="txtDocumentName"
                            ErrorMessage="*" Display="Static" CssClass="ValidatorMessage" />
                    </div>
                </div>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plcMessage" runat="server" Visible="false">
            <cms:LocalizedLabel ID="lblInfo" runat="server" EnableViewState="false" CssClass="InfoLabel" />
            <cms:LocalizedLabel ID="lblError" runat="server" EnableViewState="false" CssClass="ErrorLabel" />
        </asp:PlaceHolder>
    </div>
</div>
<div class="PageFooterLine">
    <div class="Buttons FloatRight">
        <cms:LocalizedButton ID="btnSave" runat="server" EnableViewState="false" ResourceString="general.save"
            OnClick="btnSave_Click" ButtonStyle="Primary" DisableAfterSubmit="true" />
        <cms:LocalizedButton ID="btnCancel" runat="server" ButtonStyle="Primary" CausesValidation="false" /></div>
    <div class="ClearBoth">
        &nbsp;</div>
</div>
