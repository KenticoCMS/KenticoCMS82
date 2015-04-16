<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_SocialMarketing_FormControls_LinkedInAutoPost" CodeFile="LinkedInAutoPost.ascx.cs" %>
<%@ Import Namespace="CMS.MacroEngine" %>

<cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
    <ContentTemplate>
        <div class="form-horizontal">
            <cms:AlertLabel runat="server" ID="lblInfo" CssClass="sm-post-state" />
            <cms:CMSCheckBox runat="server" ID="chkPostToLinkedIn" AutoPostBack="True" OnCheckedChanged="chkPostToLinkedIn_OnCheckedChanged" ResourceString="sm.linkedin.autopost.createpost" />
            <cms:CMSPanel runat="server" ID="pnlForm" Visible="False">
                <div class="sm-inner-control-label">
                    <cms:LocalizedLabel runat="server" AssociatedControlID="companySelector" ResourceString="sm.linkedin.posts.account" DisplayColon="True" />
                </div>
                <div class="control-group-inline-forced">
                    <cms:FormControl runat="server" ID="companySelector" FormControlName="Uni_selector">
                        <Properties>
                            <cms:Property Name="ObjectType" Value="sm.linkedinaccount" />
                            <cms:Property Name="ReturnColumnName" Value="LinkedInAccountID" />
                            <cms:Property Name="ObjectSiteName" Value="#currentsite" />
                            <cms:Property Name="SelectionMode" Value="SingleDropDownList" />
                        </Properties>
                    </cms:FormControl>
                    <span class="info-icon">
                        <cms:LocalizedLabel runat="server" ToolTipResourceString="sm.linkedin.autopost.profiletooltip" CssClass="sr-only"></cms:LocalizedLabel>
                        <i aria-hidden="true" class="icon-question-circle" title="<%=GetString("sm.linkedin.autopost.profiletooltip") %>"></i>
                    </span>
                </div>
                <div class="sm-inner-control-label">
                    <cms:LocalizedLabel runat="server" AssociatedControlID="txtPost" ResourceString="sm.linkedin.posts.content" DisplayColon="True" />
                </div>
                <div>
                    <cms:CMSTextArea runat="server" ID="txtPost" Rows="8"/>
                    <div class="sm-related-margin-top form-text">
                        <% = MacroResolver.Resolve(GetString("sm.linkedin.autopost.macrohint"))%>
                    </div>
                    <div class="sm-related-margin-top">
                        <div>
                            <cms:CMSCheckBox runat="server" ID="chkShortenUrls" ResourceString="sm.linkedin.autopost.shortenurls" OnCheckedChanged="chkShortenUrls_OnCheckedChanged" AutoPostBack="true" />
                        </div>
                        <div class="sm-related-margin-top">
                            <cms:FormControl runat="server" ID="urlShortenerSelector" FormControlName="AvailableURLShortenerSelector">
                                <Properties>
                                    <cms:Property Name="SocialNetworkName" Value="LinkedIn" />
                                </Properties>
                            </cms:FormControl>
                        </div>
                    </div>
                </div>
                <div class="sm-inner-control-label">
                    <cms:LocalizedLabel runat="server" AssociatedControlID="publishDateTime" ResourceString="sm.linkedin.posts.scheduledpublish" DisplayColon="True" />
                </div>
                <div>
                    <div>
                        <cms:FormControl runat="server" ID="publishDateTime" FormControlName="CalendarControl" />
                    </div>
                    <div class="sm-related-margin-top">
                        <cms:CMSCheckBox runat="server" ID="chkPostAfterDocumentPublish" ResourceString="sm.linkedin.autopost.postondocumentpublish" AutoPostBack="true" OnCheckedChanged="chkPostAfterDocumentPublish_OnCheckedChanged" />
                    </div>
                </div>
                <div class="sm-inner-control-label">
                    <cms:LocalizedLabel runat="server" AssociatedControlID="campaingSelector" ResourceString="sm.linkedin.posts.campaign" DisplayColon="True" />
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
