<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Newsletters_FormControls_NewsletterTemplateSelector" CodeFile="NewsletterTemplateSelector.ascx.cs" %>
<%@ Register src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" tagname="UniSelector" tagprefix="cms" %>

<cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
    <ContentTemplate>
        <cms:UniSelector ID="uniNewsletterTemplate" runat="server" AllowAll="false" AllowEmpty="false"
            ObjectType="Newsletter.EmailTemplate" SelectionMode="SingleDropDownList" />
    </ContentTemplate>
</cms:CMSUpdatePanel>
