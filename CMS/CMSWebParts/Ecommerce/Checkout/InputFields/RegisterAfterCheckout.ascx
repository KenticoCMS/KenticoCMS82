<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSWebParts_Ecommerce_Checkout_InputFields_RegisterAfterCheckout" CodeFile="~/CMSWebParts/Ecommerce/Checkout/InputFields/RegisterAfterCheckout.ascx.cs" %>

<asp:panel runat="server" ID="pnlCheckBox">
    <cms:CMSCheckBox runat="server" id="chkRegister" /> 
</asp:panel>
<asp:panel runat="server" ID="pnlError" Visible="false">
    <asp:Label runat="server" ID="lblError" CssClass="ErrorLabel" />
</asp:panel>