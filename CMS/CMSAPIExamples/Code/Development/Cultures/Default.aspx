<%@ Page Title="" Language="C#" MasterPageFile="~/CMSAPIExamples/Pages/APIExamplesPage.Master"
    Theme="Default" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="CMSAPIExamples_Code_Development_Cultures_Default" %>

<%@ Register Src="~/CMSAPIExamples/Controls/APIExample.ascx" TagName="APIExample" TagPrefix="cms" %>
<asp:Content ID="contentLeft" ContentPlaceHolderID="plcLeftContainer" runat="server">
    <%-- Culture --%>
    <cms:LocalizedHeading ID="headCreateCulture" runat="server" Text="Culture" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreateCulture" runat="server" ButtonText="Create culture" InfoMessage="Culture 'My new culture' was created." />
    <cms:APIExample ID="apiGetAndUpdateCulture" runat="server" ButtonText="Get and update culture" APIExampleType="ManageAdditional" InfoMessage="Culture 'My new culture' was updated."
        ErrorMessage="Culture 'My new culture' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateCultures" runat="server" ButtonText="Get and bulk update cultures" APIExampleType="ManageAdditional" InfoMessage="All cultures matching the condition were updated." ErrorMessage="Cultures matching the condition were not found." />
    <%-- Culture on site --%>
    <cms:LocalizedHeading ID="headAddCultureToSite" runat="server" Text="Culture on site" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiAddCultureToSite" runat="server" ButtonText="Add culture to site" InfoMessage="Culture 'My new culture' was added to site." ErrorMessage="Culture 'My new culture' was not found." />
</asp:Content>
<asp:Content ID="contentRight" ContentPlaceHolderID="plcRightContainer" runat="server">
    <%-- Culture on site --%>
    <cms:LocalizedHeading ID="headRemoveCultureFromSite" runat="server" Text="Culture on site" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiRemoveCultureFromSite" runat="server" ButtonText="Remove culture from site" APIExampleType="CleanUpMain" InfoMessage="Culture 'My new culture' was removed from site." ErrorMessage="Culture 'My new culture' was not found." />
    <%-- Culture --%>
    <cms:LocalizedHeading ID="headDeleteCulture" runat="server" Text="Culture" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteCulture" runat="server" ButtonText="Delete culture" APIExampleType="CleanUpMain" InfoMessage="Culture 'My new culture' and all its dependencies were deleted." ErrorMessage="Culture 'My new culture' was not found." />
</asp:Content>
