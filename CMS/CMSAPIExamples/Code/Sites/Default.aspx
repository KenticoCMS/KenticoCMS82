<%@ Page Title="" Language="C#" MasterPageFile="~/CMSAPIExamples/Pages/APIExamplesPage.Master"
    Theme="Default" AutoEventWireup="true" Inherits="CMSAPIExamples_Code_Sites_Default" CodeFile="Default.aspx.cs" %>

<%@ Register Src="~/CMSAPIExamples/Controls/APIExample.ascx" TagName="APIExample" TagPrefix="cms" %>
<asp:Content ID="contentLeft" ContentPlaceHolderID="plcLeftContainer" runat="server">
    <%-- Site --%>
    <cms:LocalizedHeading ID="headCreateSite" runat="server" Text="Site" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreateSite" runat="server" ButtonText="Create site" InfoMessage="Site 'My new site' was created." />
    <cms:APIExample ID="apiGetAndUpdateSite" runat="server" ButtonText="Get and update site" APIExampleType="ManageAdditional" InfoMessage="Site 'My new site' was updated." ErrorMessage="Site 'My new site' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateSites" runat="server" ButtonText="Get and bulk update sites" APIExampleType="ManageAdditional" InfoMessage="All sites matching the condition were updated." ErrorMessage="Sites matching the condition were not found." />
    <%-- Culture site --%>
    <cms:LocalizedHeading ID="headCreateCultureSite" runat="server" Text="Culture on site" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiAddCultureToSite" runat="server" ButtonText="Add culture to site" APIExampleType="ManageAdditional" InfoMessage="Culture to site 'My new site' was added." ErrorMessage="Site 'My new site' was not found." />
    <%-- Site domain alias --%>
    <cms:LocalizedHeading ID="headDomainAliasOnSite" runat="server" Text="Site domain alias" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiAddDomainAliasToSite" runat="server" ButtonText="Add domain alias to site" APIExampleType="ManageAdditional" InfoMessage="Alias 'My new alias' was created." ErrorMessage="Site 'My new site' was not found." />
    <%-- Site actions --%>
    <cms:LocalizedHeading ID="headSiteActions" runat="server" Text="Site actions" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiRunSite" runat="server" ButtonText="Run site" APIExampleType="ManageAdditional" InfoMessage="Site 'My new site' was started." ErrorMessage="Site 'My new site' was not found." />
    <cms:APIExample ID="apiStopSite" runat="server" ButtonText="Stop site" APIExampleType="ManageAdditional" InfoMessage="Site 'My new site' was stopped." ErrorMessage="Site 'My new site' was not found." />
</asp:Content>
<asp:Content ID="contentRight" ContentPlaceHolderID="plcRightContainer" runat="server">
    <%-- Site domain alias --%>
    <cms:LocalizedHeading ID="headDeleteSiteDomainAlias" runat="server" Text="Site domain alias" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteSiteDomainAlias" runat="server" ButtonText="Remove site alias from site" APIExampleType="CleanUpMain" InfoMessage="Alias 'My new alias' and all its dependencies were deleted." ErrorMessage="Alias 'My new alias' was not found." />
    <%-- Culture site --%>
    <cms:LocalizedHeading ID="headDeleteCultureSite" runat="server" Text="Culture on site" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiRemoveCultureFromSite" runat="server" ButtonText="Remove culture from site" APIExampleType="CleanUpMain" InfoMessage="Culture from site 'My new site' was removed." ErrorMessage="Site 'My new site' was not found." />
    <%-- Site --%>
    <cms:LocalizedHeading ID="headDeleteSite" runat="server" Text="Site" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteSite" runat="server" ButtonText="Delete site" APIExampleType="CleanUpMain" InfoMessage="Site 'My new site' and all its dependencies were deleted." ErrorMessage="Site 'My new site' was not found." />
</asp:Content>
