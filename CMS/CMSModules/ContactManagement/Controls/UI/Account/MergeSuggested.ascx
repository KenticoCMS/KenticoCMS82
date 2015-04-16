<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MergeSuggested.ascx.cs"
    Inherits="CMSModules_ContactManagement_Controls_UI_Account_MergeSuggested" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<%@ Register Src="~/CMSModules/ContactManagement/Controls/UI/Account/FilterSuggest.ascx"
    TagName="Filter" TagPrefix="cms" %>

<cms:MessagesPlaceHolder ID="plcMess" runat="server" />
<cms:Filter ID="filter" runat="server" ShortID="f" IsLiveSite="false" />
<cms:LocalizedHeading ID="headTitle" Level="4" runat="server" ResourceString="om.account.choosemerge"
    EnableViewState="false" CssClass="listing-title" DisplayColon="true" />
<cms:UniGrid runat="server" ID="gridElem" ObjectType="om.accountlist" OrderBy="AccountName"
    Columns="AccountID,AccountName,AccountAddress1,AccountAddress2,AccountCity,AccountZIP,AccountWebSite,AccountPhone,AccountEmail,AccountFax,PrimaryContactFullName,SecondaryContactFullName,AccountSiteID,AccountStatusID"
    IsLiveSite="false" ShowObjectMenu="false" RememberStateByParam="issitemanager">
    <GridColumns>
        <ug:Column Source="AccountName" Caption="$om.account.name$" Wrap="false">
        </ug:Column>
        <ug:Column Source="AccountStatusID" AllowSorting="false" ExternalSourceName="#transform: om.accountstatus.accountstatusdisplayname" Caption="$om.accountstatus$" Wrap="false">
        </ug:Column>
        <ug:Column Source="AccountAddress1" Name="address1" Caption="$om.contact.address1$" Wrap="false">
        </ug:Column>
        <ug:Column Source="AccountAddress2" Name="address2" Caption="$om.contact.address2$" Wrap="false">
        </ug:Column>
        <ug:Column Source="AccountCity" Name="city" Caption="$general.city$" Wrap="false">
        </ug:Column>
        <ug:Column Source="AccountZIP" Name="zip" Caption="$general.zip$" Wrap="false">
        </ug:Column>
        <ug:Column Source="AccountWebSite" Name="website" Caption="$om.account.url$" Wrap="false">
        </ug:Column>
        <ug:Column Source="AccountPhone" Name="phone" Caption="$general.phone$" Wrap="false">
        </ug:Column>
        <ug:Column Source="AccountFax" Name="fax" Caption="$general.fax$" Wrap="false">
        </ug:Column>
        <ug:Column Source="AccountEmail" Name="email" Caption="$general.email$" Wrap="false">
        </ug:Column>
        <ug:Column Source="PrimaryContactFullName" Caption="$om.contact.primary$" Name="primarycontactfullname"
            Wrap="false">
        </ug:Column>
        <ug:Column Source="SecondaryContactFullName" Caption="$om.contact.secondary$" Name="secondarycontactfullname"
            Wrap="false">
        </ug:Column>
        <ug:Column Source="AccountSiteID" AllowSorting="false" Name="sitename" ExternalSourceName="#sitenameorglobal"
            Caption="$general.sitename$" Wrap="false" Localize="true">
        </ug:Column>
        <ug:Column CssClass="filling-column" />
    </GridColumns>
    <GridOptions DisplayFilter="false" ShowSelection="true" />
</cms:UniGrid>
<asp:Panel ID="pnlFooter" runat="server" class="form-horizontal mass-action">
    <div class="form-group">
        <div class="mass-action-value-cell">
            <cms:LocalizedButton ID="btnMergeSelected" runat="server" ButtonStyle="Primary"
                ResourceString="om.contact.mergeselected" />
            <cms:LocalizedButton ID="btnMergeAll" runat="server" ButtonStyle="Primary"
                ResourceString="om.contact.mergeall" />
        </div>
    </div>
</asp:Panel>
<asp:HiddenField ID="hdnIdentifier" runat="server" EnableViewState="false" />