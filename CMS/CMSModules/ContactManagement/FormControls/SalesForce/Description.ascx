<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Description.ascx.cs" Inherits="CMSModules_ContactManagement_FormControls_SalesForce_Description" %>
<%@ Register Src="~/CMSAdminControls/UI/Macros/MacroEditor.ascx" TagPrefix="cms" TagName="MacroEditor" %>
<cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
    <ContentTemplate>
        <cms:MacroEditor runat="server" ID="DescriptionMacroEditor" />
    </ContentTemplate>
</cms:CMSUpdatePanel>
