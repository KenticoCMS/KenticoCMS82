<%@ Page Language="C#" ValidateRequest="false" AutoEventWireup="true"
    Inherits="CMSFormControls_Selectors_GridColumnDesigner" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Theme="Default" Title="Grid column designer" CodeFile="GridColumnDesigner.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/Selectors/ItemSelection.ascx" TagName="ItemSelection"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <asp:HiddenField ID="hdnSelectedColumns" runat="server" />
    <asp:HiddenField ID="hdnClassNames" runat="server" />
    <asp:HiddenField ID="hdnColumns" runat="server" />
    <asp:HiddenField ID="hdnTextClassNames" runat="server" />
    <asp:Panel runat="server" ID="pnlBody">
        <table style="width: 100%;" cellpadding="3" cellspacing="0">
            <tr>
                <td>
                    <cms:CMSRadioButton ID="radGenerate" runat="server" Checked="True" GroupName="GenerateSelect"
                        AutoPostBack="True" />
                </td>
            </tr>
            <tr>
                <td>
                    <cms:CMSRadioButton ID="radSelect" runat="server" GroupName="GenerateSelect" AutoPostBack="True" />
                </td>
            </tr>
        </table>
        <cms:ItemSelection ID="ItemSelection1" runat="server" Visible="false" />
        <asp:Panel ID="pnlProperties" runat="server" Visible="false">
            <br />
            <asp:Label ID="lblProperties" runat="server" Font-Bold="true" />
            <div class="form-horizontal">
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <asp:Label CssClass="control-label" ID="lblHeaderText" runat="server" AssociatedControlID="txtHeaderText" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSTextBox ID="txtHeaderText" runat="server" />
                    </div>
                </div>
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <asp:Label CssClass="control-label" ID="lblDisplayAsLink" runat="server" AssociatedControlID="chkDisplayAsLink" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSCheckBox ID="chkDisplayAsLink" runat="server" />
                    </div>
                </div>
                <div class="form-group">
                    <div class="editing-form-value-cell editing-form-value-cell-offset">
                        <cms:CMSButton ID="btnOk" runat="server" Text="OK" ButtonStyle="Primary" OnClick="btnOK_Click" />
                    </div>
                </div>
            </div>
        </asp:Panel>
    </asp:Panel>
</asp:Content>
<asp:Content ID="plcFooter" ContentPlaceHolderID="plcFooter" runat="server">
    <div class="FloatRight">
        <cms:CMSButton ID="btnClose" runat="server" ButtonStyle="Primary"
            OnClick="btnClose_Click" />
        <asp:Literal ID="ltlOk" runat="server" EnableViewState="false" />
        <asp:Literal ID="ltlLoad" runat="server" EnableViewState="false" />
    </div>
</asp:Content>
