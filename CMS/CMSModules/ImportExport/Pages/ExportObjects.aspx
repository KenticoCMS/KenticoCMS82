<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_ImportExport_Pages_ExportObjects"
    Theme="Default" ValidateRequest="false" EnableEventValidation="false" CodeFile="ExportObjects.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/PageElements/PageTitle.ascx" TagName="PageTitle" TagPrefix="cms" %>

<%@ Register Src="~/CMSModules/ImportExport/Controls/ExportWizard.ascx" TagName="ExportWizard"
    TagPrefix="cms" %>
<!DOCTYPE html>
<html>
<head runat="server" enableviewstate="false">
    <title>Export objects</title>
    <style type="text/css">
        body {
            margin: 0px;
            padding: 0px;
            height: 100%;
        }
    </style>
</head>
<body class="<%=mBodyClass%>">
    <form id="form1" runat="server">
        <asp:Panel ID="PanelBody" runat="server" CssClass="PageBody">
            <asp:Panel ID="PanelTitle" runat="server" CssClass="PageHeader SimpleHeader" EnableViewState="false">
                <cms:PageTitle ID="ptExportSiteSettings" runat="server" HideTitle="true" />
            </asp:Panel>
            <asp:Panel ID="PanelExportSettings" runat="server" CssClass="PageContent">
                <cms:ExportWizard ID="wzdExport" ShortID="w" runat="server" />
            </asp:Panel>
        </asp:Panel>
    </form>
</body>
</html>
