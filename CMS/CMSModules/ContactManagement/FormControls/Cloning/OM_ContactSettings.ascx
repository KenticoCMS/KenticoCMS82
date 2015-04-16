<%@ Control Language="C#" AutoEventWireup="true" CodeFile="OM_ContactSettings.ascx.cs"
    Inherits="CMSModules_ContactManagement_FormControls_Cloning_OM_ContactSettings" %>

<div class="form-horizontal">
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel AssociatedControlID="txtFirstName" CssClass="control-label" runat="server" ID="lblFirstName" ResourceString="om.contact.firstname"
                EnableViewState="false" DisplayColon="true" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSTextBox runat="server" ID="txtFirstName" MaxLength="100" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel AssociatedControlID="txtLastName" CssClass="control-label" runat="server" ID="lblLastName" ResourceString="om.contact.lastname"
                EnableViewState="false" DisplayColon="true" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSTextBox runat="server" ID="txtLastName" MaxLength="100" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel AssociatedControlID="chkMerged" CssClass="control-label" runat="server" ID="lblMerged" ResourceString="clonning.settings.contact.merged"
                EnableViewState="false" DisplayColon="true" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSCheckBox runat="server" ID="chkMerged" Checked="false" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel AssociatedControlID="chkAddressesAgents" CssClass="control-label" runat="server" ID="lblAddressesAgents" ResourceString="clonning.settings.contact.ipaddressesuseragents"
                EnableViewState="false" DisplayColon="true" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSCheckBox runat="server" ID="chkAddressesAgents" Checked="false" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel AssociatedControlID="chkActivity" CssClass="control-label" runat="server" ID="lblActivity" ResourceString="clonning.settings.contact.activity"
                EnableViewState="false" DisplayColon="true" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSCheckBox runat="server" ID="chkActivity" Checked="false" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel AssociatedControlID="chkContactGroup" CssClass="control-label" runat="server" ID="lblContactGroup" ResourceString="clonning.settings.contact.contactgroup"
                EnableViewState="false" DisplayColon="true" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSCheckBox runat="server" ID="chkContactGroup" Checked="false" />
        </div>
    </div>
</div>