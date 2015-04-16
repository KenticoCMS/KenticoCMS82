<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSPages_NewsRss" CodeFile="NewsRss.aspx.cs" %>
<rss version="2.0">
 <channel>
  <title>News RSS</title>
 <link><![CDATA[<%=HttpContext.Current.Request.Url.AbsoluteUri.Remove(HttpContext.Current.Request.Url.AbsoluteUri.Length - HttpContext.Current.Request.Url.PathAndQuery.Length) + HttpContext.Current.Request.ApplicationPath%>]]></link> 
  <description>News RSS Feed</description>  
  
  <cms:cmsrepeater ID="NewsRepeater" runat="server" OrderBy="NewsReleaseDate DESC" ClassNames="cms.news"
   TransformationName="cms.news.rssitem" SelectedItemTransformationName="cms.news.rssitem"
   Path="/news/%" WhereCondition="NewsReleaseDate < GetDate()"></cms:cmsrepeater>   
 </channel>
</rss>
