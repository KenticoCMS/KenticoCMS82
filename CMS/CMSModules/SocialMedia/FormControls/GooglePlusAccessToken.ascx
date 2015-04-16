<%@ Control Language="C#" AutoEventWireup="true" CodeFile="GooglePlusAccessToken.ascx.cs" Inherits="CMSModules_SocialMedia_FormControls_GooglePlusAccessToken" %>

<cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
    <ContentTemplate>
        <cms:AlertLabel ID="lblError" runat="server" AlertType="Error" CssClass="hidden" />
        <div class="control-group-inline">
            <cms:CMSTextBox ID="txtToken" runat="server" />
            <cms:CMSTextBox ID="txtTokenSecret" runat="server" Style="display: none;" />
            <cms:LocalizedButton ID="btnSelect" runat="server" ButtonStyle="Default" ResourceString="socialnetworking.get" />
        </div>
    </ContentTemplate>
</cms:CMSUpdatePanel>
