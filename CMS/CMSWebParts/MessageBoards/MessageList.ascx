<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/CMSWebParts/MessageBoards/MessageList.ascx.cs"
    Inherits="CMSWebParts_MessageBoards_MessageList" %>
<%@ Register Src="~/CMSModules/MessageBoards/Controls/Messages/MessageList.ascx"
    TagName="MessageList" TagPrefix="cms" %>
    
<cms:MessageList runat="server" ID="ucMessageList" />
