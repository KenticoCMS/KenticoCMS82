<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSWebParts_SharePoint_SharePointDataList" CodeFile="~/CMSWebParts/SharePoint/SharePointDataList.ascx.cs" %>
<%@ Register Src="~/CMSWebParts/SharePoint/SharePointDatasource_files/SharePointDatasource.ascx" TagName="SPDataSource" TagPrefix="cms" %>

<asp:PlaceHolder ID="plcDataset" runat="server">
    <cms:BasicDataList ID="BasicDataList" runat="server" />
</asp:PlaceHolder>
<asp:PlaceHolder ID="plcXSLT" runat="server" Visible="false">
    <asp:Literal ID="ltlTransformedOutput" runat="server" EnableViewState="false" />
</asp:PlaceHolder>
<div class="Pager">
    <cms:UniPager ID="pagerElem" runat="server" />
</div>
<cms:SPDataSource ID="SPDataSource" runat="server" />