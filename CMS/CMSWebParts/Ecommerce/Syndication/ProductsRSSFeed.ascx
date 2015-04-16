<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSWebParts_Ecommerce_Syndication_ProductsRSSFeed" CodeFile="~/CMSWebParts/Ecommerce/Syndication/ProductsRSSFeed.ascx.cs" %>
<%@ Register TagPrefix="cms" Namespace="CMS.Ecommerce" Assembly="CMS.Ecommerce" %>
<cms:ProductsDataSource runat="server" ID="srcProducts" />
<cms:RSSFeed ID="rssFeed" runat="server" />