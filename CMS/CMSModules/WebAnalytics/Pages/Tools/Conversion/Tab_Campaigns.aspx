<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Tab_Campaigns.aspx.cs" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Theme="Default" Inherits="CMSModules_WebAnalytics_Pages_Tools_Conversion_Tab_Campaigns" %>

<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <asp:PlaceHolder ID="plcTable" runat="server">
        <cms:LocalizedHeading runat="server" ID="headTitle" Level="4" ResourceString="conversion.avaiblecampaign" CssClass="listing-title" EnableViewState="false" />
        <cms:UniSelector ID="usCampaigns" runat="server" IsLiveSite="false" ObjectType="analytics.campaign"
            SelectionMode="Multiple" ResourcePrefix="campaignselect" />
    </asp:PlaceHolder>
</asp:Content>
