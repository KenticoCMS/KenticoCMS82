<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Newsletters_Tools_EmailQueue_NewsletterEmailQueue"
    Theme="Default" MaintainScrollPositionOnPostback="true" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="Newsletter - E-mail queue" CodeFile="NewsletterEmailQueue.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <cms:LocalizedHeading runat="server" ID="headTitle" Level="4" ResourceString="NewsletterEmailQueue_List.Intro"
        CssClass="listing-title" EnableViewState="false" />
    <cms:UniGrid runat="server" ID="gridElem" ShortID="g" OrderBy="EmailID" IsLiveSite="false"
        ObjectType="newsletter.emailslist" Columns="EmailID, NewsletterDisplayName, IssueSubject, EmailAddress, EmailLastSendResult, EmailLastSendAttempt"
        HideControlForZeroRows="false" ShowObjectMenu="false">
        <GridActions>
            <ug:Action Name="resend" Caption="$Unigrid.NewsletterEmailQueue.Actions.Resend$"
                FontIconClass="icon-message" />
            <ug:Action Name="delete" Caption="$General.Delete$" FontIconClass="icon-bin" FontIconStyle="Critical" Confirmation="$General.ConfirmDelete$" />
        </GridActions>
        <GridColumns>
            <ug:Column Source="NewsletterDisplayName" Caption="$unigrid.newsletteremailqueue.columns.newsletter$"
                Wrap="false">
                <Filter Type="text" />
            </ug:Column>
            <ug:Column Source="IssueSubject" ExternalSourceName="subject" Caption="$general.subject$"
                Wrap="false">
                <Filter Type="text" />
                <Tooltip Source="IssueSubject" ExternalSourceName="subjecttooltip" />
            </ug:Column>
            <ug:Column Source="EmailAddress" Caption="$general.email$" Wrap="false">
                <Filter Type="text" />
            </ug:Column>
            <ug:Column Source="EmailLastSendResult" ExternalSourceName="result" Caption="$Unigrid.NewsletterEmailQueue.Columns.ErrorMessage$"
                CssClass="main-column-100" Wrap="false">
                <Filter Type="text" />
                <Tooltip Source="EmailLastSendResult" ExternalSourceName="resulttooltip" />
            </ug:Column>
            <ug:Column Source="EmailLastSendAttempt" Caption="$Unigrid.NewsletterEmailQueue.Columns.LastSendAttempt$"
                Wrap="false" />
        </GridColumns>
        <GridOptions DisplayFilter="true" />
    </cms:UniGrid>
</asp:Content>
