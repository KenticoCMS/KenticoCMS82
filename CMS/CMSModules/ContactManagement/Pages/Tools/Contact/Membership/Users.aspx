<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Users.aspx.cs" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="Contact properties" Inherits="CMSModules_ContactManagement_Pages_Tools_Contact_Membership_Users"
    Theme="Default" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<%@ Register Src="~/CMSModules/Membership/FormControls/Users/selectuser.ascx" TagName="SelectUser"
    TagPrefix="cms" %>
<asp:Content ID="contentControls" ContentPlaceHolderID="plcActions" runat="server">
    <div class="PageHeaderItem">
        <cms:SelectUser runat="server" ID="selectUser" HideHiddenUsers="true" HideDisabledUsers="true"
            HideNonApprovedUsers="true" />
    </div>
    <div class="ClearBoth">
        &nbsp;
    </div>
</asp:Content>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
        <ContentTemplate>
            <cms:UniGrid runat="server" ID="gridElem" ObjectType="om.membershipuserlist" Columns="MembershipID,RelatedID,ContactSiteID,ContactFullNameJoined,ContactMergedWithContactID"
                IsLiveSite="false" ShowObjectMenu="false" OrderBy="RelatedID">
                <GridActions Parameters="MembershipID">
                    <ug:Action Name="delete" Caption="$General.Delete$" CommandArgument="MembershipID"
                        FontIconClass="icon-bin" FontIconStyle="Critical" ExternalSourceName="delete" Confirmation="$General.ConfirmDelete$" />
                </GridActions>
                <GridColumns>
                    <ug:Column Source="RelatedID" ExternalSourceName="#transform: cms.user : firstname #htmlencode" Caption="$general.firstname$" Wrap="false" AllowSorting="false" />
                    <ug:Column Source="RelatedID" ExternalSourceName="#transform: cms.user : lastname #htmlencode" Caption="$general.lastname$" Wrap="false" AllowSorting="false" />
                    <ug:Column Source="RelatedID" ExternalSourceName="#transform: cms.user : email #htmlencode" Caption="$general.email$" Wrap="false" AllowSorting="false" />
                    <ug:Column Source="RelatedID" ExternalSourceName="#transform:cms.user : username #htmlencode" Caption="$general.username$" Wrap="false"  AllowSorting="false" />
                    <ug:Column Source="ContactFullNameJoined" Caption="$om.contact.name$" Wrap="false"
                        Name="contactname" />
                    <ug:Column Source="ContactSiteID" ExternalSourceName="#sitenameorglobal" Caption="$general.sitename$"
                        Wrap="false" Name="sitename" />
                    <ug:Column CssClass="filling-column" />
                </GridColumns>
            </cms:UniGrid>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>
