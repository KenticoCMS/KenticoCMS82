<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ContactGroupSelector.ascx.cs" Inherits="CMSModules_ContactManagement_FormControls_ContactGroupSelector" %>
<%@ Register src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" tagname="UniSelector" tagprefix="cms" %>

<cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
    <ContentTemplate>
        <cms:UniSelector ID="uniSelector" runat="server" IsLiveSite="false" DisplayNameFormat="{%ContactGroupDisplayName%}"
            ObjectType="om.contactgroup" ResourcePrefix="om.contactgroup" ReturnColumnName="ContactGroupID" SelectionMode="MultipleButton" />
    </ContentTemplate>
</cms:CMSUpdatePanel>