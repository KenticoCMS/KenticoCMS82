<%@ Page Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Newsletters_Tools_Newsletters_Newsletter_List" Theme="Default"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Tools - Newsletters" CodeFile="Newsletter_List.aspx.cs" %>
    
<%@ Register src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" tagname="UniGrid" tagprefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>

<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <cms:UniGrid runat="server" ID="UniGrid" ShortID="g" OrderBy="NewsletterDisplayName" IsLiveSite="false"
        ObjectType="newsletter.newsletter" RememberStateByParam=""
        Columns="NewsletterID, NewsletterDisplayName, (SELECT COUNT(NewsletterID) FROM Newsletter_SubscriberNewsletter WHERE NewsletterID = Newsletter_Newsletter.NewsletterID AND (SubscriptionApproved = 1 OR SubscriptionApproved IS NULL) AND (SubscriptionEnabled=1 OR SubscriptionEnabled IS NULL)) AS Subscribers, (SELECT MAX(IssueMailoutTime) FROM Newsletter_NewsletterIssue WHERE IssueNewsletterID = Newsletter_Newsletter.NewsletterID ) AS LastIssue">
        <GridActions>
            <ug:Action Name="edit" Caption="$General.Edit$" FontIconClass="icon-edit" FontIconStyle="Allow" />
            <ug:Action Name="delete" Caption="$General.Delete$" FontIconClass="icon-bin" FontIconStyle="Critical" Confirmation="$General.ConfirmDelete$" />
        </GridActions>
        <GridColumns>
            <ug:Column Source="NewsletterDisplayName" Caption="$Unigrid.Newsletter.Columns.NewsletterDisplayName$" Wrap="false" Localize="true">
                <Filter Type="text" />
            </ug:Column>
            <ug:Column Source="Subscribers" Caption="$Unigrid.Newsletter.Columns.Subscribers$" Wrap="false" AllowSorting="false" CssClass="TableCell" />
            <ug:Column Source="LastIssue" Caption="$Unigrid.Newsletter.Columns.LastIssue$" Wrap="false" AllowSorting="false" />
            <ug:Column CssClass="filling-column" />            
        </GridColumns>   
        <GridOptions DisplayFilter="true" />
    </cms:UniGrid>                             
</asp:Content>