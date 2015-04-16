<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSWebParts_SharePoint_SharePointDataGrid" CodeFile="~/CMSWebParts/SharePoint/SharePointDataGrid.ascx.cs" %>
<%@ Register Src="~/CMSWebParts/SharePoint/SharePointDatasource_files/SharePointDatasource.ascx"
    TagName="SPDataSource" TagPrefix="cms" %>
    
<cms:BasicDataGrid ID="BasicDataGrid" runat="server" >
</cms:BasicDataGrid>
<cms:SPDataSource ID="SPDataSource" runat="server" />
