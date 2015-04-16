<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Ecommerce_TaxClassSettings.ascx.cs"
    Inherits="CMSModules_Ecommerce_FormControls_Cloning_Ecommerce_TaxClassSettings" %>

<div class="form-horizontal">
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel AssociatedControlID="chkSKUAssignments" CssClass="control-label" runat="server" ID="lblSKUAssignments" ResourceString="clonning.settings.taxclass.skuassignments"
                EnableViewState="false" DisplayColon="true" />
        </div>
        <div class="editing-form-value-cell">
           <cms:CMSCheckBox runat="server" ID="chkSKUAssignments" Checked="false" />
        </div>
    </div>
</div>