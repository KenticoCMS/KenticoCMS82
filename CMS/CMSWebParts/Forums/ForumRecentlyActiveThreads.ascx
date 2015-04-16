<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSWebParts_Forums_ForumRecentlyActiveThreads" CodeFile="~/CMSWebParts/Forums/ForumRecentlyActiveThreads.ascx.cs" %>
<%@ Register TagPrefix="cms" Namespace="CMS.Forums" Assembly="CMS.Forums" %>
<cms:BasicRepeater runat="server" ID="repLatestPosts" />
<cms:ForumPostsDataSource runat="server" ID="forumDataSource" />
