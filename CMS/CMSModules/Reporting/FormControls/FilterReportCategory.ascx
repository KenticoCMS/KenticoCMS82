<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/CMSModules/Reporting/FormControls/FilterReportCategory.ascx.cs"
    Inherits="CMSModules_Reporting_FormControls_FilterReportCategory" %>
<asp:Panel CssClass="Filter" runat="server" ID="pnlSearch">
    <cms:LocalizedLabel ID="lblCategory" ResourceString="administration-pagetemplate_general.category" runat="server" EnableViewState="false" />&nbsp;
    <cms:SelectReportCategory runat="server" ID="usCategories" ShowRootCategory="true" UseAutoPostBack="true" />
</asp:Panel>
