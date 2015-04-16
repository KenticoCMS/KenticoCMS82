<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_MyDesk_CheckedOut_Objects"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="My desk - Checked out objects"
    CodeFile="Objects.aspx.cs" EnableEventValidation="false" %>

<%@ Register TagPrefix="cms" TagName="DisabledModuleInfo" Src="~/CMSAdminControls/Basic/DisabledModuleInfo.ascx" %>
<%@ Register TagPrefix="cms" TagName="CheckedOutObjectsList" Src="~/CMSModules/Objects/Controls/Locking/CheckedOutObjectsList.ascx" %>
<asp:Content ContentPlaceHolderID="plcBeforeContent" runat="server">
    <asp:Panel runat="server" ID="pnlDisabled" CssClass="header-panel">
        <cms:DisabledModuleInfo runat="server" ID="ucDisabledModuleInfo" SettingsKeys="CMSUseObjectCheckinCheckout" />
    </asp:Panel>
</asp:Content>
<asp:Content ContentPlaceHolderID="plcContent" runat="server">
    <cms:CheckedOutObjectsList ID="ucObjectsList" runat="server" />
</asp:Content>
