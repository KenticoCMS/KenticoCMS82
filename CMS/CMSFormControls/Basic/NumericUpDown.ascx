<%@ Control Language="C#" AutoEventWireup="true" CodeFile="NumericUpDown.ascx.cs" Inherits="CMSFormControls_Basic_NumericUpDown" %>
<asp:Panel ID="pnlContainer" runat="server" CssClass="control-group-inline numeric-up-down">
    <cms:CMSTextBox ID="textbox" runat="server" />
    <div class="numeric-updown-buttons">
        <asp:ImageButton ID="btnImgUp" CssClass="numeric-updown-button-up" runat="server" Visible="false" />
        <asp:ImageButton ID="btnImgDown" CssClass="numeric-updown-button-down" runat="server" Visible="false" />
        <cms:CMSAccessibleButton ID="btnUp" IconOnly="True" IconCssClass="icon-caret-up" CssClass="numeric-updown-button-up" runat="server" Visible="True" UseSubmitBehavior="True"/>
        <cms:CMSAccessibleButton ID="btnDown" IconCssClass="icon-caret-down" IconOnly="True" CssClass="numeric-updown-button-down" runat="server" Visible="True" UseSubmitBehavior="True"/>
    </div>
</asp:Panel>
