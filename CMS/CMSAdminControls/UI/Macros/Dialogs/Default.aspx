<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSAdminControls_UI_Macros_Dialogs_Default"
    CodeFile="Default.aspx.cs" %>

<!DOCTYPE html>
<html>
<head id="Head1" runat="server" enableviewstate="false">
    <title>Insert Macro</title>
</head>
<frameset border="0" id="rowsFrameset" rows="<%=TabsFrameHeight %>, *">
    <frame name="insertMacroHeader" src="Header.aspx<%= CMS.Helpers.RequestContext.CurrentQueryString %>"
        scrolling="no" frameborder="0" noresize="noresize" />
    <frame name="insertContent" src="Tab_InsertMacroTree.aspx<%= CMS.Helpers.RequestContext.CurrentQueryString %>"
        frameborder="0" />
    <cms:NoFramesLiteral ID="ltlNoFrames" runat="server" />
</frameset>
</html>
