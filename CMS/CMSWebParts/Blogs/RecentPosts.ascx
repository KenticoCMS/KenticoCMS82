<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSWebParts_Blogs_RecentPosts" CodeFile="~/CMSWebParts/Blogs/RecentPosts.ascx.cs" %>


<cms:cmsrepeater ID="rptRecentPosts" runat="server" OrderBy="BlogPostDate DESC" ClassNames="cms.blogpost" />

