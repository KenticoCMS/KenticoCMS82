<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CampaignReportHeader.ascx.cs"
    Inherits="CMSModules_WebAnalytics_Controls_CampaignReportHeader" %>
<%@ Register Src="~/CMSModules/WebAnalytics/FormControls/SelectCampaign.ascx" TagName="SelectCampaign"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/WebAnalytics/FormControls/SelectConversion.ascx" TagName="SelectConversion"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Sites/SiteSelector.ascx" TagName="SelectSite"
    TagPrefix="cms" %>

<div class="form-horizontal form-filter">
    <asp:PlaceHolder runat="server" ID="pnlGoal" Visible="false">
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel ID="lblGoal" CssClass="control-label" runat="server" ResourceString="campaign.goal" DisplayColon="true" AssociatedControlID="drpGoals" />
            </div>
            <div class="filter-form-value-cell">
                <cms:CMSDropDownList runat="server" ID="drpGoals" CssClass="SmallDropDown " />
            </div>
        </div>
    </asp:PlaceHolder>
    <asp:PlaceHolder runat="server" ID="pnlCampaign">
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel ID="lblCampaign" CssClass="control-label" runat="server" ResourceString="analytics.campaign" AssociatedControlID="ucSelectCampaign"
                    DisplayColon="true" />
            </div>
            <div class="filter-form-value-cell">
                <cms:SelectCampaign runat="server" ID="ucSelectCampaign" AllowAll="true" PostbackOnChange="true" />
            </div>
        </div>
    </asp:PlaceHolder>
    <asp:PlaceHolder runat="server" ID="pnlConversion">
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel ID="lblConversion" CssClass="control-label" runat="server" ResourceString="analytics.conversion" AssociatedControlID="usSelectConversion"
                    DisplayColon="true" />
            </div>
            <div class="filter-form-value-cell">
                <cms:SelectConversion runat="server" ID="usSelectConversion" PostbackOnDropDownChange="true" />
            </div>
        </div>
    </asp:PlaceHolder>
    <asp:PlaceHolder runat="server" ID="pnlSite" Visible="false">
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel ID="lblSelectSite" CssClass="control-label" runat="server" ResourceString="general.site" AssociatedControlID="usSite"
                    DisplayColon="true" />
            </div>
            <div class="filter-form-value-cell">
                <cms:SelectSite runat="server" ID="usSite" IsLiveSite="false" PostbackOnDropDownChange="true" />
            </div>
        </div>
    </asp:PlaceHolder>
</div>