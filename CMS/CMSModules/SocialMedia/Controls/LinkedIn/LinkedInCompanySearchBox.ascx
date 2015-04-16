<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_SocialMedia_Controls_LinkedIn_LinkedInCompanySearchBox"
    CodeFile="LinkedInCompanySearchBox.ascx.cs" %>

<asp:Panel ID="pnlFilter" runat="server" EnableViewState="false" DefaultButton="btnSearch" CssClass="form-horizontal form-filter">
    <div class="form-group">
        <div class="filter-form-value-cell-wide-200 form-search-container">
            <asp:Label AssociatedControlID="txtSearch" runat="server" CssClass="sr-only">
               <%= GetString("general.search") %>
            </asp:Label>
            <cms:CMSTextBox ID="txtSearch" runat="server" EnableViewState="false" />
            <cms:CMSIcon ID="iconSearch" runat="server" CssClass="icon-magnifier" EnableViewState="false" />
        </div>
    </div>
    <div class="form-group">
        <div class="filter-form-buttons-cell-wide">
            <cms:LocalizedButton ID="btnSearch" runat="server" ResourceString="general.search"
                EnableViewState="false" ButtonStyle="Primary" OnClick="btnSearch_Click" />
        </div>
    </div>
</asp:Panel>