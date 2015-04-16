<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Newsletters_Tools_Subscribers_Subscriber_List"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Tools - Newsletter subscribers"
    CodeFile="Subscriber_List.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <cms:CMSUpdatePanel runat="server" ID="pnlUpdate" UpdateMode="Conditional">
        <ContentTemplate>
            <cms:UniGrid ID="UniGrid" runat="server" ShortID="g" OrderBy="Email" IsLiveSite="false"
                Columns="SubscriberID, SubscriberFullName, SubscriberEmail, Email, SubscriberType, SubscriberBounces, SubscriberRelatedID"
                ObjectType="newsletter.subscriberlist" RememberStateByParam="">
                <GridActions>
                    <ug:Action Name="edit" Caption="$General.Edit$" FontIconClass="icon-edit" FontIconStyle="Allow" />
                    <ug:Action Name="delete" Caption="$General.Delete$" FontIconClass="icon-bin" FontIconStyle="Critical" Confirmation="$General.ConfirmDelete$" />
                    <ug:Action Name="block" ExternalSourceName="block" Caption="$general.block$" FontIconClass="icon-times-circle" FontIconStyle="Critical"
                        Confirmation="$subscribers.blocksubscriber$" />
                    <ug:Action Name="unblock" ExternalSourceName="unblock" Caption="$general.unblock$"
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
                    <ug:Column Source="##ALL##" ExternalSourceName="blocked" Caption="$Unigrid.Subscribers.Columns.Blocked$"
                        CssClass="TableCell" Wrap="false" Name="blocked">
                        <Filter Source="SubscriberBounces" Path="~/CMSModules/Newsletters/Controls/SubscriberBlockedFilter.ascx" />
                    </ug:Column>
                    <ug:Column Source="##ALL##" ExternalSourceName="bounces" Caption="$Unigrid.Subscribers.Columns.Bounces$"
                        Sort="SubscriberBounces" CssClass="TableCell" Wrap="false" Name="bounces" />
                    <ug:Column CssClass="filling-column" />
                </GridColumns>
                <GridOptions DisplayFilter="true" />
            </cms:UniGrid>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>
