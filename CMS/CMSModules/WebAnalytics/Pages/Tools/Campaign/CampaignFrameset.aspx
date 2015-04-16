<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CampaignFrameset.aspx.cs" Inherits="CMSModules_WebAnalytics_Pages_Tools_Campaign_CampaignFrameset" %>

<!DOCTYPE html>
<html>
<head id="Head1" runat="server" enableviewstate="false">
    <title>Campaign properties</title>
</head>
<frameset border="0" rows="<%=TabsOnlyHeight%>, *" id="rowsFrameset">
    <frame name="header"  scrolling="no" ID="frmHeader" runat="server"
        frameborder="0" noresize="noresize" />
    <frame name="content"  frameborder="0" runat="server"  ID="frmContent" />
    <cms:NoFramesLiteral ID="ltlNoFrames" runat="server" />
</frameset>
</html>