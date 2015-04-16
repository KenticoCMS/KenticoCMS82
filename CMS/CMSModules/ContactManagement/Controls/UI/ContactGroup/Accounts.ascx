<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Accounts.ascx.cs" Inherits="CMSModules_ContactManagement_Controls_UI_ContactGroup_Accounts" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<%@ Register Src="~/CMSModules/ContactManagement/FormControls/AccountSelector.ascx"
    TagName="AccountSelector" TagPrefix="cms" %>

<asp:Panel ID="pnlSelector" runat="server" CssClass="cms-edit-menu">
    <cms:AccountSelector ID="accountSelector" runat="server" IsLiveSite="false" />
</asp:Panel>
<asp:Panel ID="pnlContent" runat="server" CssClass="PageContent">
    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <cms:MessagesPlaceHolder ID="plcMess" runat="server" />
            <cms:UniGrid runat="server" ID="gridElem" OrderBy="AccountName" ObjectType="om.contactgroupaccountlist"
                ShowObjectMenu="false" IsLiveSite="false" Columns="AccountID,AccountName,AccountStatusID,AccountCountryID,AccountSiteID" RememberStateByParam="issitemanager">
                <GridActions Parameters="AccountID">
                    <ug:Action Name="edit" Caption="$om.account.viewdetail$" FontIconClass="icon-edit" FontIconStyle="Allow"
                        CommandArgument="AccountID" ModuleName="CMS.OnlineMarketing" ExternalSourceName="edit" />
                    <ug:Action Name="remove" Caption="$General.Remove$" FontIconClass="icon-bin" FontIconStyle="Critical" Confirmation="$General.ConfirmRemove$"
                        ExternalSourceName="remove" />
                </GridActions>
                <GridColumns>
                    <ug:Column Source="AccountName" Caption="$om.account.name$" Wrap="false">
                        <Filter Type="text" Size="100" />
                    </ug:Column>
                    <ug:Column Source="AccountStatusID" AllowSorting="false" ExternalSourceName="#transform: om.accountstatus.accountstatusdisplayname"
                        Caption="$om.accountstatus.name$" Wrap="false">
                        <Filter Type="text" Format="AccountStatusID IN (SELECT AccountStatusID FROM OM_AccountStatus WHERE {3})"
                            Source="AccountStatusDisplayName" Size="100" />
                    </ug:Column>
                    <ug:Column Source="AccountCountryID" AllowSorting="false" ExternalSourceName="#transform: cms.country.countrydisplayname"
                        Caption="$objecttype.cms_country$" Wrap="false">
                        <Filter Type="text" Format="AccountCountryID IN (SELECT CountryID FROM CMS_Country WHERE {3})"
                            Source="CountryDisplayName" Size="100" />
                    </ug:Column>
                    <ug:Column Source="AccountSiteID" AllowSorting="false" Caption="$general.sitename$"
                        ExternalSourceName="#sitenameorglobal" Name="SiteName" Wrap="false" />
                    <ug:Column CssClass="filling-column" />
                </GridColumns>
                <GridOptions DisplayFilter="true" ShowSelection="true" SelectionColumn="AccountID" />
            </cms:UniGrid>
            <asp:Panel ID="pnlFooter" runat="server" CssClass="form-horizontal mass-action">
                <div class="form-group">
                    <div class="mass-action-value-cell">
                        <cms:LocalizedLabel runat="server" AssociatedControlID="drpWhat" CssClass="sr-only" ResourceString="general.scope" />
                        <cms:CMSDropDownList ID="drpWhat" runat="server" />
                        <cms:LocalizedLabel runat="server" AssociatedControlID="drpAction" CssClass="sr-only" ResourceString="general.action" />
                        <cms:CMSDropDownList ID="drpAction" runat="server" />
                        <cms:LocalizedButton ID="btnOk" runat="server" ResourceString="general.ok" ButtonStyle="Primary"
                            EnableViewState="false" OnClick="btnOk_Click" />
                    </div>
                </div>
                <asp:Label ID="lblInfo" runat="server" CssClass="InfoLabel" EnableViewState="false" />
            </asp:Panel>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Panel>