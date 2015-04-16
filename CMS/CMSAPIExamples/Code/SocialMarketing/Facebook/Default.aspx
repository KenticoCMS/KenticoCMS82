<%@ Page Title="" Language="C#" MasterPageFile="~/CMSAPIExamples/Pages/APIExamplesPage.Master"
    Theme="Default" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="CMSAPIExamples_Code_SocialMarketing_Facebook_Default" %>

<%@ Register Src="~/CMSAPIExamples/Controls/APIExample.ascx" TagName="APIExample" TagPrefix="cms" %>
<asp:Content ID="contentLeft" ContentPlaceHolderID="plcLeftContainer" runat="server">
    <%-- Facebook application --%>
    <cms:LocalizedHeading ID="headCreateFacebookApplication" runat="server" Text="Facebook app" Level="4" EnableViewState="false" />
    <div class="form-horizontal">
         <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel ResourceString="sm.facebook.apps.appid" DisplayColon="True" ShowRequiredMark="True" runat="server" ID="lblApiKey" CssClass="control-label" ></cms:LocalizedLabel>
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox runat="server" ID="txtApiKey" /></div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel ResourceString="sm.facebook.apps.appsecret" DisplayColon="True" ShowRequiredMark="True" runat="server" ID="lblApiSecret" CssClass="control-label" ></cms:LocalizedLabel></div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox runat="server" ID="txtApiSecret" /></div>
        </div>
    </div>
    <cms:APIExample ID="apiCreateFacebookApplication" runat="server" ButtonText="Create Facebook app" InfoMessage="Application 'My new application' was created." />
    <cms:APIExample ID="apiGetAndUpdateFacebookApplication" runat="server" ButtonText="Get and update Facebook app" APIExampleType="ManageAdditional" InfoMessage="Application 'My new application' was updated." ErrorMessage="Application 'My new application' was not found." />
    <%-- Facebook account --%>
    <cms:LocalizedHeading ID="headCreateFacebookPage" runat="server" Text="Facebook page" Level="4" EnableViewState="false" />
   <div class="form-horizontal">
         <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel ResourceString="sm.facebook.accounts.pageid" DisplayColon="True" ShowRequiredMark="True" runat="server" ID="lblPageUrl" CssClass="control-label" ></cms:LocalizedLabel> </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox runat="server" ID="txtPageUrl" /></div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel ResourceString="settingskey.facebookaccesstoken" DisplayColon="True" ShowRequiredMark="True" runat="server" ID="lblPageAccess" CssClass="control-label" ></cms:LocalizedLabel></div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox runat="server" ID="txtFacebookPageAccessToken" />
            </div>
        </div>
    </div>
    <cms:APIExample ID="apiCreateFacebookPage" runat="server" ButtonText="Create Facebook page" InfoMessage="Page 'My new page' was created." />
    <cms:APIExample ID="apiGetAndUpdateFacebookPage" runat="server" ButtonText="Get and update Facebook page" APIExampleType="ManageAdditional" InfoMessage="Page 'My new page' was updated." ErrorMessage="Page 'My new page' was not found." />
    <%-- Facebook post --%>
    <cms:LocalizedHeading ID="headCreateFacebookPost" runat="server" Text="Facebook post" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreateFacebookPost" runat="server" ButtonText="Create Facebook post" InfoMessage="Post 'My new post' was created." />
    <cms:APIExample ID="apiGetAndUpdateFacebookPost" runat="server" ButtonText="Get and update Facebook post" APIExampleType="ManageAdditional" InfoMessage="A post was updated." ErrorMessage="No post was not found." />
    <cms:APIExample ID="apiPublishPostToFacebook" runat="server" ButtonText="Publish post to Facebook" APIExampleType="ManageAdditional" InfoMessage="A post was posted." ErrorMessage="No post was not found." />
</asp:Content>
<asp:Content ID="contentRight" ContentPlaceHolderID="plcRightContainer" runat="server">
    <%-- Facebook post --%>
    <cms:LocalizedHeading ID="headDeleteFacebookPost" runat="server" Text="Facebook post" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteFacebookPosts" runat="server" ButtonText="Delete Facebook posts" APIExampleType="CleanUpMain" InfoMessage="Posts were deleted." ErrorMessage="No post was not found." />
    <%-- Facebook account --%>
    <cms:LocalizedHeading ID="headDeleteFacebookPage" runat="server" Text="Facebook page" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteFacebookPage" runat="server" ButtonText="Delete Facebook page" APIExampleType="CleanUpMain" InfoMessage="Page 'My new page' and all its dependencies were deleted." ErrorMessage="Page 'My new page' was not found." />
    <%-- Facebook application --%>
    <cms:LocalizedHeading ID="headDeleteFacebookApplication" runat="server" Text="Facebook app" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteFacebookApplication" runat="server" ButtonText="Delete Facebook app" APIExampleType="CleanUpMain" InfoMessage="Application 'My new application' and all its dependencies were deleted." ErrorMessage="Application 'My new application' was not found." />
</asp:Content>
