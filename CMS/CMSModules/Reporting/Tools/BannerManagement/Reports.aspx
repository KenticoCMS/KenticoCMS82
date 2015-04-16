<%@ Page Language="C#" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" AutoEventWireup="true" Inherits="CMSModules_Reporting_Tools_BannerManagement_Reports"
    Title="Banner reports" Theme="Default" CodeFile="Reports.aspx.cs" EnableEventValidation="false" %>

<%@ Register Src="~/CMSModules/WebAnalytics/Controls/GraphPreLoader.ascx" TagName="GraphPreLoader" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/WebAnalytics/Controls/ReportHeader.ascx" TagName="ReportHeader" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Reporting/Controls/DisplayReport.ascx" TagName="DisplayReport" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/WebAnalytics/Controls/SelectGraphTypeAndPeriod.ascx" TagName="GraphTypeAndPeriod" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/Basic/DisabledModuleInfo.ascx" TagPrefix="cms" TagName="DisabledModule" %>

<asp:Content ID="cntBefore" runat="server" ContentPlaceHolderID="plcBeforeContent">
    <cms:ReportHeader runat="server" ID="reportHeader" />
    <asp:Panel runat="server" ID="pnlDisabled" CssClass="header-panel-alert-above-header">
        <cms:DisabledModule runat="server" ID="ucDisabledModule" SettingsKeys="CMSAnalyticsEnabled" />
    </asp:Panel>
</asp:Content>
<asp:Content ID="cntContent" ContentPlaceHolderID="plcContent" runat="Server">
    <cms:MessagesPlaceHolder ID="plcMess" runat="server" />
    <cms:GraphTypeAndPeriod runat="server" ID="ucGraphTypePeriod" />
    <cms:DisplayReport ID="ucDisplayReport" runat="server"
        BodyCssClass="DisplayReportBody" IsLiveSite="false" RenderCssClasses="true" />
    <cms:GraphPreLoader runat="server" ID="ucGraphPreLoader" />
</asp:Content>
