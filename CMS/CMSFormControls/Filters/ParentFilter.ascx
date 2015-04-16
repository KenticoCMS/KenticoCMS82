<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ParentFilter.ascx.cs"
    Inherits="CMSFormControls_Filters_ParentFilter" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<asp:Panel ID="pnlParentSelector" runat="server" CssClass="SiteSelector">
    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <cms:UniSelector ID="uniSelector" ShortID="ss" runat="server" ObjectType="cms.site"
                ResourcePrefix="siteselect" />
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Panel>
