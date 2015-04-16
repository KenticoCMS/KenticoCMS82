<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSWebParts_Blogs_Syndication_BlogCommentsRSSFeed" CodeFile="~/CMSWebParts/Blogs/Syndication/BlogCommentsRSSFeed.ascx.cs" %>
<%@ Register TagPrefix="cms" Namespace="CMS.Blogs" Assembly="CMS.Blogs" %>
<cms:BlogCommentDataSource ID="srcComments" runat="server" />
<cms:RSSFeed ID="rssFeed" runat="server" />
