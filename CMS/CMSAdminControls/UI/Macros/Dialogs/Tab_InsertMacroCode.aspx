<%@ Page Language="C#" AutoEventWireup="true" Theme="Default" CodeFile="Tab_InsertMacroCode.aspx.cs"
    Inherits="CMSAdminControls_UI_Macros_Dialogs_Tab_InsertMacroCode" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master" %>

<%@ Register Src="~/CMSAdminControls/UI/Macros/MacroEditor.ascx" TagName="MacroEditor"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:MacroEditor ID="macroEditor" runat="server" MixedMode="false" />
</asp:Content>
<asp:Content ID="cntFooter" ContentPlaceHolderID="plcFooter" runat="server">
    <cms:LocalizedButton ID="btnInsert" runat="server" ButtonStyle="Primary" OnClick="btnInsert_Click"
        EnableViewState="False" ResourceString="dialogs.actions.insert" />
</asp:Content>
