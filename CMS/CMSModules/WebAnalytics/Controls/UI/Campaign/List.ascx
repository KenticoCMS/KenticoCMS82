<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_WebAnalytics_Controls_UI_Campaign_List"
    CodeFile="List.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<cms:UniGrid runat="server" ID="gridElem" ObjectType="analytics.campaign" OrderBy="CampaignDisplayName"
    Columns="CampaignID,CampaignDisplayName,CampaignOpenTo,CampaignEnabled,CampaignOpenFrom"
    IsLiveSite="false" >
    <GridActions Parameters="CampaignID">
        <ug:Action Name="edit" Caption="$General.Edit$" FontIconClass="icon-edit" FontIconStyle="Allow" />
        <ug:Action Name="#delete" Caption="$General.Delete$" FontIconClass="icon-bin" FontIconStyle="Critical" Confirmation="$General.ConfirmDelete$"
            ModuleName="CMS.WebAnalytics" Permissions="ManageCampaigns" />
    </GridActions>
    <GridColumns>
        <ug:Column Source="CampaignDisplayName" Caption="$campaignselect.itemname$" Wrap="false">
            <Filter Type="text" />
        </ug:Column>
        <ug:Column Source="CampaignOpenFrom" Caption="$general.openfrom$" Wrap="false">
            <Filter Type="text" />
        </ug:Column>
        <ug:Column Source="CampaignOpenTo" Caption="$general.opento$" Wrap="false">
            <Filter Type="text" />
        </ug:Column>
        <ug:Column Source="CampaignEnabled" Caption="$general.enabled$" Wrap="false" ExternalSourceName="#yesno">
            <Filter Type="bool" />
        </ug:Column>
        <ug:Column CssClass="filling-column" />
    </GridColumns>
    <GridOptions DisplayFilter="true" />
</cms:UniGrid>
