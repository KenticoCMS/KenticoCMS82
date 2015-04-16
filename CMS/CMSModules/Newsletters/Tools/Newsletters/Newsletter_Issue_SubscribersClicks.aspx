<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Newsletters_Tools_Newsletters_Newsletter_Issue_SubscribersClicks"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Title="Tools - Participated subscribers" CodeFile="Newsletter_Issue_SubscribersClicks.aspx.cs" %>

<%@ Register Src="~/CMSModules/Newsletters/Controls/OpenedByFilter.ascx" TagPrefix="cms"
    TagName="OpenedByFilter" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagPrefix="cms" TagName="UniGrid" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<asp:Content ID="cntContent" ContentPlaceHolderID="plcContent" runat="server">
    <cms:OpenedByFilter runat="server" ID="fltOpenedBy" ShortID="f" />
    <cms:UniGrid runat="server" ID="UniGrid" ShortID="g" OrderBy="Clicks DESC" IsLiveSite="false"
        ObjectType="newsletter.subscriberlinklist" ShowActionsMenu="true" Columns="SubscriberID,SubscriberFullName,SubscriberEmail,Clicks">
        <GridColumns>
            <ug:Column Source="##ALL##" Caption="$unigrid.subscribers.columns.subscribername$"
                ExternalSourceName="name" Wrap="false" CssClass="main-column-100" />
            <ug:Column Source="##ALL##" Caption="$general.emailaddress$" ExternalSourceName="email"
                Wrap="false" />
            <ug:Column Source="Clicks" Caption="$unigrid.newsletter_issue_subscribersclicks.columns.clicks$"
                CssClass="TableCell" Wrap="false" />
        </GridColumns>
    </cms:UniGrid>
</asp:Content>
