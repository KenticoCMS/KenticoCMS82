<%@ Control Language="C#" AutoEventWireup="true" CodeFile="FieldValidation.ascx.cs" Inherits="CMSModules_AdminControls_Controls_Class_FormBuilder_FieldValidation" %>
<div class="label-property Top">
    <cms:LocalizedLabel ID="lblRules" CssClass="control-label" ResourceString="formbuilder.rules" DisplayColon="true" runat="server" EnableViewState="false" />
</div>
<div class="field-property">
    <cms:FieldMacroRuleDesigner runat="server" ID="ruleDesigner"/>    
</div>