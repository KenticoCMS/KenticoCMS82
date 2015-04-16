<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CMS_InlineControlSettings.ascx.cs"
    Inherits="CMSModules_Objects_FormControls_Cloning_CMS_InlineControlSettings" %>

<div class="form-horizontal">
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblFiles" ResourceString="clonning.settings.inlinecontrol.files"
                EnableViewState="false" DisplayColon="true" AssociatedControlID="chkFiles" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSCheckBox runat="server" ID="chkFiles" Checked="true" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblFileName" ResourceString="clonning.settings.inlinecontrol.filename"
                EnableViewState="false" DisplayColon="true" AssociatedControlID="txtFileName" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSTextBox runat="server" ID="txtFileName" />
        </div>
    </div>
</div>