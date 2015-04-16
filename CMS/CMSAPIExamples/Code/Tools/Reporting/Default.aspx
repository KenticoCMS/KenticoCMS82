<%@ Page Title="" Language="C#" MasterPageFile="~/CMSAPIExamples/Pages/APIExamplesPage.Master"
    Theme="Default" AutoEventWireup="true" Inherits="CMSAPIExamples_Code_Tools_Reporting_Default" CodeFile="Default.aspx.cs" %>

<%@ Register Src="~/CMSAPIExamples/Controls/APIExample.ascx" TagName="APIExample" TagPrefix="cms" %>
<asp:Content ID="contentLeft" ContentPlaceHolderID="plcLeftContainer" runat="server">
    <%-- Report category --%>
    <cms:LocalizedHeading ID="headCreateReportCategory" runat="server" Text="Report category" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreateReportCategory" runat="server" ButtonText="Create category" InfoMessage="Category 'My new category' was created." />
    <cms:APIExample ID="apiGetAndUpdateReportCategory" runat="server" ButtonText="Get and update category" APIExampleType="ManageAdditional" InfoMessage="Category 'My new category' was updated." ErrorMessage="Category 'My new category' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateReportCategories" runat="server" ButtonText="Get and bulk update categories" APIExampleType="ManageAdditional" InfoMessage="All categories matching the condition were updated." ErrorMessage="Categories matching the condition were not found." />
    <%-- Report --%>
    <cms:LocalizedHeading ID="headCreateReport" runat="server" Text="Report" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreateReport" runat="server" ButtonText="Create report" InfoMessage="Report 'My new report' was created." ErrorMessage="Category 'My new category' was not found." />
    <cms:APIExample ID="apiGetAndUpdateReport" runat="server" ButtonText="Get and update report" APIExampleType="ManageAdditional" InfoMessage="Report 'My new report' was updated." ErrorMessage="Report 'My new report' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateReports" runat="server" ButtonText="Get and bulk update reports" APIExampleType="ManageAdditional" InfoMessage="All reports matching the condition were updated." ErrorMessage="Reports matching the condition were not found." />
    <%-- Report graph --%>
    <cms:LocalizedHeading ID="headCreateReportGraph" runat="server" Text="Report graph" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreateReportGraph" runat="server" ButtonText="Create graph" InfoMessage="Graph 'My new graph' was created." ErrorMessage="Report 'My new report' was not found." />
    <cms:APIExample ID="apiGetAndUpdateReportGraph" runat="server" ButtonText="Get and update graph" APIExampleType="ManageAdditional" InfoMessage="Graph 'My new graph' was updated." ErrorMessage="Graph 'My new graph' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateReportGraphs" runat="server" ButtonText="Get and bulk update graphs" APIExampleType="ManageAdditional" InfoMessage="All graphs matching the condition were updated." ErrorMessage="Graphs matching the condition were not found." />
    <%-- Report table --%>
    <cms:LocalizedHeading ID="headCreateReportTable" runat="server" Text="Report table" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreateReportTable" runat="server" ButtonText="Create table" InfoMessage="Table 'My new table' was created." ErrorMessage="Report 'My new report' was not found." />
    <cms:APIExample ID="apiGetAndUpdateReportTable" runat="server" ButtonText="Get and update table" APIExampleType="ManageAdditional" InfoMessage="Table 'My new table' was updated." ErrorMessage="Table 'My new table' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateReportTables" runat="server" ButtonText="Get and bulk update tables" APIExampleType="ManageAdditional" InfoMessage="All tables matching the condition were updated." ErrorMessage="Tables matching the condition were not found." />
    <%-- Report value --%>
    <cms:LocalizedHeading ID="headCreateReportValue" runat="server" Text="Report value" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreateReportValue" runat="server" ButtonText="Create value" InfoMessage="Value 'My new value' was created." ErrorMessage="Report 'My new report' was not found." />
    <cms:APIExample ID="apiGetAndUpdateReportValue" runat="server" ButtonText="Get and update value" APIExampleType="ManageAdditional" InfoMessage="Value 'My new value' was updated." ErrorMessage="Value 'My new value' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateReportValues" runat="server" ButtonText="Get and bulk update values" APIExampleType="ManageAdditional" InfoMessage="All values matching the condition were updated." ErrorMessage="Values matching the condition were not found." />
    <%-- Report actions --%>
    <cms:LocalizedHeading ID="headReportActions" runat="server" Text="Report actions" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiInsertElementsToLayout" runat="server" ButtonText="Insert elements to layout" APIExampleType="ManageAdditional" InfoMessage="Elements were added to report layout." ErrorMessage="Report 'My new report' was not found." />
</asp:Content>
<asp:Content ID="contentRight" ContentPlaceHolderID="plcRightContainer" runat="server">
    <%-- Report value --%>
    <cms:LocalizedHeading ID="headDeleteReportValue" runat="server" Text="Report value" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteReportValue" runat="server" ButtonText="Delete value" APIExampleType="CleanUpMain" InfoMessage="Value 'My new value' and all its dependencies were deleted." ErrorMessage="Value 'My new value' was not found." />
    <%-- Report table --%>
    <cms:LocalizedHeading ID="headDeleteReportTable" runat="server" Text="Report table" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteReportTable" runat="server" ButtonText="Delete table" APIExampleType="CleanUpMain" InfoMessage="Table 'My new table' and all its dependencies were deleted." ErrorMessage="Table 'My new table' was not found." />
    <%-- Report graph --%>
    <cms:LocalizedHeading ID="headDeleteReportGraph" runat="server" Text="Report graph" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteReportGraph" runat="server" ButtonText="Delete graph" APIExampleType="CleanUpMain" InfoMessage="Graph 'My new graph' and all its dependencies were deleted." ErrorMessage="Graph 'My new graph' was not found." />
    <%-- Report --%>
    <cms:LocalizedHeading ID="headDeleteReport" runat="server" Text="Report" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteReport" runat="server" ButtonText="Delete report" APIExampleType="CleanUpMain" InfoMessage="Report 'My new report' and all its dependencies were deleted." ErrorMessage="Report 'My new report' was not found." />
    <%-- Report category --%>
    <cms:LocalizedHeading ID="headDeleteReportCategory" runat="server" Text="Report category" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteReportCategory" runat="server" ButtonText="Delete category" APIExampleType="CleanUpMain" InfoMessage="Category 'My new category' and all its dependencies were deleted." ErrorMessage="Category 'My new category' was not found." />
</asp:Content>
