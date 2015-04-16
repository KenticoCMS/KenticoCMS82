<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSWebParts_MediaLibrary_Syndication_MediaRSSFeed" CodeFile="~/CMSWebParts/MediaLibrary/Syndication/MediaRSSFeed.ascx.cs" %>
<%@ Register TagPrefix="cms" Namespace="CMS.MediaLibrary" Assembly="CMS.MediaLibrary" %>

<cms:MediaFileDataSource ID="srcMedia" runat="server" />
<cms:RSSFeed runat="server" ID="rssFeed" />
