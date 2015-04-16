<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSWebParts_CustomTables_CustomTableDataGrid" CodeFile="~/CMSWebParts/CustomTables/CustomTableDataGrid.ascx.cs" %>
<cms:QueryDataGrid ID="gridItems" runat="server" DataBindByDefault="false" OnItemDataBound="gridItems_ItemDataBound" />