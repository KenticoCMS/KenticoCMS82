<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSWebParts_Ecommerce_Checkout_Selectors_ShippingSelection" CodeFile="~/CMSWebParts/Ecommerce/Checkout/Selectors/ShippingSelection.ascx.cs" %>
<%@ Register Src="~/CMSModules/ECommerce/FormControls/ShippingSelector.ascx" TagName="ShippingSelector"
    TagPrefix="cms" %>
    
<asp:Panel ID="pnlShipping" runat="server" CssClass="PanelShipping"> 
    <asp:Label runat="server" ID="lblError" Visible="false" CssClass="ErrorLabel" />
    <cms:ShippingSelector ID="drpShipping" runat="server" AddNoneRecord="true" IncludeSelected="false" IsLiveSite="true" AddAllItemsRecord="false" CssClass="SelectorClass" UseNameForSelection="false" />            
</asp:Panel>
