<%@ Control Language="C#" AutoEventWireup="true" CodeFile="EditIssue.ascx.cs" Inherits="CMSModules_Newsletters_Controls_EditIssue" %>

<%@ Register Src="~/CMSModules/Newsletters/Controls/Newsletter_ContentEditor.ascx"
    TagPrefix="cms" TagName="Newsletter_ContentEditor" %>
<%@ Register Src="~/CMSModules/Newsletters/FormControls/NewsletterTemplateSelector.ascx"
    TagPrefix="cms" TagName="NewsletterTemplateSelector" %>

<%-- Issue base properties --%>
<cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <div id="topPanel" class="header-panel" onmouseover="RememberFocusedRegion(); return false;">
            <div class="FloatLeft" style="width:75%">
                <div class="form-horizontal">
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblSubject" runat="server" ResourceString="general.subject"
                                DisplayColon="true" EnableViewState="false" AssociatedControlID="txtSubject" ShowRequiredMark="True" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:CMSTextBox ID="txtSubject" runat="server" MaxLength="450" />
                        </div>
                    </div>
                    <asp:PlaceHolder ID="plcAdvanced" runat="server" Visible="false">
                        <div class="form-group">
                            <div class="editing-form-label-cell">
                                <cms:LocalizedLabel CssClass="control-label" ID="lblSenderName" runat="server" ResourceString="newsletterissue.sender.name"
                                    DisplayColon="true" EnableViewState="false" AssociatedControlID="txtSenderName" />
                            </div>
                            <div class="editing-form-value-cell">
                                <cms:CMSTextBox ID="txtSenderName" runat="server" MaxLength="200" />
                            </div>
                        </div>
                        <div class="form-group">
                            <div class="editing-form-label-cell">
                                <cms:LocalizedLabel CssClass="control-label" ID="lblSenderEmail" runat="server" ResourceString="newsletterissue.sender.email"
                                    DisplayColon="true" EnableViewState="false" AssociatedControlID="txtSenderEmail" />
                            </div>
                            <div class="editing-form-value-cell">
                                <cms:CMSTextBox ID="txtSenderEmail" runat="server" MaxLength="200" />
                            </div>
                        </div>
                        <div class="form-group">
                            <div class="editing-form-label-cell">
                                <cms:LocalizedLabel CssClass="control-label" ID="lblTemplate" runat="server" ResourceString="newsletterissue.template"
                                    DisplayColon="true" EnableViewState="false" AssociatedControlID="issueTemplate" />
                            </div>
                            <div class="editing-form-value-cell">
                                <cms:NewsletterTemplateSelector ID="issueTemplate" runat="server" AutoPostBack="true" />
                            </div>
                        </div>
                        <div class="form-group">
                            <div class="editing-form-label-cell">
                                <cms:LocalizedLabel CssClass="control-label" ID="lblArchive" runat="server" ResourceString="newslettertemplate_edit.showinarchive"
                                    DisplayColon="true" EnableViewState="false" AssociatedControlID="chkShowInArchive" />
                            </div>
                            <div class="editing-form-value-cell">
                                <cms:CMSCheckBox runat="server" ID="chkShowInArchive" />
                            </div>
                        </div>
                    </asp:PlaceHolder>
                </div>
            </div>
            <div class="FloatRight" style="margin:4px">
                <asp:Image runat="server" ID="imgToggleAdvanced" CssClass="NewItemImage" EnableViewState="false" />
                <asp:LinkButton ID="lnkToggleAdvanced" runat="server" OnClick="lnkToggleAdvanced_Click" />
            </div>
            <div class="ClearBoth">
                &nbsp;</div>
            <asp:HiddenField ID="hdnTemplateID" runat="server" EnableViewState="false" />
        </div>
    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="lnkToggleAdvanced" EventName="click" />
    </Triggers>
</cms:CMSUpdatePanel>
<%-- Newletter issue content editor --%>
<cms:CMSUpdatePanel ID="pnlBodyUpdate" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <cms:Newsletter_ContentEditor ID="contentBody" runat="server" ShortID="ce" />
    </ContentTemplate>
    <Triggers>
        <asp:PostBackTrigger ControlID="issueTemplate" />
    </Triggers>
</cms:CMSUpdatePanel>
<asp:HiddenField ID="hdnIssueContent" runat="server" EnableViewState="false" />