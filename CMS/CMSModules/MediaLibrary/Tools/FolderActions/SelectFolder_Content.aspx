<%@ Page Language="C#" Theme="Default" AutoEventWireup="true"
    Inherits="CMSModules_MediaLibrary_Tools_FolderActions_SelectFolder_Content" EnableEventValidation="false" CodeFile="SelectFolder_Content.aspx.cs" %>

<%@ Register Src="~/CMSModules/MediaLibrary/Controls/MediaLibrary/SelectFolder.ascx"
    TagName="SelectFolder" TagPrefix="cms" %>
<!DOCTYPE html>
<html>
<head runat="server" enableviewstate="false">
    <title>Copy / Move</title>
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
    <cms:SelectFolder ID="selectFolder" runat="server" IsLiveSite="false" />
    </form>
</body>
</html>
