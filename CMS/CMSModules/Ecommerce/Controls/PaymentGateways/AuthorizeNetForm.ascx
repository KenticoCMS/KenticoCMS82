<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Controls_PaymentGateways_AuthorizeNetForm"
    CodeFile="AuthorizeNetForm.ascx.cs" %>

<div class="h4"><%= GetString("AuthorizeNetForm.Title") %></div>
<asp:Label ID="lblError" runat="server" EnableViewState="false" CssClass="ErrorLabel"
    Visible="false" />
<div class="form-horizontal">
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ResourceString="AuthorizeNetForm.CardNumber" ID="lblCardNumber" runat="server"
                ShowRequiredMark="True" EnableViewState="false" AssociatedControlID="txtCardNumber" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSTextBox ID="txtCardNumber" runat="server"  MaxLength="100"
                EnableViewState="false" autocomplete="off" />
            <cms:CMSRequiredFieldValidator ID="rfvCardNumber" runat="server" ControlToValidate="txtCardNumber"
                Display="Dynamic" ValidationGroup="ButtonNext" EnableViewState="false" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ResourceString="AuthorizeNetForm.CardCCV" ID="lblCCV" runat="server"
                AssociatedControlID="txtCCV" ShowRequiredMark="True" EnableViewState="false" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSTextBox ID="txtCCV" runat="server"  MaxLength="100"
                EnableViewState="false" autocomplete="off" />
            <cms:CMSRequiredFieldValidator ID="rfvCCV" runat="server" ControlToValidate="txtCCV"
                Display="Dynamic" ValidationGroup="ButtonNext" EnableViewState="false" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ResourceString="AuthorizeNetForm.CardExpiration" ID="lblExpiration" runat="server" EnableViewState="false" ShowRequiredMark="True"  AssociatedControlID="drpMonths"/>
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSDropDownList ID="drpMonths" runat="server" />
            <cms:CMSDropDownList ID="drpYears" runat="server" />            
            <asp:Label ID="lblErrorDate" runat="server" EnableViewState="false" ForeColor="Red" />
        </div>
    </div>
</div>