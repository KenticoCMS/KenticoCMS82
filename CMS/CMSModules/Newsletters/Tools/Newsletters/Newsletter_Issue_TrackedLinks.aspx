<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Newsletters_Tools_Newsletters_Newsletter_Issue_TrackedLinks"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Title="Tools - Link tracking" CodeFile="Newsletter_Issue_TrackedLinks.aspx.cs" %>

<%@ Register Src="~/CMSModules/Newsletters/Controls/TrackedLinksFilter.ascx" TagPrefix="cms"
    TagName="LinkFilter" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagPrefix="cms" TagName="UniGrid" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<asp:Content ID="cntContent" ContentPlaceHolderID="plcContent" runat="server">
    <cms:LinkFilter runat="server" ID="fltLinks" ShortID="f" />
    <cms:UniGrid runat="server" ID="UniGrid" ShortID="g" OrderBy="UniqueClicks DESC" IsLiveSite="false"
        ObjectType="newsletter.linklist" ShowObjectMenu="false">
        <GridActions Parameters="LinkID">
            <ug:Action Name="view" Caption="$Unigrid.Newsletter.Actions.ViewParticipated$" FontIconClass="icon-eye" FontIconStyle="Allow"
                OnClick="ViewClicks({0}); return false;" ExternalSourceName="view" />
            <ug:Action Name="deleteoutdated" Caption="$Unigrid.Newsletter.Actions.DeleteOutdated$"
                FontIconClass="icon-bin" FontIconStyle="Critical" Confirmation="$General.ConfirmDelete$" ExternalSourceName="deleteoutdated" />
        </GridActions>
        <GridColumns>
            <ug:Column Source="LinkTarget" ExternalSourceName="linktarget" Caption="$general.link$"
                Wrap="false" CssClass="main-column-100">
                <Tooltip Source="LinkTarget" ExternalSourceName="linktargettooltip" />
            </ug:Column>
            <ug:Column Source="LinkDescription" ExternalSourceName="linkdescription" Caption="$general.description$"
                Wrap="false">
                <Tooltip Source="LinkDescription" ExternalSourceName="linkdescriptiontooltip" />
            </ug:Column>
            <ug:Column Source="##ALL##" Caption="$unigrid.newsletter_issue_trackedlinks.columns.uniqueclicks$"
                Wrap="false" CssClass="TableCell" ExternalSourceName="uniqueclicks" />
            <ug:Column Source="##ALL##" Caption="$unigrid.newsletter_issue_trackedlinks.columns.totalclicks$"
                Wrap="false" CssClass="TableCell" ExternalSourceName="totalclicks" />
            <ug:Column Source="##ALL##" ExternalSourceName="clickrate" Caption="$unigrid.newsletter_issue_trackedlinks.columns.clickrate$"
                Wrap="false" CssClass="TableCell" />
        </GridColumns>
    </cms:UniGrid>
</asp:Content>
