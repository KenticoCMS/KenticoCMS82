<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSAdminControls_UI_Macros_MacroEditor" CodeFile="MacroEditor.ascx.cs" %>
<asp:Panel ID="pnlContext" runat="server" EnableViewState="false" Direction="LeftToRight" />
<asp:Panel ID="pnlQuickContext" runat="server" EnableViewState="false" Direction="LeftToRight" />
<asp:Panel ID="pnlHints" runat="server" EnableViewState="false" Direction="LeftToRight">
    <ul>
    </ul>
</asp:Panel>
<cms:ExtendedTextArea runat="server" ID="txtCode" EditorMode="Advanced" Width="500px" Height="200px" Language="CSharp" ShowInsertMacro="true" TextMode="SingleLine" />
