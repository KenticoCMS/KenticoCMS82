<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSAdminControls_UI_UniSelector_UniSelector"
    CodeFile="UniSelector.ascx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/UniControls/UniButton.ascx" TagName="UniButton" TagPrefix="cms" %>

<asp:Literal ID="ltlContentBefore" runat="server" EnableViewState="false" />
<asp:Label ID="lblStatus" runat="server" EnableViewState="False" CssClass="form-control-text InfoLabel" />

<asp:PlaceHolder runat="server" ID="plcButtonSelect" Visible="false" EnableViewState="false">
    <cms:LocalizedButton runat="server" ID="btnDialog" ButtonStyle="Default" />
</asp:PlaceHolder>

<asp:PlaceHolder runat="server" ID="plcTextBoxSelect" Visible="false">
    <div class="control-group-inline">
        <cms:CMSTextBox ID="txtSingleSelect" runat="server" ReadOnly="true" CssClass="form-control" />
        <cms:ObjectTransformation ID="objTransform"
            runat="server" Visible="false" EnableViewState="false" />
        <cms:UniButton ID="btnSelect" runat="server" EnableViewState="False" ButtonStyle="default" />
        <cms:UniButton ID="btnEdit" runat="server" Visible="false" EnableViewState="False" />
        <cms:UniButton ID="btnNew" runat="server" Visible="false" EnableViewState="False" />
        <cms:UniButton ID="btnClear" runat="server" EnableViewState="False" />
    </div>
</asp:PlaceHolder>

<asp:PlaceHolder runat="server" ID="plcDropdownSelect" Visible="false">
    <div class="control-group-inline">
        <cms:ExtendedDropDownList SaveItemAttributesToViewState="true" ID="drpSingleSelect"
            runat="server" CssClass="DropDownField" />
        <asp:PlaceHolder runat="server" ID="pnlAutocomplete" Visible="false">
            <cms:CMSTextBox runat="server" ID="txtAutocomplete" CssClass="input-width-100" />
            <i runat="server" id="btnAutocomplete" class="autocomplete-icon icon-ellipsis" />
        </asp:PlaceHolder>
        <cms:UniButton
            runat="server" ID="btnDropEdit" EnableViewState="false" Visible="false"
            RenderScript="true" ImageCssClass="SelectorImageButton" />
        <cms:UniButton ID="btnDropNew" runat="server"
            Visible="false" EnableViewState="False" ImageCssClass="SelectorImageButton" />
    </div>
</asp:PlaceHolder>

<asp:Panel runat="server" ID="pnlGrid" CssClass="UniSelector cms-bootstrap-js" Visible="false">
    <asp:PlaceHolder ID="plcContextMenu" runat="server" EnableViewState="false" />
    <cms:UniGrid ID="uniGrid" ShortID="g" runat="server" ZeroRowsText="" ShowObjectMenu="false"
        ShowActionsMenu="false" RememberState="false" />
    <div id="UniSelectorSpacer" runat="server" class="UniSelectorSpacer">
    </div>
    <div class="btn-actions keep-white-space-fixed">
        <cms:CMSMoreOptionsButton ID="btnRemove" runat="server" ButtonStyle="Default"
            EnableViewState="False" />
        <cms:LocalizedButton ID="btnAddItems" runat="server" ButtonStyle="Default"
            EnableViewState="False" />
        <asp:Button ID="btnRemoveSelected" runat="server" CssClass="HiddenButton" OnClick="btnRemoveSelected_Click" EnableViewState="False" />
        <cms:LocalizedLabel ID="lblDisabledAddButtonExplanationText" ShortID="ldbe" runat="server" CssClass="button-explanation-text" />
    </div>
</asp:Panel>

<asp:Literal ID="ltlContentAfter" runat="server" EnableViewState="false" />
<asp:HiddenField ID="hdnDialogSelect" runat="server" EnableViewState="false" />
<asp:HiddenField ID="hdnIdentifier" runat="server" EnableViewState="false" />
<asp:HiddenField ID="hiddenField" runat="server" />
<asp:HiddenField ID="hiddenSelected" runat="server" EnableViewState="false" />
<asp:HiddenField ID="hdnHash" runat="server" />
<asp:HiddenField ID="hdnValue" runat="server" />