<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSFormControls_Inputs_InternationalPhone" CodeFile="InternationalPhone.ascx.cs" %>

<div class="control-group-inline">
    <asp:Label ID="Label1" runat="server" CssClass="form-control-text" >+</asp:Label>
    <cms:CMSTextBox runat="server" ID="txt1st" MaxLength="4" CssClass="input-width-20" />
    <cms:CMSTextBox runat="server" ID="txt2nd" MaxLength="20" CssClass="input-width-60" />
</div>
