<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Newsletters_FormControls_NewsletterSubscriberSelector" CodeFile="NewsletterSubscriberSelector.ascx.cs" %>
<%@ Register src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" tagname="UniSelector" tagprefix="cms" %>

<cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
    <ContentTemplate>
        <cms:UniSelector ID="usSubscribers" runat="server" ObjectType="newsletter.subscriber"
            SelectionMode="SingleTextBox" AllowEditTextBox="false" DisplayNameFormat="{%SubscriberFullName%}, {%SubscriberEmail%}" />
    </ContentTemplate>
</cms:CMSUpdatePanel>
