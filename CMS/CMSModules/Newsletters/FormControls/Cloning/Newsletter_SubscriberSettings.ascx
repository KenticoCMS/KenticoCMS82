<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Newsletter_SubscriberSettings.ascx.cs"
    Inherits="CMSModules_Newsletters_FormControls_Cloning_Newsletter_SubscriberSettings" %>
<div class="form-horizontal">
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel AssociatedControlID="txtEmail" CssClass="control-label" runat="server" ID="lblEmail" ResourceString="general.email" EnableViewState="false"
                DisplayColon="true" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSTextBox runat="server" ID="txtEmail" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel AssociatedControlID="txtFirstName" CssClass="control-label" runat="server" ID="lblFirstName" ResourceString="general.firstname"
                EnableViewState="false" DisplayColon="true" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSTextBox runat="server" ID="txtFirstName" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel AssociatedControlID="txtLastName" CssClass="control-label" runat="server" ID="lblLastName" ResourceString="general.lastname"
                EnableViewState="false" DisplayColon="true" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSTextBox runat="server" ID="txtLastName" />
        </div>
    </div>
</div>