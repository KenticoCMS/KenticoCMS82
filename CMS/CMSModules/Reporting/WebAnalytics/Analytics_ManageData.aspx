<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Reporting_WebAnalytics_Analytics_ManageData"
    MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master" Title="Analytics - Delete data"
    Theme="Default" CodeFile="Analytics_ManageData.aspx.cs" %>

<%@ Register Src="~/CMSModules/WebAnalytics/FormControls/SelectCampaign.ascx" TagName="Campaigns"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/WebAnalytics/FormControls/SelectConversion.ascx" TagName="Conversions"
    TagPrefix="cms" %>

<asp:Content ID="cntBody" ContentPlaceHolderID="plcContent" runat="server">
    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
        <ContentTemplate>
            <asp:Timer ID="timeRefresh" runat="server" Interval="2000" EnableViewState="false"
                Enabled="false" />
            <asp:Panel runat="server" ID="pnlProgress" Visible="False" CssClass="content-block-50">
                <asp:Literal runat="server" ID="ltrProgress" EnableViewState="false" />
            </asp:Panel>
            <div>
                <div class="form-horizontal">
                    <asp:PlaceHolder runat="server" ID="pnlCampaigns" Visible="false">
                        <div class="form-group">
                            <div class="editing-form-label-cell">
                                <cms:LocalizedLabel CssClass="control-label" ID="lblCampaign" runat="server" ResourceString="campaign.campaign.list"
                                    DisplayColon="true" />
                            </div>
                            <div class="editing-form-value-cell">
                                <cms:Campaigns runat="server" ID="usCampaigns" AllowAll="True" />
                            </div>
                        </div>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder runat="server" ID="pnlConversions" Visible="false">
                        <div class="form-group">
                            <div class="editing-form-label-cell">
                                <cms:LocalizedLabel CssClass="control-label" ID="lblConversions" runat="server" ResourceString="conversion.conversion.list"
                                    DisplayColon="true" />
                            </div>
                            <div class="editing-form-value-cell">
                                <cms:Conversions runat="server" ID="ucConversions" SelectionMode="SingleDropDownList" />
                            </div>
                        </div>
                    </asp:PlaceHolder>
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblFrom" runat="server" EnableViewState="false" ResourceString="AnalyticsManageData.FromDate" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:DateTimePicker ID="pickerFrom" runat="server" EditTime="false" />
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblTo" runat="server" EnableViewState="false" ResourceString="AnalyticsManageData.ToDate" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:DateTimePicker ID="pickerTo" runat="server" EditTime="false" />
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>
<asp:Content ID="cntFooter" runat="server" ContentPlaceHolderID="plcFooter">
    <cms:LocalizedButton ID="btnDelete" runat="server" OnClick="btnDelete_Click" ButtonStyle="Primary"
        EnableViewState="false" ResourceString="general.delete" />
</asp:Content>