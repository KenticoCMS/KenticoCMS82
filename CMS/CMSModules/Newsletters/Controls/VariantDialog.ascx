<%@ Control Language="C#" AutoEventWireup="true" CodeFile="VariantDialog.ascx.cs"
    Inherits="CMSModules_Newsletters_Controls_VariantDialog" %>

<%@ Register Src="~/CMSAdminControls/UI/PageElements/PageTitle.ascx" TagName="PageTitle"
    TagPrefix="cms" %>

<cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
    <ContentTemplate>
        <cms:ModalPopupDialog ID="mdlVariants" runat="server" BackgroundCssClass="ModalBackground" CssClass="ModalPopupDialog IssueVariant">
            <asp:Panel ID="pnlVariants" runat="server" Visible="false" CssClass="DialogPageBody">
                <div style="width: 550px;">
                    <cms:PageTitle ID="ucTitle" runat="server" EnableViewState="false" DisplayMode="Simple" ShowFullScreenButton="false" ShowCloseButton="false" />
                </div>
                <asp:Panel ID="pnlScrollable" runat="server" CssClass="DialogPageContent">
                    <div>
                        <asp:PlaceHolder ID="plcAddVariant" runat="server">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel CssClass="control-label" ID="lblDisplayName" runat="server" ResourceString="general.name"
                                            DisplayColon="true" ShowRequiredMark="True" EnableViewState="false" AssociatedControlID="txtDisplayName" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSTextBox ID="txtDisplayName" runat="server" CssClass="form-control" />
                                    </div>
                                </div>
                                <div class="form-group">
                                    <div class="editing-form-value-cell-offset editing-form-value-cell">
                                        <div class="radio-list-vertical">
                                            <cms:CMSRadioButton ID="radEmpty" runat="server" ResourceString="newslettervariant.createempty" GroupName="templ" />
                                            <cms:CMSRadioButton ID="radBasedOnTemplate" runat="server" Checked="true" ResourceString="newslettervariant.createcontentfrom" GroupName="templ" />
                                            <div class="selector-subitem">
                                                <cms:CMSListBox ID="lstTemplate" runat="server" Rows="5" EnableViewState="true" CssClass="form-control" />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="plcProperties" runat="server">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel CssClass="control-label" ID="lblPropertyName" runat="server" ResourceString="general.name"
                                            DisplayColon="true" ShowRequiredMark="True" EnableViewState="false" AssociatedControlID="txtPropertyName" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSTextBox ID="txtPropertyName" runat="server" />
                                    </div>
                                </div>
                            </div>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="plcRemoveVariant" runat="server">
                            <cms:LocalizedLabel ID="lblQuestion" runat="server" ResourceString="newslettervariant.deletevariantconfirm"
                                EnableViewState="false" />
                        </asp:PlaceHolder>
                    </div>
                </asp:Panel>
                <div class="dialog-footer control-group-inline">
                    <div class="FloatLeft">
                        <cms:LocalizedLabel ID="lblError" runat="server" CssClass="ErrorLabel" ResourceString="newsletter.variantdisplaynamemissing"
                            EnableViewState="false" Style="display: none;" />
                        <cms:LocalizedLabel ID="lblError2" runat="server" CssClass="ErrorLabel" ResourceString="newsletter.variantselecttemplate"
                            EnableViewState="false" Style="display: none;" />
                    </div>
                        <cms:LocalizedButton ID="btnClose" runat="server" EnableViewState="false" ResourceString="general.cancel"
                            ButtonStyle="Default" OnClick="btnClose_Click" CausesValidation="false" />
                        <cms:LocalizedButton ID="btnOKAdd" runat="server" EnableViewState="false" ResourceString="general.saveandclose"
                            ButtonStyle="Primary" Visible="false" />
                        <cms:LocalizedButton ID="btnOKProperties" runat="server" EnableViewState="false"
                            ResourceString="general.saveandclose" ButtonStyle="Primary" Visible="false" />
                        <cms:LocalizedButton ID="btnOKRemove" runat="server" EnableViewState="false" ResourceString="general.delete"
                            ButtonStyle="Primary" Visible="false" />
                </div>
            </asp:Panel>
        </cms:ModalPopupDialog>
    </ContentTemplate>
</cms:CMSUpdatePanel>
<asp:HiddenField ID="hdnParameter" runat="server" />
<asp:HiddenField ID="hdnSelected" runat="server" />
<asp:Button ID="btnFullPostback" runat="server" CssClass="HiddenButton" />