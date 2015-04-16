<%@ Page Language="C#" AutoEventWireup="true" Theme="Default" CodeFile="Analytics_DataManagement.aspx.cs"
    Inherits="CMSModules_WebAnalytics_Tools_Analytics_DataManagement" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" %>

<%@ Register Src="~/CMSAdminControls/Basic/DisabledModuleInfo.ascx" TagPrefix="cms"
    TagName="DisabledModule" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="plcBeforeContent">
    <asp:Panel runat="server" ID="pnlDisabled" CssClass="header-panel">
        <cms:DisabledModule runat="server" ID="ucDisabledModule" />
    </asp:Panel>
</asp:Content>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
        <ContentTemplate>
            <asp:Timer ID="timeRefresh" runat="server" Interval="3000" EnableViewState="false" Enabled="false" />
            <div class="content-block-50">
                <asp:Literal runat="server" ID="ltrProgress" EnableViewState="false" />
            </div>
            <cms:LocalizedHeading runat="server" ID="headRemoveData" Level="4" ResourceString="analyt.settings.deletedata" EnableViewState="false" />
            <div class="form-horizontal">
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblDelete" ResourceString="analytics.statisticslist.columns.codename"
                            DisplayColon="true"></cms:LocalizedLabel>
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSDropDownList AutoPostBack="true" CssClass="DropDownField" runat="server" ID="drpDeleteObjects" />
                    </div>
                </div>
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblInterval" ResourceString="general.avaibledata"
                            DisplayColon="true"></cms:LocalizedLabel>
                    </div>
                    <div class="editing-form-value-cell">
                        <asp:Label CssClass="form-control-text" runat="server" ID="lblIntervalInfo" />
                    </div>
                </div>
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblDeleteFrom" ResourceString="general.from"
                            DisplayColon="true" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:DateTimePicker runat="server" ID="ucDeleteFrom" EditTime="false" />
                    </div>
                </div>
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblDeleteTo" ResourceString="general.to" DisplayColon="true" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:DateTimePicker runat="server" ID="ucDeleteTo" EditTime="false" />
                    </div>
                </div>
                <div class="form-group">
                    <div class="editing-form-value-cell editing-form-value-cell-offset">
                        <cms:LocalizedButton runat="server" ID="btnDelete" ButtonStyle="Primary" ResourceString="analyt.settings.deletebtn"
                            OnClick="btnDelete_Click" />
                    </div>
                </div>
            </div>
            <cms:LocalizedHeading runat="server" ID="headGenerateData" Level="4" ResourceString="analyt.settings.generate" EnableViewState="false" />
            <div class="form-horizontal">
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblSampleDataObject" ResourceString="analytics.statisticslist.columns.codename"
                            DisplayColon="true"></cms:LocalizedLabel>
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSDropDownList CssClass="DropDownField" runat="server" ID="drpGenerateObjects" />
                    </div>
                </div>
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblFrom" ShowRequiredMark="True"
                            ResourceString="general.from" DisplayColon="true" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:DateTimePicker runat="server" ID="ucSampleFrom" EditTime="false" />
                    </div>
                </div>
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblTo" ShowRequiredMark="True"
                            ResourceString="general.to" DisplayColon="true" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:DateTimePicker runat="server" ID="ucSampleTo" EditTime="false" />
                    </div>
                </div>
                <div class="form-group">
                    <div class="editing-form-value-cell editing-form-value-cell-offset">
                        <cms:LocalizedButton runat="server" ID="btnGenerate" ButtonStyle="Primary" ResourceString="analyt.settings.generatebtn"
                            OnClick="btnGenerate_Click" />
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>