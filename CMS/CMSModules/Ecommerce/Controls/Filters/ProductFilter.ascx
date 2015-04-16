<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Controls_Filters_ProductFilter"
    CodeFile="ProductFilter.ascx.cs" %>
<%@ Register Src="~/CMSModules/ECommerce/FormControls/PublicStatusSelector.ascx"
    TagName="PublicStatusSelector" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/ECommerce/FormControls/ManufacturerSelector.ascx"
    TagName="ManufacturerSelector" TagPrefix="cms" %>

<asp:Panel ID="pnlContainer" runat="server">
    <table class="ProductFilter">
        <asp:PlaceHolder ID="plcFirstRow" runat="server">
            <tr>
                <td>
                    <cms:LocalizedLabel ID="lblSearch" ResourceString="ecommerce.filter.product.search" runat="server" EnableViewState="false" />
                </td>
                <td>
                    <cms:CMSTextBox ID="txtSearch" runat="server" CssClass="ProductSearch" EnableViewState="false" />
                </td>
                <td>
                    <cms:LocalizedLabel ID="lblStatus" ResourceString="ecommerce.filter.product.status" runat="server" EnableViewState="false" />
                </td>
                <td>
                    <cms:PublicStatusSelector ID="statusSelector" runat="server" UseNameForSelection="false"
                        AddAllItemsRecord="true" ReflectGlobalProductsUse="true" />
                </td>
                <td>
                    <cms:LocalizedLabel ID="lblManufacturer" ResourceString="ecommerce.filter.product.manufacturer" runat="server" EnableViewState="false" />
                </td>
                <td>
                    <cms:ManufacturerSelector ID="manufacturerSelector" runat="server" UseNameForSelection="false"
                        AddAllItemsRecord="true" />
                </td>
                <td>
                    <cms:CMSCheckBox ID="chkStock" runat="server" />
                </td>
                <asp:PlaceHolder ID="plcFirstButton" runat="server" Visible="false">
                    <td>
                        <cms:LocalizedButton ID="btnFirstFilter" runat="server" ButtonStyle="Default" EnableViewState="false"
                            OnClick="btnFilter_Click" UseSubmitBehavior="false" ResourceString="ecommerce.filter.product.filter" />
                    </td>
                </asp:PlaceHolder>
            </tr>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plcSecondRow" runat="server">
            <tr>
                <td colspan="2"></td>

                <td>
                    <cms:LocalizedLabel ID="lblPaging" AssociatedControlID="drpPaging" EnableViewState="false" ResourceString="ecommerce.filter.product.paging"
                        runat="server" />
                </td>
                <td>
                    <cms:CMSDropDownList ID="drpPaging" runat="server" UseResourceStrings="true" />
                </td>
                <td>
                    <cms:LocalizedLabel ResourceString="ecommerce.filter.product.sort" ID="lblSort" AssociatedControlID="drpSort" EnableViewState="false" runat="server" />
                </td>
                <td>
                    <cms:CMSDropDownList ID="drpSort" runat="server" UseResourceStrings="true" >
                        <asp:ListItem Text="ecommerce.filter.product.nameasc" Value="nameasc" />
                        <asp:ListItem Text="ecommerce.filter.product.namedesc" Value="namedesc" />
                        <asp:ListItem Text="ecommerce.filter.product.priceasc" Value="priceasc" />
                        <asp:ListItem Text="ecommerce.filter.product.pricedesc" Value="pricedesc" />
                    </cms:CMSDropDownList>
                </td>
                <asp:PlaceHolder ID="plcSecButton" runat="server">
                    <td>
                        <cms:LocalizedButton ID="btnSecFilter" runat="server" ButtonStyle="Default"
                            OnClick="btnFilter_Click" UseSubmitBehavior="false" EnableViewState="false" ResourceString="ecommerce.filter.product.filter" />
                    </td>
                </asp:PlaceHolder>
            </tr>
        </asp:PlaceHolder>
    </table>
</asp:Panel>
