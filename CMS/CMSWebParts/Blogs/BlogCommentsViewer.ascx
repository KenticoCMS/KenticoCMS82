<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSWebParts_Blogs_BlogCommentsViewer" CodeFile="~/CMSWebParts/Blogs/BlogCommentsViewer.ascx.cs" %>
<%@ Register TagPrefix="cms" Namespace="CMS.Blogs" Assembly="CMS.Blogs" %>
<cms:BasicRepeater ID="repComments" runat="server" />
<cms:BlogCommentDataSource ID="blogDataSource" runat="server" />
<div class="Pager">
    <cms:UniPager ID="pagerElem" runat="server" />
</div>