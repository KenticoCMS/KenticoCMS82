<%@ Control Language="C#" AutoEventWireup="true" CodeFile="List.ascx.cs" Inherits="CMSModules_ContactManagement_Controls_UI_Contact_List" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<%@ Register Src="~/CMSModules/ContactManagement/Controls/UI/Contact/Filter.ascx" TagPrefix="cms" TagName="ContactFilter" %>

<cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <cms:MessagesPlaceHolder ID="plcMess" runat="server" />
        <cms:UniGrid runat="server" ID="gridElem" ObjectType="om.contactlist" OrderBy="ContactLastName"
            Columns="ContactID,ContactLastName,ContactFirstName,ContactEmail,ContactStatusID,ContactCountryID,ContactSiteID,ContactMergedWithContactID,ContactGlobalContactID, ContactCreated"
            IsLiveSite="false" HideFilterButton="true" RememberDefaultState="true" RememberStateByParam="issitemanager" FilterLimit="0">
            <GridActions Parameters="ContactID">
                <ug:Action Name="edit" Caption="$General.Edit$" FontIconClass="icon-edit" FontIconStyle="Allow" />
                <ug:Action ExternalSourceName="delete" Name="delete" Caption="$General.Delete$" FontIconClass="icon-bin" FontIconStyle="Critical"
                    ModuleName="CMS.ContactManagement" Permissions="ModifyContacts" />
            </GridActions>
            <GridColumns>
                <ug:Column Source="ContactFirstName" Caption="$om.contact.firstname$" Wrap="false" />
                <ug:Column Source="ContactLastName" Caption="$om.contact.lastname$" Wrap="false" />
                <ug:Column Source="ContactEmail" Caption="$general.emailaddress$" Wrap="false" />
                <ug:Column Source="ContactStatusID" ExternalSourceName="#transform: om.contactstatus.contactstatusdisplayname" AllowSorting="false" Caption="$om.contactstatus$" Wrap="false" />
                <ug:Column Source="ContactCountryID" ExternalSourceName="#transform: cms.country.countrydisplayname" AllowSorting="false" Caption="$general.country$" Wrap="false" />
                <ug:Column Source="ContactSiteID" Caption="$general.site$" Wrap="false" AllowSorting="false" ExternalSourceName="#sitenameorglobal" Name="sitedisplayname" Localize="true" />
                <ug:Column Source="##ALL##" Caption="$om.contact.mergedinto$" Wrap="false" ExternalSourceName="ContactMergedWithContactID" Name="merged" />
                <ug:Column Source="ContactCreated" Caption="$general.created$" Wrap="false" />
                <ug:Column Source="ContactGUID" Visible="false" />
                <ug:Column CssClass="filling-column" />
            </GridColumns>
            <GridOptions DisplayFilter="true" ShowSelection="true" FilterPath="~/CMSModules/ContactManagement/Controls/UI/Contact/Filter.ascx" />
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
        <asp:HiddenField ID="hdnIdentifier" runat="server" EnableViewState="false" />
        <asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
    </ContentTemplate>
</cms:CMSUpdatePanel>