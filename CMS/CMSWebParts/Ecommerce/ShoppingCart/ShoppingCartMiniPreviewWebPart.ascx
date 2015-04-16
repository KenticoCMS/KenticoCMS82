<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSWebParts_Ecommerce_ShoppingCart_ShoppingCartMiniPreviewWebPart"
    CodeFile="~/CMSWebParts/Ecommerce/ShoppingCart/ShoppingCartMiniPreviewWebPart.ascx.cs" %>
<cms:cmsupdatepanel id="upnlAjax" runat="server">
    <ContentTemplate>
        <table cellspacing="0" class="_div">
            <tr>
                <td style="padding-right: 5px;">
                    <asp:Literal runat="server" ID="ltlRTLFix" Text="<%#rtlFix%>" EnableViewState="false" />
                    <asp:Image ID="imgCartIcon" runat="server" CssClass="ShoppingCartIcon" EnableViewState="false" />
                    <asp:PlaceHolder ID="plcShoppingCart" runat="server" EnableViewState="false">
                        <asp:HyperLink ID="lnkShoppingCart" runat="server" CssClass="ShoppingCartLink" EnableViewState="false" />
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="plcMyAccount" runat="server" EnableViewState="false"><%=Separator%><asp:HyperLink ID="lnkMyAccount"
                        runat="server" CssClass="ShoppingCartLink" EnableViewState="false" />
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="plcMyWishlist" runat="server" EnableViewState="false"><%=Separator%><asp:HyperLink ID="lnkMyWishlist"
                        runat="server" CssClass="ShoppingCartLink" EnableViewState="false" />
                    </asp:PlaceHolder>
                </td>
            </tr>
            <asp:PlaceHolder ID="plcTotalPrice" runat="server" EnableViewState="false">
                <tr>
                    <td align="right" style="padding-right: 5px;">
                        <asp:Label ID="lblTotalPriceTitle" runat="server" CssClass="SmallTextLabel" EnableViewState="false" />
                        <asp:Label ID="lblTotalPriceValue" runat="server" CssClass="SmallTextLabel" EnableViewState="false" />
                    </td>
                </tr>
            </asp:PlaceHolder>
        </table>
    </ContentTemplate>
</cms:cmsupdatepanel>
