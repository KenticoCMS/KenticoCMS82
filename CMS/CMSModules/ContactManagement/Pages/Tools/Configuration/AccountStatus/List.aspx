<%@ Page Language="C#" AutoEventWireup="true" CodeFile="List.aspx.cs" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="Account status list" Inherits="CMSModules_ContactManagement_Pages_Tools_Configuration_AccountStatus_List"
    Theme="Default" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/HeaderActions.ascx" TagName="HeaderActions"
    TagPrefix="cms" %>
<asp:Content ID="cntControls" runat="server" ContentPlaceHolderID="plcSiteSelector">
    <div class="form-horizontal form-filter">
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel runat="server" ID="lblSite" EnableViewState="false" DisplayColon="true"
                    ResourceString="General.Site" CssClass="control-label" />
            </div>
            <div class="filter-form-value-cell">
                <cms:SiteOrGlobalSelector ID="SiteOrGlobalSelector" ShortID="s" runat="server" PostbackOnDropDownChange="True" />
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="cntActions" runat="server" ContentPlaceHolderID="plcActions">
    <cms:CMSUpdatePanel ID="pnlActons" runat="server">
        <ContentTemplate>
            <div class="control-group-inline">
                <cms:HeaderActions ID="hdrActions" runat="server" IsLiveSite="false" />
                <cms:LocalizedLabel ID="lblWarnNew" runat="server" ResourceString="com.chooseglobalorsite"
                    EnableViewState="false" Visible="false" CssClass="button-explanation-text" />
            </div>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:UniGrid runat="server" ID="Grid" ObjectType="om.accountstatus" OrderBy="AccountStatusDisplayName"
        Columns="AccountStatusID,AccountStatusDisplayName,AccountStatusSiteID" IsLiveSite="false"
        EditActionUrl="Edit.aspx?statusid={0}" RememberStateByParam="issitemanager">
        <GridActions Parameters="AccountStatusID">
            <ug:Action Name="edit" Caption="$General.Edit$" FontIconClass="icon-edit" FontIconStyle="Allow" />
            <ug:Action Name="delete" Caption="$General.Delete$" FontIconClass="icon-bin" FontIconStyle="Critical" Confirmation="$General.ConfirmDelete$"
                ModuleName="CMS.OnlineMarketing" />
        </GridActions>
        <GridColumns>
            <ug:Column Source="AccountStatusDisplayName" Caption="$om.accountstatus.displayname$"
                Wrap="false">
                <Filter Type="text" />
            </ug:Column>
            <ug:Column Source="AccountStatusSiteID" AllowSorting="false" Caption="$general.site$" Wrap="false" ExternalSourceName="#sitenameorglobal"
                Name="sitename" Localize="true" />
            <ug:Column CssClass="filling-column" />
        </GridColumns>
        <GridOptions DisplayFilter="true" />
    </cms:UniGrid>
</asp:Content>
