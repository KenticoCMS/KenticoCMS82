<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="Polls" Inherits="CMSModules_Polls_Tools_Polls_List" Theme="Default" CodeFile="Polls_List.aspx.cs" %>

<%@ Register Src="~/CMSModules/Polls/Controls/PollsList.ascx" TagName="PollsList"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/HeaderActions.ascx" TagName="HeaderActions"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Polls/Controls/Filters/SiteSelector.ascx" TagName="SiteFilter"
    TagPrefix="cms" %>
<asp:Content ID="cntControls" runat="server" ContentPlaceHolderID="plcSiteSelector">
    <cms:SiteFilter ID="fltSite" ShortID="c" runat="server" DisplayAllGlobals="true" />
</asp:Content>
<asp:Content ID="cntActions" runat="server" ContentPlaceHolderID="plcActions">
    <cms:CMSUpdatePanel ID="pnlActons" runat="server">
        <ContentTemplate>
            <div class="btn-actions">
                <cms:HeaderActions ID="hdrActions" runat="server" ShortID="ha" IsLiveSite="false" />
            </div>
            <div class="header-actions-label control-group-inline">
                <cms:LocalizedLabel ID="lblWarnNew" runat="server" ResourceString="pollslist.choosegloborsite"
                    EnableViewState="false" Visible="false" CssClass="PollsInfoLabel form-control-text" />
            </div>
            <div class="ClearBoth"></div>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
        <ContentTemplate>
            <cms:PollsList ID="PollsList" runat="server" Visible="true" DelayedReload="false"
                ShortID="l" />
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>
