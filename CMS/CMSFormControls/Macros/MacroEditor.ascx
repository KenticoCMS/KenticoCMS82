<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MacroEditor.ascx.cs" Inherits="CMSFormControls_Macros_MacroEditor" %>

<%@ Register Src="~/CMSAdminControls/UI/Macros/MacroEditor.ascx" TagPrefix="cms" TagName="MacroEditor" %>
<%@ Register Src="~/CMSAdminControls/UI/Macros/MacroSelector.ascx" TagPrefix="cms" TagName="MacroSelector" %>
<div class="macro-editor-form-control">
    <cms:MacroEditor runat="server" ID="ucEditor" />
    <asp:PlaceHolder runat="server" ID="plcInsertMacro" Visible="False">
        <cms:CMSAccessibleButton runat="server" ID="btnInsertMacro" CausesValidation="false" CssClass="btn-first"
            IconOnly="True" IconCssClass="icon-braces-octothorpe" EnableViewState="false" />
    </asp:PlaceHolder>
</div>
<asp:PlaceHolder runat="server" ID="plcMacroSelector" Visible="False">
    <cms:MacroSelector runat="server" ID="ucMacroSelector" />
</asp:PlaceHolder>
