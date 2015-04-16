<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_ProjectManagement_Controls_UI_Project_Security" CodeFile="Security.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniMatrix.ascx" TagName="UniMatrix"
    TagPrefix="cms" %>

<cms:LocalizedLabel runat="server" ID="lblInfo" CssClass="InfoLabel" Visible="false" EnableViewState="false" />
<cms:LocalizedLabel runat="server" ID="lblError" CssClass="ErrorLabel" Visible="false" EnableViewState="false" />
<asp:Table runat="server" ID="tblMatrix" CssClass="table table-hover permission-matrix">
</asp:Table>
<cms:LocalizedHeading runat="server" ID="headTitle" Level="4" ResourceString="SecurityMatrix.RolesAvailability" CssClass="listing-title" EnableViewState="false" />
<cms:UniMatrix ID="gridMatrix" runat="server" QueryName="PM.ProjectRolePermission.getpermissionMatrix"
    RowItemIDColumn="RoleID" ColumnItemIDColumn="PermissionID" RowItemCodeNameColumn="RoleName" RowItemDisplayNameColumn="RoleDisplayName"
    ColumnItemDisplayNameColumn="PermissionDisplayName" RowTooltipColumn="RowDisplayName" FirstColumnClass="first-column"
    ColumnItemTooltipColumn="PermissionDescription" ColumnTooltipColumn="PermissionDescription" ItemTooltipColumn="PermissionDescription" AddGlobalObjectSuffix="true" SiteIDColumnName="SiteID" />
