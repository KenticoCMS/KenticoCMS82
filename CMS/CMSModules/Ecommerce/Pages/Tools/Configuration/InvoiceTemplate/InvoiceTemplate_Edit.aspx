<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Tools_Configuration_InvoiceTemplate_InvoiceTemplate_Edit"
    Theme="Default" ValidateRequest="false" EnableEventValidation="false" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="E-commerce Configuration - Invoice template" CodeFile="InvoiceTemplate_Edit.aspx.cs" %>

<%@ Register Src="~/CMSModules/AdminControls/Controls/MetaFiles/FileList.ascx" TagName="FileList"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/Macros/MacroSelector.ascx" TagName="MacroSelector"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <div class="content-block">
        <cms:CMSHtmlEditor ID="htmlInvoiceTemplate" runat="server" Height="400px"
            Toolbar="Invoice" />
    </div>
    <cms:LocalizedHeading DisplayColon="true" ResourceString="macroselector.insertmacro" ID="headInsertMacro" runat="server" Level="4" EnableViewState="false" />
    <div class="content-block-50">
        <cms:MacroSelector ID="macroSelectorElm" runat="server" IsLiveSite="false" ShowMacroTreeAbove="true" />
        <div class="explanation-text">
            <a href="#" onclick="document.getElementById('moreMacros').style.display = (document.getElementById('moreMacros').style.display == 'none')? 'block' : 'none';">
                <cms:LocalizedLabel ID="lnkMoreMacros" runat="server" ResourceString="Order_Edit_Invoice.MoreMacros" />
            </a>
        </div>
    </div>
    <div class="content-block">
        <div id="moreMacros" style="display: none;">
            <asp:Table ID="tblMore" runat="server" EnableViewState="false" CellPadding="-1" CellSpacing="-1" CssClass="table table-hover" />
        </div>
    </div>
    <div class="content-block">
        <asp:PlaceHolder ID="plcAttachments" runat="server">
            <cms:LocalizedHeading ID="headAttachmentTitle" runat="server" Level="4" ResourceString="general.attachments" EnableViewState="false" />
            <cms:FileList ID="AttachmentList" runat="server" />
        </asp:PlaceHolder>
    </div>
</asp:Content>
