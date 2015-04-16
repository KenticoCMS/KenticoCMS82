<%@ Page Language="C#" AutoEventWireup="true" CodeFile="DetailFooter.aspx.cs" Inherits="CMSModules_ContactManagement_Pages_Tools_DetailFooter"
    Theme="Default" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Detail footer</title>
    <style type="text/css">
        body
        {
            margin: 0px;
            padding: 0px;
            height: 100%;
            background-color: #f5f3ec;
        }
    </style>
</head>
<body class="<%=mBodyClass%> Buttons">
    <form id="form1" runat="server">
    <asp:Panel runat="server" ID="pnlScroll" CssClass="ButtonPanel">
        <div class="FloatRight">
            <cms:LocalizedButton ID="btnClose" runat="server" ButtonStyle="Primary" EnableViewState="false"
                OnClientClick="parent.CloseDialog(); if (parent.wopener != null && parent.wopener.Refresh != null) { parent.wopener.Refresh(); } return false;"
                ResourceString="general.close" />
        </div>
    </asp:Panel>
    </form>
</body>
</html>
