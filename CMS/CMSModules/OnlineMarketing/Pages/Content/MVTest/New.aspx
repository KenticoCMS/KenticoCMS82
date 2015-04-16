<%@ Page Language="C#" AutoEventWireup="true" CodeFile="New.aspx.cs" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Inherits="CMSModules_OnlineMarketing_Pages_Content_MVTest_New" Theme="Default" %>

<%@ Register Src="~/CMSModules/OnlineMarketing/Controls/UI/Mvtest/Edit.ascx" TagName="MvtestEdit"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/Basic/DisabledModuleInfo.ascx" TagPrefix="cms"
    TagName="DisabledModule" %>
<asp:Content ID="cntHeader" runat="server" ContentPlaceHolderID="plcBeforeContent">
    <asp:Panel runat="server" ID="pnlDisabled" CssClass="header-panel">
        <cms:DisabledModule runat="server" ID="ucDisabledModule" />        
    </asp:Panel>
</asp:Content>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:MvtestEdit ID="editElem" runat="server" IsLiveSite="false" />
</asp:Content>
