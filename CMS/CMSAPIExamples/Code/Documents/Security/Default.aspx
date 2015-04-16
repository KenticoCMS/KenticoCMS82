<%@ Page Title="" Language="C#" MasterPageFile="~/CMSAPIExamples/Pages/APIExamplesPage.Master"
    Theme="Default" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="CMSAPIExamples_Code_Documents_Security_Default" %>

<%@ Register Src="~/CMSAPIExamples/Controls/APIExample.ascx" TagName="APIExample" TagPrefix="cms" %>
<asp:Content ID="contentLeft" ContentPlaceHolderID="plcLeftContainer" runat="server">
    <%-- Creating pages --%>
    <cms:LocalizedHeading ID="headCreateDocument" runat="server" Text="Creating pages" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreateDocumentStructure" runat="server" ButtonText="Create page structure" InfoMessage="Page structure for the API example created successfully." ErrorMessage="Site root node not found." />
    <%-- Setting page level permissions --%>
    <cms:LocalizedHeading ID="headSetPermissions" runat="server" Text="Setting page level permissions" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiSetUserPermissions" runat="server" ButtonText="Set user permissions" InfoMessage="The 'Modify permissions' permission for page 'API Example' was successfully granted to user 'Andy'." ErrorMessage="Page 'API Example' or user 'Andy' not found." />
    <cms:APIExample ID="apiSetRolePermissions" runat="server" ButtonText="Set role permissions" InfoMessage="The 'Modify' permission for page 'API Example' was successfully granted to role 'CMSDeskAdministrator'." ErrorMessage="Page 'API Example' or role 'CMSDeskAdministrator' not found." />
    <%-- Permission inheritance --%>
    <cms:LocalizedHeading ID="headPermissionInheritance" runat="server" Text="Permission inheritance" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiBreakPermissionInheritance" runat="server" APIExampleType="ManageAdditional" ButtonText="Break inheritance" InfoMessage="Inheritance of permissions on page 'API Example subpage' broken successfully." ErrorMessage="Page 'API Example subpage' not found." />
    <cms:APIExample ID="apiRestorePermissionInheritance" runat="server" APIExampleType="ManageAdditional" ButtonText="Restore inheritance" InfoMessage="Inheritance of permissions on page 'API Example subpage' restored successfully." ErrorMessage="Page 'API Example subpage' not found." />
    <%-- Checking permissions --%>
    <cms:LocalizedHeading ID="headCheckPermissions" runat="server" Text="Checking permissions" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCheckContentModulePermissions" runat="server" ButtonText="Check module permissions" APIExampleType="ManageAdditional" InfoMessage="User 'Andy' is allowed to read module 'Content'." ErrorMessage="User 'Andy' not found." />
    <cms:APIExample ID="apiCheckDocTypePermissions" runat="server" ButtonText="Check page type permissions" APIExampleType="ManageAdditional" InfoMessage="User 'Andy' is allowed to read page type 'Menu item'." ErrorMessage="User 'Andy' not found." />
    <cms:APIExample ID="apiCheckDocumentPermissions" runat="server" ButtonText="Check page permissions" APIExampleType="ManageAdditional" InfoMessage="User 'Andy' is allowed to modify permissions for page 'API Example'." ErrorMessage="Page 'API Example' or user 'Andy' not found." />
    <cms:APIExample ID="apiFilterDataSet" runat="server" ButtonText="Filter data set" APIExampleType="ManageAdditional" InfoMessage="Data set with all pages filtered successfully by permission 'Modify permissions' for user 'Andy'. Permission inheritance broken for filtered items." ErrorMessage="User 'Andy' not found." />
</asp:Content>
<asp:Content ID="contentRight" ContentPlaceHolderID="plcRightContainer" runat="server">
    <%-- Deleting page level permissions --%>
    <cms:LocalizedHeading ID="headDelelePermissions" runat="server" Text="Deleting page level permissions" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeletePermissions" runat="server" APIExampleType="CleanUpMain" ButtonText="Delete permissions" InfoMessage="The page level permissions deleted successfully." ErrorMessage="The page structure not found." />
    <%-- Deleting pages --%>
    <cms:LocalizedHeading ID="headDeleleDocument" runat="server" Text="Deleting pages" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteDocumentStructure" runat="server" APIExampleType="CleanUpMain" ButtonText="Delete page structure" InfoMessage="The page structure deleted successfully." ErrorMessage="The page structure not found." />
</asp:Content>
