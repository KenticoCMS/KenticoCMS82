<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Score.ascx.cs" Inherits="CMSModules_ContactManagement_FormControls_SalesForce_Score" %>
<%@ Register TagPrefix="cms" TagName="SalesForceError" Src="~/CMSModules/ContactManagement/Controls/UI/SalesForce/Error.ascx" %>
<%@ Register TagPrefix="cms" TagName="UniSelector" Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" %>
<cms:CMSUpdatePanel ID="MainUpdatePanel" runat="server">
<ContentTemplate>
    <cms:SalesForceError ID="SalesForceError" runat="server" EnableViewState="false" />
    <p id="MessageLabel" runat="server" enableviewstate="false" visible="false"></p>
    <cms:UniSelector ID="ScoreSelector" runat="server" ObjectType="om.score" SelectionMode="SingleDropDownList" AllowEmpty="true" ResourcePrefix="sf.score" Visible="false"></cms:UniSelector>
</ContentTemplate>
</cms:CMSUpdatePanel>