<%@ Page Title="Membership Edit - Products" Language="C#" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    AutoEventWireup="true" CodeFile="Membership_Edit_Products.aspx.cs" Inherits="CMSModules_Ecommerce_Pages_Administration_Membership_Membership_Edit_Products"
    Theme="Default" %>

<%@ Register TagPrefix="cms" TagName="UniGrid" Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" %>
<%@ Register TagPrefix="ug" Namespace="CMS.UIControls.UniGridConfig" Assembly="CMS.UIControls" %>
<asp:Content ID="bodyContentElem" runat="server" ContentPlaceHolderID="plcContent">
    <cms:LocalizedHeading runat="server" Level="4" ResourceString="membership.assignedtoproducts"
        DisplayColon="true" EnableViewState="false" CssClass="listing-title" />
    <cms:UniGrid ID="productsUniGridElem" runat="server" Columns="SKUName, SKUPrice, SKUValidity, SKUValidFor, SKUValidUntil, SKUSiteID"
        OrderBy="SKUName" ObjectType="ecommerce.sku" OnOnExternalDataBound="productsUniGridElem_OnExternalDataBound">
        <GridColumns>
            <ug:Column Name="Name" Source="SKUName" Caption="$product_list.productname$" Wrap="false">
                <Filter Type="Text" />
            </ug:Column>
            <ug:Column Name="Price" Source="##ALL##" ExternalSourceName="skuprice" Caption="$product_list.productprice$"
                Wrap="false" />
            <ug:Column Name="Validity" Source="##ALL##" ExternalSourceName="skuvalidity" Caption="$product_list.productvalidity$"
                Wrap="false" />
            <ug:Column Name="Fill" CssClass="main-column-100" />
        </GridColumns>
        <GridOptions DisplayFilter="true" />
    </cms:UniGrid>
</asp:Content>
