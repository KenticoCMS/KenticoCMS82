<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_ProjectManagement_Controls_UI_Projecttaskstatus_Edit" CodeFile="Edit.ascx.cs" %>

<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/System/CodeName.ascx" TagName="CodeName" TagPrefix="cms" %>
    
<cms:MessagesPlaceHolder ID="plcMess" runat="server" />
<div class="form-horizontal">
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblTaskStatusDisplayName" runat="server" EnableViewState="false" DisplayColon="true"
                ResourceString="general.displayname" AssociatedControlID="txtTaskStatusDisplayName" />
        </div>
        <div class="editing-form-value-cell">
            <cms:LocalizableTextBox ID="txtTaskStatusDisplayName" runat="server" MaxLength="200" EnableViewState="false" />
            <cms:CMSRequiredFieldValidator ID="rfvTaskStatusDisplayName" runat="server" Display="Dynamic"
                ControlToValidate="txtTaskStatusDisplayName:cntrlContainer:textbox" ValidationGroup="vgProjecttaskstatus" EnableViewState="false" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblTaskStatusName" runat="server" EnableViewState="false" DisplayColon="true"
                ResourceString="general.codename" AssociatedControlID="txtTaskStatusName" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CodeName ID="txtTaskStatusName" runat="server" MaxLength="200" EnableViewState="false" />
            <cms:CMSRequiredFieldValidator ID="rfvTaskStatusName" runat="server" Display="Dynamic"
                ControlToValidate="txtTaskStatusName" ValidationGroup="vgProjecttaskstatus" EnableViewState="false" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblTaskStatusColor" runat="server" EnableViewState="false" DisplayColon="true"
                ResourceString="pm.projectstatus.edit.color" AssociatedControlID="colorPicker" />
        </div>
        <div class="editing-form-value-cell">
            <cms:ColorPicker ID="colorPicker" runat="server"  />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblTaskStatusIcon" runat="server" EnableViewState="false" DisplayColon="true"
                ResourceString="pm.projectstatus.edit.icon" AssociatedControlID="txtTaskStatusIcon" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSTextBox ID="txtTaskStatusIcon" runat="server" MaxLength="450" EnableViewState="false" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblTaskStatusIsNotStarted" runat="server" EnableViewState="false" DisplayColon="true"
                ResourceString="pm.projectstatus.isnotstartedstatus" AssociatedControlID="chkTaskStatusIsNotStarted" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSCheckBox ID="chkTaskStatusIsNotStarted" runat="server" EnableViewState="false"
                CssClass="CheckBoxMovedLeft" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblTaskStatusIsFinished" runat="server" EnableViewState="false" DisplayColon="true"
                ResourceString="pm.projectstatus.isfinishstatus" AssociatedControlID="chkTaskStatusIsFinished" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSCheckBox ID="chkTaskStatusIsFinished" runat="server" EnableViewState="false"
                CssClass="CheckBoxMovedLeft" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblTaskStatusEnabled" runat="server" EnableViewState="false" DisplayColon="true"
                ResourceString="general.enabled" AssociatedControlID="chkTaskStatusEnabled" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSCheckBox ID="chkTaskStatusEnabled" runat="server" EnableViewState="false" Checked="true"
                CssClass="CheckBoxMovedLeft" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-value-cell editing-form-value-cell-offset">
            <cms:FormSubmitButton runat="server" ID="btnOk" EnableViewState="false"
                OnClick="btnOk_Click" ValidationGroup="vgProjecttaskstatus" />
        </div>
    </div>
</div>