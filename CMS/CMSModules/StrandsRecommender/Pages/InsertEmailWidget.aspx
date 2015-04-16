<%@ Page Language="C#" AutoEventWireup="true" CodeFile="InsertEmailWidget.aspx.cs" Inherits="CMSModules_StrandsRecommender_Pages_InsertEmailWidget"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master" EnableViewState="false" %>

<asp:Content ID="cntHeader" ContentPlaceHolderID="plcBeforeContent" runat="Server">
    <div class="InsertEmailWidget">
        <div class="PageHeaderLine">
            <p>
                <cms:LocalizedLiteral runat="server" ID="litSelectWidget" ResourceString="strands.selectitemmessage" />
                <cms:LocalizedLiteral runat="server" ID="litMoreInformation" ResourceString="strands.moreinformationabout" />
            </p>
        </div>
    </div>
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="plcContent" runat="Server">
    <div class="InsertEmailWidget">
        <div class="body row scroll-y">
            <ul id="EmailTemplatesList" class="EmailTemplatesList">
            </ul>
        </div>
    </div>
</asp:Content>
<asp:Content ID="cntFooter" ContentPlaceHolderID="plcFooter" runat="server">
    <asp:HiddenField ID="hidSelected" runat="server" />
    <cms:LocalizedButton ID="btnInsert" runat="server" ResourceString="dialogs.actions.insert" ClientIDMode="Static"
        ButtonStyle="Primary" CssClass="btn" EnableViewState="false" />
</asp:Content>