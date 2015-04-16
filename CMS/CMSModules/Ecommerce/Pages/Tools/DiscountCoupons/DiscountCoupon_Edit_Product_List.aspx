<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Tools_DiscountCoupons_DiscountCoupon_Edit_Product_List"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Discount coupon - Edit"
    CodeFile="DiscountCoupon_Edit_Product_List.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:LocalizedHeading runat="server" ID="headTitle" Level="4" ResourceString="Discount_Product.Applies" CssClass="listing-title" EnableViewState="false" />
    <cms:CMSRadioButton runat="server" ID="radFollowing" GroupName="radApplies" Checked="true"
        AutoPostBack="true" OnCheckedChanged="radFollowing_CheckedChanged" />
    <cms:CMSRadioButton runat="server" ID="radExcept" GroupName="radApplies" AutoPostBack="true"
        OnCheckedChanged="radFollowing_CheckedChanged" />
    <br />
    <br />
    <cms:UniSelector ID="uniSelector" runat="server" IsLiveSite="false" ObjectType="ecommerce.sku"
        SelectionMode="Multiple" ResourcePrefix="skuselect" WhereCondition="(SKUOptionCategoryID IS NULL)" 
        UseDefaultNameFilter="false" FilterControl="~/CMSModules/Ecommerce/Controls/Filters/SimpleProductFilter.ascx" />
</asp:Content>
