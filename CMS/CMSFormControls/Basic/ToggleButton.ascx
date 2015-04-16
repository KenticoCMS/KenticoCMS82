<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ToggleButton.ascx.cs"
    Inherits="CMSFormControls_Basic_ToggleButton" %>
<div class="toggle-button">
    <cms:CMSCheckBox ID="checkbox" runat="server" />
    <asp:Label ID="lblError" runat="server" Visible="false" CssClass="ErrorLabel FloatRight" />
    <ajaxToolkit:ToggleButtonExtender ID="exToggle" runat="server" TargetControlID="checkbox" />
</div>
