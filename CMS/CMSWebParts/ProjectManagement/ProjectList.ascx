<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSWebParts_ProjectManagement_ProjectList" CodeFile="~/CMSWebParts/ProjectManagement/ProjectList.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/System/PermissionMessage.ascx" TagName="PermissionMessage"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/ProjectManagement/Controls/LiveControls/ProjectList.ascx"
    TagPrefix="cms" TagName="ProjectList" %>
<cms:PermissionMessage ID="messageElem" runat="server" EnableViewState="false" />
<cms:UIContextPanel runat="server" ID="pnlUIContext">
    <cms:ProjectList runat="server" ID="ucProjectList" />
</cms:UIContextPanel>