<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Newsletters_Tools_Newsletters_Newsletter_Configuration"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Tools - Newsletter configuration"
    CodeFile="Newsletter_Configuration.aspx.cs" %>

<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Newsletters/FormControls/NewsletterTemplateSelector.ascx"
    TagPrefix="cms" TagName="NewsletterTemplateSelector" %>
<%@ Register Src="~/CMSAdminControls/UI/Selectors/ScheduleInterval.ascx" TagPrefix="cms"
    TagName="ScheduleInterval" %>
<%@ Register Src="~/CMSFormControls/System/UrlChecker.ascx" TagPrefix="cms" TagName="UrlChecker" %>
<%@ Register Src="~/CMSFormControls/System/CodeName.ascx" TagName="CodeName" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/Basic/DisabledModuleInfo.ascx" TagPrefix="cms" TagName="DisabledModule" %>

<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <%-- General config --%>
    <cms:LocalizedHeading runat="server" Level="4" ResourceString="general.general" />
    <asp:Panel ID="pnlGeneral" runat="server">
        <div class="form-horizontal">
            <%-- Display name --%>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblNewsletterDisplayName" EnableViewState="false"
                        ResourceString="Newsletter_Edit.NewsletterDisplayNameLabel" DisplayColon="true"
                        AssociatedControlID="txtNewsletterDisplayName" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:LocalizableTextBox ID="txtNewsletterDisplayName" runat="server"
                        MaxLength="250" />
                    <cms:CMSRequiredFieldValidator ID="rfvNewsletterDisplayName" runat="server" ControlToValidate="txtNewsletterDisplayName:cntrlContainer:textbox"
                        Display="dynamic" EnableViewState="false" />
                </div>
            </div>
            <%-- Code name --%>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblNewsletterName" EnableViewState="false"
                        ResourceString="Newsletter_Edit.NewsletterNameLabel" DisplayColon="true" AssociatedControlID="txtNewsletterName" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CodeName ID="txtNewsletterName" runat="server" MaxLength="250" />
                    <cms:CMSRequiredFieldValidator ID="rfvNewsletterName" runat="server" ControlToValidate="txtNewsletterName"
                        Display="dynamic" EnableViewState="false" />
                </div>
            </div>
            <%-- Subscription template --%>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblSubscriptionTemplate" EnableViewState="false"
                        ResourceString="Newsletter_Edit.SubscriptionTemplate" DisplayColon="true" AssociatedControlID="subscriptionTemplate" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:NewsletterTemplateSelector ID="subscriptionTemplate" runat="server" />
                </div>
            </div>
            <%-- Unsubscription template --%>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblUnsubscriptionTemplate" EnableViewState="false"
                        ResourceString="Newsletter_Edit.UnsubscriptionTemplate" DisplayColon="true" AssociatedControlID="unsubscriptionTemplate" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:NewsletterTemplateSelector ID="unsubscriptionTemplate" runat="server" />
                </div>
            </div>
            <%-- Sender name --%>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblNewsletterSenderName" EnableViewState="false"
                        ResourceString="Newsletter_Edit.NewsletterSenderNameLabel" DisplayColon="true"
                        AssociatedControlID="txtNewsletterSenderName" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSTextBox ID="txtNewsletterSenderName" runat="server" MaxLength="200" />
                    <cms:CMSRequiredFieldValidator ID="rfvNewsletterSenderName" runat="server" ErrorMessage=""
                        ControlToValidate="txtNewsletterSenderName" EnableViewState="false" />
                </div>
            </div>
            <%-- Sender email --%>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblNewsletterSenderEmail" EnableViewState="false"
                        ResourceString="Newsletter_Edit.NewsletterSenderEmailLabel" DisplayColon="true"
                        AssociatedControlID="txtNewsletterSenderEmail" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSTextBox ID="txtNewsletterSenderEmail" runat="server" MaxLength="200" />
                    <cms:CMSRequiredFieldValidator ID="rfvNewsletterSenderEmail" runat="server" ErrorMessage=""
                        ControlToValidate="txtNewsletterSenderEmail" EnableViewState="false" />
                </div>
            </div>
            <%-- Base URL --%>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblNewsletterBaseUrl" EnableViewState="false"
                        ResourceString="Newsletter_Configuration.NewsletterBaseUrl" DisplayColon="true"
                        AssociatedControlID="txtNewsletterBaseUrl" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSTextBox ID="txtNewsletterBaseUrl" runat="server"
                        MaxLength="500" />
                </div>
            </div>
            <%-- Unsubscription URL --%>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblNewsletterUnsubscribeUrl" EnableViewState="false"
                        ResourceString="Newsletter_Configuration.NewsletterUnsubscribeUrl" DisplayColon="true"
                        AssociatedControlID="txtNewsletterUnsubscribeUrl" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSTextBox ID="txtNewsletterUnsubscribeUrl" runat="server"
                        MaxLength="1000" />
                </div>
            </div>
            <%-- Draft emails --%>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblDraftEmails" runat="server" EnableViewState="false" ResourceString="newsletter.draftemails"
                        DisplayColon="true" AssociatedControlID="txtDraftEmails" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSTextBox ID="txtDraftEmails" runat="server" MaxLength="450" />
                </div>
            </div>
            <%-- Use email queue --%>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblUseEmailQueue" runat="server" EnableViewState="false"
                        ResourceString="newsletter.useemailqueue" DisplayColon="true" AssociatedControlID="chkUseEmailQueue" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSCheckBox ID="chkUseEmailQueue" runat="server" />
                </div>
            </div>
            <%-- Enable resending --%>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblEnableResending" runat="server" EnableViewState="false"
                        ResourceString="newsletter.enableresending" DisplayColon="true" AssociatedControlID="chkEnableResending" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSCheckBox ID="chkEnableResending" runat="server" />
                </div>
            </div>
        </div>
    </asp:Panel>
    <%-- Template based config --%>
    <asp:PlaceHolder ID="plcTemplate" runat="server">
        <cms:LocalizedHeading runat="server" Level="4" ResourceString="newsletter_configuration.templatebased" />
        <asp:Panel ID="pnlTemplate" runat="server">
            <div class="form-horizontal">
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblIssueTemplate" EnableViewState="false"
                            ResourceString="Newsletter_Edit.NewsletterTemplate" DisplayColon="true" AssociatedControlID="issueTemplate" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:NewsletterTemplateSelector ID="issueTemplate" runat="server" />
                    </div>
                </div>
            </div>
        </asp:Panel>
    </asp:PlaceHolder>
    <%-- Dynamic config --%>
    <asp:PlaceHolder ID="plcDynamic" runat="server">
        <cms:LocalizedHeading runat="server" Level="4" ResourceString="newsletter_configuration.dynamic" />
        <asp:Panel ID="pnlDynamic" runat="server">
            <div class="form-horizontal">
                <%-- Subject --%>
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="lblSubject" runat="server" EnableViewState="false" ResourceString="general.subject"
                            DisplayColon="true" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSUpdatePanel ID="pnlUpSubject" runat="server" UpdateMode="Conditional">
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="radPageTitle" />
                                <asp:AsyncPostBackTrigger ControlID="radFollowing" />
                            </Triggers>
                            <ContentTemplate>
                                <div class="radio-list-vertical">
                                    <cms:CMSRadioButton ID="radPageTitle" runat="server" GroupName="Subject" ResourceString="Newsletter_Configuration.PageTitleSubject"
                                        AutoPostBack="True" OnCheckedChanged="radSubject_CheckedChanged" />
                                    <cms:CMSRadioButton ID="radFollowing" runat="server" GroupName="Subject" ResourceString="Newsletter_Configuration.PageTitleFollowing"
                                        AutoPostBack="True" OnCheckedChanged="radSubject_CheckedChanged" />
                                    <div class="selector-subitem">
                                        <cms:LocalizableTextBox ID="txtSubject" runat="server" MaxLength="100" />
                                    </div>
                                </div>
                            </ContentTemplate>
                        </cms:CMSUpdatePanel>
                    </div>
                </div>
                <%-- Dynamic newsletter URL --%>
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblNewsletterDynamicURL" EnableViewState="false"
                            ResourceString="Newsletter_Edit.SourcePageURL" DisplayColon="true" AssociatedControlID="txtNewsletterDynamicURL" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:UrlChecker runat="server" ID="txtNewsletterDynamicURL" ResourcePrefix="newsletter" />
                        <cms:CMSRequiredFieldValidator ID="rfvNewsletterDynamicURL" runat="server" ControlToValidate="txtNewsletterDynamicURL:txtDomain" Display="Dynamic" />
                    </div>
                </div>
                <%-- Scheduler --%>
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblSchedule" EnableViewState="false" ResourceString="Newsletter_Edit.Schedule"
                            DisplayColon="true" AssociatedControlID="chkSchedule" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSCheckBox ID="chkSchedule" runat="server" Checked="true" AutoPostBack="true"
                            OnCheckedChanged="chkSchedule_CheckedChanged" />
                    </div>
                </div>
                <cms:CMSUpdatePanel ID="pnlUpScheduler" runat="server" UpdateMode="Conditional">
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="chkSchedule" />
                    </Triggers>
                    <ContentTemplate>
                        <cms:ScheduleInterval ID="schedulerInterval" runat="server" />
                    </ContentTemplate>
                </cms:CMSUpdatePanel>
            </div>
        </asp:Panel>
    </asp:PlaceHolder>
    <%-- Online marketing config --%>
    <asp:PlaceHolder ID="plcTracking" runat="server">
        <cms:LocalizedHeading runat="server" Level="4" ResourceString="onlinemarketing.general" />
        <asp:Panel ID="pnlOM" runat="server">
            <div class="form-horizontal">
                <%-- Track opened emails --%>
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="lblTrackOpenedEmails" runat="server" EnableViewState="false"
                            ResourceString="newsletter.trackopenedemails" DisplayColon="true" AssociatedControlID="chkTrackOpenedEmails" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSCheckBox ID="chkTrackOpenedEmails" runat="server" />
                    </div>
                </div>
                <%-- Track clicked links --%>
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="lblTrackClickedLinks" runat="server" EnableViewState="false"
                            ResourceString="newsletter.trackclickedlinks" DisplayColon="true" AssociatedControlID="chkTrackClickedLinks" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSCheckBox ID="chkTrackClickedLinks" runat="server" />
                    </div>
                </div>
                <asp:PlaceHolder ID="plcOM" runat="server">
                    <%-- Log activities --%>
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblLogActivity" runat="server" EnableViewState="false" ResourceString="newsletter.trackactivities"
                                DisplayColon="true" AssociatedControlID="chkLogActivity" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:CMSCheckBox ID="chkLogActivity" runat="server" />
                        </div>
                    </div>
                </asp:PlaceHolder>
            </div>
        </asp:Panel>
    </asp:PlaceHolder>
    <%-- Double opt-in config --%>
    <cms:LocalizedHeading runat="server" Level="4" ResourceString="newsletter_configuration.optin" />
    <asp:Panel ID="pnlDoubleOptIn" runat="server">
        <div class="form-horizontal">
            <%-- Enable double opt-in --%>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblEnableOptIn" runat="server" EnableViewState="false" ResourceString="newsletter_configuration.enableoptin"
                        DisplayColon="true" AssociatedControlID="chkEnableOptIn" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSCheckBox ID="chkEnableOptIn" runat="server" AutoPostBack="true" OnCheckedChanged="chkEnableOptIn_CheckedChanged" />
                </div>
            </div>
        </div>
        <cms:CMSUpdatePanel ID="pnlUpOptIn" runat="server" UpdateMode="Conditional">
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="chkEnableOptIn" />
            </Triggers>
            <ContentTemplate>
                <asp:PlaceHolder ID="plcOptIn" runat="server" Visible="false">
                    <div class="form-horizontal">
                        <%-- Opt-in template --%>
                        <div class="form-group">
                            <div class="editing-form-label-cell">
                                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblOptInTemplate" EnableViewState="false"
                                    ResourceString="newsletter_configuration.optnintemplate" DisplayColon="true"
                                    AssociatedControlID="optInSelector" />
                            </div>
                            <div class="editing-form-value-cell">
                                <cms:NewsletterTemplateSelector ID="optInSelector" runat="server" />
                            </div>
                        </div>
                        <%-- Approval URL --%>
                        <div class="form-group">
                            <div class="editing-form-label-cell">
                                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblOptInURL" EnableViewState="false" ResourceString="newsletter_configuration.optinurl"
                                    DisplayColon="true" AssociatedControlID="txtOptInURL" />
                            </div>
                            <div class="editing-form-value-cell">
                                <cms:CMSTextBox ID="txtOptInURL" runat="server" MaxLength="450" />
                            </div>
                        </div>
                        <%-- Send confirmation --%>
                        <div class="form-group">
                            <div class="editing-form-label-cell">
                                <cms:LocalizedLabel CssClass="control-label" ID="lblSendOptInConfirmation" runat="server" EnableViewState="false"
                                    ResourceString="newsletter_configuration.sendoptinconfirmation" DisplayColon="true"
                                    AssociatedControlID="chkSendOptInConfirmation" />
                            </div>
                            <div class="editing-form-value-cell">
                                <cms:CMSCheckBox ID="chkSendOptInConfirmation" runat="server" />
                            </div>
                        </div>
                    </div>
                </asp:PlaceHolder>
            </ContentTemplate>
        </cms:CMSUpdatePanel>
    </asp:Panel>
</asp:Content>
