<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSWebParts_Ecommerce_Checkout_Selectors_PaymentMethodSelection" CodeFile="~/CMSWebParts/Ecommerce/Checkout/Selectors/PaymentMethodSelection.ascx.cs" %>
<%@ Register Src="~/CMSModules/ECommerce/FormControls/PaymentSelector.ascx" TagName="PaymentSelector" TagPrefix="cms" %>
  
<asp:Panel ID="pnlPayment" runat="server" CssClass="PanelPayment">    
    <asp:Label runat="server" ID="lblError" Visible="false" CssClass="ErrorLabel" />                         
    <cms:PaymentSelector  AddAllItemsRecord="false" ID="drpPayment" runat="server" AddNoneRecord="true" DisplayOnlyEnabled="true" IsLiveSite="true" CssClass="SelectorClass" />
</asp:Panel>
