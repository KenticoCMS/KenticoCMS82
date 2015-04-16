<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_SocialMarketing_FormControls_FacebookAutoPost" CodeFile="FacebookAutoPost.ascx.cs" %>
<%@ Import Namespace="CMS.MacroEngine" %>

<cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
    <ContentTemplate>
        <div class="form-horizontal">
            <cms:AlertLabel runat="server" ID="lblInfo" CssClass="sm-post-state" />
            <cms:CMSCheckBox runat="server" ID="chkPostToFacebook" AutoPostBack="True" OnCheckedChanged="chkPostToFacebook_OnCheckedChanged" ResourceString="sm.facebook.autopost.createpost" />
            <cms:CMSPanel runat="server" ID="pnlForm" Visible="False">
                <div class="sm-inner-control-label">
                    <cms:LocalizedLabel runat="server" AssociatedControlID="pageSelector" ResourceString="sm.facebook.posts.account" DisplayColon="True" />
                </div>
                <div class="control-group-inline-forced">
                    <cms:FormControl runat="server" ID="pageSelector" FormControlName="Uni_selector">
                        <Properties>
                            <cms:Property Name="ObjectType" Value="sm.facebookaccount" />
                            <cms:Property Name="ReturnColumnName" Value="FacebookAccountID" />
                            <cms:Property Name="ObjectSiteName" Value="#currentsite" />
                            <cms:Property Name="SelectionMode" Value="SingleDropDownList" />
                        </Properties>
                    </cms:FormControl>
                    <span class="info-icon">
                        <cms:LocalizedLabel runat="server" ToolTipResourceString="sm.facebook.autopost.pagetooltip" CssClass="sr-only"></cms:LocalizedLabel>
                        <i aria-hidden="true" class="icon-question-circle" title="<%=GetString("sm.facebook.autopost.pagetooltip") %>"></i>
                    </span>
                </div>
                <div class="sm-inner-control-label">
                    <cms:LocalizedLabel runat="server" AssociatedControlID="txtPost" ResourceString="sm.facebook.posts.content" DisplayColon="True" />
                </div>
                <div>
                    <cms:CMSTextArea runat="server" ID="txtPost" Rows="8" />
                    <div class="sm-related-margin-top form-text">
                        <% = MacroResolver.Resolve(GetString("sm.facebook.autopost.macrohint"))%>
                    </div>
                    <div class="sm-related-margin-top">
                        <div>
                            <cms:CMSCheckBox runat="server" ID="chkShortenUrls" ResourceString="sm.facebook.autopost.shortenurls" OnCheckedChanged="chkShortenUrls_OnCheckedChanged" AutoPostBack="true" />
                        </div>
                        <div class="sm-related-margin-top">
                            <cms:FormControl runat="server" ID="urlShortenerSelector" FormControlName="AvailableURLShortenerSelector">
                                <Properties>
                                    <cms:Property Name="SocialNetworkName" Value="Facebook" />
                                </Properties>
                            </cms:FormControl>
                        </div>
                    </div>
                </div>
                <div class="sm-inner-control-label">
                    <cms:LocalizedLabel runat="server" AssociatedControlID="publishDateTime" ResourceString="sm.facebook.posts.scheduledpublish" DisplayColon="True" />
                </div>
                <div>
                    <div>
                        <cms:FormControl runat="server" ID="publishDateTime" FormControlName="CalendarControl" />
                    </div>
                    <div class="sm-related-margin-top">
                        <cms:CMSCheckBox runat="server" ID="chkPostAfterDocumentPublish" ResourceString="sm.facebook.autopost.postondocumentpublish" AutoPostBack="true" OnCheckedChanged="chkPostAfterDocumentPublish_OnCheckedChanged" />
                    </div>
                </div>
                <div class="sm-inner-control-label">
                    <cms:LocalizedLabel runat="server" AssociatedControlID="campaingSelector" ResourceString="sm.facebook.posts.campaign" DisplayColon="True" />
                </div>
                <div>
                    <cms:FormControl runat="server" ID="campaingSelector" FormControlName="Uni_selector">
                        <Properties>
                            <cms:Property Name="ObjectType" Value="Analytics.Campaign" />
                            <cms:Property Name="ReturnColumnName" Value="CampaignID" />
                            <cms:Property Name="ObjectSiteName" Value="#currentsite" />
                            <cms:Property Name="SelectionMode" Value="SingleDropDownList" />
                            <cms:Property Name="AllowEmpty" Value="True" />
                            <cms:Property Name="OrderBy" Value="CampaignDisplayName" />
                        </Properties>
                    </cms:FormControl>
                </div>
            </cms:CMSPanel>
        </div>
    </ContentTemplate>
</cms:CMSUpdatePanel>
