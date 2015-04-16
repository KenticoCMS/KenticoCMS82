<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Tab_Reports.aspx.cs" Inherits="CMSModules_WebAnalytics_Pages_Tools_Campaign_Tab_Reports"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Campaign reports"
    EnableEventValidation="false" Theme="Default" %>

<%@ Register Src="~/CMSModules/WebAnalytics/Controls/SelectGraphTypeAndPeriod.ascx"
    TagName="GraphType" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/WebAnalytics/Controls/GraphPreLoader.ascx" TagName="GraphPreLoader"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/WebAnalytics/FormControls/SelectConversion.ascx" TagName="SelectConversion"
   TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/WebAnalytics/Controls/ReportHeader.ascx" TagName="ReportHeaderActions"
    TagPrefix="cms" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="plcContent">
    <cms:ReportHeaderActions runat="server" ID="reportHeaderActions" />
    <div class="header-panel">
        <cms:GraphPreLoader runat="server" ID="ucGraphPreLoader" />
        <cms:GraphType runat="server" ID="ucGraphType" />
    </div>
    <div class="header-panel" runat="server" id="pnlRadios">
        <table class="campaign-report-radios">
            <tr>
                <td>
                    <cms:CMSRadioButton runat="server" ID="rbViews" ResourceString="reports_campaign.label_num"
                        AutoPostBack="true" CssClass="PageReportRadioButton" GroupName="Radio" />
                </td>
                <td>
                    <cms:CMSRadioButton runat="server" ID="rbCount" ResourceString="conversion.count"
                        AutoPostBack="true" CssClass="PageReportRadioButton" GroupName="Radio" />
                </td>
                <td>
                    <cms:CMSRadioButton runat="server" ID="rbValue" ResourceString="conversions.value"
                        AutoPostBack="true" CssClass="PageReportRadioButton" GroupName="Radio" />
                </td>
                <td>
                    <cms:CMSRadioButton runat="server" ID="rbDetail" ResourceString="campaign.detail"
                        AutoPostBack="true" CssClass="PageReportRadioButton" GroupName="Radio" />
                </td>
            </tr>
            <tr>
                <td>
                    <cms:CMSRadioButton runat="server" ID="rbGoalView" ResourceString="campaign.goals.visitors.title"
                        AutoPostBack="true" CssClass="PageReportRadioButton" GroupName="Radio" />
                </td>
                <td>
                    <cms:CMSRadioButton runat="server" ID="rbGoalCount" ResourceString="campaign.goals.count.title"
                        AutoPostBack="true" CssClass="PageReportRadioButton" GroupName="Radio" />
                </td>
                <td>
                    <cms:CMSRadioButton runat="server" ID="rbGoalValue" ResourceString="campaign.goals.value.title"
                        AutoPostBack="true" CssClass="PageReportRadioButton" GroupName="Radio" />
                </td>
                <td>
                    <cms:CMSRadioButton runat="server" ID="rbValuePerVisitor" ResourceString="campaign.goals.valuepervisitor.title"
                        AutoPostBack="true" CssClass="PageReportRadioButton" GroupName="Radio" />
                </td>
            </tr>
        </table>
    </div>
    <div class="ReportBody">
        <asp:Label runat="server" ID="lblInfo" CssClass="InfoLabel" Visible="false" EnableViewState="false" />
        <asp:PlaceHolder runat="server" ID="pnlConversions">
            <div class="form-horizontal form-filter">
                <div class="form-group">
                    <div class="filter-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="lblConversions" runat="server" ResourceString="abtesting.conversions"
                            DisplayColon="true" />
                    </div>
                    <div class="filter-form-value-cell">
                        <cms:SelectConversion runat="server" ID="ucConversions" SelectionMode="SingleDropDownList" />
                    </div>
                </div>
            </div>
        </asp:PlaceHolder>
        <asp:Panel runat="server" ID="pnlContent">
        </asp:Panel>
    </div>
</asp:Content>