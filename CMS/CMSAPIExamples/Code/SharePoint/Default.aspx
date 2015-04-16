<%@ Page Title="" Language="C#" MasterPageFile="~/CMSAPIExamples/Pages/APIExamplesPage.Master"
    Theme="Default" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="CMSAPIExamples_Code_SharePoint_Default" %>

<%@ Register Src="~/CMSAPIExamples/Controls/APIExample.ascx" TagName="APIExample" TagPrefix="cms" %>
<asp:Content ID="contentLeft" ContentPlaceHolderID="plcLeftContainer" runat="server">
    <%-- SharePoint connection --%>
    <cms:LocalizedHeading ID="headCreateSharePointConnection" runat="server" Text="Create and update SharePoint connection" Level="4" EnableViewState="false" />
    <div class="form-horizontal">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel ResourceString="sharepoint.connection.siteurl" ToolTipResourceString="sharepoint.connection.siteurl.description" CssClass="control-label"
                    runat="server" ID="lblSiteUrl" EnableViewState="false" AssociatedControlID="txtSiteUrl" ShowRequiredMark="true" DisplayColon="true" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox ID="txtSiteUrl" runat="server" MaxLength="512" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel ResourceString="sharepoint.connection.sharepointversion" ToolTipResourceString="sharepoint.connection.siteurl.description"
                    CssClass="control-label" runat="server" ID="lblServerVersion" EnableViewState="false" AssociatedControlID="spServerVersion" ShowRequiredMark="true" DisplayColon="true" />
            </div>
            <div class="editing-form-value-cell">
                <cms:FormControl ID="spServerVersion" runat="server" FormControlName="SharePointVersionSelector" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel ResourceString="sharepoint.connection.username" ToolTipResourceString="sharepoint.connection.username.description" CssClass="control-label"
                    runat="server" ID="lblUserName" EnableViewState="false" AssociatedControlID="txtUserName" DisplayColon="true" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox ID="txtUserName" runat="server" MaxLength="100" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel ResourceString="sharepoint.connection.domain" ToolTipResourceString="sharepoint.connection.domain.description" CssClass="control-label"
                    runat="server" ID="lblDomain" EnableViewState="false" AssociatedControlID="txtDomain" DisplayColon="true" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox ID="txtDomain" runat="server" MaxLength="100" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel ResourceString="sharepoint.connection.password" ToolTipResourceString="sharepoint.connection.password.description" CssClass="control-label"
                    runat="server" ID="lblpassword" EnableViewState="false" AssociatedControlID="txtPassword" DisplayColon="true" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox TextMode="Password" ID="txtPassword" runat="server" MaxLength="100" />
            </div>
        </div>
    </div>
    <div class="api-examples-hint">
        <cms:LocalizedLabel runat="server" ID="lblConnectionHint" CssClass="" ResourceString="sharepoint.apiexamples.hint"></cms:LocalizedLabel>
    </div>
    <cms:APIExample ID="apiCreateSharePointConnection" runat="server" ButtonText="Create SharePoint connection" InfoMessage="SharePoint connection 'My new connection' was created." />
    <cms:APIExample ID="apiGetAndUpdateSharePointConnection" runat="server" ButtonText="Get and update SharePoint connection" APIExampleType="ManageAdditional" InfoMessage="SharePoint connection 'My new connection' was updated." />
    
    <!-- Get lists -->
    <cms:LocalizedHeading ID="headGetLists" runat="server" Text="Retrieve all lists from SharePoint server" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiGetAllLists" runat="server" ButtonText="Get all lists" APIExampleType="ManageAdditional" InfoMessage="All lists were retrieved from SharePoint server using 'My new connection'." />
    
    <!-- Get list's items -->
    <cms:LocalizedHeading ID="headGetListItems" runat="server" Text="Retrieve selected list's items from SharePoint server" Level="4" EnableViewState="false" />
    <div class="form-horizontal">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel Text="List name" CssClass="control-label"
                    runat="server" ID="lblListName" EnableViewState="false" AssociatedControlID="txtListName" ShowRequiredMark="true" DisplayColon="true" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox ID="txtListName" runat="server" MaxLength="512" />
            </div>
        </div>
    </div>
    <cms:APIExample ID="apiGetListItems" runat="server" ButtonText="Get list items" APIExampleType="ManageAdditional" InfoMessage="All list items were retrieved from SharePoint server using 'My new connection'." />
    
     <!-- Get file -->
    <cms:LocalizedHeading ID="headGetFile" runat="server" Text="Retrieve file from SharePoint server" Level="4" EnableViewState="false" />
    <div class="form-horizontal">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel Text="File relative path" CssClass="control-label"
                    runat="server" ID="lblFilePath" EnableViewState="false" AssociatedControlID="txtFilePath" ShowRequiredMark="true" DisplayColon="true" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox ID="txtFilePath" runat="server" MaxLength="512" />
            </div>
        </div>
    </div>
    <cms:APIExample ID="apiGetFile" runat="server" ButtonText="Get file" APIExampleType="ManageAdditional" InfoMessage="File was retrieved from SharePoint server using 'My new connection'." />
</asp:Content>
<asp:Content ID="contentRight" ContentPlaceHolderID="plcRightContainer" runat="server">
    <%-- SharePoint connection --%>
    <cms:LocalizedHeading ID="headDeleteSharePointConnection" runat="server" Text="SharePoint connection" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteSharePointConnection" runat="server" ButtonText="Delete SharePoint connection" APIExampleType="CleanUpMain" InfoMessage="SharePoint connection was deleted." />
</asp:Content>
