<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSAdminControls_UI_Macros_MacroSelector"
    CodeFile="MacroSelector.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/Macros/MacroTreeEditor.ascx" TagName="MacroTreeEditor"
    TagPrefix="cms" %>
<div class="control-group-inline">
    <cms:MacroTreeEditor runat="server" ID="macroElem" />
    <cms:LocalizedButton ID="btnInsert" runat="server" ButtonStyle="Default" ResourceString="macroselector.insert" />
</div>
<asp:Label ID="lblError" runat="server" EnableViewState="false" />
