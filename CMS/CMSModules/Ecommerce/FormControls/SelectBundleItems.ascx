<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SelectBundleItems.ascx.cs"
    Inherits="CMSModules_Ecommerce_FormControls_SelectBundleItems" %>
<%@ Register TagPrefix="cms" TagName="UniSelector" Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" %>
<cms:UniSelector runat="server" ID="itemsUniSelector" IsLiveSite="false" ObjectType="ecommerce.productlist"
    SelectionMode="Multiple" ResourcePrefix="productselect" DisplayNameFormat="{%SKUName%}" />
