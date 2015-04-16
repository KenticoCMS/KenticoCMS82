<%@ Page Language="C#" AutoEventWireup="true" Title="Online marketing settings" CodeFile="Default.aspx.cs"
    Theme="Default" Inherits="CMSModules_Content_CMSDesk_OnlineMarketing_Settings_Default"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" %>

<%@ Register Src="~/CMSModules/WebAnalytics/FormControls/SelectConversion.ascx" TagName="ConversionSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/WebAnalytics/FormControls/SelectCampaign.ascx" TagName="SelectCampaign"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/Basic/DisabledModuleInfo.ascx" TagPrefix="cms"
    TagName="DisabledModule" %>
<%@ Register Src="~/CMSFormControls/Inputs/TextboxDoubleValidator.ascx" TagPrefix="cms"
    TagName="DoubleValidator" %>

<asp:Content ID="plcHeader" ContentPlaceHolderID="plcBeforeContent" runat="server">
    <cms:CMSDocumentPanel ID="pnlDoc" runat="server" />
    <asp:Panel runat="server" ID="pnlDisabled" CssClass="header-panel-alert">
        <cms:DisabledModule runat="server" ID="ucDisabledModule" />
    </asp:Panel>
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="plcContent" runat="server">
    <div class="form-horizontal">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblCampaign" runat="server" EnableViewState="false" ResourceString="doc.urls.trackcampaign"
                    DisplayColon="true" />
            </div>
            <div class="editing-form-value-cell">
                <cms:SelectCampaign runat="server" ID="usSelectCampaign" IsLiveSite="false" SelectionMode="SingleTextBox" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblConversionName" runat="server" EnableViewState="false"
                    ResourceString="om.trackconversionname" DisplayColon="true" />
            </div>
            <div class="editing-form-value-cell">
                <cms:ConversionSelector runat="server" ID="ucConversionSelector" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblConversionValue" runat="server" EnableViewState="false"
                    ResourceString="om.trackconversionvalue" DisplayColon="true" />
            </div>
            <div class="editing-form-value-cell">
                <cms:DoubleValidator ID="txtConversionValue" runat="server"
                    MaxLength="200" />
            </div>
        </div>
    </div>
</asp:Content>