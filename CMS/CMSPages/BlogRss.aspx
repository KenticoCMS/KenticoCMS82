<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSPages_BlogRss" CodeFile="BlogRss.aspx.cs" %>

<rss version="2.0">
    <channel>
        <title>Blog RSS</title>
        <link><![CDATA[<%=HttpContext.Current.Request.Url.AbsoluteUri.Remove(HttpContext.Current.Request.Url.AbsoluteUri.Length - HttpContext.Current.Request.Url.PathAndQuery.Length) + HttpContext.Current.Request.ApplicationPath%>]]></link>
        <description>Blog RSS Feed</description>  
        <cms:cmsrepeater ID="repeater" runat="server" OrderBy="BlogPostDate DESC" ClassNames="cms.blogpost" TransformationName="cms.blogpost.rssitem" SelectedItemTransformationName="cms.blogpost.rssitem" SelectTopN="25" SelectOnlyPublished="true" />
 </channel>
</rss>
