<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ContactGroups.ascx.cs"
    Inherits="CMSModules_ContactManagement_Controls_UI_Contact_ContactGroups" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <cms:UniGrid runat="server" ID="gridElem" ShortID="g" OrderBy="ContactGroupDisplayName" ObjectType="om.contactgroupcontactlist"
            IsLiveSite="false" Columns="ContactID, ContactGroupMemberFromCondition, ContactGroupMemberFromAccount, ContactGroupMemberFromManual, ContactGroupDisplayName, ContactGroupID"
            ShowObjectMenu="false" RememberStateByParam="issitemanager">
            <GridActions Parameters="ContactGroupID">
                    <ug:Action Name="remove" Caption="$General.Remove$" CommandArgument="ContactGroupID"
                        FontIconClass="icon-bin" FontIconStyle="Critical" ExternalSourceName="remove" Confirmation="$General.ConfirmRemove$"
                        ModuleName="CMS.Contact" />
                </GridActions>
            <GridColumns>
                <ug:Column Source="ContactGroupDisplayName" Caption="$general.displayname$" Wrap="false" />
                <ug:Column Source="ContactGroupMemberFromCondition" Caption="$om.contactgroupmember.memberfromcondition$"
                    ExternalSourceName="#yesno" Wrap="false" />
                <ug:Column Source="ContactGroupMemberFromAccount" Caption="$om.contactgroupmember.memberfromaccount$"
                    ExternalSourceName="#yesno" Wrap="false" />
                <ug:Column Source="ContactGroupMemberFromManual" Caption="$om.contactgroupmember.memberfrommanual$"
                    ExternalSourceName="#yesno" Wrap="false" />
                <ug:Column CssClass="filling-column" />
            </GridColumns>
        </cms:UniGrid>
    </ContentTemplate>
</cms:CMSUpdatePanel>
