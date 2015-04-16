<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Tools_Configuration_TaxClasses_TaxClass_Products"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" CodeFile="TaxClass_Products.aspx.cs"  %>

<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<asp:Content ID="Content1" ContentPlaceHolderID="plcContent" runat="Server">
    <cms:LocalizedHeading runat="server" ID="headTitle" Level="4" ResourceString="com.taxclass.assignedtoproducts" CssClass="listing-title" EnableViewState="false" />
    <cms:UniSelector ID="uniSelector" runat="server" IsLiveSite="false" ObjectType="ecommerce.sku" 
        SelectionMode="Multiple" ResourcePrefix="com.selectproducts" 
        UseDefaultNameFilter="false" FilterControl="~/CMSModules/Ecommerce/Controls/Filters/SimpleProductFilter.ascx" />
</asp:Content>