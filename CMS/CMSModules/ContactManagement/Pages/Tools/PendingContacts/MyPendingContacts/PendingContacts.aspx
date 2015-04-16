<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PendingContacts.aspx.cs" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Inherits="CMSModules_ContactManagement_Pages_Tools_PendingContacts_MyPendingContacts_PendingContacts" Theme="Default" %>

<%@ Register Src="~/CMSModules/ContactManagement/Controls/UI/Automation/PendingContacts.ascx" TagName="PendingContacts" TagPrefix="cms" %>
<asp:Content ID="cntControls" runat="server" ContentPlaceHolderID="plcSiteSelector">
    <div class="form-horizontal form-filter">
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel runat="server" ID="lblSite" EnableViewState="false" DisplayColon="true" ResourceString="General.Site" CssClass="control-label" />
            </div>
            <div class="filter-form-value-cell">
                <cms:SiteOrGlobalSelector ID="siteOrGlobalSelector" ShortID="sg" runat="server" ShowSiteAndGlobal="false" PostbackOnDropDownChange="True" />
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:PendingContacts ID="listContacts" runat="server" ShowOnlyMyPendingContacts="True" />
</asp:Content>
