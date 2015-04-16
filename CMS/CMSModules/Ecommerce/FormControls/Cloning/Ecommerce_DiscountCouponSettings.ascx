<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Ecommerce_DiscountCouponSettings.ascx.cs"
    Inherits="CMSModules_Ecommerce_FormControls_Cloning_Ecommerce_DiscountCouponSettings" %>

<div class="form-horizontal">
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel AssociatedControlID="txtCode" CssClass="control-label" runat="server" ID="lblCode" ResourceString="discouncoupon_edit.discountcouponcodelabel" EnableViewState="false"
                DisplayColon="true" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSTextBox runat="server" ID="txtCode" runat="server"  />
        </div>
    </div>
</div>