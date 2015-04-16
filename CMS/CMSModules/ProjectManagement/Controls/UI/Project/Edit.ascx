<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_ProjectManagement_Controls_UI_Project_Edit" CodeFile="Edit.ascx.cs" %>

<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Membership/FormControls/Users/SelectUser.ascx" TagName="UserSelector" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Content/FormControls/Documents/SelectDocument.ascx" TagName="PageSelector" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/System/CodeName.ascx" TagName="CodeName" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/AdminControls/Controls/MetaFiles/FileList.ascx" TagPrefix="cms" TagName="FileList" %>

<cms:MessagesPlaceHolder ID="plcMess" runat="server" />
<div class="form-horizontal">
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblProjectDisplayName" runat="server" EnableViewState="false"
                DisplayColon="true" ResourceString="general.displayname" AssociatedControlID="txtProjectDisplayName" />
        </div>
        <div class="editing-form-value-cell">
            <cms:LocalizableTextBox ID="txtProjectDisplayName" runat="server" MaxLength="200" EnableViewState="false" />
            <cms:CMSRequiredFieldValidator ID="rfvProjectDisplayName"
                runat="server" Display="Dynamic" ControlToValidate="txtProjectDisplayName:cntrlContainer:textbox" ValidationGroup="vgProject"
                EnableViewState="false" />
        </div>
    </div>
    <asp:PlaceHolder runat="server" ID="plcCodeName">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblProjectName" runat="server" EnableViewState="false" DisplayColon="true"
                    ResourceString="general.codename" AssociatedControlID="txtProjectName" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CodeName ID="txtProjectName" runat="server" MaxLength="200" EnableViewState="false" />
                <cms:CMSRequiredFieldValidator ID="rfvProjectName" runat="server"
                    Display="Dynamic" ControlToValidate="txtProjectName" ValidationGroup="vgProject"
                    EnableViewState="false" />
            </div>
        </div>
    </asp:PlaceHolder>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblProjectDescription" runat="server" EnableViewState="false"
                DisplayColon="true" ResourceString="pm.project.goal" AssociatedControlID="txtProjectDescription" />
        </div>
        <div class="editing-form-value-cell">
            <cms:LocalizableTextBox ID="txtProjectDescription" runat="server" TextMode="MultiLine" EnableViewState="false" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblProjectStartDate" runat="server" EnableViewState="false"
                DisplayColon="true" ResourceString="pm.project.startdate" />
        </div>
        <div class="editing-form-value-cell">
            <cms:DateTimePicker ID="dtpProjectStartDate" runat="server" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblProjectDeadline" runat="server" EnableViewState="false"
                DisplayColon="true" ResourceString="pm.project.deadline" />
        </div>
        <div class="editing-form-value-cell">
            <cms:DateTimePicker ID="dtpProjectDeadline" runat="server" />
        </div>
    </div>
    <asp:PlaceHolder runat="server" ID="plcProgress">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblProjectProgress" runat="server" EnableViewState="false"
                    DisplayColon="true" ResourceString="pm.projecttask.progress" AssociatedControlID="ltrProjectProgress" />
            </div>
            <div class="editing-form-value-cell">
                <span class="form-control-text">
                    <asp:Literal ID="ltrProjectProgress" runat="server" EnableViewState="false" />
                </span>
            </div>
        </div>
    </asp:PlaceHolder>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblProjectOwner" runat="server" EnableViewState="false" DisplayColon="true"
                ResourceString="pm.project.owner" />
        </div>
        <div class="editing-form-value-cell">
            <cms:UserSelector ID="userSelector" runat="server" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblProjectStatusID" runat="server" EnableViewState="false"
                DisplayColon="true" ResourceString="pm.project.status" AssociatedControlID="drpProjectStatus" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSDropDownList ID="drpProjectStatus" CssClass="DropDownField" runat="server" />
        </div>
    </div>
    <asp:PlaceHolder runat="server" ID="plcProjectPage">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblProjectPage" runat="server" EnableViewState="false" DisplayColon="true"
                    ResourceString="pm.project.projectpage" />
            </div>
            <div class="editing-form-value-cell">
                <cms:PageSelector ID="pageSelector" runat="server" />
            </div>
        </div>
    </asp:PlaceHolder>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblProjectAllowOrdering" runat="server" EnableViewState="false"
                DisplayColon="true" ResourceString="pm.project.allowordering" AssociatedControlID="chkProjectAllowOrdering" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSCheckBox ID="chkProjectAllowOrdering" runat="server" EnableViewState="false"
                Checked="true" CssClass="CheckBoxMovedLeft" />
        </div>
    </div>
    <asp:PlaceHolder runat="server" ID="plcAttachments">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblAttachments" runat="server" EnableViewState="false" DisplayColon="true"
                    ResourceString="pm.project.projectattachments" />
            </div>
            <div class="editing-form-value-cell">
                <cms:FileList runat="server" ID="lstAttachments" ShortID="la" />
            </div>
        </div>
    </asp:PlaceHolder>
    <div class="form-group">
        <div class="editing-form-value-cell editing-form-value-cell-offset">
            <cms:FormSubmitButton runat="server" ID="btnOk" EnableViewState="false"
                OnClick="btnOk_Click" ValidationGroup="vgProject" />
        </div>
    </div>
</div>