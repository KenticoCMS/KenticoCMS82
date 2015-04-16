<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Controls_UI_ProductTaxes"
    CodeFile="ProductTaxes.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<cms:LocalizedHeading runat="server" ID="headTitle" Level="4" CssClass="listing-title" EnableViewState="false" />
<cms:UniSelector ID="uniSelector" runat="server" IsLiveSite="false" ObjectType="ecommerce.taxclass"
    SelectionMode="Multiple" ResourcePrefix="taxselect" />
