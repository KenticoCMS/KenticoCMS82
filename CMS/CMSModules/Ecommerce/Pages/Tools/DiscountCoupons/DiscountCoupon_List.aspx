<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Tools_DiscountCoupons_DiscountCoupon_List"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Discount coupon - List"
    CodeFile="DiscountCoupon_List.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
        <ContentTemplate>
            <cms:UniGrid runat="server" ID="gridElem" OrderBy="DiscountCouponDisplayName" IsLiveSite="false"
                Columns="DiscountCouponID, DiscountCouponDisplayName, DiscountCouponValidFrom, DiscountCouponValidTo, DiscountCouponValue, DiscountCouponIsFlatValue, DiscountCouponSiteID"
                ObjectType="ecommerce.discountcoupon">
                <GridActions>
                    <ug:Action Name="edit" Caption="$General.Edit$" FontIconClass="icon-edit" FontIconStyle="Allow" />
                    <ug:Action Name="delete" ExternalSourceName="Delete" Caption="$General.Delete$" FontIconClass="icon-bin" FontIconStyle="Critical"
                        Confirmation="$General.ConfirmDelete$" />
                </GridActions>
                <GridColumns>
                    <ug:Column Source="DiscountCouponDisplayName" Caption="$general.name$" Wrap="false">
                        <Filter Type="text" />
                    </ug:Column>
                    <ug:Column Source="##ALL##" ExternalSourceName="CouponValue" Caption="$Unigrid.DiscounCoupon.Columns.DiscountCouponValue$"
                        Sort="DiscountCouponValue" CssClass="TextRight" Wrap="false" />
                    <ug:Column Source="DiscountCouponValidFrom" ExternalSourceName="#userdatetimegmt" Caption="$Unigrid.DiscounCoupon.Columns.DiscountCouponValidFrom$"
                        Wrap="false" />
                    <ug:Column Source="DiscountCouponValidTo" ExternalSourceName="#userdatetimegmt" Caption="$Unigrid.DiscounCoupon.Columns.DiscountCouponValidTo$"
                        Wrap="false" />
                    <ug:Column Source="DiscountCouponID" Sort="DiscountCouponSiteID" ExternalSourceName="#transform: ecommerce.discountcoupon: {% (ToInt(DiscountCouponSiteID, 0) == 0) ? GetResourceString(&quot;com.globally&quot;) : GetResourceString(&quot;com.onthissiteonly&quot;) %}" Name="DiscountCouponSiteID"
                        Caption="$com.available$" Wrap="false" />
                    <ug:Column CssClass="filling-column" />
                </GridColumns>
                <GridOptions DisplayFilter="true" />
            </cms:UniGrid>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>
