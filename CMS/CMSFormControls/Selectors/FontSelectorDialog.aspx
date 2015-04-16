<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSFormControls_Selectors_FontSelectorDialog"
    Theme="default" Title="Font Selector" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    CodeFile="~/CMSFormControls/Selectors/FontSelectorDialog.aspx.cs" %>

<asp:Content ID="cntContent" ContentPlaceHolderID="plcContent" runat="Server">
    <div class="font-selector">
        <asp:Label runat="server" ID="lblError" CssClass="ErrorLabel" EnableViewState="false"
            Visible="false" />
        <table>
            <tr>
                <td class="FontSelectorStyleColumnHeader">
                    <%#ResHelper.GetString("fontselector.font")%>
                </td>
                <td class="FontSelectorStyleColumnHeader">
                    <%#ResHelper.GetString("fontselector.style")%>
                </td>
                <td class="FontSelectorStyleColumnHeader">
                    <%#ResHelper.GetString("fontselector.size")%>
                </td>
            </tr>
            <tr>
                <td>
                    <cms:CMSTextBox ID="txtFontType" runat="server" ReadOnly="true" />
                </td>
                <td>
                    <cms:CMSTextBox ID="txtFontStyle" runat="server" ReadOnly="true" />
                </td>
                <td>
                    <cms:CMSTextBox ID="txtFontSize" runat="server" />
                </td>
            </tr>
            <tr>
                <td>
                    <cms:CMSListBox ID="lstFontType" CssClass="FontSelectorTypeListBox" runat="server" Rows="7" />
                </td>
                <td>
                    <cms:CMSListBox ID="lstFontStyle" CssClass="FontSelectorStyleListBox" runat="server" Rows="7" />
                </td>
                <td>
                    <cms:CMSListBox CssClass="FontSelectorStyleListBox" ID="lstFontSize" runat="server" Rows="7" />
                </td>
            </tr>
        </table>
        <div class="boxes">
            <cms:CMSCheckBox ID="chkUnderline" runat="server" />
            <cms:CMSCheckBox ID="chkStrike" runat="server" />
        </div>
        <asp:Panel ID="pnlSampleText" runat="server" class="FontSelectorTextSamplePanel">
            <asp:Label runat="server" ID="lblSampleText" Text="AaBbZzYy" />
        </asp:Panel>
    </div>
</asp:Content>
