<%@ Control Language="C#" AutoEventWireup="true" CodeFile="FilterSuggest.ascx.cs"
    Inherits="CMSModules_ContactManagement_Controls_UI_Account_FilterSuggest" %>
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
                <asp:PlaceHolder ID="plcContact" runat="server">
                    <cms:CMSCheckBox ID="chkContacts" runat="server" ResourceString="om.contacts" ToolTipResourceString="om.account.suggestaccountsbycontacts.tooltip" />
                </asp:PlaceHolder>
                <cms:CMSCheckBox ID="chkAddress" runat="server" ResourceString="general.address" ToolTipResourceString="om.account.suggestaccountsbyaddress.tooltip" />
                <cms:CMSCheckBox ID="chkEmail" runat="server" ResourceString="general.emaildomain" ToolTipResourceString="om.account.suggestaccountsbyemail.tooltip" />
                <cms:CMSCheckBox ID="chkURL" runat="server" ResourceString="om.account.url" ToolTipResourceString="om.account.suggestaccountsbyurl.tooltip" />
                <cms:CMSCheckBox ID="chkPhone" runat="server" ResourceString="om.account.phones" ToolTipResourceString="om.account.suggestaccountsbyphone.tooltip" />
                <asp:PlaceHolder ID="plcSite" runat="server" Visible="false">
             </div>
        </div>
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblSite" runat="server" EnableViewState="false" ResourceString="general.site"
                    DisplayColon="true" />
            </div>
                    <div class="filter-form-value-cell">
                        <cms:SiteSelector ID="siteSelector" runat="server" IsLiveSite="false" AllowGlobal="true" />
                        <cms:SiteOrGlobalSelector ID="siteOrGlobalSelector" runat="server" IsLiveSite="false"
                            AutoPostBack="false" />
                </asp:PlaceHolder>
            </div>
        </div>
        <div class="form-group form-group-buttons">
            <div class="filter-form-buttons-cell">
                <cms:LocalizedButton ID="btnFilter" runat="server" ButtonStyle="Primary" ResourceString="general.filter" />
            </div>
        </div>
    </div>
</asp:Panel>
