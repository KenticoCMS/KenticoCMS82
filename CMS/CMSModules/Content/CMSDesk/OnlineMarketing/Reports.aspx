<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Reports.aspx.cs" Inherits="CMSModules_Content_CMSDesk_OnlineMarketing_Reports"
    Theme="Default" Title="Page's reports" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    EnableEventValidation="false" %>

<%@ Register Src="~/CMSAdminControls/Basic/DisabledModuleInfo.ascx" TagPrefix="cms"
    TagName="DisabledModule" %>
<%@ Register Src="~/CMSModules/WebAnalytics/Controls/SelectGraphTypeAndPeriod.ascx"
    TagName="GraphType" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/WebAnalytics/Controls/GraphPreLoader.ascx" TagName="GraphPreLoader"
    TagPrefix="cms" %>
<asp:Content ID="cntHeader" runat="server" ContentPlaceHolderID="plcBeforeContent">
    <div class="header-panel">
        <cms:GraphPreLoader runat="server" ID="ucGraphPreLoader" />
        <cms:GraphType runat="server" ID="ucGraphType" />
    </div>
    <div class="header-panel radio-list-vertical">
        <cms:CMSRadioButton runat="server" ID="rbContent" ResourceString="development.content"
            AutoPostBack="true" CssClass="PageReportRadioButton" GroupName="Radio" Checked="true" />
        <cms:CMSRadioButton runat="server" ID="rbTraffic" ResourceString="analytics_codename.trafficsources"
            AutoPostBack="true" CssClass="PageReportRadioButton" GroupName="Radio" />
    </div>
    <asp:Panel runat="server" ID="pnlDisabled" CssClass="header-panel-alert">
        <cms:DisabledModule runat="server" ID="ucDisabledModule" SettingsKeys="CMSAnalyticsEnabled" />
    </asp:Panel>
</asp:Content>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <div class="ReportBody">
        <asp:Panel runat="server" ID="pnlContent">
        </asp:Panel>
    </div>
</asp:Content>
