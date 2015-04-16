<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Tab_Conversions.aspx.cs"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Theme="Default" Inherits="CMSModules_WebAnalytics_Pages_Tools_Campaign_Tab_Conversions" %>

<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <div class="form-horizontal">
        <div class="radio-list-vertical">
            <cms:CMSRadioButton ID="rbAllConversions" runat="server" ResourceString="campaign.allconversions" 
                GroupName="grpConversions" AutoPostBack="true" />
            <cms:CMSRadioButton ID="rbSelectedConversions" runat="server" ResourceString="campaign.selectedconversions"
                GroupName="grpConversions" AutoPostBack="true" />

            <asp:PlaceHolder ID="plcTable" runat="server">
                <div class="selector-subitem">
                    <cms:LocalizedHeading runat="server" ID="headTitle" Level="4" ResourceString="campaign.availableconversions" CssClass="listing-title" EnableViewState="false" />
                    <cms:UniSelector ID="usConversions" runat="server" IsLiveSite="false" ObjectType="analytics.conversion"
                        SelectionMode="Multiple" ResourcePrefix="conversionselect" />
                </div>
            </asp:PlaceHolder>
        </div>
    </div>
</asp:Content>