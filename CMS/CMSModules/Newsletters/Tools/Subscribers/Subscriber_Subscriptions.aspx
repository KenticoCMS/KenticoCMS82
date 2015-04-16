<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Newsletters_Tools_Subscribers_Subscriber_Subscriptions"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Newsletter subscribtions"
    CodeFile="Subscriber_Subscriptions.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Newsletters/FormControls/NewsletterSelector.ascx"
    TagName="NewsletterSelector" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>

<asp:Content ID="contentControls" ContentPlaceHolderID="plcBeforeContent" runat="server">
    <div class="cms-edit-menu">
        <cms:NewsletterSelector runat="server" ID="selectNewsletter" EnableViewState="False" />
    </div>
</asp:Content>
<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <cms:CMSUpdatePanel runat="server" ID="pnlUpdate" UpdateMode="Conditional">
        <ContentTemplate>
            <cms:UniGrid ID="unigridNewsletters" runat="server" ShortID="g" OrderBy="NewsletterDisplayName"
                IsLiveSite="false" Columns="NewsletterID, NewsletterDisplayName, SubscriptionApproved, SubscriptionEnabled"
                Query="newsletter.subscribernewsletter.selectsubscriptions">
                <GridActions>
                    <ug:Action Name="subscribe" ExternalSourceName="subscribe" Caption="$newsletter.renewsubscription$"
                        FontIconClass="icon-message" FontIconStyle="Allow" Confirmation="$Unigrid.Subscribers.Actions.Subscribe.Confirmation$" />
                    <ug:Action Name="unsubscribe" ExternalSourceName="unsubscribe" Caption="$newsletter.unsubscribelink$"
                        FontIconClass="icon-message" FontIconStyle="Critical" Confirmation="$Unigrid.Subscribers.Actions.unsubscribe.Confirmation$" />
                    <ug:Action Name="remove" Caption="$newsletter.deletesubscription$" FontIconClass="icon-bin" FontIconStyle="Critical"
                        Confirmation="$Unigrid.Subscribers.Actions.RemoveSubscription.Confirmation$" />
                    <ug:Action Name="approve" ExternalSourceName="approve" Caption="$newsletter.approvesubscription$"
                        FontIconClass="icon-check-circle" FontIconStyle="Allow" Confirmation="$subscribers.approvesubscription$" />
                </GridActions>
                <GridColumns>
                    <ug:Column Source="NewsletterDisplayName" Localize="true" Caption="$header.newsletter$"
                        Wrap="false">
                        <Filter Type="text" />
                    </ug:Column>
                    <ug:Column Source="##ALL##" ExternalSourceName="status" Caption="$general.status$"
                        CssClass="TableCell" Wrap="false">
                        <Filter Source="SubscriptionEnabled" Path="~/CMSModules/Newsletters/Controls/SubscriptionStatusFilter.ascx" />
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
                        <cms:CMSDropDownList ID="drpActions" runat="server" CssClass="DropDownFieldSmall" />
                        <cms:LocalizedButton ID="btnOk" runat="server" ButtonStyle="Primary" OnClick="btnOk_Clicked"
                            EnableViewState="false" ResourceString="general.ok" />
                    </div>
                </div>
            </asp:Panel>
            <div class="checkbox-list-vertical">
                <cms:CMSCheckBox ID="chkSendConfirmation" runat="server" ResourceString="newsletter_subscribers.sendconfirmation" />
                <asp:PlaceHolder runat="server" ID="plcRequireOptIn">
                    <cms:CMSCheckBox ID="chkRequireOptIn" runat="server" ResourceString="newsletter.requireoptin" />
                </asp:PlaceHolder>
            </div>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>
