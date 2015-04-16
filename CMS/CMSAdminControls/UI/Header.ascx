<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Header.ascx.cs" Inherits="CMSAdminControls_UI_Header" %>
<%@ Register Src="~/CMSFormControls/Sites/SiteSelector.ascx" TagName="SiteSelector" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Membership/Controls/PasswordExpiration.ascx" TagPrefix="cms" TagName="PasswordExpiration" %>
<%@ Register Src="~/CMSAdminControls/UI/UserMenu.ascx" TagPrefix="cms" TagName="UserMenu" %>
<%@ Register Src="~/CMSAdminControls/UI/ContextHelp.ascx" TagPrefix="cms" TagName="ContextHelp" %>
<div class="navbar navbar-inverse cms-navbar">
    <asp:PlaceHolder runat="server" ID="plcToggle">
        <a href="javascript:void(0)" class="applist-toggle navbar-left" id="cms-applist-toggle" title="<%= GetString("applicationlist.open") %>">
            <i aria-hidden="true" class="icon-kentico cms-nav-icon-large"></i>
            <span class="sr-only"><%= GetString("applicationlist.open") %></span>
        </a>
    </asp:PlaceHolder>
    <h2 class="sr-only"><%= GetString("breadcrumbs.title")  %></h2>
    <ul class="navbar-left breadcrumb cms-nav-breadcrumb" id="js-nav-breadcrumb">
        <li class="no-ico">
            <cms:LocalizedHyperlink ID="lnkDashboard" runat="server" EnableViewState="false" ToolTipResourceString="cms.dashboard.back">
                <i aria-hidden="true" class="icon-home cms-nav-icon-medium"></i> 
                <span class="sr-only"><%= GetString("cms.dashboard.back") %></span>
            </cms:LocalizedHyperlink>
        </li>
        <asp:PlaceHolder runat="server" ID="plcSiteSelector">
            <li class="dropdown no-ico header-site-selector">
                <cms:SiteSelector ID="siteSelector" ShortID="ss" runat="server" IsLiveSite="false" />
            </li>
        </asp:PlaceHolder>
    </ul>
    <ul class="nav navbar-nav navbar-right navbar-inverse">
        <asp:PlaceHolder runat="server" ID="plcSupportChat" Visible="false"></asp:PlaceHolder>
        <li>
            <cms:ContextHelp runat="server" ID="contextHelp" />
        </li>
        <li>
            <cms:UserMenu runat="server" ID="userMenu" />
        </li>
    </ul>
</div>
<div id="cms-header-contexthelp"></div>
<div id="cms-header-placeholder"></div>
<div id="cms-header-messages">
    <asp:Panel runat="server" ID="pnlTechPreview" CssClass="message-panel alert-warning">
        Please note: This is a technical preview version. Changes are directly saved to
    the development database.
        <a href="#" class="alert-link" onclick="HideWarning('<%= pnlTechPreview.ClientID %>', '<% = SESSION_KEY_TECH_PREVIEW %>'); return false;"><%= GetString("general.close") %></a>
    </asp:Panel>
    <asp:Panel runat="server" ID="pnlTrial" CssClass="message-panel alert-warning">
        <asp:Literal ID="ltlText" runat="server" EnableViewState="false" />
        <a href="#" class="alert-link" onclick="HideWarning('<%= pnlTrial.ClientID %>', '<% = SESSION_KEY_TRIAL %>'); return false;"><%= GetString("general.close") %></a>
    </asp:Panel>
    <asp:Panel runat="server" ID="pnlPwdExp" CssClass="message-panel alert-warning">
        <cms:PasswordExpiration ID="pwdExpiration" runat="server" EnableViewState="false"
            IsLiveSite="false" />
    </asp:Panel>
</div>
