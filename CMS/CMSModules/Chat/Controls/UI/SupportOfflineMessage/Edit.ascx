<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Edit.ascx.cs" Inherits="CMSModules_Chat_Controls_UI_SupportOfflineMessage_Edit" %>

<div class="form-horizontal">
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblSendersEmail" ResourceString="general.email" DisplayColon="true" AssociatedControlID="txtEmail" />
        </div>
        <div class="editing-form-value-cell">
             <cms:CMSTextBox runat="server" ID="txtEmail" MaxLength="100" />
            <cms:LocalizedLabel runat="server" ID="lblEmailError" ResourceString="EmailInput.ValidationError" CssClass="ErrorLabel" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblSubject" ResourceString="chat.support.offline.subject" DisplayColon="true" AssociatedControlID="txtSubject" />
        </div>
        <div class="editing-form-value-cell">
             <cms:CMSTextBox runat="server" ID="txtSubject" MaxLength="100" />
            <asp:RequiredFieldValidator ID="rfvSubject" Display="Dynamic" runat="server" ControlToValidate="txtSubject" CssClass="ErrorLabel" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblMessage" ResourceString="chat.message" DisplayColon="true" AssociatedControlID="txtMessage" />
        </div>
        <div class="editing-form-value-cell">
             <cms:CMSTextArea runat="server" ID="txtMessage" Rows="4" />
            <asp:RequiredFieldValidator ID="rfvMessage" Display="Dynamic" runat="server" ControlToValidate="txtMessage" CssClass="ErrorLabel" />
        </div>
    </div>
</div>