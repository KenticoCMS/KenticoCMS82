<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/CMSWebParts/Ecommerce/Donate.ascx.cs" Inherits="CMSWebParts_Ecommerce_Donate" %>

<%@ Register TagPrefix="cms" TagName="DonationProperties" Src="~/CMSModules/Ecommerce/Controls/ProductOptions/DonationProperties.ascx" %>
<%@ Register TagPrefix="cms" TagName="ShoppingCartItemSelector" Src="~/CMSModules/Ecommerce/Controls/ProductOptions/ShoppingCartItemSelector.ascx" %>
<div class="DonateWebPart">
    <asp:Label runat="server" ID="lblDescription" CssClass="Description" />
    <%-- Donation properties --%>
    <asp:PlaceHolder ID="plcDonationProperties" runat="server">
        <cms:CMSUpdatePanel ID="pnlUpdateDonationProperties" runat="server" UpdateMode="Conditional">
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="shoppingCartItemSelector" EventName="OnAddToShoppingCart" />
            </Triggers>
            <ContentTemplate>
                <cms:LocalizedLabel ID="lblError" runat="server" CssClass="ErrorLabel" EnableViewState="false"
                    Visible="false" />
                <cms:DonationProperties ID="donationProperties" runat="server" Visible="false" />
            </ContentTemplate>
        </cms:CMSUpdatePanel>
    </asp:PlaceHolder>
    <%-- Shopping cart item selector --%>
    <div class="form-horizontal">
        <div class="form-group form-group-submit">
            <asp:PlaceHolder ID="plcFieldLabel" runat="server"></asp:PlaceHolder>
            <cms:ShoppingCartItemSelector ID="shoppingCartItemSelector" runat="server" ShowProductOptions="false"
                ShowDonationProperties="false" RedirectToDetailsEnabled="false" />
        </div>
    </div>
    <%-- Hidden fields --%>
    <div style="display: none;">
        <cms:CMSUpdatePanel ID="pnlUpdateHidden" runat="server" UpdateMode="Conditional">
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="shoppingCartItemSelector" EventName="OnAddToShoppingCart" />
            </Triggers>
            <ContentTemplate>
                <asp:HiddenField runat="server" ID="hdnDialogIdentifier" />
                <asp:HiddenField runat="server" ID="hdnDonationAmount" />
                <asp:HiddenField runat="server" ID="hdnDonationIsPrivate" />
                <asp:HiddenField runat="server" ID="hdnDonationUnits" />
            </ContentTemplate>
        </cms:CMSUpdatePanel>
    </div>
</div>
