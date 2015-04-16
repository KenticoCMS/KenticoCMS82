<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MergeSplit.ascx.cs" Inherits="CMSModules_ContactManagement_Controls_UI_Account_MergeSplit" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<%@ Register Src="~/CMSModules/ContactManagement/Controls/UI/Account/Filter.ascx"
    TagName="Filter" TagPrefix="cms" %>

<cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <cms:MessagesPlaceHolder ID="plcMess" runat="server" />
        <cms:LocalizedHeading ID="headTitle" runat="server" ResourceString="om.account.splitaccount" Level="4"
            EnableViewState="false" CssClass="listing-title" DisplayColon="true" />
        <cms:UniGrid runat="server" ID="gridElem" ObjectType="om.accountlist" OrderBy="AccountName"
            Columns="AccountID,AccountName,AccountEmail,AccountStatusID,PrimaryContactFullName,AccountSiteID"
            IsLiveSite="false" ShowObjectMenu="false" HideFilterButton="true" RememberState="false">
            <GridActions>
                <ug:Action ExternalSourceName="edit" Name="edit" Caption="$om.account.viewdetail$"
                    FontIconClass="icon-edit" FontIconStyle="Allow" ModuleName="CMS.OnlineMarketing" CommandArgument="AccountID" />
            </GridActions>
            <GridColumns>
                <ug:Column Source="AccountName" Caption="$om.account.name$" Wrap="false">
                </ug:Column>
                <ug:Column Source="AccountEmail" Caption="$general.email$" Wrap="false">
                </ug:Column>
                <ug:Column Source="AccountStatusID" AllowSorting="false" ExternalSourceName="#transform: om.accountstatus.accountstatusdisplayname"
                    Caption="$om.accountstatus$" Wrap="false">
                </ug:Column>
                <ug:Column Source="PrimaryContactFullName" Caption="$om.contact.primary$" Wrap="false">
                </ug:Column>
                <ug:Column Source="AccountSiteID" AllowSorting="false" ExternalSourceName="#sitenameorglobal"
                    Name="sitename" Caption="$general.sitename$" Wrap="false" Localize="true">
                </ug:Column>
                <ug:Column Source="AccountGUID" Visible="false" />
                <ug:Column CssClass="filling-column" />
            </GridColumns>
            <GridOptions DisplayFilter="true" ShowSelection="true" FilterPath="~/CMSModules/ContactManagement/Controls/UI/Account/Filter.ascx" />
        </cms:UniGrid>
        <asp:Panel ID="pnlFooter" runat="server">
            <cms:LocalizedHeading runat="server" Level="4" ResourceString="om.contact.splitsettings" />
            <asp:Panel ID="pnlSettings" runat="server" CssClass="checkbox-list-vertical">
                <cms:CMSCheckBox ID="chkCopyMissingFields" runat="server" ResourceString="om.contact.fillfieldsinaccounts" />
                <cms:CMSCheckBox ID="chkRemoveContacts" runat="server" ResourceString="om.account.removecontacts" />
                <cms:CMSCheckBox ID="chkRemoveContactGroups" runat="server" ResourceString="om.account.removeacontactgroups" />
            </asp:Panel>
            <asp:Panel ID="pnlButton" runat="server" class="form-horizontal mass-action">
                <div class="form-group">
                    <div class="mass-action-value-cell">
                        <cms:LocalizedButton ID="btnSplit" runat="server" ButtonStyle="Primary" ResourceString="om.contact.splitselected" />
                    </div>
                </div>
            </asp:Panel>
        </asp:Panel>
        <asp:HiddenField ID="hdnValue" runat="server" EnableViewState="false" />
    </ContentTemplate>
</cms:CMSUpdatePanel>