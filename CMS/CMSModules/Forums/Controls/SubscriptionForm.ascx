<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Forums_Controls_SubscriptionForm" CodeFile="SubscriptionForm.ascx.cs" %>

<asp:Panel runat="server" ID="pnlPadding" CssClass="FormPadding" DefaultButton="btnOK">
    <cms:MessagesPlaceHolder ID="plcMess" runat="server" />
    <div class="form-horizontal">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblEmail" runat="server" EnableViewState="false" ResourceString="general.email"
                    DisplayColon="true" AssociatedControlID="txtEmail" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox ID="txtEmail" runat="server" MaxLength="100" />
                <cms:CMSRegularExpressionValidator ID="rfvEmail" runat="server" ControlToValidate="txtEmail"
                    Display="Dynamic" ValidationGroup="NewSubscription" />
                <cms:CMSRequiredFieldValidator ID="rfvEmailRequired" runat="server" ControlToValidate="txtEmail"
                    Display="Dynamic" ValidationGroup="NewSubscription" />
            </div>
        </div>
        <div class="form-group form-group-submit">
            <cms:CMSButton ID="btnOk" runat="server" ButtonStyle="Primary" ValidationGroup="NewSubscription"
                OnClick="btnOK_Click" />
            <cms:CMSButton ID="btnCancel" runat="server" ButtonStyle="Primary" OnClick="btnCancel_Click" />
        </div>
    </div>
</asp:Panel>
