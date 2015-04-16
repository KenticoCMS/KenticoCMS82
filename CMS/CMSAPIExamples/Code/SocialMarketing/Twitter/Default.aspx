<%@ Page Title="Twitter" Language="C#" MasterPageFile="~/CMSAPIExamples/Pages/APIExamplesPage.Master"
    Theme="Default" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="CMSAPIExamples_Code_SocialMarketing_Twitter_Default" %>

<%@ Register Src="~/CMSAPIExamples/Controls/APIExample.ascx" TagName="APIExample" TagPrefix="cms" %>
<asp:Content ID="contentLeft" ContentPlaceHolderID="plcLeftContainer" runat="server">
    <cms:LocalizedHeading ID="headTwitterApp" runat="server" Text="Twitter app" Level="4" EnableViewState="false" />
    <div class="form-horizontal">
         <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel ResourceString="sm.twitter.apps.consumerkey" DisplayColon="True" ShowRequiredMark="True" runat="server" ID="lblConsumerKey" CssClass="control-label" ></cms:LocalizedLabel></div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox runat="server" ID="txtConsumerKey" /></div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel ResourceString="sm.twitter.apps.consumersecret" DisplayColon="True" ShowRequiredMark="True" runat="server" ID="lblConsumerSecret" CssClass="control-label" ></cms:LocalizedLabel></div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox runat="server" ID="txtConsumerSecret" /></div>
        </div>
    </div>
    <cms:APIExample ID="apiCreateTwitterApp" runat="server" ButtonText="Create Twitter app" InfoMessage="App 'My new Twitter app' has been created." />
    <cms:APIExample ID="apiGetAndUpdateTwitterApp" runat="server" ButtonText="Get and update Twitter app" APIExampleType="ManageAdditional" InfoMessage="App 'My new twitter app' has been modified." />
    <cms:LocalizedHeading ID="headTwitterChannel" runat="server" Text="Twitter channel" Level="4" EnableViewState="false" />
    <div class="form-horizontal">
         <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel ResourceString="sm.twitter.accounts.accesstoken" DisplayColon="True" ShowRequiredMark="True" runat="server" ID="lblAccessToken" CssClass="control-label" ></cms:LocalizedLabel></div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox runat="server" ID="txtAccessToken" /></div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel ResourceString="sm.twitter.accounts.accesstokensecret" DisplayColon="True" ShowRequiredMark="True" runat="server" ID="lblAccessTokenSecret" CssClass="control-label" ></cms:LocalizedLabel></div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox runat="server" ID="txtAccessTokenSecret" /></div>
        </div>
    </div>
    <cms:APIExample ID="apiCreateTwitterChannel" runat="server" ButtonText="Create Twitter channel" InfoMessage="Channel 'My new Twitter channel' has been created." />
    <cms:APIExample ID="apiGetAndUpdateTwitterChannel" runat="server" APIExampleType="ManageAdditional" ButtonText="Get and update Twitter channel" InfoMessage="Channel 'My new Twitter channel' has been created." />
    <cms:LocalizedHeading ID="headTwitterPost" runat="server" Text="Tweet" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreateTwitterPost" runat="server" ButtonText="Create Tweet" InfoMessage="Tweet has been created." />
    <cms:APIExample ID="apiGetAndUpdateTwitterPost" runat="server" APIExampleType="ManageAdditional" ButtonText="Get and update Tweet" InfoMessage="Tweet has been modified." />
    <cms:APIExample ID="apiPublishPostToTwitter" runat="server" APIExampleType="ManageAdditional" ButtonText="Publish post to Twitter" InfoMessage="Tweet has been published." />
</asp:Content>
<asp:Content ID="contentRight" ContentPlaceHolderID="plcRightContainer" runat="server">
    <cms:LocalizedHeading ID="headTwitterPostCleanup" runat="server" Text="Tweet" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteTwitterPosts" runat="server" ButtonText="Delete Tweets" InfoMessage="Tweets have been deleted from both CMS and twitter." />
    <cms:LocalizedHeading ID="headTwitterChannelCleanup" runat="server" Text="Twitter channel" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteTwitterChannel" runat="server" ButtonText="Delete Twitter channel" InfoMessage="Channel 'My new Twitter channel' has been deleted from CMS." />
    <cms:LocalizedHeading ID="headTwitterAppCleanup" runat="server" Text="Twitter app" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteTwitterApp" runat="server" ButtonText="Delete Twitter app" InfoMessage="app 'My new twitter app' has been deleted from CMS." />
</asp:Content>
