<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_MessageBoards_Controls_Messages_BoardSelector" CodeFile="BoardSelector.ascx.cs" %>
<%@ Register src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" tagname="UniSelector" tagprefix="cms" %>

<cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
    <ContentTemplate>
        <cms:UniSelector ID="uniSelector" runat="server" ReturnColumnName="BoardID" DisplayNameFormat="{%BoardDisplayName%}"
            ObjectType="board.board" ResourcePrefix="boardselector" />
    </ContentTemplate>
</cms:CMSUpdatePanel>
