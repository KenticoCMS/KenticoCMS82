<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Newsletters_Tools_Templates_NewsletterTemplate_Edit"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Theme="Default" ValidateRequest="false"
    EnableEventValidation="false" CodeFile="NewsletterTemplate_Edit.aspx.cs" %>

<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Inputs/LargeTextArea.ascx" TagName="LargeTextArea"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/System/CodeName.ascx" TagName="CodeName" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/AdminControls/Controls/MetaFiles/File.ascx" TagName="File"
    TagPrefix="cms" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <div class="form-horizontal">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblTemplateDisplayName" EnableViewState="false"
                    ResourceString="general.displayname" DisplayColon="true" ShowRequiredMark="True" AssociatedControlID="txtTemplateDisplayName" />
            </div>
            <div class="editing-form-value-cell">
                <cms:LocalizableTextBox ID="txtTemplateDisplayName" runat="server"
                    MaxLength="250" />
                <cms:CMSRequiredFieldValidator ID="rfvTemplateDisplayName" runat="server" ControlToValidate="txtTemplateDisplayName:cntrlContainer:textbox"
                    Display="dynamic" EnableViewState="false" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblTemplateName" EnableViewState="false" ResourceString="general.codename"
                    DisplayColon="true" AssociatedControlID="txtTemplateName" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CodeName ID="txtTemplateName" runat="server" MaxLength="250" />
                <cms:CMSRequiredFieldValidator ID="rfvTemplateName" runat="server" ControlToValidate="txtTemplateName"
                    Display="dynamic" EnableViewState="false" />
            </div>
        </div>
        <asp:PlaceHolder runat="server" ID="plcThumb" EnableViewState="false" Visible="false">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblThumb" ResourceString="general.thumbnail"
                        DisplayColon="true" EnableViewState="false" AssociatedControlID="ucThumbnail" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:File ID="ucThumbnail" runat="server" Visible="false" />
                </div>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder runat="server" ID="plcSubject" EnableViewState="false" Visible="false">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblTemplateSubject" ResourceString="general.subject"
                        DisplayColon="true" EnableViewState="false" AssociatedControlID="txtTemplateSubject" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSTextBox ID="txtTemplateSubject" runat="server" MaxLength="250" />
                </div>
            </div>
        </asp:PlaceHolder>
        <div class="form-group">
            <div class="editing-form-label-cell label-full-width">
                <cms:LocalizedLabel CssClass="control-label" ID="lblTemplateHeader" runat="server" Text="Label" ResourceString="general.htmlheader"
                    DisplayColon="true" EnableViewState="false" AssociatedControlID="txtTemplateHeader" />
            </div>
            <div class="editing-form-value-cell textarea-full-width">
                <cms:LargeTextArea ID="txtTemplateHeader" Height="160px" runat="server" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell label-full-width">
                <cms:LocalizedLabel CssClass="control-label" ID="lblTemplateBody" runat="server" ResourceString="general.body"
                    DisplayColon="true" EnableViewState="false" AssociatedControlID="htmlTemplateBody" />
            </div>
            <div class="editing-form-value-cell textarea-full-width">
                <cms:CMSHtmlEditor ID="htmlTemplateBody" runat="server" Width="100%" Height="400px" />
            </div>
        </div>
        <asp:PlaceHolder ID="pnlEditableRegion" runat="server" Visible="false">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblInsertEditableRegion" runat="server" EnableViewState="false"
                        ResourceString="NewsletterTemplate_Edit.TemplateInsertEditRegLabel" AssociatedControlID="txtName" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSTextBox ID="txtName" runat="server" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblWidth" runat="server" EnableViewState="false" ResourceString="NewsletterTemplate_Edit.TemplateEditRegWidthLabel"
                        AssociatedControlID="txtWidth" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSTextBox ID="txtWidth" runat="server" />
                </div>
            </div>

            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblHeight" runat="server" EnableViewState="false" ResourceString="NewsletterTemplate_Edit.TemplateEditRegHeightLabel"
                        AssociatedControlID="txtHeight" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSTextBox ID="txtHeight" runat="server" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-value-cell editing-form-value-cell-offset">
                    <cms:LocalizedButton ID="btnInsertEditableRegion" runat="server" ButtonStyle="Default" 
                        OnClientClick="InsertEditableRegion(); return false;" ResourceString="NewsletterTemplate_Edit.Insert" />
                </div>
            </div>
        </asp:PlaceHolder>
        <div class="form-group">
            <div class="editing-form-label-cell label-full-width">
                <cms:LocalizedLabel CssClass="control-label" ID="lblTemplateFooter" runat="server" ResourceString="general.htmlfooter"
                    DisplayColon="true" EnableViewState="false" AssociatedControlID="txtTemplateFooter" />
            </div>
            <div class="editing-form-value-cell textarea-full-width">
                <cms:LargeTextArea ID="txtTemplateFooter" Height="80px" runat="server" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell label-full-width">
                <cms:LocalizedLabel CssClass="control-label" ID="lblTemplateStyleSheetText" runat="server" EnableViewState="false"
                    ResourceString="NewsletterTemplate_Edit.TemplateStylesheetTextLabel" AssociatedControlID="txtTemplateStyleSheetText" />
            </div>
            <div class="editing-form-value-cell textarea-full-width">
                <cms:ExtendedTextArea ID="txtTemplateStyleSheetText" runat="server" EnableViewState="false"
                    EditorMode="Advanced" Language="CSS" />
            </div>
        </div>
    </div>
</asp:Content>