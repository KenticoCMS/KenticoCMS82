<%@ Page Language="C#" AutoEventWireup="True" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="Automation process – Contacts" Inherits="CMSModules_ContactManagement_Pages_Tools_Automation_Process_Tab_Contacts"
    Theme="Default" CodeFile="Tab_Contacts.aspx.cs" %>

<%@ Register Src="~/CMSModules/ContactManagement/Controls/UI/Automation/Contacts.ascx" TagName="Contacts" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Sites/SiteSelector.ascx" TagName="SiteSelector" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/ContactManagement/FormControls/ContactSelector.ascx"
    TagName="ContactSelector" TagPrefix="cms" %>

<asp:Content ID="cntControls" runat="server" ContentPlaceHolderID="plcSiteSelector">
    <div class="form-horizontal form-filter">
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel runat="server" ID="lblSite" EnableViewState="false" DisplayColon="true" ResourceString="General.Site" CssClass="control-label" />
            </div>
            <div class="filter-form-value-cell-wide">
                <cms:SiteOrGlobalSelector ID="siteOrGlobalSelector" ShortID="sg" runat="server" PostbackOnDropDownChange="true" />
                <cms:SiteSelector ID="siteSelector" runat="server" ShortID="s" AllowGlobal="true" AllowAll="true" IsLiveSite="false" PostbackOnDropDownChange="true" />
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ContentPlaceHolderID="plcBeforeActions" runat="server" ID="cntBeforeActions">
    <div class="control-group-inline">
        <cms:ContactSelector ID="ucSelector" runat="server" Enabled="false" IsLiveSite="false" />
        <cms:LocalizedLabel ID="lblWarnStart" runat="server" ResourceString="ma.chooseglobalorsitetostart" EnableViewState="false" Visible="false" CssClass="button-explanation-text" />
    </div>
</asp:Content>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <cms:Contacts ID="listContacts" runat="server" />
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>
