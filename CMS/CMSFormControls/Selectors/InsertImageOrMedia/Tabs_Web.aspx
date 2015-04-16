<%@ Page Language="C#" AutoEventWireup="true" Theme="Default"
    Inherits="CMSFormControls_Selectors_InsertImageOrMedia_Tabs_Web" EnableEventValidation="false" CodeFile="Tabs_Web.aspx.cs" %>

<%@ Register Src="~/CMSModules/Content/Controls/Dialogs/Web/WebContentSelector.ascx" TagName="WebContentSelector"
    TagPrefix="cms" %>
<!DOCTYPE html>
<html>
<head id="Head1" runat="server" enableviewstate="false">
    <title>Insert image or media - web</title>
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
    <ajaxToolkit:ToolkitScriptManager ID="scriptManager" runat="server">
    </ajaxToolkit:ToolkitScriptManager>
    <cms:WebContentSelector ID="webContentSelector" runat="server" IsLiveSite="false" />
    <asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
    </form>
</body>
</html>
