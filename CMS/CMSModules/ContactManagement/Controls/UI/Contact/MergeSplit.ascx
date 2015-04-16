<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MergeSplit.ascx.cs" Inherits="CMSModules_ContactManagement_Controls_UI_Contact_MergeSplit" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<%@ Register Src="~/CMSModules/ContactManagement/Controls/UI/Contact/Filter.ascx" TagPrefix="cms" TagName="ContactFilter" %>

<cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <cms:MessagesPlaceHolder ID="plcMess" runat="server" />
        <cms:LocalizedHeading ID="headTitle" runat="server" ResourceString="om.contact.splitcontacts" Level="4"
            EnableViewState="false" CssClass="listing-title" />
        <cms:UniGrid runat="server" ID="gridElem" ObjectType="om.contactlist" OrderBy="ContactLastName"
            Columns="ContactID,ContactLastName,ContactFirstName,ContactEmail,ContactStatusID,ContactCountryID,ContactMergedWhen,ContactSiteID"
            IsLiveSite="false" ShowObjectMenu="false" HideFilterButton="true" RememberState="false">
            <GridActions>
                <ug:Action ExternalSourceName="edit" Name="edit" Caption="$om.contact.viewdetail$"
                    FontIconClass="icon-edit" FontIconStyle="Allow" ModuleName="CMS.OnlineMarketing" CommandArgument="ContactID" />
            </GridActions>
            <GridColumns>
                <ug:Column Source="ContactFirstName" Caption="$om.contact.firstname$" Wrap="false">
                </ug:Column>
                <ug:Column Source="ContactLastName" Caption="$om.contact.lastname$" Wrap="false">
                </ug:Column>
                <ug:Column Source="ContactEmail" Caption="$general.emailaddress$" Wrap="false">
                </ug:Column>
                <ug:Column Source="ContactStatusID" AllowSorting="false" ExternalSourceName="#transform: om.contactstatus.contactstatusdisplayname"
                    Caption="$om.contactstatus$" Wrap="false">
                </ug:Column>
                <ug:Column Source="ContactCountryID" AllowSorting="false" ExternalSourceName="#transform: cms.country.countrydisplayname"
                    Caption="$general.country$" Wrap="false">
                </ug:Column>
                <ug:Column Source="ContactMergedWhen" Name="mergedwhen" Caption="$om.contact.mergedwhen$"
                    Wrap="false">
                </ug:Column>
                <ug:Column Source="ContactSiteID" AllowSorting="false" Name="sitename" ExternalSourceName="#sitenameorglobal"
                    Caption="$general.sitename$" Wrap="false" Localize="true">
                </ug:Column>
                <ug:Column Source="ContactGUID" Visible="false" />
                <ug:Column CssClass="filling-column" />
            </GridColumns>
            <GridOptions DisplayFilter="true" ShowSelection="true" FilterPath="~/CMSModules/ContactManagement/Controls/UI/Contact/Filter.ascx" />
        </cms:UniGrid>
        <asp:Panel ID="pnlFooter" runat="server">
            <cms:LocalizedHeading runat="server" Level="4" ResourceString="om.contact.splitsettings" />
            <asp:Panel ID="pnlSettings" runat="server" CssClass="checkbox-list-vertical">
                <cms:CMSCheckBox ID="chkCopyActivities" runat="server" ResourceString="om.contact.copyactivities" />
                <cms:CMSCheckBox ID="chkCopyMissingFields" runat="server" ResourceString="om.contact.fillfields" />
                <cms:CMSCheckBox ID="chkRemoveAccounts" runat="server" ResourceString="om.contact.removeaccounts" />
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