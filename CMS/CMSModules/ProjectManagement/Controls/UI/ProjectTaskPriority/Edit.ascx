<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_ProjectManagement_Controls_UI_Projecttaskpriority_Edit" CodeFile="Edit.ascx.cs" %>

<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/System/CodeName.ascx" TagName="CodeName" TagPrefix="cms" %>

<cms:MessagesPlaceHolder ID="plcMess" runat="server" />
<div class="form-horizontal">
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblTaskPriorityDisplayName" runat="server" EnableViewState="false" DisplayColon="true"
                ResourceString="general.displayname" AssociatedControlID="txtTaskPriorityDisplayName" />
        </div>
        <div class="editing-form-value-cell">
            <cms:LocalizableTextBox ID="txtTaskPriorityDisplayName" runat="server" MaxLength="200" EnableViewState="false" />
            <cms:CMSRequiredFieldValidator ID="rfvTaskPriorityDisplayName" runat="server" Display="Dynamic"
                ControlToValidate="txtTaskPriorityDisplayName:cntrlContainer:textbox" ValidationGroup="vgProjecttaskpriority" EnableViewState="false" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblTaskPriorityName" runat="server" EnableViewState="false" DisplayColon="true"
                ResourceString="general.codename" AssociatedControlID="txtTaskPriorityName" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CodeName ID="txtTaskPriorityName" runat="server" MaxLength="200" EnableViewState="false" />
            <cms:CMSRequiredFieldValidator ID="rfvTaskPriorityName" runat="server" Display="Dynamic"
                ControlToValidate="txtTaskPriorityName" ValidationGroup="vgProjecttaskpriority" EnableViewState="false" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblTaskPriorityDefault" runat="server" EnableViewState="false" DisplayColon="true"
                ResourceString="general.default" AssociatedControlID="chkTaskPriorityDefault" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSCheckBox ID="chkTaskPriorityDefault" runat="server" EnableViewState="false" Checked="false"
                CssClass="CheckBoxMovedLeft" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblTaskPriorityEnabled" runat="server" EnableViewState="false" DisplayColon="true"
                ResourceString="general.enabled" AssociatedControlID="chkTaskPriorityEnabled" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSCheckBox ID="chkTaskPriorityEnabled" runat="server" EnableViewState="false" Checked="true"
                CssClass="CheckBoxMovedLeft" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-value-cell editing-form-value-cell-offset">
            <cms:FormSubmitButton runat="server" ID="btnOk" EnableViewState="false"
                OnClick="btnOk_Click" ValidationGroup="vgProjecttaskpriority" />
        </div>
    </div>
</div>