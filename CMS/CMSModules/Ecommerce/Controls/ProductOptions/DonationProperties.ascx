<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DonationProperties.ascx.cs"
    Inherits="CMSModules_Ecommerce_Controls_ProductOptions_DonationProperties" %>
<%@ Register TagPrefix="cms" TagName="PriceSelector" Src="~/CMSModules/Ecommerce/Controls/UI/PriceSelector.ascx" %>

<div class="form-horizontal">
    <%-- Donation amount --%>
    <asp:PlaceHolder runat="server" ID="plcAmount">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ResourceString="com.donationproperties.amount"
                    DisplayColon="true" EnableViewState="false" AssociatedControlID="amountPriceSelector" />
            </div>
            <div class="editing-form-value-cell">
                <cms:PriceSelector runat="server" ID="amountPriceSelector" ValidatorOnNewLine="true" AllowZero="false" FormatValueAsInteger="true" />
                <cms:CMSRequiredFieldValidator runat="server" ID="rfvPriceSelector" ControlToValidate="amountPriceSelector" EnableViewState="false" Display="Dynamic" Visible="false" />
                <cms:LocalizedLabel runat="server" ID="lblPriceRangeError" CssClass="form-control-error" EnableViewState="false" Visible="false" />
            </div>
        </div>
    </asp:PlaceHolder>
    <%-- Donation units --%>
    <asp:PlaceHolder runat="server" ID="plcUnits">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ResourceString="com.donationproperties.units"
                    DisplayColon="true" AssociatedControlID="txtUnits" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox runat="server" ID="txtUnits" Text="1" MaxLength="9" CssClass="form-control" />
                <cms:CMSRangeValidator ID="rvUnits" runat="server" ControlToValidate="txtUnits" MaximumValue="999999999"
                    MinimumValue="1" Type="Integer" EnableViewState="false" Display="Dynamic" />
            </div>
        </div>
    </asp:PlaceHolder>
    <%-- Donation is private --%>
    <asp:PlaceHolder runat="server" ID="plcIsPrivate">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ResourceString="com.donationproperties.isprivate"
                    DisplayColon="true" EnableViewState="false" AssociatedControlID="chkIsPrivate" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSCheckBox runat="server" ID="chkIsPrivate" Checked="false" />
            </div>
        </div>
    </asp:PlaceHolder>
</div>
