<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSAdminControls_UI_Macros_MacroTreeEditor"
    CodeFile="MacroTreeEditor.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/Macros/MacroEditor.ascx" TagName="MacroEditor"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/Trees/MacroTree.ascx" TagName="MacroTree"
    TagPrefix="cms" %>
<asp:PlaceHolder runat="server" ID="plcControl">
    <div class="macro-tree-editor-form-control keep-white-space-fixed" onmouseout="macroTreeHasFocus = false;" onmouseover="macroTreeHasFocus = true;">
        <asp:Panel ID="pnlTreeWrapper" runat="server">
            <asp:Panel runat="server" ID="pnlMacroTree" CssClass="MacroTreeEditor">
                <cms:MacroTree ID="treeElem" ShortID="t" runat="server" MixedMode="false" MacroExpression="CMSContext.Current" />
            </asp:Panel>
        </asp:Panel>
        <cms:MacroEditor ID="editorElem" runat="server" MixedMode="false" />
        <cms:CMSAccessibleButton runat="server" ID="btnShowTree" CausesValidation="false" CssClass="btn-first"
            IconOnly="True" IconCssClass="icon-braces-octothorpe" EnableViewState="false" />
    </div>
</asp:PlaceHolder>
