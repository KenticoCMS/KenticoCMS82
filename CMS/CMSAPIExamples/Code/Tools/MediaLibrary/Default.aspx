<%@ Page Title="" Language="C#" MasterPageFile="~/CMSAPIExamples/Pages/APIExamplesPage.Master"
    Theme="Default" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="CMSAPIExamples_Code_Tools_MediaLibrary_Default" %>

<%@ Register Src="~/CMSAPIExamples/Controls/APIExample.ascx" TagName="APIExample" TagPrefix="cms" %>
<asp:Content ID="contentLeft" ContentPlaceHolderID="plcLeftContainer" runat="server">
    <%-- Media library --%>
    <cms:LocalizedHeading ID="headCreateMediaLibrary" runat="server" Text="Media library" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreateMediaLibrary" runat="server" ButtonText="Create library" InfoMessage="Library 'My new library' was created." />
    <cms:APIExample ID="apiGetAndUpdateMediaLibrary" runat="server" ButtonText="Get and update library" APIExampleType="ManageAdditional" InfoMessage="Library 'My new library' was updated." ErrorMessage="Library 'My new library' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateMediaLibraries" runat="server" ButtonText="Get and bulk update libraries" APIExampleType="ManageAdditional" InfoMessage="All libraries matching the condition were updated." ErrorMessage="Libraries matching the condition were not found." />
    <%-- Media folder --%>
    <cms:LocalizedHeading ID="headCreateMediaFolder" runat="server" Text="Media folder" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreateMediaFolder" runat="server" ButtonText="Create folder" InfoMessage="Folder 'My new folder' was created." ErrorMessage="Media library 'My new library' was not found." />
    <%-- Media file --%>
    <cms:LocalizedHeading ID="headCreateMediaFile" runat="server" Text="Media file" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreateMediaFile" runat="server" ButtonText="Create file" InfoMessage="File 'My new file' was created." ErrorMessage="Media library 'My new library' was not found." />
    <cms:APIExample ID="apiGetAndUpdateMediaFile" runat="server" ButtonText="Get and update file" APIExampleType="ManageAdditional" InfoMessage="File 'My new file' was updated." ErrorMessage="File 'My new file' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateMediaFiles" runat="server" ButtonText="Get and bulk update files" APIExampleType="ManageAdditional" InfoMessage="All files matching the condition were updated." ErrorMessage="Files matching the condition were not found." />
    <%-- Role permission in media library --%>
    <cms:LocalizedHeading ID="headAddRolePermissionToLibrary" runat="server" Text="Role permission in media library" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiAddRolePermissionToLibrary" runat="server" ButtonText="Add role permission to library" InfoMessage="Role permission was added to library." ErrorMessage="Library 'My new library', role 'CMSDeskAdmin' or permission 'FileCreate' were not found." />
</asp:Content>
<asp:Content ID="contentRight" ContentPlaceHolderID="plcRightContainer" runat="server">
    <%-- Role permission in media library --%>
    <cms:LocalizedHeading ID="headRemoveRolePermissionFromLibrary" runat="server" Text="Role permission in media library" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiRemoveRolePermissionFromLibrary" runat="server" ButtonText="Remove role permission" APIExampleType="CleanUpMain" InfoMessage="Role permission was removed from library." ErrorMessage="Library 'My new library', role 'CMSDeskAdmin', permission 'FileCreate' or their relationship were not found." />
    <%-- Media file --%>
    <cms:LocalizedHeading ID="headDeleteMediaFile" runat="server" Text="Media file" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteMediaFile" runat="server" ButtonText="Delete file" APIExampleType="CleanUpMain" InfoMessage="File 'My new file' and all its dependencies were deleted." ErrorMessage="File 'My new file' was not found." />
    <%-- Media folder --%>
    <cms:LocalizedHeading ID="headDeleteMediaFolder" runat="server" Text="Media folder" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteMediaFolder" runat="server" ButtonText="Delete folder" APIExampleType="CleanUpMain" InfoMessage="Folder 'My new folder' and all its dependencies were deleted." ErrorMessage="Folder 'My new folder' was not found." />
    <%-- Media library --%>
    <cms:LocalizedHeading ID="headDeleteMediaLibrary" runat="server" Text="Media library" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteMediaLibrary" runat="server" ButtonText="Delete library" APIExampleType="CleanUpMain" InfoMessage="Library 'My new library' and all its dependencies were deleted." ErrorMessage="Library 'My new library' was not found." />
</asp:Content>
