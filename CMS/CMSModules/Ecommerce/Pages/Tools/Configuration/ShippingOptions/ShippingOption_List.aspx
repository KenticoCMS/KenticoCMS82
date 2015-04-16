<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Tools_Configuration_ShippingOptions_ShippingOption_List"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" CodeFile="ShippingOption_List.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<asp:Content ID="Content1" ContentPlaceHolderID="plcContent" runat="Server">
    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
        <ContentTemplate>
            <cms:UniGrid runat="server" ID="UniGrid" GridName="~/App_Data/CMSModules/Ecommerce/UI/Grids/Ecommerce_ShippingOption/default.xml" IsLiveSite="false" ObjectType="ecommerce.shippingoption" OrderBy="ShippingOptionDisplayName"/>
      </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>
