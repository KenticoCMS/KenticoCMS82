<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_ProjectManagement_Controls_UI_ProjectTask_Edit"
    CodeFile="Edit.ascx.cs" %>

<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Membership/FormControls/Users/SelectUser.ascx" TagName="UserSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/AdminControls/Controls/MetaFiles/FileList.ascx" TagPrefix="cms" TagName="FileList" %>

<cms:MessagesPlaceHolder ID="plcMess" runat="server" />
<div class="form-horizontal">
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblProjectTaskDisplayName" runat="server" EnableViewState="false"
                DisplayColon="true" ResourceString="general.title" AssociatedControlID="txtProjectTaskDisplayName" ShowRequiredMark="True" />
        </div>
        <div class="editing-form-value-cell">
            <cms:LocalizableTextBox ID="txtProjectTaskDisplayName" runat="server" MaxLength="200" EnableViewState="false" />
            <cms:CMSRequiredFieldValidator ID="rfvProjectTaskDisplayName" runat="server" Display="Dynamic"
                ControlToValidate="txtProjectTaskDisplayName:cntrlContainer:textbox" ValidationGroup="vgProjectTask"
                EnableViewState="false" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblProjectTaskOwnerID" runat="server" EnableViewState="false"
                DisplayColon="true" ResourceString="pm.projecttask.owner" />
        </div>
        <div class="editing-form-value-cell">
            <cms:UserSelector ID="selectorOwner" runat="server" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblProjectTaskProgress" runat="server" EnableViewState="false"
                DisplayColon="true" ResourceString="pm.projecttask.progress" AssociatedControlID="txtProjectTaskProgress" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSTextBox ID="txtProjectTaskProgress" runat="server" MaxLength="0" EnableViewState="false" />
            <asp:Label ID="lblProjectTaskProgressSymbol" runat="server" EnableViewState="false" CssClass="form-control-text" />
            <cms:CMSRegularExpressionValidator ID="regexProgress" runat="server" ControlToValidate="txtProjectTaskProgress" ValidationGroup="vgProjectTask"
                ValidationExpression="[0-9]*" Display="Dynamic"></cms:CMSRegularExpressionValidator>
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblProjectTaskEstimate" runat="server" EnableViewState="false"
                DisplayColon="true" ResourceString="pm.projecttask.estimate" AssociatedControlID="txtProjectTaskHours" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSTextBox ID="txtProjectTaskHours" runat="server" MaxLength="10" EnableViewState="false" />
            <asp:Label ID="lblProjectTaskHours" runat="server" EnableViewState="false" CssClass="form-control-text" />
            <cms:CMSRangeValidator ID="rvHours" runat="server" ControlToValidate="txtProjectTaskHours"
                MaximumValue="9999999999" MinimumValue="0" Type="Double" Display="Dynamic" EnableViewState="false" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblProjectTaskDeadline" runat="server" EnableViewState="false"
                DisplayColon="true" ResourceString="pm.projecttask.deadline" />
        </div>
        <div class="editing-form-value-cell">
            <cms:DateTimePicker ID="dtpProjectTaskDeadline" runat="server" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblProjectTaskStatusID" runat="server" EnableViewState="false"
                DisplayColon="true" ResourceString="pm.projecttask.status" AssociatedControlID="drpTaskStatus" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSDropDownList ID="drpTaskStatus" CssClass="DropDownField" runat="server" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblProjectTaskPriorityID" runat="server" EnableViewState="false"
                DisplayColon="true" ResourceString="pm.projecttask.priority" AssociatedControlID="drpTaskPriority" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSDropDownList ID="drpTaskPriority" CssClass="DropDownField" runat="server" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblProjectTaskIsPrivate" runat="server" EnableViewState="false"
                DisplayColon="true" ResourceString="pm.projecttask.isprivate" AssociatedControlID="chkProjectTaskIsPrivate" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSCheckBox ID="chkProjectTaskIsPrivate" runat="server" EnableViewState="false"
                Checked="false" CssClass="CheckBoxMovedLeft" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblProjectTaskAssignedToUserID" runat="server" EnableViewState="false"
                DisplayColon="true" ResourceString="pm.projecttask.assingedto" />
        </div>
        <div class="editing-form-value-cell">
            <cms:UserSelector ID="selectorAssignee" runat="server" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblProjectTaskDescription" runat="server" EnableViewState="false"
                DisplayColon="true" ResourceString="pm.projecttask.description" AssociatedControlID="htmlTaskDescription" />
        </div>
        <div class="editing-form-value-cell">
            <div style="height: 270px;">
                <cms:CMSHtmlEditor UseValueDirtyBit="true" ID="htmlTaskDescription" runat="server"
                    Width="500px" Height="200px" />
            </div>
        </div>
    </div>
    <asp:PlaceHolder ID="plcTaskUrl" runat="server">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblTaskUrl" runat="server" EnableViewState="false" DisplayColon="true"
                    ResourceString="pm.projecttask.taskurl" AssociatedControlID="txtTaskUrl" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox ReadOnly="true" ID="txtTaskUrl" runat="server" />
            </div>
        </div>
    </asp:PlaceHolder>
    <div class="form-group">
        <div class="editing-form-value-cell editing-form-value-cell-offset">
            <cms:FormSubmitButton runat="server" ID="btnOk" EnableViewState="false"
                OnClick="btnOk_Click" ValidationGroup="vgProjectTask" />
        </div>
    </div>
    <asp:PlaceHolder ID="plcAttachments" runat="server">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblAttachments" runat="server" EnableViewState="false" DisplayColon="true"
                    ResourceString="pm.projecttask.taskattachments" AssociatedControlID="lstAttachments" />
            </div>
            <div class="editing-form-value-cell">
                <cms:FileList runat="server" ID="lstAttachments"  />
            </div>
        </div>
    </asp:PlaceHolder>
</div>