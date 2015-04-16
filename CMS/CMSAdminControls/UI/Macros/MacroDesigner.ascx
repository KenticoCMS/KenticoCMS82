<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSAdminControls_UI_Macros_MacroDesigner"
    CodeFile="MacroDesigner.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/Macros/MacroEditor.ascx" TagName="MacroEditor"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/Macros/MacroRuleDesigner.ascx" TagName="MacroRuleEditor"
    TagPrefix="cms" %>
<asp:Literal runat="server" ID="ltlScript" EnableViewState="false" />
<cms:UITabs runat="server" ID="tabsElem" />
<asp:Panel runat="server" ID="pnlRuleEditor" CssClass="macro-editor">
    <cms:MacroRuleEditor runat="server" ID="ruleElem" />
</asp:Panel>
<asp:Panel runat="server" ID="pnlEditor" CssClass="MacroDesigner" Visible="false">
    <cms:MacroEditor runat="server" ID="editorElem" UseAutoComplete="true" MixedMode="false"
        Width="100%" Height="520px" />
</asp:Panel>
<asp:Button runat="server" ID="btnShowCode" CssClass="HiddenButton" EnableViewState="false" />
<asp:Button runat="server" ID="btnShowRuleEditor" CssClass="HiddenButton" EnableViewState="false" />
<asp:HiddenField runat="server" ID="hdnSelTab" EnableViewState="false" />
<asp:HiddenField runat="server" ID="hdnCondition" />
