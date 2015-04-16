<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Controls_MyDetails_MyDetails"
    CodeFile="MyDetails.ascx.cs" %>

<%@ Register Src="~/CMSModules/ECommerce/FormControls/CurrencySelector.ascx" TagName="CurrencySelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/ECommerce/FormControls/PaymentSelector.ascx" TagName="PaymentSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/ECommerce/FormControls/ShippingSelector.ascx" TagName="ShippingSelector"
    TagPrefix="cms" %>

<asp:Panel ID="pnlMyDetails" runat="server" DefaultButton="btnOK">
    <cms:MessagesPlaceHolder ID="plcMessages" runat="server" />
    <div class="form-horizontal">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ResourceString="Customers_Edit.lblCompanyAccount" ID="lblCompanyAccount" runat="server" AssociatedControlID="radAccType"
                    EnableViewState="false" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSRadioButtonList runat="server" ID="radAccType" RepeatDirection="Horizontal" AutoPostBack="True" />
            </div>
        </div>
         <asp:PlaceHolder runat="server" ID="plcCompanyInfo" Visible="false">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ResourceString="Customers_Edit.CustomerCompanyLabel" ID="lblCustomerCompany" AssociatedControlID="txtCustomerCompany"
                        EnableViewState="false" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSTextBox ID="txtCustomerCompany" runat="server" MaxLength="200" EnableViewState="true" />
                </div>
            </div>
            <asp:PlaceHolder ID="plhOrganizationID" runat="server" EnableViewState="false">
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblOrganizationID" AssociatedControlID="txtOraganizationID"
                            EnableViewState="false" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSTextBox ID="txtOraganizationID" runat="server" MaxLength="50" EnableViewState="false" />
                    </div>
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plhTaxRegistrationID" runat="server" EnableViewState="false">
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblTaxRegistrationID" AssociatedControlID="txtTaxRegistrationID"
                            EnableViewState="false" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSTextBox ID="txtTaxRegistrationID" runat="server" MaxLength="50" EnableViewState="false" />
                    </div>
                </div>
            </asp:PlaceHolder>
        </asp:PlaceHolder>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ResourceString="Customers_Edit.CustomerFirstNameLabel" runat="server" ID="lblCustomerFirstName" AssociatedControlID="txtCustomerFirstName" EnableViewState="false" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox ID="txtCustomerFirstName" runat="server" MaxLength="200" EnableViewState="false" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ResourceString="Customers_Edit.CustomerLastNameLabel" ID="lblCustomerLastName" AssociatedControlID="txtCustomerLastName" EnableViewState="false" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox ID="txtCustomerLastName" runat="server" MaxLength="200" EnableViewState="false" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ResourceString="Customers_Edit.email" runat="server" ID="lblCustomerEmail" AssociatedControlID="txtCustomerEmail"
                    EnableViewState="false" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox ID="txtCustomerEmail" runat="server" MaxLength="200" EnableViewState="false" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ResourceString="customers_edit.CustomerPhone" runat="server" ID="lblCustomerPhone" AssociatedControlID="txtCustomerPhone"
                    EnableViewState="false" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox ID="txtCustomerPhone" runat="server" MaxLength="50" EnableViewState="false" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ResourceString="customers_edit.CustomerCurrency" runat="server" ID="lblCustomerPreferredCurrency"
                     AssociatedControlID="selectCurrency" EnableViewState="false" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CurrencySelector ID="selectCurrency" runat="server" UseNameForSelection="false" AddAllItemsRecord="false" AddNoneRecord="true"
                    DisplayOnlyWithExchangeRate="true" DisplayOnlyEnabled="true" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ResourceString="customers_edit.CustomerShipping" runat="server" ID="lblCustomerPreferredShippingOption" 
                    AssociatedControlID="drpShipping" EnableViewState="false" />
            </div>
            <div class="editing-form-value-cell">
                <cms:ShippingSelector ID="drpShipping" runat="server" AddNoneRecord="true" EnsureSelectedItem="false" AddAllItemsRecord="false"
                    AutoPostBack="true" OnChanged="drpShipping_ShippingChange" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ResourceString="customers_edit.CustomerPayment" runat="server" ID="lblCustomerPrefferedPaymentOption"
                    AssociatedControlID="drpPayment" EnableViewState="false" />
            </div>
            <div class="editing-form-value-cell">
                <cms:PaymentSelector ID="drpPayment" runat="server" AddNoneRecord="true" AddAllItemsRecord="false" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-value-cell editing-form-value-cell-offset">
                <cms:CMSButton runat="server" ID="btnOk" OnClick="btnOK_Click" EnableViewState="false"
                    ButtonStyle="Primary" />
            </div>
        </div>
    </div>
</asp:Panel>