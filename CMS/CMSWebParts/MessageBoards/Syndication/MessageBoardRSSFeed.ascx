<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSWebParts_MessageBoards_Syndication_MessageBoardRSSFeed" CodeFile="~/CMSWebParts/MessageBoards/Syndication/MessageBoardRSSFeed.ascx.cs" %>
<%@ Register TagPrefix="cms" Namespace="CMS.MessageBoards" Assembly="CMS.MessageBoards" %>

<cms:BoardMessagesDataSource ID="srcMessages" runat="server" />
<cms:RSSFeed runat="server" ID="rssFeed" />
