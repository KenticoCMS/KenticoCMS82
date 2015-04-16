<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Support_Pages_SubmitIssue"
    Theme="Default" ValidateRequest="false" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="Support - Submit issue" CodeFile="SubmitIssue.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="plcContent" runat="server">
    <asp:Panel runat="server" ID="pnlContent">
        <div class="form-horizontal">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel AssociatedControlID="txtEmail" CssClass="control-label" runat="server" ID="lblEmail" EnableViewState="false" ResourceString="Support.SubmiIssue.Email" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSTextBox runat="server" ID="txtEmail" />
                    <cms:CMSRequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtEmail" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel AssociatedControlID="txtSubject" CssClass="control-label" runat="server" ID="lblSubject" ResourceString="general.subject" DisplayColon="true" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSTextBox runat="server" ID="txtSubject" />
                    <cms:CMSRequiredFieldValidator ID="rfvSubject" runat="server" ControlToValidate="txtSubject" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel AssociatedControlID="htmlTemplateBody" CssClass="control-label" runat="server" ID="lblHtmlIssue" EnableViewState="false" ResourceString="Support.SubmiIssue.CkIssue" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSHtmlEditor ID="htmlTemplateBody" runat="server" Width="600px" Height="250px" AutoDetectLanguage="false" ToolbarSet="Basic" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel AssociatedControlID="txtSysInfo" CssClass="control-label" runat="server" ID="lblSysInfo" EnableViewState="false" ResourceString="Support.SubmiIssue.SysInfo" /><br />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSTextArea runat="server" ID="txtSysInfo" Rows="7" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel AssociatedControlID="radDontKnow" CssClass="control-label" runat="server" ID="lblTemplate" EnableViewState="false" ResourceString="Support.SubmiIssue.Template" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSRadioButton ID="radDontKnow" runat="server" GroupName="TemplateGroup" ResourceString="Support.SubmiIssue.DontKnow" Checked="true" />
                    <cms:CMSRadioButton ID="radPortal" runat="server" GroupName="TemplateGroup" ResourceString="Support.SubmiIssue.Portal" />
                    <cms:CMSRadioButton ID="radAspx" runat="server" GroupName="TemplateGroup" ResourceString="Support.SubmiIssue.Aspx" />
                    <cms:CMSRadioButton ID="radMix" runat="server" GroupName="TemplateGroup" ResourceString="Support.SubmiIssue.Mix" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel AssociatedControlID="chkSettings" CssClass="control-label" runat="server" ID="lblSettings" EnableViewState="false" ResourceString="Support.SubmiIssue.Settings" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSCheckBox runat="server" ID="chkSettings" Checked="true" /><br />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel AssociatedControlID="fileUpload" CssClass="control-label" runat="server" ID="lblAttachment" EnableViewState="false" ResourceString="Support.SubmiIssue.Attachment" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSFileUpload ID="fileUpload" runat="server" />
                </div>
            </div>
            <div class="form-group" runat="server" ID="pnlButtons">
                <div class="editing-form-value-cell-offset editing-form-value-cell">
                    <cms:LocalizedButton ID="btnSend" runat="server" OnClick="btnSend_Click" ButtonStyle="Primary" EnableViewState="false" ResourceString="Support.SubmiIssue.Send" />
                </div>
            </div>
        </div>
    </asp:Panel>
</asp:Content>
