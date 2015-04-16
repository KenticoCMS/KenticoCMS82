<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Scoring.ascx.cs" Inherits="CMSModules_ContactManagement_Controls_UI_Contact_Scoring" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <cms:UniGrid runat="server" ID="gridElem" ShortID="g" OrderBy="ScoreDisplayName" ObjectType="om.contactscorelist"
            IsLiveSite="false" Columns="ScoreID, ScoreDisplayName, ScoreValue" ShowObjectMenu="false" RememberStateByParam="issitemanager">
            <GridActions Parameters="ScoreID">
                <ug:Action Name="view" ExternalSourceName="view" Caption="$General.View$" FontIconClass="icon-edit" FontIconStyle="Allow" />
            </GridActions>
            <GridColumns>
                <ug:Column Source="ScoreDisplayName" Localize="true" Caption="$scoreselect.itemname$" Wrap="false" />
                <ug:Column Source="ScoreValue" Caption="$om.score$" Wrap="false" />
                <ug:Column Source="##ALL##" Caption="$general.status$" ExternalSourceName="scorestatus" Wrap="false" />
                <ug:Column CssClass="filling-column" />
            </GridColumns>
        </cms:UniGrid>
    </ContentTemplate>
</cms:CMSUpdatePanel>
