<%@ Page Title="" Language="C#" MasterPageFile="~/CMSAPIExamples/Pages/APIExamplesPage.Master"
    Theme="Default" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="CMSAPIExamples_Code_Development_Modules_Default" %>

<%@ Register Src="~/CMSAPIExamples/Controls/APIExample.ascx" TagName="APIExample" TagPrefix="cms" %>
<asp:Content ID="contentLeft" ContentPlaceHolderID="plcLeftContainer" runat="server">
    <%-- headtion: Modules --%>
    <cms:LocalizedHeading ID="headManModules" runat="server" Text="Modules" Level="4" EnableViewState="false" />
    <%-- Module --%>
    <cms:LocalizedHeading ID="headCreateModule" runat="server" Text="Module" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiCreateModule" runat="server" ButtonText="Create module" InfoMessage="Module 'My new module' was created." />
    <cms:APIExample ID="apiGetAndUpdateModule" runat="server" ButtonText="Get and update module" APIExampleType="ManageAdditional" InfoMessage="Module 'My new module' was updated." ErrorMessage="Module 'My new module' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateModules" runat="server" ButtonText="Get and bulk update modules" APIExampleType="ManageAdditional" InfoMessage="All modules matching the condition were updated." ErrorMessage="Modules matching the condition were not found." />
    <%-- Module on site --%>
    <cms:LocalizedHeading ID="headAddModuleToSite" runat="server" Text="Module on site" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiAddModuleToSite" runat="server" ButtonText="Add module to site" InfoMessage="Module 'My new module' was added to the current site." ErrorMessage="Module 'My new module' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateSiteModules" runat="server" ButtonText="Get and bulk update site modules" APIExampleType="ManageAdditional" InfoMessage="All modules from the current site matching the condition were updated." ErrorMessage="Modules from the current site matching the condition were not found." />
    <%-- headtion: Permissions --%>
    <cms:LocalizedHeading ID="headManPermissions" runat="server" Text="Permissions" Level="4" EnableViewState="false" />
    <%-- Permission --%>
    <cms:LocalizedHeading ID="headCreatePermission" runat="server" Text="Permission" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiCreatePermission" runat="server" ButtonText="Create permission" InfoMessage="Permission 'My new permission' was created." ErrorMessage="Module 'My new module' was not found." />
    <cms:APIExample ID="apiGetAndUpdatePermission" runat="server" ButtonText="Get and update permission" APIExampleType="ManageAdditional" InfoMessage="Permission 'My new permission' was updated." ErrorMessage="Permission 'My new permission' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdatePermissions" runat="server" ButtonText="Get and bulk update permissions" APIExampleType="ManageAdditional" InfoMessage="All permissions matching the condition were updated." ErrorMessage="Permissions matching the condition were not found." />
    <%-- Role permission --%>
    <cms:LocalizedHeading ID="headAddPermissionToRole" runat="server" Text="Role permission" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiAddPermissionToRole" runat="server" ButtonText="Add permission to role" InfoMessage="Permission 'My new permission' was added to role 'CMS Desk administrators'." ErrorMessage="Permission 'My new permission' or role 'CMS Desk administrators' were not found." />
    <%-- headtion: UI elements --%>
    <cms:LocalizedHeading ID="headManUIelements" runat="server" Text="UI elements" Level="4" EnableViewState="false" />
    <%-- UI element --%>
    <cms:LocalizedHeading ID="headCreateUIElement" runat="server" Text="UI element" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiCreateUIElement" runat="server" ButtonText="Create element" InfoMessage="Element 'My new element' was created." ErrorMessage="Module 'My new module' was not found." />
    <cms:APIExample ID="apiGetAndUpdateUIElement" runat="server" ButtonText="Get and update element" APIExampleType="ManageAdditional" InfoMessage="Element 'My new element' was updated." ErrorMessage="Element 'My new element' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateUIElements" runat="server" ButtonText="Get and bulk update elements" APIExampleType="ManageAdditional" InfoMessage="All elements matching the condition were updated." ErrorMessage="Elements matching the condition were not found." />
    <%-- Role UI element --%>
    <cms:LocalizedHeading ID="headAddUIElementToRole" runat="server" Text="Role UI element" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiAddUIElementToRole" runat="server" ButtonText="Add element to role" InfoMessage="Element 'My new element' was added to role 'CMS Desk administrators'." ErrorMessage="Element 'My new element' or role 'CMS Desk administrators' were not found." />
</asp:Content>
<asp:Content ID="contentRight" ContentPlaceHolderID="plcRightContainer" runat="server">
    <%-- headtion: UI elements --%>
    <cms:LocalizedHeading ID="headCleanUIElements" runat="server" Text="UI elements" Level="4" EnableViewState="false" />
    <%-- Role UI element --%>
    <cms:LocalizedHeading ID="headRemoveUIElementFromRole" runat="server" Text="Role UI element" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiRemoveUIElementFromRole" runat="server" ButtonText="Remove element from role" APIExampleType="CleanUpMain" InfoMessage="Element 'My new element' was removed from role 'CMS Desk administrators'." ErrorMessage="Element 'My new element', role 'CMS Desk administrators' or their relationship were not found." />
    <%-- UI element --%>
    <cms:LocalizedHeading ID="headDeleteUIElement" runat="server" Text="UI element" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteUIElement" runat="server" ButtonText="Delete element" APIExampleType="CleanUpMain" InfoMessage="Element 'My new element' and all its dependencies were deleted." ErrorMessage="Element 'My new element' was not found." />
    <%-- headtion: Permissions --%>
    <cms:LocalizedHeading ID="headCleanPermissions" runat="server" Text="Permissions" Level="4" EnableViewState="false" />
    <%-- Role permission --%>
    <cms:LocalizedHeading ID="headRemovePermissionFromRole" runat="server" Text="Role permission" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiRemovePermissionFromRole" runat="server" ButtonText="Remove permission from role" APIExampleType="CleanUpMain" InfoMessage="Permission 'My new permission' was removed from role 'CMS Desk administrators'." ErrorMessage="Permission 'My new permission', role 'CMS Desk administrators' or their relationship were not found." />
    <%-- Permission --%>
    <cms:LocalizedHeading ID="headDeletePermission" runat="server" Text="Permission" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiDeletePermission" runat="server" ButtonText="Delete permission" APIExampleType="CleanUpMain" InfoMessage="Permission 'My new permission' and all its dependencies were deleted." ErrorMessage="Permission 'My new permission' was not found." />
    <%-- headtion: Modules --%>
    <cms:LocalizedHeading ID="headCleanModules" runat="server" Text="Modules" Level="4" EnableViewState="false" />
    <%-- Module on site --%>
    <cms:LocalizedHeading ID="headRemoveModuleFromSite" runat="server" Text="Module on site" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiRemoveModuleFromSite" runat="server" ButtonText="Remove module from site" APIExampleType="CleanUpMain" InfoMessage="Module 'My new module' was removed from the current site." ErrorMessage="Module 'My new module' was not found." />
    <%-- Module --%>
    <cms:LocalizedHeading ID="headDeleteModule" runat="server" Text="Module" Level="5" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteModule" runat="server" ButtonText="Delete module" APIExampleType="CleanUpMain" InfoMessage="Module 'My new module' and all its dependencies were deleted." ErrorMessage="Module 'My new module' was not found." />
</asp:Content>
