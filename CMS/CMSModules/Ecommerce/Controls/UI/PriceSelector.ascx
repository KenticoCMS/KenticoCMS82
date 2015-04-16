<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Controls_UI_PriceSelector"
    CodeFile="PriceSelector.ascx.cs" %>
<%-- Price value --%>
<cms:CMSTextBox ID="txtPrice" runat="server"  MaxLength="20" CssClass="form-control" />
<%-- Currency code --%>
<asp:Label ID="lblCurrencyCode" runat="server" EnableViewState="false" CssClass="form-control-text" />
<%-- Messages --%>
<cms:MessagesPlaceHolder ID="plcMessages" runat="server" BasicStyles="true" Visible="false" />
