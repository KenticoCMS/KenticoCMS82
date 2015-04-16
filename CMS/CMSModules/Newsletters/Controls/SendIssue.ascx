<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SendIssue.ascx.cs" Inherits="CMSModules_Newsletters_Controls_SendIssue" %>

<cms:CMSUpdatePanel ID="pnlUpdate" runat="server" ChildrenAsTriggers="true">
    <ContentTemplate>
        <div class="form-horizontal">
            <div class="radio-list-vertical">
                <cms:CMSRadioButton ID="radSendNow" runat="server" GroupName="Send" AutoPostBack="true"
                    OnCheckedChanged="radGroupSend_CheckedChanged" ResourceString="newsletterissue_send.sendnow"
                    Checked="true" />
            </div>
            <asp:PlaceHolder ID="plcSendScheduled" runat="server">
                <cms:CMSRadioButton ID="radSchedule" runat="server" GroupName="Send" AutoPostBack="true"
                    OnCheckedChanged="radGroupSend_CheckedChanged" ResourceString="newsletterissue_send.schedule" />
                <div class="selector-subitem">
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblDateTime" runat="server" ResourceString="newsletterissue_send.datetime"
                                DisplayColon="true" AssociatedControlID="calendarControl" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:DateTimePicker ID="calendarControl" runat="server" Enabled="false" SupportFolder="~/CMSAdminControls/Calendar" />
                        </div>
                    </div>
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plcSendDraft" runat="server">
                <cms:CMSRadioButton ID="radSendDraft" runat="server" GroupName="Send" AutoPostBack="true"
                    OnCheckedChanged="radGroupSend_CheckedChanged" ResourceString="newsletterissue_send.senddraft" />
                <div class="selector-subitem">
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblDraftEmails" runat="server" ResourceString="newsletterissue_send.emails"
                                DisplayColon="true" AssociatedControlID="txtSendDraft" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:CMSTextBox ID="txtSendDraft" runat="server" MaxLength="450" Enabled="false" />
                        </div>
                    </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plcSendLater" runat="server">
                <cms:CMSRadioButton ID="radSendLater" runat="server" GroupName="Send" AutoPostBack="true"
                    OnCheckedChanged="radGroupSend_CheckedChanged" ResourceString="newsletterissue_send.sendlater" />
            </asp:PlaceHolder>
        </div>
    </ContentTemplate>
</cms:CMSUpdatePanel>