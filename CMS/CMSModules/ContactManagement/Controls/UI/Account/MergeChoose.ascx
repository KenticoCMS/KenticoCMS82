<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MergeChoose.ascx.cs" Inherits="CMSModules_ContactManagement_Controls_UI_Account_MergeChoose" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>

<%@ Register Src="~/CMSModules/ContactManagement/Controls/UI/Account/Filter.ascx" TagPrefix="cms" TagName="AccountFilter" %>
<cms:MessagesPlaceHolder ID="plcMess" runat="server" />
<cms:LocalizedHeading ID="headTitle" runat="server" ResourceString="om.account.choosemerge" Level="4"
    EnableViewState="false" CssClass="listing-title" DisplayColon="true" />
<cms:UniGrid runat="server" ID="gridElem" ObjectType="om.accountlist" OrderBy="AccountName"
    Columns="AccountID,AccountName,AccountEmail,AccountStatusID,PrimaryContactFullName,AccountSiteID"
    IsLiveSite="false" ShowObjectMenu="false" HideFilterButton="true" RememberState="false">
    <GridColumns>
        <ug:Column Source="AccountName" Caption="$om.account.name$" Wrap="false">
        </ug:Column>
        <ug:Column Source="AccountEmail" Caption="$general.emailaddress$" Wrap="false">
        </ug:Column>
        <ug:Column Source="AccountStatusID" AllowSorting="false" ExternalSourceName="#transform: om.accountstatus.accountstatusdisplayname"
            Caption="$om.accountstatus$" Wrap="false">
        </ug:Column>
        <ug:Column Source="PrimaryContactFullName" Caption="$om.contact.primary$" Wrap="false">
        </ug:Column>
        <ug:Column Source="AccountSiteID" AllowSorting="false" Name="sitename" ExternalSourceName="#sitenameorglobal"
            Caption="$general.sitename$" Wrap="false" Localize="true">
        </ug:Column>
        <ug:Column Source="AccountGUID" Visible="false" />
        <ug:Column CssClass="filling-column" />
    </GridColumns>
    <GridOptions DisplayFilter="true" ShowSelection="true" FilterPath="~/CMSModules/ContactManagement/Controls/UI/Account/Filter.ascx" />
</cms:UniGrid>
<asp:Panel ID="pnlButton" runat="server" class="form-horizontal mass-action">
    <div class="form-group">
        <div class="mass-action-value-cell">
            <cms:LocalizedButton ID="btnMergeSelected" runat="server" ButtonStyle="Primary"
                ResourceString="om.contact.mergeselected" />
            <cms:LocalizedButton ID="btnMergeAll" runat="server" ButtonStyle="Default"
                ResourceString="om.contact.mergeall" />
        </div>
    </div>
</asp:Panel>
<asp:HiddenField ID="hdnIdentifier" runat="server" EnableViewState="false" />