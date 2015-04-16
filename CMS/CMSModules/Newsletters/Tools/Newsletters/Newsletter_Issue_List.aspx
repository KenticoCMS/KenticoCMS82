<%@ Page Language="C#" AutoEventWireup="true" Title="Tools - Newsletter issues"
    Inherits="CMSModules_Newsletters_Tools_Newsletters_Newsletter_Issue_List" Theme="Default"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" CodeFile="Newsletter_Issue_List.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<%@ Register Src="~/CMSAdminControls/Basic/DisabledModuleInfo.ascx" TagPrefix="cms" TagName="DisabledModule" %>

<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <cms:UniGrid runat="server" ID="UniGrid" ShortID="g" IsLiveSite="false" OrderBy="CASE WHEN IssueMailoutTime IS NULL THEN 0 ELSE 1 END, IssueMailoutTime DESC"
        ObjectType="newsletter.issue" Columns="IssueID, IssueSubject, IssueMailoutTime, IssueSentEmails, IssueOpenedEmails, IssueUnsubscribed, IssueBounces, IssueIsABTest, IssueStatus, IssueVariantOfIssueID"
        RememberStateByParam="">
        <GridActions Parameters="IssueID">
            <ug:Action Name="edit" Caption="$General.Edit$" FontIconClass="icon-edit" FontIconStyle="Allow" />
            <ug:Action Name="delete" Caption="$General.Delete$" FontIconClass="icon-bin" FontIconStyle="Critical" Confirmation="$General.ConfirmDelete$" />
            <ug:Action Name="viewclickedlinks" Caption="$Unigrid.Newsletter.Actions.ViewClickedLinks$"
                FontIconClass="icon-eye" FontIconStyle="Allow" OnClick="ViewClickedLinks({0}); return false;" ExternalSourceName="viewclickedlinks" />
        </GridActions>
        <GridColumns>
            <ug:Column Source="IssueSubject" ExternalSourceName="IssueSubject" Caption="$unigrid.newsletter_issue.columns.issuesubject$"
                Wrap="false" CssClass="main-column-100">
                <Filter Type="text" />
            </ug:Column>
            <ug:Column Source="IssueMailoutTime" Caption="$unigrid.newsletter_issue.columns.issuemailouttime$"
                Wrap="false" />
            <ug:Column Source="IssueSentEmails" Caption="$unigrid.newsletter_issue.columns.issuesentemails$"
                Wrap="false" CssClass="TableCell" />
            <ug:Column Source="##ALL##" ExternalSourceName="IssueOpenedEmails" Caption="$unigrid.newsletter_issue.columns.issueopenedemails$"
                Wrap="false" Sort="IssueOpenedEmails" CssClass="TableCell" Name="openedemails" />
            <ug:Column Source="IssueUnsubscribed" Caption="$unigrid.newsletter_issue.columns.issueunsubscribed$"
                Wrap="false" CssClass="TableCell" />
            <ug:Column Source="IssueBounces" Caption="$unigrid.newsletter_issue.columns.issuebounces$"
                Wrap="false" CssClass="TableCell" Name="bounces" />
            <ug:Column Source="IssueIsABTest" Caption="$newsletters.isabtest$" ExternalSourceName="#yesno"
                Wrap="false" CssClass="TableCell" Name="isabtest" />
            <ug:Column Source="IssueStatus" Caption="$newsletters.issuestatus$" ExternalSourceName="IssueStatus"
                Wrap="false" CssClass="TableCell" />
        </GridColumns>
        <GridOptions DisplayFilter="true" />
    </cms:UniGrid>
</asp:Content>
