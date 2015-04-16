<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Newsletters_Tools_Subscribers_Subscriber_Users"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Newsletter subscriber - role users"
    CodeFile="Subscriber_Users.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
    
<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <cms:CMSUpdatePanel runat="server" ID="pnlUpdate" UpdateMode="Conditional">   
        <ContentTemplate>            
            <cms:UniGrid ID="UniGrid" runat="server" OrderBy="FullName" IsLiveSite="false"
                Columns="UserID, FullName, Email, UserBounces"
                ObjectType="cms.usersettingsrolelist">
                <GridActions Parameters="UserID">
                    <ug:Action Name="block" ExternalSourceName="block" Caption="$general.block$" FontIconClass="icon-times-circle" FontIconStyle="Critical" Confirmation="$subscribers.blocksubscriber$" />
                    <ug:Action Name="unblock" ExternalSourceName="unblock" Caption="$general.unblock$" FontIconClass="icon-check-circle" FontIconStyle="Allow" Confirmation="$subscribers.unblocksubscriber$" />
                </GridActions>
                <GridColumns>
                    <ug:Column Source="FullName" Caption="$Unigrid.Subscribers.Columns.SubscriberName$" Wrap="false">
                        <Filter Type="text" />
                    </ug:Column>
                    <ug:Column Source="Email" Caption="$general.email$" Wrap="false">
                        <Filter Type="text" />
                    </ug:Column>
                    <ug:Column Source="UserBounces" ExternalSourceName="blocked" Caption="$Unigrid.Subscribers.Columns.Blocked$" AllowSorting="false" CssClass="TableCell" Wrap="false">
                        <Filter Path="~/CMSModules/Newsletters/Controls/UserBlockedFilter.ascx" />
                    </ug:Column>
                    <ug:Column Source="UserBounces" ExternalSourceName="bounces" Caption="$Unigrid.Subscribers.Columns.Bounces$" CssClass="TableCell" Wrap="false" />
                    <ug:Column CssClass="filling-column" />                    
                </GridColumns>
                <GridOptions DisplayFilter="true" />
            </cms:UniGrid>
        </ContentTemplate>
    </cms:CMSUpdatePanel>    
</asp:Content>