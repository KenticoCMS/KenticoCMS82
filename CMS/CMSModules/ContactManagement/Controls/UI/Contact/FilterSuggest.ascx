<%@ Control Language="C#" AutoEventWireup="true" CodeFile="FilterSuggest.ascx.cs"
    Inherits="CMSModules_ContactManagement_Controls_UI_Contact_FilterSuggest" %>
<%@ Register Src="~/CMSFormControls/Sites/SiteSelector.ascx" TagName="SiteSelector"
    TagPrefix="cms" %>
<asp:Panel ID="pnlFilter" runat="server" DefaultButton="btnFilter" CssClass="FilterPanel">
    <div class="form-horizontal form-filter">
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblSuggest" runat="server" EnableViewState="false" ResourceString="om.contact.suggest"
                    DisplayColon="true" />
            </div>
            <div class="filter-form-value-cell">
                <cms:CMSCheckBox ID="chkEmail" runat="server" ResourceString="general.emaildomain" ToolTipResourceString="om.contact.suggestcontactsbyemail.tooltip" />
                <cms:CMSCheckBox ID="chkPhone" runat="server" ResourceString="general.phone" ToolTipResourceString="om.contact.suggestcontactsbyphone.tooltip" />
                <cms:CMSCheckBox ID="chkAddress" runat="server" ResourceString="om.contact.postaddress" ToolTipResourceString="om.contact.suggestcontactsbyaddress.tooltip" />
                <cms:CMSCheckBox ID="chkBirthDay" runat="server" ResourceString="om.contact.birthday" ToolTipResourceString="om.contact.suggestcontactsbybirthday.tooltip" />
                <cms:CMSCheckBox ID="chkMembership" runat="server" ResourceString="om.membership" ToolTipResourceString="om.contact.suggestcontactsbymembership.tooltip" />
                <cms:CMSCheckBox ID="chkIPaddress" runat="server" ResourceString="om.contact.ipaddress" ToolTipResourceString="om.contact.suggestcontactsbyip.tooltip" />
            </div>
        </div>
        <asp:PlaceHolder ID="plcSite" runat="server" Visible="false">
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblSite" runat="server" EnableViewState="false" ResourceString="general.site"
                        DisplayColon="true" />
                </div>
                <div class="filter-form-value-cell">
                    <cms:SiteSelector ID="siteSelector" runat="server" IsLiveSite="false" AllowGlobal="true" />
                    <cms:SiteOrGlobalSelector ID="siteOrGlobalSelector" runat="server" IsLiveSite="false" AutoPostBack="false" />
                </div>
            </div>
        </asp:PlaceHolder>
        <div class="form-group form-group-buttons">
            <div class="filter-form-buttons-cell">
                <cms:LocalizedButton ID="btnFilter" runat="server" ButtonStyle="Primary" ResourceString="general.filter" />
            </div>
        </div>
    </div>
</asp:Panel>
