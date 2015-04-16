<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSWebParts_Community_Friends_FriendsViewer" CodeFile="~/CMSWebParts/Community/Friends/FriendsViewer.ascx.cs" %>
<%@ Register TagPrefix="cms" Namespace="CMS.Community" Assembly="CMS.Community" %>
<cms:BasicRepeater ID="repFriends" runat="server" />
<cms:FriendsDataSource ID="srcFriends" runat="server" />
<div class="Pager">
    <cms:UniPager ID="pagerElem" runat="server" />
</div>
