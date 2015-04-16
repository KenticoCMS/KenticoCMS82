<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Subscriber_Contacts.aspx.cs"
    Inherits="CMSModules_ContactManagement_Pages_Tools_Contact_Subscriber_Contacts"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Newsletter subscriber - contact group contacts" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <cms:cmsupdatepanel runat="server" id="pnlUpdate" updatemode="Conditional">   
        <ContentTemplate>            
            <cms:UniGrid ID="UniGrid" runat="server" OrderBy="ContactLastName" IsLiveSite="false"
                Columns="ContactID, ContactLastName, ContactFirstName, ContactEmail, ContactBounces"
                ObjectType="om.contact">
                <GridActions>
                    <ug:Action Name="block" ExternalSourceName="block" Caption="$general.block$" FontIconClass="icon-times-circle" FontIconStyle="Critical" Confirmation="$subscribers.blocksubscriber$" />
                    <ug:Action Name="unblock" ExternalSourceName="unblock" Caption="$general.unblock$" FontIconClass="icon-check-circle" FontIconStyle="Allow" Confirmation="$subscribers.unblocksubscriber$" />
                </GridActions>
                <GridColumns>
                    <ug:Column Source="ContactLastName" Caption="$om.contact.lastname$" Wrap="false">
                        <Filter Type="text" />
                    </ug:Column>
                    <ug:Column Source="ContactFirstName" Caption="$om.contact.firstname$" Wrap="false">
                        <Filter Type="text" />
                    </ug:Column>
                    <ug:Column Source="ContactEmail" Caption="$general.emailaddress$" Wrap="false">
                        <Filter Type="text" />
                    </ug:Column>
                    <ug:Column Source="ContactBounces" ExternalSourceName="blocked" Caption="$Unigrid.Subscribers.Columns.Blocked$"
                        AllowSorting="false" CssClass="TableCell" Wrap="false" Name="blocked">
                        <Filter Path="~/CMSModules/Newsletters/Controls/UserBlockedFilter.ascx" />
                    </ug:Column>
                    <ug:Column Source="ContactBounces" ExternalSourceName="bounces" Caption="$Unigrid.Subscribers.Columns.Bounces$"
                        CssClass="TableCell" Wrap="false" Name="bounces" />
                    <ug:Column CssClass="filling-column" />                    
                </GridColumns>
                <GridOptions DisplayFilter="true" />
            </cms:UniGrid>
        </ContentTemplate>
    </cms:cmsupdatepanel>
</asp:Content>
