<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Tab_Goals.aspx.cs" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Theme="Default" Inherits="CMSModules_WebAnalytics_Pages_Tools_Campaign_Tab_Goals" %>

<%@ Register Src="~/CMSFormControls/Basic/TextBoxControl.ascx" TagName="TextBoxControl"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Basic/RadioButtonsControl.ascx" TagName="RadioButtonsControl"
    TagPrefix="cms" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:UIForm runat="server" ID="EditForm" ObjectType="analytics.campaign" DefaultFieldLayout="Inline">
        <SecurityCheck Resource="CMS.WebAnalytics" Permission="ManageCampaigns" />
        <LayoutTemplate>
            <div class="form-group">
                <div class="editing-form-value-cell editing-form-value-cell-offset">
                    <cms:LocalizedLabel CssClass="CampaignRedFlag input-width-100 cms-form-group-text" ID="lblRedFlag" runat="server" EnableViewState="false" ResourceString="campaign.redflag" />
                    <cms:LocalizedLabel CssClass="CampaignGoal input-width-100 cms-form-group-text" ID="lblFinalGoal" runat="server" EnableViewState="false" ResourceString="campaign.finalgoal" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:FormLabel CssClass="control-label" ID="lblVisitorsWin" runat="server" EnableViewState="false" ResourceString="campaign.numofvisitors"
                        DisplayColon="true" Field="CampaignGoalVisitorsMin" />
                    <cms:FormLabel CssClass="sr-only" ID="lblVisitors" runat="server" EnableViewState="false" ResourceString="campaign.numofvisitors"
                        DisplayColon="true" Field="CampaignGoalVisitors" />
                </div>
                <div class="editing-form-value-cell control-group-inline">
                    <cms:FormField runat="server" ID="fVisitorsMin" Field="CampaignGoalVisitorsMin" UseFFI="False">
                        <cms:TextBoxControl ID="txtVisitorsMin" runat="server" MaxLength="50" />
                    </cms:FormField>
                    <cms:FormField runat="server" ID="fVistors" Field="CampaignGoalVisitors" UseFFI="False">
                        <cms:TextBoxControl ID="txtVisitors" runat="server" MaxLength="50" />
                    </cms:FormField>
                    <cms:FormField runat="server" ID="fVistorsPercent" Field="CampaignGoalVisitorsPercent" UseFFI="False">
                        <cms:RadioButtonsControl ID="rbVisitorsPercent" runat="server" />
                    </cms:FormField>
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:FormLabel CssClass="control-label" ID="lblConversionsWin" runat="server" EnableViewState="false" ResourceString="campaign.conversions"
                        DisplayColon="true" Field="CampaignGoalConversionsMin" />
                    <cms:FormLabel CssClass="sr-only" ID="lblConversions" runat="server" EnableViewState="false" ResourceString="campaign.conversions"
                        DisplayColon="true" Field="CampaignGoalConversions" />
                </div>
                <div class="editing-form-value-cell control-group-inline">
                    <cms:FormField runat="server" ID="fConversionsMin" Field="CampaignGoalConversionsMin" UseFFI="False">
                        <cms:TextBoxControl ID="txtConversionsMin" runat="server" MaxLength="50" />
                    </cms:FormField>
                    <cms:FormField runat="server" ID="fConversions" Field="CampaignGoalConversions" UseFFI="False">
                        <cms:TextBoxControl ID="txtConversions" runat="server" MaxLength="50" />
                    </cms:FormField>
                    <cms:FormField runat="server" ID="fConversionsPercent" Field="CampaignGoalConversionsPercent" UseFFI="False">
                        <cms:RadioButtonsControl ID="rbConversionsPercent" runat="server" />
                    </cms:FormField>
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:FormLabel CssClass="control-label" ID="lblValueMin" runat="server" EnableViewState="false" ResourceString="campaign.value"
                        DisplayColon="true" Field="CampaignGoalValueMin" />
                    <cms:FormLabel CssClass="sr-only" ID="lblValue" runat="server" EnableViewState="false" ResourceString="campaign.value"
                        DisplayColon="true" Field="CampaignGoalValue" />
                </div>
                <div class="editing-form-value-cell control-group-inline">
                    <cms:FormField runat="server" ID="fValueMin" Field="CampaignGoalValueMin" UseFFI="False">
                        <cms:TextBoxControl ID="txtValueMin" runat="server" MaxLength="50" />
                    </cms:FormField>
                    <cms:FormField runat="server" ID="fValue" Field="CampaignGoalValue" UseFFI="False">
                        <cms:TextBoxControl ID="txtValue" runat="server" MaxLength="50" />
                    </cms:FormField>
                    <cms:FormField runat="server" ID="fValuePercent" Field="CampaignGoalValuePercent" UseFFI="False">
                        <cms:RadioButtonsControl ID="rbValuePercent" runat="server" />
                    </cms:FormField>
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:FormLabel CssClass="control-label" ID="lblPerVisitorMin" runat="server" EnableViewState="false" ResourceString="campaign.pervisitor"
                        DisplayColon="true" Field="CampaignGoalPerVisitorMin" />
                    <cms:FormLabel CssClass="sr-only" ID="lblPerVisitor" runat="server" EnableViewState="false" ResourceString="campaign.pervisitor"
                        DisplayColon="true" Field="CampaignGoalPerVisitor" />
                </div>
                <div class="editing-form-value-cell control-group-inline">
                    <cms:FormField runat="server" ID="fPerVisitorMin" Field="CampaignGoalPerVisitorMin" UseFFI="False">
                        <cms:TextBoxControl ID="txtVPerVisitorMin" runat="server" MaxLength="50" />
                    </cms:FormField>
                    <cms:FormField runat="server" ID="fPerVisitor" Field="CampaignGoalPerVisitor" UseFFI="False">
                        <cms:TextBoxControl ID="txtPerVisitor" runat="server" MaxLength="50" />
                    </cms:FormField>
                    <cms:FormField runat="server" ID="fPerVisitorPercent" Field="CampaignGoalPerVisitorPercent" UseFFI="False">
                        <cms:RadioButtonsControl ID="rbPerVisitorPercent" runat="server" />
                    </cms:FormField>
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-value-cell-offset">
                    <cms:FormSubmitButton ID="btnOK" runat="server" OnClick="btnOk_Click" />
                </div>
            </div>
            </div>
        </LayoutTemplate>
    </cms:UIForm>
</asp:Content>
