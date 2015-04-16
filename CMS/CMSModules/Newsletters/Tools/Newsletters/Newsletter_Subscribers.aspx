<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Newsletters_Tools_Newsletters_Newsletter_Subscribers"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Newsletters - Subscribers"
    EnableEventValidation="false" CodeFile="Newsletter_Subscribers.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Membership/FormControls/Roles/selectrole.ascx" TagName="SelectRole"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Membership/FormControls/Users/selectuser.ascx" TagName="SelectUser"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Newsletters/FormControls/NewsletterSubscriberSelector.ascx"
    TagName="SelectSubscriber" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>

<asp:Content ID="contentControls" ContentPlaceHolderID="plcBeforeContent" runat="server">
    <div class="cms-edit-menu uniselector-action-buttons">
        <cms:SelectSubscriber runat="server" ID="selectSubscriber" ShortID="ss" />
        <cms:SelectUser runat="server" ID="selectUser" HideHiddenUsers="true" HideDisabledUsers="true"
            HideNonApprovedUsers="true" ShortID="su" />
        <cms:SelectRole runat="server" ID="selectRole" ShortID="sr" />
        <asp:PlaceHolder runat="server" ID="plcSelectCG" />
    </div>
</asp:Content>
<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <cms:CMSUpdatePanel runat="server" ID="pnlUpdate" UpdateMode="Conditional">
        <ContentTemplate>
            <cms:UniGrid ID="UniGridSubscribers" runat="server" ShortID="g" OrderBy="SubscriberFullName"
                IsLiveSite="false" ShowObjectMenu="false" ObjectType="newsletter.subscribernewsletterlist" Columns="SubscriberID, SubscriberFullName, SubscriberEmail, Email, SubscriptionApproved, SubscriberType, SubscriberBounces, SubscriptionEnabled, SubscriberRelatedID, SubscriberNewsletterID">
                <GridActions>
                    <ug:Action Name="subscribe" ExternalSourceName="subscribe" Caption="$newsletter.renewsubscription$" FontIconClass="icon-message" FontIconStyle="Allow"
                        Confirmation="$Unigrid.Subscribers.Actions.Subscribe.Confirmation$" />
                    <ug:Action Name="unsubscribe" ExternalSourceName="unsubscribe" Caption="$newsletter.unsubscribelink$" FontIconClass="icon-message" FontIconStyle="Critical"
                        Confirmation="$Unigrid.Subscribers.Actions.unsubscribe.Confirmation$" />
                    <ug:Action Name="remove" Caption="$newsletter.deletesubscription$" FontIconClass="icon-bin" FontIconStyle="Critical"
                        Confirmation="$Unigrid.Subscribers.Actions.RemoveSubscription.Confirmation$" />
                    <ug:Action Name="approve" ExternalSourceName="approve" Caption="$newsletter.approvesubscription$"
                        FontIconClass="icon-check-circle" FontIconStyle="Allow" Confirmation="$subscribers.approvesubscription$" />
                    <ug:Action Name="block" ExternalSourceName="block" Caption="$subscribers.block$"
                        FontIconClass="icon-times-circle" FontIconStyle="Critical" Confirmation="$subscribers.blocksubscriber$" />
                    <ug:Action Name="unblock" ExternalSourceName="unblock" Caption="$subscribers.unblock$"
                        FontIconClass="icon-check-circle" FontIconStyle="Allow" Confirmation="$subscribers.unblocksubscriber$" />
                </GridActions>
                <GridColumns>
                    <ug:Column Source="SubscriberFullName" Caption="$Unigrid.Subscribers.Columns.SubscriberName$"
                        Wrap="false">
                        <Filter Type="text" />
                    </ug:Column>
                    <ug:Column Source="##ALL##" ExternalSourceName="email" Caption="$general.emailaddress$"
                        Wrap="false">
                        <Filter Path="~/CMSModules/Newsletters/Controls/SubscriberFilter.ascx" />
                    </ug:Column>
                    <ug:Column Source="##ALL##" ExternalSourceName="status" Caption="$general.status$"
                        CssClass="TableCell" Wrap="false">
                        <Filter Source="SubscriptionEnabled" Path="~/CMSModules/Newsletters/Controls/SubscriptionStatusFilter.ascx" />
                    </ug:Column>
                    <ug:Column Source="##ALL##" ExternalSourceName="blocked" Caption="$Unigrid.Subscribers.Columns.Blocked$"
                        CssClass="TableCell" Wrap="false" Name="blocked">
                        <Filter Source="SubscriberBounces" Path="~/CMSModules/Newsletters/Controls/SubscriberBlockedFilter.ascx" />
                    </ug:Column>
                    <ug:Column Source="##ALL##" ExternalSourceName="bounces" Caption="$Unigrid.Subscribers.Columns.Bounces$"
                        Sort="SubscriberBounces" CssClass="TableCell" Wrap="false" Name="bounces">
                    </ug:Column>
                    <ug:Column CssClass="filling-column" />
                </GridColumns>
                <GridOptions DisplayFilter="true" ShowSelection="true" />
            </cms:UniGrid>
            <asp:Panel ID="pnlActions" runat="server" CssClass="form-horizontal mass-action">
                <div class="form-group">
                    <div class="mass-action-label-cell">
                        <cms:LocalizedLabel ID="lblActions" AssociatedControlID="drpActions" ResourceString="general.selecteditems"
                            CssClass="control-label" DisplayColon="true" runat="server" EnableViewState="false" />
                    </div>
                    <div class="mass-action-value-cell">
                        <cms:CMSDropDownList ID="drpActions" runat="server" />
                        <cms:LocalizedButton ID="btnOk" runat="server" ButtonStyle="Primary" OnClick="btnOk_Clicked"
                            ResourceString="general.ok" EnableViewState="false" />
                    </div>
                </div>
            </asp:Panel>
            <div class="checkbox-list-vertical">
                <cms:CMSCheckBox ID="chkSendConfirmation" runat="server" ResourceString="Newsletter_Subscribers.SendConfirmation" />
                <cms:CMSCheckBox ID="chkRequireOptIn" runat="server" ResourceString="newsletter.requireoptin" AutoPostBack="true" />
            </div>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>
