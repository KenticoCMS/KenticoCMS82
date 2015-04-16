<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Ecommerce_Controls_ShoppingCart_ShoppingCartSKUPriceDetail_Control" CodeFile="ShoppingCartSKUPriceDetail.ascx.cs" %>

<%--Product--%>
<table class="table table-hover">
    <thead>
        <tr class="PriceDetailHeader">
            <th colspan="2">
                <asp:Label ID="lblProductName" runat="server" EnableViewState="false" />
            </th>
        </tr>
    </thead>
    <tbody>
        <tr class="PriceDetailSubtotal">
            <td>
                <asp:Label ID="lblPriceWithoutTax" runat="server" EnableViewState="false" />
            </td>
            <td class="text-right">
                <asp:Label ID="lblPriceWithoutTaxValue" runat="server" EnableViewState="false" />
            </td>
        </tr>
    </tbody>
</table>
<br />

<%--Discounts--%>
<cms:UIGridView ID="gridDiscounts" runat="server" AutoGenerateColumns="false" CssClass="PriceDetailSummaryTable">
    <columns>
            <asp:TemplateField>
                <HeaderStyle />
                <ItemTemplate>
                    <%#GetFormattedName(Eval("DiscountDisplayName"))%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <HeaderStyle Width="80" />
                <ItemStyle CssClass="text-right" Width="80" />
                <ItemTemplate>
                    <%#GetFormattedValue(Eval("DiscountValue"), Eval("DiscountIsFlat"))%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <HeaderStyle Width="80" />
                <ItemStyle CssClass="text-right" Width="80" />
                <ItemTemplate>
                    <%#GetFormattedValue(Eval("UnitDiscount"), true)%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <HeaderStyle Width="80" />
                <ItemStyle CssClass="text-right" Width="80" />
                <ItemTemplate>
                    <%#Eval("DiscountedUnits")%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <HeaderStyle Width="80" />
                <ItemStyle CssClass="text-right" Width="80" />
                <ItemTemplate>
                    <%#GetFormattedValue(Eval("SubtotalDiscount"), true)%>
                </ItemTemplate>
            </asp:TemplateField>
        </columns>
</cms:UIGridView>

<%--Total discount--%>
<table class="table table-hover PriceDetailSubtotalTable">
    <asp:PlaceHolder ID="plcDiscounts" runat="server">
        <thead>
            <tr>
                <th colspan="2">
                    <cms:LocalizedLabel ResourceString="ProductPriceDetail.Discounts" ID="lblDiscounts" runat="server" EnableViewState="false" />
                </th>
            </tr>
        </thead>
    </asp:PlaceHolder>
    <tbody>
        <asp:PlaceHolder ID="plcTotalDiscount" runat="server">
            <tr>
                <td>
                    <cms:LocalizedLabel ID="lblTotalDiscount" ResourceString="productpricedetail.totaldiscount" runat="server" EnableViewState="false" />
                </td>
                <td class="text-right">
                    <asp:Label ID="lblTotalDiscountValue" runat="server" EnableViewState="false" />
                </td>
            </tr>
        </asp:PlaceHolder>
        <tr class="PriceDetailSubtotal">
            <td>
                <cms:LocalizedLabel ResourceString="ProductPriceDetail.PriceAfterDiscount" ID="lblPriceAfterDiscount" runat="server" EnableViewState="false" />
            </td>
            <td class="text-right">
                <asp:Label ID="lblPriceAfterDiscountValue" runat="server" EnableViewState="false" />
            </td>
        </tr>
    </tbody>
</table>
<br />
<%--Taxes--%>
<cms:UIGridView ID="gridTaxes" runat="server" AutoGenerateColumns="false" CssClass="PriceDetailSummaryTable">
    <columns>
            <asp:TemplateField>
                <HeaderStyle />
                <ItemTemplate>
                    <%#GetFormattedName(Eval("TaxClassDisplayName"))%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <HeaderStyle Width="80" />
                <ItemStyle CssClass="text-right" Width="80" />
                <ItemTemplate>
                    <%#GetFormattedValue(Eval("TaxValue"), Eval("TaxIsFlat"))%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <HeaderStyle Width="80" />
                <ItemStyle CssClass="text-right" Width="80" />
                <ItemTemplate>
                    <%#GetFormattedValue(Eval("UnitTax"), true)%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <HeaderStyle Width="80" />
                <ItemStyle CssClass="text-right" Width="80" />
                <ItemTemplate>
                    <%#GetFormattedValue(Eval("SubtotalTax"), true)%>
                </ItemTemplate>
            </asp:TemplateField>
        </columns>
</cms:UIGridView>

<%--Total tax---%>
<table class="table table-hover PriceDetailSubtotalTable">
    <asp:PlaceHolder ID="plcTaxes" runat="server">
        <thead>
            <tr>
                <th colspan="2">
                    <cms:LocalizedLabel ResourceString="ProductPriceDetail.Taxes" ID="lblTaxes" runat="server" EnableViewState="false" />
                </th>
            </tr>
        </thead>
    </asp:PlaceHolder>
    <tbody>
        <asp:PlaceHolder ID="plcTotalTax" runat="server">
            <tr>
                <td>
                    <cms:LocalizedLabel ResourceString="ProductPriceDetail.TotalTax" ID="lblTotalTax" runat="server" EnableViewState="false" />
                </td>
                <td class="text-right">
                    <asp:Label ID="lblTotalTaxValue" runat="server" EnableViewState="false" />
                </td>
            </tr>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plcPriceWithTax" runat="server">
            <tr class="PriceDetailSubtotal">
                <td>
                    <cms:LocalizedLabel ResourceString="ProductPriceDetail.PriceWithTax" ID="lblPriceWithTax" runat="server" EnableViewState="false" />
                </td>
                <td class="text-right">
                    <asp:Label ID="lblPriceWithTaxValue" runat="server" EnableViewState="false" />
                </td>
            </tr>
        </asp:PlaceHolder>
    </tbody>
</table>
<br />

<%--Product options--%>
<cms:BasicUniView ID="OptionsUniView" runat="server" Visible="false">
    <HeaderTemplate>
        <table class="table table-hover">
            <thead>
                <tr>
                    <th colspan="2">
                        <cms:LocalizedLabel ID="LocalizedLabel1" ResourceString="ProductPriceDetail.Accessories" runat="server" />
                    </th>
                </tr>
            </thead>
            <tbody>
    </HeaderTemplate>
    <FooterTemplate>
        </tbody>
            </table>
      <br />
    </FooterTemplate>
    <ItemTemplate>
        <tr>
            <td>
                <span><%# EvalText("SKU.SKUOptionCategory.CategoryTitle", true)%>: <%#ResHelper.LocalizeString(EvalText("SKU.SKUName"), null, true)%> </span>
            </td>
            <td class="text-right">
                <span><%#GetFormattedValue(Eval("UnitTotalPrice"), true)%> </span>
            </td>
        </tr>
    </ItemTemplate>
</cms:BasicUniView>

<%--Totals--%>
<table class="table table-hover">
    <thead>
        <tr>
            <th colspan="2">
                <cms:LocalizedLabel ID="lblTotal" ResourceString="ProductPriceDetail.Total" runat="server"
                    EnableViewState="false" />
            </th>
        </tr>
    </thead>
    <tbody>
        <asp:PlaceHolder ID="plcUnits" runat="server">
            <tr class="PriceDetailSubtotal">
                <td>
                    <cms:LocalizedLabel ResourceString="ProductPriceDetail.ProductUnits" ID="lblProductUnits" runat="server" EnableViewState="false" />
                </td>
                <td class="text-right">
                    <asp:Label ID="lblProductUnitsValue" runat="server" EnableViewState="false" />
                </td>
            </tr>
        </asp:PlaceHolder>
        <tr class="PriceDetailHeader">
            <td>
                <cms:LocalizedLabel ResourceString="ProductPriceDetail.TotalPrice" ID="lblTotalPrice" runat="server" EnableViewState="false" />
            </td>
            <td class="text-right">
                <asp:Label ID="lblTotalPriceValue" runat="server" EnableViewState="false" />
            </td>
        </tr>
    </tbody>
</table>
