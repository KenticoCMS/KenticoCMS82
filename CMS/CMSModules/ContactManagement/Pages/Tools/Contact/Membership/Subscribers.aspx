<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Subscribers.aspx.cs" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="Contact properties" Inherits="CMSModules_ContactManagement_Pages_Tools_Contact_Membership_Subscribers"
    Theme="Default" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<%@ Register Src="~/CMSModules/Newsletters/FormControls/NewsletterSubscriberSelector.ascx"
    TagName="SelectSubscriber" TagPrefix="cms" %>
<asp:Content ID="contentControls" ContentPlaceHolderID="plcActions" runat="server">
    <div class="PageHeaderItem">
        <cms:SelectSubscriber runat="server" ID="selectSubscriber" />
    </div>
    <div class="ClearBoth">
        &nbsp;
    </div>
</asp:Content>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
        <ContentTemplate>
            <cms:UniGrid runat="server" ID="gridElem" ObjectType="om.membershipsubscriberlist"
                Columns="MembershipID,RelatedID,ContactSiteID,ContactFullNameJoined,ContactMergedWithContactID"
                IsLiveSite="false" ShowObjectMenu="false" OrderBy="RelatedID">
                <GridActions Parameters="MembershipID">
                    <ug:Action Name="delete" CommandArgument="MembershipID" Caption="$General.Delete$"
                        FontIconClass="icon-bin" FontIconStyle="Critical" ExternalSourceName="delete" Confirmation="$General.ConfirmDelete$" />
                </GridActions>
                <GridColumns>
                    <ug:Column Source="RelatedID" ExternalSourceName="#transform: newsletter.subscriber.subscriberfirstname" Caption="$general.firstname$"
                        Wrap="false" AllowSorting="false" />
                    <ug:Column Source="RelatedID" ExternalSourceName="#transform: newsletter.subscriber.subscriberlastname" Caption="$general.lastname$"
                        Wrap="false" AllowSorting="false" />
                    <ug:Column Source="RelatedID" ExternalSourceName="#transform: newsletter.subscriber.subscriberemail" Caption="$general.email$"
                        Wrap="false" AllowSorting="false" />
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
