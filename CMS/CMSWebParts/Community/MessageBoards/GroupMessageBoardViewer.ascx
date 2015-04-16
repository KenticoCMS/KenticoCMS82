<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSWebParts_Community_MessageBoards_GroupMessageBoardViewer" CodeFile="~/CMSWebParts/Community/MessageBoards/GroupMessageBoardViewer.ascx.cs" %>
<%@ Register TagPrefix="cms" Namespace="CMS.MessageBoards" Assembly="CMS.MessageBoards" %>
<cms:BasicRepeater ID="repMessages" runat="server" />
<cms:BoardMessagesDataSource ID="boardDataSource" runat="server" />
<div class="Pager">
    <cms:UniPager ID="pagerElem" runat="server" />
</div>
