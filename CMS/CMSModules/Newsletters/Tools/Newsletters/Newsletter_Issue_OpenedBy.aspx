<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Newsletters_Tools_Newsletters_Newsletter_Issue_OpenedBy"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Title="Tools - Newsletter issue opened emails" CodeFile="Newsletter_Issue_OpenedBy.aspx.cs" %>

<%@ Register Src="~/CMSModules/Newsletters/Controls/OpenedByFilter.ascx" TagPrefix="cms"
    TagName="OpenedByFilter" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagPrefix="cms" TagName="UniGrid" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<asp:Content ID="cntContent" ContentPlaceHolderID="plcContent" runat="server">
    <cms:OpenedByFilter runat="server" ID="fltOpenedBy" ShortID="f" ShowDateFilter="true" />
    <cms:UniGrid runat="server" ID="UniGrid" ShortID="g" OrderBy="OpenedWhen DESC" IsLiveSite="false"
        ObjectType="newsletter.openedemaillist" ShowActionsMenu="true" Columns="IssueID, SubscriberID,SubscriberFullName,SubscriberEmail,OpenedWhen">
        <GridColumns>
            <ug:Column Source="##ALL##" ExternalSourceName="name" Caption="$unigrid.subscribers.columns.subscribername$"
                Wrap="false" CssClass="main-column-100" />
            <ug:Column Source="##ALL##" ExternalSourceName="email" Caption="$general.emailaddress$"
                Wrap="false" />
            <ug:Column Source="OpenedWhen" Caption="$unigrid.newsletter_issue_openedby.columns.openedwhen$"
                Wrap="false" />
            <ug:Column Source="IssueID" Caption="$newsletterissue_send.variantname$" Wrap="false" Name="variants"
                ExternalSourceName="variantname" AllowSorting="false" />
        </GridColumns>
    </cms:UniGrid>
</asp:Content>
