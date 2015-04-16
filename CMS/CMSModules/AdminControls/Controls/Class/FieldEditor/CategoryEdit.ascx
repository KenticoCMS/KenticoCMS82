<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CategoryEdit.ascx.cs"
    Inherits="CMSModules_AdminControls_Controls_Class_FieldEditor_CategoryEdit" %>
<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox"
    TagPrefix="cms" %>
<%@ Register TagPrefix="cms" TagName="ConditionBuilder" Src="~/CMSFormControls/Macros/ConditionBuilder.ascx" %>
<cms:LocalizedHeading runat="server" Level="4" ResourceString="templatedesigner.section.category"></cms:LocalizedHeading>
<asp:Panel ID="pnlCategory" runat="server" CssClass="FieldPanel">
    <div class="form-horizontal">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblCategoryName" runat="server" EnableViewState="false"
                    ResourceString="TemplateDesigner.CategoryName" />
            </div>
            <div class="editing-form-value-cell">
                <cms:EditingFormControl ID="txtCategoryName" runat="server" AllowMacroEditing="true" FormControlName="LocalizableTextBox" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblCollapsible" runat="server" EnableViewState="false"
                    ResourceString="categories.collapsible" DisplayColon="True" />
            </div>
            <div class="editing-form-value-cell">
                <cms:EditingFormControl runat="server" ID="chkCollapsible" AllowMacroEditing="true" FormControlName="CheckBoxControl" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblCollapsedByDefault" runat="server" EnableViewState="false"
                    ResourceString="categories.collapsedbydefault" DisplayColon="True" />
            </div>
            <div class="editing-form-value-cell">
                <cms:EditingFormControl runat="server" ID="chkCollapsedByDefault" AllowMacroEditing="true" FormControlName="CheckBoxControl" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblVisible" runat="server" EnableViewState="false" ResourceString="formengine.visibilitylabel"
                    DisplayColon="true" />
            </div>
            <div class="editing-form-value-cell">
                <cms:EditingFormControl ID="chkVisible" runat="server" AllowMacroEditing="true" FormControlName="CheckBoxControl" />
            </div>
        </div>
    </div>
</asp:Panel>
