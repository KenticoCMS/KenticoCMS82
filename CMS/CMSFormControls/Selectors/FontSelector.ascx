<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSFormControls_Selectors_FontSelector" CodeFile="~/CMSFormControls/Selectors/FontSelector.ascx.cs" %>

<cms:CMSUpdatePanel RenderMode="Block" ID="pnlFontSelector" runat="server">
    <ContentTemplate>
        <div class="control-group-inline">
            <cms:CMSTextBox runat="server" ID="txtFontType" ReadOnly="true" />
            <cms:CMSButton runat="server" ButtonStyle="Default" ID="btnChangeFontType" />
            <cms:CMSButton runat="server" ButtonStyle="Default" ID="btnClearFont"
                OnClick="btnClearFont_Click" />
            <asp:HiddenField runat="server" ID="hfValue" />
        </div>
    </ContentTemplate>
</cms:CMSUpdatePanel>


