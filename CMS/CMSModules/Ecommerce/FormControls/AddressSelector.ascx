<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Ecommerce_FormControls_AddressSelector" CodeFile="AddressSelector.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector" TagPrefix="cms" %>

<cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
    <ContentTemplate>
        <cms:UniSelector ID="uniSelector" runat="server" DisplayNameFormat="{%AddressName%}"
            ObjectType="ecommerce.address" ResourcePrefix="addressselector" ReturnColumnName="AddressID"
            SelectionMode="SingleDropDownList" EditDialogWindowHeight="350" EditDialogWindowWidth="500" />
    </ContentTemplate>
</cms:CMSUpdatePanel>
