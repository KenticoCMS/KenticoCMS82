<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSWebParts_Layouts_Wizard_ConfirmationCheckbox" CodeFile="~/CMSWebParts/Layouts/Wizard/ConfirmationCheckbox.ascx.cs" %>

<asp:panel runat="server" ID="pnlCheckBox" CssClass="CofirmationCheckbox" Visible="false">
    <cms:CMSCheckBox runat="server" id="chkAccept" visible="true"/> 
</asp:panel>
<asp:panel runat="server" ID="pnlError" CssClass="Error" Visible="false">
    <asp:Label runat="server" ID="lblError" />
</asp:panel>
