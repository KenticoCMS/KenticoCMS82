<%@ Page Language="C#" AutoEventWireup="true"
    Inherits="CMSFormControls_Selectors_AlternativeFormSelection" ValidateRequest="false"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Title="Alternative form selection" CodeFile="AlternativeFormSelection.aspx.cs" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <asp:Panel runat="server" ID="pnlContent">
        <div class="form-horizontal">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblClass" runat="server" ResourceString="general.class" DisplayColon="true" AssociatedControlID="drpClass" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSDropDownList ID="drpClass"  runat="server" AutoPostBack="True" OnSelectedIndexChanged="drpClass_SelectedIndexChanged"
                        CssClass="DropDownField" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="LocalizedLabel1" runat="server" ResourceString="objecttype.cms_alternativeform" DisplayColon="true" AssociatedControlID="lstAlternativeForms" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSListBox ID="lstAlternativeForms" runat="server" CssClass="DesignerListBox" />
                </div>
            </div>
        </div>
    </asp:Panel>
</asp:Content>
<asp:Content ID="plcFooter" ContentPlaceHolderID="plcFooter" runat="server">
    <div class="FloatRight">
        <cms:LocalizedHidden ID="constNoSelection" ClientIDMode="Static" runat="server" Value="{$altforms_selectaltform.noitemselected$}" />
        <asp:Literal ID="ltlScript" runat="server" />
    </div>

    <script type="text/javascript">
        //<![CDATA[                  
        function SelectCurrentAlternativeForm(txtClientId, lblClientId) {
            if (lstAlternativeForms.selectedIndex != -1) {
                wopener.SelectAltForm(lstAlternativeForms.options[lstAlternativeForms.selectedIndex].value, txtClientId, lblClientId);
                CloseDialog();
            }
            else {
                alert(document.getElementById('constNoSelection').value);
            }
        }

        function Cancel() {
            CloseDialog();
        }
        //]]>
    </script>

</asp:Content>