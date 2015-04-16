<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Role_Edit_Permissions_Default.aspx.cs"
    Inherits="CMSModules_Membership_Pages_Roles_Role_Edit_Permissions_Default" %>

<!DOCTYPE html>
<html>
<head id="Head1" runat="server" enableviewstate="false">
    <title>Administration - Roles</title>
</head>
<frameset border="0" rows="48, *">
    <frame name="header" src="Role_Edit_Permissions_Header.aspx<%=Request.Url.Query%>"
        scrolling="no" frameborder="0" />
    <frame name="content" scrolling="auto" frameborder="0" noresize="noresize" />
    <cms:NoFramesLiteral ID="ltlNoFrames" runat="server" />
</frameset>
</html>
