<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_ProjectManagement_Controls_UI_Projectstatus_Edit" CodeFile="Edit.ascx.cs" %>

<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/System/CodeName.ascx" TagName="CodeName" TagPrefix="cms" %>
    
<cms:MessagesPlaceHolder ID="plcMess" runat="server" />
<div class="form-horizontal">
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblStatusDisplayName" runat="server" EnableViewState="false" DisplayColon="true"
                ResourceString="general.displayname" AssociatedControlID="txtStatusDisplayName" />
        </div>
        <div class="editing-form-value-cell">
            <cms:LocalizableTextBox ID="txtStatusDisplayName" runat="server" MaxLength="200" EnableViewState="false" />
            <cms:CMSRequiredFieldValidator ID="rfvStatusDisplayName" runat="server" Display="Dynamic"
                ControlToValidate="txtStatusDisplayName:cntrlContainer:textbox" ValidationGroup="vgProjectstatus" EnableViewState="false" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblStatusName" runat="server" EnableViewState="false" DisplayColon="true"
                ResourceString="general.codename" AssociatedControlID="txtStatusName" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CodeName ID="txtStatusName" runat="server" MaxLength="200" EnableViewState="false" />
            <cms:CMSRequiredFieldValidator ID="rfvStatusName" runat="server" Display="Dynamic"
                ControlToValidate="txtStatusName" ValidationGroup="vgProjectstatus" EnableViewState="false" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblStatusColor" runat="server" EnableViewState="false" DisplayColon="true"
                ResourceString="pm.projectstatus.edit.color" AssociatedControlID="colorPicker" />
        </div>
        <div class="editing-form-value-cell">
            <cms:ColorPicker ID="colorPicker" runat="server"  />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblStatusIcon" runat="server" EnableViewState="false" DisplayColon="true"
                ResourceString="pm.projectstatus.edit.icon" AssociatedControlID="txtStatusIcon" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSTextBox ID="txtStatusIcon" runat="server" MaxLength="450" EnableViewState="false" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblStatusIsNotStarted" runat="server" EnableViewState="false" DisplayColon="true"
                ResourceString="pm.projectstatus.isnotstartedstatus" AssociatedControlID="chkStatusIsNotStarted" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSCheckBox ID="chkStatusIsNotStarted" runat="server" EnableViewState="false" 
                CssClass="CheckBoxMovedLeft" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblStatusIsFinished" runat="server" EnableViewState="false" DisplayColon="true"
                ResourceString="pm.projectstatus.isfinishstatus" AssociatedControlID="chkStatusIsFinished" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSCheckBox ID="chkStatusIsFinished" runat="server" EnableViewState="false" 
                CssClass="CheckBoxMovedLeft" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblStatusEnabled" runat="server" EnableViewState="false" DisplayColon="true"
                ResourceString="general.enabled" AssociatedControlID="chkStatusEnabled" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSCheckBox ID="chkStatusEnabled" runat="server" EnableViewState="false" Checked="true"
                CssClass="CheckBoxMovedLeft" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-value-cell editing-form-value-cell-offset">
            <cms:FormSubmitButton runat="server" ID="btnOk" EnableViewState="false"
                OnClick="btnOk_Click" ValidationGroup="vgProjectstatus" />
        </div>
    </div>
</div>