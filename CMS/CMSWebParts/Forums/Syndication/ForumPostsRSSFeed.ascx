<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSWebParts_Forums_Syndication_ForumPostsRSSFeed" CodeFile="~/CMSWebParts/Forums/Syndication/ForumPostsRSSFeed.ascx.cs" %>

<%@ Register TagPrefix="cms" Namespace="CMS.Forums" Assembly="CMS.Forums" %>
<cms:ForumPostsDataSource runat="server" ID="srcElem" />
<cms:RSSFeed runat="server" ID="rssFeed" />
