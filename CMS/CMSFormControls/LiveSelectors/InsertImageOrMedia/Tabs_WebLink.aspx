<%@ Page Language="C#" AutoEventWireup="true" Theme="Default"
    Inherits="CMSFormControls_LiveSelectors_InsertImageOrMedia_Tabs_WebLink" EnableEventValidation="false" CodeFile="Tabs_WebLink.aspx.cs" %>

<%@ Register Src="~/CMSModules/Content/Controls/Dialogs/Web/WebLinkSelector.ascx"
    TagName="WebLinkSelector" TagPrefix="cms" %>
<!DOCTYPE html>
<html>
<head id="Head1" runat="server" enableviewstate="false">
    <title>Insert link - web</title>
    <style type="text/css">
        body
        {
            margin: 0px;
            padding: 0px;
            height: 100%;
        }
    </style>
</head>
<body class="<%=mBodyClass%>">
    <form id="form1" runat="server">
    <ajaxToolkit:ToolkitScriptManager ID="scriptManager" runat="server" />
    <div class="LiveSiteDialog">
        <cms:WebLinkSelector ID="webLinkSelector" runat="server" IsLiveSite="true" />
        <asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
    </div>
    </form>
</body>
</html>
