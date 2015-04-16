<%@ Page Title="" Language="C#" MasterPageFile="~/CMSAPIExamples/Pages/APIExamplesPage.Master"
    Theme="Default" AutoEventWireup="true" Inherits="CMSAPIExamples_Code_Development_ObjectVersioning_Default"
    CodeFile="Default.aspx.cs" %>

<%@ Register Src="~/CMSAPIExamples/Controls/APIExample.ascx" TagName="APIExample"
    TagPrefix="cms" %>
<asp:Content ID="contentLeft" ContentPlaceHolderID="plcLeftContainer" runat="server">
    <asp:Label ID="versioningLabel" runat="server" Text="These examples require object versioning enabled in settings."></asp:Label>
    <%-- Object versioning --%>
    <cms:LocalizedHeading ID="headVersioning" runat="server" Text="Versioning" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreateVersionedObject" runat="server" ButtonText="Create versioned object"
        InfoMessage="Stylesheet 'My new versioned stylesheet' was created and assigned to current site."
        ErrorMessage="Object versioning of stylesheet objects is not allowed on current site" />
    <cms:APIExample ID="apiCreateVersion" runat="server" ButtonText="Create version"
        InfoMessage="New version of stylesheet 'My new versioned stylesheet' was created."
        ErrorMessage="Stylesheet 'My new versioned stylesheet' was not found." />
    <cms:APIExample ID="apiRollbackVersion" runat="server" ButtonText="Rollback version"
        InfoMessage="Stylesheet 'My new versioned stylesheet' was rolled back to previous version."
        ErrorMessage="Stylesheet 'My new versioned stylesheet' was not found or there is no version in its history." />
    <cms:APIExample ID="apiDestroyVersion" runat="server" ButtonText="Destroy version"
        InfoMessage="Latest version of stylesheet 'My new versioned stylesheet' was destroyed from version history."
        APIExampleType="ManageAdditional" ErrorMessage="Stylesheet 'My new versioned stylesheet' was not found or its version history is empty." />
    <cms:APIExample ID="apiDestroyHistory" runat="server" ButtonText="Destroy history"
        InfoMessage="Version history of stylesheet 'My new versioned stylesheet' was destroyed."
        APIExampleType="ManageAdditional" ErrorMessage="Stylesheet 'My new versioned stylesheet' was not found." />
    <cms:APIExample ID="apiEnsureVersion" runat="server" ButtonText="Ensure version"
        InfoMessage="Version of stylesheet 'My new versioned stylesheet' was ensured - new version was created if the history was empty."
        APIExampleType="ManageAdditional" ErrorMessage="Stylesheet 'My new versioned stylesheet' was not found." />
    <%-- Object recycle bin --%>
    <cms:LocalizedHeading ID="headRecycleBin" runat="server" Text="Recycle bin" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteObject" runat="server" ButtonText="Delete object" APIExampleType="ManageAdditional"
        InfoMessage="Stylesheet 'My new versioned stylesheet' was deleted to object recycle bin."
        ErrorMessage="Stylesheet 'My new versioned stylesheet' was not found or object restoring of stylesheet objects from recycle bin is not allowed on current site." />
    <cms:APIExample ID="apiRestoreObject" runat="server" ButtonText="Restore object"
        APIExampleType="ManageAdditional" InfoMessage="Stylesheet 'My new versioned stylesheet' was restored from object recycle bin."
        ErrorMessage="No version matching the parameters was found." />
</asp:Content>
<asp:Content ID="contentRight" ContentPlaceHolderID="plcRightContainer" runat="server">
    <%-- Object versioning --%>
    <cms:LocalizedHeading ID="headDestroyVersion" runat="server" Text="Versioning" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDestroyObject" runat="server" ButtonText="Destroy object"
        APIExampleType="CleanUpMain" InfoMessage="Stylesheet 'My new versioned stylesheet' was destroyed."
        ErrorMessage="Stylesheet 'My new versioned stylesheet' was not found." />
</asp:Content>
