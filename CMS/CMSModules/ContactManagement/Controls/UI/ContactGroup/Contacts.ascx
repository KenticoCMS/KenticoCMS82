<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Contacts.ascx.cs" Inherits="CMSModules_ContactManagement_Controls_UI_ContactGroup_Contacts" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<%@ Register Src="~/CMSModules/ContactManagement/FormControls/ContactSelector.ascx"
    TagName="ContactSelector" TagPrefix="cms" %>

<asp:Panel ID="pnlSelector" runat="server" CssClass="cms-edit-menu">
    <cms:ContactSelector ID="contactSelector" runat="server" IsLiveSite="false" />
</asp:Panel>
<asp:Panel ID="pnlContent" runat="server" CssClass="PageContent">
    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional" ShowProgress="true">
        <ContentTemplate>
            <cms:MessagesPlaceHolder ID="plcMess" runat="server" />
            <cms:CMSUpdateProgress ID="loading" runat="server" HandlePostback="true" DisplayTimeout="100" />
            <cms:UniGrid runat="server" ID="gridElem" OrderBy="ContactLastName" ObjectType="om.contactgroupcontactlist"
                ShowObjectMenu="false" IsLiveSite="false" RememberStateByParam="issitemanager" Columns="ContactID,ContactFirstName,ContactLastName,ContactEmail,ContactStatusID,ContactCountryID,ContactGroupMemberFromCondition,ContactGroupMemberFromAccount,ContactGroupMemberFromManual,ContactSiteID">
                <GridActions Parameters="ContactID">
                    <ug:Action Name="edit" Caption="$om.contact.viewdetail$" FontIconClass="icon-edit" FontIconStyle="Allow"
                        ExternalSourceName="edit" CommandArgument="ContactID" ModuleName="CMS.OnlineMarketing" />
                    <ug:Action Name="remove" Caption="$General.Remove$" FontIconClass="icon-bin" FontIconStyle="Critical" Confirmation="$General.ConfirmRemove$"
                        ModuleName="CMS.OnlineMarketing" ExternalSourceName="remove" />
                </GridActions>
                <GridColumns>
                    <ug:Column Source="ContactFirstName" Caption="$om.contact.firstname$" Wrap="false">
                        <Filter Type="text" Size="100" />
                    </ug:Column>
                    <ug:Column Source="ContactLastName" Caption="$om.contact.lastname$" Wrap="false">
                        <Filter Type="text" Size="100" />
                    </ug:Column>
                    <ug:Column Source="ContactEmail" Caption="$general.emailaddress$" Wrap="false">
                        <Filter Type="text" Size="100" />
                    </ug:Column>
                    <ug:Column Source="ContactStatusID" AllowSorting="false" ExternalSourceName="#transform: om.contactstatus.contactstatusdisplayname"
                        Caption="$om.contactstatus$" Wrap="false">
                        <Filter Type="text" Format="ContactStatusID IN (SELECT ContactStatusID FROM OM_ContactStatus WHERE {3})"
                            Source="ContactStatusDisplayName" Size="100" />
                    </ug:Column>
                    <ug:Column Source="ContactCountryID" AllowSorting="false" ExternalSourceName="#transform: cms.country.countrydisplayname"
                        Caption="$objecttype.cms_country$" Wrap="false">
                        <Filter Type="text" Format="ContactCountryID IN (SELECT CountryID FROM CMS_Country WHERE {3})"
                            Source="CountryDisplayName" Size="100" />
                    </ug:Column>
                    <ug:Column Source="ContactSiteID" AllowSorting="false" Caption="$general.sitename$"
                        ExternalSourceName="#sitenameorglobal" Name="SiteName" Wrap="false" />
                    <ug:Column Source="ContactGroupMemberFromCondition" ExternalSourceName="#yesno" Caption="$om.contactgroupmember.memberfromcondition$"
                        Wrap="false">
                        <Filter Type="bool" Format="{2} = ISNULL(ContactGroupMemberFromCondition, 0)"/>
                    </ug:Column>
                    <ug:Column Source="ContactGroupMemberFromAccount" ExternalSourceName="#yesno" Caption="$om.contactgroupmember.MemberFromAccount$"
                        Wrap="false">
                        <Filter Type="bool" Format="{2} = ISNULL(ContactGroupMemberFromAccount, 0)"/>
                    </ug:Column>
                    <ug:Column Source="ContactGroupMemberFromManual" ExternalSourceName="#yesno" Caption="$om.contactgroupmember.MemberFromManual$"
                        Wrap="false">
                        <Filter Type="bool" Format="{2} = ISNULL(ContactGroupMemberFromManual, 0)"/>
                    </ug:Column>
                    <ug:Column CssClass="filling-column" />
                </GridColumns>
                <GridOptions DisplayFilter="true" ShowSelection="true" SelectionColumn="ContactID" />
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
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Panel>