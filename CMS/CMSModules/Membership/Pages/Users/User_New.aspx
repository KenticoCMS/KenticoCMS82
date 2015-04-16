<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Membership_Pages_Users_User_New"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Users - New User" CodeFile="User_New.aspx.cs" %>

<%@ Register Src="~/CMSModules/Membership/FormControls/Users/UserName.ascx" TagName="UserName" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Membership/FormControls/Passwords/PasswordStrength.ascx" TagName="PasswordStrength"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/System/EnumSelector.ascx" TagName="PrivilegeLevel"
    TagPrefix="cms" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <div class="form-horizontal">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel AssociatedControlID="ucUserName" CssClass="control-label" ID="lblUserName" runat="server" EnableViewState="false" ResourceString="general.username"
                    DisplayColon="true" ShowRequiredMark="true" />
            </div>
            <div class="editing-form-value-cell">
                <cms:UserName ID="ucUserName" runat="server" UseDefaultValidationGroup="false" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel AssociatedControlID="txtFullName" CssClass="control-label" ID="lblFullName" runat="server" EnableViewState="false" ResourceString="Administration-User_New.FullName" DisplayColon="true" ShowRequiredMark="true" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox ID="txtFullName" runat="server" MaxLength="200" />
                <cms:CMSRequiredFieldValidator ID="RequiredFieldValidatorFullName" runat="server" EnableViewState="false"
                    ControlToValidate="txtFullName" Display="dynamic" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel AssociatedControlID="txtEmailAddress" CssClass="control-label" ID="lblEmail" runat="server" EnableViewState="false" ResourceString="general.email"
                    DisplayColon="true" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox ID="txtEmailAddress" runat="server" MaxLength="100" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel AssociatedControlID="chkEnabled" CssClass="control-label" ID="lblEnabled" runat="server" EnableViewState="false" ResourceString="general.enabled"
                    DisplayColon="true" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSCheckBox ID="chkEnabled" runat="server" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblPrivilegeLevel" ResourceString="user.privilegelevel" runat="server" EnableViewState="false" DisplayColon="true" />
            </div>
            <div class="editing-form-value-cell">
                <cms:PrivilegeLevel runat="server" ID="drpPrivilegeLevel" AssemblyName="CMS.Membership" TypeName="CMS.Membership.UserPrivilegeLevelEnum" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblPassword" ResourceString="Administration-User_New.Password" AssociatedControlID="passStrength"  runat="server" EnableViewState="false" />
            </div>
            <div class="editing-form-value-cell">
                <cms:PasswordStrength runat="server" ID="passStrength" AllowEmpty="true" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblConfirmPassword" ResourceString="Administration-User_New.ConfirmPassword" AssociatedControlID="txtConfirmPassword"  runat="server" EnableViewState="false" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox ID="txtConfirmPassword" runat="server" TextMode="password"
                    MaxLength="100" />
            </div>
        </div>
        <asp:PlaceHolder ID="plcAssignToSite" runat="server" Visible="false">
            <div class="form-group">
                <div class="editing-form-label-cell">
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSCheckBox ID="chkAssignToSite" runat="server" Checked="true" />
                </div>
            </div>
        </asp:PlaceHolder>
        <div class="form-group">
            <div class="editing-form-value-cell editing-form-value-cell-offset">
                <cms:FormSubmitButton ID="btnSave" runat="server" OnClick="btnSave_Click"
                    EnableViewState="false" ResourceString="general.ok" />
            </div>
        </div>
    </div>
</asp:Content>
