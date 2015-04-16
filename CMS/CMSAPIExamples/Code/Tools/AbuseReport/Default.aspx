<%@ Page Title="" Language="C#" MasterPageFile="~/CMSAPIExamples/Pages/APIExamplesPage.Master"
    Theme="Default" AutoEventWireup="true" Inherits="CMSAPIExamples_Code_Tools_AbuseReport_Default" CodeFile="Default.aspx.cs" %>

<%@ Register Src="~/CMSAPIExamples/Controls/APIExample.ascx" TagName="APIExample" TagPrefix="cms" %>
<asp:Content ID="contentLeft" ContentPlaceHolderID="plcLeftContainer" runat="server">
    <%-- Abuse report --%>
    <cms:LocalizedHeading ID="headCreateAbuseReport" runat="server" Text="Abuse report" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreateAbuseReport" runat="server" ButtonText="Create report" InfoMessage="Report 'My new report' was created." />
    <cms:APIExample ID="apiGetAndUpdateAbuseReport" runat="server" ButtonText="Get and update report" APIExampleType="ManageAdditional" InfoMessage="Report 'My new report' was updated." ErrorMessage="Report 'My new report' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateAbuseReports" runat="server" ButtonText="Get and bulk update reports" APIExampleType="ManageAdditional" InfoMessage="All reports matching the condition were updated." ErrorMessage="Reports matching the condition were not found." />
</asp:Content>
<asp:Content ID="contentRight" ContentPlaceHolderID="plcRightContainer" runat="server">
    <%-- Abuse report --%>
    <cms:LocalizedHeading ID="headDeleteAbuseReport" runat="server" Text="Abuse report" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteAbuseReport" runat="server" ButtonText="Delete report" APIExampleType="CleanUpMain" InfoMessage="Report 'My new report' and all its dependencies were deleted." ErrorMessage="Report 'My new report' was not found." />
</asp:Content>
