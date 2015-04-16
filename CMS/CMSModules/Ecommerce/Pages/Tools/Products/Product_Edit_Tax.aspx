<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Inherits="CMSModules_Ecommerce_Pages_Tools_Products_Product_Edit_Tax" Theme="Default"
    Title="Product Edit - Tax" CodeFile="Product_Edit_Tax.aspx.cs" %>

<%@ Register Src="~/CMSModules/Ecommerce/Controls/UI/ProductTaxes.ascx" TagName="Product_Edit_Tax"
    TagPrefix="cms" %>
<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <cms:Product_Edit_Tax ID="taxForm" runat="server" />
</asp:Content>
