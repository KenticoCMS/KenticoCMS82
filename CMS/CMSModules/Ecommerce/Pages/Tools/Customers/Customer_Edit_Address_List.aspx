<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Inherits="CMSModules_Ecommerce_Pages_Tools_Customers_Customer_Edit_Address_List"
    Theme="Default" CodeFile="Customer_Edit_Address_List.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <cms:UniGrid runat="server" ID="UniGrid" OrderBy="AddressName" IsLiveSite="false"
        Columns="AddressID,AddressName,AddressIsBilling,AddressIsShipping,AddressIsCompany,AddressEnabled"
        ObjectType="ecommerce.address">
        <GridActions>
            <ug:Action Name="edit" Caption="$General.Edit$" FontIconClass="icon-edit" FontIconStyle="Allow" />
            <ug:Action Name="delete" ExternalSourceName="Delete" Caption="$General.Delete$" FontIconClass="icon-bin" FontIconStyle="Critical"
                Confirmation="$General.ConfirmDelete$" />
        </GridActions>
        <GridColumns>
            <ug:Column Source="AddressName" Caption="$Unigrid.Customer_Edit_Address.Columns.AddressName$"
                Wrap="false">
                <Filter Type="text" />
            </ug:Column>
            <ug:Column Source="AddressIsBilling" ExternalSourceName="#yesno" Caption="$Unigrid.Customer_Edit_Address.Columns.AddressIsBilling$"
                Wrap="false" />
            <ug:Column Source="AddressIsShipping" ExternalSourceName="#yesno" Caption="$Unigrid.Customer_Edit_Address.Columns.AddressIsShipping$"
                Wrap="false" />
            <ug:Column Source="AddressIsCompany" ExternalSourceName="#yesno" Caption="$Unigrid.Customer_Edit_Address.Columns.AddressIsCompany$"
                Name="AddressIsCompany" Wrap="false" />
            <ug:Column Source="AddressEnabled" ExternalSourceName="#yesno" Caption="$general.enabled$"
                Wrap="false" />
            <ug:Column CssClass="filling-column" />
        </GridColumns>
        <GridOptions DisplayFilter="true" />
    </cms:UniGrid>
</asp:Content>
