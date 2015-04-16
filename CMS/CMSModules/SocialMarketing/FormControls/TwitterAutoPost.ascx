<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_SocialMarketing_FormControls_TwitterAutoPost" CodeFile="TwitterAutoPost.ascx.cs" %>
<%@ Import Namespace="CMS.MacroEngine" %>

<cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
    <ContentTemplate>
        <div class="form-horizontal">
            <cms:AlertLabel runat="server" ID="lblInfo" CssClass="sm-post-state" />
            <cms:CMSCheckBox runat="server" ID="chkPostToTwitter" AutoPostBack="True" OnCheckedChanged="chkPostToTwitter_OnCheckedChanged" ResourceString="sm.twitter.autopost.createpost" />
            <cms:CMSPanel runat="server" ID="pnlForm" Visible="False">
                <div class="sm-inner-control-label">
                    <cms:LocalizedLabel runat="server" AssociatedControlID="channelSelector" ResourceString="sm.twitter.posts.account" DisplayColon="True" />
                </div>
                <div class="control-group-inline-forced">
                    <cms:FormControl runat="server" ID="channelSelector" FormControlName="Uni_selector">
                        <Properties>
                            <cms:Property Name="ObjectType" Value="sm.twitteraccount" />
                            <cms:Property Name="ReturnColumnName" Value="TwitterAccountID" />
                            <cms:Property Name="ObjectSiteName" Value="#currentsite" />
                            <cms:Property Name="SelectionMode" Value="SingleDropDownList" />
                        </Properties>
                    </cms:FormControl>
                    <span class="info-icon">
                        <cms:LocalizedLabel runat="server" ToolTipResourceString="sm.twitter.autopost.pagetooltip" CssClass="sr-only"></cms:LocalizedLabel>
                        <i aria-hidden="true" class="icon-question-circle" title="<%=GetString("sm.twitter.autopost.pagetooltip") %>"></i>
                    </span>
                </div>
                <div class="sm-inner-control-label">
                    <cms:LocalizedLabel runat="server" AssociatedControlID="tweetContent" ResourceString="sm.twitter.posts.content" DisplayColon="True" />
                </div>
                <div>
                    <cms:FormControl runat="server" ID="tweetContent" FormControlName="SMTwitterPostTextArea" />
                    <div class="sm-related-margin-top form-text">
                        <% = MacroResolver.Resolve(GetString("sm.twitter.autopost.macrohint"))%>
                    </div>
                    <div class="sm-related-margin-top">
                        <div>
                            <cms:CMSCheckBox runat="server" ID="chkShortenUrls" ResourceString="sm.twitter.autopost.shortenurls" OnCheckedChanged="chkShortenUrls_OnCheckedChanged" AutoPostBack="true" />
                        </div>
                        <div class="sm-related-margin-top control-group-inline-forced">
                            <cms:FormControl runat="server" ID="urlShortenerSelector" FormControlName="AvailableURLShortenerSelector" CssClass="control-group-inline">
                                <Properties>
                                    <cms:Property Name="SocialNetworkName" Value="Twitter" />
                                </Properties>
                            </cms:FormControl>
                            <span class="info-icon">
                                <cms:LocalizedLabel runat="server" ToolTipResourceString="sm.twitter.autopost.urlshortenertooltip" CssClass="sr-only"></cms:LocalizedLabel>
                                <i aria-hidden="true" class="icon-question-circle" title="<%=GetString("sm.twitter.autopost.urlshortenertooltip") %>"></i>
                            </span>
                        </div>
                    </div>
                </div>
                <div class="sm-inner-control-label">
                    <cms:LocalizedLabel runat="server" AssociatedControlID="publishDateTime" ResourceString="sm.twitter.posts.scheduledpublish" DisplayColon="True" />
                </div>
                <div>
                    <div>
                        <cms:FormControl runat="server" ID="publishDateTime" FormControlName="CalendarControl" />
                    </div>
                    <div class="sm-related-margin-top">
                        <cms:CMSCheckBox runat="server" ID="chkPostAfterDocumentPublish" ResourceString="sm.twitter.autopost.postondocumentpublish" AutoPostBack="true" OnCheckedChanged="chkPostAfterDocumentPublish_OnCheckedChanged" />
                    </div>
                </div>
                <div class="sm-inner-control-label">
                    <cms:LocalizedLabel runat="server" AssociatedControlID="campaingSelector" ResourceString="sm.twitter.posts.campaign" DisplayColon="True" />
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
