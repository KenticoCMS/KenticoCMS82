<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="AB test list" Inherits="CMSModules_OnlineMarketing_Pages_Content_ABTesting_ABTest_List"
    Theme="Default" CodeFile="List.aspx.cs" %>

<%@ Register Src="~/CMSModules/OnlineMarketing/Controls/UI/AbTest/List.ascx" TagName="AbTestList"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/Basic/DisabledModuleInfo.ascx" TagPrefix="cms"
    TagName="DisabledModule" %>
<asp:Content ID="cntHeader" runat="server" ContentPlaceHolderID="plcBeforeContent">
    <asp:Panel runat="server" ID="pnlDisabled" CssClass="header-panel-alert">
        <cms:DisabledModule runat="server" ID="ucDisabledModule" />
    </asp:Panel>
</asp:Content>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <div>
        <cms:AbTestList ID="listElem" runat="server" IsLiveSite="false" />
    </div>
</asp:Content>
