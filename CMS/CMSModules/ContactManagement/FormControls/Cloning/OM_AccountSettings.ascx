<%@ Control Language="C#" AutoEventWireup="true" CodeFile="OM_AccountSettings.ascx.cs"
    Inherits="CMSModules_ContactManagement_FormControls_Cloning_OM_AccountSettings" %>

<div class="form-horizontal">
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel AssociatedControlID="chkMerged" CssClass="control-label" runat="server" ID="lblMerged" ResourceString="clonning.settings.account.merged"
                EnableViewState="false" DisplayColon="true" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSCheckBox runat="server" ID="chkMerged" Checked="false" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel AssociatedControlID="chkSubsidiaries" CssClass="control-label" runat="server" ID="lblSubsidiaries" ResourceString="clonning.settings.account.subsidiaries"
                EnableViewState="false" DisplayColon="true" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSCheckBox runat="server" ID="chkSubsidiaries" Checked="false" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel AssociatedControlID="chkContactGroup" CssClass="control-label" runat="server" ID="lblContactGroup" ResourceString="clonning.settings.account.contactgroup"
                EnableViewState="false" DisplayColon="true" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSCheckBox runat="server" ID="chkContactGroup" Checked="false" />
        </div>
    </div>
</div>