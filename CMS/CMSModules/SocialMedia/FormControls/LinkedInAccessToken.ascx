<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_SocialMedia_FormControls_LinkedInAccessToken" CodeFile="LinkedInAccessToken.ascx.cs" %>

<cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
    <ContentTemplate>
        <cms:AlertLabel ID="lblError" runat="server" AlertType="Error" CssClass="hidden" />
        <div class="control-group-inline">
            <cms:CMSTextBox ID="txtToken" runat="server" />
            <cms:LocalizedButton ID="btnSelect" runat="server" ButtonStyle="Default" ResourceString="socialnetworking.get"/>
        </div>
    </ContentTemplate>
</cms:CMSUpdatePanel>
