<%@ Page Language="C#" AutoEventWireup="true" CodeFile="List.aspx.cs" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="Objectworkflowtrigger list" Inherits="CMSModules_ContactManagement_Pages_Tools_Automation_Process_Trigger_List"
    Theme="Default" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/HeaderActions.ascx" TagName="HeaderActions"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Sites/SiteSelector.ascx" TagName="SiteSelector"
    TagPrefix="cms" %>
<asp:Content ID="cntSiteSelector" runat="server" ContentPlaceHolderID="plcSiteSelector">
    <div class="form-horizontal form-filter">
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblSite" EnableViewState="false" DisplayColon="true" ResourceString="General.Site" />
            </div>
            <div class="filter-form-value-cell-wide">
                <cms:SiteOrGlobalSelector ID="siteOrGlobalSelector" ShortID="sg" runat="server" PostbackOnDropDownChange="true" />
                <cms:SiteSelector ID="siteSelector" runat="server" ShortID="s" AllowGlobal="true"
                    AllowAll="true" IsLiveSite="false" PostbackOnDropDownChange="true" />
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="cntActions" runat="server" ContentPlaceHolderID="plcActions">
    <cms:CMSUpdatePanel ID="pnlActons" runat="server">
        <ContentTemplate>
            <div class="control-group-inline">
                <cms:HeaderActions ID="headerActions" runat="server" IsLiveSite="false" />
                <cms:LocalizedLabel ID="lblWarnNew" runat="server" ResourceString="com.chooseglobalorsite"
                    EnableViewState="false" Visible="false" CssClass="button-explanation-text" />
            </div>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:UniGrid runat="server" ID="gridElem" ObjectType="cms.objectworkflowtrigger"
        OrderBy="TriggerDisplayName" Columns="TriggerID, TriggerMacroCondition, TriggerType, TriggerSiteID, TriggerDisplayName, TriggerObjectType, TriggerTargetObjectID, TriggerTargetObjectType, TriggerParameters"
        IsLiveSite="false" EditActionUrl="Edit.aspx?objectworkflowtriggerId={0}">
        <GridActions Parameters="TriggerID">
            <ug:Action Name="edit" Caption="$General.Edit$" FontIconClass="icon-edit" FontIconStyle="Allow" />
            <ug:Action Name="#delete" ExternalSourceName="delete" Caption="$General.Delete$"
                FontIconClass="icon-bin" FontIconStyle="Critical" Confirmation="$General.ConfirmDelete$" ModuleName="CMS.OnlineMarketing" Permissions="ManageProcesses" />
        </GridActions>
        <GridColumns>
            <ug:Column Source="TriggerDisplayName" Caption="$ma.trigger.name$" Wrap="false">
                <Filter Type="text" />
            </ug:Column>
            <ug:Column Source="##ALL##" Caption="$ma.trigger.type$" Wrap="false" ExternalSourceName="type"
                AllowSorting="false" />
            <ug:Column Source="TriggerMacroCondition" Caption="$ma.trigger.macrocondition$" Wrap="false"
                AllowSorting="false" ExternalSourceName="condition" />
            <ug:Column Source="TriggerSiteID" AllowSorting="false" Caption="$general.site$" Wrap="false"
                ExternalSourceName="#sitenameorglobal" Name="sitename" Localize="true" />
            <ug:Column CssClass="filling-column" />
        </GridColumns>
        <GridOptions DisplayFilter="true" />
    </cms:UniGrid>
</asp:Content>
